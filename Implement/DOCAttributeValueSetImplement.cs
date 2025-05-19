using GraphPlugin.Helper;
using GraphPlugin.Interface;
using GraphPlugin.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace GraphPlugin.Implement
{
    public class DOCAttributeValueSetImplement : IDOCAttributeValueSetImplement
    {
        private IPluginExecutionContext _context;
        private IOrganizationService _service;
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                ServiceProviderConfig.ConfigureServices(serviceProvider, out _service, out _context);

                string levelId = _context.InputParameters["nwv_doc_levelId"].ToString();
                string attrValueId = _context.InputParameters["nwv_doc_attrValId"].ToString();
                string nextLevelId = _context.InputParameters["nwv_doc_nextLevelId"].ToString();
                var includeSetIds = (string[])_context.InputParameters["nwv_includeSetIds"];

                var batchCollection = new EntityCollection();
                int eachBatchSize = 500;
                var setIds = new List<Guid>();

                if (includeSetIds.Length > 0)
                {
                    int curBatchSize = eachBatchSize;
                    int curCount = 1;
                    
                    foreach (var set in includeSetIds)
                    {
                        setIds.Add(new Guid(set));
                        
                        if (curCount == curBatchSize || curCount == includeSetIds.Count())
                        {
                            QueryExpression query = new QueryExpression(DataverseEntity.AttributeValueSetDetail.Name)
                            {
                                ColumnSet = new ColumnSet(DataverseEntity.AttributeValueSetDetail.AttributeValueSetName)
                            };
                            query.Criteria.AddCondition(DataverseEntity.AttributeValueSetDetail.AttributeGroupDetailDefinition, ConditionOperator.Equal, new Guid(levelId));
                            query.Criteria.AddCondition(DataverseEntity.AttributeValueSetDetail.AttributeValue, ConditionOperator.Equal, new Guid(attrValueId));
                            query.Criteria.AddCondition(DataverseEntity.AttributeValueSetDetail.AttributeValueSetName, ConditionOperator.In, setIds.Cast<object>().ToArray());
                            
                            EntityCollection collection = _service.RetrieveMultiple(query);
                            batchCollection.Entities.AddRange(collection.Entities.ToList());

                            setIds.Clear();
                            curBatchSize += eachBatchSize;
                        }

                        curCount++;
                    }

                } else
                {
                    QueryExpression query = new QueryExpression(DataverseEntity.AttributeValueSetDetail.Name)
                    {
                        ColumnSet = new ColumnSet(DataverseEntity.AttributeValueSetDetail.AttributeValueSetName)
                    };
                    query.Criteria.AddCondition(DataverseEntity.AttributeValueSetDetail.AttributeGroupDetailDefinition, ConditionOperator.Equal, new Guid(levelId));
                    query.Criteria.AddCondition(DataverseEntity.AttributeValueSetDetail.AttributeValue, ConditionOperator.Equal, new Guid(attrValueId));

                    EntityCollection collection = _service.RetrieveMultiple(query);
                    batchCollection.Entities.AddRange(collection.Entities.ToList());
                }

                var collSets = new List<string>();

                if (!string.IsNullOrEmpty(nextLevelId))
                {
                    var nextCollection = new EntityCollection();

                    int nextBatchSize = eachBatchSize;
                    int nextCount = 1;
                    setIds.Clear();

                    foreach (Entity record in batchCollection.Entities)
                    {
                        EntityReference setRef = (EntityReference)record[DataverseEntity.AttributeValueSetDetail.AttributeValueSetName];
                        setIds.Add(setRef.Id);

                        if (nextCount == nextBatchSize || nextCount == batchCollection.Entities.Count())
                        {
                            QueryExpression query = new QueryExpression(DataverseEntity.AttributeValueSetDetail.Name)
                            {
                                ColumnSet = new ColumnSet(
                                    DataverseEntity.AttributeValueSetDetail.AttributeValue, DataverseEntity.AttributeValueSetDetail.AttributeGroupDetailDefinition
                                )
                            };
                            query.Criteria.AddCondition(DataverseEntity.AttributeValueSetDetail.AttributeGroupDetailDefinition, ConditionOperator.Equal, new Guid(nextLevelId));
                            query.Criteria.AddCondition(DataverseEntity.AttributeValueSetDetail.AttributeValueSetName, ConditionOperator.In, setIds.Cast<object>().ToArray());

                            EntityCollection result = _service.RetrieveMultiple(query);

                            nextCollection.Entities.AddRange(result.Entities.ToList());

                            setIds.Clear();
                            nextBatchSize += eachBatchSize;
                        }

                        nextCount++;

                        //Add collection set
                        collSets.Add(setRef.Id.ToString());
                    }

                    var distinctEntities = nextCollection.Entities
                        .GroupBy(e => e.GetAttributeValue<EntityReference>(DataverseEntity.AttributeValueSetDetail.AttributeValue)?.Id)
                        .Select(g => g.First()).ToList();

                    var nextAttributeCollection = new EntityCollection();
                    nextAttributeCollection.Entities.AddRange(distinctEntities);

                    _context.OutputParameters["nwv_nextAttrValCollection"] = nextAttributeCollection;
                    
                } else
                {
                    foreach (Entity record in batchCollection.Entities)
                    {
                        EntityReference setRef = (EntityReference)record[DataverseEntity.AttributeValueSetDetail.AttributeValueSetName];
                        collSets.Add(setRef.Id.ToString());
                    }
                }

                _context.OutputParameters["nwv_docCollSets"] = collSets.ToArray();
                _context.OutputParameters["nwv_docResults"] = $"nwv_doc_levelId: {levelId} - nwv_doc_attrValId: {attrValueId} - nwv_doc_nextLevelId: {nextLevelId}";
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
            
        }
    }
}

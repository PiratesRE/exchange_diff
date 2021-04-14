using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Fast;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Query
{
	internal abstract class RefinerDataProvider
	{
		internal abstract string Name { get; }

		internal Guid CorrelationId { get; private set; }

		internal static RefinerDataProvider Create(IStorePropertyBag[] results)
		{
			return new RefinerDataProvider.ResultBasedProvider(results);
		}

		internal static RefinerDataProvider Create(ISearchServiceConfig config, PagingImsFlowExecutor flowExecutor, Guid mdbGuid, Guid mbxGuid, string originalQuery, RefinementFilter refinementFilter = null)
		{
			return new RefinerDataProvider.FastProvider(config, flowExecutor, mdbGuid, mbxGuid, originalQuery, refinementFilter);
		}

		internal abstract IReadOnlyCollection<RefinerData> GetRefiners(IReadOnlyCollection<PropertyDefinition> requestedRefiners, int maxResults);

		private sealed class ResultBasedProvider : RefinerDataProvider
		{
			internal ResultBasedProvider(IStorePropertyBag[] results)
			{
				InstantSearch.ThrowOnNullArgument(results, "results");
				this.results = results;
			}

			internal override string Name
			{
				get
				{
					return "ResultBasedProvider";
				}
			}

			internal override IReadOnlyCollection<RefinerData> GetRefiners(IReadOnlyCollection<PropertyDefinition> requestedRefiners, int maxResults)
			{
				base.CorrelationId = Guid.NewGuid();
				if (this.results.Length == 0)
				{
					return RefinerDataProvider.ResultBasedProvider.EmptyRefinerData;
				}
				RefinerDataProvider.RefinerDataCollection refinerDataCollection = new RefinerDataProvider.RefinerDataCollection(requestedRefiners.Count);
				foreach (PropertyDefinition propertyDefinition in requestedRefiners)
				{
					if (!InstantSearchSchema.RefinablePropertiesMap.ContainsKey(propertyDefinition) && !InstantSearchSchema.RefinablePropertiesMap.Values.Contains(propertyDefinition))
					{
						ExAssert.RetailAssert(false, "Unsupported refiner: {0}", new object[]
						{
							propertyDefinition
						});
					}
					RefinerData refinerData = null;
					if (propertyDefinition == ItemSchema.From)
					{
						refinerData = this.CalculatePeopleRefiner(propertyDefinition, "from");
					}
					else if (propertyDefinition == ConversationItemSchema.ConversationMVFrom)
					{
						refinerData = this.CalculateUniqueRefiner<string>(propertyDefinition, (string x) => "(" + x + ")", "from");
					}
					else if (propertyDefinition == ItemSchema.HasAttachment || propertyDefinition == ConversationItemSchema.ConversationHasAttach)
					{
						refinerData = this.CalculateUniqueRefiner<bool>(propertyDefinition, (bool x) => x.ToString(), "hasattachment");
					}
					if (refinerData != null && refinerData.Entries.Count > 0)
					{
						refinerDataCollection.Add(refinerData);
					}
				}
				return refinerDataCollection;
			}

			private static int CompareRefinerDataEntry(RefinerDataEntry x, RefinerDataEntry y)
			{
				return (int)(y.HitCount - x.HitCount);
			}

			private RefinerData CalculatePeopleRefiner(PropertyDefinition prop, string keyword)
			{
				Dictionary<string, long> dictionary = new Dictionary<string, long>(this.results.Length);
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>(this.results.Length);
				foreach (IStorePropertyBag storePropertyBag in this.results)
				{
					Participant participant = storePropertyBag.TryGetProperty(prop) as Participant;
					if (participant != null)
					{
						long num;
						if (dictionary.TryGetValue(participant.EmailAddress, out num))
						{
							dictionary[participant.EmailAddress] = num + 1L;
						}
						else
						{
							dictionary.Add(participant.EmailAddress, 1L);
							dictionary2.Add(participant.EmailAddress, participant.DisplayName);
						}
					}
				}
				List<RefinerDataEntry> list = new List<RefinerDataEntry>(dictionary.Count);
				foreach (KeyValuePair<string, long> keyValuePair in dictionary)
				{
					string key = keyValuePair.Key;
					long value = keyValuePair.Value;
					list.Add(new RefinerDataEntry(key + "|" + dictionary2[key], value, keyword + ":" + key));
				}
				list.Sort(new Comparison<RefinerDataEntry>(RefinerDataProvider.ResultBasedProvider.CompareRefinerDataEntry));
				return new RefinerData(prop, list);
			}

			private RefinerData CalculateUniqueRefiner<T>(PropertyDefinition prop, Func<T, string> converter, string keyword)
			{
				Dictionary<string, long> dictionary = new Dictionary<string, long>(this.results.Length);
				foreach (IStorePropertyBag storePropertyBag in this.results)
				{
					object obj = storePropertyBag.TryGetProperty(prop);
					if (obj != null && !PropertyError.IsPropertyError(obj))
					{
						IEnumerable<T> enumerable = obj as IEnumerable<T>;
						if (enumerable == null)
						{
							enumerable = new List<T>(1)
							{
								(T)((object)obj)
							};
						}
						foreach (T arg in enumerable)
						{
							string key = converter(arg);
							long num;
							if (dictionary.TryGetValue(key, out num))
							{
								dictionary[key] = num + 1L;
							}
							else
							{
								dictionary.Add(key, 1L);
							}
						}
					}
				}
				List<RefinerDataEntry> list = new List<RefinerDataEntry>(dictionary.Count);
				foreach (KeyValuePair<string, long> keyValuePair in dictionary)
				{
					string key2 = keyValuePair.Key;
					long value = keyValuePair.Value;
					list.Add(new RefinerDataEntry(key2, value, keyword + ":" + key2));
				}
				list.Sort(new Comparison<RefinerDataEntry>(RefinerDataProvider.ResultBasedProvider.CompareRefinerDataEntry));
				return new RefinerData(prop, list);
			}

			private const int DefaultBuckets = 4;

			private static readonly RefinerData[] EmptyRefinerData = new RefinerData[0];

			private readonly IStorePropertyBag[] results;
		}

		private sealed class FastProvider : RefinerDataProvider
		{
			internal FastProvider(ISearchServiceConfig config, PagingImsFlowExecutor flowExecutor, Guid mdbGuid, Guid mbxGuid, string originalQuery, RefinementFilter refinementFilter = null)
			{
				InstantSearch.ThrowOnNullArgument(flowExecutor, "flowExecutor");
				InstantSearch.ThrowOnNullOrEmptyArgument(originalQuery, "originalQuery");
				this.config = config;
				this.flowExecutor = flowExecutor;
				this.flowName = FlowDescriptor.GetImsFlowDescriptor(config, FastIndexVersion.GetIndexSystemName(mdbGuid)).DisplayName;
				this.mailboxGuid = mbxGuid;
				this.originalQuery = originalQuery;
				if (refinementFilter != null)
				{
					this.filters = refinementFilter.Filters;
				}
			}

			internal override string Name
			{
				get
				{
					return "FastProvider";
				}
			}

			internal override IReadOnlyCollection<RefinerData> GetRefiners(IReadOnlyCollection<PropertyDefinition> requestedRefiners, int maxResults)
			{
				base.CorrelationId = Guid.NewGuid();
				AdditionalParameters additionalParameters = new AdditionalParameters
				{
					Refiners = RefinerDataProvider.FastProvider.ConvertToFastRefiners(requestedRefiners, maxResults),
					RefinementFilters = this.filters
				};
				if (this.config.UseExecuteAndReadPage)
				{
					QueryParameters queryParameters = new QueryParameters(this.flowExecutor.GetLookupTimeout(), this.flowName, this.originalQuery, this.mailboxGuid, Guid.NewGuid(), additionalParameters);
					IReadOnlyCollection<RefinerResult> refinerResults = this.flowExecutor.ReadRefiners(queryParameters);
					return RefinerDataProvider.FastProvider.ConvertRefinerResults(refinerResults);
				}
				IReadOnlyCollection<RefinerData> result;
				using (PagingImsFlowExecutor.QueryExecutionContext queryExecutionContext = this.flowExecutor.ExecuteRefinerQuery(this.flowName, this.mailboxGuid, base.CorrelationId, this.originalQuery, CultureInfo.InvariantCulture, additionalParameters))
				{
					IReadOnlyCollection<RefinerResult> refinerResults2 = this.flowExecutor.ReadRefiners(queryExecutionContext);
					result = RefinerDataProvider.FastProvider.ConvertRefinerResults(refinerResults2);
				}
				return result;
			}

			private static RefinerDataProvider.RefinerDataCollection ConvertRefinerResults(IReadOnlyCollection<RefinerResult> refinerResults)
			{
				RefinerDataProvider.RefinerDataCollection refinerDataCollection = new RefinerDataProvider.RefinerDataCollection(refinerResults.Count);
				foreach (RefinerResult refinerResult in refinerResults)
				{
					PropertyDefinition propertyDefinition = RefinerDataProvider.FastProvider.ConvertToProperty(refinerResult.Name);
					if (propertyDefinition != null)
					{
						List<RefinerDataEntry> list = new List<RefinerDataEntry>(refinerResult.Entries.Count);
						foreach (RefinerEntry refinerEntry in refinerResult.Entries)
						{
							list.Add(new RefinerDataEntry(refinerEntry.Name, refinerEntry.Count, refinerEntry.Filter));
						}
						refinerDataCollection.Add(new RefinerData(propertyDefinition, list));
					}
				}
				return refinerDataCollection;
			}

			private static List<string> ConvertToFastRefiners(IReadOnlyCollection<PropertyDefinition> requestedRefiners, int maxResults)
			{
				List<string> list = new List<string>(requestedRefiners.Count);
				foreach (PropertyDefinition propertyDefinition in requestedRefiners)
				{
					string text;
					if (!InstantSearchSchema.PropertyToRefinersMap.TryGetValue(propertyDefinition, out text))
					{
						ExAssert.RetailAssert(false, "Unsupported refiner: {0}", new object[]
						{
							propertyDefinition
						});
					}
					if (maxResults > 0)
					{
						object obj = text;
						text = string.Concat(new object[]
						{
							obj,
							"(filter=",
							maxResults,
							")"
						});
					}
					list.Add(text);
				}
				return list;
			}

			private static PropertyDefinition ConvertToProperty(string fastRefiner)
			{
				foreach (KeyValuePair<PropertyDefinition, string> keyValuePair in InstantSearchSchema.PropertyToRefinersMap)
				{
					if (StringComparer.OrdinalIgnoreCase.Equals(fastRefiner, keyValuePair.Value))
					{
						return keyValuePair.Key;
					}
				}
				ExAssert.RetailAssert(false, "Unknown refiner: {0}", new object[]
				{
					fastRefiner
				});
				return null;
			}

			private readonly ISearchServiceConfig config;

			private readonly PagingImsFlowExecutor flowExecutor;

			private readonly string flowName;

			private readonly Guid mailboxGuid;

			private readonly string originalQuery;

			private readonly IReadOnlyCollection<string> filters;
		}

		private class RefinerDataCollection : List<RefinerData>
		{
			public RefinerDataCollection(int capacity) : base(capacity)
			{
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder(25 * base.Count);
				foreach (RefinerData value in this)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(';');
					}
					stringBuilder.Append(value);
				}
				return stringBuilder.ToString();
			}
		}
	}
}

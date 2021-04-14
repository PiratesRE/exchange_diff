using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Common.Reputation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Hygiene.Data.DataProvider;

namespace Microsoft.Exchange.Hygiene.Data.Optics
{
	internal class OpticsSession : IOpticsSession
	{
		public OpticsSession(string callerId = "Unknown")
		{
			this.callerId = callerId;
			this.DataProvider = ConfigDataProviderFactory.CreateDataProvider(DatabaseType.Optics);
		}

		public IConfigDataProvider DataProvider { get; protected set; }

		public IEnumerable<ReputationQueryResult> Query(IEnumerable<ReputationQueryInput> queryInputs)
		{
			if (queryInputs == null || !queryInputs.Any<ReputationQueryInput>())
			{
				return Enumerable.Empty<ReputationQueryResult>();
			}
			if (!this.reputationQueryCache.Initialized)
			{
				this.reputationQueryCache.InitializeCache();
			}
			List<ReputationQueryResult> list = new List<ReputationQueryResult>();
			foreach (ReputationQueryInput reputationQueryInput in queryInputs)
			{
				long value = 0L;
				if (!this.reputationQueryCache.TryGetValue((ReputationEntityType)reputationQueryInput.EntityType, reputationQueryInput.DataPointType, reputationQueryInput.EntityKey, out value))
				{
					break;
				}
				list.Add(new ReputationQueryResult
				{
					EntityType = reputationQueryInput.EntityType,
					DataPointType = reputationQueryInput.DataPointType,
					EntityKey = reputationQueryInput.EntityKey,
					Value = value
				});
			}
			if (queryInputs.Count<ReputationQueryInput>() != list.Count)
			{
				List<QueryFilter> list2 = new List<QueryFilter>();
				list2.Add(new ComparisonFilter(ComparisonOperator.Equal, ReputationQueryResult.QueryInputCountProperty, queryInputs.Count<ReputationQueryInput>()));
				foreach (ReputationQueryInput reputationQueryInput2 in queryInputs)
				{
					list2.Add(new ComparisonFilter(ComparisonOperator.Equal, ReputationQueryResult.EntityTypeProperty, reputationQueryInput2.EntityType));
					list2.Add(new ComparisonFilter(ComparisonOperator.Equal, ReputationQueryResult.EntityKeyProperty, reputationQueryInput2.EntityKey));
					list2.Add(new ComparisonFilter(ComparisonOperator.Equal, ReputationQueryResult.DataPointTypeProperty, reputationQueryInput2.DataPointType));
					list2.Add(new ComparisonFilter(ComparisonOperator.Equal, ReputationQueryResult.FlagsProperty, reputationQueryInput2.Flags));
					list2.Add(new ComparisonFilter(ComparisonOperator.Equal, ReputationQueryResult.UdpTimeoutProperty, reputationQueryInput2.UdpTimeout));
				}
				QueryFilter filter = QueryFilter.AndTogether(list2.ToArray());
				IConfigurable[] array = this.DataProvider.Find<ReputationQueryResult>(filter, null, false, null);
				list = ((array != null) ? array.Cast<ReputationQueryResult>().ToList<ReputationQueryResult>() : new List<ReputationQueryResult>());
				using (List<ReputationQueryResult>.Enumerator enumerator3 = list.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						ReputationQueryResult result = enumerator3.Current;
						if (result.Value != -9223372036854775808L)
						{
							int ttl = queryInputs.FirstOrDefault((ReputationQueryInput input) => input.EntityType == result.EntityType && input.DataPointType == result.DataPointType).Ttl;
							this.reputationQueryCache.TryAddValue((ReputationEntityType)result.EntityType, result.DataPointType, result.EntityKey, result.Value, ttl);
						}
					}
				}
			}
			return list;
		}

		internal void Save(OpticsLogBatch batch)
		{
			this.DataProvider.Save(batch);
		}

		protected const string DefaultCallerId = "Unknown";

		private ReputationQueryCache reputationQueryCache = new ReputationQueryCache();

		protected string callerId = "Unknown";
	}
}

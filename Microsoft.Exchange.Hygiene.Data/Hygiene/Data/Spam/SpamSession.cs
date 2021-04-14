using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Hygiene.Data.DataProvider;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class SpamSession : KesSpamCommon
	{
		public SpamSession(string callerId = "Unknown")
		{
			this.callerId = callerId;
			base.DataProvider = ConfigDataProviderFactory.Default.Create(DatabaseType.Spam);
		}

		internal SpamRulePackageData FindSpamRules(RuleScopeType scopeIDs, out DateTime currentDate)
		{
			currentDate = DateTime.MinValue;
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, RuleBase.RuleTypeProperty, 0),
				new ComparisonFilter(ComparisonOperator.Equal, RuleBase.ScopeIDProperty, (byte)scopeIDs),
				new ComparisonFilter(ComparisonOperator.Equal, KesSpamSchema.ActiveOnlyProperty, true),
				base.BuildVersionParam
			});
			List<SpamRule> list = base.DataProvider.Find<SpamRule>(filter, null, true, null).Cast<SpamRule>().ToList<SpamRule>();
			if (list != null && list.Any<SpamRule>())
			{
				currentDate = (from item in list
				where item.ChangeDatetime != null
				select item.ChangeDatetime.Value).Max<DateTime>();
				return base.GenerateSpamRulePackageData(list);
			}
			return null;
		}

		internal void FindSpamRuleUpdates(RuleScopeType scopeIDs, ref DateTime changedDate, out SpamRulePackageData newRules, out SpamRulePackageData deletedRules, bool activeOnly = false)
		{
			newRules = null;
			deletedRules = null;
			QueryFilter queryFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, RuleBase.RuleTypeProperty, 0),
				new ComparisonFilter(ComparisonOperator.Equal, RuleBase.ScopeIDProperty, (byte)scopeIDs),
				new ComparisonFilter(ComparisonOperator.Equal, KesSpamSchema.ActiveOnlyProperty, activeOnly),
				base.BuildVersionParam
			});
			if (changedDate != DateTime.MinValue)
			{
				DateTime dateTime = changedDate.Subtract(TimeSpan.FromSeconds(30.0));
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					new ComparisonFilter(ComparisonOperator.Equal, RuleBase.ChangedDatetimeProperty, dateTime)
				});
			}
			List<SpamRule> list = base.DataProvider.Find<SpamRule>(queryFilter, null, true, null).Cast<SpamRule>().ToList<SpamRule>();
			if (list != null && list.Count > 0)
			{
				List<SpamRule> list2 = (from item in list
				where item.IsActive.Value
				select item).ToList<SpamRule>();
				if (list2 != null && list2.Any<SpamRule>())
				{
					newRules = base.GenerateSpamRulePackageData(list2);
				}
				List<SpamRule> list3 = (from item in list
				where !item.IsActive.Value
				select item).ToList<SpamRule>();
				if (list3 != null && list3.Count > 0)
				{
					deletedRules = base.GenerateSpamRulePackageData(list3);
				}
				changedDate = (from item in list
				where item.ChangeDatetime != null
				select item.ChangeDatetime.Value).Max<DateTime>();
			}
		}

		internal URIRulePackageData FindURIRules(out DateTime currentDate)
		{
			currentDate = DateTime.MinValue;
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, RuleBase.RuleTypeProperty, 1),
				new ComparisonFilter(ComparisonOperator.Equal, KesSpamSchema.ActiveOnlyProperty, true),
				base.BuildVersionParam
			});
			List<URIRule> list = base.DataProvider.Find<URIRule>(filter, null, true, null).Cast<URIRule>().ToList<URIRule>();
			if (list != null && list.Any<URIRule>())
			{
				currentDate = (from item in list
				where item.ChangeDatetime != null
				select item.ChangeDatetime.Value).Max<DateTime>();
				return base.GenerateURIRulePackageData(list);
			}
			return null;
		}

		internal void FindURIRuleUpdates(ref DateTime changedDate, out URIRulePackageData newRules, out URIRulePackageData deletedRules, bool activeOnly = false)
		{
			newRules = null;
			deletedRules = null;
			QueryFilter queryFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, RuleBase.RuleTypeProperty, 1),
				new ComparisonFilter(ComparisonOperator.Equal, KesSpamSchema.ActiveOnlyProperty, activeOnly),
				base.BuildVersionParam
			});
			if (changedDate != DateTime.MinValue)
			{
				DateTime dateTime = changedDate.Subtract(TimeSpan.FromSeconds(30.0));
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					new ComparisonFilter(ComparisonOperator.Equal, RuleBase.ChangedDatetimeProperty, dateTime)
				});
			}
			List<URIRule> list = base.DataProvider.Find<URIRule>(queryFilter, null, true, null).Cast<URIRule>().ToList<URIRule>();
			if (list != null && list.Count > 0)
			{
				List<URIRule> list2 = (from item in list
				where item.IsActive.Value
				select item).ToList<URIRule>();
				if (list2 != null && list2.Any<URIRule>())
				{
					newRules = base.GenerateURIRulePackageData(list2);
				}
				List<URIRule> list3 = (from item in list
				where !item.IsActive.Value
				select item).ToList<URIRule>();
				if (list3 != null && list3.Count > 0)
				{
					deletedRules = base.GenerateURIRulePackageData(list3);
				}
				changedDate = (from item in list
				where item.ChangeDatetime != null
				select item.ChangeDatetime.Value).Max<DateTime>();
			}
		}

		internal SpamRulePackageData FindSpamRules(RuleScopeType scopeIDs, out string pageCookie)
		{
			pageCookie = null;
			QueryFilter queryFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, RuleBase.RuleTypeProperty, 0),
				new ComparisonFilter(ComparisonOperator.Equal, RuleBase.ScopeIDProperty, (byte)scopeIDs),
				new ComparisonFilter(ComparisonOperator.Equal, KesSpamSchema.ActiveOnlyProperty, true),
				base.BuildVersionParam
			});
			List<SpamRule> list = base.FindPagedRules<SpamRule>(queryFilter, ref pageCookie, 1000);
			if (list.Any<SpamRule>())
			{
				return base.GenerateSpamRulePackageData(list);
			}
			return null;
		}

		internal void FindSpamRuleUpdates(RuleScopeType scopeIDs, ref string pageCookie, out SpamRulePackageData newRules, out SpamRulePackageData deletedRules, bool activeOnly = false)
		{
			newRules = null;
			deletedRules = null;
			QueryFilter queryFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, RuleBase.RuleTypeProperty, 0),
				new ComparisonFilter(ComparisonOperator.Equal, RuleBase.ScopeIDProperty, (byte)scopeIDs),
				new ComparisonFilter(ComparisonOperator.Equal, KesSpamSchema.ActiveOnlyProperty, activeOnly),
				base.BuildVersionParam
			});
			List<SpamRule> list = base.FindPagedRules<SpamRule>(queryFilter, ref pageCookie, 1000);
			if (list != null && list.Count > 0)
			{
				List<SpamRule> list2 = (from item in list
				where item.IsActive.Value
				select item).ToList<SpamRule>();
				if (list2 != null && list2.Any<SpamRule>())
				{
					newRules = base.GenerateSpamRulePackageData(list2);
				}
				List<SpamRule> list3 = (from item in list
				where !item.IsActive.Value
				select item).ToList<SpamRule>();
				if (list3 != null && list3.Count > 0)
				{
					deletedRules = base.GenerateSpamRulePackageData(list3);
				}
			}
		}

		internal URIRulePackageData FindURIRules(out string pageCookie)
		{
			pageCookie = null;
			QueryFilter queryFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, RuleBase.RuleTypeProperty, 1),
				new ComparisonFilter(ComparisonOperator.Equal, KesSpamSchema.ActiveOnlyProperty, true),
				base.BuildVersionParam
			});
			List<URIRule> list = base.FindPagedRules<URIRule>(queryFilter, ref pageCookie, 1000);
			if (list != null && list.Any<URIRule>())
			{
				return base.GenerateURIRulePackageData(list);
			}
			return null;
		}

		internal void FindURIRuleUpdates(ref string pageCookie, out URIRulePackageData newRules, out URIRulePackageData deletedRules, bool activeOnly = false)
		{
			newRules = null;
			deletedRules = null;
			QueryFilter queryFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, RuleBase.RuleTypeProperty, 1),
				new ComparisonFilter(ComparisonOperator.Equal, KesSpamSchema.ActiveOnlyProperty, activeOnly),
				base.BuildVersionParam
			});
			List<URIRule> list = base.FindPagedRules<URIRule>(queryFilter, ref pageCookie, 1000);
			if (list != null && list.Count > 0)
			{
				List<URIRule> list2 = (from item in list
				where item.IsActive.Value
				select item).ToList<URIRule>();
				if (list2.Any<URIRule>())
				{
					newRules = base.GenerateURIRulePackageData(list2);
				}
				List<URIRule> list3 = (from item in list
				where !item.IsActive.Value
				select item).ToList<URIRule>();
				if (list3.Count > 0)
				{
					deletedRules = base.GenerateURIRulePackageData(list3);
				}
			}
		}

		internal IEnumerable<SpamDataBlob> FindUnifiedSpamDataBlobUpdates(SpamDataBlobDataID dataID, byte dataTypeID, int majorVersion, int minorVersion, out bool isSnapshot)
		{
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, SpamDataBlob.DataIDProperty, dataID),
				new ComparisonFilter(ComparisonOperator.Equal, SpamDataBlob.DataTypeIDProperty, dataTypeID),
				new ComparisonFilter(ComparisonOperator.Equal, SpamDataBlob.MajorVersionProperty, majorVersion),
				new ComparisonFilter(ComparisonOperator.Equal, SpamDataBlob.MinorVersionProperty, minorVersion),
				new ComparisonFilter(ComparisonOperator.Equal, KesSpamSchema.IsUnifiedQueryProperty, true)
			});
			isSnapshot = false;
			IEnumerable<SpamDataBlob> enumerable = base.DataProvider.Find<SpamDataBlob>(filter, null, true, null).Cast<SpamDataBlob>();
			foreach (SpamDataBlob spamDataBlob in from blob in enumerable
			where blob.MinorVersion == 0
			select blob)
			{
				isSnapshot = true;
			}
			return enumerable;
		}

		internal SpamRuleBlobPackage FindSpamRulesBlob(RuleScope scopeId, RuleStatusType publishingState, ref string pageCookie)
		{
			List<SpamRuleBlob> list = this.FindPagedSpamRules(scopeId, publishingState, ref pageCookie).ToList<SpamRuleBlob>();
			if (list.Any<SpamRuleBlob>())
			{
				HashSet<string> hashSet = new HashSet<string>();
				foreach (SpamRuleBlob spamRuleBlob in list)
				{
					hashSet.AddRange(SpamRuleBlobUtils.GetProcessorIds(spamRuleBlob.ProcessorData));
				}
				List<SpamRuleProcessorBlob> spamRuleProcessorBlobs = this.ReadSpamRuleProcessorsByProcessorIds<SpamRuleProcessorBlob>(hashSet).ToList<SpamRuleProcessorBlob>();
				return new SpamRuleBlobPackage
				{
					SpamRuleBlobs = list,
					SpamRuleProcessorBlobs = spamRuleProcessorBlobs
				};
			}
			return null;
		}

		internal void Save(SpamRuleBlobPackage spamRuleBlobPackage)
		{
			List<SpamRuleBlob> spamRuleBlobs = spamRuleBlobPackage.SpamRuleBlobs;
			List<SpamRuleProcessorBlob> spamRuleProcessorBlobs = spamRuleBlobPackage.SpamRuleProcessorBlobs;
			if (spamRuleBlobs != null && spamRuleBlobs.Any<SpamRuleBlob>())
			{
				spamRuleBlobs.BatchSplit(1000).AsParallel<IEnumerable<SpamRuleBlob>>().ForAll(delegate(IEnumerable<SpamRuleBlob> batch)
				{
					SpamRuleBlobBatch configurable = new SpamRuleBlobBatch(batch);
					base.Save(configurable);
					base.ApplyAuditProperties(configurable);
				});
			}
			if (spamRuleProcessorBlobs != null && spamRuleProcessorBlobs.Any<SpamRuleProcessorBlob>())
			{
				spamRuleProcessorBlobs.BatchSplit(1000).AsParallel<IEnumerable<SpamRuleProcessorBlob>>().ForAll(delegate(IEnumerable<SpamRuleProcessorBlob> batch)
				{
					SpamRuleProcessorBlobBatch configurable = new SpamRuleProcessorBlobBatch(batch);
					base.Save(configurable);
					base.ApplyAuditProperties(configurable);
				});
			}
		}

		internal IEnumerable<SpamRuleBlob> FindPagedSpamRules(RuleScope scopeId, RuleStatusType publishingState, ref string pageCookie)
		{
			List<SpamRuleBlob> list = new List<SpamRuleBlob>();
			QueryFilter baseQueryFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, SpamRuleBlobSchema.ScopeIdProperty, (byte)scopeId),
				new ComparisonFilter(ComparisonOperator.Equal, SpamRuleBlobSchema.PublishingStateProperty, (byte)publishingState),
				base.BuildVersionParam
			});
			bool flag = false;
			while (!flag)
			{
				QueryFilter pagingQueryFilter = PagingHelper.GetPagingQueryFilter(baseQueryFilter, pageCookie);
				IEnumerable<SpamRuleBlob> collection = base.DataProvider.FindPaged<SpamRuleBlob>(pagingQueryFilter, null, true, null, 1000);
				list.AddRange(collection);
				pageCookie = PagingHelper.GetProcessedCookie(pagingQueryFilter, out flag);
			}
			return list;
		}

		internal IEnumerable<SpamRuleProcessorBlob> FindSpamRuleProcessors(QueryFilter queryFilter)
		{
			string cookie = null;
			List<SpamRuleProcessorBlob> list = new List<SpamRuleProcessorBlob>();
			bool flag = false;
			while (!flag)
			{
				QueryFilter pagingQueryFilter = PagingHelper.GetPagingQueryFilter(queryFilter, cookie);
				IEnumerable<SpamRuleProcessorBlob> collection = base.DataProvider.FindPaged<SpamRuleProcessorBlob>(pagingQueryFilter, null, true, null, 1000);
				list.AddRange(collection);
				cookie = PagingHelper.GetProcessedCookie(pagingQueryFilter, out flag);
			}
			return list;
		}

		internal IEnumerable<T> ReadSpamRuleProcessorsByProcessorIds<T>(IEnumerable<string> processorIds) where T : IConfigurable, new()
		{
			return processorIds.BatchSplit(1000).AsParallel<IEnumerable<string>>().SelectMany(delegate(IEnumerable<string> batch)
			{
				QueryFilter queryFilter = QueryFilter.AndTogether((from item in batch
				select new ComparisonFilter(ComparisonOperator.Equal, SpamRuleProcessorBlobSchema.ProcessorIdProperty, item)).ToArray<ComparisonFilter>());
				return this.FindSpamRuleProcessors(queryFilter).Cast<T>();
			});
		}
	}
}

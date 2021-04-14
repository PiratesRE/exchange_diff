using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory
{
	internal abstract class BudgetCache<T> : ThrottlingCacheBase<BudgetKey, T> where T : Budget
	{
		internal BudgetCache() : base(100000, true, BudgetCache<T>.FiveMinutes, CacheFullBehavior.ExpireExisting)
		{
		}

		internal int CacheEfficiency
		{
			get
			{
				long num = Interlocked.Read(ref this.cacheHits);
				long num2 = Interlocked.Read(ref this.cacheMisses);
				int result;
				try
				{
					double num3 = (double)(num + num2);
					result = ((num3 == 0.0) ? 100 : ((int)((double)num / num3 * 100.0)));
				}
				catch (OverflowException)
				{
					this.cacheHits = 0L;
					this.cacheMisses = 0L;
					result = 100;
				}
				return result;
			}
		}

		internal static QueryFilter ParseWhereClause(string whereClause)
		{
			QueryParser queryParser = new QueryParser(whereClause, BudgetCache<T>.filterSchema, QueryParser.Capabilities.All, null, new QueryParser.ConvertValueFromStringDelegate(BudgetCache<T>.ConvertValueFromString));
			return queryParser.ParseTree;
		}

		internal BudgetCacheHandlerMetadata GetMetadata(string filter)
		{
			QueryFilter queryFilter = null;
			if (!string.IsNullOrEmpty(filter))
			{
				queryFilter = BudgetCache<T>.ParseWhereClause(filter);
			}
			List<T> values = base.Values;
			BudgetCacheHandlerMetadata budgetCacheHandlerMetadata = new BudgetCacheHandlerMetadata();
			budgetCacheHandlerMetadata.TotalCount = base.Count;
			budgetCacheHandlerMetadata.Efficiency = this.CacheEfficiency;
			budgetCacheHandlerMetadata.Budgets = new List<BudgetHandlerMetadata>();
			foreach (T t in values)
			{
				Budget budget = t;
				if (queryFilter != null)
				{
					IReadOnlyPropertyBag readOnlyPropertyBag = budget;
					if (readOnlyPropertyBag != null && !OpathFilterEvaluator.FilterMatches(queryFilter, readOnlyPropertyBag))
					{
						continue;
					}
				}
				if (budget.Owner.IsServiceAccountBudget)
				{
					budgetCacheHandlerMetadata.ServiceAccountBudgets++;
				}
				BudgetHandlerMetadata budgetHandlerMetadata = new BudgetHandlerMetadata();
				budgetHandlerMetadata.Locked = false;
				budgetHandlerMetadata.LockedAt = null;
				budgetHandlerMetadata.LockedUntil = null;
				float balance = budget.CasTokenBucket.GetBalance();
				if (balance >= 0f)
				{
					budgetCacheHandlerMetadata.NotThrottled++;
				}
				else if (budget.CasTokenBucket.Locked)
				{
					budgetHandlerMetadata.Locked = true;
					budgetHandlerMetadata.LockedAt = budget.CasTokenBucket.LockedAt.ToString();
					budgetHandlerMetadata.LockedUntil = budget.CasTokenBucket.LockedUntilUtc.ToString();
					budgetCacheHandlerMetadata.InCutoff++;
				}
				else
				{
					budgetCacheHandlerMetadata.InMicroDelay++;
				}
				budgetHandlerMetadata.Key = budget.Owner.ToString();
				budgetHandlerMetadata.OutstandingActions = budget.OutstandingActionsCount;
				budgetHandlerMetadata.Snapshot = budget.ToString();
				budgetCacheHandlerMetadata.Budgets.Add(budgetHandlerMetadata);
			}
			budgetCacheHandlerMetadata.MatchingCount = budgetCacheHandlerMetadata.Budgets.Count;
			return budgetCacheHandlerMetadata;
		}

		protected override bool HandleShouldRemove(BudgetKey key, T value)
		{
			return value.CanExpire;
		}

		protected override T CreateOnCacheMiss(BudgetKey key, ref bool shouldAdd)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			IThrottlingPolicy throttlingPolicy = null;
			LookupBudgetKey lookupBudgetKey = key as LookupBudgetKey;
			if (lookupBudgetKey != null)
			{
				throttlingPolicy = lookupBudgetKey.Lookup();
			}
			if (throttlingPolicy == null)
			{
				ExTraceGlobals.ClientThrottlingTracer.TraceDebug<string>((long)this.GetHashCode(), "[BudgetCache.CreateOnCacheMiss] Using global policy for account: {0}", key.ToString());
				throttlingPolicy = ThrottlingPolicyCache.Singleton.GetGlobalThrottlingPolicy();
			}
			T result = this.CreateBudget(key, throttlingPolicy);
			ThrottlingPerfCounterWrapper.IncrementBudgetCount();
			Interlocked.Increment(ref this.cacheMisses);
			return result;
		}

		protected override void HandleRemove(BudgetKey key, T value, RemoveReason reason)
		{
			ThrottlingPerfCounterWrapper.DecrementBudgetCount();
			base.HandleRemove(key, value, reason);
			value.Expire();
		}

		protected override void AfterCacheHit(BudgetKey key, T value)
		{
			Interlocked.Increment(ref this.cacheHits);
			value.AfterCacheHit();
			base.AfterCacheHit(key, value);
		}

		protected abstract T CreateBudget(BudgetKey key, IThrottlingPolicy policy);

		private static object ConvertValueFromString(object valueToConvert, Type resultType)
		{
			string text = valueToConvert as string;
			bool flag;
			if (resultType == typeof(bool) && bool.TryParse(text, out flag))
			{
				return flag;
			}
			object result;
			if (resultType.IsEnum && EnumValidator.TryParse(resultType, text, EnumParseOptions.Default, out result))
			{
				return result;
			}
			if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				bool flag2 = text == null || "null".Equals(text, StringComparison.OrdinalIgnoreCase) || "$null".Equals(text, StringComparison.OrdinalIgnoreCase);
				if (flag2)
				{
					return null;
				}
			}
			return LanguagePrimitives.ConvertTo(text, resultType);
		}

		private static readonly ObjectSchema filterSchema = ObjectSchema.GetInstance<BudgetMetadataSchema>();

		private static readonly TimeSpan FiveMinutes = TimeSpan.FromMinutes(5.0);

		private long cacheHits;

		private long cacheMisses;
	}
}

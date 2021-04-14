using System;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.AnchorService
{
	internal class CacheScheduler : CacheProcessorBase
	{
		internal CacheScheduler(AnchorContext context, WaitHandle stopEvent) : base(context, stopEvent)
		{
		}

		internal override string Name
		{
			get
			{
				return "CacheScheduler";
			}
		}

		internal override bool ShouldProcess()
		{
			return true;
		}

		internal override bool Process(JobCache data)
		{
			AnchorUtil.ThrowOnNullArgument(data, "data");
			bool result = false;
			int num = 0;
			int num2 = 0;
			foreach (CacheEntryBase cacheEntryBase in data.Get())
			{
				if (base.StopEvent.WaitOne(0, false))
				{
					break;
				}
				int config = base.Context.Config.GetConfig<int>("MaximumCacheEntrySchedulerRun");
				if (config >= 0 && num++ >= config)
				{
					base.Context.Logger.Log(MigrationEventType.Error, "Skipping run of {0} because too many cache entries {1} expected {2}", new object[]
					{
						cacheEntryBase,
						num,
						config
					});
				}
				else
				{
					AnchorLogContext.Current.OrganizationId = cacheEntryBase.OrganizationId;
					try
					{
						AnchorJobProcessorResult anchorJobProcessorResult = AnchorJobProcessorResult.Working;
						try
						{
							cacheEntryBase.ServiceException = null;
							anchorJobProcessorResult = this.ProcessEntry(cacheEntryBase);
							num2 = 0;
						}
						catch (TransientException ex)
						{
							base.Context.Logger.Log(MigrationEventType.Error, "entry {0} encountered a transient error {1}", new object[]
							{
								cacheEntryBase,
								ex
							});
							anchorJobProcessorResult = AnchorJobProcessorResult.Waiting;
							num2 = 0;
						}
						catch (StoragePermanentException ex2)
						{
							base.Context.Logger.Log(MigrationEventType.Error, "entry {0} encountered a permanent error {1}", new object[]
							{
								cacheEntryBase,
								ex2
							});
							anchorJobProcessorResult = AnchorJobProcessorResult.Waiting;
							num2 = 0;
						}
						catch (MigrationPermanentException ex3)
						{
							base.Context.Logger.Log(MigrationEventType.Error, "entry {0} encountered a permanent error {1}", new object[]
							{
								cacheEntryBase,
								ex3
							});
							anchorJobProcessorResult = AnchorJobProcessorResult.Waiting;
							num2 = 0;
						}
						catch (Exception ex4)
						{
							if (base.Context.Config.GetConfig<bool>("UseWatson"))
							{
								ExWatson.SendReport(ex4);
							}
							cacheEntryBase.ServiceException = ex4;
							anchorJobProcessorResult = AnchorJobProcessorResult.Waiting;
							num2++;
							base.Context.Logger.Log(MigrationEventType.Error, "entry {0} encountered an unhandled error {1}, poison count {2} triggering alert notification", new object[]
							{
								cacheEntryBase,
								ex4,
								num2
							});
							if (!AnchorIssueCache.TrySendEventNotification(base.Context, base.Context.Config.GetConfig<string>("CacheEntryPoisonNotificationReason"), cacheEntryBase.ToString(), ResultSeverityLevel.Error))
							{
								throw;
							}
							if (num2 >= base.Context.Config.GetConfig<int>("CacheEntryPoisonThreshold"))
							{
								throw;
							}
						}
						switch (anchorJobProcessorResult)
						{
						case AnchorJobProcessorResult.Working:
							result = true;
							continue;
						case AnchorJobProcessorResult.Waiting:
							continue;
						case AnchorJobProcessorResult.Failed:
							base.Context.Logger.Log(MigrationEventType.Error, "marking job cache entry failed.. {0}", new object[]
							{
								cacheEntryBase
							});
							data.Remove(cacheEntryBase);
							continue;
						case AnchorJobProcessorResult.Deleted:
							base.Context.Logger.Log(MigrationEventType.Information, "Removing deleted job cache entry {0}", new object[]
							{
								cacheEntryBase
							});
							data.Remove(cacheEntryBase);
							result = true;
							continue;
						}
						throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Invalid AnchorJobProcessorResult {0}", new object[]
						{
							anchorJobProcessorResult
						}));
					}
					finally
					{
						AnchorLogContext.Current.OrganizationId = null;
					}
				}
			}
			return result;
		}

		protected virtual AnchorJobProcessorResult ProcessEntry(CacheEntryBase cacheEntry)
		{
			AnchorUtil.ThrowOnNullArgument(cacheEntry, "cacheEntry");
			AnchorJobProcessorResult anchorJobProcessorResult = this.ShouldProcessEntry(cacheEntry);
			if (anchorJobProcessorResult != AnchorJobProcessorResult.Working)
			{
				return anchorJobProcessorResult;
			}
			return AnchorJobProcessorResult.Waiting;
		}

		protected AnchorJobProcessorResult ShouldProcessEntry(CacheEntryBase cacheEntry)
		{
			if (base.StopEvent.WaitOne(0, false))
			{
				return AnchorJobProcessorResult.Waiting;
			}
			if (!cacheEntry.Validate())
			{
				base.Context.Logger.Log(MigrationEventType.Information, "removing cache entry {0}. not validated", new object[]
				{
					cacheEntry
				});
				return AnchorJobProcessorResult.Deleted;
			}
			return AnchorJobProcessorResult.Working;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Metering;
using Microsoft.Exchange.Data.Metering.Throttling;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Transport
{
	internal class QueueQuotaImpl : IQueueQuotaThresholdFetcher
	{
		public QueueQuotaImpl(IQueueQuotaConfig config, IFlowControlLog log, IQueueQuotaComponentPerformanceCounters perfCounters, IProcessingQuotaComponent processingQuotaComponent, ICountTracker<MeteredEntity, MeteredCount> metering) : this(config, log, perfCounters, processingQuotaComponent, metering, () => DateTime.UtcNow)
		{
		}

		public QueueQuotaImpl(IQueueQuotaConfig config, IFlowControlLog log, IQueueQuotaComponentPerformanceCounters perfCounters, IProcessingQuotaComponent processingQuotaComponent, ICountTracker<MeteredEntity, MeteredCount> metering, Func<DateTime> currentTimeProvider)
		{
			this.config = config;
			this.flowControlLog = log;
			this.flowControlLog.TrackSummary += this.UnthrottleIfNeeded;
			this.perfCounters = perfCounters;
			this.processingQuotaComponent = processingQuotaComponent;
			this.metering = metering;
			this.currentTimeProvider = currentTimeProvider;
			this.totalEntity = new CountedEntity<MeteredEntity>(new SimpleEntityName<MeteredEntity>(MeteredEntity.Total, "All"), new SimpleEntityName<MeteredEntity>(MeteredEntity.Total, "All"));
			this.acceptedCountConfig = AbsoluteCountConfig.Create(false, 0, TimeSpan.Zero, TimeSpan.Zero, false, TimeSpan.FromMinutes(5.0), TimeSpan.Zero);
			this.rejectedCountConfig = AbsoluteCountConfig.Create(false, 0, TimeSpan.Zero, TimeSpan.Zero, false, TimeSpan.FromMinutes(5.0), TimeSpan.Zero);
			this.pastRejectedCountConfig = RollingCountConfig.Create(false, 0, TimeSpan.Zero, TimeSpan.Zero, true, TimeSpan.FromMinutes(5.0), this.config.TrackSummaryLoggingInterval, this.config.TrackSummaryBucketLength);
			this.logger = new QueueQuotaDiagnosticLogger(this.config, this.flowControlLog, this.metering, this.totalEntity, this.perfCounters, this, this.currentTimeProvider);
		}

		public void TrackEnteringQueue(IQueueQuotaMailItem mailItem, QueueQuotaResources resources)
		{
			Guid externalOrganizationId = mailItem.ExternalOrganizationId;
			string originalFromAddress = mailItem.OriginalFromAddress;
			string exoAccountForest = mailItem.ExoAccountForest;
			IEnumerable<MeteredCount> meteredCount = QueueQuotaHelper.GetMeteredCount(resources);
			this.IncrementUsage(this.totalEntity, meteredCount, 1);
			if (this.config.AccountForestEnabled && !string.IsNullOrEmpty(exoAccountForest))
			{
				ICountedEntity<MeteredEntity> entity = QueueQuotaHelper.CreateEntity(exoAccountForest);
				this.IncrementUsage(entity, meteredCount, 1);
				mailItem.QueueQuotaTrackingBits[QueueQuotaEntity.AccountForest, resources] = true;
			}
			ICountedEntity<MeteredEntity> entity2 = QueueQuotaHelper.CreateEntity(externalOrganizationId);
			this.IncrementUsage(entity2, meteredCount, 1);
			mailItem.QueueQuotaTrackingBits[QueueQuotaEntity.Organization, resources] = true;
			if (!string.IsNullOrEmpty(originalFromAddress))
			{
				long usageSum = this.GetUsageSum(entity2, meteredCount);
				if (usageSum > (long)this.config.SenderTrackingThreshold)
				{
					ICountedEntity<MeteredEntity> entity3 = QueueQuotaHelper.CreateEntity(externalOrganizationId, originalFromAddress);
					this.IncrementUsage(entity3, meteredCount, 1);
					mailItem.QueueQuotaTrackingBits[QueueQuotaEntity.Sender, resources] = true;
				}
			}
		}

		public void TrackExitingQueue(IQueueQuotaMailItem mailItem, QueueQuotaResources resources)
		{
			Guid externalOrganizationId = mailItem.ExternalOrganizationId;
			string originalFromAddress = mailItem.OriginalFromAddress;
			string exoAccountForest = mailItem.ExoAccountForest;
			IEnumerable<MeteredCount> meteredCount = QueueQuotaHelper.GetMeteredCount(resources);
			this.IncrementUsage(this.totalEntity, meteredCount, -1);
			if (mailItem.QueueQuotaTrackingBits[QueueQuotaEntity.Organization, resources])
			{
				ICountedEntity<MeteredEntity> entity = QueueQuotaHelper.CreateEntity(externalOrganizationId);
				this.IncrementUsage(entity, meteredCount, -1);
				mailItem.QueueQuotaTrackingBits[QueueQuotaEntity.Organization, resources] = false;
				if (!string.IsNullOrEmpty(originalFromAddress) && mailItem.QueueQuotaTrackingBits[QueueQuotaEntity.Sender, resources])
				{
					ICountedEntity<MeteredEntity> entity2 = QueueQuotaHelper.CreateEntity(externalOrganizationId, originalFromAddress);
					this.IncrementUsage(entity2, meteredCount, -1);
					mailItem.QueueQuotaTrackingBits[QueueQuotaEntity.Sender, resources] = false;
				}
			}
			if (mailItem.QueueQuotaTrackingBits[QueueQuotaEntity.AccountForest, resources])
			{
				ICountedEntity<MeteredEntity> entity3 = QueueQuotaHelper.CreateEntity(exoAccountForest);
				this.IncrementUsage(entity3, meteredCount, -1);
				mailItem.QueueQuotaTrackingBits[QueueQuotaEntity.AccountForest, resources] = false;
			}
		}

		public bool IsOrganizationOverQuota(string accountForest, Guid externalOrganizationId, string sender, out string reason)
		{
			QueueQuotaEntity? entity;
			QueueQuotaResources? queueQuotaResources;
			bool flag = this.IsOverQuota(accountForest, externalOrganizationId, sender, out reason, out entity, out queueQuotaResources);
			if (flag)
			{
				QueueQuotaEntity valueOrDefault = entity.GetValueOrDefault();
				if (entity != null)
				{
					switch (valueOrDefault)
					{
					case QueueQuotaEntity.Organization:
					{
						ICountedEntity<MeteredEntity> entity2 = QueueQuotaHelper.CreateEntity(externalOrganizationId);
						this.metering.AddUsage(entity2, QueueQuotaHelper.GetRejectedMeter(queueQuotaResources.Value), this.rejectedCountConfig, 1L);
						goto IL_F2;
					}
					case QueueQuotaEntity.Sender:
						if (!string.IsNullOrEmpty(sender))
						{
							ICountedEntity<MeteredEntity> entity3 = QueueQuotaHelper.CreateEntity(externalOrganizationId, sender);
							this.metering.AddUsage(entity3, QueueQuotaHelper.GetRejectedMeter(queueQuotaResources.Value), this.rejectedCountConfig, 1L);
							goto IL_F2;
						}
						goto IL_F2;
					case QueueQuotaEntity.AccountForest:
					{
						ICountedEntity<MeteredEntity> entity4 = QueueQuotaHelper.CreateEntity(accountForest);
						this.metering.AddUsage(entity4, QueueQuotaHelper.GetRejectedMeter(queueQuotaResources.Value), this.rejectedCountConfig, 1L);
						goto IL_F2;
					}
					}
				}
				this.metering.AddUsage(this.totalEntity, QueueQuotaHelper.GetRejectedMeter(queueQuotaResources.Value), this.pastRejectedCountConfig, 1L);
			}
			IL_F2:
			if (this.config.EnforceQuota && flag)
			{
				this.perfCounters.IncrementMessagesRejected(entity, new Guid?(externalOrganizationId));
				return true;
			}
			return false;
		}

		public bool IsOrganizationOverWarning(string accountForest, Guid externalOrganizationId, string sender, QueueQuotaResources resource)
		{
			IEnumerable<MeteredCount> meteredCount = QueueQuotaHelper.GetMeteredCount(resource);
			IDictionary<MeteredCount, ICount<MeteredEntity, MeteredCount>> usage;
			if (this.config.AccountForestEnabled && !string.IsNullOrEmpty(accountForest))
			{
				ICountedEntity<MeteredEntity> entity = QueueQuotaHelper.CreateEntity(accountForest);
				usage = this.metering.GetUsage(entity, meteredCount.ToArray<MeteredCount>());
				if (usage.Values.Any(new Func<ICount<MeteredEntity, MeteredCount>, bool>(QueueQuotaHelper.IsWarningLogged)))
				{
					return true;
				}
			}
			ICountedEntity<MeteredEntity> entity2 = QueueQuotaHelper.CreateEntity(externalOrganizationId);
			usage = this.metering.GetUsage(entity2, meteredCount.ToArray<MeteredCount>());
			if (usage.Values.Any(new Func<ICount<MeteredEntity, MeteredCount>, bool>(QueueQuotaHelper.IsWarningLogged)))
			{
				return true;
			}
			if (!string.IsNullOrEmpty(sender))
			{
				ICountedEntity<MeteredEntity> entity3 = QueueQuotaHelper.CreateEntity(externalOrganizationId, sender);
				usage = this.metering.GetUsage(entity3, meteredCount.ToArray<MeteredCount>());
				if (usage.Values.Any(new Func<ICount<MeteredEntity, MeteredCount>, bool>(QueueQuotaHelper.IsWarningLogged)))
				{
					return true;
				}
			}
			return false;
		}

		public void TimedUpdate()
		{
			this.perfCounters.Refresh(new QueueQuotaEntity?(QueueQuotaEntity.Organization));
			this.perfCounters.Refresh(new QueueQuotaEntity?(QueueQuotaEntity.Sender));
			this.perfCounters.Refresh(null);
		}

		internal bool IsOverQuota(string accountForest, Guid externalOrganizationId, string sender, out string reason, out QueueQuotaEntity? reasonEntity, out QueueQuotaResources? reasonResource)
		{
			reason = null;
			reasonEntity = null;
			reasonResource = null;
			foreach (QueueQuotaResources queueQuotaResources in QueueQuotaHelper.AllResources)
			{
				long usageSum = this.GetUsageSum(this.totalEntity, QueueQuotaHelper.GetMeteredCount(queueQuotaResources).ToArray<MeteredCount>());
				int resourceCapacity = this.GetResourceCapacity(queueQuotaResources);
				if (usageSum > (long)resourceCapacity)
				{
					reason = string.Format("Resource {0} beyond capacity. Count:{1} Capacity:{2}", queueQuotaResources, usageSum, resourceCapacity);
					reasonResource = new QueueQuotaResources?(queueQuotaResources);
					return true;
				}
			}
			if (this.config.AccountForestEnabled && !string.IsNullOrEmpty(accountForest))
			{
				ICountedEntity<MeteredEntity> countedEntity = QueueQuotaHelper.CreateEntity(accountForest);
				if (this.CheckQuotaForEntity(countedEntity, QueueQuotaEntity.AccountForest, countedEntity.Name.Value, externalOrganizationId, new Func<QueueQuotaResources, int>(this.GetAccountForestQuotaHighMark), out reason, out reasonResource))
				{
					reasonEntity = new QueueQuotaEntity?(QueueQuotaEntity.AccountForest);
					return true;
				}
			}
			if (this.IsOrganizationBlocked(externalOrganizationId))
			{
				reason = "Organization is in block list.";
				reasonEntity = new QueueQuotaEntity?(QueueQuotaEntity.Organization);
				reasonResource = new QueueQuotaResources?(QueueQuotaResources.SubmissionQueueSize);
				return true;
			}
			if (this.IsOrganizationAllowListed(externalOrganizationId))
			{
				reason = "Organization is in allow list.";
				return false;
			}
			ICountedEntity<MeteredEntity> entity = QueueQuotaHelper.CreateEntity(externalOrganizationId);
			if (this.CheckQuotaForEntity(entity, QueueQuotaEntity.Organization, externalOrganizationId.ToString(), externalOrganizationId, (QueueQuotaResources r) => ((IQueueQuotaThresholdFetcher)this).GetOrganizationQuotaHighMark(externalOrganizationId, r), out reason, out reasonResource))
			{
				reasonEntity = new QueueQuotaEntity?(QueueQuotaEntity.Organization);
				return true;
			}
			if (!string.IsNullOrEmpty(sender))
			{
				ICountedEntity<MeteredEntity> countedEntity2 = QueueQuotaHelper.CreateEntity(externalOrganizationId, sender);
				if (this.CheckQuotaForEntity(countedEntity2, QueueQuotaEntity.Sender, QueueQuotaHelper.GetRedactedSender(QueueQuotaEntity.Sender, countedEntity2.Name.Value), externalOrganizationId, (QueueQuotaResources r) => ((IQueueQuotaThresholdFetcher)this).GetSenderQuotaHighMark(externalOrganizationId, sender, r), out reason, out reasonResource))
				{
					reasonEntity = new QueueQuotaEntity?(QueueQuotaEntity.Sender);
					return true;
				}
			}
			return false;
		}

		internal XElement GetDiagnosticInfo()
		{
			return this.logger.GetDiagnosticInfo();
		}

		internal XElement GetDiagnosticInfo(Guid externalOrganizationId)
		{
			return this.logger.GetDiagnosticInfo(externalOrganizationId);
		}

		internal XElement GetDiagnosticInfo(string accountForest)
		{
			return this.logger.GetDiagnosticInfo(accountForest);
		}

		private bool CheckQuotaForEntity(ICountedEntity<MeteredEntity> entity, QueueQuotaEntity entityType, string entityValue, Guid externalOrganizationId, Func<QueueQuotaResources, int> getHighMark, out string reason, out QueueQuotaResources? reasonResource)
		{
			IDictionary<MeteredCount, ICount<MeteredEntity, MeteredCount>> usage = this.metering.GetUsage(entity, QueueQuotaHelper.AllAcceptedCounts);
			foreach (ICount<MeteredEntity, MeteredCount> count in usage.Values)
			{
				bool wasAnyThrottled = QueueQuotaHelper.IsAnyThrottled(usage);
				QueueQuotaResources resource = QueueQuotaHelper.GetResource(count.Measure);
				if (this.ComputeIsOverQuota(externalOrganizationId, entityType, entityValue, count, resource, getHighMark(resource), false, wasAnyThrottled, out reason))
				{
					reasonResource = new QueueQuotaResources?(resource);
					return true;
				}
			}
			reason = null;
			reasonResource = null;
			return false;
		}

		private bool ComputeIsOverQuota(Guid externalOrganizationId, QueueQuotaEntity entityType, string entityId, ICount<MeteredEntity, MeteredCount> count, QueueQuotaResources resource, int high, bool onlySetUnthrottle, bool wasAnyThrottled, out string reason)
		{
			reason = null;
			long total = count.Total;
			int num = (int)((double)high * this.config.LowWatermarkRatio);
			int num2 = (int)((double)high * this.config.WarningRatio);
			DateTime value;
			bool flag = QueueQuotaHelper.HasThrottledTime(count, out value);
			bool flag2 = QueueQuotaHelper.IsWarningLogged(count);
			bool flag3 = total > (long)high || (flag && total > (long)num);
			bool flag4 = total > (long)num2 && !flag3;
			if (flag3)
			{
				if (onlySetUnthrottle)
				{
					return true;
				}
				reason = string.Format("{0} is above quota for {1}.Actual:{2} Low:{3} High:{4}", new object[]
				{
					entityType,
					resource,
					total,
					num,
					high
				});
				if (!flag)
				{
					count.SetObject("ThrottledStartTime", this.currentTimeProvider());
					count.SetObject("WarningLogged", false);
					this.flowControlLog.LogThrottle(QueueQuotaHelper.GetThrottlingResource(resource), ThrottlingAction.TempReject, high, TimeSpan.Zero, QueueQuotaHelper.GetThrottlingScope(entityType), QueueQuotaHelper.GetOrgId(entityType, externalOrganizationId), QueueQuotaHelper.GetSender(entityType, entityId), null, null, ThrottlingSource.QueueQuota, !this.config.EnforceQuota, new List<KeyValuePair<string, object>>
					{
						new KeyValuePair<string, object>("AvailableCapacity", ((IQueueQuotaThresholdFetcher)this).GetAvailableResourceSize(resource)),
						new KeyValuePair<string, object>("observedValue", (int)total),
						new KeyValuePair<string, object>("ScopeValue", entityId)
					});
					if (!wasAnyThrottled)
					{
						this.perfCounters.IncrementThrottledEntities(entityType, externalOrganizationId);
					}
				}
			}
			else if (flag)
			{
				MeteredCount rejectedMeter = QueueQuotaHelper.GetRejectedMeter(count.Measure);
				ICount<MeteredEntity, MeteredCount> usage = this.metering.GetUsage(count.Entity, rejectedMeter);
				count.SetObject("ThrottledStartTime", null);
				this.flowControlLog.LogUnthrottle(QueueQuotaHelper.GetThrottlingResource(resource), ThrottlingAction.TempReject, num, TimeSpan.Zero, (int)usage.Total, (int)total, QueueQuotaHelper.GetThrottlingScope(entityType), QueueQuotaHelper.GetOrgId(entityType, externalOrganizationId), QueueQuotaHelper.GetSender(entityType, entityId), null, null, ThrottlingSource.QueueQuota, !this.config.EnforceQuota, new List<KeyValuePair<string, object>>
				{
					new KeyValuePair<string, object>("AvailableCapacity", ((IQueueQuotaThresholdFetcher)this).GetAvailableResourceSize(resource)),
					new KeyValuePair<string, object>("throttledDuration", this.currentTimeProvider().Subtract(value).ToString("d\\.hh\\:mm\\:ss")),
					new KeyValuePair<string, object>("ScopeValue", entityId)
				});
				this.metering.AddUsage(count.Entity, QueueQuotaHelper.GetRejectedMeter(rejectedMeter), this.pastRejectedCountConfig, (long)((int)usage.Total));
				this.metering.TrySetUsage(count.Entity, rejectedMeter, 0L);
				if (!QueueQuotaHelper.IsAnyThrottled(this.metering.GetUsage(count.Entity, QueueQuotaHelper.AllAcceptedCounts)))
				{
					this.perfCounters.DecrementThrottledEntities(entityType, externalOrganizationId);
				}
			}
			else if (!onlySetUnthrottle && flag4 && !flag2)
			{
				count.SetObject("WarningLogged", true);
				this.flowControlLog.LogWarning(QueueQuotaHelper.GetThrottlingResource(resource), ThrottlingAction.TempReject, num2, TimeSpan.Zero, QueueQuotaHelper.GetThrottlingScope(entityType), QueueQuotaHelper.GetOrgId(entityType, externalOrganizationId), QueueQuotaHelper.GetSender(entityType, entityId), null, null, ThrottlingSource.QueueQuota, !this.config.EnforceQuota, new List<KeyValuePair<string, object>>
				{
					new KeyValuePair<string, object>("AvailableCapacity", ((IQueueQuotaThresholdFetcher)this).GetAvailableResourceSize(resource)),
					new KeyValuePair<string, object>("observedValue", (int)total),
					new KeyValuePair<string, object>("ScopeValue", entityId)
				});
			}
			else if (flag2 && !flag4)
			{
				count.SetObject("WarningLogged", false);
			}
			return flag3;
		}

		private void UnthrottleIfNeeded(string sequenceNumber)
		{
			IEnumerable<ICount<MeteredEntity, MeteredCount>> enumerable = this.metering.Filter((ICount<MeteredEntity, MeteredCount> count) => QueueQuotaHelper.IsQueueQuotaAcceptedCount(count) && (this.IsOrgLessThanLow(count) || this.IsSenderLessThanLow(count) || this.IsForestLessThanLow(count)));
			foreach (ICount<MeteredEntity, MeteredCount> count2 in enumerable)
			{
				if (QueueQuotaHelper.ShouldProcessEntity(count2.Entity))
				{
					QueueQuotaResources resource = QueueQuotaHelper.GetResource(count2.Measure);
					QueueQuotaLoggingContext queueQuotaLoggingContext = new QueueQuotaLoggingContext(count2.Entity, resource, this);
					bool wasAnyThrottled = QueueQuotaHelper.IsAnyThrottled(this.metering.GetUsage(count2.Entity, QueueQuotaHelper.AllAcceptedCounts));
					string text;
					this.ComputeIsOverQuota(queueQuotaLoggingContext.OrgId, QueueQuotaHelper.GetQueueQuotaEntity(count2.Entity.Name.Type), queueQuotaLoggingContext.ScopeValue, count2, resource, queueQuotaLoggingContext.HighThreshold, true, wasAnyThrottled, out text);
				}
			}
		}

		private void IncrementUsage(ICountedEntity<MeteredEntity> entity, IEnumerable<MeteredCount> meteredCounts, int increment)
		{
			foreach (MeteredCount measure in meteredCounts)
			{
				this.metering.AddUsage(entity, measure, this.acceptedCountConfig, (long)increment);
			}
		}

		private long GetUsageSum(ICountedEntity<MeteredEntity> entity, IEnumerable<MeteredCount> meteredCounts)
		{
			IDictionary<MeteredCount, ICount<MeteredEntity, MeteredCount>> usage = this.metering.GetUsage(entity, meteredCounts.ToArray<MeteredCount>());
			return QueueQuotaHelper.GetSum(usage.Values);
		}

		private bool IsOrganizationBlocked(Guid organizationId)
		{
			if (this.processingQuotaComponent != null)
			{
				ProcessingQuotaComponent.ProcessingData quotaOverride = this.processingQuotaComponent.GetQuotaOverride(organizationId);
				if (quotaOverride != null)
				{
					return quotaOverride.IsBlocked;
				}
			}
			return false;
		}

		private bool IsOrganizationAllowListed(Guid organizationId)
		{
			if (this.processingQuotaComponent != null)
			{
				ProcessingQuotaComponent.ProcessingData quotaOverride = this.processingQuotaComponent.GetQuotaOverride(organizationId);
				if (quotaOverride != null)
				{
					return quotaOverride.IsAllowListed;
				}
			}
			return false;
		}

		int IQueueQuotaThresholdFetcher.GetAvailableResourceSize(QueueQuotaResources resource)
		{
			return Math.Max(this.GetResourceCapacity(resource) - (int)this.GetUsageSum(this.totalEntity, QueueQuotaHelper.GetMeteredCount(resource).ToArray<MeteredCount>()), 0);
		}

		private bool IsOrgLessThanLow(ICount<MeteredEntity, MeteredCount> count)
		{
			double num = Math.Max(this.config.LowWatermarkRatio, this.config.WarningRatio);
			Guid organizationId;
			return QueueQuotaHelper.IsOrg(count.Entity, out organizationId) && (double)count.Total < (double)((IQueueQuotaThresholdFetcher)this).GetOrganizationQuotaHighMark(organizationId, QueueQuotaHelper.GetResource(count.Measure)) * num;
		}

		private bool IsSenderLessThanLow(ICount<MeteredEntity, MeteredCount> count)
		{
			double num = Math.Max(this.config.LowWatermarkRatio, this.config.WarningRatio);
			Guid organizationId;
			return QueueQuotaHelper.IsSender(count.Entity, out organizationId) && (double)count.Total < (double)((IQueueQuotaThresholdFetcher)this).GetSenderQuotaHighMark(organizationId, count.Entity.Name.Value, QueueQuotaHelper.GetResource(count.Measure)) * num;
		}

		private bool IsForestLessThanLow(ICount<MeteredEntity, MeteredCount> count)
		{
			double num = Math.Max(this.config.LowWatermarkRatio, this.config.WarningRatio);
			return QueueQuotaHelper.IsAccountForest(count.Entity) && (double)count.Total < (double)((IQueueQuotaThresholdFetcher)this).GetAccountForestQuotaHighMark(QueueQuotaHelper.GetResource(count.Measure)) * num;
		}

		int IQueueQuotaThresholdFetcher.GetOrganizationQuotaHighMark(Guid organizationId, QueueQuotaResources resource)
		{
			int availableResourceSize = ((IQueueQuotaThresholdFetcher)this).GetAvailableResourceSize(resource);
			if (organizationId == MultiTenantTransport.SafeTenantId)
			{
				return availableResourceSize * this.config.SafeTenantOrganizationQueueQuota / 100;
			}
			if (organizationId == TemplateTenantConfiguration.TemplateTenantExternalDirectoryOrganizationIdGuid)
			{
				return availableResourceSize * this.config.OutlookTenantOrganizationQueueQuota / 100;
			}
			return availableResourceSize * this.config.OrganizationQueueQuota / 100;
		}

		int IQueueQuotaThresholdFetcher.GetOrganizationWarningMark(Guid organizationId, QueueQuotaResources resource)
		{
			return (int)((double)((IQueueQuotaThresholdFetcher)this).GetOrganizationQuotaHighMark(organizationId, resource) * this.config.WarningRatio);
		}

		int IQueueQuotaThresholdFetcher.GetSenderQuotaHighMark(Guid organizationId, string sender, QueueQuotaResources resource)
		{
			int num = (sender == RoutingAddress.NullReversePath.ToString()) ? this.config.NullSenderQueueQuota : this.config.SenderQueueQuota;
			int num2 = this.config.OrganizationQueueQuota;
			if (organizationId == TemplateTenantConfiguration.TemplateTenantExternalDirectoryOrganizationIdGuid)
			{
				num = this.config.OutlookTenantSenderQueueQuota;
				num2 = this.config.OutlookTenantOrganizationQueueQuota;
			}
			return (int)((long)((IQueueQuotaThresholdFetcher)this).GetAvailableResourceSize(resource) * (long)num2 * (long)num / 10000L);
		}

		int IQueueQuotaThresholdFetcher.GetSenderWarningMark(Guid organizationId, string sender, QueueQuotaResources resource)
		{
			return (int)((double)((IQueueQuotaThresholdFetcher)this).GetSenderQuotaHighMark(organizationId, sender, resource) * this.config.WarningRatio);
		}

		int IQueueQuotaThresholdFetcher.GetAccountForestQuotaHighMark(QueueQuotaResources resource)
		{
			return ((IQueueQuotaThresholdFetcher)this).GetAvailableResourceSize(resource) * this.config.AccountForestQueueQuota / 100;
		}

		int IQueueQuotaThresholdFetcher.GetAccountForestWarningMark(QueueQuotaResources resource)
		{
			return (int)((double)((IQueueQuotaThresholdFetcher)this).GetAccountForestQuotaHighMark(resource) * this.config.WarningRatio);
		}

		private int GetResourceCapacity(QueueQuotaResources resource)
		{
			return QueueQuotaHelper.GetResourceCapacity(resource, this.config);
		}

		private readonly AbsoluteCountConfig acceptedCountConfig;

		private readonly AbsoluteCountConfig rejectedCountConfig;

		private readonly RollingCountConfig pastRejectedCountConfig;

		private readonly Func<DateTime> currentTimeProvider;

		private IQueueQuotaConfig config;

		private IFlowControlLog flowControlLog;

		private IQueueQuotaComponentPerformanceCounters perfCounters;

		private IProcessingQuotaComponent processingQuotaComponent;

		private ICountTracker<MeteredEntity, MeteredCount> metering;

		private ICountedEntity<MeteredEntity> totalEntity;

		private QueueQuotaDiagnosticLogger logger;
	}
}

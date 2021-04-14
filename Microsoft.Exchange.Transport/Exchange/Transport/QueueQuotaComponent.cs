using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Metering;
using Microsoft.Exchange.Data.Metering.Throttling;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.LoggingCommon;
using Microsoft.Exchange.Transport.QueueQuota;

namespace Microsoft.Exchange.Transport
{
	internal class QueueQuotaComponent : IQueueQuotaComponent, ITransportComponent, IDiagnosable
	{
		public QueueQuotaComponent() : this(() => DateTime.UtcNow)
		{
		}

		public QueueQuotaComponent(Func<DateTime> currentTimeProvider)
		{
			this.currentTimeProvider = currentTimeProvider;
		}

		public void SetRunTimeDependencies(IQueueQuotaConfig config, IFlowControlLog log, IQueueQuotaComponentPerformanceCounters perfCounters, IProcessingQuotaComponent processingQuotaComponent, IQueueQuotaObservableComponent submissionQueue, IQueueQuotaObservableComponent deliveryQueue, ICountTracker<MeteredEntity, MeteredCount> meteringComponent)
		{
			this.config = config;
			this.flowControlLog = log;
			this.flowControlLog.TrackSummary += this.LogSummary;
			this.perfCounters = perfCounters;
			this.processingQuotaComponent = processingQuotaComponent;
			this.totalData = new UsageData(this.config.TrackSummaryLoggingInterval, this.config.TrackSummaryBucketLength);
			if (submissionQueue != null)
			{
				submissionQueue.OnAcquire += delegate(TransportMailItem tmi)
				{
					this.TrackEnteringQueue(tmi, QueueQuotaResources.SubmissionQueueSize | QueueQuotaResources.TotalQueueSize);
				};
				submissionQueue.OnRelease += delegate(TransportMailItem tmi)
				{
					this.TrackExitingQueue(tmi, QueueQuotaResources.SubmissionQueueSize | QueueQuotaResources.TotalQueueSize);
				};
			}
			if (deliveryQueue != null)
			{
				deliveryQueue.OnAcquire += delegate(TransportMailItem tmi)
				{
					this.TrackEnteringQueue(tmi, QueueQuotaResources.TotalQueueSize);
				};
				deliveryQueue.OnRelease += delegate(TransportMailItem tmi)
				{
					this.TrackExitingQueue(tmi, QueueQuotaResources.TotalQueueSize);
				};
			}
		}

		public void TrackEnteringQueue(IQueueQuotaMailItem mailItem, QueueQuotaResources resources)
		{
			Guid externalOrganizationId = mailItem.ExternalOrganizationId;
			string originalFromAddress = mailItem.OriginalFromAddress;
			this.totalData.IncrementUsage(resources);
			OrganizationUsageData orAdd = this.orgQuotaDictionary.GetOrAdd(externalOrganizationId, new Func<Guid, OrganizationUsageData>(this.NewOrganizationUsageData));
			orAdd.IncrementUsage(resources);
			mailItem.QueueQuotaTrackingBits[QueueQuotaEntity.Organization, resources] = true;
			UsageData.AddOrMerge<Guid, OrganizationUsageData>(this.orgQuotaDictionary, externalOrganizationId, orAdd);
			if (!string.IsNullOrEmpty(originalFromAddress) && orAdd.GetUsage(resources) > this.config.SenderTrackingThreshold)
			{
				UsageData orAdd2 = orAdd.SenderQuotaDictionary.GetOrAdd(originalFromAddress, new Func<string, UsageData>(this.NewUsageData));
				orAdd2.IncrementUsage(resources);
				mailItem.QueueQuotaTrackingBits[QueueQuotaEntity.Sender, resources] = true;
				UsageData.AddOrMerge<string, UsageData>(orAdd.SenderQuotaDictionary, originalFromAddress, orAdd2);
			}
		}

		public void TrackExitingQueue(IQueueQuotaMailItem mailItem, QueueQuotaResources resources)
		{
			Guid externalOrganizationId = mailItem.ExternalOrganizationId;
			string originalFromAddress = mailItem.OriginalFromAddress;
			this.totalData.DecrementUsage(resources);
			if (mailItem.QueueQuotaTrackingBits[QueueQuotaEntity.Organization, resources])
			{
				OrganizationUsageData orAdd = this.orgQuotaDictionary.GetOrAdd(externalOrganizationId, new Func<Guid, OrganizationUsageData>(this.NewOrganizationUsageData));
				orAdd.DecrementUsage(resources);
				mailItem.QueueQuotaTrackingBits[QueueQuotaEntity.Organization, resources] = false;
				if (!string.IsNullOrEmpty(originalFromAddress) && mailItem.QueueQuotaTrackingBits[QueueQuotaEntity.Sender, resources])
				{
					UsageData orAdd2 = orAdd.SenderQuotaDictionary.GetOrAdd(originalFromAddress, new Func<string, UsageData>(this.NewUsageData));
					orAdd2.DecrementUsage(resources);
					mailItem.QueueQuotaTrackingBits[QueueQuotaEntity.Sender, resources] = false;
				}
			}
		}

		public bool IsOrganizationOverQuota(string accountForest, Guid externalOrganizationId, string sender, out string reason)
		{
			QueueQuotaEntity? entity;
			QueueQuotaResources? queueQuotaResources;
			bool flag = this.IsOverQuota(externalOrganizationId, sender, out reason, out entity, out queueQuotaResources);
			if (flag)
			{
				QueueQuotaEntity valueOrDefault = entity.GetValueOrDefault();
				if (entity != null)
				{
					switch (valueOrDefault)
					{
					case QueueQuotaEntity.Organization:
					{
						OrganizationUsageData orAdd = this.orgQuotaDictionary.GetOrAdd(externalOrganizationId, new Func<Guid, OrganizationUsageData>(this.NewOrganizationUsageData));
						orAdd.IncrementRejected((queueQuotaResources != null) ? queueQuotaResources.Value : QueueQuotaResources.All);
						goto IL_EB;
					}
					case QueueQuotaEntity.Sender:
						if (!string.IsNullOrEmpty(sender))
						{
							OrganizationUsageData orAdd = this.orgQuotaDictionary.GetOrAdd(externalOrganizationId, new Func<Guid, OrganizationUsageData>(this.NewOrganizationUsageData));
							UsageData orAdd2 = orAdd.SenderQuotaDictionary.GetOrAdd(sender, new Func<string, UsageData>(this.NewUsageData));
							orAdd2.IncrementRejected(queueQuotaResources.Value);
							goto IL_EB;
						}
						goto IL_EB;
					}
				}
				this.totalData.IncrementRejected(queueQuotaResources.Value);
				int num;
				DateTime dateTime;
				this.totalData.ResetThrottledData(queueQuotaResources.Value, out num, out dateTime);
			}
			IL_EB:
			if (this.config.EnforceQuota && flag)
			{
				this.perfCounters.IncrementMessagesRejected(entity, new Guid?(externalOrganizationId));
				return true;
			}
			return false;
		}

		public void TimedUpdate()
		{
			this.perfCounters.Refresh(new QueueQuotaEntity?(QueueQuotaEntity.Organization));
			this.perfCounters.Refresh(new QueueQuotaEntity?(QueueQuotaEntity.Sender));
			this.perfCounters.Refresh(null);
		}

		public bool IsOrganizationOverWarning(string accountForest, Guid externalOrganizationId, string sender, QueueQuotaResources resource)
		{
			OrganizationUsageData organizationUsageData;
			if (!this.orgQuotaDictionary.TryGetValue(externalOrganizationId, out organizationUsageData))
			{
				return false;
			}
			UsageData usageData;
			if (!string.IsNullOrEmpty(sender) && organizationUsageData.SenderQuotaDictionary.TryGetValue(sender, out usageData))
			{
				return usageData.GetOverWarningFlag(resource);
			}
			return organizationUsageData.GetOverWarningFlag(resource);
		}

		internal bool IsOverQuota(Guid externalOrganizationId, string sender, out string reason, out QueueQuotaEntity? reasonEntity, out QueueQuotaResources? reasonResource)
		{
			reason = null;
			reasonEntity = null;
			reasonResource = null;
			if (this.IsOrganizationBlocked(externalOrganizationId))
			{
				reason = "Organization is in block list.";
				reasonEntity = new QueueQuotaEntity?(QueueQuotaEntity.Organization);
				return true;
			}
			if (this.IsOrganizationAllowListed(externalOrganizationId))
			{
				reason = "Organization is in allow list.";
				return false;
			}
			foreach (QueueQuotaResources queueQuotaResources in this.allResources)
			{
				if (this.GetUsage(queueQuotaResources) > this.GetResourceCapacity(queueQuotaResources))
				{
					reason = string.Format("Resource {0} beyond capacity. Count:{1} Capacity:{2}", queueQuotaResources, this.GetUsage(queueQuotaResources), this.GetResourceCapacity(queueQuotaResources));
					reasonResource = new QueueQuotaResources?(queueQuotaResources);
					return true;
				}
			}
			OrganizationUsageData organizationUsageData;
			if (this.orgQuotaDictionary.TryGetValue(externalOrganizationId, out organizationUsageData))
			{
				UsageData usageData = null;
				bool flag = !string.IsNullOrEmpty(sender) && organizationUsageData.SenderQuotaDictionary.TryGetValue(sender, out usageData);
				foreach (QueueQuotaResources queueQuotaResources2 in this.allResources)
				{
					if (this.ComputeIsOverQuota(externalOrganizationId, QueueQuotaEntity.Organization, externalOrganizationId.ToString(), organizationUsageData, queueQuotaResources2, this.GetOrganizationQuotaHighMark(externalOrganizationId, queueQuotaResources2), false, out reason))
					{
						reasonEntity = new QueueQuotaEntity?(QueueQuotaEntity.Organization);
						reasonResource = new QueueQuotaResources?(queueQuotaResources2);
						return true;
					}
					if (flag && this.ComputeIsOverQuota(externalOrganizationId, QueueQuotaEntity.Sender, sender, usageData, queueQuotaResources2, this.GetSenderQuotaHighMark(externalOrganizationId, sender, queueQuotaResources2), false, out reason))
					{
						reasonEntity = new QueueQuotaEntity?(QueueQuotaEntity.Sender);
						reasonResource = new QueueQuotaResources?(queueQuotaResources2);
						return true;
					}
				}
			}
			return false;
		}

		private bool ComputeIsOverQuota(Guid externalOrganizationId, QueueQuotaEntity entityType, string entityId, UsageData usageData, QueueQuotaResources resource, int high, bool onlySetUnthrottle, out string reason)
		{
			reason = null;
			int usage = usageData.GetUsage(resource);
			int num = (int)((double)high * this.config.LowWatermarkRatio);
			int num2 = (int)((double)high * this.config.WarningRatio);
			bool isOverQuota = usageData.GetIsOverQuota(resource);
			bool isOverQuota2 = usageData.GetIsOverQuota(QueueQuotaResources.All);
			bool overWarningFlag = usageData.GetOverWarningFlag(resource);
			bool flag = usage > high || (isOverQuota && usage > num);
			bool flag2 = usage > num2 && !flag;
			if ((!onlySetUnthrottle && ((flag ^ isOverQuota) || (flag2 ^ overWarningFlag))) || (onlySetUnthrottle && !flag && isOverQuota) || (!flag2 && overWarningFlag))
			{
				usageData.SetOverQuotaFlags(resource, flag, flag2);
			}
			if (flag)
			{
				if (onlySetUnthrottle)
				{
					return flag;
				}
				reason = string.Format("{0} is above quota for {1}.Actual:{2} Low:{3} High:{4}", new object[]
				{
					entityType,
					resource,
					usage,
					num,
					high
				});
				if (!isOverQuota)
				{
					this.flowControlLog.LogThrottle(QueueQuotaComponent.GetThrottlingResource(resource), ThrottlingAction.TempReject, high, TimeSpan.Zero, QueueQuotaComponent.GetThrottlingScope(entityType), externalOrganizationId, QueueQuotaComponent.GetRedactedSender(entityType, entityId), null, null, ThrottlingSource.QueueQuota, !this.config.EnforceQuota, new List<KeyValuePair<string, object>>
					{
						new KeyValuePair<string, object>("AvailableCapacity", this.GetAvailableResourceSize(resource)),
						new KeyValuePair<string, object>("observedValue", usage)
					});
					if (!isOverQuota2)
					{
						this.perfCounters.IncrementThrottledEntities(entityType, externalOrganizationId);
					}
				}
			}
			else if (isOverQuota)
			{
				int impact;
				DateTime value;
				usageData.ResetThrottledData(resource, out impact, out value);
				this.flowControlLog.LogUnthrottle(QueueQuotaComponent.GetThrottlingResource(resource), ThrottlingAction.TempReject, num, TimeSpan.Zero, impact, usage, QueueQuotaComponent.GetThrottlingScope(entityType), externalOrganizationId, QueueQuotaComponent.GetRedactedSender(entityType, entityId), null, null, ThrottlingSource.QueueQuota, !this.config.EnforceQuota, new List<KeyValuePair<string, object>>
				{
					new KeyValuePair<string, object>("AvailableCapacity", this.GetAvailableResourceSize(resource)),
					new KeyValuePair<string, object>("throttledDuration", this.currentTimeProvider().Subtract(value).ToString("d\\.hh\\:mm\\:ss"))
				});
				if (!usageData.GetIsOverQuota(QueueQuotaResources.All))
				{
					this.perfCounters.DecrementThrottledEntities(entityType, externalOrganizationId);
				}
			}
			else if (!onlySetUnthrottle && flag2 && !overWarningFlag)
			{
				this.flowControlLog.LogWarning(QueueQuotaComponent.GetThrottlingResource(resource), ThrottlingAction.TempReject, num2, TimeSpan.Zero, QueueQuotaComponent.GetThrottlingScope(entityType), externalOrganizationId, QueueQuotaComponent.GetRedactedSender(entityType, entityId), null, null, ThrottlingSource.QueueQuota, !this.config.EnforceQuota, new List<KeyValuePair<string, object>>
				{
					new KeyValuePair<string, object>("AvailableCapacity", this.GetAvailableResourceSize(resource)),
					new KeyValuePair<string, object>("observedValue", usage)
				});
			}
			return flag;
		}

		private static string GetRedactedSender(QueueQuotaEntity entityType, string entityId)
		{
			string result = null;
			if (QueueQuotaComponent.GetSender(entityType, entityId) != null)
			{
				if (RoutingAddress.IsValidAddress(entityId))
				{
					result = SuppressingPiiData.Redact(RoutingAddress.Parse(entityId)).ToString();
				}
				else
				{
					result = SuppressingPiiData.Redact(entityId);
				}
			}
			return result;
		}

		private static ThrottlingScope GetThrottlingScope(QueueQuotaEntity entityType)
		{
			switch (entityType)
			{
			case QueueQuotaEntity.Organization:
				return ThrottlingScope.Tenant;
			case QueueQuotaEntity.Sender:
				return ThrottlingScope.Sender;
			default:
				throw new InvalidOperationException(string.Format("Unexpected QueueQuotaEntity found: {0}", entityType));
			}
		}

		private static ThrottlingResource GetThrottlingResource(QueueQuotaResources resource)
		{
			switch (resource)
			{
			case QueueQuotaResources.SubmissionQueueSize:
				return ThrottlingResource.SubmissionQueueQuota;
			case QueueQuotaResources.TotalQueueSize:
				return ThrottlingResource.TotalQueueQuota;
			default:
				throw new InvalidOperationException(string.Format("Unexpected QueueQuotaResources found: {0}", resource));
			}
		}

		private static string GetSender(QueueQuotaEntity entityType, string entityId)
		{
			if (entityType != QueueQuotaEntity.Sender)
			{
				return null;
			}
			return entityId;
		}

		private UsageData NewUsageData(string key)
		{
			return new UsageData(this.config.TrackSummaryLoggingInterval, this.config.TrackSummaryBucketLength, this.currentTimeProvider);
		}

		private OrganizationUsageData NewOrganizationUsageData(Guid key)
		{
			return new OrganizationUsageData(key, this.config.TrackSummaryLoggingInterval, this.config.TrackSummaryBucketLength, this.currentTimeProvider);
		}

		private int GetAvailableResourceSize(QueueQuotaResources resource)
		{
			return Math.Max(this.GetResourceCapacity(resource) - this.GetUsage(resource), 0);
		}

		private int GetOrganizationQuotaHighMark(Guid organizationId, QueueQuotaResources resource)
		{
			if (organizationId == MultiTenantTransport.SafeTenantId)
			{
				return this.GetAvailableResourceSize(resource) * this.config.SafeTenantOrganizationQueueQuota / 100;
			}
			return this.GetAvailableResourceSize(resource) * this.config.OrganizationQueueQuota / 100;
		}

		private int GetOrganizationWarningMark(Guid organizationId, QueueQuotaResources resource)
		{
			return (int)((double)this.GetOrganizationQuotaHighMark(organizationId, resource) * this.config.WarningRatio);
		}

		private int GetSenderQuotaHighMark(Guid organizationId, string sender, QueueQuotaResources resource)
		{
			int num = (sender == RoutingAddress.NullReversePath.ToString()) ? this.config.NullSenderQueueQuota : this.config.SenderQueueQuota;
			return this.GetAvailableResourceSize(resource) * this.config.OrganizationQueueQuota * num / 10000;
		}

		private int GetSenderWarningMark(Guid organizationId, string sender, QueueQuotaResources resource)
		{
			return (int)((double)this.GetSenderQuotaHighMark(organizationId, sender, resource) * this.config.WarningRatio);
		}

		private int GetResourceCapacity(QueueQuotaResources resource)
		{
			switch (resource)
			{
			case QueueQuotaResources.SubmissionQueueSize:
				return this.config.SubmissionQueueCapacity;
			case QueueQuotaResources.TotalQueueSize:
				return this.config.TotalQueueCapacity;
			default:
				throw new ArgumentOutOfRangeException("resource");
			}
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

		private void LogSummary(string sequenceNumber)
		{
			this.Cleanup(this.config.TrackerEntryLifeTime);
			foreach (QueueQuotaResources queueQuotaResources in this.allResources)
			{
				if (this.totalData.GetRejectedCount(queueQuotaResources) > 0)
				{
					this.flowControlLog.LogSummary(sequenceNumber, QueueQuotaComponent.GetThrottlingResource(queueQuotaResources), ThrottlingAction.TempReject, this.GetResourceCapacity(queueQuotaResources), TimeSpan.Zero, this.totalData.GetUsage(queueQuotaResources), this.totalData.GetRejectedCount(queueQuotaResources), ThrottlingScope.All, Guid.Empty, null, null, null, ThrottlingSource.QueueQuota, !this.config.EnforceQuota, null);
				}
			}
			IEnumerable<QueueQuotaComponent.SortableEntry> enumerable = Enumerable.Empty<QueueQuotaComponent.SortableEntry>();
			QueueQuotaResources[] array2 = this.allResources;
			for (int j = 0; j < array2.Length; j++)
			{
				QueueQuotaResources resource = array2[j];
				enumerable = enumerable.Concat(this.orgQuotaDictionary.SelectMany((KeyValuePair<Guid, OrganizationUsageData> o) => from s in o.Value
				select new QueueQuotaComponent.SortableEntry(o.Key, s.Key, resource, s.Value)));
			}
			this.LogUnthrottledAndUpdateState(enumerable);
			int num;
			if (!this.LogThrottled(enumerable, sequenceNumber, out num))
			{
				IEnumerable<QueueQuotaComponent.SortableEntry> enumerable2 = (from data in enumerable
				where data.Usage.GetOverWarningFlag(data.Resource) && !data.Usage.GetIsOverQuota(data.Resource)
				orderby data.Usage.GetUsage(data.Resource) descending
				select data).Take(Math.Max(this.config.MaxSummaryLinesLogged - num, 0));
				foreach (QueueQuotaComponent.SortableEntry sortableEntry in enumerable2)
				{
					bool isOrganization = sortableEntry.IsOrganization;
					this.flowControlLog.LogSummaryWarning(QueueQuotaComponent.GetThrottlingResource(sortableEntry.Resource), ThrottlingAction.TempReject, isOrganization ? this.GetOrganizationWarningMark(sortableEntry.Tenant, sortableEntry.Resource) : this.GetSenderWarningMark(sortableEntry.Tenant, sortableEntry.Sender, sortableEntry.Resource), TimeSpan.Zero, isOrganization ? ThrottlingScope.Tenant : ThrottlingScope.Sender, sortableEntry.Tenant, (!isOrganization) ? QueueQuotaComponent.GetRedactedSender(QueueQuotaEntity.Sender, sortableEntry.Sender) : null, null, null, ThrottlingSource.QueueQuota, !this.config.EnforceQuota, new List<KeyValuePair<string, object>>
					{
						new KeyValuePair<string, object>("AvailableCapacity", this.GetAvailableResourceSize(sortableEntry.Resource)),
						new KeyValuePair<string, object>("observedValue", sortableEntry.Usage.GetUsage(sortableEntry.Resource))
					});
				}
			}
		}

		private void LogUnthrottledAndUpdateState(IEnumerable<QueueQuotaComponent.SortableEntry> allRows)
		{
			double ratio = Math.Max(this.config.LowWatermarkRatio, this.config.WarningRatio);
			IEnumerable<QueueQuotaComponent.SortableEntry> enumerable = from r in allRows
			where (r.IsOrganization && (double)r.Usage.GetUsage(r.Resource) < (double)this.GetOrganizationQuotaHighMark(r.Tenant, r.Resource) * ratio) || (!r.IsOrganization && (double)r.Usage.GetUsage(r.Resource) < (double)this.GetSenderQuotaHighMark(r.Tenant, r.Sender, r.Resource) * ratio)
			select r;
			foreach (QueueQuotaComponent.SortableEntry sortableEntry in enumerable)
			{
				string text;
				this.ComputeIsOverQuota(sortableEntry.Tenant, sortableEntry.IsOrganization ? QueueQuotaEntity.Organization : QueueQuotaEntity.Sender, sortableEntry.IsOrganization ? sortableEntry.Tenant.ToString() : sortableEntry.Sender, sortableEntry.Usage, sortableEntry.Resource, sortableEntry.IsOrganization ? this.GetOrganizationQuotaHighMark(sortableEntry.Tenant, sortableEntry.Resource) : this.GetSenderQuotaHighMark(sortableEntry.Tenant, sortableEntry.Sender, sortableEntry.Resource), true, out text);
			}
		}

		private bool LogThrottled(IEnumerable<QueueQuotaComponent.SortableEntry> allRows, string sequenceNumber, out int resultCount)
		{
			IEnumerable<QueueQuotaComponent.SortableEntry> enumerable = from data in allRows
			where data.Usage.GetIsOverQuota(data.Resource) || data.Usage.GetRejectedCount(data.Resource) > 0
			orderby data.Usage.GetRejectedCount(data.Resource) descending, data.Usage.GetUsage(data.Resource)
			select data;
			bool flag = false;
			resultCount = enumerable.Count<QueueQuotaComponent.SortableEntry>();
			if (resultCount > this.config.MaxSummaryLinesLogged)
			{
				enumerable = enumerable.Take(this.config.MaxSummaryLinesLogged);
				flag = true;
			}
			foreach (QueueQuotaComponent.SortableEntry sortableEntry in enumerable)
			{
				bool isOrganization = sortableEntry.IsOrganization;
				this.flowControlLog.LogSummary(sequenceNumber, QueueQuotaComponent.GetThrottlingResource(sortableEntry.Resource), ThrottlingAction.TempReject, isOrganization ? this.GetOrganizationQuotaHighMark(sortableEntry.Tenant, sortableEntry.Resource) : this.GetSenderQuotaHighMark(sortableEntry.Tenant, sortableEntry.Sender, sortableEntry.Resource), TimeSpan.Zero, sortableEntry.Usage.GetUsage(sortableEntry.Resource), sortableEntry.Usage.GetRejectedCount(sortableEntry.Resource), isOrganization ? ThrottlingScope.Tenant : ThrottlingScope.Sender, sortableEntry.Tenant, (!isOrganization) ? QueueQuotaComponent.GetRedactedSender(QueueQuotaEntity.Sender, sortableEntry.Sender) : null, null, null, ThrottlingSource.QueueQuota, !this.config.EnforceQuota, new List<KeyValuePair<string, object>>
				{
					new KeyValuePair<string, object>("AvailableCapacity", this.GetAvailableResourceSize(sortableEntry.Resource))
				});
			}
			if (flag)
			{
				this.flowControlLog.LogMaxLinesExceeded(sequenceNumber, ThrottlingSource.QueueQuota, this.config.MaxSummaryLinesLogged, resultCount, null);
			}
			this.UpdatedOldestEntityPerfCounter(allRows, QueueQuotaEntity.Organization, false);
			this.UpdatedOldestEntityPerfCounter(allRows, QueueQuotaEntity.Sender, false);
			this.UpdatedOldestEntityPerfCounter(allRows, QueueQuotaEntity.Organization, true);
			return flag;
		}

		private void UpdatedOldestEntityPerfCounter(IEnumerable<QueueQuotaComponent.SortableEntry> allRows, QueueQuotaEntity entity, bool isForSafeTenant)
		{
			bool isOrg = entity == QueueQuotaEntity.Organization;
			bool flag = false;
			Guid organizationId = isForSafeTenant ? MultiTenantTransport.SafeTenantId : Guid.Empty;
			using (IEnumerator<QueueQuotaComponent.SortableEntry> enumerator = (from data in allRows
			where data.Usage.GetIsOverQuota(data.Resource) && data.IsOrganization == isOrg && (!isOrg || data.Tenant == MultiTenantTransport.SafeTenantId == isForSafeTenant)
			orderby data.Usage.ThrottlingStartTime
			select data).Take(1).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					QueueQuotaComponent.SortableEntry sortableEntry = enumerator.Current;
					this.perfCounters.UpdateOldestThrottledEntity(entity, this.currentTimeProvider() - sortableEntry.Usage.ThrottlingStartTime, sortableEntry.Tenant);
					flag = true;
					organizationId = sortableEntry.Tenant;
				}
			}
			if (!flag)
			{
				this.perfCounters.UpdateOldestThrottledEntity(entity, TimeSpan.Zero, organizationId);
			}
		}

		private XElement GetDiagnosticInfo()
		{
			XElement xelement = new XElement("QueueUsage");
			xelement.SetAttributeValue("Version", "OldQQ");
			xelement.SetAttributeValue("NumberOfOrganizationsTracked", this.TrackedOrganizationCount);
			xelement.SetAttributeValue("NumberOfSendersTracked", this.TrackedSenderCount);
			xelement.SetAttributeValue("NumberOfOrganizationsDisplayed", this.config.NumberOfOrganizationsLoggedInSummary);
			QueueQuotaResources[] array = this.allResources;
			for (int i = 0; i < array.Length; i++)
			{
				QueueQuotaResources resource = array[i];
				XElement xelement2 = new XElement(resource.ToString());
				xelement2.SetAttributeValue("TotalUsage", this.GetUsage(resource));
				foreach (KeyValuePair<Guid, OrganizationUsageData> keyValuePair in (from data in this.orgQuotaDictionary
				orderby data.Value.GetUsage(resource) descending
				select data).Take(this.config.NumberOfOrganizationsLoggedInSummary))
				{
					XElement usageElement = keyValuePair.Value.GetUsageElement(QueueQuotaEntity.Organization.ToString(), resource, keyValuePair.Key.ToString());
					foreach (KeyValuePair<string, UsageData> keyValuePair2 in (from s in keyValuePair.Value.SenderQuotaDictionary
					orderby s.Value.GetUsage(resource) descending
					select s).Take(this.config.NumberOfSendersLoggedInSummary))
					{
						usageElement.Add(keyValuePair2.Value.GetUsageElement(QueueQuotaEntity.Sender.ToString(), resource, keyValuePair2.Key));
					}
					xelement2.Add(usageElement);
				}
				xelement.Add(xelement2);
			}
			return xelement;
		}

		private XElement GetDiagnosticInfo(Guid externalOrganizationId)
		{
			XElement xelement = new XElement("QueueUsage");
			OrganizationUsageData organizationUsageData;
			if (this.orgQuotaDictionary.TryGetValue(externalOrganizationId, out organizationUsageData))
			{
				QueueQuotaResources[] array = this.allResources;
				for (int i = 0; i < array.Length; i++)
				{
					QueueQuotaResources resource = array[i];
					XElement xelement2 = new XElement(resource.ToString());
					XElement usageElement = organizationUsageData.GetUsageElement(QueueQuotaEntity.Organization.ToString(), resource, externalOrganizationId.ToString());
					foreach (KeyValuePair<string, UsageData> keyValuePair in from s in organizationUsageData.SenderQuotaDictionary
					orderby s.Value.GetUsage(resource) descending
					select s)
					{
						usageElement.Add(keyValuePair.Value.GetUsageElement(QueueQuotaEntity.Sender.ToString(), resource, keyValuePair.Key));
					}
					xelement2.Add(usageElement);
					xelement.Add(xelement2);
				}
			}
			else
			{
				xelement.Add(new XElement("Error", string.Format("Organization with id {0} not present in queue quota component", externalOrganizationId)));
			}
			return xelement;
		}

		private int TrackedOrganizationCount
		{
			get
			{
				return this.orgQuotaDictionary.Count;
			}
		}

		private int TrackedSenderCount
		{
			get
			{
				return this.orgQuotaDictionary.Sum(delegate(KeyValuePair<Guid, OrganizationUsageData> data)
				{
					if (data.Value.SenderQuotaDictionary == null)
					{
						return 0;
					}
					return data.Value.SenderQuotaDictionary.Count;
				});
			}
		}

		private int GetUsage(QueueQuotaResources resource)
		{
			return this.totalData.GetUsage(resource);
		}

		public void Cleanup(TimeSpan cleanupInterval)
		{
			UsageData.Cleanup<Guid, OrganizationUsageData>(this.orgQuotaDictionary, cleanupInterval);
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "QueueQuota";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(((IDiagnosable)this).GetDiagnosticComponentName());
			bool flag = parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag2 = parameters.Argument.IndexOf("tenant:", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag3 = parameters.Argument.Equals("config", StringComparison.InvariantCultureIgnoreCase);
			bool flag4 = (!flag3 && !flag && !flag2) || parameters.Argument.IndexOf("help", StringComparison.OrdinalIgnoreCase) != -1;
			if (flag)
			{
				xelement.Add(this.GetDiagnosticInfo());
			}
			if (flag2)
			{
				string text = parameters.Argument.Substring(7);
				Guid externalOrganizationId;
				if (Guid.TryParse(text, out externalOrganizationId))
				{
					xelement.Add(this.GetDiagnosticInfo(externalOrganizationId));
				}
				else
				{
					xelement.Add(new XElement("Error", string.Format("Invalid external organization id {0} passed as argument. Expecting a Guid.", text)));
				}
			}
			if (flag3)
			{
				xelement.Add(TransportAppConfig.GetDiagnosticInfoForType(this.config));
			}
			if (flag4)
			{
				xelement.Add(new XElement("help", "Supported arguments: verbose, help, config, tenant:{tenantID e.g.1afa2e80-0251-4521-8086-039fb2f9d8d6}."));
			}
			return xelement;
		}

		public void Load()
		{
		}

		public void Unload()
		{
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		private const string DiagnosticsComponentName = "QueueQuota";

		private const string AvailableCapacityProperty = "AvailableCapacity";

		private IQueueQuotaConfig config;

		private readonly ConcurrentDictionary<Guid, OrganizationUsageData> orgQuotaDictionary = new ConcurrentDictionary<Guid, OrganizationUsageData>(Environment.ProcessorCount, 1000);

		private readonly QueueQuotaResources[] allResources = new QueueQuotaResources[]
		{
			QueueQuotaResources.SubmissionQueueSize,
			QueueQuotaResources.TotalQueueSize
		};

		private readonly Func<DateTime> currentTimeProvider;

		private UsageData totalData;

		private IFlowControlLog flowControlLog;

		private IQueueQuotaComponentPerformanceCounters perfCounters;

		private IProcessingQuotaComponent processingQuotaComponent;

		private class SortableEntry
		{
			public SortableEntry(Guid tenant, string sender, QueueQuotaResources resource, UsageData usage)
			{
				this.Tenant = tenant;
				this.Sender = sender;
				this.Resource = resource;
				this.Usage = usage;
			}

			public Guid Tenant { get; set; }

			public string Sender { get; set; }

			public QueueQuotaResources Resource { get; set; }

			public UsageData Usage { get; set; }

			public bool IsOrganization
			{
				get
				{
					return this.Usage is OrganizationUsageData;
				}
			}
		}
	}
}

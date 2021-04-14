using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Metering;
using Microsoft.Exchange.Data.Metering.Throttling;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Transport
{
	internal class QueueQuotaDiagnosticLogger
	{
		public QueueQuotaDiagnosticLogger(IQueueQuotaConfig config, IFlowControlLog flowControlLog, ICountTracker<MeteredEntity, MeteredCount> metering, ICountedEntity<MeteredEntity> totalEntity, IQueueQuotaComponentPerformanceCounters perfCounters, IQueueQuotaThresholdFetcher thresholdFetcher, Func<DateTime> currentTimeProvider)
		{
			this.config = config;
			this.flowControlLog = flowControlLog;
			this.metering = metering;
			this.totalEntity = totalEntity;
			this.perfCounters = perfCounters;
			this.thresholdFetcher = thresholdFetcher;
			this.currentTimeProvider = currentTimeProvider;
			this.flowControlLog.TrackSummary += this.LogSummary;
		}

		internal XElement GetDiagnosticInfo()
		{
			XElement xelement = new XElement("QueueUsage");
			IEnumerable<ICount<MeteredEntity, MeteredCount>> source = this.metering.Filter((ICount<MeteredEntity, MeteredCount> c) => QueueQuotaHelper.AllAcceptedCounts.Contains(c.Measure) && c.Entity.Name.Type == MeteredEntity.Tenant);
			IEnumerable<ICount<MeteredEntity, MeteredCount>> source2 = this.metering.Filter((ICount<MeteredEntity, MeteredCount> c) => QueueQuotaHelper.AllAcceptedCounts.Contains(c.Measure) && c.Entity.Name.Type == MeteredEntity.Sender);
			IEnumerable<ICount<MeteredEntity, MeteredCount>> source3 = this.metering.Filter((ICount<MeteredEntity, MeteredCount> c) => QueueQuotaHelper.AllAcceptedCounts.Contains(c.Measure) && c.Entity.Name.Type == MeteredEntity.AccountForest);
			xelement.SetAttributeValue("NumberOfOrgEntries", source.Count<ICount<MeteredEntity, MeteredCount>>());
			xelement.SetAttributeValue("NumberOfSenderEntries", source2.Count<ICount<MeteredEntity, MeteredCount>>());
			xelement.SetAttributeValue("NumberOfForestEntries", source3.Count<ICount<MeteredEntity, MeteredCount>>());
			xelement.SetAttributeValue("NumberOfOrganizationsDisplayed", this.config.NumberOfOrganizationsLoggedInSummary);
			QueueQuotaResources[] allResources = QueueQuotaHelper.AllResources;
			for (int i = 0; i < allResources.Length; i++)
			{
				QueueQuotaResources queueQuotaResources = allResources[i];
				XElement xelement2 = new XElement(queueQuotaResources.ToString());
				MeteredCount meteredCount = QueueQuotaHelper.GetMeteredCount(queueQuotaResources).ToArray<MeteredCount>()[0];
				ICount<MeteredEntity, MeteredCount> usage = this.metering.GetUsage(this.totalEntity, meteredCount);
				xelement2.SetAttributeValue("TotalUsage", usage.Total);
				foreach (ICount<MeteredEntity, MeteredCount> count in from c in source3
				where c.Measure == meteredCount
				orderby c.Total descending
				select c)
				{
					XElement content;
					if (this.TryGetUsageElement(QueueQuotaEntity.AccountForest.ToString(), queueQuotaResources, count, out content))
					{
						xelement2.Add(content);
					}
				}
				using (IEnumerator<ICount<MeteredEntity, MeteredCount>> enumerator2 = (from c in source
				where c.Measure == meteredCount
				orderby c.Total descending
				select c).Take(this.config.NumberOfOrganizationsLoggedInSummary).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ICount<MeteredEntity, MeteredCount> org = enumerator2.Current;
						XElement xelement3;
						bool flag = this.TryGetUsageElement(QueueQuotaEntity.Organization.ToString(), queueQuotaResources, org, out xelement3);
						foreach (ICount<MeteredEntity, MeteredCount> count2 in (from c in source2
						where c.Measure == meteredCount && c.Entity.GroupName.Equals(org.Entity.Name)
						orderby c.Total descending
						select c).Take(this.config.NumberOfSendersLoggedInSummary))
						{
							XElement content2;
							if (this.TryGetUsageElement(QueueQuotaEntity.Sender.ToString(), queueQuotaResources, count2, out content2))
							{
								xelement3.Add(content2);
								flag = true;
							}
						}
						if (flag)
						{
							xelement2.Add(xelement3);
						}
					}
				}
				xelement.Add(xelement2);
			}
			return xelement;
		}

		internal XElement GetDiagnosticInfo(Guid externalOrganizationId)
		{
			XElement xelement = new XElement("QueueUsage");
			ICountedEntity<MeteredEntity> entity = QueueQuotaHelper.CreateEntity(externalOrganizationId);
			IDictionary<MeteredCount, ICount<MeteredEntity, MeteredCount>> usage = this.metering.GetUsage(entity, QueueQuotaHelper.AllAcceptedCounts);
			bool flag = false;
			using (IEnumerator<KeyValuePair<MeteredCount, ICount<MeteredEntity, MeteredCount>>> enumerator = usage.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<MeteredCount, ICount<MeteredEntity, MeteredCount>> pair = enumerator.Current;
					KeyValuePair<MeteredCount, ICount<MeteredEntity, MeteredCount>> pair4 = pair;
					QueueQuotaResources resource = QueueQuotaHelper.GetResource(pair4.Key);
					XElement xelement2 = new XElement(resource.ToString());
					string elementName = QueueQuotaEntity.Organization.ToString();
					QueueQuotaResources resource2 = resource;
					KeyValuePair<MeteredCount, ICount<MeteredEntity, MeteredCount>> pair2 = pair;
					XElement xelement3;
					bool flag2 = this.TryGetUsageElement(elementName, resource2, pair2.Value, out xelement3);
					foreach (ICount<MeteredEntity, MeteredCount> count in from c in this.metering.Filter(delegate(ICount<MeteredEntity, MeteredCount> c)
					{
						MeteredCount measure = c.Measure;
						KeyValuePair<MeteredCount, ICount<MeteredEntity, MeteredCount>> pair3 = pair;
						return measure == pair3.Key && c.Entity.GroupName.Equals(entity.Name);
					})
					orderby c.Total descending
					select c)
					{
						XElement xelement4;
						if (this.TryGetUsageElement(QueueQuotaEntity.Sender.ToString(), resource, count, out xelement4))
						{
							xelement3.Add(new object[0]);
							flag2 = true;
						}
					}
					if (flag2)
					{
						xelement2.Add(xelement3);
						xelement.Add(xelement2);
						flag = true;
					}
				}
			}
			if (!flag)
			{
				xelement.Add(new XElement("Error", string.Format("Organization with id {0} not present in queue quota component", externalOrganizationId)));
			}
			return xelement;
		}

		internal XElement GetDiagnosticInfo(string accountForest)
		{
			XElement xelement = new XElement("QueueUsage");
			ICountedEntity<MeteredEntity> entity = QueueQuotaHelper.CreateEntity(accountForest);
			IDictionary<MeteredCount, ICount<MeteredEntity, MeteredCount>> usage = this.metering.GetUsage(entity, QueueQuotaHelper.AllAcceptedCounts);
			bool flag = false;
			foreach (KeyValuePair<MeteredCount, ICount<MeteredEntity, MeteredCount>> keyValuePair in usage)
			{
				QueueQuotaResources resource = QueueQuotaHelper.GetResource(keyValuePair.Key);
				XElement xelement2 = new XElement(resource.ToString());
				XElement content;
				if (this.TryGetUsageElement(QueueQuotaEntity.Organization.ToString(), resource, keyValuePair.Value, out content))
				{
					xelement2.Add(content);
					xelement.Add(xelement2);
					flag = true;
				}
			}
			if (!flag)
			{
				xelement.Add(new XElement("Error", string.Format("Forest with id {0} not present in queue quota component", accountForest)));
			}
			return xelement;
		}

		private static bool IsThrottledOrRejected(ICountedEntityValue<MeteredEntity, MeteredCount> entity)
		{
			DateTime dateTime;
			return (entity.HasUsage(MeteredCount.AcceptedSubmissionQueue) && QueueQuotaHelper.HasThrottledTime(entity.GetUsage(MeteredCount.AcceptedSubmissionQueue), out dateTime)) || (entity.HasUsage(MeteredCount.AcceptedTotalQueue) && QueueQuotaHelper.HasThrottledTime(entity.GetUsage(MeteredCount.AcceptedTotalQueue), out dateTime)) || (entity.HasUsage(MeteredCount.RejectedSubmissionQueue) && entity.GetUsage(MeteredCount.RejectedSubmissionQueue).Total > 0L) || (entity.HasUsage(MeteredCount.RejectedTotalQueue) && entity.GetUsage(MeteredCount.RejectedTotalQueue).Total > 0L);
		}

		private void LogSummary(string sequenceNumber)
		{
			this.LogTotals(sequenceNumber);
			int num;
			if (!this.LogThrottled(sequenceNumber, out num))
			{
				this.LogWarnings(this.config.MaxSummaryLinesLogged - num);
			}
		}

		private void LogTotals(string sequenceNumber)
		{
			IEnumerable<ICount<MeteredEntity, MeteredCount>> allUsages = this.metering.GetAllUsages(this.totalEntity);
			if (allUsages == null)
			{
				return;
			}
			Dictionary<MeteredCount, ICount<MeteredEntity, MeteredCount>> dictionary = allUsages.ToDictionary((ICount<MeteredEntity, MeteredCount> c) => c.Measure);
			foreach (MeteredCount meteredCount in QueueQuotaHelper.AllAcceptedCounts)
			{
				QueueQuotaResources resource = QueueQuotaHelper.GetResource(meteredCount);
				ICount<MeteredEntity, MeteredCount> count;
				dictionary.TryGetValue(meteredCount, out count);
				MeteredCount rejectedMeter = QueueQuotaHelper.GetRejectedMeter(meteredCount);
				MeteredCount rejectedMeter2 = QueueQuotaHelper.GetRejectedMeter(rejectedMeter);
				ICount<MeteredEntity, MeteredCount> count2 = null;
				ICount<MeteredEntity, MeteredCount> count3 = null;
				if ((dictionary.TryGetValue(rejectedMeter, out count2) && count2.Total > 0L) || (dictionary.TryGetValue(rejectedMeter2, out count3) && count3.Total > 0L))
				{
					this.flowControlLog.LogSummary(sequenceNumber, QueueQuotaHelper.GetThrottlingResource(resource), ThrottlingAction.TempReject, QueueQuotaHelper.GetResourceCapacity(resource, this.config), TimeSpan.Zero, (count != null) ? ((int)count.Total) : 0, ((count2 != null) ? ((int)count2.Total) : 0) + ((count3 != null) ? ((int)count3.Total) : 0), ThrottlingScope.All, Guid.Empty, null, null, null, ThrottlingSource.QueueQuota, !this.config.EnforceQuota, null);
				}
			}
		}

		private bool LogThrottled(string sequenceNumber, out int resultCount)
		{
			IEnumerable<QueueQuotaDiagnosticLogger.EntityValue> enumerable = this.metering.Filter(new Func<ICountedEntityValue<MeteredEntity, MeteredCount>, bool>(QueueQuotaDiagnosticLogger.IsThrottledOrRejected)).SelectMany((ICountedEntityValue<MeteredEntity, MeteredCount> c) => new QueueQuotaDiagnosticLogger.EntityValue[]
			{
				new QueueQuotaDiagnosticLogger.EntityValue(c.Entity, QueueQuotaResources.SubmissionQueueSize, new ICount<MeteredEntity, MeteredCount>[]
				{
					c.GetUsage(MeteredCount.AcceptedSubmissionQueue),
					c.GetUsage(MeteredCount.RejectedSubmissionQueue),
					c.GetUsage(MeteredCount.CurrentRejectedSubmissionQueue)
				}),
				new QueueQuotaDiagnosticLogger.EntityValue(c.Entity, QueueQuotaResources.TotalQueueSize, new ICount<MeteredEntity, MeteredCount>[]
				{
					c.GetUsage(MeteredCount.AcceptedTotalQueue),
					c.GetUsage(MeteredCount.RejectedTotalQueue),
					c.GetUsage(MeteredCount.CurrentRejectedTotalQueue)
				})
			});
			IEnumerable<QueueQuotaDiagnosticLogger.EntityValue> enumerable2 = from e in enumerable
			where e.ThrottledTime < DateTime.MaxValue || e.GetRejectedSum() > 0L
			select e into v
			orderby v.GetRejectedSum() descending, v.GetUsageSum()
			select v;
			bool flag = false;
			resultCount = enumerable2.Count<QueueQuotaDiagnosticLogger.EntityValue>();
			if (resultCount > this.config.MaxSummaryLinesLogged)
			{
				enumerable2 = enumerable2.Take(this.config.MaxSummaryLinesLogged);
				flag = true;
			}
			foreach (QueueQuotaDiagnosticLogger.EntityValue entityValue in enumerable2)
			{
				if (QueueQuotaHelper.ShouldProcessEntity(entityValue.Entity))
				{
					QueueQuotaLoggingContext queueQuotaLoggingContext = new QueueQuotaLoggingContext(entityValue.Entity, entityValue.Resource, this.thresholdFetcher);
					this.flowControlLog.LogSummary(sequenceNumber, queueQuotaLoggingContext.Resource, ThrottlingAction.TempReject, queueQuotaLoggingContext.HighThreshold, TimeSpan.Zero, (int)entityValue.GetUsageSum(), (int)entityValue.GetRejectedSum(), queueQuotaLoggingContext.Scope, queueQuotaLoggingContext.OrgId, queueQuotaLoggingContext.Sender, null, null, ThrottlingSource.QueueQuota, !this.config.EnforceQuota, new List<KeyValuePair<string, object>>
					{
						new KeyValuePair<string, object>("AvailableCapacity", this.thresholdFetcher.GetAvailableResourceSize(entityValue.Resource)),
						new KeyValuePair<string, object>("ScopeValue", queueQuotaLoggingContext.ScopeValue)
					});
				}
			}
			if (flag)
			{
				this.flowControlLog.LogMaxLinesExceeded(sequenceNumber, ThrottlingSource.QueueQuota, this.config.MaxSummaryLinesLogged, resultCount, null);
			}
			this.UpdateOldestThrottledPerfCounter(enumerable);
			return flag;
		}

		private void LogWarnings(int maxRows)
		{
			IEnumerable<ICount<MeteredEntity, MeteredCount>> enumerable = (from c in this.metering.Filter(new Func<ICount<MeteredEntity, MeteredCount>, bool>(QueueQuotaHelper.IsWarningLogged))
			orderby c.Total descending
			select c).Take(Math.Max(maxRows, 0));
			foreach (ICount<MeteredEntity, MeteredCount> count in enumerable)
			{
				if (QueueQuotaHelper.ShouldProcessEntity(count.Entity))
				{
					QueueQuotaResources resource = QueueQuotaHelper.GetResource(count.Measure);
					QueueQuotaLoggingContext queueQuotaLoggingContext = new QueueQuotaLoggingContext(count.Entity, resource, this.thresholdFetcher);
					this.flowControlLog.LogSummaryWarning(queueQuotaLoggingContext.Resource, ThrottlingAction.TempReject, queueQuotaLoggingContext.WarningThreshold, TimeSpan.Zero, queueQuotaLoggingContext.Scope, queueQuotaLoggingContext.OrgId, queueQuotaLoggingContext.Sender, null, null, ThrottlingSource.QueueQuota, !this.config.EnforceQuota, new List<KeyValuePair<string, object>>
					{
						new KeyValuePair<string, object>("AvailableCapacity", this.thresholdFetcher.GetAvailableResourceSize(resource)),
						new KeyValuePair<string, object>("observedValue", (int)count.Total),
						new KeyValuePair<string, object>("ScopeValue", queueQuotaLoggingContext.ScopeValue)
					});
				}
			}
		}

		private void UpdateOldestThrottledPerfCounter(IEnumerable<QueueQuotaDiagnosticLogger.EntityValue> entityValues)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			DateTime dateTime = this.currentTimeProvider();
			foreach (QueueQuotaDiagnosticLogger.EntityValue entityValue in from e in entityValues
			where e.ThrottledTime != DateTime.MaxValue
			orderby e.ThrottledTime
			select e)
			{
				if (QueueQuotaHelper.ShouldProcessEntity(entityValue.Entity))
				{
					QueueQuotaLoggingContext queueQuotaLoggingContext = new QueueQuotaLoggingContext(entityValue.Entity, entityValue.Resource, this.thresholdFetcher);
					if (queueQuotaLoggingContext.Scope == ThrottlingScope.Tenant)
					{
						flag6 = this.CheckOldestOrg(entityValue, queueQuotaLoggingContext.OrgId, dateTime, ref flag, ref flag3, ref flag4);
					}
					else if (!flag2 && queueQuotaLoggingContext.Scope == ThrottlingScope.Sender && queueQuotaLoggingContext.OrgId != MultiTenantTransport.SafeTenantId)
					{
						this.perfCounters.UpdateOldestThrottledEntity(QueueQuotaEntity.Sender, dateTime - entityValue.ThrottledTime, queueQuotaLoggingContext.OrgId);
						flag2 = true;
					}
					else if (!flag5 && queueQuotaLoggingContext.Scope == ThrottlingScope.AccountForest)
					{
						this.perfCounters.UpdateOldestThrottledEntity(QueueQuotaEntity.AccountForest, dateTime - entityValue.ThrottledTime, queueQuotaLoggingContext.OrgId);
						flag5 = true;
					}
					if (flag6 && flag2 && flag5)
					{
						break;
					}
				}
			}
			this.ResetOldestEntity(!flag, !flag2, !flag3, !flag4, !flag5);
		}

		private bool CheckOldestOrg(QueueQuotaDiagnosticLogger.EntityValue value, Guid orgId, DateTime currentTime, ref bool orgFound, ref bool safeTenantFound, ref bool outlookTenantFound)
		{
			if (orgFound && safeTenantFound && outlookTenantFound)
			{
				return true;
			}
			if (!safeTenantFound && orgId == MultiTenantTransport.SafeTenantId)
			{
				this.perfCounters.UpdateOldestThrottledEntity(QueueQuotaEntity.Organization, currentTime - value.ThrottledTime, orgId);
				safeTenantFound = true;
			}
			else if (!outlookTenantFound && orgId == TemplateTenantConfiguration.TemplateTenantExternalDirectoryOrganizationIdGuid)
			{
				this.perfCounters.UpdateOldestThrottledEntity(QueueQuotaEntity.Organization, currentTime - value.ThrottledTime, orgId);
				outlookTenantFound = true;
			}
			else if (!orgFound)
			{
				this.perfCounters.UpdateOldestThrottledEntity(QueueQuotaEntity.Organization, currentTime - value.ThrottledTime, orgId);
				orgFound = true;
			}
			return false;
		}

		private void ResetOldestEntity(bool resetOrg, bool resetSender, bool resetSafeTenant, bool resetOutlookTenant, bool resetAccountForest)
		{
			if (resetOrg)
			{
				this.perfCounters.UpdateOldestThrottledEntity(QueueQuotaEntity.Organization, TimeSpan.Zero, Guid.Empty);
			}
			if (resetSender)
			{
				this.perfCounters.UpdateOldestThrottledEntity(QueueQuotaEntity.Sender, TimeSpan.Zero, Guid.Empty);
			}
			if (resetSafeTenant)
			{
				this.perfCounters.UpdateOldestThrottledEntity(QueueQuotaEntity.Organization, TimeSpan.Zero, MultiTenantTransport.SafeTenantId);
			}
			if (resetOutlookTenant)
			{
				this.perfCounters.UpdateOldestThrottledEntity(QueueQuotaEntity.Organization, TimeSpan.Zero, TemplateTenantConfiguration.TemplateTenantExternalDirectoryOrganizationIdGuid);
			}
			if (resetAccountForest)
			{
				this.perfCounters.UpdateOldestThrottledEntity(QueueQuotaEntity.AccountForest, TimeSpan.Zero, Guid.Empty);
			}
		}

		private bool TryGetUsageElement(string elementName, QueueQuotaResources resource, ICount<MeteredEntity, MeteredCount> count, out XElement element)
		{
			bool result = true;
			long sum = QueueQuotaHelper.GetSum(this.metering.GetUsage(count.Entity, QueueQuotaHelper.GetAllRejectedMeters(resource)).Values);
			DateTime value;
			bool flag = QueueQuotaHelper.HasThrottledTime(count, out value);
			if (!flag && count.Total == 0L && sum == 0L)
			{
				result = false;
			}
			element = new XElement(elementName);
			element.SetAttributeValue("Id", count.Entity.Name.Value);
			element.SetAttributeValue("QueueUsage", count.Total);
			element.SetAttributeValue("IsOverQuota", flag);
			element.SetAttributeValue("RejectedCount", sum);
			if (flag)
			{
				element.SetAttributeValue("ThrottlingDuration", this.currentTimeProvider().Subtract(value));
			}
			return result;
		}

		private readonly IQueueQuotaConfig config;

		private readonly IFlowControlLog flowControlLog;

		private readonly ICountTracker<MeteredEntity, MeteredCount> metering;

		private readonly ICountedEntity<MeteredEntity> totalEntity;

		private readonly IQueueQuotaComponentPerformanceCounters perfCounters;

		private readonly IQueueQuotaThresholdFetcher thresholdFetcher;

		private readonly Func<DateTime> currentTimeProvider;

		private class EntityValue
		{
			public EntityValue(ICountedEntity<MeteredEntity> entity, QueueQuotaResources resource, params ICount<MeteredEntity, MeteredCount>[] count)
			{
				this.entity = entity;
				this.resource = resource;
				if (count.Length > 0)
				{
					this.counts = (from c in count
					where c != null
					select c).ToDictionary((ICount<MeteredEntity, MeteredCount> c) => c.Measure);
				}
			}

			public ICountedEntity<MeteredEntity> Entity
			{
				get
				{
					return this.entity;
				}
			}

			public QueueQuotaResources Resource
			{
				get
				{
					return this.resource;
				}
			}

			public DateTime ThrottledTime
			{
				get
				{
					if (this.throttledTime != DateTime.MaxValue)
					{
						return this.throttledTime;
					}
					foreach (MeteredCount key in QueueQuotaHelper.GetMeteredCount(this.resource))
					{
						ICount<MeteredEntity, MeteredCount> count;
						DateTime result;
						if (this.counts.TryGetValue(key, out count) && QueueQuotaHelper.HasThrottledTime(count, out result))
						{
							this.throttledTime = result;
							return result;
						}
					}
					return DateTime.MaxValue;
				}
			}

			public long GetRejectedSum()
			{
				return this.AddTotal(QueueQuotaHelper.GetAllRejectedMeters(this.resource));
			}

			public long GetUsageSum()
			{
				return this.AddTotal(QueueQuotaHelper.GetMeteredCount(this.resource));
			}

			private long AddTotal(IEnumerable<MeteredCount> meters)
			{
				long num = 0L;
				foreach (MeteredCount key in meters)
				{
					ICount<MeteredEntity, MeteredCount> count;
					if (this.counts.TryGetValue(key, out count))
					{
						num += count.Total;
					}
				}
				return num;
			}

			private readonly ICountedEntity<MeteredEntity> entity;

			private readonly QueueQuotaResources resource;

			private Dictionary<MeteredCount, ICount<MeteredEntity, MeteredCount>> counts;

			private DateTime throttledTime = DateTime.MaxValue;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Metering;
using Microsoft.Exchange.Data.Metering.Throttling;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Transport
{
	internal static class QueueQuotaHelper
	{
		internal static ICountedEntity<MeteredEntity> CreateEntity(string accountForest)
		{
			return new CountedEntity<MeteredEntity>(new SimpleEntityName<MeteredEntity>(MeteredEntity.AccountForest, accountForest), new SimpleEntityName<MeteredEntity>(MeteredEntity.AccountForest, accountForest));
		}

		internal static ICountedEntity<MeteredEntity> CreateEntity(Guid orgId)
		{
			return new CountedEntity<MeteredEntity>(new SimpleEntityName<MeteredEntity>(MeteredEntity.Tenant, orgId.ToString()), new SimpleEntityName<MeteredEntity>(MeteredEntity.Tenant, orgId.ToString()));
		}

		internal static ICountedEntity<MeteredEntity> CreateEntity(Guid orgId, string sender)
		{
			return new CountedEntity<MeteredEntity>(new SimpleEntityName<MeteredEntity>(MeteredEntity.Tenant, orgId.ToString()), new SimpleEntityName<MeteredEntity>(MeteredEntity.Sender, sender));
		}

		internal static QueueQuotaEntity GetQueueQuotaEntity(MeteredEntity meteredEntity)
		{
			if (meteredEntity == MeteredEntity.Sender)
			{
				return QueueQuotaEntity.Sender;
			}
			if (meteredEntity == MeteredEntity.Tenant)
			{
				return QueueQuotaEntity.Organization;
			}
			throw new InvalidOperationException(string.Format("Got an unexpected entity back: {0}", meteredEntity));
		}

		internal static QueueQuotaResources GetResource(MeteredCount measure)
		{
			switch (measure)
			{
			case MeteredCount.AllQueue:
				return QueueQuotaResources.All;
			case MeteredCount.AcceptedSubmissionQueue:
				return QueueQuotaResources.SubmissionQueueSize;
			case MeteredCount.AcceptedTotalQueue:
				return QueueQuotaResources.TotalQueueSize;
			default:
				throw new InvalidOperationException(string.Format("Returned a measure that was not requested: {0}", measure));
			}
		}

		internal static IEnumerable<MeteredCount> GetMeteredCount(QueueQuotaResources resource)
		{
			List<MeteredCount> list = new List<MeteredCount>();
			if ((byte)(resource & QueueQuotaResources.SubmissionQueueSize) != 0)
			{
				list.Add(MeteredCount.AcceptedSubmissionQueue);
			}
			if ((byte)(resource & QueueQuotaResources.TotalQueueSize) != 0)
			{
				list.Add(MeteredCount.AcceptedTotalQueue);
			}
			return list;
		}

		internal static MeteredCount GetRejectedMeter(MeteredCount measure)
		{
			switch (measure)
			{
			case MeteredCount.AcceptedSubmissionQueue:
				return MeteredCount.CurrentRejectedSubmissionQueue;
			case MeteredCount.AcceptedTotalQueue:
				return MeteredCount.CurrentRejectedTotalQueue;
			case MeteredCount.CurrentRejectedSubmissionQueue:
				return MeteredCount.RejectedSubmissionQueue;
			case MeteredCount.CurrentRejectedTotalQueue:
				return MeteredCount.RejectedTotalQueue;
			default:
				throw new InvalidOperationException(string.Format("Unexpected measure: {0}", measure));
			}
		}

		internal static MeteredCount GetRejectedMeter(QueueQuotaResources resource)
		{
			switch (resource)
			{
			case QueueQuotaResources.SubmissionQueueSize:
				return MeteredCount.CurrentRejectedSubmissionQueue;
			case QueueQuotaResources.TotalQueueSize:
				return MeteredCount.CurrentRejectedTotalQueue;
			default:
				throw new InvalidOperationException(string.Format("Unexpected resouce: {0}", resource));
			}
		}

		internal static MeteredCount[] GetAllRejectedMeters(QueueQuotaResources resource)
		{
			MeteredCount rejectedMeter = QueueQuotaHelper.GetRejectedMeter(resource);
			return new MeteredCount[]
			{
				rejectedMeter,
				QueueQuotaHelper.GetRejectedMeter(rejectedMeter)
			};
		}

		internal static string GetSender(QueueQuotaEntity entityType, string entityId)
		{
			if (entityType != QueueQuotaEntity.Sender)
			{
				return null;
			}
			return entityId;
		}

		internal static string GetRedactedSender(QueueQuotaEntity entityType, string entityId)
		{
			string result = null;
			if (QueueQuotaHelper.GetSender(entityType, entityId) != null)
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

		internal static Guid GetOrgId(QueueQuotaEntity entityType, Guid externalOrganizationId)
		{
			if (entityType == QueueQuotaEntity.Organization || entityType == QueueQuotaEntity.Sender)
			{
				return externalOrganizationId;
			}
			return Guid.Empty;
		}

		internal static ThrottlingScope GetThrottlingScope(QueueQuotaEntity entityType)
		{
			switch (entityType)
			{
			case QueueQuotaEntity.Organization:
				return ThrottlingScope.Tenant;
			case QueueQuotaEntity.Sender:
				return ThrottlingScope.Sender;
			case QueueQuotaEntity.AccountForest:
				return ThrottlingScope.AccountForest;
			default:
				throw new InvalidOperationException(string.Format("Unexpected QueueQuotaEntity found: {0}", entityType));
			}
		}

		internal static ThrottlingResource GetThrottlingResource(QueueQuotaResources resource)
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

		internal static long GetSum(IEnumerable<ICount<MeteredEntity, MeteredCount>> counts)
		{
			long num = 0L;
			foreach (ICount<MeteredEntity, MeteredCount> count in counts)
			{
				num += count.Total;
			}
			return num;
		}

		internal static bool IsAnyThrottled(IDictionary<MeteredCount, ICount<MeteredEntity, MeteredCount>> counts)
		{
			return counts.Values.Any(delegate(ICount<MeteredEntity, MeteredCount> c)
			{
				DateTime dateTime;
				return QueueQuotaHelper.HasThrottledTime(c, out dateTime);
			});
		}

		internal static bool HasThrottledTime(ICount<MeteredEntity, MeteredCount> count, out DateTime throttledTime)
		{
			object obj;
			if (count.TryGetObject("ThrottledStartTime", out obj) && obj is DateTime)
			{
				throttledTime = (DateTime)obj;
				return true;
			}
			throttledTime = DateTime.MaxValue;
			return false;
		}

		internal static bool IsWarningLogged(ICount<MeteredEntity, MeteredCount> count)
		{
			object obj;
			return count.TryGetObject("WarningLogged", out obj) && obj is bool && (bool)obj;
		}

		internal static bool ShouldProcessEntity(ICountedEntity<MeteredEntity> entity)
		{
			return QueueQuotaHelper.IsOrg(entity) || QueueQuotaHelper.IsSender(entity) || QueueQuotaHelper.IsAccountForest(entity);
		}

		internal static bool IsOrg(ICountedEntity<MeteredEntity> entity, out Guid orgId)
		{
			orgId = Guid.Empty;
			if (!QueueQuotaHelper.IsOrg(entity))
			{
				return false;
			}
			Guid.TryParse(entity.Name.Value, out orgId);
			return true;
		}

		internal static bool IsOrg(ICountedEntity<MeteredEntity> entity)
		{
			return entity.Name.Type == MeteredEntity.Tenant;
		}

		internal static bool IsSender(ICountedEntity<MeteredEntity> entity, out Guid orgId)
		{
			orgId = Guid.Empty;
			if (!QueueQuotaHelper.IsSender(entity))
			{
				return false;
			}
			Guid.TryParse(entity.GroupName.Value, out orgId);
			return true;
		}

		internal static bool IsSender(ICountedEntity<MeteredEntity> entity)
		{
			return entity.Name.Type == MeteredEntity.Sender;
		}

		internal static bool IsAccountForest(ICountedEntity<MeteredEntity> entity)
		{
			return entity.Name.Type == MeteredEntity.AccountForest;
		}

		internal static bool IsQueueQuotaAcceptedCount(ICount<MeteredEntity, MeteredCount> count)
		{
			return (count.Entity.Name.Type == MeteredEntity.Tenant || count.Entity.Name.Type == MeteredEntity.Sender) && QueueQuotaHelper.AllAcceptedCounts.Contains(count.Measure);
		}

		internal static int GetResourceCapacity(QueueQuotaResources resource, IQueueQuotaConfig config)
		{
			switch (resource)
			{
			case QueueQuotaResources.SubmissionQueueSize:
				return config.SubmissionQueueCapacity;
			case QueueQuotaResources.TotalQueueSize:
				return config.TotalQueueCapacity;
			default:
				throw new ArgumentOutOfRangeException("resource");
			}
		}

		internal const string AvailableCapacityProperty = "AvailableCapacity";

		internal const string ThrottledStartTimeProperty = "ThrottledStartTime";

		internal const string WarningLoggedProperty = "WarningLogged";

		internal static readonly QueueQuotaResources[] AllResources = new QueueQuotaResources[]
		{
			QueueQuotaResources.SubmissionQueueSize,
			QueueQuotaResources.TotalQueueSize
		};

		internal static readonly MeteredCount[] AllAcceptedCounts = new MeteredCount[]
		{
			MeteredCount.AcceptedSubmissionQueue,
			MeteredCount.AcceptedTotalQueue
		};
	}
}

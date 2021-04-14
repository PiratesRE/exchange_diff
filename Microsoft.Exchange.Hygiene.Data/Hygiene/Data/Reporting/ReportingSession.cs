using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Reporting;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Hygiene.Data.DataProvider;

namespace Microsoft.Exchange.Hygiene.Data.Reporting
{
	internal sealed class ReportingSession : HygieneSession, ITenantThrottlingSession
	{
		public ReportingSession()
		{
			this.DataProvider = ConfigDataProviderFactory.Default.Create(DatabaseType.Mtrt);
		}

		public static Guid GenerateNewId()
		{
			return CombGuidGenerator.NewGuid();
		}

		public IPagedReader<AggOutboundIpHistory> FindPagedOutboundTenantHistory(int lastNMinutes, int perTenantMinimumEmailThreshold, int pageSize = 1000)
		{
			return this.GetPagedReader<AggOutboundIpHistory>(QueryFilter.AndTogether(new QueryFilter[]
			{
				ReportingSession.NewPropertyFilter(AggOutboundIPSchema.LastNMinutesQueryProperty, lastNMinutes),
				ReportingSession.NewPropertyFilter(AggOutboundIPSchema.MinimumEmailThresholdQueryProperty, perTenantMinimumEmailThreshold),
				ReportingSession.NewPropertyFilter(AggOutboundIPSchema.PageSizeQueryProperty, pageSize)
			}), pageSize);
		}

		public IPagedReader<AggOutboundEmailAddressIpHistory> FindPagedOutboundStatsGroupByIPEmailAddress(int lastNMinutes, int perTenantPerAddressMinimumEmailThreshold, int pageSize = 1000, bool summaryOnly = true)
		{
			return this.GetPagedReader<AggOutboundEmailAddressIpHistory>(QueryFilter.AndTogether(new QueryFilter[]
			{
				ReportingSession.NewPropertyFilter(AggOutboundIPSchema.LastNMinutesQueryProperty, lastNMinutes),
				ReportingSession.NewPropertyFilter(AggOutboundIPSchema.MinimumEmailThresholdQueryProperty, perTenantPerAddressMinimumEmailThreshold),
				ReportingSession.NewPropertyFilter(AggOutboundIPSchema.PageSizeQueryProperty, pageSize),
				ReportingSession.NewPropertyFilter(AggOutboundIPSchema.SummaryOnlyQueryProperty, summaryOnly)
			}), pageSize);
		}

		public IPagedReader<AggOutboundEmailAddressIpHistory> FindPagedOutboundHistoricalStatsEmailAddress(int lastNMinutes, Guid tenantId, string emailAddress, int pageSize = 1000)
		{
			return new ConfigDataProviderPagedReader<AggOutboundEmailAddressIpHistory>(this.DataProvider, null, QueryFilter.AndTogether(new QueryFilter[]
			{
				ReportingSession.NewPropertyFilter(AggOutboundIPSchema.LastNMinutesQueryProperty, lastNMinutes),
				ReportingSession.NewPropertyFilter(AggOutboundIPSchema.TenantIdProperty, tenantId),
				ReportingSession.NewPropertyFilter(AggOutboundIPSchema.FromEmailAddressProperty, emailAddress),
				ReportingSession.NewPropertyFilter(AggOutboundIPSchema.PageSizeQueryProperty, pageSize)
			}), null, pageSize);
		}

		public IPagedReader<AggInboundSpamDataHistory> FindPagedInboundSpamIPData(int lastNMinutes, double spamPercentageThreshold, int spamCountThreshold, int pageSize = 1000)
		{
			return this.GetPagedReader<AggInboundSpamDataHistory>(QueryFilter.AndTogether(new QueryFilter[]
			{
				ReportingSession.NewPropertyFilter(AggInboundIPSchema.LastNMinutesQueryProperty, lastNMinutes),
				ReportingSession.NewPropertyFilter(AggInboundIPSchema.MinimumSpamPercentageQueryProperty, spamPercentageThreshold),
				ReportingSession.NewPropertyFilter(AggInboundIPSchema.MinimumSpamCountQueryProperty, spamCountThreshold),
				ReportingSession.NewPropertyFilter(AggInboundIPSchema.PageSizeQueryProperty, pageSize)
			}), pageSize);
		}

		public IPagedReader<AggInboundSpamDataHistory> GetPagedInboundHistoricalIPData(int lastNMinutes, IPAddress startingIpAddress, IPAddress endIpAddress, int pageSize = 1000)
		{
			return this.GetPagedReader<AggInboundSpamDataHistory>(QueryFilter.AndTogether(new QueryFilter[]
			{
				ReportingSession.NewPropertyFilter(AggInboundIPSchema.LastNMinutesQueryProperty, lastNMinutes),
				ReportingSession.NewPropertyFilter(AggInboundIPSchema.StartingIPAddressQueryProperty, startingIpAddress),
				ReportingSession.NewPropertyFilter(AggInboundIPSchema.EndIPAddressQueryProperty, endIpAddress),
				ReportingSession.NewPropertyFilter(AggInboundIPSchema.PageSizeQueryProperty, pageSize)
			}), pageSize);
		}

		public bool CheckForGoodMailFromRange(int lastNMinutes, int goodMessageThreshold, IPAddress startingIpAddress, IPAddress endIpAddress)
		{
			List<GoodMessageData> source = this.DataProvider.Find<GoodMessageData>(QueryFilter.AndTogether(new QueryFilter[]
			{
				ReportingSession.NewPropertyFilter(GoodMessageSchema.LastNMinutesQueryProperty, lastNMinutes),
				ReportingSession.NewPropertyFilter(GoodMessageSchema.MinimumGoodMessageCountQueryProperty, goodMessageThreshold),
				ReportingSession.NewPropertyFilter(GoodMessageSchema.StartingIPAddressQueryProperty, startingIpAddress),
				ReportingSession.NewPropertyFilter(GoodMessageSchema.EndIPAddressQueryProperty, endIpAddress)
			}), null, true, null).Cast<GoodMessageData>().ToList<GoodMessageData>();
			return source.Any((GoodMessageData goodMsgData) => goodMsgData.GoodMessageExists);
		}

		public TransportProcessingQuotaConfig GetTransportThrottlingConfig()
		{
			string name = typeof(TransportProcessingQuotaConfig).Name;
			new ComparisonFilter(ComparisonOperator.Equal, TransportProcessingQuotaConfigSchema.SettingName, name);
			return this.DataProvider.Find<TransportProcessingQuotaConfig>(null, null, false, null).Cast<TransportProcessingQuotaConfig>().FirstOrDefault<TransportProcessingQuotaConfig>();
		}

		public void SetTransportThrottlingConfig(TransportProcessingQuotaConfig config)
		{
			this.DataProvider.Save(config);
		}

		public void SetThrottleState(TenantThrottleInfo throttleInfo)
		{
			if (throttleInfo == null)
			{
				throw new ArgumentNullException("throttleInfo");
			}
			this.SaveTenantThrottleInfo(new List<TenantThrottleInfo>
			{
				throttleInfo
			}, 0);
		}

		public TenantThrottleInfo GetThrottleState(Guid tenantId)
		{
			int partitionId = 0;
			bool overriddenOnly = true;
			List<TenantThrottleInfo> tenantThrottlingDigest = this.GetTenantThrottlingDigest(partitionId, new Guid?(tenantId), overriddenOnly, 50, false);
			if (tenantThrottlingDigest == null || tenantThrottlingDigest.Count == 0)
			{
				return null;
			}
			return tenantThrottlingDigest[0];
		}

		public void SaveTenantThrottleInfo(List<TenantThrottleInfo> throttleInfoList, int partitionId = 0)
		{
			if (throttleInfoList == null || throttleInfoList.Count == 0)
			{
				throw new ArgumentException("throttleInfo");
			}
			int physicalPartitionCopyCount = this.GetPhysicalPartitionCopyCount(partitionId);
			List<TransientDALException> list = null;
			for (int j = 0; j < physicalPartitionCopyCount; j++)
			{
				try
				{
					TenantThrottleInfoBatch batch = new TenantThrottleInfoBatch
					{
						PartitionId = partitionId,
						FssCopyId = j
					};
					throttleInfoList.ForEach(delegate(TenantThrottleInfo i)
					{
						batch.TenantThrottleInfoList.Add(i);
					});
					this.DataProvider.Save(batch);
				}
				catch (TransientDALException item)
				{
					if (list == null)
					{
						list = new List<TransientDALException>();
					}
					list.Add(item);
				}
			}
			if (list != null && list.Count == physicalPartitionCopyCount)
			{
				throw new AggregateException(list.ToArray());
			}
		}

		public List<TenantThrottleInfo> GetTenantThrottlingDigest(int partitionId = 0, Guid? tenantId = null, bool overriddenOnly = false, int tenantCount = 50, bool throttledOnly = true)
		{
			return this.DataProvider.Find<TenantThrottleInfo>(QueryFilter.AndTogether(new QueryFilter[]
			{
				ReportingSession.NewPropertyFilter(DalHelper.PhysicalInstanceKeyProp, partitionId),
				ReportingSession.NewPropertyFilter(ReportingCommonSchema.OrganizationalUnitRootProperty, tenantId),
				ReportingSession.NewPropertyFilter(ReportingCommonSchema.OverriddenOnlyProperty, overriddenOnly),
				ReportingSession.NewPropertyFilter(ReportingCommonSchema.DataCountProperty, tenantCount),
				ReportingSession.NewPropertyFilter(ReportingCommonSchema.ThrottledOnlyProperty, throttledOnly)
			}), null, true, null).Cast<TenantThrottleInfo>().ToList<TenantThrottleInfo>();
		}

		public int GetPhysicalPartitionCount()
		{
			IPartitionedDataProvider partitionedDataProvider = this.DataProvider as IPartitionedDataProvider;
			if (partitionedDataProvider == null)
			{
				throw new NotSupportedException("GetPhysicalPartitionCount may not be called from an environment that does not use a partitioned data provider.");
			}
			return partitionedDataProvider.GetNumberOfPhysicalPartitions();
		}

		private static ComparisonFilter NewPropertyFilter(PropertyDefinition property, object propertyValue)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, property, propertyValue);
		}

		private IPagedReader<T> GetPagedReader<T>(QueryFilter queryFilter, int pageSize) where T : IConfigurable, new()
		{
			List<IPagedReader<T>> list = new List<IPagedReader<T>>();
			foreach (object propertyValue in ((IPartitionedDataProvider)this.DataProvider).GetAllPhysicalPartitions())
			{
				list.Add(new ConfigDataProviderPagedReader<T>(this.DataProvider, null, QueryFilter.AndTogether(new QueryFilter[]
				{
					ReportingSession.NewPropertyFilter(DalHelper.PhysicalInstanceKeyProp, propertyValue),
					queryFilter
				}), null, pageSize));
			}
			return new CompositePagedReader<T>(list.ToArray());
		}

		private int GetPhysicalPartitionCopyCount(int physicalInstanceId)
		{
			IPartitionedDataProvider partitionedDataProvider = this.DataProvider as IPartitionedDataProvider;
			if (partitionedDataProvider == null)
			{
				throw new NotSupportedException("GetPhysicalPartitionCopyCount may not be called from an environment that does not use a partitioned data provider.");
			}
			return partitionedDataProvider.GetNumberOfPersistentCopiesPerPartition(physicalInstanceId);
		}

		internal readonly IConfigDataProvider DataProvider;
	}
}

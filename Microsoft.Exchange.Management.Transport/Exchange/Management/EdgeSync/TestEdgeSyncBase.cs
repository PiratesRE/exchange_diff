using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync;
using Microsoft.Exchange.EdgeSync.Common.Internal;
using Microsoft.Exchange.EdgeSync.Datacenter;
using Microsoft.Exchange.EdgeSync.Validation;
using Microsoft.Exchange.MessageSecurity.EdgeSync;
using Microsoft.Exchange.Monitoring;

namespace Microsoft.Exchange.Management.EdgeSync
{
	public abstract class TestEdgeSyncBase : Task
	{
		[Parameter(Mandatory = false)]
		public Fqdn DomainController
		{
			get
			{
				return (Fqdn)base.Fields["DomainController"];
			}
			set
			{
				base.Fields["DomainController"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Health")]
		public bool MonitoringContext
		{
			get
			{
				return (bool)(base.Fields["MonitoringContext"] ?? false);
			}
			set
			{
				base.Fields["MonitoringContext"] = value;
			}
		}

		protected abstract string CmdletMonitoringEventSource { get; }

		protected abstract string Service { get; }

		internal abstract bool ReadConnectorLeasePath(IConfigurationSession session, ADObjectId rootId, out string primaryLeasePath, out string backupLeasePath, out bool hasOneConnectorEnabledInCurrentForest);

		internal abstract ADObjectId GetCookieContainerId(IConfigurationSession session);

		protected abstract EnhancedTimeSpan GetSyncInterval(EdgeSyncServiceConfig config);

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || DataAccessHelper.IsDataAccessKnownException(exception) || MonitoringHelper.IsKnownExceptionForMonitoring(exception);
		}

		protected void TestGeneralSyncHealth()
		{
			try
			{
				EdgeSyncRecord edgeSyncRecord = this.TestSyncHealth(this.DomainController);
				if (this.MonitoringContext)
				{
					this.ReportMomStatus(edgeSyncRecord);
				}
				base.WriteObject(edgeSyncRecord);
			}
			catch (TransientException exception)
			{
				this.WriteErrorAndMonitoringEvent(exception, ExchangeErrorCategory.ServerOperation, null, 1003, this.CmdletMonitoringEventSource);
			}
			catch (ADOperationException exception2)
			{
				this.WriteErrorAndMonitoringEvent(exception2, ExchangeErrorCategory.ServerOperation, null, 1003, this.CmdletMonitoringEventSource);
			}
			catch (IOException exception3)
			{
				this.WriteErrorAndMonitoringEvent(exception3, ExchangeErrorCategory.ServerOperation, null, 1003, this.CmdletMonitoringEventSource);
			}
			finally
			{
				if (this.MonitoringContext)
				{
					base.WriteObject(this.monitoringData);
				}
			}
		}

		private static LeaseToken GetLease(string primaryLeaseFilePath, string backupLeaseFilePath, out string additionalInfo)
		{
			additionalInfo = null;
			FileLeaseManager.LeaseOperationResult leaseOperationResult = FileLeaseManager.TryRunLeaseOperation(new FileLeaseManager.LeaseOperation(FileLeaseManager.GetLeaseOperation), new FileLeaseManager.LeaseOperationRequest(primaryLeaseFilePath));
			if (leaseOperationResult.Succeeded)
			{
				return leaseOperationResult.ResultToken;
			}
			Exception exception = leaseOperationResult.Exception;
			leaseOperationResult = FileLeaseManager.TryRunLeaseOperation(new FileLeaseManager.LeaseOperation(FileLeaseManager.GetLeaseOperation), new FileLeaseManager.LeaseOperationRequest(backupLeaseFilePath));
			if (leaseOperationResult.Succeeded)
			{
				return leaseOperationResult.ResultToken;
			}
			additionalInfo = string.Format("PrimaryLeaseException:{0}\n\nBackupLeaseException:{1}", exception, leaseOperationResult.Exception);
			return LeaseToken.Empty;
		}

		private bool TryReadCookie(IConfigurationSession session, out Cookie cookie)
		{
			ADObjectId cookieContainerId = null;
			Container cookieContainer = null;
			cookie = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				cookieContainerId = this.GetCookieContainerId(session);
				cookieContainer = session.Read<Container>(cookieContainerId);
			}, 3);
			if (adoperationResult.Succeeded)
			{
				using (MultiValuedProperty<byte[]>.Enumerator enumerator = cookieContainer.EdgeSyncCookies.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						byte[] bytes = enumerator.Current;
						cookie = Cookie.Deserialize(Encoding.ASCII.GetString(bytes));
					}
				}
			}
			return adoperationResult.Succeeded;
		}

		internal T FindSiteEdgeSyncConnector<T>(IConfigurationSession session, ADObjectId siteId, out bool hasOneConnectorEnabledInCurrentForest) where T : EdgeSyncConnector, new()
		{
			List<T> connectors = new List<T>();
			hasOneConnectorEnabledInCurrentForest = true;
			ADNotificationAdapter.ReadConfigurationPaged<T>(() => session.FindPaged<T>(siteId, QueryScope.SubTree, null, null, 0), delegate(T connector)
			{
				if (connector.Enabled)
				{
					connectors.Add(connector);
				}
			}, 3);
			if (connectors.Count == 0)
			{
				ADNotificationAdapter.ReadConfigurationPaged<T>(() => session.FindPaged<T>(null, QueryScope.SubTree, null, null, 0), delegate(T connector)
				{
					if (connector.Enabled)
					{
						connectors.Add(connector);
					}
				}, 3);
				hasOneConnectorEnabledInCurrentForest = (connectors.Count > 0);
				return default(T);
			}
			return connectors[0];
		}

		private EdgeSyncRecord TestSyncHealth(string domainController)
		{
			ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(domainController, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 383, "TestSyncHealth", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\EdgeSync\\TestEdgeSyncBase.cs");
			ADSite localSite = null;
			EdgeSyncServiceConfig config = null;
			ADNotificationAdapter.RunADOperation(delegate()
			{
				localSite = session.GetLocalSite();
				if (localSite == null)
				{
					throw new TransientException(Strings.CannotGetLocalSite);
				}
				config = session.Read<EdgeSyncServiceConfig>(localSite.Id.GetChildId("EdgeSyncService"));
			}, 3);
			if (config == null)
			{
				return EdgeSyncRecord.GetEdgeSyncServiceNotConfiguredForCurrentSiteRecord(this.Service, localSite.Name);
			}
			bool flag = false;
			string primaryLeaseFilePath;
			string backupLeaseFilePath;
			if (!this.ReadConnectorLeasePath(session, config.Id, out primaryLeaseFilePath, out backupLeaseFilePath, out flag))
			{
				if (!flag)
				{
					return EdgeSyncRecord.GetEdgeSyncConnectorNotConfiguredForEntireForestRecord(this.Service);
				}
				return EdgeSyncRecord.GetEdgeSyncConnectorNotConfiguredForCurrentSiteRecord(this.Service, localSite.Name);
			}
			else
			{
				string additionalInfo = null;
				LeaseToken lease = TestEdgeSyncBase.GetLease(primaryLeaseFilePath, backupLeaseFilePath, out additionalInfo);
				if (lease.NotHeld)
				{
					return EdgeSyncRecord.GetFailedRecord(this.Service, base.MyInvocation.MyCommand.Name, "There is no lease file detected. It suggests synchronization has not started at all.", lease, null, additionalInfo);
				}
				Cookie cookie = null;
				string text = null;
				if (!this.TryGetNewestCookieFromAllDomainControllers(out cookie, out text))
				{
					throw new InvalidOperationException("Failed accessing all DCs: " + text);
				}
				if (cookie == null)
				{
					return EdgeSyncRecord.GetFailedRecord(this.Service, base.MyInvocation.MyCommand.Name, "There is no cookie detected. It suggests we haven't had a single successful synchronization.", lease, null, text);
				}
				EnhancedTimeSpan syncInterval = this.GetSyncInterval(config);
				switch (lease.Type)
				{
				case LeaseTokenType.Lock:
					if (DateTime.UtcNow > lease.AlertTime)
					{
						return EdgeSyncRecord.GetFailedRecord(this.Service, base.MyInvocation.MyCommand.Name, "Synchronization has completely stopped because lock has expired. It suggests the EdgeSync service died in the middle of the synchronization and no other service instance has taken over.", lease, cookie, text, true);
					}
					if (DateTime.UtcNow > cookie.LastUpdated + config.OptionDuration + 3L * syncInterval + TimeSpan.FromHours(1.0))
					{
						return EdgeSyncRecord.GetFailedRecord(this.Service, base.MyInvocation.MyCommand.Name, "Cookie has not been updated as expected. It might be caused by failure to synchronize some items which means that the sychronization might still be running but not efficiently. It might also be caused by a long full sync. Check EdgeSync log for further troubleshooting.", lease, cookie, text);
					}
					return EdgeSyncRecord.GetInconclusiveRecord(this.Service, base.MyInvocation.MyCommand.Name, "Synchronization status is inconclusive because EdgeSync is in the middle of synchronizing data. Try running this cmdlet again later.", lease, cookie, text);
				case LeaseTokenType.Option:
					if (DateTime.UtcNow > lease.AlertTime)
					{
						return EdgeSyncRecord.GetFailedRecord(this.Service, base.MyInvocation.MyCommand.Name, "Synchronization has completely stopped. We have failed to failover to another instance within the same AD site or to another AD site.", lease, cookie, text, true);
					}
					if (DateTime.UtcNow > cookie.LastUpdated + config.FailoverDCInterval + TimeSpan.FromMinutes(30.0))
					{
						return EdgeSyncRecord.GetFailedRecord(this.Service, base.MyInvocation.MyCommand.Name, "Cookie has not been updated as expected. It might be caused by failure to synchronize some items which means that the sychronization might still be running but not efficiently. It might also be caused by a long full sync. Check EdgeSync log for further troubleshooting.", lease, cookie, text);
					}
					return EdgeSyncRecord.GetNormalRecord(this.Service, "The synchronization is operating normally.", lease, cookie, text);
				default:
					throw new ArgumentException("Unknown lease type: " + lease.Type);
				}
			}
		}

		private bool TryGetNewestCookieFromAllDomainControllers(out Cookie latestCookie, out string cookieTimeStampRecordInfo)
		{
			latestCookie = null;
			cookieTimeStampRecordInfo = null;
			bool result = false;
			ADForest localForest = ADForest.GetLocalForest();
			List<ADServer> list = localForest.FindAllGlobalCatalogsInLocalSite();
			if (list != null && list.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("List of cookie timestamp from all DCs:");
				foreach (ADServer adserver in list)
				{
					IConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(adserver.DnsHostName, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 593, "TryGetNewestCookieFromAllDomainControllers", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\EdgeSync\\TestEdgeSyncBase.cs");
					Cookie cookie;
					if (this.TryReadCookie(session, out cookie))
					{
						result = true;
						if (cookie != null)
						{
							if (latestCookie == null || cookie.LastUpdated > latestCookie.LastUpdated)
							{
								latestCookie = cookie;
							}
							stringBuilder.AppendFormat("TimeStamp:{0}, SessionDC:{1}, CookieDC:{2};\r\n", cookie.LastUpdated, adserver.Name, cookie.DomainController);
						}
						else
						{
							stringBuilder.AppendFormat("Cookie Value Not Found On DC {0};\r\n", adserver.Name);
						}
					}
					else
					{
						stringBuilder.AppendFormat("Failed Accessing Domain Controller {0};\r\n", adserver.Name);
					}
				}
				cookieTimeStampRecordInfo = stringBuilder.ToString();
			}
			return result;
		}

		private void ReportMomStatus(EdgeSyncRecord record)
		{
			int eventIdentifier = 1000;
			EventTypeEnumeration eventType = EventTypeEnumeration.Success;
			switch (record.Status)
			{
			case ValidationStatus.NoSyncConfigured:
				eventIdentifier = 1005;
				eventType = EventTypeEnumeration.Warning;
				break;
			case ValidationStatus.Warning:
				eventIdentifier = 1001;
				eventType = EventTypeEnumeration.Warning;
				break;
			case ValidationStatus.Failed:
				eventIdentifier = 1002;
				eventType = EventTypeEnumeration.Error;
				break;
			case ValidationStatus.Inconclusive:
				eventIdentifier = 1004;
				eventType = EventTypeEnumeration.Information;
				break;
			case ValidationStatus.FailedUrgent:
				eventIdentifier = 1006;
				eventType = EventTypeEnumeration.Error;
				break;
			}
			MonitoringEvent item = new MonitoringEvent(this.CmdletMonitoringEventSource, eventIdentifier, eventType, record.ToString());
			this.monitoringData.Events.Add(item);
		}

		private void WriteErrorAndMonitoringEvent(Exception exception, ExchangeErrorCategory errorCategory, object target, int eventId, string eventSource)
		{
			this.monitoringData.Events.Add(new MonitoringEvent(eventSource, eventId, EventTypeEnumeration.Error, exception.Message));
			base.WriteError(exception, (ErrorCategory)errorCategory, target);
		}

		protected const string ParameterSetValidateAddress = "ValidateAddress";

		protected const string ParameterSetHealth = "Health";

		private const string NoLease = "There is no lease file detected. It suggests synchronization has not started at all.";

		private const string NoCookie = "There is no cookie detected. It suggests we haven't had a single successful synchronization.";

		private const string LockExpired = "Synchronization has completely stopped because lock has expired. It suggests the EdgeSync service died in the middle of the synchronization and no other service instance has taken over.";

		private const string InterSiteFailoverFailed = "Synchronization has completely stopped. We have failed to failover to another instance within the same AD site or to another AD site.";

		private const string StatusInconclusive = "Synchronization status is inconclusive because EdgeSync is in the middle of synchronizing data. Try running this cmdlet again later.";

		private const string CookieNotUpdated = "Cookie has not been updated as expected. It might be caused by failure to synchronize some items which means that the sychronization might still be running but not efficiently. It might also be caused by a long full sync. Check EdgeSync log for further troubleshooting.";

		private const string NormalSync = "The synchronization is operating normally.";

		private MonitoringData monitoringData = new MonitoringData();

		private static class EventId
		{
			public const int SyncNormal = 1000;

			public const int SyncAbnormal = 1001;

			public const int SyncFailed = 1002;

			public const int UnableToTestSyncHealth = 1003;

			public const int InconclusiveTestSyncHealth = 1004;

			public const int SyncNotConfigured = 1005;

			public const int SyncFailedUrgent = 1006;
		}
	}
}

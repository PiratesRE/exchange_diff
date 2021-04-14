using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync;
using Microsoft.Exchange.EdgeSync.Validation;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessageSecurity.EdgeSync;
using Microsoft.Exchange.Monitoring;

namespace Microsoft.Exchange.Management.EdgeSync
{
	[Cmdlet("Test", "EdgeSynchronization", SupportsShouldProcess = true, DefaultParameterSetName = "Default")]
	public sealed class TestEdgeSynchronization : Task
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

		[Parameter(Mandatory = false, ParameterSetName = "Default")]
		public SwitchParameter FullCompareMode
		{
			internal get
			{
				return new SwitchParameter(this.fullCompareMode);
			}
			set
			{
				this.fullCompareMode = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Default")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Default")]
		public SwitchParameter ExcludeRecipientTest
		{
			get
			{
				return (SwitchParameter)(base.Fields["ExcludeRecipientTest"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ExcludeRecipientTest"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Default")]
		public Unlimited<uint> MaxReportSize
		{
			get
			{
				return (Unlimited<uint>)(base.Fields["MaxReportSize"] ?? new Unlimited<uint>(1000U));
			}
			set
			{
				base.Fields["MaxReportSize"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Default")]
		public string TargetServer
		{
			get
			{
				return (string)base.Fields["TargetServer"];
			}
			set
			{
				base.Fields["TargetServer"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "SingleValidation")]
		public ProxyAddress VerifyRecipient
		{
			get
			{
				return (ProxyAddress)base.Fields["VerifyRecipient"];
			}
			set
			{
				base.Fields["VerifyRecipient"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestEdgeSynchronization;
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			return base.IsKnownException(e) || MonitoringHelper.IsKnownExceptionForMonitoring(e);
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			this.monitoringData = new MonitoringData();
		}

		protected override void InternalValidate()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 262, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\EdgeSync\\TestEdgeSynchronization.cs");
			try
			{
				Server server = topologyConfigurationSession.FindLocalServer();
				if (!server.IsHubTransportServer)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorTaskRunningLocationHubOnly), ErrorCategory.InvalidOperation, null);
				}
			}
			catch (LocalServerNotFoundException)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorTaskRunningLocationHubOnly), ErrorCategory.InvalidOperation, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			Exception exception = null;
			if (!ReplicationTopology.TryLoadLocalSiteTopology(this.DomainController, out this.replicationTopology, out exception))
			{
				this.WriteErrorAndMonitoringEvent(exception, ExchangeErrorCategory.ServerOperation, null, 1002, "MSExchange Monitoring EdgeSynchronization");
				return;
			}
			if (this.replicationTopology.EdgeSyncServiceConfig == null)
			{
				this.ReportStatus(new EdgeSubscriptionStatus(string.Empty)
				{
					SyncStatus = ValidationStatus.NoSyncConfigured,
					FailureDetail = Strings.EdgeSyncServiceConfigMissing((this.replicationTopology.LocalSite != null) ? this.replicationTopology.LocalSite.Name : string.Empty)
				});
				return;
			}
			if (this.replicationTopology.SiteEdgeServers.Count == 0)
			{
				this.ReportStatus(new EdgeSubscriptionStatus(string.Empty)
				{
					SyncStatus = ValidationStatus.NoSyncConfigured,
					FailureDetail = Strings.NoSubscription((this.replicationTopology.LocalSite != null) ? this.replicationTopology.LocalSite.Name : string.Empty)
				});
				return;
			}
			List<EdgeSubscriptionStatus> list = new List<EdgeSubscriptionStatus>(this.replicationTopology.SiteEdgeServers.Count);
			foreach (Server server in this.replicationTopology.SiteEdgeServers)
			{
				if (string.IsNullOrEmpty(this.TargetServer) || this.TargetServer.Equals(server.Name, StringComparison.OrdinalIgnoreCase) || this.TargetServer.Equals(server.Fqdn))
				{
					EdgeSubscriptionStatus edgeSubscriptionStatus = new EdgeSubscriptionStatus(server.Name);
					EdgeConnectionInfo edgeConnectionInfo = new EdgeConnectionInfo(this.replicationTopology, server);
					if (edgeConnectionInfo.EdgeConnection == null)
					{
						goto IL_3D5;
					}
					edgeSubscriptionStatus.LeaseHolder = edgeConnectionInfo.LeaseHolder;
					edgeSubscriptionStatus.LeaseType = edgeConnectionInfo.LeaseType;
					edgeSubscriptionStatus.LeaseExpiryUtc = edgeConnectionInfo.LeaseExpiry;
					edgeSubscriptionStatus.LastSynchronizedUtc = edgeConnectionInfo.LastSynchronizedDate;
					edgeSubscriptionStatus.CredentialRecords.Records = CredentialRecordsLoader.Load(server);
					edgeSubscriptionStatus.CookieRecords.Load(edgeConnectionInfo.Cookies);
					if (this.VerifyRecipient != null)
					{
						RecipientValidator recipientValidator = new RecipientValidator(this.replicationTopology);
						edgeSubscriptionStatus.RecipientStatus = recipientValidator.ValidateOneRecipient(edgeConnectionInfo, this.VerifyRecipient.ProxyAddressString);
					}
					else if (this.InitializeSubscriptionStatus(edgeConnectionInfo, ref edgeSubscriptionStatus))
					{
						if (!(DateTime.UtcNow > edgeSubscriptionStatus.LeaseExpiryUtc + this.GetAlertPaddingTimeSpan()))
						{
							bool flag = false;
							if (edgeSubscriptionStatus.CookieRecords.Records.Count > 0)
							{
								using (MultiValuedProperty<CookieRecord>.Enumerator enumerator2 = edgeSubscriptionStatus.CookieRecords.Records.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										CookieRecord cookieRecord = enumerator2.Current;
										if (DateTime.UtcNow > cookieRecord.LastUpdated + this.GetAlertPaddingTimeSpan())
										{
											flag = true;
										}
									}
									goto IL_2EC;
								}
								goto IL_2E9;
							}
							goto IL_2E9;
							IL_2EC:
							if (!flag)
							{
								edgeSubscriptionStatus.SyncStatus = ValidationStatus.Normal;
								goto IL_313;
							}
							edgeSubscriptionStatus.SyncStatus = ValidationStatus.Failed;
							edgeSubscriptionStatus.FailureDetail = Strings.CookieNotUpdated;
							goto IL_313;
							IL_2E9:
							flag = true;
							goto IL_2EC;
						}
						edgeSubscriptionStatus.SyncStatus = ValidationStatus.Failed;
						edgeSubscriptionStatus.FailureDetail = Strings.LeaseExpired;
						IL_313:
						if (this.fullCompareMode)
						{
							try
							{
								this.LoadValidators();
								edgeSubscriptionStatus.TransportConfigStatus = this.transportConfigValidator.Validate(edgeConnectionInfo);
								edgeSubscriptionStatus.TransportServerStatus = this.transportServerValidator.Validate(edgeConnectionInfo);
								edgeSubscriptionStatus.AcceptedDomainStatus = this.acceptedDomainValidator.Validate(edgeConnectionInfo);
								edgeSubscriptionStatus.RemoteDomainStatus = this.remoteDomainValidator.Validate(edgeConnectionInfo);
								edgeSubscriptionStatus.MessageClassificationStatus = this.messageClassificationValidator.Validate(edgeConnectionInfo);
								edgeSubscriptionStatus.SendConnectorStatus = this.sendConnectorValidator.Validate(edgeConnectionInfo);
								if (!this.ExcludeRecipientTest.IsPresent)
								{
									edgeSubscriptionStatus.RecipientStatus = this.recipientValidator.Validate(edgeConnectionInfo);
								}
								goto IL_3F5;
							}
							catch (ExDirectoryException ex)
							{
								edgeSubscriptionStatus.FailureDetail = ex.Message;
								goto IL_3F5;
							}
							goto IL_3D5;
						}
					}
					IL_3F5:
					base.WriteObject(edgeSubscriptionStatus);
					list.Add(edgeSubscriptionStatus);
					continue;
					IL_3D5:
					edgeSubscriptionStatus.SyncStatus = ValidationStatus.Failed;
					edgeSubscriptionStatus.FailureDetail = Strings.SubscriptionConnectionError(edgeConnectionInfo.FailureDetail);
					goto IL_3F5;
				}
			}
			if (this.MonitoringContext)
			{
				this.ReportMomStatus(list);
			}
			TaskLogger.LogExit();
		}

		private EnhancedTimeSpan GetAlertPaddingTimeSpan()
		{
			EnhancedTimeSpan t = (this.replicationTopology.EdgeSyncServiceConfig.ConfigurationSyncInterval > this.replicationTopology.EdgeSyncServiceConfig.RecipientSyncInterval) ? this.replicationTopology.EdgeSyncServiceConfig.ConfigurationSyncInterval : this.replicationTopology.EdgeSyncServiceConfig.RecipientSyncInterval;
			return 2L * t + TimeSpan.FromMinutes(30.0);
		}

		private void ReportMomStatus(List<EdgeSubscriptionStatus> statusList)
		{
			int num = 1000;
			EventTypeEnumeration eventType = EventTypeEnumeration.Success;
			StringBuilder stringBuilder = null;
			foreach (EdgeSubscriptionStatus edgeSubscriptionStatus in statusList)
			{
				switch (edgeSubscriptionStatus.SyncStatus)
				{
				case ValidationStatus.NoSyncConfigured:
					num = 1004;
					eventType = EventTypeEnumeration.Warning;
					break;
				case ValidationStatus.Failed:
					num = 1001;
					eventType = EventTypeEnumeration.Error;
					break;
				case ValidationStatus.Inconclusive:
					if (num == 1000)
					{
						num = 1003;
						eventType = EventTypeEnumeration.Information;
					}
					break;
				}
				if (stringBuilder == null)
				{
					string text = edgeSubscriptionStatus.ToStringForm();
					stringBuilder = new StringBuilder(text.Length * statusList.Count + 256);
					stringBuilder.Append(text);
				}
				else
				{
					stringBuilder.Append(edgeSubscriptionStatus.ToStringForm());
				}
			}
			MonitoringEvent item = new MonitoringEvent("MSExchange Monitoring EdgeSynchronization", num, eventType, stringBuilder.ToString());
			this.monitoringData.Events.Add(item);
			base.WriteObject(this.monitoringData);
		}

		private void WriteErrorAndMonitoringEvent(Exception exception, ExchangeErrorCategory errorCategory, object target, int eventId, string eventSource)
		{
			if (this.MonitoringContext)
			{
				this.monitoringData.Events.Add(new MonitoringEvent(eventSource, eventId, EventTypeEnumeration.Error, exception.Message));
				base.WriteObject(this.monitoringData);
			}
			base.WriteError(exception, (ErrorCategory)errorCategory, target);
		}

		private void ReportStatus(EdgeSubscriptionStatus subscriptionStatus)
		{
			base.WriteObject(subscriptionStatus);
			if (this.MonitoringContext)
			{
				this.ReportMomStatus(new List<EdgeSubscriptionStatus>(1)
				{
					subscriptionStatus
				});
			}
		}

		private void LoadValidators()
		{
			if (this.acceptedDomainValidator == null)
			{
				this.acceptedDomainValidator = new AcceptedDomainValidator(this.replicationTopology);
				this.acceptedDomainValidator.MaxReportedLength = this.MaxReportSize;
				this.acceptedDomainValidator.UseChangedDate = this.MonitoringContext;
				this.acceptedDomainValidator.ProgressMethod = new EdgeSyncValidator.ProgressUpdate(this.ProgressUpdate);
				this.acceptedDomainValidator.LoadValidationInfo();
			}
			if (this.remoteDomainValidator == null)
			{
				this.remoteDomainValidator = new RemoteDomainValidator(this.replicationTopology);
				this.remoteDomainValidator.MaxReportedLength = this.MaxReportSize;
				this.remoteDomainValidator.UseChangedDate = this.MonitoringContext;
				this.remoteDomainValidator.ProgressMethod = new EdgeSyncValidator.ProgressUpdate(this.ProgressUpdate);
				this.remoteDomainValidator.LoadValidationInfo();
			}
			if (this.messageClassificationValidator == null)
			{
				this.messageClassificationValidator = new MessageClassificationValidator(this.replicationTopology);
				this.messageClassificationValidator.MaxReportedLength = this.MaxReportSize;
				this.messageClassificationValidator.UseChangedDate = this.MonitoringContext;
				this.messageClassificationValidator.ProgressMethod = new EdgeSyncValidator.ProgressUpdate(this.ProgressUpdate);
				this.messageClassificationValidator.LoadValidationInfo();
			}
			if (this.sendConnectorValidator == null)
			{
				this.sendConnectorValidator = new SendConnectorValidator(this.replicationTopology);
				this.sendConnectorValidator.MaxReportedLength = this.MaxReportSize;
				this.sendConnectorValidator.UseChangedDate = this.MonitoringContext;
				this.sendConnectorValidator.ProgressMethod = new EdgeSyncValidator.ProgressUpdate(this.ProgressUpdate);
				this.sendConnectorValidator.LoadValidationInfo();
			}
			if (this.transportConfigValidator == null)
			{
				this.transportConfigValidator = new TransportConfigValidator(this.replicationTopology);
				this.transportConfigValidator.MaxReportedLength = this.MaxReportSize;
				this.transportConfigValidator.UseChangedDate = this.MonitoringContext;
				this.transportConfigValidator.ProgressMethod = new EdgeSyncValidator.ProgressUpdate(this.ProgressUpdate);
				this.transportConfigValidator.LoadValidationInfo();
			}
			if (this.transportServerValidator == null)
			{
				this.transportServerValidator = new TransportServerValidator(this.replicationTopology);
				this.transportServerValidator.MaxReportedLength = this.MaxReportSize;
				this.transportServerValidator.UseChangedDate = this.MonitoringContext;
				this.transportServerValidator.ProgressMethod = new EdgeSyncValidator.ProgressUpdate(this.ProgressUpdate);
				this.transportServerValidator.LoadValidationInfo();
			}
			if (this.recipientValidator == null && !this.ExcludeRecipientTest.IsPresent)
			{
				this.recipientValidator = new RecipientValidator(this.replicationTopology);
				this.recipientValidator.MaxReportedLength = this.MaxReportSize;
				this.recipientValidator.UseChangedDate = this.MonitoringContext;
				this.recipientValidator.ProgressMethod = new EdgeSyncValidator.ProgressUpdate(this.ProgressUpdate);
				this.recipientValidator.LoadValidationInfo();
			}
		}

		private bool InitializeSubscriptionStatus(EdgeConnectionInfo subscription, ref EdgeSubscriptionStatus status)
		{
			switch (subscription.LeaseType)
			{
			case LeaseTokenType.Lock:
				status.SyncStatus = ValidationStatus.Inconclusive;
				status.AcceptedDomainStatus.SyncStatus = SyncStatus.InProgress;
				status.RemoteDomainStatus.SyncStatus = SyncStatus.InProgress;
				status.MessageClassificationStatus.SyncStatus = SyncStatus.InProgress;
				status.RecipientStatus.SyncStatus = SyncStatus.InProgress;
				status.SendConnectorStatus.SyncStatus = SyncStatus.InProgress;
				status.TransportConfigStatus.SyncStatus = SyncStatus.InProgress;
				status.TransportServerStatus.SyncStatus = SyncStatus.InProgress;
				return false;
			case LeaseTokenType.None:
				status.SyncStatus = ValidationStatus.Inconclusive;
				status.AcceptedDomainStatus.SyncStatus = SyncStatus.NotStarted;
				status.RemoteDomainStatus.SyncStatus = SyncStatus.NotStarted;
				status.MessageClassificationStatus.SyncStatus = SyncStatus.NotStarted;
				status.RecipientStatus.SyncStatus = SyncStatus.NotStarted;
				status.SendConnectorStatus.SyncStatus = SyncStatus.NotStarted;
				status.TransportConfigStatus.SyncStatus = SyncStatus.NotStarted;
				status.TransportServerStatus.SyncStatus = SyncStatus.NotStarted;
				return false;
			}
			return true;
		}

		private void ProgressUpdate(LocalizedString title, LocalizedString updateDescription)
		{
			ExProgressRecord record = new ExProgressRecord(0, title, updateDescription);
			base.WriteProgress(record);
		}

		private const string CmdletNoun = "EdgeSynchronization";

		private const string CmdletMonitoringEventSource = "MSExchange Monitoring EdgeSynchronization";

		private const string ExcludeRecipientTestName = "ExcludeRecipientTest";

		private const string MaxReportSizeName = "MaxReportSize";

		private const string MonitoringContextName = "MonitoringContext";

		private const uint DefaultReportSize = 1000U;

		private MonitoringData monitoringData = new MonitoringData();

		private bool fullCompareMode;

		private ReplicationTopology replicationTopology;

		private TransportConfigValidator transportConfigValidator;

		private TransportServerValidator transportServerValidator;

		private SendConnectorValidator sendConnectorValidator;

		private RecipientValidator recipientValidator;

		private MessageClassificationValidator messageClassificationValidator;

		private AcceptedDomainValidator acceptedDomainValidator;

		private RemoteDomainValidator remoteDomainValidator;

		private static class ParameterSet
		{
			internal const string Default = "Default";

			internal const string SingleValidation = "SingleValidation";
		}

		private static class EventId
		{
			public const int SyncNormal = 1000;

			public const int SyncFailed = 1001;

			public const int UnableToTestSyncHealth = 1002;

			public const int InconclusiveTestSyncHealth = 1003;

			public const int SyncNotConfigured = 1004;
		}
	}
}

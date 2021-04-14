using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Storage;
using Microsoft.Exchange.Transport.Storage.Messaging;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class MailboxDeliveryInstanceAvailabilityProbe : SmtpConnectionProbe
	{
		public MailboxDeliveryInstanceAvailabilityProbe()
		{
			base.UseXmlConfiguration = false;
		}

		protected override bool DisconnectBetweenSessions
		{
			get
			{
				return true;
			}
		}

		protected StringBuilder ErrorList
		{
			get
			{
				return this.errorList;
			}
		}

		private protected override ISimpleSmtpClient Client
		{
			protected get
			{
				return base.Client;
			}
		}

		public static ProbeDefinition CreateMailboxDeliveryInstanceAvailabilityProbe(MailboxDatabaseInfo mailboxDatabase)
		{
			string text = string.Format("{0}@{1}", mailboxDatabase.MonitoringAccount, mailboxDatabase.MonitoringAccountDomain);
			Guid empty = Guid.Empty;
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && MultiTenantTransport.TryGetExternalOrgId(mailboxDatabase.MonitoringAccountOrganizationId, out empty) != ADOperationResult.Success)
			{
				throw new SmtpConnectionProbeException(string.Format("The external organization id was not found for {0}", text));
			}
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = typeof(MailboxDeliveryInstanceAvailabilityProbe).Assembly.Location;
			probeDefinition.TypeName = typeof(MailboxDeliveryInstanceAvailabilityProbe).FullName;
			probeDefinition.Name = "MailboxDeliveryInstanceAvailabilityProbe";
			probeDefinition.ServiceName = ExchangeComponent.MailboxTransport.Name;
			probeDefinition.RecurrenceIntervalSeconds = 120;
			probeDefinition.TimeoutSeconds = 90;
			probeDefinition.MaxRetryAttempts = 3;
			probeDefinition.TargetResource = mailboxDatabase.MailboxDatabaseName;
			probeDefinition.Enabled = true;
			probeDefinition.Attributes["MailboxDatabaseGuid"] = mailboxDatabase.MailboxDatabaseGuid.ToString();
			probeDefinition.Attributes["MonitoringMailboxLegacyExchangeDN"] = mailboxDatabase.MonitoringAccountLegacyDN;
			probeDefinition.Attributes["MonitoringAccountMailboxGuid"] = mailboxDatabase.MonitoringAccountMailboxGuid.ToString();
			probeDefinition.Attributes["ExternalMonitoringAccountOrganizationId"] = empty.ToString();
			probeDefinition.Attributes["RecipientAddress"] = text;
			probeDefinition.Attributes["SmtpServer"] = "127.0.0.1";
			return probeDefinition;
		}

		public static MonitorDefinition CreateMailboxDeliveryInstanceAvailabilityMonitor(MailboxDatabaseInfo mailboxDatabase, ProbeDefinition probeDefinition)
		{
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("MailboxDeliveryInstanceAvailabilityMonitor", probeDefinition.ConstructWorkItemResultName(), ExchangeComponent.MailboxTransport.Name, ExchangeComponent.MailboxTransport, 5, true, 120);
			monitorDefinition.TargetResource = mailboxDatabase.MailboxDatabaseName;
			return monitorDefinition;
		}

		public static ResponderDefinition CreateMailboxDeliveryInstanceAvailabilityEscalateResponder(MailboxDatabaseInfo mailboxDatabase, MonitorDefinition monitorDefinition, out MonitorStateTransition transition)
		{
			transition = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, TimeSpan.FromHours(14400.0));
			return EscalateResponder.CreateDefinition("MailboxDeliveryInstanceAvailabilityEscalateResponder", ExchangeComponent.MailboxTransport.Name, "MailboxDeliveryInstanceAvailabilityMonitor", monitorDefinition.ConstructWorkItemResultName(), mailboxDatabase.MailboxDatabaseName, transition.ToState, ExchangeComponent.MailboxTransport.EscalationTeam, "MailboxDatabaseAvailability for mdb {Probe.TargetResource} unhealthy", "MailboxDatabaseAvailability for mdb {Probe.TargetResource} unhealthy", true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
		}

		public static void SetActiveMDBStatus(ProbeResult result, bool active)
		{
			result.StateAttribute1 = active.ToString();
		}

		public static bool GetActiveMDBStatus(ProbeResult result)
		{
			bool result2;
			bool.TryParse(result.StateAttribute1, out result2);
			return result2;
		}

		protected virtual string GetMessageText(string mailboxDatabase)
		{
			return string.Format("X-MS-Exchange-ActiveMonitoringProbeName:{0}\r\nSubject:Delivery probe - Mbx DB - {1} Time {2}\r\n\r\nThis is a mailbox delivery probe", base.Definition.Name, mailboxDatabase, DateTime.UtcNow.ToString());
		}

		protected override void BeforeConnect()
		{
			base.WorkDefinition.MailFrom = null;
			base.WorkDefinition.MailTo = null;
			base.WorkDefinition.Data = null;
			base.TestCount = 1;
			base.WorkDefinition.Port = 475;
			base.WorkDefinition.SmtpServer = "127.0.0.1";
			base.WorkDefinition.HeloDomain = ComputerInformation.DnsPhysicalFullyQualifiedDomainName;
			base.WorkDefinition.AuthenticationType = AuthenticationType.Exchange;
			this.mailboxDatabaseName = base.Definition.TargetResource;
			this.monitoringAccountLegacyDN = base.Definition.Attributes["MonitoringMailboxLegacyExchangeDN"];
			this.recipientAddress = base.Definition.Attributes["RecipientAddress"];
			if (!Guid.TryParse(base.Definition.Attributes["MailboxDatabaseGuid"], out this.mailboxDatabaseGuid))
			{
				throw new Exception(string.Format("Unable to parse MailboxDatabaseGuid: {0}", base.Definition.TargetExtension));
			}
			if (!Guid.TryParse(base.Definition.Attributes["MonitoringAccountMailboxGuid"], out this.monitoringAccountMailboxGuid))
			{
				throw new Exception(string.Format("Unable to parse MonitoringAccountMailboxGuid: {0}", base.Definition.Attributes["MonitoringAccountMailboxGuid"]));
			}
			if (!Guid.TryParse(base.Definition.Attributes["ExternalMonitoringAccountOrganizationId"], out this.externalMonitoringAccountOrganizationId))
			{
				throw new Exception(string.Format("Unable to parse ExternalMonitoringAccountOrganizationId: {0}", base.Definition.Attributes["ExternalMonitoringAccountOrganizationId"]));
			}
			if (!DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(this.mailboxDatabaseGuid))
			{
				base.CancelProbeWithSuccess = true;
				MailboxDeliveryInstanceAvailabilityProbe.SetActiveMDBStatus(base.Result, false);
				return;
			}
			MailboxDeliveryInstanceAvailabilityProbe.SetActiveMDBStatus(base.Result, true);
		}

		protected override void AfterAuthenticate()
		{
			if (base.ShouldCancelProbe())
			{
				return;
			}
			this.InternalSendMessage();
			if (this.errorList != null)
			{
				throw new SmtpConnectionProbeException(this.errorList.ToString());
			}
		}

		protected void InternalSendMessage()
		{
			MailboxDeliveryInstanceAvailabilityProbe.MailDeliveryProbeExtendedPropertyBlob extendedPropertyBlob = new MailboxDeliveryInstanceAvailabilityProbe.MailDeliveryProbeExtendedPropertyBlob(this.recipientAddress, this.monitoringAccountLegacyDN, this.monitoringAccountMailboxGuid);
			MailboxDeliveryInstanceAvailabilityProbe.MailDeliveryProbeSmtpOutSession testSession = new MailboxDeliveryInstanceAvailabilityProbe.MailDeliveryProbeSmtpOutSession();
			string command = string.Format("XSESSIONPARAMS MDBGUID={0}", this.mailboxDatabaseGuid.ToString("N"));
			base.MeasureLatency("XSESSIONPARAMS", delegate()
			{
				this.Client.Send(command);
			});
			command = this.GetMailFromCommand();
			base.MeasureLatency("MAIL FROM", delegate()
			{
				this.Client.Send(command);
			});
			if (!base.VerifyExpectedResponse(SmtpResponse.MailFromOk.ToString()))
			{
				if (MailboxDeliveryInstanceAvailabilityProbe.IsIgnoredError(this.Client.LastResponse))
				{
					this.AddToFailureContext("Response returned was an expected error: " + this.Client.LastResponse);
					return;
				}
				this.AddToErrorList(this.recipientAddress, "MAIL FROM response not as expected. Actual: " + this.Client.LastResponse);
				return;
			}
			else
			{
				command = string.Format("{0}:{1}", "RCPT TO", this.recipientAddress);
				base.MeasureLatency("RCPT TO", delegate()
				{
					this.Client.Send(command);
				});
				if (base.VerifyExpectedResponse(SmtpResponse.RcptToOk.ToString()) || this.Client.LastResponse.Contains("thread limit exceeded"))
				{
					base.MeasureLatency("BDAT EPROP", delegate()
					{
						this.Client.BDat(extendedPropertyBlob.SerializeBlob(testSession), false);
					});
					MemoryStream messageStream = MailboxDeliveryInstanceAvailabilityProbe.GetMessageStream(this.GetMessageText(this.mailboxDatabaseName));
					base.MeasureLatency("BDAT", delegate()
					{
						this.Client.BDat(messageStream, true);
					});
					if (!base.VerifyExpectedResponse(SmtpResponse.NoopOk.ToString()))
					{
						if (MailboxDeliveryInstanceAvailabilityProbe.IsIgnoredError(this.Client.LastResponse))
						{
							this.AddToFailureContext("Response returned was an expected error: " + this.Client.LastResponse);
							return;
						}
						this.AddToErrorList(this.recipientAddress, "BDAT response not as expected. Actual: " + this.Client.LastResponse);
					}
					return;
				}
				if (MailboxDeliveryInstanceAvailabilityProbe.IsIgnoredError(this.Client.LastResponse))
				{
					this.AddToFailureContext("Response returned was an expected error: " + this.Client.LastResponse);
					return;
				}
				this.AddToErrorList(this.recipientAddress, "RCPT TO response not as expected. Actual: " + this.Client.LastResponse);
				return;
			}
		}

		private static bool IsIgnoredError(string lastResponse)
		{
			return MailboxDeliveryAvailabilityProbe.WildCardTransientResponseList.Exists((string wildcard) => TransportProbeCommon.ErrorContains(lastResponse, wildcard)) || MailboxDeliveryAvailabilityProbe.TransientSmtpResponseList.Exists((string transientResponse) => TransportProbeCommon.ErrorMatches(lastResponse, transientResponse));
		}

		private static MemoryStream GetMessageStream(string messageText)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(messageText);
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(bytes, 0, bytes.Length);
			return memoryStream;
		}

		private string GetMailFromCommand()
		{
			string text = string.Empty;
			if (!this.externalMonitoringAccountOrganizationId.Equals(Guid.Empty))
			{
				text = string.Format("MAIL FROM:maildeliveryprobe@maildeliveryprobe.com XATTRDIRECT={0} XATTRORGID=xorgid:{1} XMESSAGECONTEXT={2}", MailDirectionality.Incoming, this.externalMonitoringAccountOrganizationId, ExtendedPropertiesSmtpMessageContextBlob.VersionString);
			}
			else
			{
				text = string.Format("MAIL FROM:maildeliveryprobe@maildeliveryprobe.com XMESSAGECONTEXT={0}", ExtendedPropertiesSmtpMessageContextBlob.VersionString);
			}
			if (this.Client.IsXSysProbeAdvertised)
			{
				text = text + " XSYSPROBEID=" + base.GetProbeId();
			}
			return text;
		}

		private void AddToErrorList(string recipientAddress, string errorMessage)
		{
			string arg = " :";
			if (this.errorList == null)
			{
				this.errorList = new StringBuilder();
				arg = string.Empty;
			}
			this.errorList.AppendFormat("{0} Probe to {1} failed with error {2}", arg, recipientAddress, errorMessage);
		}

		private void AddToFailureContext(string message)
		{
			if (base.Result != null)
			{
				ProbeResult result = base.Result;
				result.FailureContext += message;
			}
		}

		internal const string MailboxDeliveryInstanceAvailabilityProbeName = "MailboxDeliveryInstanceAvailabilityProbe";

		internal const string MailboxDeliveryInstanceAvailabilityMonitorName = "MailboxDeliveryInstanceAvailabilityMonitor";

		internal const string MailboxDeliveryInstanceAvailabilityRestartResponderName = "MailboxDeliveryInstanceAvailabilityRestartResponder";

		internal const string MailboxDeliveryInstanceAvailabilityEscalateResponderName = "MailboxDeliveryInstanceAvailabilityEscalateResponder";

		internal const string EscalateResponderSubject = "MailboxDatabaseAvailability for mdb {Probe.TargetResource} unhealthy";

		internal const string EscalateResponderMessage = "MailboxDatabaseAvailability for mdb {Probe.TargetResource} unhealthy";

		internal const int ProbeRecurrenceIntervalSeconds = 120;

		internal const int ProbeTimeoutSeconds = 90;

		internal const int ProbeRetryAttempts = 3;

		internal const bool ProbeEnabled = true;

		internal const int MonitorFailureCount = 5;

		internal const bool MonitorEnabled = true;

		internal const int MonitorMonitoringInterval = 120;

		internal const bool EscalateResponderEnabled = true;

		internal const int EscalateWaitTimeSeconds = 14400;

		internal const NotificationServiceClass EscalateLevel = NotificationServiceClass.UrgentInTraining;

		internal const string MailboxDatabaseGuidAttribute = "MailboxDatabaseGuid";

		internal const string MonitoringMailboxLegacyExchangeDNAttribute = "MonitoringMailboxLegacyExchangeDN";

		internal const string RecipientAddressAttribute = "RecipientAddress";

		internal const string MonitoringAccountMailboxGuidAttribute = "MonitoringAccountMailboxGuid";

		internal const string ExternalMonitoringAccountOrganizationIdAttribute = "ExternalMonitoringAccountOrganizationId";

		internal const string SmtpServerAttribute = "SmtpServer";

		private const string XSesssionParamsCommandFormat = "XSESSIONPARAMS MDBGUID={0}";

		private const string RcptToCommand = "RCPT TO";

		private const string MailFromCommand = "MAIL FROM";

		private const string MailCommandFormat = "MAIL FROM:maildeliveryprobe@maildeliveryprobe.com ";

		private const string BdatMessageFormat = "X-MS-Exchange-ActiveMonitoringProbeName:{0}\r\nSubject:Delivery probe - Mbx DB - {1} Time {2}\r\n\r\nThis is a mailbox delivery probe";

		private const string SmtpServer = "127.0.0.1";

		private StringBuilder errorList;

		private string mailboxDatabaseName;

		private string monitoringAccountLegacyDN;

		private Guid mailboxDatabaseGuid;

		private Guid monitoringAccountMailboxGuid;

		private Guid externalMonitoringAccountOrganizationId;

		private string recipientAddress;

		private class MailDeliveryProbeSmtpOutSession : SmtpOutSession
		{
		}

		private class MailDeliveryProbeExtendedPropertyBlob : ExtendedPropertiesSmtpMessageContextBlob
		{
			public MailDeliveryProbeExtendedPropertyBlob(string recipientAddress, string legacyExchangeDN, Guid monitoringAccountMailboxGuid) : base(true, true, ProcessTransportRole.Hub)
			{
				this.recipient = MailRecipient.NewMessageRecipient(null, new MailboxDeliveryInstanceAvailabilityProbe.MailDeliveryProbeRecipientStorage
				{
					Email = recipientAddress
				});
				this.recipient.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.Transport.Resolved", true);
				this.recipient.ExtendedProperties.SetValue<Microsoft.Exchange.Data.Directory.Recipient.RecipientType>("Microsoft.Exchange.Transport.DirectoryData.RecipientType", Microsoft.Exchange.Data.Directory.Recipient.RecipientType.UserMailbox);
				this.recipient.ExtendedProperties.SetValue<string>("Microsoft.Exchange.Transport.DirectoryData.LegacyExchangeDN", legacyExchangeDN);
				this.recipient.ExtendedProperties.SetValue<Guid>("Microsoft.Exchange.Transport.DirectoryData.ExchangeGuid", monitoringAccountMailboxGuid);
			}

			protected override IEnumerable<MailRecipient> GetRecipients(SmtpOutSession smtpOutSession)
			{
				yield return this.recipient;
				yield break;
			}

			protected override IReadOnlyExtendedPropertyCollection GetMailItemExtendedProperties(SmtpOutSession smtpOutSession)
			{
				return this.mailItemExtendedProperty;
			}

			protected override int GetRecipientCount(SmtpOutSession smtpOutSession)
			{
				return 1;
			}

			private MailRecipient recipient;

			private ExtendedPropertyDictionary mailItemExtendedProperty = new ExtendedPropertyDictionary();
		}

		private class MailDeliveryProbeRecipientStorage : IMailRecipientStorage
		{
			public long RecipId
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public long MsgId
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public AdminActionStatus AdminActionStatus
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public DateTime? DeliveryTime
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public DsnFlags DsnCompleted
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public DsnFlags DsnNeeded
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public DsnRequestedFlags DsnRequested
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public Destination DeliveredDestination
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public string Email
			{
				get
				{
					return this.email;
				}
				set
				{
					this.email = value;
				}
			}

			public string ORcpt
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public string PrimaryServerFqdnGuid
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public int RetryCount
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public Status Status
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public RequiredTlsAuthLevel? TlsAuthLevel
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public int OutboundIPPool
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public IExtendedPropertyCollection ExtendedProperties
			{
				get
				{
					return this.extendedProperties;
				}
			}

			public bool IsDeleted
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public bool IsInSafetyNet
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public bool IsActive
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public bool PendingDatabaseUpdates
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public void MarkToDelete()
			{
				throw new NotImplementedException();
			}

			public void Materialize(Transaction transaction)
			{
				throw new NotImplementedException();
			}

			public void Commit(TransactionCommitMode commitMode)
			{
				throw new NotImplementedException();
			}

			public IMailRecipientStorage MoveTo(long targetMsgId)
			{
				throw new NotImplementedException();
			}

			public IMailRecipientStorage CopyTo(long target)
			{
				throw new NotImplementedException();
			}

			public void MinimizeMemory()
			{
				throw new NotImplementedException();
			}

			public void ReleaseFromActive()
			{
				throw new NotImplementedException();
			}

			public void AddToSafetyNet()
			{
				throw new NotImplementedException();
			}

			private ExtendedPropertyDictionary extendedProperties = new ExtendedPropertyDictionary();

			private string email;
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Storage;
using Microsoft.Exchange.Transport.Storage.Messaging;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class MailDeliverySmtpProbe : SmtpConnectionProbe
	{
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

		protected virtual string GetMessageText(string mailboxDatabase)
		{
			return string.Format("X-MS-Exchange-ActiveMonitoringProbeName:{0}\r\nSubject:Delivery probe - Mbx DB - {1} Time {2}\r\n\r\nThis is a mailbox delivery probe", base.Definition.Name, mailboxDatabase, DateTime.UtcNow.ToString());
		}

		protected override void BeforeConnect()
		{
			if (!base.Broker.IsLocal())
			{
				throw new SmtpConnectionProbeException("MailDeliverySmtpProbe is a local-only probe and should not be used outside in");
			}
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance == null || instance.MailboxDatabaseEndpoint == null)
			{
				throw new SmtpConnectionProbeException("No MailboxDatabaseEndpoint for Backend found on this server");
			}
			if (instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				base.Result.StateAttribute2 = "No mailboxes found, proceeding as success";
				this.cancelProbe = true;
			}
			base.WorkDefinition.MailFrom = null;
			base.WorkDefinition.MailTo = null;
			base.WorkDefinition.Data = null;
			this.mailboxCollectionForBackend = instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend;
			base.TestCount = this.mailboxCollectionForBackend.Count;
			base.WorkDefinition.Port = 475;
			base.WorkDefinition.SmtpServer = "127.0.0.1";
			base.WorkDefinition.HeloDomain = ComputerInformation.DnsPhysicalFullyQualifiedDomainName;
			base.WorkDefinition.AuthenticationType = AuthenticationType.Exchange;
		}

		protected override void AfterAuthenticate()
		{
			if (this.cancelProbe)
			{
				return;
			}
			this.SendMessage();
			this.currentIndex++;
			if (this.currentIndex == base.TestCount && this.errorList != null)
			{
				throw new SmtpConnectionProbeException(this.errorList.ToString());
			}
		}

		protected void InternalSendMessage(string recipientAddress, string mailboxDatabaseName, Guid mailboxDatabaseGuid, string mailboxLegacyExchangeDN, Guid mailboxGuid, OrganizationId monitoringAccountOrganizationId)
		{
			MailDeliverySmtpProbe.MailDeliveryProbeExtendedPropertyBlob extendedPropertyBlob = new MailDeliverySmtpProbe.MailDeliveryProbeExtendedPropertyBlob(recipientAddress, mailboxLegacyExchangeDN, mailboxGuid);
			MailDeliverySmtpProbe.MailDeliveryProbeSmtpOutSession testSession = new MailDeliverySmtpProbe.MailDeliveryProbeSmtpOutSession();
			string command = string.Format("XSESSIONPARAMS MDBGUID={0}", mailboxDatabaseGuid.ToString("N"));
			base.MeasureLatency("XSESSIONPARAMS", delegate()
			{
				this.Client.Send(command);
			});
			command = this.GetMailFromCommand(recipientAddress, mailboxDatabaseName, mailboxDatabaseGuid, mailboxLegacyExchangeDN, mailboxGuid, monitoringAccountOrganizationId);
			base.MeasureLatency("MAILFROM", delegate()
			{
				this.Client.Send(command);
			});
			if (!base.VerifyExpectedResponse(SmtpResponse.MailFromOk.ToString()))
			{
				if (MailDeliverySmtpProbe.IsIgnoredError(this.Client.LastResponse))
				{
					this.AddToFailureContext("Response returned was an expected error: " + this.Client.LastResponse);
					return;
				}
				this.AddToErrorList(recipientAddress, "MAIL FROM response not as expected. Actual: " + this.Client.LastResponse);
				return;
			}
			else
			{
				command = "RCPT TO:" + recipientAddress;
				base.MeasureLatency("RCPTTO", delegate()
				{
					this.Client.Send(command);
				});
				if (base.VerifyExpectedResponse(SmtpResponse.RcptToOk.ToString()) || this.Client.LastResponse.Contains("thread limit exceeded"))
				{
					base.MeasureLatency("BDAT EPROP", delegate()
					{
						this.Client.BDat(extendedPropertyBlob.SerializeBlob(testSession), false);
					});
					MemoryStream messageStream = MailDeliverySmtpProbe.GetMessageStream(this.GetMessageText(mailboxDatabaseName));
					base.MeasureLatency("BDAT", delegate()
					{
						this.Client.BDat(messageStream, true);
					});
					if (!base.VerifyExpectedResponse(SmtpResponse.NoopOk.ToString()))
					{
						if (MailDeliverySmtpProbe.IsIgnoredError(this.Client.LastResponse))
						{
							this.AddToFailureContext("Response returned was an expected error: " + this.Client.LastResponse);
							return;
						}
						this.AddToErrorList(recipientAddress, "BDAT response not as expected. Actual: " + this.Client.LastResponse);
					}
					return;
				}
				if (MailDeliverySmtpProbe.IsIgnoredError(this.Client.LastResponse))
				{
					this.AddToFailureContext("Response returned was an expected error: " + this.Client.LastResponse);
					return;
				}
				this.AddToErrorList(recipientAddress, "RCPT TO response not as expected. Actual: " + this.Client.LastResponse);
				return;
			}
		}

		private static bool IsIgnoredError(string lastResponse)
		{
			return lastResponse.Contains("MailboxOfflineException") || MailDeliverySmtpProbe.transientSmtpResponseList.Exists((string transientResponse) => TransportProbeCommon.ErrorMatches(lastResponse, transientResponse));
		}

		private static MemoryStream GetMessageStream(string messageText)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(messageText);
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(bytes, 0, bytes.Length);
			return memoryStream;
		}

		private string GetMailFromCommand(string recipientAddress, string mailboxDatabaseName, Guid mailboxDatabaseGuid, string mailboxLegacyExchangeDN, Guid mailboxGuid, OrganizationId monitoringAccountOrganizationId)
		{
			string text = string.Empty;
			if (Datacenter.IsMicrosoftHostedOnly(true))
			{
				Guid empty = Guid.Empty;
				if (monitoringAccountOrganizationId == null)
				{
					throw new SmtpConnectionProbeException(string.Format("The organization id was not specified for {0}", recipientAddress));
				}
				if (MultiTenantTransport.TryGetExternalOrgId(monitoringAccountOrganizationId, out empty) != ADOperationResult.Success)
				{
					throw new SmtpConnectionProbeException(string.Format("The external organization id was not found for {0}", recipientAddress));
				}
				text = string.Format("MAIL FROM:maildeliveryprobe@maildeliveryprobe.com XATTRDIRECT={0} XATTRORGID=xorgid:{1} XMESSAGECONTEXT={2}", MailDirectionality.Incoming, empty, ExtendedPropertiesSmtpMessageContextBlob.VersionString);
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

		private void SendMessage()
		{
			MailboxDatabaseInfo mailboxDatabaseInfo = this.mailboxCollectionForBackend.ElementAt(this.currentIndex);
			bool flag = DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(mailboxDatabaseInfo.MailboxDatabaseGuid);
			if (flag)
			{
				string recipientAddress = string.Format("{0}@{1}", mailboxDatabaseInfo.MonitoringAccount, mailboxDatabaseInfo.MonitoringAccountDomain);
				this.InternalSendMessage(recipientAddress, mailboxDatabaseInfo.MailboxDatabaseName, mailboxDatabaseInfo.MailboxDatabaseGuid, mailboxDatabaseInfo.MonitoringMailboxLegacyExchangeDN, mailboxDatabaseInfo.MonitoringAccountMailboxGuid, mailboxDatabaseInfo.MonitoringAccountOrganizationId);
				return;
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.SMTPConnectionTracer, new TracingContext(), string.Format("MailDeliverySmtpProbe skipped because Mailbox Database {0} copy status was not active.", mailboxDatabaseInfo.MailboxDatabaseGuid), null, "SendMessage", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\MailDeliverySmtpProbe.cs", 401);
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

		private const string XSesssionParamsCommandFormat = "XSESSIONPARAMS MDBGUID={0}";

		private const string MailCommandFormat = "MAIL FROM:maildeliveryprobe@maildeliveryprobe.com ";

		private const string BdatMessageFormat = "X-MS-Exchange-ActiveMonitoringProbeName:{0}\r\nSubject:Delivery probe - Mbx DB - {1} Time {2}\r\n\r\nThis is a mailbox delivery probe";

		private const string SmtpServer = "127.0.0.1";

		private static readonly List<string> transientSmtpResponseList = new List<string>
		{
			AckReason.MessageDelayedDeleteByAdmin.ToString(),
			AckReason.MessageDeletedByAdmin.ToString(),
			AckReason.MessageDeletedByTransportAgent.ToString(),
			AckReason.PoisonMessageDeletedByAdmin.ToString(),
			AckReason.MessageDelayedDeleteByAdmin.ToString(),
			AckReason.MessageDeletedByAdmin.ToString(),
			AckReason.MessageDeletedByTransportAgent.ToString(),
			AckReason.PoisonMessageDeletedByAdmin.ToString(),
			AckReason.MessageDelayedDeleteByAdmin.ToString(),
			AckReason.MessageDeletedByAdmin.ToString(),
			AckReason.MessageDeletedByTransportAgent.ToString(),
			AckReason.PoisonMessageDeletedByAdmin.ToString(),
			AckReason.MailboxServerOffline.ToString(),
			AckReason.MDBOffline.ToString(),
			AckReason.MapiNoAccessFailure.ToString(),
			AckReason.MailboxServerTooBusy.ToString(),
			AckReason.MailboxMapiSessionLimit.ToString(),
			AckReason.MailboxServerMaxThreadsPerMdbExceeded.ToString(),
			AckReason.MapiExceptionMaxThreadsPerSCTExceeded.ToString(),
			AckReason.MailboxDatabaseThreadLimitExceeded.ToString(),
			AckReason.RecipientThreadLimitExceeded.ToString(),
			AckReason.DeliverySourceThreadLimitExceeded.ToString(),
			AckReason.DynamicMailboxDatabaseThrottlingLimitExceeded.ToString(),
			AckReason.MailboxIOError.ToString(),
			AckReason.MailboxServerNotEnoughMemory.ToString(),
			AckReason.MissingMdbProperties.ToString(),
			AckReason.RecipientMailboxQuarantined.ToString()
		};

		private ICollection<MailboxDatabaseInfo> mailboxCollectionForBackend;

		private int currentIndex;

		private StringBuilder errorList;

		private bool cancelProbe;

		private class MailDeliveryProbeSmtpOutSession : SmtpOutSession
		{
		}

		private class MailDeliveryProbeExtendedPropertyBlob : ExtendedPropertiesSmtpMessageContextBlob
		{
			public MailDeliveryProbeExtendedPropertyBlob(string recipientAddress, string legacyExchangeDN, Guid mailboxGuid) : base(true, true, ProcessTransportRole.Hub)
			{
				this.recipient = MailRecipient.NewMessageRecipient(null, new MailDeliverySmtpProbe.MailDeliveryProbeRecipientStorage
				{
					Email = recipientAddress
				});
				this.recipient.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.Transport.Resolved", true);
				this.recipient.ExtendedProperties.SetValue<Microsoft.Exchange.Data.Directory.Recipient.RecipientType>("Microsoft.Exchange.Transport.DirectoryData.RecipientType", Microsoft.Exchange.Data.Directory.Recipient.RecipientType.UserMailbox);
				this.recipient.ExtendedProperties.SetValue<string>("Microsoft.Exchange.Transport.DirectoryData.LegacyExchangeDN", legacyExchangeDN);
				this.recipient.ExtendedProperties.SetValue<Guid>("Microsoft.Exchange.Transport.DirectoryData.ExchangeGuid", mailboxGuid);
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

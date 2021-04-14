using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Transport.RightsManagement;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal sealed class RmsDecryptionAgent : RoutingAgent
	{
		public RmsDecryptionAgent(AgentInstanceController agentInstanceController)
		{
			this.agentInstanceController = agentInstanceController;
			base.OnSubmittedMessage += this.OnSubmittedMessageEventHandler;
			lock (RmsDecryptionAgent.SyncObject)
			{
				if (RmsDecryptionAgent.localIp == null)
				{
					try
					{
						RmsDecryptionAgent.localIp = Dns.GetHostEntry(Dns.GetHostName());
					}
					catch (SocketException)
					{
					}
				}
			}
		}

		private void OnSubmittedMessageEventHandler(SubmittedMessageEventSource eventSource, QueuedMessageEventArgs args)
		{
			this.source = eventSource;
			this.tenantId = base.MailItem.TenantId;
			this.messageId = base.MailItem.Message.MessageId;
			DecryptionBaseComponent decryptionBaseComponent = new DecryptionBaseComponent(base.MailItem, new OnProcessDecryption(this.OnProcessDecryption));
			if (!decryptionBaseComponent.ShouldProcess())
			{
				return;
			}
			if (!this.agentInstanceController.TryMakeActive(this.tenantId))
			{
				this.TracePass("Unable to activate RmsDecryptionAgent because reaching capacity limit - deferring message.", new object[0]);
				this.source.Defer(RmsClientManager.AppSettings.ActiveAgentCapDeferInterval, RmsDecryptionAgent.ActiveAgentsCapResponse);
				return;
			}
			this.isActive = true;
			decryptionBaseComponent.StartDecryption();
		}

		private object OnProcessDecryption(DecryptionStatus status, TransportDecryptionSetting settings, AgentAsyncState asyncState, Exception exception)
		{
			if (status == DecryptionStatus.StartAsync)
			{
				return base.GetAgentAsyncContext();
			}
			if (status == DecryptionStatus.ConfigurationLoadFailure)
			{
				this.OnConfigurationLoadFailure(settings, exception);
			}
			else if (status == DecryptionStatus.TransientFailure)
			{
				this.OnTransientFailure(settings, exception);
			}
			else if (status == DecryptionStatus.PermanentFailure)
			{
				this.OnPermanentFailure(settings);
			}
			else if (status == DecryptionStatus.Success && RmsDecryptionAgent.localIp != null && RmsDecryptionAgent.localIp.AddressList != null && RmsDecryptionAgent.localIp.AddressList.Length != 0)
			{
				string localTcpInfo = RmsDecryptionAgent.localIp.AddressList[0].ToString();
				HeaderList headers = base.MailItem.Message.RootPart.Headers;
				Utils.PatchReceiverHeader(headers, localTcpInfo, "Transport Decrypted");
			}
			if (asyncState != null)
			{
				asyncState.Complete();
			}
			if (this.isActive)
			{
				this.agentInstanceController.MakeInactive(this.tenantId);
				this.isActive = false;
			}
			return null;
		}

		private void OnConfigurationLoadFailure(TransportDecryptionSetting setting, Exception exception)
		{
			if (this.IncrementDeferralCountAndCheckCap())
			{
				this.TracePass("Deferred Message {0}", new object[]
				{
					this.messageId
				});
				this.source.Defer(RmsClientManager.AppSettings.TransientErrorDeferInterval);
				return;
			}
			OrganizationId organizationId = Utils.OrgIdFromMailItem(base.MailItem);
			DecryptionBaseComponent.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_FailedToLoadIRMConfiguration, null, new object[]
			{
				RmsComponent.DecryptionAgent,
				organizationId.ToString(),
				exception
			});
			this.OnPermanentFailure(setting);
		}

		private void OnTransientFailure(TransportDecryptionSetting setting, Exception exception)
		{
			if (this.IncrementDeferralCountAndCheckCap())
			{
				this.TracePass("Deferred Message {0}", new object[]
				{
					this.messageId
				});
				this.source.Defer(RmsClientManager.AppSettings.TransientErrorDeferInterval);
				return;
			}
			DecryptionBaseComponent.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_TransportDecryptionPermanentException, base.MailItem.Message.MessageId, new object[]
			{
				base.MailItem.Message.MessageId,
				Utils.OrgIdFromMailItem(base.MailItem),
				exception
			});
			this.OnPermanentFailure(setting);
		}

		private void OnPermanentFailure(TransportDecryptionSetting setting)
		{
			RmsDecryptionAgentPerfCounters.MessageFailedToDecrypt.Increment();
			DecryptionBaseComponent.UpdatePercentileCounters(false);
			if (setting != TransportDecryptionSetting.Mandatory)
			{
				if (setting == TransportDecryptionSetting.Optional)
				{
					Utils.StampXHeader(base.MailItem.Message, "X-MS-Exchange-Forest-TransportDecryption-Action", "Skipped");
				}
				return;
			}
			this.TraceFail("Transport Decryption will NDR message {0}, Response {1} because of permanent decryption error", new object[]
			{
				this.messageId,
				"Microsoft Exchange Transport cannot RMS decrypt the message."
			});
			EnvelopeRecipientCollection recipients = base.MailItem.Recipients;
			if (recipients == null)
			{
				this.TraceFail("Transport Decryption has no recipients to NDR for message {0}", new object[]
				{
					this.messageId
				});
				return;
			}
			for (int i = recipients.Count - 1; i >= 0; i--)
			{
				base.MailItem.Recipients.Remove(recipients[i], DsnType.Failure, Utils.GetResponseForNDR(new string[]
				{
					"Microsoft Exchange Transport cannot RMS decrypt the message."
				}));
			}
		}

		private bool IncrementDeferralCountAndCheckCap()
		{
			int num = Utils.IncrementDeferralCount(base.MailItem, "Microsoft.Exchange.RmsDecryptionAgent.DeferralCount");
			if (num == -1)
			{
				this.TraceFail("Deferral count of message {0} is broken", new object[]
				{
					this.messageId
				});
				return false;
			}
			if (num > 1)
			{
				this.TracePass("Message {0} has been deferred {1} times", new object[]
				{
					this.messageId,
					num - 1
				});
			}
			return num <= 2;
		}

		private void TracePass(string formatString, params object[] args)
		{
			if (base.MailItem != null)
			{
				RmsClientManager.TracePass(this, base.MailItem.SystemProbeId, formatString, args);
			}
		}

		private void TraceFail(string formatString, params object[] args)
		{
			if (base.MailItem != null)
			{
				RmsClientManager.TraceFail(this, base.MailItem.SystemProbeId, formatString, args);
			}
		}

		private const int MaxDeferrals = 2;

		private const string DeferralCountProperty = "Microsoft.Exchange.RmsDecryptionAgent.DeferralCount";

		private static readonly SmtpResponse ActiveAgentsCapResponse = new SmtpResponse("452", "4.3.2", new string[]
		{
			"Already processing maximum number of RMS message for Transport Decryption"
		});

		private static readonly object SyncObject = new object();

		private static IPHostEntry localIp;

		private readonly AgentInstanceController agentInstanceController;

		private SubmittedMessageEventSource source;

		private Guid tenantId;

		private string messageId;

		private bool isActive;
	}
}

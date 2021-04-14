using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Transport.RightsManagement;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal sealed class RmsProtocolDecryptionAgent : SmtpReceiveAgent
	{
		public RmsProtocolDecryptionAgent()
		{
			base.OnEndOfData += this.EndOfDataHandler;
		}

		private void EndOfDataHandler(ReceiveMessageEventSource receiveMessageEventSource, EndOfDataEventArgs endOfDataEventArgs)
		{
			DecryptionBaseComponent decryptionBaseComponent = new DecryptionBaseComponent(base.MailItem, new OnProcessDecryption(this.OnProcessDecryption));
			if (!decryptionBaseComponent.ShouldProcess())
			{
				return;
			}
			this.source = receiveMessageEventSource;
			decryptionBaseComponent.StartDecryption();
		}

		private object OnProcessDecryption(DecryptionStatus status, TransportDecryptionSetting settings, AgentAsyncState asyncState, Exception exception)
		{
			if (status == DecryptionStatus.StartAsync)
			{
				return base.GetAgentAsyncContext();
			}
			if (status == DecryptionStatus.PermanentFailure)
			{
				this.OnPermanentFailure(settings);
			}
			else if (status == DecryptionStatus.Success)
			{
				string localTcpInfo = this.source.SmtpSession.LocalEndPoint.Address.ToString();
				HeaderList headers = base.MailItem.Message.RootPart.Headers;
				Utils.PatchReceiverHeader(headers, localTcpInfo, "Transport Decrypted");
			}
			if (asyncState != null)
			{
				asyncState.Complete();
			}
			return null;
		}

		private void OnPermanentFailure(TransportDecryptionSetting setting)
		{
			if (setting == TransportDecryptionSetting.Mandatory)
			{
				RmsDecryptionAgentPerfCounters.MessageFailedToDecrypt.Increment();
				DecryptionBaseComponent.UpdatePercentileCounters(false);
				RmsClientManager.TraceFail(this, base.MailItem.SystemProbeId, "NDRMessage for message {0}, Response {1}", new object[]
				{
					base.MailItem.Message.MessageId,
					"Microsoft Exchange Transport cannot RMS decrypt the message."
				});
				this.source.RejectMessage(Utils.GetResponseForNDR(new string[]
				{
					"Microsoft Exchange Transport cannot RMS decrypt the message."
				}));
			}
		}

		private ReceiveMessageEventSource source;
	}
}

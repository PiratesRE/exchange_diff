using System;
using System.IO;
using Microsoft.Exchange.Transport.RightsManagement;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class E4eTransportSmtpProbe : TransportSmtpProbe
	{
		protected override void SendMail(SmtpProbeWorkDefinition.SendMailDefinition sendMailDefinition, string lamNotificationID)
		{
			if (this.GetSenderTenantIdAsGuid(sendMailDefinition, out this.senderTenantId))
			{
				SendMailHelper.SendMail(base.Definition.Name, sendMailDefinition, lamNotificationID, new SendMailHelper.CreateMessageStreamDelegate(this.CreateE4eProtectedMessageStream));
			}
		}

		protected override string GetProbeResultComponent()
		{
			return ExchangeComponent.Rms.Name;
		}

		private void CreateE4eProtectedMessageStream(string probeName, SmtpProbeWorkDefinition.SendMailDefinition sendMailDefinition, MemoryStream encryptedProbeMessageStream, string lamNotificationID)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				SendMailHelper.CreateDefaultMessageStream(probeName, sendMailDefinition, memoryStream, lamNotificationID);
				E4eProbeHelper.EncryptProbeMail(this.senderTenantId, sendMailDefinition.SenderUsername, sendMailDefinition.RecipientUsername, memoryStream, encryptedProbeMessageStream);
			}
		}

		private bool GetSenderTenantIdAsGuid(SmtpProbeWorkDefinition.SendMailDefinition sendMailDefinition, out Guid senderTenantGuid)
		{
			if (!Guid.TryParse(sendMailDefinition.SenderTenantID, out senderTenantGuid))
			{
				this.TraceError("Unable to parse SendMailDefinition.SenderTenantID as a Guid: {0}", new object[]
				{
					sendMailDefinition.SenderTenantID
				});
				return false;
			}
			return true;
		}

		private Guid senderTenantId;
	}
}

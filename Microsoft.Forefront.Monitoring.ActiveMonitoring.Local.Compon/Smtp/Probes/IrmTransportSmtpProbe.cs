using System;
using System.IO;
using Microsoft.Exchange.Transport.RightsManagement;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class IrmTransportSmtpProbe : TransportSmtpProbe
	{
		protected override void SendMail(SmtpProbeWorkDefinition.SendMailDefinition sendMailDefinition, string lamNotificationID)
		{
			if (this.GetRecipientTenantIdAsGuid(sendMailDefinition, out this.recipientTenantId))
			{
				SendMailHelper.SendMail(base.Definition.Name, sendMailDefinition, lamNotificationID, new SendMailHelper.CreateMessageStreamDelegate(this.CreateIrmProtectedMessageStream));
			}
		}

		protected override string GetProbeResultComponent()
		{
			return ExchangeComponent.Rms.Name;
		}

		private void CreateIrmProtectedMessageStream(string probeName, SmtpProbeWorkDefinition.SendMailDefinition sendMailDefinition, MemoryStream encryptedProbeMessageStream, string lamNotificationID)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				SendMailHelper.CreateDefaultMessageStream(probeName, sendMailDefinition, memoryStream, lamNotificationID);
				IrmProbeHelper.EncryptProbeMail(this.recipientTenantId, memoryStream, encryptedProbeMessageStream);
			}
		}

		private bool GetRecipientTenantIdAsGuid(SmtpProbeWorkDefinition.SendMailDefinition sendMailDefinition, out Guid recipientTenantGuid)
		{
			if (!Guid.TryParse(sendMailDefinition.RecipientTenantID, out recipientTenantGuid))
			{
				this.TraceError("Unable to parse SendMailDefinition.RecipientTenantID as a Guid: {0}", new object[]
				{
					sendMailDefinition.RecipientTenantID
				});
				return false;
			}
			return true;
		}

		private Guid recipientTenantId;
	}
}

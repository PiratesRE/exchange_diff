using System;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Management.MailboxTransportSubmission.MapiProbe;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Transport.Probes
{
	internal class SendMapiMailDefinitionFactory
	{
		public static SendMapiMailDefinition CreateInstance(string lamNotificationId, ProbeDefinition probeDefinition, IMailboxProvider mailboxProviderInstance, out MailboxSelectionResult mailboxSelectionResult)
		{
			mailboxSelectionResult = MailboxSelectionResult.Success;
			if (string.IsNullOrEmpty(lamNotificationId))
			{
				throw new ArgumentNullException("lamNotificationId");
			}
			if (probeDefinition == null)
			{
				throw new ArgumentNullException("probeDefinition");
			}
			if (string.IsNullOrWhiteSpace(probeDefinition.ExtensionAttributes))
			{
				throw new ArgumentNullException("probeDefinition.ExtensionAttributes");
			}
			if (mailboxProviderInstance == null)
			{
				throw new ArgumentNullException("mailboxProviderInstance");
			}
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(probeDefinition.ExtensionAttributes);
			Guid mbxGuid;
			Guid mdbGuid;
			string emailAddress;
			mailboxSelectionResult = mailboxProviderInstance.TryGetMailboxToUse(out mbxGuid, out mdbGuid, out emailAddress);
			return SendMapiMailDefinitionFactory.FromXml(xmlDocument, lamNotificationId, emailAddress, mbxGuid, mdbGuid, mailboxSelectionResult != MailboxSelectionResult.Success);
		}

		public static SendMapiMailDefinition CreateMapiMailInstance(string lamNotificationId, ProbeDefinition probeDefinition)
		{
			if (string.IsNullOrEmpty(lamNotificationId))
			{
				throw new ArgumentNullException("lamNotificationId");
			}
			if (probeDefinition == null)
			{
				throw new ArgumentNullException("probeDefinition");
			}
			if (string.IsNullOrWhiteSpace(probeDefinition.ExtensionAttributes))
			{
				throw new ArgumentNullException("probeDefinition.ExtensionAttributes");
			}
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(probeDefinition.ExtensionAttributes);
			return SendMapiMailDefinitionFactory.FromXml(xmlDocument, lamNotificationId, null, Guid.Empty, Guid.Empty, true);
		}

		private static SendMapiMailDefinition FromXml(XmlDocument workContext, string lamNotificationId, string emailAddress, Guid mbxGuid, Guid mdbGuid, bool skipSenderValidation)
		{
			Utils.CheckXmlElement(workContext.SelectSingleNode("ExtensionAttributes/WorkContext/SendMapiMail"), "SendMapiMail");
			XmlElement xmlElement = Utils.CheckXmlElement(workContext.SelectSingleNode("ExtensionAttributes/WorkContext/SendMapiMail/Message"), "Message");
			string arg = xmlElement.GetAttribute("Subject") ?? "MapiSubmitLAMProbe";
			return SendMapiMailDefinition.CreateInstance(string.Format("{0}-{1}", lamNotificationId, arg), xmlElement.GetAttribute("Body"), Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("MessageClass"), "MessageClass"), Utils.GetBoolean(xmlElement.GetAttribute("DoNotDeliver"), "DoNotDeliver", true), Utils.GetBoolean(xmlElement.GetAttribute("DropMessageInHub"), "DropMessageInHub", true), Utils.GetBoolean(xmlElement.GetAttribute("DeleteAfterSubmit"), "DeleteAfterSubmit", true), emailAddress, mbxGuid, mdbGuid, emailAddress, skipSenderValidation);
		}
	}
}

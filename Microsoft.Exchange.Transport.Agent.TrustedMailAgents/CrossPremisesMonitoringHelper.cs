using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.SecureMail;

namespace Microsoft.Exchange.Transport.Agent.TrustedMail
{
	internal class CrossPremisesMonitoringHelper
	{
		public static bool TryHandleCrossPremisesProbe(MailItem mailItem, SmtpServer smtpServer)
		{
			if (CrossPremisesMonitoringHelper.IsCrossPremisesProbe(mailItem))
			{
				EmailMessage response = CrossPremisesMonitoringHelper.GetResponse(mailItem);
				SubmitHelper.CreateTransportMailItemWithNullReversePathAndSubmitWithoutDSNs(((TransportMailItemWrapper)mailItem).TransportMailItem, response, smtpServer.Name, smtpServer.Version, "InboundTrustAgent");
				CrossPremisesMonitoringHelper.RemoveProbeRecipient(mailItem);
				return true;
			}
			return false;
		}

		internal static bool IsCrossPremisesProbe(MailItem mailItem)
		{
			if (mailItem == null || mailItem.Recipients.Count != 1 || !MultilevelAuth.IsInternalMail(mailItem.MimeDocument.RootPart.Headers))
			{
				return false;
			}
			string subject = mailItem.Message.Subject;
			string text = mailItem.FromAddress.ToString();
			string text2 = mailItem.Recipients[0].Address.ToString();
			Guid guid;
			return !string.IsNullOrEmpty(subject) && subject.StartsWith("CrossPremiseMailFlowMonitoring-") && text.StartsWith("SystemMailbox") && text2.StartsWith("FederatedEmail.4c1f4d8b-8179-4148-93bf-00a95fa1e042") && GuidHelper.TryParseGuid(subject.Substring("CrossPremiseMailFlowMonitoring-".Length), out guid);
		}

		internal static EmailMessage GetResponse(MailItem mailItem)
		{
			EmailMessage emailMessage = EmailMessage.Create(BodyFormat.Text);
			emailMessage.Subject = "RSP: " + mailItem.Message.Subject;
			emailMessage.From = new EmailRecipient(string.Empty, mailItem.Recipients[0].Address.ToString());
			emailMessage.To.Add(new EmailRecipient(string.Empty, mailItem.FromAddress.ToString()));
			using (Stream contentWriteStream = emailMessage.Body.GetContentWriteStream())
			{
				using (new StreamWriter(contentWriteStream, Encoding.ASCII))
				{
					foreach (Header header in mailItem.Message.RootPart.Headers)
					{
						header.WriteTo(contentWriteStream);
					}
				}
			}
			return emailMessage;
		}

		private static void RemoveProbeRecipient(MailItem mailItem)
		{
			((MailRecipientCollectionWrapper)mailItem.Recipients).Remove(mailItem.Recipients[0], false);
		}

		private const string SystemMailboxAddressPrefix = "SystemMailbox";

		private const string TargetLocalPart = "FederatedEmail.4c1f4d8b-8179-4148-93bf-00a95fa1e042";

		private const string ProbeSubjectPrefix = "CrossPremiseMailFlowMonitoring-";

		private const string ResponsePrefix = "RSP: ";

		private const string AgentName = "InboundTrustAgent";
	}
}

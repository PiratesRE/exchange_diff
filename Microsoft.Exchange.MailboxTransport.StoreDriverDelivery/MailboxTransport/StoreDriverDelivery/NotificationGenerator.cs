using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class NotificationGenerator
	{
		internal static EmailMessage GenerateDecisionNotTakenNotification(RoutingAddress from, RoutingAddress recipient, string subject, string threadIndex, string threadTopic, string decisionMaker, bool? decision, ExDateTime? decisionTime, Header acceptLanguageHeader, Header contentLanguageHeader, CultureInfo fallbackCulture)
		{
			if (from.Equals(RoutingAddress.NullReversePath) || recipient.Equals(RoutingAddress.NullReversePath) || !from.IsValid || !recipient.IsValid)
			{
				return null;
			}
			DsnHumanReadableWriter dsnHumanReadableWriter = Components.DsnGenerator.DsnHumanReadableWriter;
			ApprovalInformation decisionConflictInformation = dsnHumanReadableWriter.GetDecisionConflictInformation(subject, decisionMaker, decision, decisionTime, acceptLanguageHeader, contentLanguageHeader, fallbackCulture);
			EmailMessage emailMessage = EmailMessage.Create(BodyFormat.Html, false, decisionConflictInformation.MessageCharset.Name);
			emailMessage.From = new EmailRecipient(string.Empty, from.ToString());
			emailMessage.To.Add(new EmailRecipient(string.Empty, recipient.ToString()));
			emailMessage.Subject = decisionConflictInformation.Subject;
			using (Stream contentWriteStream = emailMessage.Body.GetContentWriteStream())
			{
				dsnHumanReadableWriter.WriteHtmlModerationBody(contentWriteStream, decisionConflictInformation);
				contentWriteStream.Flush();
			}
			Header header = Header.Create("X-MS-Exchange-Organization-SCL");
			header.Value = "-1";
			emailMessage.RootPart.Headers.AppendChild(header);
			Header header2 = (TextHeader)Header.Create("Thread-Index");
			header2.Value = threadIndex;
			emailMessage.RootPart.Headers.AppendChild(header2);
			Header header3 = (TextHeader)Header.Create("Thread-Topic");
			header3.Value = threadTopic;
			emailMessage.RootPart.Headers.AppendChild(header3);
			return emailMessage;
		}
	}
}

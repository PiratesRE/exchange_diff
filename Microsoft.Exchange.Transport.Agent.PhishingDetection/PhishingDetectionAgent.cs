using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Partner;
using Microsoft.Exchange.Data.Transport.Routing;

namespace Microsoft.Exchange.Transport.Agent.PhishingDetection
{
	public sealed class PhishingDetectionAgent : RoutingAgent
	{
		public PhishingDetectionAgent(SmtpServer server)
		{
			this.smtpServer = (server as ExtendedRoutingSmtpServer);
			base.OnSubmittedMessage += this.SubmittedMessage;
		}

		private void SubmittedMessage(SubmittedMessageEventSource source, QueuedMessageEventArgs e)
		{
			if (!PhishingDetection.TenantHasPhishingEnabled(e.MailItem.TenantId))
			{
				return;
			}
			EmailMessage message = e.MailItem.Message;
			Body body = message.Body;
			Encoding encoding = string.IsNullOrEmpty(body.CharsetName) ? Encoding.UTF8 : Encoding.GetEncoding(body.CharsetName);
			using (Stream contentReadStream = body.GetContentReadStream())
			{
				using (StreamReader streamReader = new StreamReader(contentReadStream, encoding))
				{
					try
					{
						string mailBody = streamReader.ReadToEnd();
						List<KeyValuePair<string, string>> list = PhishingDetection.ExtractPhishingUrlsFromContent(mailBody);
						if (list != null && list.Count != 0)
						{
							this.smtpServer.TrackAgentInfo("Civility", "Phishing", list);
						}
					}
					catch (IOException ex)
					{
						PhishingDetection.LogWarning(ex.Message);
					}
				}
			}
		}

		private readonly ExtendedRoutingSmtpServer smtpServer;
	}
}

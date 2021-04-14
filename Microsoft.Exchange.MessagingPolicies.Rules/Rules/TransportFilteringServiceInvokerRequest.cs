using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Transport;
using Microsoft.Filtering;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class TransportFilteringServiceInvokerRequest : FilteringServiceInvokerRequest
	{
		private TransportFilteringServiceInvokerRequest(string organizationId, TimeSpan scanTimeout, int textScanLimit, MimeFipsDataStreamFilteringRequest mimeFipsDataStreamFilteringRequest) : base(organizationId, scanTimeout, textScanLimit, mimeFipsDataStreamFilteringRequest)
		{
		}

		public static TransportFilteringServiceInvokerRequest CreateInstance(MailItem mailItem, RulesScanTimeout rulesScanTimeout = null)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			string organizationId = mailItem.TenantId.ToString();
			TransportMailItem transportMailItem = TransportUtils.GetTransportMailItem(mailItem);
			if (transportMailItem != null)
			{
				organizationId = transportMailItem.ExternalOrganizationId.ToString();
			}
			int attachmentTextScanLimit = TransportRulesEvaluationContext.GetAttachmentTextScanLimit(mailItem);
			TimeSpan scanTimeout = TransportFilteringServiceInvokerRequest.GetScanTimeout(mailItem.Message, rulesScanTimeout ?? TransportFilteringServiceInvokerRequest.defaultRulesScanTimeout);
			MimeFipsDataStreamFilteringRequest mimeFipsDataStreamFilteringRequest = MimeFipsDataStreamFilteringRequest.CreateInstance(mailItem);
			return new TransportFilteringServiceInvokerRequest(organizationId, scanTimeout, attachmentTextScanLimit, mimeFipsDataStreamFilteringRequest);
		}

		internal static TimeSpan GetScanTimeout(EmailMessage message, RulesScanTimeout rulesScanTimeout)
		{
			Stream stream = null;
			List<KeyValuePair<string, Stream>> list = new List<KeyValuePair<string, Stream>>();
			TimeSpan timeout;
			try
			{
				if (message.Body.MimePart != null)
				{
					stream = message.Body.MimePart.GetRawContentReadStream();
				}
				foreach (Attachment attachment in message.Attachments)
				{
					Stream stream2 = null;
					if (attachment.MimePart != null)
					{
						stream2 = attachment.MimePart.GetRawContentReadStream();
					}
					else if (!attachment.TryGetContentReadStream(out stream2))
					{
						stream2 = null;
					}
					if (stream2 != null)
					{
						list.Add(new KeyValuePair<string, Stream>(attachment.FileName, stream2));
					}
				}
				timeout = rulesScanTimeout.GetTimeout(stream, list);
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
				}
				foreach (KeyValuePair<string, Stream> keyValuePair in list)
				{
					if (keyValuePair.Value != null)
					{
						keyValuePair.Value.Dispose();
					}
				}
			}
			return timeout;
		}

		private static readonly RulesScanTimeout defaultRulesScanTimeout = new RulesScanTimeout(Components.TransportAppConfig.TransportRuleConfig.ScanVelocities, Components.TransportAppConfig.TransportRuleConfig.TransportRuleMinFipsTimeoutInMilliseconds);
	}
}

using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal static class FindMessageUtils
	{
		internal static char MessageIdDomainPartDivider
		{
			get
			{
				return FindMessageUtils.messageIdDomainPartDivider;
			}
			set
			{
				FindMessageUtils.messageIdDomainPartDivider = value;
			}
		}

		internal static bool TryParseMessageId(string messageId, out string local, out string domain)
		{
			local = string.Empty;
			domain = string.Empty;
			if (string.IsNullOrEmpty(messageId))
			{
				FindMessageUtils.diag.TraceDebug(0L, "no message id");
				return false;
			}
			if (messageId.Length > 512)
			{
				FindMessageUtils.diag.TraceDebug<string>(0L, "message id '{0}' is longer than supported max.", messageId);
				return false;
			}
			int num = messageId.IndexOf(FindMessageUtils.MessageIdDomainPartDivider);
			if (num >= 0)
			{
				local = messageId.Substring(0, num);
				if (num + 1 < messageId.Length)
				{
					domain = messageId.Substring(num + 1, messageId.Length - num - 1);
				}
			}
			else
			{
				local = messageId;
			}
			return true;
		}

		private static readonly Trace diag = ExTraceGlobals.ApprovalAgentTracer;

		private static readonly PropertyDefinition[] FindByMessageIdPropertyDefinition = new PropertyDefinition[]
		{
			ItemSchema.InternetMessageId,
			ItemSchema.Id
		};

		private static char messageIdDomainPartDivider = '@';
	}
}

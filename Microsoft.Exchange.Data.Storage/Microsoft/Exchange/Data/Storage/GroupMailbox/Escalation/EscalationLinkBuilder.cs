using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.GroupMailbox.Escalation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EscalationLinkBuilder
	{
		public EscalationLinkBuilder(IExchangePrincipal groupExchangePrincipal, IMailboxUrls mailboxUrls)
		{
			Util.ThrowOnNullArgument(groupExchangePrincipal, "groupExchangePrincipal");
			Util.ThrowOnNullArgument(mailboxUrls, "mailboxUrls");
			this.groupExchangePrincipal = groupExchangePrincipal;
			this.mailboxUrls = mailboxUrls;
		}

		public virtual string GetEscalationLink(EscalationLinkType linkType)
		{
			string owaUrl = this.GetOwaUrl();
			if (owaUrl == null)
			{
				return null;
			}
			string domain = this.groupExchangePrincipal.MailboxInfo.PrimarySmtpAddress.Domain;
			string text = (linkType == EscalationLinkType.Subscribe) ? "subscribe" : "unsubscribe";
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}/groupsubscription.ashx?realm={2}&action={3}&exsvurl=1", new object[]
			{
				owaUrl,
				this.groupExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString(),
				domain,
				text
			});
		}

		private string GetOwaUrl()
		{
			string text = this.mailboxUrls.OwaUrl;
			if (text == null)
			{
				throw new ServerNotFoundException("Unable to find OWA Service URL.");
			}
			if (!text.EndsWith("/"))
			{
				text += "/";
			}
			return text;
		}

		public const string SubscribeActionName = "subscribe";

		public const string UnsubscribeActionName = "unsubscribe";

		private readonly IExchangePrincipal groupExchangePrincipal;

		private IMailboxUrls mailboxUrls;
	}
}

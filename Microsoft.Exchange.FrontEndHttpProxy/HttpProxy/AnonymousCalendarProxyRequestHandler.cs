using System;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.HttpProxy
{
	internal class AnonymousCalendarProxyRequestHandler : BEServerCookieProxyRequestHandler<OwaService>
	{
		protected override ClientAccessType ClientAccessType
		{
			get
			{
				return ClientAccessType.Internal;
			}
		}

		internal static bool IsAnonymousCalendarRequest(HttpRequest request)
		{
			return AnonymousPublishingUrl.IsValidAnonymousPublishingUrl(request.Url);
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			AnonymousPublishingUrl anonymousPublishingUrl = new AnonymousPublishingUrl(base.ClientRequest.Url);
			if (anonymousPublishingUrl.ParameterSegments.Length > 0)
			{
				string text = anonymousPublishingUrl.ParameterSegments[0];
				Match match = RegexUtilities.TryMatch(Constants.GuidAtDomainRegex, text, base.Logger);
				if (match != null && match.Success)
				{
					Guid mailboxGuid = new Guid(match.Result("${mailboxguid}"));
					string text2 = match.Result("${domain}");
					string value = string.Format("AnonymousPublishingUrl-MailboxGuid{0}", string.IsNullOrEmpty(text2) ? string.Empty : "WithDomainAndSmtpFallback");
					base.Logger.Set(HttpProxyMetadata.RoutingHint, value);
					MailboxGuidAnchorMailbox mailboxGuidAnchorMailbox = new MailboxGuidAnchorMailbox(mailboxGuid, text2, this);
					if (!string.IsNullOrEmpty(text2))
					{
						mailboxGuidAnchorMailbox.FallbackSmtp = text;
					}
					return mailboxGuidAnchorMailbox;
				}
				match = RegexUtilities.TryMatch(Constants.AddressRegex, text, base.Logger);
				if (match != null && match.Success)
				{
					string text3 = match.Result("${address}");
					if (!string.IsNullOrEmpty(text3) && SmtpAddress.IsValidSmtpAddress(text3))
					{
						base.Logger.Set(HttpProxyMetadata.RoutingHint, "AnonymousPublishingUrl-SMTP");
						return new SmtpAnchorMailbox(text3, this);
					}
				}
			}
			return base.ResolveAnchorMailbox();
		}
	}
}

using System;
using System.Diagnostics;
using System.Globalization;
using System.Web;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.GroupMailbox.Escalation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GroupEscalationFooter
	{
		public GroupEscalationFooter(string groupDisplayName, CultureInfo cultureInfo, EscalationLinkBuilder linkBuilder)
		{
			Util.ThrowOnNullArgument(groupDisplayName, "groupDisplayName");
			Util.ThrowOnNullArgument(cultureInfo, "cultureInfo");
			Util.ThrowOnNullArgument(linkBuilder, "linkBuilder");
			this.groupDisplayName = groupDisplayName;
			this.cultureInfo = cultureInfo;
			this.lastLinkBuildTimeMs = 0L;
			this.lastLinkOnBodyDetectionTimeMs = 0L;
			this.lastLinkInsertOnBodyTimeMs = 0L;
			this.lastBodySizeBytes = 0L;
			this.linkBuilder = linkBuilder;
		}

		public long LastLinkBuildTimeMs
		{
			get
			{
				return this.lastLinkBuildTimeMs;
			}
		}

		public long LastLinkOnBodyDetectionTimeMs
		{
			get
			{
				return this.lastLinkOnBodyDetectionTimeMs;
			}
		}

		public long LastLinkInsertOnBodyTimeMs
		{
			get
			{
				return this.lastLinkInsertOnBodyTimeMs;
			}
		}

		public long LastBodySizeBytes
		{
			get
			{
				return this.lastBodySizeBytes;
			}
		}

		public bool InsertFooterToTheBody(IMessageItem originalMessage, IMessageItem escalatedMessage)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			string escalationLink = this.linkBuilder.GetEscalationLink(EscalationLinkType.Unsubscribe);
			stopwatch.Stop();
			this.lastLinkBuildTimeMs = stopwatch.ElapsedMilliseconds;
			if (!originalMessage.IBody.IsBodyDefined || this.BodyContainsFooter(originalMessage.IBody, escalationLink))
			{
				return false;
			}
			stopwatch = new Stopwatch();
			stopwatch.Start();
			bool flag = this.InsertLinkToBodyFooter(escalationLink, originalMessage.IBody, escalatedMessage.IBody);
			if (flag)
			{
				escalatedMessage.StampMessageBodyTag();
			}
			stopwatch.Stop();
			this.lastLinkInsertOnBodyTimeMs = stopwatch.ElapsedMilliseconds;
			return flag;
		}

		private string GetHtmlLink(string unsubscribeUrl)
		{
			string link = string.Format("<a id='{0}' href=\"{1}\">{2}</a>", "BD5134C6-8D33-4ABA-A0C4-08581FDF89DB", unsubscribeUrl, ClientStrings.GroupSubscriptionUnsubscribeLinkWord.ToString(this.cultureInfo));
			string groupName = AntiXssEncoder.HtmlEncode(this.groupDisplayName, false);
			return "<br /><div style=\"display:inline-block\" ><table border=\"0\" cellspacing=\"0\" style=\"background-color:#F4F4F4;\" ><tr><td style=\"padding:20px; font-size:12px; color:#666666\" >" + ClientStrings.GroupSubscriptionUnsubscribeInfoHtml(groupName, link).ToString(this.cultureInfo) + "</tr></td></table></div>";
		}

		private string GetPlainTextLink(string unsubscribeUrl)
		{
			return "\n\n" + ClientStrings.GroupSubscriptionUnsubscribeInfoText(this.groupDisplayName, unsubscribeUrl).ToString(this.cultureInfo);
		}

		protected virtual bool InsertLinkToBodyFooter(string unsubscribeUrl, IBody originalBody, IBody escalatedMessageBody)
		{
			BodyInjectionFormat injectionFormat;
			string suffixInjectionText;
			switch (originalBody.Format)
			{
			case BodyFormat.TextPlain:
				injectionFormat = BodyInjectionFormat.Text;
				suffixInjectionText = this.GetPlainTextLink(unsubscribeUrl);
				break;
			case BodyFormat.TextHtml:
			case BodyFormat.ApplicationRtf:
				injectionFormat = BodyInjectionFormat.Html;
				suffixInjectionText = this.GetHtmlLink(unsubscribeUrl);
				break;
			default:
				throw new ArgumentException("Unsupported body format: " + originalBody.Format);
			}
			originalBody.CopyBodyInjectingText(escalatedMessageBody, injectionFormat, null, suffixInjectionText);
			return true;
		}

		protected virtual bool BodyContainsFooter(IBody originalMessageBody, string link)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			bool result = false;
			string text;
			this.lastBodySizeBytes = (long)originalMessageBody.GetLastNBytesAsString(4096, out text);
			if (this.lastBodySizeBytes > 0L)
			{
				if (originalMessageBody.Format == BodyFormat.TextHtml)
				{
					text = HttpUtility.HtmlDecode(text);
				}
				if (text.Contains(link))
				{
					result = true;
				}
			}
			stopwatch.Stop();
			this.lastLinkOnBodyDetectionTimeMs = stopwatch.ElapsedMilliseconds;
			return result;
		}

		public const string GroupEscalationUnsubscribeLinkId = "BD5134C6-8D33-4ABA-A0C4-08581FDF89DB";

		public const int FooterLinkScanSize = 4096;

		private readonly string groupDisplayName;

		private EscalationLinkBuilder linkBuilder;

		private CultureInfo cultureInfo;

		private long lastLinkBuildTimeMs;

		private long lastLinkOnBodyDetectionTimeMs;

		private long lastLinkInsertOnBodyTimeMs;

		private long lastBodySizeBytes;
	}
}

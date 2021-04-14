using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ResolverMessage
	{
		public ResolverMessage(EmailMessage message, long mimeSize)
		{
			this.emailMessage = message;
			this.headers = this.emailMessage.MimeDocument.RootPart.Headers;
			this.messageType = new ResolverMessageType?(this.GetMessageType());
			this.originalMessageSize = ResolverMessage.GetOriginalMessageSize(this.headers, mimeSize);
		}

		public ResolverMessageType Type
		{
			get
			{
				return this.messageType.Value;
			}
		}

		public bool AutoForwarded
		{
			get
			{
				bool flag;
				this.emailMessage.TryGetMapiProperty<bool>(TnefPropertyTag.AutoForwarded, out flag);
				if (flag)
				{
					return true;
				}
				bool result = false;
				Header header = this.headers.FindFirst("X-MS-Exchange-Organization-AutoForwarded");
				if (header != null)
				{
					bool.TryParse(header.Value, out result);
				}
				return result;
			}
		}

		public AutoResponseSuppress AutoResponseSuppress
		{
			get
			{
				if (this.suppress == null)
				{
					this.suppress = new AutoResponseSuppress?(this.GetAutoResponseSuppress());
				}
				return this.suppress.Value;
			}
			set
			{
				if ((value & AutoResponseSuppress.RN) != (AutoResponseSuppress)0)
				{
					this.headers.RemoveAll(HeaderId.DispositionNotificationTo);
					this.headers.RemoveAll(HeaderId.ReturnReceiptTo);
				}
				if (this.suppress != value)
				{
					this.SetAutoResponseSuppress(value);
					this.suppress = new AutoResponseSuppress?(value);
				}
			}
		}

		public bool RecipientLimitVerified
		{
			get
			{
				return this.headers.FindFirst("X-MS-Exchange-Organization-Recipient-Limit-Verified") != null;
			}
			set
			{
				this.headers.RemoveAll("X-MS-Exchange-Organization-Recipient-Limit-Verified");
				if (value)
				{
					Header newChild = new AsciiTextHeader("X-MS-Exchange-Organization-Recipient-Limit-Verified", "True");
					this.headers.AppendChild(newChild);
				}
			}
		}

		public History History
		{
			get
			{
				if (!this.loadedHistory)
				{
					this.history = History.ReadFrom(this.headers);
					this.loadedHistory = true;
				}
				return this.history;
			}
		}

		public bool RedirectHandled
		{
			get
			{
				if (!this.loadedRedirectHandled)
				{
					Header header = this.headers.FindFirst("X-MS-Exchange-Organization-Recipient-Redirection-Handled");
					this.redirectHandled = (null != header);
					this.loadedRedirectHandled = true;
				}
				return this.redirectHandled;
			}
			set
			{
				this.headers.RemoveAll("X-MS-Exchange-Organization-Recipient-Redirection-Handled");
				if (value)
				{
					Header newChild = new AsciiTextHeader("X-MS-Exchange-Organization-Recipient-Redirection-Handled", "True");
					this.headers.AppendChild(newChild);
				}
				this.redirectHandled = value;
				this.loadedRedirectHandled = true;
			}
		}

		public bool SuppressRecallReport
		{
			get
			{
				Header header = this.headers.FindFirst("X-MS-Exchange-Send-Outlook-Recall-Report");
				return header != null && header.Value.Equals("False", StringComparison.OrdinalIgnoreCase);
			}
			set
			{
				ResolverMessage.SetSuppressRecallReport(this.headers, value);
			}
		}

		public bool BypassChildModeration
		{
			get
			{
				Header header = this.headers.FindFirst("X-MS-Exchange-Organization-Bypass-Child-Moderation");
				return header != null && header.Value.Equals("True", StringComparison.OrdinalIgnoreCase);
			}
			set
			{
				this.headers.RemoveAll("X-MS-Exchange-Organization-Bypass-Child-Moderation");
				if (value)
				{
					Header newChild = new AsciiTextHeader("X-MS-Exchange-Organization-Bypass-Child-Moderation", "True");
					this.headers.AppendChild(newChild);
				}
			}
		}

		public long OriginalMessageSize
		{
			get
			{
				return this.originalMessageSize;
			}
		}

		internal bool DlExpansionProhibited
		{
			get
			{
				if (this.dlExpansionProhibited == null)
				{
					Header header = this.headers.FindFirst("X-MS-Exchange-Organization-DL-Expansion-Prohibited");
					this.dlExpansionProhibited = new bool?(header != null);
				}
				return this.dlExpansionProhibited.Value;
			}
		}

		internal bool AltRecipientProhibited
		{
			get
			{
				if (this.altRecipientProhibited == null)
				{
					Header header = this.headers.FindFirst("X-MS-Exchange-Organization-Alt-Recipient-Prohibited");
					this.altRecipientProhibited = new bool?(header != null);
				}
				return this.altRecipientProhibited.Value;
			}
		}

		public static long GetOriginalMessageSize(HeaderList headers, long mimeSize)
		{
			Header header = headers.FindFirst("X-MS-Exchange-Organization-OriginalSize");
			if (header != null)
			{
				long num = 0L;
				if (long.TryParse(header.Value, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out num) && num >= 0L)
				{
					return num;
				}
				ExTraceGlobals.ResolverTracer.TraceError<Header>(0L, "Failed to parse original-message-size header '{0}'; deleting it", header);
				headers.RemoveAll("X-MS-Exchange-Organization-OriginalSize");
			}
			headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Organization-OriginalSize", mimeSize.ToString(NumberFormatInfo.InvariantInfo)));
			ExTraceGlobals.ResolverTracer.TraceDebug<long>(0L, "Stamped original-message-size header with value {0}", mimeSize);
			return mimeSize;
		}

		public static string FormatAutoResponseSuppressHeaderValue(AutoResponseSuppress suppress)
		{
			return ResolverMessage.autoResponseSuppressFormatter.Format(suppress);
		}

		public static void SetSuppressRecallReport(HeaderList headers, bool suppress)
		{
			Header header = headers.FindFirst("X-MS-Exchange-Send-Outlook-Recall-Report");
			if (!suppress)
			{
				if (header != null)
				{
					headers.RemoveAll("X-MS-Exchange-Send-Outlook-Recall-Report");
				}
				return;
			}
			if (header != null)
			{
				header.Value = "False";
				return;
			}
			headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Send-Outlook-Recall-Report", "False"));
		}

		public void AddResentFrom(string resentFrom)
		{
			Header newChild = AddressHeader.Parse("Resent-From", resentFrom, AddressParserFlags.None);
			this.headers.PrependChild(newChild);
		}

		public void ClearHistory(TransportMailItem transportMailItem)
		{
			History.Clear(transportMailItem);
			this.history = null;
			this.loadedHistory = true;
		}

		private static AutoResponseSuppress ParseAutoResponseSuppressHeaderValue(string value)
		{
			AutoResponseSuppress autoResponseSuppress = (AutoResponseSuppress)0;
			AutoResponseSuppress result = (AutoResponseSuppress)0;
			if (EnumValidator<AutoResponseSuppress>.TryParse(value, EnumParseOptions.IgnoreCase, out autoResponseSuppress))
			{
				result = autoResponseSuppress;
			}
			return result;
		}

		private ResolverMessageType GetMessageType()
		{
			string text = this.emailMessage.MapiMessageClass;
			ExTraceGlobals.ResolverTracer.TraceDebug<string>((long)this.GetHashCode(), "GetMessageType:Class = {0}", text);
			if (text == null)
			{
				return ResolverMessageType.Note;
			}
			if (text.StartsWith("envelope.", StringComparison.OrdinalIgnoreCase))
			{
				text = text.Substring("envelope.".Length);
			}
			if (text.StartsWith("IPM.Note.Rules.OofTemplate.", StringComparison.OrdinalIgnoreCase))
			{
				int num;
				if (!this.emailMessage.TryGetMapiProperty<int>(TnefPropertyTag.OofReplyType, out num))
				{
					num = 2;
				}
				if (num == 2)
				{
					return ResolverMessageType.InternalOOF;
				}
				return ResolverMessageType.LegacyOOF;
			}
			else
			{
				if (text.StartsWith("IPM.Note.Rules.ExternalOofTemplate.", StringComparison.OrdinalIgnoreCase))
				{
					return ResolverMessageType.ExternalOOF;
				}
				if (text.StartsWith("IPM.Note.Rules.ReplyTemplate.", StringComparison.OrdinalIgnoreCase))
				{
					return ResolverMessageType.AutoReply;
				}
				if (ObjectClass.IsOutlookRecall(text))
				{
					return ResolverMessageType.Recall;
				}
				if (text.StartsWith("IPM.Recall.Report.", StringComparison.OrdinalIgnoreCase))
				{
					return ResolverMessageType.RecallReport;
				}
				if (ObjectClass.IsMeetingForwardNotification(text))
				{
					return ResolverMessageType.MeetingForwardNotification;
				}
				if (ObjectClass.IsUMMessage(text))
				{
					return ResolverMessageType.UM;
				}
				if (!text.StartsWith("report.", StringComparison.OrdinalIgnoreCase))
				{
					return ResolverMessageType.Note;
				}
				if (text.EndsWith(".ipnrn", StringComparison.OrdinalIgnoreCase))
				{
					return ResolverMessageType.RN;
				}
				if (text.EndsWith(".ipnnrn", StringComparison.OrdinalIgnoreCase))
				{
					return ResolverMessageType.NRN;
				}
				if (text.EndsWith("delayed.dr", StringComparison.OrdinalIgnoreCase))
				{
					return ResolverMessageType.DelayedDSN;
				}
				if (text.EndsWith("relayed.dr", StringComparison.OrdinalIgnoreCase))
				{
					return ResolverMessageType.RelayedDSN;
				}
				if (text.EndsWith("expanded.dr", StringComparison.OrdinalIgnoreCase))
				{
					return ResolverMessageType.ExpandedDSN;
				}
				if (text.EndsWith(".dr", StringComparison.OrdinalIgnoreCase))
				{
					return ResolverMessageType.DR;
				}
				if (text.EndsWith(".ndr", StringComparison.OrdinalIgnoreCase))
				{
					return ResolverMessageType.NDR;
				}
				return ResolverMessageType.Note;
			}
		}

		private AutoResponseSuppress GetAutoResponseSuppress()
		{
			AutoResponseSuppress autoResponseSuppress = (AutoResponseSuppress)0;
			foreach (Header header in this.headers)
			{
				try
				{
					if (string.Equals(header.Name, "X-Auto-Response-Suppress", StringComparison.OrdinalIgnoreCase))
					{
						autoResponseSuppress |= ResolverMessage.ParseAutoResponseSuppressHeaderValue(header.Value);
					}
				}
				catch (ExchangeDataException)
				{
				}
			}
			return autoResponseSuppress;
		}

		private void SetAutoResponseSuppress(AutoResponseSuppress suppress)
		{
			Header[] array = this.headers.FindAll("X-Auto-Response-Suppress");
			if (array.Length == 0)
			{
				Header newChild = new AsciiTextHeader("X-Auto-Response-Suppress", ResolverMessage.FormatAutoResponseSuppressHeaderValue(suppress));
				this.headers.AppendChild(newChild);
				return;
			}
			array[0].Value = ResolverMessage.FormatAutoResponseSuppressHeaderValue(suppress);
			for (int i = 1; i < array.Length; i++)
			{
				this.headers.RemoveChild(array[i]);
			}
		}

		private const string AutoResponseSuppressHeaderName = "X-Auto-Response-Suppress";

		private const string SendRecallReportHeaderName = "X-MS-Exchange-Send-Outlook-Recall-Report";

		private const string RecipientRedirectionHandledHeaderName = "X-MS-Exchange-Organization-Recipient-Redirection-Handled";

		private const string RecipientLimitVerifiedHeaderName = "X-MS-Exchange-Organization-Recipient-Limit-Verified";

		private const string BypassChildModerationHeaderName = "X-MS-Exchange-Organization-Bypass-Child-Moderation";

		private static AutoResponseSuppressFormatter autoResponseSuppressFormatter = new AutoResponseSuppressFormatter();

		private EmailMessage emailMessage;

		private HeaderList headers;

		private ResolverMessageType? messageType = null;

		private AutoResponseSuppress? suppress = null;

		private bool? dlExpansionProhibited = null;

		private bool? altRecipientProhibited = null;

		private History history;

		private bool loadedHistory;

		private bool redirectHandled;

		private bool loadedRedirectHandled;

		private long originalMessageSize;
	}
}

using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class MessageTemplate : IEquatable<MessageTemplate>, IComparable<MessageTemplate>
	{
		public MessageTemplate(string reversePath, AutoResponseSuppress suppress, string resentFrom, bool transmitHistory, bool redirectHandled, bool suppressRecallReport, bool bypassChildModeration)
		{
			this.reversePath = reversePath;
			this.suppress = suppress;
			this.resentFrom = resentFrom;
			this.transmitHistory = transmitHistory;
			this.redirectHandled = redirectHandled;
			this.suppressRecallReport = suppressRecallReport;
			this.bypassChildModeration = bypassChildModeration;
			this.formatted = this.Format();
		}

		public string ReversePath
		{
			get
			{
				return this.reversePath;
			}
		}

		public bool TransmitHistory
		{
			get
			{
				return this.transmitHistory;
			}
		}

		public bool BypassChildModeration
		{
			get
			{
				return this.bypassChildModeration;
			}
		}

		public static bool operator ==(MessageTemplate a, MessageTemplate b)
		{
			return a == b || (a != null && b != null && string.Equals(a.formatted, b.formatted, StringComparison.OrdinalIgnoreCase));
		}

		public static bool operator !=(MessageTemplate a, MessageTemplate b)
		{
			return !(a == b);
		}

		public static MessageTemplate ReadFrom(MailRecipient recipient)
		{
			string text;
			if (!recipient.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.Transport.MessageTemplate", out text) || string.IsNullOrEmpty(text))
			{
				return MessageTemplate.Default;
			}
			return MessageTemplate.Parse(text);
		}

		public MessageTemplate Merge(string reversePath, AutoResponseSuppress suppress, string resentFrom, bool transmitHistory, bool redirectHandled, bool suppressRecallReport, bool bypassChildModerationFlag)
		{
			return new MessageTemplate(reversePath ?? this.reversePath, suppress | this.suppress, resentFrom ?? this.resentFrom, transmitHistory, redirectHandled || this.redirectHandled, suppressRecallReport || this.suppressRecallReport, bypassChildModerationFlag || this.bypassChildModeration);
		}

		public MessageTemplate Derive(MessageTemplate other)
		{
			return this.Merge(other.reversePath, other.suppress, other.resentFrom, other.transmitHistory, other.redirectHandled, other.suppressRecallReport, other.bypassChildModeration);
		}

		public void WriteTo(MailRecipient recipient)
		{
			recipient.ExtendedProperties.SetValue<string>("Microsoft.Exchange.Transport.MessageTemplate", this.formatted);
		}

		public void ApplyTo(TransportMailItem mailItem, ResolverMessage message)
		{
			if (this.reversePath != null)
			{
				mailItem.From = (RoutingAddress)this.reversePath;
			}
			if (this.suppress != (AutoResponseSuppress)0)
			{
				message.AutoResponseSuppress |= this.suppress;
			}
			if (this.resentFrom != null)
			{
				message.AddResentFrom(this.resentFrom);
			}
			if (!this.transmitHistory)
			{
				message.ClearHistory(mailItem);
			}
			if (this.redirectHandled)
			{
				message.RedirectHandled = true;
			}
			if (this.suppressRecallReport)
			{
				message.SuppressRecallReport = true;
			}
			if (this.bypassChildModeration)
			{
				message.BypassChildModeration = true;
			}
		}

		public void Normalize(ResolverMessage message)
		{
			if (!this.transmitHistory && message.History == null)
			{
				this.transmitHistory = true;
			}
			this.formatted = this.Format();
		}

		public bool Equals(MessageTemplate other)
		{
			return this == other;
		}

		public int CompareTo(MessageTemplate other)
		{
			return string.CompareOrdinal(this.formatted, other.formatted);
		}

		public override int GetHashCode()
		{
			return this.formatted.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this == obj as MessageTemplate;
		}

		private static void FormatField(StringBuilder s, string name, string value)
		{
			s.Append(name);
			s.Append(": ");
			s.AppendLine(value);
		}

		private static bool ParseField(string line, out string name, out string value)
		{
			int num = line.IndexOf(':');
			if (num == -1)
			{
				name = null;
				value = null;
				return false;
			}
			name = line.Substring(0, num);
			value = line.Substring(num + 1).Trim();
			return true;
		}

		private static MessageTemplate Parse(string s)
		{
			string text = null;
			AutoResponseSuppress autoResponseSuppress = (AutoResponseSuppress)0;
			string text2 = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			MessageTemplate result;
			using (StringReader stringReader = new StringReader(s))
			{
				for (string line = stringReader.ReadLine(); line != null; line = stringReader.ReadLine())
				{
					string a;
					string text3;
					if (MessageTemplate.ParseField(line, out a, out text3))
					{
						if (string.Equals(a, "ReversePath", StringComparison.OrdinalIgnoreCase))
						{
							text = text3;
						}
						else if (string.Equals(a, "AutoResponseSuppress", StringComparison.OrdinalIgnoreCase))
						{
							AutoResponseSuppress autoResponseSuppress2 = (AutoResponseSuppress)0;
							if (EnumValidator<AutoResponseSuppress>.TryParse(text3, EnumParseOptions.IgnoreCase, out autoResponseSuppress2))
							{
								autoResponseSuppress = autoResponseSuppress2;
							}
							else
							{
								autoResponseSuppress = (AutoResponseSuppress)0;
							}
						}
						else if (string.Equals(a, "ResentFrom", StringComparison.OrdinalIgnoreCase))
						{
							text2 = text3;
						}
						else if (string.Equals(a, "TransmitHistory", StringComparison.OrdinalIgnoreCase))
						{
							bool.TryParse(text3, out flag);
						}
						else if (string.Equals(a, "RedirectHandled", StringComparison.OrdinalIgnoreCase))
						{
							bool.TryParse(text3, out flag2);
						}
						else if (string.Equals(a, "SuppressRecallReport", StringComparison.OrdinalIgnoreCase))
						{
							bool.TryParse(text3, out flag3);
						}
						else if (string.Equals(a, "BypassChildModeration", StringComparison.OrdinalIgnoreCase))
						{
							bool.TryParse(text3, out flag4);
						}
					}
				}
				result = new MessageTemplate(text, autoResponseSuppress, text2, flag, flag2, flag3, flag4);
			}
			return result;
		}

		private string Format()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.reversePath != null)
			{
				MessageTemplate.FormatField(stringBuilder, "ReversePath", this.reversePath);
			}
			MessageTemplate.FormatField(stringBuilder, "AutoResponseSuppress", ResolverMessage.FormatAutoResponseSuppressHeaderValue(this.suppress));
			if (this.resentFrom != null)
			{
				MessageTemplate.FormatField(stringBuilder, "ResentFrom", this.resentFrom);
			}
			MessageTemplate.FormatField(stringBuilder, "TransmitHistory", this.transmitHistory ? "True" : "False");
			if (this.redirectHandled)
			{
				MessageTemplate.FormatField(stringBuilder, "RedirectHandled", "True");
			}
			if (this.suppressRecallReport)
			{
				MessageTemplate.FormatField(stringBuilder, "SuppressRecallReport", "True");
			}
			if (this.bypassChildModeration)
			{
				MessageTemplate.FormatField(stringBuilder, "BypassChildModeration", "True");
			}
			return stringBuilder.ToString();
		}

		public const string BoolHeaderSetValue = "True";

		public const string BoolHeaderNotSetValue = "False";

		public const string TemplateProperty = "Microsoft.Exchange.Transport.MessageTemplate";

		public static readonly MessageTemplate Default = new MessageTemplate(null, (AutoResponseSuppress)0, null, true, false, false, false);

		public static readonly MessageTemplate StripHistory = new MessageTemplate(null, (AutoResponseSuppress)0, null, false, false, false, false);

		private string reversePath;

		private AutoResponseSuppress suppress;

		private string resentFrom;

		private bool transmitHistory;

		private bool redirectHandled;

		private bool suppressRecallReport;

		private bool bypassChildModeration;

		private string formatted;

		private static class Fields
		{
			public const string ReversePath = "ReversePath";

			public const string AutoResponseSuppress = "AutoResponseSuppress";

			public const string ResentFrom = "ResentFrom";

			public const string TransmitHistory = "TransmitHistory";

			public const string RedirectHandled = "RedirectHandled";

			public const string SuppressRecallReport = "SuppressRecallReport";

			public const string BypassChildModeration = "BypassChildModeration";
		}
	}
}

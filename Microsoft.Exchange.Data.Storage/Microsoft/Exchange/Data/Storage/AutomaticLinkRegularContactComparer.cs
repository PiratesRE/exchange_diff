using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AutomaticLinkRegularContactComparer : AutomaticLinkContactComparer
	{
		private AutomaticLinkRegularContactComparer()
		{
		}

		internal static AutomaticLinkRegularContactComparer Instance
		{
			get
			{
				return AutomaticLinkRegularContactComparer.instance;
			}
		}

		internal override ContactLinkingOperation Match(IContactLinkingMatchProperties contact1, IContactLinkingMatchProperties contact2)
		{
			if (base.MatchEmails(contact1, contact2))
			{
				return ContactLinkingOperation.AutoLinkViaEmailAddress;
			}
			if (this.MatchIMAddresses(contact1, contact2))
			{
				return ContactLinkingOperation.AutoLinkViaIMAddress;
			}
			return ContactLinkingOperation.None;
		}

		private bool MatchIMAddresses(IContactLinkingMatchProperties contact1, IContactLinkingMatchProperties contact2)
		{
			string text = this.RemoveSipPrefix(contact1.IMAddress);
			if (text != null && SmtpAddress.IsValidSmtpAddress(text))
			{
				string s = this.RemoveSipPrefix(contact2.IMAddress);
				return base.EqualsIgnoreCaseAndWhiteSpace(text, s);
			}
			return false;
		}

		private string RemoveSipPrefix(string imAddress)
		{
			string text = imAddress;
			if (!string.IsNullOrEmpty(text) && text.StartsWith("sip:", StringComparison.OrdinalIgnoreCase))
			{
				text = text.Substring("sip:".Length);
			}
			return text;
		}

		internal const string SipPrefix = "sip:";

		private static readonly AutomaticLinkRegularContactComparer instance = new AutomaticLinkRegularContactComparer();
	}
}

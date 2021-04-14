using System;
using System.IO;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class AutoCompleteCacheEntry
	{
		public static void RenderEntryJavascript(TextWriter writer, RecipientInfoCacheEntry entry)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("new Recip('");
			Utilities.JavascriptEncode((entry.DisplayName == null) ? string.Empty : entry.DisplayName, writer);
			writer.Write("','");
			Utilities.JavascriptEncode((entry.RoutingAddress == null) ? string.Empty : entry.RoutingAddress, writer);
			writer.Write("','");
			Utilities.JavascriptEncode((entry.SmtpAddress == null) ? string.Empty : entry.SmtpAddress, writer);
			writer.Write("','");
			Utilities.JavascriptEncode((entry.Alias == null) ? string.Empty : entry.Alias, writer);
			writer.Write("','");
			Utilities.JavascriptEncode(entry.RoutingType, writer);
			writer.Write("',{0},'", (int)entry.AddressOrigin);
			Utilities.JavascriptEncode((entry.ItemId == null) ? string.Empty : entry.ItemId, writer);
			writer.Write("',{0},", entry.RecipientFlags);
			writer.Write("{0},'", (int)entry.EmailAddressIndex);
			Utilities.JavascriptEncode((entry.SipUri == null) ? string.Empty : entry.SipUri, writer);
			writer.Write("','");
			Utilities.JavascriptEncode((entry.MobilePhoneNumber == null) ? string.Empty : entry.MobilePhoneNumber, writer);
			writer.Write("')");
		}

		public static RecipientInfoCacheEntry ParseClientEntry(RecipientInfoAC entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			if (!Utilities.IsMapiPDL(entry.RoutingType) && string.IsNullOrEmpty(entry.RoutingAddress))
			{
				return null;
			}
			return new RecipientInfoCacheEntry(entry.DisplayName, entry.SmtpAddress, entry.RoutingAddress, entry.Alias, entry.RoutingType, entry.AddressOrigin, entry.RecipientFlags, entry.ItemId, entry.EmailAddressIndex, entry.SipUri, entry.MobilePhoneNumber);
		}

		public static RecipientInfoCacheEntry ParseLogonExchangePrincipal(ExchangePrincipal principal, string sipUri, string mobilePhoneNumber)
		{
			if (principal == null)
			{
				throw new ArgumentNullException("principal");
			}
			string alias = principal.MailboxInfo.IsAggregated ? string.Empty : principal.Alias;
			return new RecipientInfoCacheEntry(principal.MailboxInfo.DisplayName, principal.MailboxInfo.PrimarySmtpAddress.ToString(), principal.LegacyDn, alias, "EX", AddressOrigin.Directory, 0, null, EmailAddressIndex.None, sipUri, mobilePhoneNumber);
		}
	}
}

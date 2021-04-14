using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal static class Utils
	{
		internal static ProxyAddress RoutingAddressToProxyAddress(string routingAddress)
		{
			ProxyAddress proxyAddress;
			if (SmtpProxyAddress.IsEncapsulatedAddress(routingAddress))
			{
				if (!SmtpProxyAddress.TryDeencapsulate(routingAddress, out proxyAddress))
				{
					ExTraceGlobals.JournalingTracer.TraceError(0L, "Failed to de-encapsulate recipient address");
					throw new ArgumentException(string.Format("Could not de-encapsulate: {0}", routingAddress));
				}
			}
			else
			{
				proxyAddress = ProxyAddress.Parse(ProxyAddressPrefix.Smtp.PrimaryPrefix, routingAddress);
			}
			if (proxyAddress is InvalidProxyAddress)
			{
				ExTraceGlobals.JournalingTracer.TraceError(0L, "Recipient address was an invalid proxy address");
				throw new ArgumentException(string.Format("{0} was unable to be parsed as a RoutingAddress", routingAddress));
			}
			return proxyAddress;
		}

		internal static object[] ADLookupUser(MailItem mailItem, ProxyAddress proxyAddress, params PropertyDefinition[] propertiesToGet)
		{
			IADRecipientCache iadrecipientCache = (IADRecipientCache)((ITransportMailItemWrapperFacade)mailItem).TransportMailItem.ADRecipientCacheAsObject;
			Result<ADRawEntry> result = default(Result<ADRawEntry>);
			result = iadrecipientCache.FindAndCacheRecipient(proxyAddress);
			if (result.Error == ProviderError.NotFound || result.Data == null)
			{
				return null;
			}
			return Utils.GetADProperties(result, propertiesToGet);
		}

		internal static List<object[]> ADLookupUsers(MailItem mailItem, List<ProxyAddress> proxyAddresses, params PropertyDefinition[] propertiesToGet)
		{
			IADRecipientCache iadrecipientCache = (IADRecipientCache)((ITransportMailItemWrapperFacade)mailItem).TransportMailItem.ADRecipientCacheAsObject;
			IList<Result<ADRawEntry>> list = iadrecipientCache.FindAndCacheRecipients(proxyAddresses);
			List<object[]> list2 = new List<object[]>(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				Result<ADRawEntry> result = list[i];
				object[] item;
				if (result.Error == ProviderError.NotFound || result.Data == null)
				{
					item = null;
				}
				else
				{
					item = Utils.GetADProperties(list[i], propertiesToGet);
				}
				list2.Add(item);
			}
			return list2;
		}

		private static object[] GetADProperties(Result<ADRawEntry> result, params PropertyDefinition[] propertiesToGet)
		{
			object[] properties = result.Data.GetProperties(propertiesToGet);
			if (properties == null || properties.Length != propertiesToGet.Length)
			{
				ExTraceGlobals.JournalingTracer.TraceError(0L, "Failed to get mandatory recipient properties from AD");
				throw new ArgumentException(string.Format("Expected {0} properties in AD for recipient, but found only {1}", propertiesToGet.Length, (properties == null) ? 0 : properties.Length));
			}
			return properties;
		}

		internal static bool IsNdr(MailItem mailItem)
		{
			return mailItem.FromAddress == RoutingAddress.NullReversePath && Utils.IsNdr(mailItem.Message.RootPart);
		}

		internal static bool IsNdr(MimePart mimePart)
		{
			if (mimePart == null || mimePart.Headers == null)
			{
				return false;
			}
			HeaderList headers = mimePart.Headers;
			ContentTypeHeader contentTypeHeader = (ContentTypeHeader)headers.FindFirst(HeaderId.ContentType);
			return "multipart/report".Equals(contentTypeHeader.Value, StringComparison.OrdinalIgnoreCase);
		}

		internal static bool IsMessageAttachment(Attachment attachment)
		{
			return attachment != null && !string.IsNullOrEmpty(attachment.ContentType) && attachment.ContentType.Equals("message/rfc822");
		}

		internal static bool IsJournalReport(MimePart mimePart)
		{
			if (mimePart == null || mimePart.Headers == null)
			{
				return false;
			}
			HeaderList headers = mimePart.Headers;
			TextHeader textHeader = headers.FindFirst("X-MS-Journal-Report") as TextHeader;
			return textHeader != null;
		}

		internal static string[] ParseJournaledToHeader(MailItem mailItem)
		{
			HeaderList headers = mailItem.Message.RootPart.Headers;
			TextHeader textHeader = headers.FindFirst("X-MS-Exchange-Organization-Journaled-To-Recipients") as TextHeader;
			ExTraceGlobals.JournalingTracer.TraceDebug<string>(0L, "XMSExchangeJournaledToRecipients = {0}", (textHeader == null) ? "<null>" : textHeader.Value);
			if (textHeader == null || string.IsNullOrEmpty(textHeader.Value))
			{
				return null;
			}
			string[] array = textHeader.Value.Split(new char[]
			{
				'+'
			});
			foreach (string text in array)
			{
				if (!RoutingAddress.IsValidAddress(text))
				{
					ExTraceGlobals.JournalingTracer.TraceError<string>(0L, "Invalid SMTP address: {0}", text);
					return null;
				}
			}
			return array;
		}

		internal static void WriteJournaledToHeader(MailItem mailItem, List<string> journalTargetRecips)
		{
			if (journalTargetRecips == null || journalTargetRecips.Count == 0)
			{
				return;
			}
			HeaderList headers = mailItem.Message.RootPart.Headers;
			headers.RemoveAll("X-MS-Exchange-Organization-Journaled-To-Recipients");
			StringBuilder stringBuilder = new StringBuilder(mailItem.Recipients.Count * 32);
			bool flag = true;
			foreach (string value in journalTargetRecips)
			{
				if (!flag)
				{
					stringBuilder.Append('+');
				}
				else
				{
					flag = false;
				}
				stringBuilder.Append(value);
			}
			string text = stringBuilder.ToString();
			ExTraceGlobals.JournalingTracer.TraceDebug<string>(0L, "Writing XMSExchangeJournaledToRecipients: {0}", text);
			Header newChild = new TextHeader("X-MS-Exchange-Organization-Journaled-To-Recipients", text);
			mailItem.Message.RootPart.Headers.AppendChild(newChild);
		}

		internal static bool IsSmtpAddressSenderOrRecipientOnMessage(SmtpAddress address, MailItem mailItem, UserComparer userComparer)
		{
			if (address == SmtpAddress.Empty)
			{
				return false;
			}
			if (mailItem.Message.From != null && RuleUtils.CompareStringValues(address.ToString(), mailItem.Message.From.SmtpAddress, userComparer, ConditionEvaluationMode.Optimized, null))
			{
				return true;
			}
			foreach (EnvelopeRecipient envelopeRecipient in mailItem.Recipients)
			{
				if (RuleUtils.CompareStringValues(address.ToString(), envelopeRecipient.Address.ToString(), userComparer, ConditionEvaluationMode.Optimized, null))
				{
					return true;
				}
			}
			return false;
		}

		internal static void AddRecipSortedToList(string recipientEmail, ref List<string> sortedRecipientList)
		{
			if (sortedRecipientList == null)
			{
				sortedRecipientList = new List<string>();
			}
			int num = sortedRecipientList.BinarySearch(recipientEmail, StringComparer.OrdinalIgnoreCase);
			if (num < 0)
			{
				sortedRecipientList.Insert(~num, recipientEmail);
			}
		}

		internal static bool TryGetADRawEntryByEmailAddress(IADRecipientCache cache, string email, out ADRawEntry recipientEntry)
		{
			recipientEntry = null;
			Result<ADRawEntry> result = default(Result<ADRawEntry>);
			if (ProxyAddressBase.IsAddressStringValid(email) && RoutingAddress.IsValidAddress(email))
			{
				SmtpProxyAddress proxyAddress = new SmtpProxyAddress(email, true);
				result = cache.FindAndCacheRecipient(proxyAddress);
				recipientEntry = result.Data;
			}
			return recipientEntry != null && result.Error != ProviderError.NotFound;
		}

		internal static bool IsAuthoritativeDomain(string address, OrganizationId orgId)
		{
			if (SmtpAddress.IsValidSmtpAddress(address))
			{
				AcceptedDomainTable acceptedDomainTable = Components.Configuration.GetAcceptedDomainTable(orgId).AcceptedDomainTable;
				SmtpAddress smtpAddress = new SmtpAddress(address);
				return acceptedDomainTable.CheckAuthoritative(new SmtpDomain(smtpAddress.Domain));
			}
			return false;
		}

		private const char JournalToSeparator = '+';
	}
}

using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal class ResendRecipientInfo
	{
		private ResendRecipientInfo()
		{
		}

		internal ProxyAddressCollection ProxyAddresses
		{
			get
			{
				return this.proxyAddresses;
			}
		}

		internal static ResendRecipientInfo Lookup(MailItem mailItem, string routingAddress)
		{
			ExTraceGlobals.JournalingTracer.TraceDebug<string>(0L, "Querying AD-recipient cache for {0}", routingAddress);
			ResendRecipientInfo resendRecipientInfo = null;
			ProxyAddress proxyAddress = Utils.RoutingAddressToProxyAddress(routingAddress);
			object[] array = Utils.ADLookupUser(mailItem, proxyAddress, ResendRecipientInfo.recipientProperties);
			if (array == null)
			{
				ExTraceGlobals.JournalingTracer.TraceDebug(0L, "Recipient is a one-off, not found in AD");
				resendRecipientInfo = new ResendRecipientInfo();
				resendRecipientInfo.proxyAddresses.Add(proxyAddress);
				return resendRecipientInfo;
			}
			ProxyAddressCollection proxyAddressCollection = array[0] as ProxyAddressCollection;
			string text = array[1] as string;
			if (proxyAddressCollection == null || proxyAddressCollection.Count == 0 || string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.JournalingTracer.TraceError<int, string>(0L, "Error querying recipient: emailAddresses={0},legDn={1}", (proxyAddressCollection == null) ? 0 : proxyAddressCollection.Count, text);
				throw new ArgumentException();
			}
			resendRecipientInfo = new ResendRecipientInfo();
			ProxyAddress proxyAddress2 = ProxyAddress.Parse(ProxyAddressPrefix.LegacyDN.PrimaryPrefix, text);
			if (!(proxyAddress2 is InvalidProxyAddress) && !proxyAddressCollection.Contains(proxyAddress2))
			{
				resendRecipientInfo.proxyAddresses.Add(proxyAddress2);
				ExTraceGlobals.JournalingTracer.TraceDebug<ProxyAddress>(0L, "Adding proxy: {0} for recipient", proxyAddress2);
			}
			foreach (ProxyAddress proxyAddress3 in proxyAddressCollection)
			{
				if (proxyAddress3 is InvalidProxyAddress)
				{
					ExTraceGlobals.JournalingTracer.TraceDebug<ProxyAddress>(0L, "Skipping invalid proxy: {0} for recipient", proxyAddress3);
				}
				else if (resendRecipientInfo.proxyAddresses.Contains(proxyAddress3))
				{
					ExTraceGlobals.JournalingTracer.TraceDebug<ProxyAddress>(0L, "Skipping duplicate proxy: {0} for recipient", proxyAddress3);
				}
				else
				{
					ExTraceGlobals.JournalingTracer.TraceDebug<ProxyAddress>(0L, "Adding proxy: {0} for recipient", proxyAddress3);
					resendRecipientInfo.proxyAddresses.Add(proxyAddress3);
				}
			}
			return resendRecipientInfo;
		}

		internal bool BinarySearchAndRemove(List<string> recipientProxyList)
		{
			if (recipientProxyList == null || recipientProxyList.Count == 0)
			{
				return false;
			}
			foreach (ProxyAddress proxyAddress in this.proxyAddresses)
			{
				int num = recipientProxyList.BinarySearch(proxyAddress.AddressString, StringComparer.OrdinalIgnoreCase);
				if (num >= 0)
				{
					recipientProxyList.RemoveAt(num);
					return true;
				}
				num = recipientProxyList.BinarySearch(proxyAddress.ProxyAddressString, StringComparer.OrdinalIgnoreCase);
				if (num >= 0)
				{
					recipientProxyList.RemoveAt(num);
					return true;
				}
			}
			return false;
		}

		private static ADPropertyDefinition[] recipientProperties = new ADPropertyDefinition[]
		{
			ADRecipientSchema.EmailAddresses,
			ADRecipientSchema.LegacyExchangeDN
		};

		private ProxyAddressCollection proxyAddresses = new ProxyAddressCollection();
	}
}

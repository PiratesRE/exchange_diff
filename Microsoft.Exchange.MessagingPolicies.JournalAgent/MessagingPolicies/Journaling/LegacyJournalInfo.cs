using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal sealed class LegacyJournalInfo
	{
		private static List<string> FindBestAddressForRecipients(MailItem mailItem, List<ProxyAddress> proxies)
		{
			List<string> list = new List<string>(proxies.Count);
			List<ProxyAddress> list2 = new List<ProxyAddress>();
			List<int> list3 = new List<int>();
			foreach (ProxyAddress proxyAddress in proxies)
			{
				if (ProxyAddressPrefix.Smtp != proxyAddress.Prefix)
				{
					list2.Add(proxyAddress);
					list3.Add(list.Count);
				}
				list.Add(proxyAddress.AddressString);
			}
			if (list2.Count > 0)
			{
				List<object[]> list4 = Utils.ADLookupUsers(mailItem, list2, new PropertyDefinition[]
				{
					ADRecipientSchema.EmailAddresses
				});
				for (int i = 0; i < list4.Count; i++)
				{
					object[] array = list4[i];
					int index = list3[i];
					if (array == null)
					{
						ExTraceGlobals.JournalingTracer.TraceDebug<string>(0L, "Recipient {0} is not found in AD", list[index]);
					}
					else
					{
						ProxyAddressCollection proxyAddressCollection = array[0] as ProxyAddressCollection;
						if (proxyAddressCollection != null)
						{
							ProxyAddress proxyAddress2 = proxyAddressCollection.FindPrimary(ProxyAddressPrefix.Smtp);
							if (proxyAddress2 != null)
							{
								list[index] = proxyAddress2.AddressString;
							}
						}
					}
				}
			}
			return list;
		}

		private static RecipientP2Type RecipientP2TypeParse(string stringType)
		{
			RecipientP2Type result = RecipientP2Type.Unknown;
			if (string.Compare(stringType, "to", StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = RecipientP2Type.To;
			}
			else if (string.Compare(stringType, "cc", StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = RecipientP2Type.Cc;
			}
			else if (string.Compare(stringType, "bcc", StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = RecipientP2Type.Bcc;
			}
			return result;
		}

		private static HistoryRecord HistoryRecordParse(string rawString)
		{
			HistoryRecord result = null;
			if (rawString != null && rawString.Length > 1)
			{
				int num = rawString.IndexOf('<');
				int num2 = rawString.IndexOf(':', num);
				if (num == -1 || num2 == -1)
				{
					throw new TransportPropertyException("Invalid legacy journal history record");
				}
				string text = rawString.Remove(num, num2 - num + 1);
				text = text.Trim(new char[]
				{
					'>'
				});
				result = HistoryRecord.Parse(text);
			}
			return result;
		}

		public static List<LegacyRecipientRecord> ParseOriginalRecipientsFromExch50(MailItem mailItem)
		{
			List<LegacyRecipientRecord> list = new List<LegacyRecipientRecord>();
			object obj = null;
			if (!mailItem.Properties.TryGetValue("Microsoft.Exchange.LegacyJournalReport", out obj))
			{
				ExTraceGlobals.JournalingTracer.TraceDebug(0L, "Cannot get LegacyJournalReportType from mailItem");
				return null;
			}
			LegacyJournalReportType legacyJournalReportType = (LegacyJournalReportType)obj;
			obj = null;
			switch (legacyJournalReportType)
			{
			case LegacyJournalReportType.Bcc:
				mailItem.Properties.TryGetValue("Microsoft.Exchange.OriginalP1RecipientList", out obj);
				break;
			case LegacyJournalReportType.Envelope:
			case LegacyJournalReportType.EnvelopeV2:
				mailItem.Properties.TryGetValue("Microsoft.Exchange.EnvelopeJournalRecipientList", out obj);
				break;
			}
			byte[] array = obj as byte[];
			if (array == null)
			{
				throw new TransportPropertyException("Cannot get legacy journal data from mailItem property");
			}
			LegacyJournalInfoReader legacyJournalInfoReader = new LegacyJournalInfoReader(array, 0, array.Length);
			List<ProxyAddress> proxyAddressProperties = legacyJournalInfoReader.GetProxyAddressProperties();
			List<string> list2 = LegacyJournalInfo.FindBestAddressForRecipients(mailItem, proxyAddressProperties);
			List<string> list3 = null;
			List<string> list4 = null;
			if (legacyJournalReportType == LegacyJournalReportType.EnvelopeV2)
			{
				if (mailItem.Properties.TryGetValue("Microsoft.Exchange.EnvelopeJournalRecipientType", out obj))
				{
					array = (obj as byte[]);
					if (array != null)
					{
						LegacyJournalInfoReader legacyJournalInfoReader2 = new LegacyJournalInfoReader(array, 0, array.Length);
						list3 = legacyJournalInfoReader2.GetStringProperties();
					}
				}
				if (mailItem.Properties.TryGetValue("Microsoft.Exchange.EnvelopeJournalExpansionHistory", out obj))
				{
					array = (obj as byte[]);
					if (array != null)
					{
						LegacyJournalInfoReader legacyJournalInfoReader3 = new LegacyJournalInfoReader(array, 0, array.Length);
						list4 = legacyJournalInfoReader3.GetStringProperties();
					}
				}
			}
			if (list3 != null && list4 != null && (list2.Count != list3.Count || list2.Count != list4.Count))
			{
				ExTraceGlobals.JournalingTracer.TraceDebug(0L, "Recipients, RecipientType or RecipientHistory has invalid data");
				throw new TransportPropertyException("Recipients, RecipientType or RecipientHistory has invalid data");
			}
			for (int i = 0; i < list2.Count; i++)
			{
				LegacyRecipientRecord legacyRecipientRecord = new LegacyRecipientRecord();
				legacyRecipientRecord.Address = list2[i];
				if (list3 != null)
				{
					legacyRecipientRecord.P2Type = LegacyJournalInfo.RecipientP2TypeParse(list3[i]);
				}
				if (list4 != null)
				{
					legacyRecipientRecord.History = LegacyJournalInfo.HistoryRecordParse(list4[i]);
				}
				list.Add(legacyRecipientRecord);
			}
			return list;
		}
	}
}

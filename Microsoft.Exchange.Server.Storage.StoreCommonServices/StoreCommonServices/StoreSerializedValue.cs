using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal class StoreSerializedValue
	{
		internal static StorePropName ParseStorePropName(byte[] buffer, ref int offset)
		{
			Guid guid = SerializedValue.ParseGuid(buffer, ref offset);
			string text = SerializedValue.ParseString(buffer, ref offset);
			uint num = (uint)SerializedValue.ParseInt32(buffer, ref offset);
			StorePropName result;
			if (text != null)
			{
				if (num != 4294967295U)
				{
					throw new CorruptDataException((LID)52408U, "Invalid value for StorePropName with name.");
				}
				result = new StorePropName(guid, text);
			}
			else
			{
				if (num == 4294967295U)
				{
					throw new CorruptDataException((LID)46264U, "Invalid value for StorePropName with display ID.");
				}
				result = new StorePropName(guid, num);
			}
			return result;
		}

		internal static void ParseNamedPropertyMap(byte[] buffer, ref int offset, int elementsNumber, Guid databaseGuid, Guid mailboxGuid, out Dictionary<ushort, StoreNamedPropInfo> numberToNameMap)
		{
			numberToNameMap = null;
			Dictionary<ushort, StoreNamedPropInfo> dictionary = new Dictionary<ushort, StoreNamedPropInfo>(elementsNumber);
			uint num = 0U;
			int num2 = elementsNumber;
			while (num2-- > 0)
			{
				ushort num3 = (ushort)SerializedValue.ParseInt16(buffer, ref offset);
				StorePropName storePropName = StoreSerializedValue.ParseStorePropName(buffer, ref offset);
				if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.MailboxSignatureTracer.TraceDebug(54856L, "Database {0} : Mailbox {1} : Parse named property: {2} : {3}", new object[]
					{
						databaseGuid,
						mailboxGuid,
						num3,
						storePropName.ToString()
					});
				}
				StoreNamedPropInfo namedPropInfo = WellKnownProperties.GetNamedPropInfo(storePropName);
				if (num3 <= 32768 || num3 > 65535)
				{
					throw new CorruptDataException((LID)41532U, "Invalid named property Id value.");
				}
				if (dictionary.ContainsKey(num3))
				{
					throw new CorruptDataException((LID)51024U, "Duplicate property ID.");
				}
				dictionary.Add(num3, namedPropInfo);
				num += (uint)(num3 - 32768);
			}
			if ((ulong)num != (ulong)((long)(elementsNumber * (elementsNumber + 1) / 2)))
			{
				throw new CorruptDataException((LID)64336U, "One of more named property IDs are invalid.");
			}
			numberToNameMap = dictionary;
		}

		internal static void ParseReplidGuidMap(byte[] buffer, ref int offset, int elementsNumber, Guid databaseGuid, Guid mailboxGuid, out Dictionary<ushort, Guid> replidGuidMap)
		{
			replidGuidMap = null;
			Dictionary<ushort, Guid> dictionary = new Dictionary<ushort, Guid>(elementsNumber);
			uint num = 0U;
			int num2 = elementsNumber;
			while (num2-- > 0)
			{
				ushort num3 = (ushort)SerializedValue.ParseInt16(buffer, ref offset);
				Guid guid = SerializedValue.ParseGuid(buffer, ref offset);
				if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.MailboxSignatureTracer.TraceDebug(57032L, "Database {0} : Mailbox {1} : ReplId/GUID: {2} : {3}", new object[]
					{
						databaseGuid,
						mailboxGuid,
						num3,
						guid
					});
				}
				if (num3 > ReplidGuidMap.MaxReplidGuidNumber)
				{
					throw new CorruptDataException((LID)47952U, "Invalid replid value.");
				}
				if (dictionary.ContainsKey(num3))
				{
					throw new CorruptDataException((LID)39760U, "Duplicate replid.");
				}
				dictionary.Add(num3, guid);
				num += (uint)num3;
			}
			if ((ulong)num != (ulong)((long)(elementsNumber * (elementsNumber + 1) / 2)))
			{
				throw new CorruptDataException((LID)56144U, "One of more replid values are invalid.");
			}
			replidGuidMap = dictionary;
		}

		internal static int SerializeStorePropName(StorePropName propName, byte[] buffer, int offset)
		{
			int num = offset;
			offset += SerializedValue.SerializeGuid(propName.Guid, buffer, offset);
			offset += SerializedValue.SerializeString(propName.Name, buffer, offset);
			offset += SerializedValue.SerializeInt32((int)propName.DispId, buffer, offset);
			return offset - num;
		}

		internal static int SerializeNamedPropertyMap(Context context, Mailbox mailbox, byte[] buffer, int offset)
		{
			int offset2 = offset;
			mailbox.NamedPropertyMap.ForEachElement(context, delegate(KeyValuePair<ushort, StoreNamedPropInfo> propidNamePair)
			{
				if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace) && buffer != null)
				{
					ExTraceGlobals.MailboxSignatureTracer.TraceDebug(44744L, "Database {0} : Mailbox {1} : serialize named property: {2} : {3}", new object[]
					{
						mailbox.Database.MdbName,
						mailbox.MailboxNumber,
						propidNamePair.Key,
						propidNamePair.Value.PropName.ToString()
					});
				}
				offset += SerializedValue.SerializeInt16((short)propidNamePair.Key, buffer, offset);
				offset += StoreSerializedValue.SerializeStorePropName(propidNamePair.Value.PropName, buffer, offset);
			});
			int num = offset - offset2;
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace) && buffer == null)
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<string, int, int>(38600L, "Database {0} : Mailbox {1} : Named property serialized length : {2}", mailbox.Database.MdbName, mailbox.MailboxNumber, num);
			}
			return num;
		}

		internal static int SerializeReplidGuidMap(Mailbox mailbox, byte[] buffer, int offset)
		{
			int offset2 = offset;
			mailbox.ReplidGuidMap.ForEachElement(delegate(KeyValuePair<ushort, Guid> replidGuidpair)
			{
				if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace) && buffer != null)
				{
					ExTraceGlobals.MailboxSignatureTracer.TraceDebug(61128L, "Database {0} : Mailbox {1} : serialize ReplId/GUID: {2} : {3}", new object[]
					{
						mailbox.Database.MdbName,
						mailbox.MailboxNumber,
						replidGuidpair.Key,
						replidGuidpair.Value
					});
				}
				offset += SerializedValue.SerializeInt16((short)replidGuidpair.Key, buffer, offset);
				offset += SerializedValue.SerializeGuid(replidGuidpair.Value, buffer, offset);
			});
			int num = offset - offset2;
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace) && buffer == null)
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<string, int, int>(54984L, "Database {0} : Mailbox {1} : ReplId/GUID serialized length : {2}", mailbox.Database.MdbName, mailbox.MailboxNumber, num);
			}
			return num;
		}
	}
}

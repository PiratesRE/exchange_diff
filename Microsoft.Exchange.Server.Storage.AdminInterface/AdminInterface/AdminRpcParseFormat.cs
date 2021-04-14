using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.Rpc.AdminRpc;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.StoreIntegrityCheck;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	internal static class AdminRpcParseFormat
	{
		public static ErrorCode ParseReadEventsRequest(Context context, byte[] request, out EventHistory.ReadEventsFlags flags, out long startCounter, out uint eventsWant, out uint eventsToCheck, out Restriction restriction)
		{
			uint num = (uint)request.Length;
			uint num2 = 0U;
			restriction = null;
			ErrorCode first = AdminRpcParseFormat.ParseReadEventsRequestHeaderBlock(request, ref num2, ref num, out flags, out startCounter, out eventsWant, out eventsToCheck);
			if (first != ErrorCode.NoError)
			{
				return first.Propagate((LID)45909U);
			}
			if (num != 0U)
			{
				first = AdminRpcParseFormat.ParseReadEventsRestrictionBlock(context, request, ref num2, ref num, out restriction);
				if (first != ErrorCode.NoError)
				{
					return first.Propagate((LID)62293U);
				}
			}
			if (num != 0U)
			{
				return ErrorCode.CreateRpcFormat((LID)37717U);
			}
			return ErrorCode.NoError;
		}

		public static ErrorCode FormatReadEventsResponse(int flags, long endCounter, List<EventEntry> events, out byte[] response)
		{
			uint num = (uint)((events == null) ? 0 : events.Count);
			uint num2 = AdminRpcParseFormat.PaddedBlockLength(AdminRpcParseFormat.ReadEventsResponseHeaderLengthV2) + num * AdminRpcParseFormat.PaddedBlockLength(AdminRpcParseFormat.EventEntryLengthV7);
			response = new byte[num2];
			ParseSerialize.SerializeInt32((int)AdminRpcParseFormat.ReadEventsResponseHeaderLengthV2, response, (int)AdminRpcParseFormat.DataBlockLengthOffset);
			ParseSerialize.SerializeInt32(flags, response, (int)AdminRpcParseFormat.ReadEventsResponseHeaderFlagsOffset);
			ParseSerialize.SerializeInt32((int)num, response, (int)AdminRpcParseFormat.ReadEventsResponseHeaderEventsCountOffset);
			ParseSerialize.SerializeInt32(0, response, (int)AdminRpcParseFormat.ReadEventsResponseHeaderPaddingOffset);
			ParseSerialize.SerializeInt64(endCounter, response, (int)AdminRpcParseFormat.ReadEventsResponseHeaderEndCounterOffset);
			if (events != null)
			{
				AdminRpcParseFormat.FormatEventList(events, response, AdminRpcParseFormat.PaddedBlockLength(AdminRpcParseFormat.ReadEventsResponseHeaderLengthV2));
			}
			return ErrorCode.NoError;
		}

		public static ErrorCode ParseWriteEventsRequest(byte[] request, out int flags, out List<EventEntry> events)
		{
			uint num = (uint)request.Length;
			uint num2 = 0U;
			events = null;
			uint eventsCount;
			ErrorCode first = AdminRpcParseFormat.ParseWriteEventsRequestHeaderBlock(request, ref num2, ref num, out flags, out eventsCount);
			if (first != ErrorCode.NoError)
			{
				return first.Propagate((LID)54101U);
			}
			first = AdminRpcParseFormat.ParseEventList(request, false, ref num2, ref num, eventsCount, out events);
			if (first != ErrorCode.NoError)
			{
				return first.Propagate((LID)41813U);
			}
			if (num != 0U)
			{
				return ErrorCode.CreateRpcFormat((LID)58197U);
			}
			return ErrorCode.NoError;
		}

		public static ErrorCode FormatWriteEventsResponse(List<long> eventCounters, out byte[] response)
		{
			uint num = AdminRpcParseFormat.WriteEventsResponseHeaderLength + (uint)(eventCounters.Count * (int)AdminRpcParseFormat.WriteEventsResponseSingleAdjustedEventCounterLength);
			response = new byte[num];
			ParseSerialize.SerializeInt32((int)num, response, (int)AdminRpcParseFormat.DataBlockLengthOffset);
			ParseSerialize.SerializeInt32(eventCounters.Count, response, (int)AdminRpcParseFormat.WriteEventsResponseEventsCountOffset);
			uint num2 = AdminRpcParseFormat.WriteEventsResponseHeaderLength;
			for (int i = 0; i < eventCounters.Count; i++)
			{
				ParseSerialize.SerializeInt64(eventCounters[i], response, (int)num2);
				num2 += AdminRpcParseFormat.WriteEventsResponseSingleAdjustedEventCounterLength;
			}
			return ErrorCode.NoError;
		}

		public static ErrorCode ParseMDBEVENTWMs(MDBEVENTWM[] wms, out List<EventWatermark> watermarks)
		{
			watermarks = new List<EventWatermark>(wms.Length);
			for (int i = 0; i < wms.Length; i++)
			{
				watermarks.Add(new EventWatermark(wms[i].MailboxGuid, wms[i].ConsumerGuid, wms[i].EventCounter));
			}
			return ErrorCode.NoError;
		}

		public static ErrorCode FormatMDBEVENTWMs(List<EventWatermark> watermarks, out MDBEVENTWM[] wms)
		{
			wms = new MDBEVENTWM[watermarks.Count];
			for (int i = 0; i < watermarks.Count; i++)
			{
				wms[i].MailboxGuid = watermarks[i].MailboxGuid;
				wms[i].ConsumerGuid = watermarks[i].ConsumerGuid;
				wms[i].EventCounter = watermarks[i].EventCounter;
			}
			return ErrorCode.NoError;
		}

		private static ErrorCode CheckDataBlock(byte[] request, uint ib, uint cb, out uint blockDataLength)
		{
			if (cb < 4U)
			{
				blockDataLength = 0U;
				return ErrorCode.CreateRpcFormat((LID)33621U);
			}
			blockDataLength = (uint)ParseSerialize.ParseInt32(request, (int)(ib + AdminRpcParseFormat.DataBlockLengthOffset));
			if (blockDataLength < AdminRpcParseFormat.DataBlockLengthLength || blockDataLength > cb || (blockDataLength < cb && AdminRpcParseFormat.PaddedBlockLength(blockDataLength) > cb))
			{
				return ErrorCode.CreateRpcFormat((LID)50005U);
			}
			return ErrorCode.NoError;
		}

		public static uint ActualBlockLength(uint blockDataLength, uint blockMaxLength)
		{
			if (blockDataLength > blockMaxLength)
			{
				throw new InvalidSerializedFormatException("at this point, we should have checked blockDataLength");
			}
			if (blockDataLength >= blockMaxLength)
			{
				return blockMaxLength;
			}
			return AdminRpcParseFormat.PaddedBlockLength(blockDataLength);
		}

		public static uint PaddedBlockLength(uint dataLength)
		{
			return dataLength + (AdminRpcParseFormat.DataBlockLengthPaddingMultiple - 1U) & ~(AdminRpcParseFormat.DataBlockLengthPaddingMultiple - 1U);
		}

		private static ErrorCode ParseReadEventsRequestHeaderBlock(byte[] request, ref uint ib, ref uint cb, out EventHistory.ReadEventsFlags flags, out long startCounter, out uint eventsWant, out uint eventsToCheck)
		{
			flags = EventHistory.ReadEventsFlags.None;
			startCounter = 0L;
			eventsWant = 0U;
			eventsToCheck = 0U;
			uint num;
			ErrorCode first = AdminRpcParseFormat.CheckDataBlock(request, ib, cb, out num);
			if (first != ErrorCode.NoError)
			{
				return first.Propagate((LID)48213U);
			}
			if (num != AdminRpcParseFormat.ReadEventsRequestHeaderLengthV1 && num != AdminRpcParseFormat.ReadEventsRequestHeaderLengthV2)
			{
				return ErrorCode.CreateRpcFormat((LID)64597U);
			}
			flags = (EventHistory.ReadEventsFlags)ParseSerialize.ParseInt32(request, (int)(ib + AdminRpcParseFormat.ReadEventsRequestHeaderFlagsOffset));
			startCounter = ParseSerialize.ParseInt64(request, (int)(ib + AdminRpcParseFormat.ReadEventsRequestHeaderStartCounterOffset));
			eventsWant = (uint)ParseSerialize.ParseInt32(request, (int)(ib + AdminRpcParseFormat.ReadEventsRequestHeaderEventsWantOffset));
			if (num == AdminRpcParseFormat.ReadEventsRequestHeaderLengthV2)
			{
				eventsToCheck = (uint)ParseSerialize.ParseInt32(request, (int)(ib + AdminRpcParseFormat.ReadEventsRequestHeaderEventsToCheckOffset));
			}
			uint num2 = AdminRpcParseFormat.ActualBlockLength(num, cb);
			ib += num2;
			cb -= num2;
			return ErrorCode.NoError;
		}

		private static ErrorCode ParseReadEventsRestrictionBlock(Context context, byte[] request, ref uint ib, ref uint cb, out Restriction restriction)
		{
			restriction = null;
			uint num;
			ErrorCode first = AdminRpcParseFormat.CheckDataBlock(request, ib, cb, out num);
			if (first != ErrorCode.NoError)
			{
				return first.Propagate((LID)40021U);
			}
			if (num > AdminRpcParseFormat.DataBlockLengthLength)
			{
				int num2 = (int)(ib + AdminRpcParseFormat.DataBlockLengthLength);
				restriction = Restriction.Deserialize(context, request, ref num2, (int)(ib + num), null, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Event);
			}
			uint num3 = AdminRpcParseFormat.ActualBlockLength(num, cb);
			ib += num3;
			cb -= num3;
			return ErrorCode.NoError;
		}

		private static ErrorCode ParseWriteEventsRequestHeaderBlock(byte[] request, ref uint ib, ref uint cb, out int flags, out uint eventsCount)
		{
			flags = 0;
			eventsCount = 0U;
			uint num;
			ErrorCode first = AdminRpcParseFormat.CheckDataBlock(request, ib, cb, out num);
			if (first != ErrorCode.NoError)
			{
				return first.Propagate((LID)56405U);
			}
			if (num != AdminRpcParseFormat.WriteEventsRequestHeaderLength)
			{
				return ErrorCode.CreateRpcFormat((LID)44117U);
			}
			flags = ParseSerialize.ParseInt32(request, (int)(ib + AdminRpcParseFormat.WriteEventsRequestHeaderFlagsOffset));
			eventsCount = (uint)ParseSerialize.ParseInt32(request, (int)(ib + AdminRpcParseFormat.WriteEventsRequestHeaderEventsCountOffset));
			uint num2 = AdminRpcParseFormat.ActualBlockLength(num, cb);
			ib += num2;
			cb -= num2;
			return ErrorCode.NoError;
		}

		public static ErrorCode ParseEventList(byte[] request, bool preserveUntrustworthyData, ref uint ib, ref uint cb, uint eventsCount, out List<EventEntry> events)
		{
			events = null;
			if (cb / AdminRpcParseFormat.PaddedBlockLength(AdminRpcParseFormat.EventEntryLengthV1) < eventsCount)
			{
				return ErrorCode.CreateRpcFormat((LID)60501U);
			}
			events = new List<EventEntry>((int)eventsCount);
			for (uint num = 0U; num < eventsCount; num += 1U)
			{
				uint num2;
				ErrorCode first = AdminRpcParseFormat.CheckDataBlock(request, ib, cb, out num2);
				if (first != ErrorCode.NoError)
				{
					return first.Propagate((LID)35925U);
				}
				if (num2 <= AdminRpcParseFormat.EventEntryLengthV7 && num2 != AdminRpcParseFormat.EventEntryLengthV1 && num2 != AdminRpcParseFormat.EventEntryLengthV2 && num2 != AdminRpcParseFormat.EventEntryLengthV3 && num2 != AdminRpcParseFormat.EventEntryLengthV4 && num2 != AdminRpcParseFormat.EventEntryLengthV5 && num2 != AdminRpcParseFormat.EventEntryLengthV6 && num2 != AdminRpcParseFormat.EventEntryLengthV7)
				{
					DiagnosticContext.TraceDword((LID)63824U, num2);
					return ErrorCode.CreateRpcFormat((LID)52309U);
				}
				EventType eventType = (EventType)ParseSerialize.ParseInt32(request, (int)(ib + AdminRpcParseFormat.EventEntryMaskOffset));
				long eventCounter = ParseSerialize.ParseInt64(request, (int)(ib + AdminRpcParseFormat.EventEntryEventCounterOffset));
				DateTime createTime = ParseSerialize.ParseFileTime(request, (int)(ib + AdminRpcParseFormat.EventEntryCreateTimeOffset));
				int transactionId = ParseSerialize.ParseInt32(request, (int)(ib + AdminRpcParseFormat.EventEntryTransacIdOffset));
				int? itemCount = AdminRpcParseFormat.ReadInt32OrNull(request, ib + AdminRpcParseFormat.EventEntryItemCountOffset, -1);
				int? unreadCount = AdminRpcParseFormat.ReadInt32OrNull(request, ib + AdminRpcParseFormat.EventEntryUnreadCountOffset, -1);
				EventFlags flags = (EventFlags)ParseSerialize.ParseInt32(request, (int)(ib + AdminRpcParseFormat.EventEntryFlagsOffset));
				Guid value = ParseSerialize.ParseGuid(request, (int)(ib + AdminRpcParseFormat.EventEntryMailboxGuidOffset));
				Guid value2 = ParseSerialize.ParseGuid(request, (int)(ib + AdminRpcParseFormat.EventEntryMapiEntryIdGuidOffset));
				byte[] fid = AdminRpcParseFormat.ReadByteArrayOrNull(request, ib + AdminRpcParseFormat.EventEntryFidOffset, AdminRpcParseFormat.EventEntryFidLength);
				byte[] mid = AdminRpcParseFormat.ReadByteArrayOrNull(request, ib + AdminRpcParseFormat.EventEntryMidOffset, AdminRpcParseFormat.EventEntryMidLength);
				byte[] parentFid = AdminRpcParseFormat.ReadByteArrayOrNull(request, ib + AdminRpcParseFormat.EventEntryParentFidOffset, AdminRpcParseFormat.EventEntryParentFidLength);
				byte[] oldFid = AdminRpcParseFormat.ReadByteArrayOrNull(request, ib + AdminRpcParseFormat.EventEntryOldFidOffset, AdminRpcParseFormat.EventEntryOldFidLength);
				byte[] oldMid = AdminRpcParseFormat.ReadByteArrayOrNull(request, ib + AdminRpcParseFormat.EventEntryOldMidOffset, AdminRpcParseFormat.EventEntryOldMidLength);
				byte[] oldParentFid = AdminRpcParseFormat.ReadByteArrayOrNull(request, ib + AdminRpcParseFormat.EventEntryOldParentFidOffset, AdminRpcParseFormat.EventEntryOldParentFidLength);
				string objectClass = AdminRpcParseFormat.ReadAsciiStringOrNull(request, ib + AdminRpcParseFormat.EventEntryObjectClassOffset, AdminRpcParseFormat.EventEntryObjectClassLength);
				ExtendedEventFlags? extendedFlags = null;
				ClientType clientType = ClientType.User;
				byte[] sid = null;
				int? documentId = null;
				int? mailboxNumber = null;
				TenantHint empty = TenantHint.Empty;
				Guid? unifiedMailboxGuid = null;
				if (num2 > AdminRpcParseFormat.EventEntryLengthV1)
				{
					long? num3 = AdminRpcParseFormat.ReadInt64OrNull(request, ib + AdminRpcParseFormat.EventEntryExtendedFlagsOffset, 0L);
					extendedFlags = ((num3 != null) ? new ExtendedEventFlags?((ExtendedEventFlags)num3.GetValueOrDefault()) : null);
				}
				if (num2 > AdminRpcParseFormat.EventEntryLengthV2)
				{
					int num4 = ParseSerialize.ParseInt32(request, (int)(ib + AdminRpcParseFormat.EventEntryClientTypeOffset));
					if (preserveUntrustworthyData)
					{
						clientType = (ClientType)num4;
					}
					else
					{
						clientType = ((num4 == 0) ? ClientType.User : ((ClientType)num4));
					}
					sid = AdminRpcParseFormat.ReadSidOrNull(request, ib + AdminRpcParseFormat.EventEntrySidOffset, AdminRpcParseFormat.EventEntrySidLength);
				}
				if (num2 > AdminRpcParseFormat.EventEntryLengthV3)
				{
					documentId = AdminRpcParseFormat.ReadInt32OrNull(request, ib + AdminRpcParseFormat.EventEntryDocIdOffset, 0);
				}
				if (num2 > AdminRpcParseFormat.EventEntryLengthV4 && preserveUntrustworthyData)
				{
					mailboxNumber = AdminRpcParseFormat.ReadInt32OrNull(request, ib + AdminRpcParseFormat.EventEntryMailboxNumberOffset, 0);
				}
				if (num2 > AdminRpcParseFormat.EventEntryLengthV5)
				{
					int? num5 = AdminRpcParseFormat.ReadInt32OrNull(request, ib + AdminRpcParseFormat.EventEntryTenantHintBlobSizeOffset, -1);
					uint? num6 = (num5 != null) ? new uint?((uint)num5.GetValueOrDefault()) : null;
					if (num6 == null || num6.Value > AdminRpcParseFormat.EventEntryTenantHintBlobLength)
					{
						return ErrorCode.CreateCorruptData((LID)49232U);
					}
					if (preserveUntrustworthyData)
					{
						byte[] tenantHintBlob = AdminRpcParseFormat.ReadByteArrayOrNull(false, request, ib + AdminRpcParseFormat.EventEntryTenantHintBlobOffset, num6.Value);
						empty = new TenantHint(tenantHintBlob);
					}
				}
				if (num2 > AdminRpcParseFormat.EventEntryLengthV6 && preserveUntrustworthyData)
				{
					unifiedMailboxGuid = new Guid?(ParseSerialize.ParseGuid(request, (int)(ib + AdminRpcParseFormat.EventEntryUnifiedMailboxGuidOffset)));
				}
				EventEntry item = new EventEntry(eventCounter, createTime, transactionId, eventType, mailboxNumber, new Guid?(value), new Guid?(value2), objectClass, fid, mid, parentFid, oldFid, oldMid, oldParentFid, itemCount, unreadCount, flags, extendedFlags, clientType, sid, documentId, empty, unifiedMailboxGuid);
				events.Add(item);
				uint num7 = AdminRpcParseFormat.ActualBlockLength(num2, cb);
				ib += num7;
				cb -= num7;
			}
			return ErrorCode.NoError;
		}

		public static int SerializeIntegrityCheckRequest(byte[] buffer, ref int pos, int posMax, Guid requestGuid, uint flags, TaskId[] taskIds, StorePropTag[] propTags)
		{
			int num = 0;
			num += 4;
			int num2 = pos;
			if (buffer != null)
			{
				pos += 4;
			}
			num += 4;
			if (buffer != null)
			{
				AdminRpcParseFormat.CheckBounds(pos, posMax, 4);
				ParseSerialize.SerializeInt32((int)flags, buffer, pos);
				pos += 4;
			}
			num += 16;
			if (buffer != null)
			{
				AdminRpcParseFormat.CheckBounds(pos, posMax, 16);
				ParseSerialize.SerializeGuid(requestGuid, buffer, pos);
				pos += 16;
			}
			num += 4;
			if (buffer != null)
			{
				AdminRpcParseFormat.CheckBounds(pos, posMax, 4);
				ParseSerialize.SerializeInt32((taskIds == null) ? 0 : taskIds.Length, buffer, pos);
				pos += 4;
			}
			if (taskIds != null)
			{
				num += 4 * taskIds.Length;
				foreach (TaskId value in taskIds)
				{
					if (buffer != null)
					{
						AdminRpcParseFormat.CheckBounds(pos, posMax, 4);
						ParseSerialize.SerializeInt32((int)value, buffer, pos);
						pos += 4;
					}
				}
			}
			num += 4;
			if (buffer != null)
			{
				AdminRpcParseFormat.CheckBounds(pos, posMax, 4);
				ParseSerialize.SerializeInt32((propTags == null) ? 0 : propTags.Length, buffer, pos);
				pos += 4;
			}
			if (propTags != null)
			{
				num += 4 * propTags.Length;
				foreach (StorePropTag storePropTag in propTags)
				{
					if (buffer != null)
					{
						AdminRpcParseFormat.CheckBounds(pos, posMax, 4);
						ParseSerialize.SerializeInt32((int)storePropTag.PropTag, buffer, pos);
						pos += 4;
					}
				}
			}
			if (buffer != null)
			{
				AdminRpcParseFormat.CheckBounds(num2, posMax, 4);
				ParseSerialize.SerializeInt32(num, buffer, num2);
			}
			return num;
		}

		public static ErrorCode ParseIntegrityCheckRequest(byte[] request, out int flags, out Guid requestGuid, out TaskId[] taskIds, out StorePropTag[] propTags)
		{
			flags = 0;
			requestGuid = Guid.Empty;
			taskIds = null;
			propTags = null;
			uint num = 0U;
			uint cb = (uint)((request == null) ? 0 : request.Length);
			uint num2;
			ErrorCode errorCode = AdminRpcParseFormat.CheckDataBlock(request, num, cb, out num2);
			if (errorCode != ErrorCode.NoError)
			{
				return errorCode.Propagate((LID)33440U);
			}
			int num3 = 32;
			if (num2 != (uint)request.Length || num2 < (uint)num3)
			{
				return ErrorCode.CreateRpcFormat((LID)49824U);
			}
			num += 4U;
			flags = ParseSerialize.ParseInt32(request, (int)num);
			num += 4U;
			requestGuid = ParseSerialize.ParseGuid(request, (int)num);
			num += 16U;
			int num4 = ParseSerialize.ParseInt32(request, (int)num);
			num += 4U;
			taskIds = new TaskId[num4];
			for (int i = 0; i < num4; i++)
			{
				if (num + 4U > num2)
				{
					return ErrorCode.CreateRpcFormat((LID)54620U);
				}
				uint num5 = (uint)ParseSerialize.ParseInt32(request, (int)num);
				taskIds[i] = (TaskId)num5;
				num += 4U;
			}
			int num6 = ParseSerialize.ParseInt32(request, (int)num);
			propTags = new StorePropTag[num6];
			num += 4U;
			for (int j = 0; j < num6; j++)
			{
				if (num + 4U > num2)
				{
					return ErrorCode.CreateRpcFormat((LID)42332U);
				}
				uint legacyPropTag = (uint)ParseSerialize.ParseInt32(request, (int)num);
				propTags[j] = LegacyHelper.ConvertFromLegacyPropTag(legacyPropTag, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.IsIntegJob, null, true);
				num += 4U;
			}
			return errorCode;
		}

		public static int SerializeIntegrityCheckResponse(byte[] buffer, ref int pos, int posMax, IEnumerable<Properties> rows)
		{
			int num = 0;
			int num2 = 0;
			num += 4;
			int num3 = pos;
			pos += 4;
			num += 4;
			int num4 = pos;
			pos += 4;
			if (rows != null)
			{
				foreach (Properties properties in rows)
				{
					num2++;
					num += AdminRpcParseFormat.SetValues(buffer, ref pos, posMax, properties);
				}
			}
			if (buffer != null)
			{
				AdminRpcParseFormat.CheckBounds(num3, posMax, 4);
				ParseSerialize.SerializeInt32(num, buffer, num3);
				AdminRpcParseFormat.CheckBounds(num4, posMax, 4);
				ParseSerialize.SerializeInt32(num2, buffer, num4);
			}
			return num;
		}

		public static void FormatEventList(List<EventEntry> events, byte[] buffer, uint ib)
		{
			for (int i = 0; i < events.Count; i++)
			{
				EventEntry eventEntry = events[i];
				ParseSerialize.SerializeInt32((int)AdminRpcParseFormat.EventEntryLengthV7, buffer, (int)(ib + AdminRpcParseFormat.DataBlockLengthOffset));
				ParseSerialize.SerializeInt32((int)eventEntry.EventType, buffer, (int)(ib + AdminRpcParseFormat.EventEntryMaskOffset));
				ParseSerialize.SerializeInt64(eventEntry.EventCounter, buffer, (int)(ib + AdminRpcParseFormat.EventEntryEventCounterOffset));
				ParseSerialize.SerializeFileTime(eventEntry.CreateTime, buffer, (int)(ib + AdminRpcParseFormat.EventEntryCreateTimeOffset));
				ParseSerialize.SerializeInt32(eventEntry.TransactionId, buffer, (int)(ib + AdminRpcParseFormat.EventEntryTransacIdOffset));
				ParseSerialize.SerializeInt32((eventEntry.ItemCount == null) ? -1 : eventEntry.ItemCount.Value, buffer, (int)(ib + AdminRpcParseFormat.EventEntryItemCountOffset));
				ParseSerialize.SerializeInt32((eventEntry.UnreadCount == null) ? -1 : eventEntry.UnreadCount.Value, buffer, (int)(ib + AdminRpcParseFormat.EventEntryUnreadCountOffset));
				ParseSerialize.SerializeInt32((int)eventEntry.Flags, buffer, (int)(ib + AdminRpcParseFormat.EventEntryFlagsOffset));
				ParseSerialize.SerializeGuid((eventEntry.MailboxGuid == null) ? Guid.Empty : eventEntry.MailboxGuid.Value, buffer, (int)(ib + AdminRpcParseFormat.EventEntryMailboxGuidOffset));
				ParseSerialize.SerializeGuid((eventEntry.MapiEntryIdGuid == null) ? Guid.Empty : eventEntry.MapiEntryIdGuid.Value, buffer, (int)(ib + AdminRpcParseFormat.EventEntryMapiEntryIdGuidOffset));
				AdminRpcParseFormat.WriteBytesIfNotNull(eventEntry.Fid24, buffer, ib + AdminRpcParseFormat.EventEntryFidOffset, AdminRpcParseFormat.EventEntryFidLength);
				AdminRpcParseFormat.WriteBytesIfNotNull(eventEntry.Mid24, buffer, ib + AdminRpcParseFormat.EventEntryMidOffset, AdminRpcParseFormat.EventEntryMidLength);
				AdminRpcParseFormat.WriteBytesIfNotNull(eventEntry.ParentFid24, buffer, ib + AdminRpcParseFormat.EventEntryParentFidOffset, AdminRpcParseFormat.EventEntryParentFidLength);
				AdminRpcParseFormat.WriteBytesIfNotNull(eventEntry.OldFid24, buffer, ib + AdminRpcParseFormat.EventEntryOldFidOffset, AdminRpcParseFormat.EventEntryOldFidLength);
				AdminRpcParseFormat.WriteBytesIfNotNull(eventEntry.OldMid24, buffer, ib + AdminRpcParseFormat.EventEntryOldMidOffset, AdminRpcParseFormat.EventEntryOldMidLength);
				AdminRpcParseFormat.WriteBytesIfNotNull(eventEntry.OldParentFid24, buffer, ib + AdminRpcParseFormat.EventEntryOldParentFidOffset, AdminRpcParseFormat.EventEntryOldParentFidLength);
				AdminRpcParseFormat.WriteAsciiStringIfNotNull(eventEntry.ObjectClass, buffer, ib + AdminRpcParseFormat.EventEntryObjectClassOffset, AdminRpcParseFormat.EventEntryObjectClassLength);
				ParseSerialize.SerializeInt64((long)((eventEntry.ExtendedFlags == null) ? ExtendedEventFlags.None : eventEntry.ExtendedFlags.Value), buffer, (int)(ib + AdminRpcParseFormat.EventEntryExtendedFlagsOffset));
				ParseSerialize.SerializeInt32((int)eventEntry.ClientType, buffer, (int)(ib + AdminRpcParseFormat.EventEntryClientTypeOffset));
				AdminRpcParseFormat.WriteBytesIfNotNull(eventEntry.Sid, buffer, ib + AdminRpcParseFormat.EventEntrySidOffset, AdminRpcParseFormat.EventEntrySidLength);
				ParseSerialize.SerializeInt32((eventEntry.DocumentId == null) ? 0 : eventEntry.DocumentId.Value, buffer, (int)(ib + AdminRpcParseFormat.EventEntryDocIdOffset));
				ParseSerialize.SerializeInt32((eventEntry.MailboxNumber == null) ? 0 : eventEntry.MailboxNumber.Value, buffer, (int)(ib + AdminRpcParseFormat.EventEntryMailboxNumberOffset));
				ParseSerialize.SerializeInt32(eventEntry.TenantHint.TenantHintBlobSize, buffer, (int)(ib + AdminRpcParseFormat.EventEntryTenantHintBlobSizeOffset));
				AdminRpcParseFormat.WriteBytesIfNotNull(eventEntry.TenantHint.TenantHintBlob, buffer, ib + AdminRpcParseFormat.EventEntryTenantHintBlobOffset, AdminRpcParseFormat.EventEntryTenantHintBlobLength);
				ParseSerialize.SerializeGuid((eventEntry.UnifiedMailboxGuid == null) ? Guid.Empty : eventEntry.UnifiedMailboxGuid.Value, buffer, (int)(ib + AdminRpcParseFormat.EventEntryUnifiedMailboxGuidOffset));
				ib += AdminRpcParseFormat.PaddedBlockLength(AdminRpcParseFormat.EventEntryLengthV7);
			}
		}

		public static void SerializeMdbStatus(List<AdminRpcServer.AdminRpcListAllMdbStatus.MdbStatus> mdbStatus, bool basicInformation, out byte[] mdbStatusRaw)
		{
			uint num = (uint)((mdbStatus == null) ? 0 : mdbStatus.Count);
			int num2 = 0;
			foreach (AdminRpcServer.AdminRpcListAllMdbStatus.MdbStatus mdbStatus2 in mdbStatus)
			{
				num2 += (int)AdminRpcParseFormat.SizeofMdbStatusRaw;
				if (!basicInformation)
				{
					num2 += mdbStatus2.MdbName.Length + 1 + mdbStatus2.VServerName.Length + 1 + mdbStatus2.LegacyDN.Length + 1;
				}
			}
			if (num2 == 0)
			{
				mdbStatusRaw = null;
				return;
			}
			mdbStatusRaw = new byte[num2];
			uint num3 = 0U;
			if (!basicInformation)
			{
				num3 = num * AdminRpcParseFormat.SizeofMdbStatusRaw;
			}
			for (int i = 0; i < mdbStatus.Count; i++)
			{
				AdminRpcServer.AdminRpcListAllMdbStatus.MdbStatus mdbStatus3 = mdbStatus[i];
				uint num4 = (uint)(i * (int)AdminRpcParseFormat.SizeofMdbStatusRaw);
				ParseSerialize.SerializeGuid(mdbStatus3.GuidMdb, mdbStatusRaw, (int)num4);
				ParseSerialize.SerializeInt32((int)mdbStatus3.UlStatus, mdbStatusRaw, (int)(num4 + AdminRpcParseFormat.MdbStatusUlStatusOffset));
				if (!basicInformation)
				{
					ParseSerialize.SerializeInt32(mdbStatus3.MdbName.Length + 1, mdbStatusRaw, (int)(num4 + AdminRpcParseFormat.MdbStatusCbMdbNameOffset));
					ParseSerialize.SerializeInt32(mdbStatus3.VServerName.Length + 1, mdbStatusRaw, (int)(num4 + AdminRpcParseFormat.MdbStatusCbVServerNameOffset));
					ParseSerialize.SerializeInt32(mdbStatus3.LegacyDN.Length + 1, mdbStatusRaw, (int)(num4 + AdminRpcParseFormat.MdbStatusCbMdbLegacyDNOffset));
					ParseSerialize.SerializeInt32(0, mdbStatusRaw, (int)(num4 + AdminRpcParseFormat.MdbStatusCbStorageGroupNameOffset));
					uint num5 = num3;
					ParseSerialize.SerializeInt32((int)num5, mdbStatusRaw, (int)(num4 + AdminRpcParseFormat.MdbStatusIbMdbNameOffset));
					num5 += (uint)(mdbStatus3.MdbName.Length + 1);
					ParseSerialize.SerializeInt32((int)num5, mdbStatusRaw, (int)(num4 + AdminRpcParseFormat.MdbStatusIbVServerNameOffset));
					num5 += (uint)(mdbStatus3.VServerName.Length + 1);
					ParseSerialize.SerializeInt32((int)num5, mdbStatusRaw, (int)(num4 + AdminRpcParseFormat.MdbStatusIbMdbLegacyDNOffset));
					ParseSerialize.SerializeInt32(0, mdbStatusRaw, (int)(num4 + AdminRpcParseFormat.MdbStatusIbStorageGroupNameOffset));
					if (mdbStatus3.MdbName != Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(mdbStatus3.MdbName)))
					{
						throw new CorruptDataException((LID)55368U, "Database name string is not ASCII.");
					}
					Encoding.ASCII.GetBytes(mdbStatus3.MdbName, 0, mdbStatus3.MdbName.Length, mdbStatusRaw, (int)num3);
					num3 += (uint)(mdbStatus3.MdbName.Length + 1);
					if (mdbStatus3.VServerName != Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(mdbStatus3.VServerName)))
					{
						throw new CorruptDataException((LID)43080U, "Virtual ServerName string is not ASCII.");
					}
					Encoding.ASCII.GetBytes(mdbStatus3.VServerName, 0, mdbStatus3.VServerName.Length, mdbStatusRaw, (int)num3);
					num3 += (uint)(mdbStatus3.VServerName.Length + 1);
					if (mdbStatus3.LegacyDN != Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(mdbStatus3.LegacyDN)))
					{
						throw new CorruptDataException((LID)59464U, "LegacyDN string is not ASCII.");
					}
					Encoding.ASCII.GetBytes(mdbStatus3.LegacyDN, 0, mdbStatus3.LegacyDN.Length, mdbStatusRaw, (int)num3);
					num3 += (uint)(mdbStatus3.LegacyDN.Length + 1);
				}
				else
				{
					ParseSerialize.SerializeInt32(0, mdbStatusRaw, (int)(num4 + AdminRpcParseFormat.MdbStatusCbMdbNameOffset));
					ParseSerialize.SerializeInt32(0, mdbStatusRaw, (int)(num4 + AdminRpcParseFormat.MdbStatusCbVServerNameOffset));
					ParseSerialize.SerializeInt32(0, mdbStatusRaw, (int)(num4 + AdminRpcParseFormat.MdbStatusCbMdbLegacyDNOffset));
					ParseSerialize.SerializeInt32(0, mdbStatusRaw, (int)(num4 + AdminRpcParseFormat.MdbStatusCbStorageGroupNameOffset));
					ParseSerialize.SerializeInt32(0, mdbStatusRaw, (int)(num4 + AdminRpcParseFormat.MdbStatusIbMdbNameOffset));
					ParseSerialize.SerializeInt32(0, mdbStatusRaw, (int)(num4 + AdminRpcParseFormat.MdbStatusIbVServerNameOffset));
					ParseSerialize.SerializeInt32(0, mdbStatusRaw, (int)(num4 + AdminRpcParseFormat.MdbStatusIbMdbLegacyDNOffset));
					ParseSerialize.SerializeInt32(0, mdbStatusRaw, (int)(num4 + AdminRpcParseFormat.MdbStatusIbStorageGroupNameOffset));
				}
			}
		}

		public static byte[] ReadSidOrNull(byte[] buffer, uint ib, uint cb)
		{
			if (cb < 8U || buffer[(int)((UIntPtr)ib)] != 1 || buffer[(int)((UIntPtr)(ib + 1U))] > 15 || cb < (uint)(8 + buffer[(int)((UIntPtr)(ib + 1U))] * 4))
			{
				return null;
			}
			return ParseSerialize.ParseBinary(buffer, (int)ib, (int)(8 + buffer[(int)((UIntPtr)(ib + 1U))] * 4));
		}

		private static bool IsZeroByteArray(byte[] buffer, uint ib, uint cb)
		{
			while (cb-- != 0U)
			{
				if (buffer[(int)((UIntPtr)(ib++))] != 0)
				{
					return false;
				}
			}
			return true;
		}

		public static byte[] ReadByteArrayOrNull(byte[] buffer, uint ib, uint cb)
		{
			return AdminRpcParseFormat.ReadByteArrayOrNull(true, buffer, ib, cb);
		}

		public static byte[] ReadByteArrayOrNull(bool checkIsZeroByteArray, byte[] buffer, uint ib, uint cb)
		{
			if (checkIsZeroByteArray && AdminRpcParseFormat.IsZeroByteArray(buffer, ib, cb))
			{
				return null;
			}
			return ParseSerialize.ParseBinary(buffer, (int)ib, (int)cb);
		}

		public static string ReadAsciiStringOrNull(byte[] buffer, uint ib, uint cb)
		{
			uint num = 0U;
			while (num != cb && buffer[(int)((UIntPtr)(ib + num))] != 0)
			{
				num += 1U;
			}
			if (num != 0U)
			{
				return Encoding.ASCII.GetString(buffer, (int)ib, (int)num);
			}
			return null;
		}

		public static int? ReadInt32OrNull(byte[] buffer, uint ib, int defaultValue)
		{
			int num = ParseSerialize.ParseInt32(buffer, (int)ib);
			if (num != defaultValue)
			{
				return new int?(num);
			}
			return null;
		}

		public static long? ReadInt64OrNull(byte[] buffer, uint ib, long defaultValue)
		{
			long num = ParseSerialize.ParseInt64(buffer, (int)ib);
			if (num != defaultValue)
			{
				return new long?(num);
			}
			return null;
		}

		public static void WriteBytesIfNotNull(byte[] bytes, byte[] buffer, uint ib, uint cb)
		{
			if (bytes != null)
			{
				Buffer.BlockCopy(bytes, 0, buffer, (int)ib, Math.Min(bytes.Length, (int)cb));
			}
		}

		public static void WriteAsciiStringIfNotNull(string str, byte[] buffer, uint ib, uint cb)
		{
			if (str != null)
			{
				if ((long)str.Length >= (long)((ulong)cb))
				{
					str = str.Substring(0, (int)(cb - 1U));
				}
				ParseSerialize.SerializeAsciiString(str, buffer, (int)ib);
			}
		}

		public static void CheckBounds(int pos, int posMax, int sizeNeeded)
		{
			if (posMax < pos + sizeNeeded)
			{
				throw new BufferTooSmall((LID)55736U, "Rpc buffer is too small for results!");
			}
		}

		public static int SetValue(byte[] buff, ref int pos, int posMax, StorePropTag propTag, object propValue)
		{
			int num = 0;
			int num2 = -1;
			PropertyType propertyType = propTag.ExternalType;
			if (propertyType == PropertyType.Unspecified)
			{
				propertyType = propTag.PropType;
			}
			else if (propTag.PropType == PropertyType.Error)
			{
				propertyType = PropertyType.Error;
			}
			PropertyType propertyType2 = propertyType;
			if (propertyType2 <= PropertyType.SRestriction)
			{
				if (propertyType2 <= PropertyType.Unicode)
				{
					switch (propertyType2)
					{
					case PropertyType.Null:
						return num;
					case PropertyType.Int16:
						num = 2;
						if (buff == null)
						{
							return num;
						}
						AdminRpcParseFormat.CheckBounds(pos, posMax, num);
						if (propValue is short)
						{
							ParseSerialize.SetWord(buff, ref pos, (short)propValue);
							return num;
						}
						string.Format("Property {0} is type {1} and must be Int16", propTag, propValue.GetType());
						return num;
					case PropertyType.Int32:
						num = 4;
						if (buff == null)
						{
							return num;
						}
						AdminRpcParseFormat.CheckBounds(pos, posMax, num);
						if (propValue is int)
						{
							ParseSerialize.SetDword(buff, ref pos, (int)propValue);
							return num;
						}
						string.Format("Property {0} is type {1} and must be Int32", propTag, propValue.GetType());
						return num;
					case PropertyType.Real32:
						num = 4;
						if (buff != null)
						{
							AdminRpcParseFormat.CheckBounds(pos, posMax, num);
							ParseSerialize.SetFloat(buff, ref pos, (float)propValue);
							return num;
						}
						return num;
					case PropertyType.Real64:
					case PropertyType.AppTime:
						num = 8;
						if (buff != null)
						{
							AdminRpcParseFormat.CheckBounds(pos, posMax, num);
							ParseSerialize.SetDouble(buff, ref pos, (double)propValue);
							return num;
						}
						return num;
					case PropertyType.Currency:
						break;
					case (PropertyType)8:
					case (PropertyType)9:
						goto IL_876;
					case PropertyType.Error:
						num = 4;
						if (buff != null)
						{
							AdminRpcParseFormat.CheckBounds(pos, posMax, num);
							ParseSerialize.SetDword(buff, ref pos, (int)((ErrorCodeValue)propValue));
							return num;
						}
						return num;
					case PropertyType.Boolean:
						num = 1;
						if (buff != null)
						{
							AdminRpcParseFormat.CheckBounds(pos, posMax, num);
							ParseSerialize.SetByte(buff, ref pos, ((bool)propValue) ? 1 : 0);
							return num;
						}
						return num;
					default:
						if (propertyType2 != PropertyType.Int64)
						{
							switch (propertyType2)
							{
							case PropertyType.String8:
								num2 = ((string)propValue).IndexOf('\0');
								if (num2 > 0)
								{
									propValue = ((string)propValue).Substring(0, num2);
								}
								num = ((string)propValue).Length + 1;
								if (buff != null)
								{
									AdminRpcParseFormat.CheckBounds(pos, posMax, num);
									ParseSerialize.SetASCIIString(buff, ref pos, (string)propValue);
									return num;
								}
								return num;
							case PropertyType.Unicode:
								num2 = ((string)propValue).IndexOf('\0');
								if (num2 > 0)
								{
									propValue = ((string)propValue).Substring(0, num2);
								}
								num = (((string)propValue).Length + 1) * 2;
								if (buff != null)
								{
									AdminRpcParseFormat.CheckBounds(pos, posMax, num);
									ParseSerialize.SetUnicodeString(buff, ref pos, (string)propValue);
									return num;
								}
								return num;
							default:
								goto IL_876;
							}
						}
						break;
					}
					num = 8;
					if (buff == null)
					{
						return num;
					}
					AdminRpcParseFormat.CheckBounds(pos, posMax, num);
					if (propValue is long)
					{
						ParseSerialize.SetQword(buff, ref pos, (long)propValue);
						return num;
					}
					string.Format("Property {0} is type {1} and must be Int64", propTag, propValue.GetType());
					return num;
				}
				else if (propertyType2 != PropertyType.SysTime)
				{
					if (propertyType2 != PropertyType.Guid)
					{
						switch (propertyType2)
						{
						case PropertyType.SvrEid:
							break;
						case (PropertyType)252:
							goto IL_876;
						case PropertyType.SRestriction:
							if (!(propValue is byte[]))
							{
								string.Format("Property {0} is type {1} and must be byte[]", propTag, propValue.GetType());
								return num;
							}
							num = ((byte[])propValue).Length;
							if (buff != null)
							{
								AdminRpcParseFormat.CheckBounds(pos, posMax, num);
								ParseSerialize.SetRestrictionByteArray(buff, ref pos, (byte[])propValue);
								return num;
							}
							return num;
						default:
							goto IL_876;
						}
					}
					else
					{
						num = 16;
						if (buff != null)
						{
							AdminRpcParseFormat.CheckBounds(pos, posMax, num);
							ParseSerialize.SetGuid(buff, ref pos, (Guid)propValue);
							return num;
						}
						return num;
					}
				}
				else
				{
					num = 8;
					if (buff != null)
					{
						AdminRpcParseFormat.CheckBounds(pos, posMax, num);
						ParseSerialize.SetSysTime(buff, ref pos, (DateTime)propValue);
						return num;
					}
					return num;
				}
			}
			else if (propertyType2 <= PropertyType.MVInt64)
			{
				if (propertyType2 != PropertyType.Binary)
				{
					switch (propertyType2)
					{
					case PropertyType.MVInt16:
					{
						short[] array = (short[])propValue;
						num = 4;
						num += array.Length * 2;
						if (buff != null)
						{
							AdminRpcParseFormat.CheckBounds(pos, posMax, num);
							ParseSerialize.SetDword(buff, ref pos, array.Length);
							for (int i = 0; i < array.Length; i++)
							{
								ParseSerialize.SetWord(buff, ref pos, array[i]);
							}
							return num;
						}
						return num;
					}
					case PropertyType.MVInt32:
					{
						int[] array2 = (int[])propValue;
						num = 4;
						num += array2.Length * 4;
						if (buff != null)
						{
							AdminRpcParseFormat.CheckBounds(pos, posMax, num);
							ParseSerialize.SetDword(buff, ref pos, array2.Length);
							for (int j = 0; j < array2.Length; j++)
							{
								ParseSerialize.SetDword(buff, ref pos, array2[j]);
							}
							return num;
						}
						return num;
					}
					case PropertyType.MVReal32:
					{
						float[] array3 = (float[])propValue;
						num = 4;
						num += array3.Length * 4;
						if (buff != null)
						{
							AdminRpcParseFormat.CheckBounds(pos, posMax, num);
							ParseSerialize.SetDword(buff, ref pos, array3.Length);
							for (int k = 0; k < array3.Length; k++)
							{
								ParseSerialize.SetFloat(buff, ref pos, array3[k]);
							}
							return num;
						}
						return num;
					}
					case PropertyType.MVReal64:
					case PropertyType.MVAppTime:
					{
						double[] array4 = (double[])propValue;
						num = 4;
						num += array4.Length * 8;
						if (buff != null)
						{
							AdminRpcParseFormat.CheckBounds(pos, posMax, num);
							ParseSerialize.SetDword(buff, ref pos, array4.Length);
							for (int l = 0; l < array4.Length; l++)
							{
								ParseSerialize.SetDouble(buff, ref pos, array4[l]);
							}
							return num;
						}
						return num;
					}
					case PropertyType.MVCurrency:
						break;
					default:
						if (propertyType2 != PropertyType.MVInt64)
						{
							goto IL_876;
						}
						break;
					}
					long[] array5 = (long[])propValue;
					num = 4;
					num += array5.Length * 8;
					if (buff != null)
					{
						AdminRpcParseFormat.CheckBounds(pos, posMax, num);
						ParseSerialize.SetDword(buff, ref pos, array5.Length);
						for (int m = 0; m < array5.Length; m++)
						{
							ParseSerialize.SetQword(buff, ref pos, array5[m]);
						}
						return num;
					}
					return num;
				}
			}
			else if (propertyType2 <= PropertyType.MVSysTime)
			{
				switch (propertyType2)
				{
				case PropertyType.MVString8:
				{
					string[] array6 = (string[])propValue;
					num = 4;
					for (int n = 0; n < array6.Length; n++)
					{
						num2 = array6[n].IndexOf('\0');
						if (num2 > 0)
						{
							num += num2 + 1;
						}
						else
						{
							num += array6[n].Length + 1;
						}
					}
					if (buff != null)
					{
						AdminRpcParseFormat.CheckBounds(pos, posMax, num);
						ParseSerialize.SetDword(buff, ref pos, (uint)array6.Length);
						for (int num3 = 0; num3 < array6.Length; num3++)
						{
							num2 = array6[num3].IndexOf('\0');
							if (num2 > 0)
							{
								string str = array6[num3].Substring(0, num2);
								ParseSerialize.SetASCIIString(buff, ref pos, str);
							}
							else
							{
								ParseSerialize.SetASCIIString(buff, ref pos, array6[num3]);
							}
						}
						return num;
					}
					return num;
				}
				case PropertyType.MVUnicode:
				{
					string[] array7 = (string[])propValue;
					num = 4;
					bool flag = false;
					for (int num4 = 0; num4 < array7.Length; num4++)
					{
						num2 = array7[num4].IndexOf('\0');
						if (num2 > 0)
						{
							flag = true;
							num += (num2 + 1) * 2;
						}
						else
						{
							num += (array7[num4].Length + 1) * 2;
						}
					}
					if (buff != null)
					{
						AdminRpcParseFormat.CheckBounds(pos, posMax, num);
						ParseSerialize.SetDword(buff, ref pos, (uint)array7.Length);
						for (int num5 = 0; num5 < array7.Length; num5++)
						{
							if (flag)
							{
								num2 = array7[num5].IndexOf('\0');
							}
							if (num2 > 0)
							{
								string str2 = array7[num5].Substring(0, num2);
								ParseSerialize.SetUnicodeString(buff, ref pos, str2);
							}
							else
							{
								ParseSerialize.SetUnicodeString(buff, ref pos, array7[num5]);
							}
						}
						return num;
					}
					return num;
				}
				default:
				{
					if (propertyType2 != PropertyType.MVSysTime)
					{
						goto IL_876;
					}
					DateTime[] array8 = (DateTime[])propValue;
					num = 4;
					num += array8.Length * 8;
					if (buff != null)
					{
						AdminRpcParseFormat.CheckBounds(pos, posMax, num);
						ParseSerialize.SetDword(buff, ref pos, array8.Length);
						for (int num6 = 0; num6 < array8.Length; num6++)
						{
							ParseSerialize.SetSysTime(buff, ref pos, array8[num6]);
						}
						return num;
					}
					return num;
				}
				}
			}
			else if (propertyType2 != PropertyType.MVGuid)
			{
				if (propertyType2 != PropertyType.MVBinary)
				{
					goto IL_876;
				}
				byte[][] array9 = (byte[][])propValue;
				num = 4;
				for (int num7 = 0; num7 < array9.Length; num7++)
				{
					num += array9[num7].Length + 2;
				}
				if (buff != null)
				{
					AdminRpcParseFormat.CheckBounds(pos, posMax, num);
					ParseSerialize.SetDword(buff, ref pos, array9.Length);
					for (int num8 = 0; num8 < array9.Length; num8++)
					{
						ParseSerialize.SetByteArray(buff, ref pos, array9[num8]);
					}
					return num;
				}
				return num;
			}
			else
			{
				Guid[] array10 = (Guid[])propValue;
				num = 4;
				num += array10.Length * 16;
				if (buff != null)
				{
					AdminRpcParseFormat.CheckBounds(pos, posMax, num);
					ParseSerialize.SetDword(buff, ref pos, array10.Length);
					for (int num9 = 0; num9 < array10.Length; num9++)
					{
						ParseSerialize.SetGuid(buff, ref pos, array10[num9]);
					}
					return num;
				}
				return num;
			}
			if (!(propValue is byte[]))
			{
				string.Format("Property {0} is type {1} and must be byte[] or ExchangeId[] or Guid", propTag, propValue.GetType());
				return num;
			}
			num = ((byte[])propValue).Length + 2;
			if (buff != null)
			{
				AdminRpcParseFormat.CheckBounds(pos, posMax, num);
				ParseSerialize.SetByteArray(buff, ref pos, (byte[])propValue);
				return num;
			}
			return num;
			IL_876:
			throw new ExExceptionNoSupport((LID)43448U, "We do not support this property type (" + propTag.PropType + ") yet!");
		}

		public static int SetValues(byte[] buff, ref int pos, int posMax, Properties properties)
		{
			bool flag = false;
			int num = 0;
			for (int i = 0; i < properties.Count; i++)
			{
				if (properties[i].IsError)
				{
					flag = true;
					break;
				}
			}
			num++;
			if (buff != null)
			{
				AdminRpcParseFormat.CheckBounds(pos, posMax, 1);
				ParseSerialize.SetByte(buff, ref pos, flag ? 1 : 0);
			}
			for (int j = 0; j < properties.Count; j++)
			{
				if (properties[j].Tag.ExternalType == PropertyType.Unspecified)
				{
					num += 2;
					if (buff != null)
					{
						PropertyType propType = properties[j].Tag.PropType;
						AdminRpcParseFormat.CheckBounds(pos, posMax, 2);
						ParseSerialize.SetWord(buff, ref pos, (ushort)propType);
					}
				}
				if (flag)
				{
					num++;
					if (buff != null)
					{
						AdminRpcParseFormat.CheckBounds(pos, posMax, 1);
						if (properties[j].IsError)
						{
							ParseSerialize.SetByte(buff, ref pos, (byte)properties[j].Tag.PropType);
						}
						else
						{
							ParseSerialize.SetByte(buff, ref pos, 0);
						}
					}
				}
				if (PropertyType.Null != properties[j].Tag.PropType)
				{
					num += AdminRpcParseFormat.SetValue(buff, ref pos, posMax, properties[j].Tag, properties[j].Value);
				}
			}
			return num;
		}

		public static readonly uint DataBlockLengthOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.DataBlock), "BlockLength"));

		public static readonly uint DataBlockLengthLength = (uint)Marshal.SizeOf(typeof(uint));

		public static readonly uint DataBlockLengthPaddingMultiple = 8U;

		public static readonly uint EventEntryMaskOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV1), "EventMask"));

		public static readonly uint EventEntryMaskLength = (uint)Marshal.SizeOf(typeof(int));

		public static readonly uint EventEntryEventCounterOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV1), "EventCounter"));

		public static readonly uint EventEntryEventCounterLength = (uint)Marshal.SizeOf(typeof(long));

		public static readonly uint EventEntryCreateTimeOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV1), "CreateTime"));

		public static readonly uint EventEntryCreateTimeLength = (uint)Marshal.SizeOf(typeof(long));

		public static readonly uint EventEntryTransacIdOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV1), "TransacId"));

		public static readonly uint EventEntryTransacIdLength = (uint)Marshal.SizeOf(typeof(int));

		public static readonly uint EventEntryItemCountOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV1), "ItemCount"));

		public static readonly uint EventEntryItemCountLength = (uint)Marshal.SizeOf(typeof(int));

		public static readonly uint EventEntryUnreadCountOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV1), "UnreadCount"));

		public static readonly uint EventEntryUnreadCountLength = (uint)Marshal.SizeOf(typeof(int));

		public static readonly uint EventEntryFlagsOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV1), "Flags"));

		public static readonly uint EventEntryFlagsLength = (uint)Marshal.SizeOf(typeof(int));

		public static readonly uint EventEntryMailboxGuidOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV1), "MailboxGuid"));

		public static readonly uint EventEntryMailboxGuidLength = (uint)Marshal.SizeOf(typeof(Guid));

		public static readonly uint EventEntryMapiEntryIdGuidOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV1), "MapiEntryIdGuid"));

		public static readonly uint EventEntryMapiEntryIdGuidLength = (uint)Marshal.SizeOf(typeof(Guid));

		public static readonly uint EventEntryFidOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV1), "Fid"));

		public static readonly uint EventEntryFidLength = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.Ltid));

		public static readonly uint EventEntryMidOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV1), "Mid"));

		public static readonly uint EventEntryMidLength = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.Ltid));

		public static readonly uint EventEntryParentFidOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV1), "ParentFid"));

		public static readonly uint EventEntryParentFidLength = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.Ltid));

		public static readonly uint EventEntryOldFidOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV1), "OldFid"));

		public static readonly uint EventEntryOldFidLength = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.Ltid));

		public static readonly uint EventEntryOldMidOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV1), "OldMid"));

		public static readonly uint EventEntryOldMidLength = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.Ltid));

		public static readonly uint EventEntryOldParentFidOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV1), "OldParentFid"));

		public static readonly uint EventEntryOldParentFidLength = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.Ltid));

		public static readonly uint EventEntryObjectClassOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV1), "ItemClass"));

		public static readonly uint EventEntryObjectClassLength = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.ObjectClass));

		public static readonly uint EventEntryLengthV1 = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.EventEntryBlockV1));

		public static readonly uint EventEntryExtendedFlagsOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV2), "ExtendedFlags"));

		public static readonly uint EventEntryExtendedFlagsLength = (uint)Marshal.SizeOf(typeof(long));

		public static readonly uint EventEntryLengthV2 = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.EventEntryBlockV2));

		public static readonly uint EventEntryClientTypeOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV3), "ClientType"));

		public static readonly uint EventEntryClientTypeLength = (uint)Marshal.SizeOf(typeof(int));

		public static readonly uint EventEntrySidOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV3), "ClientSid"));

		public static readonly uint EventEntrySidLength = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.Sid));

		public static readonly uint EventEntryLengthV3 = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.EventEntryBlockV3));

		public static readonly uint EventEntryDocIdOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV4), "DocId"));

		public static readonly uint EventEntryDocIdLength = (uint)Marshal.SizeOf(typeof(int));

		public static readonly uint EventEntryLengthV4 = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.EventEntryBlockV4));

		public static readonly uint EventEntryMailboxNumberOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV5), "MailboxNumber"));

		public static readonly uint EventEntryMailboxNumberLength = (uint)Marshal.SizeOf(typeof(int));

		public static readonly uint EventEntryLengthV5 = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.EventEntryBlockV5));

		public static readonly uint EventEntryTenantHintBlobSizeOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV6), "TenantHintBlobSize"));

		public static readonly uint EventEntryTenantHintBlobSizeLength = (uint)Marshal.SizeOf(typeof(int));

		public static readonly uint EventEntryTenantHintBlobOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV6), "TenantHintBlob"));

		public static readonly uint EventEntryTenantHintBlobLength = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.TenantHintBlob));

		public static readonly uint EventEntryLengthV6 = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.EventEntryBlockV6));

		public static readonly uint EventEntryUnifiedMailboxGuidOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.EventEntryBlockV7), "UnifiedMailboxGuid"));

		public static readonly uint EventEntryUnifiedMailboxGuidLength = (uint)Marshal.SizeOf(typeof(Guid));

		public static readonly uint EventEntryLengthV7 = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.EventEntryBlockV7));

		public static readonly uint ReadEventsRequestHeaderFlagsOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.ReadEventsRequestHeaderBlockV1), "Flags"));

		public static readonly uint ReadEventsRequestHeaderFlagsLength = (uint)Marshal.SizeOf(typeof(int));

		public static readonly uint ReadEventsRequestHeaderStartCounterOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.ReadEventsRequestHeaderBlockV1), "StartCounter"));

		public static readonly uint ReadEventsRequestHeaderStartCounterLength = (uint)Marshal.SizeOf(typeof(long));

		public static readonly uint ReadEventsRequestHeaderEventsWantOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.ReadEventsRequestHeaderBlockV1), "EventsWant"));

		public static readonly uint ReadEventsRequestHeaderEventsWantLength = (uint)Marshal.SizeOf(typeof(uint));

		public static readonly uint ReadEventsRequestHeaderLengthV1 = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.ReadEventsRequestHeaderBlockV1));

		public static readonly uint ReadEventsRequestHeaderEventsToCheckOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.ReadEventsRequestHeaderBlockV2), "EventsToCheck"));

		public static readonly uint ReadEventsRequestHeaderEventsToCheckLength = (uint)Marshal.SizeOf(typeof(uint));

		public static readonly uint ReadEventsRequestHeaderLengthV2 = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.ReadEventsRequestHeaderBlockV2));

		public static readonly uint ReadEventsResponseHeaderFlagsOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.ReadEventsResponseHeaderBlockV1), "Flags"));

		public static readonly uint ReadEventsResponseHeaderFlagsLength = (uint)Marshal.SizeOf(typeof(int));

		public static readonly uint ReadEventsResponseHeaderEventsCountOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.ReadEventsResponseHeaderBlockV1), "EventsCount"));

		public static readonly uint ReadEventsResponseHeaderEventsCountLength = (uint)Marshal.SizeOf(typeof(uint));

		public static readonly uint ReadEventsResponseHeaderLengthV1 = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.ReadEventsResponseHeaderBlockV1));

		public static readonly uint ReadEventsResponseHeaderPaddingOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.ReadEventsResponseHeaderBlockV2), "Padding"));

		public static readonly uint ReadEventsResponseHeaderPaddingLength = (uint)Marshal.SizeOf(typeof(int));

		public static readonly uint ReadEventsResponseHeaderEndCounterOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.ReadEventsResponseHeaderBlockV2), "EndCounter"));

		public static readonly uint ReadEventsResponseHeaderEndCounterLength = (uint)Marshal.SizeOf(typeof(long));

		public static readonly uint ReadEventsResponseHeaderLengthV2 = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.ReadEventsResponseHeaderBlockV2));

		public static readonly uint WriteEventsRequestHeaderFlagsOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.WriteEventsRequestHeaderBlock), "Flags"));

		public static readonly uint WriteEventsRequestHeaderFlagsLength = (uint)Marshal.SizeOf(typeof(int));

		public static readonly uint WriteEventsRequestHeaderEventsCountOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.WriteEventsRequestHeaderBlock), "EventsCount"));

		public static readonly uint WriteEventsRequestHeaderEventsCountLength = (uint)Marshal.SizeOf(typeof(uint));

		public static readonly uint WriteEventsRequestHeaderLength = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.WriteEventsRequestHeaderBlock));

		public static readonly uint WriteEventsResponseEventsCountOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.WriteEventsResponseBlockHeader), "EventsCount"));

		public static readonly uint WriteEventsResponseEventsCountLength = (uint)Marshal.SizeOf(typeof(uint));

		public static readonly uint WriteEventsResponseHeaderLength = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.WriteEventsResponseBlockHeader));

		public static readonly uint WriteEventsResponseSingleAdjustedEventCounterLength = (uint)Marshal.SizeOf(typeof(long));

		public static readonly uint SizeofMdbStatusRaw = (uint)Marshal.SizeOf(typeof(AdminRpcParseFormat.MdbStatusRaw));

		public static readonly uint MdbStatusUlStatusOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.MdbStatusRaw), "ulStatus"));

		public static readonly uint MdbStatusCbMdbNameOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.MdbStatusRaw), "cbMdbName"));

		public static readonly uint MdbStatusCbVServerNameOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.MdbStatusRaw), "cbVServerName"));

		public static readonly uint MdbStatusCbMdbLegacyDNOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.MdbStatusRaw), "cbMdbLegacyDN"));

		public static readonly uint MdbStatusCbStorageGroupNameOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.MdbStatusRaw), "cbStorageGroupName"));

		public static readonly uint MdbStatusIbMdbNameOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.MdbStatusRaw), "ibMdbName"));

		public static readonly uint MdbStatusIbVServerNameOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.MdbStatusRaw), "ibVServerName"));

		public static readonly uint MdbStatusIbMdbLegacyDNOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.MdbStatusRaw), "ibMdbLegacyDN"));

		public static readonly uint MdbStatusIbStorageGroupNameOffset = (uint)((int)Marshal.OffsetOf(typeof(AdminRpcParseFormat.MdbStatusRaw), "ibStorageGroupName"));

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct DataBlock
		{
			public uint BlockLength;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Ltid
		{
			public Guid ReplicationGuid;

			public ulong GlobcntAndPadding;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct ObjectClass
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 60)]
			public string Value;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Sid
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 68)]
			public byte[] Value;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct TenantHintBlob
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.U1)]
			public byte[] Value;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct EventEntryBlockV1
		{
			public AdminRpcParseFormat.DataBlock BlockHeader;

			public int EventMask;

			public long EventCounter;

			public long CreateTime;

			public int TransacId;

			public int ItemCount;

			public int UnreadCount;

			public int Flags;

			public Guid MailboxGuid;

			public Guid MapiEntryIdGuid;

			public AdminRpcParseFormat.Ltid Fid;

			public AdminRpcParseFormat.Ltid Mid;

			public AdminRpcParseFormat.Ltid ParentFid;

			public AdminRpcParseFormat.Ltid OldFid;

			public AdminRpcParseFormat.Ltid OldMid;

			public AdminRpcParseFormat.Ltid OldParentFid;

			public AdminRpcParseFormat.ObjectClass ItemClass;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct EventEntryBlockV2
		{
			public AdminRpcParseFormat.EventEntryBlockV1 V1;

			public long ExtendedFlags;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct EventEntryBlockV3
		{
			public AdminRpcParseFormat.EventEntryBlockV2 V2;

			public int ClientType;

			public AdminRpcParseFormat.Sid ClientSid;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct EventEntryBlockV4
		{
			public AdminRpcParseFormat.EventEntryBlockV3 V3;

			public int DocId;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct EventEntryBlockV5
		{
			public AdminRpcParseFormat.EventEntryBlockV4 V4;

			public int MailboxNumber;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct EventEntryBlockV6
		{
			public AdminRpcParseFormat.EventEntryBlockV5 V5;

			public int TenantHintBlobSize;

			public AdminRpcParseFormat.TenantHintBlob TenantHintBlob;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct EventEntryBlockV7
		{
			public AdminRpcParseFormat.EventEntryBlockV6 V6;

			public Guid UnifiedMailboxGuid;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct ReadEventsRequestHeaderBlockV1
		{
			public AdminRpcParseFormat.DataBlock BlockHeader;

			public int Flags;

			public long StartCounter;

			public uint EventsWant;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct ReadEventsRequestHeaderBlockV2
		{
			public AdminRpcParseFormat.ReadEventsRequestHeaderBlockV1 V1;

			public uint EventsToCheck;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct ReadEventsResponseHeaderBlockV1
		{
			public AdminRpcParseFormat.DataBlock BlockHeader;

			public int Flags;

			public uint EventsCount;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct ReadEventsResponseHeaderBlockV2
		{
			public AdminRpcParseFormat.ReadEventsResponseHeaderBlockV1 V1;

			public int Padding;

			public long EndCounter;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct WriteEventsRequestHeaderBlock
		{
			public AdminRpcParseFormat.DataBlock BlockHeader;

			public int Flags;

			public uint EventsCount;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct WriteEventsResponseBlockHeader
		{
			public AdminRpcParseFormat.DataBlock BlockHeader;

			public uint EventsCount;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct MdbStatusRaw
		{
			private Guid guidMdb;

			private int ulStatus;

			private int cbMdbName;

			private int cbVServerName;

			private int cbMdbLegacyDN;

			private int cbStorageGroupName;

			private int ibMdbName;

			private int ibVServerName;

			private int ibMdbLegacyDN;

			private int ibStorageGroupName;
		}
	}
}

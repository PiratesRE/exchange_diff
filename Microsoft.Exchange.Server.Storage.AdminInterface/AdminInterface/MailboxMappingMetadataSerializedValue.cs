using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.AdminInterface;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	internal static class MailboxMappingMetadataSerializedValue
	{
		internal static void Parse(byte[] buffer, ref int offset, Guid databaseGuid, Guid mailboxGuid, out Guid mappingSignatureGuid, out Guid localIdGuid, out ulong nextIdCounter, out uint reservedIdCounterRange, out ulong nextCnCounter, out uint reservedCnCounterRange)
		{
			mappingSignatureGuid = SerializedValue.ParseGuid(buffer, ref offset);
			localIdGuid = SerializedValue.ParseGuid(buffer, ref offset);
			nextIdCounter = (ulong)SerializedValue.ParseInt64(buffer, ref offset);
			reservedIdCounterRange = (uint)SerializedValue.ParseInt32(buffer, ref offset);
			nextCnCounter = (ulong)SerializedValue.ParseInt64(buffer, ref offset);
			reservedCnCounterRange = (uint)SerializedValue.ParseInt32(buffer, ref offset);
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug(48712L, "Database {0} : Mailbox {1} : Mapping signature GUID {2} : Local ID GUID {3} : Next ID counter {4} : Reserved ID counter range {5} : Next CN counter {6} : Reserved CN counter range {7}", new object[]
				{
					databaseGuid,
					mailboxGuid,
					mappingSignatureGuid,
					localIdGuid,
					nextIdCounter,
					reservedIdCounterRange,
					nextCnCounter,
					reservedCnCounterRange
				});
			}
		}

		internal static int Serialize(Mailbox mailbox, ulong nextIdCounter, uint reservedIdCounterRange, ulong nextCnCounter, uint reservedCnCounterRange, byte[] buffer, int offset)
		{
			int num = offset;
			Guid mappingSignatureGuid = mailbox.GetMappingSignatureGuid(mailbox.CurrentOperationContext);
			Guid localIdGuid = mailbox.GetLocalIdGuid(mailbox.CurrentOperationContext);
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace) && buffer != null)
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug(40520L, "Database {0} : Mailbox {1} : Mapping signature GUID {2} : Local ID GUID {3} : Next ID counter {4} : Reserved ID counter range {5} : Next CN counter {6} : Reserved CN counter range {7}", new object[]
				{
					mailbox.Database.MdbName,
					mailbox.MailboxNumber,
					mappingSignatureGuid,
					localIdGuid,
					nextIdCounter,
					reservedIdCounterRange,
					nextCnCounter,
					reservedCnCounterRange
				});
			}
			offset += SerializedValue.SerializeGuid(mappingSignatureGuid, buffer, offset);
			offset += SerializedValue.SerializeGuid(localIdGuid, buffer, offset);
			offset += SerializedValue.SerializeInt64((long)nextIdCounter, buffer, offset);
			offset += SerializedValue.SerializeInt32((int)reservedIdCounterRange, buffer, offset);
			offset += SerializedValue.SerializeInt64((long)nextCnCounter, buffer, offset);
			offset += SerializedValue.SerializeInt32((int)reservedCnCounterRange, buffer, offset);
			int num2 = offset - num;
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace) && buffer == null)
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<string, int, int>(65096L, "Database {0} : Mailbox {1} : Mailbox mapping metadata serialized length {2}", mailbox.Database.MdbName, mailbox.MailboxNumber, num2);
			}
			return num2;
		}
	}
}

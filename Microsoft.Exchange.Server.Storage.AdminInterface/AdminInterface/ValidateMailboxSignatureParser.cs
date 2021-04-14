using System;
using Microsoft.Exchange.Data.MailboxSignature;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.AdminInterface;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	internal class ValidateMailboxSignatureParser : MailboxSignatureParserBase
	{
		public override bool ParseMailboxBasicInformation(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug(56904L, "Validate mailbox basic information section.");
			}
			bool result;
			if (mailboxSignatureSectionMetadata.Version != 1)
			{
				result = false;
			}
			else
			{
				if (mailboxSignatureSectionMetadata.ElementsNumber != 1)
				{
					throw new CorruptDataException((LID)38072U, "Bad number of elements in mailbox signature basic information section.");
				}
				result = true;
			}
			MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			return result;
		}

		public override bool ParseMailboxMappingMetadata(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug(44616L, "Validate mailbox mapping metadata section.");
			}
			bool result;
			if (mailboxSignatureSectionMetadata.Version != 1)
			{
				result = false;
			}
			else
			{
				if (mailboxSignatureSectionMetadata.ElementsNumber != 1)
				{
					throw new CorruptDataException((LID)42168U, "Bad number of elements in mailbox signature mapping metadata section.");
				}
				result = true;
			}
			MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			return result;
		}

		public override bool ParseMailboxNamedPropertyMapping(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug(61000L, "Validate named property mapping section.");
			}
			bool result;
			if (mailboxSignatureSectionMetadata.Version != 1)
			{
				result = false;
			}
			else
			{
				if (mailboxSignatureSectionMetadata.ElementsNumber < 0 || mailboxSignatureSectionMetadata.ElementsNumber > 32767)
				{
					throw new CorruptDataException((LID)52040U, "Bad number of elements in mailbox signature named property mapping section.");
				}
				result = true;
			}
			MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			return result;
		}

		public override bool ParseMailboxReplidGuidMapping(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug(36424L, "Validate mailbox ReplId/GUID mapping section.");
			}
			bool result;
			if (mailboxSignatureSectionMetadata.Version != 1)
			{
				result = false;
			}
			else
			{
				if (mailboxSignatureSectionMetadata.ElementsNumber < 0 || mailboxSignatureSectionMetadata.ElementsNumber > (int)ReplidGuidMap.MaxReplidGuidNumber)
				{
					throw new CorruptDataException((LID)62280U, "Bad number of elements in mailbox signature Replid/GUID mapping section.");
				}
				result = true;
			}
			MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			return result;
		}

		public override bool ParseMailboxTenantHint(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug(44264L, "Validate mailbox tenant hint section.");
			}
			bool result;
			if (mailboxSignatureSectionMetadata.Version != 1)
			{
				result = false;
			}
			else
			{
				if (mailboxSignatureSectionMetadata.ElementsNumber != 1)
				{
					throw new CorruptDataException((LID)36072U, "Bad number of elements in mailbox signature tenant hint section.");
				}
				if (mailboxSignatureSectionMetadata.Length > 128)
				{
					throw new CorruptDataException((LID)39424U, "Persistable tenant hint exceeds maximal length.");
				}
				result = true;
			}
			MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			return result;
		}

		public override bool ParseMailboxShape(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug(38492L, "Validate mailbox-shape section.");
			}
			bool result;
			if (mailboxSignatureSectionMetadata.Version != 1)
			{
				result = false;
			}
			else
			{
				if (mailboxSignatureSectionMetadata.ElementsNumber != 1)
				{
					throw new CorruptDataException((LID)61020U, "Bad number of elements in mailbox signature mailbox-shape section.");
				}
				result = true;
			}
			MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			return result;
		}

		public override bool ParseMailboxTypeVersion(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug(60252L, "Validate mailbox type version section.");
			}
			bool result;
			if (mailboxSignatureSectionMetadata.Version != 1)
			{
				result = false;
			}
			else
			{
				if (mailboxSignatureSectionMetadata.ElementsNumber != 1)
				{
					throw new CorruptDataException((LID)43868U, "Bad number of elements in mailbox signature mailbox type version section.");
				}
				result = true;
			}
			MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			return result;
		}

		public override bool ParsePartitionInformation(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug(0L, "Validate partition information section.");
			}
			bool result;
			if (mailboxSignatureSectionMetadata.Version != 1)
			{
				result = false;
			}
			else
			{
				if (mailboxSignatureSectionMetadata.ElementsNumber != 1)
				{
					throw new CorruptDataException((LID)54684U, "Bad number of elements in mailbox partition information section.");
				}
				result = true;
			}
			MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			return result;
		}

		public override bool ParseUserInformation(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug(0L, "Validate user information section.");
			}
			bool result;
			if (mailboxSignatureSectionMetadata.Version != 1)
			{
				result = false;
			}
			else
			{
				if (mailboxSignatureSectionMetadata.ElementsNumber != 1)
				{
					throw new CorruptDataException((LID)58188U, "Bad number of elements in mailbox signature user information section.");
				}
				result = true;
			}
			MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			return result;
		}
	}
}

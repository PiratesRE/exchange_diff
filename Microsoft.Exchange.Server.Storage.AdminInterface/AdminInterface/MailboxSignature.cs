using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.MailboxSignature;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.AdminInterface;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropertyBlob;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	internal static class MailboxSignature
	{
		internal static void Parse(byte[] buffer, out MailboxSignatureSectionType foundSections)
		{
			ValidateMailboxSignatureParser sectionProcessor = new ValidateMailboxSignatureParser();
			MailboxSignatureSectionsContainer mailboxSignatureSectionsContainer;
			try
			{
				mailboxSignatureSectionsContainer = MailboxSignatureSectionsContainer.Parse(buffer, sectionProcessor);
			}
			catch (ArgumentException ex)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
				throw new CorruptDataException((LID)56552U, "Buffer doesn't contain valid mailbox signature.", ex);
			}
			foundSections = mailboxSignatureSectionsContainer.SectionTypes;
		}

		internal static void Parse(byte[] buffer, Guid databaseGuid, Guid mailboxGuid, out Guid mailboxInstanceGuid, out Guid folderGuid, out ExchangeId[] folderIdList, out Guid mappingSignatureGuid, out Guid localIdGuid, out ulong nextIdCounter, out uint? reservedIdCounterRange, out ulong nextCnCounter, out uint? reservedCnCounterRange, out TenantHint tenantHint, out Dictionary<ushort, StoreNamedPropInfo> numberToNameMap, out Dictionary<ushort, Guid> replidGuidMap, out PropertyBlob.BlobReader mailboxShapePropetyBlobReader, out Mailbox.MailboxTypeVersion mailboxTypeVersion, out PartitionInformation partitionInformation, out PropertyBlob.BlobReader userInformationPropetyBlobReader, out MailboxSignatureSectionType foundSections)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<Guid, Guid>(53320L, "Database {0} : Mailbox {1} : Parse mailbox signature.", databaseGuid, mailboxGuid);
			}
			ExtractMailboxSignatureParser extractMailboxSignatureParser = new ExtractMailboxSignatureParser(databaseGuid, mailboxGuid);
			MailboxSignatureSectionsContainer mailboxSignatureSectionsContainer;
			try
			{
				mailboxSignatureSectionsContainer = MailboxSignatureSectionsContainer.Parse(buffer, extractMailboxSignatureParser);
			}
			catch (InvalidSerializedFormatException ex)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
				throw new CorruptDataException((LID)38824U, "Mailbox signature contains corrupted data.", ex);
			}
			catch (ArgumentException ex2)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex2);
				throw new CorruptDataException((LID)36024U, "Buffer doesn't contain valid mailbox signature.", ex2);
			}
			mailboxInstanceGuid = extractMailboxSignatureParser.MailboxInstanceGuid;
			folderGuid = extractMailboxSignatureParser.FolderGuid;
			folderIdList = extractMailboxSignatureParser.FolderIdList;
			mappingSignatureGuid = extractMailboxSignatureParser.MappingSignatureGuid;
			localIdGuid = extractMailboxSignatureParser.LocalIdGuid;
			nextIdCounter = extractMailboxSignatureParser.NextIdCounter;
			reservedIdCounterRange = new uint?(extractMailboxSignatureParser.ReservedIdCounterRange);
			nextCnCounter = extractMailboxSignatureParser.NextCnCounter;
			reservedCnCounterRange = new uint?(extractMailboxSignatureParser.ReservedCnCounterRange);
			tenantHint = extractMailboxSignatureParser.TenantHint;
			numberToNameMap = extractMailboxSignatureParser.NumberToNameMap;
			replidGuidMap = extractMailboxSignatureParser.ReplidGuidMap;
			mailboxShapePropetyBlobReader = extractMailboxSignatureParser.MailboxShapePropertyBlobReader;
			mailboxTypeVersion = extractMailboxSignatureParser.MailboxTypeVersion;
			partitionInformation = extractMailboxSignatureParser.PartitionInformation;
			foundSections = mailboxSignatureSectionsContainer.SectionTypes;
			userInformationPropetyBlobReader = extractMailboxSignatureParser.UserInformationPropertyBlobReader;
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<Guid, Guid>(41032L, "Database {0} : Mailbox {1} : Parse mailbox signature done.", databaseGuid, mailboxGuid);
			}
		}

		internal static void Serialize(Context context, Mailbox mailbox, MailboxSignatureSectionType sectionsToCreate, out byte[] buffer)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<string, int, string>(57416L, "Database {0} : Mailbox {1} : {2} : Serialize mailbox signature.", mailbox.Database.MdbName, mailbox.MailboxNumber, mailbox.GetDisplayName(context));
			}
			if (((short)(sectionsToCreate & (MailboxSignatureSectionType.BasicInformation | MailboxSignatureSectionType.MappingMetadata | MailboxSignatureSectionType.NamedPropertyMapping | MailboxSignatureSectionType.ReplidGuidMapping | MailboxSignatureSectionType.TenantHint)) == 31 || (short)(sectionsToCreate & MailboxSignatureSectionType.MappingMetadata) == 2) && mailbox.GetPreservingMailboxSignature(context))
			{
				throw new Microsoft.Exchange.Server.Storage.LogicalDataModel.NotSupportedException((LID)44104U, "The mailbox is the destination of a mailbox signature preserving mailbox move.");
			}
			try
			{
				MailboxSignatureSectionsContainer mailboxSignatureSectionsContainer = MailboxSignatureSectionsContainer.Create(sectionsToCreate, new MailboxSignatureSerializer(context, mailbox, sectionsToCreate));
				buffer = mailboxSignatureSectionsContainer.Serialize();
			}
			catch (ArgumentException ex)
			{
				context.OnExceptionCatch(ex);
				throw new StoreException((LID)46312U, ErrorCodeValue.CorruptData, "ArgumentException on Create/Serialize.", ex);
			}
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<string, int, string>(32840L, "Database {0} : Mailbox {1} : {2} : Serialize mailbox signature done.", mailbox.Database.MdbName, mailbox.MailboxNumber, mailbox.GetDisplayName(context));
			}
		}

		public const short RequiredBasicInformationVersion = 1;

		public const short RequiredMappingMetadataVersion = 1;

		public const short RequiredNamedPropertyMappingVersion = 1;

		public const short RequiredReplidGuidMappingVersion = 1;

		public const short RequiredTenantHintVersion = 1;

		public const short RequiredMailboxShapeVersion = 1;

		public const short RequiredMailboxTypeVersionVersion = 1;

		public const short RequiredPartitionInformationVersion = 1;

		public const short RequiredUserInformationVersion = 1;

		public const MailboxSignatureSectionType PreserveMailboxSignatureSections = MailboxSignatureSectionType.BasicInformation | MailboxSignatureSectionType.MappingMetadata | MailboxSignatureSectionType.NamedPropertyMapping | MailboxSignatureSectionType.ReplidGuidMapping | MailboxSignatureSectionType.TenantHint;

		public const MailboxSignatureSectionType NonPreserveMailboxSignatureSections = MailboxSignatureSectionType.BasicInformation | MailboxSignatureSectionType.TenantHint;

		public const MailboxSignatureSectionType MailboxSignatureMappings = MailboxSignatureSectionType.NamedPropertyMapping | MailboxSignatureSectionType.ReplidGuidMapping;

		public const MailboxSignatureSectionType MailboxSignatureMappingMetadata = MailboxSignatureSectionType.MappingMetadata;
	}
}

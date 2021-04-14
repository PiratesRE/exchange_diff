using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.MailboxSignature;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.AdminInterface;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropertyBlob;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	internal class ExtractMailboxSignatureParser : MailboxSignatureParserBase
	{
		private ExtractMailboxSignatureParser()
		{
		}

		internal ExtractMailboxSignatureParser(Guid databaseGuid, Guid mailboxGuid)
		{
			this.databaseGuid = databaseGuid;
			this.mailboxGuid = mailboxGuid;
		}

		public Guid MailboxInstanceGuid
		{
			get
			{
				return this.mailboxInstanceGuid;
			}
		}

		public Guid FolderGuid
		{
			get
			{
				return this.folderGuid;
			}
		}

		public ExchangeId[] FolderIdList
		{
			get
			{
				return this.fidList;
			}
		}

		public Guid MappingSignatureGuid
		{
			get
			{
				return this.mappingSignatureGuid;
			}
		}

		public Guid LocalIdGuid
		{
			get
			{
				return this.localIdGuid;
			}
		}

		public ulong NextIdCounter
		{
			get
			{
				return this.nextIdCounter;
			}
		}

		public uint ReservedIdCounterRange
		{
			get
			{
				return this.reservedIdCounterRange;
			}
		}

		public ulong NextCnCounter
		{
			get
			{
				return this.nextCnCounter;
			}
		}

		public uint ReservedCnCounterRange
		{
			get
			{
				return this.reservedCnCounterRange;
			}
		}

		public TenantHint TenantHint
		{
			get
			{
				return this.tenantHint;
			}
		}

		public PropertyBlob.BlobReader MailboxShapePropertyBlobReader
		{
			get
			{
				return this.mailboxShapePropertyBlobReader;
			}
		}

		public Dictionary<ushort, StoreNamedPropInfo> NumberToNameMap
		{
			get
			{
				return this.numberToNameMap;
			}
		}

		public Dictionary<ushort, Guid> ReplidGuidMap
		{
			get
			{
				return this.replidGuidMap;
			}
		}

		public Mailbox.MailboxTypeVersion MailboxTypeVersion
		{
			get
			{
				return this.mailboxTypeVersion;
			}
		}

		public PartitionInformation PartitionInformation
		{
			get
			{
				return this.partitionInformation;
			}
		}

		public PropertyBlob.BlobReader UserInformationPropertyBlobReader
		{
			get
			{
				return this.userInformationPropertyBlobReader;
			}
		}

		public override bool ParseMailboxBasicInformation(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<Guid, Guid>(52808L, "Database {0} : Mailbox {1} : Parse mailbox basic information section.", this.databaseGuid, this.mailboxGuid);
			}
			bool result;
			if (mailboxSignatureSectionMetadata.Version != 1)
			{
				result = false;
				MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			}
			else
			{
				MailboxBasicInformation.ParseMailboxBasicInformation(buffer, ref offset, this.databaseGuid, this.mailboxGuid, out this.mailboxInstanceGuid, out this.folderGuid, out this.fidList);
				result = true;
			}
			return result;
		}

		public override bool ParseMailboxMappingMetadata(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<Guid, Guid>(46664L, "Database {0} : Mailbox {1} : Parse mailbox mapping metadata section.", this.databaseGuid, this.mailboxGuid);
			}
			bool result;
			if (mailboxSignatureSectionMetadata.Version != 1)
			{
				result = false;
				MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			}
			else
			{
				MailboxMappingMetadataSerializedValue.Parse(buffer, ref offset, this.databaseGuid, this.mailboxGuid, out this.mappingSignatureGuid, out this.localIdGuid, out this.nextIdCounter, out this.reservedIdCounterRange, out this.nextCnCounter, out this.reservedCnCounterRange);
				result = true;
			}
			return result;
		}

		public override bool ParseMailboxNamedPropertyMapping(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<Guid, Guid>(63048L, "Database {0} : Mailbox {1} : Parse named property mapping section.", this.databaseGuid, this.mailboxGuid);
			}
			bool result;
			if (mailboxSignatureSectionMetadata.Version != 1)
			{
				result = false;
				MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			}
			else
			{
				StoreSerializedValue.ParseNamedPropertyMap(buffer, ref offset, mailboxSignatureSectionMetadata.ElementsNumber, this.databaseGuid, this.mailboxGuid, out this.numberToNameMap);
				result = true;
			}
			return result;
		}

		public override bool ParseMailboxReplidGuidMapping(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<Guid, Guid>(38472L, "Database {0} : Mailbox {1} : Parse ReplId/GUID mapping section.", this.databaseGuid, this.mailboxGuid);
			}
			bool result;
			if (mailboxSignatureSectionMetadata.Version != 1)
			{
				result = false;
				MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			}
			else
			{
				StoreSerializedValue.ParseReplidGuidMap(buffer, ref offset, mailboxSignatureSectionMetadata.ElementsNumber, this.databaseGuid, this.mailboxGuid, out this.replidGuidMap);
				result = true;
			}
			return result;
		}

		public override bool ParseMailboxTenantHint(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<Guid, Guid>(62696L, "Database {0} : Mailbox {1} : Parse tenant hint section.", this.databaseGuid, this.mailboxGuid);
			}
			bool result;
			if (mailboxSignatureSectionMetadata.Version != 1)
			{
				result = false;
				MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			}
			else
			{
				byte[] tenantHintBlob = TenantHintHelper.ParseTenantHint(mailboxSignatureSectionMetadata, buffer, ref offset);
				this.tenantHint = new TenantHint(tenantHintBlob);
				result = true;
			}
			return result;
		}

		public override bool ParseMailboxShape(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<Guid, Guid>(46684L, "Database {0} : Mailbox {1} : Parse mailbox-shape section.", this.databaseGuid, this.mailboxGuid);
			}
			bool result;
			if (mailboxSignatureSectionMetadata.Version != 1)
			{
				result = false;
				MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			}
			else
			{
				if (mailboxSignatureSectionMetadata.Length > 0)
				{
					this.mailboxShapePropertyBlobReader = new PropertyBlob.BlobReader(buffer, offset);
					offset += mailboxSignatureSectionMetadata.Length;
				}
				else
				{
					this.mailboxShapePropertyBlobReader = new PropertyBlob.BlobReader(null, 0);
				}
				result = true;
			}
			return result;
		}

		public override bool ParseMailboxTypeVersion(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<Guid, Guid>(45916L, "Database {0} : Mailbox {1} : Parse mailbox type version section.", this.databaseGuid, this.mailboxGuid);
			}
			bool result;
			if (mailboxSignatureSectionMetadata.Version != 1)
			{
				result = false;
				MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			}
			else
			{
				this.mailboxTypeVersion = MailboxTypeVersionHelper.Parse(mailboxSignatureSectionMetadata, buffer, ref offset);
				result = true;
			}
			return result;
		}

		public override bool ParsePartitionInformation(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<Guid, Guid>(0L, "Database {0} : Mailbox {1} : Parse mailbox partition information section.", this.databaseGuid, this.mailboxGuid);
			}
			bool result;
			if (mailboxSignatureSectionMetadata.Version != 1)
			{
				result = false;
				MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			}
			else
			{
				this.partitionInformation = PartitionInformation.Parse(mailboxSignatureSectionMetadata, buffer, ref offset);
				result = true;
			}
			return result;
		}

		public override bool ParseUserInformation(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<Guid, Guid>(0L, "Database {0} : Mailbox {1} : Parse user information section.", this.databaseGuid, this.mailboxGuid);
			}
			bool result;
			if (mailboxSignatureSectionMetadata.Version != 1)
			{
				result = false;
				MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			}
			else
			{
				if (mailboxSignatureSectionMetadata.Length > 0)
				{
					this.userInformationPropertyBlobReader = new PropertyBlob.BlobReader(buffer, offset);
					offset += mailboxSignatureSectionMetadata.Length;
				}
				else
				{
					this.userInformationPropertyBlobReader = new PropertyBlob.BlobReader(null, 0);
				}
				result = true;
			}
			return result;
		}

		private readonly Guid databaseGuid;

		private readonly Guid mailboxGuid;

		private Guid mailboxInstanceGuid;

		private Guid folderGuid;

		private ExchangeId[] fidList;

		private Guid mappingSignatureGuid;

		private Guid localIdGuid;

		private ulong nextIdCounter;

		private uint reservedIdCounterRange;

		private ulong nextCnCounter;

		private uint reservedCnCounterRange;

		private TenantHint tenantHint;

		private PropertyBlob.BlobReader mailboxShapePropertyBlobReader;

		private Dictionary<ushort, StoreNamedPropInfo> numberToNameMap;

		private Dictionary<ushort, Guid> replidGuidMap;

		private Mailbox.MailboxTypeVersion mailboxTypeVersion;

		private PartitionInformation partitionInformation;

		private PropertyBlob.BlobReader userInformationPropertyBlobReader;
	}
}

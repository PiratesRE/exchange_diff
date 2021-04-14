using System;
using Microsoft.Exchange.Data.MailboxSignature;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.AdminInterface;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropertyBlob;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	internal class MailboxSignatureSerializer : IMailboxSignatureSectionCreator
	{
		public MailboxSignatureSerializer(Context context, Mailbox mailbox, MailboxSignatureSectionType sectionsToCreate)
		{
			this.mailbox = mailbox;
			this.context = context;
			if ((short)(sectionsToCreate & MailboxSignatureSectionType.MappingMetadata) != 0)
			{
				this.mailbox.GetReservedCounterRangesForDestinationMailbox(context, out this.nextIdCounter, out this.reservedIdCounterRange, out this.nextCnCounter, out this.reservedCnCounterRange);
			}
		}

		public bool Create(MailboxSignatureSectionType sectionType, out MailboxSignatureSectionMetadata sectionMetadata, out byte[] sectionData)
		{
			sectionMetadata = null;
			sectionData = null;
			if (sectionType <= MailboxSignatureSectionType.TenantHint)
			{
				switch (sectionType)
				{
				case MailboxSignatureSectionType.BasicInformation:
					return this.SerializeMailboxBasicInformation(out sectionMetadata, out sectionData);
				case MailboxSignatureSectionType.MappingMetadata:
					return this.SerializeMailboxMappingMetadata(out sectionMetadata, out sectionData);
				case MailboxSignatureSectionType.BasicInformation | MailboxSignatureSectionType.MappingMetadata:
					break;
				case MailboxSignatureSectionType.NamedPropertyMapping:
					return this.SerializeNamedPropertyMap(out sectionMetadata, out sectionData);
				default:
					if (sectionType == MailboxSignatureSectionType.ReplidGuidMapping)
					{
						return this.SerializeReplIdGuidMap(out sectionMetadata, out sectionData);
					}
					if (sectionType == MailboxSignatureSectionType.TenantHint)
					{
						return this.SerializeTenantHint(out sectionMetadata, out sectionData);
					}
					break;
				}
			}
			else if (sectionType <= MailboxSignatureSectionType.MailboxTypeVersion)
			{
				if (sectionType == MailboxSignatureSectionType.MailboxShape)
				{
					return this.SerializeMailboxShape(out sectionMetadata, out sectionData);
				}
				if (sectionType == MailboxSignatureSectionType.MailboxTypeVersion)
				{
					return this.SerializeMailboxTypeVersion(out sectionMetadata, out sectionData);
				}
			}
			else
			{
				if (sectionType == MailboxSignatureSectionType.PartitionInformation)
				{
					return this.SerializePartitionInformation(out sectionMetadata, out sectionData);
				}
				if (sectionType == MailboxSignatureSectionType.UserInformation)
				{
					return this.SerializeUserInformation(out sectionMetadata, out sectionData);
				}
			}
			return false;
		}

		private static bool SerializeMailboxSignatureSection(MailboxSignatureSectionType mailboxSignatureSectionType, short requiredMailboxSignatureSectionMetadataVersion, MailboxSignatureSerializer.SectionSerializer sectionSerializer, out MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, out byte[] sectionData)
		{
			int elementsNumber;
			int num = sectionSerializer(null, 0, out elementsNumber);
			if (num == 0)
			{
				mailboxSignatureSectionMetadata = null;
				sectionData = null;
				return false;
			}
			mailboxSignatureSectionMetadata = new MailboxSignatureSectionMetadata(mailboxSignatureSectionType, requiredMailboxSignatureSectionMetadataVersion, elementsNumber, num);
			sectionData = new byte[num];
			sectionSerializer(sectionData, 0, out elementsNumber);
			return true;
		}

		internal bool SerializeMailboxBasicInformation(out MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, out byte[] sectionData)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<string, int>(34888L, "Database {0} : Mailbox {1} : Serialize mailbox basic information.", this.mailbox.Database.MdbName, this.mailbox.MailboxNumber);
			}
			return MailboxSignatureSerializer.SerializeMailboxSignatureSection(MailboxSignatureSectionType.BasicInformation, 1, new MailboxSignatureSerializer.SectionSerializer(this.SerializeMailboxBasicInformationDelegate), out mailboxSignatureSectionMetadata, out sectionData);
		}

		internal bool SerializeMailboxMappingMetadata(out MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, out byte[] sectionData)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<string, int>(45128L, "Database {0} : Mailbox {1} : Serialize mailbox mapping metadata.", this.mailbox.Database.MdbName, this.mailbox.MailboxNumber);
			}
			return MailboxSignatureSerializer.SerializeMailboxSignatureSection(MailboxSignatureSectionType.MappingMetadata, 1, new MailboxSignatureSerializer.SectionSerializer(this.SerializeMailboxMappingMetadataDelegate), out mailboxSignatureSectionMetadata, out sectionData);
		}

		internal bool SerializeNamedPropertyMap(out MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, out byte[] sectionData)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<string, int>(61512L, "Database {0} : Mailbox {1} : Serialize named property mapping.", this.mailbox.Database.MdbName, this.mailbox.MailboxNumber);
			}
			return MailboxSignatureSerializer.SerializeMailboxSignatureSection(MailboxSignatureSectionType.NamedPropertyMapping, 1, new MailboxSignatureSerializer.SectionSerializer(this.SerializeNamedPropertyMappingDelegate), out mailboxSignatureSectionMetadata, out sectionData);
		}

		internal bool SerializeReplIdGuidMap(out MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, out byte[] sectionData)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<string, int>(36936L, "Database {0} : Mailbox {1} : Serialize ReplId/GUID mapping.", this.mailbox.Database.MdbName, this.mailbox.MailboxNumber);
			}
			return MailboxSignatureSerializer.SerializeMailboxSignatureSection(MailboxSignatureSectionType.ReplidGuidMapping, 1, new MailboxSignatureSerializer.SectionSerializer(this.SerializeReplidGuidMappingDelegate), out mailboxSignatureSectionMetadata, out sectionData);
		}

		internal bool SerializeTenantHint(out MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, out byte[] sectionData)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<string, int>(52456L, "Database {0} : Mailbox {1} : Serialize tenant hint.", this.mailbox.Database.MdbName, this.mailbox.MailboxNumber);
			}
			return MailboxSignatureSerializer.SerializeMailboxSignatureSection(MailboxSignatureSectionType.TenantHint, 1, new MailboxSignatureSerializer.SectionSerializer(this.SerializeTenantHintDelegate), out mailboxSignatureSectionMetadata, out sectionData);
		}

		internal bool SerializeMailboxShape(out MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, out byte[] sectionData)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<string, int>(63068L, "Database {0} : Mailbox {1} : Serialize mailbox-shape.", this.mailbox.Database.MdbName, this.mailbox.MailboxNumber);
			}
			return MailboxSignatureSerializer.SerializeMailboxSignatureSection(MailboxSignatureSectionType.MailboxShape, 1, new MailboxSignatureSerializer.SectionSerializer(this.SerializeMailboxShapeDelegate), out mailboxSignatureSectionMetadata, out sectionData);
		}

		internal bool SerializeMailboxTypeVersion(out MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, out byte[] sectionData)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<string, int>(54108L, "Database {0} : Mailbox {1} : Serialize mailbox type version.", this.mailbox.Database.MdbName, this.mailbox.MailboxNumber);
			}
			return MailboxSignatureSerializer.SerializeMailboxSignatureSection(MailboxSignatureSectionType.MailboxTypeVersion, 1, new MailboxSignatureSerializer.SectionSerializer(this.SerializeMailboxTypeVersionDelegate), out mailboxSignatureSectionMetadata, out sectionData);
		}

		internal bool SerializePartitionInformation(out MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, out byte[] sectionData)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<string, int>(0L, "Database {0} : Mailbox {1} : Serialize partition information.", this.mailbox.Database.MdbName, this.mailbox.MailboxNumber);
			}
			return MailboxSignatureSerializer.SerializeMailboxSignatureSection(MailboxSignatureSectionType.PartitionInformation, 1, new MailboxSignatureSerializer.SectionSerializer(this.SerializePartitionInformationDelegate), out mailboxSignatureSectionMetadata, out sectionData);
		}

		internal bool SerializeUserInformation(out MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, out byte[] sectionData)
		{
			if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxSignatureTracer.TraceDebug<string, int>(0L, "Database {0} : Mailbox {1} : Serialize user information.", this.mailbox.Database.MdbName, this.mailbox.MailboxNumber);
			}
			return MailboxSignatureSerializer.SerializeMailboxSignatureSection(MailboxSignatureSectionType.UserInformation, 1, new MailboxSignatureSerializer.SectionSerializer(this.SerializeUserInformationDelegate), out mailboxSignatureSectionMetadata, out sectionData);
		}

		private int SerializeMailboxBasicInformationDelegate(byte[] buffer, int offset, out int elementsCount)
		{
			elementsCount = 1;
			return MailboxBasicInformation.SerializeMailboxBasicInformation(this.mailbox, buffer, offset);
		}

		private int SerializeMailboxMappingMetadataDelegate(byte[] buffer, int offset, out int elementsCount)
		{
			elementsCount = 1;
			return MailboxMappingMetadataSerializedValue.Serialize(this.mailbox, this.nextIdCounter, this.reservedIdCounterRange, this.nextCnCounter, this.reservedCnCounterRange, buffer, offset);
		}

		private int SerializeNamedPropertyMappingDelegate(byte[] buffer, int offset, out int elementsCount)
		{
			elementsCount = this.mailbox.NamedPropertyMap.GetNamedPropertyCount(this.context);
			return StoreSerializedValue.SerializeNamedPropertyMap(this.context, this.mailbox, buffer, offset);
		}

		private int SerializeReplidGuidMappingDelegate(byte[] buffer, int offset, out int elementsCount)
		{
			elementsCount = this.mailbox.ReplidGuidMap.GetReplidCount(this.context);
			return StoreSerializedValue.SerializeReplidGuidMap(this.mailbox, buffer, offset);
		}

		private int SerializeTenantHintDelegate(byte[] buffer, int offset, out int elementsCount)
		{
			elementsCount = 1;
			return TenantHintHelper.SerializeTenantHint(this.mailbox.SharedState.TenantHint.TenantHintBlob, buffer, offset);
		}

		private int SerializeMailboxShapeDelegate(byte[] buffer, int offset, out int elementsCount)
		{
			elementsCount = 1;
			return this.mailbox.SerializeMailboxShape(buffer, offset);
		}

		private int SerializeMailboxTypeVersionDelegate(byte[] buffer, int offset, out int elementsCount)
		{
			elementsCount = 1;
			return MailboxTypeVersionHelper.Serialize(this.context, this.mailbox, buffer, offset);
		}

		private int SerializePartitionInformationDelegate(byte[] buffer, int offset, out int elementsCount)
		{
			elementsCount = 1;
			return new PartitionInformation(this.mailbox.SharedState.UnifiedState.UnifiedMailboxGuid, PartitionInformation.ControlFlags.None).Serialize(buffer, offset);
		}

		private int SerializeUserInformationDelegate(byte[] buffer, int offset, out int elementsCount)
		{
			elementsCount = 1;
			Properties allProperties = UserInformation.GetAllProperties(this.mailbox.CurrentOperationContext, this.mailbox.MailboxGuid);
			if (allProperties.Count == 0)
			{
				return 0;
			}
			byte[] array = PropertyBlob.BuildBlob((from p in allProperties
			select p.Tag).ToArray<StorePropTag>(), (from p in allProperties
			select p.Value).ToArray<object>());
			if (buffer != null)
			{
				Buffer.BlockCopy(array, 0, buffer, offset, array.Length);
			}
			return array.Length;
		}

		private uint reservedIdCounterRange;

		private uint reservedCnCounterRange;

		private Mailbox mailbox;

		private Context context;

		private ulong nextIdCounter;

		private ulong nextCnCounter;

		private delegate int SectionSerializer(byte[] buffer, int offset, out int elementsCount);
	}
}

using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class StoreParticipantEntryId : ParticipantEntryId
	{
		internal StoreParticipantEntryId(ParticipantEntryId.Reader reader)
		{
			uint num = reader.ReadUInt32();
			if (num != 3U)
			{
				throw new NotSupportedException(ServerStrings.ExUnsupportedABProvider("OLABP", num.ToString()));
			}
			uint num2 = reader.ReadUInt32();
			this.isMapiPDL = (num2 == 5U);
			this.emailAddressIndex = StoreParticipantEntryId.IndexToEmailAddressIndex(reader.ReadUInt32());
			if (reader.ReadUInt32() != 70U)
			{
				throw new NotSupportedException(ServerStrings.ExInvalidParticipantEntryId);
			}
			this.ltEntryId = reader.ReadLTEntryId();
			if (reader.BytesRemaining != 3)
			{
				reader.EnsureEnd();
			}
		}

		internal StoreParticipantEntryId(ParticipantEntryId.WabEntryFlag flags, ParticipantEntryId.Reader reader)
		{
			EnumValidator.AssertValid<ParticipantEntryId.WabEntryFlag>(flags);
			this.isMapiPDL = StoreParticipantEntryId.WabEntryFlagToMapiPDL(flags);
			this.emailAddressIndex = StoreParticipantEntryId.WabEntryFlagToEmailAddressIndex(flags);
			this.ltEntryId = reader.ReadLTEntryId();
			this.useWabFormat = true;
			reader.EnsureEnd();
		}

		private StoreParticipantEntryId(StoreObjectId itemId, bool isMapiPDL, EmailAddressIndex emailIndex, bool useWabFormat)
		{
			EnumValidator.AssertValid<EmailAddressIndex>(emailIndex);
			using (ParticipantEntryId.Reader reader = new ParticipantEntryId.Reader(itemId.ProviderLevelItemId))
			{
				this.ltEntryId = reader.ReadLTEntryId();
				reader.EnsureEnd();
			}
			this.isMapiPDL = isMapiPDL;
			this.emailAddressIndex = emailIndex;
			this.useWabFormat = useWabFormat;
		}

		internal EmailAddressIndex EmailAddressIndex
		{
			get
			{
				return this.emailAddressIndex;
			}
		}

		internal override bool? IsDL
		{
			get
			{
				return new bool?(this.isMapiPDL);
			}
		}

		public override string ToString()
		{
			return string.Format("Store contact{0}: 0x{1:X} / 0x{2:X}", (this.IsDL == true) ? " (DL)" : string.Empty, this.ltEntryId.FolderId.GlobCntLong, this.ltEntryId.MessageId.GlobCntLong);
		}

		internal static StoreParticipantEntryId TryFromParticipant(Participant participant, ParticipantEntryIdConsumer consumer)
		{
			if ((consumer & ParticipantEntryIdConsumer.SupportsStoreParticipantEntryId) != ParticipantEntryIdConsumer.SupportsNone || (participant.RoutingType == "MAPIPDL" && (consumer & ParticipantEntryIdConsumer.SupportsStoreParticipantEntryIdForPDLs) != ParticipantEntryIdConsumer.SupportsNone))
			{
				StoreParticipantOrigin storeParticipantOrigin = participant.Origin as StoreParticipantOrigin;
				if (storeParticipantOrigin != null)
				{
					return new StoreParticipantEntryId(storeParticipantOrigin.OriginItemId, participant.RoutingType == "MAPIPDL", storeParticipantOrigin.EmailAddressIndex, (consumer & ParticipantEntryIdConsumer.SupportsWindowsAddressBookEnvelope) != ParticipantEntryIdConsumer.SupportsNone);
				}
				if (participant.RoutingType == "MAPIPDL")
				{
					ExTraceGlobals.StorageTracer.TraceDebug<Participant>((long)participant.GetHashCode(), "Cannot create an entry id: ContactDL should have StoreParticipantOrigin: {0}", participant);
				}
			}
			return null;
		}

		internal override IEnumerable<PropValue> GetParticipantProperties()
		{
			return this.GetParticipantOrigin().GetProperties();
		}

		internal override ParticipantOrigin GetParticipantOrigin()
		{
			return new StoreParticipantOrigin(this.ToUniqueItemId(), this.emailAddressIndex);
		}

		internal StoreObjectId ToUniqueItemId()
		{
			StoreObjectId result;
			using (ParticipantEntryId.Writer writer = new ParticipantEntryId.Writer())
			{
				writer.Write(this.ltEntryId);
				result = StoreObjectId.FromProviderSpecificId(writer.GetBytes(), (this.IsDL == true) ? StoreObjectType.DistributionList : StoreObjectType.Contact);
			}
			return result;
		}

		protected override void Serialize(ParticipantEntryId.Writer writer)
		{
			if (this.useWabFormat)
			{
				writer.WriteEntryHeader(ParticipantEntryId.WabProviderGuid);
				writer.Write((byte)(StoreParticipantEntryId.ToWabEntryFlag(this.emailAddressIndex, this.isMapiPDL) | ParticipantEntryId.WabEntryFlag.Outlook));
				writer.Write(this.ltEntryId);
				return;
			}
			writer.WriteEntryHeader(ParticipantEntryId.OlabProviderGuid);
			writer.Write(3U);
			writer.Write(this.isMapiPDL ? 5U : 4U);
			writer.Write(StoreParticipantEntryId.EmailAddressIndexToIndex(this.emailAddressIndex));
			writer.Write(70);
			writer.Write(this.ltEntryId);
		}

		private static uint EmailAddressIndexToIndex(EmailAddressIndex emailAddressIndex)
		{
			switch (emailAddressIndex)
			{
			case EmailAddressIndex.None:
				return 255U;
			case EmailAddressIndex.Email1:
				return 0U;
			case EmailAddressIndex.Email2:
				return 1U;
			case EmailAddressIndex.Email3:
				return 2U;
			case EmailAddressIndex.BusinessFax:
				return 3U;
			case EmailAddressIndex.HomeFax:
				return 4U;
			case EmailAddressIndex.OtherFax:
				return 5U;
			default:
				throw new ArgumentException();
			}
		}

		private static ParticipantEntryId.WabEntryFlag ToWabEntryFlag(EmailAddressIndex emailAddressIndex, bool isMapiPDL)
		{
			switch (emailAddressIndex)
			{
			case EmailAddressIndex.None:
				return (ParticipantEntryId.WabEntryFlag)((isMapiPDL ? 4 : 3) | 48);
			case EmailAddressIndex.Email1:
				return (ParticipantEntryId.WabEntryFlag)67;
			case EmailAddressIndex.Email2:
				return (ParticipantEntryId.WabEntryFlag)83;
			case EmailAddressIndex.Email3:
				return (ParticipantEntryId.WabEntryFlag)99;
			case EmailAddressIndex.BusinessFax:
				return ParticipantEntryId.WabEntryFlag.ContactPerson;
			case EmailAddressIndex.HomeFax:
				return (ParticipantEntryId.WabEntryFlag)19;
			case EmailAddressIndex.OtherFax:
				return (ParticipantEntryId.WabEntryFlag)35;
			default:
				throw new ArgumentException();
			}
		}

		private static EmailAddressIndex IndexToEmailAddressIndex(uint index)
		{
			switch (index)
			{
			case 0U:
				return EmailAddressIndex.Email1;
			case 1U:
				return EmailAddressIndex.Email2;
			case 2U:
				return EmailAddressIndex.Email3;
			case 3U:
				return EmailAddressIndex.BusinessFax;
			case 4U:
				return EmailAddressIndex.HomeFax;
			case 5U:
				return EmailAddressIndex.OtherFax;
			default:
				if (index != 255U)
				{
					throw new CorruptDataException(ServerStrings.ExInvalidParticipantEntryId);
				}
				return EmailAddressIndex.None;
			}
		}

		private static EmailAddressIndex WabEntryFlagToEmailAddressIndex(ParticipantEntryId.WabEntryFlag wabEntryFlag)
		{
			ParticipantEntryId.WabEntryFlag wabEntryFlag2 = wabEntryFlag & ParticipantEntryId.WabEntryFlag.EmailIndexMask;
			if (wabEntryFlag2 <= ParticipantEntryId.WabEntryFlag.OtherFax)
			{
				if (wabEntryFlag2 == ParticipantEntryId.WabEntryFlag.Envelope)
				{
					return EmailAddressIndex.BusinessFax;
				}
				if (wabEntryFlag2 == ParticipantEntryId.WabEntryFlag.HomeFax)
				{
					return EmailAddressIndex.HomeFax;
				}
				if (wabEntryFlag2 == ParticipantEntryId.WabEntryFlag.OtherFax)
				{
					return EmailAddressIndex.OtherFax;
				}
			}
			else if (wabEntryFlag2 <= ParticipantEntryId.WabEntryFlag.EmailIndex1)
			{
				if (wabEntryFlag2 == ParticipantEntryId.WabEntryFlag.NoEmailIndex)
				{
					return EmailAddressIndex.None;
				}
				if (wabEntryFlag2 == ParticipantEntryId.WabEntryFlag.EmailIndex1)
				{
					return EmailAddressIndex.Email1;
				}
			}
			else
			{
				if (wabEntryFlag2 == ParticipantEntryId.WabEntryFlag.EmailIndex2)
				{
					return EmailAddressIndex.Email2;
				}
				if (wabEntryFlag2 == ParticipantEntryId.WabEntryFlag.EmailIndex3)
				{
					return EmailAddressIndex.Email3;
				}
			}
			throw new CorruptDataException(ServerStrings.ExInvalidParticipantEntryId);
		}

		private static bool WabEntryFlagToMapiPDL(ParticipantEntryId.WabEntryFlag wabEntryFlag)
		{
			switch ((byte)(wabEntryFlag & ParticipantEntryId.WabEntryFlag.ObjectTypeMask))
			{
			case 3:
				return false;
			case 4:
				return true;
			default:
				throw new CorruptDataException(ServerStrings.ExInvalidParticipantEntryId);
			}
		}

		private const uint TypeDL = 5U;

		private const uint TypeContact = 4U;

		private const uint OlabpVersion = 3U;

		private readonly EmailAddressIndex emailAddressIndex;

		private readonly ParticipantEntryId.LTEntryId ltEntryId;

		private readonly bool useWabFormat;

		private readonly bool isMapiPDL;
	}
}

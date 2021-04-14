using System;

namespace Microsoft.Exchange.Server.Storage.PropTags
{
	public static class AddressInfoTags
	{
		public static readonly StorePropTag[] SentRepresenting = new StorePropTag[]
		{
			PropTag.Message.SentRepresentingEntryId,
			PropTag.Message.SentRepresentingSearchKey,
			PropTag.Message.SentRepresentingAddressType,
			PropTag.Message.SentRepresentingEmailAddress,
			PropTag.Message.SentRepresentingName,
			PropTag.Message.SentRepresentingSimpleDisplayName,
			PropTag.Message.SentRepresentingFlags,
			PropTag.Message.SentRepresentingOrgAddressType,
			PropTag.Message.SentRepresentingOrgEmailAddr,
			PropTag.Message.SentRepresentingSID,
			PropTag.Message.SentRepresentingGuid
		};

		public static readonly StorePropTag[] Sender = new StorePropTag[]
		{
			PropTag.Message.SenderEntryId,
			PropTag.Message.SenderSearchKey,
			PropTag.Message.SenderAddressType,
			PropTag.Message.SenderEmailAddress,
			PropTag.Message.SenderName,
			PropTag.Message.SenderSimpleDisplayName,
			PropTag.Message.SenderFlags,
			PropTag.Message.SenderOrgAddressType,
			PropTag.Message.SenderOrgEmailAddr,
			PropTag.Message.SenderSID,
			PropTag.Message.SenderGuid
		};

		public static readonly StorePropTag[] OriginalSentRepresenting = new StorePropTag[]
		{
			PropTag.Message.OriginalSentRepresentingEntryId,
			PropTag.Message.OriginalSentRepresentingSearchKey,
			PropTag.Message.OriginalSentRepresentingAddressType,
			PropTag.Message.OriginalSentRepresentingEmailAddress,
			PropTag.Message.OriginalSentRepresentingName,
			PropTag.Message.OriginalSentRepresentingSimpleDisplayName,
			PropTag.Message.OriginalSentRepresentingFlags,
			PropTag.Message.OriginalSentRepresentingOrgAddressType,
			PropTag.Message.OriginalSentRepresentingOrgEmailAddr,
			PropTag.Message.OriginalSentRepresentingSid,
			PropTag.Message.OriginalSentRepresentingGuid
		};

		public static readonly StorePropTag[] OriginalSender = new StorePropTag[]
		{
			PropTag.Message.OriginalSenderEntryId,
			PropTag.Message.OriginalSenderSearchKey,
			PropTag.Message.OriginalSenderAddressType,
			PropTag.Message.OriginalSenderEmailAddress,
			PropTag.Message.OriginalSenderName,
			PropTag.Message.OriginalSenderSimpleDisplayName,
			PropTag.Message.OriginalSenderFlags,
			PropTag.Message.OriginalSenderOrgAddressType,
			PropTag.Message.OriginalSenderOrgEmailAddr,
			PropTag.Message.OriginalSenderSid,
			PropTag.Message.OriginalSenderGuid
		};

		public static readonly StorePropTag[] ReceivedRepresenting = new StorePropTag[]
		{
			PropTag.Message.ReceivedRepresentingEntryId,
			PropTag.Message.ReceivedRepresentingSearchKey,
			PropTag.Message.ReceivedRepresentingAddressType,
			PropTag.Message.ReceivedRepresentingEmailAddress,
			PropTag.Message.ReceivedRepresentingName,
			PropTag.Message.ReceivedRepresentingSimpleDisplayName,
			PropTag.Message.RcvdRepresentingFlags,
			PropTag.Message.RcvdRepresentingOrgAddressType,
			PropTag.Message.RcvdRepresentingOrgEmailAddr,
			PropTag.Message.RcvdRepresentingSid,
			PropTag.Message.ReceivedRepresentingGuid
		};

		public static readonly StorePropTag[] ReceivedBy = new StorePropTag[]
		{
			PropTag.Message.ReceivedByEntryId,
			PropTag.Message.ReceivedBySearchKey,
			PropTag.Message.ReceivedByAddressType,
			PropTag.Message.ReceivedByEmailAddress,
			PropTag.Message.ReceivedByName,
			PropTag.Message.ReceivedBySimpleDisplayName,
			PropTag.Message.RcvdByFlags,
			PropTag.Message.RcvdByOrgAddressType,
			PropTag.Message.RcvdByOrgEmailAddr,
			PropTag.Message.RcvdBySid,
			PropTag.Message.ReceivedByGuid
		};

		public static readonly StorePropTag[] Creator = new StorePropTag[]
		{
			PropTag.Message.CreatorEntryId,
			StorePropTag.Invalid,
			PropTag.Message.CreatorAddressType,
			PropTag.Message.CreatorEmailAddr,
			PropTag.Message.CreatorName,
			PropTag.Message.CreatorSimpleDisplayName,
			PropTag.Message.CreatorFlags,
			PropTag.Message.CreatorOrgAddressType,
			PropTag.Message.CreatorOrgEmailAddr,
			PropTag.Message.CreatorSID,
			PropTag.Message.CreatorGuid
		};

		public static readonly StorePropTag[] LastModifier = new StorePropTag[]
		{
			PropTag.Message.LastModifierEntryId,
			StorePropTag.Invalid,
			PropTag.Message.LastModifierAddressType,
			PropTag.Message.LastModifierEmailAddr,
			PropTag.Message.LastModifierName,
			PropTag.Message.LastModifierSimpleDisplayName,
			PropTag.Message.LastModifierFlags,
			PropTag.Message.LastModifierOrgAddressType,
			PropTag.Message.LastModifierOrgEmailAddr,
			PropTag.Message.LastModifierSid,
			PropTag.Message.LastModifierGuid
		};

		public static readonly StorePropTag[] ReadReceipt = new StorePropTag[]
		{
			PropTag.Message.ReadReceiptEntryId,
			PropTag.Message.ReadReceiptSearchKey,
			PropTag.Message.ReadReceiptAddressType,
			PropTag.Message.ReadReceiptEmailAddress,
			PropTag.Message.ReadReceiptDisplayName,
			PropTag.Message.ReadReceiptSimpleDisplayName,
			PropTag.Message.ReadReceiptFlags,
			PropTag.Message.ReadReceiptOrgAddressType,
			PropTag.Message.ReadReceiptOrgEmailAddr,
			PropTag.Message.ReadReceiptSid,
			PropTag.Message.ReadReceiptGuid
		};

		public static readonly StorePropTag[] Report = new StorePropTag[]
		{
			PropTag.Message.ReportEntryId,
			PropTag.Message.ReportSearchKey,
			PropTag.Message.ReportAddressType,
			PropTag.Message.ReportEmailAddress,
			PropTag.Message.ReportDisplayName,
			PropTag.Message.ReportSimpleDisplayName,
			PropTag.Message.ReportFlags,
			PropTag.Message.ReportOrgAddressType,
			PropTag.Message.ReportOrgEmailAddr,
			PropTag.Message.ReportSid,
			PropTag.Message.ReportGuid
		};

		public static readonly StorePropTag[] Originator = new StorePropTag[]
		{
			PropTag.Message.OriginatorEntryId,
			PropTag.Message.OriginatorSearchKey,
			PropTag.Message.OriginatorAddressType,
			PropTag.Message.OriginatorEmailAddress,
			PropTag.Message.OriginatorName,
			PropTag.Message.OriginatorSimpleDisplayName,
			PropTag.Message.OriginatorFlags,
			PropTag.Message.OriginatorOrgAddressType,
			PropTag.Message.OriginatorOrgEmailAddr,
			PropTag.Message.OriginatorSid,
			PropTag.Message.OriginatorGuid
		};

		public static readonly StorePropTag[] OriginalAuthor = new StorePropTag[]
		{
			PropTag.Message.OriginalAuthorEntryId,
			PropTag.Message.OriginalAuthorSearchKey,
			PropTag.Message.OriginalAuthorAddressType,
			PropTag.Message.OriginalAuthorEmailAddress,
			PropTag.Message.OriginalAuthorName,
			PropTag.Message.OriginalAuthorSimpleDispName,
			PropTag.Message.OriginalAuthorFlags,
			PropTag.Message.OriginalAuthorOrgAddressType,
			PropTag.Message.OriginalAuthorOrgEmailAddr,
			PropTag.Message.OriginalAuthorSid,
			PropTag.Message.OriginalAuthorGuid
		};

		public static readonly StorePropTag[] ReportDestination = new StorePropTag[]
		{
			PropTag.Message.ReportDestinationEntryId,
			PropTag.Message.ReportDestinationSearchKey,
			PropTag.Message.ReportDestinationAddressType,
			PropTag.Message.ReportDestinationEmailAddress,
			PropTag.Message.ReportDestinationName,
			PropTag.Message.ReportDestinationSimpleDisplayName,
			PropTag.Message.ReportDestinationFlags,
			PropTag.Message.ReportDestinationOrgEmailType,
			PropTag.Message.ReportDestinationOrgEmailAddr,
			PropTag.Message.ReportDestinationSid,
			PropTag.Message.ReportDestinationGuid
		};

		public static readonly StorePropTag[][] AddressInfoTagList = new StorePropTag[][]
		{
			AddressInfoTags.SentRepresenting,
			AddressInfoTags.Sender,
			AddressInfoTags.OriginalSentRepresenting,
			AddressInfoTags.OriginalSender,
			AddressInfoTags.ReceivedRepresenting,
			AddressInfoTags.ReceivedBy,
			AddressInfoTags.Creator,
			AddressInfoTags.LastModifier,
			AddressInfoTags.ReadReceipt,
			AddressInfoTags.Report,
			AddressInfoTags.Originator,
			AddressInfoTags.OriginalAuthor,
			AddressInfoTags.ReportDestination
		};

		public enum AddressInfoElementIndex
		{
			EntryId,
			SearchKey,
			AddressType,
			EmailAddress,
			DisplayName,
			SimpleDisplayName,
			Flags,
			OriginalAddressType,
			OriginalEmailAddress,
			Sid,
			Guid
		}

		public enum AddressInfoType
		{
			SentRepresenting,
			Sender,
			OriginalSentRepresenting,
			OriginalSender,
			ReceivedRepresenting,
			ReceivedBy,
			Creator,
			LastModifier,
			ReadReceipt,
			Report,
			Originator,
			OriginalAuthor,
			ReportDestination
		}

		public enum AlternateAddressInfoType
		{
			SentRepresenting,
			Sender = 0,
			OriginalSentRepresenting = 2,
			OriginalSender = 2,
			ReceivedRepresenting = 4,
			ReceivedBy = 4,
			Creator = 6,
			LastModifier = 6,
			ReadReceipt = 8,
			Report,
			Originator = 7,
			OriginalAuthor = 7,
			ReportDestination = 12
		}
	}
}

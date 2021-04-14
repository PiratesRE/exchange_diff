using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.CtsResources
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(3398506793U, "HeaderReaderCannotBeUsedInThisState");
			Strings.stringIDs.Add(3277623286U, "CannotLoadIntoNonEmptyDocument");
			Strings.stringIDs.Add(1258373174U, "NewChildNotMimeParameter");
			Strings.stringIDs.Add(327877968U, "RefChildIsNotMyChild");
			Strings.stringIDs.Add(134484231U, "CantSetValueOfRfc2231ContinuationSegment");
			Strings.stringIDs.Add(1744024036U, "ChildrenCannotBeAddedToReceivedHeader");
			Strings.stringIDs.Add(3057104001U, "ParameterReaderNotInitialized");
			Strings.stringIDs.Add(238701657U, "CannotWriteHeaderValueMoreThanOnce");
			Strings.stringIDs.Add(3480118652U, "OldChildIsNotMyChild");
			Strings.stringIDs.Add(1858092871U, "CannotWriteEmptyOrNullBoundary");
			Strings.stringIDs.Add(64440155U, "CannotReadContentWhileStreamIsActive");
			Strings.stringIDs.Add(973871481U, "BinHexNotSupportedForThisMethod");
			Strings.stringIDs.Add(3780024871U, "RefHeaderIsNotMyChild");
			Strings.stringIDs.Add(1065856182U, "UnicodeMimeHeaderReceivedNotSupported");
			Strings.stringIDs.Add(3506183213U, "CachingModeSourceButStreamCannotSeek");
			Strings.stringIDs.Add(1909781391U, "LoadingStopped");
			Strings.stringIDs.Add(1572349307U, "CantGetValueOfRfc2231ContinuationSegment");
			Strings.stringIDs.Add(2512231815U, "HeaderReaderNotInitialized");
			Strings.stringIDs.Add(3307514334U, "MimeHandlerErrorNotEmbeddedMessage");
			Strings.stringIDs.Add(2491124350U, "CannotWriteRecipientsHere");
			Strings.stringIDs.Add(1665262320U, "ChildrenCannotBeAddedToTextHeader");
			Strings.stringIDs.Add(2556127565U, "ModifyingRawContentOfMultipartNotSupported");
			Strings.stringIDs.Add(3341745715U, "NewChildIsNotARecipient");
			Strings.stringIDs.Add(265752974U, "CantCopyToDifferentObjectType");
			Strings.stringIDs.Add(4072557241U, "NonMultiPartPartsCannotHaveChildren");
			Strings.stringIDs.Add(1119528296U, "CannotMixReadRawContentAndReadContent");
			Strings.stringIDs.Add(3324675010U, "AddingChildrenToAsciiTextHeader");
			Strings.stringIDs.Add(3693127890U, "NewChildNotRecipientOrGroup");
			Strings.stringIDs.Add(2413759299U, "StrictComplianceViolation");
			Strings.stringIDs.Add(2605804358U, "RootPartCantHaveAParent");
			Strings.stringIDs.Add(325536703U, "InternalMimeError");
			Strings.stringIDs.Add(2332969818U, "UnrecognizedTransferEncodingUsed");
			Strings.stringIDs.Add(520133541U, "OperationNotValidInThisReaderState");
			Strings.stringIDs.Add(110823742U, "NewChildNotMimeHeader");
			Strings.stringIDs.Add(2924274099U, "CannotWriteUnicodeHeaderValue");
			Strings.stringIDs.Add(2752555714U, "MimeHandlerErrorMoreThanOneOuterContentPushStream");
			Strings.stringIDs.Add(21266136U, "CannotDecodeContentStream");
			Strings.stringIDs.Add(1044739797U, "InvalidBoundary");
			Strings.stringIDs.Add(2360968430U, "StreamMustSupportRead");
			Strings.stringIDs.Add(1018032548U, "CurrentPartIsNotEmbeddedMessage");
			Strings.stringIDs.Add(2197652894U, "CannotWriteGroupEndHere");
			Strings.stringIDs.Add(1904569722U, "MimeVersionRequiredForMultipart");
			Strings.stringIDs.Add(4119843803U, "ErrorBeforeFirst");
			Strings.stringIDs.Add(3885506491U, "CantSetRawValueOfRfc2231ContinuationSegment");
			Strings.stringIDs.Add(3099580008U, "StreamMustAllowRead");
			Strings.stringIDs.Add(1597481536U, "ReaderIsNotPositionedOnHeaderWithParameters");
			Strings.stringIDs.Add(2028330664U, "NewChildCannotHaveDifferentParent");
			Strings.stringIDs.Add(1693234793U, "CannotWriteGroupStartHere");
			Strings.stringIDs.Add(4251577240U, "CannotWriteHeadersHere");
			Strings.stringIDs.Add(1786178430U, "HeaderCannotHaveParameters");
			Strings.stringIDs.Add(2213004504U, "ParametersCannotHaveChildNodes");
			Strings.stringIDs.Add(746087816U, "HeaderReaderIsNotPositionedOnAHeader");
			Strings.stringIDs.Add(929328884U, "CurrentAddressIsGroupAndCannotHaveEmail");
			Strings.stringIDs.Add(631385300U, "AddressParserNotInitialized");
			Strings.stringIDs.Add(3697700598U, "PartContentIsBeingWritten");
			Strings.stringIDs.Add(479601644U, "MultipartCannotContainContent");
			Strings.stringIDs.Add(1300692199U, "NewChildIsNotAPart");
			Strings.stringIDs.Add(3782248328U, "RecipientsCannotHaveChildNodes");
			Strings.stringIDs.Add(1601383848U, "ContentAlreadyWritten");
			Strings.stringIDs.Add(1687756747U, "CannotAddChildrenToMimeHeaderDate");
			Strings.stringIDs.Add(161660084U, "CannotGetLoadStreamMoreThanOnce");
			Strings.stringIDs.Add(1406718177U, "ValueTooLong");
			Strings.stringIDs.Add(557180799U, "CannotDetermineHeaderNameFromId");
			Strings.stringIDs.Add(340926097U, "DocumentCloneNotSupportedInThisState");
			Strings.stringIDs.Add(1844115139U, "InvalidHeaderId");
			Strings.stringIDs.Add(1224345327U, "AddressReaderIsNotPositionedOnAddress");
			Strings.stringIDs.Add(3344340751U, "ParameterReaderIsNotPositionedOnParameter");
			Strings.stringIDs.Add(3424442414U, "ErrorAfterLast");
			Strings.stringIDs.Add(1536364454U, "CannotWriteParametersHere");
			Strings.stringIDs.Add(526908964U, "CannotWriteAfterFlush");
			Strings.stringIDs.Add(2286678017U, "UnicodeMimeHeaderAddressNotSupported");
			Strings.stringIDs.Add(42821332U, "CannotWriteParametersOnThisHeader");
			Strings.stringIDs.Add(1074868404U, "CannotStartPartHere");
			Strings.stringIDs.Add(239464507U, "CannotEndPartHere");
			Strings.stringIDs.Add(2034730061U, "ThisPartBelongsToSubtreeOfNewChild");
			Strings.stringIDs.Add(4140427462U, "StreamMustSupportWriting");
			Strings.stringIDs.Add(1264059714U, "HeaderCannotHaveAddresses");
			Strings.stringIDs.Add(1817020120U, "OnlyOneOuterContentPushStreamAllowed");
			Strings.stringIDs.Add(3644237080U, "AddressReaderNotInitialized");
			Strings.stringIDs.Add(3716863546U, "ReaderIsNotPositionedOnAddressHeader");
			Strings.stringIDs.Add(2657804913U, "EmbeddedMessageReaderNeedsToBeClosedFirst");
			Strings.stringIDs.Add(1462963282U, "CannotWritePartContentNow");
			Strings.stringIDs.Add(2748764102U, "StreamNoLongerValid");
			Strings.stringIDs.Add(892057174U, "BareLinefeedRejected");
			Strings.stringIDs.Add(3956216251U, "AddressReaderIsNotPositionedOnAGroup");
			Strings.stringIDs.Add(1674677912U, "CannotWriteHeaderValueHere");
		}

		public static string HeaderReaderCannotBeUsedInThisState
		{
			get
			{
				return Strings.ResourceManager.GetString("HeaderReaderCannotBeUsedInThisState");
			}
		}

		public static string CannotLoadIntoNonEmptyDocument
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotLoadIntoNonEmptyDocument");
			}
		}

		public static string NewChildNotMimeParameter
		{
			get
			{
				return Strings.ResourceManager.GetString("NewChildNotMimeParameter");
			}
		}

		public static string RefChildIsNotMyChild
		{
			get
			{
				return Strings.ResourceManager.GetString("RefChildIsNotMyChild");
			}
		}

		public static string CantSetValueOfRfc2231ContinuationSegment
		{
			get
			{
				return Strings.ResourceManager.GetString("CantSetValueOfRfc2231ContinuationSegment");
			}
		}

		public static string ChildrenCannotBeAddedToReceivedHeader
		{
			get
			{
				return Strings.ResourceManager.GetString("ChildrenCannotBeAddedToReceivedHeader");
			}
		}

		public static string ParameterReaderNotInitialized
		{
			get
			{
				return Strings.ResourceManager.GetString("ParameterReaderNotInitialized");
			}
		}

		public static string CannotWriteHeaderValueMoreThanOnce
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotWriteHeaderValueMoreThanOnce");
			}
		}

		public static string OldChildIsNotMyChild
		{
			get
			{
				return Strings.ResourceManager.GetString("OldChildIsNotMyChild");
			}
		}

		public static string CannotWriteEmptyOrNullBoundary
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotWriteEmptyOrNullBoundary");
			}
		}

		public static string CannotReadContentWhileStreamIsActive
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotReadContentWhileStreamIsActive");
			}
		}

		public static string BinHexNotSupportedForThisMethod
		{
			get
			{
				return Strings.ResourceManager.GetString("BinHexNotSupportedForThisMethod");
			}
		}

		public static string RefHeaderIsNotMyChild
		{
			get
			{
				return Strings.ResourceManager.GetString("RefHeaderIsNotMyChild");
			}
		}

		public static string UnicodeMimeHeaderReceivedNotSupported
		{
			get
			{
				return Strings.ResourceManager.GetString("UnicodeMimeHeaderReceivedNotSupported");
			}
		}

		public static string CachingModeSourceButStreamCannotSeek
		{
			get
			{
				return Strings.ResourceManager.GetString("CachingModeSourceButStreamCannotSeek");
			}
		}

		public static string LoadingStopped
		{
			get
			{
				return Strings.ResourceManager.GetString("LoadingStopped");
			}
		}

		public static string CantGetValueOfRfc2231ContinuationSegment
		{
			get
			{
				return Strings.ResourceManager.GetString("CantGetValueOfRfc2231ContinuationSegment");
			}
		}

		public static string HeaderReaderNotInitialized
		{
			get
			{
				return Strings.ResourceManager.GetString("HeaderReaderNotInitialized");
			}
		}

		public static string MimeHandlerErrorNotEmbeddedMessage
		{
			get
			{
				return Strings.ResourceManager.GetString("MimeHandlerErrorNotEmbeddedMessage");
			}
		}

		public static string CannotWriteRecipientsHere
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotWriteRecipientsHere");
			}
		}

		public static string ChildrenCannotBeAddedToTextHeader
		{
			get
			{
				return Strings.ResourceManager.GetString("ChildrenCannotBeAddedToTextHeader");
			}
		}

		public static string ModifyingRawContentOfMultipartNotSupported
		{
			get
			{
				return Strings.ResourceManager.GetString("ModifyingRawContentOfMultipartNotSupported");
			}
		}

		public static string NewChildIsNotARecipient
		{
			get
			{
				return Strings.ResourceManager.GetString("NewChildIsNotARecipient");
			}
		}

		public static string CantCopyToDifferentObjectType
		{
			get
			{
				return Strings.ResourceManager.GetString("CantCopyToDifferentObjectType");
			}
		}

		public static string NonMultiPartPartsCannotHaveChildren
		{
			get
			{
				return Strings.ResourceManager.GetString("NonMultiPartPartsCannotHaveChildren");
			}
		}

		public static string CannotMixReadRawContentAndReadContent
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotMixReadRawContentAndReadContent");
			}
		}

		public static string AddingChildrenToAsciiTextHeader
		{
			get
			{
				return Strings.ResourceManager.GetString("AddingChildrenToAsciiTextHeader");
			}
		}

		public static string NewChildNotRecipientOrGroup
		{
			get
			{
				return Strings.ResourceManager.GetString("NewChildNotRecipientOrGroup");
			}
		}

		public static string StrictComplianceViolation
		{
			get
			{
				return Strings.ResourceManager.GetString("StrictComplianceViolation");
			}
		}

		public static string RootPartCantHaveAParent
		{
			get
			{
				return Strings.ResourceManager.GetString("RootPartCantHaveAParent");
			}
		}

		public static string InternalMimeError
		{
			get
			{
				return Strings.ResourceManager.GetString("InternalMimeError");
			}
		}

		public static string PartNestingTooDeep(int actual, int limit)
		{
			return string.Format(Strings.ResourceManager.GetString("PartNestingTooDeep"), actual, limit);
		}

		public static string UnrecognizedTransferEncodingUsed
		{
			get
			{
				return Strings.ResourceManager.GetString("UnrecognizedTransferEncodingUsed");
			}
		}

		public static string OperationNotValidInThisReaderState
		{
			get
			{
				return Strings.ResourceManager.GetString("OperationNotValidInThisReaderState");
			}
		}

		public static string TooManyHeaderBytes(int actual, int limit)
		{
			return string.Format(Strings.ResourceManager.GetString("TooManyHeaderBytes"), actual, limit);
		}

		public static string NewChildNotMimeHeader
		{
			get
			{
				return Strings.ResourceManager.GetString("NewChildNotMimeHeader");
			}
		}

		public static string InvalidHeaderName(string name, int position)
		{
			return string.Format(Strings.ResourceManager.GetString("InvalidHeaderName"), name, position);
		}

		public static string CannotWriteUnicodeHeaderValue
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotWriteUnicodeHeaderValue");
			}
		}

		public static string MimeHandlerErrorMoreThanOneOuterContentPushStream
		{
			get
			{
				return Strings.ResourceManager.GetString("MimeHandlerErrorMoreThanOneOuterContentPushStream");
			}
		}

		public static string CannotDecodeContentStream
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotDecodeContentStream");
			}
		}

		public static string InvalidBoundary
		{
			get
			{
				return Strings.ResourceManager.GetString("InvalidBoundary");
			}
		}

		public static string InputStreamTooLong(int actual, int limit)
		{
			return string.Format(Strings.ResourceManager.GetString("InputStreamTooLong"), actual, limit);
		}

		public static string StreamMustSupportRead
		{
			get
			{
				return Strings.ResourceManager.GetString("StreamMustSupportRead");
			}
		}

		public static string CurrentPartIsNotEmbeddedMessage
		{
			get
			{
				return Strings.ResourceManager.GetString("CurrentPartIsNotEmbeddedMessage");
			}
		}

		public static string CannotWriteGroupEndHere
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotWriteGroupEndHere");
			}
		}

		public static string MimeVersionRequiredForMultipart
		{
			get
			{
				return Strings.ResourceManager.GetString("MimeVersionRequiredForMultipart");
			}
		}

		public static string ErrorBeforeFirst
		{
			get
			{
				return Strings.ResourceManager.GetString("ErrorBeforeFirst");
			}
		}

		public static string CantSetRawValueOfRfc2231ContinuationSegment
		{
			get
			{
				return Strings.ResourceManager.GetString("CantSetRawValueOfRfc2231ContinuationSegment");
			}
		}

		public static string StreamMustAllowRead
		{
			get
			{
				return Strings.ResourceManager.GetString("StreamMustAllowRead");
			}
		}

		public static string LengthExceeded(int sum, int length)
		{
			return string.Format(Strings.ResourceManager.GetString("LengthExceeded"), sum, length);
		}

		public static string ReaderIsNotPositionedOnHeaderWithParameters
		{
			get
			{
				return Strings.ResourceManager.GetString("ReaderIsNotPositionedOnHeaderWithParameters");
			}
		}

		public static string NewChildCannotHaveDifferentParent
		{
			get
			{
				return Strings.ResourceManager.GetString("NewChildCannotHaveDifferentParent");
			}
		}

		public static string CannotWriteGroupStartHere
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotWriteGroupStartHere");
			}
		}

		public static string TooManyParameters(int actual, int limit)
		{
			return string.Format(Strings.ResourceManager.GetString("TooManyParameters"), actual, limit);
		}

		public static string TooManyParts(int actual, int limit)
		{
			return string.Format(Strings.ResourceManager.GetString("TooManyParts"), actual, limit);
		}

		public static string CannotWriteHeadersHere
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotWriteHeadersHere");
			}
		}

		public static string HeaderCannotHaveParameters
		{
			get
			{
				return Strings.ResourceManager.GetString("HeaderCannotHaveParameters");
			}
		}

		public static string ParametersCannotHaveChildNodes
		{
			get
			{
				return Strings.ResourceManager.GetString("ParametersCannotHaveChildNodes");
			}
		}

		public static string HeaderReaderIsNotPositionedOnAHeader
		{
			get
			{
				return Strings.ResourceManager.GetString("HeaderReaderIsNotPositionedOnAHeader");
			}
		}

		public static string CurrentAddressIsGroupAndCannotHaveEmail
		{
			get
			{
				return Strings.ResourceManager.GetString("CurrentAddressIsGroupAndCannotHaveEmail");
			}
		}

		public static string AddressParserNotInitialized
		{
			get
			{
				return Strings.ResourceManager.GetString("AddressParserNotInitialized");
			}
		}

		public static string PartContentIsBeingWritten
		{
			get
			{
				return Strings.ResourceManager.GetString("PartContentIsBeingWritten");
			}
		}

		public static string TooManyAddressItems(int actual, int limit)
		{
			return string.Format(Strings.ResourceManager.GetString("TooManyAddressItems"), actual, limit);
		}

		public static string MultipartCannotContainContent
		{
			get
			{
				return Strings.ResourceManager.GetString("MultipartCannotContainContent");
			}
		}

		public static string NewChildIsNotAPart
		{
			get
			{
				return Strings.ResourceManager.GetString("NewChildIsNotAPart");
			}
		}

		public static string RecipientsCannotHaveChildNodes
		{
			get
			{
				return Strings.ResourceManager.GetString("RecipientsCannotHaveChildNodes");
			}
		}

		public static string ThisNodeDoesNotSupportCloning(string type)
		{
			return string.Format(Strings.ResourceManager.GetString("ThisNodeDoesNotSupportCloning"), type);
		}

		public static string ContentAlreadyWritten
		{
			get
			{
				return Strings.ResourceManager.GetString("ContentAlreadyWritten");
			}
		}

		public static string CannotAddChildrenToMimeHeaderDate
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotAddChildrenToMimeHeaderDate");
			}
		}

		public static string CannotGetLoadStreamMoreThanOnce
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotGetLoadStreamMoreThanOnce");
			}
		}

		public static string ValueTooLong
		{
			get
			{
				return Strings.ResourceManager.GetString("ValueTooLong");
			}
		}

		public static string CannotDetermineHeaderNameFromId
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotDetermineHeaderNameFromId");
			}
		}

		public static string DocumentCloneNotSupportedInThisState
		{
			get
			{
				return Strings.ResourceManager.GetString("DocumentCloneNotSupportedInThisState");
			}
		}

		public static string InvalidHeaderId
		{
			get
			{
				return Strings.ResourceManager.GetString("InvalidHeaderId");
			}
		}

		public static string AddressReaderIsNotPositionedOnAddress
		{
			get
			{
				return Strings.ResourceManager.GetString("AddressReaderIsNotPositionedOnAddress");
			}
		}

		public static string ParameterReaderIsNotPositionedOnParameter
		{
			get
			{
				return Strings.ResourceManager.GetString("ParameterReaderIsNotPositionedOnParameter");
			}
		}

		public static string ErrorAfterLast
		{
			get
			{
				return Strings.ResourceManager.GetString("ErrorAfterLast");
			}
		}

		public static string CannotWriteParametersHere
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotWriteParametersHere");
			}
		}

		public static string CannotWriteAfterFlush
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotWriteAfterFlush");
			}
		}

		public static string UnicodeMimeHeaderAddressNotSupported
		{
			get
			{
				return Strings.ResourceManager.GetString("UnicodeMimeHeaderAddressNotSupported");
			}
		}

		public static string CannotWriteParametersOnThisHeader
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotWriteParametersOnThisHeader");
			}
		}

		public static string CannotStartPartHere
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotStartPartHere");
			}
		}

		public static string CannotEndPartHere
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotEndPartHere");
			}
		}

		public static string TooManyTextValueBytes(int actual, int limit)
		{
			return string.Format(Strings.ResourceManager.GetString("TooManyTextValueBytes"), actual, limit);
		}

		public static string NameNotValidForThisHeaderType(string name, string typeName, string correctTypeName)
		{
			return string.Format(Strings.ResourceManager.GetString("NameNotValidForThisHeaderType"), name, typeName, correctTypeName);
		}

		public static string TooManyHeaders(int actual, int limit)
		{
			return string.Format(Strings.ResourceManager.GetString("TooManyHeaders"), actual, limit);
		}

		public static string ThisPartBelongsToSubtreeOfNewChild
		{
			get
			{
				return Strings.ResourceManager.GetString("ThisPartBelongsToSubtreeOfNewChild");
			}
		}

		public static string StreamMustSupportWriting
		{
			get
			{
				return Strings.ResourceManager.GetString("StreamMustSupportWriting");
			}
		}

		public static string HeaderCannotHaveAddresses
		{
			get
			{
				return Strings.ResourceManager.GetString("HeaderCannotHaveAddresses");
			}
		}

		public static string OnlyOneOuterContentPushStreamAllowed
		{
			get
			{
				return Strings.ResourceManager.GetString("OnlyOneOuterContentPushStreamAllowed");
			}
		}

		public static string AddressReaderNotInitialized
		{
			get
			{
				return Strings.ResourceManager.GetString("AddressReaderNotInitialized");
			}
		}

		public static string EmbeddedNestingTooDeep(int actual, int limit)
		{
			return string.Format(Strings.ResourceManager.GetString("EmbeddedNestingTooDeep"), actual, limit);
		}

		public static string ReaderIsNotPositionedOnAddressHeader
		{
			get
			{
				return Strings.ResourceManager.GetString("ReaderIsNotPositionedOnAddressHeader");
			}
		}

		public static string EmbeddedMessageReaderNeedsToBeClosedFirst
		{
			get
			{
				return Strings.ResourceManager.GetString("EmbeddedMessageReaderNeedsToBeClosedFirst");
			}
		}

		public static string CannotWritePartContentNow
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotWritePartContentNow");
			}
		}

		public static string StreamNoLongerValid
		{
			get
			{
				return Strings.ResourceManager.GetString("StreamNoLongerValid");
			}
		}

		public static string BareLinefeedRejected
		{
			get
			{
				return Strings.ResourceManager.GetString("BareLinefeedRejected");
			}
		}

		public static string AddressReaderIsNotPositionedOnAGroup
		{
			get
			{
				return Strings.ResourceManager.GetString("AddressReaderIsNotPositionedOnAGroup");
			}
		}

		public static string CannotWriteHeaderValueHere
		{
			get
			{
				return Strings.ResourceManager.GetString("CannotWriteHeaderValueHere");
			}
		}

		public static string GetLocalizedString(Strings.IDs key)
		{
			return Strings.ResourceManager.GetString(Strings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(86);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			HeaderReaderCannotBeUsedInThisState = 3398506793U,
			CannotLoadIntoNonEmptyDocument = 3277623286U,
			NewChildNotMimeParameter = 1258373174U,
			RefChildIsNotMyChild = 327877968U,
			CantSetValueOfRfc2231ContinuationSegment = 134484231U,
			ChildrenCannotBeAddedToReceivedHeader = 1744024036U,
			ParameterReaderNotInitialized = 3057104001U,
			CannotWriteHeaderValueMoreThanOnce = 238701657U,
			OldChildIsNotMyChild = 3480118652U,
			CannotWriteEmptyOrNullBoundary = 1858092871U,
			CannotReadContentWhileStreamIsActive = 64440155U,
			BinHexNotSupportedForThisMethod = 973871481U,
			RefHeaderIsNotMyChild = 3780024871U,
			UnicodeMimeHeaderReceivedNotSupported = 1065856182U,
			CachingModeSourceButStreamCannotSeek = 3506183213U,
			LoadingStopped = 1909781391U,
			CantGetValueOfRfc2231ContinuationSegment = 1572349307U,
			HeaderReaderNotInitialized = 2512231815U,
			MimeHandlerErrorNotEmbeddedMessage = 3307514334U,
			CannotWriteRecipientsHere = 2491124350U,
			ChildrenCannotBeAddedToTextHeader = 1665262320U,
			ModifyingRawContentOfMultipartNotSupported = 2556127565U,
			NewChildIsNotARecipient = 3341745715U,
			CantCopyToDifferentObjectType = 265752974U,
			NonMultiPartPartsCannotHaveChildren = 4072557241U,
			CannotMixReadRawContentAndReadContent = 1119528296U,
			AddingChildrenToAsciiTextHeader = 3324675010U,
			NewChildNotRecipientOrGroup = 3693127890U,
			StrictComplianceViolation = 2413759299U,
			RootPartCantHaveAParent = 2605804358U,
			InternalMimeError = 325536703U,
			UnrecognizedTransferEncodingUsed = 2332969818U,
			OperationNotValidInThisReaderState = 520133541U,
			NewChildNotMimeHeader = 110823742U,
			CannotWriteUnicodeHeaderValue = 2924274099U,
			MimeHandlerErrorMoreThanOneOuterContentPushStream = 2752555714U,
			CannotDecodeContentStream = 21266136U,
			InvalidBoundary = 1044739797U,
			StreamMustSupportRead = 2360968430U,
			CurrentPartIsNotEmbeddedMessage = 1018032548U,
			CannotWriteGroupEndHere = 2197652894U,
			MimeVersionRequiredForMultipart = 1904569722U,
			ErrorBeforeFirst = 4119843803U,
			CantSetRawValueOfRfc2231ContinuationSegment = 3885506491U,
			StreamMustAllowRead = 3099580008U,
			ReaderIsNotPositionedOnHeaderWithParameters = 1597481536U,
			NewChildCannotHaveDifferentParent = 2028330664U,
			CannotWriteGroupStartHere = 1693234793U,
			CannotWriteHeadersHere = 4251577240U,
			HeaderCannotHaveParameters = 1786178430U,
			ParametersCannotHaveChildNodes = 2213004504U,
			HeaderReaderIsNotPositionedOnAHeader = 746087816U,
			CurrentAddressIsGroupAndCannotHaveEmail = 929328884U,
			AddressParserNotInitialized = 631385300U,
			PartContentIsBeingWritten = 3697700598U,
			MultipartCannotContainContent = 479601644U,
			NewChildIsNotAPart = 1300692199U,
			RecipientsCannotHaveChildNodes = 3782248328U,
			ContentAlreadyWritten = 1601383848U,
			CannotAddChildrenToMimeHeaderDate = 1687756747U,
			CannotGetLoadStreamMoreThanOnce = 161660084U,
			ValueTooLong = 1406718177U,
			CannotDetermineHeaderNameFromId = 557180799U,
			DocumentCloneNotSupportedInThisState = 340926097U,
			InvalidHeaderId = 1844115139U,
			AddressReaderIsNotPositionedOnAddress = 1224345327U,
			ParameterReaderIsNotPositionedOnParameter = 3344340751U,
			ErrorAfterLast = 3424442414U,
			CannotWriteParametersHere = 1536364454U,
			CannotWriteAfterFlush = 526908964U,
			UnicodeMimeHeaderAddressNotSupported = 2286678017U,
			CannotWriteParametersOnThisHeader = 42821332U,
			CannotStartPartHere = 1074868404U,
			CannotEndPartHere = 239464507U,
			ThisPartBelongsToSubtreeOfNewChild = 2034730061U,
			StreamMustSupportWriting = 4140427462U,
			HeaderCannotHaveAddresses = 1264059714U,
			OnlyOneOuterContentPushStreamAllowed = 1817020120U,
			AddressReaderNotInitialized = 3644237080U,
			ReaderIsNotPositionedOnAddressHeader = 3716863546U,
			EmbeddedMessageReaderNeedsToBeClosedFirst = 2657804913U,
			CannotWritePartContentNow = 1462963282U,
			StreamNoLongerValid = 2748764102U,
			BareLinefeedRejected = 892057174U,
			AddressReaderIsNotPositionedOnAGroup = 3956216251U,
			CannotWriteHeaderValueHere = 1674677912U
		}

		private enum ParamIDs
		{
			PartNestingTooDeep,
			TooManyHeaderBytes,
			InvalidHeaderName,
			InputStreamTooLong,
			LengthExceeded,
			TooManyParameters,
			TooManyParts,
			TooManyAddressItems,
			ThisNodeDoesNotSupportCloning,
			TooManyTextValueBytes,
			NameNotValidForThisHeaderType,
			TooManyHeaders,
			EmbeddedNestingTooDeep
		}
	}
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.CtsResources
{
	internal static class EmailMessageStrings
	{
		static EmailMessageStrings()
		{
			EmailMessageStrings.stringIDs.Add(1282546634U, "ReferenceBodyDoesNotBelongToSameMessage");
			EmailMessageStrings.stringIDs.Add(4174367109U, "CannotAccessMapiPropsFromPureMimeMessage");
			EmailMessageStrings.stringIDs.Add(2479849522U, "ErrorInit");
			EmailMessageStrings.stringIDs.Add(1426955056U, "TooManyEntriesInApplefile");
			EmailMessageStrings.stringIDs.Add(392243280U, "MimeDocumentRootPartMustNotBeNull");
			EmailMessageStrings.stringIDs.Add(4142370976U, "WrongOffsetsInApplefile");
			EmailMessageStrings.stringIDs.Add(1857526624U, "CannotCreateAlternativeBody");
			EmailMessageStrings.stringIDs.Add(3435085534U, "CanOnlyAddInlineAttachmentsToHtmlBody");
			EmailMessageStrings.stringIDs.Add(4119843803U, "ErrorBeforeFirst");
			EmailMessageStrings.stringIDs.Add(3424442414U, "ErrorAfterLast");
			EmailMessageStrings.stringIDs.Add(2804270193U, "CannotAttachEmbeddedMapiMessageToMime");
			EmailMessageStrings.stringIDs.Add(764847365U, "BodyExistsInTnefMessage");
			EmailMessageStrings.stringIDs.Add(1685978706U, "NotSupportedForRtfBody");
			EmailMessageStrings.stringIDs.Add(368316767U, "CanOnlyAddInlineAttachmentForHtmlBody");
			EmailMessageStrings.stringIDs.Add(797792475U, "UnexpectedEndOfStream");
			EmailMessageStrings.stringIDs.Add(4122747589U, "CannotSetEmbeddedMessageForTnefAttachment");
			EmailMessageStrings.stringIDs.Add(4281132505U, "WrongAppleMagicNumber");
			EmailMessageStrings.stringIDs.Add(3934212261U, "ContentTypeCannotBeMultipart");
			EmailMessageStrings.stringIDs.Add(1586749494U, "CollectionHasChanged");
			EmailMessageStrings.stringIDs.Add(4139915868U, "AttachmentRemovedFromMessage");
			EmailMessageStrings.stringIDs.Add(2815148485U, "DigestCanOnlyContainMessage822Attachments");
			EmailMessageStrings.stringIDs.Add(2676324379U, "RecipientAlreadyHasParent");
			EmailMessageStrings.stringIDs.Add(1686645306U, "WrongAppleVersionNumber");
			EmailMessageStrings.stringIDs.Add(3682676966U, "MacBinWrongFilename");
			EmailMessageStrings.stringIDs.Add(532813113U, "CannotAddAttachment");
			EmailMessageStrings.stringIDs.Add(414550776U, "NoBodyForInlineAttachment");
			EmailMessageStrings.stringIDs.Add(1975150156U, "TnefIsMissingAttachRenderData");
			EmailMessageStrings.stringIDs.Add(2369512451U, "CannotSetEmbeddedMessageForNonMessageRfc822Attachment");
			EmailMessageStrings.stringIDs.Add(2544842942U, "WrongMacBinHeader");
			EmailMessageStrings.stringIDs.Add(3779231310U, "BodyAlreadyHasParent");
			EmailMessageStrings.stringIDs.Add(2792487344U, "CollectionIsReadOnly");
			EmailMessageStrings.stringIDs.Add(2839669472U, "ChangingDntNotSupportedForEmbeddedTnefMessages");
			EmailMessageStrings.stringIDs.Add(2005747751U, "CannotWriteBodyDoesNotExist");
			EmailMessageStrings.stringIDs.Add(665063789U, "CannotSetNativePropertyForMimeRecipient");
			EmailMessageStrings.stringIDs.Add(182340944U, "ArgumentInvalidOffLen");
			EmailMessageStrings.stringIDs.Add(2700714933U, "TnefContainsMultipleStreams");
		}

		public static string ReferenceBodyDoesNotBelongToSameMessage
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("ReferenceBodyDoesNotBelongToSameMessage");
			}
		}

		public static string CannotAccessMapiPropsFromPureMimeMessage
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("CannotAccessMapiPropsFromPureMimeMessage");
			}
		}

		public static string ErrorInit
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("ErrorInit");
			}
		}

		public static string UnsupportedBodyType(string value)
		{
			return string.Format(EmailMessageStrings.ResourceManager.GetString("UnsupportedBodyType"), value);
		}

		public static string TooManyEntriesInApplefile
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("TooManyEntriesInApplefile");
			}
		}

		public static string MimeDocumentRootPartMustNotBeNull
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("MimeDocumentRootPartMustNotBeNull");
			}
		}

		public static string WrongOffsetsInApplefile
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("WrongOffsetsInApplefile");
			}
		}

		public static string EntryLengthTooBigInApplefile(long length)
		{
			return string.Format(EmailMessageStrings.ResourceManager.GetString("EntryLengthTooBigInApplefile"), length);
		}

		public static string CannotCreateAlternativeBody
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("CannotCreateAlternativeBody");
			}
		}

		public static string CannotCreateSpecifiedBodyFormat(string format)
		{
			return string.Format(EmailMessageStrings.ResourceManager.GetString("CannotCreateSpecifiedBodyFormat"), format);
		}

		public static string CanOnlyAddInlineAttachmentsToHtmlBody
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("CanOnlyAddInlineAttachmentsToHtmlBody");
			}
		}

		public static string ErrorBeforeFirst
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("ErrorBeforeFirst");
			}
		}

		public static string ErrorAfterLast
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("ErrorAfterLast");
			}
		}

		public static string CannotAttachEmbeddedMapiMessageToMime
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("CannotAttachEmbeddedMapiMessageToMime");
			}
		}

		public static string BodyExistsInTnefMessage
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("BodyExistsInTnefMessage");
			}
		}

		public static string NotSupportedForRtfBody
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("NotSupportedForRtfBody");
			}
		}

		public static string CanOnlyAddInlineAttachmentForHtmlBody
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("CanOnlyAddInlineAttachmentForHtmlBody");
			}
		}

		public static string InvalidCharset(string charsetName)
		{
			return string.Format(EmailMessageStrings.ResourceManager.GetString("InvalidCharset"), charsetName);
		}

		public static string UnexpectedEndOfStream
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("UnexpectedEndOfStream");
			}
		}

		public static string CannotSetEmbeddedMessageForTnefAttachment
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("CannotSetEmbeddedMessageForTnefAttachment");
			}
		}

		public static string WrongAppleMagicNumber
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("WrongAppleMagicNumber");
			}
		}

		public static string ContentTypeCannotBeMultipart
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("ContentTypeCannotBeMultipart");
			}
		}

		public static string CollectionHasChanged
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("CollectionHasChanged");
			}
		}

		public static string AttachmentRemovedFromMessage
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("AttachmentRemovedFromMessage");
			}
		}

		public static string DigestCanOnlyContainMessage822Attachments
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("DigestCanOnlyContainMessage822Attachments");
			}
		}

		public static string RecipientAlreadyHasParent
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("RecipientAlreadyHasParent");
			}
		}

		public static string WrongAppleVersionNumber
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("WrongAppleVersionNumber");
			}
		}

		public static string NestingTooDeep(int actual, int limit)
		{
			return string.Format(EmailMessageStrings.ResourceManager.GetString("NestingTooDeep"), actual, limit);
		}

		public static string MacBinWrongFilename
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("MacBinWrongFilename");
			}
		}

		public static string CannotAddAttachment
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("CannotAddAttachment");
			}
		}

		public static string NoBodyForInlineAttachment
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("NoBodyForInlineAttachment");
			}
		}

		public static string TnefIsMissingAttachRenderData
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("TnefIsMissingAttachRenderData");
			}
		}

		public static string CannotSetEmbeddedMessageForNonMessageRfc822Attachment
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("CannotSetEmbeddedMessageForNonMessageRfc822Attachment");
			}
		}

		public static string WrongMacBinHeader
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("WrongMacBinHeader");
			}
		}

		public static string BodyAlreadyHasParent
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("BodyAlreadyHasParent");
			}
		}

		public static string CollectionIsReadOnly
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("CollectionIsReadOnly");
			}
		}

		public static string ChangingDntNotSupportedForEmbeddedTnefMessages
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("ChangingDntNotSupportedForEmbeddedTnefMessages");
			}
		}

		public static string CannotWriteBodyDoesNotExist
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("CannotWriteBodyDoesNotExist");
			}
		}

		public static string CannotSetNativePropertyForMimeRecipient
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("CannotSetNativePropertyForMimeRecipient");
			}
		}

		public static string ArgumentInvalidOffLen
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("ArgumentInvalidOffLen");
			}
		}

		public static string TnefContainsMultipleStreams
		{
			get
			{
				return EmailMessageStrings.ResourceManager.GetString("TnefContainsMultipleStreams");
			}
		}

		public static string InvalidBodyTypeForThisMessage(string value)
		{
			return string.Format(EmailMessageStrings.ResourceManager.GetString("InvalidBodyTypeForThisMessage"), value);
		}

		public static string GetLocalizedString(EmailMessageStrings.IDs key)
		{
			return EmailMessageStrings.ResourceManager.GetString(EmailMessageStrings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(36);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.EmailMessageStrings", typeof(EmailMessageStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ReferenceBodyDoesNotBelongToSameMessage = 1282546634U,
			CannotAccessMapiPropsFromPureMimeMessage = 4174367109U,
			ErrorInit = 2479849522U,
			TooManyEntriesInApplefile = 1426955056U,
			MimeDocumentRootPartMustNotBeNull = 392243280U,
			WrongOffsetsInApplefile = 4142370976U,
			CannotCreateAlternativeBody = 1857526624U,
			CanOnlyAddInlineAttachmentsToHtmlBody = 3435085534U,
			ErrorBeforeFirst = 4119843803U,
			ErrorAfterLast = 3424442414U,
			CannotAttachEmbeddedMapiMessageToMime = 2804270193U,
			BodyExistsInTnefMessage = 764847365U,
			NotSupportedForRtfBody = 1685978706U,
			CanOnlyAddInlineAttachmentForHtmlBody = 368316767U,
			UnexpectedEndOfStream = 797792475U,
			CannotSetEmbeddedMessageForTnefAttachment = 4122747589U,
			WrongAppleMagicNumber = 4281132505U,
			ContentTypeCannotBeMultipart = 3934212261U,
			CollectionHasChanged = 1586749494U,
			AttachmentRemovedFromMessage = 4139915868U,
			DigestCanOnlyContainMessage822Attachments = 2815148485U,
			RecipientAlreadyHasParent = 2676324379U,
			WrongAppleVersionNumber = 1686645306U,
			MacBinWrongFilename = 3682676966U,
			CannotAddAttachment = 532813113U,
			NoBodyForInlineAttachment = 414550776U,
			TnefIsMissingAttachRenderData = 1975150156U,
			CannotSetEmbeddedMessageForNonMessageRfc822Attachment = 2369512451U,
			WrongMacBinHeader = 2544842942U,
			BodyAlreadyHasParent = 3779231310U,
			CollectionIsReadOnly = 2792487344U,
			ChangingDntNotSupportedForEmbeddedTnefMessages = 2839669472U,
			CannotWriteBodyDoesNotExist = 2005747751U,
			CannotSetNativePropertyForMimeRecipient = 665063789U,
			ArgumentInvalidOffLen = 182340944U,
			TnefContainsMultipleStreams = 2700714933U
		}

		private enum ParamIDs
		{
			UnsupportedBodyType,
			EntryLengthTooBigInApplefile,
			CannotCreateSpecifiedBodyFormat,
			InvalidCharset,
			NestingTooDeep,
			InvalidBodyTypeForThisMessage
		}
	}
}

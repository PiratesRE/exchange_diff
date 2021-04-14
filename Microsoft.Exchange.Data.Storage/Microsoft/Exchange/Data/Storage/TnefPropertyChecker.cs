using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TnefPropertyChecker
	{
		internal TnefPropertyChecker(TnefType tnefType, bool isEmbeddedMessage, OutboundConversionOptions options)
		{
			this.isEmbeddedMessage = isEmbeddedMessage;
			this.tnefType = tnefType;
			if (isEmbeddedMessage)
			{
				this.messagePropertyExceptions = new Dictionary<NativeStorePropertyDefinition, bool>(TnefPropertyChecker.embeddedPropertyTransmittabilities);
			}
			else
			{
				this.messagePropertyExceptions = new Dictionary<NativeStorePropertyDefinition, bool>(TnefPropertyChecker.rootPropertyTransmittabilities);
			}
			this.messagePropertyExceptions.Add(InternalSchema.Categories, !options.ClearCategories);
			if (tnefType == TnefType.SummaryTnef)
			{
				this.recipientPropertyExceptions = new Dictionary<NativeStorePropertyDefinition, bool>(TnefPropertyChecker.stnefRecipientPropertyTransmittabilities);
			}
		}

		internal bool IsItemPropertyWritable(NativeStorePropertyDefinition property)
		{
			bool result = false;
			if (this.messagePropertyExceptions.TryGetValue(property, out result))
			{
				return result;
			}
			return !this.IsExtensionCustomProperty(property) && this.DefaultPropertyCheck(property);
		}

		internal bool IsAttachmentPropertyWritable(NativeStorePropertyDefinition property)
		{
			bool result = false;
			if (TnefPropertyChecker.attachmentPropertyTransmittabilities.TryGetValue(property, out result))
			{
				return result;
			}
			return this.DefaultPropertyCheck(property);
		}

		internal bool IsRecipientPropertyWritable(NativeStorePropertyDefinition property)
		{
			if (this.tnefType == TnefType.LegacyTnef)
			{
				return TnefPropertyChecker.ltnefRecipientProperties.Contains(property);
			}
			bool result = false;
			if (this.recipientPropertyExceptions.TryGetValue(property, out result))
			{
				return result;
			}
			return this.DefaultPropertyCheck(property);
		}

		private bool IsExtensionCustomProperty(NativeStorePropertyDefinition property)
		{
			GuidNamePropertyDefinition guidNamePropertyDefinition = property as GuidNamePropertyDefinition;
			return guidNamePropertyDefinition != null && guidNamePropertyDefinition.Guid == WellKnownPropertySet.PublicStrings && guidNamePropertyDefinition.PropertyName.StartsWith("cecp-", StringComparison.Ordinal);
		}

		private bool DefaultPropertyCheck(NativeStorePropertyDefinition property)
		{
			return this.isEmbeddedMessage || (property.PropertyFlags & PropertyFlags.Transmittable) == PropertyFlags.Transmittable;
		}

		static TnefPropertyChecker()
		{
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.PolicyTag, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.RetentionPeriod, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.StartDateEtc, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.RetentionDate, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.RetentionFlags, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.ArchiveTag, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.ArchiveDate, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.ArchivePeriod, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.NativeBlockStatus, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.TransportMessageHeaders, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.SpamConfidenceLevel, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.PurportedSenderDomain, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.ContentFilterPcl, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.SenderSearchKey, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.SenderFlags, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.SentRepresentingSearchKey, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.SentRepresentingFlags, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.ReceivedBySearchKey, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.ReceivedRepresentingSearchKey, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.OriginalAuthorSearchKey, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.OriginalSenderSearchKey, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.OriginalSentRepresentingSearchKey, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.OutlookVersion, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.OutlookInternalVersion, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.IsClassified, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.Classification, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.ClassificationDescription, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.ClassificationGuid, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.ClassificationKeep, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.ConversationIndexTracking, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.DRMLicense, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.DRMServerLicense, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.DRMServerLicenseCompressed, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.DRMRights, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.DRMExpiryTime, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.DRMPropsSignature, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.DrmPublishLicense, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.DRMPrelicenseFailure, false);
			TnefPropertyChecker.rootPropertyTransmittabilities.Add(InternalSchema.CalendarOriginatorId, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities = new Dictionary<NativeStorePropertyDefinition, bool>();
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.LocallyDelivered, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.XMsExchOrganizationAVStampMailbox, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.EntryId, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.StoreEntryId, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.DisplayToInternal, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.DisplayCcInternal, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.DisplayBccInternal, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.SubmitFlags, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.AccessLevel, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.NormalizedSubjectInternal, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.OutlookVersion, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.OutlookInternalVersion, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.DRMLicense, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.DRMServerLicense, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.DRMServerLicenseCompressed, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.DRMRights, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.DRMExpiryTime, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.DRMPropsSignature, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.DrmPublishLicense, false);
			TnefPropertyChecker.embeddedPropertyTransmittabilities.Add(InternalSchema.DRMPrelicenseFailure, false);
			TnefPropertyChecker.attachmentPropertyTransmittabilities = new Dictionary<NativeStorePropertyDefinition, bool>();
			TnefPropertyChecker.attachmentPropertyTransmittabilities.Add(InternalSchema.AttachCalendarHidden, true);
			TnefPropertyChecker.attachmentPropertyTransmittabilities.Add(InternalSchema.IsContactPhoto, true);
			TnefPropertyChecker.attachmentPropertyTransmittabilities.Add(InternalSchema.UrlCompName, false);
			TnefPropertyChecker.attachmentPropertyTransmittabilities.Add(InternalSchema.SenderIdStatus, false);
			TnefPropertyChecker.attachmentPropertyTransmittabilities.Add(InternalSchema.NativeBlockStatus, false);
			TnefPropertyChecker.attachmentPropertyTransmittabilities.Add(InternalSchema.AttachNum, false);
			TnefPropertyChecker.ltnefRecipientProperties = new HashSet<NativeStorePropertyDefinition>();
			TnefPropertyChecker.ltnefRecipientProperties.Add(InternalSchema.SmtpAddress);
			TnefPropertyChecker.ltnefRecipientProperties.Add(InternalSchema.TransmitableDisplayName);
			TnefPropertyChecker.ltnefRecipientProperties.Add(InternalSchema.DisplayName7Bit);
			TnefPropertyChecker.ltnefRecipientProperties.Add(InternalSchema.DisplayType);
			TnefPropertyChecker.ltnefRecipientProperties.Add(InternalSchema.ObjectType);
			TnefPropertyChecker.ltnefRecipientProperties.Add(InternalSchema.NdrDiagnosticCode);
			TnefPropertyChecker.ltnefRecipientProperties.Add(InternalSchema.NdrStatusCode);
			TnefPropertyChecker.ltnefRecipientProperties.Add(InternalSchema.SupplementaryInfo);
			TnefPropertyChecker.ltnefRecipientProperties.Add(InternalSchema.DeliveryTime);
			TnefPropertyChecker.ltnefRecipientProperties.Add(InternalSchema.ReportTime);
			TnefPropertyChecker.stnefRecipientPropertyTransmittabilities = new Dictionary<NativeStorePropertyDefinition, bool>();
			TnefPropertyChecker.stnefRecipientPropertyTransmittabilities.Add(InternalSchema.RowId, false);
			TnefPropertyChecker.stnefRecipientPropertyTransmittabilities.Add(InternalSchema.SearchKey, false);
			TnefPropertyChecker.stnefRecipientPropertyTransmittabilities.Add(InternalSchema.ObjectType, true);
			TnefPropertyChecker.stnefRecipientPropertyTransmittabilities.Add(InternalSchema.BusinessPhoneNumber, false);
			TnefPropertyChecker.stnefRecipientPropertyTransmittabilities.Add(InternalSchema.OfficeLocation, false);
			TnefPropertyChecker.stnefRecipientPropertyTransmittabilities.Add(InternalSchema.MobilePhone, false);
		}

		private bool isEmbeddedMessage;

		private TnefType tnefType;

		private Dictionary<NativeStorePropertyDefinition, bool> messagePropertyExceptions;

		private Dictionary<NativeStorePropertyDefinition, bool> recipientPropertyExceptions;

		private static Dictionary<NativeStorePropertyDefinition, bool> rootPropertyTransmittabilities = new Dictionary<NativeStorePropertyDefinition, bool>();

		private static Dictionary<NativeStorePropertyDefinition, bool> embeddedPropertyTransmittabilities;

		private static Dictionary<NativeStorePropertyDefinition, bool> attachmentPropertyTransmittabilities;

		private static Dictionary<NativeStorePropertyDefinition, bool> stnefRecipientPropertyTransmittabilities;

		private static HashSet<NativeStorePropertyDefinition> ltnefRecipientProperties;
	}
}

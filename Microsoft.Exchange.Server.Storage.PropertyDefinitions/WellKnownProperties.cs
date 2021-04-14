using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;

namespace Microsoft.Exchange.Server.Storage.PropTags
{
	public static class WellKnownProperties
	{
		public static StorePropTag GetPropTag(uint propTag, ObjectType objectType)
		{
			WellKnownProperties.ObjectPropertyDefinitions objectPropertyDefinitions = WellKnownProperties.PropertyDefinitions[(int)objectType];
			ushort num = (ushort)((propTag & 4294901760U) >> 16);
			PropertyType propertyType = (PropertyType)(propTag & 65535U);
			PropertyType propertyType2 = PropertyTypeHelper.MapToInternalPropertyType(propertyType);
			propTag = (uint)((int)num << 16 | (int)propertyType2);
			StorePropInfo propertyInfo = null;
			if (!objectPropertyDefinitions.Properties.TryGetValue(propTag, out propertyInfo) && !objectPropertyDefinitions.PropIds.TryGetValue(num, out propertyInfo) && !objectPropertyDefinitions.PropIdRanges.TryFindRange(num, out propertyInfo))
			{
				propertyInfo = WellKnownProperties.DefaultPropInfo;
			}
			return new StorePropTag(num, propertyType2, propertyInfo, propertyType, objectPropertyDefinitions.BaseObjectType);
		}

		public static bool TryGetPropTagByTagName(string tagName, ObjectType objectType, out StorePropTag propTag)
		{
			WellKnownProperties.ObjectPropertyDefinitions objectPropertyDefinitions = WellKnownProperties.PropertyDefinitions[(int)objectType];
			StorePropInfo storePropInfo;
			if (objectPropertyDefinitions.PropertiesByTagName.TryGetValue(tagName, out storePropInfo))
			{
				propTag = new StorePropTag(storePropInfo.PropId, storePropInfo.PropType, storePropInfo, objectPropertyDefinitions.BaseObjectType);
				return true;
			}
			propTag = StorePropTag.Invalid;
			return false;
		}

		public static StoreNamedPropInfo GetNamedPropInfo(StorePropName propName)
		{
			StoreNamedPropInfo result;
			if (!WellKnownProperties.NamedPropInfos.TryGetValue(propName, out result))
			{
				result = new StoreNamedPropInfo(propName);
			}
			return result;
		}

		private static ObjectType[] BuildBaseObjectTypeArray()
		{
			ObjectType[] array = new ObjectType[22];
			array[1] = ObjectType.Mailbox;
			array[2] = ObjectType.Folder;
			array[3] = ObjectType.Message;
			array[4] = ObjectType.Attachment;
			array[6] = ObjectType.Recipient;
			array[11] = ObjectType.Event;
			array[10] = ObjectType.PermissionView;
			array[14] = ObjectType.ViewDefinition;
			array[21] = ObjectType.RestrictionView;
			array[12] = ObjectType.LocalDirectory;
			array[16] = ObjectType.ResourceDigest;
			array[15] = ObjectType.IcsState;
			array[13] = ObjectType.InferenceLog;
			array[17] = ObjectType.ProcessInfo;
			array[18] = ObjectType.FastTransferStream;
			array[19] = ObjectType.IsIntegJob;
			array[20] = ObjectType.UserInfo;
			array[5] = ObjectType.Message;
			array[8] = ObjectType.Folder;
			array[9] = ObjectType.Attachment;
			array[7] = ObjectType.Message;
			return array;
		}

		private static WellKnownProperties.ObjectPropertyDefinitions[] BuildPropertyDefinitions()
		{
			WellKnownProperties.ObjectPropertyDefinitions[] array = new WellKnownProperties.ObjectPropertyDefinitions[22];
			array[1] = WellKnownProperties.BuildMailboxPropertyDefinitions();
			array[2] = WellKnownProperties.BuildFolderPropertyDefinitions();
			array[3] = WellKnownProperties.BuildMessagePropertyDefinitions();
			array[4] = WellKnownProperties.BuildAttachmentPropertyDefinitions();
			array[6] = WellKnownProperties.BuildRecipientPropertyDefinitions();
			array[11] = WellKnownProperties.BuildEventPropertyDefinitions();
			array[10] = WellKnownProperties.BuildPermissionViewPropertyDefinitions();
			array[14] = WellKnownProperties.BuildViewDefinitionPropertyDefinitions();
			array[21] = WellKnownProperties.BuildRestrictionViewPropertyDefinitions();
			array[12] = WellKnownProperties.BuildLocalDirectoryPropertyDefinitions();
			array[16] = WellKnownProperties.BuildResourceDigestPropertyDefinitions();
			array[15] = WellKnownProperties.BuildIcsStatePropertyDefinitions();
			array[13] = WellKnownProperties.BuildInferenceLogPropertyDefinitions();
			array[17] = WellKnownProperties.BuildProcessInfoPropertyDefinitions();
			array[18] = WellKnownProperties.BuildFastTransferStreamPropertyDefinitions();
			array[19] = WellKnownProperties.BuildIsIntegJobPropertyDefinitions();
			array[20] = WellKnownProperties.BuildUserInfoPropertyDefinitions();
			array[5] = new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.Message,
				Properties = array[3].Properties,
				PropertiesByTagName = array[3].PropertiesByTagName,
				PropIds = array[3].PropIds,
				PropIdRanges = array[3].PropIdRanges
			};
			array[8] = new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.Folder,
				Properties = array[2].Properties,
				PropertiesByTagName = array[2].PropertiesByTagName,
				PropIds = array[2].PropIds,
				PropIdRanges = array[2].PropIdRanges
			};
			array[9] = new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.Attachment,
				Properties = array[4].Properties,
				PropertiesByTagName = array[4].PropertiesByTagName,
				PropIds = array[4].PropIds,
				PropIdRanges = array[4].PropIdRanges
			};
			array[7] = new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.Message,
				Properties = array[3].Properties,
				PropertiesByTagName = array[3].PropertiesByTagName,
				PropIds = array[3].PropIds,
				PropIdRanges = array[3].PropIdRanges
			};
			return array;
		}

		private static WellKnownProperties.ObjectPropertyDefinitions BuildMailboxPropertyDefinitions()
		{
			Dictionary<string, StorePropInfo> propertiesByTagName = WellKnownProperties.BuildMailboxPropertyDefinitionsByName();
			Dictionary<uint, StorePropInfo> properties = WellKnownProperties.BuildMailboxPropertyDefinitionsByTag();
			Dictionary<ushort, StorePropInfo> dictionary = new Dictionary<ushort, StorePropInfo>(100);
			dictionary.Add(3592, new StorePropInfo(null, 3592, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(3619, new StorePropInfo(null, 3619, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(3676, new StorePropInfo(null, 3676, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(4084, new StorePropInfo(null, 4084, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(12289, new StorePropInfo(null, 12289, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(12291, new StorePropInfo(null, 12291, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(12292, new StorePropInfo(null, 12292, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(12295, new StorePropInfo(null, 12295, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(13313, new StorePropInfo(null, 13313, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(13314, new StorePropInfo(null, 13314, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(13315, new StorePropInfo(null, 13315, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(13316, new StorePropInfo(null, 13316, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(13317, new StorePropInfo(null, 13317, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(13318, new StorePropInfo(null, 13318, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(13319, new StorePropInfo(null, 13319, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(13320, new StorePropInfo(null, 13320, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(13326, new StorePropInfo(null, 13326, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(13334, new StorePropInfo(null, 13334, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(13826, new StorePropInfo(null, 13826, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(13847, new StorePropInfo(null, 13847, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(14847, new StorePropInfo(null, 14847, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(15782, new StorePropInfo(null, 15782, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 3)));
			dictionary.Add(15790, new StorePropInfo(null, 15790, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 3)));
			dictionary.Add(15804, new StorePropInfo(null, 15804, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 3)));
			dictionary.Add(16373, new StorePropInfo(null, 16373, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(16383, new StorePropInfo(null, 16383, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 3)));
			dictionary.Add(26136, new StorePropInfo(null, 26136, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26137, new StorePropInfo(null, 26137, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26138, new StorePropInfo(null, 26138, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26139, new StorePropInfo(null, 26139, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26140, new StorePropInfo(null, 26140, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26168, new StorePropInfo(null, 26168, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26176, new StorePropInfo(null, 26176, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26179, new StorePropInfo(null, 26179, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26186, new StorePropInfo(null, 26186, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26218, new StorePropInfo(null, 26218, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26221, new StorePropInfo(null, 26221, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26222, new StorePropInfo(null, 26222, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26255, new StorePropInfo(null, 26255, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26267, new StorePropInfo(null, 26267, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26268, new StorePropInfo(null, 26268, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26269, new StorePropInfo(null, 26269, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26274, new StorePropInfo(null, 26274, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26275, new StorePropInfo(null, 26275, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26277, new StorePropInfo(null, 26277, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26280, new StorePropInfo(null, 26280, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26281, new StorePropInfo(null, 26281, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26283, new StorePropInfo(null, 26283, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26284, new StorePropInfo(null, 26284, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26285, new StorePropInfo(null, 26285, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26286, new StorePropInfo(null, 26286, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26287, new StorePropInfo(null, 26287, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26288, new StorePropInfo(null, 26288, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26289, new StorePropInfo(null, 26289, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26290, new StorePropInfo(null, 26290, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26291, new StorePropInfo(null, 26291, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26292, new StorePropInfo(null, 26292, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26293, new StorePropInfo(null, 26293, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26294, new StorePropInfo(null, 26294, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26295, new StorePropInfo(null, 26295, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26307, new StorePropInfo(null, 26307, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26375, new StorePropInfo(null, 26375, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26376, new StorePropInfo(null, 26376, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26468, new StorePropInfo(null, 26468, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26474, new StorePropInfo(null, 26474, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26475, new StorePropInfo(null, 26475, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26483, new StorePropInfo(null, 26483, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26514, new StorePropInfo(null, 26514, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26516, new StorePropInfo(null, 26516, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26630, new StorePropInfo(null, 26630, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(26655, new StorePropInfo(null, 26655, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			WellKnownProperties.PropIdRangeList propIdRanges = WellKnownProperties.BuildPropIdRanges(new WellKnownProperties.TempIdRange[]
			{
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.NoGetProp
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.NoGetPropList
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.SetPropRestricted
				}
			});
			return new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.Mailbox,
				Properties = properties,
				PropertiesByTagName = propertiesByTagName,
				PropIds = dictionary,
				PropIdRanges = propIdRanges
			};
		}

		private static Dictionary<string, StorePropInfo> BuildMailboxPropertyDefinitionsByName()
		{
			Dictionary<string, StorePropInfo> dictionary = new Dictionary<string, StorePropInfo>(492, StringComparer.OrdinalIgnoreCase);
			dictionary["DeleteAfterSubmit"] = PropTag.Mailbox.DeleteAfterSubmit.PropInfo;
			dictionary["MessageSize"] = PropTag.Mailbox.MessageSize.PropInfo;
			dictionary["MessageSize32"] = PropTag.Mailbox.MessageSize32.PropInfo;
			dictionary["SentMailEntryId"] = PropTag.Mailbox.SentMailEntryId.PropInfo;
			dictionary["HighestFolderInternetId"] = PropTag.Mailbox.HighestFolderInternetId.PropInfo;
			dictionary["NTSecurityDescriptor"] = PropTag.Mailbox.NTSecurityDescriptor.PropInfo;
			dictionary["CISearchEnabled"] = PropTag.Mailbox.CISearchEnabled.PropInfo;
			dictionary["ExtendedRuleSizeLimit"] = PropTag.Mailbox.ExtendedRuleSizeLimit.PropInfo;
			dictionary["Access"] = PropTag.Mailbox.Access.PropInfo;
			dictionary["MappingSignature"] = PropTag.Mailbox.MappingSignature.PropInfo;
			dictionary["StoreRecordKey"] = PropTag.Mailbox.StoreRecordKey.PropInfo;
			dictionary["StoreEntryId"] = PropTag.Mailbox.StoreEntryId.PropInfo;
			dictionary["DisplayName"] = PropTag.Mailbox.DisplayName.PropInfo;
			dictionary["EmailAddress"] = PropTag.Mailbox.EmailAddress.PropInfo;
			dictionary["Comment"] = PropTag.Mailbox.Comment.PropInfo;
			dictionary["CreationTime"] = PropTag.Mailbox.CreationTime.PropInfo;
			dictionary["LastModificationTime"] = PropTag.Mailbox.LastModificationTime.PropInfo;
			dictionary["ResourceFlags"] = PropTag.Mailbox.ResourceFlags.PropInfo;
			dictionary["MessageTableTotalPages"] = PropTag.Mailbox.MessageTableTotalPages.PropInfo;
			dictionary["MessageTableAvailablePages"] = PropTag.Mailbox.MessageTableAvailablePages.PropInfo;
			dictionary["OtherTablesTotalPages"] = PropTag.Mailbox.OtherTablesTotalPages.PropInfo;
			dictionary["OtherTablesAvailablePages"] = PropTag.Mailbox.OtherTablesAvailablePages.PropInfo;
			dictionary["AttachmentTableTotalPages"] = PropTag.Mailbox.AttachmentTableTotalPages.PropInfo;
			dictionary["AttachmentTableAvailablePages"] = PropTag.Mailbox.AttachmentTableAvailablePages.PropInfo;
			dictionary["MailboxTypeVersion"] = PropTag.Mailbox.MailboxTypeVersion.PropInfo;
			dictionary["MailboxPartitionMailboxGuids"] = PropTag.Mailbox.MailboxPartitionMailboxGuids.PropInfo;
			dictionary["StoreSupportMask"] = PropTag.Mailbox.StoreSupportMask.PropInfo;
			dictionary["StoreState"] = PropTag.Mailbox.StoreState.PropInfo;
			dictionary["IPMSubtreeSearchKey"] = PropTag.Mailbox.IPMSubtreeSearchKey.PropInfo;
			dictionary["IPMOutboxSearchKey"] = PropTag.Mailbox.IPMOutboxSearchKey.PropInfo;
			dictionary["IPMWastebasketSearchKey"] = PropTag.Mailbox.IPMWastebasketSearchKey.PropInfo;
			dictionary["IPMSentmailSearchKey"] = PropTag.Mailbox.IPMSentmailSearchKey.PropInfo;
			dictionary["MdbProvider"] = PropTag.Mailbox.MdbProvider.PropInfo;
			dictionary["ReceiveFolderSettings"] = PropTag.Mailbox.ReceiveFolderSettings.PropInfo;
			dictionary["LocalDirectoryEntryID"] = PropTag.Mailbox.LocalDirectoryEntryID.PropInfo;
			dictionary["ControlDataForCalendarRepairAssistant"] = PropTag.Mailbox.ControlDataForCalendarRepairAssistant.PropInfo;
			dictionary["ControlDataForSharingPolicyAssistant"] = PropTag.Mailbox.ControlDataForSharingPolicyAssistant.PropInfo;
			dictionary["ControlDataForElcAssistant"] = PropTag.Mailbox.ControlDataForElcAssistant.PropInfo;
			dictionary["ControlDataForTopNWordsAssistant"] = PropTag.Mailbox.ControlDataForTopNWordsAssistant.PropInfo;
			dictionary["ControlDataForJunkEmailAssistant"] = PropTag.Mailbox.ControlDataForJunkEmailAssistant.PropInfo;
			dictionary["ControlDataForCalendarSyncAssistant"] = PropTag.Mailbox.ControlDataForCalendarSyncAssistant.PropInfo;
			dictionary["ExternalSharingCalendarSubscriptionCount"] = PropTag.Mailbox.ExternalSharingCalendarSubscriptionCount.PropInfo;
			dictionary["ControlDataForUMReportingAssistant"] = PropTag.Mailbox.ControlDataForUMReportingAssistant.PropInfo;
			dictionary["HasUMReportData"] = PropTag.Mailbox.HasUMReportData.PropInfo;
			dictionary["InternetCalendarSubscriptionCount"] = PropTag.Mailbox.InternetCalendarSubscriptionCount.PropInfo;
			dictionary["ExternalSharingContactSubscriptionCount"] = PropTag.Mailbox.ExternalSharingContactSubscriptionCount.PropInfo;
			dictionary["JunkEmailSafeListDirty"] = PropTag.Mailbox.JunkEmailSafeListDirty.PropInfo;
			dictionary["IsTopNEnabled"] = PropTag.Mailbox.IsTopNEnabled.PropInfo;
			dictionary["LastSharingPolicyAppliedId"] = PropTag.Mailbox.LastSharingPolicyAppliedId.PropInfo;
			dictionary["LastSharingPolicyAppliedHash"] = PropTag.Mailbox.LastSharingPolicyAppliedHash.PropInfo;
			dictionary["LastSharingPolicyAppliedTime"] = PropTag.Mailbox.LastSharingPolicyAppliedTime.PropInfo;
			dictionary["OofScheduleStart"] = PropTag.Mailbox.OofScheduleStart.PropInfo;
			dictionary["OofScheduleEnd"] = PropTag.Mailbox.OofScheduleEnd.PropInfo;
			dictionary["ControlDataForDirectoryProcessorAssistant"] = PropTag.Mailbox.ControlDataForDirectoryProcessorAssistant.PropInfo;
			dictionary["NeedsDirectoryProcessor"] = PropTag.Mailbox.NeedsDirectoryProcessor.PropInfo;
			dictionary["RetentionQueryIds"] = PropTag.Mailbox.RetentionQueryIds.PropInfo;
			dictionary["RetentionQueryInfo"] = PropTag.Mailbox.RetentionQueryInfo.PropInfo;
			dictionary["ControlDataForPublicFolderAssistant"] = PropTag.Mailbox.ControlDataForPublicFolderAssistant.PropInfo;
			dictionary["ControlDataForInferenceTrainingAssistant"] = PropTag.Mailbox.ControlDataForInferenceTrainingAssistant.PropInfo;
			dictionary["InferenceEnabled"] = PropTag.Mailbox.InferenceEnabled.PropInfo;
			dictionary["ControlDataForContactLinkingAssistant"] = PropTag.Mailbox.ControlDataForContactLinkingAssistant.PropInfo;
			dictionary["ContactLinking"] = PropTag.Mailbox.ContactLinking.PropInfo;
			dictionary["ControlDataForOABGeneratorAssistant"] = PropTag.Mailbox.ControlDataForOABGeneratorAssistant.PropInfo;
			dictionary["ContactSaveVersion"] = PropTag.Mailbox.ContactSaveVersion.PropInfo;
			dictionary["ControlDataForOrgContactsSyncAssistant"] = PropTag.Mailbox.ControlDataForOrgContactsSyncAssistant.PropInfo;
			dictionary["OrgContactsSyncTimestamp"] = PropTag.Mailbox.OrgContactsSyncTimestamp.PropInfo;
			dictionary["PushNotificationSubscriptionType"] = PropTag.Mailbox.PushNotificationSubscriptionType.PropInfo;
			dictionary["OrgContactsSyncADWatermark"] = PropTag.Mailbox.OrgContactsSyncADWatermark.PropInfo;
			dictionary["ControlDataForInferenceDataCollectionAssistant"] = PropTag.Mailbox.ControlDataForInferenceDataCollectionAssistant.PropInfo;
			dictionary["InferenceDataCollectionProcessingState"] = PropTag.Mailbox.InferenceDataCollectionProcessingState.PropInfo;
			dictionary["ControlDataForPeopleRelevanceAssistant"] = PropTag.Mailbox.ControlDataForPeopleRelevanceAssistant.PropInfo;
			dictionary["SiteMailboxInternalState"] = PropTag.Mailbox.SiteMailboxInternalState.PropInfo;
			dictionary["ControlDataForSiteMailboxAssistant"] = PropTag.Mailbox.ControlDataForSiteMailboxAssistant.PropInfo;
			dictionary["InferenceTrainingLastContentCount"] = PropTag.Mailbox.InferenceTrainingLastContentCount.PropInfo;
			dictionary["InferenceTrainingLastAttemptTimestamp"] = PropTag.Mailbox.InferenceTrainingLastAttemptTimestamp.PropInfo;
			dictionary["InferenceTrainingLastSuccessTimestamp"] = PropTag.Mailbox.InferenceTrainingLastSuccessTimestamp.PropInfo;
			dictionary["InferenceUserCapabilityFlags"] = PropTag.Mailbox.InferenceUserCapabilityFlags.PropInfo;
			dictionary["ControlDataForMailboxAssociationReplicationAssistant"] = PropTag.Mailbox.ControlDataForMailboxAssociationReplicationAssistant.PropInfo;
			dictionary["MailboxAssociationNextReplicationTime"] = PropTag.Mailbox.MailboxAssociationNextReplicationTime.PropInfo;
			dictionary["MailboxAssociationProcessingFlags"] = PropTag.Mailbox.MailboxAssociationProcessingFlags.PropInfo;
			dictionary["ControlDataForSharePointSignalStoreAssistant"] = PropTag.Mailbox.ControlDataForSharePointSignalStoreAssistant.PropInfo;
			dictionary["ControlDataForPeopleCentricTriageAssistant"] = PropTag.Mailbox.ControlDataForPeopleCentricTriageAssistant.PropInfo;
			dictionary["NotificationBrokerSubscriptions"] = PropTag.Mailbox.NotificationBrokerSubscriptions.PropInfo;
			dictionary["GroupMailboxPermissionsVersion"] = PropTag.Mailbox.GroupMailboxPermissionsVersion.PropInfo;
			dictionary["ElcLastRunTotalProcessingTime"] = PropTag.Mailbox.ElcLastRunTotalProcessingTime.PropInfo;
			dictionary["ElcLastRunSubAssistantProcessingTime"] = PropTag.Mailbox.ElcLastRunSubAssistantProcessingTime.PropInfo;
			dictionary["ElcLastRunUpdatedFolderCount"] = PropTag.Mailbox.ElcLastRunUpdatedFolderCount.PropInfo;
			dictionary["ElcLastRunTaggedFolderCount"] = PropTag.Mailbox.ElcLastRunTaggedFolderCount.PropInfo;
			dictionary["ElcLastRunUpdatedItemCount"] = PropTag.Mailbox.ElcLastRunUpdatedItemCount.PropInfo;
			dictionary["ElcLastRunTaggedWithArchiveItemCount"] = PropTag.Mailbox.ElcLastRunTaggedWithArchiveItemCount.PropInfo;
			dictionary["ElcLastRunTaggedWithExpiryItemCount"] = PropTag.Mailbox.ElcLastRunTaggedWithExpiryItemCount.PropInfo;
			dictionary["ElcLastRunDeletedFromRootItemCount"] = PropTag.Mailbox.ElcLastRunDeletedFromRootItemCount.PropInfo;
			dictionary["ElcLastRunDeletedFromDumpsterItemCount"] = PropTag.Mailbox.ElcLastRunDeletedFromDumpsterItemCount.PropInfo;
			dictionary["ElcLastRunArchivedFromRootItemCount"] = PropTag.Mailbox.ElcLastRunArchivedFromRootItemCount.PropInfo;
			dictionary["ElcLastRunArchivedFromDumpsterItemCount"] = PropTag.Mailbox.ElcLastRunArchivedFromDumpsterItemCount.PropInfo;
			dictionary["ScheduledISIntegLastFinished"] = PropTag.Mailbox.ScheduledISIntegLastFinished.PropInfo;
			dictionary["ControlDataForSearchIndexRepairAssistant"] = PropTag.Mailbox.ControlDataForSearchIndexRepairAssistant.PropInfo;
			dictionary["ELCLastSuccessTimestamp"] = PropTag.Mailbox.ELCLastSuccessTimestamp.PropInfo;
			dictionary["InferenceTruthLoggingLastAttemptTimestamp"] = PropTag.Mailbox.InferenceTruthLoggingLastAttemptTimestamp.PropInfo;
			dictionary["InferenceTruthLoggingLastSuccessTimestamp"] = PropTag.Mailbox.InferenceTruthLoggingLastSuccessTimestamp.PropInfo;
			dictionary["ControlDataForGroupMailboxAssistant"] = PropTag.Mailbox.ControlDataForGroupMailboxAssistant.PropInfo;
			dictionary["ItemsPendingUpgrade"] = PropTag.Mailbox.ItemsPendingUpgrade.PropInfo;
			dictionary["ConsumerSharingCalendarSubscriptionCount"] = PropTag.Mailbox.ConsumerSharingCalendarSubscriptionCount.PropInfo;
			dictionary["GroupMailboxGeneratedPhotoVersion"] = PropTag.Mailbox.GroupMailboxGeneratedPhotoVersion.PropInfo;
			dictionary["GroupMailboxGeneratedPhotoSignature"] = PropTag.Mailbox.GroupMailboxGeneratedPhotoSignature.PropInfo;
			dictionary["GroupMailboxExchangeResourcesPublishedVersion"] = PropTag.Mailbox.GroupMailboxExchangeResourcesPublishedVersion.PropInfo;
			dictionary["ValidFolderMask"] = PropTag.Mailbox.ValidFolderMask.PropInfo;
			dictionary["IPMSubtreeEntryId"] = PropTag.Mailbox.IPMSubtreeEntryId.PropInfo;
			dictionary["IPMOutboxEntryId"] = PropTag.Mailbox.IPMOutboxEntryId.PropInfo;
			dictionary["IPMWastebasketEntryId"] = PropTag.Mailbox.IPMWastebasketEntryId.PropInfo;
			dictionary["IPMSentmailEntryId"] = PropTag.Mailbox.IPMSentmailEntryId.PropInfo;
			dictionary["IPMViewsEntryId"] = PropTag.Mailbox.IPMViewsEntryId.PropInfo;
			dictionary["UnsearchableItems"] = PropTag.Mailbox.UnsearchableItems.PropInfo;
			dictionary["IPMFinderEntryId"] = PropTag.Mailbox.IPMFinderEntryId.PropInfo;
			dictionary["ContentCount"] = PropTag.Mailbox.ContentCount.PropInfo;
			dictionary["ContentCountInt64"] = PropTag.Mailbox.ContentCountInt64.PropInfo;
			dictionary["Search"] = PropTag.Mailbox.Search.PropInfo;
			dictionary["AssociatedContentCount"] = PropTag.Mailbox.AssociatedContentCount.PropInfo;
			dictionary["AssociatedContentCountInt64"] = PropTag.Mailbox.AssociatedContentCountInt64.PropInfo;
			dictionary["AdditionalRENEntryIds"] = PropTag.Mailbox.AdditionalRENEntryIds.PropInfo;
			dictionary["SimpleDisplayName"] = PropTag.Mailbox.SimpleDisplayName.PropInfo;
			dictionary["TestBlobProperty"] = PropTag.Mailbox.TestBlobProperty.PropInfo;
			dictionary["ScheduledISIntegCorruptionCount"] = PropTag.Mailbox.ScheduledISIntegCorruptionCount.PropInfo;
			dictionary["ScheduledISIntegExecutionTime"] = PropTag.Mailbox.ScheduledISIntegExecutionTime.PropInfo;
			dictionary["MailboxPartitionNumber"] = PropTag.Mailbox.MailboxPartitionNumber.PropInfo;
			dictionary["MailboxTypeDetail"] = PropTag.Mailbox.MailboxTypeDetail.PropInfo;
			dictionary["InternalTenantHint"] = PropTag.Mailbox.InternalTenantHint.PropInfo;
			dictionary["TenantHint"] = PropTag.Mailbox.TenantHint.PropInfo;
			dictionary["MaintenanceId"] = PropTag.Mailbox.MaintenanceId.PropInfo;
			dictionary["MailboxType"] = PropTag.Mailbox.MailboxType.PropInfo;
			dictionary["ACLData"] = PropTag.Mailbox.ACLData.PropInfo;
			dictionary["DesignInProgress"] = PropTag.Mailbox.DesignInProgress.PropInfo;
			dictionary["StorageQuotaLimit"] = PropTag.Mailbox.StorageQuotaLimit.PropInfo;
			dictionary["RulesSize"] = PropTag.Mailbox.RulesSize.PropInfo;
			dictionary["IMAPSubscribeList"] = PropTag.Mailbox.IMAPSubscribeList.PropInfo;
			dictionary["InTransitState"] = PropTag.Mailbox.InTransitState.PropInfo;
			dictionary["InTransitStatus"] = PropTag.Mailbox.InTransitStatus.PropInfo;
			dictionary["UserEntryId"] = PropTag.Mailbox.UserEntryId.PropInfo;
			dictionary["UserName"] = PropTag.Mailbox.UserName.PropInfo;
			dictionary["MailboxOwnerEntryId"] = PropTag.Mailbox.MailboxOwnerEntryId.PropInfo;
			dictionary["MailboxOwnerName"] = PropTag.Mailbox.MailboxOwnerName.PropInfo;
			dictionary["OofState"] = PropTag.Mailbox.OofState.PropInfo;
			dictionary["TestLineSpeed"] = PropTag.Mailbox.TestLineSpeed.PropInfo;
			dictionary["SerializedReplidGuidMap"] = PropTag.Mailbox.SerializedReplidGuidMap.PropInfo;
			dictionary["DeletedMsgCount"] = PropTag.Mailbox.DeletedMsgCount.PropInfo;
			dictionary["DeletedMsgCountInt64"] = PropTag.Mailbox.DeletedMsgCountInt64.PropInfo;
			dictionary["DeletedAssocMsgCount"] = PropTag.Mailbox.DeletedAssocMsgCount.PropInfo;
			dictionary["DeletedAssocMsgCountInt64"] = PropTag.Mailbox.DeletedAssocMsgCountInt64.PropInfo;
			dictionary["HasNamedProperties"] = PropTag.Mailbox.HasNamedProperties.PropInfo;
			dictionary["ActiveUserEntryId"] = PropTag.Mailbox.ActiveUserEntryId.PropInfo;
			dictionary["ProhibitReceiveQuota"] = PropTag.Mailbox.ProhibitReceiveQuota.PropInfo;
			dictionary["MaxSubmitMessageSize"] = PropTag.Mailbox.MaxSubmitMessageSize.PropInfo;
			dictionary["ProhibitSendQuota"] = PropTag.Mailbox.ProhibitSendQuota.PropInfo;
			dictionary["DeletedOn"] = PropTag.Mailbox.DeletedOn.PropInfo;
			dictionary["MailboxDatabaseVersion"] = PropTag.Mailbox.MailboxDatabaseVersion.PropInfo;
			dictionary["DeletedMessageSize"] = PropTag.Mailbox.DeletedMessageSize.PropInfo;
			dictionary["DeletedMessageSize32"] = PropTag.Mailbox.DeletedMessageSize32.PropInfo;
			dictionary["DeletedNormalMessageSize"] = PropTag.Mailbox.DeletedNormalMessageSize.PropInfo;
			dictionary["DeletedNormalMessageSize32"] = PropTag.Mailbox.DeletedNormalMessageSize32.PropInfo;
			dictionary["DeletedAssociatedMessageSize"] = PropTag.Mailbox.DeletedAssociatedMessageSize.PropInfo;
			dictionary["DeletedAssociatedMessageSize32"] = PropTag.Mailbox.DeletedAssociatedMessageSize32.PropInfo;
			dictionary["NTUsername"] = PropTag.Mailbox.NTUsername.PropInfo;
			dictionary["NTUserSid"] = PropTag.Mailbox.NTUserSid.PropInfo;
			dictionary["LocaleId"] = PropTag.Mailbox.LocaleId.PropInfo;
			dictionary["LastLogonTime"] = PropTag.Mailbox.LastLogonTime.PropInfo;
			dictionary["LastLogoffTime"] = PropTag.Mailbox.LastLogoffTime.PropInfo;
			dictionary["StorageLimitInformation"] = PropTag.Mailbox.StorageLimitInformation.PropInfo;
			dictionary["InternetMdns"] = PropTag.Mailbox.InternetMdns.PropInfo;
			dictionary["MailboxStatus"] = PropTag.Mailbox.MailboxStatus.PropInfo;
			dictionary["MailboxFlags"] = PropTag.Mailbox.MailboxFlags.PropInfo;
			dictionary["PreservingMailboxSignature"] = PropTag.Mailbox.PreservingMailboxSignature.PropInfo;
			dictionary["MRSPreservingMailboxSignature"] = PropTag.Mailbox.MRSPreservingMailboxSignature.PropInfo;
			dictionary["MailboxMessagesPerFolderCountWarningQuota"] = PropTag.Mailbox.MailboxMessagesPerFolderCountWarningQuota.PropInfo;
			dictionary["MailboxMessagesPerFolderCountReceiveQuota"] = PropTag.Mailbox.MailboxMessagesPerFolderCountReceiveQuota.PropInfo;
			dictionary["DumpsterMessagesPerFolderCountWarningQuota"] = PropTag.Mailbox.DumpsterMessagesPerFolderCountWarningQuota.PropInfo;
			dictionary["DumpsterMessagesPerFolderCountReceiveQuota"] = PropTag.Mailbox.DumpsterMessagesPerFolderCountReceiveQuota.PropInfo;
			dictionary["FolderHierarchyChildrenCountWarningQuota"] = PropTag.Mailbox.FolderHierarchyChildrenCountWarningQuota.PropInfo;
			dictionary["FolderHierarchyChildrenCountReceiveQuota"] = PropTag.Mailbox.FolderHierarchyChildrenCountReceiveQuota.PropInfo;
			dictionary["FolderHierarchyDepthWarningQuota"] = PropTag.Mailbox.FolderHierarchyDepthWarningQuota.PropInfo;
			dictionary["FolderHierarchyDepthReceiveQuota"] = PropTag.Mailbox.FolderHierarchyDepthReceiveQuota.PropInfo;
			dictionary["NormalMessageSize"] = PropTag.Mailbox.NormalMessageSize.PropInfo;
			dictionary["NormalMessageSize32"] = PropTag.Mailbox.NormalMessageSize32.PropInfo;
			dictionary["AssociatedMessageSize"] = PropTag.Mailbox.AssociatedMessageSize.PropInfo;
			dictionary["AssociatedMessageSize32"] = PropTag.Mailbox.AssociatedMessageSize32.PropInfo;
			dictionary["FoldersCountWarningQuota"] = PropTag.Mailbox.FoldersCountWarningQuota.PropInfo;
			dictionary["FoldersCountReceiveQuota"] = PropTag.Mailbox.FoldersCountReceiveQuota.PropInfo;
			dictionary["NamedPropertiesCountQuota"] = PropTag.Mailbox.NamedPropertiesCountQuota.PropInfo;
			dictionary["CodePageId"] = PropTag.Mailbox.CodePageId.PropInfo;
			dictionary["RetentionAgeLimit"] = PropTag.Mailbox.RetentionAgeLimit.PropInfo;
			dictionary["UserDisplayName"] = PropTag.Mailbox.UserDisplayName.PropInfo;
			dictionary["SortLocaleId"] = PropTag.Mailbox.SortLocaleId.PropInfo;
			dictionary["MailboxDSGuid"] = PropTag.Mailbox.MailboxDSGuid.PropInfo;
			dictionary["MailboxDSGuidGuid"] = PropTag.Mailbox.MailboxDSGuidGuid.PropInfo;
			dictionary["DateDiscoveredAbsentInDS"] = PropTag.Mailbox.DateDiscoveredAbsentInDS.PropInfo;
			dictionary["UnifiedMailboxGuidGuid"] = PropTag.Mailbox.UnifiedMailboxGuidGuid.PropInfo;
			dictionary["QuotaWarningThreshold"] = PropTag.Mailbox.QuotaWarningThreshold.PropInfo;
			dictionary["QuotaSendThreshold"] = PropTag.Mailbox.QuotaSendThreshold.PropInfo;
			dictionary["QuotaReceiveThreshold"] = PropTag.Mailbox.QuotaReceiveThreshold.PropInfo;
			dictionary["PropertyGroupMappingId"] = PropTag.Mailbox.PropertyGroupMappingId.PropInfo;
			dictionary["SentMailSvrEID"] = PropTag.Mailbox.SentMailSvrEID.PropInfo;
			dictionary["SentMailSvrEIDBin"] = PropTag.Mailbox.SentMailSvrEIDBin.PropInfo;
			dictionary["LocalIdNext"] = PropTag.Mailbox.LocalIdNext.PropInfo;
			dictionary["RootFid"] = PropTag.Mailbox.RootFid.PropInfo;
			dictionary["FIDC"] = PropTag.Mailbox.FIDC.PropInfo;
			dictionary["MdbDSGuid"] = PropTag.Mailbox.MdbDSGuid.PropInfo;
			dictionary["MailboxOwnerDN"] = PropTag.Mailbox.MailboxOwnerDN.PropInfo;
			dictionary["MapiEntryIdGuid"] = PropTag.Mailbox.MapiEntryIdGuid.PropInfo;
			dictionary["Localized"] = PropTag.Mailbox.Localized.PropInfo;
			dictionary["LCID"] = PropTag.Mailbox.LCID.PropInfo;
			dictionary["AltRecipientDN"] = PropTag.Mailbox.AltRecipientDN.PropInfo;
			dictionary["NoLocalDelivery"] = PropTag.Mailbox.NoLocalDelivery.PropInfo;
			dictionary["DeliveryContentLength"] = PropTag.Mailbox.DeliveryContentLength.PropInfo;
			dictionary["AutoReply"] = PropTag.Mailbox.AutoReply.PropInfo;
			dictionary["MailboxOwnerDisplayName"] = PropTag.Mailbox.MailboxOwnerDisplayName.PropInfo;
			dictionary["MailboxLastUpdated"] = PropTag.Mailbox.MailboxLastUpdated.PropInfo;
			dictionary["AdminSurName"] = PropTag.Mailbox.AdminSurName.PropInfo;
			dictionary["AdminGivenName"] = PropTag.Mailbox.AdminGivenName.PropInfo;
			dictionary["ActiveSearchCount"] = PropTag.Mailbox.ActiveSearchCount.PropInfo;
			dictionary["AdminNickname"] = PropTag.Mailbox.AdminNickname.PropInfo;
			dictionary["QuotaStyle"] = PropTag.Mailbox.QuotaStyle.PropInfo;
			dictionary["OverQuotaLimit"] = PropTag.Mailbox.OverQuotaLimit.PropInfo;
			dictionary["StorageQuota"] = PropTag.Mailbox.StorageQuota.PropInfo;
			dictionary["SubmitContentLength"] = PropTag.Mailbox.SubmitContentLength.PropInfo;
			dictionary["ReservedIdCounterRangeUpperLimit"] = PropTag.Mailbox.ReservedIdCounterRangeUpperLimit.PropInfo;
			dictionary["ReservedCnCounterRangeUpperLimit"] = PropTag.Mailbox.ReservedCnCounterRangeUpperLimit.PropInfo;
			dictionary["FolderIdsetIn"] = PropTag.Mailbox.FolderIdsetIn.PropInfo;
			dictionary["CnsetIn"] = PropTag.Mailbox.CnsetIn.PropInfo;
			dictionary["ShutoffQuota"] = PropTag.Mailbox.ShutoffQuota.PropInfo;
			dictionary["MailboxMiscFlags"] = PropTag.Mailbox.MailboxMiscFlags.PropInfo;
			dictionary["MailboxInCreation"] = PropTag.Mailbox.MailboxInCreation.PropInfo;
			dictionary["ObjectClassFlags"] = PropTag.Mailbox.ObjectClassFlags.PropInfo;
			dictionary["OOFStateEx"] = PropTag.Mailbox.OOFStateEx.PropInfo;
			dictionary["OofStateUserChangeTime"] = PropTag.Mailbox.OofStateUserChangeTime.PropInfo;
			dictionary["UserOofSettingsItemId"] = PropTag.Mailbox.UserOofSettingsItemId.PropInfo;
			dictionary["MailboxQuarantined"] = PropTag.Mailbox.MailboxQuarantined.PropInfo;
			dictionary["MailboxQuarantineDescription"] = PropTag.Mailbox.MailboxQuarantineDescription.PropInfo;
			dictionary["MailboxQuarantineLastCrash"] = PropTag.Mailbox.MailboxQuarantineLastCrash.PropInfo;
			dictionary["MailboxQuarantineEnd"] = PropTag.Mailbox.MailboxQuarantineEnd.PropInfo;
			dictionary["MailboxNum"] = PropTag.Mailbox.MailboxNum.PropInfo;
			dictionary["MaxMessageSize"] = PropTag.Mailbox.MaxMessageSize.PropInfo;
			dictionary["InferenceClientActivityFlags"] = PropTag.Mailbox.InferenceClientActivityFlags.PropInfo;
			dictionary["InferenceOWAUserActivityLoggingEnabledDeprecated"] = PropTag.Mailbox.InferenceOWAUserActivityLoggingEnabledDeprecated.PropInfo;
			dictionary["InferenceOLKUserActivityLoggingEnabled"] = PropTag.Mailbox.InferenceOLKUserActivityLoggingEnabled.PropInfo;
			dictionary["InferenceTrainedModelVersionBreadCrumb"] = PropTag.Mailbox.InferenceTrainedModelVersionBreadCrumb.PropInfo;
			dictionary["UserPhotoCacheId"] = PropTag.Mailbox.UserPhotoCacheId.PropInfo;
			dictionary["UserPhotoPreviewCacheId"] = PropTag.Mailbox.UserPhotoPreviewCacheId.PropInfo;
			return dictionary;
		}

		private static Dictionary<uint, StorePropInfo> BuildMailboxPropertyDefinitionsByTag()
		{
			Dictionary<uint, StorePropInfo> dictionary = new Dictionary<uint, StorePropInfo>(492);
			dictionary[234946571U] = PropTag.Mailbox.DeleteAfterSubmit.PropInfo;
			dictionary[235405332U] = PropTag.Mailbox.MessageSize.PropInfo;
			dictionary[235405315U] = PropTag.Mailbox.MessageSize32.PropInfo;
			dictionary[235536642U] = PropTag.Mailbox.SentMailEntryId.PropInfo;
			dictionary[237174787U] = PropTag.Mailbox.HighestFolderInternetId.PropInfo;
			dictionary[237437186U] = PropTag.Mailbox.NTSecurityDescriptor.PropInfo;
			dictionary[240910347U] = PropTag.Mailbox.CISearchEnabled.PropInfo;
			dictionary[245039107U] = PropTag.Mailbox.ExtendedRuleSizeLimit.PropInfo;
			dictionary[267649027U] = PropTag.Mailbox.Access.PropInfo;
			dictionary[267911426U] = PropTag.Mailbox.MappingSignature.PropInfo;
			dictionary[268042498U] = PropTag.Mailbox.StoreRecordKey.PropInfo;
			dictionary[268108034U] = PropTag.Mailbox.StoreEntryId.PropInfo;
			dictionary[805371935U] = PropTag.Mailbox.DisplayName.PropInfo;
			dictionary[805503007U] = PropTag.Mailbox.EmailAddress.PropInfo;
			dictionary[805568543U] = PropTag.Mailbox.Comment.PropInfo;
			dictionary[805765184U] = PropTag.Mailbox.CreationTime.PropInfo;
			dictionary[805830720U] = PropTag.Mailbox.LastModificationTime.PropInfo;
			dictionary[805896195U] = PropTag.Mailbox.ResourceFlags.PropInfo;
			dictionary[872480771U] = PropTag.Mailbox.MessageTableTotalPages.PropInfo;
			dictionary[872546307U] = PropTag.Mailbox.MessageTableAvailablePages.PropInfo;
			dictionary[872611843U] = PropTag.Mailbox.OtherTablesTotalPages.PropInfo;
			dictionary[872677379U] = PropTag.Mailbox.OtherTablesAvailablePages.PropInfo;
			dictionary[872742915U] = PropTag.Mailbox.AttachmentTableTotalPages.PropInfo;
			dictionary[872808451U] = PropTag.Mailbox.AttachmentTableAvailablePages.PropInfo;
			dictionary[872873987U] = PropTag.Mailbox.MailboxTypeVersion.PropInfo;
			dictionary[872943688U] = PropTag.Mailbox.MailboxPartitionMailboxGuids.PropInfo;
			dictionary[873267203U] = PropTag.Mailbox.StoreSupportMask.PropInfo;
			dictionary[873332739U] = PropTag.Mailbox.StoreState.PropInfo;
			dictionary[873464066U] = PropTag.Mailbox.IPMSubtreeSearchKey.PropInfo;
			dictionary[873529602U] = PropTag.Mailbox.IPMOutboxSearchKey.PropInfo;
			dictionary[873595138U] = PropTag.Mailbox.IPMWastebasketSearchKey.PropInfo;
			dictionary[873660674U] = PropTag.Mailbox.IPMSentmailSearchKey.PropInfo;
			dictionary[873726210U] = PropTag.Mailbox.MdbProvider.PropInfo;
			dictionary[873791501U] = PropTag.Mailbox.ReceiveFolderSettings.PropInfo;
			dictionary[873857282U] = PropTag.Mailbox.LocalDirectoryEntryID.PropInfo;
			dictionary[874512642U] = PropTag.Mailbox.ControlDataForCalendarRepairAssistant.PropInfo;
			dictionary[874578178U] = PropTag.Mailbox.ControlDataForSharingPolicyAssistant.PropInfo;
			dictionary[874643714U] = PropTag.Mailbox.ControlDataForElcAssistant.PropInfo;
			dictionary[874709250U] = PropTag.Mailbox.ControlDataForTopNWordsAssistant.PropInfo;
			dictionary[874774786U] = PropTag.Mailbox.ControlDataForJunkEmailAssistant.PropInfo;
			dictionary[874840322U] = PropTag.Mailbox.ControlDataForCalendarSyncAssistant.PropInfo;
			dictionary[874905603U] = PropTag.Mailbox.ExternalSharingCalendarSubscriptionCount.PropInfo;
			dictionary[874971394U] = PropTag.Mailbox.ControlDataForUMReportingAssistant.PropInfo;
			dictionary[875036683U] = PropTag.Mailbox.HasUMReportData.PropInfo;
			dictionary[875102211U] = PropTag.Mailbox.InternetCalendarSubscriptionCount.PropInfo;
			dictionary[875167747U] = PropTag.Mailbox.ExternalSharingContactSubscriptionCount.PropInfo;
			dictionary[875233283U] = PropTag.Mailbox.JunkEmailSafeListDirty.PropInfo;
			dictionary[875298827U] = PropTag.Mailbox.IsTopNEnabled.PropInfo;
			dictionary[875364610U] = PropTag.Mailbox.LastSharingPolicyAppliedId.PropInfo;
			dictionary[875430146U] = PropTag.Mailbox.LastSharingPolicyAppliedHash.PropInfo;
			dictionary[875495488U] = PropTag.Mailbox.LastSharingPolicyAppliedTime.PropInfo;
			dictionary[875561024U] = PropTag.Mailbox.OofScheduleStart.PropInfo;
			dictionary[875626560U] = PropTag.Mailbox.OofScheduleEnd.PropInfo;
			dictionary[875692290U] = PropTag.Mailbox.ControlDataForDirectoryProcessorAssistant.PropInfo;
			dictionary[875757579U] = PropTag.Mailbox.NeedsDirectoryProcessor.PropInfo;
			dictionary[875827231U] = PropTag.Mailbox.RetentionQueryIds.PropInfo;
			dictionary[875888660U] = PropTag.Mailbox.RetentionQueryInfo.PropInfo;
			dictionary[876019970U] = PropTag.Mailbox.ControlDataForPublicFolderAssistant.PropInfo;
			dictionary[876085506U] = PropTag.Mailbox.ControlDataForInferenceTrainingAssistant.PropInfo;
			dictionary[876150795U] = PropTag.Mailbox.InferenceEnabled.PropInfo;
			dictionary[876216578U] = PropTag.Mailbox.ControlDataForContactLinkingAssistant.PropInfo;
			dictionary[876281859U] = PropTag.Mailbox.ContactLinking.PropInfo;
			dictionary[876347650U] = PropTag.Mailbox.ControlDataForOABGeneratorAssistant.PropInfo;
			dictionary[876412931U] = PropTag.Mailbox.ContactSaveVersion.PropInfo;
			dictionary[876478722U] = PropTag.Mailbox.ControlDataForOrgContactsSyncAssistant.PropInfo;
			dictionary[876544064U] = PropTag.Mailbox.OrgContactsSyncTimestamp.PropInfo;
			dictionary[876609794U] = PropTag.Mailbox.PushNotificationSubscriptionType.PropInfo;
			dictionary[876675136U] = PropTag.Mailbox.OrgContactsSyncADWatermark.PropInfo;
			dictionary[876740866U] = PropTag.Mailbox.ControlDataForInferenceDataCollectionAssistant.PropInfo;
			dictionary[876806402U] = PropTag.Mailbox.InferenceDataCollectionProcessingState.PropInfo;
			dictionary[876871938U] = PropTag.Mailbox.ControlDataForPeopleRelevanceAssistant.PropInfo;
			dictionary[876937219U] = PropTag.Mailbox.SiteMailboxInternalState.PropInfo;
			dictionary[877003010U] = PropTag.Mailbox.ControlDataForSiteMailboxAssistant.PropInfo;
			dictionary[877068291U] = PropTag.Mailbox.InferenceTrainingLastContentCount.PropInfo;
			dictionary[877133888U] = PropTag.Mailbox.InferenceTrainingLastAttemptTimestamp.PropInfo;
			dictionary[877199424U] = PropTag.Mailbox.InferenceTrainingLastSuccessTimestamp.PropInfo;
			dictionary[877264899U] = PropTag.Mailbox.InferenceUserCapabilityFlags.PropInfo;
			dictionary[877330690U] = PropTag.Mailbox.ControlDataForMailboxAssociationReplicationAssistant.PropInfo;
			dictionary[877396032U] = PropTag.Mailbox.MailboxAssociationNextReplicationTime.PropInfo;
			dictionary[877461507U] = PropTag.Mailbox.MailboxAssociationProcessingFlags.PropInfo;
			dictionary[877527298U] = PropTag.Mailbox.ControlDataForSharePointSignalStoreAssistant.PropInfo;
			dictionary[877592834U] = PropTag.Mailbox.ControlDataForPeopleCentricTriageAssistant.PropInfo;
			dictionary[877658115U] = PropTag.Mailbox.NotificationBrokerSubscriptions.PropInfo;
			dictionary[877723651U] = PropTag.Mailbox.GroupMailboxPermissionsVersion.PropInfo;
			dictionary[877789204U] = PropTag.Mailbox.ElcLastRunTotalProcessingTime.PropInfo;
			dictionary[877854740U] = PropTag.Mailbox.ElcLastRunSubAssistantProcessingTime.PropInfo;
			dictionary[877920276U] = PropTag.Mailbox.ElcLastRunUpdatedFolderCount.PropInfo;
			dictionary[877985812U] = PropTag.Mailbox.ElcLastRunTaggedFolderCount.PropInfo;
			dictionary[878051348U] = PropTag.Mailbox.ElcLastRunUpdatedItemCount.PropInfo;
			dictionary[878116884U] = PropTag.Mailbox.ElcLastRunTaggedWithArchiveItemCount.PropInfo;
			dictionary[878182420U] = PropTag.Mailbox.ElcLastRunTaggedWithExpiryItemCount.PropInfo;
			dictionary[878247956U] = PropTag.Mailbox.ElcLastRunDeletedFromRootItemCount.PropInfo;
			dictionary[878313492U] = PropTag.Mailbox.ElcLastRunDeletedFromDumpsterItemCount.PropInfo;
			dictionary[878379028U] = PropTag.Mailbox.ElcLastRunArchivedFromRootItemCount.PropInfo;
			dictionary[878444564U] = PropTag.Mailbox.ElcLastRunArchivedFromDumpsterItemCount.PropInfo;
			dictionary[878510144U] = PropTag.Mailbox.ScheduledISIntegLastFinished.PropInfo;
			dictionary[878575874U] = PropTag.Mailbox.ControlDataForSearchIndexRepairAssistant.PropInfo;
			dictionary[878641216U] = PropTag.Mailbox.ELCLastSuccessTimestamp.PropInfo;
			dictionary[878772288U] = PropTag.Mailbox.InferenceTruthLoggingLastAttemptTimestamp.PropInfo;
			dictionary[878837824U] = PropTag.Mailbox.InferenceTruthLoggingLastSuccessTimestamp.PropInfo;
			dictionary[878903554U] = PropTag.Mailbox.ControlDataForGroupMailboxAssistant.PropInfo;
			dictionary[878968835U] = PropTag.Mailbox.ItemsPendingUpgrade.PropInfo;
			dictionary[879034371U] = PropTag.Mailbox.ConsumerSharingCalendarSubscriptionCount.PropInfo;
			dictionary[879099907U] = PropTag.Mailbox.GroupMailboxGeneratedPhotoVersion.PropInfo;
			dictionary[879165698U] = PropTag.Mailbox.GroupMailboxGeneratedPhotoSignature.PropInfo;
			dictionary[879230979U] = PropTag.Mailbox.GroupMailboxExchangeResourcesPublishedVersion.PropInfo;
			dictionary[903806979U] = PropTag.Mailbox.ValidFolderMask.PropInfo;
			dictionary[903872770U] = PropTag.Mailbox.IPMSubtreeEntryId.PropInfo;
			dictionary[904003842U] = PropTag.Mailbox.IPMOutboxEntryId.PropInfo;
			dictionary[904069378U] = PropTag.Mailbox.IPMWastebasketEntryId.PropInfo;
			dictionary[904134914U] = PropTag.Mailbox.IPMSentmailEntryId.PropInfo;
			dictionary[904200450U] = PropTag.Mailbox.IPMViewsEntryId.PropInfo;
			dictionary[905838850U] = PropTag.Mailbox.UnsearchableItems.PropInfo;
			dictionary[905969922U] = PropTag.Mailbox.IPMFinderEntryId.PropInfo;
			dictionary[906100739U] = PropTag.Mailbox.ContentCount.PropInfo;
			dictionary[906100756U] = PropTag.Mailbox.ContentCountInt64.PropInfo;
			dictionary[906428429U] = PropTag.Mailbox.Search.PropInfo;
			dictionary[907476995U] = PropTag.Mailbox.AssociatedContentCount.PropInfo;
			dictionary[907477012U] = PropTag.Mailbox.AssociatedContentCountInt64.PropInfo;
			dictionary[920129794U] = PropTag.Mailbox.AdditionalRENEntryIds.PropInfo;
			dictionary[973013023U] = PropTag.Mailbox.SimpleDisplayName.PropInfo;
			dictionary[1023410196U] = PropTag.Mailbox.TestBlobProperty.PropInfo;
			dictionary[1033699331U] = PropTag.Mailbox.ScheduledISIntegCorruptionCount.PropInfo;
			dictionary[1033764867U] = PropTag.Mailbox.ScheduledISIntegExecutionTime.PropInfo;
			dictionary[1033830403U] = PropTag.Mailbox.MailboxPartitionNumber.PropInfo;
			dictionary[1034289155U] = PropTag.Mailbox.MailboxTypeDetail.PropInfo;
			dictionary[1034354946U] = PropTag.Mailbox.InternalTenantHint.PropInfo;
			dictionary[1034813698U] = PropTag.Mailbox.TenantHint.PropInfo;
			dictionary[1035665480U] = PropTag.Mailbox.MaintenanceId.PropInfo;
			dictionary[1035730947U] = PropTag.Mailbox.MailboxType.PropInfo;
			dictionary[1071644930U] = PropTag.Mailbox.ACLData.PropInfo;
			dictionary[1071906827U] = PropTag.Mailbox.DesignInProgress.PropInfo;
			dictionary[1073020931U] = PropTag.Mailbox.StorageQuotaLimit.PropInfo;
			dictionary[1073676291U] = PropTag.Mailbox.RulesSize.PropInfo;
			dictionary[1710624799U] = PropTag.Mailbox.IMAPSubscribeList.PropInfo;
			dictionary[1712848907U] = PropTag.Mailbox.InTransitState.PropInfo;
			dictionary[1712848899U] = PropTag.Mailbox.InTransitStatus.PropInfo;
			dictionary[1712914690U] = PropTag.Mailbox.UserEntryId.PropInfo;
			dictionary[1712979999U] = PropTag.Mailbox.UserName.PropInfo;
			dictionary[1713045762U] = PropTag.Mailbox.MailboxOwnerEntryId.PropInfo;
			dictionary[1713111071U] = PropTag.Mailbox.MailboxOwnerName.PropInfo;
			dictionary[1713176587U] = PropTag.Mailbox.OofState.PropInfo;
			dictionary[1714094338U] = PropTag.Mailbox.TestLineSpeed.PropInfo;
			dictionary[1714946306U] = PropTag.Mailbox.SerializedReplidGuidMap.PropInfo;
			dictionary[1715470339U] = PropTag.Mailbox.DeletedMsgCount.PropInfo;
			dictionary[1715470356U] = PropTag.Mailbox.DeletedMsgCountInt64.PropInfo;
			dictionary[1715666947U] = PropTag.Mailbox.DeletedAssocMsgCount.PropInfo;
			dictionary[1715666964U] = PropTag.Mailbox.DeletedAssocMsgCountInt64.PropInfo;
			dictionary[1716125707U] = PropTag.Mailbox.HasNamedProperties.PropInfo;
			dictionary[1716650242U] = PropTag.Mailbox.ActiveUserEntryId.PropInfo;
			dictionary[1718222851U] = PropTag.Mailbox.ProhibitReceiveQuota.PropInfo;
			dictionary[1718419459U] = PropTag.Mailbox.MaxSubmitMessageSize.PropInfo;
			dictionary[1718484995U] = PropTag.Mailbox.ProhibitSendQuota.PropInfo;
			dictionary[1720647744U] = PropTag.Mailbox.DeletedOn.PropInfo;
			dictionary[1721368579U] = PropTag.Mailbox.MailboxDatabaseVersion.PropInfo;
			dictionary[1721434132U] = PropTag.Mailbox.DeletedMessageSize.PropInfo;
			dictionary[1721434115U] = PropTag.Mailbox.DeletedMessageSize32.PropInfo;
			dictionary[1721499668U] = PropTag.Mailbox.DeletedNormalMessageSize.PropInfo;
			dictionary[1721499651U] = PropTag.Mailbox.DeletedNormalMessageSize32.PropInfo;
			dictionary[1721565204U] = PropTag.Mailbox.DeletedAssociatedMessageSize.PropInfo;
			dictionary[1721565187U] = PropTag.Mailbox.DeletedAssociatedMessageSize32.PropInfo;
			dictionary[1721761823U] = PropTag.Mailbox.NTUsername.PropInfo;
			dictionary[1721762050U] = PropTag.Mailbox.NTUserSid.PropInfo;
			dictionary[1721827331U] = PropTag.Mailbox.LocaleId.PropInfo;
			dictionary[1721892928U] = PropTag.Mailbox.LastLogonTime.PropInfo;
			dictionary[1721958464U] = PropTag.Mailbox.LastLogoffTime.PropInfo;
			dictionary[1722023939U] = PropTag.Mailbox.StorageLimitInformation.PropInfo;
			dictionary[1722089483U] = PropTag.Mailbox.InternetMdns.PropInfo;
			dictionary[1722089474U] = PropTag.Mailbox.MailboxStatus.PropInfo;
			dictionary[1722220547U] = PropTag.Mailbox.MailboxFlags.PropInfo;
			dictionary[1722286091U] = PropTag.Mailbox.PreservingMailboxSignature.PropInfo;
			dictionary[1722351627U] = PropTag.Mailbox.MRSPreservingMailboxSignature.PropInfo;
			dictionary[1722482691U] = PropTag.Mailbox.MailboxMessagesPerFolderCountWarningQuota.PropInfo;
			dictionary[1722548227U] = PropTag.Mailbox.MailboxMessagesPerFolderCountReceiveQuota.PropInfo;
			dictionary[1722613763U] = PropTag.Mailbox.DumpsterMessagesPerFolderCountWarningQuota.PropInfo;
			dictionary[1722679299U] = PropTag.Mailbox.DumpsterMessagesPerFolderCountReceiveQuota.PropInfo;
			dictionary[1722744835U] = PropTag.Mailbox.FolderHierarchyChildrenCountWarningQuota.PropInfo;
			dictionary[1722810371U] = PropTag.Mailbox.FolderHierarchyChildrenCountReceiveQuota.PropInfo;
			dictionary[1722875907U] = PropTag.Mailbox.FolderHierarchyDepthWarningQuota.PropInfo;
			dictionary[1722941443U] = PropTag.Mailbox.FolderHierarchyDepthReceiveQuota.PropInfo;
			dictionary[1723006996U] = PropTag.Mailbox.NormalMessageSize.PropInfo;
			dictionary[1723006979U] = PropTag.Mailbox.NormalMessageSize32.PropInfo;
			dictionary[1723072532U] = PropTag.Mailbox.AssociatedMessageSize.PropInfo;
			dictionary[1723072515U] = PropTag.Mailbox.AssociatedMessageSize32.PropInfo;
			dictionary[1723138051U] = PropTag.Mailbox.FoldersCountWarningQuota.PropInfo;
			dictionary[1723203587U] = PropTag.Mailbox.FoldersCountReceiveQuota.PropInfo;
			dictionary[1723269123U] = PropTag.Mailbox.NamedPropertiesCountQuota.PropInfo;
			dictionary[1724055555U] = PropTag.Mailbox.CodePageId.PropInfo;
			dictionary[1724121091U] = PropTag.Mailbox.RetentionAgeLimit.PropInfo;
			dictionary[1724579871U] = PropTag.Mailbox.UserDisplayName.PropInfo;
			dictionary[1728380931U] = PropTag.Mailbox.SortLocaleId.PropInfo;
			dictionary[1728512258U] = PropTag.Mailbox.MailboxDSGuid.PropInfo;
			dictionary[1728512072U] = PropTag.Mailbox.MailboxDSGuidGuid.PropInfo;
			dictionary[1728577600U] = PropTag.Mailbox.DateDiscoveredAbsentInDS.PropInfo;
			dictionary[1728577608U] = PropTag.Mailbox.UnifiedMailboxGuidGuid.PropInfo;
			dictionary[1730215939U] = PropTag.Mailbox.QuotaWarningThreshold.PropInfo;
			dictionary[1730281475U] = PropTag.Mailbox.QuotaSendThreshold.PropInfo;
			dictionary[1730347011U] = PropTag.Mailbox.QuotaReceiveThreshold.PropInfo;
			dictionary[1731461123U] = PropTag.Mailbox.PropertyGroupMappingId.PropInfo;
			dictionary[1732247803U] = PropTag.Mailbox.SentMailSvrEID.PropInfo;
			dictionary[1732247810U] = PropTag.Mailbox.SentMailSvrEIDBin.PropInfo;
			dictionary[1734410498U] = PropTag.Mailbox.LocalIdNext.PropInfo;
			dictionary[1734606868U] = PropTag.Mailbox.RootFid.PropInfo;
			dictionary[1734738178U] = PropTag.Mailbox.FIDC.PropInfo;
			dictionary[1735000322U] = PropTag.Mailbox.MdbDSGuid.PropInfo;
			dictionary[1735065631U] = PropTag.Mailbox.MailboxOwnerDN.PropInfo;
			dictionary[1735131394U] = PropTag.Mailbox.MapiEntryIdGuid.PropInfo;
			dictionary[1735196683U] = PropTag.Mailbox.Localized.PropInfo;
			dictionary[1735262211U] = PropTag.Mailbox.LCID.PropInfo;
			dictionary[1735327775U] = PropTag.Mailbox.AltRecipientDN.PropInfo;
			dictionary[1735393291U] = PropTag.Mailbox.NoLocalDelivery.PropInfo;
			dictionary[1735458819U] = PropTag.Mailbox.DeliveryContentLength.PropInfo;
			dictionary[1735524363U] = PropTag.Mailbox.AutoReply.PropInfo;
			dictionary[1735589919U] = PropTag.Mailbox.MailboxOwnerDisplayName.PropInfo;
			dictionary[1735655488U] = PropTag.Mailbox.MailboxLastUpdated.PropInfo;
			dictionary[1735720991U] = PropTag.Mailbox.AdminSurName.PropInfo;
			dictionary[1735786527U] = PropTag.Mailbox.AdminGivenName.PropInfo;
			dictionary[1735852035U] = PropTag.Mailbox.ActiveSearchCount.PropInfo;
			dictionary[1735917599U] = PropTag.Mailbox.AdminNickname.PropInfo;
			dictionary[1735983107U] = PropTag.Mailbox.QuotaStyle.PropInfo;
			dictionary[1736048643U] = PropTag.Mailbox.OverQuotaLimit.PropInfo;
			dictionary[1736114179U] = PropTag.Mailbox.StorageQuota.PropInfo;
			dictionary[1736179715U] = PropTag.Mailbox.SubmitContentLength.PropInfo;
			dictionary[1736310804U] = PropTag.Mailbox.ReservedIdCounterRangeUpperLimit.PropInfo;
			dictionary[1736376340U] = PropTag.Mailbox.ReservedCnCounterRangeUpperLimit.PropInfo;
			dictionary[1737621762U] = PropTag.Mailbox.FolderIdsetIn.PropInfo;
			dictionary[1737752834U] = PropTag.Mailbox.CnsetIn.PropInfo;
			dictionary[1745092611U] = PropTag.Mailbox.ShutoffQuota.PropInfo;
			dictionary[1745223683U] = PropTag.Mailbox.MailboxMiscFlags.PropInfo;
			dictionary[1745551371U] = PropTag.Mailbox.MailboxInCreation.PropInfo;
			dictionary[1745682435U] = PropTag.Mailbox.ObjectClassFlags.PropInfo;
			dictionary[1745879043U] = PropTag.Mailbox.OOFStateEx.PropInfo;
			dictionary[1746075712U] = PropTag.Mailbox.OofStateUserChangeTime.PropInfo;
			dictionary[1746141442U] = PropTag.Mailbox.UserOofSettingsItemId.PropInfo;
			dictionary[1746534411U] = PropTag.Mailbox.MailboxQuarantined.PropInfo;
			dictionary[1746599967U] = PropTag.Mailbox.MailboxQuarantineDescription.PropInfo;
			dictionary[1746665536U] = PropTag.Mailbox.MailboxQuarantineLastCrash.PropInfo;
			dictionary[1746731072U] = PropTag.Mailbox.MailboxQuarantineEnd.PropInfo;
			dictionary[1746862083U] = PropTag.Mailbox.MailboxNum.PropInfo;
			dictionary[1747779587U] = PropTag.Mailbox.MaxMessageSize.PropInfo;
			dictionary[1748238339U] = PropTag.Mailbox.InferenceClientActivityFlags.PropInfo;
			dictionary[1748303883U] = PropTag.Mailbox.InferenceOWAUserActivityLoggingEnabledDeprecated.PropInfo;
			dictionary[1748369419U] = PropTag.Mailbox.InferenceOLKUserActivityLoggingEnabled.PropInfo;
			dictionary[1752367362U] = PropTag.Mailbox.InferenceTrainedModelVersionBreadCrumb.PropInfo;
			dictionary[2082078723U] = PropTag.Mailbox.UserPhotoCacheId.PropInfo;
			dictionary[2082144259U] = PropTag.Mailbox.UserPhotoPreviewCacheId.PropInfo;
			return dictionary;
		}

		private static WellKnownProperties.ObjectPropertyDefinitions BuildFolderPropertyDefinitions()
		{
			Dictionary<string, StorePropInfo> propertiesByTagName = WellKnownProperties.BuildFolderPropertyDefinitionsByName();
			Dictionary<uint, StorePropInfo> properties = WellKnownProperties.BuildFolderPropertyDefinitionsByTag();
			Dictionary<ushort, StorePropInfo> dictionary = new Dictionary<ushort, StorePropInfo>(100);
			dictionary.Add(3592, new StorePropInfo(null, 3592, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(3593, new StorePropInfo(null, 3593, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3, 9)));
			dictionary.Add(3619, new StorePropInfo(null, 3619, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(3623, new StorePropInfo(null, 3623, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(9)));
			dictionary.Add(3647, new StorePropInfo(null, 3647, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(9)));
			dictionary.Add(3672, new StorePropInfo(null, 3672, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(3673, new StorePropInfo(null, 3673, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(3679, new StorePropInfo(null, 3679, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(3683, new StorePropInfo(null, 3683, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(3684, new StorePropInfo(null, 3684, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(3707, new StorePropInfo(null, 3707, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(3840, new StorePropInfo(null, 3840, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(9)));
			dictionary.Add(4084, new StorePropInfo(null, 4084, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(4086, new StorePropInfo(null, 4086, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3, 9)));
			dictionary.Add(4087, new StorePropInfo(null, 4087, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(4088, new StorePropInfo(null, 4088, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 9)));
			dictionary.Add(4089, new StorePropInfo(null, 4089, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3, 9)));
			dictionary.Add(4090, new StorePropInfo(null, 4090, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 9)));
			dictionary.Add(4091, new StorePropInfo(null, 4091, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 9)));
			dictionary.Add(4094, new StorePropInfo(null, 4094, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 9)));
			dictionary.Add(4095, new StorePropInfo(null, 4095, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3, 9)));
			dictionary.Add(12293, new StorePropInfo(null, 12293, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(12295, new StorePropInfo(null, 12295, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(12296, new StorePropInfo(null, 12296, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(13325, new StorePropInfo(null, 13325, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 9)));
			dictionary.Add(13795, new StorePropInfo(null, 13795, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(13825, new StorePropInfo(null, 13825, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(13826, new StorePropInfo(null, 13826, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(13827, new StorePropInfo(null, 13827, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(13834, new StorePropInfo(null, 13834, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(13838, new StorePropInfo(null, 13838, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(13839, new StorePropInfo(null, 13839, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(13840, new StorePropInfo(null, 13840, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(13847, new StorePropInfo(null, 13847, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(13968, new StorePropInfo(null, 13968, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(13969, new StorePropInfo(null, 13969, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(14031, new StorePropInfo(null, 14031, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(14065, new StorePropInfo(null, 14065, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(14592, new StorePropInfo(null, 14592, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 9)));
			dictionary.Add(15825, new StorePropInfo(null, 15825, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(16329, new StorePropInfo(null, 16329, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(16342, new StorePropInfo(null, 16342, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(9)));
			dictionary.Add(16343, new StorePropInfo(null, 16343, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(0, 1, 2, 3, 9)));
			dictionary.Add(16352, new StorePropInfo(null, 16352, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(0, 1, 2, 3, 9)));
			dictionary.Add(16353, new StorePropInfo(null, 16353, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(16382, new StorePropInfo(null, 16382, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(0, 1, 2, 3, 9)));
			dictionary.Add(16514, new StorePropInfo(null, 16514, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26080, new StorePropInfo(null, 26080, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26081, new StorePropInfo(null, 26081, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26082, new StorePropInfo(null, 26082, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26083, new StorePropInfo(null, 26083, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26105, new StorePropInfo(null, 26105, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(10)));
			dictionary.Add(26168, new StorePropInfo(null, 26168, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26169, new StorePropInfo(null, 26169, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26174, new StorePropInfo(null, 26174, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 16)));
			dictionary.Add(26175, new StorePropInfo(null, 26175, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26176, new StorePropInfo(null, 26176, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26177, new StorePropInfo(null, 26177, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26179, new StorePropInfo(null, 26179, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26181, new StorePropInfo(null, 26181, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26182, new StorePropInfo(null, 26182, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26186, new StorePropInfo(null, 26186, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26197, new StorePropInfo(null, 26197, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26255, new StorePropInfo(null, 26255, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26267, new StorePropInfo(null, 26267, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26268, new StorePropInfo(null, 26268, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26269, new StorePropInfo(null, 26269, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26280, new StorePropInfo(null, 26280, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26285, new StorePropInfo(null, 26285, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26286, new StorePropInfo(null, 26286, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26287, new StorePropInfo(null, 26287, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26288, new StorePropInfo(null, 26288, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26289, new StorePropInfo(null, 26289, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26290, new StorePropInfo(null, 26290, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26291, new StorePropInfo(null, 26291, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26292, new StorePropInfo(null, 26292, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26293, new StorePropInfo(null, 26293, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26377, new StorePropInfo(null, 26377, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26378, new StorePropInfo(null, 26378, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26379, new StorePropInfo(null, 26379, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26386, new StorePropInfo(null, 26386, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26401, new StorePropInfo(null, 26401, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26402, new StorePropInfo(null, 26402, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26413, new StorePropInfo(null, 26413, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26417, new StorePropInfo(null, 26417, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26418, new StorePropInfo(null, 26418, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26419, new StorePropInfo(null, 26419, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26440, new StorePropInfo(null, 26440, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26441, new StorePropInfo(null, 26441, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26449, new StorePropInfo(null, 26449, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26457, new StorePropInfo(null, 26457, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26458, new StorePropInfo(null, 26458, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26459, new StorePropInfo(null, 26459, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26460, new StorePropInfo(null, 26460, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26464, new StorePropInfo(null, 26464, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26468, new StorePropInfo(null, 26468, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26489, new StorePropInfo(null, 26489, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26491, new StorePropInfo(null, 26491, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26494, new StorePropInfo(null, 26494, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26495, new StorePropInfo(null, 26495, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26498, new StorePropInfo(null, 26498, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26499, new StorePropInfo(null, 26499, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26514, new StorePropInfo(null, 26514, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3, 9)));
			dictionary.Add(26516, new StorePropInfo(null, 26516, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 16)));
			dictionary.Add(26518, new StorePropInfo(null, 26518, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 16)));
			dictionary.Add(26523, new StorePropInfo(null, 26523, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26532, new StorePropInfo(null, 26532, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26533, new StorePropInfo(null, 26533, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26534, new StorePropInfo(null, 26534, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26574, new StorePropInfo(null, 26574, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26586, new StorePropInfo(null, 26586, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 16)));
			dictionary.Add(26655, new StorePropInfo(null, 26655, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26672, new StorePropInfo(null, 26672, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26673, new StorePropInfo(null, 26673, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			WellKnownProperties.PropIdRangeList propIdRanges = WellKnownProperties.BuildPropIdRanges(new WellKnownProperties.TempIdRange[]
			{
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.NoGetProp
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.NoGetPropList
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.NoGetPropListForFastTransfer
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26112,
					Max = 26135,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26440,
					Max = 26447,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26456,
					Max = 26463,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26420,
					Max = 26431,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26464,
					Max = 26623,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.NoCopy
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.DoNotBumpChangeNumber
				}
			});
			return new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.Folder,
				Properties = properties,
				PropertiesByTagName = propertiesByTagName,
				PropIds = dictionary,
				PropIdRanges = propIdRanges
			};
		}

		private static Dictionary<string, StorePropInfo> BuildFolderPropertyDefinitionsByName()
		{
			Dictionary<string, StorePropInfo> dictionary = new Dictionary<string, StorePropInfo>(708, StringComparer.OrdinalIgnoreCase);
			dictionary["MessageClass"] = PropTag.Folder.MessageClass.PropInfo;
			dictionary["MessageSize"] = PropTag.Folder.MessageSize.PropInfo;
			dictionary["MessageSize32"] = PropTag.Folder.MessageSize32.PropInfo;
			dictionary["ParentEntryId"] = PropTag.Folder.ParentEntryId.PropInfo;
			dictionary["ParentEntryIdSvrEid"] = PropTag.Folder.ParentEntryIdSvrEid.PropInfo;
			dictionary["SentMailEntryId"] = PropTag.Folder.SentMailEntryId.PropInfo;
			dictionary["MessageDownloadTime"] = PropTag.Folder.MessageDownloadTime.PropInfo;
			dictionary["FolderInternetId"] = PropTag.Folder.FolderInternetId.PropInfo;
			dictionary["NTSecurityDescriptor"] = PropTag.Folder.NTSecurityDescriptor.PropInfo;
			dictionary["AclTableAndSecurityDescriptor"] = PropTag.Folder.AclTableAndSecurityDescriptor.PropInfo;
			dictionary["CreatorSID"] = PropTag.Folder.CreatorSID.PropInfo;
			dictionary["LastModifierSid"] = PropTag.Folder.LastModifierSid.PropInfo;
			dictionary["Catalog"] = PropTag.Folder.Catalog.PropInfo;
			dictionary["CISearchEnabled"] = PropTag.Folder.CISearchEnabled.PropInfo;
			dictionary["CINotificationEnabled"] = PropTag.Folder.CINotificationEnabled.PropInfo;
			dictionary["MaxIndices"] = PropTag.Folder.MaxIndices.PropInfo;
			dictionary["SourceFid"] = PropTag.Folder.SourceFid.PropInfo;
			dictionary["PFContactsGuid"] = PropTag.Folder.PFContactsGuid.PropInfo;
			dictionary["SubfolderCount"] = PropTag.Folder.SubfolderCount.PropInfo;
			dictionary["DeletedSubfolderCt"] = PropTag.Folder.DeletedSubfolderCt.PropInfo;
			dictionary["MaxCachedViews"] = PropTag.Folder.MaxCachedViews.PropInfo;
			dictionary["NTSecurityDescriptorAsXML"] = PropTag.Folder.NTSecurityDescriptorAsXML.PropInfo;
			dictionary["AdminNTSecurityDescriptorAsXML"] = PropTag.Folder.AdminNTSecurityDescriptorAsXML.PropInfo;
			dictionary["CreatorSidAsXML"] = PropTag.Folder.CreatorSidAsXML.PropInfo;
			dictionary["LastModifierSidAsXML"] = PropTag.Folder.LastModifierSidAsXML.PropInfo;
			dictionary["MergeMidsetDeleted"] = PropTag.Folder.MergeMidsetDeleted.PropInfo;
			dictionary["ReserveRangeOfIDs"] = PropTag.Folder.ReserveRangeOfIDs.PropInfo;
			dictionary["FreeBusyNTSD"] = PropTag.Folder.FreeBusyNTSD.PropInfo;
			dictionary["Access"] = PropTag.Folder.Access.PropInfo;
			dictionary["InstanceKey"] = PropTag.Folder.InstanceKey.PropInfo;
			dictionary["InstanceKeySvrEid"] = PropTag.Folder.InstanceKeySvrEid.PropInfo;
			dictionary["AccessLevel"] = PropTag.Folder.AccessLevel.PropInfo;
			dictionary["MappingSignature"] = PropTag.Folder.MappingSignature.PropInfo;
			dictionary["RecordKey"] = PropTag.Folder.RecordKey.PropInfo;
			dictionary["RecordKeySvrEid"] = PropTag.Folder.RecordKeySvrEid.PropInfo;
			dictionary["StoreRecordKey"] = PropTag.Folder.StoreRecordKey.PropInfo;
			dictionary["StoreEntryId"] = PropTag.Folder.StoreEntryId.PropInfo;
			dictionary["ObjectType"] = PropTag.Folder.ObjectType.PropInfo;
			dictionary["EntryId"] = PropTag.Folder.EntryId.PropInfo;
			dictionary["EntryIdSvrEid"] = PropTag.Folder.EntryIdSvrEid.PropInfo;
			dictionary["URLCompName"] = PropTag.Folder.URLCompName.PropInfo;
			dictionary["AttrHidden"] = PropTag.Folder.AttrHidden.PropInfo;
			dictionary["AttrSystem"] = PropTag.Folder.AttrSystem.PropInfo;
			dictionary["AttrReadOnly"] = PropTag.Folder.AttrReadOnly.PropInfo;
			dictionary["DisplayName"] = PropTag.Folder.DisplayName.PropInfo;
			dictionary["EmailAddress"] = PropTag.Folder.EmailAddress.PropInfo;
			dictionary["Comment"] = PropTag.Folder.Comment.PropInfo;
			dictionary["Depth"] = PropTag.Folder.Depth.PropInfo;
			dictionary["CreationTime"] = PropTag.Folder.CreationTime.PropInfo;
			dictionary["LastModificationTime"] = PropTag.Folder.LastModificationTime.PropInfo;
			dictionary["StoreSupportMask"] = PropTag.Folder.StoreSupportMask.PropInfo;
			dictionary["IPMWastebasketEntryId"] = PropTag.Folder.IPMWastebasketEntryId.PropInfo;
			dictionary["IPMCommonViewsEntryId"] = PropTag.Folder.IPMCommonViewsEntryId.PropInfo;
			dictionary["IPMConversationsEntryId"] = PropTag.Folder.IPMConversationsEntryId.PropInfo;
			dictionary["IPMAllItemsEntryId"] = PropTag.Folder.IPMAllItemsEntryId.PropInfo;
			dictionary["IPMSharingEntryId"] = PropTag.Folder.IPMSharingEntryId.PropInfo;
			dictionary["AdminDataEntryId"] = PropTag.Folder.AdminDataEntryId.PropInfo;
			dictionary["FolderType"] = PropTag.Folder.FolderType.PropInfo;
			dictionary["ContentCount"] = PropTag.Folder.ContentCount.PropInfo;
			dictionary["ContentCountInt64"] = PropTag.Folder.ContentCountInt64.PropInfo;
			dictionary["UnreadCount"] = PropTag.Folder.UnreadCount.PropInfo;
			dictionary["UnreadCountInt64"] = PropTag.Folder.UnreadCountInt64.PropInfo;
			dictionary["Subfolders"] = PropTag.Folder.Subfolders.PropInfo;
			dictionary["FolderStatus"] = PropTag.Folder.FolderStatus.PropInfo;
			dictionary["ContentsSortOrder"] = PropTag.Folder.ContentsSortOrder.PropInfo;
			dictionary["ContainerHierarchy"] = PropTag.Folder.ContainerHierarchy.PropInfo;
			dictionary["ContainerContents"] = PropTag.Folder.ContainerContents.PropInfo;
			dictionary["FolderAssociatedContents"] = PropTag.Folder.FolderAssociatedContents.PropInfo;
			dictionary["ContainerClass"] = PropTag.Folder.ContainerClass.PropInfo;
			dictionary["ContainerModifyVersion"] = PropTag.Folder.ContainerModifyVersion.PropInfo;
			dictionary["DefaultViewEntryId"] = PropTag.Folder.DefaultViewEntryId.PropInfo;
			dictionary["AssociatedContentCount"] = PropTag.Folder.AssociatedContentCount.PropInfo;
			dictionary["AssociatedContentCountInt64"] = PropTag.Folder.AssociatedContentCountInt64.PropInfo;
			dictionary["PackedNamedProps"] = PropTag.Folder.PackedNamedProps.PropInfo;
			dictionary["AllowAgeOut"] = PropTag.Folder.AllowAgeOut.PropInfo;
			dictionary["SearchFolderMsgCount"] = PropTag.Folder.SearchFolderMsgCount.PropInfo;
			dictionary["PartOfContentIndexing"] = PropTag.Folder.PartOfContentIndexing.PropInfo;
			dictionary["OwnerLogonUserConfigurationCache"] = PropTag.Folder.OwnerLogonUserConfigurationCache.PropInfo;
			dictionary["SearchFolderAgeOutTimeout"] = PropTag.Folder.SearchFolderAgeOutTimeout.PropInfo;
			dictionary["SearchFolderPopulationResult"] = PropTag.Folder.SearchFolderPopulationResult.PropInfo;
			dictionary["SearchFolderPopulationDiagnostics"] = PropTag.Folder.SearchFolderPopulationDiagnostics.PropInfo;
			dictionary["ConversationTopicHashEntries"] = PropTag.Folder.ConversationTopicHashEntries.PropInfo;
			dictionary["ContentAggregationFlags"] = PropTag.Folder.ContentAggregationFlags.PropInfo;
			dictionary["TransportRulesSnapshot"] = PropTag.Folder.TransportRulesSnapshot.PropInfo;
			dictionary["TransportRulesSnapshotId"] = PropTag.Folder.TransportRulesSnapshotId.PropInfo;
			dictionary["CurrentIPMWasteBasketContainerEntryId"] = PropTag.Folder.CurrentIPMWasteBasketContainerEntryId.PropInfo;
			dictionary["IPMAppointmentEntryId"] = PropTag.Folder.IPMAppointmentEntryId.PropInfo;
			dictionary["IPMContactEntryId"] = PropTag.Folder.IPMContactEntryId.PropInfo;
			dictionary["IPMJournalEntryId"] = PropTag.Folder.IPMJournalEntryId.PropInfo;
			dictionary["IPMNoteEntryId"] = PropTag.Folder.IPMNoteEntryId.PropInfo;
			dictionary["IPMTaskEntryId"] = PropTag.Folder.IPMTaskEntryId.PropInfo;
			dictionary["REMOnlineEntryId"] = PropTag.Folder.REMOnlineEntryId.PropInfo;
			dictionary["IPMOfflineEntryId"] = PropTag.Folder.IPMOfflineEntryId.PropInfo;
			dictionary["IPMDraftsEntryId"] = PropTag.Folder.IPMDraftsEntryId.PropInfo;
			dictionary["AdditionalRENEntryIds"] = PropTag.Folder.AdditionalRENEntryIds.PropInfo;
			dictionary["AdditionalRENEntryIdsExtended"] = PropTag.Folder.AdditionalRENEntryIdsExtended.PropInfo;
			dictionary["AdditionalRENEntryIdsExtendedMV"] = PropTag.Folder.AdditionalRENEntryIdsExtendedMV.PropInfo;
			dictionary["ExtendedFolderFlags"] = PropTag.Folder.ExtendedFolderFlags.PropInfo;
			dictionary["ContainerTimestamp"] = PropTag.Folder.ContainerTimestamp.PropInfo;
			dictionary["INetUnread"] = PropTag.Folder.INetUnread.PropInfo;
			dictionary["NetFolderFlags"] = PropTag.Folder.NetFolderFlags.PropInfo;
			dictionary["FolderWebViewInfo"] = PropTag.Folder.FolderWebViewInfo.PropInfo;
			dictionary["FolderWebViewInfoExtended"] = PropTag.Folder.FolderWebViewInfoExtended.PropInfo;
			dictionary["FolderViewFlags"] = PropTag.Folder.FolderViewFlags.PropInfo;
			dictionary["FreeBusyEntryIds"] = PropTag.Folder.FreeBusyEntryIds.PropInfo;
			dictionary["DefaultPostMsgClass"] = PropTag.Folder.DefaultPostMsgClass.PropInfo;
			dictionary["DefaultPostDisplayName"] = PropTag.Folder.DefaultPostDisplayName.PropInfo;
			dictionary["FolderViewList"] = PropTag.Folder.FolderViewList.PropInfo;
			dictionary["AgingPeriod"] = PropTag.Folder.AgingPeriod.PropInfo;
			dictionary["AgingGranularity"] = PropTag.Folder.AgingGranularity.PropInfo;
			dictionary["DefaultFoldersLocaleId"] = PropTag.Folder.DefaultFoldersLocaleId.PropInfo;
			dictionary["InternalAccess"] = PropTag.Folder.InternalAccess.PropInfo;
			dictionary["SyncEventSuppressGuid"] = PropTag.Folder.SyncEventSuppressGuid.PropInfo;
			dictionary["DisplayType"] = PropTag.Folder.DisplayType.PropInfo;
			dictionary["TestBlobProperty"] = PropTag.Folder.TestBlobProperty.PropInfo;
			dictionary["AdminSecurityDescriptor"] = PropTag.Folder.AdminSecurityDescriptor.PropInfo;
			dictionary["Win32NTSecurityDescriptor"] = PropTag.Folder.Win32NTSecurityDescriptor.PropInfo;
			dictionary["NonWin32ACL"] = PropTag.Folder.NonWin32ACL.PropInfo;
			dictionary["ItemLevelACL"] = PropTag.Folder.ItemLevelACL.PropInfo;
			dictionary["ICSGid"] = PropTag.Folder.ICSGid.PropInfo;
			dictionary["SystemFolderFlags"] = PropTag.Folder.SystemFolderFlags.PropInfo;
			dictionary["MaterializedRestrictionSearchRoot"] = PropTag.Folder.MaterializedRestrictionSearchRoot.PropInfo;
			dictionary["MailboxPartitionNumber"] = PropTag.Folder.MailboxPartitionNumber.PropInfo;
			dictionary["MailboxNumberInternal"] = PropTag.Folder.MailboxNumberInternal.PropInfo;
			dictionary["QueryCriteriaInternal"] = PropTag.Folder.QueryCriteriaInternal.PropInfo;
			dictionary["LastQuotaNotificationTime"] = PropTag.Folder.LastQuotaNotificationTime.PropInfo;
			dictionary["PropertyPromotionInProgressHiddenItems"] = PropTag.Folder.PropertyPromotionInProgressHiddenItems.PropInfo;
			dictionary["PropertyPromotionInProgressNormalItems"] = PropTag.Folder.PropertyPromotionInProgressNormalItems.PropInfo;
			dictionary["VirtualUnreadMessageCount"] = PropTag.Folder.VirtualUnreadMessageCount.PropInfo;
			dictionary["InternalChangeKey"] = PropTag.Folder.InternalChangeKey.PropInfo;
			dictionary["InternalSourceKey"] = PropTag.Folder.InternalSourceKey.PropInfo;
			dictionary["CorrelationId"] = PropTag.Folder.CorrelationId.PropInfo;
			dictionary["LastConflict"] = PropTag.Folder.LastConflict.PropInfo;
			dictionary["NTSDModificationTime"] = PropTag.Folder.NTSDModificationTime.PropInfo;
			dictionary["ACLDataChecksum"] = PropTag.Folder.ACLDataChecksum.PropInfo;
			dictionary["ACLData"] = PropTag.Folder.ACLData.PropInfo;
			dictionary["ACLTable"] = PropTag.Folder.ACLTable.PropInfo;
			dictionary["RulesData"] = PropTag.Folder.RulesData.PropInfo;
			dictionary["RulesTable"] = PropTag.Folder.RulesTable.PropInfo;
			dictionary["OofHistory"] = PropTag.Folder.OofHistory.PropInfo;
			dictionary["DesignInProgress"] = PropTag.Folder.DesignInProgress.PropInfo;
			dictionary["SecureOrigination"] = PropTag.Folder.SecureOrigination.PropInfo;
			dictionary["PublishInAddressBook"] = PropTag.Folder.PublishInAddressBook.PropInfo;
			dictionary["ResolveMethod"] = PropTag.Folder.ResolveMethod.PropInfo;
			dictionary["AddressBookDisplayName"] = PropTag.Folder.AddressBookDisplayName.PropInfo;
			dictionary["EFormsLocaleId"] = PropTag.Folder.EFormsLocaleId.PropInfo;
			dictionary["ExtendedACLData"] = PropTag.Folder.ExtendedACLData.PropInfo;
			dictionary["RulesSize"] = PropTag.Folder.RulesSize.PropInfo;
			dictionary["NewAttach"] = PropTag.Folder.NewAttach.PropInfo;
			dictionary["StartEmbed"] = PropTag.Folder.StartEmbed.PropInfo;
			dictionary["EndEmbed"] = PropTag.Folder.EndEmbed.PropInfo;
			dictionary["StartRecip"] = PropTag.Folder.StartRecip.PropInfo;
			dictionary["EndRecip"] = PropTag.Folder.EndRecip.PropInfo;
			dictionary["EndCcRecip"] = PropTag.Folder.EndCcRecip.PropInfo;
			dictionary["EndBccRecip"] = PropTag.Folder.EndBccRecip.PropInfo;
			dictionary["EndP1Recip"] = PropTag.Folder.EndP1Recip.PropInfo;
			dictionary["DNPrefix"] = PropTag.Folder.DNPrefix.PropInfo;
			dictionary["StartTopFolder"] = PropTag.Folder.StartTopFolder.PropInfo;
			dictionary["StartSubFolder"] = PropTag.Folder.StartSubFolder.PropInfo;
			dictionary["EndFolder"] = PropTag.Folder.EndFolder.PropInfo;
			dictionary["StartMessage"] = PropTag.Folder.StartMessage.PropInfo;
			dictionary["EndMessage"] = PropTag.Folder.EndMessage.PropInfo;
			dictionary["EndAttach"] = PropTag.Folder.EndAttach.PropInfo;
			dictionary["EcWarning"] = PropTag.Folder.EcWarning.PropInfo;
			dictionary["StartFAIMessage"] = PropTag.Folder.StartFAIMessage.PropInfo;
			dictionary["NewFXFolder"] = PropTag.Folder.NewFXFolder.PropInfo;
			dictionary["IncrSyncChange"] = PropTag.Folder.IncrSyncChange.PropInfo;
			dictionary["IncrSyncDelete"] = PropTag.Folder.IncrSyncDelete.PropInfo;
			dictionary["IncrSyncEnd"] = PropTag.Folder.IncrSyncEnd.PropInfo;
			dictionary["IncrSyncMessage"] = PropTag.Folder.IncrSyncMessage.PropInfo;
			dictionary["FastTransferDelProp"] = PropTag.Folder.FastTransferDelProp.PropInfo;
			dictionary["IdsetGiven"] = PropTag.Folder.IdsetGiven.PropInfo;
			dictionary["IdsetGivenInt32"] = PropTag.Folder.IdsetGivenInt32.PropInfo;
			dictionary["FastTransferErrorInfo"] = PropTag.Folder.FastTransferErrorInfo.PropInfo;
			dictionary["SoftDeletes"] = PropTag.Folder.SoftDeletes.PropInfo;
			dictionary["IdsetRead"] = PropTag.Folder.IdsetRead.PropInfo;
			dictionary["IdsetUnread"] = PropTag.Folder.IdsetUnread.PropInfo;
			dictionary["IncrSyncRead"] = PropTag.Folder.IncrSyncRead.PropInfo;
			dictionary["IncrSyncStateBegin"] = PropTag.Folder.IncrSyncStateBegin.PropInfo;
			dictionary["IncrSyncStateEnd"] = PropTag.Folder.IncrSyncStateEnd.PropInfo;
			dictionary["IncrSyncImailStream"] = PropTag.Folder.IncrSyncImailStream.PropInfo;
			dictionary["IncrSyncImailStreamContinue"] = PropTag.Folder.IncrSyncImailStreamContinue.PropInfo;
			dictionary["IncrSyncImailStreamCancel"] = PropTag.Folder.IncrSyncImailStreamCancel.PropInfo;
			dictionary["IncrSyncImailStream2Continue"] = PropTag.Folder.IncrSyncImailStream2Continue.PropInfo;
			dictionary["IncrSyncProgressMode"] = PropTag.Folder.IncrSyncProgressMode.PropInfo;
			dictionary["SyncProgressPerMsg"] = PropTag.Folder.SyncProgressPerMsg.PropInfo;
			dictionary["IncrSyncMsgPartial"] = PropTag.Folder.IncrSyncMsgPartial.PropInfo;
			dictionary["IncrSyncGroupInfo"] = PropTag.Folder.IncrSyncGroupInfo.PropInfo;
			dictionary["IncrSyncGroupId"] = PropTag.Folder.IncrSyncGroupId.PropInfo;
			dictionary["IncrSyncChangePartial"] = PropTag.Folder.IncrSyncChangePartial.PropInfo;
			dictionary["HierRev"] = PropTag.Folder.HierRev.PropInfo;
			dictionary["SourceKey"] = PropTag.Folder.SourceKey.PropInfo;
			dictionary["ParentSourceKey"] = PropTag.Folder.ParentSourceKey.PropInfo;
			dictionary["ChangeKey"] = PropTag.Folder.ChangeKey.PropInfo;
			dictionary["PredecessorChangeList"] = PropTag.Folder.PredecessorChangeList.PropInfo;
			dictionary["PreventMsgCreate"] = PropTag.Folder.PreventMsgCreate.PropInfo;
			dictionary["LISSD"] = PropTag.Folder.LISSD.PropInfo;
			dictionary["FavoritesDefaultName"] = PropTag.Folder.FavoritesDefaultName.PropInfo;
			dictionary["FolderChildCount"] = PropTag.Folder.FolderChildCount.PropInfo;
			dictionary["FolderChildCountInt64"] = PropTag.Folder.FolderChildCountInt64.PropInfo;
			dictionary["Rights"] = PropTag.Folder.Rights.PropInfo;
			dictionary["HasRules"] = PropTag.Folder.HasRules.PropInfo;
			dictionary["AddressBookEntryId"] = PropTag.Folder.AddressBookEntryId.PropInfo;
			dictionary["HierarchyChangeNumber"] = PropTag.Folder.HierarchyChangeNumber.PropInfo;
			dictionary["HasModeratorRules"] = PropTag.Folder.HasModeratorRules.PropInfo;
			dictionary["ModeratorRuleCount"] = PropTag.Folder.ModeratorRuleCount.PropInfo;
			dictionary["DeletedMsgCount"] = PropTag.Folder.DeletedMsgCount.PropInfo;
			dictionary["DeletedMsgCountInt64"] = PropTag.Folder.DeletedMsgCountInt64.PropInfo;
			dictionary["DeletedFolderCount"] = PropTag.Folder.DeletedFolderCount.PropInfo;
			dictionary["DeletedAssocMsgCount"] = PropTag.Folder.DeletedAssocMsgCount.PropInfo;
			dictionary["DeletedAssocMsgCountInt64"] = PropTag.Folder.DeletedAssocMsgCountInt64.PropInfo;
			dictionary["PromotedProperties"] = PropTag.Folder.PromotedProperties.PropInfo;
			dictionary["HiddenPromotedProperties"] = PropTag.Folder.HiddenPromotedProperties.PropInfo;
			dictionary["LinkedSiteAuthorityUrl"] = PropTag.Folder.LinkedSiteAuthorityUrl.PropInfo;
			dictionary["HasNamedProperties"] = PropTag.Folder.HasNamedProperties.PropInfo;
			dictionary["FidMid"] = PropTag.Folder.FidMid.PropInfo;
			dictionary["ICSChangeKey"] = PropTag.Folder.ICSChangeKey.PropInfo;
			dictionary["SetPropsCondition"] = PropTag.Folder.SetPropsCondition.PropInfo;
			dictionary["DeletedOn"] = PropTag.Folder.DeletedOn.PropInfo;
			dictionary["ReplicationStyle"] = PropTag.Folder.ReplicationStyle.PropInfo;
			dictionary["ReplicationTIB"] = PropTag.Folder.ReplicationTIB.PropInfo;
			dictionary["ReplicationMsgPriority"] = PropTag.Folder.ReplicationMsgPriority.PropInfo;
			dictionary["ReplicaList"] = PropTag.Folder.ReplicaList.PropInfo;
			dictionary["OverallAgeLimit"] = PropTag.Folder.OverallAgeLimit.PropInfo;
			dictionary["DeletedMessageSize"] = PropTag.Folder.DeletedMessageSize.PropInfo;
			dictionary["DeletedMessageSize32"] = PropTag.Folder.DeletedMessageSize32.PropInfo;
			dictionary["DeletedNormalMessageSize"] = PropTag.Folder.DeletedNormalMessageSize.PropInfo;
			dictionary["DeletedNormalMessageSize32"] = PropTag.Folder.DeletedNormalMessageSize32.PropInfo;
			dictionary["DeletedAssociatedMessageSize"] = PropTag.Folder.DeletedAssociatedMessageSize.PropInfo;
			dictionary["DeletedAssociatedMessageSize32"] = PropTag.Folder.DeletedAssociatedMessageSize32.PropInfo;
			dictionary["SecureInSite"] = PropTag.Folder.SecureInSite.PropInfo;
			dictionary["FolderFlags"] = PropTag.Folder.FolderFlags.PropInfo;
			dictionary["LastAccessTime"] = PropTag.Folder.LastAccessTime.PropInfo;
			dictionary["NormalMsgWithAttachCount"] = PropTag.Folder.NormalMsgWithAttachCount.PropInfo;
			dictionary["NormalMsgWithAttachCountInt64"] = PropTag.Folder.NormalMsgWithAttachCountInt64.PropInfo;
			dictionary["AssocMsgWithAttachCount"] = PropTag.Folder.AssocMsgWithAttachCount.PropInfo;
			dictionary["AssocMsgWithAttachCountInt64"] = PropTag.Folder.AssocMsgWithAttachCountInt64.PropInfo;
			dictionary["RecipientOnNormalMsgCount"] = PropTag.Folder.RecipientOnNormalMsgCount.PropInfo;
			dictionary["RecipientOnNormalMsgCountInt64"] = PropTag.Folder.RecipientOnNormalMsgCountInt64.PropInfo;
			dictionary["RecipientOnAssocMsgCount"] = PropTag.Folder.RecipientOnAssocMsgCount.PropInfo;
			dictionary["RecipientOnAssocMsgCountInt64"] = PropTag.Folder.RecipientOnAssocMsgCountInt64.PropInfo;
			dictionary["AttachOnNormalMsgCt"] = PropTag.Folder.AttachOnNormalMsgCt.PropInfo;
			dictionary["AttachOnNormalMsgCtInt64"] = PropTag.Folder.AttachOnNormalMsgCtInt64.PropInfo;
			dictionary["AttachOnAssocMsgCt"] = PropTag.Folder.AttachOnAssocMsgCt.PropInfo;
			dictionary["AttachOnAssocMsgCtInt64"] = PropTag.Folder.AttachOnAssocMsgCtInt64.PropInfo;
			dictionary["NormalMessageSize"] = PropTag.Folder.NormalMessageSize.PropInfo;
			dictionary["NormalMessageSize32"] = PropTag.Folder.NormalMessageSize32.PropInfo;
			dictionary["AssociatedMessageSize"] = PropTag.Folder.AssociatedMessageSize.PropInfo;
			dictionary["AssociatedMessageSize32"] = PropTag.Folder.AssociatedMessageSize32.PropInfo;
			dictionary["FolderPathName"] = PropTag.Folder.FolderPathName.PropInfo;
			dictionary["OwnerCount"] = PropTag.Folder.OwnerCount.PropInfo;
			dictionary["ContactCount"] = PropTag.Folder.ContactCount.PropInfo;
			dictionary["RetentionAgeLimit"] = PropTag.Folder.RetentionAgeLimit.PropInfo;
			dictionary["DisablePerUserRead"] = PropTag.Folder.DisablePerUserRead.PropInfo;
			dictionary["ServerDN"] = PropTag.Folder.ServerDN.PropInfo;
			dictionary["BackfillRanking"] = PropTag.Folder.BackfillRanking.PropInfo;
			dictionary["LastTransmissionTime"] = PropTag.Folder.LastTransmissionTime.PropInfo;
			dictionary["StatusSendTime"] = PropTag.Folder.StatusSendTime.PropInfo;
			dictionary["BackfillEntryCount"] = PropTag.Folder.BackfillEntryCount.PropInfo;
			dictionary["NextBroadcastTime"] = PropTag.Folder.NextBroadcastTime.PropInfo;
			dictionary["NextBackfillTime"] = PropTag.Folder.NextBackfillTime.PropInfo;
			dictionary["LastCNBroadcast"] = PropTag.Folder.LastCNBroadcast.PropInfo;
			dictionary["LastShortCNBroadcast"] = PropTag.Folder.LastShortCNBroadcast.PropInfo;
			dictionary["AverageTransmissionTime"] = PropTag.Folder.AverageTransmissionTime.PropInfo;
			dictionary["ReplicationStatus"] = PropTag.Folder.ReplicationStatus.PropInfo;
			dictionary["LastDataReceivalTime"] = PropTag.Folder.LastDataReceivalTime.PropInfo;
			dictionary["AdminDisplayName"] = PropTag.Folder.AdminDisplayName.PropInfo;
			dictionary["URLName"] = PropTag.Folder.URLName.PropInfo;
			dictionary["LocalCommitTime"] = PropTag.Folder.LocalCommitTime.PropInfo;
			dictionary["LocalCommitTimeMax"] = PropTag.Folder.LocalCommitTimeMax.PropInfo;
			dictionary["DeletedCountTotal"] = PropTag.Folder.DeletedCountTotal.PropInfo;
			dictionary["DeletedCountTotalInt64"] = PropTag.Folder.DeletedCountTotalInt64.PropInfo;
			dictionary["ScopeFIDs"] = PropTag.Folder.ScopeFIDs.PropInfo;
			dictionary["PFAdminDescription"] = PropTag.Folder.PFAdminDescription.PropInfo;
			dictionary["PFProxy"] = PropTag.Folder.PFProxy.PropInfo;
			dictionary["PFPlatinumHomeMdb"] = PropTag.Folder.PFPlatinumHomeMdb.PropInfo;
			dictionary["PFProxyRequired"] = PropTag.Folder.PFProxyRequired.PropInfo;
			dictionary["PFOverHardQuotaLimit"] = PropTag.Folder.PFOverHardQuotaLimit.PropInfo;
			dictionary["PFMsgSizeLimit"] = PropTag.Folder.PFMsgSizeLimit.PropInfo;
			dictionary["PFDisallowMdbWideExpiry"] = PropTag.Folder.PFDisallowMdbWideExpiry.PropInfo;
			dictionary["FolderAdminFlags"] = PropTag.Folder.FolderAdminFlags.PropInfo;
			dictionary["ProvisionedFID"] = PropTag.Folder.ProvisionedFID.PropInfo;
			dictionary["ELCFolderSize"] = PropTag.Folder.ELCFolderSize.PropInfo;
			dictionary["ELCFolderQuota"] = PropTag.Folder.ELCFolderQuota.PropInfo;
			dictionary["ELCPolicyId"] = PropTag.Folder.ELCPolicyId.PropInfo;
			dictionary["ELCPolicyComment"] = PropTag.Folder.ELCPolicyComment.PropInfo;
			dictionary["PropertyGroupMappingId"] = PropTag.Folder.PropertyGroupMappingId.PropInfo;
			dictionary["Fid"] = PropTag.Folder.Fid.PropInfo;
			dictionary["FidBin"] = PropTag.Folder.FidBin.PropInfo;
			dictionary["ParentFid"] = PropTag.Folder.ParentFid.PropInfo;
			dictionary["ParentFidBin"] = PropTag.Folder.ParentFidBin.PropInfo;
			dictionary["ArticleNumNext"] = PropTag.Folder.ArticleNumNext.PropInfo;
			dictionary["ImapLastArticleId"] = PropTag.Folder.ImapLastArticleId.PropInfo;
			dictionary["CnExport"] = PropTag.Folder.CnExport.PropInfo;
			dictionary["PclExport"] = PropTag.Folder.PclExport.PropInfo;
			dictionary["CnMvExport"] = PropTag.Folder.CnMvExport.PropInfo;
			dictionary["MidsetDeletedExport"] = PropTag.Folder.MidsetDeletedExport.PropInfo;
			dictionary["ArticleNumMic"] = PropTag.Folder.ArticleNumMic.PropInfo;
			dictionary["ArticleNumMost"] = PropTag.Folder.ArticleNumMost.PropInfo;
			dictionary["RulesSync"] = PropTag.Folder.RulesSync.PropInfo;
			dictionary["ReplicaListR"] = PropTag.Folder.ReplicaListR.PropInfo;
			dictionary["ReplicaListRC"] = PropTag.Folder.ReplicaListRC.PropInfo;
			dictionary["ReplicaListRBUG"] = PropTag.Folder.ReplicaListRBUG.PropInfo;
			dictionary["RootFid"] = PropTag.Folder.RootFid.PropInfo;
			dictionary["SoftDeleted"] = PropTag.Folder.SoftDeleted.PropInfo;
			dictionary["QuotaStyle"] = PropTag.Folder.QuotaStyle.PropInfo;
			dictionary["StorageQuota"] = PropTag.Folder.StorageQuota.PropInfo;
			dictionary["FolderPropTagArray"] = PropTag.Folder.FolderPropTagArray.PropInfo;
			dictionary["MsgFolderPropTagArray"] = PropTag.Folder.MsgFolderPropTagArray.PropInfo;
			dictionary["SetReceiveCount"] = PropTag.Folder.SetReceiveCount.PropInfo;
			dictionary["SubmittedCount"] = PropTag.Folder.SubmittedCount.PropInfo;
			dictionary["CreatorToken"] = PropTag.Folder.CreatorToken.PropInfo;
			dictionary["SearchState"] = PropTag.Folder.SearchState.PropInfo;
			dictionary["SearchRestriction"] = PropTag.Folder.SearchRestriction.PropInfo;
			dictionary["SearchFIDs"] = PropTag.Folder.SearchFIDs.PropInfo;
			dictionary["RecursiveSearchFIDs"] = PropTag.Folder.RecursiveSearchFIDs.PropInfo;
			dictionary["SearchBacklinks"] = PropTag.Folder.SearchBacklinks.PropInfo;
			dictionary["CategFIDs"] = PropTag.Folder.CategFIDs.PropInfo;
			dictionary["FolderCDN"] = PropTag.Folder.FolderCDN.PropInfo;
			dictionary["MidSegmentStart"] = PropTag.Folder.MidSegmentStart.PropInfo;
			dictionary["MidsetDeleted"] = PropTag.Folder.MidsetDeleted.PropInfo;
			dictionary["MidsetExpired"] = PropTag.Folder.MidsetExpired.PropInfo;
			dictionary["CnsetIn"] = PropTag.Folder.CnsetIn.PropInfo;
			dictionary["CnsetSeen"] = PropTag.Folder.CnsetSeen.PropInfo;
			dictionary["MidsetTombstones"] = PropTag.Folder.MidsetTombstones.PropInfo;
			dictionary["GWFolder"] = PropTag.Folder.GWFolder.PropInfo;
			dictionary["IPMFolder"] = PropTag.Folder.IPMFolder.PropInfo;
			dictionary["PublicFolderPath"] = PropTag.Folder.PublicFolderPath.PropInfo;
			dictionary["MidSegmentIndex"] = PropTag.Folder.MidSegmentIndex.PropInfo;
			dictionary["MidSegmentSize"] = PropTag.Folder.MidSegmentSize.PropInfo;
			dictionary["CnSegmentStart"] = PropTag.Folder.CnSegmentStart.PropInfo;
			dictionary["CnSegmentIndex"] = PropTag.Folder.CnSegmentIndex.PropInfo;
			dictionary["CnSegmentSize"] = PropTag.Folder.CnSegmentSize.PropInfo;
			dictionary["ChangeNumber"] = PropTag.Folder.ChangeNumber.PropInfo;
			dictionary["ChangeNumberBin"] = PropTag.Folder.ChangeNumberBin.PropInfo;
			dictionary["PCL"] = PropTag.Folder.PCL.PropInfo;
			dictionary["CnMv"] = PropTag.Folder.CnMv.PropInfo;
			dictionary["FolderTreeRootFID"] = PropTag.Folder.FolderTreeRootFID.PropInfo;
			dictionary["SourceEntryId"] = PropTag.Folder.SourceEntryId.PropInfo;
			dictionary["AnonymousRights"] = PropTag.Folder.AnonymousRights.PropInfo;
			dictionary["SearchGUID"] = PropTag.Folder.SearchGUID.PropInfo;
			dictionary["CnsetRead"] = PropTag.Folder.CnsetRead.PropInfo;
			dictionary["CnsetSeenFAI"] = PropTag.Folder.CnsetSeenFAI.PropInfo;
			dictionary["IdSetDeleted"] = PropTag.Folder.IdSetDeleted.PropInfo;
			dictionary["ModifiedCount"] = PropTag.Folder.ModifiedCount.PropInfo;
			dictionary["DeletedState"] = PropTag.Folder.DeletedState.PropInfo;
			dictionary["ptagMsgHeaderTableFID"] = PropTag.Folder.ptagMsgHeaderTableFID.PropInfo;
			dictionary["MailboxNum"] = PropTag.Folder.MailboxNum.PropInfo;
			dictionary["LastUserAccessTime"] = PropTag.Folder.LastUserAccessTime.PropInfo;
			dictionary["LastUserModificationTime"] = PropTag.Folder.LastUserModificationTime.PropInfo;
			dictionary["SyncCustomState"] = PropTag.Folder.SyncCustomState.PropInfo;
			dictionary["SyncFolderChangeKey"] = PropTag.Folder.SyncFolderChangeKey.PropInfo;
			dictionary["SyncFolderLastModificationTime"] = PropTag.Folder.SyncFolderLastModificationTime.PropInfo;
			dictionary["ptagSyncState"] = PropTag.Folder.ptagSyncState.PropInfo;
			return dictionary;
		}

		private static Dictionary<uint, StorePropInfo> BuildFolderPropertyDefinitionsByTag()
		{
			Dictionary<uint, StorePropInfo> dictionary = new Dictionary<uint, StorePropInfo>(708);
			dictionary[1703967U] = PropTag.Folder.MessageClass.PropInfo;
			dictionary[235405332U] = PropTag.Folder.MessageSize.PropInfo;
			dictionary[235405315U] = PropTag.Folder.MessageSize32.PropInfo;
			dictionary[235471106U] = PropTag.Folder.ParentEntryId.PropInfo;
			dictionary[235471099U] = PropTag.Folder.ParentEntryIdSvrEid.PropInfo;
			dictionary[235536642U] = PropTag.Folder.SentMailEntryId.PropInfo;
			dictionary[236453891U] = PropTag.Folder.MessageDownloadTime.PropInfo;
			dictionary[237174787U] = PropTag.Folder.FolderInternetId.PropInfo;
			dictionary[237437186U] = PropTag.Folder.NTSecurityDescriptor.PropInfo;
			dictionary[239010050U] = PropTag.Folder.AclTableAndSecurityDescriptor.PropInfo;
			dictionary[240648450U] = PropTag.Folder.CreatorSID.PropInfo;
			dictionary[240713986U] = PropTag.Folder.LastModifierSid.PropInfo;
			dictionary[240845058U] = PropTag.Folder.Catalog.PropInfo;
			dictionary[240910347U] = PropTag.Folder.CISearchEnabled.PropInfo;
			dictionary[240975883U] = PropTag.Folder.CINotificationEnabled.PropInfo;
			dictionary[241041411U] = PropTag.Folder.MaxIndices.PropInfo;
			dictionary[241106964U] = PropTag.Folder.SourceFid.PropInfo;
			dictionary[241172738U] = PropTag.Folder.PFContactsGuid.PropInfo;
			dictionary[241369091U] = PropTag.Folder.SubfolderCount.PropInfo;
			dictionary[241434627U] = PropTag.Folder.DeletedSubfolderCt.PropInfo;
			dictionary[241696771U] = PropTag.Folder.MaxCachedViews.PropInfo;
			dictionary[241827871U] = PropTag.Folder.NTSecurityDescriptorAsXML.PropInfo;
			dictionary[241893407U] = PropTag.Folder.AdminNTSecurityDescriptorAsXML.PropInfo;
			dictionary[241958943U] = PropTag.Folder.CreatorSidAsXML.PropInfo;
			dictionary[242024479U] = PropTag.Folder.LastModifierSidAsXML.PropInfo;
			dictionary[242876674U] = PropTag.Folder.MergeMidsetDeleted.PropInfo;
			dictionary[242942210U] = PropTag.Folder.ReserveRangeOfIDs.PropInfo;
			dictionary[251658498U] = PropTag.Folder.FreeBusyNTSD.PropInfo;
			dictionary[267649027U] = PropTag.Folder.Access.PropInfo;
			dictionary[267780354U] = PropTag.Folder.InstanceKey.PropInfo;
			dictionary[267780347U] = PropTag.Folder.InstanceKeySvrEid.PropInfo;
			dictionary[267845635U] = PropTag.Folder.AccessLevel.PropInfo;
			dictionary[267911426U] = PropTag.Folder.MappingSignature.PropInfo;
			dictionary[267976962U] = PropTag.Folder.RecordKey.PropInfo;
			dictionary[267976955U] = PropTag.Folder.RecordKeySvrEid.PropInfo;
			dictionary[268042498U] = PropTag.Folder.StoreRecordKey.PropInfo;
			dictionary[268108034U] = PropTag.Folder.StoreEntryId.PropInfo;
			dictionary[268304387U] = PropTag.Folder.ObjectType.PropInfo;
			dictionary[268370178U] = PropTag.Folder.EntryId.PropInfo;
			dictionary[268370171U] = PropTag.Folder.EntryIdSvrEid.PropInfo;
			dictionary[284360735U] = PropTag.Folder.URLCompName.PropInfo;
			dictionary[284426251U] = PropTag.Folder.AttrHidden.PropInfo;
			dictionary[284491787U] = PropTag.Folder.AttrSystem.PropInfo;
			dictionary[284557323U] = PropTag.Folder.AttrReadOnly.PropInfo;
			dictionary[805371935U] = PropTag.Folder.DisplayName.PropInfo;
			dictionary[805503007U] = PropTag.Folder.EmailAddress.PropInfo;
			dictionary[805568543U] = PropTag.Folder.Comment.PropInfo;
			dictionary[805634051U] = PropTag.Folder.Depth.PropInfo;
			dictionary[805765184U] = PropTag.Folder.CreationTime.PropInfo;
			dictionary[805830720U] = PropTag.Folder.LastModificationTime.PropInfo;
			dictionary[873267203U] = PropTag.Folder.StoreSupportMask.PropInfo;
			dictionary[904069378U] = PropTag.Folder.IPMWastebasketEntryId.PropInfo;
			dictionary[904265986U] = PropTag.Folder.IPMCommonViewsEntryId.PropInfo;
			dictionary[904659202U] = PropTag.Folder.IPMConversationsEntryId.PropInfo;
			dictionary[904790274U] = PropTag.Folder.IPMAllItemsEntryId.PropInfo;
			dictionary[904855810U] = PropTag.Folder.IPMSharingEntryId.PropInfo;
			dictionary[905773314U] = PropTag.Folder.AdminDataEntryId.PropInfo;
			dictionary[906035203U] = PropTag.Folder.FolderType.PropInfo;
			dictionary[906100739U] = PropTag.Folder.ContentCount.PropInfo;
			dictionary[906100756U] = PropTag.Folder.ContentCountInt64.PropInfo;
			dictionary[906166275U] = PropTag.Folder.UnreadCount.PropInfo;
			dictionary[906166292U] = PropTag.Folder.UnreadCountInt64.PropInfo;
			dictionary[906625035U] = PropTag.Folder.Subfolders.PropInfo;
			dictionary[906690563U] = PropTag.Folder.FolderStatus.PropInfo;
			dictionary[906825731U] = PropTag.Folder.ContentsSortOrder.PropInfo;
			dictionary[906887181U] = PropTag.Folder.ContainerHierarchy.PropInfo;
			dictionary[906952717U] = PropTag.Folder.ContainerContents.PropInfo;
			dictionary[907018253U] = PropTag.Folder.FolderAssociatedContents.PropInfo;
			dictionary[907214879U] = PropTag.Folder.ContainerClass.PropInfo;
			dictionary[907280404U] = PropTag.Folder.ContainerModifyVersion.PropInfo;
			dictionary[907411714U] = PropTag.Folder.DefaultViewEntryId.PropInfo;
			dictionary[907476995U] = PropTag.Folder.AssociatedContentCount.PropInfo;
			dictionary[907477012U] = PropTag.Folder.AssociatedContentCountInt64.PropInfo;
			dictionary[907804930U] = PropTag.Folder.PackedNamedProps.PropInfo;
			dictionary[908001291U] = PropTag.Folder.AllowAgeOut.PropInfo;
			dictionary[910426115U] = PropTag.Folder.SearchFolderMsgCount.PropInfo;
			dictionary[910491659U] = PropTag.Folder.PartOfContentIndexing.PropInfo;
			dictionary[910557442U] = PropTag.Folder.OwnerLogonUserConfigurationCache.PropInfo;
			dictionary[910622723U] = PropTag.Folder.SearchFolderAgeOutTimeout.PropInfo;
			dictionary[910688259U] = PropTag.Folder.SearchFolderPopulationResult.PropInfo;
			dictionary[910754050U] = PropTag.Folder.SearchFolderPopulationDiagnostics.PropInfo;
			dictionary[912261378U] = PropTag.Folder.ConversationTopicHashEntries.PropInfo;
			dictionary[915341315U] = PropTag.Folder.ContentAggregationFlags.PropInfo;
			dictionary[915407106U] = PropTag.Folder.TransportRulesSnapshot.PropInfo;
			dictionary[915472456U] = PropTag.Folder.TransportRulesSnapshotId.PropInfo;
			dictionary[919535874U] = PropTag.Folder.CurrentIPMWasteBasketContainerEntryId.PropInfo;
			dictionary[919601410U] = PropTag.Folder.IPMAppointmentEntryId.PropInfo;
			dictionary[919666946U] = PropTag.Folder.IPMContactEntryId.PropInfo;
			dictionary[919732482U] = PropTag.Folder.IPMJournalEntryId.PropInfo;
			dictionary[919798018U] = PropTag.Folder.IPMNoteEntryId.PropInfo;
			dictionary[919863554U] = PropTag.Folder.IPMTaskEntryId.PropInfo;
			dictionary[919929090U] = PropTag.Folder.REMOnlineEntryId.PropInfo;
			dictionary[919994626U] = PropTag.Folder.IPMOfflineEntryId.PropInfo;
			dictionary[920060162U] = PropTag.Folder.IPMDraftsEntryId.PropInfo;
			dictionary[920129794U] = PropTag.Folder.AdditionalRENEntryIds.PropInfo;
			dictionary[920191234U] = PropTag.Folder.AdditionalRENEntryIdsExtended.PropInfo;
			dictionary[920195330U] = PropTag.Folder.AdditionalRENEntryIdsExtendedMV.PropInfo;
			dictionary[920256770U] = PropTag.Folder.ExtendedFolderFlags.PropInfo;
			dictionary[920322112U] = PropTag.Folder.ContainerTimestamp.PropInfo;
			dictionary[920453123U] = PropTag.Folder.INetUnread.PropInfo;
			dictionary[920518659U] = PropTag.Folder.NetFolderFlags.PropInfo;
			dictionary[920584450U] = PropTag.Folder.FolderWebViewInfo.PropInfo;
			dictionary[920649986U] = PropTag.Folder.FolderWebViewInfoExtended.PropInfo;
			dictionary[920715267U] = PropTag.Folder.FolderViewFlags.PropInfo;
			dictionary[920916226U] = PropTag.Folder.FreeBusyEntryIds.PropInfo;
			dictionary[920977439U] = PropTag.Folder.DefaultPostMsgClass.PropInfo;
			dictionary[921042975U] = PropTag.Folder.DefaultPostDisplayName.PropInfo;
			dictionary[921370882U] = PropTag.Folder.FolderViewList.PropInfo;
			dictionary[921436163U] = PropTag.Folder.AgingPeriod.PropInfo;
			dictionary[921567235U] = PropTag.Folder.AgingGranularity.PropInfo;
			dictionary[921698307U] = PropTag.Folder.DefaultFoldersLocaleId.PropInfo;
			dictionary[921763851U] = PropTag.Folder.InternalAccess.PropInfo;
			dictionary[947912962U] = PropTag.Folder.SyncEventSuppressGuid.PropInfo;
			dictionary[956301315U] = PropTag.Folder.DisplayType.PropInfo;
			dictionary[1023410196U] = PropTag.Folder.TestBlobProperty.PropInfo;
			dictionary[1025573122U] = PropTag.Folder.AdminSecurityDescriptor.PropInfo;
			dictionary[1025638658U] = PropTag.Folder.Win32NTSecurityDescriptor.PropInfo;
			dictionary[1025703947U] = PropTag.Folder.NonWin32ACL.PropInfo;
			dictionary[1025769483U] = PropTag.Folder.ItemLevelACL.PropInfo;
			dictionary[1026425090U] = PropTag.Folder.ICSGid.PropInfo;
			dictionary[1026490371U] = PropTag.Folder.SystemFolderFlags.PropInfo;
			dictionary[1033634050U] = PropTag.Folder.MaterializedRestrictionSearchRoot.PropInfo;
			dictionary[1033830403U] = PropTag.Folder.MailboxPartitionNumber.PropInfo;
			dictionary[1033895939U] = PropTag.Folder.MailboxNumberInternal.PropInfo;
			dictionary[1033961730U] = PropTag.Folder.QueryCriteriaInternal.PropInfo;
			dictionary[1034027072U] = PropTag.Folder.LastQuotaNotificationTime.PropInfo;
			dictionary[1034092555U] = PropTag.Folder.PropertyPromotionInProgressHiddenItems.PropInfo;
			dictionary[1034158091U] = PropTag.Folder.PropertyPromotionInProgressNormalItems.PropInfo;
			dictionary[1034616852U] = PropTag.Folder.VirtualUnreadMessageCount.PropInfo;
			dictionary[1035862274U] = PropTag.Folder.InternalChangeKey.PropInfo;
			dictionary[1035927810U] = PropTag.Folder.InternalSourceKey.PropInfo;
			dictionary[1037107272U] = PropTag.Folder.CorrelationId.PropInfo;
			dictionary[1070137602U] = PropTag.Folder.LastConflict.PropInfo;
			dictionary[1070989376U] = PropTag.Folder.NTSDModificationTime.PropInfo;
			dictionary[1071054851U] = PropTag.Folder.ACLDataChecksum.PropInfo;
			dictionary[1071644930U] = PropTag.Folder.ACLData.PropInfo;
			dictionary[1071644685U] = PropTag.Folder.ACLTable.PropInfo;
			dictionary[1071710466U] = PropTag.Folder.RulesData.PropInfo;
			dictionary[1071710221U] = PropTag.Folder.RulesTable.PropInfo;
			dictionary[1071841538U] = PropTag.Folder.OofHistory.PropInfo;
			dictionary[1071906827U] = PropTag.Folder.DesignInProgress.PropInfo;
			dictionary[1071972363U] = PropTag.Folder.SecureOrigination.PropInfo;
			dictionary[1072037899U] = PropTag.Folder.PublishInAddressBook.PropInfo;
			dictionary[1072103427U] = PropTag.Folder.ResolveMethod.PropInfo;
			dictionary[1072168991U] = PropTag.Folder.AddressBookDisplayName.PropInfo;
			dictionary[1072234499U] = PropTag.Folder.EFormsLocaleId.PropInfo;
			dictionary[1073611010U] = PropTag.Folder.ExtendedACLData.PropInfo;
			dictionary[1073676291U] = PropTag.Folder.RulesSize.PropInfo;
			dictionary[1073741827U] = PropTag.Folder.NewAttach.PropInfo;
			dictionary[1073807363U] = PropTag.Folder.StartEmbed.PropInfo;
			dictionary[1073872899U] = PropTag.Folder.EndEmbed.PropInfo;
			dictionary[1073938435U] = PropTag.Folder.StartRecip.PropInfo;
			dictionary[1074003971U] = PropTag.Folder.EndRecip.PropInfo;
			dictionary[1074069507U] = PropTag.Folder.EndCcRecip.PropInfo;
			dictionary[1074135043U] = PropTag.Folder.EndBccRecip.PropInfo;
			dictionary[1074200579U] = PropTag.Folder.EndP1Recip.PropInfo;
			dictionary[1074266143U] = PropTag.Folder.DNPrefix.PropInfo;
			dictionary[1074331651U] = PropTag.Folder.StartTopFolder.PropInfo;
			dictionary[1074397187U] = PropTag.Folder.StartSubFolder.PropInfo;
			dictionary[1074462723U] = PropTag.Folder.EndFolder.PropInfo;
			dictionary[1074528259U] = PropTag.Folder.StartMessage.PropInfo;
			dictionary[1074593795U] = PropTag.Folder.EndMessage.PropInfo;
			dictionary[1074659331U] = PropTag.Folder.EndAttach.PropInfo;
			dictionary[1074724867U] = PropTag.Folder.EcWarning.PropInfo;
			dictionary[1074790403U] = PropTag.Folder.StartFAIMessage.PropInfo;
			dictionary[1074856194U] = PropTag.Folder.NewFXFolder.PropInfo;
			dictionary[1074921475U] = PropTag.Folder.IncrSyncChange.PropInfo;
			dictionary[1074987011U] = PropTag.Folder.IncrSyncDelete.PropInfo;
			dictionary[1075052547U] = PropTag.Folder.IncrSyncEnd.PropInfo;
			dictionary[1075118083U] = PropTag.Folder.IncrSyncMessage.PropInfo;
			dictionary[1075183619U] = PropTag.Folder.FastTransferDelProp.PropInfo;
			dictionary[1075249410U] = PropTag.Folder.IdsetGiven.PropInfo;
			dictionary[1075249155U] = PropTag.Folder.IdsetGivenInt32.PropInfo;
			dictionary[1075314691U] = PropTag.Folder.FastTransferErrorInfo.PropInfo;
			dictionary[1075904770U] = PropTag.Folder.SoftDeletes.PropInfo;
			dictionary[1076691202U] = PropTag.Folder.IdsetRead.PropInfo;
			dictionary[1076756738U] = PropTag.Folder.IdsetUnread.PropInfo;
			dictionary[1076822019U] = PropTag.Folder.IncrSyncRead.PropInfo;
			dictionary[1077542915U] = PropTag.Folder.IncrSyncStateBegin.PropInfo;
			dictionary[1077608451U] = PropTag.Folder.IncrSyncStateEnd.PropInfo;
			dictionary[1077673987U] = PropTag.Folder.IncrSyncImailStream.PropInfo;
			dictionary[1080426499U] = PropTag.Folder.IncrSyncImailStreamContinue.PropInfo;
			dictionary[1080492035U] = PropTag.Folder.IncrSyncImailStreamCancel.PropInfo;
			dictionary[1081147395U] = PropTag.Folder.IncrSyncImailStream2Continue.PropInfo;
			dictionary[1081344011U] = PropTag.Folder.IncrSyncProgressMode.PropInfo;
			dictionary[1081409547U] = PropTag.Folder.SyncProgressPerMsg.PropInfo;
			dictionary[1081737219U] = PropTag.Folder.IncrSyncMsgPartial.PropInfo;
			dictionary[1081802755U] = PropTag.Folder.IncrSyncGroupInfo.PropInfo;
			dictionary[1081868291U] = PropTag.Folder.IncrSyncGroupId.PropInfo;
			dictionary[1081933827U] = PropTag.Folder.IncrSyncChangePartial.PropInfo;
			dictionary[1082261568U] = PropTag.Folder.HierRev.PropInfo;
			dictionary[1709179138U] = PropTag.Folder.SourceKey.PropInfo;
			dictionary[1709244674U] = PropTag.Folder.ParentSourceKey.PropInfo;
			dictionary[1709310210U] = PropTag.Folder.ChangeKey.PropInfo;
			dictionary[1709375746U] = PropTag.Folder.PredecessorChangeList.PropInfo;
			dictionary[1710489611U] = PropTag.Folder.PreventMsgCreate.PropInfo;
			dictionary[1710817538U] = PropTag.Folder.LISSD.PropInfo;
			dictionary[1714749471U] = PropTag.Folder.FavoritesDefaultName.PropInfo;
			dictionary[1714946051U] = PropTag.Folder.FolderChildCount.PropInfo;
			dictionary[1714946068U] = PropTag.Folder.FolderChildCountInt64.PropInfo;
			dictionary[1715011587U] = PropTag.Folder.Rights.PropInfo;
			dictionary[1715077131U] = PropTag.Folder.HasRules.PropInfo;
			dictionary[1715142914U] = PropTag.Folder.AddressBookEntryId.PropInfo;
			dictionary[1715339267U] = PropTag.Folder.HierarchyChangeNumber.PropInfo;
			dictionary[1715404811U] = PropTag.Folder.HasModeratorRules.PropInfo;
			dictionary[1715404803U] = PropTag.Folder.ModeratorRuleCount.PropInfo;
			dictionary[1715470339U] = PropTag.Folder.DeletedMsgCount.PropInfo;
			dictionary[1715470356U] = PropTag.Folder.DeletedMsgCountInt64.PropInfo;
			dictionary[1715535875U] = PropTag.Folder.DeletedFolderCount.PropInfo;
			dictionary[1715666947U] = PropTag.Folder.DeletedAssocMsgCount.PropInfo;
			dictionary[1715666964U] = PropTag.Folder.DeletedAssocMsgCountInt64.PropInfo;
			dictionary[1715798274U] = PropTag.Folder.PromotedProperties.PropInfo;
			dictionary[1715863810U] = PropTag.Folder.HiddenPromotedProperties.PropInfo;
			dictionary[1715929119U] = PropTag.Folder.LinkedSiteAuthorityUrl.PropInfo;
			dictionary[1716125707U] = PropTag.Folder.HasNamedProperties.PropInfo;
			dictionary[1716257026U] = PropTag.Folder.FidMid.PropInfo;
			dictionary[1716846850U] = PropTag.Folder.ICSChangeKey.PropInfo;
			dictionary[1716977922U] = PropTag.Folder.SetPropsCondition.PropInfo;
			dictionary[1720647744U] = PropTag.Folder.DeletedOn.PropInfo;
			dictionary[1720713219U] = PropTag.Folder.ReplicationStyle.PropInfo;
			dictionary[1720779010U] = PropTag.Folder.ReplicationTIB.PropInfo;
			dictionary[1720844291U] = PropTag.Folder.ReplicationMsgPriority.PropInfo;
			dictionary[1721237762U] = PropTag.Folder.ReplicaList.PropInfo;
			dictionary[1721303043U] = PropTag.Folder.OverallAgeLimit.PropInfo;
			dictionary[1721434132U] = PropTag.Folder.DeletedMessageSize.PropInfo;
			dictionary[1721434115U] = PropTag.Folder.DeletedMessageSize32.PropInfo;
			dictionary[1721499668U] = PropTag.Folder.DeletedNormalMessageSize.PropInfo;
			dictionary[1721499651U] = PropTag.Folder.DeletedNormalMessageSize32.PropInfo;
			dictionary[1721565204U] = PropTag.Folder.DeletedAssociatedMessageSize.PropInfo;
			dictionary[1721565187U] = PropTag.Folder.DeletedAssociatedMessageSize32.PropInfo;
			dictionary[1721630731U] = PropTag.Folder.SecureInSite.PropInfo;
			dictionary[1722286083U] = PropTag.Folder.FolderFlags.PropInfo;
			dictionary[1722351680U] = PropTag.Folder.LastAccessTime.PropInfo;
			dictionary[1722613763U] = PropTag.Folder.NormalMsgWithAttachCount.PropInfo;
			dictionary[1722613780U] = PropTag.Folder.NormalMsgWithAttachCountInt64.PropInfo;
			dictionary[1722679299U] = PropTag.Folder.AssocMsgWithAttachCount.PropInfo;
			dictionary[1722679316U] = PropTag.Folder.AssocMsgWithAttachCountInt64.PropInfo;
			dictionary[1722744835U] = PropTag.Folder.RecipientOnNormalMsgCount.PropInfo;
			dictionary[1722744852U] = PropTag.Folder.RecipientOnNormalMsgCountInt64.PropInfo;
			dictionary[1722810371U] = PropTag.Folder.RecipientOnAssocMsgCount.PropInfo;
			dictionary[1722810388U] = PropTag.Folder.RecipientOnAssocMsgCountInt64.PropInfo;
			dictionary[1722875907U] = PropTag.Folder.AttachOnNormalMsgCt.PropInfo;
			dictionary[1722875924U] = PropTag.Folder.AttachOnNormalMsgCtInt64.PropInfo;
			dictionary[1722941443U] = PropTag.Folder.AttachOnAssocMsgCt.PropInfo;
			dictionary[1722941460U] = PropTag.Folder.AttachOnAssocMsgCtInt64.PropInfo;
			dictionary[1723006996U] = PropTag.Folder.NormalMessageSize.PropInfo;
			dictionary[1723006979U] = PropTag.Folder.NormalMessageSize32.PropInfo;
			dictionary[1723072532U] = PropTag.Folder.AssociatedMessageSize.PropInfo;
			dictionary[1723072515U] = PropTag.Folder.AssociatedMessageSize32.PropInfo;
			dictionary[1723138079U] = PropTag.Folder.FolderPathName.PropInfo;
			dictionary[1723203587U] = PropTag.Folder.OwnerCount.PropInfo;
			dictionary[1723269123U] = PropTag.Folder.ContactCount.PropInfo;
			dictionary[1724121091U] = PropTag.Folder.RetentionAgeLimit.PropInfo;
			dictionary[1724186635U] = PropTag.Folder.DisablePerUserRead.PropInfo;
			dictionary[1725956127U] = PropTag.Folder.ServerDN.PropInfo;
			dictionary[1726021635U] = PropTag.Folder.BackfillRanking.PropInfo;
			dictionary[1726087171U] = PropTag.Folder.LastTransmissionTime.PropInfo;
			dictionary[1726152768U] = PropTag.Folder.StatusSendTime.PropInfo;
			dictionary[1726218243U] = PropTag.Folder.BackfillEntryCount.PropInfo;
			dictionary[1726283840U] = PropTag.Folder.NextBroadcastTime.PropInfo;
			dictionary[1726349376U] = PropTag.Folder.NextBackfillTime.PropInfo;
			dictionary[1726415106U] = PropTag.Folder.LastCNBroadcast.PropInfo;
			dictionary[1727267074U] = PropTag.Folder.LastShortCNBroadcast.PropInfo;
			dictionary[1727725632U] = PropTag.Folder.AverageTransmissionTime.PropInfo;
			dictionary[1727791124U] = PropTag.Folder.ReplicationStatus.PropInfo;
			dictionary[1727856704U] = PropTag.Folder.LastDataReceivalTime.PropInfo;
			dictionary[1727922207U] = PropTag.Folder.AdminDisplayName.PropInfo;
			dictionary[1728512031U] = PropTag.Folder.URLName.PropInfo;
			dictionary[1728643136U] = PropTag.Folder.LocalCommitTime.PropInfo;
			dictionary[1728708672U] = PropTag.Folder.LocalCommitTimeMax.PropInfo;
			dictionary[1728774147U] = PropTag.Folder.DeletedCountTotal.PropInfo;
			dictionary[1728774164U] = PropTag.Folder.DeletedCountTotalInt64.PropInfo;
			dictionary[1729233154U] = PropTag.Folder.ScopeFIDs.PropInfo;
			dictionary[1729560607U] = PropTag.Folder.PFAdminDescription.PropInfo;
			dictionary[1729954050U] = PropTag.Folder.PFProxy.PropInfo;
			dictionary[1730019339U] = PropTag.Folder.PFPlatinumHomeMdb.PropInfo;
			dictionary[1730084875U] = PropTag.Folder.PFProxyRequired.PropInfo;
			dictionary[1730215939U] = PropTag.Folder.PFOverHardQuotaLimit.PropInfo;
			dictionary[1730281475U] = PropTag.Folder.PFMsgSizeLimit.PropInfo;
			dictionary[1730347019U] = PropTag.Folder.PFDisallowMdbWideExpiry.PropInfo;
			dictionary[1731002371U] = PropTag.Folder.FolderAdminFlags.PropInfo;
			dictionary[1731133460U] = PropTag.Folder.ProvisionedFID.PropInfo;
			dictionary[1731198996U] = PropTag.Folder.ELCFolderSize.PropInfo;
			dictionary[1731264515U] = PropTag.Folder.ELCFolderQuota.PropInfo;
			dictionary[1731330079U] = PropTag.Folder.ELCPolicyId.PropInfo;
			dictionary[1731395615U] = PropTag.Folder.ELCPolicyComment.PropInfo;
			dictionary[1731461123U] = PropTag.Folder.PropertyGroupMappingId.PropInfo;
			dictionary[1732771860U] = PropTag.Folder.Fid.PropInfo;
			dictionary[1732772098U] = PropTag.Folder.FidBin.PropInfo;
			dictionary[1732837396U] = PropTag.Folder.ParentFid.PropInfo;
			dictionary[1732837634U] = PropTag.Folder.ParentFidBin.PropInfo;
			dictionary[1733361667U] = PropTag.Folder.ArticleNumNext.PropInfo;
			dictionary[1733427203U] = PropTag.Folder.ImapLastArticleId.PropInfo;
			dictionary[1733886210U] = PropTag.Folder.CnExport.PropInfo;
			dictionary[1733951746U] = PropTag.Folder.PclExport.PropInfo;
			dictionary[1734017282U] = PropTag.Folder.CnMvExport.PropInfo;
			dictionary[1734082818U] = PropTag.Folder.MidsetDeletedExport.PropInfo;
			dictionary[1734148099U] = PropTag.Folder.ArticleNumMic.PropInfo;
			dictionary[1734213635U] = PropTag.Folder.ArticleNumMost.PropInfo;
			dictionary[1734344707U] = PropTag.Folder.RulesSync.PropInfo;
			dictionary[1734410498U] = PropTag.Folder.ReplicaListR.PropInfo;
			dictionary[1734476034U] = PropTag.Folder.ReplicaListRC.PropInfo;
			dictionary[1734541570U] = PropTag.Folder.ReplicaListRBUG.PropInfo;
			dictionary[1734606868U] = PropTag.Folder.RootFid.PropInfo;
			dictionary[1735393291U] = PropTag.Folder.SoftDeleted.PropInfo;
			dictionary[1735983107U] = PropTag.Folder.QuotaStyle.PropInfo;
			dictionary[1736114179U] = PropTag.Folder.StorageQuota.PropInfo;
			dictionary[1736311042U] = PropTag.Folder.FolderPropTagArray.PropInfo;
			dictionary[1736376578U] = PropTag.Folder.MsgFolderPropTagArray.PropInfo;
			dictionary[1736441859U] = PropTag.Folder.SetReceiveCount.PropInfo;
			dictionary[1736572931U] = PropTag.Folder.SubmittedCount.PropInfo;
			dictionary[1736638722U] = PropTag.Folder.CreatorToken.PropInfo;
			dictionary[1736638467U] = PropTag.Folder.SearchState.PropInfo;
			dictionary[1736704258U] = PropTag.Folder.SearchRestriction.PropInfo;
			dictionary[1736769794U] = PropTag.Folder.SearchFIDs.PropInfo;
			dictionary[1736835330U] = PropTag.Folder.RecursiveSearchFIDs.PropInfo;
			dictionary[1736900866U] = PropTag.Folder.SearchBacklinks.PropInfo;
			dictionary[1737097474U] = PropTag.Folder.CategFIDs.PropInfo;
			dictionary[1737294082U] = PropTag.Folder.FolderCDN.PropInfo;
			dictionary[1737555988U] = PropTag.Folder.MidSegmentStart.PropInfo;
			dictionary[1737621762U] = PropTag.Folder.MidsetDeleted.PropInfo;
			dictionary[1737687298U] = PropTag.Folder.MidsetExpired.PropInfo;
			dictionary[1737752834U] = PropTag.Folder.CnsetIn.PropInfo;
			dictionary[1737883906U] = PropTag.Folder.CnsetSeen.PropInfo;
			dictionary[1738014978U] = PropTag.Folder.MidsetTombstones.PropInfo;
			dictionary[1738145803U] = PropTag.Folder.GWFolder.PropInfo;
			dictionary[1738211339U] = PropTag.Folder.IPMFolder.PropInfo;
			dictionary[1738276895U] = PropTag.Folder.PublicFolderPath.PropInfo;
			dictionary[1738473474U] = PropTag.Folder.MidSegmentIndex.PropInfo;
			dictionary[1738539010U] = PropTag.Folder.MidSegmentSize.PropInfo;
			dictionary[1738604546U] = PropTag.Folder.CnSegmentStart.PropInfo;
			dictionary[1738670082U] = PropTag.Folder.CnSegmentIndex.PropInfo;
			dictionary[1738735618U] = PropTag.Folder.CnSegmentSize.PropInfo;
			dictionary[1738801172U] = PropTag.Folder.ChangeNumber.PropInfo;
			dictionary[1738801410U] = PropTag.Folder.ChangeNumberBin.PropInfo;
			dictionary[1738866946U] = PropTag.Folder.PCL.PropInfo;
			dictionary[1738936340U] = PropTag.Folder.CnMv.PropInfo;
			dictionary[1738997780U] = PropTag.Folder.FolderTreeRootFID.PropInfo;
			dictionary[1739063554U] = PropTag.Folder.SourceEntryId.PropInfo;
			dictionary[1740898306U] = PropTag.Folder.AnonymousRights.PropInfo;
			dictionary[1741553922U] = PropTag.Folder.SearchGUID.PropInfo;
			dictionary[1741816066U] = PropTag.Folder.CnsetRead.PropInfo;
			dictionary[1742340354U] = PropTag.Folder.CnsetSeenFAI.PropInfo;
			dictionary[1743061250U] = PropTag.Folder.IdSetDeleted.PropInfo;
			dictionary[1744175107U] = PropTag.Folder.ModifiedCount.PropInfo;
			dictionary[1744240643U] = PropTag.Folder.DeletedState.PropInfo;
			dictionary[1745747988U] = PropTag.Folder.ptagMsgHeaderTableFID.PropInfo;
			dictionary[1746862083U] = PropTag.Folder.MailboxNum.PropInfo;
			dictionary[1747976256U] = PropTag.Folder.LastUserAccessTime.PropInfo;
			dictionary[1748041792U] = PropTag.Folder.LastUserModificationTime.PropInfo;
			dictionary[2080506114U] = PropTag.Folder.SyncCustomState.PropInfo;
			dictionary[2080637186U] = PropTag.Folder.SyncFolderChangeKey.PropInfo;
			dictionary[2080702528U] = PropTag.Folder.SyncFolderLastModificationTime.PropInfo;
			dictionary[2081030402U] = PropTag.Folder.ptagSyncState.PropInfo;
			return dictionary;
		}

		private static WellKnownProperties.ObjectPropertyDefinitions BuildMessagePropertyDefinitions()
		{
			Dictionary<string, StorePropInfo> propertiesByTagName = WellKnownProperties.BuildMessagePropertyDefinitionsByName();
			Dictionary<uint, StorePropInfo> properties = WellKnownProperties.BuildMessagePropertyDefinitionsByTag();
			Dictionary<ushort, StorePropInfo> dictionary = new Dictionary<ushort, StorePropInfo>(100);
			dictionary.Add(4, new StorePropInfo(null, 4, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)));
			dictionary.Add(9, new StorePropInfo(null, 9, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)));
			dictionary.Add(31, new StorePropInfo(null, 31, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)));
			dictionary.Add(33, new StorePropInfo(null, 33, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)));
			dictionary.Add(36, new StorePropInfo(null, 36, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)));
			dictionary.Add(55, new StorePropInfo(null, 55, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 9)));
			dictionary.Add(61, new StorePropInfo(null, 61, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1)));
			dictionary.Add(3586, new StorePropInfo(null, 3586, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 14)));
			dictionary.Add(3587, new StorePropInfo(null, 3587, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 14)));
			dictionary.Add(3588, new StorePropInfo(null, 3588, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 14)));
			dictionary.Add(3589, new StorePropInfo(null, 3589, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3592, new StorePropInfo(null, 3592, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 14)));
			dictionary.Add(3593, new StorePropInfo(null, 3593, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(3594, new StorePropInfo(null, 3594, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1)));
			dictionary.Add(3602, new StorePropInfo(null, 3602, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3603, new StorePropInfo(null, 3603, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3604, new StorePropInfo(null, 3604, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3607, new StorePropInfo(null, 3607, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 9)));
			dictionary.Add(3611, new StorePropInfo(null, 3611, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 14)));
			dictionary.Add(3613, new StorePropInfo(null, 3613, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1)));
			dictionary.Add(3619, new StorePropInfo(null, 3619, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3631, new StorePropInfo(null, 3631, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3642, new StorePropInfo(null, 3642, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3659, new StorePropInfo(null, 3659, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3660, new StorePropInfo(null, 3660, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3672, new StorePropInfo(null, 3672, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3673, new StorePropInfo(null, 3673, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3689, new StorePropInfo(null, 3689, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2)));
			dictionary.Add(3734, new StorePropInfo(null, 3734, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3749, new StorePropInfo(null, 3749, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3750, new StorePropInfo(null, 3750, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3751, new StorePropInfo(null, 3751, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3752, new StorePropInfo(null, 3752, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3756, new StorePropInfo(null, 3756, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3757, new StorePropInfo(null, 3757, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3759, new StorePropInfo(null, 3759, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3761, new StorePropInfo(null, 3761, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3762, new StorePropInfo(null, 3762, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3763, new StorePropInfo(null, 3763, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3764, new StorePropInfo(null, 3764, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3765, new StorePropInfo(null, 3765, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3766, new StorePropInfo(null, 3766, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3767, new StorePropInfo(null, 3767, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3768, new StorePropInfo(null, 3768, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3769, new StorePropInfo(null, 3769, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3770, new StorePropInfo(null, 3770, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3771, new StorePropInfo(null, 3771, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3772, new StorePropInfo(null, 3772, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3773, new StorePropInfo(null, 3773, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3774, new StorePropInfo(null, 3774, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(3790, new StorePropInfo(null, 3790, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(4084, new StorePropInfo(null, 4084, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 14)));
			dictionary.Add(4085, new StorePropInfo(null, 4085, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(4086, new StorePropInfo(null, 4086, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9)));
			dictionary.Add(4087, new StorePropInfo(null, 4087, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(4088, new StorePropInfo(null, 4088, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 9)));
			dictionary.Add(4089, new StorePropInfo(null, 4089, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(4090, new StorePropInfo(null, 4090, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 9, 14)));
			dictionary.Add(4091, new StorePropInfo(null, 4091, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 9, 14)));
			dictionary.Add(4094, new StorePropInfo(null, 4094, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 9, 14)));
			dictionary.Add(4095, new StorePropInfo(null, 4095, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(4096, new StorePropInfo(null, 4096, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(12, 15)));
			dictionary.Add(4105, new StorePropInfo(null, 4105, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(12, 15)));
			dictionary.Add(4115, new StorePropInfo(null, 4115, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(12, 15)));
			dictionary.Add(4118, new StorePropInfo(null, 4118, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 15)));
			dictionary.Add(4150, new StorePropInfo(null, 4150, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)));
			dictionary.Add(4288, new StorePropInfo(null, 4288, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)));
			dictionary.Add(4289, new StorePropInfo(null, 4289, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)));
			dictionary.Add(4290, new StorePropInfo(null, 4290, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)));
			dictionary.Add(4336, new StorePropInfo(null, 4336, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1)));
			dictionary.Add(12293, new StorePropInfo(null, 12293, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(12295, new StorePropInfo(null, 12295, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(12296, new StorePropInfo(null, 12296, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(12307, new StorePropInfo(null, 12307, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(13325, new StorePropInfo(null, 13325, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 9)));
			dictionary.Add(13332, new StorePropInfo(null, 13332, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 9)));
			dictionary.Add(13408, new StorePropInfo(null, 13408, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(9)));
			dictionary.Add(13826, new StorePropInfo(null, 13826, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(13827, new StorePropInfo(null, 13827, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(16341, new StorePropInfo(null, 16341, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2)));
			dictionary.Add(16344, new StorePropInfo(null, 16344, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 15)));
			dictionary.Add(16345, new StorePropInfo(null, 16345, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(15)));
			dictionary.Add(16376, new StorePropInfo(null, 16376, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(16377, new StorePropInfo(null, 16377, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(16378, new StorePropInfo(null, 16378, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(16379, new StorePropInfo(null, 16379, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(16418, new StorePropInfo(null, 16418, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(16419, new StorePropInfo(null, 16419, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(16420, new StorePropInfo(null, 16420, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(16421, new StorePropInfo(null, 16421, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(16440, new StorePropInfo(null, 16440, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(16441, new StorePropInfo(null, 16441, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(16463, new StorePropInfo(null, 16463, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(16464, new StorePropInfo(null, 16464, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(16465, new StorePropInfo(null, 16465, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(16466, new StorePropInfo(null, 16466, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(16473, new StorePropInfo(null, 16473, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(16474, new StorePropInfo(null, 16474, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(25840, new StorePropInfo(null, 25840, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)));
			dictionary.Add(26080, new StorePropInfo(null, 26080, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(2, 3, 9)));
			dictionary.Add(26081, new StorePropInfo(null, 26081, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(26082, new StorePropInfo(null, 26082, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(2, 3, 9)));
			dictionary.Add(26083, new StorePropInfo(null, 26083, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(2, 3, 9)));
			dictionary.Add(26180, new StorePropInfo(null, 26180, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 9, 14)));
			dictionary.Add(26186, new StorePropInfo(null, 26186, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 14)));
			dictionary.Add(26221, new StorePropInfo(null, 26221, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 14)));
			dictionary.Add(26223, new StorePropInfo(null, 26223, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 14)));
			dictionary.Add(26224, new StorePropInfo(null, 26224, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 9, 14)));
			dictionary.Add(26255, new StorePropInfo(null, 26255, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(26375, new StorePropInfo(null, 26375, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 14)));
			dictionary.Add(26377, new StorePropInfo(null, 26377, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 14)));
			dictionary.Add(26380, new StorePropInfo(null, 26380, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)));
			dictionary.Add(26430, new StorePropInfo(null, 26430, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(26431, new StorePropInfo(null, 26431, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(26437, new StorePropInfo(null, 26437, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 14)));
			dictionary.Add(26438, new StorePropInfo(null, 26438, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)));
			dictionary.Add(26439, new StorePropInfo(null, 26439, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)));
			dictionary.Add(26440, new StorePropInfo(null, 26440, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(26442, new StorePropInfo(null, 26442, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(26443, new StorePropInfo(null, 26443, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 14)));
			dictionary.Add(26444, new StorePropInfo(null, 26444, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 14)));
			dictionary.Add(26445, new StorePropInfo(null, 26445, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(26446, new StorePropInfo(null, 26446, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(26448, new StorePropInfo(null, 26448, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 14)));
			dictionary.Add(26449, new StorePropInfo(null, 26449, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 14)));
			dictionary.Add(26456, new StorePropInfo(null, 26456, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 14)));
			dictionary.Add(26457, new StorePropInfo(null, 26457, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(26458, new StorePropInfo(null, 26458, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(26459, new StorePropInfo(null, 26459, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 3, 9, 14)));
			dictionary.Add(26476, new StorePropInfo(null, 26476, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(26477, new StorePropInfo(null, 26477, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 14)));
			dictionary.Add(26516, new StorePropInfo(null, 26516, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(26532, new StorePropInfo(null, 26532, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(26533, new StorePropInfo(null, 26533, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(26534, new StorePropInfo(null, 26534, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(26537, new StorePropInfo(null, 26537, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(26538, new StorePropInfo(null, 26538, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(26539, new StorePropInfo(null, 26539, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(26549, new StorePropInfo(null, 26549, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})));
			dictionary.Add(26550, new StorePropInfo(null, 26550, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})));
			dictionary.Add(26551, new StorePropInfo(null, 26551, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})));
			dictionary.Add(26552, new StorePropInfo(null, 26552, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})));
			dictionary.Add(26553, new StorePropInfo(null, 26553, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})));
			dictionary.Add(26554, new StorePropInfo(null, 26554, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})));
			dictionary.Add(26555, new StorePropInfo(null, 26555, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})));
			dictionary.Add(26556, new StorePropInfo(null, 26556, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})));
			dictionary.Add(26557, new StorePropInfo(null, 26557, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})));
			dictionary.Add(26558, new StorePropInfo(null, 26558, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})));
			dictionary.Add(26616, new StorePropInfo(null, 26616, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})));
			dictionary.Add(26617, new StorePropInfo(null, 26617, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})));
			dictionary.Add(26618, new StorePropInfo(null, 26618, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14,
				18
			})));
			dictionary.Add(26622, new StorePropInfo(null, 26622, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)));
			dictionary.Add(26645, new StorePropInfo(null, 26645, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9)));
			dictionary.Add(26655, new StorePropInfo(null, 26655, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(26663, new StorePropInfo(null, 26663, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			dictionary.Add(26662, new StorePropInfo(null, 26662, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)));
			WellKnownProperties.PropIdRangeList propIdRanges = WellKnownProperties.BuildPropIdRanges(new WellKnownProperties.TempIdRange[]
			{
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.NoGetProp
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.NoGetPropList
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26112,
					Max = 26135,
					Category = PropCategory.NoGetPropList
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26420,
					Max = 26431,
					Category = PropCategory.NoGetPropList
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26464,
					Max = 26623,
					Category = PropCategory.NoGetPropList
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.NoGetPropListForFastTransfer
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26112,
					Max = 26135,
					Category = PropCategory.NoGetPropListForFastTransfer
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26420,
					Max = 26431,
					Category = PropCategory.NoGetPropListForFastTransfer
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26464,
					Max = 26623,
					Category = PropCategory.NoGetPropListForFastTransfer
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26112,
					Max = 26135,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26440,
					Max = 26447,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26456,
					Max = 26463,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26420,
					Max = 26431,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26464,
					Max = 26623,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.NoCopy
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.ServerOnlySyncGroupProperty
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26112,
					Max = 26623,
					Category = PropCategory.ServerOnlySyncGroupProperty
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 8,
					Max = 23,
					Category = PropCategory.Test
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 0,
					Max = 15,
					Category = PropCategory.Test
				}
			});
			return new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.Message,
				Properties = properties,
				PropertiesByTagName = propertiesByTagName,
				PropIds = dictionary,
				PropIdRanges = propIdRanges
			};
		}

		private static Dictionary<string, StorePropInfo> BuildMessagePropertyDefinitionsByName()
		{
			Dictionary<string, StorePropInfo> dictionary = new Dictionary<string, StorePropInfo>(1862, StringComparer.OrdinalIgnoreCase);
			dictionary["AcknowledgementMode"] = PropTag.Message.AcknowledgementMode.PropInfo;
			dictionary["TestTest"] = PropTag.Message.TestTest.PropInfo;
			dictionary["AlternateRecipientAllowed"] = PropTag.Message.AlternateRecipientAllowed.PropInfo;
			dictionary["AuthorizingUsers"] = PropTag.Message.AuthorizingUsers.PropInfo;
			dictionary["AutoForwardComment"] = PropTag.Message.AutoForwardComment.PropInfo;
			dictionary["AutoForwarded"] = PropTag.Message.AutoForwarded.PropInfo;
			dictionary["ContentConfidentialityAlgorithmId"] = PropTag.Message.ContentConfidentialityAlgorithmId.PropInfo;
			dictionary["ContentCorrelator"] = PropTag.Message.ContentCorrelator.PropInfo;
			dictionary["ContentIdentifier"] = PropTag.Message.ContentIdentifier.PropInfo;
			dictionary["ContentLength"] = PropTag.Message.ContentLength.PropInfo;
			dictionary["ContentReturnRequested"] = PropTag.Message.ContentReturnRequested.PropInfo;
			dictionary["ConversationKey"] = PropTag.Message.ConversationKey.PropInfo;
			dictionary["ConversionEits"] = PropTag.Message.ConversionEits.PropInfo;
			dictionary["ConversionWithLossProhibited"] = PropTag.Message.ConversionWithLossProhibited.PropInfo;
			dictionary["ConvertedEits"] = PropTag.Message.ConvertedEits.PropInfo;
			dictionary["DeferredDeliveryTime"] = PropTag.Message.DeferredDeliveryTime.PropInfo;
			dictionary["DeliverTime"] = PropTag.Message.DeliverTime.PropInfo;
			dictionary["DiscardReason"] = PropTag.Message.DiscardReason.PropInfo;
			dictionary["DisclosureOfRecipients"] = PropTag.Message.DisclosureOfRecipients.PropInfo;
			dictionary["DLExpansionHistory"] = PropTag.Message.DLExpansionHistory.PropInfo;
			dictionary["DLExpansionProhibited"] = PropTag.Message.DLExpansionProhibited.PropInfo;
			dictionary["ExpiryTime"] = PropTag.Message.ExpiryTime.PropInfo;
			dictionary["ImplicitConversionProhibited"] = PropTag.Message.ImplicitConversionProhibited.PropInfo;
			dictionary["Importance"] = PropTag.Message.Importance.PropInfo;
			dictionary["IPMID"] = PropTag.Message.IPMID.PropInfo;
			dictionary["LatestDeliveryTime"] = PropTag.Message.LatestDeliveryTime.PropInfo;
			dictionary["MessageClass"] = PropTag.Message.MessageClass.PropInfo;
			dictionary["MessageDeliveryId"] = PropTag.Message.MessageDeliveryId.PropInfo;
			dictionary["MessageSecurityLabel"] = PropTag.Message.MessageSecurityLabel.PropInfo;
			dictionary["ObsoletedIPMS"] = PropTag.Message.ObsoletedIPMS.PropInfo;
			dictionary["OriginallyIntendedRecipientName"] = PropTag.Message.OriginallyIntendedRecipientName.PropInfo;
			dictionary["OriginalEITS"] = PropTag.Message.OriginalEITS.PropInfo;
			dictionary["OriginatorCertificate"] = PropTag.Message.OriginatorCertificate.PropInfo;
			dictionary["DeliveryReportRequested"] = PropTag.Message.DeliveryReportRequested.PropInfo;
			dictionary["OriginatorReturnAddress"] = PropTag.Message.OriginatorReturnAddress.PropInfo;
			dictionary["ParentKey"] = PropTag.Message.ParentKey.PropInfo;
			dictionary["Priority"] = PropTag.Message.Priority.PropInfo;
			dictionary["OriginCheck"] = PropTag.Message.OriginCheck.PropInfo;
			dictionary["ProofOfSubmissionRequested"] = PropTag.Message.ProofOfSubmissionRequested.PropInfo;
			dictionary["ReadReceiptRequested"] = PropTag.Message.ReadReceiptRequested.PropInfo;
			dictionary["ReceiptTime"] = PropTag.Message.ReceiptTime.PropInfo;
			dictionary["RecipientReassignmentProhibited"] = PropTag.Message.RecipientReassignmentProhibited.PropInfo;
			dictionary["RedirectionHistory"] = PropTag.Message.RedirectionHistory.PropInfo;
			dictionary["RelatedIPMS"] = PropTag.Message.RelatedIPMS.PropInfo;
			dictionary["OriginalSensitivity"] = PropTag.Message.OriginalSensitivity.PropInfo;
			dictionary["Languages"] = PropTag.Message.Languages.PropInfo;
			dictionary["ReplyTime"] = PropTag.Message.ReplyTime.PropInfo;
			dictionary["ReportTag"] = PropTag.Message.ReportTag.PropInfo;
			dictionary["ReportTime"] = PropTag.Message.ReportTime.PropInfo;
			dictionary["ReturnedIPM"] = PropTag.Message.ReturnedIPM.PropInfo;
			dictionary["Security"] = PropTag.Message.Security.PropInfo;
			dictionary["IncompleteCopy"] = PropTag.Message.IncompleteCopy.PropInfo;
			dictionary["Sensitivity"] = PropTag.Message.Sensitivity.PropInfo;
			dictionary["Subject"] = PropTag.Message.Subject.PropInfo;
			dictionary["SubjectIPM"] = PropTag.Message.SubjectIPM.PropInfo;
			dictionary["ClientSubmitTime"] = PropTag.Message.ClientSubmitTime.PropInfo;
			dictionary["ReportName"] = PropTag.Message.ReportName.PropInfo;
			dictionary["SentRepresentingSearchKey"] = PropTag.Message.SentRepresentingSearchKey.PropInfo;
			dictionary["X400ContentType"] = PropTag.Message.X400ContentType.PropInfo;
			dictionary["SubjectPrefix"] = PropTag.Message.SubjectPrefix.PropInfo;
			dictionary["NonReceiptReason"] = PropTag.Message.NonReceiptReason.PropInfo;
			dictionary["ReceivedByEntryId"] = PropTag.Message.ReceivedByEntryId.PropInfo;
			dictionary["ReceivedByName"] = PropTag.Message.ReceivedByName.PropInfo;
			dictionary["SentRepresentingEntryId"] = PropTag.Message.SentRepresentingEntryId.PropInfo;
			dictionary["SentRepresentingName"] = PropTag.Message.SentRepresentingName.PropInfo;
			dictionary["ReceivedRepresentingEntryId"] = PropTag.Message.ReceivedRepresentingEntryId.PropInfo;
			dictionary["ReceivedRepresentingName"] = PropTag.Message.ReceivedRepresentingName.PropInfo;
			dictionary["ReportEntryId"] = PropTag.Message.ReportEntryId.PropInfo;
			dictionary["ReadReceiptEntryId"] = PropTag.Message.ReadReceiptEntryId.PropInfo;
			dictionary["MessageSubmissionId"] = PropTag.Message.MessageSubmissionId.PropInfo;
			dictionary["ProviderSubmitTime"] = PropTag.Message.ProviderSubmitTime.PropInfo;
			dictionary["OriginalSubject"] = PropTag.Message.OriginalSubject.PropInfo;
			dictionary["DiscVal"] = PropTag.Message.DiscVal.PropInfo;
			dictionary["OriginalMessageClass"] = PropTag.Message.OriginalMessageClass.PropInfo;
			dictionary["OriginalAuthorEntryId"] = PropTag.Message.OriginalAuthorEntryId.PropInfo;
			dictionary["OriginalAuthorName"] = PropTag.Message.OriginalAuthorName.PropInfo;
			dictionary["OriginalSubmitTime"] = PropTag.Message.OriginalSubmitTime.PropInfo;
			dictionary["ReplyRecipientEntries"] = PropTag.Message.ReplyRecipientEntries.PropInfo;
			dictionary["ReplyRecipientNames"] = PropTag.Message.ReplyRecipientNames.PropInfo;
			dictionary["ReceivedBySearchKey"] = PropTag.Message.ReceivedBySearchKey.PropInfo;
			dictionary["ReceivedRepresentingSearchKey"] = PropTag.Message.ReceivedRepresentingSearchKey.PropInfo;
			dictionary["ReadReceiptSearchKey"] = PropTag.Message.ReadReceiptSearchKey.PropInfo;
			dictionary["ReportSearchKey"] = PropTag.Message.ReportSearchKey.PropInfo;
			dictionary["OriginalDeliveryTime"] = PropTag.Message.OriginalDeliveryTime.PropInfo;
			dictionary["OriginalAuthorSearchKey"] = PropTag.Message.OriginalAuthorSearchKey.PropInfo;
			dictionary["MessageToMe"] = PropTag.Message.MessageToMe.PropInfo;
			dictionary["MessageCCMe"] = PropTag.Message.MessageCCMe.PropInfo;
			dictionary["MessageRecipMe"] = PropTag.Message.MessageRecipMe.PropInfo;
			dictionary["OriginalSenderName"] = PropTag.Message.OriginalSenderName.PropInfo;
			dictionary["OriginalSenderEntryId"] = PropTag.Message.OriginalSenderEntryId.PropInfo;
			dictionary["OriginalSenderSearchKey"] = PropTag.Message.OriginalSenderSearchKey.PropInfo;
			dictionary["OriginalSentRepresentingName"] = PropTag.Message.OriginalSentRepresentingName.PropInfo;
			dictionary["OriginalSentRepresentingEntryId"] = PropTag.Message.OriginalSentRepresentingEntryId.PropInfo;
			dictionary["OriginalSentRepresentingSearchKey"] = PropTag.Message.OriginalSentRepresentingSearchKey.PropInfo;
			dictionary["StartDate"] = PropTag.Message.StartDate.PropInfo;
			dictionary["EndDate"] = PropTag.Message.EndDate.PropInfo;
			dictionary["OwnerApptId"] = PropTag.Message.OwnerApptId.PropInfo;
			dictionary["ResponseRequested"] = PropTag.Message.ResponseRequested.PropInfo;
			dictionary["SentRepresentingAddressType"] = PropTag.Message.SentRepresentingAddressType.PropInfo;
			dictionary["SentRepresentingEmailAddress"] = PropTag.Message.SentRepresentingEmailAddress.PropInfo;
			dictionary["OriginalSenderAddressType"] = PropTag.Message.OriginalSenderAddressType.PropInfo;
			dictionary["OriginalSenderEmailAddress"] = PropTag.Message.OriginalSenderEmailAddress.PropInfo;
			dictionary["OriginalSentRepresentingAddressType"] = PropTag.Message.OriginalSentRepresentingAddressType.PropInfo;
			dictionary["OriginalSentRepresentingEmailAddress"] = PropTag.Message.OriginalSentRepresentingEmailAddress.PropInfo;
			dictionary["ConversationTopic"] = PropTag.Message.ConversationTopic.PropInfo;
			dictionary["ConversationIndex"] = PropTag.Message.ConversationIndex.PropInfo;
			dictionary["OriginalDisplayBcc"] = PropTag.Message.OriginalDisplayBcc.PropInfo;
			dictionary["OriginalDisplayCc"] = PropTag.Message.OriginalDisplayCc.PropInfo;
			dictionary["OriginalDisplayTo"] = PropTag.Message.OriginalDisplayTo.PropInfo;
			dictionary["ReceivedByAddressType"] = PropTag.Message.ReceivedByAddressType.PropInfo;
			dictionary["ReceivedByEmailAddress"] = PropTag.Message.ReceivedByEmailAddress.PropInfo;
			dictionary["ReceivedRepresentingAddressType"] = PropTag.Message.ReceivedRepresentingAddressType.PropInfo;
			dictionary["ReceivedRepresentingEmailAddress"] = PropTag.Message.ReceivedRepresentingEmailAddress.PropInfo;
			dictionary["OriginalAuthorAddressType"] = PropTag.Message.OriginalAuthorAddressType.PropInfo;
			dictionary["OriginalAuthorEmailAddress"] = PropTag.Message.OriginalAuthorEmailAddress.PropInfo;
			dictionary["OriginallyIntendedRecipientAddressType"] = PropTag.Message.OriginallyIntendedRecipientAddressType.PropInfo;
			dictionary["TransportMessageHeaders"] = PropTag.Message.TransportMessageHeaders.PropInfo;
			dictionary["Delegation"] = PropTag.Message.Delegation.PropInfo;
			dictionary["ReportDisposition"] = PropTag.Message.ReportDisposition.PropInfo;
			dictionary["ReportDispositionMode"] = PropTag.Message.ReportDispositionMode.PropInfo;
			dictionary["ReportOriginalSender"] = PropTag.Message.ReportOriginalSender.PropInfo;
			dictionary["ReportDispositionToNames"] = PropTag.Message.ReportDispositionToNames.PropInfo;
			dictionary["ReportDispositionToEmailAddress"] = PropTag.Message.ReportDispositionToEmailAddress.PropInfo;
			dictionary["ReportDispositionOptions"] = PropTag.Message.ReportDispositionOptions.PropInfo;
			dictionary["RichContent"] = PropTag.Message.RichContent.PropInfo;
			dictionary["AdministratorEMail"] = PropTag.Message.AdministratorEMail.PropInfo;
			dictionary["ContentIntegrityCheck"] = PropTag.Message.ContentIntegrityCheck.PropInfo;
			dictionary["ExplicitConversion"] = PropTag.Message.ExplicitConversion.PropInfo;
			dictionary["ReturnRequested"] = PropTag.Message.ReturnRequested.PropInfo;
			dictionary["MessageToken"] = PropTag.Message.MessageToken.PropInfo;
			dictionary["NDRReasonCode"] = PropTag.Message.NDRReasonCode.PropInfo;
			dictionary["NDRDiagCode"] = PropTag.Message.NDRDiagCode.PropInfo;
			dictionary["NonReceiptNotificationRequested"] = PropTag.Message.NonReceiptNotificationRequested.PropInfo;
			dictionary["DeliveryPoint"] = PropTag.Message.DeliveryPoint.PropInfo;
			dictionary["NonDeliveryReportRequested"] = PropTag.Message.NonDeliveryReportRequested.PropInfo;
			dictionary["OriginatorRequestedAlterateRecipient"] = PropTag.Message.OriginatorRequestedAlterateRecipient.PropInfo;
			dictionary["PhysicalDeliveryBureauFaxDelivery"] = PropTag.Message.PhysicalDeliveryBureauFaxDelivery.PropInfo;
			dictionary["PhysicalDeliveryMode"] = PropTag.Message.PhysicalDeliveryMode.PropInfo;
			dictionary["PhysicalDeliveryReportRequest"] = PropTag.Message.PhysicalDeliveryReportRequest.PropInfo;
			dictionary["PhysicalForwardingAddress"] = PropTag.Message.PhysicalForwardingAddress.PropInfo;
			dictionary["PhysicalForwardingAddressRequested"] = PropTag.Message.PhysicalForwardingAddressRequested.PropInfo;
			dictionary["PhysicalForwardingProhibited"] = PropTag.Message.PhysicalForwardingProhibited.PropInfo;
			dictionary["ProofOfDelivery"] = PropTag.Message.ProofOfDelivery.PropInfo;
			dictionary["ProofOfDeliveryRequested"] = PropTag.Message.ProofOfDeliveryRequested.PropInfo;
			dictionary["RecipientCertificate"] = PropTag.Message.RecipientCertificate.PropInfo;
			dictionary["RecipientNumberForAdvice"] = PropTag.Message.RecipientNumberForAdvice.PropInfo;
			dictionary["RecipientType"] = PropTag.Message.RecipientType.PropInfo;
			dictionary["RegisteredMailType"] = PropTag.Message.RegisteredMailType.PropInfo;
			dictionary["ReplyRequested"] = PropTag.Message.ReplyRequested.PropInfo;
			dictionary["RequestedDeliveryMethod"] = PropTag.Message.RequestedDeliveryMethod.PropInfo;
			dictionary["SenderEntryId"] = PropTag.Message.SenderEntryId.PropInfo;
			dictionary["SenderName"] = PropTag.Message.SenderName.PropInfo;
			dictionary["SupplementaryInfo"] = PropTag.Message.SupplementaryInfo.PropInfo;
			dictionary["TypeOfMTSUser"] = PropTag.Message.TypeOfMTSUser.PropInfo;
			dictionary["SenderSearchKey"] = PropTag.Message.SenderSearchKey.PropInfo;
			dictionary["SenderAddressType"] = PropTag.Message.SenderAddressType.PropInfo;
			dictionary["SenderEmailAddress"] = PropTag.Message.SenderEmailAddress.PropInfo;
			dictionary["ParticipantSID"] = PropTag.Message.ParticipantSID.PropInfo;
			dictionary["ParticipantGuid"] = PropTag.Message.ParticipantGuid.PropInfo;
			dictionary["ToGroupExpansionRecipients"] = PropTag.Message.ToGroupExpansionRecipients.PropInfo;
			dictionary["CcGroupExpansionRecipients"] = PropTag.Message.CcGroupExpansionRecipients.PropInfo;
			dictionary["BccGroupExpansionRecipients"] = PropTag.Message.BccGroupExpansionRecipients.PropInfo;
			dictionary["CurrentVersion"] = PropTag.Message.CurrentVersion.PropInfo;
			dictionary["DeleteAfterSubmit"] = PropTag.Message.DeleteAfterSubmit.PropInfo;
			dictionary["DisplayBcc"] = PropTag.Message.DisplayBcc.PropInfo;
			dictionary["DisplayCc"] = PropTag.Message.DisplayCc.PropInfo;
			dictionary["DisplayTo"] = PropTag.Message.DisplayTo.PropInfo;
			dictionary["ParentDisplay"] = PropTag.Message.ParentDisplay.PropInfo;
			dictionary["MessageDeliveryTime"] = PropTag.Message.MessageDeliveryTime.PropInfo;
			dictionary["MessageFlags"] = PropTag.Message.MessageFlags.PropInfo;
			dictionary["MessageSize"] = PropTag.Message.MessageSize.PropInfo;
			dictionary["MessageSize32"] = PropTag.Message.MessageSize32.PropInfo;
			dictionary["ParentEntryId"] = PropTag.Message.ParentEntryId.PropInfo;
			dictionary["ParentEntryIdSvrEid"] = PropTag.Message.ParentEntryIdSvrEid.PropInfo;
			dictionary["SentMailEntryId"] = PropTag.Message.SentMailEntryId.PropInfo;
			dictionary["Correlate"] = PropTag.Message.Correlate.PropInfo;
			dictionary["CorrelateMTSID"] = PropTag.Message.CorrelateMTSID.PropInfo;
			dictionary["DiscreteValues"] = PropTag.Message.DiscreteValues.PropInfo;
			dictionary["Responsibility"] = PropTag.Message.Responsibility.PropInfo;
			dictionary["SpoolerStatus"] = PropTag.Message.SpoolerStatus.PropInfo;
			dictionary["TransportStatus"] = PropTag.Message.TransportStatus.PropInfo;
			dictionary["MessageRecipients"] = PropTag.Message.MessageRecipients.PropInfo;
			dictionary["MessageRecipientsMVBin"] = PropTag.Message.MessageRecipientsMVBin.PropInfo;
			dictionary["MessageAttachments"] = PropTag.Message.MessageAttachments.PropInfo;
			dictionary["ItemSubobjectsBin"] = PropTag.Message.ItemSubobjectsBin.PropInfo;
			dictionary["SubmitFlags"] = PropTag.Message.SubmitFlags.PropInfo;
			dictionary["RecipientStatus"] = PropTag.Message.RecipientStatus.PropInfo;
			dictionary["TransportKey"] = PropTag.Message.TransportKey.PropInfo;
			dictionary["MsgStatus"] = PropTag.Message.MsgStatus.PropInfo;
			dictionary["CreationVersion"] = PropTag.Message.CreationVersion.PropInfo;
			dictionary["ModifyVersion"] = PropTag.Message.ModifyVersion.PropInfo;
			dictionary["HasAttach"] = PropTag.Message.HasAttach.PropInfo;
			dictionary["BodyCRC"] = PropTag.Message.BodyCRC.PropInfo;
			dictionary["NormalizedSubject"] = PropTag.Message.NormalizedSubject.PropInfo;
			dictionary["RTFInSync"] = PropTag.Message.RTFInSync.PropInfo;
			dictionary["Preprocess"] = PropTag.Message.Preprocess.PropInfo;
			dictionary["InternetArticleNumber"] = PropTag.Message.InternetArticleNumber.PropInfo;
			dictionary["OriginatingMTACertificate"] = PropTag.Message.OriginatingMTACertificate.PropInfo;
			dictionary["ProofOfSubmission"] = PropTag.Message.ProofOfSubmission.PropInfo;
			dictionary["NTSecurityDescriptor"] = PropTag.Message.NTSecurityDescriptor.PropInfo;
			dictionary["PrimarySendAccount"] = PropTag.Message.PrimarySendAccount.PropInfo;
			dictionary["NextSendAccount"] = PropTag.Message.NextSendAccount.PropInfo;
			dictionary["TodoItemFlags"] = PropTag.Message.TodoItemFlags.PropInfo;
			dictionary["SwappedTODOStore"] = PropTag.Message.SwappedTODOStore.PropInfo;
			dictionary["SwappedTODOData"] = PropTag.Message.SwappedTODOData.PropInfo;
			dictionary["IMAPId"] = PropTag.Message.IMAPId.PropInfo;
			dictionary["OriginalSourceServerVersion"] = PropTag.Message.OriginalSourceServerVersion.PropInfo;
			dictionary["ReplFlags"] = PropTag.Message.ReplFlags.PropInfo;
			dictionary["MessageDeepAttachments"] = PropTag.Message.MessageDeepAttachments.PropInfo;
			dictionary["SenderGuid"] = PropTag.Message.SenderGuid.PropInfo;
			dictionary["SentRepresentingGuid"] = PropTag.Message.SentRepresentingGuid.PropInfo;
			dictionary["OriginalSenderGuid"] = PropTag.Message.OriginalSenderGuid.PropInfo;
			dictionary["OriginalSentRepresentingGuid"] = PropTag.Message.OriginalSentRepresentingGuid.PropInfo;
			dictionary["ReadReceiptGuid"] = PropTag.Message.ReadReceiptGuid.PropInfo;
			dictionary["ReportGuid"] = PropTag.Message.ReportGuid.PropInfo;
			dictionary["OriginatorGuid"] = PropTag.Message.OriginatorGuid.PropInfo;
			dictionary["ReportDestinationGuid"] = PropTag.Message.ReportDestinationGuid.PropInfo;
			dictionary["OriginalAuthorGuid"] = PropTag.Message.OriginalAuthorGuid.PropInfo;
			dictionary["ReceivedByGuid"] = PropTag.Message.ReceivedByGuid.PropInfo;
			dictionary["ReceivedRepresentingGuid"] = PropTag.Message.ReceivedRepresentingGuid.PropInfo;
			dictionary["CreatorGuid"] = PropTag.Message.CreatorGuid.PropInfo;
			dictionary["LastModifierGuid"] = PropTag.Message.LastModifierGuid.PropInfo;
			dictionary["SenderSID"] = PropTag.Message.SenderSID.PropInfo;
			dictionary["SentRepresentingSID"] = PropTag.Message.SentRepresentingSID.PropInfo;
			dictionary["OriginalSenderSid"] = PropTag.Message.OriginalSenderSid.PropInfo;
			dictionary["OriginalSentRepresentingSid"] = PropTag.Message.OriginalSentRepresentingSid.PropInfo;
			dictionary["ReadReceiptSid"] = PropTag.Message.ReadReceiptSid.PropInfo;
			dictionary["ReportSid"] = PropTag.Message.ReportSid.PropInfo;
			dictionary["OriginatorSid"] = PropTag.Message.OriginatorSid.PropInfo;
			dictionary["ReportDestinationSid"] = PropTag.Message.ReportDestinationSid.PropInfo;
			dictionary["OriginalAuthorSid"] = PropTag.Message.OriginalAuthorSid.PropInfo;
			dictionary["RcvdBySid"] = PropTag.Message.RcvdBySid.PropInfo;
			dictionary["RcvdRepresentingSid"] = PropTag.Message.RcvdRepresentingSid.PropInfo;
			dictionary["CreatorSID"] = PropTag.Message.CreatorSID.PropInfo;
			dictionary["LastModifierSid"] = PropTag.Message.LastModifierSid.PropInfo;
			dictionary["RecipientCAI"] = PropTag.Message.RecipientCAI.PropInfo;
			dictionary["ConversationCreatorSID"] = PropTag.Message.ConversationCreatorSID.PropInfo;
			dictionary["URLCompNamePostfix"] = PropTag.Message.URLCompNamePostfix.PropInfo;
			dictionary["URLCompNameSet"] = PropTag.Message.URLCompNameSet.PropInfo;
			dictionary["Read"] = PropTag.Message.Read.PropInfo;
			dictionary["CreatorSidAsXML"] = PropTag.Message.CreatorSidAsXML.PropInfo;
			dictionary["LastModifierSidAsXML"] = PropTag.Message.LastModifierSidAsXML.PropInfo;
			dictionary["SenderSIDAsXML"] = PropTag.Message.SenderSIDAsXML.PropInfo;
			dictionary["SentRepresentingSidAsXML"] = PropTag.Message.SentRepresentingSidAsXML.PropInfo;
			dictionary["OriginalSenderSIDAsXML"] = PropTag.Message.OriginalSenderSIDAsXML.PropInfo;
			dictionary["OriginalSentRepresentingSIDAsXML"] = PropTag.Message.OriginalSentRepresentingSIDAsXML.PropInfo;
			dictionary["ReadReceiptSIDAsXML"] = PropTag.Message.ReadReceiptSIDAsXML.PropInfo;
			dictionary["ReportSIDAsXML"] = PropTag.Message.ReportSIDAsXML.PropInfo;
			dictionary["OriginatorSidAsXML"] = PropTag.Message.OriginatorSidAsXML.PropInfo;
			dictionary["ReportDestinationSIDAsXML"] = PropTag.Message.ReportDestinationSIDAsXML.PropInfo;
			dictionary["OriginalAuthorSIDAsXML"] = PropTag.Message.OriginalAuthorSIDAsXML.PropInfo;
			dictionary["ReceivedBySIDAsXML"] = PropTag.Message.ReceivedBySIDAsXML.PropInfo;
			dictionary["ReceivedRepersentingSIDAsXML"] = PropTag.Message.ReceivedRepersentingSIDAsXML.PropInfo;
			dictionary["TrustSender"] = PropTag.Message.TrustSender.PropInfo;
			dictionary["SenderSMTPAddress"] = PropTag.Message.SenderSMTPAddress.PropInfo;
			dictionary["SentRepresentingSMTPAddress"] = PropTag.Message.SentRepresentingSMTPAddress.PropInfo;
			dictionary["OriginalSenderSMTPAddress"] = PropTag.Message.OriginalSenderSMTPAddress.PropInfo;
			dictionary["OriginalSentRepresentingSMTPAddress"] = PropTag.Message.OriginalSentRepresentingSMTPAddress.PropInfo;
			dictionary["ReadReceiptSMTPAddress"] = PropTag.Message.ReadReceiptSMTPAddress.PropInfo;
			dictionary["ReportSMTPAddress"] = PropTag.Message.ReportSMTPAddress.PropInfo;
			dictionary["OriginatorSMTPAddress"] = PropTag.Message.OriginatorSMTPAddress.PropInfo;
			dictionary["ReportDestinationSMTPAddress"] = PropTag.Message.ReportDestinationSMTPAddress.PropInfo;
			dictionary["OriginalAuthorSMTPAddress"] = PropTag.Message.OriginalAuthorSMTPAddress.PropInfo;
			dictionary["ReceivedBySMTPAddress"] = PropTag.Message.ReceivedBySMTPAddress.PropInfo;
			dictionary["ReceivedRepresentingSMTPAddress"] = PropTag.Message.ReceivedRepresentingSMTPAddress.PropInfo;
			dictionary["CreatorSMTPAddress"] = PropTag.Message.CreatorSMTPAddress.PropInfo;
			dictionary["LastModifierSMTPAddress"] = PropTag.Message.LastModifierSMTPAddress.PropInfo;
			dictionary["VirusScannerStamp"] = PropTag.Message.VirusScannerStamp.PropInfo;
			dictionary["VirusTransportStamp"] = PropTag.Message.VirusTransportStamp.PropInfo;
			dictionary["AddrTo"] = PropTag.Message.AddrTo.PropInfo;
			dictionary["AddrCc"] = PropTag.Message.AddrCc.PropInfo;
			dictionary["ExtendedRuleActions"] = PropTag.Message.ExtendedRuleActions.PropInfo;
			dictionary["ExtendedRuleCondition"] = PropTag.Message.ExtendedRuleCondition.PropInfo;
			dictionary["EntourageSentHistory"] = PropTag.Message.EntourageSentHistory.PropInfo;
			dictionary["ProofInProgress"] = PropTag.Message.ProofInProgress.PropInfo;
			dictionary["SearchAttachmentsOLK"] = PropTag.Message.SearchAttachmentsOLK.PropInfo;
			dictionary["SearchRecipEmailTo"] = PropTag.Message.SearchRecipEmailTo.PropInfo;
			dictionary["SearchRecipEmailCc"] = PropTag.Message.SearchRecipEmailCc.PropInfo;
			dictionary["SearchRecipEmailBcc"] = PropTag.Message.SearchRecipEmailBcc.PropInfo;
			dictionary["SFGAOFlags"] = PropTag.Message.SFGAOFlags.PropInfo;
			dictionary["SearchFullTextSubject"] = PropTag.Message.SearchFullTextSubject.PropInfo;
			dictionary["SearchFullTextBody"] = PropTag.Message.SearchFullTextBody.PropInfo;
			dictionary["FullTextConversationIndex"] = PropTag.Message.FullTextConversationIndex.PropInfo;
			dictionary["SearchAllIndexedProps"] = PropTag.Message.SearchAllIndexedProps.PropInfo;
			dictionary["SearchRecipients"] = PropTag.Message.SearchRecipients.PropInfo;
			dictionary["SearchRecipientsTo"] = PropTag.Message.SearchRecipientsTo.PropInfo;
			dictionary["SearchRecipientsCc"] = PropTag.Message.SearchRecipientsCc.PropInfo;
			dictionary["SearchRecipientsBcc"] = PropTag.Message.SearchRecipientsBcc.PropInfo;
			dictionary["SearchAccountTo"] = PropTag.Message.SearchAccountTo.PropInfo;
			dictionary["SearchAccountCc"] = PropTag.Message.SearchAccountCc.PropInfo;
			dictionary["SearchAccountBcc"] = PropTag.Message.SearchAccountBcc.PropInfo;
			dictionary["SearchEmailAddressTo"] = PropTag.Message.SearchEmailAddressTo.PropInfo;
			dictionary["SearchEmailAddressCc"] = PropTag.Message.SearchEmailAddressCc.PropInfo;
			dictionary["SearchEmailAddressBcc"] = PropTag.Message.SearchEmailAddressBcc.PropInfo;
			dictionary["SearchSmtpAddressTo"] = PropTag.Message.SearchSmtpAddressTo.PropInfo;
			dictionary["SearchSmtpAddressCc"] = PropTag.Message.SearchSmtpAddressCc.PropInfo;
			dictionary["SearchSmtpAddressBcc"] = PropTag.Message.SearchSmtpAddressBcc.PropInfo;
			dictionary["SearchSender"] = PropTag.Message.SearchSender.PropInfo;
			dictionary["IsIRMMessage"] = PropTag.Message.IsIRMMessage.PropInfo;
			dictionary["SearchIsPartiallyIndexed"] = PropTag.Message.SearchIsPartiallyIndexed.PropInfo;
			dictionary["RenewTime"] = PropTag.Message.RenewTime.PropInfo;
			dictionary["DeliveryOrRenewTime"] = PropTag.Message.DeliveryOrRenewTime.PropInfo;
			dictionary["ConversationFamilyId"] = PropTag.Message.ConversationFamilyId.PropInfo;
			dictionary["LikeCount"] = PropTag.Message.LikeCount.PropInfo;
			dictionary["RichContentDeprecated"] = PropTag.Message.RichContentDeprecated.PropInfo;
			dictionary["PeopleCentricConversationId"] = PropTag.Message.PeopleCentricConversationId.PropInfo;
			dictionary["DiscoveryAnnotation"] = PropTag.Message.DiscoveryAnnotation.PropInfo;
			dictionary["Access"] = PropTag.Message.Access.PropInfo;
			dictionary["RowType"] = PropTag.Message.RowType.PropInfo;
			dictionary["InstanceKey"] = PropTag.Message.InstanceKey.PropInfo;
			dictionary["InstanceKeySvrEid"] = PropTag.Message.InstanceKeySvrEid.PropInfo;
			dictionary["AccessLevel"] = PropTag.Message.AccessLevel.PropInfo;
			dictionary["MappingSignature"] = PropTag.Message.MappingSignature.PropInfo;
			dictionary["RecordKey"] = PropTag.Message.RecordKey.PropInfo;
			dictionary["RecordKeySvrEid"] = PropTag.Message.RecordKeySvrEid.PropInfo;
			dictionary["StoreRecordKey"] = PropTag.Message.StoreRecordKey.PropInfo;
			dictionary["StoreEntryId"] = PropTag.Message.StoreEntryId.PropInfo;
			dictionary["MiniIcon"] = PropTag.Message.MiniIcon.PropInfo;
			dictionary["Icon"] = PropTag.Message.Icon.PropInfo;
			dictionary["ObjectType"] = PropTag.Message.ObjectType.PropInfo;
			dictionary["EntryId"] = PropTag.Message.EntryId.PropInfo;
			dictionary["EntryIdSvrEid"] = PropTag.Message.EntryIdSvrEid.PropInfo;
			dictionary["BodyUnicode"] = PropTag.Message.BodyUnicode.PropInfo;
			dictionary["ReportText"] = PropTag.Message.ReportText.PropInfo;
			dictionary["OriginatorAndDLExpansionHistory"] = PropTag.Message.OriginatorAndDLExpansionHistory.PropInfo;
			dictionary["ReportingDLName"] = PropTag.Message.ReportingDLName.PropInfo;
			dictionary["ReportingMTACertificate"] = PropTag.Message.ReportingMTACertificate.PropInfo;
			dictionary["RtfSyncBodyCrc"] = PropTag.Message.RtfSyncBodyCrc.PropInfo;
			dictionary["RtfSyncBodyCount"] = PropTag.Message.RtfSyncBodyCount.PropInfo;
			dictionary["RtfSyncBodyTag"] = PropTag.Message.RtfSyncBodyTag.PropInfo;
			dictionary["RtfCompressed"] = PropTag.Message.RtfCompressed.PropInfo;
			dictionary["AlternateBestBody"] = PropTag.Message.AlternateBestBody.PropInfo;
			dictionary["RtfSyncPrefixCount"] = PropTag.Message.RtfSyncPrefixCount.PropInfo;
			dictionary["RtfSyncTrailingCount"] = PropTag.Message.RtfSyncTrailingCount.PropInfo;
			dictionary["OriginallyIntendedRecipientEntryId"] = PropTag.Message.OriginallyIntendedRecipientEntryId.PropInfo;
			dictionary["BodyHtml"] = PropTag.Message.BodyHtml.PropInfo;
			dictionary["BodyHtmlUnicode"] = PropTag.Message.BodyHtmlUnicode.PropInfo;
			dictionary["BodyContentLocation"] = PropTag.Message.BodyContentLocation.PropInfo;
			dictionary["BodyContentId"] = PropTag.Message.BodyContentId.PropInfo;
			dictionary["NativeBodyInfo"] = PropTag.Message.NativeBodyInfo.PropInfo;
			dictionary["NativeBodyType"] = PropTag.Message.NativeBodyType.PropInfo;
			dictionary["NativeBody"] = PropTag.Message.NativeBody.PropInfo;
			dictionary["AnnotationToken"] = PropTag.Message.AnnotationToken.PropInfo;
			dictionary["InternetApproved"] = PropTag.Message.InternetApproved.PropInfo;
			dictionary["InternetFollowupTo"] = PropTag.Message.InternetFollowupTo.PropInfo;
			dictionary["InternetMessageId"] = PropTag.Message.InternetMessageId.PropInfo;
			dictionary["InetNewsgroups"] = PropTag.Message.InetNewsgroups.PropInfo;
			dictionary["InternetReferences"] = PropTag.Message.InternetReferences.PropInfo;
			dictionary["PostReplyFolderEntries"] = PropTag.Message.PostReplyFolderEntries.PropInfo;
			dictionary["NNTPXRef"] = PropTag.Message.NNTPXRef.PropInfo;
			dictionary["InReplyToId"] = PropTag.Message.InReplyToId.PropInfo;
			dictionary["OriginalInternetMessageId"] = PropTag.Message.OriginalInternetMessageId.PropInfo;
			dictionary["IconIndex"] = PropTag.Message.IconIndex.PropInfo;
			dictionary["LastVerbExecuted"] = PropTag.Message.LastVerbExecuted.PropInfo;
			dictionary["LastVerbExecutionTime"] = PropTag.Message.LastVerbExecutionTime.PropInfo;
			dictionary["Relevance"] = PropTag.Message.Relevance.PropInfo;
			dictionary["FlagStatus"] = PropTag.Message.FlagStatus.PropInfo;
			dictionary["FlagCompleteTime"] = PropTag.Message.FlagCompleteTime.PropInfo;
			dictionary["FormatPT"] = PropTag.Message.FormatPT.PropInfo;
			dictionary["FollowupIcon"] = PropTag.Message.FollowupIcon.PropInfo;
			dictionary["BlockStatus"] = PropTag.Message.BlockStatus.PropInfo;
			dictionary["ItemTempFlags"] = PropTag.Message.ItemTempFlags.PropInfo;
			dictionary["SMTPTempTblData"] = PropTag.Message.SMTPTempTblData.PropInfo;
			dictionary["SMTPTempTblData2"] = PropTag.Message.SMTPTempTblData2.PropInfo;
			dictionary["SMTPTempTblData3"] = PropTag.Message.SMTPTempTblData3.PropInfo;
			dictionary["DAVSubmitData"] = PropTag.Message.DAVSubmitData.PropInfo;
			dictionary["ImapCachedMsgSize"] = PropTag.Message.ImapCachedMsgSize.PropInfo;
			dictionary["DisableFullFidelity"] = PropTag.Message.DisableFullFidelity.PropInfo;
			dictionary["URLCompName"] = PropTag.Message.URLCompName.PropInfo;
			dictionary["AttrHidden"] = PropTag.Message.AttrHidden.PropInfo;
			dictionary["AttrSystem"] = PropTag.Message.AttrSystem.PropInfo;
			dictionary["AttrReadOnly"] = PropTag.Message.AttrReadOnly.PropInfo;
			dictionary["PredictedActions"] = PropTag.Message.PredictedActions.PropInfo;
			dictionary["GroupingActions"] = PropTag.Message.GroupingActions.PropInfo;
			dictionary["PredictedActionsSummary"] = PropTag.Message.PredictedActionsSummary.PropInfo;
			dictionary["PredictedActionsThresholds"] = PropTag.Message.PredictedActionsThresholds.PropInfo;
			dictionary["IsClutter"] = PropTag.Message.IsClutter.PropInfo;
			dictionary["InferencePredictedReplyForwardReasons"] = PropTag.Message.InferencePredictedReplyForwardReasons.PropInfo;
			dictionary["InferencePredictedDeleteReasons"] = PropTag.Message.InferencePredictedDeleteReasons.PropInfo;
			dictionary["InferencePredictedIgnoreReasons"] = PropTag.Message.InferencePredictedIgnoreReasons.PropInfo;
			dictionary["OriginalDeliveryFolderInfo"] = PropTag.Message.OriginalDeliveryFolderInfo.PropInfo;
			dictionary["RowId"] = PropTag.Message.RowId.PropInfo;
			dictionary["DisplayName"] = PropTag.Message.DisplayName.PropInfo;
			dictionary["AddressType"] = PropTag.Message.AddressType.PropInfo;
			dictionary["EmailAddress"] = PropTag.Message.EmailAddress.PropInfo;
			dictionary["Comment"] = PropTag.Message.Comment.PropInfo;
			dictionary["Depth"] = PropTag.Message.Depth.PropInfo;
			dictionary["CreationTime"] = PropTag.Message.CreationTime.PropInfo;
			dictionary["LastModificationTime"] = PropTag.Message.LastModificationTime.PropInfo;
			dictionary["SearchKey"] = PropTag.Message.SearchKey.PropInfo;
			dictionary["SearchKeySvrEid"] = PropTag.Message.SearchKeySvrEid.PropInfo;
			dictionary["TargetEntryId"] = PropTag.Message.TargetEntryId.PropInfo;
			dictionary["ConversationId"] = PropTag.Message.ConversationId.PropInfo;
			dictionary["BodyTag"] = PropTag.Message.BodyTag.PropInfo;
			dictionary["ConversationIndexTrackingObsolete"] = PropTag.Message.ConversationIndexTrackingObsolete.PropInfo;
			dictionary["ConversationIndexTracking"] = PropTag.Message.ConversationIndexTracking.PropInfo;
			dictionary["ArchiveTag"] = PropTag.Message.ArchiveTag.PropInfo;
			dictionary["PolicyTag"] = PropTag.Message.PolicyTag.PropInfo;
			dictionary["RetentionPeriod"] = PropTag.Message.RetentionPeriod.PropInfo;
			dictionary["StartDateEtc"] = PropTag.Message.StartDateEtc.PropInfo;
			dictionary["RetentionDate"] = PropTag.Message.RetentionDate.PropInfo;
			dictionary["RetentionFlags"] = PropTag.Message.RetentionFlags.PropInfo;
			dictionary["ArchivePeriod"] = PropTag.Message.ArchivePeriod.PropInfo;
			dictionary["ArchiveDate"] = PropTag.Message.ArchiveDate.PropInfo;
			dictionary["FormVersion"] = PropTag.Message.FormVersion.PropInfo;
			dictionary["FormCLSID"] = PropTag.Message.FormCLSID.PropInfo;
			dictionary["FormContactName"] = PropTag.Message.FormContactName.PropInfo;
			dictionary["FormCategory"] = PropTag.Message.FormCategory.PropInfo;
			dictionary["FormCategorySub"] = PropTag.Message.FormCategorySub.PropInfo;
			dictionary["FormHidden"] = PropTag.Message.FormHidden.PropInfo;
			dictionary["FormDesignerName"] = PropTag.Message.FormDesignerName.PropInfo;
			dictionary["FormDesignerGuid"] = PropTag.Message.FormDesignerGuid.PropInfo;
			dictionary["FormMessageBehavior"] = PropTag.Message.FormMessageBehavior.PropInfo;
			dictionary["StoreSupportMask"] = PropTag.Message.StoreSupportMask.PropInfo;
			dictionary["MdbProvider"] = PropTag.Message.MdbProvider.PropInfo;
			dictionary["EventEmailReminderTimer"] = PropTag.Message.EventEmailReminderTimer.PropInfo;
			dictionary["ContentCount"] = PropTag.Message.ContentCount.PropInfo;
			dictionary["UnreadCount"] = PropTag.Message.UnreadCount.PropInfo;
			dictionary["UnreadCountInt64"] = PropTag.Message.UnreadCountInt64.PropInfo;
			dictionary["DetailsTable"] = PropTag.Message.DetailsTable.PropInfo;
			dictionary["AppointmentColorName"] = PropTag.Message.AppointmentColorName.PropInfo;
			dictionary["ContentId"] = PropTag.Message.ContentId.PropInfo;
			dictionary["MimeUrl"] = PropTag.Message.MimeUrl.PropInfo;
			dictionary["DisplayType"] = PropTag.Message.DisplayType.PropInfo;
			dictionary["SmtpAddress"] = PropTag.Message.SmtpAddress.PropInfo;
			dictionary["SimpleDisplayName"] = PropTag.Message.SimpleDisplayName.PropInfo;
			dictionary["Account"] = PropTag.Message.Account.PropInfo;
			dictionary["AlternateRecipient"] = PropTag.Message.AlternateRecipient.PropInfo;
			dictionary["CallbackTelephoneNumber"] = PropTag.Message.CallbackTelephoneNumber.PropInfo;
			dictionary["ConversionProhibited"] = PropTag.Message.ConversionProhibited.PropInfo;
			dictionary["Generation"] = PropTag.Message.Generation.PropInfo;
			dictionary["GivenName"] = PropTag.Message.GivenName.PropInfo;
			dictionary["GovernmentIDNumber"] = PropTag.Message.GovernmentIDNumber.PropInfo;
			dictionary["BusinessTelephoneNumber"] = PropTag.Message.BusinessTelephoneNumber.PropInfo;
			dictionary["HomeTelephoneNumber"] = PropTag.Message.HomeTelephoneNumber.PropInfo;
			dictionary["Initials"] = PropTag.Message.Initials.PropInfo;
			dictionary["Keyword"] = PropTag.Message.Keyword.PropInfo;
			dictionary["Language"] = PropTag.Message.Language.PropInfo;
			dictionary["Location"] = PropTag.Message.Location.PropInfo;
			dictionary["MailPermission"] = PropTag.Message.MailPermission.PropInfo;
			dictionary["MHSCommonName"] = PropTag.Message.MHSCommonName.PropInfo;
			dictionary["OrganizationalIDNumber"] = PropTag.Message.OrganizationalIDNumber.PropInfo;
			dictionary["SurName"] = PropTag.Message.SurName.PropInfo;
			dictionary["OriginalEntryId"] = PropTag.Message.OriginalEntryId.PropInfo;
			dictionary["OriginalDisplayName"] = PropTag.Message.OriginalDisplayName.PropInfo;
			dictionary["OriginalSearchKey"] = PropTag.Message.OriginalSearchKey.PropInfo;
			dictionary["PostalAddress"] = PropTag.Message.PostalAddress.PropInfo;
			dictionary["CompanyName"] = PropTag.Message.CompanyName.PropInfo;
			dictionary["Title"] = PropTag.Message.Title.PropInfo;
			dictionary["DepartmentName"] = PropTag.Message.DepartmentName.PropInfo;
			dictionary["OfficeLocation"] = PropTag.Message.OfficeLocation.PropInfo;
			dictionary["PrimaryTelephoneNumber"] = PropTag.Message.PrimaryTelephoneNumber.PropInfo;
			dictionary["Business2TelephoneNumber"] = PropTag.Message.Business2TelephoneNumber.PropInfo;
			dictionary["Business2TelephoneNumberMv"] = PropTag.Message.Business2TelephoneNumberMv.PropInfo;
			dictionary["MobileTelephoneNumber"] = PropTag.Message.MobileTelephoneNumber.PropInfo;
			dictionary["RadioTelephoneNumber"] = PropTag.Message.RadioTelephoneNumber.PropInfo;
			dictionary["CarTelephoneNumber"] = PropTag.Message.CarTelephoneNumber.PropInfo;
			dictionary["OtherTelephoneNumber"] = PropTag.Message.OtherTelephoneNumber.PropInfo;
			dictionary["TransmitableDisplayName"] = PropTag.Message.TransmitableDisplayName.PropInfo;
			dictionary["PagerTelephoneNumber"] = PropTag.Message.PagerTelephoneNumber.PropInfo;
			dictionary["UserCertificate"] = PropTag.Message.UserCertificate.PropInfo;
			dictionary["PrimaryFaxNumber"] = PropTag.Message.PrimaryFaxNumber.PropInfo;
			dictionary["BusinessFaxNumber"] = PropTag.Message.BusinessFaxNumber.PropInfo;
			dictionary["HomeFaxNumber"] = PropTag.Message.HomeFaxNumber.PropInfo;
			dictionary["Country"] = PropTag.Message.Country.PropInfo;
			dictionary["Locality"] = PropTag.Message.Locality.PropInfo;
			dictionary["StateOrProvince"] = PropTag.Message.StateOrProvince.PropInfo;
			dictionary["StreetAddress"] = PropTag.Message.StreetAddress.PropInfo;
			dictionary["PostalCode"] = PropTag.Message.PostalCode.PropInfo;
			dictionary["PostOfficeBox"] = PropTag.Message.PostOfficeBox.PropInfo;
			dictionary["TelexNumber"] = PropTag.Message.TelexNumber.PropInfo;
			dictionary["ISDNNumber"] = PropTag.Message.ISDNNumber.PropInfo;
			dictionary["AssistantTelephoneNumber"] = PropTag.Message.AssistantTelephoneNumber.PropInfo;
			dictionary["Home2TelephoneNumber"] = PropTag.Message.Home2TelephoneNumber.PropInfo;
			dictionary["Home2TelephoneNumberMv"] = PropTag.Message.Home2TelephoneNumberMv.PropInfo;
			dictionary["Assistant"] = PropTag.Message.Assistant.PropInfo;
			dictionary["SendRichInfo"] = PropTag.Message.SendRichInfo.PropInfo;
			dictionary["WeddingAnniversary"] = PropTag.Message.WeddingAnniversary.PropInfo;
			dictionary["Birthday"] = PropTag.Message.Birthday.PropInfo;
			dictionary["Hobbies"] = PropTag.Message.Hobbies.PropInfo;
			dictionary["MiddleName"] = PropTag.Message.MiddleName.PropInfo;
			dictionary["DisplayNamePrefix"] = PropTag.Message.DisplayNamePrefix.PropInfo;
			dictionary["Profession"] = PropTag.Message.Profession.PropInfo;
			dictionary["ReferredByName"] = PropTag.Message.ReferredByName.PropInfo;
			dictionary["SpouseName"] = PropTag.Message.SpouseName.PropInfo;
			dictionary["ComputerNetworkName"] = PropTag.Message.ComputerNetworkName.PropInfo;
			dictionary["CustomerId"] = PropTag.Message.CustomerId.PropInfo;
			dictionary["TTYTDDPhoneNumber"] = PropTag.Message.TTYTDDPhoneNumber.PropInfo;
			dictionary["FTPSite"] = PropTag.Message.FTPSite.PropInfo;
			dictionary["Gender"] = PropTag.Message.Gender.PropInfo;
			dictionary["ManagerName"] = PropTag.Message.ManagerName.PropInfo;
			dictionary["NickName"] = PropTag.Message.NickName.PropInfo;
			dictionary["PersonalHomePage"] = PropTag.Message.PersonalHomePage.PropInfo;
			dictionary["BusinessHomePage"] = PropTag.Message.BusinessHomePage.PropInfo;
			dictionary["ContactVersion"] = PropTag.Message.ContactVersion.PropInfo;
			dictionary["ContactEntryIds"] = PropTag.Message.ContactEntryIds.PropInfo;
			dictionary["ContactAddressTypes"] = PropTag.Message.ContactAddressTypes.PropInfo;
			dictionary["ContactDefaultAddressIndex"] = PropTag.Message.ContactDefaultAddressIndex.PropInfo;
			dictionary["ContactEmailAddress"] = PropTag.Message.ContactEmailAddress.PropInfo;
			dictionary["CompanyMainPhoneNumber"] = PropTag.Message.CompanyMainPhoneNumber.PropInfo;
			dictionary["ChildrensNames"] = PropTag.Message.ChildrensNames.PropInfo;
			dictionary["HomeAddressCity"] = PropTag.Message.HomeAddressCity.PropInfo;
			dictionary["HomeAddressCountry"] = PropTag.Message.HomeAddressCountry.PropInfo;
			dictionary["HomeAddressPostalCode"] = PropTag.Message.HomeAddressPostalCode.PropInfo;
			dictionary["HomeAddressStateOrProvince"] = PropTag.Message.HomeAddressStateOrProvince.PropInfo;
			dictionary["HomeAddressStreet"] = PropTag.Message.HomeAddressStreet.PropInfo;
			dictionary["HomeAddressPostOfficeBox"] = PropTag.Message.HomeAddressPostOfficeBox.PropInfo;
			dictionary["OtherAddressCity"] = PropTag.Message.OtherAddressCity.PropInfo;
			dictionary["OtherAddressCountry"] = PropTag.Message.OtherAddressCountry.PropInfo;
			dictionary["OtherAddressPostalCode"] = PropTag.Message.OtherAddressPostalCode.PropInfo;
			dictionary["OtherAddressStateOrProvince"] = PropTag.Message.OtherAddressStateOrProvince.PropInfo;
			dictionary["OtherAddressStreet"] = PropTag.Message.OtherAddressStreet.PropInfo;
			dictionary["OtherAddressPostOfficeBox"] = PropTag.Message.OtherAddressPostOfficeBox.PropInfo;
			dictionary["UserX509CertificateABSearchPath"] = PropTag.Message.UserX509CertificateABSearchPath.PropInfo;
			dictionary["SendInternetEncoding"] = PropTag.Message.SendInternetEncoding.PropInfo;
			dictionary["PartnerNetworkId"] = PropTag.Message.PartnerNetworkId.PropInfo;
			dictionary["PartnerNetworkUserId"] = PropTag.Message.PartnerNetworkUserId.PropInfo;
			dictionary["PartnerNetworkThumbnailPhotoUrl"] = PropTag.Message.PartnerNetworkThumbnailPhotoUrl.PropInfo;
			dictionary["PartnerNetworkProfilePhotoUrl"] = PropTag.Message.PartnerNetworkProfilePhotoUrl.PropInfo;
			dictionary["PartnerNetworkContactType"] = PropTag.Message.PartnerNetworkContactType.PropInfo;
			dictionary["RelevanceScore"] = PropTag.Message.RelevanceScore.PropInfo;
			dictionary["IsDistributionListContact"] = PropTag.Message.IsDistributionListContact.PropInfo;
			dictionary["IsPromotedContact"] = PropTag.Message.IsPromotedContact.PropInfo;
			dictionary["OrgUnitName"] = PropTag.Message.OrgUnitName.PropInfo;
			dictionary["OrganizationName"] = PropTag.Message.OrganizationName.PropInfo;
			dictionary["TestBlobProperty"] = PropTag.Message.TestBlobProperty.PropInfo;
			dictionary["FilteringHooks"] = PropTag.Message.FilteringHooks.PropInfo;
			dictionary["MailboxPartitionNumber"] = PropTag.Message.MailboxPartitionNumber.PropInfo;
			dictionary["MailboxNumberInternal"] = PropTag.Message.MailboxNumberInternal.PropInfo;
			dictionary["VirtualParentDisplay"] = PropTag.Message.VirtualParentDisplay.PropInfo;
			dictionary["InternalConversationIndexTracking"] = PropTag.Message.InternalConversationIndexTracking.PropInfo;
			dictionary["InternalConversationIndex"] = PropTag.Message.InternalConversationIndex.PropInfo;
			dictionary["ConversationItemConversationId"] = PropTag.Message.ConversationItemConversationId.PropInfo;
			dictionary["VirtualUnreadMessageCount"] = PropTag.Message.VirtualUnreadMessageCount.PropInfo;
			dictionary["VirtualIsRead"] = PropTag.Message.VirtualIsRead.PropInfo;
			dictionary["IsReadColumn"] = PropTag.Message.IsReadColumn.PropInfo;
			dictionary["Internal9ByteChangeNumber"] = PropTag.Message.Internal9ByteChangeNumber.PropInfo;
			dictionary["Internal9ByteReadCnNew"] = PropTag.Message.Internal9ByteReadCnNew.PropInfo;
			dictionary["CategoryHeaderLevelStub1"] = PropTag.Message.CategoryHeaderLevelStub1.PropInfo;
			dictionary["CategoryHeaderLevelStub2"] = PropTag.Message.CategoryHeaderLevelStub2.PropInfo;
			dictionary["CategoryHeaderLevelStub3"] = PropTag.Message.CategoryHeaderLevelStub3.PropInfo;
			dictionary["CategoryHeaderAggregateProp0"] = PropTag.Message.CategoryHeaderAggregateProp0.PropInfo;
			dictionary["CategoryHeaderAggregateProp1"] = PropTag.Message.CategoryHeaderAggregateProp1.PropInfo;
			dictionary["CategoryHeaderAggregateProp2"] = PropTag.Message.CategoryHeaderAggregateProp2.PropInfo;
			dictionary["CategoryHeaderAggregateProp3"] = PropTag.Message.CategoryHeaderAggregateProp3.PropInfo;
			dictionary["MessageFlagsActual"] = PropTag.Message.MessageFlagsActual.PropInfo;
			dictionary["InternalChangeKey"] = PropTag.Message.InternalChangeKey.PropInfo;
			dictionary["InternalSourceKey"] = PropTag.Message.InternalSourceKey.PropInfo;
			dictionary["HeaderFolderEntryId"] = PropTag.Message.HeaderFolderEntryId.PropInfo;
			dictionary["RemoteProgress"] = PropTag.Message.RemoteProgress.PropInfo;
			dictionary["RemoteProgressText"] = PropTag.Message.RemoteProgressText.PropInfo;
			dictionary["VID"] = PropTag.Message.VID.PropInfo;
			dictionary["GVid"] = PropTag.Message.GVid.PropInfo;
			dictionary["GDID"] = PropTag.Message.GDID.PropInfo;
			dictionary["XVid"] = PropTag.Message.XVid.PropInfo;
			dictionary["GDefVid"] = PropTag.Message.GDefVid.PropInfo;
			dictionary["PrimaryMailboxOverQuota"] = PropTag.Message.PrimaryMailboxOverQuota.PropInfo;
			dictionary["InternalPostReply"] = PropTag.Message.InternalPostReply.PropInfo;
			dictionary["PreviewUnread"] = PropTag.Message.PreviewUnread.PropInfo;
			dictionary["Preview"] = PropTag.Message.Preview.PropInfo;
			dictionary["InternetCPID"] = PropTag.Message.InternetCPID.PropInfo;
			dictionary["AutoResponseSuppress"] = PropTag.Message.AutoResponseSuppress.PropInfo;
			dictionary["HasDAMs"] = PropTag.Message.HasDAMs.PropInfo;
			dictionary["DeferredSendNumber"] = PropTag.Message.DeferredSendNumber.PropInfo;
			dictionary["DeferredSendUnits"] = PropTag.Message.DeferredSendUnits.PropInfo;
			dictionary["ExpiryNumber"] = PropTag.Message.ExpiryNumber.PropInfo;
			dictionary["ExpiryUnits"] = PropTag.Message.ExpiryUnits.PropInfo;
			dictionary["DeferredSendTime"] = PropTag.Message.DeferredSendTime.PropInfo;
			dictionary["MessageLocaleId"] = PropTag.Message.MessageLocaleId.PropInfo;
			dictionary["RuleTriggerHistory"] = PropTag.Message.RuleTriggerHistory.PropInfo;
			dictionary["MoveToStoreEid"] = PropTag.Message.MoveToStoreEid.PropInfo;
			dictionary["MoveToFolderEid"] = PropTag.Message.MoveToFolderEid.PropInfo;
			dictionary["StorageQuotaLimit"] = PropTag.Message.StorageQuotaLimit.PropInfo;
			dictionary["ExcessStorageUsed"] = PropTag.Message.ExcessStorageUsed.PropInfo;
			dictionary["ServerGeneratingQuotaMsg"] = PropTag.Message.ServerGeneratingQuotaMsg.PropInfo;
			dictionary["CreatorName"] = PropTag.Message.CreatorName.PropInfo;
			dictionary["CreatorEntryId"] = PropTag.Message.CreatorEntryId.PropInfo;
			dictionary["LastModifierName"] = PropTag.Message.LastModifierName.PropInfo;
			dictionary["LastModifierEntryId"] = PropTag.Message.LastModifierEntryId.PropInfo;
			dictionary["MessageCodePage"] = PropTag.Message.MessageCodePage.PropInfo;
			dictionary["QuotaType"] = PropTag.Message.QuotaType.PropInfo;
			dictionary["IsPublicFolderQuotaMessage"] = PropTag.Message.IsPublicFolderQuotaMessage.PropInfo;
			dictionary["NewAttach"] = PropTag.Message.NewAttach.PropInfo;
			dictionary["StartEmbed"] = PropTag.Message.StartEmbed.PropInfo;
			dictionary["EndEmbed"] = PropTag.Message.EndEmbed.PropInfo;
			dictionary["StartRecip"] = PropTag.Message.StartRecip.PropInfo;
			dictionary["EndRecip"] = PropTag.Message.EndRecip.PropInfo;
			dictionary["EndCcRecip"] = PropTag.Message.EndCcRecip.PropInfo;
			dictionary["EndBccRecip"] = PropTag.Message.EndBccRecip.PropInfo;
			dictionary["EndP1Recip"] = PropTag.Message.EndP1Recip.PropInfo;
			dictionary["DNPrefix"] = PropTag.Message.DNPrefix.PropInfo;
			dictionary["StartTopFolder"] = PropTag.Message.StartTopFolder.PropInfo;
			dictionary["StartSubFolder"] = PropTag.Message.StartSubFolder.PropInfo;
			dictionary["EndFolder"] = PropTag.Message.EndFolder.PropInfo;
			dictionary["StartMessage"] = PropTag.Message.StartMessage.PropInfo;
			dictionary["EndMessage"] = PropTag.Message.EndMessage.PropInfo;
			dictionary["EndAttach"] = PropTag.Message.EndAttach.PropInfo;
			dictionary["EcWarning"] = PropTag.Message.EcWarning.PropInfo;
			dictionary["StartFAIMessage"] = PropTag.Message.StartFAIMessage.PropInfo;
			dictionary["NewFXFolder"] = PropTag.Message.NewFXFolder.PropInfo;
			dictionary["IncrSyncChange"] = PropTag.Message.IncrSyncChange.PropInfo;
			dictionary["IncrSyncDelete"] = PropTag.Message.IncrSyncDelete.PropInfo;
			dictionary["IncrSyncEnd"] = PropTag.Message.IncrSyncEnd.PropInfo;
			dictionary["IncrSyncMessage"] = PropTag.Message.IncrSyncMessage.PropInfo;
			dictionary["FastTransferDelProp"] = PropTag.Message.FastTransferDelProp.PropInfo;
			dictionary["IdsetGiven"] = PropTag.Message.IdsetGiven.PropInfo;
			dictionary["IdsetGivenInt32"] = PropTag.Message.IdsetGivenInt32.PropInfo;
			dictionary["FastTransferErrorInfo"] = PropTag.Message.FastTransferErrorInfo.PropInfo;
			dictionary["SenderFlags"] = PropTag.Message.SenderFlags.PropInfo;
			dictionary["SentRepresentingFlags"] = PropTag.Message.SentRepresentingFlags.PropInfo;
			dictionary["RcvdByFlags"] = PropTag.Message.RcvdByFlags.PropInfo;
			dictionary["RcvdRepresentingFlags"] = PropTag.Message.RcvdRepresentingFlags.PropInfo;
			dictionary["OriginalSenderFlags"] = PropTag.Message.OriginalSenderFlags.PropInfo;
			dictionary["OriginalSentRepresentingFlags"] = PropTag.Message.OriginalSentRepresentingFlags.PropInfo;
			dictionary["ReportFlags"] = PropTag.Message.ReportFlags.PropInfo;
			dictionary["ReadReceiptFlags"] = PropTag.Message.ReadReceiptFlags.PropInfo;
			dictionary["SoftDeletes"] = PropTag.Message.SoftDeletes.PropInfo;
			dictionary["CreatorAddressType"] = PropTag.Message.CreatorAddressType.PropInfo;
			dictionary["CreatorEmailAddr"] = PropTag.Message.CreatorEmailAddr.PropInfo;
			dictionary["LastModifierAddressType"] = PropTag.Message.LastModifierAddressType.PropInfo;
			dictionary["LastModifierEmailAddr"] = PropTag.Message.LastModifierEmailAddr.PropInfo;
			dictionary["ReportAddressType"] = PropTag.Message.ReportAddressType.PropInfo;
			dictionary["ReportEmailAddress"] = PropTag.Message.ReportEmailAddress.PropInfo;
			dictionary["ReportDisplayName"] = PropTag.Message.ReportDisplayName.PropInfo;
			dictionary["ReadReceiptAddressType"] = PropTag.Message.ReadReceiptAddressType.PropInfo;
			dictionary["ReadReceiptEmailAddress"] = PropTag.Message.ReadReceiptEmailAddress.PropInfo;
			dictionary["ReadReceiptDisplayName"] = PropTag.Message.ReadReceiptDisplayName.PropInfo;
			dictionary["IdsetRead"] = PropTag.Message.IdsetRead.PropInfo;
			dictionary["IdsetUnread"] = PropTag.Message.IdsetUnread.PropInfo;
			dictionary["IncrSyncRead"] = PropTag.Message.IncrSyncRead.PropInfo;
			dictionary["SenderSimpleDisplayName"] = PropTag.Message.SenderSimpleDisplayName.PropInfo;
			dictionary["SentRepresentingSimpleDisplayName"] = PropTag.Message.SentRepresentingSimpleDisplayName.PropInfo;
			dictionary["OriginalSenderSimpleDisplayName"] = PropTag.Message.OriginalSenderSimpleDisplayName.PropInfo;
			dictionary["OriginalSentRepresentingSimpleDisplayName"] = PropTag.Message.OriginalSentRepresentingSimpleDisplayName.PropInfo;
			dictionary["ReceivedBySimpleDisplayName"] = PropTag.Message.ReceivedBySimpleDisplayName.PropInfo;
			dictionary["ReceivedRepresentingSimpleDisplayName"] = PropTag.Message.ReceivedRepresentingSimpleDisplayName.PropInfo;
			dictionary["ReadReceiptSimpleDisplayName"] = PropTag.Message.ReadReceiptSimpleDisplayName.PropInfo;
			dictionary["ReportSimpleDisplayName"] = PropTag.Message.ReportSimpleDisplayName.PropInfo;
			dictionary["CreatorSimpleDisplayName"] = PropTag.Message.CreatorSimpleDisplayName.PropInfo;
			dictionary["LastModifierSimpleDisplayName"] = PropTag.Message.LastModifierSimpleDisplayName.PropInfo;
			dictionary["IncrSyncStateBegin"] = PropTag.Message.IncrSyncStateBegin.PropInfo;
			dictionary["IncrSyncStateEnd"] = PropTag.Message.IncrSyncStateEnd.PropInfo;
			dictionary["IncrSyncImailStream"] = PropTag.Message.IncrSyncImailStream.PropInfo;
			dictionary["SenderOrgAddressType"] = PropTag.Message.SenderOrgAddressType.PropInfo;
			dictionary["SenderOrgEmailAddr"] = PropTag.Message.SenderOrgEmailAddr.PropInfo;
			dictionary["SentRepresentingOrgAddressType"] = PropTag.Message.SentRepresentingOrgAddressType.PropInfo;
			dictionary["SentRepresentingOrgEmailAddr"] = PropTag.Message.SentRepresentingOrgEmailAddr.PropInfo;
			dictionary["OriginalSenderOrgAddressType"] = PropTag.Message.OriginalSenderOrgAddressType.PropInfo;
			dictionary["OriginalSenderOrgEmailAddr"] = PropTag.Message.OriginalSenderOrgEmailAddr.PropInfo;
			dictionary["OriginalSentRepresentingOrgAddressType"] = PropTag.Message.OriginalSentRepresentingOrgAddressType.PropInfo;
			dictionary["OriginalSentRepresentingOrgEmailAddr"] = PropTag.Message.OriginalSentRepresentingOrgEmailAddr.PropInfo;
			dictionary["RcvdByOrgAddressType"] = PropTag.Message.RcvdByOrgAddressType.PropInfo;
			dictionary["RcvdByOrgEmailAddr"] = PropTag.Message.RcvdByOrgEmailAddr.PropInfo;
			dictionary["RcvdRepresentingOrgAddressType"] = PropTag.Message.RcvdRepresentingOrgAddressType.PropInfo;
			dictionary["RcvdRepresentingOrgEmailAddr"] = PropTag.Message.RcvdRepresentingOrgEmailAddr.PropInfo;
			dictionary["ReadReceiptOrgAddressType"] = PropTag.Message.ReadReceiptOrgAddressType.PropInfo;
			dictionary["ReadReceiptOrgEmailAddr"] = PropTag.Message.ReadReceiptOrgEmailAddr.PropInfo;
			dictionary["ReportOrgAddressType"] = PropTag.Message.ReportOrgAddressType.PropInfo;
			dictionary["ReportOrgEmailAddr"] = PropTag.Message.ReportOrgEmailAddr.PropInfo;
			dictionary["CreatorOrgAddressType"] = PropTag.Message.CreatorOrgAddressType.PropInfo;
			dictionary["CreatorOrgEmailAddr"] = PropTag.Message.CreatorOrgEmailAddr.PropInfo;
			dictionary["LastModifierOrgAddressType"] = PropTag.Message.LastModifierOrgAddressType.PropInfo;
			dictionary["LastModifierOrgEmailAddr"] = PropTag.Message.LastModifierOrgEmailAddr.PropInfo;
			dictionary["OriginatorOrgAddressType"] = PropTag.Message.OriginatorOrgAddressType.PropInfo;
			dictionary["OriginatorOrgEmailAddr"] = PropTag.Message.OriginatorOrgEmailAddr.PropInfo;
			dictionary["ReportDestinationOrgEmailType"] = PropTag.Message.ReportDestinationOrgEmailType.PropInfo;
			dictionary["ReportDestinationOrgEmailAddr"] = PropTag.Message.ReportDestinationOrgEmailAddr.PropInfo;
			dictionary["OriginalAuthorOrgAddressType"] = PropTag.Message.OriginalAuthorOrgAddressType.PropInfo;
			dictionary["OriginalAuthorOrgEmailAddr"] = PropTag.Message.OriginalAuthorOrgEmailAddr.PropInfo;
			dictionary["CreatorFlags"] = PropTag.Message.CreatorFlags.PropInfo;
			dictionary["LastModifierFlags"] = PropTag.Message.LastModifierFlags.PropInfo;
			dictionary["OriginatorFlags"] = PropTag.Message.OriginatorFlags.PropInfo;
			dictionary["ReportDestinationFlags"] = PropTag.Message.ReportDestinationFlags.PropInfo;
			dictionary["OriginalAuthorFlags"] = PropTag.Message.OriginalAuthorFlags.PropInfo;
			dictionary["OriginatorSimpleDisplayName"] = PropTag.Message.OriginatorSimpleDisplayName.PropInfo;
			dictionary["ReportDestinationSimpleDisplayName"] = PropTag.Message.ReportDestinationSimpleDisplayName.PropInfo;
			dictionary["OriginalAuthorSimpleDispName"] = PropTag.Message.OriginalAuthorSimpleDispName.PropInfo;
			dictionary["OriginatorSearchKey"] = PropTag.Message.OriginatorSearchKey.PropInfo;
			dictionary["ReportDestinationAddressType"] = PropTag.Message.ReportDestinationAddressType.PropInfo;
			dictionary["ReportDestinationEmailAddress"] = PropTag.Message.ReportDestinationEmailAddress.PropInfo;
			dictionary["ReportDestinationSearchKey"] = PropTag.Message.ReportDestinationSearchKey.PropInfo;
			dictionary["IncrSyncImailStreamContinue"] = PropTag.Message.IncrSyncImailStreamContinue.PropInfo;
			dictionary["IncrSyncImailStreamCancel"] = PropTag.Message.IncrSyncImailStreamCancel.PropInfo;
			dictionary["IncrSyncImailStream2Continue"] = PropTag.Message.IncrSyncImailStream2Continue.PropInfo;
			dictionary["IncrSyncProgressMode"] = PropTag.Message.IncrSyncProgressMode.PropInfo;
			dictionary["SyncProgressPerMsg"] = PropTag.Message.SyncProgressPerMsg.PropInfo;
			dictionary["ContentFilterSCL"] = PropTag.Message.ContentFilterSCL.PropInfo;
			dictionary["IncrSyncMsgPartial"] = PropTag.Message.IncrSyncMsgPartial.PropInfo;
			dictionary["IncrSyncGroupInfo"] = PropTag.Message.IncrSyncGroupInfo.PropInfo;
			dictionary["IncrSyncGroupId"] = PropTag.Message.IncrSyncGroupId.PropInfo;
			dictionary["IncrSyncChangePartial"] = PropTag.Message.IncrSyncChangePartial.PropInfo;
			dictionary["ContentFilterPCL"] = PropTag.Message.ContentFilterPCL.PropInfo;
			dictionary["DeliverAsRead"] = PropTag.Message.DeliverAsRead.PropInfo;
			dictionary["InetMailOverrideFormat"] = PropTag.Message.InetMailOverrideFormat.PropInfo;
			dictionary["MessageEditorFormat"] = PropTag.Message.MessageEditorFormat.PropInfo;
			dictionary["SenderSMTPAddressXSO"] = PropTag.Message.SenderSMTPAddressXSO.PropInfo;
			dictionary["SentRepresentingSMTPAddressXSO"] = PropTag.Message.SentRepresentingSMTPAddressXSO.PropInfo;
			dictionary["OriginalSenderSMTPAddressXSO"] = PropTag.Message.OriginalSenderSMTPAddressXSO.PropInfo;
			dictionary["OriginalSentRepresentingSMTPAddressXSO"] = PropTag.Message.OriginalSentRepresentingSMTPAddressXSO.PropInfo;
			dictionary["ReadReceiptSMTPAddressXSO"] = PropTag.Message.ReadReceiptSMTPAddressXSO.PropInfo;
			dictionary["OriginalAuthorSMTPAddressXSO"] = PropTag.Message.OriginalAuthorSMTPAddressXSO.PropInfo;
			dictionary["ReceivedBySMTPAddressXSO"] = PropTag.Message.ReceivedBySMTPAddressXSO.PropInfo;
			dictionary["ReceivedRepresentingSMTPAddressXSO"] = PropTag.Message.ReceivedRepresentingSMTPAddressXSO.PropInfo;
			dictionary["RecipientOrder"] = PropTag.Message.RecipientOrder.PropInfo;
			dictionary["RecipientSipUri"] = PropTag.Message.RecipientSipUri.PropInfo;
			dictionary["RecipientDisplayName"] = PropTag.Message.RecipientDisplayName.PropInfo;
			dictionary["RecipientEntryId"] = PropTag.Message.RecipientEntryId.PropInfo;
			dictionary["RecipientFlags"] = PropTag.Message.RecipientFlags.PropInfo;
			dictionary["RecipientTrackStatus"] = PropTag.Message.RecipientTrackStatus.PropInfo;
			dictionary["DotStuffState"] = PropTag.Message.DotStuffState.PropInfo;
			dictionary["InternetMessageIdHash"] = PropTag.Message.InternetMessageIdHash.PropInfo;
			dictionary["ConversationTopicHash"] = PropTag.Message.ConversationTopicHash.PropInfo;
			dictionary["MimeSkeleton"] = PropTag.Message.MimeSkeleton.PropInfo;
			dictionary["ReplyTemplateId"] = PropTag.Message.ReplyTemplateId.PropInfo;
			dictionary["SecureSubmitFlags"] = PropTag.Message.SecureSubmitFlags.PropInfo;
			dictionary["SourceKey"] = PropTag.Message.SourceKey.PropInfo;
			dictionary["ParentSourceKey"] = PropTag.Message.ParentSourceKey.PropInfo;
			dictionary["ChangeKey"] = PropTag.Message.ChangeKey.PropInfo;
			dictionary["PredecessorChangeList"] = PropTag.Message.PredecessorChangeList.PropInfo;
			dictionary["RuleMsgState"] = PropTag.Message.RuleMsgState.PropInfo;
			dictionary["RuleMsgUserFlags"] = PropTag.Message.RuleMsgUserFlags.PropInfo;
			dictionary["RuleMsgProvider"] = PropTag.Message.RuleMsgProvider.PropInfo;
			dictionary["RuleMsgName"] = PropTag.Message.RuleMsgName.PropInfo;
			dictionary["RuleMsgLevel"] = PropTag.Message.RuleMsgLevel.PropInfo;
			dictionary["RuleMsgProviderData"] = PropTag.Message.RuleMsgProviderData.PropInfo;
			dictionary["RuleMsgActions"] = PropTag.Message.RuleMsgActions.PropInfo;
			dictionary["RuleMsgCondition"] = PropTag.Message.RuleMsgCondition.PropInfo;
			dictionary["RuleMsgConditionLCID"] = PropTag.Message.RuleMsgConditionLCID.PropInfo;
			dictionary["RuleMsgVersion"] = PropTag.Message.RuleMsgVersion.PropInfo;
			dictionary["RuleMsgSequence"] = PropTag.Message.RuleMsgSequence.PropInfo;
			dictionary["LISSD"] = PropTag.Message.LISSD.PropInfo;
			dictionary["ReplicaServer"] = PropTag.Message.ReplicaServer.PropInfo;
			dictionary["DAMOriginalEntryId"] = PropTag.Message.DAMOriginalEntryId.PropInfo;
			dictionary["HasNamedProperties"] = PropTag.Message.HasNamedProperties.PropInfo;
			dictionary["FidMid"] = PropTag.Message.FidMid.PropInfo;
			dictionary["InternetContent"] = PropTag.Message.InternetContent.PropInfo;
			dictionary["OriginatorName"] = PropTag.Message.OriginatorName.PropInfo;
			dictionary["OriginatorEmailAddress"] = PropTag.Message.OriginatorEmailAddress.PropInfo;
			dictionary["OriginatorAddressType"] = PropTag.Message.OriginatorAddressType.PropInfo;
			dictionary["OriginatorEntryId"] = PropTag.Message.OriginatorEntryId.PropInfo;
			dictionary["RecipientNumber"] = PropTag.Message.RecipientNumber.PropInfo;
			dictionary["ReportDestinationName"] = PropTag.Message.ReportDestinationName.PropInfo;
			dictionary["ReportDestinationEntryId"] = PropTag.Message.ReportDestinationEntryId.PropInfo;
			dictionary["ProhibitReceiveQuota"] = PropTag.Message.ProhibitReceiveQuota.PropInfo;
			dictionary["SearchAttachments"] = PropTag.Message.SearchAttachments.PropInfo;
			dictionary["ProhibitSendQuota"] = PropTag.Message.ProhibitSendQuota.PropInfo;
			dictionary["SubmittedByAdmin"] = PropTag.Message.SubmittedByAdmin.PropInfo;
			dictionary["LongTermEntryIdFromTable"] = PropTag.Message.LongTermEntryIdFromTable.PropInfo;
			dictionary["RuleIds"] = PropTag.Message.RuleIds.PropInfo;
			dictionary["RuleMsgConditionOld"] = PropTag.Message.RuleMsgConditionOld.PropInfo;
			dictionary["RuleMsgActionsOld"] = PropTag.Message.RuleMsgActionsOld.PropInfo;
			dictionary["DeletedOn"] = PropTag.Message.DeletedOn.PropInfo;
			dictionary["CodePageId"] = PropTag.Message.CodePageId.PropInfo;
			dictionary["UserDN"] = PropTag.Message.UserDN.PropInfo;
			dictionary["MailboxDSGuidGuid"] = PropTag.Message.MailboxDSGuidGuid.PropInfo;
			dictionary["URLName"] = PropTag.Message.URLName.PropInfo;
			dictionary["LocalCommitTime"] = PropTag.Message.LocalCommitTime.PropInfo;
			dictionary["AutoReset"] = PropTag.Message.AutoReset.PropInfo;
			dictionary["ELCAutoCopyTag"] = PropTag.Message.ELCAutoCopyTag.PropInfo;
			dictionary["ELCMoveDate"] = PropTag.Message.ELCMoveDate.PropInfo;
			dictionary["PropGroupInfo"] = PropTag.Message.PropGroupInfo.PropInfo;
			dictionary["PropertyGroupChangeMask"] = PropTag.Message.PropertyGroupChangeMask.PropInfo;
			dictionary["ReadCnNewExport"] = PropTag.Message.ReadCnNewExport.PropInfo;
			dictionary["SentMailSvrEID"] = PropTag.Message.SentMailSvrEID.PropInfo;
			dictionary["SentMailSvrEIDBin"] = PropTag.Message.SentMailSvrEIDBin.PropInfo;
			dictionary["LocallyDelivered"] = PropTag.Message.LocallyDelivered.PropInfo;
			dictionary["MimeSize"] = PropTag.Message.MimeSize.PropInfo;
			dictionary["MimeSize32"] = PropTag.Message.MimeSize32.PropInfo;
			dictionary["FileSize"] = PropTag.Message.FileSize.PropInfo;
			dictionary["FileSize32"] = PropTag.Message.FileSize32.PropInfo;
			dictionary["Fid"] = PropTag.Message.Fid.PropInfo;
			dictionary["FidBin"] = PropTag.Message.FidBin.PropInfo;
			dictionary["ParentFid"] = PropTag.Message.ParentFid.PropInfo;
			dictionary["Mid"] = PropTag.Message.Mid.PropInfo;
			dictionary["MidBin"] = PropTag.Message.MidBin.PropInfo;
			dictionary["CategID"] = PropTag.Message.CategID.PropInfo;
			dictionary["ParentCategID"] = PropTag.Message.ParentCategID.PropInfo;
			dictionary["InstanceId"] = PropTag.Message.InstanceId.PropInfo;
			dictionary["InstanceNum"] = PropTag.Message.InstanceNum.PropInfo;
			dictionary["ChangeType"] = PropTag.Message.ChangeType.PropInfo;
			dictionary["RequiresRefResolve"] = PropTag.Message.RequiresRefResolve.PropInfo;
			dictionary["LTID"] = PropTag.Message.LTID.PropInfo;
			dictionary["CnExport"] = PropTag.Message.CnExport.PropInfo;
			dictionary["PclExport"] = PropTag.Message.PclExport.PropInfo;
			dictionary["CnMvExport"] = PropTag.Message.CnMvExport.PropInfo;
			dictionary["MailboxGuid"] = PropTag.Message.MailboxGuid.PropInfo;
			dictionary["MapiEntryIdGuidGuid"] = PropTag.Message.MapiEntryIdGuidGuid.PropInfo;
			dictionary["ImapCachedBodystructure"] = PropTag.Message.ImapCachedBodystructure.PropInfo;
			dictionary["StorageQuota"] = PropTag.Message.StorageQuota.PropInfo;
			dictionary["CnsetIn"] = PropTag.Message.CnsetIn.PropInfo;
			dictionary["CnsetSeen"] = PropTag.Message.CnsetSeen.PropInfo;
			dictionary["ChangeNumber"] = PropTag.Message.ChangeNumber.PropInfo;
			dictionary["ChangeNumberBin"] = PropTag.Message.ChangeNumberBin.PropInfo;
			dictionary["PCL"] = PropTag.Message.PCL.PropInfo;
			dictionary["CnMv"] = PropTag.Message.CnMv.PropInfo;
			dictionary["SourceEntryId"] = PropTag.Message.SourceEntryId.PropInfo;
			dictionary["MailFlags"] = PropTag.Message.MailFlags.PropInfo;
			dictionary["Associated"] = PropTag.Message.Associated.PropInfo;
			dictionary["SubmitResponsibility"] = PropTag.Message.SubmitResponsibility.PropInfo;
			dictionary["SharedReceiptHandling"] = PropTag.Message.SharedReceiptHandling.PropInfo;
			dictionary["Inid"] = PropTag.Message.Inid.PropInfo;
			dictionary["MessageAttachList"] = PropTag.Message.MessageAttachList.PropInfo;
			dictionary["SenderCAI"] = PropTag.Message.SenderCAI.PropInfo;
			dictionary["SentRepresentingCAI"] = PropTag.Message.SentRepresentingCAI.PropInfo;
			dictionary["OriginalSenderCAI"] = PropTag.Message.OriginalSenderCAI.PropInfo;
			dictionary["OriginalSentRepresentingCAI"] = PropTag.Message.OriginalSentRepresentingCAI.PropInfo;
			dictionary["ReceivedByCAI"] = PropTag.Message.ReceivedByCAI.PropInfo;
			dictionary["ReceivedRepresentingCAI"] = PropTag.Message.ReceivedRepresentingCAI.PropInfo;
			dictionary["ReadReceiptCAI"] = PropTag.Message.ReadReceiptCAI.PropInfo;
			dictionary["ReportCAI"] = PropTag.Message.ReportCAI.PropInfo;
			dictionary["CreatorCAI"] = PropTag.Message.CreatorCAI.PropInfo;
			dictionary["LastModifierCAI"] = PropTag.Message.LastModifierCAI.PropInfo;
			dictionary["CnsetRead"] = PropTag.Message.CnsetRead.PropInfo;
			dictionary["CnsetSeenFAI"] = PropTag.Message.CnsetSeenFAI.PropInfo;
			dictionary["IdSetDeleted"] = PropTag.Message.IdSetDeleted.PropInfo;
			dictionary["OriginatorCAI"] = PropTag.Message.OriginatorCAI.PropInfo;
			dictionary["ReportDestinationCAI"] = PropTag.Message.ReportDestinationCAI.PropInfo;
			dictionary["OriginalAuthorCAI"] = PropTag.Message.OriginalAuthorCAI.PropInfo;
			dictionary["ReadCnNew"] = PropTag.Message.ReadCnNew.PropInfo;
			dictionary["ReadCnNewBin"] = PropTag.Message.ReadCnNewBin.PropInfo;
			dictionary["SenderTelephoneNumber"] = PropTag.Message.SenderTelephoneNumber.PropInfo;
			dictionary["VoiceMessageAttachmentOrder"] = PropTag.Message.VoiceMessageAttachmentOrder.PropInfo;
			dictionary["DocumentId"] = PropTag.Message.DocumentId.PropInfo;
			dictionary["MailboxNum"] = PropTag.Message.MailboxNum.PropInfo;
			dictionary["ConversationIdHash"] = PropTag.Message.ConversationIdHash.PropInfo;
			dictionary["ConversationDocumentId"] = PropTag.Message.ConversationDocumentId.PropInfo;
			dictionary["LocalDirectoryBlob"] = PropTag.Message.LocalDirectoryBlob.PropInfo;
			dictionary["ViewStyle"] = PropTag.Message.ViewStyle.PropInfo;
			dictionary["FreebusyEMA"] = PropTag.Message.FreebusyEMA.PropInfo;
			dictionary["WunderbarLinkEntryID"] = PropTag.Message.WunderbarLinkEntryID.PropInfo;
			dictionary["WunderbarLinkStoreEntryId"] = PropTag.Message.WunderbarLinkStoreEntryId.PropInfo;
			dictionary["SchdInfoFreebusyMerged"] = PropTag.Message.SchdInfoFreebusyMerged.PropInfo;
			dictionary["WunderbarLinkGroupClsId"] = PropTag.Message.WunderbarLinkGroupClsId.PropInfo;
			dictionary["WunderbarLinkGroupName"] = PropTag.Message.WunderbarLinkGroupName.PropInfo;
			dictionary["WunderbarLinkSection"] = PropTag.Message.WunderbarLinkSection.PropInfo;
			dictionary["NavigationNodeCalendarColor"] = PropTag.Message.NavigationNodeCalendarColor.PropInfo;
			dictionary["NavigationNodeAddressbookEntryId"] = PropTag.Message.NavigationNodeAddressbookEntryId.PropInfo;
			dictionary["AgingDeleteItems"] = PropTag.Message.AgingDeleteItems.PropInfo;
			dictionary["AgingFileName9AndPrev"] = PropTag.Message.AgingFileName9AndPrev.PropInfo;
			dictionary["AgingAgeFolder"] = PropTag.Message.AgingAgeFolder.PropInfo;
			dictionary["AgingDontAgeMe"] = PropTag.Message.AgingDontAgeMe.PropInfo;
			dictionary["AgingFileNameAfter9"] = PropTag.Message.AgingFileNameAfter9.PropInfo;
			dictionary["AgingWhenDeletedOnServer"] = PropTag.Message.AgingWhenDeletedOnServer.PropInfo;
			dictionary["AgingWaitUntilExpired"] = PropTag.Message.AgingWaitUntilExpired.PropInfo;
			dictionary["ConversationMvFrom"] = PropTag.Message.ConversationMvFrom.PropInfo;
			dictionary["ConversationMvFromMailboxWide"] = PropTag.Message.ConversationMvFromMailboxWide.PropInfo;
			dictionary["ConversationMvTo"] = PropTag.Message.ConversationMvTo.PropInfo;
			dictionary["ConversationMvToMailboxWide"] = PropTag.Message.ConversationMvToMailboxWide.PropInfo;
			dictionary["ConversationMessageDeliveryTime"] = PropTag.Message.ConversationMessageDeliveryTime.PropInfo;
			dictionary["ConversationMessageDeliveryTimeMailboxWide"] = PropTag.Message.ConversationMessageDeliveryTimeMailboxWide.PropInfo;
			dictionary["ConversationCategories"] = PropTag.Message.ConversationCategories.PropInfo;
			dictionary["ConversationCategoriesMailboxWide"] = PropTag.Message.ConversationCategoriesMailboxWide.PropInfo;
			dictionary["ConversationFlagStatus"] = PropTag.Message.ConversationFlagStatus.PropInfo;
			dictionary["ConversationFlagStatusMailboxWide"] = PropTag.Message.ConversationFlagStatusMailboxWide.PropInfo;
			dictionary["ConversationFlagCompleteTime"] = PropTag.Message.ConversationFlagCompleteTime.PropInfo;
			dictionary["ConversationFlagCompleteTimeMailboxWide"] = PropTag.Message.ConversationFlagCompleteTimeMailboxWide.PropInfo;
			dictionary["ConversationHasAttach"] = PropTag.Message.ConversationHasAttach.PropInfo;
			dictionary["ConversationHasAttachMailboxWide"] = PropTag.Message.ConversationHasAttachMailboxWide.PropInfo;
			dictionary["ConversationContentCount"] = PropTag.Message.ConversationContentCount.PropInfo;
			dictionary["ConversationContentCountMailboxWide"] = PropTag.Message.ConversationContentCountMailboxWide.PropInfo;
			dictionary["ConversationContentUnread"] = PropTag.Message.ConversationContentUnread.PropInfo;
			dictionary["ConversationContentUnreadMailboxWide"] = PropTag.Message.ConversationContentUnreadMailboxWide.PropInfo;
			dictionary["ConversationMessageSize"] = PropTag.Message.ConversationMessageSize.PropInfo;
			dictionary["ConversationMessageSizeMailboxWide"] = PropTag.Message.ConversationMessageSizeMailboxWide.PropInfo;
			dictionary["ConversationMessageClasses"] = PropTag.Message.ConversationMessageClasses.PropInfo;
			dictionary["ConversationMessageClassesMailboxWide"] = PropTag.Message.ConversationMessageClassesMailboxWide.PropInfo;
			dictionary["ConversationReplyForwardState"] = PropTag.Message.ConversationReplyForwardState.PropInfo;
			dictionary["ConversationReplyForwardStateMailboxWide"] = PropTag.Message.ConversationReplyForwardStateMailboxWide.PropInfo;
			dictionary["ConversationImportance"] = PropTag.Message.ConversationImportance.PropInfo;
			dictionary["ConversationImportanceMailboxWide"] = PropTag.Message.ConversationImportanceMailboxWide.PropInfo;
			dictionary["ConversationMvFromUnread"] = PropTag.Message.ConversationMvFromUnread.PropInfo;
			dictionary["ConversationMvFromUnreadMailboxWide"] = PropTag.Message.ConversationMvFromUnreadMailboxWide.PropInfo;
			dictionary["ConversationMvItemIds"] = PropTag.Message.ConversationMvItemIds.PropInfo;
			dictionary["ConversationMvItemIdsMailboxWide"] = PropTag.Message.ConversationMvItemIdsMailboxWide.PropInfo;
			dictionary["ConversationHasIrm"] = PropTag.Message.ConversationHasIrm.PropInfo;
			dictionary["ConversationHasIrmMailboxWide"] = PropTag.Message.ConversationHasIrmMailboxWide.PropInfo;
			dictionary["PersonCompanyNameMailboxWide"] = PropTag.Message.PersonCompanyNameMailboxWide.PropInfo;
			dictionary["PersonDisplayNameMailboxWide"] = PropTag.Message.PersonDisplayNameMailboxWide.PropInfo;
			dictionary["PersonGivenNameMailboxWide"] = PropTag.Message.PersonGivenNameMailboxWide.PropInfo;
			dictionary["PersonSurnameMailboxWide"] = PropTag.Message.PersonSurnameMailboxWide.PropInfo;
			dictionary["PersonPhotoContactEntryIdMailboxWide"] = PropTag.Message.PersonPhotoContactEntryIdMailboxWide.PropInfo;
			dictionary["ConversationInferredImportanceInternal"] = PropTag.Message.ConversationInferredImportanceInternal.PropInfo;
			dictionary["ConversationInferredImportanceOverride"] = PropTag.Message.ConversationInferredImportanceOverride.PropInfo;
			dictionary["ConversationInferredUnimportanceInternal"] = PropTag.Message.ConversationInferredUnimportanceInternal.PropInfo;
			dictionary["ConversationInferredImportanceInternalMailboxWide"] = PropTag.Message.ConversationInferredImportanceInternalMailboxWide.PropInfo;
			dictionary["ConversationInferredImportanceOverrideMailboxWide"] = PropTag.Message.ConversationInferredImportanceOverrideMailboxWide.PropInfo;
			dictionary["ConversationInferredUnimportanceInternalMailboxWide"] = PropTag.Message.ConversationInferredUnimportanceInternalMailboxWide.PropInfo;
			dictionary["PersonFileAsMailboxWide"] = PropTag.Message.PersonFileAsMailboxWide.PropInfo;
			dictionary["PersonRelevanceScoreMailboxWide"] = PropTag.Message.PersonRelevanceScoreMailboxWide.PropInfo;
			dictionary["PersonIsDistributionListMailboxWide"] = PropTag.Message.PersonIsDistributionListMailboxWide.PropInfo;
			dictionary["PersonHomeCityMailboxWide"] = PropTag.Message.PersonHomeCityMailboxWide.PropInfo;
			dictionary["PersonCreationTimeMailboxWide"] = PropTag.Message.PersonCreationTimeMailboxWide.PropInfo;
			dictionary["PersonGALLinkIDMailboxWide"] = PropTag.Message.PersonGALLinkIDMailboxWide.PropInfo;
			dictionary["PersonMvEmailAddressMailboxWide"] = PropTag.Message.PersonMvEmailAddressMailboxWide.PropInfo;
			dictionary["PersonMvEmailDisplayNameMailboxWide"] = PropTag.Message.PersonMvEmailDisplayNameMailboxWide.PropInfo;
			dictionary["PersonMvEmailRoutingTypeMailboxWide"] = PropTag.Message.PersonMvEmailRoutingTypeMailboxWide.PropInfo;
			dictionary["PersonImAddressMailboxWide"] = PropTag.Message.PersonImAddressMailboxWide.PropInfo;
			dictionary["PersonWorkCityMailboxWide"] = PropTag.Message.PersonWorkCityMailboxWide.PropInfo;
			dictionary["PersonDisplayNameFirstLastMailboxWide"] = PropTag.Message.PersonDisplayNameFirstLastMailboxWide.PropInfo;
			dictionary["PersonDisplayNameLastFirstMailboxWide"] = PropTag.Message.PersonDisplayNameLastFirstMailboxWide.PropInfo;
			dictionary["ConversationGroupingActions"] = PropTag.Message.ConversationGroupingActions.PropInfo;
			dictionary["ConversationGroupingActionsMailboxWide"] = PropTag.Message.ConversationGroupingActionsMailboxWide.PropInfo;
			dictionary["ConversationPredictedActionsSummary"] = PropTag.Message.ConversationPredictedActionsSummary.PropInfo;
			dictionary["ConversationPredictedActionsSummaryMailboxWide"] = PropTag.Message.ConversationPredictedActionsSummaryMailboxWide.PropInfo;
			dictionary["ConversationHasClutter"] = PropTag.Message.ConversationHasClutter.PropInfo;
			dictionary["ConversationHasClutterMailboxWide"] = PropTag.Message.ConversationHasClutterMailboxWide.PropInfo;
			dictionary["ConversationLastMemberDocumentId"] = PropTag.Message.ConversationLastMemberDocumentId.PropInfo;
			dictionary["ConversationPreview"] = PropTag.Message.ConversationPreview.PropInfo;
			dictionary["ConversationLastMemberDocumentIdMailboxWide"] = PropTag.Message.ConversationLastMemberDocumentIdMailboxWide.PropInfo;
			dictionary["ConversationInitialMemberDocumentId"] = PropTag.Message.ConversationInitialMemberDocumentId.PropInfo;
			dictionary["ConversationMemberDocumentIds"] = PropTag.Message.ConversationMemberDocumentIds.PropInfo;
			dictionary["ConversationMessageDeliveryOrRenewTimeMailboxWide"] = PropTag.Message.ConversationMessageDeliveryOrRenewTimeMailboxWide.PropInfo;
			dictionary["FamilyId"] = PropTag.Message.FamilyId.PropInfo;
			dictionary["ConversationMessageRichContentMailboxWide"] = PropTag.Message.ConversationMessageRichContentMailboxWide.PropInfo;
			dictionary["ConversationPreviewMailboxWide"] = PropTag.Message.ConversationPreviewMailboxWide.PropInfo;
			dictionary["ConversationMessageDeliveryOrRenewTime"] = PropTag.Message.ConversationMessageDeliveryOrRenewTime.PropInfo;
			dictionary["ConversationWorkingSetSourcePartition"] = PropTag.Message.ConversationWorkingSetSourcePartition.PropInfo;
			dictionary["NDRFromName"] = PropTag.Message.NDRFromName.PropInfo;
			dictionary["SecurityFlags"] = PropTag.Message.SecurityFlags.PropInfo;
			dictionary["SecurityReceiptRequestProcessed"] = PropTag.Message.SecurityReceiptRequestProcessed.PropInfo;
			dictionary["FavoriteDisplayName"] = PropTag.Message.FavoriteDisplayName.PropInfo;
			dictionary["FavoriteDisplayAlias"] = PropTag.Message.FavoriteDisplayAlias.PropInfo;
			dictionary["FavPublicSourceKey"] = PropTag.Message.FavPublicSourceKey.PropInfo;
			dictionary["SyncFolderSourceKey"] = PropTag.Message.SyncFolderSourceKey.PropInfo;
			dictionary["UserConfigurationDataType"] = PropTag.Message.UserConfigurationDataType.PropInfo;
			dictionary["UserConfigurationXmlStream"] = PropTag.Message.UserConfigurationXmlStream.PropInfo;
			dictionary["UserConfigurationStream"] = PropTag.Message.UserConfigurationStream.PropInfo;
			dictionary["ReplyFwdStatus"] = PropTag.Message.ReplyFwdStatus.PropInfo;
			dictionary["OscSyncEnabledOnServer"] = PropTag.Message.OscSyncEnabledOnServer.PropInfo;
			dictionary["Processed"] = PropTag.Message.Processed.PropInfo;
			dictionary["FavLevelMask"] = PropTag.Message.FavLevelMask.PropInfo;
			return dictionary;
		}

		private static Dictionary<uint, StorePropInfo> BuildMessagePropertyDefinitionsByTag()
		{
			Dictionary<uint, StorePropInfo> dictionary = new Dictionary<uint, StorePropInfo>(1862);
			dictionary[65539U] = PropTag.Message.AcknowledgementMode.PropInfo;
			dictionary[65794U] = PropTag.Message.TestTest.PropInfo;
			dictionary[131083U] = PropTag.Message.AlternateRecipientAllowed.PropInfo;
			dictionary[196866U] = PropTag.Message.AuthorizingUsers.PropInfo;
			dictionary[262175U] = PropTag.Message.AutoForwardComment.PropInfo;
			dictionary[327691U] = PropTag.Message.AutoForwarded.PropInfo;
			dictionary[393474U] = PropTag.Message.ContentConfidentialityAlgorithmId.PropInfo;
			dictionary[459010U] = PropTag.Message.ContentCorrelator.PropInfo;
			dictionary[524319U] = PropTag.Message.ContentIdentifier.PropInfo;
			dictionary[589827U] = PropTag.Message.ContentLength.PropInfo;
			dictionary[655371U] = PropTag.Message.ContentReturnRequested.PropInfo;
			dictionary[721154U] = PropTag.Message.ConversationKey.PropInfo;
			dictionary[786690U] = PropTag.Message.ConversionEits.PropInfo;
			dictionary[851979U] = PropTag.Message.ConversionWithLossProhibited.PropInfo;
			dictionary[917762U] = PropTag.Message.ConvertedEits.PropInfo;
			dictionary[983104U] = PropTag.Message.DeferredDeliveryTime.PropInfo;
			dictionary[1048640U] = PropTag.Message.DeliverTime.PropInfo;
			dictionary[1114115U] = PropTag.Message.DiscardReason.PropInfo;
			dictionary[1179659U] = PropTag.Message.DisclosureOfRecipients.PropInfo;
			dictionary[1245442U] = PropTag.Message.DLExpansionHistory.PropInfo;
			dictionary[1310731U] = PropTag.Message.DLExpansionProhibited.PropInfo;
			dictionary[1376320U] = PropTag.Message.ExpiryTime.PropInfo;
			dictionary[1441803U] = PropTag.Message.ImplicitConversionProhibited.PropInfo;
			dictionary[1507331U] = PropTag.Message.Importance.PropInfo;
			dictionary[1573122U] = PropTag.Message.IPMID.PropInfo;
			dictionary[1638464U] = PropTag.Message.LatestDeliveryTime.PropInfo;
			dictionary[1703967U] = PropTag.Message.MessageClass.PropInfo;
			dictionary[1769730U] = PropTag.Message.MessageDeliveryId.PropInfo;
			dictionary[1966338U] = PropTag.Message.MessageSecurityLabel.PropInfo;
			dictionary[2031874U] = PropTag.Message.ObsoletedIPMS.PropInfo;
			dictionary[2097410U] = PropTag.Message.OriginallyIntendedRecipientName.PropInfo;
			dictionary[2162946U] = PropTag.Message.OriginalEITS.PropInfo;
			dictionary[2228482U] = PropTag.Message.OriginatorCertificate.PropInfo;
			dictionary[2293771U] = PropTag.Message.DeliveryReportRequested.PropInfo;
			dictionary[2359554U] = PropTag.Message.OriginatorReturnAddress.PropInfo;
			dictionary[2425090U] = PropTag.Message.ParentKey.PropInfo;
			dictionary[2490371U] = PropTag.Message.Priority.PropInfo;
			dictionary[2556162U] = PropTag.Message.OriginCheck.PropInfo;
			dictionary[2621451U] = PropTag.Message.ProofOfSubmissionRequested.PropInfo;
			dictionary[2686987U] = PropTag.Message.ReadReceiptRequested.PropInfo;
			dictionary[2752576U] = PropTag.Message.ReceiptTime.PropInfo;
			dictionary[2818059U] = PropTag.Message.RecipientReassignmentProhibited.PropInfo;
			dictionary[2883842U] = PropTag.Message.RedirectionHistory.PropInfo;
			dictionary[2949378U] = PropTag.Message.RelatedIPMS.PropInfo;
			dictionary[3014659U] = PropTag.Message.OriginalSensitivity.PropInfo;
			dictionary[3080223U] = PropTag.Message.Languages.PropInfo;
			dictionary[3145792U] = PropTag.Message.ReplyTime.PropInfo;
			dictionary[3211522U] = PropTag.Message.ReportTag.PropInfo;
			dictionary[3276864U] = PropTag.Message.ReportTime.PropInfo;
			dictionary[3342347U] = PropTag.Message.ReturnedIPM.PropInfo;
			dictionary[3407875U] = PropTag.Message.Security.PropInfo;
			dictionary[3473419U] = PropTag.Message.IncompleteCopy.PropInfo;
			dictionary[3538947U] = PropTag.Message.Sensitivity.PropInfo;
			dictionary[3604511U] = PropTag.Message.Subject.PropInfo;
			dictionary[3670274U] = PropTag.Message.SubjectIPM.PropInfo;
			dictionary[3735616U] = PropTag.Message.ClientSubmitTime.PropInfo;
			dictionary[3801119U] = PropTag.Message.ReportName.PropInfo;
			dictionary[3866882U] = PropTag.Message.SentRepresentingSearchKey.PropInfo;
			dictionary[3932418U] = PropTag.Message.X400ContentType.PropInfo;
			dictionary[3997727U] = PropTag.Message.SubjectPrefix.PropInfo;
			dictionary[4063235U] = PropTag.Message.NonReceiptReason.PropInfo;
			dictionary[4129026U] = PropTag.Message.ReceivedByEntryId.PropInfo;
			dictionary[4194335U] = PropTag.Message.ReceivedByName.PropInfo;
			dictionary[4260098U] = PropTag.Message.SentRepresentingEntryId.PropInfo;
			dictionary[4325407U] = PropTag.Message.SentRepresentingName.PropInfo;
			dictionary[4391170U] = PropTag.Message.ReceivedRepresentingEntryId.PropInfo;
			dictionary[4456479U] = PropTag.Message.ReceivedRepresentingName.PropInfo;
			dictionary[4522242U] = PropTag.Message.ReportEntryId.PropInfo;
			dictionary[4587778U] = PropTag.Message.ReadReceiptEntryId.PropInfo;
			dictionary[4653314U] = PropTag.Message.MessageSubmissionId.PropInfo;
			dictionary[4718656U] = PropTag.Message.ProviderSubmitTime.PropInfo;
			dictionary[4784159U] = PropTag.Message.OriginalSubject.PropInfo;
			dictionary[4849675U] = PropTag.Message.DiscVal.PropInfo;
			dictionary[4915231U] = PropTag.Message.OriginalMessageClass.PropInfo;
			dictionary[4980994U] = PropTag.Message.OriginalAuthorEntryId.PropInfo;
			dictionary[5046303U] = PropTag.Message.OriginalAuthorName.PropInfo;
			dictionary[5111872U] = PropTag.Message.OriginalSubmitTime.PropInfo;
			dictionary[5177602U] = PropTag.Message.ReplyRecipientEntries.PropInfo;
			dictionary[5242911U] = PropTag.Message.ReplyRecipientNames.PropInfo;
			dictionary[5308674U] = PropTag.Message.ReceivedBySearchKey.PropInfo;
			dictionary[5374210U] = PropTag.Message.ReceivedRepresentingSearchKey.PropInfo;
			dictionary[5439746U] = PropTag.Message.ReadReceiptSearchKey.PropInfo;
			dictionary[5505282U] = PropTag.Message.ReportSearchKey.PropInfo;
			dictionary[5570624U] = PropTag.Message.OriginalDeliveryTime.PropInfo;
			dictionary[5636354U] = PropTag.Message.OriginalAuthorSearchKey.PropInfo;
			dictionary[5701643U] = PropTag.Message.MessageToMe.PropInfo;
			dictionary[5767179U] = PropTag.Message.MessageCCMe.PropInfo;
			dictionary[5832715U] = PropTag.Message.MessageRecipMe.PropInfo;
			dictionary[5898271U] = PropTag.Message.OriginalSenderName.PropInfo;
			dictionary[5964034U] = PropTag.Message.OriginalSenderEntryId.PropInfo;
			dictionary[6029570U] = PropTag.Message.OriginalSenderSearchKey.PropInfo;
			dictionary[6094879U] = PropTag.Message.OriginalSentRepresentingName.PropInfo;
			dictionary[6160642U] = PropTag.Message.OriginalSentRepresentingEntryId.PropInfo;
			dictionary[6226178U] = PropTag.Message.OriginalSentRepresentingSearchKey.PropInfo;
			dictionary[6291520U] = PropTag.Message.StartDate.PropInfo;
			dictionary[6357056U] = PropTag.Message.EndDate.PropInfo;
			dictionary[6422531U] = PropTag.Message.OwnerApptId.PropInfo;
			dictionary[6488075U] = PropTag.Message.ResponseRequested.PropInfo;
			dictionary[6553631U] = PropTag.Message.SentRepresentingAddressType.PropInfo;
			dictionary[6619167U] = PropTag.Message.SentRepresentingEmailAddress.PropInfo;
			dictionary[6684703U] = PropTag.Message.OriginalSenderAddressType.PropInfo;
			dictionary[6750239U] = PropTag.Message.OriginalSenderEmailAddress.PropInfo;
			dictionary[6815775U] = PropTag.Message.OriginalSentRepresentingAddressType.PropInfo;
			dictionary[6881311U] = PropTag.Message.OriginalSentRepresentingEmailAddress.PropInfo;
			dictionary[7340063U] = PropTag.Message.ConversationTopic.PropInfo;
			dictionary[7405826U] = PropTag.Message.ConversationIndex.PropInfo;
			dictionary[7471135U] = PropTag.Message.OriginalDisplayBcc.PropInfo;
			dictionary[7536671U] = PropTag.Message.OriginalDisplayCc.PropInfo;
			dictionary[7602207U] = PropTag.Message.OriginalDisplayTo.PropInfo;
			dictionary[7667743U] = PropTag.Message.ReceivedByAddressType.PropInfo;
			dictionary[7733279U] = PropTag.Message.ReceivedByEmailAddress.PropInfo;
			dictionary[7798815U] = PropTag.Message.ReceivedRepresentingAddressType.PropInfo;
			dictionary[7864351U] = PropTag.Message.ReceivedRepresentingEmailAddress.PropInfo;
			dictionary[7929887U] = PropTag.Message.OriginalAuthorAddressType.PropInfo;
			dictionary[7995423U] = PropTag.Message.OriginalAuthorEmailAddress.PropInfo;
			dictionary[8126495U] = PropTag.Message.OriginallyIntendedRecipientAddressType.PropInfo;
			dictionary[8192031U] = PropTag.Message.TransportMessageHeaders.PropInfo;
			dictionary[8257794U] = PropTag.Message.Delegation.PropInfo;
			dictionary[8388639U] = PropTag.Message.ReportDisposition.PropInfo;
			dictionary[8454175U] = PropTag.Message.ReportDispositionMode.PropInfo;
			dictionary[8519711U] = PropTag.Message.ReportOriginalSender.PropInfo;
			dictionary[8585247U] = PropTag.Message.ReportDispositionToNames.PropInfo;
			dictionary[8650783U] = PropTag.Message.ReportDispositionToEmailAddress.PropInfo;
			dictionary[8716319U] = PropTag.Message.ReportDispositionOptions.PropInfo;
			dictionary[8781826U] = PropTag.Message.RichContent.PropInfo;
			dictionary[16781343U] = PropTag.Message.AdministratorEMail.PropInfo;
			dictionary[201326850U] = PropTag.Message.ContentIntegrityCheck.PropInfo;
			dictionary[201392131U] = PropTag.Message.ExplicitConversion.PropInfo;
			dictionary[201457675U] = PropTag.Message.ReturnRequested.PropInfo;
			dictionary[201523458U] = PropTag.Message.MessageToken.PropInfo;
			dictionary[201588739U] = PropTag.Message.NDRReasonCode.PropInfo;
			dictionary[201654275U] = PropTag.Message.NDRDiagCode.PropInfo;
			dictionary[201719819U] = PropTag.Message.NonReceiptNotificationRequested.PropInfo;
			dictionary[201785347U] = PropTag.Message.DeliveryPoint.PropInfo;
			dictionary[201850891U] = PropTag.Message.NonDeliveryReportRequested.PropInfo;
			dictionary[201916674U] = PropTag.Message.OriginatorRequestedAlterateRecipient.PropInfo;
			dictionary[201981963U] = PropTag.Message.PhysicalDeliveryBureauFaxDelivery.PropInfo;
			dictionary[202047491U] = PropTag.Message.PhysicalDeliveryMode.PropInfo;
			dictionary[202113027U] = PropTag.Message.PhysicalDeliveryReportRequest.PropInfo;
			dictionary[202178818U] = PropTag.Message.PhysicalForwardingAddress.PropInfo;
			dictionary[202244107U] = PropTag.Message.PhysicalForwardingAddressRequested.PropInfo;
			dictionary[202309643U] = PropTag.Message.PhysicalForwardingProhibited.PropInfo;
			dictionary[202440962U] = PropTag.Message.ProofOfDelivery.PropInfo;
			dictionary[202506251U] = PropTag.Message.ProofOfDeliveryRequested.PropInfo;
			dictionary[202572034U] = PropTag.Message.RecipientCertificate.PropInfo;
			dictionary[202637343U] = PropTag.Message.RecipientNumberForAdvice.PropInfo;
			dictionary[202702851U] = PropTag.Message.RecipientType.PropInfo;
			dictionary[202768387U] = PropTag.Message.RegisteredMailType.PropInfo;
			dictionary[202833931U] = PropTag.Message.ReplyRequested.PropInfo;
			dictionary[202899459U] = PropTag.Message.RequestedDeliveryMethod.PropInfo;
			dictionary[202965250U] = PropTag.Message.SenderEntryId.PropInfo;
			dictionary[203030559U] = PropTag.Message.SenderName.PropInfo;
			dictionary[203096095U] = PropTag.Message.SupplementaryInfo.PropInfo;
			dictionary[203161603U] = PropTag.Message.TypeOfMTSUser.PropInfo;
			dictionary[203227394U] = PropTag.Message.SenderSearchKey.PropInfo;
			dictionary[203292703U] = PropTag.Message.SenderAddressType.PropInfo;
			dictionary[203358239U] = PropTag.Message.SenderEmailAddress.PropInfo;
			dictionary[203686146U] = PropTag.Message.ParticipantSID.PropInfo;
			dictionary[203751682U] = PropTag.Message.ParticipantGuid.PropInfo;
			dictionary[203816991U] = PropTag.Message.ToGroupExpansionRecipients.PropInfo;
			dictionary[203882527U] = PropTag.Message.CcGroupExpansionRecipients.PropInfo;
			dictionary[203948063U] = PropTag.Message.BccGroupExpansionRecipients.PropInfo;
			dictionary[234881044U] = PropTag.Message.CurrentVersion.PropInfo;
			dictionary[234946571U] = PropTag.Message.DeleteAfterSubmit.PropInfo;
			dictionary[235012127U] = PropTag.Message.DisplayBcc.PropInfo;
			dictionary[235077663U] = PropTag.Message.DisplayCc.PropInfo;
			dictionary[235143199U] = PropTag.Message.DisplayTo.PropInfo;
			dictionary[235208735U] = PropTag.Message.ParentDisplay.PropInfo;
			dictionary[235274304U] = PropTag.Message.MessageDeliveryTime.PropInfo;
			dictionary[235339779U] = PropTag.Message.MessageFlags.PropInfo;
			dictionary[235405332U] = PropTag.Message.MessageSize.PropInfo;
			dictionary[235405315U] = PropTag.Message.MessageSize32.PropInfo;
			dictionary[235471106U] = PropTag.Message.ParentEntryId.PropInfo;
			dictionary[235471099U] = PropTag.Message.ParentEntryIdSvrEid.PropInfo;
			dictionary[235536642U] = PropTag.Message.SentMailEntryId.PropInfo;
			dictionary[235667467U] = PropTag.Message.Correlate.PropInfo;
			dictionary[235733250U] = PropTag.Message.CorrelateMTSID.PropInfo;
			dictionary[235798539U] = PropTag.Message.DiscreteValues.PropInfo;
			dictionary[235864075U] = PropTag.Message.Responsibility.PropInfo;
			dictionary[235929603U] = PropTag.Message.SpoolerStatus.PropInfo;
			dictionary[235995139U] = PropTag.Message.TransportStatus.PropInfo;
			dictionary[236060685U] = PropTag.Message.MessageRecipients.PropInfo;
			dictionary[236065026U] = PropTag.Message.MessageRecipientsMVBin.PropInfo;
			dictionary[236126221U] = PropTag.Message.MessageAttachments.PropInfo;
			dictionary[236126466U] = PropTag.Message.ItemSubobjectsBin.PropInfo;
			dictionary[236191747U] = PropTag.Message.SubmitFlags.PropInfo;
			dictionary[236257283U] = PropTag.Message.RecipientStatus.PropInfo;
			dictionary[236322819U] = PropTag.Message.TransportKey.PropInfo;
			dictionary[236388355U] = PropTag.Message.MsgStatus.PropInfo;
			dictionary[236519444U] = PropTag.Message.CreationVersion.PropInfo;
			dictionary[236584980U] = PropTag.Message.ModifyVersion.PropInfo;
			dictionary[236650507U] = PropTag.Message.HasAttach.PropInfo;
			dictionary[236716035U] = PropTag.Message.BodyCRC.PropInfo;
			dictionary[236781599U] = PropTag.Message.NormalizedSubject.PropInfo;
			dictionary[236912651U] = PropTag.Message.RTFInSync.PropInfo;
			dictionary[237109259U] = PropTag.Message.Preprocess.PropInfo;
			dictionary[237174787U] = PropTag.Message.InternetArticleNumber.PropInfo;
			dictionary[237306114U] = PropTag.Message.OriginatingMTACertificate.PropInfo;
			dictionary[237371650U] = PropTag.Message.ProofOfSubmission.PropInfo;
			dictionary[237437186U] = PropTag.Message.NTSecurityDescriptor.PropInfo;
			dictionary[237502495U] = PropTag.Message.PrimarySendAccount.PropInfo;
			dictionary[237568031U] = PropTag.Message.NextSendAccount.PropInfo;
			dictionary[237699075U] = PropTag.Message.TodoItemFlags.PropInfo;
			dictionary[237764866U] = PropTag.Message.SwappedTODOStore.PropInfo;
			dictionary[237830402U] = PropTag.Message.SwappedTODOData.PropInfo;
			dictionary[237961219U] = PropTag.Message.IMAPId.PropInfo;
			dictionary[238092290U] = PropTag.Message.OriginalSourceServerVersion.PropInfo;
			dictionary[238551043U] = PropTag.Message.ReplFlags.PropInfo;
			dictionary[238682125U] = PropTag.Message.MessageDeepAttachments.PropInfo;
			dictionary[239075586U] = PropTag.Message.SenderGuid.PropInfo;
			dictionary[239141122U] = PropTag.Message.SentRepresentingGuid.PropInfo;
			dictionary[239206658U] = PropTag.Message.OriginalSenderGuid.PropInfo;
			dictionary[239272194U] = PropTag.Message.OriginalSentRepresentingGuid.PropInfo;
			dictionary[239337730U] = PropTag.Message.ReadReceiptGuid.PropInfo;
			dictionary[239403266U] = PropTag.Message.ReportGuid.PropInfo;
			dictionary[239468802U] = PropTag.Message.OriginatorGuid.PropInfo;
			dictionary[239534338U] = PropTag.Message.ReportDestinationGuid.PropInfo;
			dictionary[239599874U] = PropTag.Message.OriginalAuthorGuid.PropInfo;
			dictionary[239665410U] = PropTag.Message.ReceivedByGuid.PropInfo;
			dictionary[239730946U] = PropTag.Message.ReceivedRepresentingGuid.PropInfo;
			dictionary[239796482U] = PropTag.Message.CreatorGuid.PropInfo;
			dictionary[239862018U] = PropTag.Message.LastModifierGuid.PropInfo;
			dictionary[239927554U] = PropTag.Message.SenderSID.PropInfo;
			dictionary[239993090U] = PropTag.Message.SentRepresentingSID.PropInfo;
			dictionary[240058626U] = PropTag.Message.OriginalSenderSid.PropInfo;
			dictionary[240124162U] = PropTag.Message.OriginalSentRepresentingSid.PropInfo;
			dictionary[240189698U] = PropTag.Message.ReadReceiptSid.PropInfo;
			dictionary[240255234U] = PropTag.Message.ReportSid.PropInfo;
			dictionary[240320770U] = PropTag.Message.OriginatorSid.PropInfo;
			dictionary[240386306U] = PropTag.Message.ReportDestinationSid.PropInfo;
			dictionary[240451842U] = PropTag.Message.OriginalAuthorSid.PropInfo;
			dictionary[240517378U] = PropTag.Message.RcvdBySid.PropInfo;
			dictionary[240582914U] = PropTag.Message.RcvdRepresentingSid.PropInfo;
			dictionary[240648450U] = PropTag.Message.CreatorSID.PropInfo;
			dictionary[240713986U] = PropTag.Message.LastModifierSid.PropInfo;
			dictionary[240779522U] = PropTag.Message.RecipientCAI.PropInfo;
			dictionary[240845058U] = PropTag.Message.ConversationCreatorSID.PropInfo;
			dictionary[241238019U] = PropTag.Message.URLCompNamePostfix.PropInfo;
			dictionary[241303563U] = PropTag.Message.URLCompNameSet.PropInfo;
			dictionary[241762315U] = PropTag.Message.Read.PropInfo;
			dictionary[241958943U] = PropTag.Message.CreatorSidAsXML.PropInfo;
			dictionary[242024479U] = PropTag.Message.LastModifierSidAsXML.PropInfo;
			dictionary[242090015U] = PropTag.Message.SenderSIDAsXML.PropInfo;
			dictionary[242155551U] = PropTag.Message.SentRepresentingSidAsXML.PropInfo;
			dictionary[242221087U] = PropTag.Message.OriginalSenderSIDAsXML.PropInfo;
			dictionary[242286623U] = PropTag.Message.OriginalSentRepresentingSIDAsXML.PropInfo;
			dictionary[242352159U] = PropTag.Message.ReadReceiptSIDAsXML.PropInfo;
			dictionary[242417695U] = PropTag.Message.ReportSIDAsXML.PropInfo;
			dictionary[242483231U] = PropTag.Message.OriginatorSidAsXML.PropInfo;
			dictionary[242548767U] = PropTag.Message.ReportDestinationSIDAsXML.PropInfo;
			dictionary[242614303U] = PropTag.Message.OriginalAuthorSIDAsXML.PropInfo;
			dictionary[242679839U] = PropTag.Message.ReceivedBySIDAsXML.PropInfo;
			dictionary[242745375U] = PropTag.Message.ReceivedRepersentingSIDAsXML.PropInfo;
			dictionary[242810883U] = PropTag.Message.TrustSender.PropInfo;
			dictionary[243859487U] = PropTag.Message.SenderSMTPAddress.PropInfo;
			dictionary[243925023U] = PropTag.Message.SentRepresentingSMTPAddress.PropInfo;
			dictionary[243990559U] = PropTag.Message.OriginalSenderSMTPAddress.PropInfo;
			dictionary[244056095U] = PropTag.Message.OriginalSentRepresentingSMTPAddress.PropInfo;
			dictionary[244121631U] = PropTag.Message.ReadReceiptSMTPAddress.PropInfo;
			dictionary[244187167U] = PropTag.Message.ReportSMTPAddress.PropInfo;
			dictionary[244252703U] = PropTag.Message.OriginatorSMTPAddress.PropInfo;
			dictionary[244318239U] = PropTag.Message.ReportDestinationSMTPAddress.PropInfo;
			dictionary[244383775U] = PropTag.Message.OriginalAuthorSMTPAddress.PropInfo;
			dictionary[244449311U] = PropTag.Message.ReceivedBySMTPAddress.PropInfo;
			dictionary[244514847U] = PropTag.Message.ReceivedRepresentingSMTPAddress.PropInfo;
			dictionary[244580383U] = PropTag.Message.CreatorSMTPAddress.PropInfo;
			dictionary[244645919U] = PropTag.Message.LastModifierSMTPAddress.PropInfo;
			dictionary[244711682U] = PropTag.Message.VirusScannerStamp.PropInfo;
			dictionary[244715551U] = PropTag.Message.VirusTransportStamp.PropInfo;
			dictionary[244776991U] = PropTag.Message.AddrTo.PropInfo;
			dictionary[244842527U] = PropTag.Message.AddrCc.PropInfo;
			dictionary[244908290U] = PropTag.Message.ExtendedRuleActions.PropInfo;
			dictionary[244973826U] = PropTag.Message.ExtendedRuleCondition.PropInfo;
			dictionary[245305375U] = PropTag.Message.EntourageSentHistory.PropInfo;
			dictionary[245497859U] = PropTag.Message.ProofInProgress.PropInfo;
			dictionary[245694495U] = PropTag.Message.SearchAttachmentsOLK.PropInfo;
			dictionary[245760031U] = PropTag.Message.SearchRecipEmailTo.PropInfo;
			dictionary[245825567U] = PropTag.Message.SearchRecipEmailCc.PropInfo;
			dictionary[245891103U] = PropTag.Message.SearchRecipEmailBcc.PropInfo;
			dictionary[246022147U] = PropTag.Message.SFGAOFlags.PropInfo;
			dictionary[246153247U] = PropTag.Message.SearchFullTextSubject.PropInfo;
			dictionary[246218783U] = PropTag.Message.SearchFullTextBody.PropInfo;
			dictionary[246284319U] = PropTag.Message.FullTextConversationIndex.PropInfo;
			dictionary[246349855U] = PropTag.Message.SearchAllIndexedProps.PropInfo;
			dictionary[246480927U] = PropTag.Message.SearchRecipients.PropInfo;
			dictionary[246546463U] = PropTag.Message.SearchRecipientsTo.PropInfo;
			dictionary[246611999U] = PropTag.Message.SearchRecipientsCc.PropInfo;
			dictionary[246677535U] = PropTag.Message.SearchRecipientsBcc.PropInfo;
			dictionary[246743071U] = PropTag.Message.SearchAccountTo.PropInfo;
			dictionary[246808607U] = PropTag.Message.SearchAccountCc.PropInfo;
			dictionary[246874143U] = PropTag.Message.SearchAccountBcc.PropInfo;
			dictionary[246939679U] = PropTag.Message.SearchEmailAddressTo.PropInfo;
			dictionary[247005215U] = PropTag.Message.SearchEmailAddressCc.PropInfo;
			dictionary[247070751U] = PropTag.Message.SearchEmailAddressBcc.PropInfo;
			dictionary[247136287U] = PropTag.Message.SearchSmtpAddressTo.PropInfo;
			dictionary[247201823U] = PropTag.Message.SearchSmtpAddressCc.PropInfo;
			dictionary[247267359U] = PropTag.Message.SearchSmtpAddressBcc.PropInfo;
			dictionary[247332895U] = PropTag.Message.SearchSender.PropInfo;
			dictionary[248315915U] = PropTag.Message.IsIRMMessage.PropInfo;
			dictionary[248381451U] = PropTag.Message.SearchIsPartiallyIndexed.PropInfo;
			dictionary[251723840U] = PropTag.Message.RenewTime.PropInfo;
			dictionary[251789376U] = PropTag.Message.DeliveryOrRenewTime.PropInfo;
			dictionary[251855106U] = PropTag.Message.ConversationFamilyId.PropInfo;
			dictionary[251920387U] = PropTag.Message.LikeCount.PropInfo;
			dictionary[251985922U] = PropTag.Message.RichContentDeprecated.PropInfo;
			dictionary[252051459U] = PropTag.Message.PeopleCentricConversationId.PropInfo;
			dictionary[252575775U] = PropTag.Message.DiscoveryAnnotation.PropInfo;
			dictionary[267649027U] = PropTag.Message.Access.PropInfo;
			dictionary[267714563U] = PropTag.Message.RowType.PropInfo;
			dictionary[267780354U] = PropTag.Message.InstanceKey.PropInfo;
			dictionary[267780347U] = PropTag.Message.InstanceKeySvrEid.PropInfo;
			dictionary[267845635U] = PropTag.Message.AccessLevel.PropInfo;
			dictionary[267911426U] = PropTag.Message.MappingSignature.PropInfo;
			dictionary[267976962U] = PropTag.Message.RecordKey.PropInfo;
			dictionary[267976955U] = PropTag.Message.RecordKeySvrEid.PropInfo;
			dictionary[268042498U] = PropTag.Message.StoreRecordKey.PropInfo;
			dictionary[268108034U] = PropTag.Message.StoreEntryId.PropInfo;
			dictionary[268173570U] = PropTag.Message.MiniIcon.PropInfo;
			dictionary[268239106U] = PropTag.Message.Icon.PropInfo;
			dictionary[268304387U] = PropTag.Message.ObjectType.PropInfo;
			dictionary[268370178U] = PropTag.Message.EntryId.PropInfo;
			dictionary[268370171U] = PropTag.Message.EntryIdSvrEid.PropInfo;
			dictionary[268435487U] = PropTag.Message.BodyUnicode.PropInfo;
			dictionary[268501023U] = PropTag.Message.ReportText.PropInfo;
			dictionary[268566786U] = PropTag.Message.OriginatorAndDLExpansionHistory.PropInfo;
			dictionary[268632322U] = PropTag.Message.ReportingDLName.PropInfo;
			dictionary[268697858U] = PropTag.Message.ReportingMTACertificate.PropInfo;
			dictionary[268828675U] = PropTag.Message.RtfSyncBodyCrc.PropInfo;
			dictionary[268894211U] = PropTag.Message.RtfSyncBodyCount.PropInfo;
			dictionary[268959775U] = PropTag.Message.RtfSyncBodyTag.PropInfo;
			dictionary[269025538U] = PropTag.Message.RtfCompressed.PropInfo;
			dictionary[269091074U] = PropTag.Message.AlternateBestBody.PropInfo;
			dictionary[269484035U] = PropTag.Message.RtfSyncPrefixCount.PropInfo;
			dictionary[269549571U] = PropTag.Message.RtfSyncTrailingCount.PropInfo;
			dictionary[269615362U] = PropTag.Message.OriginallyIntendedRecipientEntryId.PropInfo;
			dictionary[269680898U] = PropTag.Message.BodyHtml.PropInfo;
			dictionary[269680671U] = PropTag.Message.BodyHtmlUnicode.PropInfo;
			dictionary[269746207U] = PropTag.Message.BodyContentLocation.PropInfo;
			dictionary[269811743U] = PropTag.Message.BodyContentId.PropInfo;
			dictionary[269877251U] = PropTag.Message.NativeBodyInfo.PropInfo;
			dictionary[269877250U] = PropTag.Message.NativeBodyType.PropInfo;
			dictionary[269877506U] = PropTag.Message.NativeBody.PropInfo;
			dictionary[269943042U] = PropTag.Message.AnnotationToken.PropInfo;
			dictionary[271581215U] = PropTag.Message.InternetApproved.PropInfo;
			dictionary[271777823U] = PropTag.Message.InternetFollowupTo.PropInfo;
			dictionary[271908895U] = PropTag.Message.InternetMessageId.PropInfo;
			dictionary[271974431U] = PropTag.Message.InetNewsgroups.PropInfo;
			dictionary[272171039U] = PropTag.Message.InternetReferences.PropInfo;
			dictionary[272433410U] = PropTag.Message.PostReplyFolderEntries.PropInfo;
			dictionary[272629791U] = PropTag.Message.NNTPXRef.PropInfo;
			dictionary[272760863U] = PropTag.Message.InReplyToId.PropInfo;
			dictionary[273023007U] = PropTag.Message.OriginalInternetMessageId.PropInfo;
			dictionary[276824067U] = PropTag.Message.IconIndex.PropInfo;
			dictionary[276889603U] = PropTag.Message.LastVerbExecuted.PropInfo;
			dictionary[276955200U] = PropTag.Message.LastVerbExecutionTime.PropInfo;
			dictionary[277086211U] = PropTag.Message.Relevance.PropInfo;
			dictionary[277872643U] = PropTag.Message.FlagStatus.PropInfo;
			dictionary[277938240U] = PropTag.Message.FlagCompleteTime.PropInfo;
			dictionary[278003715U] = PropTag.Message.FormatPT.PropInfo;
			dictionary[278200323U] = PropTag.Message.FollowupIcon.PropInfo;
			dictionary[278265859U] = PropTag.Message.BlockStatus.PropInfo;
			dictionary[278331395U] = PropTag.Message.ItemTempFlags.PropInfo;
			dictionary[281018626U] = PropTag.Message.SMTPTempTblData.PropInfo;
			dictionary[281083907U] = PropTag.Message.SMTPTempTblData2.PropInfo;
			dictionary[281149698U] = PropTag.Message.SMTPTempTblData3.PropInfo;
			dictionary[281411615U] = PropTag.Message.DAVSubmitData.PropInfo;
			dictionary[284164354U] = PropTag.Message.ImapCachedMsgSize.PropInfo;
			dictionary[284295179U] = PropTag.Message.DisableFullFidelity.PropInfo;
			dictionary[284360735U] = PropTag.Message.URLCompName.PropInfo;
			dictionary[284426251U] = PropTag.Message.AttrHidden.PropInfo;
			dictionary[284491787U] = PropTag.Message.AttrSystem.PropInfo;
			dictionary[284557323U] = PropTag.Message.AttrReadOnly.PropInfo;
			dictionary[302256130U] = PropTag.Message.PredictedActions.PropInfo;
			dictionary[302321666U] = PropTag.Message.GroupingActions.PropInfo;
			dictionary[302383107U] = PropTag.Message.PredictedActionsSummary.PropInfo;
			dictionary[302448898U] = PropTag.Message.PredictedActionsThresholds.PropInfo;
			dictionary[302448651U] = PropTag.Message.IsClutter.PropInfo;
			dictionary[302514434U] = PropTag.Message.InferencePredictedReplyForwardReasons.PropInfo;
			dictionary[302579970U] = PropTag.Message.InferencePredictedDeleteReasons.PropInfo;
			dictionary[302645506U] = PropTag.Message.InferencePredictedIgnoreReasons.PropInfo;
			dictionary[302711042U] = PropTag.Message.OriginalDeliveryFolderInfo.PropInfo;
			dictionary[805306371U] = PropTag.Message.RowId.PropInfo;
			dictionary[805371935U] = PropTag.Message.DisplayName.PropInfo;
			dictionary[805437471U] = PropTag.Message.AddressType.PropInfo;
			dictionary[805503007U] = PropTag.Message.EmailAddress.PropInfo;
			dictionary[805568543U] = PropTag.Message.Comment.PropInfo;
			dictionary[805634051U] = PropTag.Message.Depth.PropInfo;
			dictionary[805765184U] = PropTag.Message.CreationTime.PropInfo;
			dictionary[805830720U] = PropTag.Message.LastModificationTime.PropInfo;
			dictionary[806027522U] = PropTag.Message.SearchKey.PropInfo;
			dictionary[806027515U] = PropTag.Message.SearchKeySvrEid.PropInfo;
			dictionary[806355202U] = PropTag.Message.TargetEntryId.PropInfo;
			dictionary[806551810U] = PropTag.Message.ConversationId.PropInfo;
			dictionary[806617346U] = PropTag.Message.BodyTag.PropInfo;
			dictionary[806682644U] = PropTag.Message.ConversationIndexTrackingObsolete.PropInfo;
			dictionary[806748171U] = PropTag.Message.ConversationIndexTracking.PropInfo;
			dictionary[806879490U] = PropTag.Message.ArchiveTag.PropInfo;
			dictionary[806945026U] = PropTag.Message.PolicyTag.PropInfo;
			dictionary[807010307U] = PropTag.Message.RetentionPeriod.PropInfo;
			dictionary[807076098U] = PropTag.Message.StartDateEtc.PropInfo;
			dictionary[807141440U] = PropTag.Message.RetentionDate.PropInfo;
			dictionary[807206915U] = PropTag.Message.RetentionFlags.PropInfo;
			dictionary[807272451U] = PropTag.Message.ArchivePeriod.PropInfo;
			dictionary[807338048U] = PropTag.Message.ArchiveDate.PropInfo;
			dictionary[855703583U] = PropTag.Message.FormVersion.PropInfo;
			dictionary[855769160U] = PropTag.Message.FormCLSID.PropInfo;
			dictionary[855834655U] = PropTag.Message.FormContactName.PropInfo;
			dictionary[855900191U] = PropTag.Message.FormCategory.PropInfo;
			dictionary[855965727U] = PropTag.Message.FormCategorySub.PropInfo;
			dictionary[856096779U] = PropTag.Message.FormHidden.PropInfo;
			dictionary[856162335U] = PropTag.Message.FormDesignerName.PropInfo;
			dictionary[856227912U] = PropTag.Message.FormDesignerGuid.PropInfo;
			dictionary[856293379U] = PropTag.Message.FormMessageBehavior.PropInfo;
			dictionary[873267203U] = PropTag.Message.StoreSupportMask.PropInfo;
			dictionary[873726210U] = PropTag.Message.MdbProvider.PropInfo;
			dictionary[878706752U] = PropTag.Message.EventEmailReminderTimer.PropInfo;
			dictionary[906100739U] = PropTag.Message.ContentCount.PropInfo;
			dictionary[906166275U] = PropTag.Message.UnreadCount.PropInfo;
			dictionary[906166292U] = PropTag.Message.UnreadCountInt64.PropInfo;
			dictionary[906297357U] = PropTag.Message.DetailsTable.PropInfo;
			dictionary[920387842U] = PropTag.Message.AppointmentColorName.PropInfo;
			dictionary[922943519U] = PropTag.Message.ContentId.PropInfo;
			dictionary[923205663U] = PropTag.Message.MimeUrl.PropInfo;
			dictionary[956301315U] = PropTag.Message.DisplayType.PropInfo;
			dictionary[972947487U] = PropTag.Message.SmtpAddress.PropInfo;
			dictionary[973013023U] = PropTag.Message.SimpleDisplayName.PropInfo;
			dictionary[973078559U] = PropTag.Message.Account.PropInfo;
			dictionary[973144322U] = PropTag.Message.AlternateRecipient.PropInfo;
			dictionary[973209631U] = PropTag.Message.CallbackTelephoneNumber.PropInfo;
			dictionary[973275147U] = PropTag.Message.ConversionProhibited.PropInfo;
			dictionary[973406239U] = PropTag.Message.Generation.PropInfo;
			dictionary[973471775U] = PropTag.Message.GivenName.PropInfo;
			dictionary[973537311U] = PropTag.Message.GovernmentIDNumber.PropInfo;
			dictionary[973602847U] = PropTag.Message.BusinessTelephoneNumber.PropInfo;
			dictionary[973668383U] = PropTag.Message.HomeTelephoneNumber.PropInfo;
			dictionary[973733919U] = PropTag.Message.Initials.PropInfo;
			dictionary[973799455U] = PropTag.Message.Keyword.PropInfo;
			dictionary[973864991U] = PropTag.Message.Language.PropInfo;
			dictionary[973930527U] = PropTag.Message.Location.PropInfo;
			dictionary[973996043U] = PropTag.Message.MailPermission.PropInfo;
			dictionary[974061599U] = PropTag.Message.MHSCommonName.PropInfo;
			dictionary[974127135U] = PropTag.Message.OrganizationalIDNumber.PropInfo;
			dictionary[974192671U] = PropTag.Message.SurName.PropInfo;
			dictionary[974258434U] = PropTag.Message.OriginalEntryId.PropInfo;
			dictionary[974323743U] = PropTag.Message.OriginalDisplayName.PropInfo;
			dictionary[974389506U] = PropTag.Message.OriginalSearchKey.PropInfo;
			dictionary[974454815U] = PropTag.Message.PostalAddress.PropInfo;
			dictionary[974520351U] = PropTag.Message.CompanyName.PropInfo;
			dictionary[974585887U] = PropTag.Message.Title.PropInfo;
			dictionary[974651423U] = PropTag.Message.DepartmentName.PropInfo;
			dictionary[974716959U] = PropTag.Message.OfficeLocation.PropInfo;
			dictionary[974782495U] = PropTag.Message.PrimaryTelephoneNumber.PropInfo;
			dictionary[974848031U] = PropTag.Message.Business2TelephoneNumber.PropInfo;
			dictionary[974852127U] = PropTag.Message.Business2TelephoneNumberMv.PropInfo;
			dictionary[974913567U] = PropTag.Message.MobileTelephoneNumber.PropInfo;
			dictionary[974979103U] = PropTag.Message.RadioTelephoneNumber.PropInfo;
			dictionary[975044639U] = PropTag.Message.CarTelephoneNumber.PropInfo;
			dictionary[975110175U] = PropTag.Message.OtherTelephoneNumber.PropInfo;
			dictionary[975175711U] = PropTag.Message.TransmitableDisplayName.PropInfo;
			dictionary[975241247U] = PropTag.Message.PagerTelephoneNumber.PropInfo;
			dictionary[975307010U] = PropTag.Message.UserCertificate.PropInfo;
			dictionary[975372319U] = PropTag.Message.PrimaryFaxNumber.PropInfo;
			dictionary[975437855U] = PropTag.Message.BusinessFaxNumber.PropInfo;
			dictionary[975503391U] = PropTag.Message.HomeFaxNumber.PropInfo;
			dictionary[975568927U] = PropTag.Message.Country.PropInfo;
			dictionary[975634463U] = PropTag.Message.Locality.PropInfo;
			dictionary[975699999U] = PropTag.Message.StateOrProvince.PropInfo;
			dictionary[975765535U] = PropTag.Message.StreetAddress.PropInfo;
			dictionary[975831071U] = PropTag.Message.PostalCode.PropInfo;
			dictionary[975896607U] = PropTag.Message.PostOfficeBox.PropInfo;
			dictionary[975962143U] = PropTag.Message.TelexNumber.PropInfo;
			dictionary[976027679U] = PropTag.Message.ISDNNumber.PropInfo;
			dictionary[976093215U] = PropTag.Message.AssistantTelephoneNumber.PropInfo;
			dictionary[976158751U] = PropTag.Message.Home2TelephoneNumber.PropInfo;
			dictionary[976162847U] = PropTag.Message.Home2TelephoneNumberMv.PropInfo;
			dictionary[976224287U] = PropTag.Message.Assistant.PropInfo;
			dictionary[977272843U] = PropTag.Message.SendRichInfo.PropInfo;
			dictionary[977338432U] = PropTag.Message.WeddingAnniversary.PropInfo;
			dictionary[977403968U] = PropTag.Message.Birthday.PropInfo;
			dictionary[977469471U] = PropTag.Message.Hobbies.PropInfo;
			dictionary[977535007U] = PropTag.Message.MiddleName.PropInfo;
			dictionary[977600543U] = PropTag.Message.DisplayNamePrefix.PropInfo;
			dictionary[977666079U] = PropTag.Message.Profession.PropInfo;
			dictionary[977731615U] = PropTag.Message.ReferredByName.PropInfo;
			dictionary[977797151U] = PropTag.Message.SpouseName.PropInfo;
			dictionary[977862687U] = PropTag.Message.ComputerNetworkName.PropInfo;
			dictionary[977928223U] = PropTag.Message.CustomerId.PropInfo;
			dictionary[977993759U] = PropTag.Message.TTYTDDPhoneNumber.PropInfo;
			dictionary[978059295U] = PropTag.Message.FTPSite.PropInfo;
			dictionary[978124802U] = PropTag.Message.Gender.PropInfo;
			dictionary[978190367U] = PropTag.Message.ManagerName.PropInfo;
			dictionary[978255903U] = PropTag.Message.NickName.PropInfo;
			dictionary[978321439U] = PropTag.Message.PersonalHomePage.PropInfo;
			dictionary[978386975U] = PropTag.Message.BusinessHomePage.PropInfo;
			dictionary[978452552U] = PropTag.Message.ContactVersion.PropInfo;
			dictionary[978522370U] = PropTag.Message.ContactEntryIds.PropInfo;
			dictionary[978587679U] = PropTag.Message.ContactAddressTypes.PropInfo;
			dictionary[978649091U] = PropTag.Message.ContactDefaultAddressIndex.PropInfo;
			dictionary[978718751U] = PropTag.Message.ContactEmailAddress.PropInfo;
			dictionary[978780191U] = PropTag.Message.CompanyMainPhoneNumber.PropInfo;
			dictionary[978849823U] = PropTag.Message.ChildrensNames.PropInfo;
			dictionary[978911263U] = PropTag.Message.HomeAddressCity.PropInfo;
			dictionary[978976799U] = PropTag.Message.HomeAddressCountry.PropInfo;
			dictionary[979042335U] = PropTag.Message.HomeAddressPostalCode.PropInfo;
			dictionary[979107871U] = PropTag.Message.HomeAddressStateOrProvince.PropInfo;
			dictionary[979173407U] = PropTag.Message.HomeAddressStreet.PropInfo;
			dictionary[979238943U] = PropTag.Message.HomeAddressPostOfficeBox.PropInfo;
			dictionary[979304479U] = PropTag.Message.OtherAddressCity.PropInfo;
			dictionary[979370015U] = PropTag.Message.OtherAddressCountry.PropInfo;
			dictionary[979435551U] = PropTag.Message.OtherAddressPostalCode.PropInfo;
			dictionary[979501087U] = PropTag.Message.OtherAddressStateOrProvince.PropInfo;
			dictionary[979566623U] = PropTag.Message.OtherAddressStreet.PropInfo;
			dictionary[979632159U] = PropTag.Message.OtherAddressPostOfficeBox.PropInfo;
			dictionary[980422914U] = PropTag.Message.UserX509CertificateABSearchPath.PropInfo;
			dictionary[980484099U] = PropTag.Message.SendInternetEncoding.PropInfo;
			dictionary[980811807U] = PropTag.Message.PartnerNetworkId.PropInfo;
			dictionary[980877343U] = PropTag.Message.PartnerNetworkUserId.PropInfo;
			dictionary[980942879U] = PropTag.Message.PartnerNetworkThumbnailPhotoUrl.PropInfo;
			dictionary[981008415U] = PropTag.Message.PartnerNetworkProfilePhotoUrl.PropInfo;
			dictionary[981073951U] = PropTag.Message.PartnerNetworkContactType.PropInfo;
			dictionary[981139459U] = PropTag.Message.RelevanceScore.PropInfo;
			dictionary[981205003U] = PropTag.Message.IsDistributionListContact.PropInfo;
			dictionary[981270539U] = PropTag.Message.IsPromotedContact.PropInfo;
			dictionary[1006501919U] = PropTag.Message.OrgUnitName.PropInfo;
			dictionary[1006567455U] = PropTag.Message.OrganizationName.PropInfo;
			dictionary[1023410196U] = PropTag.Message.TestBlobProperty.PropInfo;
			dictionary[1023934722U] = PropTag.Message.FilteringHooks.PropInfo;
			dictionary[1033830403U] = PropTag.Message.MailboxPartitionNumber.PropInfo;
			dictionary[1033895939U] = PropTag.Message.MailboxNumberInternal.PropInfo;
			dictionary[1034223647U] = PropTag.Message.VirtualParentDisplay.PropInfo;
			dictionary[1034420235U] = PropTag.Message.InternalConversationIndexTracking.PropInfo;
			dictionary[1034486018U] = PropTag.Message.InternalConversationIndex.PropInfo;
			dictionary[1034551554U] = PropTag.Message.ConversationItemConversationId.PropInfo;
			dictionary[1034616852U] = PropTag.Message.VirtualUnreadMessageCount.PropInfo;
			dictionary[1034682379U] = PropTag.Message.VirtualIsRead.PropInfo;
			dictionary[1034747915U] = PropTag.Message.IsReadColumn.PropInfo;
			dictionary[1034879234U] = PropTag.Message.Internal9ByteChangeNumber.PropInfo;
			dictionary[1034944770U] = PropTag.Message.Internal9ByteReadCnNew.PropInfo;
			dictionary[1035010059U] = PropTag.Message.CategoryHeaderLevelStub1.PropInfo;
			dictionary[1035075595U] = PropTag.Message.CategoryHeaderLevelStub2.PropInfo;
			dictionary[1035141131U] = PropTag.Message.CategoryHeaderLevelStub3.PropInfo;
			dictionary[1035206914U] = PropTag.Message.CategoryHeaderAggregateProp0.PropInfo;
			dictionary[1035272450U] = PropTag.Message.CategoryHeaderAggregateProp1.PropInfo;
			dictionary[1035337986U] = PropTag.Message.CategoryHeaderAggregateProp2.PropInfo;
			dictionary[1035403522U] = PropTag.Message.CategoryHeaderAggregateProp3.PropInfo;
			dictionary[1035796483U] = PropTag.Message.MessageFlagsActual.PropInfo;
			dictionary[1035862274U] = PropTag.Message.InternalChangeKey.PropInfo;
			dictionary[1035927810U] = PropTag.Message.InternalSourceKey.PropInfo;
			dictionary[1040843010U] = PropTag.Message.HeaderFolderEntryId.PropInfo;
			dictionary[1040908291U] = PropTag.Message.RemoteProgress.PropInfo;
			dictionary[1040973855U] = PropTag.Message.RemoteProgressText.PropInfo;
			dictionary[1065877524U] = PropTag.Message.VID.PropInfo;
			dictionary[1065943298U] = PropTag.Message.GVid.PropInfo;
			dictionary[1066008834U] = PropTag.Message.GDID.PropInfo;
			dictionary[1066729730U] = PropTag.Message.XVid.PropInfo;
			dictionary[1066795266U] = PropTag.Message.GDefVid.PropInfo;
			dictionary[1069678603U] = PropTag.Message.PrimaryMailboxOverQuota.PropInfo;
			dictionary[1070924034U] = PropTag.Message.InternalPostReply.PropInfo;
			dictionary[1071120415U] = PropTag.Message.PreviewUnread.PropInfo;
			dictionary[1071185951U] = PropTag.Message.Preview.PropInfo;
			dictionary[1071513603U] = PropTag.Message.InternetCPID.PropInfo;
			dictionary[1071579139U] = PropTag.Message.AutoResponseSuppress.PropInfo;
			dictionary[1072300043U] = PropTag.Message.HasDAMs.PropInfo;
			dictionary[1072365571U] = PropTag.Message.DeferredSendNumber.PropInfo;
			dictionary[1072431107U] = PropTag.Message.DeferredSendUnits.PropInfo;
			dictionary[1072496643U] = PropTag.Message.ExpiryNumber.PropInfo;
			dictionary[1072562179U] = PropTag.Message.ExpiryUnits.PropInfo;
			dictionary[1072627776U] = PropTag.Message.DeferredSendTime.PropInfo;
			dictionary[1072758787U] = PropTag.Message.MessageLocaleId.PropInfo;
			dictionary[1072824578U] = PropTag.Message.RuleTriggerHistory.PropInfo;
			dictionary[1072890114U] = PropTag.Message.MoveToStoreEid.PropInfo;
			dictionary[1072955650U] = PropTag.Message.MoveToFolderEid.PropInfo;
			dictionary[1073020931U] = PropTag.Message.StorageQuotaLimit.PropInfo;
			dictionary[1073086467U] = PropTag.Message.ExcessStorageUsed.PropInfo;
			dictionary[1073152031U] = PropTag.Message.ServerGeneratingQuotaMsg.PropInfo;
			dictionary[1073217567U] = PropTag.Message.CreatorName.PropInfo;
			dictionary[1073283330U] = PropTag.Message.CreatorEntryId.PropInfo;
			dictionary[1073348639U] = PropTag.Message.LastModifierName.PropInfo;
			dictionary[1073414402U] = PropTag.Message.LastModifierEntryId.PropInfo;
			dictionary[1073545219U] = PropTag.Message.MessageCodePage.PropInfo;
			dictionary[1073610755U] = PropTag.Message.QuotaType.PropInfo;
			dictionary[1073676299U] = PropTag.Message.IsPublicFolderQuotaMessage.PropInfo;
			dictionary[1073741827U] = PropTag.Message.NewAttach.PropInfo;
			dictionary[1073807363U] = PropTag.Message.StartEmbed.PropInfo;
			dictionary[1073872899U] = PropTag.Message.EndEmbed.PropInfo;
			dictionary[1073938435U] = PropTag.Message.StartRecip.PropInfo;
			dictionary[1074003971U] = PropTag.Message.EndRecip.PropInfo;
			dictionary[1074069507U] = PropTag.Message.EndCcRecip.PropInfo;
			dictionary[1074135043U] = PropTag.Message.EndBccRecip.PropInfo;
			dictionary[1074200579U] = PropTag.Message.EndP1Recip.PropInfo;
			dictionary[1074266143U] = PropTag.Message.DNPrefix.PropInfo;
			dictionary[1074331651U] = PropTag.Message.StartTopFolder.PropInfo;
			dictionary[1074397187U] = PropTag.Message.StartSubFolder.PropInfo;
			dictionary[1074462723U] = PropTag.Message.EndFolder.PropInfo;
			dictionary[1074528259U] = PropTag.Message.StartMessage.PropInfo;
			dictionary[1074593795U] = PropTag.Message.EndMessage.PropInfo;
			dictionary[1074659331U] = PropTag.Message.EndAttach.PropInfo;
			dictionary[1074724867U] = PropTag.Message.EcWarning.PropInfo;
			dictionary[1074790403U] = PropTag.Message.StartFAIMessage.PropInfo;
			dictionary[1074856194U] = PropTag.Message.NewFXFolder.PropInfo;
			dictionary[1074921475U] = PropTag.Message.IncrSyncChange.PropInfo;
			dictionary[1074987011U] = PropTag.Message.IncrSyncDelete.PropInfo;
			dictionary[1075052547U] = PropTag.Message.IncrSyncEnd.PropInfo;
			dictionary[1075118083U] = PropTag.Message.IncrSyncMessage.PropInfo;
			dictionary[1075183619U] = PropTag.Message.FastTransferDelProp.PropInfo;
			dictionary[1075249410U] = PropTag.Message.IdsetGiven.PropInfo;
			dictionary[1075249155U] = PropTag.Message.IdsetGivenInt32.PropInfo;
			dictionary[1075314691U] = PropTag.Message.FastTransferErrorInfo.PropInfo;
			dictionary[1075380227U] = PropTag.Message.SenderFlags.PropInfo;
			dictionary[1075445763U] = PropTag.Message.SentRepresentingFlags.PropInfo;
			dictionary[1075511299U] = PropTag.Message.RcvdByFlags.PropInfo;
			dictionary[1075576835U] = PropTag.Message.RcvdRepresentingFlags.PropInfo;
			dictionary[1075642371U] = PropTag.Message.OriginalSenderFlags.PropInfo;
			dictionary[1075707907U] = PropTag.Message.OriginalSentRepresentingFlags.PropInfo;
			dictionary[1075773443U] = PropTag.Message.ReportFlags.PropInfo;
			dictionary[1075838979U] = PropTag.Message.ReadReceiptFlags.PropInfo;
			dictionary[1075904770U] = PropTag.Message.SoftDeletes.PropInfo;
			dictionary[1075970079U] = PropTag.Message.CreatorAddressType.PropInfo;
			dictionary[1076035615U] = PropTag.Message.CreatorEmailAddr.PropInfo;
			dictionary[1076101151U] = PropTag.Message.LastModifierAddressType.PropInfo;
			dictionary[1076166687U] = PropTag.Message.LastModifierEmailAddr.PropInfo;
			dictionary[1076232223U] = PropTag.Message.ReportAddressType.PropInfo;
			dictionary[1076297759U] = PropTag.Message.ReportEmailAddress.PropInfo;
			dictionary[1076363295U] = PropTag.Message.ReportDisplayName.PropInfo;
			dictionary[1076428831U] = PropTag.Message.ReadReceiptAddressType.PropInfo;
			dictionary[1076494367U] = PropTag.Message.ReadReceiptEmailAddress.PropInfo;
			dictionary[1076559903U] = PropTag.Message.ReadReceiptDisplayName.PropInfo;
			dictionary[1076691202U] = PropTag.Message.IdsetRead.PropInfo;
			dictionary[1076756738U] = PropTag.Message.IdsetUnread.PropInfo;
			dictionary[1076822019U] = PropTag.Message.IncrSyncRead.PropInfo;
			dictionary[1076887583U] = PropTag.Message.SenderSimpleDisplayName.PropInfo;
			dictionary[1076953119U] = PropTag.Message.SentRepresentingSimpleDisplayName.PropInfo;
			dictionary[1077018655U] = PropTag.Message.OriginalSenderSimpleDisplayName.PropInfo;
			dictionary[1077084191U] = PropTag.Message.OriginalSentRepresentingSimpleDisplayName.PropInfo;
			dictionary[1077149727U] = PropTag.Message.ReceivedBySimpleDisplayName.PropInfo;
			dictionary[1077215263U] = PropTag.Message.ReceivedRepresentingSimpleDisplayName.PropInfo;
			dictionary[1077280799U] = PropTag.Message.ReadReceiptSimpleDisplayName.PropInfo;
			dictionary[1077346335U] = PropTag.Message.ReportSimpleDisplayName.PropInfo;
			dictionary[1077411871U] = PropTag.Message.CreatorSimpleDisplayName.PropInfo;
			dictionary[1077477407U] = PropTag.Message.LastModifierSimpleDisplayName.PropInfo;
			dictionary[1077542915U] = PropTag.Message.IncrSyncStateBegin.PropInfo;
			dictionary[1077608451U] = PropTag.Message.IncrSyncStateEnd.PropInfo;
			dictionary[1077673987U] = PropTag.Message.IncrSyncImailStream.PropInfo;
			dictionary[1077870623U] = PropTag.Message.SenderOrgAddressType.PropInfo;
			dictionary[1077936159U] = PropTag.Message.SenderOrgEmailAddr.PropInfo;
			dictionary[1078001695U] = PropTag.Message.SentRepresentingOrgAddressType.PropInfo;
			dictionary[1078067231U] = PropTag.Message.SentRepresentingOrgEmailAddr.PropInfo;
			dictionary[1078132767U] = PropTag.Message.OriginalSenderOrgAddressType.PropInfo;
			dictionary[1078198303U] = PropTag.Message.OriginalSenderOrgEmailAddr.PropInfo;
			dictionary[1078263839U] = PropTag.Message.OriginalSentRepresentingOrgAddressType.PropInfo;
			dictionary[1078329375U] = PropTag.Message.OriginalSentRepresentingOrgEmailAddr.PropInfo;
			dictionary[1078394911U] = PropTag.Message.RcvdByOrgAddressType.PropInfo;
			dictionary[1078460447U] = PropTag.Message.RcvdByOrgEmailAddr.PropInfo;
			dictionary[1078525983U] = PropTag.Message.RcvdRepresentingOrgAddressType.PropInfo;
			dictionary[1078591519U] = PropTag.Message.RcvdRepresentingOrgEmailAddr.PropInfo;
			dictionary[1078657055U] = PropTag.Message.ReadReceiptOrgAddressType.PropInfo;
			dictionary[1078722591U] = PropTag.Message.ReadReceiptOrgEmailAddr.PropInfo;
			dictionary[1078788127U] = PropTag.Message.ReportOrgAddressType.PropInfo;
			dictionary[1078853663U] = PropTag.Message.ReportOrgEmailAddr.PropInfo;
			dictionary[1078919199U] = PropTag.Message.CreatorOrgAddressType.PropInfo;
			dictionary[1078984735U] = PropTag.Message.CreatorOrgEmailAddr.PropInfo;
			dictionary[1079050271U] = PropTag.Message.LastModifierOrgAddressType.PropInfo;
			dictionary[1079115807U] = PropTag.Message.LastModifierOrgEmailAddr.PropInfo;
			dictionary[1079181343U] = PropTag.Message.OriginatorOrgAddressType.PropInfo;
			dictionary[1079246879U] = PropTag.Message.OriginatorOrgEmailAddr.PropInfo;
			dictionary[1079312415U] = PropTag.Message.ReportDestinationOrgEmailType.PropInfo;
			dictionary[1079377951U] = PropTag.Message.ReportDestinationOrgEmailAddr.PropInfo;
			dictionary[1079443487U] = PropTag.Message.OriginalAuthorOrgAddressType.PropInfo;
			dictionary[1079509023U] = PropTag.Message.OriginalAuthorOrgEmailAddr.PropInfo;
			dictionary[1079574531U] = PropTag.Message.CreatorFlags.PropInfo;
			dictionary[1079640067U] = PropTag.Message.LastModifierFlags.PropInfo;
			dictionary[1079705603U] = PropTag.Message.OriginatorFlags.PropInfo;
			dictionary[1079771139U] = PropTag.Message.ReportDestinationFlags.PropInfo;
			dictionary[1079836675U] = PropTag.Message.OriginalAuthorFlags.PropInfo;
			dictionary[1079902239U] = PropTag.Message.OriginatorSimpleDisplayName.PropInfo;
			dictionary[1079967775U] = PropTag.Message.ReportDestinationSimpleDisplayName.PropInfo;
			dictionary[1080033311U] = PropTag.Message.OriginalAuthorSimpleDispName.PropInfo;
			dictionary[1080099074U] = PropTag.Message.OriginatorSearchKey.PropInfo;
			dictionary[1080164383U] = PropTag.Message.ReportDestinationAddressType.PropInfo;
			dictionary[1080229919U] = PropTag.Message.ReportDestinationEmailAddress.PropInfo;
			dictionary[1080295682U] = PropTag.Message.ReportDestinationSearchKey.PropInfo;
			dictionary[1080426499U] = PropTag.Message.IncrSyncImailStreamContinue.PropInfo;
			dictionary[1080492035U] = PropTag.Message.IncrSyncImailStreamCancel.PropInfo;
			dictionary[1081147395U] = PropTag.Message.IncrSyncImailStream2Continue.PropInfo;
			dictionary[1081344011U] = PropTag.Message.IncrSyncProgressMode.PropInfo;
			dictionary[1081409547U] = PropTag.Message.SyncProgressPerMsg.PropInfo;
			dictionary[1081475075U] = PropTag.Message.ContentFilterSCL.PropInfo;
			dictionary[1081737219U] = PropTag.Message.IncrSyncMsgPartial.PropInfo;
			dictionary[1081802755U] = PropTag.Message.IncrSyncGroupInfo.PropInfo;
			dictionary[1081868291U] = PropTag.Message.IncrSyncGroupId.PropInfo;
			dictionary[1081933827U] = PropTag.Message.IncrSyncChangePartial.PropInfo;
			dictionary[1082392579U] = PropTag.Message.ContentFilterPCL.PropInfo;
			dictionary[1476788235U] = PropTag.Message.DeliverAsRead.PropInfo;
			dictionary[1493303299U] = PropTag.Message.InetMailOverrideFormat.PropInfo;
			dictionary[1493762051U] = PropTag.Message.MessageEditorFormat.PropInfo;
			dictionary[1560346655U] = PropTag.Message.SenderSMTPAddressXSO.PropInfo;
			dictionary[1560412191U] = PropTag.Message.SentRepresentingSMTPAddressXSO.PropInfo;
			dictionary[1560477727U] = PropTag.Message.OriginalSenderSMTPAddressXSO.PropInfo;
			dictionary[1560543263U] = PropTag.Message.OriginalSentRepresentingSMTPAddressXSO.PropInfo;
			dictionary[1560608799U] = PropTag.Message.ReadReceiptSMTPAddressXSO.PropInfo;
			dictionary[1560674335U] = PropTag.Message.OriginalAuthorSMTPAddressXSO.PropInfo;
			dictionary[1560739871U] = PropTag.Message.ReceivedBySMTPAddressXSO.PropInfo;
			dictionary[1560805407U] = PropTag.Message.ReceivedRepresentingSMTPAddressXSO.PropInfo;
			dictionary[1608450051U] = PropTag.Message.RecipientOrder.PropInfo;
			dictionary[1608843295U] = PropTag.Message.RecipientSipUri.PropInfo;
			dictionary[1609957407U] = PropTag.Message.RecipientDisplayName.PropInfo;
			dictionary[1610023170U] = PropTag.Message.RecipientEntryId.PropInfo;
			dictionary[1610416131U] = PropTag.Message.RecipientFlags.PropInfo;
			dictionary[1610547203U] = PropTag.Message.RecipientTrackStatus.PropInfo;
			dictionary[1610678303U] = PropTag.Message.DotStuffState.PropInfo;
			dictionary[1644167171U] = PropTag.Message.InternetMessageIdHash.PropInfo;
			dictionary[1644232707U] = PropTag.Message.ConversationTopicHash.PropInfo;
			dictionary[1693450498U] = PropTag.Message.MimeSkeleton.PropInfo;
			dictionary[1707213058U] = PropTag.Message.ReplyTemplateId.PropInfo;
			dictionary[1707474947U] = PropTag.Message.SecureSubmitFlags.PropInfo;
			dictionary[1709179138U] = PropTag.Message.SourceKey.PropInfo;
			dictionary[1709244674U] = PropTag.Message.ParentSourceKey.PropInfo;
			dictionary[1709310210U] = PropTag.Message.ChangeKey.PropInfo;
			dictionary[1709375746U] = PropTag.Message.PredecessorChangeList.PropInfo;
			dictionary[1709768707U] = PropTag.Message.RuleMsgState.PropInfo;
			dictionary[1709834243U] = PropTag.Message.RuleMsgUserFlags.PropInfo;
			dictionary[1709899807U] = PropTag.Message.RuleMsgProvider.PropInfo;
			dictionary[1709965343U] = PropTag.Message.RuleMsgName.PropInfo;
			dictionary[1710030851U] = PropTag.Message.RuleMsgLevel.PropInfo;
			dictionary[1710096642U] = PropTag.Message.RuleMsgProviderData.PropInfo;
			dictionary[1710162178U] = PropTag.Message.RuleMsgActions.PropInfo;
			dictionary[1710227714U] = PropTag.Message.RuleMsgCondition.PropInfo;
			dictionary[1710292995U] = PropTag.Message.RuleMsgConditionLCID.PropInfo;
			dictionary[1710358530U] = PropTag.Message.RuleMsgVersion.PropInfo;
			dictionary[1710424067U] = PropTag.Message.RuleMsgSequence.PropInfo;
			dictionary[1710817538U] = PropTag.Message.LISSD.PropInfo;
			dictionary[1715732511U] = PropTag.Message.ReplicaServer.PropInfo;
			dictionary[1715863810U] = PropTag.Message.DAMOriginalEntryId.PropInfo;
			dictionary[1716125707U] = PropTag.Message.HasNamedProperties.PropInfo;
			dictionary[1716257026U] = PropTag.Message.FidMid.PropInfo;
			dictionary[1717108994U] = PropTag.Message.InternetContent.PropInfo;
			dictionary[1717239839U] = PropTag.Message.OriginatorName.PropInfo;
			dictionary[1717305375U] = PropTag.Message.OriginatorEmailAddress.PropInfo;
			dictionary[1717370911U] = PropTag.Message.OriginatorAddressType.PropInfo;
			dictionary[1717436674U] = PropTag.Message.OriginatorEntryId.PropInfo;
			dictionary[1717698563U] = PropTag.Message.RecipientNumber.PropInfo;
			dictionary[1717829663U] = PropTag.Message.ReportDestinationName.PropInfo;
			dictionary[1717895426U] = PropTag.Message.ReportDestinationEntryId.PropInfo;
			dictionary[1718222851U] = PropTag.Message.ProhibitReceiveQuota.PropInfo;
			dictionary[1718419487U] = PropTag.Message.SearchAttachments.PropInfo;
			dictionary[1718484995U] = PropTag.Message.ProhibitSendQuota.PropInfo;
			dictionary[1718550539U] = PropTag.Message.SubmittedByAdmin.PropInfo;
			dictionary[1718616322U] = PropTag.Message.LongTermEntryIdFromTable.PropInfo;
			dictionary[1718944002U] = PropTag.Message.RuleIds.PropInfo;
			dictionary[1719206146U] = PropTag.Message.RuleMsgConditionOld.PropInfo;
			dictionary[1719664898U] = PropTag.Message.RuleMsgActionsOld.PropInfo;
			dictionary[1720647744U] = PropTag.Message.DeletedOn.PropInfo;
			dictionary[1724055555U] = PropTag.Message.CodePageId.PropInfo;
			dictionary[1724514335U] = PropTag.Message.UserDN.PropInfo;
			dictionary[1728512072U] = PropTag.Message.MailboxDSGuidGuid.PropInfo;
			dictionary[1728512031U] = PropTag.Message.URLName.PropInfo;
			dictionary[1728643136U] = PropTag.Message.LocalCommitTime.PropInfo;
			dictionary[1728843848U] = PropTag.Message.AutoReset.PropInfo;
			dictionary[1729495298U] = PropTag.Message.ELCAutoCopyTag.PropInfo;
			dictionary[1729560834U] = PropTag.Message.ELCMoveDate.PropInfo;
			dictionary[1732116738U] = PropTag.Message.PropGroupInfo.PropInfo;
			dictionary[1732116483U] = PropTag.Message.PropertyGroupChangeMask.PropInfo;
			dictionary[1732182274U] = PropTag.Message.ReadCnNewExport.PropInfo;
			dictionary[1732247803U] = PropTag.Message.SentMailSvrEID.PropInfo;
			dictionary[1732247810U] = PropTag.Message.SentMailSvrEIDBin.PropInfo;
			dictionary[1732575490U] = PropTag.Message.LocallyDelivered.PropInfo;
			dictionary[1732640788U] = PropTag.Message.MimeSize.PropInfo;
			dictionary[1732640771U] = PropTag.Message.MimeSize32.PropInfo;
			dictionary[1732706324U] = PropTag.Message.FileSize.PropInfo;
			dictionary[1732706307U] = PropTag.Message.FileSize32.PropInfo;
			dictionary[1732771860U] = PropTag.Message.Fid.PropInfo;
			dictionary[1732772098U] = PropTag.Message.FidBin.PropInfo;
			dictionary[1732837396U] = PropTag.Message.ParentFid.PropInfo;
			dictionary[1732902932U] = PropTag.Message.Mid.PropInfo;
			dictionary[1732903170U] = PropTag.Message.MidBin.PropInfo;
			dictionary[1732968468U] = PropTag.Message.CategID.PropInfo;
			dictionary[1733034004U] = PropTag.Message.ParentCategID.PropInfo;
			dictionary[1733099540U] = PropTag.Message.InstanceId.PropInfo;
			dictionary[1733165059U] = PropTag.Message.InstanceNum.PropInfo;
			dictionary[1733296130U] = PropTag.Message.ChangeType.PropInfo;
			dictionary[1733361675U] = PropTag.Message.RequiresRefResolve.PropInfo;
			dictionary[1733820674U] = PropTag.Message.LTID.PropInfo;
			dictionary[1733886210U] = PropTag.Message.CnExport.PropInfo;
			dictionary[1733951746U] = PropTag.Message.PclExport.PropInfo;
			dictionary[1734017282U] = PropTag.Message.CnMvExport.PropInfo;
			dictionary[1735131394U] = PropTag.Message.MailboxGuid.PropInfo;
			dictionary[1735131208U] = PropTag.Message.MapiEntryIdGuidGuid.PropInfo;
			dictionary[1735196930U] = PropTag.Message.ImapCachedBodystructure.PropInfo;
			dictionary[1736114179U] = PropTag.Message.StorageQuota.PropInfo;
			dictionary[1737752834U] = PropTag.Message.CnsetIn.PropInfo;
			dictionary[1737883906U] = PropTag.Message.CnsetSeen.PropInfo;
			dictionary[1738801172U] = PropTag.Message.ChangeNumber.PropInfo;
			dictionary[1738801410U] = PropTag.Message.ChangeNumberBin.PropInfo;
			dictionary[1738866946U] = PropTag.Message.PCL.PropInfo;
			dictionary[1738936340U] = PropTag.Message.CnMv.PropInfo;
			dictionary[1739063554U] = PropTag.Message.SourceEntryId.PropInfo;
			dictionary[1739128834U] = PropTag.Message.MailFlags.PropInfo;
			dictionary[1739194379U] = PropTag.Message.Associated.PropInfo;
			dictionary[1739259907U] = PropTag.Message.SubmitResponsibility.PropInfo;
			dictionary[1739390987U] = PropTag.Message.SharedReceiptHandling.PropInfo;
			dictionary[1739587842U] = PropTag.Message.Inid.PropInfo;
			dictionary[1739784450U] = PropTag.Message.MessageAttachList.PropInfo;
			dictionary[1739915522U] = PropTag.Message.SenderCAI.PropInfo;
			dictionary[1739981058U] = PropTag.Message.SentRepresentingCAI.PropInfo;
			dictionary[1740046594U] = PropTag.Message.OriginalSenderCAI.PropInfo;
			dictionary[1740112130U] = PropTag.Message.OriginalSentRepresentingCAI.PropInfo;
			dictionary[1740177666U] = PropTag.Message.ReceivedByCAI.PropInfo;
			dictionary[1740243202U] = PropTag.Message.ReceivedRepresentingCAI.PropInfo;
			dictionary[1740308738U] = PropTag.Message.ReadReceiptCAI.PropInfo;
			dictionary[1740374274U] = PropTag.Message.ReportCAI.PropInfo;
			dictionary[1740439810U] = PropTag.Message.CreatorCAI.PropInfo;
			dictionary[1740505346U] = PropTag.Message.LastModifierCAI.PropInfo;
			dictionary[1741816066U] = PropTag.Message.CnsetRead.PropInfo;
			dictionary[1742340354U] = PropTag.Message.CnsetSeenFAI.PropInfo;
			dictionary[1743061250U] = PropTag.Message.IdSetDeleted.PropInfo;
			dictionary[1744306434U] = PropTag.Message.OriginatorCAI.PropInfo;
			dictionary[1744371970U] = PropTag.Message.ReportDestinationCAI.PropInfo;
			dictionary[1744437506U] = PropTag.Message.OriginalAuthorCAI.PropInfo;
			dictionary[1744699412U] = PropTag.Message.ReadCnNew.PropInfo;
			dictionary[1744699650U] = PropTag.Message.ReadCnNewBin.PropInfo;
			dictionary[1744961567U] = PropTag.Message.SenderTelephoneNumber.PropInfo;
			dictionary[1745158175U] = PropTag.Message.VoiceMessageAttachmentOrder.PropInfo;
			dictionary[1746206723U] = PropTag.Message.DocumentId.PropInfo;
			dictionary[1746862083U] = PropTag.Message.MailboxNum.PropInfo;
			dictionary[1747386371U] = PropTag.Message.ConversationIdHash.PropInfo;
			dictionary[1747320835U] = PropTag.Message.ConversationDocumentId.PropInfo;
			dictionary[1747452162U] = PropTag.Message.LocalDirectoryBlob.PropInfo;
			dictionary[1748238339U] = PropTag.Message.ViewStyle.PropInfo;
			dictionary[1749614623U] = PropTag.Message.FreebusyEMA.PropInfo;
			dictionary[1749811458U] = PropTag.Message.WunderbarLinkEntryID.PropInfo;
			dictionary[1749942530U] = PropTag.Message.WunderbarLinkStoreEntryId.PropInfo;
			dictionary[1750077698U] = PropTag.Message.SchdInfoFreebusyMerged.PropInfo;
			dictionary[1750073602U] = PropTag.Message.WunderbarLinkGroupClsId.PropInfo;
			dictionary[1750138911U] = PropTag.Message.WunderbarLinkGroupName.PropInfo;
			dictionary[1750204419U] = PropTag.Message.WunderbarLinkSection.PropInfo;
			dictionary[1750269955U] = PropTag.Message.NavigationNodeCalendarColor.PropInfo;
			dictionary[1750335746U] = PropTag.Message.NavigationNodeAddressbookEntryId.PropInfo;
			dictionary[1750401027U] = PropTag.Message.AgingDeleteItems.PropInfo;
			dictionary[1750466591U] = PropTag.Message.AgingFileName9AndPrev.PropInfo;
			dictionary[1750532107U] = PropTag.Message.AgingAgeFolder.PropInfo;
			dictionary[1750597643U] = PropTag.Message.AgingDontAgeMe.PropInfo;
			dictionary[1750663199U] = PropTag.Message.AgingFileNameAfter9.PropInfo;
			dictionary[1750794251U] = PropTag.Message.AgingWhenDeletedOnServer.PropInfo;
			dictionary[1750859787U] = PropTag.Message.AgingWaitUntilExpired.PropInfo;
			dictionary[1753223199U] = PropTag.Message.ConversationMvFrom.PropInfo;
			dictionary[1753288735U] = PropTag.Message.ConversationMvFromMailboxWide.PropInfo;
			dictionary[1753354271U] = PropTag.Message.ConversationMvTo.PropInfo;
			dictionary[1753419807U] = PropTag.Message.ConversationMvToMailboxWide.PropInfo;
			dictionary[1753481280U] = PropTag.Message.ConversationMessageDeliveryTime.PropInfo;
			dictionary[1753546816U] = PropTag.Message.ConversationMessageDeliveryTimeMailboxWide.PropInfo;
			dictionary[1753616415U] = PropTag.Message.ConversationCategories.PropInfo;
			dictionary[1753681951U] = PropTag.Message.ConversationCategoriesMailboxWide.PropInfo;
			dictionary[1753743363U] = PropTag.Message.ConversationFlagStatus.PropInfo;
			dictionary[1753808899U] = PropTag.Message.ConversationFlagStatusMailboxWide.PropInfo;
			dictionary[1753874496U] = PropTag.Message.ConversationFlagCompleteTime.PropInfo;
			dictionary[1753940032U] = PropTag.Message.ConversationFlagCompleteTimeMailboxWide.PropInfo;
			dictionary[1754005515U] = PropTag.Message.ConversationHasAttach.PropInfo;
			dictionary[1754071051U] = PropTag.Message.ConversationHasAttachMailboxWide.PropInfo;
			dictionary[1754136579U] = PropTag.Message.ConversationContentCount.PropInfo;
			dictionary[1754202115U] = PropTag.Message.ConversationContentCountMailboxWide.PropInfo;
			dictionary[1754267651U] = PropTag.Message.ConversationContentUnread.PropInfo;
			dictionary[1754333187U] = PropTag.Message.ConversationContentUnreadMailboxWide.PropInfo;
			dictionary[1754398723U] = PropTag.Message.ConversationMessageSize.PropInfo;
			dictionary[1754464259U] = PropTag.Message.ConversationMessageSizeMailboxWide.PropInfo;
			dictionary[1754533919U] = PropTag.Message.ConversationMessageClasses.PropInfo;
			dictionary[1754599455U] = PropTag.Message.ConversationMessageClassesMailboxWide.PropInfo;
			dictionary[1754660867U] = PropTag.Message.ConversationReplyForwardState.PropInfo;
			dictionary[1754726403U] = PropTag.Message.ConversationReplyForwardStateMailboxWide.PropInfo;
			dictionary[1754791939U] = PropTag.Message.ConversationImportance.PropInfo;
			dictionary[1754857475U] = PropTag.Message.ConversationImportanceMailboxWide.PropInfo;
			dictionary[1754927135U] = PropTag.Message.ConversationMvFromUnread.PropInfo;
			dictionary[1754992671U] = PropTag.Message.ConversationMvFromUnreadMailboxWide.PropInfo;
			dictionary[1755320578U] = PropTag.Message.ConversationMvItemIds.PropInfo;
			dictionary[1755386114U] = PropTag.Message.ConversationMvItemIdsMailboxWide.PropInfo;
			dictionary[1755447307U] = PropTag.Message.ConversationHasIrm.PropInfo;
			dictionary[1755512843U] = PropTag.Message.ConversationHasIrmMailboxWide.PropInfo;
			dictionary[1755578399U] = PropTag.Message.PersonCompanyNameMailboxWide.PropInfo;
			dictionary[1755643935U] = PropTag.Message.PersonDisplayNameMailboxWide.PropInfo;
			dictionary[1755709471U] = PropTag.Message.PersonGivenNameMailboxWide.PropInfo;
			dictionary[1755775007U] = PropTag.Message.PersonSurnameMailboxWide.PropInfo;
			dictionary[1755840770U] = PropTag.Message.PersonPhotoContactEntryIdMailboxWide.PropInfo;
			dictionary[1755971589U] = PropTag.Message.ConversationInferredImportanceInternal.PropInfo;
			dictionary[1756037123U] = PropTag.Message.ConversationInferredImportanceOverride.PropInfo;
			dictionary[1756102661U] = PropTag.Message.ConversationInferredUnimportanceInternal.PropInfo;
			dictionary[1756168197U] = PropTag.Message.ConversationInferredImportanceInternalMailboxWide.PropInfo;
			dictionary[1756233731U] = PropTag.Message.ConversationInferredImportanceOverrideMailboxWide.PropInfo;
			dictionary[1756299269U] = PropTag.Message.ConversationInferredUnimportanceInternalMailboxWide.PropInfo;
			dictionary[1756364831U] = PropTag.Message.PersonFileAsMailboxWide.PropInfo;
			dictionary[1756430339U] = PropTag.Message.PersonRelevanceScoreMailboxWide.PropInfo;
			dictionary[1756495883U] = PropTag.Message.PersonIsDistributionListMailboxWide.PropInfo;
			dictionary[1756561439U] = PropTag.Message.PersonHomeCityMailboxWide.PropInfo;
			dictionary[1756627008U] = PropTag.Message.PersonCreationTimeMailboxWide.PropInfo;
			dictionary[1756823624U] = PropTag.Message.PersonGALLinkIDMailboxWide.PropInfo;
			dictionary[1757024287U] = PropTag.Message.PersonMvEmailAddressMailboxWide.PropInfo;
			dictionary[1757089823U] = PropTag.Message.PersonMvEmailDisplayNameMailboxWide.PropInfo;
			dictionary[1757155359U] = PropTag.Message.PersonMvEmailRoutingTypeMailboxWide.PropInfo;
			dictionary[1757216799U] = PropTag.Message.PersonImAddressMailboxWide.PropInfo;
			dictionary[1757282335U] = PropTag.Message.PersonWorkCityMailboxWide.PropInfo;
			dictionary[1757347871U] = PropTag.Message.PersonDisplayNameFirstLastMailboxWide.PropInfo;
			dictionary[1757413407U] = PropTag.Message.PersonDisplayNameLastFirstMailboxWide.PropInfo;
			dictionary[1757286402U] = PropTag.Message.ConversationGroupingActions.PropInfo;
			dictionary[1757351938U] = PropTag.Message.ConversationGroupingActionsMailboxWide.PropInfo;
			dictionary[1757413379U] = PropTag.Message.ConversationPredictedActionsSummary.PropInfo;
			dictionary[1757478915U] = PropTag.Message.ConversationPredictedActionsSummaryMailboxWide.PropInfo;
			dictionary[1757544459U] = PropTag.Message.ConversationHasClutter.PropInfo;
			dictionary[1757609995U] = PropTag.Message.ConversationHasClutterMailboxWide.PropInfo;
			dictionary[1761607683U] = PropTag.Message.ConversationLastMemberDocumentId.PropInfo;
			dictionary[1761673247U] = PropTag.Message.ConversationPreview.PropInfo;
			dictionary[1761738755U] = PropTag.Message.ConversationLastMemberDocumentIdMailboxWide.PropInfo;
			dictionary[1761804291U] = PropTag.Message.ConversationInitialMemberDocumentId.PropInfo;
			dictionary[1761873923U] = PropTag.Message.ConversationMemberDocumentIds.PropInfo;
			dictionary[1761935424U] = PropTag.Message.ConversationMessageDeliveryOrRenewTimeMailboxWide.PropInfo;
			dictionary[1762001154U] = PropTag.Message.FamilyId.PropInfo;
			dictionary[1762070530U] = PropTag.Message.ConversationMessageRichContentMailboxWide.PropInfo;
			dictionary[1762131999U] = PropTag.Message.ConversationPreviewMailboxWide.PropInfo;
			dictionary[1762197568U] = PropTag.Message.ConversationMessageDeliveryOrRenewTime.PropInfo;
			dictionary[1762263071U] = PropTag.Message.ConversationWorkingSetSourcePartition.PropInfo;
			dictionary[1761935391U] = PropTag.Message.NDRFromName.PropInfo;
			dictionary[1845559299U] = PropTag.Message.SecurityFlags.PropInfo;
			dictionary[1845755915U] = PropTag.Message.SecurityReceiptRequestProcessed.PropInfo;
			dictionary[2080374815U] = PropTag.Message.FavoriteDisplayName.PropInfo;
			dictionary[2080440351U] = PropTag.Message.FavoriteDisplayAlias.PropInfo;
			dictionary[2080506114U] = PropTag.Message.FavPublicSourceKey.PropInfo;
			dictionary[2080571650U] = PropTag.Message.SyncFolderSourceKey.PropInfo;
			dictionary[2080768003U] = PropTag.Message.UserConfigurationDataType.PropInfo;
			dictionary[2080899330U] = PropTag.Message.UserConfigurationXmlStream.PropInfo;
			dictionary[2080964866U] = PropTag.Message.UserConfigurationStream.PropInfo;
			dictionary[2081095711U] = PropTag.Message.ReplyFwdStatus.PropInfo;
			dictionary[2082734091U] = PropTag.Message.OscSyncEnabledOnServer.PropInfo;
			dictionary[2097217547U] = PropTag.Message.Processed.PropInfo;
			dictionary[2097348611U] = PropTag.Message.FavLevelMask.PropInfo;
			return dictionary;
		}

		private static WellKnownProperties.ObjectPropertyDefinitions BuildAttachmentPropertyDefinitions()
		{
			Dictionary<string, StorePropInfo> propertiesByTagName = WellKnownProperties.BuildAttachmentPropertyDefinitionsByName();
			Dictionary<uint, StorePropInfo> properties = WellKnownProperties.BuildAttachmentPropertyDefinitionsByTag();
			Dictionary<ushort, StorePropInfo> dictionary = new Dictionary<ushort, StorePropInfo>(100);
			dictionary.Add(3603, new StorePropInfo(null, 3603, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(3616, new StorePropInfo(null, 3616, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(3617, new StorePropInfo(null, 3617, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(3734, new StorePropInfo(null, 3734, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(4087, new StorePropInfo(null, 4087, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(4088, new StorePropInfo(null, 4088, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 9)));
			dictionary.Add(4089, new StorePropInfo(null, 4089, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(4094, new StorePropInfo(null, 4094, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 9)));
			dictionary.Add(14081, new StorePropInfo(null, 14081, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(15)));
			dictionary.Add(16264, new StorePropInfo(null, 16264, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(16328, new StorePropInfo(null, 16328, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26186, new StorePropInfo(null, 26186, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26439, new StorePropInfo(null, 26439, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26442, new StorePropInfo(null, 26442, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26456, new StorePropInfo(null, 26456, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			dictionary.Add(26655, new StorePropInfo(null, 26655, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)));
			WellKnownProperties.PropIdRangeList propIdRanges = WellKnownProperties.BuildPropIdRanges(new WellKnownProperties.TempIdRange[]
			{
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.NoGetProp
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.NoGetPropList
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26112,
					Max = 26135,
					Category = PropCategory.NoGetPropList
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26420,
					Max = 26431,
					Category = PropCategory.NoGetPropList
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26464,
					Max = 26623,
					Category = PropCategory.NoGetPropList
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.NoGetPropListForFastTransfer
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26112,
					Max = 26135,
					Category = PropCategory.NoGetPropListForFastTransfer
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26420,
					Max = 26431,
					Category = PropCategory.NoGetPropListForFastTransfer
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26464,
					Max = 26623,
					Category = PropCategory.NoGetPropListForFastTransfer
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26112,
					Max = 26135,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26440,
					Max = 26447,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26456,
					Max = 26463,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26420,
					Max = 26431,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 26464,
					Max = 26623,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.NoCopy
				}
			});
			return new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.Attachment,
				Properties = properties,
				PropertiesByTagName = propertiesByTagName,
				PropIds = dictionary,
				PropIdRanges = propIdRanges
			};
		}

		private static Dictionary<string, StorePropInfo> BuildAttachmentPropertyDefinitionsByName()
		{
			Dictionary<string, StorePropInfo> dictionary = new Dictionary<string, StorePropInfo>(234, StringComparer.OrdinalIgnoreCase);
			dictionary["TNEFCorrelationKey"] = PropTag.Attachment.TNEFCorrelationKey.PropInfo;
			dictionary["PhysicalRenditionAttributes"] = PropTag.Attachment.PhysicalRenditionAttributes.PropInfo;
			dictionary["ItemSubobjectsBin"] = PropTag.Attachment.ItemSubobjectsBin.PropInfo;
			dictionary["AttachSize"] = PropTag.Attachment.AttachSize.PropInfo;
			dictionary["AttachSizeInt64"] = PropTag.Attachment.AttachSizeInt64.PropInfo;
			dictionary["AttachNum"] = PropTag.Attachment.AttachNum.PropInfo;
			dictionary["CreatorSID"] = PropTag.Attachment.CreatorSID.PropInfo;
			dictionary["LastModifierSid"] = PropTag.Attachment.LastModifierSid.PropInfo;
			dictionary["VirusScannerStamp"] = PropTag.Attachment.VirusScannerStamp.PropInfo;
			dictionary["VirusTransportStamp"] = PropTag.Attachment.VirusTransportStamp.PropInfo;
			dictionary["AccessLevel"] = PropTag.Attachment.AccessLevel.PropInfo;
			dictionary["MappingSignature"] = PropTag.Attachment.MappingSignature.PropInfo;
			dictionary["RecordKey"] = PropTag.Attachment.RecordKey.PropInfo;
			dictionary["ObjectType"] = PropTag.Attachment.ObjectType.PropInfo;
			dictionary["DisplayName"] = PropTag.Attachment.DisplayName.PropInfo;
			dictionary["Comment"] = PropTag.Attachment.Comment.PropInfo;
			dictionary["CreationTime"] = PropTag.Attachment.CreationTime.PropInfo;
			dictionary["LastModificationTime"] = PropTag.Attachment.LastModificationTime.PropInfo;
			dictionary["AttachmentX400Parameters"] = PropTag.Attachment.AttachmentX400Parameters.PropInfo;
			dictionary["Content"] = PropTag.Attachment.Content.PropInfo;
			dictionary["ContentObj"] = PropTag.Attachment.ContentObj.PropInfo;
			dictionary["AttachmentEncoding"] = PropTag.Attachment.AttachmentEncoding.PropInfo;
			dictionary["ContentId"] = PropTag.Attachment.ContentId.PropInfo;
			dictionary["ContentType"] = PropTag.Attachment.ContentType.PropInfo;
			dictionary["AttachMethod"] = PropTag.Attachment.AttachMethod.PropInfo;
			dictionary["MimeUrl"] = PropTag.Attachment.MimeUrl.PropInfo;
			dictionary["AttachmentPathName"] = PropTag.Attachment.AttachmentPathName.PropInfo;
			dictionary["AttachRendering"] = PropTag.Attachment.AttachRendering.PropInfo;
			dictionary["AttachTag"] = PropTag.Attachment.AttachTag.PropInfo;
			dictionary["RenderingPosition"] = PropTag.Attachment.RenderingPosition.PropInfo;
			dictionary["AttachTransportName"] = PropTag.Attachment.AttachTransportName.PropInfo;
			dictionary["AttachmentLongPathName"] = PropTag.Attachment.AttachmentLongPathName.PropInfo;
			dictionary["AttachmentMimeTag"] = PropTag.Attachment.AttachmentMimeTag.PropInfo;
			dictionary["AttachAdditionalInfo"] = PropTag.Attachment.AttachAdditionalInfo.PropInfo;
			dictionary["AttachmentMimeSequence"] = PropTag.Attachment.AttachmentMimeSequence.PropInfo;
			dictionary["AttachContentBase"] = PropTag.Attachment.AttachContentBase.PropInfo;
			dictionary["AttachContentId"] = PropTag.Attachment.AttachContentId.PropInfo;
			dictionary["AttachContentLocation"] = PropTag.Attachment.AttachContentLocation.PropInfo;
			dictionary["AttachmentFlags"] = PropTag.Attachment.AttachmentFlags.PropInfo;
			dictionary["AttachDisposition"] = PropTag.Attachment.AttachDisposition.PropInfo;
			dictionary["AttachPayloadProviderGuidString"] = PropTag.Attachment.AttachPayloadProviderGuidString.PropInfo;
			dictionary["AttachPayloadClass"] = PropTag.Attachment.AttachPayloadClass.PropInfo;
			dictionary["TextAttachmentCharset"] = PropTag.Attachment.TextAttachmentCharset.PropInfo;
			dictionary["Language"] = PropTag.Attachment.Language.PropInfo;
			dictionary["TestBlobProperty"] = PropTag.Attachment.TestBlobProperty.PropInfo;
			dictionary["MailboxPartitionNumber"] = PropTag.Attachment.MailboxPartitionNumber.PropInfo;
			dictionary["MailboxNumberInternal"] = PropTag.Attachment.MailboxNumberInternal.PropInfo;
			dictionary["AttachmentId"] = PropTag.Attachment.AttachmentId.PropInfo;
			dictionary["AttachmentIdBin"] = PropTag.Attachment.AttachmentIdBin.PropInfo;
			dictionary["ReplicaChangeNumber"] = PropTag.Attachment.ReplicaChangeNumber.PropInfo;
			dictionary["NewAttach"] = PropTag.Attachment.NewAttach.PropInfo;
			dictionary["StartEmbed"] = PropTag.Attachment.StartEmbed.PropInfo;
			dictionary["EndEmbed"] = PropTag.Attachment.EndEmbed.PropInfo;
			dictionary["StartRecip"] = PropTag.Attachment.StartRecip.PropInfo;
			dictionary["EndRecip"] = PropTag.Attachment.EndRecip.PropInfo;
			dictionary["EndCcRecip"] = PropTag.Attachment.EndCcRecip.PropInfo;
			dictionary["EndBccRecip"] = PropTag.Attachment.EndBccRecip.PropInfo;
			dictionary["EndP1Recip"] = PropTag.Attachment.EndP1Recip.PropInfo;
			dictionary["DNPrefix"] = PropTag.Attachment.DNPrefix.PropInfo;
			dictionary["StartTopFolder"] = PropTag.Attachment.StartTopFolder.PropInfo;
			dictionary["StartSubFolder"] = PropTag.Attachment.StartSubFolder.PropInfo;
			dictionary["EndFolder"] = PropTag.Attachment.EndFolder.PropInfo;
			dictionary["StartMessage"] = PropTag.Attachment.StartMessage.PropInfo;
			dictionary["EndMessage"] = PropTag.Attachment.EndMessage.PropInfo;
			dictionary["EndAttach"] = PropTag.Attachment.EndAttach.PropInfo;
			dictionary["EcWarning"] = PropTag.Attachment.EcWarning.PropInfo;
			dictionary["StartFAIMessage"] = PropTag.Attachment.StartFAIMessage.PropInfo;
			dictionary["NewFXFolder"] = PropTag.Attachment.NewFXFolder.PropInfo;
			dictionary["IncrSyncChange"] = PropTag.Attachment.IncrSyncChange.PropInfo;
			dictionary["IncrSyncDelete"] = PropTag.Attachment.IncrSyncDelete.PropInfo;
			dictionary["IncrSyncEnd"] = PropTag.Attachment.IncrSyncEnd.PropInfo;
			dictionary["IncrSyncMessage"] = PropTag.Attachment.IncrSyncMessage.PropInfo;
			dictionary["FastTransferDelProp"] = PropTag.Attachment.FastTransferDelProp.PropInfo;
			dictionary["IdsetGiven"] = PropTag.Attachment.IdsetGiven.PropInfo;
			dictionary["IdsetGivenInt32"] = PropTag.Attachment.IdsetGivenInt32.PropInfo;
			dictionary["FastTransferErrorInfo"] = PropTag.Attachment.FastTransferErrorInfo.PropInfo;
			dictionary["SoftDeletes"] = PropTag.Attachment.SoftDeletes.PropInfo;
			dictionary["IdsetRead"] = PropTag.Attachment.IdsetRead.PropInfo;
			dictionary["IdsetUnread"] = PropTag.Attachment.IdsetUnread.PropInfo;
			dictionary["IncrSyncRead"] = PropTag.Attachment.IncrSyncRead.PropInfo;
			dictionary["IncrSyncStateBegin"] = PropTag.Attachment.IncrSyncStateBegin.PropInfo;
			dictionary["IncrSyncStateEnd"] = PropTag.Attachment.IncrSyncStateEnd.PropInfo;
			dictionary["IncrSyncImailStream"] = PropTag.Attachment.IncrSyncImailStream.PropInfo;
			dictionary["IncrSyncImailStreamContinue"] = PropTag.Attachment.IncrSyncImailStreamContinue.PropInfo;
			dictionary["IncrSyncImailStreamCancel"] = PropTag.Attachment.IncrSyncImailStreamCancel.PropInfo;
			dictionary["IncrSyncImailStream2Continue"] = PropTag.Attachment.IncrSyncImailStream2Continue.PropInfo;
			dictionary["IncrSyncProgressMode"] = PropTag.Attachment.IncrSyncProgressMode.PropInfo;
			dictionary["SyncProgressPerMsg"] = PropTag.Attachment.SyncProgressPerMsg.PropInfo;
			dictionary["IncrSyncMsgPartial"] = PropTag.Attachment.IncrSyncMsgPartial.PropInfo;
			dictionary["IncrSyncGroupInfo"] = PropTag.Attachment.IncrSyncGroupInfo.PropInfo;
			dictionary["IncrSyncGroupId"] = PropTag.Attachment.IncrSyncGroupId.PropInfo;
			dictionary["IncrSyncChangePartial"] = PropTag.Attachment.IncrSyncChangePartial.PropInfo;
			dictionary["HasNamedProperties"] = PropTag.Attachment.HasNamedProperties.PropInfo;
			dictionary["CodePageId"] = PropTag.Attachment.CodePageId.PropInfo;
			dictionary["URLName"] = PropTag.Attachment.URLName.PropInfo;
			dictionary["MimeSize"] = PropTag.Attachment.MimeSize.PropInfo;
			dictionary["MimeSize32"] = PropTag.Attachment.MimeSize32.PropInfo;
			dictionary["FileSize"] = PropTag.Attachment.FileSize.PropInfo;
			dictionary["FileSize32"] = PropTag.Attachment.FileSize32.PropInfo;
			dictionary["Mid"] = PropTag.Attachment.Mid.PropInfo;
			dictionary["MidBin"] = PropTag.Attachment.MidBin.PropInfo;
			dictionary["LTID"] = PropTag.Attachment.LTID.PropInfo;
			dictionary["CnsetSeen"] = PropTag.Attachment.CnsetSeen.PropInfo;
			dictionary["Inid"] = PropTag.Attachment.Inid.PropInfo;
			dictionary["CnsetRead"] = PropTag.Attachment.CnsetRead.PropInfo;
			dictionary["CnsetSeenFAI"] = PropTag.Attachment.CnsetSeenFAI.PropInfo;
			dictionary["IdSetDeleted"] = PropTag.Attachment.IdSetDeleted.PropInfo;
			dictionary["MailboxNum"] = PropTag.Attachment.MailboxNum.PropInfo;
			dictionary["AttachEXCLIVersion"] = PropTag.Attachment.AttachEXCLIVersion.PropInfo;
			dictionary["HasDlpDetectedAttachmentClassifications"] = PropTag.Attachment.HasDlpDetectedAttachmentClassifications.PropInfo;
			dictionary["SExceptionReplaceTime"] = PropTag.Attachment.SExceptionReplaceTime.PropInfo;
			dictionary["AttachmentLinkId"] = PropTag.Attachment.AttachmentLinkId.PropInfo;
			dictionary["ExceptionStartTime"] = PropTag.Attachment.ExceptionStartTime.PropInfo;
			dictionary["ExceptionEndTime"] = PropTag.Attachment.ExceptionEndTime.PropInfo;
			dictionary["AttachmentFlags2"] = PropTag.Attachment.AttachmentFlags2.PropInfo;
			dictionary["AttachmentHidden"] = PropTag.Attachment.AttachmentHidden.PropInfo;
			dictionary["AttachmentContactPhoto"] = PropTag.Attachment.AttachmentContactPhoto.PropInfo;
			return dictionary;
		}

		private static Dictionary<uint, StorePropInfo> BuildAttachmentPropertyDefinitionsByTag()
		{
			Dictionary<uint, StorePropInfo> dictionary = new Dictionary<uint, StorePropInfo>(234);
			dictionary[8323330U] = PropTag.Attachment.TNEFCorrelationKey.PropInfo;
			dictionary[202375426U] = PropTag.Attachment.PhysicalRenditionAttributes.PropInfo;
			dictionary[236126466U] = PropTag.Attachment.ItemSubobjectsBin.PropInfo;
			dictionary[236978179U] = PropTag.Attachment.AttachSize.PropInfo;
			dictionary[236978196U] = PropTag.Attachment.AttachSizeInt64.PropInfo;
			dictionary[237043715U] = PropTag.Attachment.AttachNum.PropInfo;
			dictionary[240648450U] = PropTag.Attachment.CreatorSID.PropInfo;
			dictionary[240713986U] = PropTag.Attachment.LastModifierSid.PropInfo;
			dictionary[244711682U] = PropTag.Attachment.VirusScannerStamp.PropInfo;
			dictionary[244715551U] = PropTag.Attachment.VirusTransportStamp.PropInfo;
			dictionary[267845635U] = PropTag.Attachment.AccessLevel.PropInfo;
			dictionary[267911426U] = PropTag.Attachment.MappingSignature.PropInfo;
			dictionary[267976962U] = PropTag.Attachment.RecordKey.PropInfo;
			dictionary[268304387U] = PropTag.Attachment.ObjectType.PropInfo;
			dictionary[805371935U] = PropTag.Attachment.DisplayName.PropInfo;
			dictionary[805568543U] = PropTag.Attachment.Comment.PropInfo;
			dictionary[805765184U] = PropTag.Attachment.CreationTime.PropInfo;
			dictionary[805830720U] = PropTag.Attachment.LastModificationTime.PropInfo;
			dictionary[922747138U] = PropTag.Attachment.AttachmentX400Parameters.PropInfo;
			dictionary[922812674U] = PropTag.Attachment.Content.PropInfo;
			dictionary[922812429U] = PropTag.Attachment.ContentObj.PropInfo;
			dictionary[922878210U] = PropTag.Attachment.AttachmentEncoding.PropInfo;
			dictionary[922943519U] = PropTag.Attachment.ContentId.PropInfo;
			dictionary[923009055U] = PropTag.Attachment.ContentType.PropInfo;
			dictionary[923074563U] = PropTag.Attachment.AttachMethod.PropInfo;
			dictionary[923205663U] = PropTag.Attachment.MimeUrl.PropInfo;
			dictionary[923271199U] = PropTag.Attachment.AttachmentPathName.PropInfo;
			dictionary[923336962U] = PropTag.Attachment.AttachRendering.PropInfo;
			dictionary[923402498U] = PropTag.Attachment.AttachTag.PropInfo;
			dictionary[923467779U] = PropTag.Attachment.RenderingPosition.PropInfo;
			dictionary[923533343U] = PropTag.Attachment.AttachTransportName.PropInfo;
			dictionary[923598879U] = PropTag.Attachment.AttachmentLongPathName.PropInfo;
			dictionary[923664415U] = PropTag.Attachment.AttachmentMimeTag.PropInfo;
			dictionary[923730178U] = PropTag.Attachment.AttachAdditionalInfo.PropInfo;
			dictionary[923795459U] = PropTag.Attachment.AttachmentMimeSequence.PropInfo;
			dictionary[923861023U] = PropTag.Attachment.AttachContentBase.PropInfo;
			dictionary[923926559U] = PropTag.Attachment.AttachContentId.PropInfo;
			dictionary[923992095U] = PropTag.Attachment.AttachContentLocation.PropInfo;
			dictionary[924057603U] = PropTag.Attachment.AttachmentFlags.PropInfo;
			dictionary[924188703U] = PropTag.Attachment.AttachDisposition.PropInfo;
			dictionary[924385311U] = PropTag.Attachment.AttachPayloadProviderGuidString.PropInfo;
			dictionary[924450847U] = PropTag.Attachment.AttachPayloadClass.PropInfo;
			dictionary[924516383U] = PropTag.Attachment.TextAttachmentCharset.PropInfo;
			dictionary[973864991U] = PropTag.Attachment.Language.PropInfo;
			dictionary[1023410196U] = PropTag.Attachment.TestBlobProperty.PropInfo;
			dictionary[1033830403U] = PropTag.Attachment.MailboxPartitionNumber.PropInfo;
			dictionary[1033895939U] = PropTag.Attachment.MailboxNumberInternal.PropInfo;
			dictionary[1065877524U] = PropTag.Attachment.AttachmentId.PropInfo;
			dictionary[1065877762U] = PropTag.Attachment.AttachmentIdBin.PropInfo;
			dictionary[1070072066U] = PropTag.Attachment.ReplicaChangeNumber.PropInfo;
			dictionary[1073741827U] = PropTag.Attachment.NewAttach.PropInfo;
			dictionary[1073807363U] = PropTag.Attachment.StartEmbed.PropInfo;
			dictionary[1073872899U] = PropTag.Attachment.EndEmbed.PropInfo;
			dictionary[1073938435U] = PropTag.Attachment.StartRecip.PropInfo;
			dictionary[1074003971U] = PropTag.Attachment.EndRecip.PropInfo;
			dictionary[1074069507U] = PropTag.Attachment.EndCcRecip.PropInfo;
			dictionary[1074135043U] = PropTag.Attachment.EndBccRecip.PropInfo;
			dictionary[1074200579U] = PropTag.Attachment.EndP1Recip.PropInfo;
			dictionary[1074266143U] = PropTag.Attachment.DNPrefix.PropInfo;
			dictionary[1074331651U] = PropTag.Attachment.StartTopFolder.PropInfo;
			dictionary[1074397187U] = PropTag.Attachment.StartSubFolder.PropInfo;
			dictionary[1074462723U] = PropTag.Attachment.EndFolder.PropInfo;
			dictionary[1074528259U] = PropTag.Attachment.StartMessage.PropInfo;
			dictionary[1074593795U] = PropTag.Attachment.EndMessage.PropInfo;
			dictionary[1074659331U] = PropTag.Attachment.EndAttach.PropInfo;
			dictionary[1074724867U] = PropTag.Attachment.EcWarning.PropInfo;
			dictionary[1074790403U] = PropTag.Attachment.StartFAIMessage.PropInfo;
			dictionary[1074856194U] = PropTag.Attachment.NewFXFolder.PropInfo;
			dictionary[1074921475U] = PropTag.Attachment.IncrSyncChange.PropInfo;
			dictionary[1074987011U] = PropTag.Attachment.IncrSyncDelete.PropInfo;
			dictionary[1075052547U] = PropTag.Attachment.IncrSyncEnd.PropInfo;
			dictionary[1075118083U] = PropTag.Attachment.IncrSyncMessage.PropInfo;
			dictionary[1075183619U] = PropTag.Attachment.FastTransferDelProp.PropInfo;
			dictionary[1075249410U] = PropTag.Attachment.IdsetGiven.PropInfo;
			dictionary[1075249155U] = PropTag.Attachment.IdsetGivenInt32.PropInfo;
			dictionary[1075314691U] = PropTag.Attachment.FastTransferErrorInfo.PropInfo;
			dictionary[1075904770U] = PropTag.Attachment.SoftDeletes.PropInfo;
			dictionary[1076691202U] = PropTag.Attachment.IdsetRead.PropInfo;
			dictionary[1076756738U] = PropTag.Attachment.IdsetUnread.PropInfo;
			dictionary[1076822019U] = PropTag.Attachment.IncrSyncRead.PropInfo;
			dictionary[1077542915U] = PropTag.Attachment.IncrSyncStateBegin.PropInfo;
			dictionary[1077608451U] = PropTag.Attachment.IncrSyncStateEnd.PropInfo;
			dictionary[1077673987U] = PropTag.Attachment.IncrSyncImailStream.PropInfo;
			dictionary[1080426499U] = PropTag.Attachment.IncrSyncImailStreamContinue.PropInfo;
			dictionary[1080492035U] = PropTag.Attachment.IncrSyncImailStreamCancel.PropInfo;
			dictionary[1081147395U] = PropTag.Attachment.IncrSyncImailStream2Continue.PropInfo;
			dictionary[1081344011U] = PropTag.Attachment.IncrSyncProgressMode.PropInfo;
			dictionary[1081409547U] = PropTag.Attachment.SyncProgressPerMsg.PropInfo;
			dictionary[1081737219U] = PropTag.Attachment.IncrSyncMsgPartial.PropInfo;
			dictionary[1081802755U] = PropTag.Attachment.IncrSyncGroupInfo.PropInfo;
			dictionary[1081868291U] = PropTag.Attachment.IncrSyncGroupId.PropInfo;
			dictionary[1081933827U] = PropTag.Attachment.IncrSyncChangePartial.PropInfo;
			dictionary[1716125707U] = PropTag.Attachment.HasNamedProperties.PropInfo;
			dictionary[1724055555U] = PropTag.Attachment.CodePageId.PropInfo;
			dictionary[1728512031U] = PropTag.Attachment.URLName.PropInfo;
			dictionary[1732640788U] = PropTag.Attachment.MimeSize.PropInfo;
			dictionary[1732640771U] = PropTag.Attachment.MimeSize32.PropInfo;
			dictionary[1732706324U] = PropTag.Attachment.FileSize.PropInfo;
			dictionary[1732706307U] = PropTag.Attachment.FileSize32.PropInfo;
			dictionary[1732902932U] = PropTag.Attachment.Mid.PropInfo;
			dictionary[1732903170U] = PropTag.Attachment.MidBin.PropInfo;
			dictionary[1733820674U] = PropTag.Attachment.LTID.PropInfo;
			dictionary[1737883906U] = PropTag.Attachment.CnsetSeen.PropInfo;
			dictionary[1739587842U] = PropTag.Attachment.Inid.PropInfo;
			dictionary[1741816066U] = PropTag.Attachment.CnsetRead.PropInfo;
			dictionary[1742340354U] = PropTag.Attachment.CnsetSeenFAI.PropInfo;
			dictionary[1743061250U] = PropTag.Attachment.IdSetDeleted.PropInfo;
			dictionary[1746862083U] = PropTag.Attachment.MailboxNum.PropInfo;
			dictionary[1762197507U] = PropTag.Attachment.AttachEXCLIVersion.PropInfo;
			dictionary[2146959391U] = PropTag.Attachment.HasDlpDetectedAttachmentClassifications.PropInfo;
			dictionary[2147024960U] = PropTag.Attachment.SExceptionReplaceTime.PropInfo;
			dictionary[2147090435U] = PropTag.Attachment.AttachmentLinkId.PropInfo;
			dictionary[2147155975U] = PropTag.Attachment.ExceptionStartTime.PropInfo;
			dictionary[2147221511U] = PropTag.Attachment.ExceptionEndTime.PropInfo;
			dictionary[2147287043U] = PropTag.Attachment.AttachmentFlags2.PropInfo;
			dictionary[2147352587U] = PropTag.Attachment.AttachmentHidden.PropInfo;
			dictionary[2147418123U] = PropTag.Attachment.AttachmentContactPhoto.PropInfo;
			return dictionary;
		}

		private static WellKnownProperties.ObjectPropertyDefinitions BuildRecipientPropertyDefinitions()
		{
			Dictionary<string, StorePropInfo> propertiesByTagName = WellKnownProperties.BuildRecipientPropertyDefinitionsByName();
			Dictionary<uint, StorePropInfo> properties = WellKnownProperties.BuildRecipientPropertyDefinitionsByTag();
			Dictionary<ushort, StorePropInfo> dictionary = new Dictionary<ushort, StorePropInfo>(100);
			dictionary.Add(4089, new StorePropInfo(null, 4089, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(9)));
			dictionary.Add(4094, new StorePropInfo(null, 4094, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(9)));
			dictionary.Add(4095, new StorePropInfo(null, 4095, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(9)));
			dictionary.Add(12288, new StorePropInfo(null, 12288, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(9)));
			WellKnownProperties.PropIdRangeList propIdRanges = WellKnownProperties.BuildPropIdRanges(new WellKnownProperties.TempIdRange[]
			{
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.NoGetProp
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.NoGetPropList
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.NoGetPropListForFastTransfer
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.SetPropRestricted
				},
				new WellKnownProperties.TempIdRange
				{
					Min = 15664,
					Max = 15807,
					Category = PropCategory.NoCopy
				}
			});
			return new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.Recipient,
				Properties = properties,
				PropertiesByTagName = propertiesByTagName,
				PropIds = dictionary,
				PropIdRanges = propIdRanges
			};
		}

		private static Dictionary<string, StorePropInfo> BuildRecipientPropertyDefinitionsByName()
		{
			Dictionary<string, StorePropInfo> dictionary = new Dictionary<string, StorePropInfo>(394, StringComparer.OrdinalIgnoreCase);
			dictionary["DeliveryReportRequested"] = PropTag.Recipient.DeliveryReportRequested.PropInfo;
			dictionary["ReadReceiptRequested"] = PropTag.Recipient.ReadReceiptRequested.PropInfo;
			dictionary["ReportTime"] = PropTag.Recipient.ReportTime.PropInfo;
			dictionary["DiscVal"] = PropTag.Recipient.DiscVal.PropInfo;
			dictionary["ExplicitConversion"] = PropTag.Recipient.ExplicitConversion.PropInfo;
			dictionary["NDRReasonCode"] = PropTag.Recipient.NDRReasonCode.PropInfo;
			dictionary["NDRDiagCode"] = PropTag.Recipient.NDRDiagCode.PropInfo;
			dictionary["NonReceiptNotificationRequested"] = PropTag.Recipient.NonReceiptNotificationRequested.PropInfo;
			dictionary["NonDeliveryReportRequested"] = PropTag.Recipient.NonDeliveryReportRequested.PropInfo;
			dictionary["OriginatorRequestedAlterateRecipient"] = PropTag.Recipient.OriginatorRequestedAlterateRecipient.PropInfo;
			dictionary["PhysicalDeliveryMode"] = PropTag.Recipient.PhysicalDeliveryMode.PropInfo;
			dictionary["PhysicalDeliveryReportRequest"] = PropTag.Recipient.PhysicalDeliveryReportRequest.PropInfo;
			dictionary["PhysicalForwardingAddress"] = PropTag.Recipient.PhysicalForwardingAddress.PropInfo;
			dictionary["PhysicalForwardingAddressRequested"] = PropTag.Recipient.PhysicalForwardingAddressRequested.PropInfo;
			dictionary["PhysicalForwardingProhibited"] = PropTag.Recipient.PhysicalForwardingProhibited.PropInfo;
			dictionary["ProofOfDelivery"] = PropTag.Recipient.ProofOfDelivery.PropInfo;
			dictionary["ProofOfDeliveryRequested"] = PropTag.Recipient.ProofOfDeliveryRequested.PropInfo;
			dictionary["RecipientCertificate"] = PropTag.Recipient.RecipientCertificate.PropInfo;
			dictionary["RecipientNumberForAdvice"] = PropTag.Recipient.RecipientNumberForAdvice.PropInfo;
			dictionary["RecipientType"] = PropTag.Recipient.RecipientType.PropInfo;
			dictionary["TypeOfMTSUser"] = PropTag.Recipient.TypeOfMTSUser.PropInfo;
			dictionary["DiscreteValues"] = PropTag.Recipient.DiscreteValues.PropInfo;
			dictionary["Responsibility"] = PropTag.Recipient.Responsibility.PropInfo;
			dictionary["RecipientStatus"] = PropTag.Recipient.RecipientStatus.PropInfo;
			dictionary["InstanceKey"] = PropTag.Recipient.InstanceKey.PropInfo;
			dictionary["AccessLevel"] = PropTag.Recipient.AccessLevel.PropInfo;
			dictionary["RecordKey"] = PropTag.Recipient.RecordKey.PropInfo;
			dictionary["RecordKeySvrEid"] = PropTag.Recipient.RecordKeySvrEid.PropInfo;
			dictionary["ObjectType"] = PropTag.Recipient.ObjectType.PropInfo;
			dictionary["EntryId"] = PropTag.Recipient.EntryId.PropInfo;
			dictionary["EntryIdSvrEid"] = PropTag.Recipient.EntryIdSvrEid.PropInfo;
			dictionary["RowId"] = PropTag.Recipient.RowId.PropInfo;
			dictionary["DisplayName"] = PropTag.Recipient.DisplayName.PropInfo;
			dictionary["AddressType"] = PropTag.Recipient.AddressType.PropInfo;
			dictionary["EmailAddress"] = PropTag.Recipient.EmailAddress.PropInfo;
			dictionary["Comment"] = PropTag.Recipient.Comment.PropInfo;
			dictionary["LastModificationTime"] = PropTag.Recipient.LastModificationTime.PropInfo;
			dictionary["SearchKey"] = PropTag.Recipient.SearchKey.PropInfo;
			dictionary["SearchKeySvrEid"] = PropTag.Recipient.SearchKeySvrEid.PropInfo;
			dictionary["DetailsTable"] = PropTag.Recipient.DetailsTable.PropInfo;
			dictionary["DisplayType"] = PropTag.Recipient.DisplayType.PropInfo;
			dictionary["SmtpAddress"] = PropTag.Recipient.SmtpAddress.PropInfo;
			dictionary["SimpleDisplayName"] = PropTag.Recipient.SimpleDisplayName.PropInfo;
			dictionary["Account"] = PropTag.Recipient.Account.PropInfo;
			dictionary["AlternateRecipient"] = PropTag.Recipient.AlternateRecipient.PropInfo;
			dictionary["CallbackTelephoneNumber"] = PropTag.Recipient.CallbackTelephoneNumber.PropInfo;
			dictionary["Generation"] = PropTag.Recipient.Generation.PropInfo;
			dictionary["GivenName"] = PropTag.Recipient.GivenName.PropInfo;
			dictionary["GovernmentIDNumber"] = PropTag.Recipient.GovernmentIDNumber.PropInfo;
			dictionary["BusinessTelephoneNumber"] = PropTag.Recipient.BusinessTelephoneNumber.PropInfo;
			dictionary["HomeTelephoneNumber"] = PropTag.Recipient.HomeTelephoneNumber.PropInfo;
			dictionary["Initials"] = PropTag.Recipient.Initials.PropInfo;
			dictionary["Keyword"] = PropTag.Recipient.Keyword.PropInfo;
			dictionary["Language"] = PropTag.Recipient.Language.PropInfo;
			dictionary["Location"] = PropTag.Recipient.Location.PropInfo;
			dictionary["MailPermission"] = PropTag.Recipient.MailPermission.PropInfo;
			dictionary["OrganizationalIDNumber"] = PropTag.Recipient.OrganizationalIDNumber.PropInfo;
			dictionary["SurName"] = PropTag.Recipient.SurName.PropInfo;
			dictionary["OriginalEntryId"] = PropTag.Recipient.OriginalEntryId.PropInfo;
			dictionary["OriginalDisplayName"] = PropTag.Recipient.OriginalDisplayName.PropInfo;
			dictionary["OriginalSearchKey"] = PropTag.Recipient.OriginalSearchKey.PropInfo;
			dictionary["PostalAddress"] = PropTag.Recipient.PostalAddress.PropInfo;
			dictionary["CompanyName"] = PropTag.Recipient.CompanyName.PropInfo;
			dictionary["Title"] = PropTag.Recipient.Title.PropInfo;
			dictionary["DepartmentName"] = PropTag.Recipient.DepartmentName.PropInfo;
			dictionary["OfficeLocation"] = PropTag.Recipient.OfficeLocation.PropInfo;
			dictionary["PrimaryTelephoneNumber"] = PropTag.Recipient.PrimaryTelephoneNumber.PropInfo;
			dictionary["Business2TelephoneNumber"] = PropTag.Recipient.Business2TelephoneNumber.PropInfo;
			dictionary["Business2TelephoneNumberMv"] = PropTag.Recipient.Business2TelephoneNumberMv.PropInfo;
			dictionary["MobileTelephoneNumber"] = PropTag.Recipient.MobileTelephoneNumber.PropInfo;
			dictionary["RadioTelephoneNumber"] = PropTag.Recipient.RadioTelephoneNumber.PropInfo;
			dictionary["CarTelephoneNumber"] = PropTag.Recipient.CarTelephoneNumber.PropInfo;
			dictionary["OtherTelephoneNumber"] = PropTag.Recipient.OtherTelephoneNumber.PropInfo;
			dictionary["TransmitableDisplayName"] = PropTag.Recipient.TransmitableDisplayName.PropInfo;
			dictionary["PagerTelephoneNumber"] = PropTag.Recipient.PagerTelephoneNumber.PropInfo;
			dictionary["UserCertificate"] = PropTag.Recipient.UserCertificate.PropInfo;
			dictionary["PrimaryFaxNumber"] = PropTag.Recipient.PrimaryFaxNumber.PropInfo;
			dictionary["BusinessFaxNumber"] = PropTag.Recipient.BusinessFaxNumber.PropInfo;
			dictionary["HomeFaxNumber"] = PropTag.Recipient.HomeFaxNumber.PropInfo;
			dictionary["Country"] = PropTag.Recipient.Country.PropInfo;
			dictionary["Locality"] = PropTag.Recipient.Locality.PropInfo;
			dictionary["StateOrProvince"] = PropTag.Recipient.StateOrProvince.PropInfo;
			dictionary["StreetAddress"] = PropTag.Recipient.StreetAddress.PropInfo;
			dictionary["PostalCode"] = PropTag.Recipient.PostalCode.PropInfo;
			dictionary["PostOfficeBox"] = PropTag.Recipient.PostOfficeBox.PropInfo;
			dictionary["TelexNumber"] = PropTag.Recipient.TelexNumber.PropInfo;
			dictionary["ISDNNumber"] = PropTag.Recipient.ISDNNumber.PropInfo;
			dictionary["AssistantTelephoneNumber"] = PropTag.Recipient.AssistantTelephoneNumber.PropInfo;
			dictionary["Home2TelephoneNumber"] = PropTag.Recipient.Home2TelephoneNumber.PropInfo;
			dictionary["Home2TelephoneNumberMv"] = PropTag.Recipient.Home2TelephoneNumberMv.PropInfo;
			dictionary["Assistant"] = PropTag.Recipient.Assistant.PropInfo;
			dictionary["SendRichInfo"] = PropTag.Recipient.SendRichInfo.PropInfo;
			dictionary["WeddingAnniversary"] = PropTag.Recipient.WeddingAnniversary.PropInfo;
			dictionary["Birthday"] = PropTag.Recipient.Birthday.PropInfo;
			dictionary["Hobbies"] = PropTag.Recipient.Hobbies.PropInfo;
			dictionary["MiddleName"] = PropTag.Recipient.MiddleName.PropInfo;
			dictionary["DisplayNamePrefix"] = PropTag.Recipient.DisplayNamePrefix.PropInfo;
			dictionary["Profession"] = PropTag.Recipient.Profession.PropInfo;
			dictionary["ReferredByName"] = PropTag.Recipient.ReferredByName.PropInfo;
			dictionary["SpouseName"] = PropTag.Recipient.SpouseName.PropInfo;
			dictionary["ComputerNetworkName"] = PropTag.Recipient.ComputerNetworkName.PropInfo;
			dictionary["CustomerId"] = PropTag.Recipient.CustomerId.PropInfo;
			dictionary["TTYTDDPhoneNumber"] = PropTag.Recipient.TTYTDDPhoneNumber.PropInfo;
			dictionary["FTPSite"] = PropTag.Recipient.FTPSite.PropInfo;
			dictionary["Gender"] = PropTag.Recipient.Gender.PropInfo;
			dictionary["ManagerName"] = PropTag.Recipient.ManagerName.PropInfo;
			dictionary["NickName"] = PropTag.Recipient.NickName.PropInfo;
			dictionary["PersonalHomePage"] = PropTag.Recipient.PersonalHomePage.PropInfo;
			dictionary["BusinessHomePage"] = PropTag.Recipient.BusinessHomePage.PropInfo;
			dictionary["ContactVersion"] = PropTag.Recipient.ContactVersion.PropInfo;
			dictionary["ContactEntryIds"] = PropTag.Recipient.ContactEntryIds.PropInfo;
			dictionary["ContactAddressTypes"] = PropTag.Recipient.ContactAddressTypes.PropInfo;
			dictionary["ContactDefaultAddressIndex"] = PropTag.Recipient.ContactDefaultAddressIndex.PropInfo;
			dictionary["ContactEmailAddress"] = PropTag.Recipient.ContactEmailAddress.PropInfo;
			dictionary["CompanyMainPhoneNumber"] = PropTag.Recipient.CompanyMainPhoneNumber.PropInfo;
			dictionary["ChildrensNames"] = PropTag.Recipient.ChildrensNames.PropInfo;
			dictionary["HomeAddressCity"] = PropTag.Recipient.HomeAddressCity.PropInfo;
			dictionary["HomeAddressCountry"] = PropTag.Recipient.HomeAddressCountry.PropInfo;
			dictionary["HomeAddressPostalCode"] = PropTag.Recipient.HomeAddressPostalCode.PropInfo;
			dictionary["HomeAddressStateOrProvince"] = PropTag.Recipient.HomeAddressStateOrProvince.PropInfo;
			dictionary["HomeAddressStreet"] = PropTag.Recipient.HomeAddressStreet.PropInfo;
			dictionary["HomeAddressPostOfficeBox"] = PropTag.Recipient.HomeAddressPostOfficeBox.PropInfo;
			dictionary["OtherAddressCity"] = PropTag.Recipient.OtherAddressCity.PropInfo;
			dictionary["OtherAddressCountry"] = PropTag.Recipient.OtherAddressCountry.PropInfo;
			dictionary["OtherAddressPostalCode"] = PropTag.Recipient.OtherAddressPostalCode.PropInfo;
			dictionary["OtherAddressStateOrProvince"] = PropTag.Recipient.OtherAddressStateOrProvince.PropInfo;
			dictionary["OtherAddressStreet"] = PropTag.Recipient.OtherAddressStreet.PropInfo;
			dictionary["OtherAddressPostOfficeBox"] = PropTag.Recipient.OtherAddressPostOfficeBox.PropInfo;
			dictionary["UserX509CertificateABSearchPath"] = PropTag.Recipient.UserX509CertificateABSearchPath.PropInfo;
			dictionary["SendInternetEncoding"] = PropTag.Recipient.SendInternetEncoding.PropInfo;
			dictionary["PartnerNetworkId"] = PropTag.Recipient.PartnerNetworkId.PropInfo;
			dictionary["PartnerNetworkUserId"] = PropTag.Recipient.PartnerNetworkUserId.PropInfo;
			dictionary["PartnerNetworkThumbnailPhotoUrl"] = PropTag.Recipient.PartnerNetworkThumbnailPhotoUrl.PropInfo;
			dictionary["PartnerNetworkProfilePhotoUrl"] = PropTag.Recipient.PartnerNetworkProfilePhotoUrl.PropInfo;
			dictionary["PartnerNetworkContactType"] = PropTag.Recipient.PartnerNetworkContactType.PropInfo;
			dictionary["RelevanceScore"] = PropTag.Recipient.RelevanceScore.PropInfo;
			dictionary["IsDistributionListContact"] = PropTag.Recipient.IsDistributionListContact.PropInfo;
			dictionary["IsPromotedContact"] = PropTag.Recipient.IsPromotedContact.PropInfo;
			dictionary["OrgUnitName"] = PropTag.Recipient.OrgUnitName.PropInfo;
			dictionary["OrganizationName"] = PropTag.Recipient.OrganizationName.PropInfo;
			dictionary["TestBlobProperty"] = PropTag.Recipient.TestBlobProperty.PropInfo;
			dictionary["NewAttach"] = PropTag.Recipient.NewAttach.PropInfo;
			dictionary["StartEmbed"] = PropTag.Recipient.StartEmbed.PropInfo;
			dictionary["EndEmbed"] = PropTag.Recipient.EndEmbed.PropInfo;
			dictionary["StartRecip"] = PropTag.Recipient.StartRecip.PropInfo;
			dictionary["EndRecip"] = PropTag.Recipient.EndRecip.PropInfo;
			dictionary["EndCcRecip"] = PropTag.Recipient.EndCcRecip.PropInfo;
			dictionary["EndBccRecip"] = PropTag.Recipient.EndBccRecip.PropInfo;
			dictionary["EndP1Recip"] = PropTag.Recipient.EndP1Recip.PropInfo;
			dictionary["DNPrefix"] = PropTag.Recipient.DNPrefix.PropInfo;
			dictionary["StartTopFolder"] = PropTag.Recipient.StartTopFolder.PropInfo;
			dictionary["StartSubFolder"] = PropTag.Recipient.StartSubFolder.PropInfo;
			dictionary["EndFolder"] = PropTag.Recipient.EndFolder.PropInfo;
			dictionary["StartMessage"] = PropTag.Recipient.StartMessage.PropInfo;
			dictionary["EndMessage"] = PropTag.Recipient.EndMessage.PropInfo;
			dictionary["EndAttach"] = PropTag.Recipient.EndAttach.PropInfo;
			dictionary["EcWarning"] = PropTag.Recipient.EcWarning.PropInfo;
			dictionary["StartFAIMessage"] = PropTag.Recipient.StartFAIMessage.PropInfo;
			dictionary["NewFXFolder"] = PropTag.Recipient.NewFXFolder.PropInfo;
			dictionary["IncrSyncChange"] = PropTag.Recipient.IncrSyncChange.PropInfo;
			dictionary["IncrSyncDelete"] = PropTag.Recipient.IncrSyncDelete.PropInfo;
			dictionary["IncrSyncEnd"] = PropTag.Recipient.IncrSyncEnd.PropInfo;
			dictionary["IncrSyncMessage"] = PropTag.Recipient.IncrSyncMessage.PropInfo;
			dictionary["FastTransferDelProp"] = PropTag.Recipient.FastTransferDelProp.PropInfo;
			dictionary["IdsetGiven"] = PropTag.Recipient.IdsetGiven.PropInfo;
			dictionary["IdsetGivenInt32"] = PropTag.Recipient.IdsetGivenInt32.PropInfo;
			dictionary["FastTransferErrorInfo"] = PropTag.Recipient.FastTransferErrorInfo.PropInfo;
			dictionary["SoftDeletes"] = PropTag.Recipient.SoftDeletes.PropInfo;
			dictionary["IdsetRead"] = PropTag.Recipient.IdsetRead.PropInfo;
			dictionary["IdsetUnread"] = PropTag.Recipient.IdsetUnread.PropInfo;
			dictionary["IncrSyncRead"] = PropTag.Recipient.IncrSyncRead.PropInfo;
			dictionary["IncrSyncStateBegin"] = PropTag.Recipient.IncrSyncStateBegin.PropInfo;
			dictionary["IncrSyncStateEnd"] = PropTag.Recipient.IncrSyncStateEnd.PropInfo;
			dictionary["IncrSyncImailStream"] = PropTag.Recipient.IncrSyncImailStream.PropInfo;
			dictionary["IncrSyncImailStreamContinue"] = PropTag.Recipient.IncrSyncImailStreamContinue.PropInfo;
			dictionary["IncrSyncImailStreamCancel"] = PropTag.Recipient.IncrSyncImailStreamCancel.PropInfo;
			dictionary["IncrSyncImailStream2Continue"] = PropTag.Recipient.IncrSyncImailStream2Continue.PropInfo;
			dictionary["IncrSyncProgressMode"] = PropTag.Recipient.IncrSyncProgressMode.PropInfo;
			dictionary["SyncProgressPerMsg"] = PropTag.Recipient.SyncProgressPerMsg.PropInfo;
			dictionary["IncrSyncMsgPartial"] = PropTag.Recipient.IncrSyncMsgPartial.PropInfo;
			dictionary["IncrSyncGroupInfo"] = PropTag.Recipient.IncrSyncGroupInfo.PropInfo;
			dictionary["IncrSyncGroupId"] = PropTag.Recipient.IncrSyncGroupId.PropInfo;
			dictionary["IncrSyncChangePartial"] = PropTag.Recipient.IncrSyncChangePartial.PropInfo;
			dictionary["RecipientOrder"] = PropTag.Recipient.RecipientOrder.PropInfo;
			dictionary["RecipientSipUri"] = PropTag.Recipient.RecipientSipUri.PropInfo;
			dictionary["RecipientDisplayName"] = PropTag.Recipient.RecipientDisplayName.PropInfo;
			dictionary["RecipientEntryId"] = PropTag.Recipient.RecipientEntryId.PropInfo;
			dictionary["RecipientFlags"] = PropTag.Recipient.RecipientFlags.PropInfo;
			dictionary["RecipientTrackStatus"] = PropTag.Recipient.RecipientTrackStatus.PropInfo;
			dictionary["DotStuffState"] = PropTag.Recipient.DotStuffState.PropInfo;
			dictionary["RecipientNumber"] = PropTag.Recipient.RecipientNumber.PropInfo;
			dictionary["UserDN"] = PropTag.Recipient.UserDN.PropInfo;
			dictionary["CnsetSeen"] = PropTag.Recipient.CnsetSeen.PropInfo;
			dictionary["SourceEntryId"] = PropTag.Recipient.SourceEntryId.PropInfo;
			dictionary["CnsetRead"] = PropTag.Recipient.CnsetRead.PropInfo;
			dictionary["CnsetSeenFAI"] = PropTag.Recipient.CnsetSeenFAI.PropInfo;
			dictionary["IdSetDeleted"] = PropTag.Recipient.IdSetDeleted.PropInfo;
			return dictionary;
		}

		private static Dictionary<uint, StorePropInfo> BuildRecipientPropertyDefinitionsByTag()
		{
			Dictionary<uint, StorePropInfo> dictionary = new Dictionary<uint, StorePropInfo>(394);
			dictionary[2293771U] = PropTag.Recipient.DeliveryReportRequested.PropInfo;
			dictionary[2686987U] = PropTag.Recipient.ReadReceiptRequested.PropInfo;
			dictionary[3276864U] = PropTag.Recipient.ReportTime.PropInfo;
			dictionary[4849675U] = PropTag.Recipient.DiscVal.PropInfo;
			dictionary[201392131U] = PropTag.Recipient.ExplicitConversion.PropInfo;
			dictionary[201588739U] = PropTag.Recipient.NDRReasonCode.PropInfo;
			dictionary[201654275U] = PropTag.Recipient.NDRDiagCode.PropInfo;
			dictionary[201719819U] = PropTag.Recipient.NonReceiptNotificationRequested.PropInfo;
			dictionary[201850891U] = PropTag.Recipient.NonDeliveryReportRequested.PropInfo;
			dictionary[201916674U] = PropTag.Recipient.OriginatorRequestedAlterateRecipient.PropInfo;
			dictionary[202047491U] = PropTag.Recipient.PhysicalDeliveryMode.PropInfo;
			dictionary[202113027U] = PropTag.Recipient.PhysicalDeliveryReportRequest.PropInfo;
			dictionary[202178818U] = PropTag.Recipient.PhysicalForwardingAddress.PropInfo;
			dictionary[202244107U] = PropTag.Recipient.PhysicalForwardingAddressRequested.PropInfo;
			dictionary[202309643U] = PropTag.Recipient.PhysicalForwardingProhibited.PropInfo;
			dictionary[202440962U] = PropTag.Recipient.ProofOfDelivery.PropInfo;
			dictionary[202506251U] = PropTag.Recipient.ProofOfDeliveryRequested.PropInfo;
			dictionary[202572034U] = PropTag.Recipient.RecipientCertificate.PropInfo;
			dictionary[202637343U] = PropTag.Recipient.RecipientNumberForAdvice.PropInfo;
			dictionary[202702851U] = PropTag.Recipient.RecipientType.PropInfo;
			dictionary[203161603U] = PropTag.Recipient.TypeOfMTSUser.PropInfo;
			dictionary[235798539U] = PropTag.Recipient.DiscreteValues.PropInfo;
			dictionary[235864075U] = PropTag.Recipient.Responsibility.PropInfo;
			dictionary[236257283U] = PropTag.Recipient.RecipientStatus.PropInfo;
			dictionary[267780354U] = PropTag.Recipient.InstanceKey.PropInfo;
			dictionary[267845635U] = PropTag.Recipient.AccessLevel.PropInfo;
			dictionary[267976962U] = PropTag.Recipient.RecordKey.PropInfo;
			dictionary[267976955U] = PropTag.Recipient.RecordKeySvrEid.PropInfo;
			dictionary[268304387U] = PropTag.Recipient.ObjectType.PropInfo;
			dictionary[268370178U] = PropTag.Recipient.EntryId.PropInfo;
			dictionary[268370171U] = PropTag.Recipient.EntryIdSvrEid.PropInfo;
			dictionary[805306371U] = PropTag.Recipient.RowId.PropInfo;
			dictionary[805371935U] = PropTag.Recipient.DisplayName.PropInfo;
			dictionary[805437471U] = PropTag.Recipient.AddressType.PropInfo;
			dictionary[805503007U] = PropTag.Recipient.EmailAddress.PropInfo;
			dictionary[805568543U] = PropTag.Recipient.Comment.PropInfo;
			dictionary[805830720U] = PropTag.Recipient.LastModificationTime.PropInfo;
			dictionary[806027522U] = PropTag.Recipient.SearchKey.PropInfo;
			dictionary[806027515U] = PropTag.Recipient.SearchKeySvrEid.PropInfo;
			dictionary[906297357U] = PropTag.Recipient.DetailsTable.PropInfo;
			dictionary[956301315U] = PropTag.Recipient.DisplayType.PropInfo;
			dictionary[972947487U] = PropTag.Recipient.SmtpAddress.PropInfo;
			dictionary[973013023U] = PropTag.Recipient.SimpleDisplayName.PropInfo;
			dictionary[973078559U] = PropTag.Recipient.Account.PropInfo;
			dictionary[973144322U] = PropTag.Recipient.AlternateRecipient.PropInfo;
			dictionary[973209631U] = PropTag.Recipient.CallbackTelephoneNumber.PropInfo;
			dictionary[973406239U] = PropTag.Recipient.Generation.PropInfo;
			dictionary[973471775U] = PropTag.Recipient.GivenName.PropInfo;
			dictionary[973537311U] = PropTag.Recipient.GovernmentIDNumber.PropInfo;
			dictionary[973602847U] = PropTag.Recipient.BusinessTelephoneNumber.PropInfo;
			dictionary[973668383U] = PropTag.Recipient.HomeTelephoneNumber.PropInfo;
			dictionary[973733919U] = PropTag.Recipient.Initials.PropInfo;
			dictionary[973799455U] = PropTag.Recipient.Keyword.PropInfo;
			dictionary[973864991U] = PropTag.Recipient.Language.PropInfo;
			dictionary[973930527U] = PropTag.Recipient.Location.PropInfo;
			dictionary[973996043U] = PropTag.Recipient.MailPermission.PropInfo;
			dictionary[974127135U] = PropTag.Recipient.OrganizationalIDNumber.PropInfo;
			dictionary[974192671U] = PropTag.Recipient.SurName.PropInfo;
			dictionary[974258434U] = PropTag.Recipient.OriginalEntryId.PropInfo;
			dictionary[974323743U] = PropTag.Recipient.OriginalDisplayName.PropInfo;
			dictionary[974389506U] = PropTag.Recipient.OriginalSearchKey.PropInfo;
			dictionary[974454815U] = PropTag.Recipient.PostalAddress.PropInfo;
			dictionary[974520351U] = PropTag.Recipient.CompanyName.PropInfo;
			dictionary[974585887U] = PropTag.Recipient.Title.PropInfo;
			dictionary[974651423U] = PropTag.Recipient.DepartmentName.PropInfo;
			dictionary[974716959U] = PropTag.Recipient.OfficeLocation.PropInfo;
			dictionary[974782495U] = PropTag.Recipient.PrimaryTelephoneNumber.PropInfo;
			dictionary[974848031U] = PropTag.Recipient.Business2TelephoneNumber.PropInfo;
			dictionary[974852127U] = PropTag.Recipient.Business2TelephoneNumberMv.PropInfo;
			dictionary[974913567U] = PropTag.Recipient.MobileTelephoneNumber.PropInfo;
			dictionary[974979103U] = PropTag.Recipient.RadioTelephoneNumber.PropInfo;
			dictionary[975044639U] = PropTag.Recipient.CarTelephoneNumber.PropInfo;
			dictionary[975110175U] = PropTag.Recipient.OtherTelephoneNumber.PropInfo;
			dictionary[975175711U] = PropTag.Recipient.TransmitableDisplayName.PropInfo;
			dictionary[975241247U] = PropTag.Recipient.PagerTelephoneNumber.PropInfo;
			dictionary[975307010U] = PropTag.Recipient.UserCertificate.PropInfo;
			dictionary[975372319U] = PropTag.Recipient.PrimaryFaxNumber.PropInfo;
			dictionary[975437855U] = PropTag.Recipient.BusinessFaxNumber.PropInfo;
			dictionary[975503391U] = PropTag.Recipient.HomeFaxNumber.PropInfo;
			dictionary[975568927U] = PropTag.Recipient.Country.PropInfo;
			dictionary[975634463U] = PropTag.Recipient.Locality.PropInfo;
			dictionary[975699999U] = PropTag.Recipient.StateOrProvince.PropInfo;
			dictionary[975765535U] = PropTag.Recipient.StreetAddress.PropInfo;
			dictionary[975831071U] = PropTag.Recipient.PostalCode.PropInfo;
			dictionary[975896607U] = PropTag.Recipient.PostOfficeBox.PropInfo;
			dictionary[975962143U] = PropTag.Recipient.TelexNumber.PropInfo;
			dictionary[976027679U] = PropTag.Recipient.ISDNNumber.PropInfo;
			dictionary[976093215U] = PropTag.Recipient.AssistantTelephoneNumber.PropInfo;
			dictionary[976158751U] = PropTag.Recipient.Home2TelephoneNumber.PropInfo;
			dictionary[976162847U] = PropTag.Recipient.Home2TelephoneNumberMv.PropInfo;
			dictionary[976224287U] = PropTag.Recipient.Assistant.PropInfo;
			dictionary[977272843U] = PropTag.Recipient.SendRichInfo.PropInfo;
			dictionary[977338432U] = PropTag.Recipient.WeddingAnniversary.PropInfo;
			dictionary[977403968U] = PropTag.Recipient.Birthday.PropInfo;
			dictionary[977469471U] = PropTag.Recipient.Hobbies.PropInfo;
			dictionary[977535007U] = PropTag.Recipient.MiddleName.PropInfo;
			dictionary[977600543U] = PropTag.Recipient.DisplayNamePrefix.PropInfo;
			dictionary[977666079U] = PropTag.Recipient.Profession.PropInfo;
			dictionary[977731615U] = PropTag.Recipient.ReferredByName.PropInfo;
			dictionary[977797151U] = PropTag.Recipient.SpouseName.PropInfo;
			dictionary[977862687U] = PropTag.Recipient.ComputerNetworkName.PropInfo;
			dictionary[977928223U] = PropTag.Recipient.CustomerId.PropInfo;
			dictionary[977993759U] = PropTag.Recipient.TTYTDDPhoneNumber.PropInfo;
			dictionary[978059295U] = PropTag.Recipient.FTPSite.PropInfo;
			dictionary[978124802U] = PropTag.Recipient.Gender.PropInfo;
			dictionary[978190367U] = PropTag.Recipient.ManagerName.PropInfo;
			dictionary[978255903U] = PropTag.Recipient.NickName.PropInfo;
			dictionary[978321439U] = PropTag.Recipient.PersonalHomePage.PropInfo;
			dictionary[978386975U] = PropTag.Recipient.BusinessHomePage.PropInfo;
			dictionary[978452552U] = PropTag.Recipient.ContactVersion.PropInfo;
			dictionary[978522370U] = PropTag.Recipient.ContactEntryIds.PropInfo;
			dictionary[978587679U] = PropTag.Recipient.ContactAddressTypes.PropInfo;
			dictionary[978649091U] = PropTag.Recipient.ContactDefaultAddressIndex.PropInfo;
			dictionary[978718751U] = PropTag.Recipient.ContactEmailAddress.PropInfo;
			dictionary[978780191U] = PropTag.Recipient.CompanyMainPhoneNumber.PropInfo;
			dictionary[978849823U] = PropTag.Recipient.ChildrensNames.PropInfo;
			dictionary[978911263U] = PropTag.Recipient.HomeAddressCity.PropInfo;
			dictionary[978976799U] = PropTag.Recipient.HomeAddressCountry.PropInfo;
			dictionary[979042335U] = PropTag.Recipient.HomeAddressPostalCode.PropInfo;
			dictionary[979107871U] = PropTag.Recipient.HomeAddressStateOrProvince.PropInfo;
			dictionary[979173407U] = PropTag.Recipient.HomeAddressStreet.PropInfo;
			dictionary[979238943U] = PropTag.Recipient.HomeAddressPostOfficeBox.PropInfo;
			dictionary[979304479U] = PropTag.Recipient.OtherAddressCity.PropInfo;
			dictionary[979370015U] = PropTag.Recipient.OtherAddressCountry.PropInfo;
			dictionary[979435551U] = PropTag.Recipient.OtherAddressPostalCode.PropInfo;
			dictionary[979501087U] = PropTag.Recipient.OtherAddressStateOrProvince.PropInfo;
			dictionary[979566623U] = PropTag.Recipient.OtherAddressStreet.PropInfo;
			dictionary[979632159U] = PropTag.Recipient.OtherAddressPostOfficeBox.PropInfo;
			dictionary[980422914U] = PropTag.Recipient.UserX509CertificateABSearchPath.PropInfo;
			dictionary[980484099U] = PropTag.Recipient.SendInternetEncoding.PropInfo;
			dictionary[980811807U] = PropTag.Recipient.PartnerNetworkId.PropInfo;
			dictionary[980877343U] = PropTag.Recipient.PartnerNetworkUserId.PropInfo;
			dictionary[980942879U] = PropTag.Recipient.PartnerNetworkThumbnailPhotoUrl.PropInfo;
			dictionary[981008415U] = PropTag.Recipient.PartnerNetworkProfilePhotoUrl.PropInfo;
			dictionary[981073951U] = PropTag.Recipient.PartnerNetworkContactType.PropInfo;
			dictionary[981139459U] = PropTag.Recipient.RelevanceScore.PropInfo;
			dictionary[981205003U] = PropTag.Recipient.IsDistributionListContact.PropInfo;
			dictionary[981270539U] = PropTag.Recipient.IsPromotedContact.PropInfo;
			dictionary[1006501919U] = PropTag.Recipient.OrgUnitName.PropInfo;
			dictionary[1006567455U] = PropTag.Recipient.OrganizationName.PropInfo;
			dictionary[1023410196U] = PropTag.Recipient.TestBlobProperty.PropInfo;
			dictionary[1073741827U] = PropTag.Recipient.NewAttach.PropInfo;
			dictionary[1073807363U] = PropTag.Recipient.StartEmbed.PropInfo;
			dictionary[1073872899U] = PropTag.Recipient.EndEmbed.PropInfo;
			dictionary[1073938435U] = PropTag.Recipient.StartRecip.PropInfo;
			dictionary[1074003971U] = PropTag.Recipient.EndRecip.PropInfo;
			dictionary[1074069507U] = PropTag.Recipient.EndCcRecip.PropInfo;
			dictionary[1074135043U] = PropTag.Recipient.EndBccRecip.PropInfo;
			dictionary[1074200579U] = PropTag.Recipient.EndP1Recip.PropInfo;
			dictionary[1074266143U] = PropTag.Recipient.DNPrefix.PropInfo;
			dictionary[1074331651U] = PropTag.Recipient.StartTopFolder.PropInfo;
			dictionary[1074397187U] = PropTag.Recipient.StartSubFolder.PropInfo;
			dictionary[1074462723U] = PropTag.Recipient.EndFolder.PropInfo;
			dictionary[1074528259U] = PropTag.Recipient.StartMessage.PropInfo;
			dictionary[1074593795U] = PropTag.Recipient.EndMessage.PropInfo;
			dictionary[1074659331U] = PropTag.Recipient.EndAttach.PropInfo;
			dictionary[1074724867U] = PropTag.Recipient.EcWarning.PropInfo;
			dictionary[1074790403U] = PropTag.Recipient.StartFAIMessage.PropInfo;
			dictionary[1074856194U] = PropTag.Recipient.NewFXFolder.PropInfo;
			dictionary[1074921475U] = PropTag.Recipient.IncrSyncChange.PropInfo;
			dictionary[1074987011U] = PropTag.Recipient.IncrSyncDelete.PropInfo;
			dictionary[1075052547U] = PropTag.Recipient.IncrSyncEnd.PropInfo;
			dictionary[1075118083U] = PropTag.Recipient.IncrSyncMessage.PropInfo;
			dictionary[1075183619U] = PropTag.Recipient.FastTransferDelProp.PropInfo;
			dictionary[1075249410U] = PropTag.Recipient.IdsetGiven.PropInfo;
			dictionary[1075249155U] = PropTag.Recipient.IdsetGivenInt32.PropInfo;
			dictionary[1075314691U] = PropTag.Recipient.FastTransferErrorInfo.PropInfo;
			dictionary[1075904770U] = PropTag.Recipient.SoftDeletes.PropInfo;
			dictionary[1076691202U] = PropTag.Recipient.IdsetRead.PropInfo;
			dictionary[1076756738U] = PropTag.Recipient.IdsetUnread.PropInfo;
			dictionary[1076822019U] = PropTag.Recipient.IncrSyncRead.PropInfo;
			dictionary[1077542915U] = PropTag.Recipient.IncrSyncStateBegin.PropInfo;
			dictionary[1077608451U] = PropTag.Recipient.IncrSyncStateEnd.PropInfo;
			dictionary[1077673987U] = PropTag.Recipient.IncrSyncImailStream.PropInfo;
			dictionary[1080426499U] = PropTag.Recipient.IncrSyncImailStreamContinue.PropInfo;
			dictionary[1080492035U] = PropTag.Recipient.IncrSyncImailStreamCancel.PropInfo;
			dictionary[1081147395U] = PropTag.Recipient.IncrSyncImailStream2Continue.PropInfo;
			dictionary[1081344011U] = PropTag.Recipient.IncrSyncProgressMode.PropInfo;
			dictionary[1081409547U] = PropTag.Recipient.SyncProgressPerMsg.PropInfo;
			dictionary[1081737219U] = PropTag.Recipient.IncrSyncMsgPartial.PropInfo;
			dictionary[1081802755U] = PropTag.Recipient.IncrSyncGroupInfo.PropInfo;
			dictionary[1081868291U] = PropTag.Recipient.IncrSyncGroupId.PropInfo;
			dictionary[1081933827U] = PropTag.Recipient.IncrSyncChangePartial.PropInfo;
			dictionary[1608450051U] = PropTag.Recipient.RecipientOrder.PropInfo;
			dictionary[1608843295U] = PropTag.Recipient.RecipientSipUri.PropInfo;
			dictionary[1609957407U] = PropTag.Recipient.RecipientDisplayName.PropInfo;
			dictionary[1610023170U] = PropTag.Recipient.RecipientEntryId.PropInfo;
			dictionary[1610416131U] = PropTag.Recipient.RecipientFlags.PropInfo;
			dictionary[1610547203U] = PropTag.Recipient.RecipientTrackStatus.PropInfo;
			dictionary[1610678303U] = PropTag.Recipient.DotStuffState.PropInfo;
			dictionary[1717698563U] = PropTag.Recipient.RecipientNumber.PropInfo;
			dictionary[1724514335U] = PropTag.Recipient.UserDN.PropInfo;
			dictionary[1737883906U] = PropTag.Recipient.CnsetSeen.PropInfo;
			dictionary[1739063554U] = PropTag.Recipient.SourceEntryId.PropInfo;
			dictionary[1741816066U] = PropTag.Recipient.CnsetRead.PropInfo;
			dictionary[1742340354U] = PropTag.Recipient.CnsetSeenFAI.PropInfo;
			dictionary[1743061250U] = PropTag.Recipient.IdSetDeleted.PropInfo;
			return dictionary;
		}

		private static WellKnownProperties.ObjectPropertyDefinitions BuildEventPropertyDefinitions()
		{
			Dictionary<string, StorePropInfo> propertiesByTagName = WellKnownProperties.BuildEventPropertyDefinitionsByName();
			Dictionary<uint, StorePropInfo> properties = WellKnownProperties.BuildEventPropertyDefinitionsByTag();
			Dictionary<ushort, StorePropInfo> propIds = new Dictionary<ushort, StorePropInfo>(100);
			WellKnownProperties.PropIdRangeList propIdRanges = WellKnownProperties.BuildPropIdRanges(new WellKnownProperties.TempIdRange[0]);
			return new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.Event,
				Properties = properties,
				PropertiesByTagName = propertiesByTagName,
				PropIds = propIds,
				PropIdRanges = propIdRanges
			};
		}

		private static Dictionary<string, StorePropInfo> BuildEventPropertyDefinitionsByName()
		{
			Dictionary<string, StorePropInfo> dictionary = new Dictionary<string, StorePropInfo>(42, StringComparer.OrdinalIgnoreCase);
			dictionary["EventMailboxGuid"] = PropTag.Event.EventMailboxGuid.PropInfo;
			dictionary["EventCounter"] = PropTag.Event.EventCounter.PropInfo;
			dictionary["EventMask"] = PropTag.Event.EventMask.PropInfo;
			dictionary["EventFid"] = PropTag.Event.EventFid.PropInfo;
			dictionary["EventMid"] = PropTag.Event.EventMid.PropInfo;
			dictionary["EventFidParent"] = PropTag.Event.EventFidParent.PropInfo;
			dictionary["EventFidOld"] = PropTag.Event.EventFidOld.PropInfo;
			dictionary["EventMidOld"] = PropTag.Event.EventMidOld.PropInfo;
			dictionary["EventFidOldParent"] = PropTag.Event.EventFidOldParent.PropInfo;
			dictionary["EventCreatedTime"] = PropTag.Event.EventCreatedTime.PropInfo;
			dictionary["EventMessageClass"] = PropTag.Event.EventMessageClass.PropInfo;
			dictionary["EventItemCount"] = PropTag.Event.EventItemCount.PropInfo;
			dictionary["EventFidRoot"] = PropTag.Event.EventFidRoot.PropInfo;
			dictionary["EventUnreadCount"] = PropTag.Event.EventUnreadCount.PropInfo;
			dictionary["EventTransacId"] = PropTag.Event.EventTransacId.PropInfo;
			dictionary["EventFlags"] = PropTag.Event.EventFlags.PropInfo;
			dictionary["EventExtendedFlags"] = PropTag.Event.EventExtendedFlags.PropInfo;
			dictionary["EventClientType"] = PropTag.Event.EventClientType.PropInfo;
			dictionary["EventSid"] = PropTag.Event.EventSid.PropInfo;
			dictionary["EventDocId"] = PropTag.Event.EventDocId.PropInfo;
			dictionary["MailboxNum"] = PropTag.Event.MailboxNum.PropInfo;
			return dictionary;
		}

		private static Dictionary<uint, StorePropInfo> BuildEventPropertyDefinitionsByTag()
		{
			Dictionary<uint, StorePropInfo> dictionary = new Dictionary<uint, StorePropInfo>(42);
			dictionary[1735000322U] = PropTag.Event.EventMailboxGuid.PropInfo;
			dictionary[1745289236U] = PropTag.Event.EventCounter.PropInfo;
			dictionary[1745354755U] = PropTag.Event.EventMask.PropInfo;
			dictionary[1745420546U] = PropTag.Event.EventFid.PropInfo;
			dictionary[1745486082U] = PropTag.Event.EventMid.PropInfo;
			dictionary[1745551618U] = PropTag.Event.EventFidParent.PropInfo;
			dictionary[1745617154U] = PropTag.Event.EventFidOld.PropInfo;
			dictionary[1745682690U] = PropTag.Event.EventMidOld.PropInfo;
			dictionary[1745748226U] = PropTag.Event.EventFidOldParent.PropInfo;
			dictionary[1745813568U] = PropTag.Event.EventCreatedTime.PropInfo;
			dictionary[1745879071U] = PropTag.Event.EventMessageClass.PropInfo;
			dictionary[1745944579U] = PropTag.Event.EventItemCount.PropInfo;
			dictionary[1746010370U] = PropTag.Event.EventFidRoot.PropInfo;
			dictionary[1746075651U] = PropTag.Event.EventUnreadCount.PropInfo;
			dictionary[1746141187U] = PropTag.Event.EventTransacId.PropInfo;
			dictionary[1746206723U] = PropTag.Event.EventFlags.PropInfo;
			dictionary[1746403348U] = PropTag.Event.EventExtendedFlags.PropInfo;
			dictionary[1746468867U] = PropTag.Event.EventClientType.PropInfo;
			dictionary[1746534658U] = PropTag.Event.EventSid.PropInfo;
			dictionary[1746599939U] = PropTag.Event.EventDocId.PropInfo;
			dictionary[1746862083U] = PropTag.Event.MailboxNum.PropInfo;
			return dictionary;
		}

		private static WellKnownProperties.ObjectPropertyDefinitions BuildPermissionViewPropertyDefinitions()
		{
			Dictionary<string, StorePropInfo> propertiesByTagName = WellKnownProperties.BuildPermissionViewPropertyDefinitionsByName();
			Dictionary<uint, StorePropInfo> properties = WellKnownProperties.BuildPermissionViewPropertyDefinitionsByTag();
			Dictionary<ushort, StorePropInfo> propIds = new Dictionary<ushort, StorePropInfo>(100);
			WellKnownProperties.PropIdRangeList propIdRanges = WellKnownProperties.BuildPropIdRanges(new WellKnownProperties.TempIdRange[0]);
			return new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.PermissionView,
				Properties = properties,
				PropertiesByTagName = propertiesByTagName,
				PropIds = propIds,
				PropIdRanges = propIdRanges
			};
		}

		private static Dictionary<string, StorePropInfo> BuildPermissionViewPropertyDefinitionsByName()
		{
			Dictionary<string, StorePropInfo> dictionary = new Dictionary<string, StorePropInfo>(12, StringComparer.OrdinalIgnoreCase);
			dictionary["EntryId"] = PropTag.PermissionView.EntryId.PropInfo;
			dictionary["MemberId"] = PropTag.PermissionView.MemberId.PropInfo;
			dictionary["MemberName"] = PropTag.PermissionView.MemberName.PropInfo;
			dictionary["MemberRights"] = PropTag.PermissionView.MemberRights.PropInfo;
			dictionary["MemberSecurityIdentifier"] = PropTag.PermissionView.MemberSecurityIdentifier.PropInfo;
			dictionary["MemberIsGroup"] = PropTag.PermissionView.MemberIsGroup.PropInfo;
			return dictionary;
		}

		private static Dictionary<uint, StorePropInfo> BuildPermissionViewPropertyDefinitionsByTag()
		{
			Dictionary<uint, StorePropInfo> dictionary = new Dictionary<uint, StorePropInfo>(12);
			dictionary[268370178U] = PropTag.PermissionView.EntryId.PropInfo;
			dictionary[1718681620U] = PropTag.PermissionView.MemberId.PropInfo;
			dictionary[1718747167U] = PropTag.PermissionView.MemberName.PropInfo;
			dictionary[1718812675U] = PropTag.PermissionView.MemberRights.PropInfo;
			dictionary[1718878466U] = PropTag.PermissionView.MemberSecurityIdentifier.PropInfo;
			dictionary[1718943755U] = PropTag.PermissionView.MemberIsGroup.PropInfo;
			return dictionary;
		}

		private static WellKnownProperties.ObjectPropertyDefinitions BuildViewDefinitionPropertyDefinitions()
		{
			Dictionary<string, StorePropInfo> propertiesByTagName = WellKnownProperties.BuildViewDefinitionPropertyDefinitionsByName();
			Dictionary<uint, StorePropInfo> properties = WellKnownProperties.BuildViewDefinitionPropertyDefinitionsByTag();
			Dictionary<ushort, StorePropInfo> propIds = new Dictionary<ushort, StorePropInfo>(100);
			WellKnownProperties.PropIdRangeList propIdRanges = WellKnownProperties.BuildPropIdRanges(new WellKnownProperties.TempIdRange[0]);
			return new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.ViewDefinition,
				Properties = properties,
				PropertiesByTagName = propertiesByTagName,
				PropIds = propIds,
				PropIdRanges = propIdRanges
			};
		}

		private static Dictionary<string, StorePropInfo> BuildViewDefinitionPropertyDefinitionsByName()
		{
			Dictionary<string, StorePropInfo> dictionary = new Dictionary<string, StorePropInfo>(18, StringComparer.OrdinalIgnoreCase);
			dictionary["SortOrder"] = PropTag.ViewDefinition.SortOrder.PropInfo;
			dictionary["LCID"] = PropTag.ViewDefinition.LCID.PropInfo;
			dictionary["ViewCoveringPropertyTags"] = PropTag.ViewDefinition.ViewCoveringPropertyTags.PropInfo;
			dictionary["ViewAccessTime"] = PropTag.ViewDefinition.ViewAccessTime.PropInfo;
			dictionary["ICSViewFilter"] = PropTag.ViewDefinition.ICSViewFilter.PropInfo;
			dictionary["SoftDeletedFilter"] = PropTag.ViewDefinition.SoftDeletedFilter.PropInfo;
			dictionary["AssociatedFilter"] = PropTag.ViewDefinition.AssociatedFilter.PropInfo;
			dictionary["ConversationsFilter"] = PropTag.ViewDefinition.ConversationsFilter.PropInfo;
			dictionary["CategCount"] = PropTag.ViewDefinition.CategCount.PropInfo;
			return dictionary;
		}

		private static Dictionary<uint, StorePropInfo> BuildViewDefinitionPropertyDefinitionsByTag()
		{
			Dictionary<uint, StorePropInfo> dictionary = new Dictionary<uint, StorePropInfo>(18);
			dictionary[1734410498U] = PropTag.ViewDefinition.SortOrder.PropInfo;
			dictionary[1735262211U] = PropTag.ViewDefinition.LCID.PropInfo;
			dictionary[1743917059U] = PropTag.ViewDefinition.ViewCoveringPropertyTags.PropInfo;
			dictionary[1743978560U] = PropTag.ViewDefinition.ViewAccessTime.PropInfo;
			dictionary[1744044043U] = PropTag.ViewDefinition.ICSViewFilter.PropInfo;
			dictionary[1746468875U] = PropTag.ViewDefinition.SoftDeletedFilter.PropInfo;
			dictionary[1746534411U] = PropTag.ViewDefinition.AssociatedFilter.PropInfo;
			dictionary[1746599947U] = PropTag.ViewDefinition.ConversationsFilter.PropInfo;
			dictionary[1755185155U] = PropTag.ViewDefinition.CategCount.PropInfo;
			return dictionary;
		}

		private static WellKnownProperties.ObjectPropertyDefinitions BuildRestrictionViewPropertyDefinitions()
		{
			Dictionary<string, StorePropInfo> propertiesByTagName = WellKnownProperties.BuildRestrictionViewPropertyDefinitionsByName();
			Dictionary<uint, StorePropInfo> properties = WellKnownProperties.BuildRestrictionViewPropertyDefinitionsByTag();
			Dictionary<ushort, StorePropInfo> propIds = new Dictionary<ushort, StorePropInfo>(100);
			WellKnownProperties.PropIdRangeList propIdRanges = WellKnownProperties.BuildPropIdRanges(new WellKnownProperties.TempIdRange[0]);
			return new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.RestrictionView,
				Properties = properties,
				PropertiesByTagName = propertiesByTagName,
				PropIds = propIds,
				PropIdRanges = propIdRanges
			};
		}

		private static Dictionary<string, StorePropInfo> BuildRestrictionViewPropertyDefinitionsByName()
		{
			Dictionary<string, StorePropInfo> dictionary = new Dictionary<string, StorePropInfo>(14, StringComparer.OrdinalIgnoreCase);
			dictionary["EntryId"] = PropTag.RestrictionView.EntryId.PropInfo;
			dictionary["DisplayName"] = PropTag.RestrictionView.DisplayName.PropInfo;
			dictionary["ContentCount"] = PropTag.RestrictionView.ContentCount.PropInfo;
			dictionary["UnreadCount"] = PropTag.RestrictionView.UnreadCount.PropInfo;
			dictionary["LCIDRestriction"] = PropTag.RestrictionView.LCIDRestriction.PropInfo;
			dictionary["ViewRestriction"] = PropTag.RestrictionView.ViewRestriction.PropInfo;
			dictionary["ViewAccessTime"] = PropTag.RestrictionView.ViewAccessTime.PropInfo;
			return dictionary;
		}

		private static Dictionary<uint, StorePropInfo> BuildRestrictionViewPropertyDefinitionsByTag()
		{
			Dictionary<uint, StorePropInfo> dictionary = new Dictionary<uint, StorePropInfo>(14);
			dictionary[268370178U] = PropTag.RestrictionView.EntryId.PropInfo;
			dictionary[805371935U] = PropTag.RestrictionView.DisplayName.PropInfo;
			dictionary[906100739U] = PropTag.RestrictionView.ContentCount.PropInfo;
			dictionary[906166275U] = PropTag.RestrictionView.UnreadCount.PropInfo;
			dictionary[1736966147U] = PropTag.RestrictionView.LCIDRestriction.PropInfo;
			dictionary[1739587837U] = PropTag.RestrictionView.ViewRestriction.PropInfo;
			dictionary[1743978560U] = PropTag.RestrictionView.ViewAccessTime.PropInfo;
			return dictionary;
		}

		private static WellKnownProperties.ObjectPropertyDefinitions BuildLocalDirectoryPropertyDefinitions()
		{
			Dictionary<string, StorePropInfo> propertiesByTagName = WellKnownProperties.BuildLocalDirectoryPropertyDefinitionsByName();
			Dictionary<uint, StorePropInfo> properties = WellKnownProperties.BuildLocalDirectoryPropertyDefinitionsByTag();
			Dictionary<ushort, StorePropInfo> propIds = new Dictionary<ushort, StorePropInfo>(100);
			WellKnownProperties.PropIdRangeList propIdRanges = WellKnownProperties.BuildPropIdRanges(new WellKnownProperties.TempIdRange[0]);
			return new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.LocalDirectory,
				Properties = properties,
				PropertiesByTagName = propertiesByTagName,
				PropIds = propIds,
				PropIdRanges = propIdRanges
			};
		}

		private static Dictionary<string, StorePropInfo> BuildLocalDirectoryPropertyDefinitionsByName()
		{
			Dictionary<string, StorePropInfo> dictionary = new Dictionary<string, StorePropInfo>(10, StringComparer.OrdinalIgnoreCase);
			dictionary["LocalDirectoryEntryID"] = PropTag.LocalDirectory.LocalDirectoryEntryID.PropInfo;
			dictionary["MemberName"] = PropTag.LocalDirectory.MemberName.PropInfo;
			dictionary["MemberEmail"] = PropTag.LocalDirectory.MemberEmail.PropInfo;
			dictionary["MemberExternalId"] = PropTag.LocalDirectory.MemberExternalId.PropInfo;
			dictionary["MemberSID"] = PropTag.LocalDirectory.MemberSID.PropInfo;
			return dictionary;
		}

		private static Dictionary<uint, StorePropInfo> BuildLocalDirectoryPropertyDefinitionsByTag()
		{
			Dictionary<uint, StorePropInfo> dictionary = new Dictionary<uint, StorePropInfo>(10);
			dictionary[873857282U] = PropTag.LocalDirectory.LocalDirectoryEntryID.PropInfo;
			dictionary[1718747167U] = PropTag.LocalDirectory.MemberName.PropInfo;
			dictionary[1747517471U] = PropTag.LocalDirectory.MemberEmail.PropInfo;
			dictionary[1747583007U] = PropTag.LocalDirectory.MemberExternalId.PropInfo;
			dictionary[1747648770U] = PropTag.LocalDirectory.MemberSID.PropInfo;
			return dictionary;
		}

		private static WellKnownProperties.ObjectPropertyDefinitions BuildResourceDigestPropertyDefinitions()
		{
			Dictionary<string, StorePropInfo> propertiesByTagName = WellKnownProperties.BuildResourceDigestPropertyDefinitionsByName();
			Dictionary<uint, StorePropInfo> properties = WellKnownProperties.BuildResourceDigestPropertyDefinitionsByTag();
			Dictionary<ushort, StorePropInfo> propIds = new Dictionary<ushort, StorePropInfo>(100);
			WellKnownProperties.PropIdRangeList propIdRanges = WellKnownProperties.BuildPropIdRanges(new WellKnownProperties.TempIdRange[0]);
			return new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.ResourceDigest,
				Properties = properties,
				PropertiesByTagName = propertiesByTagName,
				PropIds = propIds,
				PropIdRanges = propIdRanges
			};
		}

		private static Dictionary<string, StorePropInfo> BuildResourceDigestPropertyDefinitionsByName()
		{
			Dictionary<string, StorePropInfo> dictionary = new Dictionary<string, StorePropInfo>(30, StringComparer.OrdinalIgnoreCase);
			dictionary["DisplayName"] = PropTag.ResourceDigest.DisplayName.PropInfo;
			dictionary["MailboxDSGuid"] = PropTag.ResourceDigest.MailboxDSGuid.PropInfo;
			dictionary["TimeInServer"] = PropTag.ResourceDigest.TimeInServer.PropInfo;
			dictionary["TimeInCpu"] = PropTag.ResourceDigest.TimeInCpu.PropInfo;
			dictionary["RopCount"] = PropTag.ResourceDigest.RopCount.PropInfo;
			dictionary["PageRead"] = PropTag.ResourceDigest.PageRead.PropInfo;
			dictionary["PagePreread"] = PropTag.ResourceDigest.PagePreread.PropInfo;
			dictionary["LogRecordCount"] = PropTag.ResourceDigest.LogRecordCount.PropInfo;
			dictionary["LogRecordBytes"] = PropTag.ResourceDigest.LogRecordBytes.PropInfo;
			dictionary["LdapReads"] = PropTag.ResourceDigest.LdapReads.PropInfo;
			dictionary["LdapSearches"] = PropTag.ResourceDigest.LdapSearches.PropInfo;
			dictionary["DigestCategory"] = PropTag.ResourceDigest.DigestCategory.PropInfo;
			dictionary["SampleId"] = PropTag.ResourceDigest.SampleId.PropInfo;
			dictionary["SampleTime"] = PropTag.ResourceDigest.SampleTime.PropInfo;
			dictionary["MailboxQuarantined"] = PropTag.ResourceDigest.MailboxQuarantined.PropInfo;
			return dictionary;
		}

		private static Dictionary<uint, StorePropInfo> BuildResourceDigestPropertyDefinitionsByTag()
		{
			Dictionary<uint, StorePropInfo> dictionary = new Dictionary<uint, StorePropInfo>(30);
			dictionary[805371935U] = PropTag.ResourceDigest.DisplayName.PropInfo;
			dictionary[1728512258U] = PropTag.ResourceDigest.MailboxDSGuid.PropInfo;
			dictionary[1731002371U] = PropTag.ResourceDigest.TimeInServer.PropInfo;
			dictionary[1731067907U] = PropTag.ResourceDigest.TimeInCpu.PropInfo;
			dictionary[1731133443U] = PropTag.ResourceDigest.RopCount.PropInfo;
			dictionary[1731198979U] = PropTag.ResourceDigest.PageRead.PropInfo;
			dictionary[1731264515U] = PropTag.ResourceDigest.PagePreread.PropInfo;
			dictionary[1731330051U] = PropTag.ResourceDigest.LogRecordCount.PropInfo;
			dictionary[1731395587U] = PropTag.ResourceDigest.LogRecordBytes.PropInfo;
			dictionary[1731461123U] = PropTag.ResourceDigest.LdapReads.PropInfo;
			dictionary[1731526659U] = PropTag.ResourceDigest.LdapSearches.PropInfo;
			dictionary[1731592223U] = PropTag.ResourceDigest.DigestCategory.PropInfo;
			dictionary[1731657731U] = PropTag.ResourceDigest.SampleId.PropInfo;
			dictionary[1731723328U] = PropTag.ResourceDigest.SampleTime.PropInfo;
			dictionary[1746534411U] = PropTag.ResourceDigest.MailboxQuarantined.PropInfo;
			return dictionary;
		}

		private static WellKnownProperties.ObjectPropertyDefinitions BuildIcsStatePropertyDefinitions()
		{
			Dictionary<string, StorePropInfo> propertiesByTagName = WellKnownProperties.BuildIcsStatePropertyDefinitionsByName();
			Dictionary<uint, StorePropInfo> properties = WellKnownProperties.BuildIcsStatePropertyDefinitionsByTag();
			Dictionary<ushort, StorePropInfo> propIds = new Dictionary<ushort, StorePropInfo>(100);
			WellKnownProperties.PropIdRangeList propIdRanges = WellKnownProperties.BuildPropIdRanges(new WellKnownProperties.TempIdRange[0]);
			return new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.IcsState,
				Properties = properties,
				PropertiesByTagName = propertiesByTagName,
				PropIds = propIds,
				PropIdRanges = propIdRanges
			};
		}

		private static Dictionary<string, StorePropInfo> BuildIcsStatePropertyDefinitionsByName()
		{
			Dictionary<string, StorePropInfo> dictionary = new Dictionary<string, StorePropInfo>(10, StringComparer.OrdinalIgnoreCase);
			dictionary["IdsetGiven"] = PropTag.IcsState.IdsetGiven.PropInfo;
			dictionary["IdsetGivenInt32"] = PropTag.IcsState.IdsetGivenInt32.PropInfo;
			dictionary["CnsetSeen"] = PropTag.IcsState.CnsetSeen.PropInfo;
			dictionary["CnsetRead"] = PropTag.IcsState.CnsetRead.PropInfo;
			dictionary["CnsetSeenFAI"] = PropTag.IcsState.CnsetSeenFAI.PropInfo;
			return dictionary;
		}

		private static Dictionary<uint, StorePropInfo> BuildIcsStatePropertyDefinitionsByTag()
		{
			Dictionary<uint, StorePropInfo> dictionary = new Dictionary<uint, StorePropInfo>(10);
			dictionary[1075249410U] = PropTag.IcsState.IdsetGiven.PropInfo;
			dictionary[1075249155U] = PropTag.IcsState.IdsetGivenInt32.PropInfo;
			dictionary[1737883906U] = PropTag.IcsState.CnsetSeen.PropInfo;
			dictionary[1741816066U] = PropTag.IcsState.CnsetRead.PropInfo;
			dictionary[1742340354U] = PropTag.IcsState.CnsetSeenFAI.PropInfo;
			return dictionary;
		}

		private static WellKnownProperties.ObjectPropertyDefinitions BuildInferenceLogPropertyDefinitions()
		{
			Dictionary<string, StorePropInfo> propertiesByTagName = WellKnownProperties.BuildInferenceLogPropertyDefinitionsByName();
			Dictionary<uint, StorePropInfo> properties = WellKnownProperties.BuildInferenceLogPropertyDefinitionsByTag();
			Dictionary<ushort, StorePropInfo> propIds = new Dictionary<ushort, StorePropInfo>(100);
			WellKnownProperties.PropIdRangeList propIdRanges = WellKnownProperties.BuildPropIdRanges(new WellKnownProperties.TempIdRange[0]);
			return new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.InferenceLog,
				Properties = properties,
				PropertiesByTagName = propertiesByTagName,
				PropIds = propIds,
				PropIdRanges = propIdRanges
			};
		}

		private static Dictionary<string, StorePropInfo> BuildInferenceLogPropertyDefinitionsByName()
		{
			Dictionary<string, StorePropInfo> dictionary = new Dictionary<string, StorePropInfo>(50, StringComparer.OrdinalIgnoreCase);
			dictionary["RowId"] = PropTag.InferenceLog.RowId.PropInfo;
			dictionary["MailboxPartitionNumber"] = PropTag.InferenceLog.MailboxPartitionNumber.PropInfo;
			dictionary["MailboxNumberInternal"] = PropTag.InferenceLog.MailboxNumberInternal.PropInfo;
			dictionary["Mid"] = PropTag.InferenceLog.Mid.PropInfo;
			dictionary["MailboxNum"] = PropTag.InferenceLog.MailboxNum.PropInfo;
			dictionary["InferenceActivityId"] = PropTag.InferenceLog.InferenceActivityId.PropInfo;
			dictionary["InferenceClientId"] = PropTag.InferenceLog.InferenceClientId.PropInfo;
			dictionary["InferenceItemId"] = PropTag.InferenceLog.InferenceItemId.PropInfo;
			dictionary["InferenceTimeStamp"] = PropTag.InferenceLog.InferenceTimeStamp.PropInfo;
			dictionary["InferenceWindowId"] = PropTag.InferenceLog.InferenceWindowId.PropInfo;
			dictionary["InferenceSessionId"] = PropTag.InferenceLog.InferenceSessionId.PropInfo;
			dictionary["InferenceFolderId"] = PropTag.InferenceLog.InferenceFolderId.PropInfo;
			dictionary["InferenceOofEnabled"] = PropTag.InferenceLog.InferenceOofEnabled.PropInfo;
			dictionary["InferenceDeleteType"] = PropTag.InferenceLog.InferenceDeleteType.PropInfo;
			dictionary["InferenceBrowser"] = PropTag.InferenceLog.InferenceBrowser.PropInfo;
			dictionary["InferenceLocaleId"] = PropTag.InferenceLog.InferenceLocaleId.PropInfo;
			dictionary["InferenceLocation"] = PropTag.InferenceLog.InferenceLocation.PropInfo;
			dictionary["InferenceConversationId"] = PropTag.InferenceLog.InferenceConversationId.PropInfo;
			dictionary["InferenceIpAddress"] = PropTag.InferenceLog.InferenceIpAddress.PropInfo;
			dictionary["InferenceTimeZone"] = PropTag.InferenceLog.InferenceTimeZone.PropInfo;
			dictionary["InferenceCategory"] = PropTag.InferenceLog.InferenceCategory.PropInfo;
			dictionary["InferenceAttachmentId"] = PropTag.InferenceLog.InferenceAttachmentId.PropInfo;
			dictionary["InferenceGlobalObjectId"] = PropTag.InferenceLog.InferenceGlobalObjectId.PropInfo;
			dictionary["InferenceModuleSelected"] = PropTag.InferenceLog.InferenceModuleSelected.PropInfo;
			dictionary["InferenceLayoutType"] = PropTag.InferenceLog.InferenceLayoutType.PropInfo;
			return dictionary;
		}

		private static Dictionary<uint, StorePropInfo> BuildInferenceLogPropertyDefinitionsByTag()
		{
			Dictionary<uint, StorePropInfo> dictionary = new Dictionary<uint, StorePropInfo>(50);
			dictionary[805306371U] = PropTag.InferenceLog.RowId.PropInfo;
			dictionary[1033830403U] = PropTag.InferenceLog.MailboxPartitionNumber.PropInfo;
			dictionary[1033895939U] = PropTag.InferenceLog.MailboxNumberInternal.PropInfo;
			dictionary[1732902932U] = PropTag.InferenceLog.Mid.PropInfo;
			dictionary[1746862083U] = PropTag.InferenceLog.MailboxNum.PropInfo;
			dictionary[1746927619U] = PropTag.InferenceLog.InferenceActivityId.PropInfo;
			dictionary[1746993155U] = PropTag.InferenceLog.InferenceClientId.PropInfo;
			dictionary[1747058946U] = PropTag.InferenceLog.InferenceItemId.PropInfo;
			dictionary[1747124288U] = PropTag.InferenceLog.InferenceTimeStamp.PropInfo;
			dictionary[1747189832U] = PropTag.InferenceLog.InferenceWindowId.PropInfo;
			dictionary[1747255368U] = PropTag.InferenceLog.InferenceSessionId.PropInfo;
			dictionary[1747321090U] = PropTag.InferenceLog.InferenceFolderId.PropInfo;
			dictionary[1747386379U] = PropTag.InferenceLog.InferenceOofEnabled.PropInfo;
			dictionary[1747451907U] = PropTag.InferenceLog.InferenceDeleteType.PropInfo;
			dictionary[1747517471U] = PropTag.InferenceLog.InferenceBrowser.PropInfo;
			dictionary[1747582979U] = PropTag.InferenceLog.InferenceLocaleId.PropInfo;
			dictionary[1747648543U] = PropTag.InferenceLog.InferenceLocation.PropInfo;
			dictionary[1747714306U] = PropTag.InferenceLog.InferenceConversationId.PropInfo;
			dictionary[1747779615U] = PropTag.InferenceLog.InferenceIpAddress.PropInfo;
			dictionary[1747845151U] = PropTag.InferenceLog.InferenceTimeZone.PropInfo;
			dictionary[1747910687U] = PropTag.InferenceLog.InferenceCategory.PropInfo;
			dictionary[1747976450U] = PropTag.InferenceLog.InferenceAttachmentId.PropInfo;
			dictionary[1748041986U] = PropTag.InferenceLog.InferenceGlobalObjectId.PropInfo;
			dictionary[1748107267U] = PropTag.InferenceLog.InferenceModuleSelected.PropInfo;
			dictionary[1748172831U] = PropTag.InferenceLog.InferenceLayoutType.PropInfo;
			return dictionary;
		}

		private static WellKnownProperties.ObjectPropertyDefinitions BuildProcessInfoPropertyDefinitions()
		{
			Dictionary<string, StorePropInfo> propertiesByTagName = WellKnownProperties.BuildProcessInfoPropertyDefinitionsByName();
			Dictionary<uint, StorePropInfo> properties = WellKnownProperties.BuildProcessInfoPropertyDefinitionsByTag();
			Dictionary<ushort, StorePropInfo> propIds = new Dictionary<ushort, StorePropInfo>(100);
			WellKnownProperties.PropIdRangeList propIdRanges = WellKnownProperties.BuildPropIdRanges(new WellKnownProperties.TempIdRange[0]);
			return new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.ProcessInfo,
				Properties = properties,
				PropertiesByTagName = propertiesByTagName,
				PropIds = propIds,
				PropIdRanges = propIdRanges
			};
		}

		private static Dictionary<string, StorePropInfo> BuildProcessInfoPropertyDefinitionsByName()
		{
			Dictionary<string, StorePropInfo> dictionary = new Dictionary<string, StorePropInfo>(10, StringComparer.OrdinalIgnoreCase);
			dictionary["WorkerProcessId"] = PropTag.ProcessInfo.WorkerProcessId.PropInfo;
			dictionary["MinimumDatabaseSchemaVersion"] = PropTag.ProcessInfo.MinimumDatabaseSchemaVersion.PropInfo;
			dictionary["MaximumDatabaseSchemaVersion"] = PropTag.ProcessInfo.MaximumDatabaseSchemaVersion.PropInfo;
			dictionary["CurrentDatabaseSchemaVersion"] = PropTag.ProcessInfo.CurrentDatabaseSchemaVersion.PropInfo;
			dictionary["RequestedDatabaseSchemaVersion"] = PropTag.ProcessInfo.RequestedDatabaseSchemaVersion.PropInfo;
			return dictionary;
		}

		private static Dictionary<uint, StorePropInfo> BuildProcessInfoPropertyDefinitionsByTag()
		{
			Dictionary<uint, StorePropInfo> dictionary = new Dictionary<uint, StorePropInfo>(10);
			dictionary[1721171971U] = PropTag.ProcessInfo.WorkerProcessId.PropInfo;
			dictionary[1721237507U] = PropTag.ProcessInfo.MinimumDatabaseSchemaVersion.PropInfo;
			dictionary[1721303043U] = PropTag.ProcessInfo.MaximumDatabaseSchemaVersion.PropInfo;
			dictionary[1721368579U] = PropTag.ProcessInfo.CurrentDatabaseSchemaVersion.PropInfo;
			dictionary[1721434115U] = PropTag.ProcessInfo.RequestedDatabaseSchemaVersion.PropInfo;
			return dictionary;
		}

		private static WellKnownProperties.ObjectPropertyDefinitions BuildFastTransferStreamPropertyDefinitions()
		{
			Dictionary<string, StorePropInfo> propertiesByTagName = WellKnownProperties.BuildFastTransferStreamPropertyDefinitionsByName();
			Dictionary<uint, StorePropInfo> properties = WellKnownProperties.BuildFastTransferStreamPropertyDefinitionsByTag();
			Dictionary<ushort, StorePropInfo> propIds = new Dictionary<ushort, StorePropInfo>(100);
			WellKnownProperties.PropIdRangeList propIdRanges = WellKnownProperties.BuildPropIdRanges(new WellKnownProperties.TempIdRange[0]);
			return new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.FastTransferStream,
				Properties = properties,
				PropertiesByTagName = propertiesByTagName,
				PropIds = propIds,
				PropIdRanges = propIdRanges
			};
		}

		private static Dictionary<string, StorePropInfo> BuildFastTransferStreamPropertyDefinitionsByName()
		{
			Dictionary<string, StorePropInfo> dictionary = new Dictionary<string, StorePropInfo>(2, StringComparer.OrdinalIgnoreCase);
			dictionary["InstanceGuid"] = PropTag.FastTransferStream.InstanceGuid.PropInfo;
			return dictionary;
		}

		private static Dictionary<uint, StorePropInfo> BuildFastTransferStreamPropertyDefinitionsByTag()
		{
			Dictionary<uint, StorePropInfo> dictionary = new Dictionary<uint, StorePropInfo>(2);
			dictionary[1746731080U] = PropTag.FastTransferStream.InstanceGuid.PropInfo;
			return dictionary;
		}

		private static WellKnownProperties.ObjectPropertyDefinitions BuildIsIntegJobPropertyDefinitions()
		{
			Dictionary<string, StorePropInfo> propertiesByTagName = WellKnownProperties.BuildIsIntegJobPropertyDefinitionsByName();
			Dictionary<uint, StorePropInfo> properties = WellKnownProperties.BuildIsIntegJobPropertyDefinitionsByTag();
			Dictionary<ushort, StorePropInfo> propIds = new Dictionary<ushort, StorePropInfo>(100);
			WellKnownProperties.PropIdRangeList propIdRanges = WellKnownProperties.BuildPropIdRanges(new WellKnownProperties.TempIdRange[0]);
			return new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.IsIntegJob,
				Properties = properties,
				PropertiesByTagName = propertiesByTagName,
				PropIds = propIds,
				PropIdRanges = propIdRanges
			};
		}

		private static Dictionary<string, StorePropInfo> BuildIsIntegJobPropertyDefinitionsByName()
		{
			Dictionary<string, StorePropInfo> dictionary = new Dictionary<string, StorePropInfo>(36, StringComparer.OrdinalIgnoreCase);
			dictionary["IsIntegJobMailboxGuid"] = PropTag.IsIntegJob.IsIntegJobMailboxGuid.PropInfo;
			dictionary["IsIntegJobGuid"] = PropTag.IsIntegJob.IsIntegJobGuid.PropInfo;
			dictionary["IsIntegJobFlags"] = PropTag.IsIntegJob.IsIntegJobFlags.PropInfo;
			dictionary["IsIntegJobTask"] = PropTag.IsIntegJob.IsIntegJobTask.PropInfo;
			dictionary["IsIntegJobState"] = PropTag.IsIntegJob.IsIntegJobState.PropInfo;
			dictionary["IsIntegJobCreationTime"] = PropTag.IsIntegJob.IsIntegJobCreationTime.PropInfo;
			dictionary["IsIntegJobCompletedTime"] = PropTag.IsIntegJob.IsIntegJobCompletedTime.PropInfo;
			dictionary["IsIntegJobLastExecutionTime"] = PropTag.IsIntegJob.IsIntegJobLastExecutionTime.PropInfo;
			dictionary["IsIntegJobCorruptionsDetected"] = PropTag.IsIntegJob.IsIntegJobCorruptionsDetected.PropInfo;
			dictionary["IsIntegJobCorruptionsFixed"] = PropTag.IsIntegJob.IsIntegJobCorruptionsFixed.PropInfo;
			dictionary["IsIntegJobRequestGuid"] = PropTag.IsIntegJob.IsIntegJobRequestGuid.PropInfo;
			dictionary["IsIntegJobProgress"] = PropTag.IsIntegJob.IsIntegJobProgress.PropInfo;
			dictionary["IsIntegJobCorruptions"] = PropTag.IsIntegJob.IsIntegJobCorruptions.PropInfo;
			dictionary["IsIntegJobSource"] = PropTag.IsIntegJob.IsIntegJobSource.PropInfo;
			dictionary["IsIntegJobPriority"] = PropTag.IsIntegJob.IsIntegJobPriority.PropInfo;
			dictionary["IsIntegJobTimeInServer"] = PropTag.IsIntegJob.IsIntegJobTimeInServer.PropInfo;
			dictionary["IsIntegJobMailboxNumber"] = PropTag.IsIntegJob.IsIntegJobMailboxNumber.PropInfo;
			dictionary["IsIntegJobError"] = PropTag.IsIntegJob.IsIntegJobError.PropInfo;
			return dictionary;
		}

		private static Dictionary<uint, StorePropInfo> BuildIsIntegJobPropertyDefinitionsByTag()
		{
			Dictionary<uint, StorePropInfo> dictionary = new Dictionary<uint, StorePropInfo>(36);
			dictionary[268435528U] = PropTag.IsIntegJob.IsIntegJobMailboxGuid.PropInfo;
			dictionary[268501064U] = PropTag.IsIntegJob.IsIntegJobGuid.PropInfo;
			dictionary[268566531U] = PropTag.IsIntegJob.IsIntegJobFlags.PropInfo;
			dictionary[268632067U] = PropTag.IsIntegJob.IsIntegJobTask.PropInfo;
			dictionary[268697602U] = PropTag.IsIntegJob.IsIntegJobState.PropInfo;
			dictionary[268763200U] = PropTag.IsIntegJob.IsIntegJobCreationTime.PropInfo;
			dictionary[268828736U] = PropTag.IsIntegJob.IsIntegJobCompletedTime.PropInfo;
			dictionary[268894272U] = PropTag.IsIntegJob.IsIntegJobLastExecutionTime.PropInfo;
			dictionary[268959747U] = PropTag.IsIntegJob.IsIntegJobCorruptionsDetected.PropInfo;
			dictionary[269025283U] = PropTag.IsIntegJob.IsIntegJobCorruptionsFixed.PropInfo;
			dictionary[269090888U] = PropTag.IsIntegJob.IsIntegJobRequestGuid.PropInfo;
			dictionary[269156354U] = PropTag.IsIntegJob.IsIntegJobProgress.PropInfo;
			dictionary[269222146U] = PropTag.IsIntegJob.IsIntegJobCorruptions.PropInfo;
			dictionary[269287426U] = PropTag.IsIntegJob.IsIntegJobSource.PropInfo;
			dictionary[269352962U] = PropTag.IsIntegJob.IsIntegJobPriority.PropInfo;
			dictionary[269418501U] = PropTag.IsIntegJob.IsIntegJobTimeInServer.PropInfo;
			dictionary[269484035U] = PropTag.IsIntegJob.IsIntegJobMailboxNumber.PropInfo;
			dictionary[269549571U] = PropTag.IsIntegJob.IsIntegJobError.PropInfo;
			return dictionary;
		}

		private static WellKnownProperties.ObjectPropertyDefinitions BuildUserInfoPropertyDefinitions()
		{
			Dictionary<string, StorePropInfo> propertiesByTagName = WellKnownProperties.BuildUserInfoPropertyDefinitionsByName();
			Dictionary<uint, StorePropInfo> properties = WellKnownProperties.BuildUserInfoPropertyDefinitionsByTag();
			Dictionary<ushort, StorePropInfo> dictionary = new Dictionary<ushort, StorePropInfo>(100);
			dictionary.Add(12288, new StorePropInfo(null, 12288, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(12290, new StorePropInfo(null, 12290, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(12291, new StorePropInfo(null, 12291, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			dictionary.Add(12292, new StorePropInfo(null, 12292, PropertyType.Invalid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)));
			WellKnownProperties.PropIdRangeList propIdRanges = WellKnownProperties.BuildPropIdRanges(new WellKnownProperties.TempIdRange[0]);
			return new WellKnownProperties.ObjectPropertyDefinitions
			{
				BaseObjectType = ObjectType.UserInfo,
				Properties = properties,
				PropertiesByTagName = propertiesByTagName,
				PropIds = dictionary,
				PropIdRanges = propIdRanges
			};
		}

		private static Dictionary<string, StorePropInfo> BuildUserInfoPropertyDefinitionsByName()
		{
			Dictionary<string, StorePropInfo> dictionary = new Dictionary<string, StorePropInfo>(364, StringComparer.OrdinalIgnoreCase);
			dictionary["UserInformationGuid"] = PropTag.UserInfo.UserInformationGuid.PropInfo;
			dictionary["UserInformationDisplayName"] = PropTag.UserInfo.UserInformationDisplayName.PropInfo;
			dictionary["UserInformationCreationTime"] = PropTag.UserInfo.UserInformationCreationTime.PropInfo;
			dictionary["UserInformationLastModificationTime"] = PropTag.UserInfo.UserInformationLastModificationTime.PropInfo;
			dictionary["UserInformationChangeNumber"] = PropTag.UserInfo.UserInformationChangeNumber.PropInfo;
			dictionary["UserInformationLastInteractiveLogonTime"] = PropTag.UserInfo.UserInformationLastInteractiveLogonTime.PropInfo;
			dictionary["UserInformationActiveSyncAllowedDeviceIDs"] = PropTag.UserInfo.UserInformationActiveSyncAllowedDeviceIDs.PropInfo;
			dictionary["UserInformationActiveSyncBlockedDeviceIDs"] = PropTag.UserInfo.UserInformationActiveSyncBlockedDeviceIDs.PropInfo;
			dictionary["UserInformationActiveSyncDebugLogging"] = PropTag.UserInfo.UserInformationActiveSyncDebugLogging.PropInfo;
			dictionary["UserInformationActiveSyncEnabled"] = PropTag.UserInfo.UserInformationActiveSyncEnabled.PropInfo;
			dictionary["UserInformationAdminDisplayName"] = PropTag.UserInfo.UserInformationAdminDisplayName.PropInfo;
			dictionary["UserInformationAggregationSubscriptionCredential"] = PropTag.UserInfo.UserInformationAggregationSubscriptionCredential.PropInfo;
			dictionary["UserInformationAllowArchiveAddressSync"] = PropTag.UserInfo.UserInformationAllowArchiveAddressSync.PropInfo;
			dictionary["UserInformationAltitude"] = PropTag.UserInfo.UserInformationAltitude.PropInfo;
			dictionary["UserInformationAntispamBypassEnabled"] = PropTag.UserInfo.UserInformationAntispamBypassEnabled.PropInfo;
			dictionary["UserInformationArchiveDomain"] = PropTag.UserInfo.UserInformationArchiveDomain.PropInfo;
			dictionary["UserInformationArchiveGuid"] = PropTag.UserInfo.UserInformationArchiveGuid.PropInfo;
			dictionary["UserInformationArchiveName"] = PropTag.UserInfo.UserInformationArchiveName.PropInfo;
			dictionary["UserInformationArchiveQuota"] = PropTag.UserInfo.UserInformationArchiveQuota.PropInfo;
			dictionary["UserInformationArchiveRelease"] = PropTag.UserInfo.UserInformationArchiveRelease.PropInfo;
			dictionary["UserInformationArchiveStatus"] = PropTag.UserInfo.UserInformationArchiveStatus.PropInfo;
			dictionary["UserInformationArchiveWarningQuota"] = PropTag.UserInfo.UserInformationArchiveWarningQuota.PropInfo;
			dictionary["UserInformationAssistantName"] = PropTag.UserInfo.UserInformationAssistantName.PropInfo;
			dictionary["UserInformationBirthdate"] = PropTag.UserInfo.UserInformationBirthdate.PropInfo;
			dictionary["UserInformationBypassNestedModerationEnabled"] = PropTag.UserInfo.UserInformationBypassNestedModerationEnabled.PropInfo;
			dictionary["UserInformationC"] = PropTag.UserInfo.UserInformationC.PropInfo;
			dictionary["UserInformationCalendarLoggingQuota"] = PropTag.UserInfo.UserInformationCalendarLoggingQuota.PropInfo;
			dictionary["UserInformationCalendarRepairDisabled"] = PropTag.UserInfo.UserInformationCalendarRepairDisabled.PropInfo;
			dictionary["UserInformationCalendarVersionStoreDisabled"] = PropTag.UserInfo.UserInformationCalendarVersionStoreDisabled.PropInfo;
			dictionary["UserInformationCity"] = PropTag.UserInfo.UserInformationCity.PropInfo;
			dictionary["UserInformationCountry"] = PropTag.UserInfo.UserInformationCountry.PropInfo;
			dictionary["UserInformationCountryCode"] = PropTag.UserInfo.UserInformationCountryCode.PropInfo;
			dictionary["UserInformationCountryOrRegion"] = PropTag.UserInfo.UserInformationCountryOrRegion.PropInfo;
			dictionary["UserInformationDefaultMailTip"] = PropTag.UserInfo.UserInformationDefaultMailTip.PropInfo;
			dictionary["UserInformationDeliverToMailboxAndForward"] = PropTag.UserInfo.UserInformationDeliverToMailboxAndForward.PropInfo;
			dictionary["UserInformationDescription"] = PropTag.UserInfo.UserInformationDescription.PropInfo;
			dictionary["UserInformationDisabledArchiveGuid"] = PropTag.UserInfo.UserInformationDisabledArchiveGuid.PropInfo;
			dictionary["UserInformationDowngradeHighPriorityMessagesEnabled"] = PropTag.UserInfo.UserInformationDowngradeHighPriorityMessagesEnabled.PropInfo;
			dictionary["UserInformationECPEnabled"] = PropTag.UserInfo.UserInformationECPEnabled.PropInfo;
			dictionary["UserInformationEmailAddressPolicyEnabled"] = PropTag.UserInfo.UserInformationEmailAddressPolicyEnabled.PropInfo;
			dictionary["UserInformationEwsAllowEntourage"] = PropTag.UserInfo.UserInformationEwsAllowEntourage.PropInfo;
			dictionary["UserInformationEwsAllowMacOutlook"] = PropTag.UserInfo.UserInformationEwsAllowMacOutlook.PropInfo;
			dictionary["UserInformationEwsAllowOutlook"] = PropTag.UserInfo.UserInformationEwsAllowOutlook.PropInfo;
			dictionary["UserInformationEwsApplicationAccessPolicy"] = PropTag.UserInfo.UserInformationEwsApplicationAccessPolicy.PropInfo;
			dictionary["UserInformationEwsEnabled"] = PropTag.UserInfo.UserInformationEwsEnabled.PropInfo;
			dictionary["UserInformationEwsExceptions"] = PropTag.UserInfo.UserInformationEwsExceptions.PropInfo;
			dictionary["UserInformationEwsWellKnownApplicationAccessPolicies"] = PropTag.UserInfo.UserInformationEwsWellKnownApplicationAccessPolicies.PropInfo;
			dictionary["UserInformationExchangeGuid"] = PropTag.UserInfo.UserInformationExchangeGuid.PropInfo;
			dictionary["UserInformationExternalOofOptions"] = PropTag.UserInfo.UserInformationExternalOofOptions.PropInfo;
			dictionary["UserInformationFirstName"] = PropTag.UserInfo.UserInformationFirstName.PropInfo;
			dictionary["UserInformationForwardingSmtpAddress"] = PropTag.UserInfo.UserInformationForwardingSmtpAddress.PropInfo;
			dictionary["UserInformationGender"] = PropTag.UserInfo.UserInformationGender.PropInfo;
			dictionary["UserInformationGenericForwardingAddress"] = PropTag.UserInfo.UserInformationGenericForwardingAddress.PropInfo;
			dictionary["UserInformationGeoCoordinates"] = PropTag.UserInfo.UserInformationGeoCoordinates.PropInfo;
			dictionary["UserInformationHABSeniorityIndex"] = PropTag.UserInfo.UserInformationHABSeniorityIndex.PropInfo;
			dictionary["UserInformationHasActiveSyncDevicePartnership"] = PropTag.UserInfo.UserInformationHasActiveSyncDevicePartnership.PropInfo;
			dictionary["UserInformationHiddenFromAddressListsEnabled"] = PropTag.UserInfo.UserInformationHiddenFromAddressListsEnabled.PropInfo;
			dictionary["UserInformationHiddenFromAddressListsValue"] = PropTag.UserInfo.UserInformationHiddenFromAddressListsValue.PropInfo;
			dictionary["UserInformationHomePhone"] = PropTag.UserInfo.UserInformationHomePhone.PropInfo;
			dictionary["UserInformationImapEnabled"] = PropTag.UserInfo.UserInformationImapEnabled.PropInfo;
			dictionary["UserInformationImapEnableExactRFC822Size"] = PropTag.UserInfo.UserInformationImapEnableExactRFC822Size.PropInfo;
			dictionary["UserInformationImapForceICalForCalendarRetrievalOption"] = PropTag.UserInfo.UserInformationImapForceICalForCalendarRetrievalOption.PropInfo;
			dictionary["UserInformationImapMessagesRetrievalMimeFormat"] = PropTag.UserInfo.UserInformationImapMessagesRetrievalMimeFormat.PropInfo;
			dictionary["UserInformationImapProtocolLoggingEnabled"] = PropTag.UserInfo.UserInformationImapProtocolLoggingEnabled.PropInfo;
			dictionary["UserInformationImapSuppressReadReceipt"] = PropTag.UserInfo.UserInformationImapSuppressReadReceipt.PropInfo;
			dictionary["UserInformationImapUseProtocolDefaults"] = PropTag.UserInfo.UserInformationImapUseProtocolDefaults.PropInfo;
			dictionary["UserInformationIncludeInGarbageCollection"] = PropTag.UserInfo.UserInformationIncludeInGarbageCollection.PropInfo;
			dictionary["UserInformationInitials"] = PropTag.UserInfo.UserInformationInitials.PropInfo;
			dictionary["UserInformationInPlaceHolds"] = PropTag.UserInfo.UserInformationInPlaceHolds.PropInfo;
			dictionary["UserInformationInternalOnly"] = PropTag.UserInfo.UserInformationInternalOnly.PropInfo;
			dictionary["UserInformationInternalUsageLocation"] = PropTag.UserInfo.UserInformationInternalUsageLocation.PropInfo;
			dictionary["UserInformationInternetEncoding"] = PropTag.UserInfo.UserInformationInternetEncoding.PropInfo;
			dictionary["UserInformationIsCalculatedTargetAddress"] = PropTag.UserInfo.UserInformationIsCalculatedTargetAddress.PropInfo;
			dictionary["UserInformationIsExcludedFromServingHierarchy"] = PropTag.UserInfo.UserInformationIsExcludedFromServingHierarchy.PropInfo;
			dictionary["UserInformationIsHierarchyReady"] = PropTag.UserInfo.UserInformationIsHierarchyReady.PropInfo;
			dictionary["UserInformationIsInactiveMailbox"] = PropTag.UserInfo.UserInformationIsInactiveMailbox.PropInfo;
			dictionary["UserInformationIsSoftDeletedByDisable"] = PropTag.UserInfo.UserInformationIsSoftDeletedByDisable.PropInfo;
			dictionary["UserInformationIsSoftDeletedByRemove"] = PropTag.UserInfo.UserInformationIsSoftDeletedByRemove.PropInfo;
			dictionary["UserInformationIssueWarningQuota"] = PropTag.UserInfo.UserInformationIssueWarningQuota.PropInfo;
			dictionary["UserInformationJournalArchiveAddress"] = PropTag.UserInfo.UserInformationJournalArchiveAddress.PropInfo;
			dictionary["UserInformationLanguages"] = PropTag.UserInfo.UserInformationLanguages.PropInfo;
			dictionary["UserInformationLastExchangeChangedTime"] = PropTag.UserInfo.UserInformationLastExchangeChangedTime.PropInfo;
			dictionary["UserInformationLastName"] = PropTag.UserInfo.UserInformationLastName.PropInfo;
			dictionary["UserInformationLatitude"] = PropTag.UserInfo.UserInformationLatitude.PropInfo;
			dictionary["UserInformationLEOEnabled"] = PropTag.UserInfo.UserInformationLEOEnabled.PropInfo;
			dictionary["UserInformationLocaleID"] = PropTag.UserInfo.UserInformationLocaleID.PropInfo;
			dictionary["UserInformationLongitude"] = PropTag.UserInfo.UserInformationLongitude.PropInfo;
			dictionary["UserInformationMacAttachmentFormat"] = PropTag.UserInfo.UserInformationMacAttachmentFormat.PropInfo;
			dictionary["UserInformationMailboxContainerGuid"] = PropTag.UserInfo.UserInformationMailboxContainerGuid.PropInfo;
			dictionary["UserInformationMailboxMoveBatchName"] = PropTag.UserInfo.UserInformationMailboxMoveBatchName.PropInfo;
			dictionary["UserInformationMailboxMoveRemoteHostName"] = PropTag.UserInfo.UserInformationMailboxMoveRemoteHostName.PropInfo;
			dictionary["UserInformationMailboxMoveStatus"] = PropTag.UserInfo.UserInformationMailboxMoveStatus.PropInfo;
			dictionary["UserInformationMailboxRelease"] = PropTag.UserInfo.UserInformationMailboxRelease.PropInfo;
			dictionary["UserInformationMailTipTranslations"] = PropTag.UserInfo.UserInformationMailTipTranslations.PropInfo;
			dictionary["UserInformationMAPIBlockOutlookNonCachedMode"] = PropTag.UserInfo.UserInformationMAPIBlockOutlookNonCachedMode.PropInfo;
			dictionary["UserInformationMAPIBlockOutlookRpcHttp"] = PropTag.UserInfo.UserInformationMAPIBlockOutlookRpcHttp.PropInfo;
			dictionary["UserInformationMAPIBlockOutlookVersions"] = PropTag.UserInfo.UserInformationMAPIBlockOutlookVersions.PropInfo;
			dictionary["UserInformationMAPIEnabled"] = PropTag.UserInfo.UserInformationMAPIEnabled.PropInfo;
			dictionary["UserInformationMapiRecipient"] = PropTag.UserInfo.UserInformationMapiRecipient.PropInfo;
			dictionary["UserInformationMaxBlockedSenders"] = PropTag.UserInfo.UserInformationMaxBlockedSenders.PropInfo;
			dictionary["UserInformationMaxReceiveSize"] = PropTag.UserInfo.UserInformationMaxReceiveSize.PropInfo;
			dictionary["UserInformationMaxSafeSenders"] = PropTag.UserInfo.UserInformationMaxSafeSenders.PropInfo;
			dictionary["UserInformationMaxSendSize"] = PropTag.UserInfo.UserInformationMaxSendSize.PropInfo;
			dictionary["UserInformationMemberName"] = PropTag.UserInfo.UserInformationMemberName.PropInfo;
			dictionary["UserInformationMessageBodyFormat"] = PropTag.UserInfo.UserInformationMessageBodyFormat.PropInfo;
			dictionary["UserInformationMessageFormat"] = PropTag.UserInfo.UserInformationMessageFormat.PropInfo;
			dictionary["UserInformationMessageTrackingReadStatusDisabled"] = PropTag.UserInfo.UserInformationMessageTrackingReadStatusDisabled.PropInfo;
			dictionary["UserInformationMobileFeaturesEnabled"] = PropTag.UserInfo.UserInformationMobileFeaturesEnabled.PropInfo;
			dictionary["UserInformationMobilePhone"] = PropTag.UserInfo.UserInformationMobilePhone.PropInfo;
			dictionary["UserInformationModerationFlags"] = PropTag.UserInfo.UserInformationModerationFlags.PropInfo;
			dictionary["UserInformationNotes"] = PropTag.UserInfo.UserInformationNotes.PropInfo;
			dictionary["UserInformationOccupation"] = PropTag.UserInfo.UserInformationOccupation.PropInfo;
			dictionary["UserInformationOpenDomainRoutingDisabled"] = PropTag.UserInfo.UserInformationOpenDomainRoutingDisabled.PropInfo;
			dictionary["UserInformationOtherHomePhone"] = PropTag.UserInfo.UserInformationOtherHomePhone.PropInfo;
			dictionary["UserInformationOtherMobile"] = PropTag.UserInfo.UserInformationOtherMobile.PropInfo;
			dictionary["UserInformationOtherTelephone"] = PropTag.UserInfo.UserInformationOtherTelephone.PropInfo;
			dictionary["UserInformationOWAEnabled"] = PropTag.UserInfo.UserInformationOWAEnabled.PropInfo;
			dictionary["UserInformationOWAforDevicesEnabled"] = PropTag.UserInfo.UserInformationOWAforDevicesEnabled.PropInfo;
			dictionary["UserInformationPager"] = PropTag.UserInfo.UserInformationPager.PropInfo;
			dictionary["UserInformationPersistedCapabilities"] = PropTag.UserInfo.UserInformationPersistedCapabilities.PropInfo;
			dictionary["UserInformationPhone"] = PropTag.UserInfo.UserInformationPhone.PropInfo;
			dictionary["UserInformationPhoneProviderId"] = PropTag.UserInfo.UserInformationPhoneProviderId.PropInfo;
			dictionary["UserInformationPopEnabled"] = PropTag.UserInfo.UserInformationPopEnabled.PropInfo;
			dictionary["UserInformationPopEnableExactRFC822Size"] = PropTag.UserInfo.UserInformationPopEnableExactRFC822Size.PropInfo;
			dictionary["UserInformationPopForceICalForCalendarRetrievalOption"] = PropTag.UserInfo.UserInformationPopForceICalForCalendarRetrievalOption.PropInfo;
			dictionary["UserInformationPopMessagesRetrievalMimeFormat"] = PropTag.UserInfo.UserInformationPopMessagesRetrievalMimeFormat.PropInfo;
			dictionary["UserInformationPopProtocolLoggingEnabled"] = PropTag.UserInfo.UserInformationPopProtocolLoggingEnabled.PropInfo;
			dictionary["UserInformationPopSuppressReadReceipt"] = PropTag.UserInfo.UserInformationPopSuppressReadReceipt.PropInfo;
			dictionary["UserInformationPopUseProtocolDefaults"] = PropTag.UserInfo.UserInformationPopUseProtocolDefaults.PropInfo;
			dictionary["UserInformationPostalCode"] = PropTag.UserInfo.UserInformationPostalCode.PropInfo;
			dictionary["UserInformationPostOfficeBox"] = PropTag.UserInfo.UserInformationPostOfficeBox.PropInfo;
			dictionary["UserInformationPreviousExchangeGuid"] = PropTag.UserInfo.UserInformationPreviousExchangeGuid.PropInfo;
			dictionary["UserInformationPreviousRecipientTypeDetails"] = PropTag.UserInfo.UserInformationPreviousRecipientTypeDetails.PropInfo;
			dictionary["UserInformationProhibitSendQuota"] = PropTag.UserInfo.UserInformationProhibitSendQuota.PropInfo;
			dictionary["UserInformationProhibitSendReceiveQuota"] = PropTag.UserInfo.UserInformationProhibitSendReceiveQuota.PropInfo;
			dictionary["UserInformationQueryBaseDNRestrictionEnabled"] = PropTag.UserInfo.UserInformationQueryBaseDNRestrictionEnabled.PropInfo;
			dictionary["UserInformationRecipientDisplayType"] = PropTag.UserInfo.UserInformationRecipientDisplayType.PropInfo;
			dictionary["UserInformationRecipientLimits"] = PropTag.UserInfo.UserInformationRecipientLimits.PropInfo;
			dictionary["UserInformationRecipientSoftDeletedStatus"] = PropTag.UserInfo.UserInformationRecipientSoftDeletedStatus.PropInfo;
			dictionary["UserInformationRecoverableItemsQuota"] = PropTag.UserInfo.UserInformationRecoverableItemsQuota.PropInfo;
			dictionary["UserInformationRecoverableItemsWarningQuota"] = PropTag.UserInfo.UserInformationRecoverableItemsWarningQuota.PropInfo;
			dictionary["UserInformationRegion"] = PropTag.UserInfo.UserInformationRegion.PropInfo;
			dictionary["UserInformationRemotePowerShellEnabled"] = PropTag.UserInfo.UserInformationRemotePowerShellEnabled.PropInfo;
			dictionary["UserInformationRemoteRecipientType"] = PropTag.UserInfo.UserInformationRemoteRecipientType.PropInfo;
			dictionary["UserInformationRequireAllSendersAreAuthenticated"] = PropTag.UserInfo.UserInformationRequireAllSendersAreAuthenticated.PropInfo;
			dictionary["UserInformationResetPasswordOnNextLogon"] = PropTag.UserInfo.UserInformationResetPasswordOnNextLogon.PropInfo;
			dictionary["UserInformationRetainDeletedItemsFor"] = PropTag.UserInfo.UserInformationRetainDeletedItemsFor.PropInfo;
			dictionary["UserInformationRetainDeletedItemsUntilBackup"] = PropTag.UserInfo.UserInformationRetainDeletedItemsUntilBackup.PropInfo;
			dictionary["UserInformationRulesQuota"] = PropTag.UserInfo.UserInformationRulesQuota.PropInfo;
			dictionary["UserInformationShouldUseDefaultRetentionPolicy"] = PropTag.UserInfo.UserInformationShouldUseDefaultRetentionPolicy.PropInfo;
			dictionary["UserInformationSimpleDisplayName"] = PropTag.UserInfo.UserInformationSimpleDisplayName.PropInfo;
			dictionary["UserInformationSingleItemRecoveryEnabled"] = PropTag.UserInfo.UserInformationSingleItemRecoveryEnabled.PropInfo;
			dictionary["UserInformationStateOrProvince"] = PropTag.UserInfo.UserInformationStateOrProvince.PropInfo;
			dictionary["UserInformationStreetAddress"] = PropTag.UserInfo.UserInformationStreetAddress.PropInfo;
			dictionary["UserInformationSubscriberAccessEnabled"] = PropTag.UserInfo.UserInformationSubscriberAccessEnabled.PropInfo;
			dictionary["UserInformationTextEncodedORAddress"] = PropTag.UserInfo.UserInformationTextEncodedORAddress.PropInfo;
			dictionary["UserInformationTextMessagingState"] = PropTag.UserInfo.UserInformationTextMessagingState.PropInfo;
			dictionary["UserInformationTimezone"] = PropTag.UserInfo.UserInformationTimezone.PropInfo;
			dictionary["UserInformationUCSImListMigrationCompleted"] = PropTag.UserInfo.UserInformationUCSImListMigrationCompleted.PropInfo;
			dictionary["UserInformationUpgradeDetails"] = PropTag.UserInfo.UserInformationUpgradeDetails.PropInfo;
			dictionary["UserInformationUpgradeMessage"] = PropTag.UserInfo.UserInformationUpgradeMessage.PropInfo;
			dictionary["UserInformationUpgradeRequest"] = PropTag.UserInfo.UserInformationUpgradeRequest.PropInfo;
			dictionary["UserInformationUpgradeStage"] = PropTag.UserInfo.UserInformationUpgradeStage.PropInfo;
			dictionary["UserInformationUpgradeStageTimeStamp"] = PropTag.UserInfo.UserInformationUpgradeStageTimeStamp.PropInfo;
			dictionary["UserInformationUpgradeStatus"] = PropTag.UserInfo.UserInformationUpgradeStatus.PropInfo;
			dictionary["UserInformationUsageLocation"] = PropTag.UserInfo.UserInformationUsageLocation.PropInfo;
			dictionary["UserInformationUseMapiRichTextFormat"] = PropTag.UserInfo.UserInformationUseMapiRichTextFormat.PropInfo;
			dictionary["UserInformationUsePreferMessageFormat"] = PropTag.UserInfo.UserInformationUsePreferMessageFormat.PropInfo;
			dictionary["UserInformationUseUCCAuditConfig"] = PropTag.UserInfo.UserInformationUseUCCAuditConfig.PropInfo;
			dictionary["UserInformationWebPage"] = PropTag.UserInfo.UserInformationWebPage.PropInfo;
			dictionary["UserInformationWhenMailboxCreated"] = PropTag.UserInfo.UserInformationWhenMailboxCreated.PropInfo;
			dictionary["UserInformationWhenSoftDeleted"] = PropTag.UserInfo.UserInformationWhenSoftDeleted.PropInfo;
			dictionary["UserInformationBirthdayPrecision"] = PropTag.UserInfo.UserInformationBirthdayPrecision.PropInfo;
			dictionary["UserInformationNameVersion"] = PropTag.UserInfo.UserInformationNameVersion.PropInfo;
			dictionary["UserInformationOptInUser"] = PropTag.UserInfo.UserInformationOptInUser.PropInfo;
			dictionary["UserInformationIsMigratedConsumerMailbox"] = PropTag.UserInfo.UserInformationIsMigratedConsumerMailbox.PropInfo;
			dictionary["UserInformationMigrationDryRun"] = PropTag.UserInfo.UserInformationMigrationDryRun.PropInfo;
			dictionary["UserInformationIsPremiumConsumerMailbox"] = PropTag.UserInfo.UserInformationIsPremiumConsumerMailbox.PropInfo;
			dictionary["UserInformationAlternateSupportEmailAddresses"] = PropTag.UserInfo.UserInformationAlternateSupportEmailAddresses.PropInfo;
			dictionary["UserInformationEmailAddresses"] = PropTag.UserInfo.UserInformationEmailAddresses.PropInfo;
			dictionary["UserInformationMapiHttpEnabled"] = PropTag.UserInfo.UserInformationMapiHttpEnabled.PropInfo;
			dictionary["UserInformationMAPIBlockOutlookExternalConnectivity"] = PropTag.UserInfo.UserInformationMAPIBlockOutlookExternalConnectivity.PropInfo;
			return dictionary;
		}

		private static Dictionary<uint, StorePropInfo> BuildUserInfoPropertyDefinitionsByTag()
		{
			Dictionary<uint, StorePropInfo> dictionary = new Dictionary<uint, StorePropInfo>(364);
			dictionary[805306440U] = PropTag.UserInfo.UserInformationGuid.PropInfo;
			dictionary[805371935U] = PropTag.UserInfo.UserInformationDisplayName.PropInfo;
			dictionary[805437504U] = PropTag.UserInfo.UserInformationCreationTime.PropInfo;
			dictionary[805503040U] = PropTag.UserInfo.UserInformationLastModificationTime.PropInfo;
			dictionary[805568532U] = PropTag.UserInfo.UserInformationChangeNumber.PropInfo;
			dictionary[805634112U] = PropTag.UserInfo.UserInformationLastInteractiveLogonTime.PropInfo;
			dictionary[805703711U] = PropTag.UserInfo.UserInformationActiveSyncAllowedDeviceIDs.PropInfo;
			dictionary[805769247U] = PropTag.UserInfo.UserInformationActiveSyncBlockedDeviceIDs.PropInfo;
			dictionary[805830659U] = PropTag.UserInfo.UserInformationActiveSyncDebugLogging.PropInfo;
			dictionary[805896203U] = PropTag.UserInfo.UserInformationActiveSyncEnabled.PropInfo;
			dictionary[805961759U] = PropTag.UserInfo.UserInformationAdminDisplayName.PropInfo;
			dictionary[806031391U] = PropTag.UserInfo.UserInformationAggregationSubscriptionCredential.PropInfo;
			dictionary[806092811U] = PropTag.UserInfo.UserInformationAllowArchiveAddressSync.PropInfo;
			dictionary[806158339U] = PropTag.UserInfo.UserInformationAltitude.PropInfo;
			dictionary[806223883U] = PropTag.UserInfo.UserInformationAntispamBypassEnabled.PropInfo;
			dictionary[806289439U] = PropTag.UserInfo.UserInformationArchiveDomain.PropInfo;
			dictionary[806355016U] = PropTag.UserInfo.UserInformationArchiveGuid.PropInfo;
			dictionary[806424607U] = PropTag.UserInfo.UserInformationArchiveName.PropInfo;
			dictionary[806486047U] = PropTag.UserInfo.UserInformationArchiveQuota.PropInfo;
			dictionary[806551583U] = PropTag.UserInfo.UserInformationArchiveRelease.PropInfo;
			dictionary[806617091U] = PropTag.UserInfo.UserInformationArchiveStatus.PropInfo;
			dictionary[806682655U] = PropTag.UserInfo.UserInformationArchiveWarningQuota.PropInfo;
			dictionary[806748191U] = PropTag.UserInfo.UserInformationAssistantName.PropInfo;
			dictionary[806813760U] = PropTag.UserInfo.UserInformationBirthdate.PropInfo;
			dictionary[806879243U] = PropTag.UserInfo.UserInformationBypassNestedModerationEnabled.PropInfo;
			dictionary[806944799U] = PropTag.UserInfo.UserInformationC.PropInfo;
			dictionary[807010335U] = PropTag.UserInfo.UserInformationCalendarLoggingQuota.PropInfo;
			dictionary[807075851U] = PropTag.UserInfo.UserInformationCalendarRepairDisabled.PropInfo;
			dictionary[807141387U] = PropTag.UserInfo.UserInformationCalendarVersionStoreDisabled.PropInfo;
			dictionary[807206943U] = PropTag.UserInfo.UserInformationCity.PropInfo;
			dictionary[807272479U] = PropTag.UserInfo.UserInformationCountry.PropInfo;
			dictionary[807337987U] = PropTag.UserInfo.UserInformationCountryCode.PropInfo;
			dictionary[807403551U] = PropTag.UserInfo.UserInformationCountryOrRegion.PropInfo;
			dictionary[807469087U] = PropTag.UserInfo.UserInformationDefaultMailTip.PropInfo;
			dictionary[807534603U] = PropTag.UserInfo.UserInformationDeliverToMailboxAndForward.PropInfo;
			dictionary[807604255U] = PropTag.UserInfo.UserInformationDescription.PropInfo;
			dictionary[807665736U] = PropTag.UserInfo.UserInformationDisabledArchiveGuid.PropInfo;
			dictionary[807731211U] = PropTag.UserInfo.UserInformationDowngradeHighPriorityMessagesEnabled.PropInfo;
			dictionary[807796747U] = PropTag.UserInfo.UserInformationECPEnabled.PropInfo;
			dictionary[807862283U] = PropTag.UserInfo.UserInformationEmailAddressPolicyEnabled.PropInfo;
			dictionary[807927819U] = PropTag.UserInfo.UserInformationEwsAllowEntourage.PropInfo;
			dictionary[807993355U] = PropTag.UserInfo.UserInformationEwsAllowMacOutlook.PropInfo;
			dictionary[808058891U] = PropTag.UserInfo.UserInformationEwsAllowOutlook.PropInfo;
			dictionary[808124419U] = PropTag.UserInfo.UserInformationEwsApplicationAccessPolicy.PropInfo;
			dictionary[808189955U] = PropTag.UserInfo.UserInformationEwsEnabled.PropInfo;
			dictionary[808259615U] = PropTag.UserInfo.UserInformationEwsExceptions.PropInfo;
			dictionary[808325151U] = PropTag.UserInfo.UserInformationEwsWellKnownApplicationAccessPolicies.PropInfo;
			dictionary[808386632U] = PropTag.UserInfo.UserInformationExchangeGuid.PropInfo;
			dictionary[808452099U] = PropTag.UserInfo.UserInformationExternalOofOptions.PropInfo;
			dictionary[808517663U] = PropTag.UserInfo.UserInformationFirstName.PropInfo;
			dictionary[808583199U] = PropTag.UserInfo.UserInformationForwardingSmtpAddress.PropInfo;
			dictionary[808648735U] = PropTag.UserInfo.UserInformationGender.PropInfo;
			dictionary[808714271U] = PropTag.UserInfo.UserInformationGenericForwardingAddress.PropInfo;
			dictionary[808779807U] = PropTag.UserInfo.UserInformationGeoCoordinates.PropInfo;
			dictionary[808845315U] = PropTag.UserInfo.UserInformationHABSeniorityIndex.PropInfo;
			dictionary[808910859U] = PropTag.UserInfo.UserInformationHasActiveSyncDevicePartnership.PropInfo;
			dictionary[808976395U] = PropTag.UserInfo.UserInformationHiddenFromAddressListsEnabled.PropInfo;
			dictionary[809041931U] = PropTag.UserInfo.UserInformationHiddenFromAddressListsValue.PropInfo;
			dictionary[809107487U] = PropTag.UserInfo.UserInformationHomePhone.PropInfo;
			dictionary[809173003U] = PropTag.UserInfo.UserInformationImapEnabled.PropInfo;
			dictionary[809238539U] = PropTag.UserInfo.UserInformationImapEnableExactRFC822Size.PropInfo;
			dictionary[809304075U] = PropTag.UserInfo.UserInformationImapForceICalForCalendarRetrievalOption.PropInfo;
			dictionary[809369603U] = PropTag.UserInfo.UserInformationImapMessagesRetrievalMimeFormat.PropInfo;
			dictionary[809435139U] = PropTag.UserInfo.UserInformationImapProtocolLoggingEnabled.PropInfo;
			dictionary[809500683U] = PropTag.UserInfo.UserInformationImapSuppressReadReceipt.PropInfo;
			dictionary[809566219U] = PropTag.UserInfo.UserInformationImapUseProtocolDefaults.PropInfo;
			dictionary[809631755U] = PropTag.UserInfo.UserInformationIncludeInGarbageCollection.PropInfo;
			dictionary[809697311U] = PropTag.UserInfo.UserInformationInitials.PropInfo;
			dictionary[809766943U] = PropTag.UserInfo.UserInformationInPlaceHolds.PropInfo;
			dictionary[809828363U] = PropTag.UserInfo.UserInformationInternalOnly.PropInfo;
			dictionary[809893919U] = PropTag.UserInfo.UserInformationInternalUsageLocation.PropInfo;
			dictionary[809959427U] = PropTag.UserInfo.UserInformationInternetEncoding.PropInfo;
			dictionary[810024971U] = PropTag.UserInfo.UserInformationIsCalculatedTargetAddress.PropInfo;
			dictionary[810090507U] = PropTag.UserInfo.UserInformationIsExcludedFromServingHierarchy.PropInfo;
			dictionary[810156043U] = PropTag.UserInfo.UserInformationIsHierarchyReady.PropInfo;
			dictionary[810221579U] = PropTag.UserInfo.UserInformationIsInactiveMailbox.PropInfo;
			dictionary[810287115U] = PropTag.UserInfo.UserInformationIsSoftDeletedByDisable.PropInfo;
			dictionary[810352651U] = PropTag.UserInfo.UserInformationIsSoftDeletedByRemove.PropInfo;
			dictionary[810418207U] = PropTag.UserInfo.UserInformationIssueWarningQuota.PropInfo;
			dictionary[810483743U] = PropTag.UserInfo.UserInformationJournalArchiveAddress.PropInfo;
			dictionary[810553375U] = PropTag.UserInfo.UserInformationLanguages.PropInfo;
			dictionary[810614848U] = PropTag.UserInfo.UserInformationLastExchangeChangedTime.PropInfo;
			dictionary[810680351U] = PropTag.UserInfo.UserInformationLastName.PropInfo;
			dictionary[810745859U] = PropTag.UserInfo.UserInformationLatitude.PropInfo;
			dictionary[810811403U] = PropTag.UserInfo.UserInformationLEOEnabled.PropInfo;
			dictionary[810881027U] = PropTag.UserInfo.UserInformationLocaleID.PropInfo;
			dictionary[810942467U] = PropTag.UserInfo.UserInformationLongitude.PropInfo;
			dictionary[811008003U] = PropTag.UserInfo.UserInformationMacAttachmentFormat.PropInfo;
			dictionary[811073608U] = PropTag.UserInfo.UserInformationMailboxContainerGuid.PropInfo;
			dictionary[811139103U] = PropTag.UserInfo.UserInformationMailboxMoveBatchName.PropInfo;
			dictionary[811204639U] = PropTag.UserInfo.UserInformationMailboxMoveRemoteHostName.PropInfo;
			dictionary[811270147U] = PropTag.UserInfo.UserInformationMailboxMoveStatus.PropInfo;
			dictionary[811335711U] = PropTag.UserInfo.UserInformationMailboxRelease.PropInfo;
			dictionary[811405343U] = PropTag.UserInfo.UserInformationMailTipTranslations.PropInfo;
			dictionary[811466763U] = PropTag.UserInfo.UserInformationMAPIBlockOutlookNonCachedMode.PropInfo;
			dictionary[811532299U] = PropTag.UserInfo.UserInformationMAPIBlockOutlookRpcHttp.PropInfo;
			dictionary[811597855U] = PropTag.UserInfo.UserInformationMAPIBlockOutlookVersions.PropInfo;
			dictionary[811663371U] = PropTag.UserInfo.UserInformationMAPIEnabled.PropInfo;
			dictionary[811728907U] = PropTag.UserInfo.UserInformationMapiRecipient.PropInfo;
			dictionary[811794435U] = PropTag.UserInfo.UserInformationMaxBlockedSenders.PropInfo;
			dictionary[811859999U] = PropTag.UserInfo.UserInformationMaxReceiveSize.PropInfo;
			dictionary[811925507U] = PropTag.UserInfo.UserInformationMaxSafeSenders.PropInfo;
			dictionary[811991071U] = PropTag.UserInfo.UserInformationMaxSendSize.PropInfo;
			dictionary[812056607U] = PropTag.UserInfo.UserInformationMemberName.PropInfo;
			dictionary[812122115U] = PropTag.UserInfo.UserInformationMessageBodyFormat.PropInfo;
			dictionary[812187651U] = PropTag.UserInfo.UserInformationMessageFormat.PropInfo;
			dictionary[812253195U] = PropTag.UserInfo.UserInformationMessageTrackingReadStatusDisabled.PropInfo;
			dictionary[812318723U] = PropTag.UserInfo.UserInformationMobileFeaturesEnabled.PropInfo;
			dictionary[812384287U] = PropTag.UserInfo.UserInformationMobilePhone.PropInfo;
			dictionary[812449795U] = PropTag.UserInfo.UserInformationModerationFlags.PropInfo;
			dictionary[812515359U] = PropTag.UserInfo.UserInformationNotes.PropInfo;
			dictionary[812580895U] = PropTag.UserInfo.UserInformationOccupation.PropInfo;
			dictionary[812646411U] = PropTag.UserInfo.UserInformationOpenDomainRoutingDisabled.PropInfo;
			dictionary[812716063U] = PropTag.UserInfo.UserInformationOtherHomePhone.PropInfo;
			dictionary[812781599U] = PropTag.UserInfo.UserInformationOtherMobile.PropInfo;
			dictionary[812847135U] = PropTag.UserInfo.UserInformationOtherTelephone.PropInfo;
			dictionary[812908555U] = PropTag.UserInfo.UserInformationOWAEnabled.PropInfo;
			dictionary[812974091U] = PropTag.UserInfo.UserInformationOWAforDevicesEnabled.PropInfo;
			dictionary[813039647U] = PropTag.UserInfo.UserInformationPager.PropInfo;
			dictionary[813109251U] = PropTag.UserInfo.UserInformationPersistedCapabilities.PropInfo;
			dictionary[813170719U] = PropTag.UserInfo.UserInformationPhone.PropInfo;
			dictionary[813236255U] = PropTag.UserInfo.UserInformationPhoneProviderId.PropInfo;
			dictionary[813301771U] = PropTag.UserInfo.UserInformationPopEnabled.PropInfo;
			dictionary[813367307U] = PropTag.UserInfo.UserInformationPopEnableExactRFC822Size.PropInfo;
			dictionary[813432843U] = PropTag.UserInfo.UserInformationPopForceICalForCalendarRetrievalOption.PropInfo;
			dictionary[813498371U] = PropTag.UserInfo.UserInformationPopMessagesRetrievalMimeFormat.PropInfo;
			dictionary[813563907U] = PropTag.UserInfo.UserInformationPopProtocolLoggingEnabled.PropInfo;
			dictionary[813629451U] = PropTag.UserInfo.UserInformationPopSuppressReadReceipt.PropInfo;
			dictionary[813694987U] = PropTag.UserInfo.UserInformationPopUseProtocolDefaults.PropInfo;
			dictionary[813760543U] = PropTag.UserInfo.UserInformationPostalCode.PropInfo;
			dictionary[813830175U] = PropTag.UserInfo.UserInformationPostOfficeBox.PropInfo;
			dictionary[813891656U] = PropTag.UserInfo.UserInformationPreviousExchangeGuid.PropInfo;
			dictionary[813957123U] = PropTag.UserInfo.UserInformationPreviousRecipientTypeDetails.PropInfo;
			dictionary[814022687U] = PropTag.UserInfo.UserInformationProhibitSendQuota.PropInfo;
			dictionary[814088223U] = PropTag.UserInfo.UserInformationProhibitSendReceiveQuota.PropInfo;
			dictionary[814153739U] = PropTag.UserInfo.UserInformationQueryBaseDNRestrictionEnabled.PropInfo;
			dictionary[814219267U] = PropTag.UserInfo.UserInformationRecipientDisplayType.PropInfo;
			dictionary[814284831U] = PropTag.UserInfo.UserInformationRecipientLimits.PropInfo;
			dictionary[814350339U] = PropTag.UserInfo.UserInformationRecipientSoftDeletedStatus.PropInfo;
			dictionary[814415903U] = PropTag.UserInfo.UserInformationRecoverableItemsQuota.PropInfo;
			dictionary[814481439U] = PropTag.UserInfo.UserInformationRecoverableItemsWarningQuota.PropInfo;
			dictionary[814546975U] = PropTag.UserInfo.UserInformationRegion.PropInfo;
			dictionary[814612491U] = PropTag.UserInfo.UserInformationRemotePowerShellEnabled.PropInfo;
			dictionary[814678019U] = PropTag.UserInfo.UserInformationRemoteRecipientType.PropInfo;
			dictionary[814743563U] = PropTag.UserInfo.UserInformationRequireAllSendersAreAuthenticated.PropInfo;
			dictionary[814809099U] = PropTag.UserInfo.UserInformationResetPasswordOnNextLogon.PropInfo;
			dictionary[814874644U] = PropTag.UserInfo.UserInformationRetainDeletedItemsFor.PropInfo;
			dictionary[814940171U] = PropTag.UserInfo.UserInformationRetainDeletedItemsUntilBackup.PropInfo;
			dictionary[815005727U] = PropTag.UserInfo.UserInformationRulesQuota.PropInfo;
			dictionary[815071243U] = PropTag.UserInfo.UserInformationShouldUseDefaultRetentionPolicy.PropInfo;
			dictionary[815136799U] = PropTag.UserInfo.UserInformationSimpleDisplayName.PropInfo;
			dictionary[815202315U] = PropTag.UserInfo.UserInformationSingleItemRecoveryEnabled.PropInfo;
			dictionary[815267871U] = PropTag.UserInfo.UserInformationStateOrProvince.PropInfo;
			dictionary[815333407U] = PropTag.UserInfo.UserInformationStreetAddress.PropInfo;
			dictionary[815398923U] = PropTag.UserInfo.UserInformationSubscriberAccessEnabled.PropInfo;
			dictionary[815464479U] = PropTag.UserInfo.UserInformationTextEncodedORAddress.PropInfo;
			dictionary[815534111U] = PropTag.UserInfo.UserInformationTextMessagingState.PropInfo;
			dictionary[815595551U] = PropTag.UserInfo.UserInformationTimezone.PropInfo;
			dictionary[815661067U] = PropTag.UserInfo.UserInformationUCSImListMigrationCompleted.PropInfo;
			dictionary[815726623U] = PropTag.UserInfo.UserInformationUpgradeDetails.PropInfo;
			dictionary[815792159U] = PropTag.UserInfo.UserInformationUpgradeMessage.PropInfo;
			dictionary[815857667U] = PropTag.UserInfo.UserInformationUpgradeRequest.PropInfo;
			dictionary[815923203U] = PropTag.UserInfo.UserInformationUpgradeStage.PropInfo;
			dictionary[815988800U] = PropTag.UserInfo.UserInformationUpgradeStageTimeStamp.PropInfo;
			dictionary[816054275U] = PropTag.UserInfo.UserInformationUpgradeStatus.PropInfo;
			dictionary[816119839U] = PropTag.UserInfo.UserInformationUsageLocation.PropInfo;
			dictionary[816185347U] = PropTag.UserInfo.UserInformationUseMapiRichTextFormat.PropInfo;
			dictionary[816250891U] = PropTag.UserInfo.UserInformationUsePreferMessageFormat.PropInfo;
			dictionary[816316427U] = PropTag.UserInfo.UserInformationUseUCCAuditConfig.PropInfo;
			dictionary[816381983U] = PropTag.UserInfo.UserInformationWebPage.PropInfo;
			dictionary[816447552U] = PropTag.UserInfo.UserInformationWhenMailboxCreated.PropInfo;
			dictionary[816513088U] = PropTag.UserInfo.UserInformationWhenSoftDeleted.PropInfo;
			dictionary[816578591U] = PropTag.UserInfo.UserInformationBirthdayPrecision.PropInfo;
			dictionary[816644127U] = PropTag.UserInfo.UserInformationNameVersion.PropInfo;
			dictionary[816709643U] = PropTag.UserInfo.UserInformationOptInUser.PropInfo;
			dictionary[816775179U] = PropTag.UserInfo.UserInformationIsMigratedConsumerMailbox.PropInfo;
			dictionary[816840715U] = PropTag.UserInfo.UserInformationMigrationDryRun.PropInfo;
			dictionary[816906251U] = PropTag.UserInfo.UserInformationIsPremiumConsumerMailbox.PropInfo;
			dictionary[816971807U] = PropTag.UserInfo.UserInformationAlternateSupportEmailAddresses.PropInfo;
			dictionary[817041439U] = PropTag.UserInfo.UserInformationEmailAddresses.PropInfo;
			dictionary[819331083U] = PropTag.UserInfo.UserInformationMapiHttpEnabled.PropInfo;
			dictionary[819396619U] = PropTag.UserInfo.UserInformationMAPIBlockOutlookExternalConnectivity.PropInfo;
			return dictionary;
		}

		private static Dictionary<StorePropName, StoreNamedPropInfo> BuildNamedPropInfos()
		{
			Dictionary<StorePropName, StoreNamedPropInfo> dictionary = new Dictionary<StorePropName, StoreNamedPropInfo>(2914);
			WellKnownProperties.AddUnnamedNamedPropInfos(dictionary);
			WellKnownProperties.AddPublicStringsNamedPropInfos(dictionary);
			WellKnownProperties.AddCalendarNamedPropInfos(dictionary);
			WellKnownProperties.AddAppointmentNamedPropInfos(dictionary);
			WellKnownProperties.AddTaskNamedPropInfos(dictionary);
			WellKnownProperties.AddAddressNamedPropInfos(dictionary);
			WellKnownProperties.AddRecipientNamedPropInfos(dictionary);
			WellKnownProperties.AddDelegationNamedPropInfos(dictionary);
			WellKnownProperties.AddCommonNamedPropInfos(dictionary);
			WellKnownProperties.AddMailNamedPropInfos(dictionary);
			WellKnownProperties.AddLogNamedPropInfos(dictionary);
			WellKnownProperties.AddTrackingNamedPropInfos(dictionary);
			WellKnownProperties.AddMAPIExtraNamedPropInfos(dictionary);
			WellKnownProperties.AddExceptionNamedPropInfos(dictionary);
			WellKnownProperties.AddRenStoreNamedPropInfos(dictionary);
			WellKnownProperties.AddSystemNamedPropInfos(dictionary);
			WellKnownProperties.AddAddrPersonalNamedPropInfos(dictionary);
			WellKnownProperties.AddReportNamedPropInfos(dictionary);
			WellKnownProperties.AddRemoteNamedPropInfos(dictionary);
			WellKnownProperties.AddFATSystemNamedPropInfos(dictionary);
			WellKnownProperties.AddFATCommonNamedPropInfos(dictionary);
			WellKnownProperties.AddFATCustomNamedPropInfos(dictionary);
			WellKnownProperties.AddNewsNamedPropInfos(dictionary);
			WellKnownProperties.AddSharingNamedPropInfos(dictionary);
			WellKnownProperties.AddPostRssNamedPropInfos(dictionary);
			WellKnownProperties.AddNoteNamedPropInfos(dictionary);
			WellKnownProperties.AddOutlineViewNamedPropInfos(dictionary);
			WellKnownProperties.AddMessageLookupNamedPropInfos(dictionary);
			WellKnownProperties.AddGenericViewNamedPropInfos(dictionary);
			WellKnownProperties.AddInternetHeadersNamedPropInfos(dictionary);
			WellKnownProperties.AddExternalSharingNamedPropInfos(dictionary);
			WellKnownProperties.AddMeetingNamedPropInfos(dictionary);
			WellKnownProperties.AddIMAPStoreNamedPropInfos(dictionary);
			WellKnownProperties.AddIMAPMsgNamedPropInfos(dictionary);
			WellKnownProperties.AddIMAPFoldNamedPropInfos(dictionary);
			WellKnownProperties.AddProxyNamedPropInfos(dictionary);
			WellKnownProperties.AddAirSyncNamedPropInfos(dictionary);
			WellKnownProperties.AddUnifiedMessagingNamedPropInfos(dictionary);
			WellKnownProperties.AddElcNamedPropInfos(dictionary);
			WellKnownProperties.AddAttachmentNamedPropInfos(dictionary);
			WellKnownProperties.AddCalendarAssistantNamedPropInfos(dictionary);
			WellKnownProperties.AddInboxFolderLazyStreamNamedPropInfos(dictionary);
			WellKnownProperties.AddMessagingNamedPropInfos(dictionary);
			WellKnownProperties.AddStorageNamedPropInfos(dictionary);
			WellKnownProperties.AddIConverterSessionNamedPropInfos(dictionary);
			WellKnownProperties.AddPkmLinkInformationNamedPropInfos(dictionary);
			WellKnownProperties.AddPkmHTMLInformationNamedPropInfos(dictionary);
			WellKnownProperties.AddPkmMetaInformationNamedPropInfos(dictionary);
			WellKnownProperties.AddPkmScriptInfoNamedPropInfos(dictionary);
			WellKnownProperties.AddPkmCharacterizationNamedPropInfos(dictionary);
			WellKnownProperties.AddPkmDocSummaryInformationNamedPropInfos(dictionary);
			WellKnownProperties.AddPkmGathererNamedPropInfos(dictionary);
			WellKnownProperties.AddPkmIndexServerQueryNamedPropInfos(dictionary);
			WellKnownProperties.AddPkmNetLibraryInfoNamedPropInfos(dictionary);
			WellKnownProperties.AddPkmSummaryInformationNamedPropInfos(dictionary);
			WellKnownProperties.AddLocationNamedPropInfos(dictionary);
			WellKnownProperties.AddSearchNamedPropInfos(dictionary);
			WellKnownProperties.AddConversationsNamedPropInfos(dictionary);
			WellKnownProperties.AddDAVNamedPropInfos(dictionary);
			WellKnownProperties.AddDrmNamedPropInfos(dictionary);
			WellKnownProperties.AddPushNotificationSubscriptionNamedPropInfos(dictionary);
			WellKnownProperties.AddGroupNotificationsNamedPropInfos(dictionary);
			WellKnownProperties.AddAuditLogSearchNamedPropInfos(dictionary);
			WellKnownProperties.AddLawEnforcementDataNamedPropInfos(dictionary);
			WellKnownProperties.AddMRSLegacy1NamedPropInfos(dictionary);
			WellKnownProperties.AddMRSLegacy2NamedPropInfos(dictionary);
			WellKnownProperties.AddMRSLegacy3NamedPropInfos(dictionary);
			WellKnownProperties.AddMRSLegacy4NamedPropInfos(dictionary);
			WellKnownProperties.AddMRSLegacy5NamedPropInfos(dictionary);
			WellKnownProperties.AddMRSLegacy6NamedPropInfos(dictionary);
			WellKnownProperties.AddMRSLegacy7NamedPropInfos(dictionary);
			WellKnownProperties.AddMRSLegacy8NamedPropInfos(dictionary);
			WellKnownProperties.AddMRSLegacy9NamedPropInfos(dictionary);
			WellKnownProperties.AddMRSLegacy10NamedPropInfos(dictionary);
			WellKnownProperties.AddMRSLegacy11NamedPropInfos(dictionary);
			WellKnownProperties.AddMRSLegacy12NamedPropInfos(dictionary);
			WellKnownProperties.AddMRSLegacy13NamedPropInfos(dictionary);
			WellKnownProperties.AddMRSLegacy14NamedPropInfos(dictionary);
			WellKnownProperties.AddMRSLegacy15NamedPropInfos(dictionary);
			WellKnownProperties.AddMRSLegacy16NamedPropInfos(dictionary);
			WellKnownProperties.AddMRSLegacy17NamedPropInfos(dictionary);
			WellKnownProperties.AddMRSLegacy18NamedPropInfos(dictionary);
			WellKnownProperties.AddMigrationServiceNamedPropInfos(dictionary);
			WellKnownProperties.AddRemindersNamedPropInfos(dictionary);
			WellKnownProperties.AddComplianceNamedPropInfos(dictionary);
			WellKnownProperties.AddInferenceNamedPropInfos(dictionary);
			WellKnownProperties.AddPICWNamedPropInfos(dictionary);
			WellKnownProperties.AddWorkingSetNamedPropInfos(dictionary);
			WellKnownProperties.AddConsumerCalendarNamedPropInfos(dictionary);
			return dictionary;
		}

		private static void AddUnnamedNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddPublicStringsNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.PublicStrings.TelURI.PropName, NamedPropInfo.PublicStrings.TelURI);
			propInfos.Add(NamedPropInfo.PublicStrings.PeopleConnectionCreationTime.PropName, NamedPropInfo.PublicStrings.PeopleConnectionCreationTime);
			propInfos.Add(NamedPropInfo.PublicStrings.Categories.PropName, NamedPropInfo.PublicStrings.Categories);
			propInfos.Add(NamedPropInfo.PublicStrings.CategoriesStr.PropName, NamedPropInfo.PublicStrings.CategoriesStr);
			propInfos.Add(NamedPropInfo.PublicStrings.AddLevel1.PropName, NamedPropInfo.PublicStrings.AddLevel1);
			propInfos.Add(NamedPropInfo.PublicStrings.AddLevel2.PropName, NamedPropInfo.PublicStrings.AddLevel2);
			propInfos.Add(NamedPropInfo.PublicStrings.AllowOleAct.PropName, NamedPropInfo.PublicStrings.AllowOleAct);
			propInfos.Add(NamedPropInfo.PublicStrings.AllowOlePack.PropName, NamedPropInfo.PublicStrings.AllowOlePack);
			propInfos.Add(NamedPropInfo.PublicStrings.AllowOneOffs.PropName, NamedPropInfo.PublicStrings.AllowOneOffs);
			propInfos.Add(NamedPropInfo.PublicStrings.AllowUserAttachSetting.PropName, NamedPropInfo.PublicStrings.AllowUserAttachSetting);
			propInfos.Add(NamedPropInfo.PublicStrings.AllUsers.PropName, NamedPropInfo.PublicStrings.AllUsers);
			propInfos.Add(NamedPropInfo.PublicStrings.AppName.PropName, NamedPropInfo.PublicStrings.AppName);
			propInfos.Add(NamedPropInfo.PublicStrings.AttachClosePrompt.PropName, NamedPropInfo.PublicStrings.AttachClosePrompt);
			propInfos.Add(NamedPropInfo.PublicStrings.AttachSendPrompt.PropName, NamedPropInfo.PublicStrings.AttachSendPrompt);
			propInfos.Add(NamedPropInfo.PublicStrings.Author.PropName, NamedPropInfo.PublicStrings.Author);
			propInfos.Add(NamedPropInfo.PublicStrings.ByteCount.PropName, NamedPropInfo.PublicStrings.ByteCount);
			propInfos.Add(NamedPropInfo.PublicStrings.Category.PropName, NamedPropInfo.PublicStrings.Category);
			propInfos.Add(NamedPropInfo.PublicStrings.CharCount.PropName, NamedPropInfo.PublicStrings.CharCount);
			propInfos.Add(NamedPropInfo.PublicStrings.Comments.PropName, NamedPropInfo.PublicStrings.Comments);
			propInfos.Add(NamedPropInfo.PublicStrings.Company.PropName, NamedPropInfo.PublicStrings.Company);
			propInfos.Add(NamedPropInfo.PublicStrings.CreateDtmRo.PropName, NamedPropInfo.PublicStrings.CreateDtmRo);
			propInfos.Add(NamedPropInfo.PublicStrings.DocParts.PropName, NamedPropInfo.PublicStrings.DocParts);
			propInfos.Add(NamedPropInfo.PublicStrings.DoFormLookup.PropName, NamedPropInfo.PublicStrings.DoFormLookup);
			propInfos.Add(NamedPropInfo.PublicStrings.DRMLicense.PropName, NamedPropInfo.PublicStrings.DRMLicense);
			propInfos.Add(NamedPropInfo.PublicStrings.EditTime.PropName, NamedPropInfo.PublicStrings.EditTime);
			propInfos.Add(NamedPropInfo.PublicStrings.FormControlProp.PropName, NamedPropInfo.PublicStrings.FormControlProp);
			propInfos.Add(NamedPropInfo.PublicStrings.FormVersion.PropName, NamedPropInfo.PublicStrings.FormVersion);
			propInfos.Add(NamedPropInfo.PublicStrings.HeadingPair.PropName, NamedPropInfo.PublicStrings.HeadingPair);
			propInfos.Add(NamedPropInfo.PublicStrings.HeadingsPair.PropName, NamedPropInfo.PublicStrings.HeadingsPair);
			propInfos.Add(NamedPropInfo.PublicStrings.HiddenCount.PropName, NamedPropInfo.PublicStrings.HiddenCount);
			propInfos.Add(NamedPropInfo.PublicStrings.OnlineMeetingExternalLink.PropName, NamedPropInfo.PublicStrings.OnlineMeetingExternalLink);
			propInfos.Add(NamedPropInfo.PublicStrings.OnlineMeetingConfLink.PropName, NamedPropInfo.PublicStrings.OnlineMeetingConfLink);
			propInfos.Add(NamedPropInfo.PublicStrings.Age.PropName, NamedPropInfo.PublicStrings.Age);
			propInfos.Add(NamedPropInfo.PublicStrings.AstrologySign.PropName, NamedPropInfo.PublicStrings.AstrologySign);
			propInfos.Add(NamedPropInfo.PublicStrings.BloodType.PropName, NamedPropInfo.PublicStrings.BloodType);
			propInfos.Add(NamedPropInfo.PublicStrings.Custom1.PropName, NamedPropInfo.PublicStrings.Custom1);
			propInfos.Add(NamedPropInfo.PublicStrings.Custom2.PropName, NamedPropInfo.PublicStrings.Custom2);
			propInfos.Add(NamedPropInfo.PublicStrings.Custom3.PropName, NamedPropInfo.PublicStrings.Custom3);
			propInfos.Add(NamedPropInfo.PublicStrings.Custom4.PropName, NamedPropInfo.PublicStrings.Custom4);
			propInfos.Add(NamedPropInfo.PublicStrings.Custom5.PropName, NamedPropInfo.PublicStrings.Custom5);
			propInfos.Add(NamedPropInfo.PublicStrings.Custom6.PropName, NamedPropInfo.PublicStrings.Custom6);
			propInfos.Add(NamedPropInfo.PublicStrings.Custom7.PropName, NamedPropInfo.PublicStrings.Custom7);
			propInfos.Add(NamedPropInfo.PublicStrings.Custom8.PropName, NamedPropInfo.PublicStrings.Custom8);
			propInfos.Add(NamedPropInfo.PublicStrings.CustomDate1.PropName, NamedPropInfo.PublicStrings.CustomDate1);
			propInfos.Add(NamedPropInfo.PublicStrings.CustomDate2.PropName, NamedPropInfo.PublicStrings.CustomDate2);
			propInfos.Add(NamedPropInfo.PublicStrings.CustomPhone1.PropName, NamedPropInfo.PublicStrings.CustomPhone1);
			propInfos.Add(NamedPropInfo.PublicStrings.CustomPhone2.PropName, NamedPropInfo.PublicStrings.CustomPhone2);
			propInfos.Add(NamedPropInfo.PublicStrings.CustomPhone3.PropName, NamedPropInfo.PublicStrings.CustomPhone3);
			propInfos.Add(NamedPropInfo.PublicStrings.CustomPhone4.PropName, NamedPropInfo.PublicStrings.CustomPhone4);
			propInfos.Add(NamedPropInfo.PublicStrings.EMail10OriginalDisplayName.PropName, NamedPropInfo.PublicStrings.EMail10OriginalDisplayName);
			propInfos.Add(NamedPropInfo.PublicStrings.EMail11OriginalDisplayName.PropName, NamedPropInfo.PublicStrings.EMail11OriginalDisplayName);
			propInfos.Add(NamedPropInfo.PublicStrings.EMail12OriginalDisplayName.PropName, NamedPropInfo.PublicStrings.EMail12OriginalDisplayName);
			propInfos.Add(NamedPropInfo.PublicStrings.EMail3OriginalDisplayName.PropName, NamedPropInfo.PublicStrings.EMail3OriginalDisplayName);
			propInfos.Add(NamedPropInfo.PublicStrings.EMail4OriginalDisplayName.PropName, NamedPropInfo.PublicStrings.EMail4OriginalDisplayName);
			propInfos.Add(NamedPropInfo.PublicStrings.EMail5OriginalDisplayName.PropName, NamedPropInfo.PublicStrings.EMail5OriginalDisplayName);
			propInfos.Add(NamedPropInfo.PublicStrings.EMail6OriginalDisplayName.PropName, NamedPropInfo.PublicStrings.EMail6OriginalDisplayName);
			propInfos.Add(NamedPropInfo.PublicStrings.EMail7OriginalDisplayName.PropName, NamedPropInfo.PublicStrings.EMail7OriginalDisplayName);
			propInfos.Add(NamedPropInfo.PublicStrings.EMail8OriginalDisplayName.PropName, NamedPropInfo.PublicStrings.EMail8OriginalDisplayName);
			propInfos.Add(NamedPropInfo.PublicStrings.EMail9OriginalDisplayName.PropName, NamedPropInfo.PublicStrings.EMail9OriginalDisplayName);
			propInfos.Add(NamedPropInfo.PublicStrings.EmailLabel1.PropName, NamedPropInfo.PublicStrings.EmailLabel1);
			propInfos.Add(NamedPropInfo.PublicStrings.EmailLabel10.PropName, NamedPropInfo.PublicStrings.EmailLabel10);
			propInfos.Add(NamedPropInfo.PublicStrings.EmailLabel11.PropName, NamedPropInfo.PublicStrings.EmailLabel11);
			propInfos.Add(NamedPropInfo.PublicStrings.EmailLabel12.PropName, NamedPropInfo.PublicStrings.EmailLabel12);
			propInfos.Add(NamedPropInfo.PublicStrings.EmailLabel13.PropName, NamedPropInfo.PublicStrings.EmailLabel13);
			propInfos.Add(NamedPropInfo.PublicStrings.EmailLabel2.PropName, NamedPropInfo.PublicStrings.EmailLabel2);
			propInfos.Add(NamedPropInfo.PublicStrings.EmailLabel3.PropName, NamedPropInfo.PublicStrings.EmailLabel3);
			propInfos.Add(NamedPropInfo.PublicStrings.EmailLabel4.PropName, NamedPropInfo.PublicStrings.EmailLabel4);
			propInfos.Add(NamedPropInfo.PublicStrings.EmailLabel5.PropName, NamedPropInfo.PublicStrings.EmailLabel5);
			propInfos.Add(NamedPropInfo.PublicStrings.EmailLabel6.PropName, NamedPropInfo.PublicStrings.EmailLabel6);
			propInfos.Add(NamedPropInfo.PublicStrings.EmailLabel7.PropName, NamedPropInfo.PublicStrings.EmailLabel7);
			propInfos.Add(NamedPropInfo.PublicStrings.EmailLabel8.PropName, NamedPropInfo.PublicStrings.EmailLabel8);
			propInfos.Add(NamedPropInfo.PublicStrings.EmailLabel9.PropName, NamedPropInfo.PublicStrings.EmailLabel9);
			propInfos.Add(NamedPropInfo.PublicStrings.Flagged.PropName, NamedPropInfo.PublicStrings.Flagged);
			propInfos.Add(NamedPropInfo.PublicStrings.JapaneseContact.PropName, NamedPropInfo.PublicStrings.JapaneseContact);
			propInfos.Add(NamedPropInfo.PublicStrings.JapaneseFormat.PropName, NamedPropInfo.PublicStrings.JapaneseFormat);
			propInfos.Add(NamedPropInfo.PublicStrings.SpouseFurigana.PropName, NamedPropInfo.PublicStrings.SpouseFurigana);
			propInfos.Add(NamedPropInfo.PublicStrings.ABLuMultiLine.PropName, NamedPropInfo.PublicStrings.ABLuMultiLine);
			propInfos.Add(NamedPropInfo.PublicStrings.ABLuPreview.PropName, NamedPropInfo.PublicStrings.ABLuPreview);
			propInfos.Add(NamedPropInfo.PublicStrings.ABPKmultiLine.PropName, NamedPropInfo.PublicStrings.ABPKmultiLine);
			propInfos.Add(NamedPropInfo.PublicStrings.ANRContactsFirst.PropName, NamedPropInfo.PublicStrings.ANRContactsFirst);
			propInfos.Add(NamedPropInfo.PublicStrings.Archive.PropName, NamedPropInfo.PublicStrings.Archive);
			propInfos.Add(NamedPropInfo.PublicStrings.AutoAddSignature.PropName, NamedPropInfo.PublicStrings.AutoAddSignature);
			propInfos.Add(NamedPropInfo.PublicStrings.BlockExternalContent.PropName, NamedPropInfo.PublicStrings.BlockExternalContent);
			propInfos.Add(NamedPropInfo.PublicStrings.CalViewType.PropName, NamedPropInfo.PublicStrings.CalViewType);
			propInfos.Add(NamedPropInfo.PublicStrings.ComposeFontColor.PropName, NamedPropInfo.PublicStrings.ComposeFontColor);
			propInfos.Add(NamedPropInfo.PublicStrings.ComposeFontFlags.PropName, NamedPropInfo.PublicStrings.ComposeFontFlags);
			propInfos.Add(NamedPropInfo.PublicStrings.ComposeFontName.PropName, NamedPropInfo.PublicStrings.ComposeFontName);
			propInfos.Add(NamedPropInfo.PublicStrings.ComposeFontSize.PropName, NamedPropInfo.PublicStrings.ComposeFontSize);
			propInfos.Add(NamedPropInfo.PublicStrings.DailyViewDays.PropName, NamedPropInfo.PublicStrings.DailyViewDays);
			propInfos.Add(NamedPropInfo.PublicStrings.DeliverAndRedirect.PropName, NamedPropInfo.PublicStrings.DeliverAndRedirect);
			propInfos.Add(NamedPropInfo.PublicStrings.EnableReminders.PropName, NamedPropInfo.PublicStrings.EnableReminders);
			propInfos.Add(NamedPropInfo.PublicStrings.FBOldBusyStatus.PropName, NamedPropInfo.PublicStrings.FBOldBusyStatus);
			propInfos.Add(NamedPropInfo.PublicStrings.FBOldEnd.PropName, NamedPropInfo.PublicStrings.FBOldEnd);
			propInfos.Add(NamedPropInfo.PublicStrings.FBoldStart.PropName, NamedPropInfo.PublicStrings.FBoldStart);
			propInfos.Add(NamedPropInfo.PublicStrings.FBQueryEnd.PropName, NamedPropInfo.PublicStrings.FBQueryEnd);
			propInfos.Add(NamedPropInfo.PublicStrings.FBQueryInterval.PropName, NamedPropInfo.PublicStrings.FBQueryInterval);
			propInfos.Add(NamedPropInfo.PublicStrings.FBQueryStart.PropName, NamedPropInfo.PublicStrings.FBQueryStart);
			propInfos.Add(NamedPropInfo.PublicStrings.FBRecursDirty.PropName, NamedPropInfo.PublicStrings.FBRecursDirty);
			propInfos.Add(NamedPropInfo.PublicStrings.FirstWeekOfYear.PropName, NamedPropInfo.PublicStrings.FirstWeekOfYear);
			propInfos.Add(NamedPropInfo.PublicStrings.ForwardingAddress.PropName, NamedPropInfo.PublicStrings.ForwardingAddress);
			propInfos.Add(NamedPropInfo.PublicStrings.JunkMailMoveStamp.PropName, NamedPropInfo.PublicStrings.JunkMailMoveStamp);
			propInfos.Add(NamedPropInfo.PublicStrings.LongDateFormat.PropName, NamedPropInfo.PublicStrings.LongDateFormat);
			propInfos.Add(NamedPropInfo.PublicStrings.MSExchEmbAcceptedDevices.PropName, NamedPropInfo.PublicStrings.MSExchEmbAcceptedDevices);
			propInfos.Add(NamedPropInfo.PublicStrings.MSExchEmbCultureInfo.PropName, NamedPropInfo.PublicStrings.MSExchEmbCultureInfo);
			propInfos.Add(NamedPropInfo.PublicStrings.MSExchEmbDateFormat.PropName, NamedPropInfo.PublicStrings.MSExchEmbDateFormat);
			propInfos.Add(NamedPropInfo.PublicStrings.MSExchEmbDefaultMailFolder.PropName, NamedPropInfo.PublicStrings.MSExchEmbDefaultMailFolder);
			propInfos.Add(NamedPropInfo.PublicStrings.MSExchEmbDefaultMailFolderType.PropName, NamedPropInfo.PublicStrings.MSExchEmbDefaultMailFolderType);
			propInfos.Add(NamedPropInfo.PublicStrings.MSExchEmbMarkRead.PropName, NamedPropInfo.PublicStrings.MSExchEmbMarkRead);
			propInfos.Add(NamedPropInfo.PublicStrings.MSExchEmbTimeFormat.PropName, NamedPropInfo.PublicStrings.MSExchEmbTimeFormat);
			propInfos.Add(NamedPropInfo.PublicStrings.MSExchEmbTimeZone.PropName, NamedPropInfo.PublicStrings.MSExchEmbTimeZone);
			propInfos.Add(NamedPropInfo.PublicStrings.NewMailNotify.PropName, NamedPropInfo.PublicStrings.NewMailNotify);
			propInfos.Add(NamedPropInfo.PublicStrings.NextSel.PropName, NamedPropInfo.PublicStrings.NextSel);
			propInfos.Add(NamedPropInfo.PublicStrings.OtherProxyAddress.PropName, NamedPropInfo.PublicStrings.OtherProxyAddress);
			propInfos.Add(NamedPropInfo.PublicStrings.Preview.PropName, NamedPropInfo.PublicStrings.Preview);
			propInfos.Add(NamedPropInfo.PublicStrings.PreviewMarkAsRead.PropName, NamedPropInfo.PublicStrings.PreviewMarkAsRead);
			propInfos.Add(NamedPropInfo.PublicStrings.PreviewMultiDay.PropName, NamedPropInfo.PublicStrings.PreviewMultiDay);
			propInfos.Add(NamedPropInfo.PublicStrings.PrevReadDelayTime.PropName, NamedPropInfo.PublicStrings.PrevReadDelayTime);
			propInfos.Add(NamedPropInfo.PublicStrings.QuickLinks.PropName, NamedPropInfo.PublicStrings.QuickLinks);
			propInfos.Add(NamedPropInfo.PublicStrings.ReadReceipt.PropName, NamedPropInfo.PublicStrings.ReadReceipt);
			propInfos.Add(NamedPropInfo.PublicStrings.ReminderInterval.PropName, NamedPropInfo.PublicStrings.ReminderInterval);
			propInfos.Add(NamedPropInfo.PublicStrings.RunAt.PropName, NamedPropInfo.PublicStrings.RunAt);
			propInfos.Add(NamedPropInfo.PublicStrings.SchemaVersion.PropName, NamedPropInfo.PublicStrings.SchemaVersion);
			propInfos.Add(NamedPropInfo.PublicStrings.ShortDateFormat.PropName, NamedPropInfo.PublicStrings.ShortDateFormat);
			propInfos.Add(NamedPropInfo.PublicStrings.ShowRulePont.PropName, NamedPropInfo.PublicStrings.ShowRulePont);
			propInfos.Add(NamedPropInfo.PublicStrings.SignatureHTML.PropName, NamedPropInfo.PublicStrings.SignatureHTML);
			propInfos.Add(NamedPropInfo.PublicStrings.SignatureText.PropName, NamedPropInfo.PublicStrings.SignatureText);
			propInfos.Add(NamedPropInfo.PublicStrings.SMIMEEncrypt.PropName, NamedPropInfo.PublicStrings.SMIMEEncrypt);
			propInfos.Add(NamedPropInfo.PublicStrings.SMIMESign.PropName, NamedPropInfo.PublicStrings.SMIMESign);
			propInfos.Add(NamedPropInfo.PublicStrings.SpellingCheckBeforeSend.PropName, NamedPropInfo.PublicStrings.SpellingCheckBeforeSend);
			propInfos.Add(NamedPropInfo.PublicStrings.SpellingDictionaryLanguage.PropName, NamedPropInfo.PublicStrings.SpellingDictionaryLanguage);
			propInfos.Add(NamedPropInfo.PublicStrings.SpellingIgnoreMixedDigits.PropName, NamedPropInfo.PublicStrings.SpellingIgnoreMixedDigits);
			propInfos.Add(NamedPropInfo.PublicStrings.SpellingIgnoreUpperCase.PropName, NamedPropInfo.PublicStrings.SpellingIgnoreUpperCase);
			propInfos.Add(NamedPropInfo.PublicStrings.ThemeId.PropName, NamedPropInfo.PublicStrings.ThemeId);
			propInfos.Add(NamedPropInfo.PublicStrings.TimeFormat.PropName, NamedPropInfo.PublicStrings.TimeFormat);
			propInfos.Add(NamedPropInfo.PublicStrings.TimeZone.PropName, NamedPropInfo.PublicStrings.TimeZone);
			propInfos.Add(NamedPropInfo.PublicStrings.ViewRowCount.PropName, NamedPropInfo.PublicStrings.ViewRowCount);
			propInfos.Add(NamedPropInfo.PublicStrings.VWFlt.PropName, NamedPropInfo.PublicStrings.VWFlt);
			propInfos.Add(NamedPropInfo.PublicStrings.WcMultiLine.PropName, NamedPropInfo.PublicStrings.WcMultiLine);
			propInfos.Add(NamedPropInfo.PublicStrings.WcSortColumn.PropName, NamedPropInfo.PublicStrings.WcSortColumn);
			propInfos.Add(NamedPropInfo.PublicStrings.WcSortOrder.PropName, NamedPropInfo.PublicStrings.WcSortOrder);
			propInfos.Add(NamedPropInfo.PublicStrings.WCViewHeight.PropName, NamedPropInfo.PublicStrings.WCViewHeight);
			propInfos.Add(NamedPropInfo.PublicStrings.WCViewWidth.PropName, NamedPropInfo.PublicStrings.WCViewWidth);
			propInfos.Add(NamedPropInfo.PublicStrings.WebClientLastUsedSortCols.PropName, NamedPropInfo.PublicStrings.WebClientLastUsedSortCols);
			propInfos.Add(NamedPropInfo.PublicStrings.WebClientLastUsedView.PropName, NamedPropInfo.PublicStrings.WebClientLastUsedView);
			propInfos.Add(NamedPropInfo.PublicStrings.WebClientNavBarWidth.PropName, NamedPropInfo.PublicStrings.WebClientNavBarWidth);
			propInfos.Add(NamedPropInfo.PublicStrings.WebClientShowHierarchy.PropName, NamedPropInfo.PublicStrings.WebClientShowHierarchy);
			propInfos.Add(NamedPropInfo.PublicStrings.WeekStartDay.PropName, NamedPropInfo.PublicStrings.WeekStartDay);
			propInfos.Add(NamedPropInfo.PublicStrings.WorkDayEndTime.PropName, NamedPropInfo.PublicStrings.WorkDayEndTime);
			propInfos.Add(NamedPropInfo.PublicStrings.WorkDays.PropName, NamedPropInfo.PublicStrings.WorkDays);
			propInfos.Add(NamedPropInfo.PublicStrings.WordDayStartTime.PropName, NamedPropInfo.PublicStrings.WordDayStartTime);
			propInfos.Add(NamedPropInfo.PublicStrings.PhishingStamp.PropName, NamedPropInfo.PublicStrings.PhishingStamp);
			propInfos.Add(NamedPropInfo.PublicStrings.SpoofingStamp.PropName, NamedPropInfo.PublicStrings.SpoofingStamp);
			propInfos.Add(NamedPropInfo.PublicStrings.Keywords.PropName, NamedPropInfo.PublicStrings.Keywords);
			propInfos.Add(NamedPropInfo.PublicStrings.LastAuthor.PropName, NamedPropInfo.PublicStrings.LastAuthor);
			propInfos.Add(NamedPropInfo.PublicStrings.LastPrinted.PropName, NamedPropInfo.PublicStrings.LastPrinted);
			propInfos.Add(NamedPropInfo.PublicStrings.LastSaveDtm.PropName, NamedPropInfo.PublicStrings.LastSaveDtm);
			propInfos.Add(NamedPropInfo.PublicStrings.LineCount.PropName, NamedPropInfo.PublicStrings.LineCount);
			propInfos.Add(NamedPropInfo.PublicStrings.LinksDirty.PropName, NamedPropInfo.PublicStrings.LinksDirty);
			propInfos.Add(NamedPropInfo.PublicStrings.Manager.PropName, NamedPropInfo.PublicStrings.Manager);
			propInfos.Add(NamedPropInfo.PublicStrings.MMClipCount.PropName, NamedPropInfo.PublicStrings.MMClipCount);
			propInfos.Add(NamedPropInfo.PublicStrings.NoteCount.PropName, NamedPropInfo.PublicStrings.NoteCount);
			propInfos.Add(NamedPropInfo.PublicStrings.NumElements.PropName, NamedPropInfo.PublicStrings.NumElements);
			propInfos.Add(NamedPropInfo.PublicStrings.PageCount.PropName, NamedPropInfo.PublicStrings.PageCount);
			propInfos.Add(NamedPropInfo.PublicStrings.ParCount.PropName, NamedPropInfo.PublicStrings.ParCount);
			propInfos.Add(NamedPropInfo.PublicStrings.PresFormat.PropName, NamedPropInfo.PublicStrings.PresFormat);
			propInfos.Add(NamedPropInfo.PublicStrings.ProgAddrBookCDO.PropName, NamedPropInfo.PublicStrings.ProgAddrBookCDO);
			propInfos.Add(NamedPropInfo.PublicStrings.ProgAddrBookOOM.PropName, NamedPropInfo.PublicStrings.ProgAddrBookOOM);
			propInfos.Add(NamedPropInfo.PublicStrings.ProgAddrBookSMAPI.PropName, NamedPropInfo.PublicStrings.ProgAddrBookSMAPI);
			propInfos.Add(NamedPropInfo.PublicStrings.ProgAddrInfoCDO.PropName, NamedPropInfo.PublicStrings.ProgAddrInfoCDO);
			propInfos.Add(NamedPropInfo.PublicStrings.ProgAddrInfoOOM.PropName, NamedPropInfo.PublicStrings.ProgAddrInfoOOM);
			propInfos.Add(NamedPropInfo.PublicStrings.ProgAddrInfoSMAPI.PropName, NamedPropInfo.PublicStrings.ProgAddrInfoSMAPI);
			propInfos.Add(NamedPropInfo.PublicStrings.ProgCustVerb.PropName, NamedPropInfo.PublicStrings.ProgCustVerb);
			propInfos.Add(NamedPropInfo.PublicStrings.ProgRespond.PropName, NamedPropInfo.PublicStrings.ProgRespond);
			propInfos.Add(NamedPropInfo.PublicStrings.ProgSaveAs.PropName, NamedPropInfo.PublicStrings.ProgSaveAs);
			propInfos.Add(NamedPropInfo.PublicStrings.ProgSearch.PropName, NamedPropInfo.PublicStrings.ProgSearch);
			propInfos.Add(NamedPropInfo.PublicStrings.ProgSendCDO.PropName, NamedPropInfo.PublicStrings.ProgSendCDO);
			propInfos.Add(NamedPropInfo.PublicStrings.ProgSendOOM.PropName, NamedPropInfo.PublicStrings.ProgSendOOM);
			propInfos.Add(NamedPropInfo.PublicStrings.ProgSendSMAPI.PropName, NamedPropInfo.PublicStrings.ProgSendSMAPI);
			propInfos.Add(NamedPropInfo.PublicStrings.QuarantineOriginalSender.PropName, NamedPropInfo.PublicStrings.QuarantineOriginalSender);
			propInfos.Add(NamedPropInfo.PublicStrings.RemoveLevel1.PropName, NamedPropInfo.PublicStrings.RemoveLevel1);
			propInfos.Add(NamedPropInfo.PublicStrings.RemoveLevel2.PropName, NamedPropInfo.PublicStrings.RemoveLevel2);
			propInfos.Add(NamedPropInfo.PublicStrings.RevNumber.PropName, NamedPropInfo.PublicStrings.RevNumber);
			propInfos.Add(NamedPropInfo.PublicStrings.Scale.PropName, NamedPropInfo.PublicStrings.Scale);
			propInfos.Add(NamedPropInfo.PublicStrings.Security.PropName, NamedPropInfo.PublicStrings.Security);
			propInfos.Add(NamedPropInfo.PublicStrings.ShowAllAttach.PropName, NamedPropInfo.PublicStrings.ShowAllAttach);
			propInfos.Add(NamedPropInfo.PublicStrings.SlideCount.PropName, NamedPropInfo.PublicStrings.SlideCount);
			propInfos.Add(NamedPropInfo.PublicStrings.STSAttachIdTable.PropName, NamedPropInfo.PublicStrings.STSAttachIdTable);
			propInfos.Add(NamedPropInfo.PublicStrings.STSBaseUrl.PropName, NamedPropInfo.PublicStrings.STSBaseUrl);
			propInfos.Add(NamedPropInfo.PublicStrings.STSEventType.PropName, NamedPropInfo.PublicStrings.STSEventType);
			propInfos.Add(NamedPropInfo.PublicStrings.STSEventUid.PropName, NamedPropInfo.PublicStrings.STSEventUid);
			propInfos.Add(NamedPropInfo.PublicStrings.STSFooterInfo.PropName, NamedPropInfo.PublicStrings.STSFooterInfo);
			propInfos.Add(NamedPropInfo.PublicStrings.STSHeaderInfo.PropName, NamedPropInfo.PublicStrings.STSHeaderInfo);
			propInfos.Add(NamedPropInfo.PublicStrings.STSHiddenVer.PropName, NamedPropInfo.PublicStrings.STSHiddenVer);
			propInfos.Add(NamedPropInfo.PublicStrings.STSId.PropName, NamedPropInfo.PublicStrings.STSId);
			propInfos.Add(NamedPropInfo.PublicStrings.STSIDTable.PropName, NamedPropInfo.PublicStrings.STSIDTable);
			propInfos.Add(NamedPropInfo.PublicStrings.STSLastSync.PropName, NamedPropInfo.PublicStrings.STSLastSync);
			propInfos.Add(NamedPropInfo.PublicStrings.STSListFriendlyName.PropName, NamedPropInfo.PublicStrings.STSListFriendlyName);
			propInfos.Add(NamedPropInfo.PublicStrings.STSListGuid.PropName, NamedPropInfo.PublicStrings.STSListGuid);
			propInfos.Add(NamedPropInfo.PublicStrings.STSListUrl.PropName, NamedPropInfo.PublicStrings.STSListUrl);
			propInfos.Add(NamedPropInfo.PublicStrings.STSRecurrenceId.PropName, NamedPropInfo.PublicStrings.STSRecurrenceId);
			propInfos.Add(NamedPropInfo.PublicStrings.STSRecurXml.PropName, NamedPropInfo.PublicStrings.STSRecurXml);
			propInfos.Add(NamedPropInfo.PublicStrings.STSSharePointFolder.PropName, NamedPropInfo.PublicStrings.STSSharePointFolder);
			propInfos.Add(NamedPropInfo.PublicStrings.STSSiteName.PropName, NamedPropInfo.PublicStrings.STSSiteName);
			propInfos.Add(NamedPropInfo.PublicStrings.STSTimeStamp.PropName, NamedPropInfo.PublicStrings.STSTimeStamp);
			propInfos.Add(NamedPropInfo.PublicStrings.STSTimeStamp2.PropName, NamedPropInfo.PublicStrings.STSTimeStamp2);
			propInfos.Add(NamedPropInfo.PublicStrings.STSUserId.PropName, NamedPropInfo.PublicStrings.STSUserId);
			propInfos.Add(NamedPropInfo.PublicStrings.Subject.PropName, NamedPropInfo.PublicStrings.Subject);
			propInfos.Add(NamedPropInfo.PublicStrings.Template.PropName, NamedPropInfo.PublicStrings.Template);
			propInfos.Add(NamedPropInfo.PublicStrings.Thumbnail.PropName, NamedPropInfo.PublicStrings.Thumbnail);
			propInfos.Add(NamedPropInfo.PublicStrings.ThumbNail.PropName, NamedPropInfo.PublicStrings.ThumbNail);
			propInfos.Add(NamedPropInfo.PublicStrings.Title.PropName, NamedPropInfo.PublicStrings.Title);
			propInfos.Add(NamedPropInfo.PublicStrings.TrustedCode.PropName, NamedPropInfo.PublicStrings.TrustedCode);
			propInfos.Add(NamedPropInfo.PublicStrings.AttendeeRole.PropName, NamedPropInfo.PublicStrings.AttendeeRole);
			propInfos.Add(NamedPropInfo.PublicStrings.Contact.PropName, NamedPropInfo.PublicStrings.Contact);
			propInfos.Add(NamedPropInfo.PublicStrings.ContactURL.PropName, NamedPropInfo.PublicStrings.ContactURL);
			propInfos.Add(NamedPropInfo.PublicStrings.DescriptionURL.PropName, NamedPropInfo.PublicStrings.DescriptionURL);
			propInfos.Add(NamedPropInfo.PublicStrings.ExRule.PropName, NamedPropInfo.PublicStrings.ExRule);
			propInfos.Add(NamedPropInfo.PublicStrings.GeoLatitude.PropName, NamedPropInfo.PublicStrings.GeoLatitude);
			propInfos.Add(NamedPropInfo.PublicStrings.GeoLongitude.PropName, NamedPropInfo.PublicStrings.GeoLongitude);
			propInfos.Add(NamedPropInfo.PublicStrings.IsOrganizer.PropName, NamedPropInfo.PublicStrings.IsOrganizer);
			propInfos.Add(NamedPropInfo.PublicStrings.LocationUrl.PropName, NamedPropInfo.PublicStrings.LocationUrl);
			propInfos.Add(NamedPropInfo.PublicStrings.ProdId.PropName, NamedPropInfo.PublicStrings.ProdId);
			propInfos.Add(NamedPropInfo.PublicStrings.RDate.PropName, NamedPropInfo.PublicStrings.RDate);
			propInfos.Add(NamedPropInfo.PublicStrings.RecurrenceIdRange.PropName, NamedPropInfo.PublicStrings.RecurrenceIdRange);
			propInfos.Add(NamedPropInfo.PublicStrings.Resources.PropName, NamedPropInfo.PublicStrings.Resources);
			propInfos.Add(NamedPropInfo.PublicStrings.RSVP.PropName, NamedPropInfo.PublicStrings.RSVP);
			propInfos.Add(NamedPropInfo.PublicStrings.TimezoneId.PropName, NamedPropInfo.PublicStrings.TimezoneId);
			propInfos.Add(NamedPropInfo.PublicStrings.Transparent.PropName, NamedPropInfo.PublicStrings.Transparent);
			propInfos.Add(NamedPropInfo.PublicStrings.AlternateRecipient.PropName, NamedPropInfo.PublicStrings.AlternateRecipient);
			propInfos.Add(NamedPropInfo.PublicStrings.ContactC.PropName, NamedPropInfo.PublicStrings.ContactC);
			propInfos.Add(NamedPropInfo.PublicStrings.HomeLatitude.PropName, NamedPropInfo.PublicStrings.HomeLatitude);
			propInfos.Add(NamedPropInfo.PublicStrings.HomeLongitude.PropName, NamedPropInfo.PublicStrings.HomeLongitude);
			propInfos.Add(NamedPropInfo.PublicStrings.HomeTimeZone.PropName, NamedPropInfo.PublicStrings.HomeTimeZone);
			propInfos.Add(NamedPropInfo.PublicStrings.OtherCountryCode.PropName, NamedPropInfo.PublicStrings.OtherCountryCode);
			propInfos.Add(NamedPropInfo.PublicStrings.OtherPager.PropName, NamedPropInfo.PublicStrings.OtherPager);
			propInfos.Add(NamedPropInfo.PublicStrings.OtherTimeZone.PropName, NamedPropInfo.PublicStrings.OtherTimeZone);
			propInfos.Add(NamedPropInfo.PublicStrings.ContactsPSRDate.PropName, NamedPropInfo.PublicStrings.ContactsPSRDate);
			propInfos.Add(NamedPropInfo.PublicStrings.RRule.PropName, NamedPropInfo.PublicStrings.RRule);
			propInfos.Add(NamedPropInfo.PublicStrings.SecretaryURL.PropName, NamedPropInfo.PublicStrings.SecretaryURL);
			propInfos.Add(NamedPropInfo.PublicStrings.SourceURL.PropName, NamedPropInfo.PublicStrings.SourceURL);
			propInfos.Add(NamedPropInfo.PublicStrings.FileAs.PropName, NamedPropInfo.PublicStrings.FileAs);
			propInfos.Add(NamedPropInfo.PublicStrings.SaveDestination.PropName, NamedPropInfo.PublicStrings.SaveDestination);
			propInfos.Add(NamedPropInfo.PublicStrings.SaveInSent.PropName, NamedPropInfo.PublicStrings.SaveInSent);
			propInfos.Add(NamedPropInfo.PublicStrings.Special.PropName, NamedPropInfo.PublicStrings.Special);
			propInfos.Add(NamedPropInfo.PublicStrings.Type.PropName, NamedPropInfo.PublicStrings.Type);
			propInfos.Add(NamedPropInfo.PublicStrings.ClosedExpectedContentClasses.PropName, NamedPropInfo.PublicStrings.ClosedExpectedContentClasses);
			propInfos.Add(NamedPropInfo.PublicStrings.CodeBase.PropName, NamedPropInfo.PublicStrings.CodeBase);
			propInfos.Add(NamedPropInfo.PublicStrings.ComClassId.PropName, NamedPropInfo.PublicStrings.ComClassId);
			propInfos.Add(NamedPropInfo.PublicStrings.Comprogid.PropName, NamedPropInfo.PublicStrings.Comprogid);
			propInfos.Add(NamedPropInfo.PublicStrings.Default.PropName, NamedPropInfo.PublicStrings.Default);
			propInfos.Add(NamedPropInfo.PublicStrings.Dictionary.PropName, NamedPropInfo.PublicStrings.Dictionary);
			propInfos.Add(NamedPropInfo.PublicStrings.Form.PropName, NamedPropInfo.PublicStrings.Form);
			propInfos.Add(NamedPropInfo.PublicStrings.IsContentIndexed.PropName, NamedPropInfo.PublicStrings.IsContentIndexed);
			propInfos.Add(NamedPropInfo.PublicStrings.IsIndexed.PropName, NamedPropInfo.PublicStrings.IsIndexed);
			propInfos.Add(NamedPropInfo.PublicStrings.IsMultiValued.PropName, NamedPropInfo.PublicStrings.IsMultiValued);
			propInfos.Add(NamedPropInfo.PublicStrings.IsReadOnly.PropName, NamedPropInfo.PublicStrings.IsReadOnly);
			propInfos.Add(NamedPropInfo.PublicStrings.IsRequired.PropName, NamedPropInfo.PublicStrings.IsRequired);
			propInfos.Add(NamedPropInfo.PublicStrings.IsVisible.PropName, NamedPropInfo.PublicStrings.IsVisible);
			propInfos.Add(NamedPropInfo.PublicStrings.PropertyDef.PropName, NamedPropInfo.PublicStrings.PropertyDef);
			propInfos.Add(NamedPropInfo.PublicStrings.Synchronize.PropName, NamedPropInfo.PublicStrings.Synchronize);
			propInfos.Add(NamedPropInfo.PublicStrings.Version.PropName, NamedPropInfo.PublicStrings.Version);
			propInfos.Add(NamedPropInfo.PublicStrings.DesignerMembership.PropName, NamedPropInfo.PublicStrings.DesignerMembership);
			propInfos.Add(NamedPropInfo.PublicStrings.TemplateDescription.PropName, NamedPropInfo.PublicStrings.TemplateDescription);
			propInfos.Add(NamedPropInfo.PublicStrings.OutlookAllOrNoneMtgUpdatedLG.PropName, NamedPropInfo.PublicStrings.OutlookAllOrNoneMtgUpdatedLG);
			propInfos.Add(NamedPropInfo.PublicStrings.OutlookDoCoercion.PropName, NamedPropInfo.PublicStrings.OutlookDoCoercion);
			propInfos.Add(NamedPropInfo.PublicStrings.OutlookFixupFBFolder.PropName, NamedPropInfo.PublicStrings.OutlookFixupFBFolder);
			propInfos.Add(NamedPropInfo.PublicStrings.OutlookHideFilterTab.PropName, NamedPropInfo.PublicStrings.OutlookHideFilterTab);
			propInfos.Add(NamedPropInfo.PublicStrings.OutlookIsHidden.PropName, NamedPropInfo.PublicStrings.OutlookIsHidden);
			propInfos.Add(NamedPropInfo.PublicStrings.OutlookNoAclUI.PropName, NamedPropInfo.PublicStrings.OutlookNoAclUI);
			propInfos.Add(NamedPropInfo.PublicStrings.OutlookNoDuplicateDocuments.PropName, NamedPropInfo.PublicStrings.OutlookNoDuplicateDocuments);
			propInfos.Add(NamedPropInfo.PublicStrings.OutlookServerFolderSizes.PropName, NamedPropInfo.PublicStrings.OutlookServerFolderSizes);
			propInfos.Add(NamedPropInfo.PublicStrings.OutlookShowForward.PropName, NamedPropInfo.PublicStrings.OutlookShowForward);
			propInfos.Add(NamedPropInfo.PublicStrings.OutlookShowReply.PropName, NamedPropInfo.PublicStrings.OutlookShowReply);
			propInfos.Add(NamedPropInfo.PublicStrings.OutlookStoreTypePrivate.PropName, NamedPropInfo.PublicStrings.OutlookStoreTypePrivate);
			propInfos.Add(NamedPropInfo.PublicStrings.Element.PropName, NamedPropInfo.PublicStrings.Element);
			propInfos.Add(NamedPropInfo.PublicStrings.Extends.PropName, NamedPropInfo.PublicStrings.Extends);
			propInfos.Add(NamedPropInfo.PublicStrings.Name.PropName, NamedPropInfo.PublicStrings.Name);
			propInfos.Add(NamedPropInfo.PublicStrings.UserFormula.PropName, NamedPropInfo.PublicStrings.UserFormula);
			propInfos.Add(NamedPropInfo.PublicStrings.WordCount.PropName, NamedPropInfo.PublicStrings.WordCount);
			propInfos.Add(NamedPropInfo.PublicStrings.DRMServerLicenseCompressed.PropName, NamedPropInfo.PublicStrings.DRMServerLicenseCompressed);
			propInfos.Add(NamedPropInfo.PublicStrings.DRMServerLicense.PropName, NamedPropInfo.PublicStrings.DRMServerLicense);
			propInfos.Add(NamedPropInfo.PublicStrings.MonitoringUniqueId.PropName, NamedPropInfo.PublicStrings.MonitoringUniqueId);
			propInfos.Add(NamedPropInfo.PublicStrings.MonitoringEventSource.PropName, NamedPropInfo.PublicStrings.MonitoringEventSource);
			propInfos.Add(NamedPropInfo.PublicStrings.MonitoringEventInstanceId.PropName, NamedPropInfo.PublicStrings.MonitoringEventInstanceId);
			propInfos.Add(NamedPropInfo.PublicStrings.MonitoringInsertionStrings.PropName, NamedPropInfo.PublicStrings.MonitoringInsertionStrings);
			propInfos.Add(NamedPropInfo.PublicStrings.MonitoringCreationTimeUtc.PropName, NamedPropInfo.PublicStrings.MonitoringCreationTimeUtc);
			propInfos.Add(NamedPropInfo.PublicStrings.MonitoringNotificationEmailSent.PropName, NamedPropInfo.PublicStrings.MonitoringNotificationEmailSent);
			propInfos.Add(NamedPropInfo.PublicStrings.MonitoringEventEntryType.PropName, NamedPropInfo.PublicStrings.MonitoringEventEntryType);
			propInfos.Add(NamedPropInfo.PublicStrings.MonitoringNotificationRecipients.PropName, NamedPropInfo.PublicStrings.MonitoringNotificationRecipients);
			propInfos.Add(NamedPropInfo.PublicStrings.MonitoringEventCategoryId.PropName, NamedPropInfo.PublicStrings.MonitoringEventCategoryId);
			propInfos.Add(NamedPropInfo.PublicStrings.MonitoringEventTimeUtc.PropName, NamedPropInfo.PublicStrings.MonitoringEventTimeUtc);
			propInfos.Add(NamedPropInfo.PublicStrings.MonitoringNotificationMessageIds.PropName, NamedPropInfo.PublicStrings.MonitoringNotificationMessageIds);
			propInfos.Add(NamedPropInfo.PublicStrings.MonitoringEventPeriodicKey.PropName, NamedPropInfo.PublicStrings.MonitoringEventPeriodicKey);
			propInfos.Add(NamedPropInfo.PublicStrings.OutlookContactLinkDateTime.PropName, NamedPropInfo.PublicStrings.OutlookContactLinkDateTime);
			propInfos.Add(NamedPropInfo.PublicStrings.OutlookContactLinkVersion.PropName, NamedPropInfo.PublicStrings.OutlookContactLinkVersion);
		}

		private static void AddCalendarNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddAppointmentNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Appointment.SendMtgAsICAL.PropName, NamedPropInfo.Appointment.SendMtgAsICAL);
			propInfos.Add(NamedPropInfo.Appointment.ApptSequence.PropName, NamedPropInfo.Appointment.ApptSequence);
			propInfos.Add(NamedPropInfo.Appointment.ApptSeqTime.PropName, NamedPropInfo.Appointment.ApptSeqTime);
			propInfos.Add(NamedPropInfo.Appointment.ApptLastSequence.PropName, NamedPropInfo.Appointment.ApptLastSequence);
			propInfos.Add(NamedPropInfo.Appointment.ChangeHighlight.PropName, NamedPropInfo.Appointment.ChangeHighlight);
			propInfos.Add(NamedPropInfo.Appointment.BusyStatus.PropName, NamedPropInfo.Appointment.BusyStatus);
			propInfos.Add(NamedPropInfo.Appointment.FExceptionalBody.PropName, NamedPropInfo.Appointment.FExceptionalBody);
			propInfos.Add(NamedPropInfo.Appointment.ApptAuxFlags.PropName, NamedPropInfo.Appointment.ApptAuxFlags);
			propInfos.Add(NamedPropInfo.Appointment.Location.PropName, NamedPropInfo.Appointment.Location);
			propInfos.Add(NamedPropInfo.Appointment.MWSURL.PropName, NamedPropInfo.Appointment.MWSURL);
			propInfos.Add(NamedPropInfo.Appointment.FwrdInstance.PropName, NamedPropInfo.Appointment.FwrdInstance);
			propInfos.Add(NamedPropInfo.Appointment.LinkedTaskItems.PropName, NamedPropInfo.Appointment.LinkedTaskItems);
			propInfos.Add(NamedPropInfo.Appointment.ApptStartWhole.PropName, NamedPropInfo.Appointment.ApptStartWhole);
			propInfos.Add(NamedPropInfo.Appointment.ApptEndWhole.PropName, NamedPropInfo.Appointment.ApptEndWhole);
			propInfos.Add(NamedPropInfo.Appointment.ApptStartTime.PropName, NamedPropInfo.Appointment.ApptStartTime);
			propInfos.Add(NamedPropInfo.Appointment.ApptEndTime.PropName, NamedPropInfo.Appointment.ApptEndTime);
			propInfos.Add(NamedPropInfo.Appointment.ApptEndDate.PropName, NamedPropInfo.Appointment.ApptEndDate);
			propInfos.Add(NamedPropInfo.Appointment.ApptStartDate.PropName, NamedPropInfo.Appointment.ApptStartDate);
			propInfos.Add(NamedPropInfo.Appointment.ApptDuration.PropName, NamedPropInfo.Appointment.ApptDuration);
			propInfos.Add(NamedPropInfo.Appointment.ApptColor.PropName, NamedPropInfo.Appointment.ApptColor);
			propInfos.Add(NamedPropInfo.Appointment.ApptSubType.PropName, NamedPropInfo.Appointment.ApptSubType);
			propInfos.Add(NamedPropInfo.Appointment.ApptRecur.PropName, NamedPropInfo.Appointment.ApptRecur);
			propInfos.Add(NamedPropInfo.Appointment.ApptStateFlags.PropName, NamedPropInfo.Appointment.ApptStateFlags);
			propInfos.Add(NamedPropInfo.Appointment.ResponseStatus.PropName, NamedPropInfo.Appointment.ResponseStatus);
			propInfos.Add(NamedPropInfo.Appointment.ApptReplyTime.PropName, NamedPropInfo.Appointment.ApptReplyTime);
			propInfos.Add(NamedPropInfo.Appointment.Recurring.PropName, NamedPropInfo.Appointment.Recurring);
			propInfos.Add(NamedPropInfo.Appointment.IntendedBusyStatus.PropName, NamedPropInfo.Appointment.IntendedBusyStatus);
			propInfos.Add(NamedPropInfo.Appointment.DeleteAssocRequest.PropName, NamedPropInfo.Appointment.DeleteAssocRequest);
			propInfos.Add(NamedPropInfo.Appointment.ApptUpdateTime.PropName, NamedPropInfo.Appointment.ApptUpdateTime);
			propInfos.Add(NamedPropInfo.Appointment.DirtyTimesOrStatus.PropName, NamedPropInfo.Appointment.DirtyTimesOrStatus);
			propInfos.Add(NamedPropInfo.Appointment.ExceptionReplaceTime.PropName, NamedPropInfo.Appointment.ExceptionReplaceTime);
			propInfos.Add(NamedPropInfo.Appointment.FInvited.PropName, NamedPropInfo.Appointment.FInvited);
			propInfos.Add(NamedPropInfo.Appointment.OrganizerExceptionReplaceTime.PropName, NamedPropInfo.Appointment.OrganizerExceptionReplaceTime);
			propInfos.Add(NamedPropInfo.Appointment.FExceptionalAttendees.PropName, NamedPropInfo.Appointment.FExceptionalAttendees);
			propInfos.Add(NamedPropInfo.Appointment.FDirtyTimes.PropName, NamedPropInfo.Appointment.FDirtyTimes);
			propInfos.Add(NamedPropInfo.Appointment.FDirtyLocation.PropName, NamedPropInfo.Appointment.FDirtyLocation);
			propInfos.Add(NamedPropInfo.Appointment.OwnerName.PropName, NamedPropInfo.Appointment.OwnerName);
			propInfos.Add(NamedPropInfo.Appointment.FOthersAppt.PropName, NamedPropInfo.Appointment.FOthersAppt);
			propInfos.Add(NamedPropInfo.Appointment.ApptReplyName.PropName, NamedPropInfo.Appointment.ApptReplyName);
			propInfos.Add(NamedPropInfo.Appointment.RecurType.PropName, NamedPropInfo.Appointment.RecurType);
			propInfos.Add(NamedPropInfo.Appointment.RecurPattern.PropName, NamedPropInfo.Appointment.RecurPattern);
			propInfos.Add(NamedPropInfo.Appointment.TimeZoneStruct.PropName, NamedPropInfo.Appointment.TimeZoneStruct);
			propInfos.Add(NamedPropInfo.Appointment.TimeZoneDesc.PropName, NamedPropInfo.Appointment.TimeZoneDesc);
			propInfos.Add(NamedPropInfo.Appointment.ClipStart.PropName, NamedPropInfo.Appointment.ClipStart);
			propInfos.Add(NamedPropInfo.Appointment.ClipEnd.PropName, NamedPropInfo.Appointment.ClipEnd);
			propInfos.Add(NamedPropInfo.Appointment.OrigStoreEid.PropName, NamedPropInfo.Appointment.OrigStoreEid);
			propInfos.Add(NamedPropInfo.Appointment.AllAttendeesString.PropName, NamedPropInfo.Appointment.AllAttendeesString);
			propInfos.Add(NamedPropInfo.Appointment.FMtgDataChanged.PropName, NamedPropInfo.Appointment.FMtgDataChanged);
			propInfos.Add(NamedPropInfo.Appointment.AutoFillLocation.PropName, NamedPropInfo.Appointment.AutoFillLocation);
			propInfos.Add(NamedPropInfo.Appointment.ToAttendeesString.PropName, NamedPropInfo.Appointment.ToAttendeesString);
			propInfos.Add(NamedPropInfo.Appointment.CCAttendeesString.PropName, NamedPropInfo.Appointment.CCAttendeesString);
			propInfos.Add(NamedPropInfo.Appointment.SowFBFlags.PropName, NamedPropInfo.Appointment.SowFBFlags);
			propInfos.Add(NamedPropInfo.Appointment.TrustRecipHighlights.PropName, NamedPropInfo.Appointment.TrustRecipHighlights);
			propInfos.Add(NamedPropInfo.Appointment.ConfCheckChanged.PropName, NamedPropInfo.Appointment.ConfCheckChanged);
			propInfos.Add(NamedPropInfo.Appointment.ConfCheck.PropName, NamedPropInfo.Appointment.ConfCheck);
			propInfos.Add(NamedPropInfo.Appointment.ConfType.PropName, NamedPropInfo.Appointment.ConfType);
			propInfos.Add(NamedPropInfo.Appointment.Directory.PropName, NamedPropInfo.Appointment.Directory);
			propInfos.Add(NamedPropInfo.Appointment.OrgAlias.PropName, NamedPropInfo.Appointment.OrgAlias);
			propInfos.Add(NamedPropInfo.Appointment.AutoStartCheck.PropName, NamedPropInfo.Appointment.AutoStartCheck);
			propInfos.Add(NamedPropInfo.Appointment.AutoStartWhen.PropName, NamedPropInfo.Appointment.AutoStartWhen);
			propInfos.Add(NamedPropInfo.Appointment.AllowExternCheck.PropName, NamedPropInfo.Appointment.AllowExternCheck);
			propInfos.Add(NamedPropInfo.Appointment.CollaborateDoc.PropName, NamedPropInfo.Appointment.CollaborateDoc);
			propInfos.Add(NamedPropInfo.Appointment.NetShowURL.PropName, NamedPropInfo.Appointment.NetShowURL);
			propInfos.Add(NamedPropInfo.Appointment.OnlinePassword.PropName, NamedPropInfo.Appointment.OnlinePassword);
			propInfos.Add(NamedPropInfo.Appointment.ApptProposedStartWhole.PropName, NamedPropInfo.Appointment.ApptProposedStartWhole);
			propInfos.Add(NamedPropInfo.Appointment.ApptProposedEndWhole.PropName, NamedPropInfo.Appointment.ApptProposedEndWhole);
			propInfos.Add(NamedPropInfo.Appointment.ApptProposedStartTime.PropName, NamedPropInfo.Appointment.ApptProposedStartTime);
			propInfos.Add(NamedPropInfo.Appointment.ApptProposedEndTime.PropName, NamedPropInfo.Appointment.ApptProposedEndTime);
			propInfos.Add(NamedPropInfo.Appointment.ApptProposedStartDate.PropName, NamedPropInfo.Appointment.ApptProposedStartDate);
			propInfos.Add(NamedPropInfo.Appointment.ApptProposedEndDate.PropName, NamedPropInfo.Appointment.ApptProposedEndDate);
			propInfos.Add(NamedPropInfo.Appointment.ApptProposedDuration.PropName, NamedPropInfo.Appointment.ApptProposedDuration);
			propInfos.Add(NamedPropInfo.Appointment.ApptCounterProposal.PropName, NamedPropInfo.Appointment.ApptCounterProposal);
			propInfos.Add(NamedPropInfo.Appointment.ApptProposalNum.PropName, NamedPropInfo.Appointment.ApptProposalNum);
			propInfos.Add(NamedPropInfo.Appointment.ApptNotAllowPropose.PropName, NamedPropInfo.Appointment.ApptNotAllowPropose);
			propInfos.Add(NamedPropInfo.Appointment.ApptOpenViewProposal.PropName, NamedPropInfo.Appointment.ApptOpenViewProposal);
			propInfos.Add(NamedPropInfo.Appointment.ApptProposedLocation.PropName, NamedPropInfo.Appointment.ApptProposedLocation);
			propInfos.Add(NamedPropInfo.Appointment.ApptUnsendableRecips.PropName, NamedPropInfo.Appointment.ApptUnsendableRecips);
			propInfos.Add(NamedPropInfo.Appointment.ApptTZDefStartDisplay.PropName, NamedPropInfo.Appointment.ApptTZDefStartDisplay);
			propInfos.Add(NamedPropInfo.Appointment.ApptTZDefEndDisplay.PropName, NamedPropInfo.Appointment.ApptTZDefEndDisplay);
			propInfos.Add(NamedPropInfo.Appointment.ApptTZDefRecur.PropName, NamedPropInfo.Appointment.ApptTZDefRecur);
			propInfos.Add(NamedPropInfo.Appointment.SeriesCreationHash.PropName, NamedPropInfo.Appointment.SeriesCreationHash);
			propInfos.Add(NamedPropInfo.Appointment.SeriesMasterId.PropName, NamedPropInfo.Appointment.SeriesMasterId);
			propInfos.Add(NamedPropInfo.Appointment.InstanceCreationIndex.PropName, NamedPropInfo.Appointment.InstanceCreationIndex);
			propInfos.Add(NamedPropInfo.Appointment.EventClientId.PropName, NamedPropInfo.Appointment.EventClientId);
			propInfos.Add(NamedPropInfo.Appointment.SeriesId.PropName, NamedPropInfo.Appointment.SeriesId);
			propInfos.Add(NamedPropInfo.Appointment.SeriesReminderIsSet.PropName, NamedPropInfo.Appointment.SeriesReminderIsSet);
			propInfos.Add(NamedPropInfo.Appointment.IsHiddenFromLegacyClients.PropName, NamedPropInfo.Appointment.IsHiddenFromLegacyClients);
			propInfos.Add(NamedPropInfo.Appointment.PropertyChangeMetadataRaw.PropName, NamedPropInfo.Appointment.PropertyChangeMetadataRaw);
			propInfos.Add(NamedPropInfo.Appointment.ForwardNotificationRecipients.PropName, NamedPropInfo.Appointment.ForwardNotificationRecipients);
		}

		private static void AddTaskNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Task.TaskStatus.PropName, NamedPropInfo.Task.TaskStatus);
			propInfos.Add(NamedPropInfo.Task.PercentComplete.PropName, NamedPropInfo.Task.PercentComplete);
			propInfos.Add(NamedPropInfo.Task.TeamTask.PropName, NamedPropInfo.Task.TeamTask);
			propInfos.Add(NamedPropInfo.Task.TaskStartDate.PropName, NamedPropInfo.Task.TaskStartDate);
			propInfos.Add(NamedPropInfo.Task.TaskDueDate.PropName, NamedPropInfo.Task.TaskDueDate);
			propInfos.Add(NamedPropInfo.Task.TaskDuration.PropName, NamedPropInfo.Task.TaskDuration);
			propInfos.Add(NamedPropInfo.Task.TaskResetReminder.PropName, NamedPropInfo.Task.TaskResetReminder);
			propInfos.Add(NamedPropInfo.Task.TaskAccepted.PropName, NamedPropInfo.Task.TaskAccepted);
			propInfos.Add(NamedPropInfo.Task.TaskDeadOccur.PropName, NamedPropInfo.Task.TaskDeadOccur);
			propInfos.Add(NamedPropInfo.Task.IntegerTest.PropName, NamedPropInfo.Task.IntegerTest);
			propInfos.Add(NamedPropInfo.Task.FloatTest.PropName, NamedPropInfo.Task.FloatTest);
			propInfos.Add(NamedPropInfo.Task.TaskDateCompleted.PropName, NamedPropInfo.Task.TaskDateCompleted);
			propInfos.Add(NamedPropInfo.Task.TaskActualEffort.PropName, NamedPropInfo.Task.TaskActualEffort);
			propInfos.Add(NamedPropInfo.Task.TaskEstimatedEffort.PropName, NamedPropInfo.Task.TaskEstimatedEffort);
			propInfos.Add(NamedPropInfo.Task.TaskVersion.PropName, NamedPropInfo.Task.TaskVersion);
			propInfos.Add(NamedPropInfo.Task.TaskState.PropName, NamedPropInfo.Task.TaskState);
			propInfos.Add(NamedPropInfo.Task.TaskLastUpdate.PropName, NamedPropInfo.Task.TaskLastUpdate);
			propInfos.Add(NamedPropInfo.Task.TaskRecur.PropName, NamedPropInfo.Task.TaskRecur);
			propInfos.Add(NamedPropInfo.Task.TaskMyDelegators.PropName, NamedPropInfo.Task.TaskMyDelegators);
			propInfos.Add(NamedPropInfo.Task.TaskSOC.PropName, NamedPropInfo.Task.TaskSOC);
			propInfos.Add(NamedPropInfo.Task.TaskHistory.PropName, NamedPropInfo.Task.TaskHistory);
			propInfos.Add(NamedPropInfo.Task.TaskUpdates.PropName, NamedPropInfo.Task.TaskUpdates);
			propInfos.Add(NamedPropInfo.Task.TaskComplete.PropName, NamedPropInfo.Task.TaskComplete);
			propInfos.Add(NamedPropInfo.Task.TaskOriginalRecurring.PropName, NamedPropInfo.Task.TaskOriginalRecurring);
			propInfos.Add(NamedPropInfo.Task.TaskFCreator.PropName, NamedPropInfo.Task.TaskFCreator);
			propInfos.Add(NamedPropInfo.Task.TaskOwner.PropName, NamedPropInfo.Task.TaskOwner);
			propInfos.Add(NamedPropInfo.Task.TaskMultRecips.PropName, NamedPropInfo.Task.TaskMultRecips);
			propInfos.Add(NamedPropInfo.Task.TaskDelegator.PropName, NamedPropInfo.Task.TaskDelegator);
			propInfos.Add(NamedPropInfo.Task.TaskLastUser.PropName, NamedPropInfo.Task.TaskLastUser);
			propInfos.Add(NamedPropInfo.Task.TaskOrdinal.PropName, NamedPropInfo.Task.TaskOrdinal);
			propInfos.Add(NamedPropInfo.Task.TaskNoCompute.PropName, NamedPropInfo.Task.TaskNoCompute);
			propInfos.Add(NamedPropInfo.Task.TaskLastDelegate.PropName, NamedPropInfo.Task.TaskLastDelegate);
			propInfos.Add(NamedPropInfo.Task.TaskFRecur.PropName, NamedPropInfo.Task.TaskFRecur);
			propInfos.Add(NamedPropInfo.Task.TaskRole.PropName, NamedPropInfo.Task.TaskRole);
			propInfos.Add(NamedPropInfo.Task.TaskOwnership.PropName, NamedPropInfo.Task.TaskOwnership);
			propInfos.Add(NamedPropInfo.Task.TaskDelegValue.PropName, NamedPropInfo.Task.TaskDelegValue);
			propInfos.Add(NamedPropInfo.Task.TaskCardData.PropName, NamedPropInfo.Task.TaskCardData);
			propInfos.Add(NamedPropInfo.Task.TaskFFixOffline.PropName, NamedPropInfo.Task.TaskFFixOffline);
			propInfos.Add(NamedPropInfo.Task.TaskSchdPrio.PropName, NamedPropInfo.Task.TaskSchdPrio);
			propInfos.Add(NamedPropInfo.Task.TaskFormURN.PropName, NamedPropInfo.Task.TaskFormURN);
			propInfos.Add(NamedPropInfo.Task.TaskWebUrl.PropName, NamedPropInfo.Task.TaskWebUrl);
			propInfos.Add(NamedPropInfo.Task.TaskCustomStatus.PropName, NamedPropInfo.Task.TaskCustomStatus);
			propInfos.Add(NamedPropInfo.Task.TaskCustomPriority.PropName, NamedPropInfo.Task.TaskCustomPriority);
			propInfos.Add(NamedPropInfo.Task.TaskCustomFlags.PropName, NamedPropInfo.Task.TaskCustomFlags);
			propInfos.Add(NamedPropInfo.Task.DoItTime.PropName, NamedPropInfo.Task.DoItTime);
		}

		private static void AddAddressNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Address.FSendPlainText.PropName, NamedPropInfo.Address.FSendPlainText);
			propInfos.Add(NamedPropInfo.Address.FPostalAddress.PropName, NamedPropInfo.Address.FPostalAddress);
			propInfos.Add(NamedPropInfo.Address.FileUnder.PropName, NamedPropInfo.Address.FileUnder);
			propInfos.Add(NamedPropInfo.Address.FileUnderId.PropName, NamedPropInfo.Address.FileUnderId);
			propInfos.Add(NamedPropInfo.Address.ContactItemData.PropName, NamedPropInfo.Address.ContactItemData);
			propInfos.Add(NamedPropInfo.Address.SelectedEmailAddress.PropName, NamedPropInfo.Address.SelectedEmailAddress);
			propInfos.Add(NamedPropInfo.Address.SelectedOriginalDisplayName.PropName, NamedPropInfo.Address.SelectedOriginalDisplayName);
			propInfos.Add(NamedPropInfo.Address.SelectedOriginalEntryID.PropName, NamedPropInfo.Address.SelectedOriginalEntryID);
			propInfos.Add(NamedPropInfo.Address.ContactItemData2.PropName, NamedPropInfo.Address.ContactItemData2);
			propInfos.Add(NamedPropInfo.Address.ChildrenStr.PropName, NamedPropInfo.Address.ChildrenStr);
			propInfos.Add(NamedPropInfo.Address.ReferredBy.PropName, NamedPropInfo.Address.ReferredBy);
			propInfos.Add(NamedPropInfo.Address.Department.PropName, NamedPropInfo.Address.Department);
			propInfos.Add(NamedPropInfo.Address.HasPicture.PropName, NamedPropInfo.Address.HasPicture);
			propInfos.Add(NamedPropInfo.Address.UserCertificateStr.PropName, NamedPropInfo.Address.UserCertificateStr);
			propInfos.Add(NamedPropInfo.Address.LastNameAndFirstName.PropName, NamedPropInfo.Address.LastNameAndFirstName);
			propInfos.Add(NamedPropInfo.Address.CompanyAndFullName.PropName, NamedPropInfo.Address.CompanyAndFullName);
			propInfos.Add(NamedPropInfo.Address.FullNameAndCompany.PropName, NamedPropInfo.Address.FullNameAndCompany);
			propInfos.Add(NamedPropInfo.Address.HomeAddress.PropName, NamedPropInfo.Address.HomeAddress);
			propInfos.Add(NamedPropInfo.Address.WorkAddress.PropName, NamedPropInfo.Address.WorkAddress);
			propInfos.Add(NamedPropInfo.Address.OtherAddress.PropName, NamedPropInfo.Address.OtherAddress);
			propInfos.Add(NamedPropInfo.Address.PostalAddressId.PropName, NamedPropInfo.Address.PostalAddressId);
			propInfos.Add(NamedPropInfo.Address.ContactCharSet.PropName, NamedPropInfo.Address.ContactCharSet);
			propInfos.Add(NamedPropInfo.Address.AutoLog.PropName, NamedPropInfo.Address.AutoLog);
			propInfos.Add(NamedPropInfo.Address.FileUnderList.PropName, NamedPropInfo.Address.FileUnderList);
			propInfos.Add(NamedPropInfo.Address.EmailList.PropName, NamedPropInfo.Address.EmailList);
			propInfos.Add(NamedPropInfo.Address.ABPEmailList.PropName, NamedPropInfo.Address.ABPEmailList);
			propInfos.Add(NamedPropInfo.Address.ABPArrayType.PropName, NamedPropInfo.Address.ABPArrayType);
			propInfos.Add(NamedPropInfo.Address.DontAgeLog.PropName, NamedPropInfo.Address.DontAgeLog);
			propInfos.Add(NamedPropInfo.Address.HTML.PropName, NamedPropInfo.Address.HTML);
			propInfos.Add(NamedPropInfo.Address.YomiFirstName.PropName, NamedPropInfo.Address.YomiFirstName);
			propInfos.Add(NamedPropInfo.Address.YomiLastName.PropName, NamedPropInfo.Address.YomiLastName);
			propInfos.Add(NamedPropInfo.Address.YomiCompanyName.PropName, NamedPropInfo.Address.YomiCompanyName);
			propInfos.Add(NamedPropInfo.Address.LastFirstNoSpace.PropName, NamedPropInfo.Address.LastFirstNoSpace);
			propInfos.Add(NamedPropInfo.Address.LastFirstSpaceOnly.PropName, NamedPropInfo.Address.LastFirstSpaceOnly);
			propInfos.Add(NamedPropInfo.Address.CompanyLastFirstNoSpace.PropName, NamedPropInfo.Address.CompanyLastFirstNoSpace);
			propInfos.Add(NamedPropInfo.Address.CompanyLastFirstSpaceOnly.PropName, NamedPropInfo.Address.CompanyLastFirstSpaceOnly);
			propInfos.Add(NamedPropInfo.Address.LastFirstNoSpaceCompany.PropName, NamedPropInfo.Address.LastFirstNoSpaceCompany);
			propInfos.Add(NamedPropInfo.Address.LastFirstSpaceOnlyCompany.PropName, NamedPropInfo.Address.LastFirstSpaceOnlyCompany);
			propInfos.Add(NamedPropInfo.Address.LastFirstAndSuffix.PropName, NamedPropInfo.Address.LastFirstAndSuffix);
			propInfos.Add(NamedPropInfo.Address.FirstMiddleLastSuffix.PropName, NamedPropInfo.Address.FirstMiddleLastSuffix);
			propInfos.Add(NamedPropInfo.Address.LastFirstNoSpaceAndSuffix.PropName, NamedPropInfo.Address.LastFirstNoSpaceAndSuffix);
			propInfos.Add(NamedPropInfo.Address.BCDisplayDefinition.PropName, NamedPropInfo.Address.BCDisplayDefinition);
			propInfos.Add(NamedPropInfo.Address.BCCardPicture.PropName, NamedPropInfo.Address.BCCardPicture);
			propInfos.Add(NamedPropInfo.Address.InterConnectBizcard.PropName, NamedPropInfo.Address.InterConnectBizcard);
			propInfos.Add(NamedPropInfo.Address.InterConnectBizcardFlag.PropName, NamedPropInfo.Address.InterConnectBizcardFlag);
			propInfos.Add(NamedPropInfo.Address.InterConnectBizcardLastUpdate.PropName, NamedPropInfo.Address.InterConnectBizcardLastUpdate);
			propInfos.Add(NamedPropInfo.Address.WorkAddressStreet.PropName, NamedPropInfo.Address.WorkAddressStreet);
			propInfos.Add(NamedPropInfo.Address.WorkAddressCity.PropName, NamedPropInfo.Address.WorkAddressCity);
			propInfos.Add(NamedPropInfo.Address.WorkAddressState.PropName, NamedPropInfo.Address.WorkAddressState);
			propInfos.Add(NamedPropInfo.Address.WorkAddressPostalCode.PropName, NamedPropInfo.Address.WorkAddressPostalCode);
			propInfos.Add(NamedPropInfo.Address.WorkAddressCountry.PropName, NamedPropInfo.Address.WorkAddressCountry);
			propInfos.Add(NamedPropInfo.Address.WorkAddressPostOfficeBox.PropName, NamedPropInfo.Address.WorkAddressPostOfficeBox);
			propInfos.Add(NamedPropInfo.Address.DLCountMembers.PropName, NamedPropInfo.Address.DLCountMembers);
			propInfos.Add(NamedPropInfo.Address.DLChecksum.PropName, NamedPropInfo.Address.DLChecksum);
			propInfos.Add(NamedPropInfo.Address.BirthdayEventEID.PropName, NamedPropInfo.Address.BirthdayEventEID);
			propInfos.Add(NamedPropInfo.Address.AnniversaryEventEID.PropName, NamedPropInfo.Address.AnniversaryEventEID);
			propInfos.Add(NamedPropInfo.Address.ContactUserField1.PropName, NamedPropInfo.Address.ContactUserField1);
			propInfos.Add(NamedPropInfo.Address.ContactUserField2.PropName, NamedPropInfo.Address.ContactUserField2);
			propInfos.Add(NamedPropInfo.Address.ContactUserField3.PropName, NamedPropInfo.Address.ContactUserField3);
			propInfos.Add(NamedPropInfo.Address.ContactUserField4.PropName, NamedPropInfo.Address.ContactUserField4);
			propInfos.Add(NamedPropInfo.Address.DLName.PropName, NamedPropInfo.Address.DLName);
			propInfos.Add(NamedPropInfo.Address.DLOneOffMembers.PropName, NamedPropInfo.Address.DLOneOffMembers);
			propInfos.Add(NamedPropInfo.Address.DLMembers.PropName, NamedPropInfo.Address.DLMembers);
			propInfos.Add(NamedPropInfo.Address.ConfServerNames.PropName, NamedPropInfo.Address.ConfServerNames);
			propInfos.Add(NamedPropInfo.Address.ConfDefServerIndex.PropName, NamedPropInfo.Address.ConfDefServerIndex);
			propInfos.Add(NamedPropInfo.Address.ConfBackupServerIndex.PropName, NamedPropInfo.Address.ConfBackupServerIndex);
			propInfos.Add(NamedPropInfo.Address.ConfEmailIndex.PropName, NamedPropInfo.Address.ConfEmailIndex);
			propInfos.Add(NamedPropInfo.Address.MoreAddressType.PropName, NamedPropInfo.Address.MoreAddressType);
			propInfos.Add(NamedPropInfo.Address.MoreEmailAddress.PropName, NamedPropInfo.Address.MoreEmailAddress);
			propInfos.Add(NamedPropInfo.Address.ContactEmailAddressesStr.PropName, NamedPropInfo.Address.ContactEmailAddressesStr);
			propInfos.Add(NamedPropInfo.Address.ConfServerNamesStr.PropName, NamedPropInfo.Address.ConfServerNamesStr);
			propInfos.Add(NamedPropInfo.Address.ConfAliasDisplay.PropName, NamedPropInfo.Address.ConfAliasDisplay);
			propInfos.Add(NamedPropInfo.Address.ConfServerDisplay.PropName, NamedPropInfo.Address.ConfServerDisplay);
			propInfos.Add(NamedPropInfo.Address.MeFlag.PropName, NamedPropInfo.Address.MeFlag);
			propInfos.Add(NamedPropInfo.Address.InstMsg.PropName, NamedPropInfo.Address.InstMsg);
			propInfos.Add(NamedPropInfo.Address.NetMeetingOrganizerAlias.PropName, NamedPropInfo.Address.NetMeetingOrganizerAlias);
			propInfos.Add(NamedPropInfo.Address.AddressSelection.PropName, NamedPropInfo.Address.AddressSelection);
			propInfos.Add(NamedPropInfo.Address.EmailSelection.PropName, NamedPropInfo.Address.EmailSelection);
			propInfos.Add(NamedPropInfo.Address.Phone1Selection.PropName, NamedPropInfo.Address.Phone1Selection);
			propInfos.Add(NamedPropInfo.Address.Phone2Selection.PropName, NamedPropInfo.Address.Phone2Selection);
			propInfos.Add(NamedPropInfo.Address.Phone3Selection.PropName, NamedPropInfo.Address.Phone3Selection);
			propInfos.Add(NamedPropInfo.Address.Phone4Selection.PropName, NamedPropInfo.Address.Phone4Selection);
			propInfos.Add(NamedPropInfo.Address.Phone5Selection.PropName, NamedPropInfo.Address.Phone5Selection);
			propInfos.Add(NamedPropInfo.Address.Phone6Selection.PropName, NamedPropInfo.Address.Phone6Selection);
			propInfos.Add(NamedPropInfo.Address.Phone7Selection.PropName, NamedPropInfo.Address.Phone7Selection);
			propInfos.Add(NamedPropInfo.Address.Phone8Selection.PropName, NamedPropInfo.Address.Phone8Selection);
			propInfos.Add(NamedPropInfo.Address.SelectedAddress.PropName, NamedPropInfo.Address.SelectedAddress);
			propInfos.Add(NamedPropInfo.Address.SelectedPhone1.PropName, NamedPropInfo.Address.SelectedPhone1);
			propInfos.Add(NamedPropInfo.Address.SelectedPhone2.PropName, NamedPropInfo.Address.SelectedPhone2);
			propInfos.Add(NamedPropInfo.Address.EmailDisplayName.PropName, NamedPropInfo.Address.EmailDisplayName);
			propInfos.Add(NamedPropInfo.Address.EmailEntryID.PropName, NamedPropInfo.Address.EmailEntryID);
			propInfos.Add(NamedPropInfo.Address.EmailAddressType.PropName, NamedPropInfo.Address.EmailAddressType);
			propInfos.Add(NamedPropInfo.Address.EmailEmailAddress.PropName, NamedPropInfo.Address.EmailEmailAddress);
			propInfos.Add(NamedPropInfo.Address.EmailOriginalDisplayName.PropName, NamedPropInfo.Address.EmailOriginalDisplayName);
			propInfos.Add(NamedPropInfo.Address.EmailOriginalEntryID.PropName, NamedPropInfo.Address.EmailOriginalEntryID);
			propInfos.Add(NamedPropInfo.Address.Email1RTF.PropName, NamedPropInfo.Address.Email1RTF);
			propInfos.Add(NamedPropInfo.Address.EmailEmailType.PropName, NamedPropInfo.Address.EmailEmailType);
			propInfos.Add(NamedPropInfo.Address.SelectedPhone3.PropName, NamedPropInfo.Address.SelectedPhone3);
			propInfos.Add(NamedPropInfo.Address.SelectedPhone4.PropName, NamedPropInfo.Address.SelectedPhone4);
			propInfos.Add(NamedPropInfo.Address.SelectedPhone5.PropName, NamedPropInfo.Address.SelectedPhone5);
			propInfos.Add(NamedPropInfo.Address.SelectedPhone6.PropName, NamedPropInfo.Address.SelectedPhone6);
			propInfos.Add(NamedPropInfo.Address.SelectedPhone7.PropName, NamedPropInfo.Address.SelectedPhone7);
			propInfos.Add(NamedPropInfo.Address.SelectedPhone8.PropName, NamedPropInfo.Address.SelectedPhone8);
			propInfos.Add(NamedPropInfo.Address.Email2DisplayName.PropName, NamedPropInfo.Address.Email2DisplayName);
			propInfos.Add(NamedPropInfo.Address.Email2EntryID.PropName, NamedPropInfo.Address.Email2EntryID);
			propInfos.Add(NamedPropInfo.Address.Email2AddressType.PropName, NamedPropInfo.Address.Email2AddressType);
			propInfos.Add(NamedPropInfo.Address.Email2EmailAddress.PropName, NamedPropInfo.Address.Email2EmailAddress);
			propInfos.Add(NamedPropInfo.Address.Email2OriginalDisplayName.PropName, NamedPropInfo.Address.Email2OriginalDisplayName);
			propInfos.Add(NamedPropInfo.Address.Email2OriginalEntryID.PropName, NamedPropInfo.Address.Email2OriginalEntryID);
			propInfos.Add(NamedPropInfo.Address.Email2RTF.PropName, NamedPropInfo.Address.Email2RTF);
			propInfos.Add(NamedPropInfo.Address.Email2EmailType.PropName, NamedPropInfo.Address.Email2EmailType);
			propInfos.Add(NamedPropInfo.Address.Email3DisplayName.PropName, NamedPropInfo.Address.Email3DisplayName);
			propInfos.Add(NamedPropInfo.Address.Email3EntryID.PropName, NamedPropInfo.Address.Email3EntryID);
			propInfos.Add(NamedPropInfo.Address.Email3AddressType.PropName, NamedPropInfo.Address.Email3AddressType);
			propInfos.Add(NamedPropInfo.Address.Email3EmailAddress.PropName, NamedPropInfo.Address.Email3EmailAddress);
			propInfos.Add(NamedPropInfo.Address.Email3OriginalDisplayName.PropName, NamedPropInfo.Address.Email3OriginalDisplayName);
			propInfos.Add(NamedPropInfo.Address.Email3OriginalEntryID.PropName, NamedPropInfo.Address.Email3OriginalEntryID);
			propInfos.Add(NamedPropInfo.Address.Email3RTF.PropName, NamedPropInfo.Address.Email3RTF);
			propInfos.Add(NamedPropInfo.Address.Email3EmailType.PropName, NamedPropInfo.Address.Email3EmailType);
			propInfos.Add(NamedPropInfo.Address.Fax1DisplayName.PropName, NamedPropInfo.Address.Fax1DisplayName);
			propInfos.Add(NamedPropInfo.Address.Fax1EntryID.PropName, NamedPropInfo.Address.Fax1EntryID);
			propInfos.Add(NamedPropInfo.Address.Fax1AddressType.PropName, NamedPropInfo.Address.Fax1AddressType);
			propInfos.Add(NamedPropInfo.Address.Fax1EmailAddress.PropName, NamedPropInfo.Address.Fax1EmailAddress);
			propInfos.Add(NamedPropInfo.Address.Fax1OriginalDisplayName.PropName, NamedPropInfo.Address.Fax1OriginalDisplayName);
			propInfos.Add(NamedPropInfo.Address.Fax1OriginalEntryID.PropName, NamedPropInfo.Address.Fax1OriginalEntryID);
			propInfos.Add(NamedPropInfo.Address.Fax1RTF.PropName, NamedPropInfo.Address.Fax1RTF);
			propInfos.Add(NamedPropInfo.Address.Fax1EmailType.PropName, NamedPropInfo.Address.Fax1EmailType);
			propInfos.Add(NamedPropInfo.Address.Fax2DisplayName.PropName, NamedPropInfo.Address.Fax2DisplayName);
			propInfos.Add(NamedPropInfo.Address.Fax2EntryID.PropName, NamedPropInfo.Address.Fax2EntryID);
			propInfos.Add(NamedPropInfo.Address.Fax2AddressType.PropName, NamedPropInfo.Address.Fax2AddressType);
			propInfos.Add(NamedPropInfo.Address.Fax2EmailAddress.PropName, NamedPropInfo.Address.Fax2EmailAddress);
			propInfos.Add(NamedPropInfo.Address.Fax2OriginalDisplayName.PropName, NamedPropInfo.Address.Fax2OriginalDisplayName);
			propInfos.Add(NamedPropInfo.Address.Fax2OriginalEntryID.PropName, NamedPropInfo.Address.Fax2OriginalEntryID);
			propInfos.Add(NamedPropInfo.Address.Fax2RTF.PropName, NamedPropInfo.Address.Fax2RTF);
			propInfos.Add(NamedPropInfo.Address.Fax2EmailType.PropName, NamedPropInfo.Address.Fax2EmailType);
			propInfos.Add(NamedPropInfo.Address.Fax3DisplayName.PropName, NamedPropInfo.Address.Fax3DisplayName);
			propInfos.Add(NamedPropInfo.Address.Fax3EntryID.PropName, NamedPropInfo.Address.Fax3EntryID);
			propInfos.Add(NamedPropInfo.Address.Fax3AddressType.PropName, NamedPropInfo.Address.Fax3AddressType);
			propInfos.Add(NamedPropInfo.Address.Fax3EmailAddress.PropName, NamedPropInfo.Address.Fax3EmailAddress);
			propInfos.Add(NamedPropInfo.Address.Fax3OriginalDisplayName.PropName, NamedPropInfo.Address.Fax3OriginalDisplayName);
			propInfos.Add(NamedPropInfo.Address.Fax3OriginalEntryID.PropName, NamedPropInfo.Address.Fax3OriginalEntryID);
			propInfos.Add(NamedPropInfo.Address.Fax3RTF.PropName, NamedPropInfo.Address.Fax3RTF);
			propInfos.Add(NamedPropInfo.Address.Fax3EmailType.PropName, NamedPropInfo.Address.Fax3EmailType);
			propInfos.Add(NamedPropInfo.Address.FreeBusyLocation.PropName, NamedPropInfo.Address.FreeBusyLocation);
			propInfos.Add(NamedPropInfo.Address.EMSAbX509Cert.PropName, NamedPropInfo.Address.EMSAbX509Cert);
			propInfos.Add(NamedPropInfo.Address.HomeAddressCountryCode.PropName, NamedPropInfo.Address.HomeAddressCountryCode);
			propInfos.Add(NamedPropInfo.Address.WorkAddressCountryCode.PropName, NamedPropInfo.Address.WorkAddressCountryCode);
			propInfos.Add(NamedPropInfo.Address.OtherAddressCountryCode.PropName, NamedPropInfo.Address.OtherAddressCountryCode);
			propInfos.Add(NamedPropInfo.Address.AddressCountryCode.PropName, NamedPropInfo.Address.AddressCountryCode);
			propInfos.Add(NamedPropInfo.Address.AddressLinked.PropName, NamedPropInfo.Address.AddressLinked);
			propInfos.Add(NamedPropInfo.Address.AddressBookEntryId.PropName, NamedPropInfo.Address.AddressBookEntryId);
			propInfos.Add(NamedPropInfo.Address.SmtpAddressCache.PropName, NamedPropInfo.Address.SmtpAddressCache);
			propInfos.Add(NamedPropInfo.Address.LinkRejectHistoryRaw.PropName, NamedPropInfo.Address.LinkRejectHistoryRaw);
			propInfos.Add(NamedPropInfo.Address.GALLinkState.PropName, NamedPropInfo.Address.GALLinkState);
			propInfos.Add(NamedPropInfo.Address.ContactGALLinkID.PropName, NamedPropInfo.Address.ContactGALLinkID);
			propInfos.Add(NamedPropInfo.Address.Schools.PropName, NamedPropInfo.Address.Schools);
			propInfos.Add(NamedPropInfo.Address.InternalPersonType.PropName, NamedPropInfo.Address.InternalPersonType);
			propInfos.Add(NamedPropInfo.Address.UserApprovedLink.PropName, NamedPropInfo.Address.UserApprovedLink);
			propInfos.Add(NamedPropInfo.Address.DisplayNameFirstLast.PropName, NamedPropInfo.Address.DisplayNameFirstLast);
			propInfos.Add(NamedPropInfo.Address.DisplayNameLastFirst.PropName, NamedPropInfo.Address.DisplayNameLastFirst);
			propInfos.Add(NamedPropInfo.Address.DisplayNamePriority.PropName, NamedPropInfo.Address.DisplayNamePriority);
			propInfos.Add(NamedPropInfo.Address.ContactOtherPhone2.PropName, NamedPropInfo.Address.ContactOtherPhone2);
			propInfos.Add(NamedPropInfo.Address.MobileTelephoneNumber2.PropName, NamedPropInfo.Address.MobileTelephoneNumber2);
			propInfos.Add(NamedPropInfo.Address.BirthdayLocal.PropName, NamedPropInfo.Address.BirthdayLocal);
			propInfos.Add(NamedPropInfo.Address.WeddingAnniversaryLocal.PropName, NamedPropInfo.Address.WeddingAnniversaryLocal);
			propInfos.Add(NamedPropInfo.Address.ProtectedEmailAddress.PropName, NamedPropInfo.Address.ProtectedEmailAddress);
			propInfos.Add(NamedPropInfo.Address.ProtectedPhoneNumber.PropName, NamedPropInfo.Address.ProtectedPhoneNumber);
			propInfos.Add(NamedPropInfo.Address.NetMeetingDocPathName.PropName, NamedPropInfo.Address.NetMeetingDocPathName);
			propInfos.Add(NamedPropInfo.Address.NetMeetingConferenceServerAllowExternal.PropName, NamedPropInfo.Address.NetMeetingConferenceServerAllowExternal);
			propInfos.Add(NamedPropInfo.Address.NetMeetingConferenceSerPassword.PropName, NamedPropInfo.Address.NetMeetingConferenceSerPassword);
			propInfos.Add(NamedPropInfo.Address.ImContactSipUriAddress.PropName, NamedPropInfo.Address.ImContactSipUriAddress);
			propInfos.Add(NamedPropInfo.Address.IsFavorite.PropName, NamedPropInfo.Address.IsFavorite);
			propInfos.Add(NamedPropInfo.Address.BirthdayContactAttributionDisplayName.PropName, NamedPropInfo.Address.BirthdayContactAttributionDisplayName);
			propInfos.Add(NamedPropInfo.Address.BirthdayContactEntryId.PropName, NamedPropInfo.Address.BirthdayContactEntryId);
			propInfos.Add(NamedPropInfo.Address.IsBirthdayContactWritable.PropName, NamedPropInfo.Address.IsBirthdayContactWritable);
			propInfos.Add(NamedPropInfo.Address.MyContactsFolderEntryId.PropName, NamedPropInfo.Address.MyContactsFolderEntryId);
			propInfos.Add(NamedPropInfo.Address.MyContactsExtendedFolderEntryId.PropName, NamedPropInfo.Address.MyContactsExtendedFolderEntryId);
			propInfos.Add(NamedPropInfo.Address.MyContactsFolders.PropName, NamedPropInfo.Address.MyContactsFolders);
			propInfos.Add(NamedPropInfo.Address.LinkChangeHistory.PropName, NamedPropInfo.Address.LinkChangeHistory);
			propInfos.Add(NamedPropInfo.Address.PeopleIKnowEmailAddressCollection.PropName, NamedPropInfo.Address.PeopleIKnowEmailAddressCollection);
			propInfos.Add(NamedPropInfo.Address.PeopleIKnowEmailAddressRelevanceScoreCollection.PropName, NamedPropInfo.Address.PeopleIKnowEmailAddressRelevanceScoreCollection);
			propInfos.Add(NamedPropInfo.Address.SenderRelevanceScore.PropName, NamedPropInfo.Address.SenderRelevanceScore);
		}

		private static void AddRecipientNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddDelegationNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddCommonNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Common.DayOfMonth.PropName, NamedPropInfo.Common.DayOfMonth);
			propInfos.Add(NamedPropInfo.Common.DayOfWeekMask.PropName, NamedPropInfo.Common.DayOfWeekMask);
			propInfos.Add(NamedPropInfo.Common.EndRecurDate.PropName, NamedPropInfo.Common.EndRecurDate);
			propInfos.Add(NamedPropInfo.Common.Instance.PropName, NamedPropInfo.Common.Instance);
			propInfos.Add(NamedPropInfo.Common.Interval.PropName, NamedPropInfo.Common.Interval);
			propInfos.Add(NamedPropInfo.Common.Occurrences.PropName, NamedPropInfo.Common.Occurrences);
			propInfos.Add(NamedPropInfo.Common.MonthOfYear.PropName, NamedPropInfo.Common.MonthOfYear);
			propInfos.Add(NamedPropInfo.Common.RecurrenceType.PropName, NamedPropInfo.Common.RecurrenceType);
			propInfos.Add(NamedPropInfo.Common.StartRecurDate.PropName, NamedPropInfo.Common.StartRecurDate);
			propInfos.Add(NamedPropInfo.Common.FSliding.PropName, NamedPropInfo.Common.FSliding);
			propInfos.Add(NamedPropInfo.Common.FNoEndDate.PropName, NamedPropInfo.Common.FNoEndDate);
			propInfos.Add(NamedPropInfo.Common.EndRecurTime.PropName, NamedPropInfo.Common.EndRecurTime);
			propInfos.Add(NamedPropInfo.Common.RecurDuration.PropName, NamedPropInfo.Common.RecurDuration);
			propInfos.Add(NamedPropInfo.Common.Exceptions.PropName, NamedPropInfo.Common.Exceptions);
			propInfos.Add(NamedPropInfo.Common.FEndByOcc.PropName, NamedPropInfo.Common.FEndByOcc);
			propInfos.Add(NamedPropInfo.Common.ApptStickerID.PropName, NamedPropInfo.Common.ApptStickerID);
			propInfos.Add(NamedPropInfo.Common.ApptExtractVersion.PropName, NamedPropInfo.Common.ApptExtractVersion);
			propInfos.Add(NamedPropInfo.Common.ApptExtractTime.PropName, NamedPropInfo.Common.ApptExtractTime);
			propInfos.Add(NamedPropInfo.Common.ReminderDelta.PropName, NamedPropInfo.Common.ReminderDelta);
			propInfos.Add(NamedPropInfo.Common.ReminderTime.PropName, NamedPropInfo.Common.ReminderTime);
			propInfos.Add(NamedPropInfo.Common.ReminderSet.PropName, NamedPropInfo.Common.ReminderSet);
			propInfos.Add(NamedPropInfo.Common.ReminderTimeTime.PropName, NamedPropInfo.Common.ReminderTimeTime);
			propInfos.Add(NamedPropInfo.Common.ReminderTimeDate.PropName, NamedPropInfo.Common.ReminderTimeDate);
			propInfos.Add(NamedPropInfo.Common.Private.PropName, NamedPropInfo.Common.Private);
			propInfos.Add(NamedPropInfo.Common.AgingDontAgeMe.PropName, NamedPropInfo.Common.AgingDontAgeMe);
			propInfos.Add(NamedPropInfo.Common.FormStorage.PropName, NamedPropInfo.Common.FormStorage);
			propInfos.Add(NamedPropInfo.Common.SideEffects.PropName, NamedPropInfo.Common.SideEffects);
			propInfos.Add(NamedPropInfo.Common.RemoteStatus.PropName, NamedPropInfo.Common.RemoteStatus);
			propInfos.Add(NamedPropInfo.Common.PageDirStream.PropName, NamedPropInfo.Common.PageDirStream);
			propInfos.Add(NamedPropInfo.Common.SmartNoAttach.PropName, NamedPropInfo.Common.SmartNoAttach);
			propInfos.Add(NamedPropInfo.Common.CustomPages.PropName, NamedPropInfo.Common.CustomPages);
			propInfos.Add(NamedPropInfo.Common.CommonStart.PropName, NamedPropInfo.Common.CommonStart);
			propInfos.Add(NamedPropInfo.Common.CommonEnd.PropName, NamedPropInfo.Common.CommonEnd);
			propInfos.Add(NamedPropInfo.Common.TaskMode.PropName, NamedPropInfo.Common.TaskMode);
			propInfos.Add(NamedPropInfo.Common.TaskGlobalObjId.PropName, NamedPropInfo.Common.TaskGlobalObjId);
			propInfos.Add(NamedPropInfo.Common.SniffState.PropName, NamedPropInfo.Common.SniffState);
			propInfos.Add(NamedPropInfo.Common.FormPropStream.PropName, NamedPropInfo.Common.FormPropStream);
			propInfos.Add(NamedPropInfo.Common.ReminderOverride.PropName, NamedPropInfo.Common.ReminderOverride);
			propInfos.Add(NamedPropInfo.Common.ReminderType.PropName, NamedPropInfo.Common.ReminderType);
			propInfos.Add(NamedPropInfo.Common.ReminderPlaySound.PropName, NamedPropInfo.Common.ReminderPlaySound);
			propInfos.Add(NamedPropInfo.Common.ReminderFileParam.PropName, NamedPropInfo.Common.ReminderFileParam);
			propInfos.Add(NamedPropInfo.Common.VerbStream.PropName, NamedPropInfo.Common.VerbStream);
			propInfos.Add(NamedPropInfo.Common.VerbResponse.PropName, NamedPropInfo.Common.VerbResponse);
			propInfos.Add(NamedPropInfo.Common.Request.PropName, NamedPropInfo.Common.Request);
			propInfos.Add(NamedPropInfo.Common.Mileage.PropName, NamedPropInfo.Common.Mileage);
			propInfos.Add(NamedPropInfo.Common.Billing.PropName, NamedPropInfo.Common.Billing);
			propInfos.Add(NamedPropInfo.Common.NonSendableTo.PropName, NamedPropInfo.Common.NonSendableTo);
			propInfos.Add(NamedPropInfo.Common.NonSendableCC.PropName, NamedPropInfo.Common.NonSendableCC);
			propInfos.Add(NamedPropInfo.Common.NonSendableBCC.PropName, NamedPropInfo.Common.NonSendableBCC);
			propInfos.Add(NamedPropInfo.Common.Companies.PropName, NamedPropInfo.Common.Companies);
			propInfos.Add(NamedPropInfo.Common.Contacts.PropName, NamedPropInfo.Common.Contacts);
			propInfos.Add(NamedPropInfo.Common.CompaniesStr.PropName, NamedPropInfo.Common.CompaniesStr);
			propInfos.Add(NamedPropInfo.Common.ContactsStr.PropName, NamedPropInfo.Common.ContactsStr);
			propInfos.Add(NamedPropInfo.Common.Url.PropName, NamedPropInfo.Common.Url);
			propInfos.Add(NamedPropInfo.Common.HtmlForm.PropName, NamedPropInfo.Common.HtmlForm);
			propInfos.Add(NamedPropInfo.Common.PropDefStream.PropName, NamedPropInfo.Common.PropDefStream);
			propInfos.Add(NamedPropInfo.Common.ScriptStream.PropName, NamedPropInfo.Common.ScriptStream);
			propInfos.Add(NamedPropInfo.Common.CustomFlag.PropName, NamedPropInfo.Common.CustomFlag);
			propInfos.Add(NamedPropInfo.Common.NonSendToTrackStatus.PropName, NamedPropInfo.Common.NonSendToTrackStatus);
			propInfos.Add(NamedPropInfo.Common.NonSendCcTrackStatus.PropName, NamedPropInfo.Common.NonSendCcTrackStatus);
			propInfos.Add(NamedPropInfo.Common.NonSendBccTrackStatus.PropName, NamedPropInfo.Common.NonSendBccTrackStatus);
			propInfos.Add(NamedPropInfo.Common.RecallTime.PropName, NamedPropInfo.Common.RecallTime);
			propInfos.Add(NamedPropInfo.Common.AttachStripped.PropName, NamedPropInfo.Common.AttachStripped);
			propInfos.Add(NamedPropInfo.Common.MinReadVersion.PropName, NamedPropInfo.Common.MinReadVersion);
			propInfos.Add(NamedPropInfo.Common.MinWriteVersion.PropName, NamedPropInfo.Common.MinWriteVersion);
			propInfos.Add(NamedPropInfo.Common.CurrentVersion.PropName, NamedPropInfo.Common.CurrentVersion);
			propInfos.Add(NamedPropInfo.Common.CurrentVersionName.PropName, NamedPropInfo.Common.CurrentVersionName);
			propInfos.Add(NamedPropInfo.Common.ReminderNextTime.PropName, NamedPropInfo.Common.ReminderNextTime);
			propInfos.Add(NamedPropInfo.Common.ImapDeleted.PropName, NamedPropInfo.Common.ImapDeleted);
			propInfos.Add(NamedPropInfo.Common.MarkedForDownload.PropName, NamedPropInfo.Common.MarkedForDownload);
			propInfos.Add(NamedPropInfo.Common.HeaderItem.PropName, NamedPropInfo.Common.HeaderItem);
			propInfos.Add(NamedPropInfo.Common.InetAcctName.PropName, NamedPropInfo.Common.InetAcctName);
			propInfos.Add(NamedPropInfo.Common.InetAcctStamp.PropName, NamedPropInfo.Common.InetAcctStamp);
			propInfos.Add(NamedPropInfo.Common.UseTNEF.PropName, NamedPropInfo.Common.UseTNEF);
			propInfos.Add(NamedPropInfo.Common.LastAuthorClass.PropName, NamedPropInfo.Common.LastAuthorClass);
			propInfos.Add(NamedPropInfo.Common.ContactLinkSearchKey.PropName, NamedPropInfo.Common.ContactLinkSearchKey);
			propInfos.Add(NamedPropInfo.Common.ContactLinkEntry.PropName, NamedPropInfo.Common.ContactLinkEntry);
			propInfos.Add(NamedPropInfo.Common.ContactLinkName.PropName, NamedPropInfo.Common.ContactLinkName);
			propInfos.Add(NamedPropInfo.Common.DocObjWordmail.PropName, NamedPropInfo.Common.DocObjWordmail);
			propInfos.Add(NamedPropInfo.Common.StampedAccount.PropName, NamedPropInfo.Common.StampedAccount);
			propInfos.Add(NamedPropInfo.Common.UseInternetZone.PropName, NamedPropInfo.Common.UseInternetZone);
			propInfos.Add(NamedPropInfo.Common.FramesetBody.PropName, NamedPropInfo.Common.FramesetBody);
			propInfos.Add(NamedPropInfo.Common.SharingEnabled.PropName, NamedPropInfo.Common.SharingEnabled);
			propInfos.Add(NamedPropInfo.Common.UberGroup.PropName, NamedPropInfo.Common.UberGroup);
			propInfos.Add(NamedPropInfo.Common.SharingServerUrl.PropName, NamedPropInfo.Common.SharingServerUrl);
			propInfos.Add(NamedPropInfo.Common.SharingTitle.PropName, NamedPropInfo.Common.SharingTitle);
			propInfos.Add(NamedPropInfo.Common.SharingAutoPane.PropName, NamedPropInfo.Common.SharingAutoPane);
			propInfos.Add(NamedPropInfo.Common.SharingFooterID.PropName, NamedPropInfo.Common.SharingFooterID);
			propInfos.Add(NamedPropInfo.Common.ImgAttchmtsCompressLevel.PropName, NamedPropInfo.Common.ImgAttchmtsCompressLevel);
			propInfos.Add(NamedPropInfo.Common.ImgAttchmtsPreviewSize.PropName, NamedPropInfo.Common.ImgAttchmtsPreviewSize);
			propInfos.Add(NamedPropInfo.Common.ConflictItems.PropName, NamedPropInfo.Common.ConflictItems);
			propInfos.Add(NamedPropInfo.Common.SharingWebUrl.PropName, NamedPropInfo.Common.SharingWebUrl);
			propInfos.Add(NamedPropInfo.Common.SyncFailures.PropName, NamedPropInfo.Common.SyncFailures);
			propInfos.Add(NamedPropInfo.Common.ContentClass.PropName, NamedPropInfo.Common.ContentClass);
			propInfos.Add(NamedPropInfo.Common.ContentType.PropName, NamedPropInfo.Common.ContentType);
			propInfos.Add(NamedPropInfo.Common.SharingServerStatus.PropName, NamedPropInfo.Common.SharingServerStatus);
			propInfos.Add(NamedPropInfo.Common.IsIPFax.PropName, NamedPropInfo.Common.IsIPFax);
			propInfos.Add(NamedPropInfo.Common.SpamOriginalFolder.PropName, NamedPropInfo.Common.SpamOriginalFolder);
			propInfos.Add(NamedPropInfo.Common.DrmAttachmentNumber.PropName, NamedPropInfo.Common.DrmAttachmentNumber);
			propInfos.Add(NamedPropInfo.Common.IsInfoMailPost.PropName, NamedPropInfo.Common.IsInfoMailPost);
			propInfos.Add(NamedPropInfo.Common.ToDoOrdinalDate.PropName, NamedPropInfo.Common.ToDoOrdinalDate);
			propInfos.Add(NamedPropInfo.Common.ToDoSubOrdinal.PropName, NamedPropInfo.Common.ToDoSubOrdinal);
			propInfos.Add(NamedPropInfo.Common.ToDoTitle.PropName, NamedPropInfo.Common.ToDoTitle);
			propInfos.Add(NamedPropInfo.Common.FShouldTNEF.PropName, NamedPropInfo.Common.FShouldTNEF);
			propInfos.Add(NamedPropInfo.Common.AutoSaveOriginalItemInfo.PropName, NamedPropInfo.Common.AutoSaveOriginalItemInfo);
			propInfos.Add(NamedPropInfo.Common.InfoPathFormType.PropName, NamedPropInfo.Common.InfoPathFormType);
			propInfos.Add(NamedPropInfo.Common.LinkedApptItems.PropName, NamedPropInfo.Common.LinkedApptItems);
			propInfos.Add(NamedPropInfo.Common.ApptStartTimes.PropName, NamedPropInfo.Common.ApptStartTimes);
			propInfos.Add(NamedPropInfo.Common.Classified.PropName, NamedPropInfo.Common.Classified);
			propInfos.Add(NamedPropInfo.Common.Classification.PropName, NamedPropInfo.Common.Classification);
			propInfos.Add(NamedPropInfo.Common.ClassDesc.PropName, NamedPropInfo.Common.ClassDesc);
			propInfos.Add(NamedPropInfo.Common.ClassGuid.PropName, NamedPropInfo.Common.ClassGuid);
			propInfos.Add(NamedPropInfo.Common.OfflineStatus.PropName, NamedPropInfo.Common.OfflineStatus);
			propInfos.Add(NamedPropInfo.Common.ClassKeep.PropName, NamedPropInfo.Common.ClassKeep);
			propInfos.Add(NamedPropInfo.Common.Creator.PropName, NamedPropInfo.Common.Creator);
			propInfos.Add(NamedPropInfo.Common.ReferenceEID.PropName, NamedPropInfo.Common.ReferenceEID);
			propInfos.Add(NamedPropInfo.Common.StsContentTypeId.PropName, NamedPropInfo.Common.StsContentTypeId);
			propInfos.Add(NamedPropInfo.Common.ValidFlagStringProof.PropName, NamedPropInfo.Common.ValidFlagStringProof);
			propInfos.Add(NamedPropInfo.Common.FlagStringEnum.PropName, NamedPropInfo.Common.FlagStringEnum);
			propInfos.Add(NamedPropInfo.Common.SharedItemOwner.PropName, NamedPropInfo.Common.SharedItemOwner);
			propInfos.Add(NamedPropInfo.Common.ThemeDataXML.PropName, NamedPropInfo.Common.ThemeDataXML);
			propInfos.Add(NamedPropInfo.Common.ColorSchemeMappingXML.PropName, NamedPropInfo.Common.ColorSchemeMappingXML);
			propInfos.Add(NamedPropInfo.Common.PendingStateforTMDocument.PropName, NamedPropInfo.Common.PendingStateforTMDocument);
			propInfos.Add(NamedPropInfo.Common.AllowedFlagString.PropName, NamedPropInfo.Common.AllowedFlagString);
			propInfos.Add(NamedPropInfo.Common.ToDoTitleOM.PropName, NamedPropInfo.Common.ToDoTitleOM);
			propInfos.Add(NamedPropInfo.Common.StartRecurTime.PropName, NamedPropInfo.Common.StartRecurTime);
			propInfos.Add(NamedPropInfo.Common.ArchiveSourseSupportMask.PropName, NamedPropInfo.Common.ArchiveSourseSupportMask);
			propInfos.Add(NamedPropInfo.Common.CrawlSourceSupportMask.PropName, NamedPropInfo.Common.CrawlSourceSupportMask);
			propInfos.Add(NamedPropInfo.Common.MailboxAssociationExternalId.PropName, NamedPropInfo.Common.MailboxAssociationExternalId);
			propInfos.Add(NamedPropInfo.Common.MailboxAssociationLegacyDN.PropName, NamedPropInfo.Common.MailboxAssociationLegacyDN);
			propInfos.Add(NamedPropInfo.Common.MailboxAssociationIsMember.PropName, NamedPropInfo.Common.MailboxAssociationIsMember);
			propInfos.Add(NamedPropInfo.Common.MailboxAssociationJoinedBy.PropName, NamedPropInfo.Common.MailboxAssociationJoinedBy);
			propInfos.Add(NamedPropInfo.Common.MailboxAssociationIsPin.PropName, NamedPropInfo.Common.MailboxAssociationIsPin);
			propInfos.Add(NamedPropInfo.Common.MailboxAssociationShouldEscalate.PropName, NamedPropInfo.Common.MailboxAssociationShouldEscalate);
			propInfos.Add(NamedPropInfo.Common.MailboxAssociationIsAutoSubscribed.PropName, NamedPropInfo.Common.MailboxAssociationIsAutoSubscribed);
			propInfos.Add(NamedPropInfo.Common.MailboxAssociationSmtpAddress.PropName, NamedPropInfo.Common.MailboxAssociationSmtpAddress);
			propInfos.Add(NamedPropInfo.Common.MailboxAssociationJoinDate.PropName, NamedPropInfo.Common.MailboxAssociationJoinDate);
			propInfos.Add(NamedPropInfo.Common.MailboxAssociationPinDate.PropName, NamedPropInfo.Common.MailboxAssociationPinDate);
			propInfos.Add(NamedPropInfo.Common.MailboxAssociationLastVisitedDate.PropName, NamedPropInfo.Common.MailboxAssociationLastVisitedDate);
			propInfos.Add(NamedPropInfo.Common.MailboxAssociationCurrentVersion.PropName, NamedPropInfo.Common.MailboxAssociationCurrentVersion);
			propInfos.Add(NamedPropInfo.Common.MailboxAssociationSyncedVersion.PropName, NamedPropInfo.Common.MailboxAssociationSyncedVersion);
			propInfos.Add(NamedPropInfo.Common.MailboxAssociationLastSyncError.PropName, NamedPropInfo.Common.MailboxAssociationLastSyncError);
			propInfos.Add(NamedPropInfo.Common.MailboxAssociationSyncAttempts.PropName, NamedPropInfo.Common.MailboxAssociationSyncAttempts);
			propInfos.Add(NamedPropInfo.Common.MailboxAssociationSyncedSchemaVersion.PropName, NamedPropInfo.Common.MailboxAssociationSyncedSchemaVersion);
			propInfos.Add(NamedPropInfo.Common.MailboxAssociationSyncedIdentityHash.PropName, NamedPropInfo.Common.MailboxAssociationSyncedIdentityHash);
			propInfos.Add(NamedPropInfo.Common.HierarchySyncLastAttemptedSyncTime.PropName, NamedPropInfo.Common.HierarchySyncLastAttemptedSyncTime);
			propInfos.Add(NamedPropInfo.Common.HierarchySyncLastFailedSyncTime.PropName, NamedPropInfo.Common.HierarchySyncLastFailedSyncTime);
			propInfos.Add(NamedPropInfo.Common.HierarchySyncLastSuccessfulSyncTime.PropName, NamedPropInfo.Common.HierarchySyncLastSuccessfulSyncTime);
			propInfos.Add(NamedPropInfo.Common.HierarchySyncFirstFailedSyncTimeAfterLastSuccess.PropName, NamedPropInfo.Common.HierarchySyncFirstFailedSyncTimeAfterLastSuccess);
			propInfos.Add(NamedPropInfo.Common.HierarchySyncLastSyncFailure.PropName, NamedPropInfo.Common.HierarchySyncLastSyncFailure);
			propInfos.Add(NamedPropInfo.Common.HierarchySyncNumberOfAttemptsAfterLastSuccess.PropName, NamedPropInfo.Common.HierarchySyncNumberOfAttemptsAfterLastSuccess);
			propInfos.Add(NamedPropInfo.Common.HierarchySyncNumberOfBatchesExecuted.PropName, NamedPropInfo.Common.HierarchySyncNumberOfBatchesExecuted);
			propInfos.Add(NamedPropInfo.Common.HierarchySyncNumberOfFoldersSynced.PropName, NamedPropInfo.Common.HierarchySyncNumberOfFoldersSynced);
			propInfos.Add(NamedPropInfo.Common.HierarchySyncNumberOfFoldersToBeSynced.PropName, NamedPropInfo.Common.HierarchySyncNumberOfFoldersToBeSynced);
			propInfos.Add(NamedPropInfo.Common.HierarchySyncBatchSize.PropName, NamedPropInfo.Common.HierarchySyncBatchSize);
			propInfos.Add(NamedPropInfo.Common.UserPhotoHR648x648.PropName, NamedPropInfo.Common.UserPhotoHR648x648);
			propInfos.Add(NamedPropInfo.Common.UserPhotoHR504x504.PropName, NamedPropInfo.Common.UserPhotoHR504x504);
			propInfos.Add(NamedPropInfo.Common.UserPhotoHR432x432.PropName, NamedPropInfo.Common.UserPhotoHR432x432);
			propInfos.Add(NamedPropInfo.Common.UserPhotoHR360x360.PropName, NamedPropInfo.Common.UserPhotoHR360x360);
			propInfos.Add(NamedPropInfo.Common.UserPhotoHR240x240.PropName, NamedPropInfo.Common.UserPhotoHR240x240);
			propInfos.Add(NamedPropInfo.Common.UserPhotoHR120x120.PropName, NamedPropInfo.Common.UserPhotoHR120x120);
			propInfos.Add(NamedPropInfo.Common.UserPhotoHR96x96.PropName, NamedPropInfo.Common.UserPhotoHR96x96);
			propInfos.Add(NamedPropInfo.Common.UserPhotoHR64x64.PropName, NamedPropInfo.Common.UserPhotoHR64x64);
			propInfos.Add(NamedPropInfo.Common.UserPhotoHR48x48.PropName, NamedPropInfo.Common.UserPhotoHR48x48);
			propInfos.Add(NamedPropInfo.Common.PeopleHubSortGroupPriority.PropName, NamedPropInfo.Common.PeopleHubSortGroupPriority);
			propInfos.Add(NamedPropInfo.Common.PeopleHubSortGroupPriorityVersion.PropName, NamedPropInfo.Common.PeopleHubSortGroupPriorityVersion);
			propInfos.Add(NamedPropInfo.Common.IsPeopleConnectSyncFolder.PropName, NamedPropInfo.Common.IsPeopleConnectSyncFolder);
			propInfos.Add(NamedPropInfo.Common.TemporarySavesFolderEntryId.PropName, NamedPropInfo.Common.TemporarySavesFolderEntryId);
			propInfos.Add(NamedPropInfo.Common.BirthdayCalendarFolderEntryId.PropName, NamedPropInfo.Common.BirthdayCalendarFolderEntryId);
			propInfos.Add(NamedPropInfo.Common.SnackyAppsFolderEntryId.PropName, NamedPropInfo.Common.SnackyAppsFolderEntryId);
			propInfos.Add(NamedPropInfo.Common.PropertyExistenceTracker.PropName, NamedPropInfo.Common.PropertyExistenceTracker);
			propInfos.Add(NamedPropInfo.Common.MailboxAssociationFolderEntryId.PropName, NamedPropInfo.Common.MailboxAssociationFolderEntryId);
			propInfos.Add(NamedPropInfo.Common.ExchangeApplicationFlags.PropName, NamedPropInfo.Common.ExchangeApplicationFlags);
			propInfos.Add(NamedPropInfo.Common.ItemMovedByRule.PropName, NamedPropInfo.Common.ItemMovedByRule);
			propInfos.Add(NamedPropInfo.Common.ItemMovedByConversationAction.PropName, NamedPropInfo.Common.ItemMovedByConversationAction);
			propInfos.Add(NamedPropInfo.Common.ItemSenderClass.PropName, NamedPropInfo.Common.ItemSenderClass);
			propInfos.Add(NamedPropInfo.Common.ItemCurrentFolderReason.PropName, NamedPropInfo.Common.ItemCurrentFolderReason);
			propInfos.Add(NamedPropInfo.Common.OfficeGraphLocation.PropName, NamedPropInfo.Common.OfficeGraphLocation);
		}

		private static void AddMailNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddLogNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Log.LogType.PropName, NamedPropInfo.Log.LogType);
			propInfos.Add(NamedPropInfo.Log.LogStartDate.PropName, NamedPropInfo.Log.LogStartDate);
			propInfos.Add(NamedPropInfo.Log.LogStartTime.PropName, NamedPropInfo.Log.LogStartTime);
			propInfos.Add(NamedPropInfo.Log.LogStart.PropName, NamedPropInfo.Log.LogStart);
			propInfos.Add(NamedPropInfo.Log.LogDuration.PropName, NamedPropInfo.Log.LogDuration);
			propInfos.Add(NamedPropInfo.Log.LogEnd.PropName, NamedPropInfo.Log.LogEnd);
			propInfos.Add(NamedPropInfo.Log.LogFlags.PropName, NamedPropInfo.Log.LogFlags);
			propInfos.Add(NamedPropInfo.Log.LogContactLog.PropName, NamedPropInfo.Log.LogContactLog);
			propInfos.Add(NamedPropInfo.Log.LogDocPrinted.PropName, NamedPropInfo.Log.LogDocPrinted);
			propInfos.Add(NamedPropInfo.Log.LogDocSaved.PropName, NamedPropInfo.Log.LogDocSaved);
			propInfos.Add(NamedPropInfo.Log.LogDocRouted.PropName, NamedPropInfo.Log.LogDocRouted);
			propInfos.Add(NamedPropInfo.Log.LogDocPosted.PropName, NamedPropInfo.Log.LogDocPosted);
			propInfos.Add(NamedPropInfo.Log.LogTypeDesc.PropName, NamedPropInfo.Log.LogTypeDesc);
		}

		private static void AddTrackingNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Tracking.FHaveWrittenTracking.PropName, NamedPropInfo.Tracking.FHaveWrittenTracking);
			propInfos.Add(NamedPropInfo.Tracking.UnifiedTracking.PropName, NamedPropInfo.Tracking.UnifiedTracking);
		}

		private static void AddMAPIExtraNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddExceptionNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddRenStoreNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddSystemNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddAddrPersonalNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddReportNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Report.ResendTime.PropName, NamedPropInfo.Report.ResendTime);
		}

		private static void AddRemoteNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Remote.RemoteEID.PropName, NamedPropInfo.Remote.RemoteEID);
			propInfos.Add(NamedPropInfo.Remote.RemoteMsgClass.PropName, NamedPropInfo.Remote.RemoteMsgClass);
			propInfos.Add(NamedPropInfo.Remote.RemoteXP.PropName, NamedPropInfo.Remote.RemoteXP);
			propInfos.Add(NamedPropInfo.Remote.RemoteXferTime.PropName, NamedPropInfo.Remote.RemoteXferTime);
			propInfos.Add(NamedPropInfo.Remote.RemoteXferSize.PropName, NamedPropInfo.Remote.RemoteXferSize);
			propInfos.Add(NamedPropInfo.Remote.RemoteSearchKey.PropName, NamedPropInfo.Remote.RemoteSearchKey);
			propInfos.Add(NamedPropInfo.Remote.RemoteAttachment.PropName, NamedPropInfo.Remote.RemoteAttachment);
		}

		private static void AddFATSystemNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddFATCommonNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddFATCustomNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddNewsNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddSharingNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Sharing.SharingStatus.PropName, NamedPropInfo.Sharing.SharingStatus);
			propInfos.Add(NamedPropInfo.Sharing.SharingProviderGuid.PropName, NamedPropInfo.Sharing.SharingProviderGuid);
			propInfos.Add(NamedPropInfo.Sharing.SharingProviderName.PropName, NamedPropInfo.Sharing.SharingProviderName);
			propInfos.Add(NamedPropInfo.Sharing.SharingProviderUrl.PropName, NamedPropInfo.Sharing.SharingProviderUrl);
			propInfos.Add(NamedPropInfo.Sharing.SharingRemotePath.PropName, NamedPropInfo.Sharing.SharingRemotePath);
			propInfos.Add(NamedPropInfo.Sharing.SharingRemoteName.PropName, NamedPropInfo.Sharing.SharingRemoteName);
			propInfos.Add(NamedPropInfo.Sharing.SharingRemoteUid.PropName, NamedPropInfo.Sharing.SharingRemoteUid);
			propInfos.Add(NamedPropInfo.Sharing.SharingInitiatorName.PropName, NamedPropInfo.Sharing.SharingInitiatorName);
			propInfos.Add(NamedPropInfo.Sharing.SharingInitiatorSmtp.PropName, NamedPropInfo.Sharing.SharingInitiatorSmtp);
			propInfos.Add(NamedPropInfo.Sharing.SharingInitiatorEid.PropName, NamedPropInfo.Sharing.SharingInitiatorEid);
			propInfos.Add(NamedPropInfo.Sharing.SharingFlags.PropName, NamedPropInfo.Sharing.SharingFlags);
			propInfos.Add(NamedPropInfo.Sharing.SharingProviderExtension.PropName, NamedPropInfo.Sharing.SharingProviderExtension);
			propInfos.Add(NamedPropInfo.Sharing.SharingRemoteUser.PropName, NamedPropInfo.Sharing.SharingRemoteUser);
			propInfos.Add(NamedPropInfo.Sharing.SharingRemotePass.PropName, NamedPropInfo.Sharing.SharingRemotePass);
			propInfos.Add(NamedPropInfo.Sharing.SharingLocalPath.PropName, NamedPropInfo.Sharing.SharingLocalPath);
			propInfos.Add(NamedPropInfo.Sharing.SharingLocalName.PropName, NamedPropInfo.Sharing.SharingLocalName);
			propInfos.Add(NamedPropInfo.Sharing.SharingLocalUid.PropName, NamedPropInfo.Sharing.SharingLocalUid);
			propInfos.Add(NamedPropInfo.Sharing.SharingFilter.PropName, NamedPropInfo.Sharing.SharingFilter);
			propInfos.Add(NamedPropInfo.Sharing.SharingLocalType.PropName, NamedPropInfo.Sharing.SharingLocalType);
			propInfos.Add(NamedPropInfo.Sharing.SharingFolderEid.PropName, NamedPropInfo.Sharing.SharingFolderEid);
			propInfos.Add(NamedPropInfo.Sharing.SharingCaps.PropName, NamedPropInfo.Sharing.SharingCaps);
			propInfos.Add(NamedPropInfo.Sharing.SharingFlavor.PropName, NamedPropInfo.Sharing.SharingFlavor);
			propInfos.Add(NamedPropInfo.Sharing.SharingAnonymity.PropName, NamedPropInfo.Sharing.SharingAnonymity);
			propInfos.Add(NamedPropInfo.Sharing.SharingReciprocation.PropName, NamedPropInfo.Sharing.SharingReciprocation);
			propInfos.Add(NamedPropInfo.Sharing.SharingPermissions.PropName, NamedPropInfo.Sharing.SharingPermissions);
			propInfos.Add(NamedPropInfo.Sharing.SharingInstanceGuid.PropName, NamedPropInfo.Sharing.SharingInstanceGuid);
			propInfos.Add(NamedPropInfo.Sharing.SharingRemoteType.PropName, NamedPropInfo.Sharing.SharingRemoteType);
			propInfos.Add(NamedPropInfo.Sharing.SharingParticipants.PropName, NamedPropInfo.Sharing.SharingParticipants);
			propInfos.Add(NamedPropInfo.Sharing.SharingLastSync.PropName, NamedPropInfo.Sharing.SharingLastSync);
			propInfos.Add(NamedPropInfo.Sharing.SharingRssHash.PropName, NamedPropInfo.Sharing.SharingRssHash);
			propInfos.Add(NamedPropInfo.Sharing.SharingExtXml.PropName, NamedPropInfo.Sharing.SharingExtXml);
			propInfos.Add(NamedPropInfo.Sharing.SharingRemoteLastMod.PropName, NamedPropInfo.Sharing.SharingRemoteLastMod);
			propInfos.Add(NamedPropInfo.Sharing.SharingLocalLastMod.PropName, NamedPropInfo.Sharing.SharingLocalLastMod);
			propInfos.Add(NamedPropInfo.Sharing.SharingConfigUrl.PropName, NamedPropInfo.Sharing.SharingConfigUrl);
			propInfos.Add(NamedPropInfo.Sharing.SharingStart.PropName, NamedPropInfo.Sharing.SharingStart);
			propInfos.Add(NamedPropInfo.Sharing.SharingStop.PropName, NamedPropInfo.Sharing.SharingStop);
			propInfos.Add(NamedPropInfo.Sharing.SharingResponseType.PropName, NamedPropInfo.Sharing.SharingResponseType);
			propInfos.Add(NamedPropInfo.Sharing.SharingResponseTime.PropName, NamedPropInfo.Sharing.SharingResponseTime);
			propInfos.Add(NamedPropInfo.Sharing.SharingOriginalMessageEid.PropName, NamedPropInfo.Sharing.SharingOriginalMessageEid);
			propInfos.Add(NamedPropInfo.Sharing.SharingSyncInterval.PropName, NamedPropInfo.Sharing.SharingSyncInterval);
			propInfos.Add(NamedPropInfo.Sharing.SharingDetail.PropName, NamedPropInfo.Sharing.SharingDetail);
			propInfos.Add(NamedPropInfo.Sharing.SharingTimeToLive.PropName, NamedPropInfo.Sharing.SharingTimeToLive);
			propInfos.Add(NamedPropInfo.Sharing.SharingBindingEid.PropName, NamedPropInfo.Sharing.SharingBindingEid);
			propInfos.Add(NamedPropInfo.Sharing.SharingIndexEid.PropName, NamedPropInfo.Sharing.SharingIndexEid);
			propInfos.Add(NamedPropInfo.Sharing.SharingRemoteComment.PropName, NamedPropInfo.Sharing.SharingRemoteComment);
			propInfos.Add(NamedPropInfo.Sharing.SharingWssVer.PropName, NamedPropInfo.Sharing.SharingWssVer);
			propInfos.Add(NamedPropInfo.Sharing.SharingWssCmd.PropName, NamedPropInfo.Sharing.SharingWssCmd);
			propInfos.Add(NamedPropInfo.Sharing.SharingWssListRelUrl.PropName, NamedPropInfo.Sharing.SharingWssListRelUrl);
			propInfos.Add(NamedPropInfo.Sharing.SharingWssSiteName.PropName, NamedPropInfo.Sharing.SharingWssSiteName);
			propInfos.Add(NamedPropInfo.Sharing.XSharingConfigUrl.PropName, NamedPropInfo.Sharing.XSharingConfigUrl);
			propInfos.Add(NamedPropInfo.Sharing.XSharingRemotePath.PropName, NamedPropInfo.Sharing.XSharingRemotePath);
			propInfos.Add(NamedPropInfo.Sharing.XSharingRemoteName.PropName, NamedPropInfo.Sharing.XSharingRemoteName);
			propInfos.Add(NamedPropInfo.Sharing.XSharingRemoteUid.PropName, NamedPropInfo.Sharing.XSharingRemoteUid);
			propInfos.Add(NamedPropInfo.Sharing.XSharingRemoteType.PropName, NamedPropInfo.Sharing.XSharingRemoteType);
			propInfos.Add(NamedPropInfo.Sharing.XSharingInstanceGuid.PropName, NamedPropInfo.Sharing.XSharingInstanceGuid);
			propInfos.Add(NamedPropInfo.Sharing.XSharingCapabilities.PropName, NamedPropInfo.Sharing.XSharingCapabilities);
			propInfos.Add(NamedPropInfo.Sharing.XSharingFlavor.PropName, NamedPropInfo.Sharing.XSharingFlavor);
			propInfos.Add(NamedPropInfo.Sharing.XSharingProviderGuid.PropName, NamedPropInfo.Sharing.XSharingProviderGuid);
			propInfos.Add(NamedPropInfo.Sharing.XSharingProviderName.PropName, NamedPropInfo.Sharing.XSharingProviderName);
			propInfos.Add(NamedPropInfo.Sharing.XSharingProviderUrl.PropName, NamedPropInfo.Sharing.XSharingProviderUrl);
			propInfos.Add(NamedPropInfo.Sharing.SharingWorkingHoursStart.PropName, NamedPropInfo.Sharing.SharingWorkingHoursStart);
			propInfos.Add(NamedPropInfo.Sharing.SharingWorkingHoursEnd.PropName, NamedPropInfo.Sharing.SharingWorkingHoursEnd);
			propInfos.Add(NamedPropInfo.Sharing.SharingWorkingHoursDays.PropName, NamedPropInfo.Sharing.SharingWorkingHoursDays);
			propInfos.Add(NamedPropInfo.Sharing.SharingWorkingHoursTZ.PropName, NamedPropInfo.Sharing.SharingWorkingHoursTZ);
			propInfos.Add(NamedPropInfo.Sharing.SharingDataRangeStart.PropName, NamedPropInfo.Sharing.SharingDataRangeStart);
			propInfos.Add(NamedPropInfo.Sharing.SharingDataRangeEnd.PropName, NamedPropInfo.Sharing.SharingDataRangeEnd);
			propInfos.Add(NamedPropInfo.Sharing.SharingRangeStart.PropName, NamedPropInfo.Sharing.SharingRangeStart);
			propInfos.Add(NamedPropInfo.Sharing.SharingRangeEnd.PropName, NamedPropInfo.Sharing.SharingRangeEnd);
			propInfos.Add(NamedPropInfo.Sharing.SharingRemoteStoreUid.PropName, NamedPropInfo.Sharing.SharingRemoteStoreUid);
			propInfos.Add(NamedPropInfo.Sharing.SharingLocalStoreUid.PropName, NamedPropInfo.Sharing.SharingLocalStoreUid);
			propInfos.Add(NamedPropInfo.Sharing.XSharingRemoteStoreUid.PropName, NamedPropInfo.Sharing.XSharingRemoteStoreUid);
			propInfos.Add(NamedPropInfo.Sharing.SharingRemoteByteSize.PropName, NamedPropInfo.Sharing.SharingRemoteByteSize);
			propInfos.Add(NamedPropInfo.Sharing.SharingRemoteCrc.PropName, NamedPropInfo.Sharing.SharingRemoteCrc);
			propInfos.Add(NamedPropInfo.Sharing.SharingLocalComment.PropName, NamedPropInfo.Sharing.SharingLocalComment);
			propInfos.Add(NamedPropInfo.Sharing.SharingRoamLog.PropName, NamedPropInfo.Sharing.SharingRoamLog);
			propInfos.Add(NamedPropInfo.Sharing.SharingRemoteMsgCount.PropName, NamedPropInfo.Sharing.SharingRemoteMsgCount);
			propInfos.Add(NamedPropInfo.Sharing.XSharingLocalType.PropName, NamedPropInfo.Sharing.XSharingLocalType);
			propInfos.Add(NamedPropInfo.Sharing.SharingBrowseUrl.PropName, NamedPropInfo.Sharing.SharingBrowseUrl);
			propInfos.Add(NamedPropInfo.Sharing.XSharingBrowseUrl.PropName, NamedPropInfo.Sharing.XSharingBrowseUrl);
			propInfos.Add(NamedPropInfo.Sharing.SharingLastAutoSync.PropName, NamedPropInfo.Sharing.SharingLastAutoSync);
			propInfos.Add(NamedPropInfo.Sharing.SharingTimeToLiveAuto.PropName, NamedPropInfo.Sharing.SharingTimeToLiveAuto);
			propInfos.Add(NamedPropInfo.Sharing.SharingWssFolderRelUrl.PropName, NamedPropInfo.Sharing.SharingWssFolderRelUrl);
			propInfos.Add(NamedPropInfo.Sharing.SharingWssFileRelUrl.PropName, NamedPropInfo.Sharing.SharingWssFileRelUrl);
			propInfos.Add(NamedPropInfo.Sharing.SharingWssPrevFolderRelUrls.PropName, NamedPropInfo.Sharing.SharingWssPrevFolderRelUrls);
			propInfos.Add(NamedPropInfo.Sharing.SharingWssAlternateUrls.PropName, NamedPropInfo.Sharing.SharingWssAlternateUrls);
			propInfos.Add(NamedPropInfo.Sharing.SharingRemoteVersion.PropName, NamedPropInfo.Sharing.SharingRemoteVersion);
			propInfos.Add(NamedPropInfo.Sharing.SharingParentBindingEid.PropName, NamedPropInfo.Sharing.SharingParentBindingEid);
			propInfos.Add(NamedPropInfo.Sharing.SharingWssCachedSchema.PropName, NamedPropInfo.Sharing.SharingWssCachedSchema);
			propInfos.Add(NamedPropInfo.Sharing.SharingSavedSession.PropName, NamedPropInfo.Sharing.SharingSavedSession);
			propInfos.Add(NamedPropInfo.Sharing.SharingWssServerRelUrl.PropName, NamedPropInfo.Sharing.SharingWssServerRelUrl);
			propInfos.Add(NamedPropInfo.Sharing.SharingSyncFlags.PropName, NamedPropInfo.Sharing.SharingSyncFlags);
			propInfos.Add(NamedPropInfo.Sharing.SharingWssFolderID.PropName, NamedPropInfo.Sharing.SharingWssFolderID);
			propInfos.Add(NamedPropInfo.Sharing.SharingWssAllFolderIDs.PropName, NamedPropInfo.Sharing.SharingWssAllFolderIDs);
			propInfos.Add(NamedPropInfo.Sharing.SharingRemoteEwsId.PropName, NamedPropInfo.Sharing.SharingRemoteEwsId);
			propInfos.Add(NamedPropInfo.Sharing.SharingLocalEwsId.PropName, NamedPropInfo.Sharing.SharingLocalEwsId);
			propInfos.Add(NamedPropInfo.Sharing.SharingDetailedStatus.PropName, NamedPropInfo.Sharing.SharingDetailedStatus);
			propInfos.Add(NamedPropInfo.Sharing.SharingLastSuccessSyncTime.PropName, NamedPropInfo.Sharing.SharingLastSuccessSyncTime);
			propInfos.Add(NamedPropInfo.Sharing.SharingSyncRange.PropName, NamedPropInfo.Sharing.SharingSyncRange);
			propInfos.Add(NamedPropInfo.Sharing.SharingAggregationStatus.PropName, NamedPropInfo.Sharing.SharingAggregationStatus);
			propInfos.Add(NamedPropInfo.Sharing.SharingWlidAuthPolicy.PropName, NamedPropInfo.Sharing.SharingWlidAuthPolicy);
			propInfos.Add(NamedPropInfo.Sharing.SharingWlidUserPuid.PropName, NamedPropInfo.Sharing.SharingWlidUserPuid);
			propInfos.Add(NamedPropInfo.Sharing.SharingWlidAuthToken.PropName, NamedPropInfo.Sharing.SharingWlidAuthToken);
			propInfos.Add(NamedPropInfo.Sharing.SharingWlidAuthTokenExpireTime.PropName, NamedPropInfo.Sharing.SharingWlidAuthTokenExpireTime);
			propInfos.Add(NamedPropInfo.Sharing.SharingMinSyncPollInterval.PropName, NamedPropInfo.Sharing.SharingMinSyncPollInterval);
			propInfos.Add(NamedPropInfo.Sharing.SharingMinSettingPollInterval.PropName, NamedPropInfo.Sharing.SharingMinSettingPollInterval);
			propInfos.Add(NamedPropInfo.Sharing.SharingSyncMultiplier.PropName, NamedPropInfo.Sharing.SharingSyncMultiplier);
			propInfos.Add(NamedPropInfo.Sharing.SharingMaxObjectsInSync.PropName, NamedPropInfo.Sharing.SharingMaxObjectsInSync);
			propInfos.Add(NamedPropInfo.Sharing.SharingMaxNumberOfEmails.PropName, NamedPropInfo.Sharing.SharingMaxNumberOfEmails);
			propInfos.Add(NamedPropInfo.Sharing.SharingMaxNumberOfFolders.PropName, NamedPropInfo.Sharing.SharingMaxNumberOfFolders);
			propInfos.Add(NamedPropInfo.Sharing.SharingMaxAttachments.PropName, NamedPropInfo.Sharing.SharingMaxAttachments);
			propInfos.Add(NamedPropInfo.Sharing.SharingMaxMessageSize.PropName, NamedPropInfo.Sharing.SharingMaxMessageSize);
			propInfos.Add(NamedPropInfo.Sharing.SharingMaxRecipients.PropName, NamedPropInfo.Sharing.SharingMaxRecipients);
			propInfos.Add(NamedPropInfo.Sharing.SharingMigrationState.PropName, NamedPropInfo.Sharing.SharingMigrationState);
			propInfos.Add(NamedPropInfo.Sharing.SharingDiagnostics.PropName, NamedPropInfo.Sharing.SharingDiagnostics);
			propInfos.Add(NamedPropInfo.Sharing.SharingPoisonCallstack.PropName, NamedPropInfo.Sharing.SharingPoisonCallstack);
			propInfos.Add(NamedPropInfo.Sharing.SharingAggregationType.PropName, NamedPropInfo.Sharing.SharingAggregationType);
			propInfos.Add(NamedPropInfo.Sharing.SharingSubscriptionConfiguration.PropName, NamedPropInfo.Sharing.SharingSubscriptionConfiguration);
			propInfos.Add(NamedPropInfo.Sharing.SharingSharingAggregationProtocolVersion.PropName, NamedPropInfo.Sharing.SharingSharingAggregationProtocolVersion);
			propInfos.Add(NamedPropInfo.Sharing.SharingAggregationProtocolName.PropName, NamedPropInfo.Sharing.SharingAggregationProtocolName);
			propInfos.Add(NamedPropInfo.Sharing.SharingSubscriptionName.PropName, NamedPropInfo.Sharing.SharingSubscriptionName);
			propInfos.Add(NamedPropInfo.Sharing.SharingSubscriptions.PropName, NamedPropInfo.Sharing.SharingSubscriptions);
			propInfos.Add(NamedPropInfo.Sharing.SharingAdjustedLastSuccessfulSyncTime.PropName, NamedPropInfo.Sharing.SharingAdjustedLastSuccessfulSyncTime);
			propInfos.Add(NamedPropInfo.Sharing.SharingOutageDetectionDiagnostics.PropName, NamedPropInfo.Sharing.SharingOutageDetectionDiagnostics);
			propInfos.Add(NamedPropInfo.Sharing.SharingEwsUri.PropName, NamedPropInfo.Sharing.SharingEwsUri);
			propInfos.Add(NamedPropInfo.Sharing.SharingRemoteExchangeVersion.PropName, NamedPropInfo.Sharing.SharingRemoteExchangeVersion);
			propInfos.Add(NamedPropInfo.Sharing.SharingRemoteUserDomain.PropName, NamedPropInfo.Sharing.SharingRemoteUserDomain);
			propInfos.Add(NamedPropInfo.Sharing.SharingSendAsState.PropName, NamedPropInfo.Sharing.SharingSendAsState);
			propInfos.Add(NamedPropInfo.Sharing.SharingSendAsSubmissionUrl.PropName, NamedPropInfo.Sharing.SharingSendAsSubmissionUrl);
			propInfos.Add(NamedPropInfo.Sharing.SharingSendAsValidatedEmail.PropName, NamedPropInfo.Sharing.SharingSendAsValidatedEmail);
			propInfos.Add(NamedPropInfo.Sharing.SharingSubscriptionVersion.PropName, NamedPropInfo.Sharing.SharingSubscriptionVersion);
			propInfos.Add(NamedPropInfo.Sharing.SharingSubscriptionCreationType.PropName, NamedPropInfo.Sharing.SharingSubscriptionCreationType);
			propInfos.Add(NamedPropInfo.Sharing.SharingSubscriptionSyncPhase.PropName, NamedPropInfo.Sharing.SharingSubscriptionSyncPhase);
			propInfos.Add(NamedPropInfo.Sharing.SharingSubscriptionExclusionFolders.PropName, NamedPropInfo.Sharing.SharingSubscriptionExclusionFolders);
			propInfos.Add(NamedPropInfo.Sharing.SharingSendAsVerificationEmailState.PropName, NamedPropInfo.Sharing.SharingSendAsVerificationEmailState);
			propInfos.Add(NamedPropInfo.Sharing.SharingSendAsVerificationMessageId.PropName, NamedPropInfo.Sharing.SharingSendAsVerificationMessageId);
			propInfos.Add(NamedPropInfo.Sharing.SharingSendAsVerificationTimestamp.PropName, NamedPropInfo.Sharing.SharingSendAsVerificationTimestamp);
			propInfos.Add(NamedPropInfo.Sharing.SharingSubscriptionEvents.PropName, NamedPropInfo.Sharing.SharingSubscriptionEvents);
			propInfos.Add(NamedPropInfo.Sharing.SharingImapPathPrefix.PropName, NamedPropInfo.Sharing.SharingImapPathPrefix);
			propInfos.Add(NamedPropInfo.Sharing.SharingSubscriptionItemsSynced.PropName, NamedPropInfo.Sharing.SharingSubscriptionItemsSynced);
			propInfos.Add(NamedPropInfo.Sharing.SharingSubscriptionItemsSkipped.PropName, NamedPropInfo.Sharing.SharingSubscriptionItemsSkipped);
			propInfos.Add(NamedPropInfo.Sharing.SharingSubscriptionTotalItemsInSourceMailbox.PropName, NamedPropInfo.Sharing.SharingSubscriptionTotalItemsInSourceMailbox);
			propInfos.Add(NamedPropInfo.Sharing.SharingSubscriptionTotalSizeOfSourceMailbox.PropName, NamedPropInfo.Sharing.SharingSubscriptionTotalSizeOfSourceMailbox);
			propInfos.Add(NamedPropInfo.Sharing.SharingLastSyncNowRequest.PropName, NamedPropInfo.Sharing.SharingLastSyncNowRequest);
			propInfos.Add(NamedPropInfo.Sharing.MigrationJobId.PropName, NamedPropInfo.Sharing.MigrationJobId);
			propInfos.Add(NamedPropInfo.Sharing.MigrationJobItemId.PropName, NamedPropInfo.Sharing.MigrationJobItemId);
			propInfos.Add(NamedPropInfo.Sharing.MigrationJobItemStatus.PropName, NamedPropInfo.Sharing.MigrationJobItemStatus);
			propInfos.Add(NamedPropInfo.Sharing.MigrationJobItemEmailAddress.PropName, NamedPropInfo.Sharing.MigrationJobItemEmailAddress);
			propInfos.Add(NamedPropInfo.Sharing.MigrationJobItemLocalMailboxIdentifier.PropName, NamedPropInfo.Sharing.MigrationJobItemLocalMailboxIdentifier);
			propInfos.Add(NamedPropInfo.Sharing.MigrationJobItemMailboxId.PropName, NamedPropInfo.Sharing.MigrationJobItemMailboxId);
			propInfos.Add(NamedPropInfo.Sharing.MigrationJobItemMailboxLegacyDN.PropName, NamedPropInfo.Sharing.MigrationJobItemMailboxLegacyDN);
			propInfos.Add(NamedPropInfo.Sharing.MigrationJobItemSubscriptionStateLastChecked.PropName, NamedPropInfo.Sharing.MigrationJobItemSubscriptionStateLastChecked);
			propInfos.Add(NamedPropInfo.Sharing.MigrationJobItemMRSId.PropName, NamedPropInfo.Sharing.MigrationJobItemMRSId);
			propInfos.Add(NamedPropInfo.Sharing.MigrationJobStatisticsEnabled.PropName, NamedPropInfo.Sharing.MigrationJobStatisticsEnabled);
			propInfos.Add(NamedPropInfo.Sharing.MigrationJobItemItemsSynced.PropName, NamedPropInfo.Sharing.MigrationJobItemItemsSynced);
			propInfos.Add(NamedPropInfo.Sharing.MigrationJobItemItemsSkipped.PropName, NamedPropInfo.Sharing.MigrationJobItemItemsSkipped);
			propInfos.Add(NamedPropInfo.Sharing.MigrationLastSuccessfulSyncTime.PropName, NamedPropInfo.Sharing.MigrationLastSuccessfulSyncTime);
			propInfos.Add(NamedPropInfo.Sharing.OWANavigationNodeCalendarTypeFromOlderExchange.PropName, NamedPropInfo.Sharing.OWANavigationNodeCalendarTypeFromOlderExchange);
		}

		private static void AddPostRssNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.PostRss.PostRssChannelLink.PropName, NamedPropInfo.PostRss.PostRssChannelLink);
			propInfos.Add(NamedPropInfo.PostRss.PostRssItemLink.PropName, NamedPropInfo.PostRss.PostRssItemLink);
			propInfos.Add(NamedPropInfo.PostRss.PostRssItemHash.PropName, NamedPropInfo.PostRss.PostRssItemHash);
			propInfos.Add(NamedPropInfo.PostRss.PostRssItemGuid.PropName, NamedPropInfo.PostRss.PostRssItemGuid);
			propInfos.Add(NamedPropInfo.PostRss.PostRssChannel.PropName, NamedPropInfo.PostRss.PostRssChannel);
			propInfos.Add(NamedPropInfo.PostRss.PostRssItemXml.PropName, NamedPropInfo.PostRss.PostRssItemXml);
			propInfos.Add(NamedPropInfo.PostRss.PostRssSubscription.PropName, NamedPropInfo.PostRss.PostRssSubscription);
		}

		private static void AddNoteNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Note.NoteColor.PropName, NamedPropInfo.Note.NoteColor);
			propInfos.Add(NamedPropInfo.Note.NoteOnTop.PropName, NamedPropInfo.Note.NoteOnTop);
			propInfos.Add(NamedPropInfo.Note.NoteWidth.PropName, NamedPropInfo.Note.NoteWidth);
			propInfos.Add(NamedPropInfo.Note.NoteHeight.PropName, NamedPropInfo.Note.NoteHeight);
			propInfos.Add(NamedPropInfo.Note.NoteX.PropName, NamedPropInfo.Note.NoteX);
			propInfos.Add(NamedPropInfo.Note.NoteY.PropName, NamedPropInfo.Note.NoteY);
		}

		private static void AddOutlineViewNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddMessageLookupNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddGenericViewNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddInternetHeadersNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.InternetHeaders.AcceptLanguage.PropName, NamedPropInfo.InternetHeaders.AcceptLanguage);
			propInfos.Add(NamedPropInfo.InternetHeaders.Approved.PropName, NamedPropInfo.InternetHeaders.Approved);
			propInfos.Add(NamedPropInfo.InternetHeaders.Bcc.PropName, NamedPropInfo.InternetHeaders.Bcc);
			propInfos.Add(NamedPropInfo.InternetHeaders.Cc.PropName, NamedPropInfo.InternetHeaders.Cc);
			propInfos.Add(NamedPropInfo.InternetHeaders.Comment.PropName, NamedPropInfo.InternetHeaders.Comment);
			propInfos.Add(NamedPropInfo.InternetHeaders.ContentBase.PropName, NamedPropInfo.InternetHeaders.ContentBase);
			propInfos.Add(NamedPropInfo.InternetHeaders.ContentDisposition.PropName, NamedPropInfo.InternetHeaders.ContentDisposition);
			propInfos.Add(NamedPropInfo.InternetHeaders.ContentId.PropName, NamedPropInfo.InternetHeaders.ContentId);
			propInfos.Add(NamedPropInfo.InternetHeaders.ContentLanguage.PropName, NamedPropInfo.InternetHeaders.ContentLanguage);
			propInfos.Add(NamedPropInfo.InternetHeaders.ContentLocation.PropName, NamedPropInfo.InternetHeaders.ContentLocation);
			propInfos.Add(NamedPropInfo.InternetHeaders.ContentTransferEncoding.PropName, NamedPropInfo.InternetHeaders.ContentTransferEncoding);
			propInfos.Add(NamedPropInfo.InternetHeaders.ContentType.PropName, NamedPropInfo.InternetHeaders.ContentType);
			propInfos.Add(NamedPropInfo.InternetHeaders.Control.PropName, NamedPropInfo.InternetHeaders.Control);
			propInfos.Add(NamedPropInfo.InternetHeaders.Disposition.PropName, NamedPropInfo.InternetHeaders.Disposition);
			propInfos.Add(NamedPropInfo.InternetHeaders.DispositionNotificationOptions.PropName, NamedPropInfo.InternetHeaders.DispositionNotificationOptions);
			propInfos.Add(NamedPropInfo.InternetHeaders.DispositionNotificationTo.PropName, NamedPropInfo.InternetHeaders.DispositionNotificationTo);
			propInfos.Add(NamedPropInfo.InternetHeaders.Distribution.PropName, NamedPropInfo.InternetHeaders.Distribution);
			propInfos.Add(NamedPropInfo.InternetHeaders.Expires.PropName, NamedPropInfo.InternetHeaders.Expires);
			propInfos.Add(NamedPropInfo.InternetHeaders.ExpiryDate.PropName, NamedPropInfo.InternetHeaders.ExpiryDate);
			propInfos.Add(NamedPropInfo.InternetHeaders.FollowupTo.PropName, NamedPropInfo.InternetHeaders.FollowupTo);
			propInfos.Add(NamedPropInfo.InternetHeaders.From.PropName, NamedPropInfo.InternetHeaders.From);
			propInfos.Add(NamedPropInfo.InternetHeaders.Importance.PropName, NamedPropInfo.InternetHeaders.Importance);
			propInfos.Add(NamedPropInfo.InternetHeaders.InReplyTo.PropName, NamedPropInfo.InternetHeaders.InReplyTo);
			propInfos.Add(NamedPropInfo.InternetHeaders.Keywords.PropName, NamedPropInfo.InternetHeaders.Keywords);
			propInfos.Add(NamedPropInfo.InternetHeaders.MessageId.PropName, NamedPropInfo.InternetHeaders.MessageId);
			propInfos.Add(NamedPropInfo.InternetHeaders.MimeVersion.PropName, NamedPropInfo.InternetHeaders.MimeVersion);
			propInfos.Add(NamedPropInfo.InternetHeaders.Newsgroups.PropName, NamedPropInfo.InternetHeaders.Newsgroups);
			propInfos.Add(NamedPropInfo.InternetHeaders.NNTPPostingHost.PropName, NamedPropInfo.InternetHeaders.NNTPPostingHost);
			propInfos.Add(NamedPropInfo.InternetHeaders.NNTPPostingUser.PropName, NamedPropInfo.InternetHeaders.NNTPPostingUser);
			propInfos.Add(NamedPropInfo.InternetHeaders.Organization.PropName, NamedPropInfo.InternetHeaders.Organization);
			propInfos.Add(NamedPropInfo.InternetHeaders.OriginalRecipient.PropName, NamedPropInfo.InternetHeaders.OriginalRecipient);
			propInfos.Add(NamedPropInfo.InternetHeaders.Path.PropName, NamedPropInfo.InternetHeaders.Path);
			propInfos.Add(NamedPropInfo.InternetHeaders.PostingVersion.PropName, NamedPropInfo.InternetHeaders.PostingVersion);
			propInfos.Add(NamedPropInfo.InternetHeaders.Priority.PropName, NamedPropInfo.InternetHeaders.Priority);
			propInfos.Add(NamedPropInfo.InternetHeaders.Received.PropName, NamedPropInfo.InternetHeaders.Received);
			propInfos.Add(NamedPropInfo.InternetHeaders.References.PropName, NamedPropInfo.InternetHeaders.References);
			propInfos.Add(NamedPropInfo.InternetHeaders.RelayVersion.PropName, NamedPropInfo.InternetHeaders.RelayVersion);
			propInfos.Add(NamedPropInfo.InternetHeaders.ReplyBy.PropName, NamedPropInfo.InternetHeaders.ReplyBy);
			propInfos.Add(NamedPropInfo.InternetHeaders.ReplyTo.PropName, NamedPropInfo.InternetHeaders.ReplyTo);
			propInfos.Add(NamedPropInfo.InternetHeaders.ReturnPath.PropName, NamedPropInfo.InternetHeaders.ReturnPath);
			propInfos.Add(NamedPropInfo.InternetHeaders.ReturnReceiptTo.PropName, NamedPropInfo.InternetHeaders.ReturnReceiptTo);
			propInfos.Add(NamedPropInfo.InternetHeaders.Sender.PropName, NamedPropInfo.InternetHeaders.Sender);
			propInfos.Add(NamedPropInfo.InternetHeaders.Sensitivity.PropName, NamedPropInfo.InternetHeaders.Sensitivity);
			propInfos.Add(NamedPropInfo.InternetHeaders.Subject.PropName, NamedPropInfo.InternetHeaders.Subject);
			propInfos.Add(NamedPropInfo.InternetHeaders.Summary.PropName, NamedPropInfo.InternetHeaders.Summary);
			propInfos.Add(NamedPropInfo.InternetHeaders.ThreadIndex.PropName, NamedPropInfo.InternetHeaders.ThreadIndex);
			propInfos.Add(NamedPropInfo.InternetHeaders.ThreadTopic.PropName, NamedPropInfo.InternetHeaders.ThreadTopic);
			propInfos.Add(NamedPropInfo.InternetHeaders.To.PropName, NamedPropInfo.InternetHeaders.To);
			propInfos.Add(NamedPropInfo.InternetHeaders.AttachmentOrder.PropName, NamedPropInfo.InternetHeaders.AttachmentOrder);
			propInfos.Add(NamedPropInfo.InternetHeaders.CallId.PropName, NamedPropInfo.InternetHeaders.CallId);
			propInfos.Add(NamedPropInfo.InternetHeaders.CallingTelephone.PropName, NamedPropInfo.InternetHeaders.CallingTelephone);
			propInfos.Add(NamedPropInfo.InternetHeaders.FaxNumberOfPages.PropName, NamedPropInfo.InternetHeaders.FaxNumberOfPages);
			propInfos.Add(NamedPropInfo.InternetHeaders.XListUnsubscribe.PropName, NamedPropInfo.InternetHeaders.XListUnsubscribe);
			propInfos.Add(NamedPropInfo.InternetHeaders.XMailer.PropName, NamedPropInfo.InternetHeaders.XMailer);
			propInfos.Add(NamedPropInfo.InternetHeaders.MessageCompleted.PropName, NamedPropInfo.InternetHeaders.MessageCompleted);
			propInfos.Add(NamedPropInfo.InternetHeaders.XMessageFlag.PropName, NamedPropInfo.InternetHeaders.XMessageFlag);
			propInfos.Add(NamedPropInfo.InternetHeaders.AuthAs.PropName, NamedPropInfo.InternetHeaders.AuthAs);
			propInfos.Add(NamedPropInfo.InternetHeaders.AuthDomain.PropName, NamedPropInfo.InternetHeaders.AuthDomain);
			propInfos.Add(NamedPropInfo.InternetHeaders.AuthMechanism.PropName, NamedPropInfo.InternetHeaders.AuthMechanism);
			propInfos.Add(NamedPropInfo.InternetHeaders.AuthSource.PropName, NamedPropInfo.InternetHeaders.AuthSource);
			propInfos.Add(NamedPropInfo.InternetHeaders.JournalReport.PropName, NamedPropInfo.InternetHeaders.JournalReport);
			propInfos.Add(NamedPropInfo.InternetHeaders.TNEFCorrelator.PropName, NamedPropInfo.InternetHeaders.TNEFCorrelator);
			propInfos.Add(NamedPropInfo.InternetHeaders.Xref.PropName, NamedPropInfo.InternetHeaders.Xref);
			propInfos.Add(NamedPropInfo.InternetHeaders.SharingBrowseUrl.PropName, NamedPropInfo.InternetHeaders.SharingBrowseUrl);
			propInfos.Add(NamedPropInfo.InternetHeaders.SharingCapacilities.PropName, NamedPropInfo.InternetHeaders.SharingCapacilities);
			propInfos.Add(NamedPropInfo.InternetHeaders.SharingConfigUrl.PropName, NamedPropInfo.InternetHeaders.SharingConfigUrl);
			propInfos.Add(NamedPropInfo.InternetHeaders.ExtendedCaps.PropName, NamedPropInfo.InternetHeaders.ExtendedCaps);
			propInfos.Add(NamedPropInfo.InternetHeaders.SharingFlavor.PropName, NamedPropInfo.InternetHeaders.SharingFlavor);
			propInfos.Add(NamedPropInfo.InternetHeaders.SharingInstanceGuid.PropName, NamedPropInfo.InternetHeaders.SharingInstanceGuid);
			propInfos.Add(NamedPropInfo.InternetHeaders.SharingLocalType.PropName, NamedPropInfo.InternetHeaders.SharingLocalType);
			propInfos.Add(NamedPropInfo.InternetHeaders.SharingProviderGuid.PropName, NamedPropInfo.InternetHeaders.SharingProviderGuid);
			propInfos.Add(NamedPropInfo.InternetHeaders.SharingProviderName.PropName, NamedPropInfo.InternetHeaders.SharingProviderName);
			propInfos.Add(NamedPropInfo.InternetHeaders.SharingProviderUrl.PropName, NamedPropInfo.InternetHeaders.SharingProviderUrl);
			propInfos.Add(NamedPropInfo.InternetHeaders.SharingRemoteName.PropName, NamedPropInfo.InternetHeaders.SharingRemoteName);
			propInfos.Add(NamedPropInfo.InternetHeaders.SharingRemotePath.PropName, NamedPropInfo.InternetHeaders.SharingRemotePath);
			propInfos.Add(NamedPropInfo.InternetHeaders.SharingRemoteStoreUid.PropName, NamedPropInfo.InternetHeaders.SharingRemoteStoreUid);
			propInfos.Add(NamedPropInfo.InternetHeaders.SharingRemoteType.PropName, NamedPropInfo.InternetHeaders.SharingRemoteType);
			propInfos.Add(NamedPropInfo.InternetHeaders.SharingRemoteUid.PropName, NamedPropInfo.InternetHeaders.SharingRemoteUid);
			propInfos.Add(NamedPropInfo.InternetHeaders.XUnsent.PropName, NamedPropInfo.InternetHeaders.XUnsent);
			propInfos.Add(NamedPropInfo.InternetHeaders.VoiceMessageDuration.PropName, NamedPropInfo.InternetHeaders.VoiceMessageDuration);
			propInfos.Add(NamedPropInfo.InternetHeaders.VoiceMessageSenderName.PropName, NamedPropInfo.InternetHeaders.VoiceMessageSenderName);
			propInfos.Add(NamedPropInfo.InternetHeaders.MsgSubmitClientIp.PropName, NamedPropInfo.InternetHeaders.MsgSubmitClientIp);
			propInfos.Add(NamedPropInfo.InternetHeaders.XMsExchOrganizationOriginalServerIPAddress.PropName, NamedPropInfo.InternetHeaders.XMsExchOrganizationOriginalServerIPAddress);
			propInfos.Add(NamedPropInfo.InternetHeaders.XRequireProtectedPlayOnPhone.PropName, NamedPropInfo.InternetHeaders.XRequireProtectedPlayOnPhone);
			propInfos.Add(NamedPropInfo.InternetHeaders.ContentClass.PropName, NamedPropInfo.InternetHeaders.ContentClass);
			propInfos.Add(NamedPropInfo.InternetHeaders.Lines.PropName, NamedPropInfo.InternetHeaders.Lines);
			propInfos.Add(NamedPropInfo.InternetHeaders.XMSExchangeOrganizationApprovalAllowedDecisionMakers.PropName, NamedPropInfo.InternetHeaders.XMSExchangeOrganizationApprovalAllowedDecisionMakers);
			propInfos.Add(NamedPropInfo.InternetHeaders.XMSExchangeOrganizationApprovalRequestor.PropName, NamedPropInfo.InternetHeaders.XMSExchangeOrganizationApprovalRequestor);
			propInfos.Add(NamedPropInfo.InternetHeaders.XMSExchangeOrganizationRightsProtectMessage.PropName, NamedPropInfo.InternetHeaders.XMSExchangeOrganizationRightsProtectMessage);
			propInfos.Add(NamedPropInfo.InternetHeaders.XMSHasAttach.PropName, NamedPropInfo.InternetHeaders.XMSHasAttach);
			propInfos.Add(NamedPropInfo.InternetHeaders.XMSExchOrganizationAuthDomain.PropName, NamedPropInfo.InternetHeaders.XMSExchOrganizationAuthDomain);
			propInfos.Add(NamedPropInfo.InternetHeaders.DlpSenderOverride.PropName, NamedPropInfo.InternetHeaders.DlpSenderOverride);
			propInfos.Add(NamedPropInfo.InternetHeaders.DlpFalsePositive.PropName, NamedPropInfo.InternetHeaders.DlpFalsePositive);
			propInfos.Add(NamedPropInfo.InternetHeaders.DlpDetectedClassifications.PropName, NamedPropInfo.InternetHeaders.DlpDetectedClassifications);
		}

		private static void AddExternalSharingNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.ExternalSharing.DataType.PropName, NamedPropInfo.ExternalSharing.DataType);
			propInfos.Add(NamedPropInfo.ExternalSharing.IsPrimary.PropName, NamedPropInfo.ExternalSharing.IsPrimary);
			propInfos.Add(NamedPropInfo.ExternalSharing.LastSuccessfulSyncTime.PropName, NamedPropInfo.ExternalSharing.LastSuccessfulSyncTime);
			propInfos.Add(NamedPropInfo.ExternalSharing.LevelOfDetails.PropName, NamedPropInfo.ExternalSharing.LevelOfDetails);
			propInfos.Add(NamedPropInfo.ExternalSharing.LocalFolderId.PropName, NamedPropInfo.ExternalSharing.LocalFolderId);
			propInfos.Add(NamedPropInfo.ExternalSharing.MasterId.PropName, NamedPropInfo.ExternalSharing.MasterId);
			propInfos.Add(NamedPropInfo.ExternalSharing.RemoteFolderId.PropName, NamedPropInfo.ExternalSharing.RemoteFolderId);
			propInfos.Add(NamedPropInfo.ExternalSharing.RemoteFolderName.PropName, NamedPropInfo.ExternalSharing.RemoteFolderName);
			propInfos.Add(NamedPropInfo.ExternalSharing.SharerIdentity.PropName, NamedPropInfo.ExternalSharing.SharerIdentity);
			propInfos.Add(NamedPropInfo.ExternalSharing.SharerIdentityFederationUri.PropName, NamedPropInfo.ExternalSharing.SharerIdentityFederationUri);
			propInfos.Add(NamedPropInfo.ExternalSharing.SharerName.PropName, NamedPropInfo.ExternalSharing.SharerName);
			propInfos.Add(NamedPropInfo.ExternalSharing.SharingKey.PropName, NamedPropInfo.ExternalSharing.SharingKey);
			propInfos.Add(NamedPropInfo.ExternalSharing.SubscriberIdentity.PropName, NamedPropInfo.ExternalSharing.SubscriberIdentity);
			propInfos.Add(NamedPropInfo.ExternalSharing.SyncState.PropName, NamedPropInfo.ExternalSharing.SyncState);
			propInfos.Add(NamedPropInfo.ExternalSharing.Url.PropName, NamedPropInfo.ExternalSharing.Url);
		}

		private static void AddMeetingNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Meeting.AttendeeCriticalChange.PropName, NamedPropInfo.Meeting.AttendeeCriticalChange);
			propInfos.Add(NamedPropInfo.Meeting.Where.PropName, NamedPropInfo.Meeting.Where);
			propInfos.Add(NamedPropInfo.Meeting.GlobalObjId.PropName, NamedPropInfo.Meeting.GlobalObjId);
			propInfos.Add(NamedPropInfo.Meeting.IsSilent.PropName, NamedPropInfo.Meeting.IsSilent);
			propInfos.Add(NamedPropInfo.Meeting.IsRecurring.PropName, NamedPropInfo.Meeting.IsRecurring);
			propInfos.Add(NamedPropInfo.Meeting.RequiredAttendees.PropName, NamedPropInfo.Meeting.RequiredAttendees);
			propInfos.Add(NamedPropInfo.Meeting.OptionalAttendees.PropName, NamedPropInfo.Meeting.OptionalAttendees);
			propInfos.Add(NamedPropInfo.Meeting.ResourceAttendees.PropName, NamedPropInfo.Meeting.ResourceAttendees);
			propInfos.Add(NamedPropInfo.Meeting.DelegateMail.PropName, NamedPropInfo.Meeting.DelegateMail);
			propInfos.Add(NamedPropInfo.Meeting.IsException.PropName, NamedPropInfo.Meeting.IsException);
			propInfos.Add(NamedPropInfo.Meeting.SingleInvite.PropName, NamedPropInfo.Meeting.SingleInvite);
			propInfos.Add(NamedPropInfo.Meeting.TimeZone.PropName, NamedPropInfo.Meeting.TimeZone);
			propInfos.Add(NamedPropInfo.Meeting.StartRecurDate.PropName, NamedPropInfo.Meeting.StartRecurDate);
			propInfos.Add(NamedPropInfo.Meeting.StartRecurTime.PropName, NamedPropInfo.Meeting.StartRecurTime);
			propInfos.Add(NamedPropInfo.Meeting.EndRecurDate.PropName, NamedPropInfo.Meeting.EndRecurDate);
			propInfos.Add(NamedPropInfo.Meeting.EndRecurTime.PropName, NamedPropInfo.Meeting.EndRecurTime);
			propInfos.Add(NamedPropInfo.Meeting.DayInterval.PropName, NamedPropInfo.Meeting.DayInterval);
			propInfos.Add(NamedPropInfo.Meeting.WeekInterval.PropName, NamedPropInfo.Meeting.WeekInterval);
			propInfos.Add(NamedPropInfo.Meeting.MonthInterval.PropName, NamedPropInfo.Meeting.MonthInterval);
			propInfos.Add(NamedPropInfo.Meeting.YearInterval.PropName, NamedPropInfo.Meeting.YearInterval);
			propInfos.Add(NamedPropInfo.Meeting.DowMask.PropName, NamedPropInfo.Meeting.DowMask);
			propInfos.Add(NamedPropInfo.Meeting.DomMask.PropName, NamedPropInfo.Meeting.DomMask);
			propInfos.Add(NamedPropInfo.Meeting.MoyMask.PropName, NamedPropInfo.Meeting.MoyMask);
			propInfos.Add(NamedPropInfo.Meeting.MtgRecurType.PropName, NamedPropInfo.Meeting.MtgRecurType);
			propInfos.Add(NamedPropInfo.Meeting.DowPref.PropName, NamedPropInfo.Meeting.DowPref);
			propInfos.Add(NamedPropInfo.Meeting.OwnerCriticalChange.PropName, NamedPropInfo.Meeting.OwnerCriticalChange);
			propInfos.Add(NamedPropInfo.Meeting.LID_WANT_SILENT_RESP.PropName, NamedPropInfo.Meeting.LID_WANT_SILENT_RESP);
			propInfos.Add(NamedPropInfo.Meeting.CalendarType.PropName, NamedPropInfo.Meeting.CalendarType);
			propInfos.Add(NamedPropInfo.Meeting.AllAttendeesList.PropName, NamedPropInfo.Meeting.AllAttendeesList);
			propInfos.Add(NamedPropInfo.Meeting.ResponseState.PropName, NamedPropInfo.Meeting.ResponseState);
			propInfos.Add(NamedPropInfo.Meeting.WhenProp.PropName, NamedPropInfo.Meeting.WhenProp);
			propInfos.Add(NamedPropInfo.Meeting.CleanGlobalObjId.PropName, NamedPropInfo.Meeting.CleanGlobalObjId);
			propInfos.Add(NamedPropInfo.Meeting.ApptMessageClass.PropName, NamedPropInfo.Meeting.ApptMessageClass);
			propInfos.Add(NamedPropInfo.Meeting.ProposedWhenProp.PropName, NamedPropInfo.Meeting.ProposedWhenProp);
			propInfos.Add(NamedPropInfo.Meeting.MeetingType.PropName, NamedPropInfo.Meeting.MeetingType);
			propInfos.Add(NamedPropInfo.Meeting.OldWhen.PropName, NamedPropInfo.Meeting.OldWhen);
			propInfos.Add(NamedPropInfo.Meeting.OldLocation.PropName, NamedPropInfo.Meeting.OldLocation);
			propInfos.Add(NamedPropInfo.Meeting.OldWhenStartWhole.PropName, NamedPropInfo.Meeting.OldWhenStartWhole);
			propInfos.Add(NamedPropInfo.Meeting.OldWhenEndWhole.PropName, NamedPropInfo.Meeting.OldWhenEndWhole);
			propInfos.Add(NamedPropInfo.Meeting.ProsedWhenProp.PropName, NamedPropInfo.Meeting.ProsedWhenProp);
		}

		private static void AddIMAPStoreNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.IMAPStore.Acct.PropName, NamedPropInfo.IMAPStore.Acct);
			propInfos.Add(NamedPropInfo.IMAPStore.Prefix.PropName, NamedPropInfo.IMAPStore.Prefix);
			propInfos.Add(NamedPropInfo.IMAPStore.OfflineMsg.PropName, NamedPropInfo.IMAPStore.OfflineMsg);
			propInfos.Add(NamedPropInfo.IMAPStore.OfflineChgNum.PropName, NamedPropInfo.IMAPStore.OfflineChgNum);
			propInfos.Add(NamedPropInfo.IMAPStore.OfflineFldrs.PropName, NamedPropInfo.IMAPStore.OfflineFldrs);
		}

		private static void AddIMAPMsgNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.IMAPMsg.UID.PropName, NamedPropInfo.IMAPMsg.UID);
			propInfos.Add(NamedPropInfo.IMAPMsg.Flags.PropName, NamedPropInfo.IMAPMsg.Flags);
			propInfos.Add(NamedPropInfo.IMAPMsg.State.PropName, NamedPropInfo.IMAPMsg.State);
			propInfos.Add(NamedPropInfo.IMAPMsg.GUID.PropName, NamedPropInfo.IMAPMsg.GUID);
			propInfos.Add(NamedPropInfo.IMAPMsg.OfflineChgs.PropName, NamedPropInfo.IMAPMsg.OfflineChgs);
			propInfos.Add(NamedPropInfo.IMAPMsg.Headers.PropName, NamedPropInfo.IMAPMsg.Headers);
			propInfos.Add(NamedPropInfo.IMAPMsg.ImapMIMEOptions.PropName, NamedPropInfo.IMAPMsg.ImapMIMEOptions);
			propInfos.Add(NamedPropInfo.IMAPMsg.ImapMIMESize.PropName, NamedPropInfo.IMAPMsg.ImapMIMESize);
			propInfos.Add(NamedPropInfo.IMAPMsg.PopMIMEOptions.PropName, NamedPropInfo.IMAPMsg.PopMIMEOptions);
			propInfos.Add(NamedPropInfo.IMAPMsg.PopMIMESize.PropName, NamedPropInfo.IMAPMsg.PopMIMESize);
		}

		private static void AddIMAPFoldNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.IMAPFold.UIDValidity.PropName, NamedPropInfo.IMAPFold.UIDValidity);
			propInfos.Add(NamedPropInfo.IMAPFold.NextUID.PropName, NamedPropInfo.IMAPFold.NextUID);
			propInfos.Add(NamedPropInfo.IMAPFold.Sep.PropName, NamedPropInfo.IMAPFold.Sep);
			propInfos.Add(NamedPropInfo.IMAPFold.Flags.PropName, NamedPropInfo.IMAPFold.Flags);
			propInfos.Add(NamedPropInfo.IMAPFold.PendingAppend.PropName, NamedPropInfo.IMAPFold.PendingAppend);
		}

		private static void AddProxyNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Proxy.ObjType.PropName, NamedPropInfo.Proxy.ObjType);
		}

		private static void AddAirSyncNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.AirSync.ASDeletedCountTotal.PropName, NamedPropInfo.AirSync.ASDeletedCountTotal);
			propInfos.Add(NamedPropInfo.AirSync.ASFilter.PropName, NamedPropInfo.AirSync.ASFilter);
			propInfos.Add(NamedPropInfo.AirSync.ASLastSyncDay.PropName, NamedPropInfo.AirSync.ASLastSyncDay);
			propInfos.Add(NamedPropInfo.AirSync.ASLocalCommitTimeMax.PropName, NamedPropInfo.AirSync.ASLocalCommitTimeMax);
			propInfos.Add(NamedPropInfo.AirSync.ASStoreObjectId.PropName, NamedPropInfo.AirSync.ASStoreObjectId);
			propInfos.Add(NamedPropInfo.AirSync.ASSyncKey.PropName, NamedPropInfo.AirSync.ASSyncKey);
			propInfos.Add(NamedPropInfo.AirSync.ASIMAddress2.PropName, NamedPropInfo.AirSync.ASIMAddress2);
			propInfos.Add(NamedPropInfo.AirSync.ASIMAddress3.PropName, NamedPropInfo.AirSync.ASIMAddress3);
			propInfos.Add(NamedPropInfo.AirSync.ASMMS.PropName, NamedPropInfo.AirSync.ASMMS);
			propInfos.Add(NamedPropInfo.AirSync.ASMaxItems.PropName, NamedPropInfo.AirSync.ASMaxItems);
			propInfos.Add(NamedPropInfo.AirSync.ASConversationMode.PropName, NamedPropInfo.AirSync.ASConversationMode);
			propInfos.Add(NamedPropInfo.AirSync.ASSettingsHash.PropName, NamedPropInfo.AirSync.ASSettingsHash);
			propInfos.Add(NamedPropInfo.AirSync.ASLastSyncTime.PropName, NamedPropInfo.AirSync.ASLastSyncTime);
			propInfos.Add(NamedPropInfo.AirSync.ASClientCategoryList.PropName, NamedPropInfo.AirSync.ASClientCategoryList);
			propInfos.Add(NamedPropInfo.AirSync.ASLastSeenClientIds.PropName, NamedPropInfo.AirSync.ASLastSeenClientIds);
			propInfos.Add(NamedPropInfo.AirSync.ASLastSyncAttemptTime.PropName, NamedPropInfo.AirSync.ASLastSyncAttemptTime);
			propInfos.Add(NamedPropInfo.AirSync.ASLastSyncSuccessTime.PropName, NamedPropInfo.AirSync.ASLastSyncSuccessTime);
			propInfos.Add(NamedPropInfo.AirSync.ASLastSyncUserAgent.PropName, NamedPropInfo.AirSync.ASLastSyncUserAgent);
			propInfos.Add(NamedPropInfo.AirSync.ASLastPingHeartbeatInterval.PropName, NamedPropInfo.AirSync.ASLastPingHeartbeatInterval);
			propInfos.Add(NamedPropInfo.AirSync.ASDeviceBlockedUntil.PropName, NamedPropInfo.AirSync.ASDeviceBlockedUntil);
			propInfos.Add(NamedPropInfo.AirSync.ASDeviceBlockedAt.PropName, NamedPropInfo.AirSync.ASDeviceBlockedAt);
			propInfos.Add(NamedPropInfo.AirSync.ASDeviceBlockedReason.PropName, NamedPropInfo.AirSync.ASDeviceBlockedReason);
		}

		private static void AddUnifiedMessagingNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.UnifiedMessaging.MsExchangeUMPartnerContent.PropName, NamedPropInfo.UnifiedMessaging.MsExchangeUMPartnerContent);
			propInfos.Add(NamedPropInfo.UnifiedMessaging.MsExchangeUMPartnerContext.PropName, NamedPropInfo.UnifiedMessaging.MsExchangeUMPartnerContext);
			propInfos.Add(NamedPropInfo.UnifiedMessaging.MsExchangeUMPartnerStatus.PropName, NamedPropInfo.UnifiedMessaging.MsExchangeUMPartnerStatus);
			propInfos.Add(NamedPropInfo.UnifiedMessaging.MsExchangeUMPartnerAssignedID.PropName, NamedPropInfo.UnifiedMessaging.MsExchangeUMPartnerAssignedID);
			propInfos.Add(NamedPropInfo.UnifiedMessaging.MsExchangeUMDialPlanLanguage.PropName, NamedPropInfo.UnifiedMessaging.MsExchangeUMDialPlanLanguage);
			propInfos.Add(NamedPropInfo.UnifiedMessaging.MsExchangeUMCallerInformedOfAnalysis.PropName, NamedPropInfo.UnifiedMessaging.MsExchangeUMCallerInformedOfAnalysis);
			propInfos.Add(NamedPropInfo.UnifiedMessaging.UnifiedMessagingOptions.PropName, NamedPropInfo.UnifiedMessaging.UnifiedMessagingOptions);
			propInfos.Add(NamedPropInfo.UnifiedMessaging.OfficeCommunicatorOptions.PropName, NamedPropInfo.UnifiedMessaging.OfficeCommunicatorOptions);
			propInfos.Add(NamedPropInfo.UnifiedMessaging.UMAudioNotes.PropName, NamedPropInfo.UnifiedMessaging.UMAudioNotes);
			propInfos.Add(NamedPropInfo.UnifiedMessaging.OutlookProtectionRuleAddinVersion.PropName, NamedPropInfo.UnifiedMessaging.OutlookProtectionRuleAddinVersion);
			propInfos.Add(NamedPropInfo.UnifiedMessaging.OutlookProtectionRuleConfigTimestamp.PropName, NamedPropInfo.UnifiedMessaging.OutlookProtectionRuleConfigTimestamp);
			propInfos.Add(NamedPropInfo.UnifiedMessaging.OutlookProtectionRuleOverridden.PropName, NamedPropInfo.UnifiedMessaging.OutlookProtectionRuleOverridden);
			propInfos.Add(NamedPropInfo.UnifiedMessaging.IsVoiceReminderEnabled.PropName, NamedPropInfo.UnifiedMessaging.IsVoiceReminderEnabled);
			propInfos.Add(NamedPropInfo.UnifiedMessaging.VoiceReminderPhoneNumber.PropName, NamedPropInfo.UnifiedMessaging.VoiceReminderPhoneNumber);
			propInfos.Add(NamedPropInfo.UnifiedMessaging.PstnCallbackTelephoneNumber.PropName, NamedPropInfo.UnifiedMessaging.PstnCallbackTelephoneNumber);
			propInfos.Add(NamedPropInfo.UnifiedMessaging.XMSExchangeUMPartnerContent.PropName, NamedPropInfo.UnifiedMessaging.XMSExchangeUMPartnerContent);
			propInfos.Add(NamedPropInfo.UnifiedMessaging.XMSExchangeUMPartnerStatus.PropName, NamedPropInfo.UnifiedMessaging.XMSExchangeUMPartnerStatus);
		}

		private static void AddElcNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Elc.ElcFolderLocalizedName.PropName, NamedPropInfo.Elc.ElcFolderLocalizedName);
			propInfos.Add(NamedPropInfo.Elc.ElcExplicitPolicyTag.PropName, NamedPropInfo.Elc.ElcExplicitPolicyTag);
			propInfos.Add(NamedPropInfo.Elc.ElcExplicitArchiveTag.PropName, NamedPropInfo.Elc.ElcExplicitArchiveTag);
		}

		private static void AddAttachmentNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Attachment.MacContentType.PropName, NamedPropInfo.Attachment.MacContentType);
			propInfos.Add(NamedPropInfo.Attachment.MacInfo.PropName, NamedPropInfo.Attachment.MacInfo);
			propInfos.Add(NamedPropInfo.Attachment.ProviderEndpointUrl.PropName, NamedPropInfo.Attachment.ProviderEndpointUrl);
			propInfos.Add(NamedPropInfo.Attachment.ProviderType.PropName, NamedPropInfo.Attachment.ProviderType);
		}

		private static void AddCalendarAssistantNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.CalendarAssistant.ViewStartTime.PropName, NamedPropInfo.CalendarAssistant.ViewStartTime);
			propInfos.Add(NamedPropInfo.CalendarAssistant.ViewEndTime.PropName, NamedPropInfo.CalendarAssistant.ViewEndTime);
			propInfos.Add(NamedPropInfo.CalendarAssistant.CalendarFolderVersion.PropName, NamedPropInfo.CalendarAssistant.CalendarFolderVersion);
			propInfos.Add(NamedPropInfo.CalendarAssistant.CharmId.PropName, NamedPropInfo.CalendarAssistant.CharmId);
			propInfos.Add(NamedPropInfo.CalendarAssistant.OccurrencesExceptionalViewProperties.PropName, NamedPropInfo.CalendarAssistant.OccurrencesExceptionalViewProperties);
			propInfos.Add(NamedPropInfo.CalendarAssistant.SeriesSequenceNumber.PropName, NamedPropInfo.CalendarAssistant.SeriesSequenceNumber);
			propInfos.Add(NamedPropInfo.CalendarAssistant.CalendarInteropActionQueueInternal.PropName, NamedPropInfo.CalendarAssistant.CalendarInteropActionQueueInternal);
			propInfos.Add(NamedPropInfo.CalendarAssistant.CalendarInteropActionQueueHasDataInternal.PropName, NamedPropInfo.CalendarAssistant.CalendarInteropActionQueueHasDataInternal);
			propInfos.Add(NamedPropInfo.CalendarAssistant.LastExecutedCalendarInteropAction.PropName, NamedPropInfo.CalendarAssistant.LastExecutedCalendarInteropAction);
			propInfos.Add(NamedPropInfo.CalendarAssistant.PropertyChangeMetadataProcessingFlags.PropName, NamedPropInfo.CalendarAssistant.PropertyChangeMetadataProcessingFlags);
			propInfos.Add(NamedPropInfo.CalendarAssistant.MasterGlobalObjectId.PropName, NamedPropInfo.CalendarAssistant.MasterGlobalObjectId);
			propInfos.Add(NamedPropInfo.CalendarAssistant.ParkedCorrelationId.PropName, NamedPropInfo.CalendarAssistant.ParkedCorrelationId);
			propInfos.Add(NamedPropInfo.CalendarAssistant.CalendarProcessed.PropName, NamedPropInfo.CalendarAssistant.CalendarProcessed);
			propInfos.Add(NamedPropInfo.CalendarAssistant.CalendarLastChangeAction.PropName, NamedPropInfo.CalendarAssistant.CalendarLastChangeAction);
			propInfos.Add(NamedPropInfo.CalendarAssistant.ChangeList.PropName, NamedPropInfo.CalendarAssistant.ChangeList);
			propInfos.Add(NamedPropInfo.CalendarAssistant.CalendarLogTriggerAction.PropName, NamedPropInfo.CalendarAssistant.CalendarLogTriggerAction);
			propInfos.Add(NamedPropInfo.CalendarAssistant.OriginalFolderId.PropName, NamedPropInfo.CalendarAssistant.OriginalFolderId);
			propInfos.Add(NamedPropInfo.CalendarAssistant.OriginalCreationTime.PropName, NamedPropInfo.CalendarAssistant.OriginalCreationTime);
			propInfos.Add(NamedPropInfo.CalendarAssistant.OriginalLastModifiedTime.PropName, NamedPropInfo.CalendarAssistant.OriginalLastModifiedTime);
			propInfos.Add(NamedPropInfo.CalendarAssistant.ResponsibleUserName.PropName, NamedPropInfo.CalendarAssistant.ResponsibleUserName);
			propInfos.Add(NamedPropInfo.CalendarAssistant.ClientInfoString.PropName, NamedPropInfo.CalendarAssistant.ClientInfoString);
			propInfos.Add(NamedPropInfo.CalendarAssistant.ClientProcessName.PropName, NamedPropInfo.CalendarAssistant.ClientProcessName);
			propInfos.Add(NamedPropInfo.CalendarAssistant.ClientMachineName.PropName, NamedPropInfo.CalendarAssistant.ClientMachineName);
			propInfos.Add(NamedPropInfo.CalendarAssistant.ClientBuildVersion.PropName, NamedPropInfo.CalendarAssistant.ClientBuildVersion);
			propInfos.Add(NamedPropInfo.CalendarAssistant.MiddleTierProcessName.PropName, NamedPropInfo.CalendarAssistant.MiddleTierProcessName);
			propInfos.Add(NamedPropInfo.CalendarAssistant.MiddleTierServerName.PropName, NamedPropInfo.CalendarAssistant.MiddleTierServerName);
			propInfos.Add(NamedPropInfo.CalendarAssistant.MiddleTierServerBuildVersion.PropName, NamedPropInfo.CalendarAssistant.MiddleTierServerBuildVersion);
			propInfos.Add(NamedPropInfo.CalendarAssistant.ServerName.PropName, NamedPropInfo.CalendarAssistant.ServerName);
			propInfos.Add(NamedPropInfo.CalendarAssistant.ServerBuildVersion.PropName, NamedPropInfo.CalendarAssistant.ServerBuildVersion);
			propInfos.Add(NamedPropInfo.CalendarAssistant.MailboxDatabaseName.PropName, NamedPropInfo.CalendarAssistant.MailboxDatabaseName);
			propInfos.Add(NamedPropInfo.CalendarAssistant.ClientIntent.PropName, NamedPropInfo.CalendarAssistant.ClientIntent);
			propInfos.Add(NamedPropInfo.CalendarAssistant.ItemVersion.PropName, NamedPropInfo.CalendarAssistant.ItemVersion);
			propInfos.Add(NamedPropInfo.CalendarAssistant.OriginalEntryId.PropName, NamedPropInfo.CalendarAssistant.OriginalEntryId);
			propInfos.Add(NamedPropInfo.CalendarAssistant.ParkedMessagesFolderEntryId.PropName, NamedPropInfo.CalendarAssistant.ParkedMessagesFolderEntryId);
		}

		private static void AddInboxFolderLazyStreamNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.InboxFolderLazyStream.AccociatedMessageEID.PropName, NamedPropInfo.InboxFolderLazyStream.AccociatedMessageEID);
		}

		private static void AddMessagingNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Messaging.DEPRECATED_IsGroupEscalationMessage.PropName, NamedPropInfo.Messaging.DEPRECATED_IsGroupEscalationMessage);
			propInfos.Add(NamedPropInfo.Messaging.AggregationSyncProgress.PropName, NamedPropInfo.Messaging.AggregationSyncProgress);
			propInfos.Add(NamedPropInfo.Messaging.TextMessagingDeliveryStatus.PropName, NamedPropInfo.Messaging.TextMessagingDeliveryStatus);
			propInfos.Add(NamedPropInfo.Messaging.CloudId.PropName, NamedPropInfo.Messaging.CloudId);
			propInfos.Add(NamedPropInfo.Messaging.CloudVersion.PropName, NamedPropInfo.Messaging.CloudVersion);
			propInfos.Add(NamedPropInfo.Messaging.OriginalSentTimeForEscalation.PropName, NamedPropInfo.Messaging.OriginalSentTimeForEscalation);
			propInfos.Add(NamedPropInfo.Messaging.MessageBccMe.PropName, NamedPropInfo.Messaging.MessageBccMe);
			propInfos.Add(NamedPropInfo.Messaging.Likers.PropName, NamedPropInfo.Messaging.Likers);
			propInfos.Add(NamedPropInfo.Messaging.DlpDetectedClassificationObjects.PropName, NamedPropInfo.Messaging.DlpDetectedClassificationObjects);
			propInfos.Add(NamedPropInfo.Messaging.HasDlpDetectedClassifications.PropName, NamedPropInfo.Messaging.HasDlpDetectedClassifications);
			propInfos.Add(NamedPropInfo.Messaging.RecoveryOptions.PropName, NamedPropInfo.Messaging.RecoveryOptions);
		}

		private static void AddStorageNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Storage.X_0009.PropName, NamedPropInfo.Storage.X_0009);
			propInfos.Add(NamedPropInfo.Storage.PID_STG_NAME.PropName, NamedPropInfo.Storage.PID_STG_NAME);
		}

		private static void AddIConverterSessionNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.IConverterSession.Body.PropName, NamedPropInfo.IConverterSession.Body);
			propInfos.Add(NamedPropInfo.IConverterSession.PartialMessageId.PropName, NamedPropInfo.IConverterSession.PartialMessageId);
			propInfos.Add(NamedPropInfo.IConverterSession.PartialMessageNumber.PropName, NamedPropInfo.IConverterSession.PartialMessageNumber);
			propInfos.Add(NamedPropInfo.IConverterSession.PartialMessageTotal.PropName, NamedPropInfo.IConverterSession.PartialMessageTotal);
		}

		private static void AddPkmLinkInformationNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.PkmLinkInformation.AHref.PropName, NamedPropInfo.PkmLinkInformation.AHref);
			propInfos.Add(NamedPropInfo.PkmLinkInformation.AppletCode.PropName, NamedPropInfo.PkmLinkInformation.AppletCode);
			propInfos.Add(NamedPropInfo.PkmLinkInformation.AppletCodeBase.PropName, NamedPropInfo.PkmLinkInformation.AppletCodeBase);
			propInfos.Add(NamedPropInfo.PkmLinkInformation.AreaHref.PropName, NamedPropInfo.PkmLinkInformation.AreaHref);
			propInfos.Add(NamedPropInfo.PkmLinkInformation.BaseHref.PropName, NamedPropInfo.PkmLinkInformation.BaseHref);
			propInfos.Add(NamedPropInfo.PkmLinkInformation.BGSoundSrc.PropName, NamedPropInfo.PkmLinkInformation.BGSoundSrc);
			propInfos.Add(NamedPropInfo.PkmLinkInformation.BodyBgnd.PropName, NamedPropInfo.PkmLinkInformation.BodyBgnd);
			propInfos.Add(NamedPropInfo.PkmLinkInformation.EmbedSrc.PropName, NamedPropInfo.PkmLinkInformation.EmbedSrc);
			propInfos.Add(NamedPropInfo.PkmLinkInformation.FrameSrc.PropName, NamedPropInfo.PkmLinkInformation.FrameSrc);
			propInfos.Add(NamedPropInfo.PkmLinkInformation.IframeSrc.PropName, NamedPropInfo.PkmLinkInformation.IframeSrc);
			propInfos.Add(NamedPropInfo.PkmLinkInformation.ImgDynSrc.PropName, NamedPropInfo.PkmLinkInformation.ImgDynSrc);
			propInfos.Add(NamedPropInfo.PkmLinkInformation.ImgSrc.PropName, NamedPropInfo.PkmLinkInformation.ImgSrc);
			propInfos.Add(NamedPropInfo.PkmLinkInformation.ImgUseMap.PropName, NamedPropInfo.PkmLinkInformation.ImgUseMap);
			propInfos.Add(NamedPropInfo.PkmLinkInformation.LinkHref.PropName, NamedPropInfo.PkmLinkInformation.LinkHref);
			propInfos.Add(NamedPropInfo.PkmLinkInformation.MetaUrl.PropName, NamedPropInfo.PkmLinkInformation.MetaUrl);
			propInfos.Add(NamedPropInfo.PkmLinkInformation.ObjectCodeBase.PropName, NamedPropInfo.PkmLinkInformation.ObjectCodeBase);
			propInfos.Add(NamedPropInfo.PkmLinkInformation.ObjectName.PropName, NamedPropInfo.PkmLinkInformation.ObjectName);
			propInfos.Add(NamedPropInfo.PkmLinkInformation.ObjectUseMap.PropName, NamedPropInfo.PkmLinkInformation.ObjectUseMap);
		}

		private static void AddPkmHTMLInformationNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.AHref.PropName, NamedPropInfo.PkmHTMLInformation.AHref);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.AppletCode.PropName, NamedPropInfo.PkmHTMLInformation.AppletCode);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.AppletCodeBase.PropName, NamedPropInfo.PkmHTMLInformation.AppletCodeBase);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.AreaHref.PropName, NamedPropInfo.PkmHTMLInformation.AreaHref);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.BaseHref.PropName, NamedPropInfo.PkmHTMLInformation.BaseHref);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.BGSoundSrc.PropName, NamedPropInfo.PkmHTMLInformation.BGSoundSrc);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.BodyBgnd.PropName, NamedPropInfo.PkmHTMLInformation.BodyBgnd);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.EmbedSrc.PropName, NamedPropInfo.PkmHTMLInformation.EmbedSrc);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.FrameSrc.PropName, NamedPropInfo.PkmHTMLInformation.FrameSrc);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.IframeSrc.PropName, NamedPropInfo.PkmHTMLInformation.IframeSrc);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.ImgDynSrc.PropName, NamedPropInfo.PkmHTMLInformation.ImgDynSrc);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.ImgSrc.PropName, NamedPropInfo.PkmHTMLInformation.ImgSrc);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.ImgUseMap.PropName, NamedPropInfo.PkmHTMLInformation.ImgUseMap);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.LinkHref.PropName, NamedPropInfo.PkmHTMLInformation.LinkHref);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.MetaUrl.PropName, NamedPropInfo.PkmHTMLInformation.MetaUrl);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.ObjectCodeBase.PropName, NamedPropInfo.PkmHTMLInformation.ObjectCodeBase);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.ObjectName.PropName, NamedPropInfo.PkmHTMLInformation.ObjectName);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.ObjectUseMap.PropName, NamedPropInfo.PkmHTMLInformation.ObjectUseMap);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.ImgAlt.PropName, NamedPropInfo.PkmHTMLInformation.ImgAlt);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.X_0003.PropName, NamedPropInfo.PkmHTMLInformation.X_0003);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.X_0004.PropName, NamedPropInfo.PkmHTMLInformation.X_0004);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.X_0005.PropName, NamedPropInfo.PkmHTMLInformation.X_0005);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.X_0006.PropName, NamedPropInfo.PkmHTMLInformation.X_0006);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.X_0007.PropName, NamedPropInfo.PkmHTMLInformation.X_0007);
			propInfos.Add(NamedPropInfo.PkmHTMLInformation.X_0008.PropName, NamedPropInfo.PkmHTMLInformation.X_0008);
		}

		private static void AddPkmMetaInformationNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.PkmMetaInformation.Product.PropName, NamedPropInfo.PkmMetaInformation.Product);
			propInfos.Add(NamedPropInfo.PkmMetaInformation.Topic.PropName, NamedPropInfo.PkmMetaInformation.Topic);
		}

		private static void AddPkmScriptInfoNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.PkmScriptInfo.JavaScript.PropName, NamedPropInfo.PkmScriptInfo.JavaScript);
			propInfos.Add(NamedPropInfo.PkmScriptInfo.VBScript.PropName, NamedPropInfo.PkmScriptInfo.VBScript);
		}

		private static void AddPkmCharacterizationNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.PkmCharacterization.X_0002.PropName, NamedPropInfo.PkmCharacterization.X_0002);
		}

		private static void AddPkmDocSummaryInformationNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.PkmDocSummaryInformation.X_0002.PropName, NamedPropInfo.PkmDocSummaryInformation.X_0002);
			propInfos.Add(NamedPropInfo.PkmDocSummaryInformation.X_0003.PropName, NamedPropInfo.PkmDocSummaryInformation.X_0003);
			propInfos.Add(NamedPropInfo.PkmDocSummaryInformation.X_0004.PropName, NamedPropInfo.PkmDocSummaryInformation.X_0004);
			propInfos.Add(NamedPropInfo.PkmDocSummaryInformation.X_0005.PropName, NamedPropInfo.PkmDocSummaryInformation.X_0005);
			propInfos.Add(NamedPropInfo.PkmDocSummaryInformation.X_0006.PropName, NamedPropInfo.PkmDocSummaryInformation.X_0006);
			propInfos.Add(NamedPropInfo.PkmDocSummaryInformation.X_0007.PropName, NamedPropInfo.PkmDocSummaryInformation.X_0007);
			propInfos.Add(NamedPropInfo.PkmDocSummaryInformation.X_0008.PropName, NamedPropInfo.PkmDocSummaryInformation.X_0008);
			propInfos.Add(NamedPropInfo.PkmDocSummaryInformation.X_0009.PropName, NamedPropInfo.PkmDocSummaryInformation.X_0009);
			propInfos.Add(NamedPropInfo.PkmDocSummaryInformation.X_000A.PropName, NamedPropInfo.PkmDocSummaryInformation.X_000A);
			propInfos.Add(NamedPropInfo.PkmDocSummaryInformation.X_000B.PropName, NamedPropInfo.PkmDocSummaryInformation.X_000B);
			propInfos.Add(NamedPropInfo.PkmDocSummaryInformation.X_000C.PropName, NamedPropInfo.PkmDocSummaryInformation.X_000C);
			propInfos.Add(NamedPropInfo.PkmDocSummaryInformation.X_000D.PropName, NamedPropInfo.PkmDocSummaryInformation.X_000D);
			propInfos.Add(NamedPropInfo.PkmDocSummaryInformation.X_000E.PropName, NamedPropInfo.PkmDocSummaryInformation.X_000E);
			propInfos.Add(NamedPropInfo.PkmDocSummaryInformation.X_000F.PropName, NamedPropInfo.PkmDocSummaryInformation.X_000F);
			propInfos.Add(NamedPropInfo.PkmDocSummaryInformation.X_0010.PropName, NamedPropInfo.PkmDocSummaryInformation.X_0010);
			propInfos.Add(NamedPropInfo.PkmDocSummaryInformation.X_0011.PropName, NamedPropInfo.PkmDocSummaryInformation.X_0011);
		}

		private static void AddPkmGathererNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.PkmGatherer.X_0004.PropName, NamedPropInfo.PkmGatherer.X_0004);
		}

		private static void AddPkmIndexServerQueryNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.PkmIndexServerQuery.X_0002.PropName, NamedPropInfo.PkmIndexServerQuery.X_0002);
			propInfos.Add(NamedPropInfo.PkmIndexServerQuery.X_0004.PropName, NamedPropInfo.PkmIndexServerQuery.X_0004);
			propInfos.Add(NamedPropInfo.PkmIndexServerQuery.X_0005.PropName, NamedPropInfo.PkmIndexServerQuery.X_0005);
			propInfos.Add(NamedPropInfo.PkmIndexServerQuery.X_0006.PropName, NamedPropInfo.PkmIndexServerQuery.X_0006);
			propInfos.Add(NamedPropInfo.PkmIndexServerQuery.X_0007.PropName, NamedPropInfo.PkmIndexServerQuery.X_0007);
			propInfos.Add(NamedPropInfo.PkmIndexServerQuery.X_0009.PropName, NamedPropInfo.PkmIndexServerQuery.X_0009);
			propInfos.Add(NamedPropInfo.PkmIndexServerQuery.X_000A.PropName, NamedPropInfo.PkmIndexServerQuery.X_000A);
			propInfos.Add(NamedPropInfo.PkmIndexServerQuery.X_000B.PropName, NamedPropInfo.PkmIndexServerQuery.X_000B);
		}

		private static void AddPkmNetLibraryInfoNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.PkmNetLibraryInfo.X_0003.PropName, NamedPropInfo.PkmNetLibraryInfo.X_0003);
		}

		private static void AddPkmSummaryInformationNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.PkmSummaryInformation.X_0002.PropName, NamedPropInfo.PkmSummaryInformation.X_0002);
			propInfos.Add(NamedPropInfo.PkmSummaryInformation.X_0003.PropName, NamedPropInfo.PkmSummaryInformation.X_0003);
			propInfos.Add(NamedPropInfo.PkmSummaryInformation.X_0004.PropName, NamedPropInfo.PkmSummaryInformation.X_0004);
			propInfos.Add(NamedPropInfo.PkmSummaryInformation.X_0005.PropName, NamedPropInfo.PkmSummaryInformation.X_0005);
			propInfos.Add(NamedPropInfo.PkmSummaryInformation.X_0006.PropName, NamedPropInfo.PkmSummaryInformation.X_0006);
			propInfos.Add(NamedPropInfo.PkmSummaryInformation.X_0007.PropName, NamedPropInfo.PkmSummaryInformation.X_0007);
			propInfos.Add(NamedPropInfo.PkmSummaryInformation.X_0008.PropName, NamedPropInfo.PkmSummaryInformation.X_0008);
			propInfos.Add(NamedPropInfo.PkmSummaryInformation.X_0009.PropName, NamedPropInfo.PkmSummaryInformation.X_0009);
			propInfos.Add(NamedPropInfo.PkmSummaryInformation.X_000A.PropName, NamedPropInfo.PkmSummaryInformation.X_000A);
			propInfos.Add(NamedPropInfo.PkmSummaryInformation.X_000B.PropName, NamedPropInfo.PkmSummaryInformation.X_000B);
			propInfos.Add(NamedPropInfo.PkmSummaryInformation.X_000C.PropName, NamedPropInfo.PkmSummaryInformation.X_000C);
			propInfos.Add(NamedPropInfo.PkmSummaryInformation.X_000D.PropName, NamedPropInfo.PkmSummaryInformation.X_000D);
			propInfos.Add(NamedPropInfo.PkmSummaryInformation.X_000E.PropName, NamedPropInfo.PkmSummaryInformation.X_000E);
			propInfos.Add(NamedPropInfo.PkmSummaryInformation.X_000F.PropName, NamedPropInfo.PkmSummaryInformation.X_000F);
			propInfos.Add(NamedPropInfo.PkmSummaryInformation.X_0010.PropName, NamedPropInfo.PkmSummaryInformation.X_0010);
			propInfos.Add(NamedPropInfo.PkmSummaryInformation.X_0011.PropName, NamedPropInfo.PkmSummaryInformation.X_0011);
			propInfos.Add(NamedPropInfo.PkmSummaryInformation.X_0012.PropName, NamedPropInfo.PkmSummaryInformation.X_0012);
			propInfos.Add(NamedPropInfo.PkmSummaryInformation.X_0013.PropName, NamedPropInfo.PkmSummaryInformation.X_0013);
		}

		private static void AddLocationNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Location.LocationRelevanceRank.PropName, NamedPropInfo.Location.LocationRelevanceRank);
			propInfos.Add(NamedPropInfo.Location.LocationDisplayName.PropName, NamedPropInfo.Location.LocationDisplayName);
			propInfos.Add(NamedPropInfo.Location.LocationAnnotation.PropName, NamedPropInfo.Location.LocationAnnotation);
			propInfos.Add(NamedPropInfo.Location.LocationType.PropName, NamedPropInfo.Location.LocationType);
			propInfos.Add(NamedPropInfo.Location.LocationSource.PropName, NamedPropInfo.Location.LocationSource);
			propInfos.Add(NamedPropInfo.Location.LocationUri.PropName, NamedPropInfo.Location.LocationUri);
			propInfos.Add(NamedPropInfo.Location.Latitude.PropName, NamedPropInfo.Location.Latitude);
			propInfos.Add(NamedPropInfo.Location.Longitude.PropName, NamedPropInfo.Location.Longitude);
			propInfos.Add(NamedPropInfo.Location.Accuracy.PropName, NamedPropInfo.Location.Accuracy);
			propInfos.Add(NamedPropInfo.Location.Altitude.PropName, NamedPropInfo.Location.Altitude);
			propInfos.Add(NamedPropInfo.Location.AltitudeAccuracy.PropName, NamedPropInfo.Location.AltitudeAccuracy);
			propInfos.Add(NamedPropInfo.Location.StreetAddress.PropName, NamedPropInfo.Location.StreetAddress);
			propInfos.Add(NamedPropInfo.Location.LocationCity.PropName, NamedPropInfo.Location.LocationCity);
			propInfos.Add(NamedPropInfo.Location.LocationState.PropName, NamedPropInfo.Location.LocationState);
			propInfos.Add(NamedPropInfo.Location.LocationCountry.PropName, NamedPropInfo.Location.LocationCountry);
			propInfos.Add(NamedPropInfo.Location.LocationPostalCode.PropName, NamedPropInfo.Location.LocationPostalCode);
			propInfos.Add(NamedPropInfo.Location.WorkLatitude.PropName, NamedPropInfo.Location.WorkLatitude);
			propInfos.Add(NamedPropInfo.Location.WorkLongitude.PropName, NamedPropInfo.Location.WorkLongitude);
			propInfos.Add(NamedPropInfo.Location.WorkAccuracy.PropName, NamedPropInfo.Location.WorkAccuracy);
			propInfos.Add(NamedPropInfo.Location.WorkAltitude.PropName, NamedPropInfo.Location.WorkAltitude);
			propInfos.Add(NamedPropInfo.Location.WorkAltitudeAccuracy.PropName, NamedPropInfo.Location.WorkAltitudeAccuracy);
			propInfos.Add(NamedPropInfo.Location.WorkLocationSource.PropName, NamedPropInfo.Location.WorkLocationSource);
			propInfos.Add(NamedPropInfo.Location.WorkLocationUri.PropName, NamedPropInfo.Location.WorkLocationUri);
			propInfos.Add(NamedPropInfo.Location.HomeLatitude.PropName, NamedPropInfo.Location.HomeLatitude);
			propInfos.Add(NamedPropInfo.Location.HomeLongitude.PropName, NamedPropInfo.Location.HomeLongitude);
			propInfos.Add(NamedPropInfo.Location.HomeAccuracy.PropName, NamedPropInfo.Location.HomeAccuracy);
			propInfos.Add(NamedPropInfo.Location.HomeAltitude.PropName, NamedPropInfo.Location.HomeAltitude);
			propInfos.Add(NamedPropInfo.Location.HomeAltitudeAccuracy.PropName, NamedPropInfo.Location.HomeAltitudeAccuracy);
			propInfos.Add(NamedPropInfo.Location.HomeLocationSource.PropName, NamedPropInfo.Location.HomeLocationSource);
			propInfos.Add(NamedPropInfo.Location.HomeLocationUri.PropName, NamedPropInfo.Location.HomeLocationUri);
			propInfos.Add(NamedPropInfo.Location.OtherLatitude.PropName, NamedPropInfo.Location.OtherLatitude);
			propInfos.Add(NamedPropInfo.Location.OtherLongitude.PropName, NamedPropInfo.Location.OtherLongitude);
			propInfos.Add(NamedPropInfo.Location.OtherAccuracy.PropName, NamedPropInfo.Location.OtherAccuracy);
			propInfos.Add(NamedPropInfo.Location.OtherAltitude.PropName, NamedPropInfo.Location.OtherAltitude);
			propInfos.Add(NamedPropInfo.Location.OtherAltitudeAccuracy.PropName, NamedPropInfo.Location.OtherAltitudeAccuracy);
			propInfos.Add(NamedPropInfo.Location.OtherLocationSource.PropName, NamedPropInfo.Location.OtherLocationSource);
			propInfos.Add(NamedPropInfo.Location.OtherLocationUri.PropName, NamedPropInfo.Location.OtherLocationUri);
		}

		private static void AddSearchNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Search.X_0004.PropName, NamedPropInfo.Search.X_0004);
			propInfos.Add(NamedPropInfo.Search.X_0005.PropName, NamedPropInfo.Search.X_0005);
			propInfos.Add(NamedPropInfo.Search.X_0006.PropName, NamedPropInfo.Search.X_0006);
			propInfos.Add(NamedPropInfo.Search.X_0007.PropName, NamedPropInfo.Search.X_0007);
			propInfos.Add(NamedPropInfo.Search.X_000A.PropName, NamedPropInfo.Search.X_000A);
			propInfos.Add(NamedPropInfo.Search.X_000B.PropName, NamedPropInfo.Search.X_000B);
			propInfos.Add(NamedPropInfo.Search.X_0014.PropName, NamedPropInfo.Search.X_0014);
		}

		private static void AddConversationsNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Conversations.ConversationIndexTracking.PropName, NamedPropInfo.Conversations.ConversationIndexTracking);
			propInfos.Add(NamedPropInfo.Conversations.ConversationIndexTrackingEx.PropName, NamedPropInfo.Conversations.ConversationIndexTrackingEx);
		}

		private static void AddDAVNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.DAV.X_A100.PropName, NamedPropInfo.DAV.X_A100);
			propInfos.Add(NamedPropInfo.DAV.X_A101.PropName, NamedPropInfo.DAV.X_A101);
			propInfos.Add(NamedPropInfo.DAV.X_A103.PropName, NamedPropInfo.DAV.X_A103);
			propInfos.Add(NamedPropInfo.DAV.X_A104.PropName, NamedPropInfo.DAV.X_A104);
		}

		private static void AddDrmNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddPushNotificationSubscriptionNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.PushNotificationSubscription.PushNotificationFolderEntryId.PropName, NamedPropInfo.PushNotificationSubscription.PushNotificationFolderEntryId);
			propInfos.Add(NamedPropInfo.PushNotificationSubscription.PushNotificationSubscriptionId.PropName, NamedPropInfo.PushNotificationSubscription.PushNotificationSubscriptionId);
			propInfos.Add(NamedPropInfo.PushNotificationSubscription.PushNotificationSubscriptionLastUpdateTimeUTC.PropName, NamedPropInfo.PushNotificationSubscription.PushNotificationSubscriptionLastUpdateTimeUTC);
			propInfos.Add(NamedPropInfo.PushNotificationSubscription.SerializedPushNotificationSubscription.PropName, NamedPropInfo.PushNotificationSubscription.SerializedPushNotificationSubscription);
		}

		private static void AddGroupNotificationsNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.GroupNotifications.GroupNotificationsFolderEntryId.PropName, NamedPropInfo.GroupNotifications.GroupNotificationsFolderEntryId);
		}

		private static void AddAuditLogSearchNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.AuditLogSearch.AuditLogSearchIdentity.PropName, NamedPropInfo.AuditLogSearch.AuditLogSearchIdentity);
		}

		private static void AddLawEnforcementDataNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.LawEnforcementData.LawEnforcementDataIdentity.PropName, NamedPropInfo.LawEnforcementData.LawEnforcementDataIdentity);
			propInfos.Add(NamedPropInfo.LawEnforcementData.LawEnforcementDataInternalName.PropName, NamedPropInfo.LawEnforcementData.LawEnforcementDataInternalName);
		}

		private static void AddMRSLegacy1NamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MRSLegacy1.MailboxMoveStatus.PropName, NamedPropInfo.MRSLegacy1.MailboxMoveStatus);
		}

		private static void AddMRSLegacy2NamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MRSLegacy2.MoveState.PropName, NamedPropInfo.MRSLegacy2.MoveState);
		}

		private static void AddMRSLegacy3NamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MRSLegacy3.MoveServerName.PropName, NamedPropInfo.MRSLegacy3.MoveServerName);
		}

		private static void AddMRSLegacy4NamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MRSLegacy4.AllowedToFinishMove.PropName, NamedPropInfo.MRSLegacy4.AllowedToFinishMove);
		}

		private static void AddMRSLegacy5NamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MRSLegacy5.CancelMove.PropName, NamedPropInfo.MRSLegacy5.CancelMove);
		}

		private static void AddMRSLegacy6NamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MRSLegacy6.ExchangeGuid.PropName, NamedPropInfo.MRSLegacy6.ExchangeGuid);
		}

		private static void AddMRSLegacy7NamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MRSLegacy7.LastUpdateTimestamp.PropName, NamedPropInfo.MRSLegacy7.LastUpdateTimestamp);
		}

		private static void AddMRSLegacy8NamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MRSLegacy8.CreationTimestamp.PropName, NamedPropInfo.MRSLegacy8.CreationTimestamp);
		}

		private static void AddMRSLegacy9NamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MRSLegacy9.JobType.PropName, NamedPropInfo.MRSLegacy9.JobType);
		}

		private static void AddMRSLegacy10NamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MRSLegacy10.MailboxMoveFlags.PropName, NamedPropInfo.MRSLegacy10.MailboxMoveFlags);
		}

		private static void AddMRSLegacy11NamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MRSLegacy11.MailboxMoveSourceMDB.PropName, NamedPropInfo.MRSLegacy11.MailboxMoveSourceMDB);
		}

		private static void AddMRSLegacy12NamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MRSLegacy12.MailboxMoveTargetMDB.PropName, NamedPropInfo.MRSLegacy12.MailboxMoveTargetMDB);
		}

		private static void AddMRSLegacy13NamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MRSLegacy13.DoNotPickUntilTimestamp.PropName, NamedPropInfo.MRSLegacy13.DoNotPickUntilTimestamp);
		}

		private static void AddMRSLegacy14NamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MRSLegacy14.RequestType.PropName, NamedPropInfo.MRSLegacy14.RequestType);
			propInfos.Add(NamedPropInfo.MRSLegacy14.TargetArchiveDatabase.PropName, NamedPropInfo.MRSLegacy14.TargetArchiveDatabase);
		}

		private static void AddMRSLegacy15NamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MRSLegacy15.SourceArchiveDatabase.PropName, NamedPropInfo.MRSLegacy15.SourceArchiveDatabase);
		}

		private static void AddMRSLegacy16NamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MRSLegacy16.Priority.PropName, NamedPropInfo.MRSLegacy16.Priority);
		}

		private static void AddMRSLegacy17NamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MRSLegacy17.SourceExchangeGuid.PropName, NamedPropInfo.MRSLegacy17.SourceExchangeGuid);
		}

		private static void AddMRSLegacy18NamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MRSLegacy18.TargetExchangeGuid.PropName, NamedPropInfo.MRSLegacy18.TargetExchangeGuid);
		}

		private static void AddMigrationServiceNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.MigrationService.RehomeRequest.PropName, NamedPropInfo.MigrationService.RehomeRequest);
			propInfos.Add(NamedPropInfo.MigrationService.InternalFlags.PropName, NamedPropInfo.MigrationService.InternalFlags);
			propInfos.Add(NamedPropInfo.MigrationService.OrganizationGuid.PropName, NamedPropInfo.MigrationService.OrganizationGuid);
			propInfos.Add(NamedPropInfo.MigrationService.PoisonCount.PropName, NamedPropInfo.MigrationService.PoisonCount);
		}

		private static void AddRemindersNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Reminders.EventTimeBasedInboxReminders.PropName, NamedPropInfo.Reminders.EventTimeBasedInboxReminders);
			propInfos.Add(NamedPropInfo.Reminders.QuickCaptureReminders.PropName, NamedPropInfo.Reminders.QuickCaptureReminders);
			propInfos.Add(NamedPropInfo.Reminders.EventTimeBasedInboxRemindersState.PropName, NamedPropInfo.Reminders.EventTimeBasedInboxRemindersState);
			propInfos.Add(NamedPropInfo.Reminders.ModernReminders.PropName, NamedPropInfo.Reminders.ModernReminders);
			propInfos.Add(NamedPropInfo.Reminders.ModernRemindersState.PropName, NamedPropInfo.Reminders.ModernRemindersState);
			propInfos.Add(NamedPropInfo.Reminders.HasExceptionalInboxReminders.PropName, NamedPropInfo.Reminders.HasExceptionalInboxReminders);
			propInfos.Add(NamedPropInfo.Reminders.ReminderId.PropName, NamedPropInfo.Reminders.ReminderId);
			propInfos.Add(NamedPropInfo.Reminders.ReminderItemGlobalObjectId.PropName, NamedPropInfo.Reminders.ReminderItemGlobalObjectId);
			propInfos.Add(NamedPropInfo.Reminders.ReminderOccurrenceGlobalObjectId.PropName, NamedPropInfo.Reminders.ReminderOccurrenceGlobalObjectId);
			propInfos.Add(NamedPropInfo.Reminders.ScheduledReminderTime.PropName, NamedPropInfo.Reminders.ScheduledReminderTime);
			propInfos.Add(NamedPropInfo.Reminders.ReminderText.PropName, NamedPropInfo.Reminders.ReminderText);
			propInfos.Add(NamedPropInfo.Reminders.ReminderStartTime.PropName, NamedPropInfo.Reminders.ReminderStartTime);
			propInfos.Add(NamedPropInfo.Reminders.ReminderEndTime.PropName, NamedPropInfo.Reminders.ReminderEndTime);
		}

		private static void AddComplianceNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Compliance.GroupExpansionRecipients.PropName, NamedPropInfo.Compliance.GroupExpansionRecipients);
			propInfos.Add(NamedPropInfo.Compliance.NeedGroupExpansion.PropName, NamedPropInfo.Compliance.NeedGroupExpansion);
		}

		private static void AddInferenceNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.Inference.InferenceProcessingNeeded.PropName, NamedPropInfo.Inference.InferenceProcessingNeeded);
			propInfos.Add(NamedPropInfo.Inference.InferenceProcessingActions.PropName, NamedPropInfo.Inference.InferenceProcessingActions);
			propInfos.Add(NamedPropInfo.Inference.InferenceActionTruth.PropName, NamedPropInfo.Inference.InferenceActionTruth);
			propInfos.Add(NamedPropInfo.Inference.InferenceNeverClutterOverrideApplied.PropName, NamedPropInfo.Inference.InferenceNeverClutterOverrideApplied);
			propInfos.Add(NamedPropInfo.Inference.InferenceUniqueActionLabelData.PropName, NamedPropInfo.Inference.InferenceUniqueActionLabelData);
		}

		private static void AddPICWNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
		}

		private static void AddWorkingSetNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.WorkingSet.WorkingSetId.PropName, NamedPropInfo.WorkingSet.WorkingSetId);
			propInfos.Add(NamedPropInfo.WorkingSet.WorkingSetSource.PropName, NamedPropInfo.WorkingSet.WorkingSetSource);
			propInfos.Add(NamedPropInfo.WorkingSet.WorkingSetSourcePartition.PropName, NamedPropInfo.WorkingSet.WorkingSetSourcePartition);
			propInfos.Add(NamedPropInfo.WorkingSet.WorkingSetSourcePartitionInternal.PropName, NamedPropInfo.WorkingSet.WorkingSetSourcePartitionInternal);
			propInfos.Add(NamedPropInfo.WorkingSet.WorkingSetFlags.PropName, NamedPropInfo.WorkingSet.WorkingSetFlags);
			propInfos.Add(NamedPropInfo.WorkingSet.WorkingSetFolderEntryId.PropName, NamedPropInfo.WorkingSet.WorkingSetFolderEntryId);
		}

		private static void AddConsumerCalendarNamedPropInfos(Dictionary<StorePropName, StoreNamedPropInfo> propInfos)
		{
			propInfos.Add(NamedPropInfo.ConsumerCalendar.ConsumerCalendarGuid.PropName, NamedPropInfo.ConsumerCalendar.ConsumerCalendarGuid);
			propInfos.Add(NamedPropInfo.ConsumerCalendar.ConsumerCalendarOwnerId.PropName, NamedPropInfo.ConsumerCalendar.ConsumerCalendarOwnerId);
			propInfos.Add(NamedPropInfo.ConsumerCalendar.ConsumerCalendarPrivateFreeBusyId.PropName, NamedPropInfo.ConsumerCalendar.ConsumerCalendarPrivateFreeBusyId);
			propInfos.Add(NamedPropInfo.ConsumerCalendar.ConsumerCalendarPrivateDetailId.PropName, NamedPropInfo.ConsumerCalendar.ConsumerCalendarPrivateDetailId);
			propInfos.Add(NamedPropInfo.ConsumerCalendar.ConsumerCalendarPublishVisibility.PropName, NamedPropInfo.ConsumerCalendar.ConsumerCalendarPublishVisibility);
			propInfos.Add(NamedPropInfo.ConsumerCalendar.ConsumerCalendarSharingInvitations.PropName, NamedPropInfo.ConsumerCalendar.ConsumerCalendarSharingInvitations);
			propInfos.Add(NamedPropInfo.ConsumerCalendar.ConsumerCalendarPermissionLevel.PropName, NamedPropInfo.ConsumerCalendar.ConsumerCalendarPermissionLevel);
			propInfos.Add(NamedPropInfo.ConsumerCalendar.ConsumerCalendarSynchronizationState.PropName, NamedPropInfo.ConsumerCalendar.ConsumerCalendarSynchronizationState);
		}

		internal static WellKnownProperties.PropIdRangeList BuildPropIdRanges(WellKnownProperties.TempIdRange[] ranges)
		{
			if (ranges.Length == 0)
			{
				return default(WellKnownProperties.PropIdRangeList);
			}
			List<WellKnownProperties.TempIdRangeBoundary> list = new List<WellKnownProperties.TempIdRangeBoundary>(ranges.Length * 2);
			for (int i = 0; i < ranges.Length; i++)
			{
				list.Add(new WellKnownProperties.TempIdRangeBoundary
				{
					Boundary = ranges[i].Min,
					IsEnding = false,
					OriginalRangeindex = i
				});
				list.Add(new WellKnownProperties.TempIdRangeBoundary
				{
					Boundary = ranges[i].Max,
					IsEnding = true,
					OriginalRangeindex = i
				});
			}
			list.Sort();
			HashSet<int> hashSet = new HashSet<int>();
			List<ushort> list2 = new List<ushort>(ranges.Length * 2);
			List<StorePropInfo> list3 = new List<StorePropInfo>(ranges.Length);
			for (int j = 0; j < list.Count; j++)
			{
				if (!list[j].IsEnding)
				{
					if (list2.Count % 2 == 0 || list2[list2.Count - 1] != list[j].Boundary)
					{
						if (list2.Count % 2 != 0)
						{
							list2.Add(list[j].Boundary - 1);
							list3.Add(new StorePropInfo(null, 0, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories((from r in hashSet
							select (int)ranges[r].Category).ToArray<int>())));
						}
						list2.Add(list[j].Boundary);
					}
					int num = j;
					while (num < list.Count && !list[num].IsEnding && list[num].Boundary == list[j].Boundary)
					{
						hashSet.Add(list[num].OriginalRangeindex);
						num++;
					}
					j = num - 1;
				}
				else
				{
					list2.Add(list[j].Boundary);
					list3.Add(new StorePropInfo(null, 0, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories((from r in hashSet
					select (int)ranges[r].Category).ToArray<int>())));
					int num2 = j;
					while (num2 < list.Count && list[num2].IsEnding && list[num2].Boundary == list[j].Boundary)
					{
						hashSet.Remove(list[num2].OriginalRangeindex);
						num2++;
					}
					if (hashSet.Count != 0)
					{
						list2.Add(list[j].Boundary + 1);
					}
					j = num2 - 1;
				}
			}
			return new WellKnownProperties.PropIdRangeList(list2.ToArray(), list3.ToArray());
		}

		public static readonly StorePropInfo DefaultPropInfo = new StorePropInfo(null, 0, PropertyType.Invalid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(15));

		public static readonly ObjectType[] BaseObjectType = WellKnownProperties.BuildBaseObjectTypeArray();

		private static readonly WellKnownProperties.ObjectPropertyDefinitions[] PropertyDefinitions = WellKnownProperties.BuildPropertyDefinitions();

		private static readonly Dictionary<StorePropName, StoreNamedPropInfo> NamedPropInfos = WellKnownProperties.BuildNamedPropInfos();

		private struct ObjectPropertyDefinitions
		{
			public ObjectType BaseObjectType;

			public Dictionary<uint, StorePropInfo> Properties;

			public Dictionary<ushort, StorePropInfo> PropIds;

			public WellKnownProperties.PropIdRangeList PropIdRanges;

			public Dictionary<string, StorePropInfo> PropertiesByTagName;
		}

		internal struct PropIdRangeList
		{
			public PropIdRangeList(ushort[] rangeList, StorePropInfo[] infoList)
			{
				this.rangeList = rangeList;
				this.infoList = infoList;
			}

			public bool TryFindRange(ushort propId, out StorePropInfo propInfo)
			{
				if (this.rangeList != null)
				{
					int num = Array.BinarySearch<ushort>(this.rangeList, propId);
					if (num >= 0)
					{
						propInfo = this.infoList[num / 2];
						return true;
					}
					num = ~num;
					if ((num & 1) != 0)
					{
						propInfo = this.infoList[num / 2];
						return true;
					}
				}
				propInfo = null;
				return false;
			}

			private ushort[] rangeList;

			private StorePropInfo[] infoList;
		}

		internal struct TempIdRange : IComparable<WellKnownProperties.TempIdRange>
		{
			public int CompareTo(WellKnownProperties.TempIdRange other)
			{
				int num = this.Min.CompareTo(other.Min);
				if (num == 0)
				{
					num = this.Max.CompareTo(other.Max);
				}
				return num;
			}

			public ushort Min;

			public ushort Max;

			public PropCategory Category;
		}

		internal struct TempIdRangeBoundary : IComparable<WellKnownProperties.TempIdRangeBoundary>
		{
			public int CompareTo(WellKnownProperties.TempIdRangeBoundary other)
			{
				int num = this.Boundary.CompareTo(other.Boundary);
				if (num == 0)
				{
					num = this.IsEnding.CompareTo(other.IsEnding);
					if (num == 0)
					{
						num = this.OriginalRangeindex.CompareTo(other.OriginalRangeindex);
					}
				}
				return num;
			}

			public ushort Boundary;

			public bool IsEnding;

			public int OriginalRangeindex;
		}
	}
}

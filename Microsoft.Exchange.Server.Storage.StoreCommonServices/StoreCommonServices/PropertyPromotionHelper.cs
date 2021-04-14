using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal static class PropertyPromotionHelper
	{
		internal static short[] BuildDefaultPromotedPropertyIds(Context context, Mailbox mailbox)
		{
			return PropertyPromotionHelper.BuildPromotedPropertyIds(context, mailbox, PropertyPromotionHelper.DefaultPromotedMessageProperties);
		}

		internal static List<StorePropTag> BuildDefaultPromotedPropertyTags(Context context, Mailbox mailbox)
		{
			return PropertyPromotionHelper.BuildPromotedPropertyTags(context, mailbox, PropertyPromotionHelper.DefaultPromotedMessageProperties);
		}

		internal static short[] BuildAlwaysPromotedPropertyIds(Context context, Mailbox mailbox)
		{
			return PropertyPromotionHelper.BuildPromotedPropertyIds(context, mailbox, PropertyPromotionHelper.AlwaysPromotedMessageProperties);
		}

		internal static void CollectPropertiesToPromote(Context context, StorePropTag propTag, HashSet<ushort> promotedPropertyIds, HashSet<ushort> excludePropertyIds, ref List<StorePropTag> propertiesToPromote)
		{
			PropertyMapping propertyMapping = PropertySchema.FindMapping(context.Database, ObjectType.Message, propTag);
			if (propertyMapping != null)
			{
				PropertyPromotionHelper.CollectPropertiesToPromoteFromMapping(propertyMapping, promotedPropertyIds, excludePropertyIds, ref propertiesToPromote);
				return;
			}
			PropertyPromotionHelper.CollectOnePropertyToPromote(propTag, promotedPropertyIds, excludePropertyIds, ref propertiesToPromote);
		}

		internal static void ValidatePropertiesPromotion(Context context, Mailbox mailbox, string folderName, IList<Column> columns)
		{
			if (!ConfigurationSchema.EnablePropertiesPromotionValidation.Value)
			{
				return;
			}
			HashSet<ushort> storeDefaultPromotedMessagePropertyIds = mailbox.GetStoreDefaultPromotedMessagePropertyIds(context);
			List<StorePropTag> list = null;
			foreach (Column column in columns)
			{
				ExtendedPropertyColumn extendedPropertyColumn = column as ExtendedPropertyColumn;
				if (extendedPropertyColumn != null)
				{
					PropertyPromotionHelper.CollectPropertiesToPromote(context, extendedPropertyColumn.StorePropTag, null, storeDefaultPromotedMessagePropertyIds, ref list);
				}
			}
			if (!mailbox.MailboxInfo.IsArchiveMailbox && list != null && context.ClientType == ClientType.OWA)
			{
				StringBuilder stringBuilder = new StringBuilder(150);
				stringBuilder.Append("{");
				foreach (StorePropTag storePropTag in list)
				{
					stringBuilder.Append("[");
					storePropTag.AppendToString(stringBuilder, true);
					stringBuilder.Append("]");
				}
				stringBuilder.Append("}");
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_SetColumnsHasNonPromotedProperties, new object[]
				{
					stringBuilder.ToString(),
					mailbox.MailboxGuid,
					folderName,
					context.ClientType
				});
				foreach (StorePropTag storePropTag2 in list)
				{
					DiagnosticContext.TraceDword((LID)53568U, storePropTag2.PropTag);
				}
				throw new StoreException((LID)38208U, ErrorCodeValue.TooComplex);
			}
		}

		private static short[] BuildPromotedPropertyIds(Context context, Mailbox mailbox, StorePropInfo[] propmotedPropertyInfos)
		{
			List<StorePropTag> source = PropertyPromotionHelper.BuildPromotedPropertyTags(context, mailbox, propmotedPropertyInfos);
			return (from x in source
			select (short)x.PropId).Distinct<short>().ToArray<short>();
		}

		private static List<StorePropTag> BuildPromotedPropertyTags(Context context, Mailbox mailbox, StorePropInfo[] propmotedPropertyInfos)
		{
			List<StorePropTag> result = new List<StorePropTag>(propmotedPropertyInfos.Length);
			foreach (StorePropInfo storePropInfo in propmotedPropertyInfos)
			{
				StorePropTag namedPropStorePropTag;
				if (storePropInfo.IsNamedProperty)
				{
					namedPropStorePropTag = mailbox.GetNamedPropStorePropTag(context, storePropInfo.PropName, storePropInfo.PropType, ObjectType.Message);
				}
				else
				{
					namedPropStorePropTag = new StorePropTag(storePropInfo.PropId, storePropInfo.PropType, storePropInfo, ObjectType.Message);
				}
				PropertyPromotionHelper.CollectPropertiesToPromote(context, namedPropStorePropTag, null, null, ref result);
			}
			return result;
		}

		private static bool ExcludePropertyFromPromotion(StorePropTag propTag)
		{
			uint propTag2 = propTag.PropTag;
			if (propTag2 <= 246874143U)
			{
				if (propTag2 <= 246480927U)
				{
					if (propTag2 <= 245825567U)
					{
						if (propTag2 != 245694495U && propTag2 != 245760031U && propTag2 != 245825567U)
						{
							return false;
						}
					}
					else if (propTag2 != 245891103U && propTag2 != 246349855U && propTag2 != 246480927U)
					{
						return false;
					}
				}
				else if (propTag2 <= 246677535U)
				{
					if (propTag2 != 246546463U && propTag2 != 246611999U && propTag2 != 246677535U)
					{
						return false;
					}
				}
				else if (propTag2 != 246743071U && propTag2 != 246808607U && propTag2 != 246874143U)
				{
					return false;
				}
			}
			else if (propTag2 <= 247267359U)
			{
				if (propTag2 <= 247070751U)
				{
					if (propTag2 != 246939679U && propTag2 != 247005215U && propTag2 != 247070751U)
					{
						return false;
					}
				}
				else if (propTag2 != 247136287U && propTag2 != 247201823U && propTag2 != 247267359U)
				{
					return false;
				}
			}
			else if (propTag2 <= 272171039U)
			{
				if (propTag2 != 247332895U && propTag2 != 248381451U && propTag2 != 272171039U)
				{
					return false;
				}
			}
			else if (propTag2 != 272760863U && propTag2 != 1071185951U && propTag2 != 1718419487U)
			{
				return false;
			}
			return true;
		}

		private static void CollectOnePropertyToPromote(StorePropTag propTag, HashSet<ushort> promotedPropertyIds, HashSet<ushort> excludePropertyIds, ref List<StorePropTag> propertiesToPromote)
		{
			if ((promotedPropertyIds == null || !promotedPropertyIds.Contains(propTag.PropId)) && !PropertyPromotionHelper.ExcludePropertyFromPromotion(propTag) && (excludePropertyIds == null || !excludePropertyIds.Contains(propTag.PropId)))
			{
				if (propertiesToPromote == null)
				{
					propertiesToPromote = new List<StorePropTag>(5);
				}
				bool flag = false;
				foreach (StorePropTag storePropTag in propertiesToPromote)
				{
					if (storePropTag.PropId == propTag.PropId)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					propertiesToPromote.Add(propTag);
				}
			}
		}

		private static void CollectPropertiesToPromoteFromMapping(PropertyMapping mapping, HashSet<ushort> promotedPropertyIds, HashSet<ushort> excludePropertyIds, ref List<StorePropTag> propertiesToPromote)
		{
			if (mapping is DefaultPropertyMapping)
			{
				PropertyPromotionHelper.CollectOnePropertyToPromote(mapping.PropertyTag, promotedPropertyIds, excludePropertyIds, ref propertiesToPromote);
				return;
			}
			if (mapping is ComputedPropertyMapping)
			{
				ComputedPropertyMapping computedPropertyMapping = (ComputedPropertyMapping)mapping;
				if (computedPropertyMapping.CanBeOverridden)
				{
					PropertyPromotionHelper.CollectOnePropertyToPromote(mapping.PropertyTag, promotedPropertyIds, excludePropertyIds, ref propertiesToPromote);
				}
				if (computedPropertyMapping.DependentPropertyMappings != null)
				{
					foreach (PropertyMapping mapping2 in computedPropertyMapping.DependentPropertyMappings)
					{
						PropertyPromotionHelper.CollectPropertiesToPromoteFromMapping(mapping2, promotedPropertyIds, excludePropertyIds, ref propertiesToPromote);
					}
					return;
				}
			}
			else
			{
				if (mapping is ConversionPropertyMapping)
				{
					ConversionPropertyMapping conversionPropertyMapping = (ConversionPropertyMapping)mapping;
					PropertyPromotionHelper.CollectPropertiesToPromoteFromMapping(conversionPropertyMapping.ArgumentPropertyMapping, promotedPropertyIds, excludePropertyIds, ref propertiesToPromote);
					return;
				}
				if (mapping is FunctionPropertyMapping)
				{
					FunctionPropertyMapping functionPropertyMapping = (FunctionPropertyMapping)mapping;
					if (functionPropertyMapping.ArgumentPropertyMappings != null)
					{
						foreach (PropertyMapping mapping3 in functionPropertyMapping.ArgumentPropertyMappings)
						{
							PropertyPromotionHelper.CollectPropertiesToPromoteFromMapping(mapping3, promotedPropertyIds, excludePropertyIds, ref propertiesToPromote);
						}
					}
				}
			}
		}

		private const int TypicalColumnsToPromote = 5;

		internal static readonly StorePropInfo[] AlwaysPromotedMessageProperties = new StorePropInfo[]
		{
			PropTag.Message.NormalizedSubject.PropInfo,
			PropTag.Message.ConversationTopic.PropInfo
		};

		internal static readonly StorePropInfo[] DefaultPromotedMessageProperties = new StorePropInfo[]
		{
			PropTag.Message.SentRepresentingName.PropInfo,
			PropTag.Message.SentRepresentingEmailAddress.PropInfo,
			PropTag.Message.ELCAutoCopyTag.PropInfo,
			PropTag.Message.ELCMoveDate.PropInfo,
			PropTag.Message.PolicyTag.PropInfo,
			PropTag.Message.ArchiveDate.PropInfo,
			PropTag.Message.RetentionDate.PropInfo,
			PropTag.Message.RetentionFlags.PropInfo,
			PropTag.Message.BodyTag.PropInfo,
			PropTag.Message.NormalizedSubject.PropInfo,
			PropTag.Message.ConversationTopic.PropInfo,
			PropTag.Message.FlagStatus.PropInfo,
			PropTag.Message.FlagCompleteTime.PropInfo,
			PropTag.Message.IconIndex.PropInfo,
			PropTag.Message.CompanyName.PropInfo,
			PropTag.Message.DisplayName.PropInfo,
			PropTag.Message.DisplayNamePrefix.PropInfo,
			PropTag.Message.GivenName.PropInfo,
			PropTag.Message.MiddleName.PropInfo,
			PropTag.Message.SurName.PropInfo,
			PropTag.Message.Generation.PropInfo,
			PropTag.Message.RelevanceScore.PropInfo,
			PropTag.Message.IsDistributionListContact.PropInfo,
			PropTag.Message.HomeAddressCity.PropInfo,
			PropTag.Message.PredictedActions.PropInfo,
			PropTag.Message.PartnerNetworkId.PropInfo,
			PropTag.Message.Title.PropInfo,
			PropTag.Message.HomeAddressStreet.PropInfo,
			PropTag.Message.HomeAddressStateOrProvince.PropInfo,
			PropTag.Message.HomeAddressCountry.PropInfo,
			PropTag.Message.HomeAddressPostalCode.PropInfo,
			PropTag.Message.HomeAddressPostOfficeBox.PropInfo,
			PropTag.Message.OtherAddressStreet.PropInfo,
			PropTag.Message.OtherAddressCity.PropInfo,
			PropTag.Message.OtherAddressStateOrProvince.PropInfo,
			PropTag.Message.OtherAddressCountry.PropInfo,
			PropTag.Message.OtherAddressPostalCode.PropInfo,
			PropTag.Message.OtherAddressPostOfficeBox.PropInfo,
			PropTag.Message.PostOfficeBox.PropInfo,
			PropTag.Message.RelevanceScore.PropInfo,
			PropTag.Message.IsClutter.PropInfo,
			PropTag.Message.InternetMessageId.PropInfo,
			PropTag.Message.FollowupIcon.PropInfo,
			PropTag.Message.SearchKey.PropInfo,
			PropTag.Message.TodoItemFlags.PropInfo,
			PropTag.Message.ItemTempFlags.PropInfo,
			PropTag.Message.SecureSubmitFlags.PropInfo,
			PropTag.Message.ReplyTime.PropInfo,
			PropTag.Message.ExpiryTime.PropInfo,
			PropTag.Message.SecurityFlags.PropInfo,
			PropTag.Message.ProofInProgress.PropInfo,
			PropTag.Message.OriginalSourceServerVersion.PropInfo,
			PropTag.Message.PartnerNetworkUserId.PropInfo,
			PropTag.Message.StartDate.PropInfo,
			PropTag.Message.EndDate.PropInfo,
			PropTag.Message.DisplayCc.PropInfo,
			PropTag.Message.DisplayBcc.PropInfo,
			PropTag.Message.ArchiveTag.PropInfo,
			PropTag.Message.SenderName.PropInfo,
			PropTag.Message.SenderFlags.PropInfo,
			PropTag.Message.SenderEntryId.PropInfo,
			PropTag.Message.SenderSimpleDisplayName.PropInfo,
			PropTag.Message.SenderEmailAddress.PropInfo,
			PropTag.Message.RetentionPeriod.PropInfo,
			PropTag.Message.MessageToMe.PropInfo,
			PropTag.Message.MessageCCMe.PropInfo,
			PropTag.Message.ArchivePeriod.PropInfo,
			PropTag.Message.PartnerNetworkThumbnailPhotoUrl.PropInfo,
			PropTag.Message.SenderAddressType.PropInfo,
			PropTag.Message.SentRepresentingAddressType.PropInfo,
			PropTag.Message.DeliveryReportRequested.PropInfo,
			PropTag.Message.ResponseRequested.PropInfo,
			PropTag.Message.SendRichInfo.PropInfo,
			PropTag.Message.Processed.PropInfo,
			PropTag.Message.ReceivedRepresentingEntryId.PropInfo,
			PropTag.Message.RcvdRepresentingFlags.PropInfo,
			PropTag.Message.RecipientSipUri.PropInfo,
			PropTag.Message.SenderSMTPAddressXSO.PropInfo,
			PropTag.Message.SentRepresentingSMTPAddressXSO.PropInfo,
			PropTag.Message.CreatorSID.PropInfo,
			PropTag.Message.CreatorFlags.PropInfo,
			PropTag.Message.LastVerbExecuted.PropInfo,
			PropTag.Message.StartDateEtc.PropInfo,
			PropTag.Message.FidMid.PropInfo,
			PropTag.Message.ReportTag.PropInfo,
			PropTag.Message.Access.PropInfo,
			PropTag.Message.WunderbarLinkSection.PropInfo,
			PropTag.Message.WunderbarLinkGroupName.PropInfo,
			PropTag.Message.WunderbarLinkGroupClsId.PropInfo,
			PropTag.Message.FreebusyEMA.PropInfo,
			PropTag.Message.SchdInfoFreebusyMerged.PropInfo,
			PropTag.Message.WunderbarLinkEntryID.PropInfo,
			PropTag.Message.NavigationNodeCalendarColor.PropInfo,
			PropTag.Message.NavigationNodeAddressbookEntryId.PropInfo,
			PropTag.Message.WunderbarLinkStoreEntryId.PropInfo,
			PropTag.Message.SenderSID.PropInfo,
			PropTag.Message.SentRepresentingFlags.PropInfo,
			PropTag.Message.SentRepresentingSID.PropInfo,
			PropTag.Message.SentRepresentingEntryId.PropInfo,
			PropTag.Message.SentRepresentingSimpleDisplayName.PropInfo,
			PropTag.Message.ReceivedRepresentingAddressType.PropInfo,
			PropTag.Message.ReceivedRepresentingEmailAddress.PropInfo,
			PropTag.Message.ReceivedRepresentingName.PropInfo,
			PropTag.Message.ReceivedRepresentingSimpleDisplayName.PropInfo,
			PropTag.Message.FavoriteDisplayName.PropInfo,
			PropTag.Message.FavPublicSourceKey.PropInfo,
			PropTag.Message.FavLevelMask.PropInfo,
			PropTag.Message.Account.PropInfo,
			PropTag.Message.NickName.PropInfo,
			PropTag.Message.DepartmentName.PropInfo,
			PropTag.Message.Initials.PropInfo,
			PropTag.Message.BusinessTelephoneNumber.PropInfo,
			PropTag.Message.Business2TelephoneNumber.PropInfo,
			PropTag.Message.HomeTelephoneNumber.PropInfo,
			PropTag.Message.Home2TelephoneNumber.PropInfo,
			PropTag.Message.MobileTelephoneNumber.PropInfo,
			PropTag.Message.AssistantTelephoneNumber.PropInfo,
			PropTag.Message.CallbackTelephoneNumber.PropInfo,
			PropTag.Message.CarTelephoneNumber.PropInfo,
			PropTag.Message.HomeFaxNumber.PropInfo,
			PropTag.Message.CompanyMainPhoneNumber.PropInfo,
			PropTag.Message.PrimaryFaxNumber.PropInfo,
			PropTag.Message.OtherTelephoneNumber.PropInfo,
			PropTag.Message.PagerTelephoneNumber.PropInfo,
			PropTag.Message.RadioTelephoneNumber.PropInfo,
			PropTag.Message.TelexNumber.PropInfo,
			PropTag.Message.TTYTDDPhoneNumber.PropInfo,
			PropTag.Message.BusinessHomePage.PropInfo,
			PropTag.Message.PersonalHomePage.PropInfo,
			PropTag.Message.OfficeLocation.PropInfo,
			PropTag.Message.ManagerName.PropInfo,
			PropTag.Message.Assistant.PropInfo,
			PropTag.Message.Profession.PropInfo,
			PropTag.Message.SpouseName.PropInfo,
			PropTag.Message.ChildrensNames.PropInfo,
			PropTag.Message.Hobbies.PropInfo,
			PropTag.Message.WeddingAnniversary.PropInfo,
			PropTag.Message.Birthday.PropInfo,
			PropTag.Message.SendInternetEncoding.PropInfo,
			PropTag.Message.ReplyTemplateId.PropInfo,
			PropTag.Message.OwnerApptId.PropInfo,
			PropTag.Message.BusinessTelephoneNumber.PropInfo,
			PropTag.Message.SubmitResponsibility.PropInfo,
			PropTag.Message.MobileTelephoneNumber.PropInfo,
			PropTag.Message.LastVerbExecutionTime.PropInfo,
			PropTag.Message.AutoResponseSuppress.PropInfo,
			PropTag.Message.BlockStatus.PropInfo,
			PropTag.Message.ContentFilterPCL.PropInfo,
			PropTag.Message.VoiceMessageAttachmentOrder.PropInfo,
			PropTag.Message.InternetCPID.PropInfo,
			PropTag.Message.MessageLocaleId.PropInfo,
			PropTag.Message.LastModifierName.PropInfo,
			PropTag.Message.LastModifierFlags.PropInfo,
			PropTag.Message.LastModifierEntryId.PropInfo,
			PropTag.Message.CreatorEntryId.PropInfo,
			PropTag.Message.LastModifierSimpleDisplayName.PropInfo,
			PropTag.Message.LastModifierEmailAddr.PropInfo,
			PropTag.Message.CreatorEmailAddr.PropInfo,
			PropTag.Message.CreatorSimpleDisplayName.PropInfo,
			PropTag.Message.CreatorName.PropInfo,
			PropTag.Message.LocalCommitTime.PropInfo,
			PropTag.Message.DeliveryReportRequested.PropInfo,
			PropTag.Message.DAMOriginalEntryId.PropInfo,
			PropTag.Message.BusinessFaxNumber.PropInfo,
			PropTag.Message.EmailAddress.PropInfo,
			PropTag.Message.PrimaryTelephoneNumber.PropInfo,
			PropTag.Message.ISDNNumber.PropInfo,
			PropTag.Message.UserConfigurationDataType.PropInfo,
			PropTag.Message.OriginalDeliveryFolderInfo.PropInfo,
			PropTag.Message.ReplyRecipientNames.PropInfo,
			PropTag.Message.RenewTime.PropInfo,
			PropTag.Message.RichContent.PropInfo,
			NamedPropInfo.AuditLogSearch.AuditLogSearchIdentity,
			NamedPropInfo.PublicStrings.Keywords,
			NamedPropInfo.PublicStrings.OnlineMeetingExternalLink,
			NamedPropInfo.PublicStrings.Categories,
			NamedPropInfo.PublicStrings.MonitoringUniqueId,
			NamedPropInfo.PublicStrings.MonitoringEventSource,
			NamedPropInfo.PublicStrings.MonitoringInsertionStrings,
			NamedPropInfo.PublicStrings.MonitoringCreationTimeUtc,
			NamedPropInfo.PublicStrings.MonitoringNotificationEmailSent,
			NamedPropInfo.PublicStrings.MonitoringEventEntryType,
			NamedPropInfo.PublicStrings.MonitoringNotificationRecipients,
			NamedPropInfo.PublicStrings.MonitoringEventCategoryId,
			NamedPropInfo.PublicStrings.MonitoringEventTimeUtc,
			NamedPropInfo.PublicStrings.MonitoringNotificationMessageIds,
			NamedPropInfo.PublicStrings.MonitoringEventPeriodicKey,
			NamedPropInfo.PublicStrings.MonitoringEventInstanceId,
			NamedPropInfo.PublicStrings.PhishingStamp,
			NamedPropInfo.PublicStrings.SpoofingStamp,
			NamedPropInfo.PublicStrings.TelURI,
			NamedPropInfo.PublicStrings.PeopleConnectionCreationTime,
			NamedPropInfo.InternetHeaders.XMSExchOrganizationAuthDomain,
			NamedPropInfo.InternetHeaders.ContentClass,
			NamedPropInfo.InternetHeaders.XRequireProtectedPlayOnPhone,
			NamedPropInfo.Common.SmartNoAttach,
			NamedPropInfo.Common.SideEffects,
			NamedPropInfo.Common.HeaderItem,
			NamedPropInfo.Common.ReminderSet,
			NamedPropInfo.Common.ReminderTime,
			NamedPropInfo.Common.AutoSaveOriginalItemInfo,
			NamedPropInfo.Common.ImapDeleted,
			NamedPropInfo.Common.CurrentVersion,
			NamedPropInfo.Common.PendingStateforTMDocument,
			NamedPropInfo.Common.ReminderDelta,
			NamedPropInfo.Common.ReminderNextTime,
			NamedPropInfo.Common.CommonStart,
			NamedPropInfo.Common.CommonEnd,
			NamedPropInfo.Common.SniffState,
			NamedPropInfo.Common.Request,
			NamedPropInfo.Common.Classified,
			NamedPropInfo.Common.PropertyExistenceTracker,
			NamedPropInfo.Common.Private,
			NamedPropInfo.Common.VerbStream,
			NamedPropInfo.Common.VerbResponse,
			NamedPropInfo.Address.EmailEmailAddress,
			NamedPropInfo.Address.EmailAddressType,
			NamedPropInfo.Address.EmailDisplayName,
			NamedPropInfo.Address.EmailOriginalDisplayName,
			NamedPropInfo.Address.EmailOriginalEntryID,
			NamedPropInfo.Address.Email2EmailAddress,
			NamedPropInfo.Address.Email2AddressType,
			NamedPropInfo.Address.Email2DisplayName,
			NamedPropInfo.Address.Email2OriginalDisplayName,
			NamedPropInfo.Address.Email2OriginalEntryID,
			NamedPropInfo.Address.MobileTelephoneNumber2,
			NamedPropInfo.Address.Email3EmailAddress,
			NamedPropInfo.Address.Email3AddressType,
			NamedPropInfo.Address.Email3DisplayName,
			NamedPropInfo.Address.Email3OriginalDisplayName,
			NamedPropInfo.Address.Email3OriginalEntryID,
			NamedPropInfo.Address.ContactGALLinkID,
			NamedPropInfo.Address.FileUnder,
			NamedPropInfo.Address.FileUnderId,
			NamedPropInfo.Address.HasPicture,
			NamedPropInfo.Address.InstMsg,
			NamedPropInfo.Address.AutoLog,
			NamedPropInfo.Address.DontAgeLog,
			NamedPropInfo.Address.DisplayNameFirstLast,
			NamedPropInfo.Address.DisplayNameLastFirst,
			NamedPropInfo.Address.DisplayNamePriority,
			NamedPropInfo.Address.WorkAddressCity,
			NamedPropInfo.Address.WorkAddressStreet,
			NamedPropInfo.Address.WorkAddressState,
			NamedPropInfo.Address.WorkAddressCountry,
			NamedPropInfo.Address.WorkAddressPostalCode,
			NamedPropInfo.Address.LinkRejectHistoryRaw,
			NamedPropInfo.Address.YomiCompanyName,
			NamedPropInfo.Address.YomiFirstName,
			NamedPropInfo.Address.YomiLastName,
			NamedPropInfo.Address.ContactOtherPhone2,
			NamedPropInfo.Address.Schools,
			NamedPropInfo.Address.Fax1AddressType,
			NamedPropInfo.Address.Fax1EmailAddress,
			NamedPropInfo.Address.Fax1OriginalDisplayName,
			NamedPropInfo.Address.Fax1OriginalEntryID,
			NamedPropInfo.Address.Email1RTF,
			NamedPropInfo.Address.Fax1RTF,
			NamedPropInfo.Address.Fax2AddressType,
			NamedPropInfo.Address.Fax2EmailAddress,
			NamedPropInfo.Address.Fax2OriginalDisplayName,
			NamedPropInfo.Address.Fax2OriginalEntryID,
			NamedPropInfo.Address.Email2RTF,
			NamedPropInfo.Address.Fax2RTF,
			NamedPropInfo.Address.Fax3AddressType,
			NamedPropInfo.Address.Fax3EmailAddress,
			NamedPropInfo.Address.Fax3OriginalDisplayName,
			NamedPropInfo.Address.Fax3OriginalEntryID,
			NamedPropInfo.Address.Email3RTF,
			NamedPropInfo.Address.Fax3RTF,
			NamedPropInfo.Address.ABPArrayType,
			NamedPropInfo.Address.ABPEmailList,
			NamedPropInfo.Address.AddressLinked,
			NamedPropInfo.Address.AddressBookEntryId,
			NamedPropInfo.Address.SmtpAddressCache,
			NamedPropInfo.Address.GALLinkState,
			NamedPropInfo.Address.InternalPersonType,
			NamedPropInfo.Address.UserApprovedLink,
			NamedPropInfo.Address.ImContactSipUriAddress,
			NamedPropInfo.Address.ProtectedEmailAddress,
			NamedPropInfo.Address.ProtectedPhoneNumber,
			NamedPropInfo.Address.BirthdayLocal,
			NamedPropInfo.Address.WeddingAnniversaryLocal,
			NamedPropInfo.Address.DLMembers,
			NamedPropInfo.Address.DLOneOffMembers,
			NamedPropInfo.Address.IsFavorite,
			NamedPropInfo.AirSync.ASIMAddress2,
			NamedPropInfo.AirSync.ASIMAddress3,
			NamedPropInfo.Appointment.ApptRecur,
			NamedPropInfo.Appointment.ApptSequence,
			NamedPropInfo.Appointment.ApptTZDefRecur,
			NamedPropInfo.Appointment.TimeZoneStruct,
			NamedPropInfo.Appointment.TimeZoneDesc,
			NamedPropInfo.Appointment.ApptStartWhole,
			NamedPropInfo.Appointment.ApptTZDefStartDisplay,
			NamedPropInfo.Appointment.ApptTZDefEndDisplay,
			NamedPropInfo.Appointment.ApptEndWhole,
			NamedPropInfo.Appointment.Location,
			NamedPropInfo.Appointment.ApptStateFlags,
			NamedPropInfo.Appointment.Recurring,
			NamedPropInfo.Appointment.ApptSubType,
			NamedPropInfo.Appointment.BusyStatus,
			NamedPropInfo.Appointment.ResponseStatus,
			NamedPropInfo.Appointment.ApptReplyTime,
			NamedPropInfo.Appointment.ChangeHighlight,
			NamedPropInfo.Appointment.FInvited,
			NamedPropInfo.Appointment.ClipStart,
			NamedPropInfo.Appointment.ClipEnd,
			NamedPropInfo.Appointment.ApptColor,
			NamedPropInfo.Appointment.ApptAuxFlags,
			NamedPropInfo.Appointment.MWSURL,
			NamedPropInfo.Appointment.RecurType,
			NamedPropInfo.Appointment.FDirtyTimes,
			NamedPropInfo.Appointment.FDirtyLocation,
			NamedPropInfo.Appointment.ApptLastSequence,
			NamedPropInfo.Appointment.IsHiddenFromLegacyClients,
			NamedPropInfo.CalendarAssistant.CalendarProcessed,
			NamedPropInfo.CalendarAssistant.ClientInfoString,
			NamedPropInfo.CalendarAssistant.ClientProcessName,
			NamedPropInfo.CalendarAssistant.ClientMachineName,
			NamedPropInfo.CalendarAssistant.OriginalLastModifiedTime,
			NamedPropInfo.CalendarAssistant.ItemVersion,
			NamedPropInfo.CalendarAssistant.ClientIntent,
			NamedPropInfo.CalendarAssistant.OriginalFolderId,
			NamedPropInfo.CalendarAssistant.CalendarLogTriggerAction,
			NamedPropInfo.CalendarAssistant.ChangeList,
			NamedPropInfo.CalendarAssistant.MiddleTierProcessName,
			NamedPropInfo.CalendarAssistant.ViewStartTime,
			NamedPropInfo.CalendarAssistant.ViewEndTime,
			NamedPropInfo.Elc.ElcExplicitPolicyTag,
			NamedPropInfo.Elc.ElcExplicitArchiveTag,
			NamedPropInfo.ExternalSharing.Url,
			NamedPropInfo.ExternalSharing.LocalFolderId,
			NamedPropInfo.ExternalSharing.DataType,
			NamedPropInfo.ExternalSharing.SharerIdentity,
			NamedPropInfo.ExternalSharing.SharerName,
			NamedPropInfo.ExternalSharing.RemoteFolderId,
			NamedPropInfo.ExternalSharing.RemoteFolderName,
			NamedPropInfo.ExternalSharing.IsPrimary,
			NamedPropInfo.ExternalSharing.SharerIdentityFederationUri,
			NamedPropInfo.ExternalSharing.SharingKey,
			NamedPropInfo.ExternalSharing.SubscriberIdentity,
			NamedPropInfo.ExternalSharing.MasterId,
			NamedPropInfo.LawEnforcementData.LawEnforcementDataIdentity,
			NamedPropInfo.LawEnforcementData.LawEnforcementDataInternalName,
			NamedPropInfo.Location.HomeAccuracy,
			NamedPropInfo.Location.HomeAltitude,
			NamedPropInfo.Location.HomeAltitudeAccuracy,
			NamedPropInfo.Location.HomeLatitude,
			NamedPropInfo.Location.HomeLongitude,
			NamedPropInfo.Location.WorkAccuracy,
			NamedPropInfo.Location.WorkAltitude,
			NamedPropInfo.Location.WorkAltitudeAccuracy,
			NamedPropInfo.Location.WorkLatitude,
			NamedPropInfo.Location.WorkLongitude,
			NamedPropInfo.Location.WorkLocationSource,
			NamedPropInfo.Location.WorkLocationUri,
			NamedPropInfo.Location.HomeLocationSource,
			NamedPropInfo.Location.HomeLocationUri,
			NamedPropInfo.Location.OtherAccuracy,
			NamedPropInfo.Location.OtherAltitude,
			NamedPropInfo.Location.OtherAltitudeAccuracy,
			NamedPropInfo.Location.OtherLatitude,
			NamedPropInfo.Location.OtherLongitude,
			NamedPropInfo.Location.OtherLocationSource,
			NamedPropInfo.Location.OtherLocationUri,
			NamedPropInfo.Location.LocationRelevanceRank,
			NamedPropInfo.Location.LocationDisplayName,
			NamedPropInfo.Location.LocationAnnotation,
			NamedPropInfo.Location.LocationSource,
			NamedPropInfo.Location.LocationUri,
			NamedPropInfo.Location.Latitude,
			NamedPropInfo.Location.Longitude,
			NamedPropInfo.Location.Accuracy,
			NamedPropInfo.Location.Altitude,
			NamedPropInfo.Location.AltitudeAccuracy,
			NamedPropInfo.Location.StreetAddress,
			NamedPropInfo.Location.LocationCity,
			NamedPropInfo.Location.LocationState,
			NamedPropInfo.Location.LocationCountry,
			NamedPropInfo.Location.LocationPostalCode,
			NamedPropInfo.Meeting.CleanGlobalObjId,
			NamedPropInfo.Meeting.IsException,
			NamedPropInfo.Meeting.GlobalObjId,
			NamedPropInfo.Meeting.OwnerCriticalChange,
			NamedPropInfo.Meeting.OldWhenStartWhole,
			NamedPropInfo.Meeting.OldWhenEndWhole,
			NamedPropInfo.Meeting.OldLocation,
			NamedPropInfo.Meeting.MeetingType,
			NamedPropInfo.Meeting.IsRecurring,
			NamedPropInfo.Meeting.AttendeeCriticalChange,
			NamedPropInfo.Meeting.ResponseState,
			NamedPropInfo.Messaging.TextMessagingDeliveryStatus,
			NamedPropInfo.Messaging.MessageBccMe,
			NamedPropInfo.Task.TaskStatus,
			NamedPropInfo.Task.TaskStartDate,
			NamedPropInfo.Task.TaskDueDate,
			NamedPropInfo.Task.TaskComplete,
			NamedPropInfo.Task.TaskDateCompleted,
			NamedPropInfo.Task.TaskRecur,
			NamedPropInfo.Task.TaskDeadOccur,
			NamedPropInfo.Task.TaskAccepted,
			NamedPropInfo.Task.TaskState,
			NamedPropInfo.Task.DoItTime,
			NamedPropInfo.Remote.RemoteAttachment,
			NamedPropInfo.Remote.RemoteXferSize,
			NamedPropInfo.Sharing.SharingInstanceGuid,
			NamedPropInfo.Sharing.SharingProviderGuid,
			NamedPropInfo.Sharing.SharingRemotePath,
			NamedPropInfo.Sharing.SharingRemoteName,
			NamedPropInfo.Sharing.SharingLocalName,
			NamedPropInfo.Sharing.SharingRemoteComment,
			NamedPropInfo.Sharing.SharingBrowseUrl,
			NamedPropInfo.Sharing.SharingInitiatorName,
			NamedPropInfo.Sharing.MigrationJobId,
			NamedPropInfo.Sharing.MigrationJobItemId,
			NamedPropInfo.Sharing.MigrationJobItemEmailAddress,
			NamedPropInfo.Sharing.MigrationJobItemLocalMailboxIdentifier,
			NamedPropInfo.Sharing.MigrationJobItemItemsSkipped,
			NamedPropInfo.Sharing.MigrationJobItemItemsSynced,
			NamedPropInfo.Sharing.MigrationJobItemMailboxId,
			NamedPropInfo.Sharing.MigrationJobItemMailboxLegacyDN,
			NamedPropInfo.Sharing.MigrationJobItemMRSId,
			NamedPropInfo.Sharing.MigrationLastSuccessfulSyncTime,
			NamedPropInfo.Sharing.MigrationJobItemStatus,
			NamedPropInfo.Sharing.MigrationJobItemSubscriptionStateLastChecked,
			NamedPropInfo.MRSLegacy1.MailboxMoveStatus,
			NamedPropInfo.MRSLegacy2.MoveState,
			NamedPropInfo.MRSLegacy3.MoveServerName,
			NamedPropInfo.MRSLegacy4.AllowedToFinishMove,
			NamedPropInfo.MRSLegacy5.CancelMove,
			NamedPropInfo.MRSLegacy6.ExchangeGuid,
			NamedPropInfo.MRSLegacy7.LastUpdateTimestamp,
			NamedPropInfo.MRSLegacy8.CreationTimestamp,
			NamedPropInfo.MRSLegacy9.JobType,
			NamedPropInfo.MRSLegacy10.MailboxMoveFlags,
			NamedPropInfo.MRSLegacy11.MailboxMoveSourceMDB,
			NamedPropInfo.MRSLegacy12.MailboxMoveTargetMDB,
			NamedPropInfo.MRSLegacy13.DoNotPickUntilTimestamp,
			NamedPropInfo.MRSLegacy14.RequestType,
			NamedPropInfo.MRSLegacy15.SourceArchiveDatabase,
			NamedPropInfo.MRSLegacy14.TargetArchiveDatabase,
			NamedPropInfo.MRSLegacy16.Priority,
			NamedPropInfo.MRSLegacy17.SourceExchangeGuid,
			NamedPropInfo.MRSLegacy18.TargetExchangeGuid,
			NamedPropInfo.MigrationService.RehomeRequest,
			NamedPropInfo.MigrationService.InternalFlags,
			NamedPropInfo.MigrationService.OrganizationGuid,
			NamedPropInfo.MigrationService.PoisonCount,
			NamedPropInfo.Common.MailboxAssociationExternalId,
			NamedPropInfo.Common.MailboxAssociationLegacyDN,
			NamedPropInfo.Common.MailboxAssociationIsMember,
			NamedPropInfo.Common.MailboxAssociationIsPin,
			NamedPropInfo.Common.MailboxAssociationShouldEscalate,
			NamedPropInfo.Common.MailboxAssociationIsAutoSubscribed,
			NamedPropInfo.Common.MailboxAssociationJoinDate,
			NamedPropInfo.Common.MailboxAssociationLastVisitedDate,
			NamedPropInfo.Common.MailboxAssociationPinDate,
			NamedPropInfo.Common.MailboxAssociationSmtpAddress,
			NamedPropInfo.Common.MailboxAssociationCurrentVersion,
			NamedPropInfo.Common.MailboxAssociationSyncedVersion,
			NamedPropInfo.Common.MailboxAssociationSyncedIdentityHash,
			PropTag.Message.ConversationCreatorSID.PropInfo,
			NamedPropInfo.Address.BirthdayContactAttributionDisplayName,
			NamedPropInfo.Address.BirthdayContactEntryId,
			NamedPropInfo.Address.IsBirthdayContactWritable,
			NamedPropInfo.PushNotificationSubscription.PushNotificationSubscriptionId,
			NamedPropInfo.PushNotificationSubscription.PushNotificationSubscriptionLastUpdateTimeUTC,
			NamedPropInfo.PushNotificationSubscription.SerializedPushNotificationSubscription,
			NamedPropInfo.Common.ExchangeApplicationFlags,
			PropTag.Message.SwappedTODOStore.PropInfo,
			PropTag.Message.MimeUrl.PropInfo,
			PropTag.Message.ContentId.PropInfo,
			PropTag.Message.ReplFlags.PropInfo,
			PropTag.Message.SFGAOFlags.PropInfo,
			PropTag.Message.RuleMsgName.PropInfo,
			PropTag.Message.RuleMsgProviderData.PropInfo,
			PropTag.Message.RuleMsgProvider.PropInfo,
			PropTag.Message.RuleMsgUserFlags.PropInfo,
			PropTag.Message.RuleMsgLevel.PropInfo,
			NamedPropInfo.Common.ToDoTitle,
			NamedPropInfo.UnifiedMessaging.UMAudioNotes,
			NamedPropInfo.Compliance.NeedGroupExpansion,
			PropTag.Message.ConversationFamilyId.PropInfo,
			NamedPropInfo.Inference.InferenceProcessingNeeded,
			PropTag.Message.LikeCount.PropInfo,
			NamedPropInfo.Messaging.Likers,
			PropTag.Message.PeopleCentricConversationId.PropInfo,
			NamedPropInfo.AirSync.ASLastSyncTime,
			NamedPropInfo.AirSync.ASLocalCommitTimeMax,
			NamedPropInfo.AirSync.ASDeletedCountTotal,
			NamedPropInfo.AirSync.ASSyncKey,
			NamedPropInfo.AirSync.ASFilter,
			NamedPropInfo.AirSync.ASMaxItems,
			NamedPropInfo.AirSync.ASConversationMode,
			NamedPropInfo.AirSync.ASSettingsHash,
			PropTag.Message.SyncFolderSourceKey.PropInfo
		};
	}
}

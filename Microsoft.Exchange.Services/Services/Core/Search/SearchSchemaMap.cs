using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal static class SearchSchemaMap
	{
		private static void AddMapping(PropertyUriEnum fieldUri, PropertyDefinition propDef)
		{
			SearchSchemaMap.AddMapping(fieldUri, propDef, SearchSchemaMap.SearchPropertyDirection.Both);
		}

		private static void AddReverseOnlyMapping(PropertyDefinition propDef, PropertyPath propertyPath)
		{
			if (!SearchSchemaMap.reverseMap.ContainsKey(propDef))
			{
				SearchSchemaMap.reverseMap.Add(propDef, propertyPath);
			}
		}

		private static void AddMapping(PropertyUriEnum fieldUri, PropertyDefinition propDef, SearchSchemaMap.SearchPropertyDirection direction)
		{
			SearchSchemaMap.AddMapping(new PropertyUri(fieldUri), propDef, direction);
		}

		private static void AddMapping(PropertyPath propertyPath, PropertyDefinition propDef, SearchSchemaMap.SearchPropertyDirection direction)
		{
			switch (direction)
			{
			case SearchSchemaMap.SearchPropertyDirection.Forward:
				SearchSchemaMap.forwardMap.Add(propertyPath, propDef);
				return;
			case SearchSchemaMap.SearchPropertyDirection.Reverse:
				SearchSchemaMap.AddReverseOnlyMapping(propDef, propertyPath);
				return;
			case SearchSchemaMap.SearchPropertyDirection.Both:
				SearchSchemaMap.forwardMap.Add(propertyPath, propDef);
				SearchSchemaMap.AddReverseOnlyMapping(propDef, propertyPath);
				return;
			default:
				return;
			}
		}

		private static void AddMapping(DictionaryUriEnum fieldUri, string fieldIndex, PropertyDefinition propDef)
		{
			SearchSchemaMap.AddMapping(new DictionaryPropertyUri(fieldUri, fieldIndex), propDef, SearchSchemaMap.SearchPropertyDirection.Both);
		}

		static SearchSchemaMap()
		{
			SearchSchemaMap.AddMapping(PropertyUriEnum.AllowNewTimeProposal, CalendarItemBaseSchema.DisallowNewTimeProposal);
			SearchSchemaMap.AddMapping(PropertyUriEnum.AppointmentReplyTime, CalendarItemBaseSchema.AppointmentReplyTime);
			SearchSchemaMap.AddMapping(PropertyUriEnum.AppointmentSequenceNumber, CalendarItemBaseSchema.AppointmentSequenceNumber);
			SearchSchemaMap.AddMapping(PropertyUriEnum.AppointmentState, CalendarItemBaseSchema.AppointmentState);
			SearchSchemaMap.AddMapping(PropertyUriEnum.AssistantName, ContactSchema.AssistantName);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Birthday, ContactSchema.Birthday);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Body, ItemSchema.TextBody);
			SearchSchemaMap.AddMapping(PropertyUriEnum.BusinessHomePage, ContactSchema.BusinessHomePage);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Categories, ItemSchema.Categories);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ChildFolderCount, FolderSchema.ChildCount);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Children, ContactSchema.Children);
			SearchSchemaMap.AddMapping(PropertyUriEnum.CompanyName, ContactSchema.CompanyName);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConferenceType, CalendarItemBaseSchema.ConferenceType);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationIndex, ItemSchema.ConversationIndex);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationTopic, ItemSchema.ConversationTopic);
			SearchSchemaMap.AddMapping(PropertyUriEnum.DateTimeCreated, StoreObjectSchema.CreationTime);
			SearchSchemaMap.AddMapping(PropertyUriEnum.DateTimeReceived, ItemSchema.ReceivedTime);
			SearchSchemaMap.AddMapping(PropertyUriEnum.DateTimeSent, ItemSchema.SentTime);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Department, ContactSchema.Department);
			SearchSchemaMap.AddMapping(PropertyUriEnum.DisplayCc, ItemSchema.DisplayCc);
			SearchSchemaMap.AddMapping(PropertyUriEnum.DisplayTo, ItemSchema.DisplayTo);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ReceivedOrRenewTime, ItemSchema.ReceivedOrRenewTime);
			SearchSchemaMap.AddMapping(PropertyUriEnum.End, CalendarItemInstanceSchema.EndTime);
			SearchSchemaMap.AddMapping(PropertyUriEnum.FileAs, ContactBaseSchema.FileAs);
			SearchSchemaMap.AddMapping(PropertyUriEnum.FileAsMapping, ContactSchema.FileAsId);
			SearchSchemaMap.AddMapping(PropertyUriEnum.FolderClass, StoreObjectSchema.ContainerClass);
			SearchSchemaMap.AddMapping(PropertyUriEnum.FolderDisplayName, FolderSchema.DisplayName);
			SearchSchemaMap.AddMapping(PropertyUriEnum.From, ItemSchema.SentRepresentingEmailAddress);
			SearchSchemaMap.AddMapping(PropertyUriEnum.GivenName, ContactSchema.GivenName);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Generation, ContactSchema.Generation);
			SearchSchemaMap.AddMapping(PropertyUriEnum.HasAttachments, ItemSchema.HasAttachment);
			SearchSchemaMap.AddMapping(PropertyUriEnum.HasBeenProcessed, MeetingMessageInstanceSchema.IsProcessed);
			SearchSchemaMap.AddMapping(PropertyUriEnum.HasPicture, ContactSchema.HasPicturePropertyDef);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Importance, ItemSchema.Importance);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Initials, ContactSchema.Initials);
			SearchSchemaMap.AddMapping(PropertyUriEnum.InReplyTo, ItemSchema.InReplyTo);
			SearchSchemaMap.AddMapping(PropertyUriEnum.InstanceKey, ItemSchema.InstanceKey);
			SearchSchemaMap.AddMapping(PropertyUriEnum.IntendedFreeBusyStatus, CalendarItemBaseSchema.IntendedFreeBusyStatus);
			SearchSchemaMap.AddMapping(PropertyUriEnum.InternetMessageHeaders, MessageItemSchema.TransportMessageHeaders);
			SearchSchemaMap.AddMapping(PropertyUriEnum.InternetMessageId, ItemSchema.InternetMessageId);
			SearchSchemaMap.AddMapping(PropertyUriEnum.IsAllDayEvent, CalendarItemBaseSchema.MapiIsAllDayEvent);
			SearchSchemaMap.AddMapping(PropertyUriEnum.IsDraft, MessageItemSchema.IsDraft);
			SearchSchemaMap.AddMapping(PropertyUriEnum.IsOnlineMeeting, CalendarItemBaseSchema.IsOnlineMeeting);
			SearchSchemaMap.AddMapping(PropertyUriEnum.IsRead, MessageItemSchema.IsRead);
			SearchSchemaMap.AddMapping(PropertyUriEnum.IsRecurring, CalendarItemBaseSchema.IsRecurring);
			SearchSchemaMap.AddMapping(PropertyUriEnum.IsResponseRequested, ItemSchema.IsResponseRequested);
			SearchSchemaMap.AddMapping(PropertyUriEnum.IsUnmodified, ItemSchema.IsUnmodified);
			SearchSchemaMap.AddMapping(PropertyUriEnum.IsSubmitted, MessageItemSchema.HasBeenSubmitted);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ItemClass, StoreObjectSchema.ItemClass);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ItemId, ItemSchema.Id);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ItemLastModifiedTime, StoreObjectSchema.LastModifiedTime);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ItemParentId, StoreObjectSchema.ParentItemId);
			SearchSchemaMap.AddMapping(PropertyUriEnum.JobTitle, ContactSchema.Title);
			SearchSchemaMap.AddMapping(PropertyUriEnum.LastModifiedName, ItemSchema.LastModifiedBy);
			SearchSchemaMap.AddMapping(PropertyUriEnum.LegacyFreeBusyStatus, CalendarItemBaseSchema.FreeBusyStatus);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Location, CalendarItemBaseSchema.Location);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Manager, ContactSchema.Manager);
			SearchSchemaMap.AddMapping(PropertyUriEnum.MeetingRequestWasSent, CalendarItemBaseSchema.MeetingRequestWasSent);
			SearchSchemaMap.AddMapping(PropertyUriEnum.MeetingWorkspaceUrl, CalendarItemBaseSchema.MeetingWorkspaceUrl);
			SearchSchemaMap.AddMapping(PropertyUriEnum.MiddleName, ContactSchema.MiddleName);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Mileage, ContactSchema.Mileage);
			SearchSchemaMap.AddMapping(PropertyUriEnum.MyResponseType, CalendarItemBaseSchema.ResponseType);
			SearchSchemaMap.AddMapping(PropertyUriEnum.NetShowUrl, CalendarItemBaseSchema.NetShowURL);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Nickname, ContactSchema.Nickname);
			SearchSchemaMap.AddMapping(PropertyUriEnum.OfficeLocation, ContactSchema.OfficeLocation);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Organizer, CalendarItemBaseSchema.OrganizerEmailAddress);
			SearchSchemaMap.AddMapping(PropertyUriEnum.PostedTime, ItemSchema.SentTime);
			SearchSchemaMap.AddMapping(PropertyUriEnum.PostalAddressIndex, ContactSchema.PostalAddressId);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ParentFolderId, StoreObjectSchema.ParentItemId);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Profession, ContactSchema.Profession);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ReceivedBy, MessageItemSchema.ReceivedByEmailAddress);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ReceivedRepresenting, MessageItemSchema.ReceivedRepresentingEmailAddress);
			SearchSchemaMap.AddMapping(PropertyUriEnum.References, ItemSchema.InternetReferences);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ReminderIsSet, ItemSchema.ReminderIsSet);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ReminderMinutesBeforeStart, ItemSchema.ReminderMinutesBeforeStart);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ResponseType, CalendarItemBaseSchema.ResponseType);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Sender, MessageItemSchema.SenderEmailAddress);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Sensitivity, ItemSchema.Sensitivity);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Size, ItemSchema.Size);
			SearchSchemaMap.AddMapping(PropertyUriEnum.SpouseName, ContactSchema.SpouseName);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Start, CalendarItemInstanceSchema.StartTime);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Subject, ItemSchema.Subject);
			SearchSchemaMap.AddMapping(PropertyUriEnum.Surname, ContactSchema.Surname);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TotalCount, FolderSchema.ItemCount);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TimeZone, CalendarItemBaseSchema.TimeZone);
			SearchSchemaMap.AddMapping(PropertyUriEnum.UnreadCount, FolderSchema.UnreadCount);
			SearchSchemaMap.AddMapping(PropertyUriEnum.WeddingAnniversary, ContactSchema.WeddingAnniversary);
			SearchSchemaMap.AddMapping(PropertyUriEnum.When, CalendarItemBaseSchema.When);
			SearchSchemaMap.AddMapping(PropertyUriEnum.IsReadReceiptRequested, MessageItemSchema.IsReadReceiptRequested);
			SearchSchemaMap.AddMapping(PropertyUriEnum.IsDeliveryReceiptRequested, MessageItemSchema.IsDeliveryReceiptRequested);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.EmailAddress, "EmailAddress1", ContactSchema.Email1EmailAddress);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.EmailAddress, "EmailAddress2", ContactSchema.Email2EmailAddress);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.EmailAddress, "EmailAddress3", ContactSchema.Email3EmailAddress);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.ImAddress, "ImAddress1", ContactSchema.IMAddress);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.ImAddress, "ImAddress2", ContactSchema.IMAddress2);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.ImAddress, "ImAddress3", ContactSchema.IMAddress3);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "AssistantPhone", ContactSchema.AssistantPhoneNumber);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "BusinessFax", ContactSchema.WorkFax);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "BusinessPhone", ContactSchema.BusinessPhoneNumber);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "BusinessPhone2", ContactSchema.BusinessPhoneNumber2);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "Callback", ContactSchema.CallbackPhone);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "CarPhone", ContactSchema.CarPhone);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "CompanyMainPhone", ContactSchema.OrganizationMainPhone);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "HomeFax", ContactSchema.HomeFax);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "HomePhone", ContactSchema.HomePhone);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "HomePhone2", ContactSchema.HomePhone2);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "Isdn", ContactSchema.InternationalIsdnNumber);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "MobilePhone", ContactSchema.MobilePhone);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "OtherFax", ContactSchema.OtherFax);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "OtherTelephone", ContactSchema.OtherTelephone);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "Pager", ContactSchema.Pager);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "PrimaryPhone", ContactSchema.PrimaryTelephoneNumber);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "RadioPhone", ContactSchema.RadioPhone);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "Telex", ContactSchema.TelexNumber);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhoneNumber, "TtyTddPhone", ContactSchema.TtyTddPhoneNumber);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhysicalAddressCity, "Business", ContactSchema.WorkAddressCity);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhysicalAddressCountryOrRegion, "Business", ContactSchema.WorkAddressCountry);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhysicalAddressPostalCode, "Business", ContactSchema.WorkAddressPostalCode);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhysicalAddressState, "Business", ContactSchema.WorkAddressState);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhysicalAddressStreet, "Business", ContactSchema.WorkAddressStreet);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhysicalAddressCity, "Home", ContactSchema.HomeCity);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhysicalAddressCountryOrRegion, "Home", ContactSchema.HomeCountry);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhysicalAddressPostalCode, "Home", ContactSchema.HomePostalCode);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhysicalAddressState, "Home", ContactSchema.HomeState);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhysicalAddressStreet, "Home", ContactSchema.HomeStreet);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhysicalAddressCity, "Other", ContactSchema.OtherCity);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhysicalAddressCountryOrRegion, "Other", ContactSchema.OtherCountry);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhysicalAddressPostalCode, "Other", ContactSchema.OtherPostalCode);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhysicalAddressState, "Other", ContactSchema.OtherState);
			SearchSchemaMap.AddMapping(DictionaryUriEnum.PhysicalAddressStreet, "Other", ContactSchema.OtherStreet);
			SearchSchemaMap.AddMapping(PropertyUriEnum.DisplayName, StoreObjectSchema.DisplayName, SearchSchemaMap.SearchPropertyDirection.Forward);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskActualWork, TaskSchema.ActualWork);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskAssignedTime, TaskSchema.AssignedTime);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskBillingInformation, TaskSchema.BillingInformation);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskChangeCount, TaskSchema.TaskChangeCount);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskCompanies, TaskSchema.Companies);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskCompleteDate, ItemSchema.CompleteDate);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskContacts, TaskSchema.Contacts);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskDelegationState, TaskSchema.DelegationState);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskDelegator, TaskSchema.TaskDelegator);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskDueDate, TaskSchema.DueDate);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskIsAssignmentEditable, TaskSchema.IsAssignmentEditable);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskIsTaskRecurring, TaskSchema.IsTaskRecurring);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskIsTeamTask, TaskSchema.IsTeamTask);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskMileage, TaskSchema.Mileage);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskOwner, TaskSchema.TaskOwner);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskPercentComplete, ItemSchema.PercentComplete);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskStartDate, TaskSchema.StartDate);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskStatus, ItemSchema.TaskStatus);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskTotalWork, TaskSchema.TotalWork);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskIsComplete, ItemSchema.IsComplete);
			SearchSchemaMap.AddMapping(PropertyUriEnum.TaskDoItTime, TaskSchema.DoItTime);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationId, ItemSchema.ConversationId);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationUniqueRecipients, ConversationItemSchema.ConversationMVTo);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationGlobalUniqueRecipients, ConversationItemSchema.ConversationGlobalMVTo);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationUniqueUnreadSenders, ConversationItemSchema.ConversationMVUnreadFrom);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationGlobalUniqueUnreadSenders, ConversationItemSchema.ConversationGlobalMVUnreadFrom);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationUniqueSenders, ConversationItemSchema.ConversationMVFrom);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationGlobalUniqueSenders, ConversationItemSchema.ConversationGlobalMVFrom);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationLastDeliveryTime, ConversationItemSchema.ConversationLastDeliveryTime);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationGlobalLastDeliveryTime, ConversationItemSchema.ConversationGlobalLastDeliveryTime);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationFlagStatus, ConversationItemSchema.ConversationFlagStatus);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationGlobalFlagStatus, ConversationItemSchema.ConversationGlobalFlagStatus);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationHasAttachments, ConversationItemSchema.ConversationHasAttach);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationGlobalHasAttachments, ConversationItemSchema.ConversationGlobalHasAttach);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationMessageCount, ConversationItemSchema.ConversationMessageCount);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationGlobalMessageCount, ConversationItemSchema.ConversationGlobalMessageCount);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationUnreadCount, ConversationItemSchema.ConversationUnreadMessageCount);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationGlobalUnreadCount, ConversationItemSchema.ConversationGlobalUnreadMessageCount);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationSize, ConversationItemSchema.ConversationMessageSize);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationGlobalSize, ConversationItemSchema.ConversationGlobalMessageSize);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationImportance, ConversationItemSchema.ConversationImportance);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationGlobalImportance, ConversationItemSchema.ConversationGlobalImportance);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationInstanceKey, ItemSchema.InstanceKey);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationItemClasses, ConversationItemSchema.ConversationMessageClasses);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationGlobalLastDeliveryOrRenewTime, ConversationItemSchema.ConversationGlobalLastDeliveryOrRenewTime);
			SearchSchemaMap.AddMapping(PropertyUriEnum.ConversationLastDeliveryOrRenewTime, ConversationItemSchema.ConversationLastDeliveryOrRenewTime);
			SearchSchemaMap.AddMapping(PropertyUriEnum.PersonaGivenName, PersonSchema.GivenName);
			SearchSchemaMap.AddMapping(PropertyUriEnum.PersonaSurname, PersonSchema.Surname);
			SearchSchemaMap.AddMapping(PropertyUriEnum.PersonaCompanyName, PersonSchema.CompanyName);
			SearchSchemaMap.AddMapping(PropertyUriEnum.PersonaDisplayName, PersonSchema.DisplayName);
			SearchSchemaMap.AddMapping(PropertyUriEnum.PersonaEmailAddresses, PersonSchema.EmailAddresses);
			SearchSchemaMap.AddMapping(PropertyUriEnum.PersonaFileAs, PersonSchema.FileAs);
			SearchSchemaMap.AddMapping(PropertyUriEnum.PersonaHomeCity, PersonSchema.HomeCity);
			SearchSchemaMap.AddMapping(PropertyUriEnum.PersonaCreationTime, PersonSchema.CreationTime);
			SearchSchemaMap.AddMapping(PropertyUriEnum.PersonaImAddress, PersonSchema.IMAddress);
			SearchSchemaMap.AddMapping(PropertyUriEnum.PersonaRelevanceScore, PersonSchema.RelevanceScore);
			SearchSchemaMap.AddMapping(PropertyUriEnum.PersonaWorkCity, PersonSchema.WorkCity);
			SearchSchemaMap.AddMapping(PropertyUriEnum.PersonaDisplayNameFirstLast, PersonSchema.DisplayNameFirstLast);
			SearchSchemaMap.AddMapping(PropertyUriEnum.PersonaDisplayNameLastFirst, PersonSchema.DisplayNameLastFirst);
			SearchSchemaMap.AddMapping(PropertyUriEnum.PersonaEmailAddress, PersonSchema.EmailAddress);
			SearchSchemaMap.AddMapping(PropertyUriEnum.PersonaPostalAddress, PersonSchema.PostalAddress);
			SearchSchemaMap.AddMapping(PropertyUriEnum.PersonaType, PersonSchema.PersonType);
			SearchSchemaMap.AddReverseOnlyMapping(PropertyTagPropertyDefinition.CreateCustom("Subject", 3604511U), new PropertyUri(PropertyUriEnum.Subject));
			SearchSchemaMap.AddReverseOnlyMapping(ItemSchema.FlagStatus, new ExtendedPropertyUri
			{
				PropertyTag = "0x1090",
				PropertyType = MapiPropertyType.Integer
			});
			SearchSchemaMap.AddReverseOnlyMapping(ItemSchema.SentRepresentingDisplayName, new ExtendedPropertyUri
			{
				PropertyTag = "0x0042",
				PropertyType = MapiPropertyType.String
			});
			SearchSchemaMap.AddReverseOnlyMapping(ItemSchema.TextBody, new PropertyUri(PropertyUriEnum.Body));
			SearchSchemaMap.AddReverseOnlyMapping(ItemSchema.SentRepresentingDisplayName, new PropertyUri(PropertyUriEnum.From));
			SearchSchemaMap.AddReverseOnlyMapping(MessageItemSchema.SenderDisplayName, new PropertyUri(PropertyUriEnum.Sender));
			SearchSchemaMap.AddReverseOnlyMapping(CalendarItemBaseSchema.OrganizerDisplayName, new PropertyUri(PropertyUriEnum.Organizer));
			SearchSchemaMap.AddReverseOnlyMapping(ItemSchema.ReminderDueBy, new PropertyUri(PropertyUriEnum.ReminderDueBy));
			SearchSchemaMap.AddReverseOnlyMapping(ItemSchema.ReminderNextTime, new PropertyUri(PropertyUriEnum.ReminderNextTime));
			SearchSchemaMap.AddReverseOnlyMapping(MeetingMessageSchema.IsOutOfDate, new PropertyUri(PropertyUriEnum.IsOutOfDate));
			SearchSchemaMap.AddReverseOnlyMapping(MessageItemSchema.ReplyToNames, new PropertyUri(PropertyUriEnum.ReplyTo));
			SearchSchemaMap.AddReverseOnlyMapping(CalendarItemBaseSchema.IsAllDayEvent, new PropertyUri(PropertyUriEnum.IsAllDayEvent));
			SearchSchemaMap.AddReverseOnlyMapping(CalendarItemBaseSchema.IsMeeting, new PropertyUri(PropertyUriEnum.IsMeeting));
			SearchSchemaMap.AddReverseOnlyMapping(CalendarItemBaseSchema.CalendarItemType, new PropertyUri(PropertyUriEnum.CalendarItemType));
			SearchSchemaMap.AddReverseOnlyMapping(FolderSchema.AdminFolderFlags, new PropertyUri(PropertyUriEnum.ManagedFolderInformation));
			SearchSchemaMap.AddReverseOnlyMapping(FolderSchema.ELCPolicyIds, new PropertyUri(PropertyUriEnum.ManagedFolderInformation));
			SearchSchemaMap.AddReverseOnlyMapping(FolderSchema.ELCFolderComment, new PropertyUri(PropertyUriEnum.ManagedFolderInformation));
			SearchSchemaMap.AddReverseOnlyMapping(FolderSchema.FolderQuota, new PropertyUri(PropertyUriEnum.ManagedFolderInformation));
			SearchSchemaMap.AddReverseOnlyMapping(FolderSchema.FolderSize, new PropertyUri(PropertyUriEnum.ManagedFolderInformation));
			SearchSchemaMap.AddReverseOnlyMapping(FolderSchema.FolderHomePageUrl, new PropertyUri(PropertyUriEnum.ManagedFolderInformation));
			SearchSchemaMap.subFilterToPTEMap.Add(SubFilterType.Attachment, new PropertyUri(PropertyUriEnum.Attachments));
			SearchSchemaMap.subFilterToPTEMap.Add(SubFilterType.AttendeeOptional, new PropertyUri(PropertyUriEnum.OptionalAttendees));
			SearchSchemaMap.subFilterToPTEMap.Add(SubFilterType.AttendeeRequired, new PropertyUri(PropertyUriEnum.RequiredAttendees));
			SearchSchemaMap.subFilterToPTEMap.Add(SubFilterType.AttendeeResource, new PropertyUri(PropertyUriEnum.Resources));
			SearchSchemaMap.subFilterToPTEMap.Add(SubFilterType.RecipientBcc, new PropertyUri(PropertyUriEnum.BccRecipients));
			SearchSchemaMap.subFilterToPTEMap.Add(SubFilterType.RecipientCc, new PropertyUri(PropertyUriEnum.CcRecipients));
			SearchSchemaMap.subFilterToPTEMap.Add(SubFilterType.RecipientTo, new PropertyUri(PropertyUriEnum.ToRecipients));
		}

		public static PropertyPath GetPathToElementForSubFilter(SubFilterType subFilterType)
		{
			return SearchSchemaMap.subFilterToPTEMap[subFilterType];
		}

		public static PropertyPath GetPropertyPath(PropertyDefinition propertyDefinition)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition", "[SearchSchemaMap::GetPropertyPath] property Definition is null");
			}
			PropertyPath result = null;
			if (!SearchSchemaMap.TryGetPropertyPath(propertyDefinition, out result))
			{
				throw new UnsupportedPropertyDefinitionException(propertyDefinition.Name);
			}
			return result;
		}

		public static bool TryGetPropertyPath(PropertyDefinition propertyDefinition, out PropertyPath propertyPath)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition", "[SearchSchemaMap::TryGetPropertyPath] property Definition is null");
			}
			if (SearchSchemaMap.reverseMap.TryGetValue(propertyDefinition, out propertyPath))
			{
				return true;
			}
			NativeStorePropertyDefinition nativeStorePropertyDefinition = propertyDefinition as NativeStorePropertyDefinition;
			if (nativeStorePropertyDefinition != null)
			{
				propertyPath = new ExtendedPropertyUri(nativeStorePropertyDefinition);
				return true;
			}
			propertyPath = null;
			return false;
		}

		public static bool TryGetPropertyDefinition(PropertyPath propertyPath, out PropertyDefinition propertyDefinition)
		{
			DictionaryPropertyUri dictionaryPropertyUri = propertyPath as DictionaryPropertyUri;
			if (dictionaryPropertyUri != null && dictionaryPropertyUri.Uri == DictionaryUriEnum.InternetMessageHeader)
			{
				propertyDefinition = PropertyDefinitionHelper.GenerateInternetHeaderPropertyDefinition(dictionaryPropertyUri.Key);
				return true;
			}
			ExtendedPropertyUri extendedPropertyUri = propertyPath as ExtendedPropertyUri;
			if (extendedPropertyUri != null)
			{
				propertyDefinition = extendedPropertyUri.ToPropertyDefinition();
				return true;
			}
			return SearchSchemaMap.forwardMap.TryGetValue(propertyPath, out propertyDefinition);
		}

		internal static PropertyPath[] GetPropertyPaths(PropertyError[] propertyErrors)
		{
			PropertyPath[] array = new PropertyPath[propertyErrors.Length];
			int num = 0;
			foreach (PropertyError propertyError in propertyErrors)
			{
				PropertyPath propertyPath = SearchSchemaMap.GetPropertyPath(propertyError.PropertyDefinition);
				array[num] = propertyPath;
				num++;
			}
			return array;
		}

		internal static PropertyPath[] GetPropertyPaths(PropertyValidationError[] propertyErrors)
		{
			PropertyPath[] array = new PropertyPath[propertyErrors.Length];
			int num = 0;
			foreach (PropertyValidationError propertyValidationError in propertyErrors)
			{
				PropertyPath propertyPath = SearchSchemaMap.GetPropertyPath(propertyValidationError.PropertyDefinition);
				array[num] = propertyPath;
				num++;
			}
			return array;
		}

		internal static PropertyPath[] GetPropertyPaths(IList<StoreObjectValidationError> propertyErrors)
		{
			List<PropertyPath> list = new List<PropertyPath>();
			foreach (StoreObjectValidationError storeObjectValidationError in propertyErrors)
			{
				if (storeObjectValidationError.PropertyDefinition != null)
				{
					PropertyPath propertyPath = SearchSchemaMap.GetPropertyPath(storeObjectValidationError.PropertyDefinition);
					list.Add(propertyPath);
				}
			}
			return list.ToArray();
		}

		public static bool TryGetPropertyDefinition(XmlElement element, out PropertyDefinition propertyDefinition)
		{
			if (ExtendedPropertyUri.IsExtendedPropertyUriXml(element))
			{
				propertyDefinition = ExtendedPropertyUri.Parse(element).ToPropertyDefinition();
				return true;
			}
			if (PropertyUri.IsPropertyUriXml(element))
			{
				PropertyUri propertyPath = PropertyUri.Parse(element);
				return SearchSchemaMap.TryGetPropertyDefinition(propertyPath, out propertyDefinition);
			}
			if (!DictionaryPropertyUri.IsDictionaryPropertyUriXml(element))
			{
				propertyDefinition = null;
				return false;
			}
			DictionaryPropertyUri dictionaryPropertyUri = DictionaryPropertyUri.Parse(element);
			if (dictionaryPropertyUri.Uri == DictionaryUriEnum.InternetMessageHeader)
			{
				propertyDefinition = PropertyDefinitionHelper.GenerateInternetHeaderPropertyDefinition(dictionaryPropertyUri.Key);
				return true;
			}
			return SearchSchemaMap.TryGetPropertyDefinition(dictionaryPropertyUri, out propertyDefinition);
		}

		private static Dictionary<PropertyPath, PropertyDefinition> forwardMap = new Dictionary<PropertyPath, PropertyDefinition>();

		private static Dictionary<PropertyDefinition, PropertyPath> reverseMap = new Dictionary<PropertyDefinition, PropertyPath>();

		private static Dictionary<SubFilterType, PropertyPath> subFilterToPTEMap = new Dictionary<SubFilterType, PropertyPath>();

		private enum SearchPropertyDirection
		{
			Forward = 1,
			Reverse,
			Both
		}
	}
}

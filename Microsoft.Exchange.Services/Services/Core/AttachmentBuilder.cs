using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class AttachmentBuilder : IDisposeTrackable, IDisposable
	{
		static AttachmentBuilder()
		{
			AttachmentBuilder.storeObjectTypeMapper.Add(typeof(ItemType), StoreObjectType.Note);
			AttachmentBuilder.storeObjectTypeMapper.Add(typeof(MessageType), StoreObjectType.Message);
			AttachmentBuilder.storeObjectTypeMapper.Add(typeof(EwsCalendarItemType), StoreObjectType.CalendarItem);
			AttachmentBuilder.storeObjectTypeMapper.Add(typeof(ContactItemType), StoreObjectType.Contact);
			AttachmentBuilder.storeObjectTypeMapper.Add(typeof(TaskType), StoreObjectType.Task);
			AttachmentBuilder.storeObjectTypeMapper.Add(typeof(PostItemType), StoreObjectType.Post);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AttachmentBuilder>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public AttachmentBuilder(AttachmentHierarchy attachments, AttachmentType[] attachmentTypes, IdConverter idConverter) : this(attachments, attachmentTypes, idConverter, false)
		{
		}

		public AttachmentBuilder(AttachmentHierarchy attachments, AttachmentType[] attachmentTypes, IdConverter idConverter, bool clientSupportsIrm)
		{
			this.attachmentTypes = attachmentTypes;
			this.attachments = attachments;
			this.idConverter = idConverter;
			this.clientSupportsIrm = clientSupportsIrm;
			this.disposeTracker = this.GetDisposeTracker();
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private static ExceptionPropertyUriEnum GetPropertyUri(string elementName)
		{
			ExceptionPropertyUriEnum result = ExceptionPropertyUriEnum.Month;
			if (elementName != null)
			{
				if (!(elementName == "Content"))
				{
					if (!(elementName == "Name"))
					{
						if (elementName == "ContentType")
						{
							result = ExceptionPropertyUriEnum.ContentType;
						}
					}
					else
					{
						result = ExceptionPropertyUriEnum.AttachmentName;
					}
				}
				else
				{
					result = ExceptionPropertyUriEnum.Content;
				}
			}
			return result;
		}

		private static void SetStandardAttachmentProperties(Attachment attachment, AttachmentType attachmentType)
		{
			if (string.IsNullOrEmpty(attachmentType.Name))
			{
				throw new RequiredPropertyMissingException(new ExceptionPropertyUri(AttachmentBuilder.GetPropertyUri("Name")));
			}
			attachment.FileName = attachmentType.Name;
			if (!string.IsNullOrEmpty(attachmentType.ContentType))
			{
				attachment.ContentType = attachmentType.ContentType;
			}
			if (!string.IsNullOrEmpty(attachmentType.ContentId))
			{
				attachment[AttachmentSchema.AttachContentId] = attachmentType.ContentId;
			}
			if (!string.IsNullOrEmpty(attachmentType.ContentLocation))
			{
				attachment[AttachmentSchema.AttachContentLocation] = attachmentType.ContentLocation;
			}
			attachment.IsInline = attachmentType.IsInline;
			if (attachmentType is FileAttachmentType)
			{
				bool isContactPhoto = (attachmentType as FileAttachmentType).IsContactPhoto;
				attachment[AttachmentSchema.IsContactPhoto] = isContactPhoto;
				if (isContactPhoto)
				{
					attachment[AttachmentSchema.DisplayName] = "ContactPicture.jpg";
					attachment.FileName = "ContactPicture.jpg";
				}
			}
		}

		public void CreateAllAttachments()
		{
			foreach (AttachmentType attachmentType in this.attachmentTypes)
			{
				ServiceError serviceError;
				using (this.CreateAttachment(attachmentType, out serviceError))
				{
				}
			}
		}

		public Attachment CreateAttachment(AttachmentType attachmentType, out ServiceError warning)
		{
			warning = null;
			CalendarItemOccurrence calendarItemOccurrence = this.attachments.LastAsXsoItem as CalendarItemOccurrence;
			if (calendarItemOccurrence != null && calendarItemOccurrence.CalendarItemType != CalendarItemType.Exception)
			{
				calendarItemOccurrence.MakeModifiedOccurrence();
			}
			if (string.Compare(attachmentType.ContentType, "text/html", StringComparison.OrdinalIgnoreCase) == 0)
			{
				attachmentType.IsInline = false;
			}
			Type type = attachmentType.GetType();
			if (type == typeof(FileAttachmentType))
			{
				FileAttachmentType fileAttachmentType = attachmentType as FileAttachmentType;
				if (fileAttachmentType.IsContactPhoto)
				{
					this.DeleteExistingContactPhotoAttachments();
				}
				this.attachments.RootItem[ContactSchema.HasPicturePropertyDef] = true;
				return this.CreateFileAttachment(fileAttachmentType);
			}
			if (type == typeof(ReferenceAttachmentType))
			{
				return this.CreateReferenceAttachment(attachmentType as ReferenceAttachmentType);
			}
			if (type == typeof(ItemIdAttachmentType))
			{
				return this.CreateItemAttachment(attachmentType as ItemIdAttachmentType);
			}
			return this.CreateItemAttachment(attachmentType as ItemAttachmentType, out warning);
		}

		private void DeleteExistingContactPhotoAttachments()
		{
			List<AttachmentId> list = new List<AttachmentId>();
			AttachmentCollection attachmentCollection = IrmUtils.GetAttachmentCollection(this.attachments.RootItem);
			foreach (AttachmentHandle handle in attachmentCollection)
			{
				using (Attachment attachment = attachmentCollection.Open(handle))
				{
					if (attachment.IsContactPhoto)
					{
						list.Add(attachment.Id);
					}
				}
			}
			foreach (AttachmentId attachmentId in list)
			{
				attachmentCollection.Remove(attachmentId);
			}
		}

		private Attachment CreateFileAttachment(FileAttachmentType fileAttachmentType)
		{
			AttachmentCollection attachmentCollection = IrmUtils.GetAttachmentCollection(this.attachments.LastAsXsoItem);
			StreamAttachment streamAttachment = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				streamAttachment = (attachmentCollection.Create(AttachmentType.Stream) as StreamAttachment);
				disposeGuard.Add<StreamAttachment>(streamAttachment);
				AttachmentBuilder.SetStandardAttachmentProperties(streamAttachment, fileAttachmentType);
				byte[] content = fileAttachmentType.Content;
				if (content == null)
				{
					throw new RequiredPropertyMissingException(new ExceptionPropertyUri(AttachmentBuilder.GetPropertyUri("Content")));
				}
				if (content.Length > 2147483647)
				{
					throw new AttachmentSizeLimitExceededException();
				}
				using (Stream contentStream = streamAttachment.GetContentStream())
				{
					contentStream.Write(content, 0, content.Length);
				}
				streamAttachment.Save();
				disposeGuard.Success();
			}
			return streamAttachment;
		}

		private Attachment CreateItemAttachment(ItemAttachmentType itemAttachmentType, out ServiceError warning)
		{
			warning = null;
			ServiceObject item = itemAttachmentType.Item;
			if (item == null)
			{
				throw new MissingItemForCreateItemAttachmentException();
			}
			StoreObjectType type;
			try
			{
				type = AttachmentBuilder.storeObjectTypeMapper[item.GetType()];
			}
			catch (KeyNotFoundException)
			{
				throw new InvalidItemForOperationException("CreateItemAttachment");
			}
			if ((item is EwsCalendarItemType || item is TaskType) && !ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010))
			{
				TimeZoneType meetingTimeZone = item.PropertyBag.Contains(CalendarItemSchema.MeetingTimeZone) ? (item.PropertyBag[CalendarItemSchema.MeetingTimeZone] as TimeZoneType) : null;
				this.attachments.RootItem.Session.ExTimeZone = RecurrenceHelper.MeetingTimeZone.GetMeetingTimeZone(meetingTimeZone, out warning);
			}
			AttachmentCollection attachmentCollection = IrmUtils.GetAttachmentCollection(this.attachments.LastAsXsoItem);
			ItemAttachment itemAttachment = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				itemAttachment = attachmentCollection.Create(type);
				disposeGuard.Add<ItemAttachment>(itemAttachment);
				AttachmentBuilder.SetStandardAttachmentProperties(itemAttachment, itemAttachmentType);
				using (Item item2 = itemAttachment.GetItem())
				{
					if (item.LoadedProperties.Count > 0)
					{
						string className = item2.ClassName;
						XsoDataConverter.SetProperties(item2, item, this.idConverter);
						ServiceCommandBase.ValidateClassChange(item2, className);
					}
					item2.Save(SaveMode.NoConflictResolution);
				}
				itemAttachment.Save();
				disposeGuard.Success();
			}
			return itemAttachment;
		}

		private Attachment CreateItemAttachment(ItemIdAttachmentType itemIdAttachmentType)
		{
			IdAndSession idAndSession;
			if (itemIdAttachmentType.ItemId != null)
			{
				idAndSession = this.idConverter.ConvertItemIdToIdAndSessionReadOnly(itemIdAttachmentType.ItemId);
			}
			else
			{
				if (string.IsNullOrEmpty(itemIdAttachmentType.AttachmentIdToAttach))
				{
					throw new MissingItemIdForCreateItemAttachmentException();
				}
				idAndSession = this.idConverter.ConvertAttachmentIdToIdAndSessionReadOnly(new AttachmentIdType(itemIdAttachmentType.AttachmentIdToAttach));
			}
			Item lastAsXsoItem = this.attachments.LastAsXsoItem;
			if (idAndSession.Id.Equals(lastAsXsoItem.Id) || idAndSession.Id.Equals(lastAsXsoItem.Id.ObjectId))
			{
				throw new CannotAttachSelfException();
			}
			AttachmentCollection attachmentCollection = IrmUtils.GetAttachmentCollection(this.attachments.LastAsXsoItem);
			if (itemIdAttachmentType.ItemId != null)
			{
				using (Item rootXsoItem = idAndSession.GetRootXsoItem(null))
				{
					ItemAttachment itemAttachment = null;
					using (DisposeGuard disposeGuard = default(DisposeGuard))
					{
						itemAttachment = attachmentCollection.AddExistingItem(rootXsoItem);
						disposeGuard.Add<ItemAttachment>(itemAttachment);
						itemAttachment[AttachmentSchema.DisplayName] = itemIdAttachmentType.Name;
						itemAttachment.Save();
						disposeGuard.Success();
					}
					return itemAttachment;
				}
			}
			Attachment result;
			using (AttachmentHierarchy attachmentHierarchy = new AttachmentHierarchy(idAndSession, false, this.clientSupportsIrm))
			{
				Attachment attachment = attachmentHierarchy.Last.Attachment;
				Attachment attachment2 = null;
				using (DisposeGuard disposeGuard2 = default(DisposeGuard))
				{
					attachment2 = attachmentCollection.Create(new AttachmentType?(attachment.AttachmentType), attachment);
					disposeGuard2.Add<Attachment>(attachment2);
					attachment2.Save();
					disposeGuard2.Success();
				}
				result = attachment2;
			}
			return result;
		}

		private Attachment CreateReferenceAttachment(ReferenceAttachmentType referenceAttachmentType)
		{
			AttachmentCollection attachmentCollection = IrmUtils.GetAttachmentCollection(this.attachments.LastAsXsoItem);
			ReferenceAttachment referenceAttachment = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				referenceAttachment = (attachmentCollection.Create(AttachmentType.Reference) as ReferenceAttachment);
				disposeGuard.Add<ReferenceAttachment>(referenceAttachment);
				AttachmentBuilder.SetStandardAttachmentProperties(referenceAttachment, referenceAttachmentType);
				string attachLongPathName = referenceAttachmentType.AttachLongPathName;
				if (string.IsNullOrWhiteSpace(attachLongPathName))
				{
					throw new RequiredPropertyMissingException(new ExceptionPropertyUri(AttachmentBuilder.GetPropertyUri("AttachLongPathName")));
				}
				string providerEndpointUrl = referenceAttachmentType.ProviderEndpointUrl;
				if (string.IsNullOrWhiteSpace(providerEndpointUrl))
				{
					throw new RequiredPropertyMissingException(new ExceptionPropertyUri(AttachmentBuilder.GetPropertyUri("ProviderEndpointUrl")));
				}
				string providerType = referenceAttachmentType.ProviderType;
				if (string.IsNullOrWhiteSpace(providerType))
				{
					throw new RequiredPropertyMissingException(new ExceptionPropertyUri(AttachmentBuilder.GetPropertyUri("ProviderType")));
				}
				referenceAttachment[AttachmentSchema.AttachLongPathName] = attachLongPathName;
				referenceAttachment[AttachmentSchema.AttachmentProviderEndpointUrl] = providerEndpointUrl;
				referenceAttachment[AttachmentSchema.AttachmentProviderType] = providerType;
				referenceAttachment.Save();
				disposeGuard.Success();
			}
			return referenceAttachment;
		}

		private void Dispose(bool fromDispose)
		{
			if (this.attachments != null)
			{
				this.attachments.Dispose();
				this.attachments = null;
			}
		}

		private const string ContactPictureName = "ContactPicture.jpg";

		private readonly DisposeTracker disposeTracker;

		private readonly IdConverter idConverter;

		private readonly AttachmentType[] attachmentTypes;

		private readonly bool clientSupportsIrm;

		private static Dictionary<Type, StoreObjectType> storeObjectTypeMapper = new Dictionary<Type, StoreObjectType>();

		private AttachmentHierarchy attachments;
	}
}

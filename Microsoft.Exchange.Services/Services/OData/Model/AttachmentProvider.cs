using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.ExchangeService;
using Microsoft.Exchange.Services.OData.Web;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class AttachmentProvider : ExchangeServiceProvider
	{
		public AttachmentProvider(IExchangeService exchangeService) : base(exchangeService)
		{
		}

		public IFindEntitiesResult<Attachment> Find(string rootItemId, AttachmentQueryAdapter queryAdapter = null)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("rootItemId", rootItemId);
			queryAdapter = (queryAdapter ?? AttachmentQueryAdapter.Default);
			string ewsRootItemId = EwsIdConverter.ODataIdToEwsId(rootItemId);
			string parentItemNavigationName = null;
			AttachmentType[] itemAttachments = this.GetItemAttachments(ewsRootItemId, out parentItemNavigationName);
			List<Attachment> list = new List<Attachment>();
			int totalCount = (itemAttachments == null) ? 0 : itemAttachments.Length;
			if (itemAttachments != null && itemAttachments.Length > 0)
			{
				foreach (AttachmentType attachmentType in itemAttachments)
				{
					Attachment attachment = this.AttachmentTypeToEntity(attachmentType, queryAdapter);
					attachment.ParentItemId = rootItemId;
					attachment.ParentItemNavigationName = parentItemNavigationName;
					list.Add(attachment);
				}
			}
			return new FindEntitiesResult<Attachment>(list, totalCount);
		}

		public Attachment Read(string rootItemId, string attachmentId, AttachmentQueryAdapter queryAdapter)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("rootItemId", rootItemId);
			ArgumentValidator.ThrowIfNullOrEmpty("attachmentId", rootItemId);
			ArgumentValidator.ThrowIfNull("queryAdapter", queryAdapter);
			string ewsRootItemId = EwsIdConverter.ODataIdToEwsId(rootItemId);
			string ewsAttachmentId = EwsIdConverter.ODataIdToEwsId(attachmentId);
			string parentItemNavigationName = null;
			AttachmentType[] itemAttachments = this.GetItemAttachments(ewsRootItemId, out parentItemNavigationName);
			if (itemAttachments == null || itemAttachments.Length == 0)
			{
				throw new InvalidStoreIdException((CoreResources.IDs)4005418156U);
			}
			AttachmentType attachmentType = itemAttachments.FirstOrDefault((AttachmentType a) => a.AttachmentId.Id.Equals(ewsAttachmentId, StringComparison.OrdinalIgnoreCase));
			if (attachmentType == null)
			{
				throw new InvalidStoreIdException(CoreResources.IDs.ErrorInvalidAttachmentId);
			}
			Attachment attachment = this.AttachmentTypeToEntity(attachmentType, queryAdapter);
			attachment.ParentItemId = rootItemId;
			attachment.ParentItemNavigationName = parentItemNavigationName;
			return attachment;
		}

		public Attachment Create(string rootItemId, Attachment template)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("rootItemId", rootItemId);
			ArgumentValidator.ThrowIfNull("template", template);
			string id = EwsIdConverter.ODataIdToEwsId(rootItemId);
			AttachmentType attachmentType = EwsServiceObjectFactory.CreateServiceObject<AttachmentType>(template);
			foreach (PropertyDefinition propertyDefinition in template.PropertyBag.GetProperties())
			{
				if (!propertyDefinition.IsNavigation && propertyDefinition.Flags.HasFlag(PropertyDefinitionFlags.CanCreate))
				{
					propertyDefinition.EwsPropertyProvider.SetPropertyToDataSource(template, propertyDefinition, attachmentType);
				}
			}
			if (template is ItemAttachment)
			{
				ItemAttachment itemAttachment = (ItemAttachment)template;
				ItemType itemType = EwsServiceObjectFactory.CreateServiceObject<ItemType>(itemAttachment.Item);
				foreach (PropertyDefinition propertyDefinition2 in itemAttachment.Item.PropertyBag.GetProperties())
				{
					if (!propertyDefinition2.IsNavigation && propertyDefinition2.Flags.HasFlag(PropertyDefinitionFlags.CanCreate))
					{
						propertyDefinition2.EwsPropertyProvider.SetPropertyToDataSource(itemAttachment.Item, propertyDefinition2, itemType);
					}
				}
				ItemAttachmentType itemAttachmentType = (ItemAttachmentType)attachmentType;
				itemAttachmentType.Item = itemType;
			}
			CreateAttachmentRequest request = new CreateAttachmentRequest
			{
				ParentItemId = new ItemId(id, null),
				Attachments = new AttachmentType[]
				{
					attachmentType
				}
			};
			Attachment result;
			using (IDisposableResponse<CreateAttachmentResponse> disposableResponse = base.ExchangeService.CreateAttachment(request, null))
			{
				CreateAttachmentResponse response = disposableResponse.Response;
				AttachmentInfoResponseMessage attachmentInfoResponseMessage = response.ResponseMessages.Items[0] as AttachmentInfoResponseMessage;
				AttachmentType attachmentType2 = attachmentInfoResponseMessage.Attachments[0];
				string attachmentId = EwsIdConverter.EwsIdToODataId(attachmentType2.AttachmentId.Id);
				Attachment attachment = this.Read(rootItemId, attachmentId, new AttachmentQueryAdapter(template.Schema as AttachmentSchema, ODataQueryOptions.Empty));
				attachment.ParentItemId = rootItemId;
				attachment.ParentItemNavigationName = template.ParentItemNavigationName;
				result = attachment;
			}
			return result;
		}

		public void Delete(string rootItemId, string attachmentId)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("rootItemId", rootItemId);
			ArgumentValidator.ThrowIfNullOrEmpty("attachmentId", rootItemId);
			string rootItemId2 = EwsIdConverter.ODataIdToEwsId(rootItemId);
			string id = EwsIdConverter.ODataIdToEwsId(attachmentId);
			DeleteAttachmentRequest request = new DeleteAttachmentRequest
			{
				AttachmentIds = new AttachmentIdType[]
				{
					new AttachmentIdType
					{
						RootItemId = rootItemId2,
						Id = id
					}
				}
			};
			base.ExchangeService.DeleteAttachment(request, null);
		}

		private Attachment AttachmentTypeToEntity(AttachmentType attachmentType, AttachmentQueryAdapter queryAdapter)
		{
			Attachment attachment = EwsServiceObjectFactory.CreateEntity<Attachment>(attachmentType);
			AttachmentQueryAdapter attachmentQueryAdapter = new AttachmentQueryAdapter(attachment.Schema as AttachmentSchema, queryAdapter.ODataQueryOptions);
			foreach (PropertyDefinition propertyDefinition in attachmentQueryAdapter.RequestedProperties)
			{
				if (!propertyDefinition.IsNavigation)
				{
					propertyDefinition.EwsPropertyProvider.GetPropertyFromDataSource(attachment, propertyDefinition, attachmentType);
				}
			}
			this.LoadContent(attachment, attachmentType);
			return attachment;
		}

		private AttachmentType[] GetItemAttachments(string ewsRootItemId, out string parentNavigationName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("ewsRootItemId", ewsRootItemId);
			GetItemRequest request = new GetItemRequest
			{
				Ids = new BaseItemId[]
				{
					new ItemId(ewsRootItemId, null)
				},
				ItemShape = new ItemResponseShape(ShapeEnum.IdOnly, BodyResponseType.Best, false, new PropertyPath[]
				{
					ItemSchema.Attachments.PropertyPath
				})
			};
			GetItemResponse item = base.ExchangeService.GetItem(request, null);
			ItemInfoResponseMessage itemInfoResponseMessage = item.ResponseMessages.Items[0] as ItemInfoResponseMessage;
			ItemType itemType = itemInfoResponseMessage.Items.Items[0];
			parentNavigationName = this.ResolveParentNavigationName(itemType);
			return itemType.Attachments;
		}

		private string ResolveParentNavigationName(ItemType itemType)
		{
			if (itemType is EwsCalendarItemType)
			{
				return UserSchema.Events.Name;
			}
			if (itemType is ContactItemType)
			{
				return UserSchema.Contacts.Name;
			}
			return UserSchema.Messages.Name;
		}

		private void LoadContent(Attachment attachmentEntity, AttachmentType metaAttachmentType)
		{
			GetAttachmentRequest request = new GetAttachmentRequest
			{
				AttachmentShape = AttachmentQueryAdapter.AttachmentResponseShape,
				AttachmentIds = new AttachmentIdType[]
				{
					metaAttachmentType.AttachmentId
				}
			};
			using (IDisposableResponse<GetAttachmentResponse> attachment = base.ExchangeService.GetAttachment(request, null))
			{
				GetAttachmentResponse response = attachment.Response;
				AttachmentInfoResponseMessage attachmentInfoResponseMessage = response.ResponseMessages.Items[0] as AttachmentInfoResponseMessage;
				AttachmentType attachmentContentType = attachmentInfoResponseMessage.Attachments[0];
				if (attachmentEntity is FileAttachment)
				{
					this.ExpandFileAttachment(attachmentEntity, attachmentContentType);
				}
				if (attachmentEntity is ItemAttachment)
				{
					this.ExpandItemAttachment(attachmentEntity, attachmentContentType);
				}
			}
		}

		private void ExpandItemAttachment(Attachment attachmentEntity, AttachmentType attachmentContentType)
		{
			ItemAttachment itemAttachment = (ItemAttachment)attachmentEntity;
			ItemAttachmentType itemAttachmentType = (ItemAttachmentType)attachmentContentType;
			Item item = EwsServiceObjectFactory.CreateEntity<Item>(itemAttachmentType.Item);
			foreach (PropertyDefinition propertyDefinition in item.Schema.AllProperties)
			{
				if (!propertyDefinition.IsNavigation)
				{
					if (propertyDefinition.Equals(EntitySchema.Id))
					{
						item.Id = null;
					}
					else if (propertyDefinition.EwsPropertyProvider != null)
					{
						PropertyInformation propertyInformation = propertyDefinition.EwsPropertyProvider.GetEwsPropertyProvider(item.Schema).PropertyInformation;
						if (itemAttachmentType.Item.PropertyBag.Contains(propertyInformation))
						{
							propertyDefinition.EwsPropertyProvider.GetPropertyFromDataSource(item, propertyDefinition, itemAttachmentType.Item);
						}
					}
				}
			}
			itemAttachment.Item = item;
		}

		private void ExpandFileAttachment(Attachment attachmentEntity, AttachmentType attachmentContentType)
		{
			FileAttachment fileAttachment = (FileAttachment)attachmentEntity;
			FileAttachmentType fileAttachmentType = (FileAttachmentType)attachmentContentType;
			fileAttachment.ContentBytes = fileAttachmentType.Content;
		}
	}
}

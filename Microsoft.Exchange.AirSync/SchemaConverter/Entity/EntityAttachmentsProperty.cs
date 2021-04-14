using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Entity
{
	[Serializable]
	internal class EntityAttachmentsProperty : EntityProperty, IAttachments12Property, IMultivaluedProperty<Attachment12Data>, IProperty, IEnumerable<Attachment12Data>, IEnumerable
	{
		public EntityAttachmentsProperty() : base(SchematizedObject<EventSchema>.SchemaInstance.AttachmentsProperty, PropertyType.ReadWrite, false)
		{
		}

		public int Count
		{
			get
			{
				if (this.values == null)
				{
					return 0;
				}
				return this.values.Count;
			}
		}

		public override void Bind(IItem item)
		{
			base.Bind(item);
			this.values = base.Item.Attachments;
		}

		public override void Unbind()
		{
			try
			{
				this.values = null;
			}
			finally
			{
				base.Unbind();
			}
		}

		public IEnumerator<Attachment12Data> GetEnumerator()
		{
			if (this.values != null)
			{
				foreach (IAttachment attachment in this.values)
				{
					Attachment12Data attachment12Data = this.CreateAttachment14Data(attachment);
					if (attachment12Data != null)
					{
						yield return attachment12Data;
					}
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public override void CopyFrom(IProperty srcProperty)
		{
			IAttachments12Property attachments12Property = srcProperty as IAttachments12Property;
			if (attachments12Property == null)
			{
				throw new UnexpectedTypeException("IAttachments12Property", srcProperty);
			}
			List<IAttachment> list = new List<IAttachment>(attachments12Property.Count);
			foreach (Attachment12Data attachment12Data in attachments12Property)
			{
				Attachment16Data attachment16Data = attachment12Data as Attachment16Data;
				if (attachment16Data == null)
				{
					throw new UnexpectedTypeException("Attachment16Data", attachment12Data);
				}
				IAttachment attachment = null;
				if (attachment16Data.ChangeType == AttachmentAction.Add)
				{
					byte method = attachment16Data.Method;
					if (method != 1)
					{
						if (method != 5)
						{
						}
						throw new ConversionException(string.Format("The attachment method \"{0}\" is not suported.", attachment16Data.Method));
					}
					attachment = new FileAttachment
					{
						ContentId = attachment16Data.ContentId,
						ContentLocation = attachment16Data.ContentLocation,
						Content = attachment16Data.Content
					};
					attachment.Name = attachment16Data.DisplayName;
					attachment.IsInline = attachment16Data.IsInline;
					attachment.ContentType = attachment16Data.ContentType;
					attachment.Id = attachment16Data.ClientId;
				}
				else if (attachment16Data.ChangeType == AttachmentAction.Delete)
				{
					if (!attachment16Data.FileReference.StartsWith(base.Item.Id + ":"))
					{
						throw new ConversionException(string.Format("The attachment \"{0}\" does not belog to the item \"{1}\".", attachment16Data.FileReference, base.Item.Id));
					}
					attachment = new EntityDeleteAttachment();
					attachment.Id = attachment16Data.FileReference.Substring(base.Item.Id.Length + 1);
				}
				list.Add(attachment);
			}
			base.Item.Attachments = list;
		}

		private Attachment14Data CreateAttachment14Data(IAttachment attachment)
		{
			Attachment14Data attachment14Data = new Attachment14Data();
			attachment14Data.DisplayName = attachment.Name;
			attachment14Data.EstimatedDataSize = attachment.Size;
			attachment14Data.IsInline = attachment.IsInline;
			attachment14Data.FileReference = HttpUtility.UrlEncode(base.Item.Id + ":" + attachment.Id);
			attachment14Data.Id = EntitySyncItemId.GetAttachmentId(attachment.Id);
			FileAttachment fileAttachment = attachment as FileAttachment;
			ItemAttachment itemAttachment = attachment as ItemAttachment;
			if (fileAttachment != null)
			{
				attachment14Data.Method = 1;
				attachment14Data.ContentId = fileAttachment.ContentId;
				attachment14Data.ContentLocation = fileAttachment.ContentLocation;
				if (fileAttachment.Content != null)
				{
					attachment14Data.EstimatedDataSize = (long)fileAttachment.Content.Length;
				}
			}
			else
			{
				if (itemAttachment == null)
				{
					throw new ConversionException(string.Format("Attachment type \"{0}\" is not supported.", attachment.GetType().FullName));
				}
				attachment14Data.Method = 5;
			}
			return attachment14Data;
		}

		private List<IAttachment> values;
	}
}

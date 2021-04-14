using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Entity
{
	[Serializable]
	internal class EntityContentProperty : EntityProperty, IContent16Property, IContent14Property, IContentProperty, IMIMEDataProperty, IMIMERelatedProperty, IProperty
	{
		public EntityContentProperty() : base(SchematizedObject<EventSchema>.SchemaInstance.BodyProperty, PropertyType.ReadWrite, false)
		{
		}

		public EntityContentProperty(PropertyType type) : base(SchematizedObject<EventSchema>.SchemaInstance.BodyProperty, type, false)
		{
		}

		public bool IsOnSMIMEMessage
		{
			get
			{
				throw new NotImplementedException("IsOnSMIMEMessage");
			}
		}

		public Stream MIMEData
		{
			get
			{
				throw new NotImplementedException("get_MIMEData");
			}
			set
			{
				throw new NotImplementedException("set_MIMEData");
			}
		}

		public long MIMESize
		{
			get
			{
				throw new NotImplementedException("get_MIMESize");
			}
			set
			{
				throw new NotImplementedException("set_MIMESize");
			}
		}

		public string Preview
		{
			get
			{
				if (base.Item == null)
				{
					return null;
				}
				return base.Item.Preview;
			}
		}

		public Stream Body
		{
			get
			{
				if (this.itemBody == null || this.itemBody.Content == null)
				{
					return null;
				}
				return new MemoryStream(Encoding.UTF8.GetBytes(this.itemBody.Content));
			}
		}

		public string BodyString
		{
			get
			{
				if (this.itemBody == null || this.itemBody.Content == null)
				{
					return null;
				}
				return this.itemBody.Content;
			}
		}

		public long Size
		{
			get
			{
				if (this.itemBody == null || this.itemBody.Content == null)
				{
					return 0L;
				}
				return (long)this.itemBody.Content.Length;
			}
		}

		public bool IsIrmErrorMessage
		{
			get
			{
				return false;
			}
		}

		public override void Bind(IItem item)
		{
			base.Bind(item);
			this.itemBody = base.Item.Body;
		}

		public override void Unbind()
		{
			try
			{
				this.itemBody = null;
			}
			finally
			{
				base.Unbind();
			}
		}

		public Stream GetData(Microsoft.Exchange.AirSync.SchemaConverter.Common.BodyType type, long truncationSize, out long estimatedDataSize, out IEnumerable<AirSyncAttachmentInfo> attachments)
		{
			estimatedDataSize = this.Size;
			attachments = null;
			if (this.GetNativeType() == type)
			{
				return new MemoryStream(Encoding.UTF8.GetBytes(this.itemBody.Content.Substring(0, (int)((truncationSize > -1L) ? Math.Min(truncationSize, estimatedDataSize) : estimatedDataSize))));
			}
			throw new NotImplementedException(string.Format("EntityContentProperty.GetData of type {0} when the item has body type of {1}", type, this.GetNativeType()));
		}

		public Microsoft.Exchange.AirSync.SchemaConverter.Common.BodyType GetNativeType()
		{
			if (this.itemBody == null)
			{
				return Microsoft.Exchange.AirSync.SchemaConverter.Common.BodyType.None;
			}
			switch (this.itemBody.ContentType)
			{
			case Microsoft.Exchange.Entities.DataModel.Items.BodyType.Text:
				return Microsoft.Exchange.AirSync.SchemaConverter.Common.BodyType.PlainText;
			case Microsoft.Exchange.Entities.DataModel.Items.BodyType.Html:
				return Microsoft.Exchange.AirSync.SchemaConverter.Common.BodyType.Html;
			default:
				throw new NotImplementedException(string.Format("Unable to convert body content type {0}", this.itemBody.ContentType));
			}
		}

		public void PreProcessProperty()
		{
		}

		public void PostProcessProperty()
		{
		}

		public override void CopyFrom(IProperty srcProperty)
		{
			IContent16Property content16Property = srcProperty as IContent16Property;
			if (content16Property != null)
			{
				ItemBody itemBody = new ItemBody();
				switch (content16Property.GetNativeType())
				{
				case Microsoft.Exchange.AirSync.SchemaConverter.Common.BodyType.None:
					base.Item.Body = null;
					return;
				case Microsoft.Exchange.AirSync.SchemaConverter.Common.BodyType.PlainText:
					itemBody.ContentType = Microsoft.Exchange.Entities.DataModel.Items.BodyType.Text;
					break;
				case Microsoft.Exchange.AirSync.SchemaConverter.Common.BodyType.Html:
					itemBody.ContentType = Microsoft.Exchange.Entities.DataModel.Items.BodyType.Html;
					break;
				default:
					throw new NotImplementedException(string.Format("Unable to convert content type {0}", content16Property.GetNativeType()));
				}
				itemBody.Content = content16Property.BodyString;
				base.Item.Body = itemBody;
			}
		}

		private ItemBody itemBody;
	}
}

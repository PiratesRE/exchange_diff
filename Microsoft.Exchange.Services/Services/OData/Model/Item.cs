using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class Item : Entity
	{
		public string ChangeKey
		{
			get
			{
				return (string)base[ItemSchema.ChangeKey];
			}
			set
			{
				base[ItemSchema.ChangeKey] = value;
			}
		}

		public string ClassName
		{
			get
			{
				return (string)base[ItemSchema.ClassName];
			}
			set
			{
				base[ItemSchema.ClassName] = value;
			}
		}

		public string Subject
		{
			get
			{
				return (string)base[ItemSchema.Subject];
			}
			set
			{
				base[ItemSchema.Subject] = value;
			}
		}

		public ItemBody Body
		{
			get
			{
				return (ItemBody)base[ItemSchema.Body];
			}
			set
			{
				base[ItemSchema.Body] = value;
			}
		}

		public string BodyPreview
		{
			get
			{
				return (string)base[ItemSchema.BodyPreview];
			}
			set
			{
				base[ItemSchema.BodyPreview] = value;
			}
		}

		public bool HasAttachments
		{
			get
			{
				return (bool)base[ItemSchema.HasAttachments];
			}
			set
			{
				base[ItemSchema.HasAttachments] = value;
			}
		}

		public string[] Categories
		{
			get
			{
				return (string[])base[ItemSchema.Categories];
			}
			set
			{
				base[ItemSchema.Categories] = value;
			}
		}

		public DateTimeOffset DateTimeCreated
		{
			get
			{
				return (DateTimeOffset)base[ItemSchema.DateTimeCreated];
			}
			set
			{
				base[ItemSchema.DateTimeCreated] = value;
			}
		}

		public DateTimeOffset LastModifiedTime
		{
			get
			{
				return (DateTimeOffset)base[ItemSchema.LastModifiedTime];
			}
			set
			{
				base[ItemSchema.LastModifiedTime] = value;
			}
		}

		public Importance Importance
		{
			get
			{
				return (Importance)base[ItemSchema.Importance];
			}
			set
			{
				base[ItemSchema.Importance] = value;
			}
		}

		public IEnumerable<Attachment> Attachments
		{
			get
			{
				return (IEnumerable<Attachment>)base[ItemSchema.Attachments];
			}
			set
			{
				base[ItemSchema.Attachments] = value;
			}
		}

		internal override EntitySchema Schema
		{
			get
			{
				return ItemSchema.SchemaInstance;
			}
		}

		internal new static readonly EdmEntityType EdmEntityType = new EdmEntityType(typeof(Item).Namespace, typeof(Item).Name, Entity.EdmEntityType, true, false);
	}
}

using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class Attachment : Entity
	{
		public string Name
		{
			get
			{
				return (string)base[AttachmentSchema.Name];
			}
			set
			{
				base[AttachmentSchema.Name] = value;
			}
		}

		public string ContentType
		{
			get
			{
				return (string)base[AttachmentSchema.ContentType];
			}
			set
			{
				base[AttachmentSchema.ContentType] = value;
			}
		}

		public int Size
		{
			get
			{
				return (int)base[AttachmentSchema.Size];
			}
			set
			{
				base[AttachmentSchema.Size] = value;
			}
		}

		public DateTimeOffset LastModifiedTime
		{
			get
			{
				return (DateTimeOffset)base[AttachmentSchema.LastModifiedTime];
			}
			set
			{
				base[AttachmentSchema.LastModifiedTime] = value;
			}
		}

		public bool IsInline
		{
			get
			{
				return (bool)base[AttachmentSchema.IsInline];
			}
			set
			{
				base[AttachmentSchema.IsInline] = value;
			}
		}

		internal override EntitySchema Schema
		{
			get
			{
				return AttachmentSchema.SchemaInstance;
			}
		}

		internal string ParentItemNavigationName { get; set; }

		internal string ParentItemId { get; set; }

		internal override Uri GetWebUri(ODataContext odataContext)
		{
			ArgumentValidator.ThrowIfNull("odataContext", odataContext);
			string uriString = string.Format("{0}Users('{1}')/{2}('{3}')/Attachments('{4}')", new object[]
			{
				odataContext.HttpContext.GetServiceRootUri(),
				odataContext.TargetMailbox.PrimarySmtpAddress,
				this.ParentItemNavigationName,
				this.ParentItemId,
				base.Id
			});
			return new Uri(uriString);
		}

		internal new static readonly EdmEntityType EdmEntityType = new EdmEntityType(typeof(Attachment).Namespace, typeof(Attachment).Name, Entity.EdmEntityType, true, false);
	}
}

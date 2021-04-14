using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public sealed class MessageCategory : XsoMailboxConfigurationObject
	{
		public override ObjectId Identity
		{
			get
			{
				return (ObjectId)this[MessageCategorySchema.Identity];
			}
		}

		public string Name
		{
			get
			{
				return (string)this[MessageCategorySchema.Name];
			}
			set
			{
				this[MessageCategorySchema.Name] = value;
			}
		}

		public int Color
		{
			get
			{
				return (int)this[MessageCategorySchema.Color];
			}
			set
			{
				this[MessageCategorySchema.Color] = value;
			}
		}

		public Guid Guid
		{
			get
			{
				return (Guid)this[MessageCategorySchema.Guid];
			}
			set
			{
				this[MessageCategorySchema.Guid] = value;
			}
		}

		internal override XsoMailboxConfigurationObjectSchema Schema
		{
			get
			{
				return MessageCategory.schema;
			}
		}

		public MessageCategory()
		{
			((SimplePropertyBag)this.propertyBag).SetObjectIdentityPropertyDefinition(MessageCategorySchema.Identity);
		}

		public override string ToString()
		{
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			if (!string.IsNullOrEmpty(this.Name))
			{
				return this.Name;
			}
			return base.ToString();
		}

		internal static object IdentityGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[XsoMailboxConfigurationObjectSchema.MailboxOwnerId];
			Guid value = (Guid)propertyBag[MessageCategorySchema.Guid];
			string name = (string)propertyBag[MessageCategorySchema.Name];
			if (adobjectId != null)
			{
				return new MessageCategoryId(adobjectId, name, new Guid?(value));
			}
			return null;
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
		}

		private static MessageCategorySchema schema = ObjectSchema.GetInstance<MessageCategorySchema>();
	}
}

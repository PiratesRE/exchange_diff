using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal sealed class MessageRecipientStatus : MessageTraceEntityBase, IExtendedPropertyStore<MessageRecipientStatusProperty>
	{
		public MessageRecipientStatus()
		{
			this.extendedProperties = new ExtendedPropertyStore<MessageRecipientStatusProperty>();
			this.RecipientStatusId = IdGenerator.GenerateIdentifier(IdScope.MessageRecipientStatus);
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this[MessageRecipientStatusSchema.RecipientStatusIdProperty].ToString());
			}
		}

		public Guid RecipientStatusId
		{
			get
			{
				return (Guid)this[MessageRecipientStatusSchema.RecipientStatusIdProperty];
			}
			set
			{
				this[MessageRecipientStatusSchema.RecipientStatusIdProperty] = value;
			}
		}

		public Guid RecipientId
		{
			get
			{
				return (Guid)this[MessageRecipientStatusSchema.RecipientIdProperty];
			}
			set
			{
				this[MessageRecipientStatusSchema.RecipientIdProperty] = value;
			}
		}

		public Guid EventId
		{
			get
			{
				return (Guid)this[MessageRecipientStatusSchema.EventIdProperty];
			}
			set
			{
				this[MessageRecipientStatusSchema.EventIdProperty] = value;
			}
		}

		public string Status
		{
			get
			{
				return this[MessageRecipientStatusSchema.StatusProperty] as string;
			}
			set
			{
				this[MessageRecipientStatusSchema.StatusProperty] = value;
			}
		}

		public string Reference
		{
			get
			{
				return this[MessageRecipientStatusSchema.ReferenceProperty] as string;
			}
			set
			{
				this[MessageRecipientStatusSchema.ReferenceProperty] = value;
			}
		}

		public int ExtendedPropertiesCount
		{
			get
			{
				return this.extendedProperties.ExtendedPropertiesCount;
			}
		}

		public bool TryGetExtendedProperty(string nameSpace, string name, out MessageRecipientStatusProperty extendedProperty)
		{
			return this.extendedProperties.TryGetExtendedProperty(nameSpace, name, out extendedProperty);
		}

		public MessageRecipientStatusProperty GetExtendedProperty(string nameSpace, string name)
		{
			return this.extendedProperties.GetExtendedProperty(nameSpace, name);
		}

		public IEnumerable<MessageRecipientStatusProperty> GetExtendedPropertiesEnumerable()
		{
			return this.extendedProperties.GetExtendedPropertiesEnumerable();
		}

		public void AddExtendedProperty(MessageRecipientStatusProperty extendedProperty)
		{
			this.extendedProperties.AddExtendedProperty(extendedProperty);
			extendedProperty.RecipientStatusId = this.RecipientStatusId;
		}

		public override void Accept(IMessageTraceVisitor visitor)
		{
			visitor.Visit(this);
			foreach (MessageRecipientStatusProperty messageRecipientStatusProperty in this.GetExtendedPropertiesEnumerable())
			{
				messageRecipientStatusProperty.Accept(visitor);
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(MessageRecipientStatusSchema);
		}

		public override HygienePropertyDefinition[] GetAllProperties()
		{
			return MessageRecipientStatus.Properties;
		}

		internal static readonly HygienePropertyDefinition[] Properties = new HygienePropertyDefinition[]
		{
			MessageRecipientStatusSchema.RecipientStatusIdProperty,
			MessageRecipientStatusSchema.RecipientIdProperty,
			CommonMessageTraceSchema.ExMessageIdProperty,
			MessageRecipientStatusSchema.EventIdProperty,
			MessageRecipientStatusSchema.StatusProperty,
			MessageRecipientStatusSchema.ReferenceProperty,
			CommonMessageTraceSchema.EmailHashKeyProperty,
			CommonMessageTraceSchema.EventHashKeyProperty
		};

		private ExtendedPropertyStore<MessageRecipientStatusProperty> extendedProperties;
	}
}

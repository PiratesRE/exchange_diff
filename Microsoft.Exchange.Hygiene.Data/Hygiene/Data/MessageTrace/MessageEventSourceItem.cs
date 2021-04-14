using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal sealed class MessageEventSourceItem : MessageTraceEntityBase, IExtendedPropertyStore<MessageEventSourceItemProperty>
	{
		public MessageEventSourceItem()
		{
			this.extendedProperties = new ExtendedPropertyStore<MessageEventSourceItemProperty>();
			this.SourceItemId = IdGenerator.GenerateIdentifier(IdScope.MessageEventSourceItem);
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this[MessageEventSourceItemSchema.SourceItemIdProperty].ToString());
			}
		}

		public Guid SourceItemId
		{
			get
			{
				return (Guid)this[MessageEventSourceItemSchema.SourceItemIdProperty];
			}
			set
			{
				this[MessageEventSourceItemSchema.SourceItemIdProperty] = value;
			}
		}

		public Guid EventId
		{
			get
			{
				return (Guid)this[MessageEventSourceItemSchema.EventIdProperty];
			}
			set
			{
				this[MessageEventSourceItemSchema.EventIdProperty] = value;
			}
		}

		public string Name
		{
			get
			{
				return (string)this[MessageEventSourceItemSchema.NameProperty];
			}
			set
			{
				this[MessageEventSourceItemSchema.NameProperty] = value;
			}
		}

		public int ExtendedPropertiesCount
		{
			get
			{
				return this.extendedProperties.ExtendedPropertiesCount;
			}
		}

		public bool TryGetExtendedProperty(string nameSpace, string name, out MessageEventSourceItemProperty extendedProperty)
		{
			return this.extendedProperties.TryGetExtendedProperty(nameSpace, name, out extendedProperty);
		}

		public MessageEventSourceItemProperty GetExtendedProperty(string nameSpace, string name)
		{
			return this.extendedProperties.GetExtendedProperty(nameSpace, name);
		}

		public IEnumerable<MessageEventSourceItemProperty> GetExtendedPropertiesEnumerable()
		{
			return this.extendedProperties.GetExtendedPropertiesEnumerable();
		}

		public void AddExtendedProperty(MessageEventSourceItemProperty extendedProperty)
		{
			this.extendedProperties.AddExtendedProperty(extendedProperty);
			extendedProperty.SourceItemId = this.SourceItemId;
		}

		public override void Accept(IMessageTraceVisitor visitor)
		{
			visitor.Visit(this);
			foreach (MessageEventSourceItemProperty messageEventSourceItemProperty in this.GetExtendedPropertiesEnumerable())
			{
				messageEventSourceItemProperty.Accept(visitor);
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(MessageEventSourceItemSchema);
		}

		public override HygienePropertyDefinition[] GetAllProperties()
		{
			return MessageEventSourceItem.Properties;
		}

		internal static readonly HygienePropertyDefinition[] Properties = new HygienePropertyDefinition[]
		{
			MessageEventSourceItemSchema.SourceItemIdProperty,
			MessageEventSourceItemSchema.EventIdProperty,
			MessageEventSourceItemSchema.NameProperty,
			CommonMessageTraceSchema.ExMessageIdProperty,
			CommonMessageTraceSchema.EventHashKeyProperty
		};

		private ExtendedPropertyStore<MessageEventSourceItemProperty> extendedProperties;
	}
}

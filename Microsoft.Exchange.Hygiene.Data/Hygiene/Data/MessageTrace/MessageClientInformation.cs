using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageClientInformation : MessageTraceEntityBase, IExtendedPropertyStore<MessageClientInformationProperty>
	{
		public MessageClientInformation()
		{
			this.extendedProperties = new ExtendedPropertyStore<MessageClientInformationProperty>();
			this.ClientInformationId = IdGenerator.GenerateIdentifier(IdScope.MessageClientInformation);
		}

		public Guid ClientInformationId
		{
			get
			{
				return (Guid)this[MessageClientInformationSchema.ClientInformationIdProperty];
			}
			set
			{
				this[MessageClientInformationSchema.ClientInformationIdProperty] = value;
			}
		}

		public Guid DataClassificationId
		{
			get
			{
				return (Guid)this[MessageClientInformationSchema.DataClassificationIdProperty];
			}
			set
			{
				this[MessageClientInformationSchema.DataClassificationIdProperty] = value;
			}
		}

		public Guid ExMessageId
		{
			get
			{
				return (Guid)this[CommonMessageTraceSchema.ExMessageIdProperty];
			}
			set
			{
				this[CommonMessageTraceSchema.ExMessageIdProperty] = value;
			}
		}

		public int ExtendedPropertiesCount
		{
			get
			{
				return this.extendedProperties.ExtendedPropertiesCount;
			}
		}

		public bool TryGetExtendedProperty(string nameSpace, string name, out MessageClientInformationProperty extendedProperty)
		{
			return this.extendedProperties.TryGetExtendedProperty(nameSpace, name, out extendedProperty);
		}

		public MessageClientInformationProperty GetExtendedProperty(string nameSpace, string name)
		{
			return this.extendedProperties.GetExtendedProperty(nameSpace, name);
		}

		public IEnumerable<MessageClientInformationProperty> GetExtendedPropertiesEnumerable()
		{
			return this.extendedProperties.GetExtendedPropertiesEnumerable();
		}

		public void AddExtendedProperty(MessageClientInformationProperty extendedProperty)
		{
			this.extendedProperties.AddExtendedProperty(extendedProperty);
			extendedProperty.ClientInformationId = this.ClientInformationId;
		}

		public override void Accept(IMessageTraceVisitor visitor)
		{
			visitor.Visit(this);
			foreach (MessageClientInformationProperty messageClientInformationProperty in this.GetExtendedPropertiesEnumerable())
			{
				messageClientInformationProperty.Accept(visitor);
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(MessageClientInformation);
		}

		public override HygienePropertyDefinition[] GetAllProperties()
		{
			return MessageClientInformation.Properties;
		}

		internal static readonly HygienePropertyDefinition[] Properties = new HygienePropertyDefinition[]
		{
			MessageClientInformationSchema.ClientInformationIdProperty,
			MessageClientInformationSchema.DataClassificationIdProperty,
			CommonMessageTraceSchema.ExMessageIdProperty
		};

		private ExtendedPropertyStore<MessageClientInformationProperty> extendedProperties;
	}
}

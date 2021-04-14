using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageClassification : MessageTraceEntityBase, IExtendedPropertyStore<MessageClassificationProperty>
	{
		public MessageClassification()
		{
			this.extendedProperties = new ExtendedPropertyStore<MessageClassificationProperty>();
			this.ClassificationId = IdGenerator.GenerateIdentifier(IdScope.MessageClassification);
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this[MessageClassificationSchema.ClassificationIdProperty].ToString());
			}
		}

		public Guid ClassificationId
		{
			get
			{
				return (Guid)this[MessageClassificationSchema.ClassificationIdProperty];
			}
			set
			{
				this[MessageClassificationSchema.ClassificationIdProperty] = value;
			}
		}

		public Guid ExMessageId
		{
			get
			{
				return (Guid)this[MessageClassificationSchema.ExMessageIdProperty];
			}
			set
			{
				this[MessageClassificationSchema.ExMessageIdProperty] = value;
			}
		}

		public Guid DataClassificationId
		{
			get
			{
				return (Guid)this[MessageClassificationSchema.DataClassificationIdProperty];
			}
			set
			{
				this[MessageClassificationSchema.DataClassificationIdProperty] = value;
			}
		}

		public int ExtendedPropertiesCount
		{
			get
			{
				return this.extendedProperties.ExtendedPropertiesCount;
			}
		}

		public bool TryGetExtendedProperty(string nameSpace, string name, out MessageClassificationProperty extendedProperty)
		{
			return this.extendedProperties.TryGetExtendedProperty(nameSpace, name, out extendedProperty);
		}

		public MessageClassificationProperty GetExtendedProperty(string nameSpace, string name)
		{
			return this.extendedProperties.GetExtendedProperty(nameSpace, name);
		}

		public IEnumerable<MessageClassificationProperty> GetExtendedPropertiesEnumerable()
		{
			return this.extendedProperties.GetExtendedPropertiesEnumerable();
		}

		public void AddExtendedProperty(MessageClassificationProperty extendedProperty)
		{
			this.extendedProperties.AddExtendedProperty(extendedProperty);
			extendedProperty.ClassificationId = this.ClassificationId;
		}

		public override void Accept(IMessageTraceVisitor visitor)
		{
			visitor.Visit(this);
			foreach (MessageClassificationProperty messageClassificationProperty in this.GetExtendedPropertiesEnumerable())
			{
				messageClassificationProperty.Accept(visitor);
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(MessageClassificationSchema);
		}

		public override HygienePropertyDefinition[] GetAllProperties()
		{
			return MessageClassification.Properties;
		}

		internal static readonly HygienePropertyDefinition[] Properties = new HygienePropertyDefinition[]
		{
			MessageClassificationSchema.ClassificationIdProperty,
			CommonMessageTraceSchema.ExMessageIdProperty,
			MessageClassificationSchema.DataClassificationIdProperty
		};

		private ExtendedPropertyStore<MessageClassificationProperty> extendedProperties;
	}
}

using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageEventRuleClassification : MessageTraceEntityBase, IExtendedPropertyStore<MessageEventRuleClassificationProperty>
	{
		public MessageEventRuleClassification()
		{
			this.extendedProperties = new ExtendedPropertyStore<MessageEventRuleClassificationProperty>();
			this.EventRuleClassificationId = IdGenerator.GenerateIdentifier(IdScope.MessageEventRuleClassification);
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this[MessageEventRuleClassificationSchema.DataClassificationIdProperty].ToString());
			}
		}

		public Guid EventRuleClassificationId
		{
			get
			{
				return (Guid)this[CommonMessageTraceSchema.EventRuleClassificationIdProperty];
			}
			set
			{
				this[CommonMessageTraceSchema.EventRuleClassificationIdProperty] = value;
			}
		}

		public Guid EventRuleId
		{
			get
			{
				return (Guid)this[CommonMessageTraceSchema.EventRuleIdProperty];
			}
			set
			{
				this[CommonMessageTraceSchema.EventRuleIdProperty] = value;
			}
		}

		public Guid DataClassificationId
		{
			get
			{
				return (Guid)this[CommonMessageTraceSchema.DataClassificationIdProperty];
			}
			set
			{
				this[CommonMessageTraceSchema.DataClassificationIdProperty] = value;
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

		public bool TryGetExtendedProperty(string nameSpace, string name, out MessageEventRuleClassificationProperty extendedProperty)
		{
			return this.extendedProperties.TryGetExtendedProperty(nameSpace, name, out extendedProperty);
		}

		public MessageEventRuleClassificationProperty GetExtendedProperty(string nameSpace, string name)
		{
			return this.extendedProperties.GetExtendedProperty(nameSpace, name);
		}

		public IEnumerable<MessageEventRuleClassificationProperty> GetExtendedPropertiesEnumerable()
		{
			return this.extendedProperties.GetExtendedPropertiesEnumerable();
		}

		public void AddExtendedProperty(MessageEventRuleClassificationProperty extendedProperty)
		{
			this.extendedProperties.AddExtendedProperty(extendedProperty);
			extendedProperty.EventRuleClassificationId = this.EventRuleClassificationId;
		}

		public override void Accept(IMessageTraceVisitor visitor)
		{
			visitor.Visit(this);
			foreach (MessageEventRuleClassificationProperty messageEventRuleClassificationProperty in this.GetExtendedPropertiesEnumerable())
			{
				messageEventRuleClassificationProperty.Accept(visitor);
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(MessageEventRuleClassificationSchema);
		}

		public override HygienePropertyDefinition[] GetAllProperties()
		{
			return MessageEventRuleClassification.Properties;
		}

		internal static readonly HygienePropertyDefinition[] Properties = new HygienePropertyDefinition[]
		{
			CommonMessageTraceSchema.EventRuleClassificationIdProperty,
			CommonMessageTraceSchema.EventRuleIdProperty,
			CommonMessageTraceSchema.DataClassificationIdProperty,
			CommonMessageTraceSchema.ExMessageIdProperty,
			CommonMessageTraceSchema.RuleIdProperty,
			CommonMessageTraceSchema.EventHashKeyProperty
		};

		private ExtendedPropertyStore<MessageEventRuleClassificationProperty> extendedProperties;
	}
}

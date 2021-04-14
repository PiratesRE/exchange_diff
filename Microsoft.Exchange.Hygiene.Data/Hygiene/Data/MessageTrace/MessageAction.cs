using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageAction : MessageTraceEntityBase, IExtendedPropertyStore<MessageActionProperty>
	{
		public MessageAction()
		{
			this.extendedProperties = new ExtendedPropertyStore<MessageActionProperty>();
			this.RuleActionId = IdGenerator.GenerateIdentifier(IdScope.MessageAction);
		}

		public Guid RuleActionId
		{
			get
			{
				return (Guid)this[MessageActionSchema.RuleActionIdProperty];
			}
			set
			{
				this[MessageActionSchema.RuleActionIdProperty] = value;
			}
		}

		public Guid EventRuleId
		{
			get
			{
				return (Guid)this[MessageActionSchema.EventRuleIdProperty];
			}
			set
			{
				this[MessageActionSchema.EventRuleIdProperty] = value;
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

		public string Name
		{
			get
			{
				return (string)this[MessageActionSchema.NameProperty];
			}
			set
			{
				this[MessageActionSchema.NameProperty] = value;
			}
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[CommonMessageTraceSchema.OrganizationalUnitRootProperty];
			}
			set
			{
				this[CommonMessageTraceSchema.OrganizationalUnitRootProperty] = value;
			}
		}

		public int ExtendedPropertiesCount
		{
			get
			{
				return this.extendedProperties.ExtendedPropertiesCount;
			}
		}

		public bool TryGetExtendedProperty(string nameSpace, string name, out MessageActionProperty extendedProperty)
		{
			return this.extendedProperties.TryGetExtendedProperty(nameSpace, name, out extendedProperty);
		}

		public MessageActionProperty GetExtendedProperty(string nameSpace, string name)
		{
			return this.extendedProperties.GetExtendedProperty(nameSpace, name);
		}

		public IEnumerable<MessageActionProperty> GetExtendedPropertiesEnumerable()
		{
			return this.extendedProperties.GetExtendedPropertiesEnumerable();
		}

		public void AddExtendedProperty(MessageActionProperty extendedProperty)
		{
			this.extendedProperties.AddExtendedProperty(extendedProperty);
			extendedProperty.RuleActionId = this.RuleActionId;
		}

		public override void Accept(IMessageTraceVisitor visitor)
		{
			visitor.Visit(this);
			foreach (MessageActionProperty messageActionProperty in this.GetExtendedPropertiesEnumerable())
			{
				messageActionProperty.Accept(visitor);
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(MessageActionSchema);
		}

		public override HygienePropertyDefinition[] GetAllProperties()
		{
			return MessageAction.Properties;
		}

		internal static readonly HygienePropertyDefinition[] Properties = new HygienePropertyDefinition[]
		{
			MessageActionSchema.RuleActionIdProperty,
			CommonMessageTraceSchema.ExMessageIdProperty,
			MessageActionSchema.EventRuleIdProperty,
			MessageActionSchema.NameProperty,
			CommonMessageTraceSchema.RuleIdProperty,
			CommonMessageTraceSchema.EventHashKeyProperty
		};

		private ExtendedPropertyStore<MessageActionProperty> extendedProperties;
	}
}

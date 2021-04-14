using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageEventRule : MessageTraceEntityBase, IExtendedPropertyStore<MessageEventRuleProperty>
	{
		public MessageEventRule()
		{
			this.extendedProperties = new ExtendedPropertyStore<MessageEventRuleProperty>();
			this.EventRuleId = IdGenerator.GenerateIdentifier(IdScope.MessageEventRule);
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this[MessageEventRuleSchema.EventRuleIdProperty].ToString());
			}
		}

		public Guid EventRuleId
		{
			get
			{
				return (Guid)this[MessageEventRuleSchema.EventRuleIdProperty];
			}
			set
			{
				this[MessageEventRuleSchema.EventRuleIdProperty] = value;
			}
		}

		public Guid RuleId
		{
			get
			{
				return (Guid)this[MessageEventRuleSchema.RuleIdProperty];
			}
			set
			{
				this[MessageEventRuleSchema.RuleIdProperty] = value;
			}
		}

		public Guid EventId
		{
			get
			{
				return (Guid)this[MessageEventRuleSchema.EventIdProperty];
			}
			set
			{
				this[MessageEventRuleSchema.EventIdProperty] = value;
			}
		}

		public string RuleType
		{
			get
			{
				return (string)this[MessageEventRuleSchema.RuleTypeProperty];
			}
			set
			{
				this[MessageEventRuleSchema.RuleTypeProperty] = value;
			}
		}

		public List<MessageAction> Actions
		{
			get
			{
				List<MessageAction> result;
				if ((result = this.actions) == null)
				{
					result = (this.actions = new List<MessageAction>());
				}
				return result;
			}
		}

		public List<MessageEventRuleClassification> Classifications
		{
			get
			{
				List<MessageEventRuleClassification> result;
				if ((result = this.classifications) == null)
				{
					result = (this.classifications = new List<MessageEventRuleClassification>());
				}
				return result;
			}
		}

		public int ExtendedPropertiesCount
		{
			get
			{
				return this.extendedProperties.ExtendedPropertiesCount;
			}
		}

		public bool TryGetExtendedProperty(string nameSpace, string name, out MessageEventRuleProperty extendedProperty)
		{
			return this.extendedProperties.TryGetExtendedProperty(nameSpace, name, out extendedProperty);
		}

		public MessageEventRuleProperty GetExtendedProperty(string nameSpace, string name)
		{
			return this.extendedProperties.GetExtendedProperty(nameSpace, name);
		}

		public IEnumerable<MessageEventRuleProperty> GetExtendedPropertiesEnumerable()
		{
			return this.extendedProperties.GetExtendedPropertiesEnumerable();
		}

		public void AddExtendedProperty(MessageEventRuleProperty extendedProperty)
		{
			this.extendedProperties.AddExtendedProperty(extendedProperty);
			extendedProperty.EventRuleId = this.EventRuleId;
		}

		public override void Accept(IMessageTraceVisitor visitor)
		{
			visitor.Visit(this);
			if (this.actions != null)
			{
				foreach (MessageAction messageAction in this.actions)
				{
					messageAction.Accept(visitor);
				}
			}
			if (this.classifications != null)
			{
				foreach (MessageEventRuleClassification messageEventRuleClassification in this.classifications)
				{
					messageEventRuleClassification.Accept(visitor);
				}
			}
			foreach (MessageEventRuleProperty messageEventRuleProperty in this.GetExtendedPropertiesEnumerable())
			{
				messageEventRuleProperty.Accept(visitor);
			}
		}

		public void Add(MessageAction action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			action.EventRuleId = this.EventRuleId;
			this.Actions.Add(action);
		}

		public void Add(MessageEventRuleClassification classification)
		{
			if (classification == null)
			{
				throw new ArgumentNullException("classification");
			}
			classification.EventRuleId = this.EventRuleId;
			this.Classifications.Add(classification);
		}

		public override Type GetSchemaType()
		{
			return typeof(MessageEventRuleSchema);
		}

		public override HygienePropertyDefinition[] GetAllProperties()
		{
			return MessageEventRule.Properties;
		}

		internal static readonly HygienePropertyDefinition[] Properties = new HygienePropertyDefinition[]
		{
			MessageEventRuleSchema.EventRuleIdProperty,
			MessageEventRuleSchema.EventIdProperty,
			MessageEventRuleSchema.RuleIdProperty,
			MessageEventRuleSchema.RuleTypeProperty,
			CommonMessageTraceSchema.ExMessageIdProperty,
			CommonMessageTraceSchema.EventHashKeyProperty
		};

		private List<MessageAction> actions;

		private List<MessageEventRuleClassification> classifications;

		private ExtendedPropertyStore<MessageEventRuleProperty> extendedProperties;
	}
}

using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageEvent : MessageTraceEntityBase, IExtendedPropertyStore<MessageEventProperty>
	{
		public MessageEvent()
		{
			this.extendedProperties = new ExtendedPropertyStore<MessageEventProperty>();
			this.EventId = IdGenerator.GenerateIdentifier(IdScope.MessageEvent);
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this[MessageEventSchema.EventIdProperty].ToString());
			}
		}

		public Guid ExMessageId
		{
			get
			{
				return (Guid)this[MessageEventSchema.ExMessageIdProperty];
			}
			set
			{
				this[MessageEventSchema.ExMessageIdProperty] = value;
			}
		}

		public Guid EventId
		{
			get
			{
				return (Guid)this[MessageEventSchema.EventIdProperty];
			}
			set
			{
				this[MessageEventSchema.EventIdProperty] = value;
			}
		}

		public MessageTrackingEvent EventType
		{
			get
			{
				return (MessageTrackingEvent)this[MessageEventSchema.EventTypeProperty];
			}
			set
			{
				this[MessageEventSchema.EventTypeProperty] = value;
			}
		}

		public MessageTrackingSource EventSource
		{
			get
			{
				return (MessageTrackingSource)this[MessageEventSchema.EventSourceProperty];
			}
			set
			{
				this[MessageEventSchema.EventSourceProperty] = value;
			}
		}

		public DateTime TimeStamp
		{
			get
			{
				return (DateTime)this[MessageEventSchema.TimeStampProperty];
			}
			set
			{
				this[MessageEventSchema.TimeStampProperty] = value;
			}
		}

		public int ExtendedPropertiesCount
		{
			get
			{
				return this.extendedProperties.ExtendedPropertiesCount;
			}
		}

		public List<MessageEventRule> Rules
		{
			get
			{
				List<MessageEventRule> result;
				if ((result = this.rules) == null)
				{
					result = (this.rules = new List<MessageEventRule>());
				}
				return result;
			}
		}

		public List<MessageEventSourceItem> SourceItems
		{
			get
			{
				List<MessageEventSourceItem> result;
				if ((result = this.sourceItems) == null)
				{
					result = (this.sourceItems = new List<MessageEventSourceItem>());
				}
				return result;
			}
		}

		public List<MessageRecipientStatus> Statuses
		{
			get
			{
				List<MessageRecipientStatus> result;
				if ((result = this.statuses) == null)
				{
					result = (this.statuses = new List<MessageRecipientStatus>());
				}
				return result;
			}
		}

		public bool TryGetExtendedProperty(string nameSpace, string name, out MessageEventProperty extendedProperty)
		{
			return this.extendedProperties.TryGetExtendedProperty(nameSpace, name, out extendedProperty);
		}

		public MessageEventProperty GetExtendedProperty(string nameSpace, string name)
		{
			return this.extendedProperties.GetExtendedProperty(nameSpace, name);
		}

		public IEnumerable<MessageEventProperty> GetExtendedPropertiesEnumerable()
		{
			return this.extendedProperties.GetExtendedPropertiesEnumerable();
		}

		public void AddExtendedProperty(MessageEventProperty extendedProperty)
		{
			this.extendedProperties.AddExtendedProperty(extendedProperty);
			extendedProperty.EventId = this.EventId;
		}

		public override void Accept(IMessageTraceVisitor visitor)
		{
			visitor.Visit(this);
			foreach (MessageEventProperty messageEventProperty in this.GetExtendedPropertiesEnumerable())
			{
				messageEventProperty.Accept(visitor);
			}
			if (this.sourceItems != null)
			{
				foreach (MessageEventSourceItem messageEventSourceItem in this.sourceItems)
				{
					messageEventSourceItem.Accept(visitor);
				}
			}
			if (this.rules != null)
			{
				foreach (MessageEventRule messageEventRule in this.rules)
				{
					messageEventRule.Accept(visitor);
				}
			}
			if (this.statuses != null)
			{
				foreach (MessageRecipientStatus messageRecipientStatus in this.statuses)
				{
					messageRecipientStatus.Accept(visitor);
				}
			}
		}

		public void Add(MessageEventRule eventRule)
		{
			if (eventRule == null)
			{
				throw new ArgumentNullException("eventRule");
			}
			eventRule.EventId = this.EventId;
			this.Rules.Add(eventRule);
		}

		public void Add(MessageEventSourceItem sourceItem)
		{
			if (sourceItem == null)
			{
				throw new ArgumentNullException("sourceItem");
			}
			sourceItem.EventId = this.EventId;
			this.SourceItems.Add(sourceItem);
		}

		public void Add(MessageRecipientStatus status)
		{
			if (status == null)
			{
				throw new ArgumentNullException("status");
			}
			status.EventId = this.EventId;
			this.Statuses.Add(status);
		}

		public override Type GetSchemaType()
		{
			return typeof(MessageEventSchema);
		}

		public override HygienePropertyDefinition[] GetAllProperties()
		{
			return MessageEvent.Properties;
		}

		internal static readonly HygienePropertyDefinition[] Properties = new HygienePropertyDefinition[]
		{
			CommonMessageTraceSchema.ExMessageIdProperty,
			CommonMessageTraceSchema.EventIdProperty,
			MessageEventSchema.EventTypeProperty,
			MessageEventSchema.EventSourceProperty,
			MessageEventSchema.TimeStampProperty,
			CommonMessageTraceSchema.EventHashKeyProperty
		};

		private List<MessageEventRule> rules;

		private List<MessageEventSourceItem> sourceItems;

		private List<MessageRecipientStatus> statuses;

		private ExtendedPropertyStore<MessageEventProperty> extendedProperties;
	}
}

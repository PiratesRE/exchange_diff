using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageRecipient : MessageTraceEntityBase, IExtendedPropertyStore<MessageRecipientProperty>
	{
		public MessageRecipient()
		{
			this.extendedProperties = new ExtendedPropertyStore<MessageRecipientProperty>();
			this.RecipientId = IdGenerator.GenerateIdentifier(IdScope.MessageRecipient);
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

		public Guid RecipientId
		{
			get
			{
				return (Guid)this[CommonMessageTraceSchema.RecipientIdProperty];
			}
			set
			{
				this[CommonMessageTraceSchema.RecipientIdProperty] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this[MessageRecipientSchema.RecipientIdProperty].ToString());
			}
		}

		public string ToEmailPrefix
		{
			get
			{
				return (string)this[MessageRecipientSchema.ToEmailPrefixProperty];
			}
			set
			{
				this[MessageRecipientSchema.ToEmailPrefixProperty] = value;
			}
		}

		public string ToEmailDomain
		{
			get
			{
				return (string)this[MessageRecipientSchema.ToEmailDomainProperty];
			}
			set
			{
				this[MessageRecipientSchema.ToEmailDomainProperty] = value;
			}
		}

		public MailDeliveryStatus MailDeliveryStatus
		{
			get
			{
				return (MailDeliveryStatus)this[MessageRecipientSchema.MailDeliveryStatusProperty];
			}
			set
			{
				this[MessageRecipientSchema.MailDeliveryStatusProperty] = value;
			}
		}

		public int ExtendedPropertiesCount
		{
			get
			{
				return this.extendedProperties.ExtendedPropertiesCount;
			}
		}

		public static string GetEmailAddress(string emailPrefix, string emailDomain)
		{
			if (string.IsNullOrWhiteSpace(emailDomain))
			{
				return MessageTraceEntityBase.StandardizeEmailPrefix(emailPrefix);
			}
			return MessageTraceEntityBase.StandardizeEmailPrefix(emailPrefix) + "@" + emailDomain;
		}

		public bool TryGetExtendedProperty(string nameSpace, string name, out MessageRecipientProperty extendedProperty)
		{
			return this.extendedProperties.TryGetExtendedProperty(nameSpace, name, out extendedProperty);
		}

		public MessageRecipientProperty GetExtendedProperty(string nameSpace, string name)
		{
			return this.extendedProperties.GetExtendedProperty(nameSpace, name);
		}

		public IEnumerable<MessageRecipientProperty> GetExtendedPropertiesEnumerable()
		{
			return this.extendedProperties.GetExtendedPropertiesEnumerable();
		}

		public void AddExtendedProperty(MessageRecipientProperty extendedProperty)
		{
			this.extendedProperties.AddExtendedProperty(extendedProperty);
			extendedProperty.RecipientId = this.RecipientId;
		}

		public override void Accept(IMessageTraceVisitor visitor)
		{
			visitor.Visit(this);
			foreach (MessageRecipientProperty messageRecipientProperty in this.GetExtendedPropertiesEnumerable())
			{
				messageRecipientProperty.Accept(visitor);
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(MessageRecipientSchema);
		}

		public override HygienePropertyDefinition[] GetAllProperties()
		{
			return MessageRecipient.Properties;
		}

		internal static readonly HygienePropertyDefinition[] Properties = new HygienePropertyDefinition[]
		{
			CommonMessageTraceSchema.ExMessageIdProperty,
			CommonMessageTraceSchema.RecipientIdProperty,
			MessageRecipientSchema.ToEmailPrefixProperty,
			MessageRecipientSchema.ToEmailDomainProperty,
			MessageRecipientSchema.MailDeliveryStatusProperty,
			CommonMessageTraceSchema.EmailHashKeyProperty,
			CommonMessageTraceSchema.EmailDomainHashKeyProperty
		};

		private ExtendedPropertyStore<MessageRecipientProperty> extendedProperties;
	}
}

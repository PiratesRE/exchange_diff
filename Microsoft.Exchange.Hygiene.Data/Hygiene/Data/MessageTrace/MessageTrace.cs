using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageTrace : MessageTraceEntityBase, IExtendedPropertyStore<MessageProperty>, IComparable<MessageTrace>
	{
		public MessageTrace()
		{
			this.extendedProperties = new ExtendedPropertyStore<MessageProperty>();
			this.ExMessageId = IdGenerator.GenerateIdentifier(IdScope.MessageTrace);
			this.ClientMessageId = Guid.NewGuid().ToString();
			this.SetReceivedTime(this.ExMessageId);
		}

		public override ObjectId Identity
		{
			get
			{
				return new MessageTraceObjectId(this.OrganizationalUnitRoot, this.ExMessageId);
			}
		}

		public Guid ExMessageId
		{
			get
			{
				return (Guid)this[MessageTraceSchema.ExMessageIdProperty];
			}
			set
			{
				this[MessageTraceSchema.ExMessageIdProperty] = value;
				this.SetReceivedTime(value);
			}
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[MessageTraceSchema.OrganizationalUnitRootProperty];
			}
			set
			{
				this[MessageTraceSchema.OrganizationalUnitRootProperty] = value;
			}
		}

		public string ClientMessageId
		{
			get
			{
				return (string)this[MessageTraceSchema.ClientMessageIdProperty];
			}
			set
			{
				this[MessageTraceSchema.ClientMessageIdProperty] = value;
			}
		}

		public MailDirection Direction
		{
			get
			{
				return (MailDirection)this[MessageTraceSchema.DirectionProperty];
			}
			set
			{
				this[MessageTraceSchema.DirectionProperty] = value;
			}
		}

		public string FromEmailPrefix
		{
			get
			{
				return (string)this[MessageTraceSchema.FromEmailPrefixProperty];
			}
			set
			{
				this[MessageTraceSchema.FromEmailPrefixProperty] = value;
			}
		}

		public string FromEmailDomain
		{
			get
			{
				return (string)this[MessageTraceSchema.FromEmailDomainProperty];
			}
			set
			{
				this[MessageTraceSchema.FromEmailDomainProperty] = value;
			}
		}

		public IPAddress IPAddress
		{
			get
			{
				return (IPAddress)this[MessageTraceSchema.IPAddressProperty];
			}
			set
			{
				this[MessageTraceSchema.IPAddressProperty] = value;
				this.SetIPOctetProperties();
			}
		}

		public byte? IP1
		{
			get
			{
				return (byte?)this[MessageTraceSchema.IP1Property];
			}
			private set
			{
				this[MessageTraceSchema.IP1Property] = value;
			}
		}

		public byte? IP2
		{
			get
			{
				return (byte?)this[MessageTraceSchema.IP2Property];
			}
			private set
			{
				this[MessageTraceSchema.IP2Property] = value;
			}
		}

		public byte? IP3
		{
			get
			{
				return (byte?)this[MessageTraceSchema.IP3Property];
			}
			private set
			{
				this[MessageTraceSchema.IP3Property] = value;
			}
		}

		public byte? IP4
		{
			get
			{
				return (byte?)this[MessageTraceSchema.IP4Property];
			}
			private set
			{
				this[MessageTraceSchema.IP4Property] = value;
			}
		}

		public byte? IP5
		{
			get
			{
				return (byte?)this[MessageTraceSchema.IP5Property];
			}
			private set
			{
				this[MessageTraceSchema.IP5Property] = value;
			}
		}

		public byte? IP6
		{
			get
			{
				return (byte?)this[MessageTraceSchema.IP6Property];
			}
			private set
			{
				this[MessageTraceSchema.IP6Property] = value;
			}
		}

		public byte? IP7
		{
			get
			{
				return (byte?)this[MessageTraceSchema.IP7Property];
			}
			private set
			{
				this[MessageTraceSchema.IP7Property] = value;
			}
		}

		public byte? IP8
		{
			get
			{
				return (byte?)this[MessageTraceSchema.IP8Property];
			}
			private set
			{
				this[MessageTraceSchema.IP8Property] = value;
			}
		}

		public byte? IP9
		{
			get
			{
				return (byte?)this[MessageTraceSchema.IP9Property];
			}
			private set
			{
				this[MessageTraceSchema.IP9Property] = value;
			}
		}

		public byte? IP10
		{
			get
			{
				return (byte?)this[MessageTraceSchema.IP10Property];
			}
			private set
			{
				this[MessageTraceSchema.IP10Property] = value;
			}
		}

		public byte? IP11
		{
			get
			{
				return (byte?)this[MessageTraceSchema.IP11Property];
			}
			private set
			{
				this[MessageTraceSchema.IP11Property] = value;
			}
		}

		public byte? IP12
		{
			get
			{
				return (byte?)this[MessageTraceSchema.IP12Property];
			}
			private set
			{
				this[MessageTraceSchema.IP12Property] = value;
			}
		}

		public byte? IP13
		{
			get
			{
				return (byte?)this[MessageTraceSchema.IP13Property];
			}
			private set
			{
				this[MessageTraceSchema.IP13Property] = value;
			}
		}

		public byte? IP14
		{
			get
			{
				return (byte?)this[MessageTraceSchema.IP14Property];
			}
			private set
			{
				this[MessageTraceSchema.IP14Property] = value;
			}
		}

		public byte? IP15
		{
			get
			{
				return (byte?)this[MessageTraceSchema.IP15Property];
			}
			private set
			{
				this[MessageTraceSchema.IP15Property] = value;
			}
		}

		public byte? IP16
		{
			get
			{
				return (byte?)this[MessageTraceSchema.IP16Property];
			}
			private set
			{
				this[MessageTraceSchema.IP16Property] = value;
			}
		}

		public List<MessageRecipient> Recipients
		{
			get
			{
				List<MessageRecipient> result;
				if ((result = this.msgRecipients) == null)
				{
					result = (this.msgRecipients = new List<MessageRecipient>());
				}
				return result;
			}
		}

		public List<MessageEvent> Events
		{
			get
			{
				List<MessageEvent> result;
				if ((result = this.msgEvents) == null)
				{
					result = (this.msgEvents = new List<MessageEvent>());
				}
				return result;
			}
		}

		public List<MessageClassification> Classifications
		{
			get
			{
				List<MessageClassification> result;
				if ((result = this.msgClassifications) == null)
				{
					result = (this.msgClassifications = new List<MessageClassification>());
				}
				return result;
			}
		}

		public List<MessageClientInformation> ClientInformation
		{
			get
			{
				List<MessageClientInformation> result;
				if ((result = this.msgClientInformation) == null)
				{
					result = (this.msgClientInformation = new List<MessageClientInformation>());
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

		public DateTime ReceivedTime
		{
			get
			{
				return (DateTime)this[MessageTraceSchema.ReceivedTimeProperty];
			}
			private set
			{
				this[MessageTraceSchema.ReceivedTimeProperty] = value;
			}
		}

		public bool TryGetExtendedProperty(string nameSpace, string name, out MessageProperty extendedProperty)
		{
			return this.extendedProperties.TryGetExtendedProperty(nameSpace, name, out extendedProperty);
		}

		public MessageProperty GetExtendedProperty(string nameSpace, string name)
		{
			return this.extendedProperties.GetExtendedProperty(nameSpace, name);
		}

		public IEnumerable<MessageProperty> GetExtendedPropertiesEnumerable()
		{
			return this.extendedProperties.GetExtendedPropertiesEnumerable();
		}

		public void AddExtendedProperty(MessageProperty extendedProperty)
		{
			this.extendedProperties.AddExtendedProperty(extendedProperty);
			extendedProperty.ExMessageId = this.ExMessageId;
		}

		public override void Accept(IMessageTraceVisitor visitor)
		{
			visitor.Visit(this);
			foreach (MessageProperty messageProperty in this.GetExtendedPropertiesEnumerable())
			{
				messageProperty.Accept(visitor);
			}
			if (this.msgRecipients != null)
			{
				foreach (MessageRecipient messageRecipient in this.msgRecipients)
				{
					messageRecipient.Accept(visitor);
				}
			}
			if (this.msgEvents != null)
			{
				foreach (MessageEvent messageEvent in this.msgEvents)
				{
					messageEvent.Accept(visitor);
				}
			}
			if (this.msgClassifications != null)
			{
				foreach (MessageClassification messageClassification in this.msgClassifications)
				{
					messageClassification.Accept(visitor);
				}
			}
			if (this.msgClientInformation != null)
			{
				foreach (MessageClientInformation messageClientInformation in this.msgClientInformation)
				{
					messageClientInformation.Accept(visitor);
				}
			}
		}

		public void Add(MessageRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			recipient.ExMessageId = this.ExMessageId;
			this.Recipients.Add(recipient);
		}

		public void Add(MessageEvent msgEvent)
		{
			if (msgEvent == null)
			{
				throw new ArgumentNullException("msgEvent");
			}
			msgEvent.ExMessageId = this.ExMessageId;
			this.Events.Add(msgEvent);
		}

		public void Add(MessageClassification msgClassification)
		{
			if (msgClassification == null)
			{
				throw new ArgumentNullException("msgClassification");
			}
			msgClassification.ExMessageId = this.ExMessageId;
			this.Classifications.Add(msgClassification);
		}

		public void Add(MessageClientInformation msgClientInformation)
		{
			if (msgClientInformation == null)
			{
				throw new ArgumentNullException("msgClientInformation");
			}
			msgClientInformation.ExMessageId = this.ExMessageId;
			this.ClientInformation.Add(msgClientInformation);
		}

		public override Type GetSchemaType()
		{
			return typeof(MessageTraceSchema);
		}

		public override HygienePropertyDefinition[] GetAllProperties()
		{
			return MessageTrace.Properties;
		}

		public int CompareTo(MessageTrace other)
		{
			if (other == null)
			{
				return 1;
			}
			int num = 0;
			byte[] array = this.ExMessageId.ToByteArray();
			byte[] array2 = other.ExMessageId.ToByteArray();
			int num2 = 10;
			while (num == 0 && num2 < 16)
			{
				num = array[num2].CompareTo(array2[num2]);
				num2++;
			}
			return num;
		}

		private void SetIPOctetProperties()
		{
			if (this.IPAddress == null)
			{
				this.IP1 = (this.IP2 = (this.IP3 = (this.IP4 = (this.IP5 = (this.IP6 = (this.IP7 = (this.IP8 = (this.IP9 = (this.IP10 = (this.IP11 = (this.IP12 = (this.IP13 = (this.IP14 = (this.IP15 = (this.IP16 = null)))))))))))))));
				return;
			}
			byte[] addressBytes = this.IPAddress.GetAddressBytes();
			if (this.IPAddress.AddressFamily == AddressFamily.InterNetwork)
			{
				this.IP1 = new byte?(addressBytes[0]);
				this.IP2 = new byte?(addressBytes[1]);
				this.IP3 = new byte?(addressBytes[2]);
				this.IP4 = new byte?(addressBytes[3]);
				this.IP5 = (this.IP6 = (this.IP7 = (this.IP8 = (this.IP9 = (this.IP10 = (this.IP11 = (this.IP12 = (this.IP13 = (this.IP14 = (this.IP15 = (this.IP16 = null)))))))))));
				return;
			}
			this.IP1 = new byte?(addressBytes[0]);
			this.IP2 = new byte?(addressBytes[1]);
			this.IP3 = new byte?(addressBytes[2]);
			this.IP4 = new byte?(addressBytes[3]);
			this.IP5 = new byte?(addressBytes[4]);
			this.IP6 = new byte?(addressBytes[5]);
			this.IP7 = new byte?(addressBytes[6]);
			this.IP8 = new byte?(addressBytes[7]);
			this.IP9 = new byte?(addressBytes[8]);
			this.IP10 = new byte?(addressBytes[9]);
			this.IP11 = new byte?(addressBytes[10]);
			this.IP12 = new byte?(addressBytes[11]);
			this.IP13 = new byte?(addressBytes[12]);
			this.IP14 = new byte?(addressBytes[13]);
			this.IP15 = new byte?(addressBytes[14]);
			this.IP16 = new byte?(addressBytes[15]);
		}

		private void SetReceivedTime(Guid guid)
		{
			if (CombGuidGenerator.IsCombGuid(guid))
			{
				this.ReceivedTime = CombGuidGenerator.ExtractDateTimeFromCombGuid(guid);
				return;
			}
			this.ReceivedTime = DateTime.UtcNow;
		}

		internal static readonly HygienePropertyDefinition[] Properties = new HygienePropertyDefinition[]
		{
			MessageTraceSchema.OrganizationalUnitRootProperty,
			MessageTraceSchema.ClientMessageIdProperty,
			MessageTraceSchema.ExMessageIdProperty,
			MessageTraceSchema.DirectionProperty,
			MessageTraceSchema.FromEmailPrefixProperty,
			MessageTraceSchema.FromEmailDomainProperty,
			MessageTraceSchema.IPAddressProperty,
			MessageTraceSchema.IP1Property,
			MessageTraceSchema.IP2Property,
			MessageTraceSchema.IP3Property,
			MessageTraceSchema.IP4Property,
			MessageTraceSchema.IP5Property,
			MessageTraceSchema.IP6Property,
			MessageTraceSchema.IP7Property,
			MessageTraceSchema.IP8Property,
			MessageTraceSchema.IP9Property,
			MessageTraceSchema.IP10Property,
			MessageTraceSchema.IP11Property,
			MessageTraceSchema.IP12Property,
			MessageTraceSchema.IP13Property,
			MessageTraceSchema.IP14Property,
			MessageTraceSchema.IP15Property,
			MessageTraceSchema.IP16Property,
			CommonMessageTraceSchema.EmailHashKeyProperty,
			CommonMessageTraceSchema.EmailDomainHashKeyProperty,
			CommonMessageTraceSchema.IPHashKeyProperty,
			CommonMessageTraceSchema.HashBucketProperty,
			MessageTraceSchema.ReceivedTimeProperty
		};

		private ExtendedPropertyStore<MessageProperty> extendedProperties;

		private List<MessageRecipient> msgRecipients;

		private List<MessageEvent> msgEvents;

		private List<MessageClassification> msgClassifications;

		private List<MessageClientInformation> msgClientInformation;
	}
}

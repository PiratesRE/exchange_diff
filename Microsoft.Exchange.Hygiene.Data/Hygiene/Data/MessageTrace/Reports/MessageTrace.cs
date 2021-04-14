using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports
{
	internal class MessageTrace : Schema
	{
		public string Organization
		{
			get
			{
				return (string)this[MessageTrace.OrganizationDefinition];
			}
			set
			{
				this[MessageTrace.OrganizationDefinition] = value;
			}
		}

		public Guid InternalMessageId
		{
			get
			{
				return (Guid)this[MessageTrace.InternalMessageIdDefinition];
			}
			set
			{
				this[MessageTrace.InternalMessageIdDefinition] = value;
			}
		}

		public string ClientMessageId
		{
			get
			{
				return (string)this[MessageTrace.ClientMessageIdDefinition];
			}
			set
			{
				this[MessageTrace.ClientMessageIdDefinition] = value;
			}
		}

		public DateTime Received
		{
			get
			{
				return (DateTime)this[MessageTrace.ReceivedDefinition];
			}
			set
			{
				this[MessageTrace.ReceivedDefinition] = value;
			}
		}

		public string SenderAddress
		{
			get
			{
				return (string)this[MessageTrace.SenderAddressDefinition];
			}
			set
			{
				this[MessageTrace.SenderAddressDefinition] = value;
			}
		}

		public string RecipientAddress
		{
			get
			{
				return (string)this[MessageTrace.RecipientAddressDefinition];
			}
			set
			{
				this[MessageTrace.RecipientAddressDefinition] = value;
			}
		}

		public string MailDeliveryStatus
		{
			get
			{
				return (string)this[MessageTrace.MailDeliveryStatusDefinition];
			}
			set
			{
				this[MessageTrace.MailDeliveryStatusDefinition] = value;
			}
		}

		public string MessageSubject
		{
			get
			{
				return (string)this[MessageTrace.MessageSubjectDefinition];
			}
			set
			{
				this[MessageTrace.MessageSubjectDefinition] = value;
			}
		}

		public int MessageSize
		{
			get
			{
				return (int)this[MessageTrace.MessageSizeDefinition];
			}
			set
			{
				this[MessageTrace.MessageSizeDefinition] = value;
			}
		}

		public string FromIP
		{
			get
			{
				return (string)this[MessageTrace.FromIPAddressDefinition];
			}
			set
			{
				this[MessageTrace.FromIPAddressDefinition] = value;
			}
		}

		public string ToIP
		{
			get
			{
				return (string)this[MessageTrace.ToIPAddressDefinition];
			}
			set
			{
				this[MessageTrace.ToIPAddressDefinition] = value;
			}
		}

		internal static readonly HygienePropertyDefinition OrganizationDefinition = new HygienePropertyDefinition("OrganizationalUnitRootId", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition InternalMessageIdDefinition = new HygienePropertyDefinition("InternalMessageId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ClientMessageIdDefinition = new HygienePropertyDefinition("ClientMessageId", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ReceivedDefinition = new HygienePropertyDefinition("Received", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition SenderAddressDefinition = new HygienePropertyDefinition("SenderAddress", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition RecipientAddressDefinition = new HygienePropertyDefinition("RecipientAddress", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MailDeliveryStatusDefinition = new HygienePropertyDefinition("MailDeliveryStatus", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MessageSubjectDefinition = new HygienePropertyDefinition("MessageSubject", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MessageSizeDefinition = new HygienePropertyDefinition("MessageSize", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition FromIPAddressDefinition = new HygienePropertyDefinition("FromIPAddress", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ToIPAddressDefinition = new HygienePropertyDefinition("ToIPAddress", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}

using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports
{
	internal class SpamMessageDetail : Schema
	{
		public string Organization
		{
			get
			{
				return (string)this[SpamMessageDetail.OrganizationDefinition];
			}
			set
			{
				this[SpamMessageDetail.OrganizationDefinition] = value;
			}
		}

		public string Domain
		{
			get
			{
				return (string)this[SpamMessageDetail.DomainDefinition];
			}
			set
			{
				this[SpamMessageDetail.DomainDefinition] = value;
			}
		}

		public DateTime Received
		{
			get
			{
				return (DateTime)this[SpamMessageDetail.ReceivedDefinition];
			}
			set
			{
				this[SpamMessageDetail.ReceivedDefinition] = value;
			}
		}

		public string ClientMessageId
		{
			get
			{
				return (string)this[SpamMessageDetail.ClientMessageIdDefinition];
			}
			set
			{
				this[SpamMessageDetail.ClientMessageIdDefinition] = value;
			}
		}

		public string Direction
		{
			get
			{
				return (string)this[SpamMessageDetail.DirectionDefinition];
			}
			set
			{
				this[SpamMessageDetail.DirectionDefinition] = value;
			}
		}

		public string RecipientAddress
		{
			get
			{
				return (string)this[SpamMessageDetail.RecipientAddressDefinition];
			}
			set
			{
				this[SpamMessageDetail.RecipientAddressDefinition] = value;
			}
		}

		public string SenderAddress
		{
			get
			{
				return (string)this[SpamMessageDetail.SenderAddressDefinition];
			}
			set
			{
				this[SpamMessageDetail.SenderAddressDefinition] = value;
			}
		}

		public string MessageSubject
		{
			get
			{
				return (string)this[SpamMessageDetail.MessageSubjectDefinition];
			}
			set
			{
				this[SpamMessageDetail.MessageSubjectDefinition] = value;
			}
		}

		public int MessageSize
		{
			get
			{
				return (int)this[SpamMessageDetail.MessageSizeDefinition];
			}
			set
			{
				this[SpamMessageDetail.MessageSizeDefinition] = value;
			}
		}

		public string EventType
		{
			get
			{
				return (string)this[SpamMessageDetail.EventTypeDefinition];
			}
			set
			{
				this[SpamMessageDetail.EventTypeDefinition] = value;
			}
		}

		public string Action
		{
			get
			{
				return (string)this[SpamMessageDetail.ActionDefinition];
			}
			set
			{
				this[SpamMessageDetail.ActionDefinition] = value;
			}
		}

		public Guid InternalMessageId
		{
			get
			{
				return (Guid)this[SpamMessageDetail.InternalMessageIdDefinition];
			}
			set
			{
				this[SpamMessageDetail.InternalMessageIdDefinition] = value;
			}
		}

		internal static readonly HygienePropertyDefinition OrganizationDefinition = new HygienePropertyDefinition("OrganizationalUnitRootId", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DomainDefinition = new HygienePropertyDefinition("DomainName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ReceivedDefinition = new HygienePropertyDefinition("Received", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ClientMessageIdDefinition = new HygienePropertyDefinition("ClientMessageId", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DirectionDefinition = new HygienePropertyDefinition("Direction", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition RecipientAddressDefinition = new HygienePropertyDefinition("RecipientAddress", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition SenderAddressDefinition = new HygienePropertyDefinition("SenderAddress", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MessageSubjectDefinition = new HygienePropertyDefinition("MessageSubject", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MessageSizeDefinition = new HygienePropertyDefinition("MessageSize", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EventTypeDefinition = new HygienePropertyDefinition("EventType", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ActionDefinition = new HygienePropertyDefinition("Action", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition InternalMessageIdDefinition = new HygienePropertyDefinition("InternalMessageId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}

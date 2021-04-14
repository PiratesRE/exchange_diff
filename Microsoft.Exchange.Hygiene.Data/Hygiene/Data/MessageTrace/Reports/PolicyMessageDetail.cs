using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports
{
	internal class PolicyMessageDetail : Schema
	{
		public string Organization
		{
			get
			{
				return (string)this[PolicyMessageDetail.OrganizationDefinition];
			}
			set
			{
				this[PolicyMessageDetail.OrganizationDefinition] = value;
			}
		}

		public string Domain
		{
			get
			{
				return (string)this[PolicyMessageDetail.DomainDefinition];
			}
			set
			{
				this[PolicyMessageDetail.DomainDefinition] = value;
			}
		}

		public DateTime Received
		{
			get
			{
				return (DateTime)this[PolicyMessageDetail.ReceivedDefinition];
			}
			set
			{
				this[PolicyMessageDetail.ReceivedDefinition] = value;
			}
		}

		public string ClientMessageId
		{
			get
			{
				return (string)this[PolicyMessageDetail.ClientMessageIdDefinition];
			}
			set
			{
				this[PolicyMessageDetail.ClientMessageIdDefinition] = value;
			}
		}

		public string Direction
		{
			get
			{
				return (string)this[PolicyMessageDetail.DirectionDefinition];
			}
			set
			{
				this[PolicyMessageDetail.DirectionDefinition] = value;
			}
		}

		public string RecipientAddress
		{
			get
			{
				return (string)this[PolicyMessageDetail.RecipientAddressDefinition];
			}
			set
			{
				this[PolicyMessageDetail.RecipientAddressDefinition] = value;
			}
		}

		public string SenderAddress
		{
			get
			{
				return (string)this[PolicyMessageDetail.SenderAddressDefinition];
			}
			set
			{
				this[PolicyMessageDetail.SenderAddressDefinition] = value;
			}
		}

		public string MessageSubject
		{
			get
			{
				return (string)this[PolicyMessageDetail.MessageSubjectDefinition];
			}
			set
			{
				this[PolicyMessageDetail.MessageSubjectDefinition] = value;
			}
		}

		public int MessageSize
		{
			get
			{
				return (int)this[PolicyMessageDetail.MessageSizeDefinition];
			}
			set
			{
				this[PolicyMessageDetail.MessageSizeDefinition] = value;
			}
		}

		public string EventType
		{
			get
			{
				return (string)this[PolicyMessageDetail.EventTypeDefinition];
			}
			set
			{
				this[PolicyMessageDetail.EventTypeDefinition] = value;
			}
		}

		public string Action
		{
			get
			{
				return (string)this[PolicyMessageDetail.ActionDefinition];
			}
			set
			{
				this[PolicyMessageDetail.ActionDefinition] = value;
			}
		}

		public Guid InternalMessageId
		{
			get
			{
				return (Guid)this[PolicyMessageDetail.InternalMessageIdDefinition];
			}
			set
			{
				this[PolicyMessageDetail.InternalMessageIdDefinition] = value;
			}
		}

		public string TransportRuleName
		{
			get
			{
				return (string)this[PolicyMessageDetail.TransportRuleNameDefinition];
			}
			set
			{
				this[PolicyMessageDetail.TransportRuleNameDefinition] = value;
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

		internal static readonly HygienePropertyDefinition TransportRuleNameDefinition = new HygienePropertyDefinition("TransportRuleName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EventTypeDefinition = new HygienePropertyDefinition("EventType", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ActionDefinition = new HygienePropertyDefinition("Action", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition InternalMessageIdDefinition = new HygienePropertyDefinition("InternalMessageId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}

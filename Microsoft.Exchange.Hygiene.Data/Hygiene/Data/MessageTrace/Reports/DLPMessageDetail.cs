using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports
{
	internal class DLPMessageDetail : Schema
	{
		public string Organization
		{
			get
			{
				return (string)this[DLPMessageDetail.OrganizationDefinition];
			}
			set
			{
				this[DLPMessageDetail.OrganizationDefinition] = value;
			}
		}

		public string Domain
		{
			get
			{
				return (string)this[DLPMessageDetail.DomainDefinition];
			}
			set
			{
				this[DLPMessageDetail.DomainDefinition] = value;
			}
		}

		public DateTime Received
		{
			get
			{
				return (DateTime)this[DLPMessageDetail.ReceivedDefinition];
			}
			set
			{
				this[DLPMessageDetail.ReceivedDefinition] = value;
			}
		}

		public string ClientMessageId
		{
			get
			{
				return (string)this[DLPMessageDetail.ClientMessageIdDefinition];
			}
			set
			{
				this[DLPMessageDetail.ClientMessageIdDefinition] = value;
			}
		}

		public string Direction
		{
			get
			{
				return (string)this[DLPMessageDetail.DirectionDefinition];
			}
			set
			{
				this[DLPMessageDetail.DirectionDefinition] = value;
			}
		}

		public string RecipientAddress
		{
			get
			{
				return (string)this[DLPMessageDetail.RecipientAddressDefinition];
			}
			set
			{
				this[DLPMessageDetail.RecipientAddressDefinition] = value;
			}
		}

		public string SenderAddress
		{
			get
			{
				return (string)this[DLPMessageDetail.SenderAddressDefinition];
			}
			set
			{
				this[DLPMessageDetail.SenderAddressDefinition] = value;
			}
		}

		public string MessageSubject
		{
			get
			{
				return (string)this[DLPMessageDetail.MessageSubjectDefinition];
			}
			set
			{
				this[DLPMessageDetail.MessageSubjectDefinition] = value;
			}
		}

		public int MessageSize
		{
			get
			{
				return (int)this[DLPMessageDetail.MessageSizeDefinition];
			}
			set
			{
				this[DLPMessageDetail.MessageSizeDefinition] = value;
			}
		}

		public string PolicyName
		{
			get
			{
				return (string)this[DLPMessageDetail.PolicyNameDefinition];
			}
			set
			{
				this[DLPMessageDetail.PolicyNameDefinition] = value;
			}
		}

		public string TransportRuleName
		{
			get
			{
				return (string)this[DLPMessageDetail.TransportRuleNameDefinition];
			}
			set
			{
				this[DLPMessageDetail.TransportRuleNameDefinition] = value;
			}
		}

		public string DataClassification
		{
			get
			{
				return (string)this[DLPMessageDetail.DataClassificationDefinition];
			}
			set
			{
				this[DLPMessageDetail.DataClassificationDefinition] = value;
			}
		}

		public int ClassificationConfidence
		{
			get
			{
				return (int)this[DLPMessageDetail.ClassificationConfidenceDefinition];
			}
			set
			{
				this[DLPMessageDetail.ClassificationConfidenceDefinition] = value;
			}
		}

		public int ClassificationCount
		{
			get
			{
				return (int)this[DLPMessageDetail.ClassificationCountDefinition];
			}
			set
			{
				this[DLPMessageDetail.ClassificationCountDefinition] = value;
			}
		}

		public string ClassificationJustification
		{
			get
			{
				return (string)this[DLPMessageDetail.ClassificationJustificationDefinition];
			}
			set
			{
				this[DLPMessageDetail.ClassificationJustificationDefinition] = value;
			}
		}

		public string ClassificationSndoverride
		{
			get
			{
				return (string)this[DLPMessageDetail.ClassificationSndoverrideDefinition];
			}
			set
			{
				this[DLPMessageDetail.ClassificationSndoverrideDefinition] = value;
			}
		}

		public string EventType
		{
			get
			{
				return (string)this[DLPMessageDetail.EventTypeDefinition];
			}
			set
			{
				this[DLPMessageDetail.EventTypeDefinition] = value;
			}
		}

		public string Action
		{
			get
			{
				return (string)this[DLPMessageDetail.ActionDefinition];
			}
			set
			{
				this[DLPMessageDetail.ActionDefinition] = value;
			}
		}

		public Guid InternalMessageId
		{
			get
			{
				return (Guid)this[DLPMessageDetail.InternalMessageIdDefinition];
			}
			set
			{
				this[DLPMessageDetail.InternalMessageIdDefinition] = value;
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

		internal static readonly HygienePropertyDefinition PolicyNameDefinition = new HygienePropertyDefinition("PolicyName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition TransportRuleNameDefinition = new HygienePropertyDefinition("TransportRuleName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DataClassificationDefinition = new HygienePropertyDefinition("DataClassification", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ClassificationConfidenceDefinition = new HygienePropertyDefinition("ClassificationConfidence", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ClassificationCountDefinition = new HygienePropertyDefinition("ClassificationCount", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ClassificationJustificationDefinition = new HygienePropertyDefinition("ClassificationJustification", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ClassificationSndoverrideDefinition = new HygienePropertyDefinition("ClassificationSndoverride", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EventTypeDefinition = new HygienePropertyDefinition("EventType", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ActionDefinition = new HygienePropertyDefinition("Action", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition InternalMessageIdDefinition = new HygienePropertyDefinition("InternalMessageId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}

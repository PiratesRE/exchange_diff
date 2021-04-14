using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports
{
	internal class MessageDetailReport : Schema
	{
		public string Organization
		{
			get
			{
				return (string)this[MessageDetailReport.OrganizationDefinition];
			}
			set
			{
				this[MessageDetailReport.OrganizationDefinition] = value;
			}
		}

		public string Domain
		{
			get
			{
				return (string)this[MessageDetailReport.DomainDefinition];
			}
			set
			{
				this[MessageDetailReport.DomainDefinition] = value;
			}
		}

		public DateTime Date
		{
			get
			{
				return (DateTime)this[MessageDetailReport.DateDefinition];
			}
			set
			{
				this[MessageDetailReport.DateDefinition] = value;
			}
		}

		public Guid InternalMessageId
		{
			get
			{
				return (Guid)this[MessageDetailReport.InternalMessageIdDefinition];
			}
			set
			{
				this[MessageDetailReport.InternalMessageIdDefinition] = value;
			}
		}

		public string MessageId
		{
			get
			{
				return (string)this[MessageDetailReport.MessageIdDefinition];
			}
			set
			{
				this[MessageDetailReport.MessageIdDefinition] = value;
			}
		}

		public string Direction
		{
			get
			{
				return (string)this[MessageDetailReport.DirectionDefinition];
			}
			set
			{
				this[MessageDetailReport.DirectionDefinition] = value;
			}
		}

		public string RecipientAddress
		{
			get
			{
				return (string)this[MessageDetailReport.RecipientAddressDefinition];
			}
			set
			{
				this[MessageDetailReport.RecipientAddressDefinition] = value;
			}
		}

		public string SenderAddress
		{
			get
			{
				return (string)this[MessageDetailReport.SenderAddressDefinition];
			}
			set
			{
				this[MessageDetailReport.SenderAddressDefinition] = value;
			}
		}

		public string SenderIP
		{
			get
			{
				return (string)this[MessageDetailReport.SenderIPDefinition];
			}
			set
			{
				this[MessageDetailReport.SenderIPDefinition] = value;
			}
		}

		public string Subject
		{
			get
			{
				return (string)this[MessageDetailReport.SubjectDefinition];
			}
			set
			{
				this[MessageDetailReport.SubjectDefinition] = value;
			}
		}

		public int MessageSize
		{
			get
			{
				return (int)this[MessageDetailReport.MessageSizeDefinition];
			}
			set
			{
				this[MessageDetailReport.MessageSizeDefinition] = value;
			}
		}

		internal static readonly HygienePropertyDefinition OrganizationDefinition = new HygienePropertyDefinition("OrganizationalUnitRootId", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DomainDefinition = new HygienePropertyDefinition("DomainName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DateDefinition = new HygienePropertyDefinition("Date", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition InternalMessageIdDefinition = new HygienePropertyDefinition("InternalMessageId", typeof(Guid));

		internal static readonly HygienePropertyDefinition MessageIdDefinition = new HygienePropertyDefinition("nvc_ClientMessageId", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DirectionDefinition = new HygienePropertyDefinition("Direction", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition RecipientAddressDefinition = new HygienePropertyDefinition("RecipientAddress", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition SenderIPDefinition = new HygienePropertyDefinition("SenderIP", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition SenderAddressDefinition = new HygienePropertyDefinition("SenderAddress", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition SubjectDefinition = new HygienePropertyDefinition("Subject", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MessageSizeDefinition = new HygienePropertyDefinition("MessageSize", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}

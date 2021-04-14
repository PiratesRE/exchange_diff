using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.Tasks
{
	[Serializable]
	public sealed class MailboxAssociationPresentationObject : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MailboxAssociationPresentationObject.schema;
			}
		}

		public new ObjectId Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				this[SimpleProviderObjectSchema.Identity] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ExternalId
		{
			get
			{
				return (string)this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.ExternalId];
			}
			set
			{
				this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.ExternalId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LegacyDn
		{
			get
			{
				return (string)this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.LegacyDn];
			}
			set
			{
				this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.LegacyDn] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsMember
		{
			get
			{
				return ((bool?)this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.IsMember]).GetValueOrDefault();
			}
			set
			{
				this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.IsMember] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string JoinedBy
		{
			get
			{
				return (string)this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.JoinedBy];
			}
			set
			{
				this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.JoinedBy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress GroupSmtpAddress
		{
			get
			{
				return (SmtpAddress)this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.GroupSmtpAddress];
			}
			set
			{
				this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.GroupSmtpAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress UserSmtpAddress
		{
			get
			{
				return (SmtpAddress)this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.UserSmtpAddress];
			}
			set
			{
				this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.UserSmtpAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsPin
		{
			get
			{
				return ((bool?)this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.IsPin]).GetValueOrDefault();
			}
			set
			{
				this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.IsPin] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ShouldEscalate
		{
			get
			{
				return ((bool?)this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.ShouldEscalate]).GetValueOrDefault();
			}
			set
			{
				this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.ShouldEscalate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsAutoSubscribed
		{
			get
			{
				return ((bool?)this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.IsAutoSubscribed]).GetValueOrDefault();
			}
			set
			{
				this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.IsAutoSubscribed] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExDateTime JoinDate
		{
			get
			{
				return ((ExDateTime?)this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.JoinDate]).GetValueOrDefault();
			}
			set
			{
				this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.JoinDate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExDateTime LastVisitedDate
		{
			get
			{
				return ((ExDateTime?)this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.LastVisitedDate]).GetValueOrDefault();
			}
			set
			{
				this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.LastVisitedDate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExDateTime PinDate
		{
			get
			{
				return ((ExDateTime?)this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.PinDate]).GetValueOrDefault();
			}
			set
			{
				this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.PinDate] = value;
			}
		}

		public ExDateTime LastModified
		{
			get
			{
				return ((ExDateTime?)this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.LastModified]).GetValueOrDefault();
			}
			set
			{
				this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.LastModified] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int CurrentVersion
		{
			get
			{
				return ((int?)this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.CurrentVersion]).GetValueOrDefault();
			}
			set
			{
				this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.CurrentVersion] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int SyncedVersion
		{
			get
			{
				return ((int?)this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.SyncedVersion]).GetValueOrDefault();
			}
			set
			{
				this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.SyncedVersion] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LastSyncError
		{
			get
			{
				return (string)this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.LastSyncError];
			}
			set
			{
				this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.LastSyncError] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int SyncAttempts
		{
			get
			{
				return ((int?)this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.SyncAttempts]).GetValueOrDefault();
			}
			set
			{
				this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.SyncAttempts] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SyncedSchemaVersion
		{
			get
			{
				return (string)this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.SyncedSchemaVersion];
			}
			set
			{
				this[MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.SyncedSchemaVersion] = value;
			}
		}

		public MailboxAssociationPresentationObject() : base(new SimpleProviderPropertyBag())
		{
			base.SetExchangeVersion(ExchangeObjectVersion.Current);
			base.ResetChangeTracking();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("ExternalId=");
			stringBuilder.Append(this.ExternalId);
			stringBuilder.Append(", LegacyDn=");
			stringBuilder.Append(this.LegacyDn);
			stringBuilder.Append(", IsMember=");
			stringBuilder.Append(this.IsMember);
			stringBuilder.Append(", JoinedBy=");
			stringBuilder.Append(this.JoinedBy);
			stringBuilder.Append(", JoinDate=");
			stringBuilder.Append(this.JoinDate);
			stringBuilder.Append(", GroupSmtpAddress=");
			stringBuilder.Append(this.GroupSmtpAddress);
			stringBuilder.Append(", UserSmtpAddress=");
			stringBuilder.Append(this.UserSmtpAddress);
			stringBuilder.Append(", IsPin=");
			stringBuilder.Append(this.IsPin);
			stringBuilder.Append(", ShouldEscalate=");
			stringBuilder.Append(this.ShouldEscalate);
			stringBuilder.Append(", IsAutoSubscribed=");
			stringBuilder.Append(this.IsAutoSubscribed);
			stringBuilder.Append(", LastVisitedDate=");
			stringBuilder.Append(this.LastVisitedDate);
			stringBuilder.Append(", PinDate=");
			stringBuilder.Append(this.PinDate);
			stringBuilder.Append(", CurrentVersion=");
			stringBuilder.Append(this.CurrentVersion);
			stringBuilder.Append(", SyncedVersion=");
			stringBuilder.Append(this.SyncedVersion);
			stringBuilder.Append(", LastSyncError=");
			stringBuilder.Append(this.LastSyncError);
			stringBuilder.Append(", SyncAttempts =");
			stringBuilder.Append(this.SyncAttempts);
			stringBuilder.Append(", SyncedSchemaVersion=");
			stringBuilder.Append(this.SyncedSchemaVersion);
			stringBuilder.Append(", LastModified=");
			stringBuilder.Append(this.LastModified);
			return stringBuilder.ToString();
		}

		internal bool UpdateAssociation(MailboxAssociationFromStore association, IAssociationAdaptor associationAdaptor)
		{
			bool result = false;
			MailboxLocator slaveMailboxLocator = associationAdaptor.GetSlaveMailboxLocator(association);
			if (base.IsChanged(MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.ExternalId))
			{
				slaveMailboxLocator.ExternalId = this.ExternalId;
			}
			if (base.IsChanged(MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.LegacyDn))
			{
				slaveMailboxLocator.LegacyDn = this.LegacyDn;
			}
			if (base.IsChanged(MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.IsMember))
			{
				association.IsMember = this.IsMember;
			}
			if (base.IsChanged(MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.JoinedBy))
			{
				association.JoinedBy = this.JoinedBy;
			}
			if (base.IsChanged(MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.GroupSmtpAddress))
			{
				association.GroupSmtpAddress = this.GroupSmtpAddress;
			}
			if (base.IsChanged(MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.UserSmtpAddress))
			{
				association.UserSmtpAddress = this.UserSmtpAddress;
			}
			if (base.IsChanged(MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.IsPin))
			{
				association.IsPin = this.IsPin;
			}
			if (base.IsChanged(MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.ShouldEscalate))
			{
				association.ShouldEscalate = this.ShouldEscalate;
			}
			if (base.IsChanged(MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.IsAutoSubscribed))
			{
				association.IsAutoSubscribed = this.IsAutoSubscribed;
			}
			if (base.IsChanged(MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.JoinDate))
			{
				association.JoinDate = this.JoinDate;
			}
			if (base.IsChanged(MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.LastVisitedDate))
			{
				association.LastVisitedDate = this.LastVisitedDate;
			}
			if (base.IsChanged(MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.PinDate))
			{
				association.PinDate = this.PinDate;
			}
			if (base.IsChanged(MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.CurrentVersion))
			{
				association.CurrentVersion = this.CurrentVersion;
				result = true;
			}
			if (base.IsChanged(MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.SyncedVersion))
			{
				association.SyncedVersion = this.SyncedVersion;
				result = true;
			}
			if (base.IsChanged(MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.LastSyncError))
			{
				association.LastSyncError = this.LastSyncError;
				result = true;
			}
			if (base.IsChanged(MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.SyncAttempts))
			{
				association.SyncAttempts = this.SyncAttempts;
				result = true;
			}
			if (base.IsChanged(MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema.SyncedSchemaVersion))
			{
				association.SyncedSchemaVersion = this.SyncedSchemaVersion;
				result = true;
			}
			return result;
		}

		private static MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema schema = ObjectSchema.GetInstance<MailboxAssociationPresentationObject.MailboxAssociationPresentationObjectSchema>();

		internal class MailboxAssociationPresentationObjectSchema : SimpleProviderObjectSchema
		{
			public static readonly SimpleProviderPropertyDefinition ExternalId = new SimpleProviderPropertyDefinition("ExternalId", ExchangeObjectVersion.Current, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition LegacyDn = new SimpleProviderPropertyDefinition("LegacyDn", ExchangeObjectVersion.Current, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition IsMember = new SimpleProviderPropertyDefinition("IsMember", ExchangeObjectVersion.Current, typeof(bool?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition JoinedBy = new SimpleProviderPropertyDefinition("JoinedBy", ExchangeObjectVersion.Current, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition IsPin = new SimpleProviderPropertyDefinition("IsPin", ExchangeObjectVersion.Current, typeof(bool?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition ShouldEscalate = new SimpleProviderPropertyDefinition("ShouldEscalate", ExchangeObjectVersion.Current, typeof(bool?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition IsAutoSubscribed = new SimpleProviderPropertyDefinition("IsAutoSubscribed", ExchangeObjectVersion.Current, typeof(bool?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition JoinDate = new SimpleProviderPropertyDefinition("JoinDate", ExchangeObjectVersion.Current, typeof(ExDateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition GroupSmtpAddress = new SimpleProviderPropertyDefinition("GroupSmtpAddress", ExchangeObjectVersion.Current, typeof(SmtpAddress?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition UserSmtpAddress = new SimpleProviderPropertyDefinition("UserSmtpAddress", ExchangeObjectVersion.Current, typeof(SmtpAddress?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition LastVisitedDate = new SimpleProviderPropertyDefinition("LastVisitedDate", ExchangeObjectVersion.Current, typeof(ExDateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition PinDate = new SimpleProviderPropertyDefinition("PinDate", ExchangeObjectVersion.Current, typeof(ExDateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition LastModified = new SimpleProviderPropertyDefinition("LastModified", ExchangeObjectVersion.Current, typeof(ExDateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition CurrentVersion = new SimpleProviderPropertyDefinition("CurrentVersion", ExchangeObjectVersion.Current, typeof(int?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition SyncedVersion = new SimpleProviderPropertyDefinition("SyncedVersion", ExchangeObjectVersion.Current, typeof(int?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition LastSyncError = new SimpleProviderPropertyDefinition("LastSyncError", ExchangeObjectVersion.Current, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition SyncAttempts = new SimpleProviderPropertyDefinition("SyncAttempts", ExchangeObjectVersion.Current, typeof(int?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition SyncedSchemaVersion = new SimpleProviderPropertyDefinition("SyncedSchemaVersion", ExchangeObjectVersion.Current, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}
	}
}

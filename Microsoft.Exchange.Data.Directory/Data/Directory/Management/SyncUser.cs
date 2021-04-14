using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[ProvisioningObjectTag("SyncUser")]
	[Serializable]
	public class SyncUser : User
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return SyncUser.schema;
			}
		}

		public SyncUser()
		{
			base.SetObjectClass("user");
		}

		public SyncUser(ADUser dataObject) : base(dataObject)
		{
		}

		internal new static SyncUser FromDataObject(ADUser dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new SyncUser(dataObject);
		}

		[Parameter(Mandatory = false)]
		public string OnPremisesObjectId
		{
			get
			{
				return (string)this[SyncUserSchema.OnPremisesObjectId];
			}
			set
			{
				this[SyncUserSchema.OnPremisesObjectId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsDirSynced
		{
			get
			{
				return (bool)this[SyncUserSchema.IsDirSynced];
			}
			set
			{
				this[SyncUserSchema.IsDirSynced] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ReleaseTrack? ReleaseTrack
		{
			get
			{
				return (ReleaseTrack?)this[SyncUserSchema.ReleaseTrack];
			}
			set
			{
				this[SyncUserSchema.ReleaseTrack] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> DirSyncAuthorityMetadata
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncUserSchema.DirSyncAuthorityMetadata];
			}
			set
			{
				this[SyncUserSchema.DirSyncAuthorityMetadata] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CountryInfo UsageLocation
		{
			get
			{
				return (CountryInfo)this[SyncUserSchema.UsageLocation];
			}
			set
			{
				this[SyncUserSchema.UsageLocation] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RemoteRecipientType RemoteRecipientType
		{
			get
			{
				return (RemoteRecipientType)this[SyncUserSchema.RemoteRecipientType];
			}
			set
			{
				this[SyncUserSchema.RemoteRecipientType] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AccountDisabled
		{
			get
			{
				return (bool)this[SyncUserSchema.AccountDisabled];
			}
			set
			{
				this[SyncUserSchema.AccountDisabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? StsRefreshTokensValidFrom
		{
			get
			{
				return (DateTime?)this[SyncUserSchema.StsRefreshTokensValidFrom];
			}
			set
			{
				this[SyncUserSchema.StsRefreshTokensValidFrom] = value;
			}
		}

		private static SyncUserSchema schema = ObjectSchema.GetInstance<SyncUserSchema>();
	}
}

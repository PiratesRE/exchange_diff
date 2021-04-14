using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class IRMConfiguration : ADContainer
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return IRMConfiguration.adSchema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchControlPointConfig";
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return IRMConfiguration.parentPath;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		[Parameter(Mandatory = false)]
		public TransportDecryptionSetting TransportDecryptionSetting
		{
			get
			{
				if ((bool)this[IRMConfigurationSchema.TransportDecryptionOptional])
				{
					return TransportDecryptionSetting.Optional;
				}
				if ((bool)this[IRMConfigurationSchema.TransportDecryptionMandatory])
				{
					return TransportDecryptionSetting.Mandatory;
				}
				return TransportDecryptionSetting.Disabled;
			}
			set
			{
				if (value == TransportDecryptionSetting.Optional)
				{
					this[IRMConfigurationSchema.TransportDecryptionOptional] = true;
					this[IRMConfigurationSchema.TransportDecryptionMandatory] = false;
					return;
				}
				if (value == TransportDecryptionSetting.Mandatory)
				{
					this[IRMConfigurationSchema.TransportDecryptionMandatory] = true;
					this[IRMConfigurationSchema.TransportDecryptionOptional] = false;
					return;
				}
				this[IRMConfigurationSchema.TransportDecryptionMandatory] = false;
				this[IRMConfigurationSchema.TransportDecryptionOptional] = false;
			}
		}

		[Parameter]
		public Uri ServiceLocation
		{
			get
			{
				return (Uri)this[IRMConfigurationSchema.ServiceLocation];
			}
			set
			{
				this[IRMConfigurationSchema.ServiceLocation] = value;
			}
		}

		[Parameter]
		public Uri PublishingLocation
		{
			get
			{
				return (Uri)this[IRMConfigurationSchema.PublishingLocation];
			}
			set
			{
				this[IRMConfigurationSchema.PublishingLocation] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<Uri> LicensingLocation
		{
			get
			{
				return (MultiValuedProperty<Uri>)this[IRMConfigurationSchema.LicensingLocation];
			}
			set
			{
				this[IRMConfigurationSchema.LicensingLocation] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool JournalReportDecryptionEnabled
		{
			get
			{
				return (bool)this[IRMConfigurationSchema.JournalReportDecryptionEnabled];
			}
			set
			{
				this[IRMConfigurationSchema.JournalReportDecryptionEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExternalLicensingEnabled
		{
			get
			{
				return (bool)this[IRMConfigurationSchema.ExternalLicensingEnabled];
			}
			set
			{
				this[IRMConfigurationSchema.ExternalLicensingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool InternalLicensingEnabled
		{
			get
			{
				return (bool)this[IRMConfigurationSchema.InternalLicensingEnabled];
			}
			set
			{
				this[IRMConfigurationSchema.InternalLicensingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SearchEnabled
		{
			get
			{
				return (bool)this[IRMConfigurationSchema.SearchEnabled];
			}
			set
			{
				this[IRMConfigurationSchema.SearchEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ClientAccessServerEnabled
		{
			get
			{
				return (bool)this[IRMConfigurationSchema.ClientAccessServerEnabled];
			}
			set
			{
				this[IRMConfigurationSchema.ClientAccessServerEnabled] = value;
			}
		}

		public bool InternetConfidentialEnabled
		{
			get
			{
				return (bool)this[IRMConfigurationSchema.InternetConfidentialEnabled];
			}
			set
			{
				this[IRMConfigurationSchema.InternetConfidentialEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EDiscoverySuperUserEnabled
		{
			get
			{
				return !(bool)this[IRMConfigurationSchema.EDiscoverySuperUserDisabled];
			}
			set
			{
				this[IRMConfigurationSchema.EDiscoverySuperUserDisabled] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public Uri RMSOnlineKeySharingLocation
		{
			get
			{
				return (Uri)this[IRMConfigurationSchema.RMSOnlineKeySharingLocation];
			}
			set
			{
				this[IRMConfigurationSchema.RMSOnlineKeySharingLocation] = value;
			}
		}

		public int ServerCertificatesVersion
		{
			get
			{
				return (int)this[IRMConfigurationSchema.ServerCertificatesVersion];
			}
			set
			{
				this[IRMConfigurationSchema.ServerCertificatesVersion] = value;
			}
		}

		public string SharedServerBoxRacIdentity
		{
			get
			{
				return (string)this[IRMConfigurationSchema.SharedServerBoxRacIdentity];
			}
			set
			{
				this[IRMConfigurationSchema.SharedServerBoxRacIdentity] = value;
			}
		}

		public string RMSOnlineVersion
		{
			get
			{
				return (string)this[IRMConfigurationSchema.RMSOnlineVersion];
			}
			set
			{
				this[IRMConfigurationSchema.RMSOnlineVersion] = value;
			}
		}

		internal static IRMConfiguration Read(IConfigurationSession session)
		{
			bool flag;
			return IRMConfiguration.Read(session, out flag);
		}

		internal static IRMConfiguration Read(IConfigurationSession session, out bool inMemory)
		{
			inMemory = false;
			IRMConfiguration[] array = session.Find<IRMConfiguration>(null, QueryScope.SubTree, null, null, 1);
			if (array != null && array.Length != 0 && array[0] != null)
			{
				return array[0];
			}
			inMemory = true;
			IRMConfiguration irmconfiguration = new IRMConfiguration();
			irmconfiguration.SetId(session, "ControlPoint Config");
			irmconfiguration.OrganizationId = session.SessionSettings.CurrentOrganizationId;
			if (session.SessionSettings.CurrentOrganizationId != OrganizationId.ForestWideOrgId)
			{
				irmconfiguration.ExternalLicensingEnabled = true;
			}
			return irmconfiguration;
		}

		private const string MostDerivedClassInternal = "msExchControlPointConfig";

		private const string singletonName = "ControlPoint Config";

		private static readonly IRMConfigurationSchema adSchema = ObjectSchema.GetInstance<IRMConfigurationSchema>();

		private static readonly ADObjectId parentPath = new ADObjectId("CN=Transport Settings");
	}
}

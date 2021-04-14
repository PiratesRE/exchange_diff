using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.EdgeSync
{
	[Cmdlet("New", "EdgeSyncServiceConfig", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class NewEdgeSyncServiceConfig : NewFixedNameSystemConfigurationObjectTask<EdgeSyncServiceConfig>
	{
		[Parameter(Mandatory = false)]
		public AdSiteIdParameter Site
		{
			get
			{
				return (AdSiteIdParameter)base.Fields["Site"];
			}
			set
			{
				base.Fields["Site"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ConfigurationSyncInterval
		{
			get
			{
				return this.DataObject.ConfigurationSyncInterval;
			}
			set
			{
				this.DataObject.ConfigurationSyncInterval = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan RecipientSyncInterval
		{
			get
			{
				return this.DataObject.RecipientSyncInterval;
			}
			set
			{
				this.DataObject.RecipientSyncInterval = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan LockDuration
		{
			get
			{
				return this.DataObject.LockDuration;
			}
			set
			{
				this.DataObject.LockDuration = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan LockRenewalDuration
		{
			get
			{
				return this.DataObject.LockRenewalDuration;
			}
			set
			{
				this.DataObject.LockRenewalDuration = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan OptionDuration
		{
			get
			{
				return this.DataObject.OptionDuration;
			}
			set
			{
				this.DataObject.OptionDuration = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan CookieValidDuration
		{
			get
			{
				return this.DataObject.CookieValidDuration;
			}
			set
			{
				this.DataObject.CookieValidDuration = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan FailoverDCInterval
		{
			get
			{
				return this.DataObject.FailoverDCInterval;
			}
			set
			{
				this.DataObject.FailoverDCInterval = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool LogEnabled
		{
			get
			{
				return this.DataObject.LogEnabled;
			}
			set
			{
				this.DataObject.LogEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan LogMaxAge
		{
			get
			{
				return this.DataObject.LogMaxAge;
			}
			set
			{
				this.DataObject.LogMaxAge = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> LogMaxDirectorySize
		{
			get
			{
				return this.DataObject.LogMaxDirectorySize;
			}
			set
			{
				this.DataObject.LogMaxDirectorySize = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> LogMaxFileSize
		{
			get
			{
				return this.DataObject.LogMaxFileSize;
			}
			set
			{
				this.DataObject.LogMaxFileSize = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EdgeSyncLoggingLevel LogLevel
		{
			get
			{
				return this.DataObject.LogLevel;
			}
			set
			{
				this.DataObject.LogLevel = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LogPath
		{
			get
			{
				return this.DataObject.LogPath;
			}
			set
			{
				this.DataObject.LogPath = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.Site == null)
				{
					return Strings.ConfirmationMessageNewEdgeSyncServiceConfigOnLocalSite;
				}
				return Strings.ConfirmationMessageNewEdgeSyncServiceConfigWithSiteSpecified(this.Site.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 213, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\EdgeSync\\NewEdgeSyncServiceConfig.cs");
		}

		protected override IConfigurable PrepareDataObject()
		{
			base.PrepareDataObject();
			this.DataObject.Name = "EdgeSyncService";
			if (this.Site == null)
			{
				this.siteObject = ((ITopologyConfigurationSession)base.DataSession).GetLocalSite();
				if (this.siteObject == null)
				{
					base.WriteError(new NeedToSpecifyADSiteObjectException(), ErrorCategory.InvalidOperation, this.DataObject);
				}
			}
			else
			{
				this.siteObject = (ADSite)base.GetDataObject<ADSite>(this.Site, base.DataSession, null, new LocalizedString?(Strings.ErrorSiteNotFound(this.Site.ToString())), new LocalizedString?(Strings.ErrorSiteNotUnique(this.Site.ToString())));
			}
			ADObjectId childId = this.siteObject.Id.GetChildId("EdgeSyncService");
			this.DataObject.SetId(childId);
			return this.DataObject;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (!EdgeSyncServiceConfig.ValidLogSizeCompatibility(this.LogMaxFileSize, this.LogMaxDirectorySize, this.siteObject.Id, (ITopologyConfigurationSession)base.DataSession))
			{
				base.WriteError(new InvalidOperationException(), ErrorCategory.InvalidOperation, this.DataObject);
			}
		}

		private const string DefaultCommonName = "EdgeSyncService";

		private ADSite siteObject;
	}
}

using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync;
using Microsoft.Exchange.EdgeSync.Ehf;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.EdgeSync
{
	[Cmdlet("New", "EdgeSyncEhfConnector", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class NewEdgeSyncEhfConnector : NewSystemConfigurationObjectTask<EdgeSyncEhfConnector>
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
		public bool Enabled
		{
			get
			{
				return (bool)(base.Fields["Enabled"] ?? true);
			}
			set
			{
				base.Fields["Enabled"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public Uri ProvisioningUrl
		{
			get
			{
				return (Uri)base.Fields["ProvisioningUrl"];
			}
			set
			{
				base.Fields["ProvisioningUrl"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string PrimaryLeaseLocation
		{
			get
			{
				return (string)base.Fields["PrimaryLeaseLocation"];
			}
			set
			{
				base.Fields["PrimaryLeaseLocation"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string BackupLeaseLocation
		{
			get
			{
				return (string)base.Fields["BackupLeaseLocation"];
			}
			set
			{
				base.Fields["BackupLeaseLocation"] = value;
			}
		}

		[Parameter]
		public PSCredential AuthenticationCredential
		{
			get
			{
				return (PSCredential)base.Fields["AuthenticationCredential"];
			}
			set
			{
				base.Fields["AuthenticationCredential"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string ResellerId
		{
			get
			{
				return (string)base.Fields["ResellerId"];
			}
			set
			{
				base.Fields["ResellerId"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AdminSyncEnabled
		{
			get
			{
				return (bool)(base.Fields["AdminSyncEnabled"] ?? false);
			}
			set
			{
				base.Fields["AdminSyncEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int Version
		{
			get
			{
				return (int)(base.Fields["Version"] ?? 1);
			}
			set
			{
				base.Fields["Version"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.Site == null)
				{
					return Strings.ConfirmationMessageNewEdgeSyncEhfConnectorOnLocalSite;
				}
				return Strings.ConfirmationMessageNewEdgeSyncEhfConnectorWithSiteSpecified(this.Site.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 151, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\EdgeSync\\NewEdgeSyncEHFConnector.cs");
		}

		protected override IConfigurable PrepareDataObject()
		{
			base.PrepareDataObject();
			this.SetID();
			this.DataObject.PrimaryLeaseLocation = this.PrimaryLeaseLocation;
			this.DataObject.BackupLeaseLocation = this.BackupLeaseLocation;
			this.DataObject.AuthenticationCredential = this.AuthenticationCredential;
			this.DataObject.Enabled = this.Enabled;
			this.DataObject.ProvisioningUrl = this.ProvisioningUrl;
			this.DataObject.ResellerId = this.ResellerId;
			this.DataObject.SynchronizationProvider = "Microsoft.Exchange.EdgeSync.Ehf.EhfSynchronizationProvider";
			this.DataObject.AssemblyPath = "Microsoft.Exchange.EdgeSync.DatacenterProviders.dll";
			this.DataObject.AdminSyncEnabled = this.AdminSyncEnabled;
			this.DataObject.Version = this.Version;
			return this.DataObject;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (!Utils.IsLeaseDirectoryValidPath(this.PrimaryLeaseLocation))
			{
				base.WriteError(new ArgumentException(Strings.InvalidPrimaryLeaseLocation), ErrorCategory.InvalidArgument, this.DataObject);
			}
			if (!Utils.IsLeaseDirectoryValidPath(this.BackupLeaseLocation))
			{
				base.WriteError(new ArgumentException(Strings.InvalidBackupLeaseLocation), ErrorCategory.InvalidArgument, this.DataObject);
			}
			try
			{
				EhfSynchronizationProvider.ValidateProvisioningUrl(this.ProvisioningUrl, this.AuthenticationCredential, base.Name);
			}
			catch (ExDirectoryException ex)
			{
				base.WriteError(ex.InnerException ?? ex, ErrorCategory.InvalidArgument, this.DataObject);
			}
			if (this.Enabled)
			{
				EdgeSyncEhfConnector edgeSyncEhfConnector = Utils.FindEnabledEhfSyncConnector((IConfigurationSession)base.DataSession, null);
				if (edgeSyncEhfConnector != null)
				{
					base.WriteError(new ArgumentException(Strings.EnabledEhfConnectorAlreadyExists(edgeSyncEhfConnector.DistinguishedName)), ErrorCategory.InvalidArgument, this.DataObject);
				}
			}
		}

		private void SetID()
		{
			ADSite adsite;
			if (this.Site == null)
			{
				adsite = ((ITopologyConfigurationSession)base.DataSession).GetLocalSite();
				if (adsite == null)
				{
					base.WriteError(new NeedToSpecifyADSiteObjectException(), ErrorCategory.InvalidOperation, this.DataObject);
				}
			}
			else
			{
				adsite = (ADSite)base.GetDataObject<ADSite>(this.Site, base.DataSession, null, new LocalizedString?(Strings.ErrorSiteNotFound(this.Site.ToString())), new LocalizedString?(Strings.ErrorSiteNotUnique(this.Site.ToString())));
				if (adsite == null)
				{
					return;
				}
			}
			ADObjectId id = adsite.Id;
			ADObjectId childId = id.GetChildId("EdgeSyncService").GetChildId(this.DataObject.Name);
			this.DataObject.SetId(childId);
		}
	}
}

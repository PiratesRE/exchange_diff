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
	[Cmdlet("New", "EdgeSyncMservConnector", SupportsShouldProcess = true, DefaultParameterSetName = "Simple")]
	public sealed class NewEdgeSyncMservConnector : NewSystemConfigurationObjectTask<EdgeSyncMservConnector>
	{
		[Parameter(ParameterSetName = "Simple", Mandatory = false)]
		[Parameter(ParameterSetName = "Custom", Mandatory = false)]
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

		[Parameter(ParameterSetName = "Custom", Mandatory = true)]
		public Uri ProvisionUrl
		{
			get
			{
				return (Uri)base.Fields["ProvisionUrl"];
			}
			set
			{
				base.Fields["ProvisionUrl"] = value;
			}
		}

		[Parameter(ParameterSetName = "Custom", Mandatory = true)]
		public Uri SettingUrl
		{
			get
			{
				return (Uri)base.Fields["SettingUrl"];
			}
			set
			{
				base.Fields["SettingUrl"] = value;
			}
		}

		[Parameter(ParameterSetName = "Custom", Mandatory = true)]
		public string LocalCertificate
		{
			get
			{
				return (string)base.Fields["LocalCertificate"];
			}
			set
			{
				base.Fields["LocalCertificate"] = value;
			}
		}

		[Parameter(ParameterSetName = "Custom", Mandatory = true)]
		public string RemoteCertificate
		{
			get
			{
				return (string)base.Fields["RemoteCertificate"];
			}
			set
			{
				base.Fields["RemoteCertificate"] = value;
			}
		}

		[Parameter(ParameterSetName = "Custom", Mandatory = true)]
		[Parameter(ParameterSetName = "Simple", Mandatory = true)]
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

		[Parameter(ParameterSetName = "Simple", Mandatory = true)]
		[Parameter(ParameterSetName = "Custom", Mandatory = true)]
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

		[Parameter(ParameterSetName = "Custom", Mandatory = false)]
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.Site == null)
				{
					return Strings.ConfirmationMessageNewEdgeSyncMservConnectorOnLocalSite;
				}
				return Strings.ConfirmationMessageNewEdgeSyncMservConnectorWithSiteSpecified(this.Site.ToString());
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			base.PrepareDataObject();
			this.SetID();
			this.SetDefaultValue();
			this.DataObject.SynchronizationProvider = "Microsoft.Exchange.EdgeSync.Mserve.MserveSynchronizationProvider";
			this.DataObject.AssemblyPath = "Microsoft.Exchange.EdgeSync.DatacenterProviders.dll";
			this.DataObject.ProvisionUrl = this.ProvisionUrl;
			this.DataObject.SettingUrl = this.SettingUrl;
			this.DataObject.LocalCertificate = this.LocalCertificate;
			this.DataObject.RemoteCertificate = this.RemoteCertificate;
			this.DataObject.PrimaryLeaseLocation = this.PrimaryLeaseLocation;
			this.DataObject.BackupLeaseLocation = this.BackupLeaseLocation;
			this.DataObject.Enabled = this.Enabled;
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
				base.WriteError(new InvalidOperationException(Strings.InvalidPrimaryLeaseLocation), ErrorCategory.InvalidOperation, this.DataObject);
			}
			if (!Utils.IsLeaseDirectoryValidPath(this.BackupLeaseLocation))
			{
				base.WriteError(new InvalidOperationException(Strings.InvalidBackupLeaseLocation), ErrorCategory.InvalidOperation, this.DataObject);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 251, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\EdgeSync\\NewEdgeSyncMservConnector.cs");
		}

		private void SetDefaultValue()
		{
			try
			{
				ServiceEndpointContainer endpointContainer = ((ITopologyConfigurationSession)base.DataSession).GetEndpointContainer();
				ServiceEndpoint endpoint = endpointContainer.GetEndpoint(ServiceEndpointId.DeltaSyncPartnerProvision);
				ServiceEndpoint endpoint2 = endpointContainer.GetEndpoint(ServiceEndpointId.DeltaSyncPartnerSettings);
				ServiceEndpoint endpoint3 = endpointContainer.GetEndpoint(ServiceEndpointId.DeltaSyncPartnerClientCertificate);
				if (!base.Fields.Contains("ProvisionUrl"))
				{
					this.ProvisionUrl = endpoint.Uri;
				}
				if (!base.Fields.Contains("SettingUrl"))
				{
					this.SettingUrl = endpoint2.Uri;
				}
				if (!base.Fields.Contains("LocalCertificate"))
				{
					this.LocalCertificate = endpoint3.CertificateSubject;
				}
				if (!base.Fields.Contains("RemoteCertificate"))
				{
					this.RemoteCertificate = endpoint.CertificateSubject;
				}
			}
			catch (ServiceEndpointNotFoundException exception)
			{
				base.WriteError(exception, ErrorCategory.ObjectNotFound, null);
			}
			catch (EndpointContainerNotFoundException exception2)
			{
				base.WriteError(exception2, ErrorCategory.ObjectNotFound, null);
			}
			catch (ADTransientException exception3)
			{
				base.WriteError(exception3, ErrorCategory.NotSpecified, null);
			}
			catch (ADOperationException exception4)
			{
				base.WriteError(exception4, ErrorCategory.NotSpecified, null);
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

		private const string SimpleParameterSetName = "Simple";

		private const string CustomParameterSetName = "Custom";

		private const string SiteParam = "Site";

		private const string ProvisionUrlParam = "ProvisionUrl";

		private const string SettingUrlParam = "SettingUrl";

		private const string LocalCertificateParam = "LocalCertificate";

		private const string RemoteCertificateParam = "RemoteCertificate";

		private const string PrimaryLeaseLocationParam = "PrimaryLeaseLocation";

		private const string BackupLeaseLocationParam = "BackupLeaseLocation";

		private const string EnabledParam = "Enabled";
	}
}

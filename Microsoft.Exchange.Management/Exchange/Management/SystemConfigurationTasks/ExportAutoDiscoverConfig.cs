using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Export", "AutoDiscoverConfig", SupportsShouldProcess = true)]
	public sealed class ExportAutoDiscoverConfig : Task
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationExportAutoDiscoverConfig;
			}
		}

		[Parameter]
		public Fqdn DomainController
		{
			get
			{
				return (Fqdn)base.Fields["DomainController"];
			}
			set
			{
				base.Fields["DomainController"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public PSCredential SourceForestCredential
		{
			get
			{
				return (PSCredential)base.Fields["SourceForestCredential"];
			}
			set
			{
				base.Fields["SourceForestCredential"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Fqdn PreferredSourceFqdn
		{
			get
			{
				return (Fqdn)base.Fields["PreferredSourceFqdn"];
			}
			set
			{
				base.Fields["PreferredSourceFqdn"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? DeleteConfig
		{
			get
			{
				return (bool?)base.Fields["DeleteConfig"];
			}
			set
			{
				base.Fields["DeleteConfig"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string TargetForestDomainController
		{
			get
			{
				return (string)base.Fields["TargetForestDomainController"];
			}
			set
			{
				base.Fields["TargetForestDomainController"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public PSCredential TargetForestCredential
		{
			get
			{
				return (PSCredential)base.Fields["TargetForestCredential"];
			}
			set
			{
				base.Fields["TargetForestCredential"] = value;
			}
		}

		[Parameter]
		public bool MultipleExchangeDeployments
		{
			get
			{
				return (bool)(base.Fields["MultipleExchangeDeployments"] ?? false);
			}
			set
			{
				base.Fields["MultipleExchangeDeployments"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			try
			{
				this.sourceConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, (this.SourceForestCredential == null) ? null : this.SourceForestCredential.GetNetworkCredential(), ADSessionSettings.FromRootOrgScopeSet(), 153, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\AutoDiscover\\ExportAutodiscoverConfig.cs");
				this.serverConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 161, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\AutoDiscover\\ExportAutodiscoverConfig.cs");
				string path = "LDAP://" + this.TargetForestDomainController + "/rootDSE";
				DirectoryEntry directoryEntry = null;
				if (this.TargetForestCredential == null)
				{
					directoryEntry = new DirectoryEntry(path);
				}
				else
				{
					try
					{
						string password = this.TargetForestCredential.Password.ConvertToUnsecureString();
						directoryEntry = new DirectoryEntry(path, this.TargetForestCredential.UserName.Replace("\\\\", "\\"), password);
					}
					catch (DirectoryServicesCOMException exception)
					{
						base.ThrowTerminatingError(exception, ErrorCategory.InvalidArgument, null);
					}
				}
				try
				{
					directoryEntry.AuthenticationType = (AuthenticationTypes.Signing | AuthenticationTypes.Sealing);
					string str = directoryEntry.Properties["configurationNamingContext"][0] as string;
					this.targetServiceContainer = "CN=Services," + str;
				}
				catch (COMException exception2)
				{
					base.ThrowTerminatingError(exception2, ErrorCategory.InvalidArgument, null);
				}
				path = "LDAP://" + this.TargetForestDomainController + "/" + this.targetServiceContainer;
				directoryEntry = null;
				if (this.TargetForestCredential == null)
				{
					directoryEntry = new DirectoryEntry(path);
				}
				else
				{
					try
					{
						string password2 = this.TargetForestCredential.Password.ConvertToUnsecureString();
						directoryEntry = new DirectoryEntry(path, this.TargetForestCredential.UserName.Replace("\\\\", "\\"), password2);
					}
					catch (DirectoryServicesCOMException exception3)
					{
						base.ThrowTerminatingError(exception3, ErrorCategory.InvalidArgument, null);
					}
				}
				directoryEntry.AuthenticationType = (AuthenticationTypes.Signing | AuthenticationTypes.Sealing);
				try
				{
					if (this.mea == null)
					{
						this.mea = directoryEntry.Children.Add("CN=Microsoft Exchange Autodiscover", "container");
						this.mea.CommitChanges();
					}
				}
				catch (DirectoryServicesCOMException)
				{
				}
				catch (UnauthorizedAccessException)
				{
					this.WriteWarning(Strings.EADCInsufficientRights("Export-AutoDiscoverConfig"));
				}
				try
				{
					this.mea = directoryEntry.Children.Find("CN=Microsoft Exchange Autodiscover");
				}
				catch (DirectoryServicesCOMException)
				{
				}
				catch (UnauthorizedAccessException)
				{
					this.WriteWarning(Strings.EADCInsufficientRights("Export-AutoDiscoverConfig"));
				}
				if (this.mea == null)
				{
					base.WriteError(new InvalidOperationException(Strings.EADCInsufficientRights("Export-AutoDiscoverConfig")), ErrorCategory.InvalidOperation, null);
				}
				this.sourceServiceContainer = this.sourceConfigSession.GetServicesContainer();
				ServicesContainer servicesContainer = this.serverConfigSession.GetServicesContainer();
				if (servicesContainer.Id.Equals(this.sourceServiceContainer.Id))
				{
					this.inSource = true;
				}
				if (servicesContainer.Id.Equals(this.targetServiceContainer))
				{
					this.inTarget = true;
				}
			}
			catch (PSArgumentException exception4)
			{
				base.WriteError(exception4, ErrorCategory.InvalidArgument, null);
			}
			catch (LocalizedException exception5)
			{
				base.WriteError(exception5, ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			if (this.inSource && this.inTarget)
			{
				base.WriteError(new InvalidOperationException(Strings.ExportAutoDiscoverSameForest), ErrorCategory.InvalidOperation, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				this.DeleteCurrentSCP();
				if (this.DeleteConfig == null || this.DeleteConfig == false)
				{
					this.CreateNewSCP();
				}
			}
			catch (DirectoryServicesCOMException)
			{
			}
			catch (LocalizedException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
			}
			catch (UnauthorizedAccessException)
			{
				this.WriteWarning(Strings.EADCInsufficientRights("Export-AutoDiscoverConfig"));
			}
		}

		private void CreateNewSCP()
		{
			DirectoryEntry directoryEntry = this.mea.Children.Add(this.ConfigCN(), "ServiceConnectionPoint");
			directoryEntry.Properties["Keywords"].Add("67661d7F-8FC4-4fa7-BFAC-E1D7794C1F68");
			if (this.MultipleExchangeDeployments)
			{
				ADPagedReader<AcceptedDomain> adpagedReader = this.sourceConfigSession.FindAllPaged<AcceptedDomain>();
				int num = 0;
				if (adpagedReader != null)
				{
					foreach (AcceptedDomain acceptedDomain in adpagedReader)
					{
						num++;
						if (acceptedDomain.DomainType == AcceptedDomainType.Authoritative)
						{
							directoryEntry.Properties["Keywords"].Add("Domain=" + acceptedDomain.DomainName.Domain);
						}
					}
				}
				if (num == 0)
				{
					this.WriteWarning(Strings.EADCWeakSourceCreds);
				}
			}
			directoryEntry.Properties["ServiceBindingInformation"].Value = "LDAP://" + this.SourceForestFqdn();
			directoryEntry.CommitChanges();
		}

		private void DeleteCurrentSCP()
		{
			try
			{
				DirectoryEntry entry = this.mea.Children.Find(this.ConfigCN());
				this.mea.Children.Remove(entry);
			}
			catch (DirectoryServicesCOMException)
			{
			}
		}

		private string ConfigCN()
		{
			if (this.configCN == null)
			{
				this.configCN = "CN=" + this.SourceForestFqdn();
			}
			return this.configCN;
		}

		private string SourceForestFqdn()
		{
			string result = string.Empty;
			if (this.PreferredSourceFqdn != null)
			{
				result = this.PreferredSourceFqdn.ToString();
			}
			else if (this.inSource)
			{
				result = ADForest.GetLocalForest(this.sourceConfigSession.DomainController).Fqdn;
			}
			else
			{
				result = NativeHelpers.CanonicalNameFromDistinguishedName(this.sourceServiceContainer.Id.Parent.Parent.DistinguishedName);
			}
			return result;
		}

		private ITopologyConfigurationSession sourceConfigSession;

		private ServicesContainer sourceServiceContainer;

		private string targetServiceContainer;

		private ITopologyConfigurationSession serverConfigSession;

		private bool inSource;

		private bool inTarget;

		private DirectoryEntry mea;

		private string configCN;
	}
}

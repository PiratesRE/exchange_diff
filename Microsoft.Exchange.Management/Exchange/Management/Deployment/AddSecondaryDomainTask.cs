using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Add", "SecondaryDomain", SupportsShouldProcess = true, DefaultParameterSetName = "OrgScopedParameterSet")]
	public sealed class AddSecondaryDomainTask : SecondaryDomainTaskBase
	{
		private ExchangeConfigurationUnit TenantCU
		{
			get
			{
				return (ExchangeConfigurationUnit)base.Fields["TenantCU"];
			}
			set
			{
				base.Fields["TenantCU"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AuthenticationType AuthenticationType
		{
			get
			{
				return (AuthenticationType)(base.Fields["AuthenticationType"] ?? AuthenticationType.Managed);
			}
			set
			{
				base.Fields["AuthenticationType"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LiveIdInstanceType LiveIdInstanceType
		{
			get
			{
				return (LiveIdInstanceType)(base.Fields["LiveIdInstanceType"] ?? LiveIdInstanceType.Consumer);
			}
			set
			{
				base.Fields["LiveIdInstanceType"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OutBoundOnly { get; set; }

		[Parameter(Mandatory = false)]
		public bool MakeDefault { get; set; }

		public AddSecondaryDomainTask()
		{
			base.Fields["InstallationMode"] = InstallationModes.Install;
			base.Fields["PrepareOrganization"] = true;
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.AddSecondaryDomainDescription;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageAddSecondaryDomain(this.Organization.ToString(), this.DomainName.ToString());
			}
		}

		[Parameter(Mandatory = true, Position = 0, ParameterSetName = "OrgScopedParameterSet")]
		[Parameter(Mandatory = true, Position = 0, ParameterSetName = "DefaultParameterSet")]
		public string Name
		{
			get
			{
				return (string)base.Fields["SecondaryDomainName"];
			}
			set
			{
				base.Fields["SecondaryDomainName"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "OrgScopedParameterSet")]
		[Parameter(Mandatory = true, ParameterSetName = "DefaultParameterSet")]
		public SmtpDomain DomainName
		{
			get
			{
				return (SmtpDomain)base.Fields["SecondarySmtpDomainName"];
			}
			set
			{
				base.Fields["SecondarySmtpDomainName"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "DefaultParameterSet")]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["PrimaryOrganization"];
			}
			set
			{
				base.Fields["PrimaryOrganization"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "OrgScopedParameterSet")]
		[Parameter(Mandatory = false, ParameterSetName = "DefaultParameterSet")]
		public SwitchParameter DomainOwnershipVerified
		{
			get
			{
				return (SwitchParameter)(base.Fields["DomainOwnershipVerified"] ?? false);
			}
			set
			{
				base.Fields["DomainOwnershipVerified"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "OrgScopedParameterSet")]
		[Parameter(Mandatory = false, ParameterSetName = "DefaultParameterSet")]
		public AcceptedDomainType DomainType
		{
			get
			{
				return (AcceptedDomainType)(base.Fields["DomainType"] ?? AcceptedDomainType.Authoritative);
			}
			set
			{
				base.Fields["DomainType"] = value;
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new AddSecondaryDomainTaskModuleFactory();
		}

		private void WriteWrappedError(Exception exception, ErrorCategory category, object target)
		{
			OrganizationValidationException exception2 = new OrganizationValidationException(Strings.ErrorValidationException(exception.ToString()), exception);
			base.WriteError(exception2, category, target);
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			LocalizedString empty = LocalizedString.Empty;
			this.Name = MailboxTaskHelper.GetNameOfAcceptableLengthForMultiTenantMode(this.Name, out empty);
			if (empty != LocalizedString.Empty)
			{
				this.WriteWarning(empty);
			}
			base.InternalBeginProcessing();
			if (this.Organization == null)
			{
				if (base.CurrentOrganizationId == OrganizationId.ForestWideOrgId)
				{
					base.WriteError(new ArgumentException(Strings.ErrorOrganizationParameterRequired), ErrorCategory.InvalidOperation, null);
				}
				else
				{
					this.Organization = new OrganizationIdParameter(base.CurrentOrganizationId.OrganizationalUnit.Name);
				}
			}
			base.CurrentOrganizationId = this.ResolveCurrentOrganization();
			OrganizationTaskHelper.ValidateParamString("Name", this.Name, new Task.TaskErrorLoggingDelegate(base.WriteError));
			if (base.Fields["AuthenticationType"] == null)
			{
				this.AuthenticationType = AuthenticationType.Managed;
			}
			if (base.Fields["LiveIdInstanceType"] == null)
			{
				this.LiveIdInstanceType = LiveIdInstanceType.Consumer;
			}
			base.Fields["OutBoundOnly"] = this.OutBoundOnly;
			base.Fields["MakeDefault"] = this.MakeDefault;
			string value = string.Empty;
			if (this.Organization != null)
			{
				PartitionId partitionIdByAcceptedDomainName = ADAccountPartitionLocator.GetPartitionIdByAcceptedDomainName(this.Organization.RawIdentity);
				if (partitionIdByAcceptedDomainName != null)
				{
					value = base.ServerSettings.PreferredGlobalCatalog(partitionIdByAcceptedDomainName.ForestFQDN);
				}
			}
			base.Fields["PreferredServer"] = value;
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			this.CheckForDuplicateExistingDomain();
			AcceptedDomain acceptedDomain = new AcceptedDomain();
			acceptedDomain.Name = this.Name;
			acceptedDomain.DomainName = new SmtpDomainWithSubdomains(this.DomainName, false);
			acceptedDomain.DomainType = this.DomainType;
			acceptedDomain.SetId(base.Session, this.DomainName.ToString());
			acceptedDomain.OrganizationId = base.CurrentOrganizationId;
			NewAcceptedDomain.ValidateDomainName(acceptedDomain, new Task.TaskErrorLoggingDelegate(this.WriteWrappedError));
			if (!this.DomainOwnershipVerified && this.AuthenticationType == AuthenticationType.Federated)
			{
				bool flag = false;
				AcceptedDomainIdParameter acceptedDomainIdParameter = AcceptedDomainIdParameter.Parse("*");
				IEnumerable<AcceptedDomain> objects = acceptedDomainIdParameter.GetObjects<AcceptedDomain>(this.TenantCU.Id, base.Session);
				foreach (AcceptedDomain acceptedDomain2 in objects)
				{
					SmtpDomainWithSubdomains smtpDomainWithSubdomains = new SmtpDomainWithSubdomains(acceptedDomain2.DomainName.Domain.ToString(), true);
					if (smtpDomainWithSubdomains.Match(this.DomainName.ToString()) == acceptedDomain2.DomainName.Domain.Length)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					base.WriteError(new OrganizationTaskException(Strings.ErrorTenantAdminsCanOnlyAddSubdomains(this.DomainName.ToString())), ErrorCategory.InvalidOperation, null);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.TenantCU.OrganizationStatus == OrganizationStatus.PendingAcceptedDomainAddition || this.TenantCU.OrganizationStatus == OrganizationStatus.PendingAcceptedDomainRemoval)
			{
				OrganizationTaskHelper.SetOrganizationStatus(base.Session, this.TenantCU, OrganizationStatus.Active);
			}
			try
			{
				base.InternalProcessRecord();
			}
			catch (Exception)
			{
				this.CleanupSecondaryDomain();
				throw;
			}
			if (!base.HasErrors)
			{
				AcceptedDomain acceptedDomain = OrganizationTaskHelper.GetAcceptedDomain(AcceptedDomainIdParameter.Parse(this.Name), base.Session, new Task.TaskErrorLoggingDelegate(base.WriteError), true);
				base.WriteObject(acceptedDomain);
			}
			else
			{
				this.CleanupSecondaryDomain();
			}
			TaskLogger.LogExit();
		}

		private void CheckForDuplicateExistingDomain()
		{
			AcceptedDomain acceptedDomain = OrganizationTaskHelper.GetAcceptedDomain(AcceptedDomainIdParameter.Parse(this.Name), base.Session, new Task.TaskErrorLoggingDelegate(base.WriteError), false);
			if (acceptedDomain != null)
			{
				base.WriteError(new ManagementObjectAlreadyExistsException(Strings.ErrorAcceptedDomainExists(this.Name)), ErrorCategory.ResourceExists, null);
			}
		}

		private void CleanupSecondaryDomain()
		{
			AcceptedDomain acceptedDomain = OrganizationTaskHelper.GetAcceptedDomain(AcceptedDomainIdParameter.Parse(this.Name), base.Session, new Task.TaskErrorLoggingDelegate(base.WriteError), false);
			if (acceptedDomain != null)
			{
				try
				{
					base.Session.Delete(acceptedDomain);
				}
				catch (Exception ex)
				{
					this.WriteWarning(Strings.ErrorNonActiveOrganizationFound(ex.ToString()));
				}
			}
		}

		internal override IConfigurationSession CreateSession()
		{
			PartitionId partitionIdByAcceptedDomainName = ADAccountPartitionLocator.GetPartitionIdByAcceptedDomainName(this.Organization.RawIdentity);
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerId(partitionIdByAcceptedDomainName.ForestFQDN, null, null), base.CurrentOrganizationId, base.ExecutingUserOrganizationId, false);
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.RescopeToSubtree(sessionSettings), 480, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\AddSecondaryDomainTask.cs");
		}

		private OrganizationId ResolveCurrentOrganization()
		{
			base.Session = this.CreateSession();
			if (!OrganizationTaskHelper.CanProceedWithOrganizationTask(this.Organization, base.Session, AddSecondaryDomainTask.IgnorableFlagsOnStatusTimeout, new Task.TaskErrorLoggingDelegate(base.WriteError)))
			{
				base.WriteError(new OrganizationPendingOperationException(Strings.ErrorCannotOperateOnOrgInCurrentState), ErrorCategory.InvalidOperation, null);
			}
			base.Session.UseConfigNC = false;
			ADOrganizationalUnit oufromOrganizationId = OrganizationTaskHelper.GetOUFromOrganizationId(this.Organization, base.Session, new Task.TaskErrorLoggingDelegate(base.WriteError), true);
			base.Session.UseConfigNC = true;
			this.TenantCU = OrganizationTaskHelper.GetExchangeConfigUnitFromOrganizationId(this.Organization, base.Session, new Task.TaskErrorLoggingDelegate(base.WriteError), true);
			base.Fields["TenantExternalDirectoryOrganizationId"] = this.TenantCU.ExternalDirectoryOrganizationId;
			return oufromOrganizationId.OrganizationId;
		}

		private const string OrgScopedParameterSet = "OrgScopedParameterSet";

		private const string DefaultParameterSet = "DefaultParameterSet";

		private static readonly OrganizationStatus[] IgnorableFlagsOnStatusTimeout = new OrganizationStatus[]
		{
			OrganizationStatus.PendingAcceptedDomainAddition,
			OrganizationStatus.PendingAcceptedDomainRemoval
		};
	}
}

using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.RightsManagement;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[Cmdlet("Set", "RMSTemplate", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetRMSTemplate : SetTaskBase<RmsTemplatePresentation>
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public RmsTemplateIdParameter Identity
		{
			get
			{
				return (RmsTemplateIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public RmsTemplateType Type
		{
			get
			{
				return (RmsTemplateType)base.Fields["Type"];
			}
			set
			{
				base.Fields["Type"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetRMSTemplate(this.Identity.ToString());
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (this.Organization != null)
			{
				ADObjectId rootOrgContainerId = ADSystemConfigurationSession.GetRootOrgContainerId(this.DomainController, null);
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, ConfigScopes.TenantSubTree, 105, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\rms\\SetRmsTemplate.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
				base.CurrentOrganizationId = adorganizationalUnit.OrganizationId;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			if (OrganizationId.ForestWideOrgId.Equals(base.CurrentOrganizationId))
			{
				base.WriteError(new ArgumentException(Strings.TenantOrganizationMissing, string.Empty), (ErrorCategory)1000, null);
			}
			ADObjectId rootOrgContainerIdForLocalForest = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
			ADSessionSettings sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, rootOrgContainerIdForLocalForest, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 149, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\rms\\SetRmsTemplate.cs");
			return new RmsTemplateDataProvider(tenantOrTopologyConfigurationSession, RmsTemplateType.All, true);
		}

		protected override IConfigurable PrepareDataObject()
		{
			IConfigurable[] array = ((RmsTemplateDataProvider)base.DataSession).Find<RmsTemplatePresentation>(new RmsTemplateQueryFilter(this.Identity.TemplateId, this.Identity.TemplateName), null, false, null);
			if (array == null || array.Length == 0)
			{
				base.WriteError(new RmsTemplateNotFoundException(this.Identity.ToString()), (ErrorCategory)1000, this.Identity);
			}
			return array[0];
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				TaskLogger.LogExit();
				return;
			}
			Guid templateId = ((RmsTemplateIdentity)this.DataObject.Identity).TemplateId;
			if (templateId == RmsTemplate.DoNotForward.Id || templateId == RmsTemplate.InternetConfidential.Id)
			{
				base.WriteError(new CannotModifyOneOffTemplatesException(), (ErrorCategory)1000, this.Identity);
			}
			if (this.Type != RmsTemplateType.Archived && this.Type != RmsTemplateType.Distributed)
			{
				base.WriteError(new TemplateTypeNotValidException(this.Type.ToString()), (ErrorCategory)1000, this.Type);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			if (this.DataObject.Type == this.Type)
			{
				this.WriteWarning(Strings.WarningTemplateNotModified(this.DataObject.Identity.ToString()));
				return;
			}
			if (this.DataObject.Type == RmsTemplateType.Distributed && this.Type == RmsTemplateType.Archived)
			{
				this.WriteWarning(Strings.WarningChangeTemplateState(this.DataObject.Identity.ToString()));
			}
			this.DataObject.Type = this.Type;
			base.InternalProcessRecord();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || RmsUtil.IsKnownException(exception);
		}
	}
}

using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("Move", "ManagementRoleAssignment", SupportsShouldProcess = true)]
	public sealed class MoveManagementRoleAssignment : SystemConfigurationObjectActionTask<RoleAssignmentIdParameter, ExchangeRoleAssignment>
	{
		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override RoleAssignmentIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = true)]
		public SecurityPrincipalIdParameter User
		{
			get
			{
				return (SecurityPrincipalIdParameter)base.Fields[RbacCommonParameters.ParameterUser];
			}
			set
			{
				base.Fields[RbacCommonParameters.ParameterUser] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			this.DataObject = (ExchangeRoleAssignment)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			ADRecipient adrecipient = (ADRecipient)base.GetDataObject<ADRecipient>(this.User, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorUserOrSecurityGroupNotFound(this.User.ToString())), new LocalizedString?(Strings.ErrorUserOrSecurityGroupNotUnique(this.User.ToString())));
			RoleHelper.ValidateRoleAssignmentUser(adrecipient, new Task.TaskErrorLoggingDelegate(base.WriteError), false);
			this.originalUserId = this.DataObject.User;
			this.DataObject.User = adrecipient.Id;
			this.DataObject.RoleAssigneeType = ExchangeRoleAssignment.RoleAssigneeTypeFromADRecipient(adrecipient);
			((IDirectorySession)base.DataSession).LinkResolutionServer = adrecipient.OriginatingServer;
			if (!adrecipient.OrganizationId.Equals(OrganizationId.ForestWideOrgId) && !adrecipient.OrganizationId.Equals(this.DataObject.OrganizationId) && (OrganizationId.ForestWideOrgId.Equals(this.DataObject.OrganizationId) || !this.DataObject.OrganizationId.OrganizationalUnit.IsDescendantOf(adrecipient.OrganizationId.OrganizationalUnit)))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorOrgUserBeAssignedToParentOrg(this.User.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Id);
			}
			TaskLogger.LogExit();
			return this.DataObject;
		}

		protected override void InternalProcessRecord()
		{
			if (this.IsNonDeprecatedRole())
			{
				this.CloneRoleAssignment(this.DataObject);
				base.InternalProcessRecord();
			}
		}

		private void CloneRoleAssignment(ExchangeRoleAssignment templateAssignment)
		{
			ExchangeRoleAssignment exchangeRoleAssignment = new ExchangeRoleAssignment();
			exchangeRoleAssignment.ProvisionalClone(templateAssignment);
			exchangeRoleAssignment.User = this.originalUserId;
			string unescapedCommonName = templateAssignment.Name.Substring(0, Math.Min(templateAssignment.Name.Length, 55)) + "_" + (Environment.TickCount % 1000).ToString("0000");
			exchangeRoleAssignment.SetId(templateAssignment.Id.Parent.GetChildId(unescapedCommonName));
			base.WriteVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(exchangeRoleAssignment, templateAssignment.Session, typeof(ExchangeRoleAssignment)));
			templateAssignment.Session.Save(exchangeRoleAssignment);
		}

		private bool IsNonDeprecatedRole()
		{
			ExchangeRole exchangeRole = (ExchangeRole)base.GetDataObject<ExchangeRole>(new RoleIdParameter(this.DataObject.Role), base.DataSession, null, new LocalizedString?(Strings.ErrorRoleNotFound(this.DataObject.Role.ToString())), new LocalizedString?(Strings.ErrorRoleNotUnique(this.DataObject.Role.ToString())));
			if (exchangeRole != null && exchangeRole.IsDeprecated)
			{
				this.WriteWarning(Strings.ErrorCannotMoveRoleAssignmentOfDeprecatedRole(exchangeRole.ToString()));
				return false;
			}
			return true;
		}

		private ADObjectId originalUserId;
	}
}

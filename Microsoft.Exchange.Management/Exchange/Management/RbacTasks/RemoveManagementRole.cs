using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("Remove", "ManagementRole", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveManagementRole : RemoveSystemConfigurationObjectTask<RoleIdParameter, ExchangeRole>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter]
		public SwitchParameter Recurse
		{
			get
			{
				return (SwitchParameter)(base.Fields["Recurse"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Recurse"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter UnScopedTopLevel
		{
			get
			{
				return (SwitchParameter)(base.Fields["UnScopedTopLevel"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["UnScopedTopLevel"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.Recurse)
				{
					return Strings.ConfirmationMessageRemoveManagementRoleRecursive(this.Identity.ToString(), this.rolesToRemove);
				}
				return Strings.ConfirmationMessageRemoveManagementRole(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				if ((!base.DataObject.IsUnscopedTopLevel || !this.UnScopedTopLevel) && base.DataObject.IsRootRole)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorCannotDeletePrecannedRole(this.Identity.ToString())), ErrorCategory.InvalidOperation, base.DataObject.Id);
				}
				if (!base.DataObject.IsUnscopedTopLevel && this.UnScopedTopLevel)
				{
					base.WriteError(new InvalidOperationException(Strings.ParameterAllowedOnlyForTopLevelRoleManipulation("UnScopedTopLevel", RoleType.UnScoped.ToString())), ErrorCategory.InvalidOperation, null);
				}
			}
			ADPagedReader<ADRawEntry> adpagedReader = this.ConfigurationSession.FindPagedADRawEntryWithDefaultFilters<ExchangeRole>(base.DataObject.Id, this.Recurse ? QueryScope.SubTree : QueryScope.Base, null, null, 0, new PropertyDefinition[]
			{
				ADObjectSchema.Id
			});
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ADRawEntry adrawEntry in adpagedReader)
			{
				stringBuilder.Append("\n\t");
				stringBuilder.Append(adrawEntry.Id.ToString());
			}
			this.rolesToRemove = stringBuilder.ToString();
			TaskLogger.LogExit();
		}

		private void MoveToNextPercent()
		{
			this.currentPercent = 5 + this.currentPercent % 95;
		}

		private void ReportDeleteTreeProgress(ADTreeDeleteNotFinishedException de)
		{
			this.MoveToNextPercent();
			if (de != null)
			{
				base.WriteVerbose(de.LocalizedString);
			}
			else
			{
				base.WriteVerbose(Strings.ProgressStatusRemovingManagementRoleTree);
			}
			base.WriteProgress(Strings.ProgressActivityRemovingManagementRoleTree(base.DataObject.Id.ToString()), Strings.ProgressStatusRemovingManagementRoleTree, this.currentPercent);
		}

		protected override IConfigurable ResolveDataObject()
		{
			SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			return base.ResolveDataObject();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (!this.Force && SharedConfiguration.IsSharedConfiguration(base.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(base.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			ADPagedReader<ADRawEntry> adpagedReader = this.ConfigurationSession.FindPagedADRawEntryWithDefaultFilters<ExchangeRole>(base.DataObject.Id, this.Recurse ? QueryScope.SubTree : QueryScope.Base, new ExistsFilter(ExchangeRoleSchema.RoleAssignments), null, 0, new PropertyDefinition[]
			{
				ADObjectSchema.Id,
				ExchangeRoleSchema.RoleAssignments
			});
			foreach (ADRawEntry adrawEntry in adpagedReader)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorCannotDeleteRoleWithAssignment(adrawEntry.Id.ToString())), ErrorCategory.InvalidOperation, base.DataObject.Id);
			}
			if (this.Recurse)
			{
				((IConfigurationSession)base.DataSession).DeleteTree(base.DataObject, new TreeDeleteNotFinishedHandler(this.ReportDeleteTreeProgress));
			}
			else
			{
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}

		private const string ParameterRecurse = "Recurse";

		private int currentPercent;

		private string rolesToRemove;
	}
}

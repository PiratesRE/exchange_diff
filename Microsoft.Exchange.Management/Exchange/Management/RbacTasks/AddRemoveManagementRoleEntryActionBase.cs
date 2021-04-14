using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RbacTasks
{
	public abstract class AddRemoveManagementRoleEntryActionBase : ManagementRoleEntryActionBase
	{
		protected abstract void InternalAddRemoveRoleEntry(MultiValuedProperty<RoleEntry> roleEntries);

		protected abstract string GetRoleEntryString();

		protected abstract LocalizedException GetRoleSaveException(string roleEntry, string role, string exception);

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			base.Validate(this.DataObject);
			if (base.HasErrors)
			{
				return;
			}
			if (!base.Force && SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			this.DataObject.CheckWritable();
			string key = this.DataObject.Guid.ToString();
			if (this.roleAndSessionCache.ContainsKey(key))
			{
				AddRemoveManagementRoleEntryActionBase.RoleAndSession roleAndSession = this.roleAndSessionCache[key];
				this.InternalAddRemoveRoleEntry(roleAndSession.Role.RoleEntries);
				base.Validate(roleAndSession.Role);
				if (base.HasErrors)
				{
					return;
				}
			}
			else
			{
				this.roleAndSessionCache[key] = new AddRemoveManagementRoleEntryActionBase.RoleAndSession(this.DataObject, base.DataSession, base.ParentRole);
			}
			this.roleAndSessionCache[key].PipelinedElements.Add(new AddRemoveManagementRoleEntryActionBase.PipelinedElement(base.CurrentObjectIndex, this.GetRoleEntryString()));
			TaskLogger.LogExit();
		}

		protected override void InternalEndProcessing()
		{
			foreach (AddRemoveManagementRoleEntryActionBase.RoleAndSession roleAndSession in this.roleAndSessionCache.Values)
			{
				LocalizedException ex = null;
				try
				{
					if (roleAndSession.Role.Identity != null)
					{
						base.WriteVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(roleAndSession.Role, roleAndSession.ConfigSession, typeof(ExchangeRole)));
					}
					roleAndSession.Role.ApplyChangesToDownlevelData(roleAndSession.ParentRole ?? roleAndSession.Role);
					roleAndSession.ConfigSession.Save(roleAndSession.Role);
				}
				catch (ADOperationException ex2)
				{
					ex = ex2;
				}
				catch (DataSourceTransientException ex3)
				{
					ex = ex3;
				}
				finally
				{
					if (roleAndSession.Role.Identity != null)
					{
						base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(roleAndSession.ConfigSession));
					}
				}
				if (ex != null)
				{
					foreach (AddRemoveManagementRoleEntryActionBase.PipelinedElement pipelinedElement in roleAndSession.PipelinedElements)
					{
						this.WriteError(this.GetRoleSaveException(pipelinedElement.RoleEntryString, roleAndSession.Role.Id.ToString(), ex.Message), ErrorCategory.WriteError, pipelinedElement.ObjectIndex, false);
					}
				}
			}
			base.InternalEndProcessing();
		}

		private Dictionary<string, AddRemoveManagementRoleEntryActionBase.RoleAndSession> roleAndSessionCache = new Dictionary<string, AddRemoveManagementRoleEntryActionBase.RoleAndSession>(StringComparer.InvariantCultureIgnoreCase);

		private struct PipelinedElement
		{
			internal PipelinedElement(int objectIndex, string roleEntryString)
			{
				this.ObjectIndex = objectIndex;
				this.RoleEntryString = roleEntryString;
			}

			internal int ObjectIndex;

			internal string RoleEntryString;
		}

		private struct RoleAndSession
		{
			internal RoleAndSession(ExchangeRole role, IConfigDataProvider configSession, ExchangeRole parentRole)
			{
				this.Role = role;
				this.ConfigSession = configSession;
				this.ParentRole = parentRole;
				this.PipelinedElements = new List<AddRemoveManagementRoleEntryActionBase.PipelinedElement>();
			}

			internal ExchangeRole Role;

			internal IConfigDataProvider ConfigSession;

			internal ExchangeRole ParentRole;

			internal List<AddRemoveManagementRoleEntryActionBase.PipelinedElement> PipelinedElements;
		}
	}
}

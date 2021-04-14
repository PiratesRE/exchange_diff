using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal class CmdletBasedRunspaceConfiguration : ExchangeRunspaceConfiguration
	{
		protected CmdletBasedRunspaceConfiguration(string identityName) : base(new GenericIdentity(identityName), new ExchangeRunspaceConfigurationSettings(ExchangeRunspaceConfigurationSettings.ExchangeApplication.EMC, null, ExchangeRunspaceConfigurationSettings.SerializationLevel.Partial))
		{
		}

		public static CmdletBasedRunspaceConfiguration Create(MonadConnectionInfo connectionInfo, string identityName, IReportProgress reportProgress)
		{
			CmdletBasedRunspaceConfiguration.reportProgress = reportProgress;
			CmdletBasedRunspaceConfiguration.connectionInfo = connectionInfo;
			return new CmdletBasedRunspaceConfiguration(identityName);
		}

		protected override SerializedAccessToken PopulateGroupMemberships(IIdentity identity)
		{
			return null;
		}

		protected override ICollection<SecurityIdentifier> GetGroupAccountsSIDs(IIdentity logonIdentity)
		{
			if (logonIdentity is WindowsIdentity)
			{
				return base.GetGroupAccountsSIDs(logonIdentity);
			}
			return null;
		}

		internal SmtpAddress LogonUserLiveID
		{
			get
			{
				if (this.executingUser != null)
				{
					return (SmtpAddress)this.executingUser[ADRecipientSchema.WindowsLiveID];
				}
				return SmtpAddress.Empty;
			}
		}

		protected override ADRawEntry LoadExecutingUser(IIdentity identity, IList<PropertyDefinition> properties)
		{
			MonadConnection connection = new MonadConnection("timeout=30", new CommandInteractionHandler(), null, CmdletBasedRunspaceConfiguration.connectionInfo);
			CmdletBasedRunspaceConfiguration.reportProgress.ReportProgress(70, 100, Strings.LoadingLogonUser(base.IdentityName), Strings.LoadingLogonUserErrorText(base.IdentityName));
			ADRawEntry result;
			using (new OpenConnection(connection))
			{
				MonadCommand monadCommand = new MonadCommand("Get-LogonUser", connection);
				result = (ADRawEntry)monadCommand.Execute()[0];
			}
			return result;
		}

		protected override Result<ExchangeRoleAssignment>[] LoadRoleAssignments(IConfigurationSession session, ADRawEntry user, List<ADObjectId> existingRoleGroups)
		{
			MonadConnection connection = new MonadConnection("timeout=30", new CommandInteractionHandler(), null, CmdletBasedRunspaceConfiguration.connectionInfo);
			object[] array;
			using (new OpenConnection(connection))
			{
				CmdletBasedRunspaceConfiguration.reportProgress.ReportProgress(75, 100, Strings.LoadingRoleAssignment(base.IdentityName), Strings.LoadingRoleAssignmentErrorText(base.IdentityName));
				MonadCommand monadCommand = new MonadCommand("Get-ManagementRoleAssignmentForLogonUser", connection);
				array = monadCommand.Execute();
			}
			Result<ExchangeRoleAssignment>[] array2 = new Result<ExchangeRoleAssignment>[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = new Result<ExchangeRoleAssignment>((ExchangeRoleAssignment)((ExchangeRoleAssignmentPresentation)array[i]).DataObject, null);
			}
			return array2;
		}

		protected override Result<ExchangeRole>[] LoadRoles(IConfigurationSession session, List<ADObjectId> roleIds)
		{
			MonadConnection connection = new MonadConnection("timeout=30", new CommandInteractionHandler(), null, CmdletBasedRunspaceConfiguration.connectionInfo);
			object[] roles;
			using (new OpenConnection(connection))
			{
				CmdletBasedRunspaceConfiguration.reportProgress.ReportProgress(90, 100, Strings.LoadingRole(base.IdentityName), Strings.LoadingRoleErrorText(base.IdentityName));
				MonadCommand monadCommand = new MonadCommand("Get-ManagementRoleForLogonUser", connection);
				roles = monadCommand.Execute();
			}
			Result<ExchangeRole>[] array = new Result<ExchangeRole>[roleIds.Count];
			for (int i = 0; i < roleIds.Count; i++)
			{
				array[i] = new Result<ExchangeRole>(this.FindRole(roles, roleIds[i]), null);
			}
			return array;
		}

		protected override Result<ManagementScope>[] LoadScopes(IConfigurationSession session, ADObjectId[] scopeIds)
		{
			MonadConnection connection = new MonadConnection("timeout=30", new CommandInteractionHandler(), null, CmdletBasedRunspaceConfiguration.connectionInfo);
			object[] scopes;
			using (new OpenConnection(connection))
			{
				this.scopeReported = true;
				CmdletBasedRunspaceConfiguration.reportProgress.ReportProgress(80, 100, Strings.LoadingScope(base.IdentityName), Strings.LoadingScopeErrorText(base.IdentityName));
				MonadCommand monadCommand = new MonadCommand("Get-ManagementScopeForLogonUser", connection);
				scopes = monadCommand.Execute(scopeIds);
			}
			Result<ManagementScope>[] array = new Result<ManagementScope>[scopeIds.Length];
			for (int i = 0; i < scopeIds.Length; i++)
			{
				array[i] = new Result<ManagementScope>(this.FindScope(scopes, scopeIds[i]), null);
			}
			return array;
		}

		protected override ManagementScope[] LoadExclusiveScopes()
		{
			MonadConnection connection = new MonadConnection("timeout=30", new CommandInteractionHandler(), null, CmdletBasedRunspaceConfiguration.connectionInfo);
			object[] array;
			using (new OpenConnection(connection))
			{
				if (!this.scopeReported)
				{
					CmdletBasedRunspaceConfiguration.reportProgress.ReportProgress(80, 100, Strings.LoadingScope(base.IdentityName), Strings.LoadingScopeErrorText(base.IdentityName));
				}
				MonadCommand monadCommand = new MonadCommand("Get-ExclusiveManagementScopeForLogonUser", connection);
				array = monadCommand.Execute();
			}
			ManagementScope[] array2 = new ManagementScope[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = (array[i] as ManagementScope);
			}
			return array2;
		}

		private ExchangeRole FindRole(object[] roles, ADObjectId roleId)
		{
			return (from ExchangeRole c in roles
			where ADObjectId.Equals(roleId, c.Id)
			select c).First<ExchangeRole>();
		}

		private ManagementScope FindScope(object[] scopes, ADObjectId scopeId)
		{
			return (from ManagementScope c in scopes
			where ADObjectId.Equals(scopeId, c.Id)
			select c).FirstOrDefault<ManagementScope>();
		}

		private void CheckCmdlet(string cmdletName, string[] parameters)
		{
			ScopeSet scopeSet = new ScopeSet(new ADScope(null, null), new ADScopeCollection[0], null, null);
			if (!this.IsCmdletAllowedInScope("Microsoft.Exchange.Management.PowerShell.E2010\\" + cmdletName, parameters, scopeSet))
			{
				throw new CommandNotFoundException(Strings.CommandNotFoundError(cmdletName));
			}
		}

		protected override bool ApplyValidationRules
		{
			get
			{
				return false;
			}
		}

		private const string PSSnapInName = "Microsoft.Exchange.Management.PowerShell.E2010";

		private static MonadConnectionInfo connectionInfo;

		private static IReportProgress reportProgress;

		private bool scopeReported;
	}
}

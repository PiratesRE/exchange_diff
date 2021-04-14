using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "ADPermission", DefaultParameterSetName = "AccessRights")]
	public sealed class GetADPermission : GetPermissionTaskBase<ADRawEntryIdParameter, ADRawEntry>
	{
		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.readOnlySession = PermissionTaskHelper.GetReadOnlySession(base.DomainController);
			TaskLogger.LogExit();
		}

		protected override IConfigDataProvider CreateSession()
		{
			return this.readOnlySession;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			base.HasObjectMatchingIdentity = true;
			ActiveDirectorySecurity activeDirectorySecurity = PermissionTaskHelper.ReadAdSecurityDescriptor((ADRawEntry)dataObject, (IConfigurationSession)base.DataSession, new Task.TaskErrorLoggingDelegate(base.WriteError));
			if (!base.Owner.IsPresent)
			{
				AuthorizationRuleCollection accessRules = activeDirectorySecurity.GetAccessRules(true, true, typeof(SecurityIdentifier));
				int num = 0;
				while (accessRules.Count > num)
				{
					ActiveDirectoryAccessRule activeDirectoryAccessRule = (ActiveDirectoryAccessRule)accessRules[num];
					if (base.SecurityPrincipal == null || (base.SecurityPrincipal != null && base.SecurityPrincipal == activeDirectoryAccessRule.IdentityReference))
					{
						ADAcePresentationObject adacePresentationObject = new ADAcePresentationObject(activeDirectoryAccessRule, ((ADRawEntry)dataObject).Id);
						adacePresentationObject.ResetChangeTracking(true);
						base.WriteResult(adacePresentationObject);
					}
					num++;
				}
			}
			else
			{
				IdentityReference owner = activeDirectorySecurity.GetOwner(typeof(NTAccount));
				base.WriteResult(new OwnerPresentationObject(((ADRawEntry)dataObject).Id, owner.ToString()));
			}
			TaskLogger.LogExit();
		}

		private IConfigurationSession readOnlySession;
	}
}

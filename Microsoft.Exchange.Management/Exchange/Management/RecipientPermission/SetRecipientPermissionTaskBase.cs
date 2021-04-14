using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Permission;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientPermission
{
	public abstract class SetRecipientPermissionTaskBase : SetRecipientObjectTask<RecipientIdParameter, RecipientPermission, ADRecipient>
	{
		protected new SwitchParameter IgnoreDefaultScope
		{
			get
			{
				return base.IgnoreDefaultScope;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public SecurityPrincipalIdParameter Trustee
		{
			get
			{
				return (SecurityPrincipalIdParameter)base.Fields["Trustee"];
			}
			set
			{
				base.Fields["Trustee"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public MultiValuedProperty<RecipientAccessRight> AccessRights
		{
			get
			{
				return (MultiValuedProperty<RecipientAccessRight>)base.Fields["AccessRights"];
			}
			set
			{
				base.Fields["AccessRights"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.trustee = (ADRecipient)SecurityPrincipalIdParameter.GetSecurityPrincipal((IRecipientSession)base.DataSession, this.Trustee, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			List<ActiveDirectoryAccessRule> list = new List<ActiveDirectoryAccessRule>();
			foreach (RecipientAccessRight right in this.AccessRights)
			{
				list.Add(new ActiveDirectoryAccessRule(((IADSecurityPrincipal)this.trustee).Sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, RecipientPermissionHelper.GetRecipientAccessRightGuid(right), this.GetInheritanceType(), Guid.Empty));
			}
			this.ApplyModification(list.ToArray());
			TaskLogger.LogExit();
		}

		protected void WriteResults(ActiveDirectoryAccessRule[] modifiedAces)
		{
			foreach (ActiveDirectoryAccessRule activeDirectoryAccessRule in modifiedAces)
			{
				string friendlyNameOfSecurityIdentifier = RecipientPermissionTaskHelper.GetFriendlyNameOfSecurityIdentifier((SecurityIdentifier)activeDirectoryAccessRule.IdentityReference, base.TenantGlobalCatalogSession, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				RecipientPermission sendToPipeline = new RecipientPermission(activeDirectoryAccessRule, this.DataObject.Id, friendlyNameOfSecurityIdentifier);
				base.WriteObject(sendToPipeline);
			}
		}

		protected abstract void ApplyModification(ActiveDirectoryAccessRule[] modifiedAces);

		protected abstract ActiveDirectorySecurityInheritance GetInheritanceType();

		protected ADRecipient trustee;
	}
}

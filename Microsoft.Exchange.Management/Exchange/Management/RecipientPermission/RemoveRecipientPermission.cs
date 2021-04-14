using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Permission;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientPermission
{
	[Cmdlet("Remove", "RecipientPermission", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class RemoveRecipientPermission : SetRecipientPermissionTaskBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveRecipientPermission(this.Identity.ToString(), base.Trustee.ToString(), base.FormatMultiValuedProperty(base.AccessRights));
			}
		}

		protected override ActiveDirectorySecurityInheritance GetInheritanceType()
		{
			return ActiveDirectorySecurityInheritance.All;
		}

		protected override void ApplyModification(ActiveDirectoryAccessRule[] modifiedAces)
		{
			TaskLogger.LogEnter();
			if (this.trustee != null)
			{
				List<ActiveDirectoryAccessRule> list = new List<ActiveDirectoryAccessRule>();
				foreach (SecurityIdentifier identity in ((IADSecurityPrincipal)this.trustee).SidHistory)
				{
					foreach (RecipientAccessRight right in base.AccessRights)
					{
						list.Add(new ActiveDirectoryAccessRule(identity, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, RecipientPermissionHelper.GetRecipientAccessRightGuid(right), this.GetInheritanceType(), Guid.Empty));
					}
				}
				if (list.Count > 0)
				{
					list.AddRange(modifiedAces);
					modifiedAces = list.ToArray();
				}
			}
			DirectoryCommon.RemoveAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.ErrorLoggerDelegate(base.WriteError), this.DataObject, modifiedAces);
			TaskLogger.LogExit();
		}
	}
}

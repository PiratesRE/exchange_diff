using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "ADPermission", DefaultParameterSetName = "AccessRights", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveADPermission : SetADPermissionTaskBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveADPermissionAccessRights(this.Identity.ToString(), (base.Instance.AccessRights != null) ? base.FormatMultiValuedProperty(base.Instance.AccessRights) : base.FormatMultiValuedProperty(base.Instance.ExtendedRights), base.Instance.User.ToString());
			}
		}

		protected override void ApplyModification(ADRawEntry modifiedObject, ActiveDirectoryAccessRule[] modifiedAces)
		{
			DirectoryCommon.RemoveAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.ErrorLoggerDelegate(this.WriteErrorPerObject), base.GetWritableSession(modifiedObject.Id), modifiedObject.Id, modifiedAces);
		}

		protected override void WriteAces(ADObjectId id, IEnumerable<ActiveDirectoryAccessRule> aces)
		{
		}
	}
}

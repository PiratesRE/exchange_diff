using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Select", "UserForReconciliation")]
	public sealed class SelectUserForReconciliation : Task
	{
		[Parameter(Mandatory = true)]
		[ValidateCount(2, 2)]
		public ADObjectId[] User
		{
			get
			{
				return (ADObjectId[])base.Fields["User"];
			}
			set
			{
				base.Fields["User"] = value;
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			return e is DataSourceOperationException || e is DataSourceTransientException || e is DataValidationException || e is ManagementObjectNotFoundException || e is ManagementObjectAmbiguousException || base.IsKnownException(e);
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.User == null || this.User.Length != 2 || this.User[0] == null || this.User[1] == null || !this.User[0].PartitionGuid.Equals(this.User[1].PartitionGuid))
			{
				base.WriteError(new TaskException(Strings.ErrorSelectUserCmdletOnlyWorksForTwoUsers), ExchangeErrorCategory.Client, null);
			}
			this.recipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAllTenantsObjectId(this.User[0]), 70, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\common\\SelectUserForReconciliation.cs");
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ADObjectId adobjectId = this.User[0];
			ADObjectId adobjectId2 = this.User[1];
			base.InternalProcessRecord();
			if (adobjectId != null && adobjectId2 != null)
			{
				ADObjectId adobjectId3 = this.recipientSession.ChooseBetweenAmbiguousUsers(adobjectId, adobjectId2);
				if (adobjectId3 != null)
				{
					if (adobjectId.Equals(adobjectId3))
					{
						base.WriteObject(adobjectId2);
					}
					else
					{
						base.WriteObject(adobjectId);
					}
				}
			}
			TaskLogger.LogExit();
		}

		private ITenantRecipientSession recipientSession;
	}
}

using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "MailboxDatabase", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveMailboxDatabase : RemoveDatabaseTask<MailboxDatabase>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveMailboxDatabase(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.DataObject.IsExchange2009OrLater)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorModifyE12ServerNotAllowed), ErrorCategory.InvalidOperation, base.DataObject.Server);
			}
			if (!base.DataObject.Recovery)
			{
				base.WriteVerbose(Strings.VerboseNoAssociatedUserMailboxOrMoveRequestOnDatabaseCondition(base.DataObject.Identity.ToString()));
				ADUser aduser;
				if (!new NoAssociatedUserMailboxOrMoveRequestOnDatabaseCondition(base.DataObject).Verify(out aduser))
				{
					base.WriteVerbose(Strings.VerboseMailboxDistinguishedName(aduser.DistinguishedName));
					if (aduser.MailboxMoveTargetMDB != null)
					{
						base.WriteError(new AssociatedMoveRequestExistsException(), ErrorCategory.InvalidOperation, this.Identity);
					}
					else if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
					{
						base.WriteError(new AssociatedUserMailboxExistExceptionInDC(), ErrorCategory.InvalidOperation, this.Identity);
					}
					else
					{
						base.WriteError(new AssociatedUserMailboxExistException(), ErrorCategory.InvalidOperation, this.Identity);
					}
				}
				base.WriteVerbose(Strings.VerboseNoAssociatedMRSRequestOnDatabaseCondition(base.DataObject.Identity.ToString()));
				NoAssociatedMRSRequestOnDatabaseCondition noAssociatedMRSRequestOnDatabaseCondition = new NoAssociatedMRSRequestOnDatabaseCondition(base.DataObject);
				MRSRequest mrsrequest;
				if (!noAssociatedMRSRequestOnDatabaseCondition.Verify(out mrsrequest))
				{
					base.WriteVerbose(Strings.VerboseMRSRequestDistinguishedName(mrsrequest.DistinguishedName));
					base.WriteError(new AssociatedMRSRequestExistsException(noAssociatedMRSRequestOnDatabaseCondition.Type.ToString()), ErrorCategory.InvalidOperation, this.Identity);
				}
			}
			base.CheckDatabaseStatus();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			try
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, MailboxDatabaseSchema.OriginalDatabase, new ADObjectId(null, base.DataObject.Id.ObjectGuid));
				base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(base.DataSession, typeof(MailboxDatabase), filter, null, true));
				MailboxDatabase[] array = this.ConfigurationSession.Find<MailboxDatabase>(null, QueryScope.SubTree, filter, null, 1);
				if (array.Length == 1)
				{
					base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(this.ConfigurationSession));
					array[0].OriginalDatabase = null;
					base.WriteVerbose(Strings.VerboseResetRecoveryDatabase(array[0].Id.ToString()));
					base.DataSession.Save(array[0]);
				}
			}
			catch (DataSourceTransientException ex)
			{
				this.WriteWarning(Strings.FailedToResetRecoveryDatabase(this.Identity.ToString(), ex.Message));
			}
			catch (DataSourceOperationException ex2)
			{
				this.WriteWarning(Strings.FailedToResetRecoveryDatabase(this.Identity.ToString(), ex2.Message));
			}
			catch (DataValidationException ex3)
			{
				this.WriteWarning(Strings.FailedToResetRecoveryDatabase(this.Identity.ToString(), ex3.Message));
			}
			finally
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(this.ConfigurationSession));
			}
			TaskLogger.LogExit();
		}
	}
}

using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class DisableMailUserBase<TIdentity> : RecipientObjectActionTask<TIdentity, ADUser> where TIdentity : MailUserIdParameterBase, new()
	{
		public DisableMailUserBase()
		{
		}

		[Parameter]
		public SwitchParameter IgnoreDefaultScope
		{
			get
			{
				return base.InternalIgnoreDefaultScope;
			}
			set
			{
				base.InternalIgnoreDefaultScope = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Archive")]
		public SwitchParameter Archive
		{
			get
			{
				return (SwitchParameter)(base.Fields["Archive"] ?? false);
			}
			set
			{
				base.Fields["Archive"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override TIdentity Identity
		{
			get
			{
				return (TIdentity)((object)base.Fields["Identity"]);
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IgnoreLegalHold
		{
			get
			{
				return (SwitchParameter)(base.Fields["IgnoreLegalHold"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IgnoreLegalHold"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter PreventRecordingPreviousDatabase
		{
			get
			{
				return (SwitchParameter)(base.Fields["PreventRecordingPreviousDatabase"] ?? false);
			}
			set
			{
				base.Fields["PreventRecordingPreviousDatabase"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Archive" == base.ParameterSetName)
				{
					TIdentity identity = this.Identity;
					return Strings.ConfirmationMessageDisableArchive(identity.ToString());
				}
				TIdentity identity2 = this.Identity;
				return Strings.ConfirmationMessageDisableMailUser(identity2.ToString());
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADUser aduser = (ADUser)base.PrepareDataObject();
			MailboxTaskHelper.BlockRemoveOrDisableIfLitigationHoldEnabled(aduser, new Task.ErrorLoggerDelegate(base.WriteError), true, this.IgnoreLegalHold.ToBool());
			MailboxTaskHelper.BlockRemoveOrDisableIfDiscoveryHoldEnabled(aduser, new Task.ErrorLoggerDelegate(base.WriteError), true, this.IgnoreLegalHold.ToBool());
			if (ComplianceConfigImpl.JournalArchivingHardeningEnabled)
			{
				MailboxTaskHelper.BlockRemoveOrDisableMailUserIfJournalArchiveEnabled(base.DataSession as IRecipientSession, this.ConfigurationSession, aduser, new Task.ErrorLoggerDelegate(base.WriteError), true, false);
			}
			if (!aduser.ExchangeVersion.IsOlderThan(ADUserSchema.ArchiveGuid.VersionAdded))
			{
				if (aduser.ArchiveGuid != Guid.Empty)
				{
					if (!this.PreventRecordingPreviousDatabase)
					{
						aduser.DisabledArchiveGuid = aduser.ArchiveGuid;
						aduser.DisabledArchiveDatabase = aduser.ArchiveDatabase;
					}
					else
					{
						aduser.DisabledArchiveGuid = Guid.Empty;
						aduser.DisabledArchiveDatabase = null;
					}
				}
				aduser.ArchiveGuid = Guid.Empty;
				aduser.ArchiveName = null;
				aduser.ArchiveDatabase = null;
				aduser.ArchiveStatus = (aduser.ArchiveStatus &= ~ArchiveStatusFlags.Active);
				aduser.AllowArchiveAddressSync = false;
			}
			if ("Archive" == base.ParameterSetName)
			{
				MailboxTaskHelper.ClearExchangeProperties(aduser, DisableMailUserBase<MailUserIdParameter>.MailboxMovePropertiesToReset);
				TaskLogger.Trace("DisableMailbox -Archive skipping PrepareDataObject", new object[0]);
				TaskLogger.LogExit();
				return aduser;
			}
			int recipientSoftDeletedStatus = aduser.RecipientSoftDeletedStatus;
			DateTime? whenSoftDeleted = aduser.WhenSoftDeleted;
			Guid disabledArchiveGuid = aduser.DisabledArchiveGuid;
			ADObjectId disabledArchiveDatabase = aduser.DisabledArchiveDatabase;
			MailboxTaskHelper.ClearExchangeProperties(aduser, RecipientConstants.DisableMailUserBase_PropertiesToReset);
			aduser.SetExchangeVersion(null);
			aduser.OverrideCorruptedValuesWithDefault();
			aduser.propertyBag.SetField(ADRecipientSchema.RecipientSoftDeletedStatus, recipientSoftDeletedStatus);
			aduser.propertyBag.SetField(ADRecipientSchema.WhenSoftDeleted, whenSoftDeleted);
			if (disabledArchiveGuid != Guid.Empty)
			{
				aduser.propertyBag.SetField(ADUserSchema.DisabledArchiveGuid, disabledArchiveGuid);
				aduser.propertyBag.SetField(ADUserSchema.DisabledArchiveDatabase, disabledArchiveDatabase);
			}
			TaskLogger.LogExit();
			return aduser;
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return MailUser.FromDataObject((ADUser)dataObject);
		}

		internal static readonly PropertyDefinition[] MailboxMovePropertiesToReset = new PropertyDefinition[]
		{
			ADUserSchema.MailboxMoveTargetMDB,
			ADUserSchema.MailboxMoveSourceMDB,
			ADUserSchema.MailboxMoveTargetArchiveMDB,
			ADUserSchema.MailboxMoveSourceArchiveMDB,
			ADUserSchema.MailboxMoveFlags,
			ADUserSchema.MailboxMoveStatus,
			ADUserSchema.MailboxMoveRemoteHostName,
			ADUserSchema.MailboxMoveBatchName
		};
	}
}

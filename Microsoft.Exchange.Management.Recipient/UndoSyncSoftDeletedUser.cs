using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Undo", "SyncSoftDeletedUser", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class UndoSyncSoftDeletedUser : NewGeneralRecipientObjectTask<ADUser>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRecoveringSoftDeletedObject(this.SoftDeletedObject.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public WindowsLiveId WindowsLiveID { get; set; }

		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
		public new NonMailEnabledUserIdParameter SoftDeletedObject
		{
			get
			{
				return (NonMailEnabledUserIdParameter)base.SoftDeletedObject;
			}
			set
			{
				base.SoftDeletedObject = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SecureString Password
		{
			get
			{
				return (SecureString)base.Fields["Password"];
			}
			set
			{
				base.Fields["Password"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter BypassLiveId
		{
			get
			{
				return (SwitchParameter)(base.Fields["BypassLiveId"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["BypassLiveId"] = value;
			}
		}

		protected override bool EnforceExchangeObjectVersion
		{
			get
			{
				return false;
			}
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			string name = this.DataObject.Name;
			string displayName = this.DataObject.DisplayName;
			this.DataObject = SoftDeletedTaskHelper.GetSoftDeletedADUser(base.CurrentOrganizationId, this.SoftDeletedObject, new Task.ErrorLoggerDelegate(base.WriteError));
			this.previousExchangeVersion = this.DataObject.ExchangeVersion;
			this.DataObject.SetExchangeVersion(ADRecipientSchema.WhenSoftDeleted.VersionAdded);
			if (this.WindowsLiveID != null && this.WindowsLiveID.SmtpAddress != SmtpAddress.Empty && this.WindowsLiveID.SmtpAddress != this.DataObject.WindowsLiveID)
			{
				this.DataObject.EmailAddressPolicyEnabled = false;
				this.DataObject.WindowsLiveID = this.WindowsLiveID.SmtpAddress;
				this.DataObject.UserPrincipalName = this.WindowsLiveID.SmtpAddress.ToString();
				this.DataObject.PrimarySmtpAddress = this.WindowsLiveID.SmtpAddress;
			}
			if (!string.IsNullOrEmpty(name))
			{
				this.DataObject.Name = name;
			}
			this.DataObject.Name = SoftDeletedTaskHelper.GetUniqueNameForRecovery((IRecipientSession)base.DataSession, this.DataObject.Name, this.DataObject.Id);
			if (!string.IsNullOrEmpty(displayName))
			{
				this.DataObject.DisplayName = displayName;
			}
		}

		protected override void PrepareRecipientObject(ADUser user)
		{
			TaskLogger.LogEnter();
			base.PrepareRecipientObject(user);
			if (this.WindowsLiveID != null && this.WindowsLiveID.SmtpAddress != SmtpAddress.Empty)
			{
				MailboxTaskHelper.IsLiveIdExists((IRecipientSession)base.DataSession, user.WindowsLiveID, user.NetID, new Task.ErrorLoggerDelegate(base.WriteError));
				user.UserPrincipalName = user.WindowsLiveID.ToString();
			}
			if (!user.IsModified(ADRecipientSchema.DisplayName))
			{
				user.DisplayName = user.Name;
			}
			SoftDeletedTaskHelper.UpdateShadowWhenSoftDeletedProperty((IRecipientSession)base.DataSession, this.ConfigurationSession, base.CurrentOrganizationId, this.DataObject);
			this.DataObject.RecipientSoftDeletedStatus = 0;
			this.DataObject.WhenSoftDeleted = null;
			this.DataObject.InternalOnly = false;
			this.DataObject.propertyBag.MarkAsChanged(ADRecipientSchema.RecipientSoftDeletedStatus);
			this.DataObject.propertyBag.MarkAsChanged(ADRecipientSchema.WhenSoftDeleted);
			this.DataObject.propertyBag.MarkAsChanged(ADRecipientSchema.TransportSettingFlags);
			TaskLogger.LogExit();
		}

		internal override void PreInternalProcessRecord()
		{
			if (base.IsProvisioningLayerAvailable)
			{
				ProvisioningLayer.PreInternalProcessRecord(this, this.ConvertDataObjectToPresentationObject(this.DataObject), false);
			}
		}

		protected override void InternalProcessRecord()
		{
			this.DataObject.SetExchangeVersion(this.previousExchangeVersion);
			base.InternalProcessRecord();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return SyncUser.FromDataObject((ADUser)dataObject);
		}

		private new OrganizationalUnitIdParameter OrganizationalUnit
		{
			get
			{
				return base.OrganizationalUnit;
			}
		}

		private new string ExternalDirectoryObjectId
		{
			get
			{
				return base.ExternalDirectoryObjectId;
			}
		}

		private ExchangeObjectVersion previousExchangeVersion;
	}
}

using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("get", "User", DefaultParameterSetName = "Identity")]
	public sealed class GetUser : GetADUserBase<UserIdParameter>
	{
		protected override RecipientTypeDetails[] InternalRecipientTypeDetails
		{
			get
			{
				if (this.Arbitration)
				{
					return GetUser.ArbitrationAllowedRecipientTypeDetails;
				}
				if (this.PublicFolder)
				{
					return GetUser.PublicFolderAllowedRecipientTypeDetails;
				}
				if (this.AuditLog)
				{
					return GetUser.AuditLogAllowedRecipientTypeDetails;
				}
				return this.RecipientTypeDetails ?? GetUser.AllowedRecipientTypeDetails;
			}
		}

		[Parameter]
		public SwitchParameter Arbitration
		{
			get
			{
				return (SwitchParameter)(base.Fields["Arbitration"] ?? false);
			}
			set
			{
				base.Fields["Arbitration"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter PublicFolder
		{
			get
			{
				return (SwitchParameter)(base.Fields["PublicFolder"] ?? false);
			}
			set
			{
				base.Fields["PublicFolder"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AuditLog
		{
			get
			{
				return (SwitchParameter)(base.Fields["AuditLog"] ?? false);
			}
			set
			{
				base.Fields["AuditLog"] = value;
			}
		}

		[Parameter]
		public NetID ConsumerNetID
		{
			get
			{
				return (NetID)base.Fields[ADUserSchema.ConsumerNetID];
			}
			set
			{
				base.Fields[ADUserSchema.ConsumerNetID] = value;
			}
		}

		[Parameter]
		[ValidateNotNullOrEmpty]
		public RecipientTypeDetails[] RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails[])base.Fields["RecipientTypeDetails"];
			}
			set
			{
				base.VerifyValues<RecipientTypeDetails>(GetUser.PublicFolderAllowedRecipientTypeDetails.Union(GetUser.AllowedRecipientTypeDetails).ToArray<RecipientTypeDetails>(), value);
				base.Fields["RecipientTypeDetails"] = value;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			ADUser aduser = (ADUser)dataObject;
			if (null != aduser.MasterAccountSid)
			{
				aduser.LinkedMasterAccount = SecurityPrincipalIdParameter.GetFriendlyUserName(aduser.MasterAccountSid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				aduser.ResetChangeTracking();
			}
			return new User(aduser);
		}

		internal static readonly RecipientTypeDetails[] ArbitrationAllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.ArbitrationMailbox
		};

		internal static readonly RecipientTypeDetails[] PublicFolderAllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.PublicFolderMailbox
		};

		internal static readonly RecipientTypeDetails[] MonitoringAllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.MonitoringMailbox
		};

		internal static readonly RecipientTypeDetails[] AuditLogAllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.AuditLogMailbox
		};

		private static readonly RecipientTypeDetails[] AllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.RoomMailbox,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.LinkedRoomMailbox,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.DisabledUser,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.EquipmentMailbox,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.LegacyMailbox,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.LinkedMailbox,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.UserMailbox,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.MailUser,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.SharedMailbox,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.TeamMailbox,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.GroupMailbox,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.DiscoveryMailbox,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.User,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.LinkedUser,
			(RecipientTypeDetails)((ulong)int.MinValue),
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.RemoteRoomMailbox,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.RemoteEquipmentMailbox,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.RemoteSharedMailbox,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.RemoteTeamMailbox
		};
	}
}

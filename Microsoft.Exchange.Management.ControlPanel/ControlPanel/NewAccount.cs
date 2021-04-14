using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewAccount : OrgPersonBasicProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				if (this.associatedCmdlet != null)
				{
					return this.associatedCmdlet;
				}
				return "New-Mailbox";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}

		public SetCalendarProcessing SetCalendarProcessing { get; private set; }

		[DataMember]
		public bool ResetPasswordOnNextLogon
		{
			get
			{
				return (bool)(base[UserSchema.ResetPasswordOnNextLogon] ?? false);
			}
			set
			{
				base[UserSchema.ResetPasswordOnNextLogon] = value;
			}
		}

		[DataMember]
		public string UserName { get; set; }

		[DataMember]
		public string DomainName { get; set; }

		[DataMember]
		public string Password { private get; set; }

		public string UserPrincipalName
		{
			get
			{
				return (string)base[MailboxSchema.UserPrincipalName];
			}
			set
			{
				base[MailboxSchema.UserPrincipalName] = value;
			}
		}

		public string WindowsLiveID
		{
			get
			{
				return (string)base[MailboxSchema.WindowsLiveID];
			}
			set
			{
				base[MailboxSchema.WindowsLiveID] = value;
			}
		}

		public string MicrosoftOnlineServicesID
		{
			get
			{
				return (string)base["MicrosoftOnlineServicesID"];
			}
			set
			{
				base["MicrosoftOnlineServicesID"] = value;
			}
		}

		public string PrimarySmtpAddress
		{
			get
			{
				return (string)base[MailEnabledRecipientSchema.PrimarySmtpAddress];
			}
			set
			{
				base[MailEnabledRecipientSchema.PrimarySmtpAddress] = value;
			}
		}

		public string Name
		{
			get
			{
				return (string)base[ADObjectSchema.Name];
			}
			set
			{
				base[ADObjectSchema.Name] = value;
			}
		}

		public bool? UseExistingLiveId
		{
			get
			{
				return (bool?)base["UseExistingLiveId"];
			}
			set
			{
				base["UseExistingLiveId"] = value;
			}
		}

		public ADObjectId Organization
		{
			get
			{
				return (ADObjectId)base["Organization"];
			}
			set
			{
				base["Organization"] = value;
			}
		}

		[DataMember]
		public string MailboxPlan
		{
			get
			{
				return (string)base[MailboxSchema.MailboxPlan];
			}
			set
			{
				base[MailboxSchema.MailboxPlan] = value;
			}
		}

		[DataMember]
		public string RoleAssignmentPolicy
		{
			get
			{
				return (string)base[MailboxSchema.RoleAssignmentPolicy];
			}
			set
			{
				base[MailboxSchema.RoleAssignmentPolicy] = value;
			}
		}

		[DataMember]
		public bool? Room
		{
			get
			{
				return (bool?)base["Room"];
			}
			set
			{
				base["Room"] = value;
			}
		}

		[DataMember]
		public string Office
		{
			get
			{
				return (string)base[MailboxSchema.Office];
			}
			set
			{
				base[MailboxSchema.Office] = value;
			}
		}

		[DataMember]
		public string Phone
		{
			get
			{
				return (string)base[ADUserSchema.Phone];
			}
			set
			{
				base[ADUserSchema.Phone] = value;
			}
		}

		[DataMember]
		public string ResourceCapacity
		{
			get
			{
				return (string)base[MailboxSchema.ResourceCapacity];
			}
			set
			{
				base[MailboxSchema.ResourceCapacity] = value;
			}
		}

		[DataMember]
		public string AutomaticBooking
		{
			get
			{
				return this.SetCalendarProcessing.AutomaticBooking;
			}
			set
			{
				this.SetCalendarProcessing.AutomaticBooking = value;
			}
		}

		[DataMember]
		public Identity[] ResourceDelegates
		{
			get
			{
				return this.SetCalendarProcessing.ResourceDelegates;
			}
			set
			{
				this.SetCalendarProcessing.ResourceDelegates = value;
			}
		}

		[DataMember]
		public bool? ImportLiveId
		{
			get
			{
				return (bool?)base["ImportLiveId"];
			}
			set
			{
				base["ImportLiveId"] = value;
			}
		}

		[DataMember]
		public bool EvictLiveId
		{
			get
			{
				return (bool)base["EvictLiveId"];
			}
			set
			{
				base["EvictLiveId"] = value;
			}
		}

		[DataMember]
		public string RemovedMailbox { get; set; }

		[DataMember]
		public string OriginalLiveID { get; set; }

		[DataMember]
		public bool IsPasswordRequired { get; set; }

		[DataMember]
		public string SoftDeletedMailbox { get; set; }

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.SetCalendarProcessing = new SetCalendarProcessing();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (string.IsNullOrEmpty(this.SoftDeletedMailbox))
			{
				base.DisplayName.FaultIfNullOrEmpty(OwaOptionStrings.DisplayNameNotSetError);
			}
			this.Name = ((!string.IsNullOrEmpty(base.DisplayName) && base.DisplayName.Length >= 64) ? base.DisplayName.SurrogateSubstring(0, 64) : base.DisplayName);
			this.UserName.FaultIfNullOrEmpty(OwaOptionStrings.UserNameNotSetError);
			this.DomainName.FaultIfNullOrEmpty(OwaOptionStrings.DomainNameNotSetError);
			string text = this.UserName + "@" + this.DomainName;
			if (!string.IsNullOrEmpty(this.SoftDeletedMailbox))
			{
				this.associatedCmdlet = "Undo-SoftDeletedMailbox";
				base["SoftDeletedObject"] = this.SoftDeletedMailbox;
				if (RbacPrincipal.Current.IsInRole("LiveID"))
				{
					this.WindowsLiveID = text;
				}
				if (!string.IsNullOrEmpty(this.Password))
				{
					base["Password"] = this.Password.ToSecureString();
					return;
				}
			}
			else
			{
				if (this.Room == true)
				{
					this.PrimarySmtpAddress = text;
				}
				else if (RbacPrincipal.Current.IsInRole("MultiTenant+New-Mailbox?DisplayName&Password&Name&MicrosoftOnlineServicesID@W:Organization"))
				{
					this.MicrosoftOnlineServicesID = text;
				}
				else if (RbacPrincipal.Current.IsInRole("LiveID"))
				{
					this.WindowsLiveID = text;
				}
				else
				{
					this.UserPrincipalName = text;
				}
				if (!this.IsNameUnique())
				{
					string text2 = string.Format(" {0}", Guid.NewGuid().ToString("B").ToUpperInvariant());
					int num = 64 - text2.Length;
					if (this.Name.Length > num)
					{
						this.Name = this.Name.SurrogateSubstring(0, num);
					}
					this.Name += text2;
				}
				if (!string.IsNullOrEmpty(this.RemovedMailbox))
				{
					base["RemovedMailbox"] = this.RemovedMailbox;
					if (!this.IsPasswordRequired && !string.IsNullOrEmpty(this.WindowsLiveID) && this.WindowsLiveID == this.OriginalLiveID)
					{
						this.UseExistingLiveId = new bool?(true);
					}
				}
				if (this.Room != true && this.ImportLiveId != true && this.UseExistingLiveId != true)
				{
					base["Password"] = this.Password.ToSecureString();
				}
			}
		}

		private bool IsNameUnique()
		{
			IRecipientSession recipientSession = (IRecipientSession)((RecipientObjectResolver)RecipientObjectResolver.Instance).CreateAdSession();
			recipientSession.UseConfigNC = false;
			recipientSession.UseGlobalCatalog = true;
			recipientSession.EnforceDefaultScope = false;
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, this.Name);
			ADRecipient[] array = recipientSession.Find(null, QueryScope.SubTree, filter, null, 1);
			return array.Length <= 0;
		}

		public const string RbacParameters_NonLiveID = "?DisplayName&Password&Name&UserPrincipalName";

		public const string RbacParameters_WLID = "?DisplayName&Password&Name&WindowsLiveID";

		public const string RbacParameters_MOSID = "?DisplayName&Password&Name&MicrosoftOnlineServicesID";

		public const string RbacParameters_MultiTenant_Room = "?DisplayName&Name&Room&PrimarySmtpAddress";

		private const string RbacParameters = "?DisplayName&Password&Name";

		private string associatedCmdlet;
	}
}

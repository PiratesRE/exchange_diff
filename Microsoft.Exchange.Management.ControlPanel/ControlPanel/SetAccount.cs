using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetAccount : SetOrgPerson
	{
		public SetAccount()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		public bool? EnableUM { get; private set; }

		public SetMailbox SetMailbox { get; private set; }

		public SetCasMailbox SetCasMailbox { get; private set; }

		public SetCalendarProcessing SetCalendarProcessing { get; private set; }

		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-User";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self|Organization";
			}
		}

		[DataMember]
		public IEnumerable<string> EmailAddresses
		{
			get
			{
				return this.SetMailbox.EmailAddresses;
			}
			set
			{
				this.SetMailbox.EmailAddresses = value;
			}
		}

		[DataMember]
		public string MailTip
		{
			get
			{
				return this.SetMailbox.MailTip;
			}
			set
			{
				this.SetMailbox.MailTip = value;
			}
		}

		[DataMember]
		public string MailboxPlan
		{
			get
			{
				return this.SetMailbox.MailboxPlan;
			}
			set
			{
				this.SetMailbox.MailboxPlan = value;
			}
		}

		[DataMember]
		public string RoleAssignmentPolicy
		{
			get
			{
				return this.SetMailbox.RoleAssignmentPolicy;
			}
			set
			{
				this.SetMailbox.RoleAssignmentPolicy = value;
			}
		}

		[DataMember]
		public string RetentionPolicy
		{
			get
			{
				return this.SetMailbox.RetentionPolicy;
			}
			set
			{
				this.SetMailbox.RetentionPolicy = value;
			}
		}

		[DataMember]
		public string ResourceCapacity
		{
			get
			{
				return this.SetMailbox.ResourceCapacity;
			}
			set
			{
				this.SetMailbox.ResourceCapacity = value;
			}
		}

		public bool? EnableLitigationHold { get; private set; }

		[DataMember]
		public bool? LitigationHoldEnabled
		{
			get
			{
				return this.SetMailbox.LitigationHoldEnabled;
			}
			set
			{
				this.SetMailbox.LitigationHoldEnabled = value;
			}
		}

		[DataMember]
		public string RetentionComment
		{
			get
			{
				return this.SetMailbox.RetentionComment;
			}
			set
			{
				this.SetMailbox.RetentionComment = value;
			}
		}

		[DataMember]
		public string RetentionUrl
		{
			get
			{
				return this.SetMailbox.RetentionUrl;
			}
			set
			{
				this.SetMailbox.RetentionUrl = value;
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
		public IEnumerable<string> AllowedSenders { [PrincipalPermission(SecurityAction.Demand, Role = "Get-SupervisionListEntry?Identity@R:Organization")] get; [PrincipalPermission(SecurityAction.Demand, Role = "Get-User?Identity@R:Organization+Get-Mailbox?Identity@R:Organization+Add-SupervisionListEntry?Identity@W:Self+Remove-SupervisionListEntry?Identity@W:Self")] [PrincipalPermission(SecurityAction.Demand, Role = "Get-User?Identity@R:Organization+Get-Mailbox?Identity@R:Organization+Add-SupervisionListEntry?Identity@W:Organization+Remove-SupervisionListEntry?Identity@W:Organization")] set; }

		[DataMember]
		public IEnumerable<string> BlockedSenders { [PrincipalPermission(SecurityAction.Demand, Role = "Get-SupervisionListEntry?Identity@R:Organization")] get; [PrincipalPermission(SecurityAction.Demand, Role = "Get-User?Identity@R:Organization+Get-Mailbox?Identity@R:Organization+Add-SupervisionListEntry?Identity@W:Self+Remove-SupervisionListEntry?Identity@W:Self")] [PrincipalPermission(SecurityAction.Demand, Role = "Get-User?Identity@R:Organization+Get-Mailbox?Identity@R:Organization+Add-SupervisionListEntry?Identity@W:Organization+Remove-SupervisionListEntry?Identity@W:Organization")] set; }

		[DataMember]
		private IEnumerable<MailboxFeatureInfo> PhoneAndVoiceFeatures
		{
			get
			{
				return null;
			}
			set
			{
				foreach (MailboxFeatureInfo mailboxFeatureInfo in value)
				{
					UMMailboxFeatureInfo ummailboxFeatureInfo = mailboxFeatureInfo as UMMailboxFeatureInfo;
					if (ummailboxFeatureInfo != null)
					{
						this.EnableUM = MailboxFeatureInfo.IsEnabled(ummailboxFeatureInfo.Status);
					}
					else if (mailboxFeatureInfo is EASMailboxFeatureInfo)
					{
						EASMailboxFeatureInfo easmailboxFeatureInfo = mailboxFeatureInfo as EASMailboxFeatureInfo;
						this.SetCasMailbox.ActiveSyncEnabled = new bool?(easmailboxFeatureInfo.Status == ClientStrings.EnabledDisplayText || easmailboxFeatureInfo.Status == ClientStrings.EnabledPendingDisplayText);
					}
				}
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.SetMailbox = new SetMailbox();
			this.SetCasMailbox = new SetCasMailbox();
			this.SetCalendarProcessing = new SetCalendarProcessing();
		}
	}
}

using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract(Name = "SetDistributionGroupBase{0}{1}")]
	public abstract class SetDistributionGroupBase<T, U> : SetObjectProperties where T : SetGroupBase, new() where U : UpdateDistributionGroupMemberBase, new()
	{
		public SetDistributionGroupBase()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.SetGroup = Activator.CreateInstance<T>();
			this.UpdateDistributionGroupMember = Activator.CreateInstance<U>();
		}

		public T SetGroup { get; private set; }

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (RbacPrincipal.Current.IsInRole("MultiTenant") && (this.PrimaryEAAlias != null || this.DomainName != null))
			{
				SmtpAddress smtpAddress = new SmtpAddress(this.HiddenPrimarySmtpAddress);
				string str = this.PrimaryEAAlias ?? smtpAddress.Local;
				string str2 = this.DomainName ?? smtpAddress.Domain;
				this.PrimarySmtpAddress = str + "@" + str2;
			}
		}

		public U UpdateDistributionGroupMember { get; private set; }

		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-DistributionGroup";
			}
		}

		[DataMember]
		public string Notes
		{
			get
			{
				T setGroup = this.SetGroup;
				return setGroup.Notes;
			}
			set
			{
				T setGroup = this.SetGroup;
				setGroup.Notes = value;
			}
		}

		[DataMember]
		public Identity[] Members
		{
			get
			{
				U updateDistributionGroupMember = this.UpdateDistributionGroupMember;
				return updateDistributionGroupMember.Members;
			}
			set
			{
				U updateDistributionGroupMember = this.UpdateDistributionGroupMember;
				updateDistributionGroupMember.Members = value;
			}
		}

		[DataMember]
		public string DisplayName
		{
			get
			{
				return (string)base[MailEnabledRecipientSchema.DisplayName];
			}
			set
			{
				base[MailEnabledRecipientSchema.DisplayName] = value;
			}
		}

		[DataMember]
		public Identity[] AcceptMessagesOnlyFromSendersOrMembers
		{
			get
			{
				return Identity.FromIdParameters(base[MailEnabledRecipientSchema.AcceptMessagesOnlyFromSendersOrMembers]);
			}
			set
			{
				base[MailEnabledRecipientSchema.AcceptMessagesOnlyFromSendersOrMembers] = value.ToIdParameters();
			}
		}

		[DataMember]
		public Identity[] ManagedBy
		{
			get
			{
				return Identity.FromIdParameters(base[DistributionGroupSchema.ManagedBy]);
			}
			set
			{
				base[DistributionGroupSchema.ManagedBy] = value.ToIdParameters();
			}
		}

		[DataMember]
		public CollectionDelta SendAsPermissionsEnterprise { get; set; }

		[DataMember]
		public CollectionDelta SendAsPermissionsCloud { get; set; }

		[DataMember]
		public Identity[] GrantSendOnBehalfTo
		{
			get
			{
				return Identity.FromIdParameters(base[MailEnabledRecipientSchema.GrantSendOnBehalfTo]);
			}
			set
			{
				base[MailEnabledRecipientSchema.GrantSendOnBehalfTo] = value.ToIdParameters();
			}
		}

		[DataMember]
		public bool ModerationEnabled
		{
			get
			{
				return (bool)(base[MailEnabledRecipientSchema.ModerationEnabled] ?? false);
			}
			set
			{
				base[MailEnabledRecipientSchema.ModerationEnabled] = value;
			}
		}

		[DataMember]
		public Identity[] ModeratedBy
		{
			get
			{
				return Identity.FromIdParameters(base[MailEnabledRecipientSchema.ModeratedBy]);
			}
			set
			{
				base[MailEnabledRecipientSchema.ModeratedBy] = value.ToIdParameters();
			}
		}

		[DataMember]
		public Identity[] BypassModerationFromSendersOrMembers
		{
			get
			{
				return Identity.FromIdParameters(base[MailEnabledRecipientSchema.BypassModerationFromSendersOrMembers]);
			}
			set
			{
				base[MailEnabledRecipientSchema.BypassModerationFromSendersOrMembers] = value.ToIdParameters();
			}
		}

		[DataMember]
		public string SendModerationNotifications
		{
			get
			{
				return (string)base[MailEnabledRecipientSchema.SendModerationNotifications];
			}
			set
			{
				base[MailEnabledRecipientSchema.SendModerationNotifications] = value;
			}
		}

		[DataMember]
		public string Alias
		{
			get
			{
				return (string)base[MailEnabledRecipientSchema.Alias];
			}
			set
			{
				base[MailEnabledRecipientSchema.Alias] = value;
			}
		}

		[DataMember]
		public bool IsSecurityGroupMemberJoinApprovalRequired
		{
			get
			{
				return this.MemberJoinRestriction == "ApprovalRequired";
			}
			set
			{
				this.MemberJoinRestriction = (value ? "ApprovalRequired" : "Closed");
			}
		}

		[DataMember]
		public string MemberJoinRestriction
		{
			get
			{
				return (string)base[DistributionGroupSchema.MemberJoinRestriction];
			}
			set
			{
				base[DistributionGroupSchema.MemberJoinRestriction] = value;
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

		[DataMember]
		public string PrimaryEAAlias { get; set; }

		[DataMember]
		public string DomainName { get; set; }

		[DataMember]
		public string HiddenPrimarySmtpAddress { get; set; }

		[DataMember]
		public string MemberDepartRestriction
		{
			get
			{
				return (string)base[DistributionGroupSchema.MemberDepartRestriction];
			}
			set
			{
				base[DistributionGroupSchema.MemberDepartRestriction] = value;
			}
		}

		[DataMember]
		public bool RequireSenderAuthenticationEnabled
		{
			get
			{
				return (bool)(base["RequireSenderAuthenticationEnabled"] ?? false);
			}
			set
			{
				base["RequireSenderAuthenticationEnabled"] = value;
			}
		}

		[DataMember]
		public string MailTip
		{
			get
			{
				return (string)base["MailTip"];
			}
			set
			{
				base["MailTip"] = value;
			}
		}

		[DataMember]
		public bool HiddenFromAddressListsEnabled
		{
			get
			{
				return base[MailEnabledRecipientSchema.HiddenFromAddressListsEnabled] != null && (bool)base[MailEnabledRecipientSchema.HiddenFromAddressListsEnabled];
			}
			set
			{
				base[MailEnabledRecipientSchema.HiddenFromAddressListsEnabled] = value;
			}
		}
	}
}

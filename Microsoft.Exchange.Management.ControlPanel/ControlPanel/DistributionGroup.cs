using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[KnownType(typeof(DistributionGroup))]
	[KnownType(typeof(TrusteeRow))]
	public class DistributionGroup : DistributionGroupRow
	{
		public DistributionGroup(DistributionGroup distributionGroup) : base(distributionGroup)
		{
			this.OriginalDistributionGroup = distributionGroup;
		}

		public DistributionGroup OriginalDistributionGroup { get; private set; }

		public WindowsGroup WindowsGroup { get; set; }

		[DataMember]
		public string Caption
		{
			get
			{
				return this.OriginalDistributionGroup.DisplayName;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Name
		{
			get
			{
				return this.OriginalDistributionGroup.Name;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Alias
		{
			get
			{
				return this.OriginalDistributionGroup.Alias;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsSecurityGroupType
		{
			get
			{
				return (this.OriginalDistributionGroup.GroupType & GroupTypeFlags.SecurityEnabled) == GroupTypeFlags.SecurityEnabled;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string PrimaryEAAlias
		{
			get
			{
				return this.OriginalDistributionGroup.PrimarySmtpAddress.Local;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DomainName
		{
			get
			{
				return this.OriginalDistributionGroup.PrimarySmtpAddress.Domain;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string HiddenPrimarySmtpAddress
		{
			get
			{
				return base.PrimarySmtpAddress;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsSecurityGroupMemberJoinApprovalRequired
		{
			get
			{
				return this.MemberJoinRestriction == "ApprovalRequired";
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string MemberJoinRestriction
		{
			get
			{
				return this.OriginalDistributionGroup.MemberJoinRestriction.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string MemberDepartRestriction
		{
			get
			{
				return this.OriginalDistributionGroup.MemberDepartRestriction.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool HiddenFromAddressListsEnabled
		{
			get
			{
				return this.OriginalDistributionGroup.HiddenFromAddressListsEnabled;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string RequireSenderAuthenticationEnabled
		{
			get
			{
				return this.OriginalDistributionGroup.RequireSenderAuthenticationEnabled.ToString().ToLowerInvariant();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string MailTip
		{
			get
			{
				if (this.OriginalDistributionGroup.MailTip != null)
				{
					return this.OriginalDistributionGroup.MailTip;
				}
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<RecipientObjectResolverRow> ManagedBy
		{
			get
			{
				return this.OriginalDistributionGroup.ManagedBy.ResolveRecipients();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<RecipientObjectResolverRow> AcceptMessagesOnlyFromSendersOrMembers
		{
			get
			{
				return this.OriginalDistributionGroup.AcceptMessagesOnlyFromSendersOrMembers.ResolveRecipients();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool ModerationEnabled
		{
			get
			{
				return this.OriginalDistributionGroup.ModerationEnabled;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<RecipientObjectResolverRow> ModeratedBy
		{
			get
			{
				return this.OriginalDistributionGroup.ModeratedBy.ResolveRecipients();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<RecipientObjectResolverRow> BypassModerationFromSendersOrMembers
		{
			get
			{
				return this.OriginalDistributionGroup.BypassModerationFromSendersOrMembers.ResolveRecipients();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SendModerationNotifications
		{
			get
			{
				return this.OriginalDistributionGroup.SendModerationNotifications.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<Identity> EmailAddresses
		{
			get
			{
				return from address in this.OriginalDistributionGroup.EmailAddresses
				where address is SmtpProxyAddress
				select new Identity(address.AddressString, address.AddressString);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string OrganizationalUnit
		{
			get
			{
				return this.OriginalDistributionGroup.OrganizationalUnit;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<RecipientObjectResolverRow> GrantSendOnBehalfTo
		{
			get
			{
				return this.OriginalDistributionGroup.GrantSendOnBehalfTo.ResolveRecipients();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Notes
		{
			get
			{
				if (this.WindowsGroup == null)
				{
					return null;
				}
				return this.WindowsGroup.Notes;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<RecipientObjectResolverRow> Members
		{
			get
			{
				if (this.WindowsGroup == null)
				{
					return null;
				}
				return this.WindowsGroup.Members.ResolveRecipients();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<AcePermissionRecipientRow> SendAsPermissionsEnterprise { get; set; }

		[DataMember]
		public List<object> SendAsPermissionsCloud { get; set; }
	}
}

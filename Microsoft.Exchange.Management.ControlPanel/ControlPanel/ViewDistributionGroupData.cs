using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ViewDistributionGroupData : DistributionGroup
	{
		public ViewDistributionGroupData(DistributionGroup distributionGroup) : base(distributionGroup)
		{
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			if (base.WindowsGroup != null)
			{
				this.currentUserIsMemberOfGroup = new bool?(base.WindowsGroup.Members.Contains(RbacPrincipal.Current.ExecutingUserId));
			}
		}

		[DataMember]
		public int TotalMembers
		{
			get
			{
				if (base.WindowsGroup == null)
				{
					return 0;
				}
				return base.WindowsGroup.Members.Count;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string MemberJoinRestrictionDetails
		{
			get
			{
				switch (base.OriginalDistributionGroup.MemberJoinRestriction)
				{
				case MemberUpdateType.Closed:
					return OwaOptionStrings.JoinRestrictionClosedDetails;
				case MemberUpdateType.Open:
					return OwaOptionStrings.JoinRestrictionOpenDetails;
				case MemberUpdateType.ApprovalRequired:
					return OwaOptionStrings.JoinRestrictionApprovalRequiredDetails;
				default:
					throw new NotSupportedException();
				}
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ActionShown
		{
			get
			{
				if (this.currentUserIsMemberOfGroup != null)
				{
					return this.currentUserIsMemberOfGroup.Value ? OwaOptionStrings.Depart : OwaOptionStrings.Join;
				}
				return OwaOptionStrings.OkButtonText;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool? CurrentUserIsMemberOfGroup
		{
			get
			{
				if (this.currentUserIsMemberOfGroup != null)
				{
					return new bool?(this.currentUserIsMemberOfGroup.Value);
				}
				return null;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string CommitConfirmMessage
		{
			get
			{
				if (!this.currentUserIsMemberOfGroup.GetValueOrDefault())
				{
					return null;
				}
				return OwaOptionStrings.DepartGroupConfirmation.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string CommitConfirmMessageTargetName
		{
			get
			{
				if (!this.currentUserIsMemberOfGroup.GetValueOrDefault())
				{
					return null;
				}
				return base.DisplayName;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private bool? currentUserIsMemberOfGroup;
	}
}

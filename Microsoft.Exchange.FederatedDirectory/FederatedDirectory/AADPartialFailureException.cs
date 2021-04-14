using System;
using System.Text;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.FederatedDirectory
{
	internal sealed class AADPartialFailureException : LocalizedException
	{
		public AADPartialFailureException(string group) : base(Strings.PartiallyFailedToUpdateGroup(group))
		{
		}

		public AADPartialFailureException.FailedLink[] FailedAddedMembers { get; set; }

		public AADPartialFailureException.FailedLink[] FailedRemovedMembers { get; set; }

		public AADPartialFailureException.FailedLink[] FailedAddedOwners { get; set; }

		public AADPartialFailureException.FailedLink[] FailedRemovedOwners { get; set; }

		public AADPartialFailureException.FailedLink[] FailedAddedPendingMembers { get; set; }

		public AADPartialFailureException.FailedLink[] FailedRemovedPendingMembers { get; set; }

		public override string Message
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(base.Message);
				if (this.FailedAddedMembers != null)
				{
					foreach (AADPartialFailureException.FailedLink failedLink in this.FailedAddedMembers)
					{
						stringBuilder.AppendLine(Strings.FailedToAddMember(failedLink.Link, failedLink.Exception.ToString()));
					}
				}
				if (this.FailedRemovedMembers != null)
				{
					foreach (AADPartialFailureException.FailedLink failedLink2 in this.FailedRemovedMembers)
					{
						stringBuilder.AppendLine(Strings.FailedToRemoveMember(failedLink2.Link, failedLink2.Exception.ToString()));
					}
				}
				if (this.FailedAddedOwners != null)
				{
					foreach (AADPartialFailureException.FailedLink failedLink3 in this.FailedAddedOwners)
					{
						stringBuilder.AppendLine(Strings.FailedToAddOwner(failedLink3.Link, failedLink3.Exception.ToString()));
					}
				}
				if (this.FailedRemovedOwners != null)
				{
					foreach (AADPartialFailureException.FailedLink failedLink4 in this.FailedRemovedOwners)
					{
						stringBuilder.AppendLine(Strings.FailedToRemoveOwner(failedLink4.Link, failedLink4.Exception.ToString()));
					}
				}
				if (this.FailedAddedPendingMembers != null)
				{
					foreach (AADPartialFailureException.FailedLink failedLink5 in this.FailedAddedPendingMembers)
					{
						stringBuilder.AppendLine(Strings.FailedToAddPendingMember(failedLink5.Link, failedLink5.Exception.ToString()));
					}
				}
				if (this.FailedRemovedPendingMembers != null)
				{
					foreach (AADPartialFailureException.FailedLink failedLink6 in this.FailedRemovedPendingMembers)
					{
						stringBuilder.AppendLine(Strings.FailedToRemovePendingMember(failedLink6.Link, failedLink6.Exception.ToString()));
					}
				}
				return stringBuilder.ToString();
			}
		}

		public struct FailedLink
		{
			public string Link;

			public Exception Exception;
		}
	}
}

using System;
using Microsoft.SharePoint.Client.Sharing;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class UserSharingResultWrapper : IUserSharingResult
	{
		public string InvitationLink
		{
			get
			{
				return this.backingResult.InvitationLink;
			}
		}

		public bool Status
		{
			get
			{
				return this.backingResult.Status;
			}
		}

		public string User
		{
			get
			{
				return this.backingResult.User;
			}
		}

		public UserSharingResultWrapper(UserSharingResult result)
		{
			this.backingResult = result;
		}

		private readonly UserSharingResult backingResult;
	}
}

using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public interface IUserSharingResult
	{
		string InvitationLink { get; }

		bool Status { get; }

		string User { get; }
	}
}

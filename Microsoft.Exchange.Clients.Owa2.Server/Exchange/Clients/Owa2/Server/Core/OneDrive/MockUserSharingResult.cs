using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class MockUserSharingResult : IUserSharingResult
	{
		public string InvitationLink { get; set; }

		public bool Status { get; set; }

		public string User { get; set; }
	}
}

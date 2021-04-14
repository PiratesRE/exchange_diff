using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UserNotificationEventContext
	{
		public RedirectionTarget.ResultSet Backend { get; set; }

		public string User { get; set; }
	}
}

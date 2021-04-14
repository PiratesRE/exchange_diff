using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal sealed class ADNotificationRequestCookie
	{
		internal ADNotificationRequest[] Requests
		{
			get
			{
				return this.requests;
			}
		}

		internal ADNotificationRequestCookie(params ADNotificationRequest[] requests)
		{
			this.requests = requests;
		}

		private ADNotificationRequest[] requests;
	}
}

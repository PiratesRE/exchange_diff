using System;

namespace Microsoft.Exchange.PopImap.Core
{
	public class SessionsInfo
	{
		internal SessionsInfo()
		{
		}

		public int Count { get; set; }

		public bool Stopping { get; set; }

		public SessionInfo[] Sessions { get; set; }

		public UserInfo[] Users { get; set; }
	}
}

using System;

namespace Microsoft.Exchange.PopImap.Core
{
	public class UserInfo
	{
		internal UserInfo()
		{
		}

		public string Name { get; set; }

		public int SessionCount { get; set; }
	}
}

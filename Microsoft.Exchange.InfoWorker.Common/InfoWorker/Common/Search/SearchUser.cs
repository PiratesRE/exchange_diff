using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal class SearchUser
	{
		public SearchUser(ADObjectId id, string displayName, string serverName)
		{
			this.id = id;
			this.serverName = serverName;
			this.displayName = displayName;
		}

		public ADObjectId Id
		{
			get
			{
				return this.id;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		private ADObjectId id;

		private string serverName;

		private string displayName;
	}
}

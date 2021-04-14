using System;

namespace Microsoft.Exchange.Clients.Common
{
	public class LiveHeaderMenu : ILiveHeaderElement
	{
		public LiveHeaderLinkCollection List
		{
			get
			{
				if (this.list == null)
				{
					this.list = new LiveHeaderLinkCollection();
				}
				return this.list;
			}
			set
			{
				this.list = value;
			}
		}

		public LiveHeaderLink Link
		{
			get
			{
				if (this.link == null)
				{
					this.link = new LiveHeaderLink();
				}
				return this.link;
			}
			set
			{
				this.link = value;
			}
		}

		private LiveHeaderLinkCollection list;

		private LiveHeaderLink link;
	}
}

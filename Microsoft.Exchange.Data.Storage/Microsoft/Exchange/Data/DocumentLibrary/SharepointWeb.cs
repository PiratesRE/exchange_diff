using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SharepointWeb
	{
		internal SharepointWeb(string title, SharepointSiteId siteId)
		{
			this.title = title;
			this.siteId = siteId;
		}

		public string Title
		{
			get
			{
				return this.title;
			}
		}

		public Uri Uri
		{
			get
			{
				return this.siteId.SiteUri;
			}
		}

		public ObjectId Id
		{
			get
			{
				return this.siteId;
			}
		}

		private readonly string title;

		private readonly SharepointSiteId siteId;
	}
}

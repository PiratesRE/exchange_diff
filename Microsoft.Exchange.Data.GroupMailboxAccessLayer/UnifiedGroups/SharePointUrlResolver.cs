using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UnifiedGroups
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SharePointUrlResolver
	{
		public SharePointUrlResolver(ADUser groupAdUser)
		{
			ArgumentValidator.ThrowIfNull("groupADUser", groupAdUser);
			this.groupADUser = groupAdUser;
		}

		public Uri RootUri
		{
			get
			{
				if (this.rootUri == null)
				{
					this.rootUri = SharePointUrl.GetRootSiteUrl(this.groupADUser.OrganizationId);
				}
				return this.rootUri;
			}
		}

		public string GetSiteUrl()
		{
			if (this.groupADUser.SharePointSiteUrl != null)
			{
				return this.groupADUser.SharePointSiteUrl;
			}
			if (this.groupADUser.SharePointUrl != null)
			{
				return this.groupADUser.SharePointUrl.ToString();
			}
			if (this.RootUri != null)
			{
				return new Uri(this.RootUri, "_layouts/groupstatus.aspx?id=" + this.groupADUser.ExternalDirectoryObjectId + "&target=site").ToString();
			}
			return null;
		}

		public string GetDocumentsUrl()
		{
			if (this.groupADUser.SharePointDocumentsUrl != null)
			{
				return this.groupADUser.SharePointDocumentsUrl;
			}
			if (this.RootUri != null)
			{
				return new Uri(this.RootUri, "_layouts/groupstatus.aspx?id=" + this.groupADUser.ExternalDirectoryObjectId + "&target=documents").ToString();
			}
			return null;
		}

		private ADUser groupADUser;

		private Uri rootUri;
	}
}

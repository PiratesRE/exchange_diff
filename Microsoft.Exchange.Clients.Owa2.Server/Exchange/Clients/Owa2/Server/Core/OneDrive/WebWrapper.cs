using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class WebWrapper : ClientObjectWrapper<Web>, IWeb, IClientObject<Web>
	{
		public IListCollection Lists
		{
			get
			{
				ListCollectionWrapper result;
				if ((result = this.lists) == null)
				{
					result = (this.lists = new ListCollectionWrapper(this.backingWeb.Lists));
				}
				return result;
			}
		}

		public WebWrapper(Web web) : base(web)
		{
			this.backingWeb = web;
		}

		public IFolder GetFolderByServerRelativeUrl(string serverRelativeUrl)
		{
			return new FolderWrapper(this.backingWeb.GetFolderByServerRelativeUrl(serverRelativeUrl));
		}

		public IFile GetFileByServerRelativeUrl(string relativeLocation)
		{
			return new FileWrapper(this.backingWeb.GetFileByServerRelativeUrl(relativeLocation));
		}

		public IList GetList(string url)
		{
			return new ListWrapper(this.backingWeb.GetList(url));
		}

		private Web backingWeb;

		private ListCollectionWrapper lists;
	}
}

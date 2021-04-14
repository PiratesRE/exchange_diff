using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal abstract class ListViewViewState : ModuleViewState
	{
		public ListViewViewState(NavigationModule navigationModule, StoreObjectId folderId, string folderType, int pageNumber) : base(navigationModule, folderId, folderType)
		{
			this.pageNumber = pageNumber;
		}

		public int PageNumber
		{
			get
			{
				return this.pageNumber;
			}
			set
			{
				this.pageNumber = value;
			}
		}

		public override PreFormActionResponse ToPreFormActionResponse()
		{
			PreFormActionResponse preFormActionResponse = base.ToPreFormActionResponse();
			if (this.pageNumber > 0)
			{
				preFormActionResponse.AddParameter("pg", this.pageNumber.ToString());
			}
			return preFormActionResponse;
		}

		private int pageNumber;
	}
}

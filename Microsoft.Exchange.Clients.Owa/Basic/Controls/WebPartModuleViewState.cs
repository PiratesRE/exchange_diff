using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class WebPartModuleViewState : ListViewViewState
	{
		public WebPartModuleViewState(StoreObjectId folderId, string folderType, int pageNumber, NavigationModule navigationModule, SortOrder sortOrder, ColumnId sortedColumn) : base(navigationModule, folderId, folderType, pageNumber)
		{
			this.sortOrder = sortOrder;
			this.sortedColumn = sortedColumn;
		}

		public SortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		public ColumnId SortedColumn
		{
			get
			{
				return this.sortedColumn;
			}
		}

		public override PreFormActionResponse ToPreFormActionResponse()
		{
			PreFormActionResponse preFormActionResponse = base.ToPreFormActionResponse();
			preFormActionResponse.ApplicationElement = ApplicationElement.WebPartFolder;
			return preFormActionResponse;
		}

		private SortOrder sortOrder;

		private ColumnId sortedColumn;
	}
}

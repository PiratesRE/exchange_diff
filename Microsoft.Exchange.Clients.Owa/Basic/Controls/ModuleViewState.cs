using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal abstract class ModuleViewState : ClientViewState
	{
		public ModuleViewState(NavigationModule navigationModule, StoreObjectId folderId, string folderType)
		{
			this.navigationModule = navigationModule;
			this.folderId = folderId;
			this.folderType = folderType;
		}

		public StoreObjectId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public string FolderType
		{
			get
			{
				return this.folderType;
			}
		}

		public NavigationModule NavigationModule
		{
			get
			{
				return this.navigationModule;
			}
		}

		public override PreFormActionResponse ToPreFormActionResponse()
		{
			PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
			preFormActionResponse.ApplicationElement = ApplicationElement.Folder;
			preFormActionResponse.Type = this.FolderType;
			if (this.FolderId != null)
			{
				preFormActionResponse.AddParameter("id", this.FolderId.ToBase64String());
			}
			return preFormActionResponse;
		}

		private StoreObjectId folderId;

		private string folderType;

		private NavigationModule navigationModule;
	}
}

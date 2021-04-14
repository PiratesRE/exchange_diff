using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventObjectId(typeof(OwaStoreObjectId))]
	[OwaEventNamespace("DumpsterVLV")]
	internal sealed class DumpsterVirtualListViewEventHandler : VirtualListViewEventHandler2
	{
		protected override VirtualListViewState ListViewState
		{
			get
			{
				return (FolderVirtualListViewState)base.GetParameter("St");
			}
		}

		private Folder DataFolder
		{
			get
			{
				return this.searchFolder ?? this.folder;
			}
		}

		protected override VirtualListView2 GetListView()
		{
			this.BindToFolder();
			return new DumpsterVirtualListView(base.UserContext, "divVLV", this.ListViewState.SortedColumn, this.ListViewState.SortOrder, this.DataFolder);
		}

		private void BindToFolder()
		{
			if (this.folder == null)
			{
				OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("fId");
				if (owaStoreObjectId == null)
				{
					owaStoreObjectId = (OwaStoreObjectId)this.ListViewState.SourceContainerId;
				}
				this.folder = Utilities.GetFolder<Folder>(base.UserContext, owaStoreObjectId, new PropertyDefinition[]
				{
					FolderSchema.ItemCount,
					FolderSchema.ExtendedFolderFlags,
					ViewStateProperties.ViewFilter,
					ViewStateProperties.SortColumn
				});
				if (base.IsParameterSet("srchf") && (base.UserContext.IsInMyMailbox(this.folder) || Utilities.IsInArchiveMailbox(this.folder)))
				{
					FolderVirtualListViewSearchFilter folderVirtualListViewSearchFilter = (FolderVirtualListViewSearchFilter)base.GetParameter("srchf");
					if (folderVirtualListViewSearchFilter.SearchString != null && 256 < folderVirtualListViewSearchFilter.SearchString.Length)
					{
						throw new OwaInvalidRequestException("Search string is longer than maximum length of " + 256);
					}
					FolderSearch folderSearch = new FolderSearch();
					string searchString = folderVirtualListViewSearchFilter.SearchString;
					this.searchFolder = folderSearch.Execute(base.UserContext, this.folder, folderVirtualListViewSearchFilter.Scope, searchString, folderVirtualListViewSearchFilter.ReExecuteSearch, folderVirtualListViewSearchFilter.IsAsyncSearchEnabled);
				}
			}
		}

		protected override void EndProcessEvent()
		{
			if (this.folder != null)
			{
				this.folder.Dispose();
				this.folder = null;
			}
			if (this.searchFolder != null)
			{
				this.searchFolder.Dispose();
				this.searchFolder = null;
			}
		}

		private void RenderErrorInfobarMessage(Strings.IDs errID)
		{
			this.Writer.Write("<div id=eib>");
			this.Writer.Write(LocalizedStrings.GetHtmlEncoded(errID));
			this.Writer.Write("</div>");
		}

		[OwaEventParameter("SR", typeof(int))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventSegmentation(Feature.Dumpster)]
		[OwaEvent("LoadFresh")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		public override void LoadFresh()
		{
			base.InternalLoadFresh();
		}

		[OwaEventParameter("AId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEvent("LoadNext")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventSegmentation(Feature.Dumpster)]
		public override void LoadNext()
		{
			base.InternalLoadNext();
		}

		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEvent("LoadPrevious")]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventSegmentation(Feature.Dumpster)]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("AId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("RC", typeof(int))]
		public override void LoadPrevious()
		{
			base.InternalLoadPrevious();
		}

		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("SId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventParameter("nwSel", typeof(bool), false, true)]
		[OwaEventSegmentation(Feature.Dumpster)]
		[OwaEvent("SeekNext")]
		public override void SeekNext()
		{
			base.InternalSeekNext();
		}

		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEvent("SeekPrevious")]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventParameter("nwSel", typeof(bool), false, true)]
		[OwaEventSegmentation(Feature.Dumpster)]
		[OwaEventParameter("SId", typeof(OwaStoreObjectId))]
		public override void SeekPrevious()
		{
			base.InternalSeekPrevious();
		}

		[OwaEvent("Sort")]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventParameter("SId", typeof(OwaStoreObjectId), false, true)]
		[OwaEventSegmentation(Feature.Dumpster)]
		[OwaEventVerb(OwaEventVerb.Post)]
		public override void Sort()
		{
			base.InternalSort();
		}

		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventSegmentation(Feature.Dumpster)]
		[OwaEvent("SetML")]
		[OwaEventParameter("SR", typeof(int))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		public override void SetMultiLineState()
		{
			base.InternalSetMultiLineState();
		}

		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("td", typeof(string))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventParameter("fltr", typeof(int), false, true)]
		[OwaEventSegmentation(Feature.Dumpster)]
		[OwaEvent("TypeDown")]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		public override void TypeDownSearch()
		{
			base.InternalTypeDownSearch();
		}

		[OwaEventSegmentation(Feature.Dumpster)]
		[OwaEventParameter("Itms", typeof(OwaStoreObjectId), true)]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		[OwaEvent("PermanentDelete")]
		public void PermanentDelete()
		{
			try
			{
				ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "DumpsterVirtualListViewEventHandler.PermanentDelete");
				OwaStoreObjectId folderId = (OwaStoreObjectId)base.GetParameter("fId");
				this.folder = Utilities.GetFolderForContent<Folder>(base.UserContext, folderId, null);
				OperationResult operationResult = this.DoDelete();
				if (operationResult == OperationResult.PartiallySucceeded)
				{
					this.RenderErrorInfobarMessage(1086565410);
				}
				else if (operationResult == OperationResult.Failed)
				{
					this.RenderErrorInfobarMessage(1546128956);
				}
			}
			finally
			{
				this.EndProcessEvent();
			}
		}

		private OperationResult DoDelete()
		{
			StoreObjectId[] dumpsterIds = this.GetDumpsterIds();
			return this.folder.DeleteObjects(DeleteItemFlags.HardDelete, dumpsterIds).OperationResult;
		}

		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(DumpsterVirtualListViewEventHandler));
		}

		private StoreObjectId[] GetDumpsterIds()
		{
			OwaStoreObjectId[] array = (OwaStoreObjectId[])base.GetParameter("Itms");
			if (array.Length > 500)
			{
				throw new OwaInvalidOperationException(string.Format("Operating on {0} item(s) in a single request is not supported", array.Length));
			}
			StoreObjectId[] array2 = new StoreObjectId[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].StoreObjectId;
			}
			return array2;
		}

		public const string EventNamespace = "DumpsterVLV";

		public const string MethodPermanentDelete = "PermanentDelete";

		public const string Items = "Itms";

		public const string FolderId = "fId";

		public const string SearchFilter = "srchf";

		private const int MaxItemsPerRequest = 500;

		private Folder folder;

		private Folder searchFolder;
	}
}

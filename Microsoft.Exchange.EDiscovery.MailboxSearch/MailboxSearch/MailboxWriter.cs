using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.EDiscovery.Export;
using Microsoft.Exchange.EDiscovery.Export.EwsProxy;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch
{
	internal class MailboxWriter : IContextualBatchDataWriter<List<ItemInformation>>, IBatchDataWriter<List<ItemInformation>>, IDisposable
	{
		public MailboxWriter(IExportContext exportContext, ITargetMailbox targetMailbox, IProgressController progressController) : this(exportContext, targetMailbox, progressController, null)
		{
		}

		public MailboxWriter(IExportContext exportContext, ITargetMailbox targetMailbox, IProgressController progressController, TargetFolderProvider<string, BaseFolderType, ITargetMailbox> targetFolderProvider)
		{
			Util.ThrowIfNull(exportContext, "exportContext");
			Util.ThrowIfNull(targetMailbox, "targetMailbox");
			Util.ThrowIfNull(progressController, "progressController");
			this.includeDuplicates = exportContext.ExportMetadata.IncludeDuplicates;
			this.targetMailbox = targetMailbox;
			this.progressController = progressController;
			this.timer = new Stopwatch();
			this.targetFolderProvider = targetFolderProvider;
			if (this.targetFolderProvider == null)
			{
				this.targetFolderProvider = new MailboxTargetFolderProvider(exportContext, this.targetMailbox);
			}
		}

		public void EnterDataContext(DataContext dataContext)
		{
			this.dataContext = dataContext;
			this.timer.Restart();
			this.targetFolderProvider.Reset(this.dataContext);
		}

		public void ExitDataContext(bool errorHappened)
		{
			if (this.dataContext != null)
			{
				this.dataContext = null;
			}
		}

		public void ExitPFDataContext(bool errorHappened)
		{
			if (this.dataContext != null)
			{
				this.dataContext = null;
			}
		}

		public void WriteDataBatch(List<ItemInformation> dataBatch)
		{
			List<ItemInformation> list = new List<ItemInformation>(Math.Min(dataBatch.Count, Constants.ReadWriteBatchSize));
			string parentFolderId = null;
			string text = string.Empty;
			ProgressRecord progressRecord = new ProgressRecord(this.dataContext);
			try
			{
				foreach (ItemInformation itemInformation in dataBatch)
				{
					if (this.progressController.IsStopRequested)
					{
						break;
					}
					BaseFolderType parentFolder = this.targetFolderProvider.GetParentFolder(this.targetMailbox, itemInformation.Id.ParentFolder, this.includeDuplicates);
					if (parentFolder == null)
					{
						progressRecord.ReportItemError(itemInformation.Id, null, ExportErrorType.ParentFolderNotFound, string.Format("EDiscoveryError:E007:: Parent folder for this item is not found.", new object[0]));
					}
					else
					{
						parentFolderId = parentFolder.FolderId.Id;
						if (text == string.Empty)
						{
							text = parentFolder.FolderId.Id;
						}
						if (itemInformation.Error == null)
						{
							if (itemInformation.Id.IsDuplicate)
							{
								progressRecord.ReportItemExported(itemInformation.Id, null, null);
							}
							else
							{
								if (text != parentFolder.FolderId.Id || list.Count == Constants.ReadWriteBatchSize)
								{
									this.CopyItemsToTargetMailbox(text, list, progressRecord);
									text = parentFolder.FolderId.Id;
									list.Clear();
								}
								list.Add(itemInformation);
							}
						}
						else
						{
							progressRecord.ReportItemError(itemInformation.Id, null, itemInformation.Error.ErrorType, itemInformation.Error.Message);
						}
					}
				}
				if (list.Count > 0)
				{
					this.CopyItemsToTargetMailbox(parentFolderId, list, progressRecord);
					list.Clear();
				}
			}
			finally
			{
				this.timer.Stop();
				if (progressRecord != null)
				{
					progressRecord.ReportDuration(this.timer.Elapsed);
					this.progressController.ReportProgress(progressRecord);
				}
				this.timer.Restart();
			}
		}

		public void Dispose()
		{
		}

		private void CopyItemsToTargetMailbox(string parentFolderId, IList<ItemInformation> items, ProgressRecord progressRecord)
		{
			List<ItemInformation> list = this.targetMailbox.CopyItems(parentFolderId, items);
			if (list != null && list.Count > 0)
			{
				foreach (ItemInformation itemInformation in list)
				{
					if (itemInformation.Error != null)
					{
						progressRecord.ReportItemError(itemInformation.Id, null, itemInformation.Error.ErrorType, itemInformation.Error.Message);
					}
					else
					{
						progressRecord.ReportItemExported(itemInformation.Id, itemInformation.Id.Id, null);
					}
				}
			}
		}

		private readonly Stopwatch timer;

		private readonly TargetFolderProvider<string, BaseFolderType, ITargetMailbox> targetFolderProvider;

		private readonly ITargetMailbox targetMailbox;

		private readonly IProgressController progressController;

		private readonly bool includeDuplicates;

		private DataContext dataContext;
	}
}

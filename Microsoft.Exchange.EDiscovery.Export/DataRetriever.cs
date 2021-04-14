using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class DataRetriever : IBatchDataReader<List<ItemInformation>>
	{
		public DataRetriever(DataContext dataContext, IProgressController progressController)
		{
			this.dataContext = dataContext;
			this.processedItemCount = this.dataContext.ProcessedItemCount;
			this.currentBatch = new List<ItemId>(ConstantProvider.ExportBatchItemCountLimit);
			this.currentBatchSize = 0U;
			this.currentBatchForDuplicates = new List<ItemId>(ConstantProvider.ExportBatchItemCountLimit);
			this.progressController = progressController;
		}

		public event EventHandler<DataBatchEventArgs<List<ItemInformation>>> DataBatchRead;

		public event EventHandler AbortingOnError;

		public DataContext DataContext
		{
			get
			{
				return this.dataContext;
			}
		}

		public IContextualBatchDataWriter<List<ItemInformation>> DataWriter { get; set; }

		public void StartReading()
		{
			try
			{
				int num = 0;
				int num2 = this.dataContext.ProcessedItemCount;
				foreach (ItemId itemId in this.dataContext.ItemIdList.ReadItemIds())
				{
					if (this.progressController.IsStopRequested)
					{
						Tracer.TraceInformation("DataRetriever.StartReading: Stop requested", new object[0]);
						return;
					}
					if (num < num2)
					{
						num++;
					}
					else
					{
						this.ProcessItemIdLegacy(itemId);
					}
				}
				this.ProcessBatchDataLegacy();
			}
			catch (ExportException ex)
			{
				Tracer.TraceError("DataRetriever.StartReading: Exception happend: {0}", new object[]
				{
					ex
				});
				this.StopOnError(new ErrorRecord
				{
					ErrorType = ex.ErrorType,
					DiagnosticMessage = ex.Message,
					Time = DateTime.UtcNow,
					Item = null,
					SourceId = this.dataContext.SourceId
				});
			}
		}

		public void ProcessItems(ref List<ItemId> itemList)
		{
			try
			{
				int num = 0;
				int num2 = 0;
				foreach (ItemId itemId in itemList)
				{
					if (this.progressController.IsStopRequested)
					{
						Tracer.TraceInformation("DataRetriever.ProcessItems: Stop requested", new object[0]);
						return;
					}
					if (num < this.processedItemCount)
					{
						num++;
					}
					else
					{
						this.ProcessItemId(itemId);
					}
					num2++;
					if ((ulong)this.currentBatchSize >= (ulong)((long)ConstantProvider.ExportBatchSizeLimit) || this.currentBatch.Count + this.currentBatchForDuplicates.Count >= ConstantProvider.ExportBatchItemCountLimit)
					{
						break;
					}
				}
				if (num2 == itemList.Count)
				{
					itemList.Clear();
				}
				else if (itemList.Count > 0 && itemList.Count > num2)
				{
					itemList.RemoveRange(0, num2);
				}
				else
				{
					Tracer.TraceError("DataRetriever.ProcessItems: error: tempCount > itemList.Count, this shouldn't happen tempCount: {0}; itemList.Count: {1}", new object[]
					{
						num2,
						itemList.Count
					});
				}
			}
			catch (ExportException ex)
			{
				Tracer.TraceError("DataRetriever.ProcessItems: Exception happend: {0}", new object[]
				{
					ex
				});
				this.StopOnError(new ErrorRecord
				{
					ErrorType = ex.ErrorType,
					DiagnosticMessage = ex.Message,
					Time = DateTime.UtcNow,
					Item = null,
					SourceId = this.dataContext.SourceId
				});
			}
		}

		public List<ItemInformation> ProcessBatchData()
		{
			Tracer.TraceInformation("DataRetriever.ProcessBatchData: Processing batch data. currentBatch.Count={0}, currentBatchForDuplicates.Count={1}", new object[]
			{
				this.currentBatch.Count,
				this.currentBatchForDuplicates.Count
			});
			List<ItemInformation> list = null;
			if (this.currentBatch.Count > 0)
			{
				list = this.dataContext.ServiceClient.ExportItems(this.dataContext.IsPublicFolder ? this.dataContext.SourceLegacyExchangeDN : this.dataContext.SourceId, this.currentBatch, this.progressController.IsDocumentIdHintFlightingEnabled);
				this.currentBatch.Clear();
				this.currentBatchSize = 0U;
			}
			Tracer.TraceInformation("DataRetriever.ProcessBatchData: batch data exported.", new object[0]);
			if (this.currentBatchForDuplicates.Count > 0)
			{
				if (list == null)
				{
					list = new List<ItemInformation>(this.currentBatchForDuplicates.Count);
				}
				list.AddRange(from itemId in this.currentBatchForDuplicates
				select new ItemInformation
				{
					Id = itemId
				});
				this.currentBatchForDuplicates.Clear();
			}
			return list;
		}

		public void OnDataBatchRead(List<ItemInformation> items)
		{
			EventHandler<DataBatchEventArgs<List<ItemInformation>>> dataBatchRead = this.DataBatchRead;
			if (dataBatchRead != null)
			{
				dataBatchRead(this, new DataBatchEventArgs<List<ItemInformation>>
				{
					DataBatch = items
				});
			}
		}

		private void ProcessItemIdLegacy(ItemId itemId)
		{
			if (itemId.IsDuplicate)
			{
				this.currentBatchForDuplicates.Add(itemId);
			}
			else
			{
				if (this.currentBatch.Count > 0 && !string.Equals(this.dataContext.ServiceClient.GetPhysicalPartitionIdentifier(this.currentBatch[0]), this.dataContext.ServiceClient.GetPhysicalPartitionIdentifier(itemId), StringComparison.OrdinalIgnoreCase))
				{
					this.ProcessBatchDataLegacy();
				}
				this.currentBatch.Add(itemId);
				this.currentBatchSize += itemId.Size;
			}
			if ((ulong)this.currentBatchSize >= (ulong)((long)ConstantProvider.ExportBatchSizeLimit) || this.currentBatch.Count + this.currentBatchForDuplicates.Count >= ConstantProvider.ExportBatchItemCountLimit)
			{
				this.ProcessBatchDataLegacy();
			}
		}

		private void ProcessItemId(ItemId itemId)
		{
			if (itemId.IsDuplicate)
			{
				this.currentBatchForDuplicates.Add(itemId);
				return;
			}
			this.currentBatch.Add(itemId);
			this.currentBatchSize += itemId.Size;
		}

		private void ProcessBatchDataLegacy()
		{
			Tracer.TraceInformation("DataRetriever.ProcessBatchData2: Processing batch data. currentBatch.Count={0}, currentBatchForDuplicates.Count={1}", new object[]
			{
				this.currentBatch.Count,
				this.currentBatchForDuplicates.Count
			});
			List<ItemInformation> list = null;
			if (this.currentBatch.Count > 0)
			{
				list = this.dataContext.ServiceClient.ExportItems(this.dataContext.IsPublicFolder ? this.dataContext.SourceLegacyExchangeDN : this.dataContext.SourceId, this.currentBatch, this.progressController.IsDocumentIdHintFlightingEnabled);
				this.currentBatch.Clear();
				this.currentBatchSize = 0U;
			}
			Tracer.TraceInformation("DataRetriever.ProcessBatchData: batch data exported.", new object[0]);
			if (this.currentBatchForDuplicates.Count > 0)
			{
				if (list == null)
				{
					list = new List<ItemInformation>(this.currentBatchForDuplicates.Count);
				}
				list.AddRange(from itemId in this.currentBatchForDuplicates
				select new ItemInformation
				{
					Id = itemId
				});
				this.currentBatchForDuplicates.Clear();
			}
			if (list != null && list.Count > 0)
			{
				this.OnDataBatchRead(list);
				Tracer.TraceInformation("DataRetriever.ProcessBatchData: OnDataBatchRead triggered.", new object[0]);
			}
		}

		private void StopOnError(ErrorRecord error)
		{
			EventHandler abortingOnError = this.AbortingOnError;
			if (abortingOnError != null)
			{
				abortingOnError(this, EventArgs.Empty);
			}
			this.currentBatch.Clear();
			this.currentBatchForDuplicates.Clear();
			ProgressRecord progressRecord = new ProgressRecord();
			progressRecord.ReportSourceError(error);
			this.progressController.ReportProgress(progressRecord);
		}

		private readonly DataContext dataContext;

		private readonly List<ItemId> currentBatch;

		private readonly List<ItemId> currentBatchForDuplicates;

		private readonly IProgressController progressController;

		private readonly int processedItemCount;

		private uint currentBatchSize;
	}
}

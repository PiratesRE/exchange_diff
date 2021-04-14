using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class ProgressController : IExportHandler, IDisposable, IProgressController
	{
		public ProgressController(ITarget target, IServiceClientFactory serviceClientFactory)
		{
			ProgressController.TargetWrapper targetWrapper = new ProgressController.TargetWrapper(target);
			this.target = targetWrapper;
			if (target.ExportContext.ExportMetadata.IncludeUnsearchableItems && !Util.IncludeUnsearchableItems(target.ExportContext))
			{
				targetWrapper.ExportContextInternal.ExportMetadataInternal.IncludeUnsearchableItemsInternal = false;
				Tracer.TraceInformation("ProgressController.ProgressController: IncludeUnsearchableItems is disabled. search query={0}.", new object[]
				{
					target.ExportContext.Sources[0].SourceFilter
				});
			}
			this.abortTokenSourceForTasks = new CancellationTokenSource();
			this.progressAvailable = new Semaphore(0, int.MaxValue);
			this.progressQueue = new ConcurrentQueue<ProgressRecord>();
			this.isDocIdHintFlightingEnabled = false;
			this.StatusManager = new StatusManager(this.target);
			this.ItemListGenerator = new ItemListGenerator(this.StatusManager.AllSourceInformation, this.target, this);
			this.SearchResults = new SearchResults(this.StatusManager.AllSourceInformation, this.target);
			this.sourceDataProviderManager = new SourceDataProviderManager(serviceClientFactory, this.abortTokenSourceForTasks.Token);
			this.sourceDataProviderManager.ProgressController = this;
		}

		public event EventHandler<ExportStatusEventArgs> OnReportStatistics;

		public IExportContext ExportContext
		{
			get
			{
				return this.target.ExportContext;
			}
		}

		public bool IsStopRequested { get; private set; }

		public ISearchResults SearchResults { get; private set; }

		public ItemListGenerator ItemListGenerator { get; private set; }

		public OperationStatus CurrentStatus
		{
			get
			{
				return this.StatusManager.CurrentStatus;
			}
		}

		public bool IsDocIdHintFlightingEnabled
		{
			get
			{
				return this.isDocIdHintFlightingEnabled;
			}
			set
			{
				this.isDocIdHintFlightingEnabled = value;
			}
		}

		public bool IsDocumentIdHintFlightingEnabled
		{
			get
			{
				return this.IsDocIdHintFlightingEnabled;
			}
		}

		internal IStatusManager StatusManager { get; set; }

		public void Dispose()
		{
			if (this.progressAvailable != null)
			{
				this.progressAvailable.Close();
				this.progressAvailable = null;
			}
			if (this.StatusManager != null)
			{
				this.StatusManager.Dispose();
				this.StatusManager = null;
			}
			if (this.abortTokenSourceForTasks != null)
			{
				this.abortTokenSourceForTasks.Dispose();
				this.abortTokenSourceForTasks = null;
			}
		}

		public void EnsureAuthentication(ICredentialHandler credentialHandler, Uri configurationServiceEndpoint = null)
		{
			if (credentialHandler != null)
			{
				this.sourceDataProviderManager.AutoDiscoverSourceServiceEndpoints(this.StatusManager.AllSourceInformation, configurationServiceEndpoint, credentialHandler);
				this.sourceDataProviderManager.CreateSourceServiceClients(this.StatusManager.AllSourceInformation);
			}
		}

		public void Prepare()
		{
			this.ExecuteProcedureWithProgress(ProcedureType.Prepare, new Action(this.InternalPrepare));
		}

		public void Export()
		{
			this.ExecuteProcedureWithProgress(ProcedureType.Export, new Action(this.InternalExport));
		}

		public void Stop()
		{
			this.IsStopRequested = true;
			Tracer.TraceInformation("ProgressController.Stop: calling abortTokenSourceForTasks.Cancel()", new object[0]);
			this.abortTokenSourceForTasks.Cancel();
		}

		public void Rollback()
		{
			try
			{
				if (this.StatusManager.BeginProcedure(ProcedureType.Rollback))
				{
					Tracer.TraceInformation("ProgressController.Rollback: Rollbacking and removing status log.", new object[0]);
					this.StatusManager.Rollback(true);
				}
			}
			finally
			{
				this.StatusManager.EndProcedure();
			}
		}

		public void ReportProgress(ProgressRecord progressRecord)
		{
			Tracer.TraceInformation("ProgressController.ReportProgress: ProgressRecord reported. Queue count before enqueue: {0}", new object[]
			{
				this.progressQueue.Count
			});
			this.progressQueue.Enqueue(progressRecord);
			this.progressAvailable.Release(1);
		}

		private void ExecuteProcedureWithProgress(ProcedureType procedureType, Action action)
		{
			if (this.IsStopRequested)
			{
				Tracer.TraceInformation("ProgressController.ExecuteProcedureWithProgress: Stop requested. Procedure type: {0}", new object[]
				{
					procedureType
				});
				return;
			}
			try
			{
				if (this.StatusManager.BeginProcedure(procedureType))
				{
					ScenarioData scenarioData = ScenarioData.Current;
					Task task = Task.Factory.StartNew(delegate()
					{
						ScenarioData scenarioData;
						using (new ScenarioData(scenarioData))
						{
							this.WaitForAndProcessProgress();
						}
					});
					Exception ex = null;
					try
					{
						action();
					}
					finally
					{
						this.progressAvailable.Release(1);
						ex = AsynchronousTaskHandler.WaitForAsynchronousTask(task);
					}
					if (ex != null)
					{
						Tracer.TraceError("ProgressController.ExecuteProcedureWithProgress: Hitting exception in progress task. Exception: {0}", new object[]
						{
							ex
						});
						throw ex;
					}
				}
			}
			finally
			{
				this.StatusManager.EndProcedure();
			}
		}

		private void InternalPrepare()
		{
			this.ItemListGenerator.AllSourceInformation = this.StatusManager.AllSourceInformation;
			if (this.StatusManager.AllSourceInformation != null)
			{
				ScenarioData.Current["M"] = this.StatusManager.AllSourceInformation.Count.ToString();
			}
			this.ItemListGenerator.DataRetriever = null;
			this.ItemListGenerator.InitItemList();
			if (this.IsStopRequested)
			{
				Tracer.TraceInformation("ProgressController.InternalPrepare: Stop requested.", new object[0]);
				if (this.StatusManager.BeginProcedure(ProcedureType.Stop))
				{
					this.StatusManager.Rollback(false);
				}
			}
		}

		private void InternalExport()
		{
			using (IContextualBatchDataWriter<List<ItemInformation>> contextualBatchDataWriter = this.target.CreateDataWriter(this))
			{
				bool errorHappened = false;
				try
				{
					if (this.StatusManager.AllSourceInformation != null)
					{
						ScenarioData.Current["M"] = this.StatusManager.AllSourceInformation.Count.ToString();
					}
					this.sourceDataProviderManager.CreateSourceServiceClients(this.StatusManager.AllSourceInformation);
					this.ItemListGenerator.AllSourceInformation = this.StatusManager.AllSourceInformation;
					int num = 0;
					this.ReportStatistics(new ExportStatusEventArgs
					{
						ActualBytes = 0L,
						ActualCount = 0,
						ActualMailboxesProcessed = 0,
						ActualMailboxesTotal = this.StatusManager.AllSourceInformation.Count,
						TotalDuration = TimeSpan.Zero
					});
					foreach (SourceInformation sourceInformation in this.StatusManager.AllSourceInformation.Values)
					{
						if (sourceInformation.Configuration != null && sourceInformation.Configuration.SourceFilter != null)
						{
							ScenarioData.Current["QL"] = sourceInformation.Configuration.SourceFilter.Length.ToString();
						}
						Tracer.TraceInformation("ProgressController.InternalExport: Exporting source '{0}'. ItemCount: {1}; ProcessedItemCount: {2}; UnsearchableItemCount: {3}; ProcessedUnsearchableItemCount: {4}; DuplicateItemCount: {5}; UnsearchableDuplicateItemCount: {6}; ErrorItemCount: {7}", new object[]
						{
							sourceInformation.Configuration.Id,
							sourceInformation.Status.ItemCount,
							sourceInformation.Status.ProcessedItemCount,
							sourceInformation.Status.UnsearchableItemCount,
							sourceInformation.Status.ProcessedUnsearchableItemCount,
							sourceInformation.Status.DuplicateItemCount,
							sourceInformation.Status.UnsearchableDuplicateItemCount,
							sourceInformation.Status.ErrorItemCount
						});
						if (this.IsStopRequested)
						{
							Tracer.TraceInformation("ProgressController.InternalExport: Stop requested.", new object[0]);
							this.StatusManager.BeginProcedure(ProcedureType.Stop);
							break;
						}
						try
						{
							num++;
							this.ExportSourceMailbox(contextualBatchDataWriter, sourceInformation, false);
							this.ExportSourceMailbox(contextualBatchDataWriter, sourceInformation, true);
							if (sourceInformation.Configuration.Id.StartsWith("\\"))
							{
								errorHappened = true;
							}
							Tracer.TraceInformation("ProgressController.InternalExport: Exporting source '{0}'. ItemCount: {1}; ProcessedItemCount: {2}; UnsearchableItemCount: {3}; ProcessedUnsearchableItemCount: {4}; DuplicateItemCount: {5}; UnsearchableDuplicateItemCount: {6}; ErrorItemCount: {7}", new object[]
							{
								sourceInformation.Configuration.Id,
								sourceInformation.Status.ItemCount,
								sourceInformation.Status.ProcessedItemCount,
								sourceInformation.Status.UnsearchableItemCount,
								sourceInformation.Status.ProcessedUnsearchableItemCount,
								sourceInformation.Status.DuplicateItemCount,
								sourceInformation.Status.UnsearchableDuplicateItemCount,
								sourceInformation.Status.ErrorItemCount
							});
						}
						catch (ExportException ex)
						{
							if (ex.ErrorType == ExportErrorType.TargetOutOfSpace)
							{
								Tracer.TraceInformation("ProgressController.InternalExport: Target level error occurs during export.: {0}", new object[]
								{
									ex
								});
								throw;
							}
							Tracer.TraceInformation("ProgressController.InternalExport: Source level error occurs during export: {0}", new object[]
							{
								ex
							});
							ProgressRecord progressRecord = new ProgressRecord();
							progressRecord.ReportSourceError(new ErrorRecord
							{
								Item = null,
								ErrorType = ex.ErrorType,
								DiagnosticMessage = ex.Message,
								SourceId = sourceInformation.Configuration.Id,
								Time = DateTime.UtcNow
							});
							this.ReportProgress(progressRecord);
						}
						finally
						{
							this.ReportStatistics(new ExportStatusEventArgs
							{
								ActualBytes = 0L,
								ActualCount = 0,
								ActualMailboxesProcessed = num,
								ActualMailboxesTotal = this.StatusManager.AllSourceInformation.Count,
								TotalDuration = TimeSpan.Zero
							});
						}
					}
					errorHappened = true;
				}
				finally
				{
					contextualBatchDataWriter.ExitPFDataContext(errorHappened);
				}
			}
		}

		private void ExportSourceMailbox(IContextualBatchDataWriter<List<ItemInformation>> dataWriter, SourceInformation source, bool isUnsearchable)
		{
			Tracer.TraceInformation("ProgressController.ExportSourceMailbox: Source Id: {0}; isUnsearchable: {1}", new object[]
			{
				source.Configuration.Id,
				isUnsearchable
			});
			this.ItemListGenerator.DoUnSearchable = isUnsearchable;
			bool flag = (!isUnsearchable && source.Status.ItemCount <= 0) || (isUnsearchable && source.Status.UnsearchableItemCount <= 0);
			if ((!isUnsearchable && source.Status.ItemCount > source.Status.ProcessedItemCount) || (isUnsearchable && source.Status.UnsearchableItemCount > source.Status.ProcessedUnsearchableItemCount) || !this.target.ExportContext.IsResume || flag)
			{
				IItemIdList itemIdList = this.target.CreateItemIdList(source.Configuration.Id, isUnsearchable);
				DataContext dataContext = new DataContext(source, itemIdList);
				DataRetriever dataRetriever = new DataRetriever(dataContext, this);
				dataRetriever.DataWriter = dataWriter;
				this.ItemListGenerator.DataRetriever = dataRetriever;
				this.ItemListGenerator.DataBatchRead += this.ItemListGenerator.WriteDataBatchItemListGen;
				dataRetriever.DataBatchRead += this.ItemListGenerator.WriteDataBatchDataRetriever;
				ExportException ex = null;
				try
				{
					this.ItemListGenerator.DoExportForSourceMailbox(dataRetriever.DataContext.SourceInformation);
				}
				finally
				{
					ex = AsynchronousTaskHandler.WaitForAsynchronousTask(this.ItemListGenerator.WritingTask);
					this.ItemListGenerator.WritingTask = null;
					this.ItemListGenerator.DataRetriever = null;
				}
				if (ex != null)
				{
					throw ex;
				}
			}
		}

		private void WaitForAndProcessProgress()
		{
			ProgressRecord progressRecord = null;
			bool flag = true;
			while (flag)
			{
				this.progressAvailable.WaitOne();
				Tracer.TraceInformation("ProgressController.WaitForAndProcessProgress: Progress signaled.", new object[0]);
				flag = this.progressQueue.TryDequeue(out progressRecord);
				if (progressRecord != null)
				{
					try
					{
						if (progressRecord.RootExportedRecord != null)
						{
							this.target.ExportContext.WriteResultManifest(new ExportRecord[]
							{
								progressRecord.RootExportedRecord
							});
						}
						else if (progressRecord.SourceErrorRecord != null)
						{
							this.target.ExportContext.WriteErrorLog(new ErrorRecord[]
							{
								progressRecord.SourceErrorRecord
							});
						}
						else
						{
							if (progressRecord.ItemExportedRecords != null && progressRecord.ItemExportedRecords.Count > 0)
							{
								this.target.ExportContext.WriteResultManifest(progressRecord.ItemExportedRecords);
							}
							if (progressRecord.ItemErrorRecords != null && progressRecord.ItemErrorRecords.Count > 0)
							{
								this.target.ExportContext.WriteErrorLog(progressRecord.ItemErrorRecords);
								foreach (ErrorRecord errorRecord in progressRecord.ItemErrorRecords)
								{
									this.SearchResults.IncrementErrorItemCount(errorRecord.Item.SourceId);
								}
							}
							this.ReportStatistics(new ExportStatusEventArgs
							{
								ActualBytes = progressRecord.Size,
								ActualCount = progressRecord.ItemExportedRecords.Count + progressRecord.ItemErrorRecords.Count,
								ActualMailboxesProcessed = 0,
								ActualMailboxesTotal = 0,
								TotalDuration = progressRecord.Duration
							});
							progressRecord.DataContext.ProcessedItemCount += progressRecord.ItemExportedRecords.Count + progressRecord.ItemErrorRecords.Count;
							this.StatusManager.Checkpoint(progressRecord.DataContext.SourceId);
						}
					}
					catch (Exception ex)
					{
						Tracer.TraceError("ProgressController.WaitForAndProcessProgress: Progress reporting thread error: {0}", new object[]
						{
							ex
						});
						this.Stop();
						throw;
					}
				}
			}
		}

		private void ReportStatistics(ExportStatusEventArgs args)
		{
			EventHandler<ExportStatusEventArgs> onReportStatistics = this.OnReportStatistics;
			if (onReportStatistics != null)
			{
				onReportStatistics(this, args);
			}
		}

		private Semaphore progressAvailable;

		private ConcurrentQueue<ProgressRecord> progressQueue;

		private ITarget target;

		private SourceDataProviderManager sourceDataProviderManager;

		private bool isDocIdHintFlightingEnabled;

		private CancellationTokenSource abortTokenSourceForTasks;

		private class TargetWrapper : ITarget
		{
			internal TargetWrapper(ITarget target)
			{
				this.ExportContextInternal = new ProgressController.ExportContextWrapper(target.ExportContext);
				this.TargetInternal = target;
			}

			public IExportContext ExportContext
			{
				get
				{
					return this.ExportContextInternal;
				}
			}

			public ExportSettings ExportSettings
			{
				get
				{
					return this.TargetInternal.ExportSettings;
				}
				set
				{
					this.TargetInternal.ExportSettings = value;
				}
			}

			internal ProgressController.ExportContextWrapper ExportContextInternal { get; private set; }

			private ITarget TargetInternal { get; set; }

			public IItemIdList CreateItemIdList(string mailboxId, bool isUnsearchable)
			{
				return this.TargetInternal.CreateItemIdList(mailboxId, isUnsearchable);
			}

			public void RemoveItemIdList(string mailboxId, bool isUnsearchable)
			{
				this.TargetInternal.RemoveItemIdList(mailboxId, isUnsearchable);
			}

			public IContextualBatchDataWriter<List<ItemInformation>> CreateDataWriter(IProgressController progressController)
			{
				return this.TargetInternal.CreateDataWriter(progressController);
			}

			public void Rollback(SourceInformationCollection allSourceInformation)
			{
				this.TargetInternal.Rollback(allSourceInformation);
			}

			public IStatusLog GetStatusLog()
			{
				return this.TargetInternal.GetStatusLog();
			}

			public void CheckInitialStatus(SourceInformationCollection allSourceInformation, OperationStatus status)
			{
				this.TargetInternal.CheckInitialStatus(allSourceInformation, status);
			}
		}

		private class ExportContextWrapper : IExportContext
		{
			internal ExportContextWrapper(IExportContext exportContext)
			{
				this.ExportMetadataInternal = new ProgressController.ExportMetadataWrapper(exportContext.ExportMetadata);
				this.ExportContextInternal = exportContext;
			}

			public bool IsResume
			{
				get
				{
					return this.ExportContextInternal.IsResume;
				}
			}

			public IExportMetadata ExportMetadata
			{
				get
				{
					return this.ExportMetadataInternal;
				}
			}

			public IList<ISource> Sources
			{
				get
				{
					return this.ExportContextInternal.Sources;
				}
			}

			public ITargetLocation TargetLocation
			{
				get
				{
					return this.ExportContextInternal.TargetLocation;
				}
			}

			internal ProgressController.ExportMetadataWrapper ExportMetadataInternal { get; private set; }

			private IExportContext ExportContextInternal { get; set; }

			public void WriteResultManifest(IEnumerable<ExportRecord> records)
			{
				this.ExportContextInternal.WriteResultManifest(records);
			}

			public void WriteErrorLog(IEnumerable<ErrorRecord> errorRecords)
			{
				this.ExportContextInternal.WriteErrorLog(errorRecords);
			}
		}

		private class ExportMetadataWrapper : IExportMetadata
		{
			internal ExportMetadataWrapper(IExportMetadata exportMetadata)
			{
				this.ExportMetadataInternal = exportMetadata;
				this.IncludeUnsearchableItemsInternal = exportMetadata.IncludeUnsearchableItems;
			}

			public string ExportName
			{
				get
				{
					return this.ExportMetadataInternal.ExportName;
				}
			}

			public string ExportId
			{
				get
				{
					return this.ExportMetadataInternal.ExportId;
				}
			}

			public DateTime ExportStartTime
			{
				get
				{
					return this.ExportMetadataInternal.ExportStartTime;
				}
			}

			public bool RemoveRms
			{
				get
				{
					return this.ExportMetadataInternal.RemoveRms;
				}
			}

			public bool IncludeDuplicates
			{
				get
				{
					return this.ExportMetadataInternal.IncludeDuplicates;
				}
			}

			public bool IncludeUnsearchableItems
			{
				get
				{
					return this.IncludeUnsearchableItemsInternal;
				}
				set
				{
					this.IncludeUnsearchableItemsInternal = value;
				}
			}

			public bool IncludeSearchableItems
			{
				get
				{
					return this.ExportMetadataInternal.IncludeSearchableItems;
				}
			}

			public int EstimateItems
			{
				get
				{
					return this.ExportMetadataInternal.EstimateItems;
				}
			}

			public ulong EstimateBytes
			{
				get
				{
					return this.ExportMetadataInternal.EstimateBytes;
				}
			}

			public string Language
			{
				get
				{
					return this.ExportMetadataInternal.Language;
				}
			}

			internal bool IncludeUnsearchableItemsInternal { private get; set; }

			private IExportMetadata ExportMetadataInternal { get; set; }
		}
	}
}

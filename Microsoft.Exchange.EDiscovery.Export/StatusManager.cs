using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class StatusManager : IStatusManager, IDisposable
	{
		public StatusManager(ITarget target)
		{
			this.target = target;
			this.EnsureStatusLogValid();
		}

		public OperationStatus CurrentStatus { get; internal set; }

		public SourceInformationCollection AllSourceInformation { get; internal set; }

		public void Dispose()
		{
			this.CloseStatusLog();
		}

		public void Rollback(bool removeStatusLog)
		{
			lock (this)
			{
				this.EnsureStatusLogValid();
				Tracer.TraceInformation("StatusManager.Rollback: removeStatusLog: {0}; CurrentStatus: {1}", new object[]
				{
					removeStatusLog,
					this.CurrentStatus
				});
				if (this.CurrentStatus != OperationStatus.Rollbacking)
				{
					throw new ArgumentException("IStatusManager.Rollback(bool) should be only called when the status is Rollbacking");
				}
				this.target.Rollback(this.AllSourceInformation);
				if (removeStatusLog)
				{
					if (this.statusLog != null)
					{
						this.statusLog.Delete();
						this.statusLog.Dispose();
						this.statusLog = null;
					}
					this.CurrentStatus = OperationStatus.None;
					this.AllSourceInformation = null;
				}
				else
				{
					this.ResetStatusToDefault();
				}
			}
		}

		public bool BeginProcedure(ProcedureType procedureRequest)
		{
			bool flag = false;
			lock (this)
			{
				this.EnsureStatusLogValid();
				Tracer.TraceInformation("StatusManager.BeginProcedure: procedureRequest: {0}; CurrentStatus: {1}", new object[]
				{
					procedureRequest,
					this.CurrentStatus
				});
				bool flag3 = true;
				OperationStatus operationStatus = this.CurrentStatus;
				switch (procedureRequest)
				{
				case ProcedureType.Prepare:
				{
					OperationStatus currentStatus = this.CurrentStatus;
					if (currentStatus != OperationStatus.Pending)
					{
						switch (currentStatus)
						{
						case OperationStatus.SearchCompleted:
							operationStatus = (this.AllSourceInformation.Values.Any((SourceInformation source) => !source.Status.IsSearchCompleted(this.target.ExportContext.ExportMetadata.IncludeSearchableItems, this.target.ExportContext.ExportMetadata.IncludeUnsearchableItems)) ? OperationStatus.Searching : OperationStatus.SearchCompleted);
							goto IL_185;
						case OperationStatus.PartiallyProcessed:
							operationStatus = OperationStatus.RetrySearching;
							goto IL_185;
						case OperationStatus.Processed:
							goto IL_185;
						}
						flag3 = false;
					}
					else
					{
						operationStatus = OperationStatus.Searching;
					}
					break;
				}
				case ProcedureType.Export:
					switch (this.CurrentStatus)
					{
					case OperationStatus.SearchCompleted:
					case OperationStatus.PartiallyProcessed:
						operationStatus = OperationStatus.Processing;
						goto IL_185;
					case OperationStatus.Processed:
						goto IL_185;
					}
					flag3 = false;
					break;
				case ProcedureType.Stop:
					switch (this.CurrentStatus)
					{
					case OperationStatus.Searching:
						operationStatus = OperationStatus.Rollbacking;
						break;
					case OperationStatus.RetrySearching:
					case OperationStatus.Processing:
						operationStatus = OperationStatus.Stopping;
						break;
					}
					break;
				case ProcedureType.Rollback:
					switch (this.CurrentStatus)
					{
					case OperationStatus.Pending:
						goto IL_185;
					case OperationStatus.Searching:
					case OperationStatus.RetrySearching:
					case OperationStatus.Stopping:
					case OperationStatus.Processing:
					case OperationStatus.Rollbacking:
						flag3 = false;
						goto IL_185;
					}
					operationStatus = OperationStatus.Rollbacking;
					break;
				default:
					throw new NotSupportedException();
				}
				IL_185:
				if (!flag3)
				{
					Tracer.TraceError("StatusManager.BeginProcedure: !valid: throwing exception", new object[0]);
					throw new ExportException(ExportErrorType.OperationNotSupportedWithCurrentStatus, string.Format(CultureInfo.CurrentCulture, "Procedure '{0}' is not supported when the operation status is '{1}' while 'IsResume' flag is '{2}'", new object[]
					{
						procedureRequest,
						this.CurrentStatus,
						this.target.ExportContext.IsResume
					}));
				}
				if (this.CurrentStatus != operationStatus)
				{
					this.CurrentStatus = operationStatus;
					flag = (operationStatus != OperationStatus.Stopping);
					Tracer.TraceInformation("StatusManager.BeginProcedure: shouldContinue: {0}", new object[]
					{
						flag ? "true" : "false"
					});
					this.Checkpoint(null);
				}
			}
			return flag;
		}

		public void EndProcedure()
		{
			lock (this)
			{
				Tracer.TraceInformation("StatusManager.EndProcedure: status log empty?: {0}; CurrentStatus: {1}", new object[]
				{
					this.statusLog == null,
					this.CurrentStatus
				});
				if (this.statusLog != null)
				{
					OperationStatus operationStatus = this.CurrentStatus;
					switch (this.CurrentStatus)
					{
					case OperationStatus.None:
						throw new NotSupportedException("Bug : EndProcedure shouldn't be called when the status is None");
					case OperationStatus.Searching:
						operationStatus = OperationStatus.SearchCompleted;
						break;
					case OperationStatus.RetrySearching:
					case OperationStatus.Stopping:
						operationStatus = OperationStatus.PartiallyProcessed;
						break;
					case OperationStatus.Processing:
						operationStatus = (this.AllSourceInformation.Values.Any((SourceInformation source) => !source.Status.IsSearchCompleted(this.target.ExportContext.ExportMetadata.IncludeSearchableItems, this.target.ExportContext.ExportMetadata.IncludeUnsearchableItems) || source.Status.ItemCount > source.Status.ProcessedItemCount) ? OperationStatus.PartiallyProcessed : OperationStatus.Processed);
						break;
					case OperationStatus.Rollbacking:
						operationStatus = OperationStatus.Pending;
						break;
					}
					if (this.CurrentStatus != operationStatus)
					{
						this.CurrentStatus = operationStatus;
						this.Checkpoint(null);
					}
				}
			}
		}

		public void Checkpoint(string sourceId)
		{
			lock (this)
			{
				Tracer.TraceInformation("StatusManager.Checkpoint: sourceId: {0}", new object[]
				{
					sourceId
				});
				this.EnsureStatusLogValid();
				if (string.IsNullOrEmpty(sourceId))
				{
					this.statusLog.UpdateStatus(this.AllSourceInformation, this.CurrentStatus);
				}
				else
				{
					this.statusLog.UpdateSourceStatus(this.AllSourceInformation[sourceId], this.AllSourceInformation.GetSourceIndex(sourceId));
				}
			}
		}

		private static bool IsTransientStatus(OperationStatus status)
		{
			switch (status)
			{
			case OperationStatus.Searching:
			case OperationStatus.RetrySearching:
			case OperationStatus.Stopping:
			case OperationStatus.Processing:
			case OperationStatus.Rollbacking:
				return true;
			}
			return false;
		}

		private void RollbackToCheckpoint()
		{
		}

		private void CloseStatusLog()
		{
			Tracer.TraceInformation("StatusManager.CloseStatusLog: status log empty?: {0}", new object[]
			{
				this.statusLog == null
			});
			if (this.statusLog != null)
			{
				this.statusLog.Dispose();
				this.statusLog = null;
			}
		}

		private void EnsureStatusLogValid()
		{
			if (this.statusLog == null)
			{
				Tracer.TraceInformation("StatusManager.EnsureStatusLogValid: initializing status log in memory", new object[0]);
				try
				{
					this.statusLog = this.target.GetStatusLog();
					SourceInformationCollection sourceInformationCollection = null;
					OperationStatus currentStatus = OperationStatus.None;
					ExportSettings exportSettings = this.statusLog.LoadStatus(out sourceInformationCollection, out currentStatus);
					if (sourceInformationCollection == null)
					{
						Tracer.TraceInformation("StatusManager.EnsureStatusLogValid: creating status log since no existing log found", new object[0]);
						if (this.target.ExportContext.IsResume)
						{
							throw new ExportException(ExportErrorType.StatusNotFoundToResume, "This is a resume request of previous job. But there is no corresponding previous job found.");
						}
						this.ResetStatusToDefault();
						this.target.ExportSettings = new ExportSettings
						{
							ExportTime = this.target.ExportContext.ExportMetadata.ExportStartTime,
							IncludeDuplicates = this.target.ExportContext.ExportMetadata.IncludeDuplicates,
							IncludeSearchableItems = this.target.ExportContext.ExportMetadata.IncludeSearchableItems,
							IncludeUnsearchableItems = this.target.ExportContext.ExportMetadata.IncludeUnsearchableItems,
							RemoveRms = this.target.ExportContext.ExportMetadata.RemoveRms
						};
						this.statusLog.ResetStatusLog(this.AllSourceInformation, this.CurrentStatus, this.target.ExportSettings);
					}
					else
					{
						Tracer.TraceInformation("StatusManager.EnsureStatusLogValid: Using existing status log", new object[0]);
						if (!this.target.ExportContext.IsResume)
						{
							throw new ExportException(ExportErrorType.ExistingStatusMustBeAResumeOperation, "This operation has previous job status so it must be a resume operation this time.");
						}
						if (exportSettings == null)
						{
							throw new ExportException(ExportErrorType.CorruptedStatus, "The export settings in the status log is missing or corrupted.");
						}
						this.target.ExportSettings = exportSettings;
						bool flag = false;
						if (this.target.ExportContext.ExportMetadata.IncludeDuplicates == this.target.ExportSettings.IncludeDuplicates && this.target.ExportContext.ExportMetadata.IncludeSearchableItems == this.target.ExportSettings.IncludeSearchableItems && this.target.ExportContext.ExportMetadata.IncludeUnsearchableItems == this.target.ExportSettings.IncludeUnsearchableItems && this.target.ExportContext.ExportMetadata.RemoveRms == this.target.ExportSettings.RemoveRms && this.target.ExportContext.Sources.Count == sourceInformationCollection.Count)
						{
							using (IEnumerator<ISource> enumerator = this.target.ExportContext.Sources.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									ISource source = enumerator.Current;
									try
									{
										SourceInformation sourceInformation = sourceInformationCollection[source.Id];
										if (sourceInformation.Configuration.Name != source.Name || sourceInformation.Configuration.SourceFilter != source.SourceFilter || sourceInformation.Configuration.LegacyExchangeDN != source.LegacyExchangeDN)
										{
											Tracer.TraceInformation("StatusManager.EnsureStatusLogValid: Configuration changed for source '{0}'", new object[]
											{
												source.Id
											});
											flag = true;
											break;
										}
									}
									catch (KeyNotFoundException)
									{
										Tracer.TraceInformation("StatusManager.EnsureStatusLogValid: Configuration changed(new source detected) for source '{0}'", new object[]
										{
											source.Id
										});
										flag = true;
										break;
									}
								}
								goto IL_37E;
							}
						}
						Tracer.TraceInformation("StatusManager.EnsureStatusLogValid: Configuration changed(source count doesn't match). Original source count: {0}; New source count: {1}", new object[]
						{
							sourceInformationCollection.Count,
							this.target.ExportContext.Sources.Count
						});
						flag = true;
						IL_37E:
						if (flag)
						{
							throw new ExportException(ExportErrorType.CannotResumeWithConfigurationChange);
						}
						this.AllSourceInformation = sourceInformationCollection;
						this.CurrentStatus = currentStatus;
						if (StatusManager.IsTransientStatus(this.CurrentStatus))
						{
							Tracer.TraceInformation("StatusManager.EnsureStatusLogValid: Recovering on status: {0}", new object[]
							{
								this.CurrentStatus
							});
							this.Recover();
						}
					}
					this.target.CheckInitialStatus(this.AllSourceInformation, this.CurrentStatus);
				}
				catch (Exception ex)
				{
					Tracer.TraceError("StatusManager.EnsureStatusLogValid: Exception happend, closing status log. Exception: {0}", new object[]
					{
						ex
					});
					this.CloseStatusLog();
					throw;
				}
			}
		}

		private void ResetStatusToDefault()
		{
			Tracer.TraceInformation("StatusManager.ResetStatusToDefault", new object[0]);
			this.AllSourceInformation = new SourceInformationCollection(this.target.ExportContext.Sources.Count);
			foreach (ISource source in this.target.ExportContext.Sources)
			{
				SourceInformation value = new SourceInformation(source.Name, source.Id, source.SourceFilter, source.ServiceEndpoint, source.LegacyExchangeDN);
				this.AllSourceInformation[source.Id] = value;
			}
			this.CurrentStatus = OperationStatus.Pending;
		}

		private void Recover()
		{
			switch (this.CurrentStatus)
			{
			case OperationStatus.Searching:
			case OperationStatus.Rollbacking:
				this.CurrentStatus = OperationStatus.Rollbacking;
				this.Rollback(false);
				this.CurrentStatus = OperationStatus.Pending;
				this.Checkpoint(null);
				return;
			case OperationStatus.RetrySearching:
			case OperationStatus.Stopping:
			case OperationStatus.Processing:
				this.RollbackToCheckpoint();
				this.CurrentStatus = OperationStatus.PartiallyProcessed;
				this.Checkpoint(null);
				break;
			case OperationStatus.SearchCompleted:
			case OperationStatus.PartiallyProcessed:
			case OperationStatus.Processed:
				break;
			default:
				return;
			}
		}

		private ITarget target;

		private IStatusLog statusLog;
	}
}

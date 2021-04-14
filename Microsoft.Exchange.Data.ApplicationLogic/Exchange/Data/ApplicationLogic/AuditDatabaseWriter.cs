using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.LogUploader;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Office.Compliance.Audit;
using Microsoft.Office.Compliance.Audit.Schema;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AuditDatabaseWriter : DatabaseWriter<AuditLogDataBatch>
	{
		public AuditDatabaseWriter(ThreadSafeQueue<AuditLogDataBatch> queue, int id, ConfigInstance config, string logPrefix, Trace tracer) : base(queue, id, config, logPrefix)
		{
			this.Tracer = tracer;
			AuditHealthInfo.RegisterComponent<AuditDatabaseWriterHealth>();
			this.visitor = new AuditRecordDatabaseWriterVisitor(this.Tracer);
		}

		protected override bool WriteBatchDataToDataStore(AuditLogDataBatch batch)
		{
			AuditDatabaseWriterHealth instance = Singleton<AuditDatabaseWriterHealth>.Instance;
			List<AuditRecord> records = batch.GetRecords();
			foreach (AuditRecord record in records)
			{
				if (base.CheckServiceStopRequest("WriteBatchDataToDataStore()"))
				{
					return false;
				}
				instance.RecordCount++;
				if (!this.Write(record))
				{
					instance.BatchRetryCount++;
					return false;
				}
			}
			return true;
		}

		private bool Write(AuditRecord record)
		{
			Exception exception = null;
			bool retry = false;
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					try
					{
						record.Visit(this.visitor);
						exception = null;
						retry = false;
					}
					catch (ServerInMMException exception2)
					{
						exception = exception2;
						retry = true;
					}
					catch (TenantAccessBlockedException exception3)
					{
						exception = exception3;
						retry = false;
					}
					catch (DataSourceOperationException exception4)
					{
						exception = exception4;
						retry = true;
					}
					catch (ADTransientException exception5)
					{
						exception = exception5;
						retry = true;
					}
					catch (AuditException exception6)
					{
						exception = exception6;
						retry = false;
					}
					catch (AuditLogServiceException ex)
					{
						exception = ex;
						retry = false;
						ResponseCodeType responseCodeType;
						Enum.TryParse<ResponseCodeType>(ex.Code, true, out responseCodeType);
						ResponseCodeType responseCodeType2 = responseCodeType;
						if (responseCodeType2 <= ResponseCodeType.ErrorNotEnoughMemory)
						{
							if (responseCodeType2 <= ResponseCodeType.ErrorClientDisconnected)
							{
								if (responseCodeType2 != ResponseCodeType.ErrorADUnavailable && responseCodeType2 != ResponseCodeType.ErrorBatchProcessingStopped && responseCodeType2 != ResponseCodeType.ErrorClientDisconnected)
								{
									goto IL_185;
								}
							}
							else if (responseCodeType2 <= ResponseCodeType.ErrorInternalServerTransientError)
							{
								if (responseCodeType2 != ResponseCodeType.ErrorConnectionFailed)
								{
									switch (responseCodeType2)
									{
									case ResponseCodeType.ErrorInsufficientResources:
									case ResponseCodeType.ErrorInternalServerTransientError:
										break;
									case ResponseCodeType.ErrorInternalServerError:
										goto IL_185;
									default:
										goto IL_185;
									}
								}
							}
							else
							{
								switch (responseCodeType2)
								{
								case ResponseCodeType.ErrorMailboxMoveInProgress:
								case ResponseCodeType.ErrorMailboxStoreUnavailable:
									break;
								default:
									if (responseCodeType2 != ResponseCodeType.ErrorNotEnoughMemory)
									{
										goto IL_185;
									}
									break;
								}
							}
						}
						else if (responseCodeType2 <= ResponseCodeType.ErrorTimeoutExpired)
						{
							if (responseCodeType2 != ResponseCodeType.ErrorRequestAborted && responseCodeType2 != ResponseCodeType.ErrorServerBusy && responseCodeType2 != ResponseCodeType.ErrorTimeoutExpired)
							{
								goto IL_185;
							}
						}
						else if (responseCodeType2 <= ResponseCodeType.ErrorMailboxFailover)
						{
							if (responseCodeType2 != ResponseCodeType.ErrorTooManyObjectsOpened && responseCodeType2 != ResponseCodeType.ErrorMailboxFailover)
							{
								goto IL_185;
							}
						}
						else if (responseCodeType2 != ResponseCodeType.ErrorUMServerUnavailable && responseCodeType2 != ResponseCodeType.ErrorLocationServicesRequestTimedOut)
						{
							goto IL_185;
						}
						retry = true;
						IL_185:;
					}
					catch (AuditLogException ex2)
					{
						exception = ex2;
						WebException ex3 = ex2.InnerException as WebException;
						if (ex3 != null)
						{
							WebExceptionStatus status = ex3.Status;
							if (status == WebExceptionStatus.Timeout)
							{
								retry = true;
							}
							else
							{
								retry = false;
							}
						}
						else
						{
							retry = false;
						}
					}
					if (exception != null && this.Tracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						this.Tracer.TraceDebug((long)this.GetHashCode(), "AuditDatabaseWriter.Write. Failed while processing audit record: Id={0}, OrgId={1}, Operation={2}, Type={3}. Error: {4}", new object[]
						{
							record.Id,
							record.OrganizationId,
							record.Operation,
							record.RecordType,
							exception
						});
					}
				});
			}
			catch (GrayException exception)
			{
				GrayException exception7;
				exception = exception7;
				retry = false;
				if (exception != null && this.Tracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					this.Tracer.TraceError((long)this.GetHashCode(), "AuditDatabaseWriter.Write. Failed while processing audit record: Id={0}, OrgId={1}, Operation={2}, Type={3}. Error: {4}", new object[]
					{
						record.Id,
						record.OrganizationId,
						record.Operation,
						record.RecordType,
						exception
					});
				}
			}
			finally
			{
				if (exception != null)
				{
					AuditHealthInfo.Instance.AddException(exception);
					AuditDatabaseWriterHealth instance = Singleton<AuditDatabaseWriterHealth>.Instance;
					instance.Add(new RecordProcessingResult(record, exception, retry));
					if (!retry)
					{
						instance.FailedBatchCount++;
					}
				}
			}
			return !retry;
		}

		private readonly Trace Tracer;

		private readonly AuditRecordDatabaseWriterVisitor visitor;
	}
}

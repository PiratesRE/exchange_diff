using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.LogSearch;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class RpcLogReader : ILogReader
	{
		public string Server
		{
			get
			{
				return this.server;
			}
		}

		public ServerVersion ServerVersion
		{
			get
			{
				return this.version;
			}
		}

		public MtrSchemaVersion MtrSchemaVersion
		{
			get
			{
				return VersionConverter.GetMtrSchemaVersion(this.version);
			}
		}

		public static ILogReader GetLogReader(string server, DirectoryContext context)
		{
			ServerInfo serverInfo = ServerCache.Instance.FindMailboxOrHubServer(server, 34UL);
			if (!serverInfo.IsSearchable)
			{
				return null;
			}
			return new RpcLogReader(serverInfo.Key, serverInfo.AdminDisplayVersion, context);
		}

		private RpcLogReader(string server, ServerVersion version, DirectoryContext context)
		{
			this.server = server;
			this.version = version;
			this.context = context;
		}

		public List<MessageTrackingLogEntry> ReadLogs(RpcReason rpcReason, string logFilePrefix, string messageId, DateTime startTime, DateTime endTime, TrackingEventBudget eventBudget)
		{
			return this.ReadLogs(rpcReason, logFilePrefix, null, messageId, startTime, endTime, eventBudget);
		}

		public List<MessageTrackingLogEntry> ReadLogs(RpcReason rpcReason, string logFilePrefix, ProxyAddressCollection senderProxyAddresses, DateTime startTime, DateTime endTime, TrackingEventBudget eventBudget)
		{
			return this.ReadLogs(rpcReason, logFilePrefix, senderProxyAddresses, null, startTime, endTime, eventBudget);
		}

		private List<MessageTrackingLogEntry> ReadLogs(RpcReason rpcReason, string logFilePrefix, ProxyAddressCollection senderProxyAddresses, string messageId, DateTime startTime, DateTime endTime, TrackingEventBudget eventBudget)
		{
			Exception ex = null;
			int num = 0;
			List<MessageTrackingLogEntry> list = null;
			bool flag = !string.IsNullOrEmpty(messageId);
			bool flag2 = senderProxyAddresses != null;
			if (flag && flag2)
			{
				throw new InvalidOperationException("Cannot get logs with both message id and sender address criteria");
			}
			if (rpcReason == RpcReason.None)
			{
				InfoWorkerMessageTrackingPerformanceCounters.GetMessageTrackingReportQueries.Increment();
			}
			else
			{
				InfoWorkerMessageTrackingPerformanceCounters.SearchMessageTrackingReportQueries.Increment();
			}
			try
			{
				this.context.DiagnosticsContext.LogRpcStart(this.server, rpcReason);
				using (LogSearchCursor cursor = TrackingSearch.GetCursor(this.server, this.version, logFilePrefix, messageId, senderProxyAddresses, startTime, endTime))
				{
					list = this.ReadLogsFromCursor(cursor, eventBudget);
				}
			}
			catch (LogSearchException ex2)
			{
				ex = ex2;
				num = ex2.ErrorCode;
			}
			catch (RpcException ex3)
			{
				ex = ex3;
				num = ex3.ErrorCode;
			}
			finally
			{
				this.context.DiagnosticsContext.LogRpcEnd(ex, (list == null) ? 0 : list.Count);
			}
			if (ex != null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Error reading from log-search RPC server for {0}: {1}, ErrorCode: {2}, Exception: {3}", new object[]
				{
					flag ? "message id" : "sender",
					flag ? messageId : senderProxyAddresses[0].ToString(),
					num,
					ex
				});
				TrackingTransientException.AddAndRaiseETX(this.context.Errors, ErrorCode.LogSearchConnection, this.server, ex.ToString());
			}
			return list;
		}

		private List<MessageTrackingLogEntry> ReadLogsFromCursor(LogSearchCursor cursor, TrackingEventBudget eventBudget)
		{
			List<MessageTrackingLogEntry> list = new List<MessageTrackingLogEntry>();
			while (cursor.MoveNext())
			{
				MessageTrackingLogEntry messageTrackingLogEntry;
				if (MessageTrackingLogEntry.TryCreateFromCursor(cursor, this.server, this.context.Errors, out messageTrackingLogEntry))
				{
					if (list.Count % ServerCache.Instance.RowsBeforeTimeBudgetCheck == 0)
					{
						eventBudget.TestDelayOperation("GetRpcsBeforeDelay");
						eventBudget.CheckTimeBudget();
					}
					list.Add(messageTrackingLogEntry);
					if (messageTrackingLogEntry.RecipientAddresses != null)
					{
						eventBudget.IncrementBy((uint)messageTrackingLogEntry.RecipientAddresses.Length);
					}
					else
					{
						eventBudget.IncrementBy(1U);
					}
				}
			}
			return list;
		}

		private string server;

		private DirectoryContext context;

		private ServerVersion version;
	}
}

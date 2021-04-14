using System;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.LogSearch;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.LogSearch;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	internal class LogSearchStream : Stream
	{
		public LogSearchStream(string server, ServerVersion version, string logName, LogQuery query, IProgressReport progressReport)
		{
			ExTraceGlobals.CommonTracer.TraceDebug<string, string>((long)this.GetHashCode(), "MsExchangeLogSearch construct LogSearchStream with server name {0} and log name {1}", server, logName);
			this.buffer = new byte[32768];
			this.progressReport = progressReport;
			this.server = server;
			this.serverVersion = version;
			this.logName = logName;
			this.query = query;
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override int Read(byte[] dest, int offset, int count)
		{
			int i = offset;
			int num = offset + count;
			if (this.client == null)
			{
				ExTraceGlobals.CommonTracer.TraceDebug((long)this.GetHashCode(), "MsExchangeLogSearch LogSearchStream Read create new LogSearchClient");
				this.client = new LogSearchClient(this.server, this.serverVersion);
				this.bufferEnd = this.client.Search(this.logName, this.query, true, this.buffer, out this.sessionId, out this.more, out this.progress);
			}
			while (i < num)
			{
				if (this.bufferPos < this.bufferEnd)
				{
					int num2 = Math.Min(num - i, this.bufferEnd - this.bufferPos);
					Buffer.BlockCopy(this.buffer, this.bufferPos, dest, i, num2);
					this.bufferPos += num2;
					i += num2;
					return i - offset;
				}
				if (this.more)
				{
					this.bufferPos = 0;
					bool flag = false;
					try
					{
						int num3;
						this.bufferEnd = this.client.Continue(this.sessionId, true, this.buffer, out this.more, out num3);
						flag = true;
						if (num3 > this.progress)
						{
							this.progress = num3;
							if (this.progressReport != null)
							{
								this.progressReport.Report(this.progress);
							}
						}
						continue;
					}
					finally
					{
						if (!flag)
						{
							ExTraceGlobals.CommonTracer.TraceError((long)this.GetHashCode(), "MsExchangeLogSearch LogSearchStream Read client continue failed");
							this.more = false;
						}
					}
				}
				return i - offset;
			}
			return i - offset;
		}

		public override void Write(byte[] src, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long length)
		{
			throw new NotSupportedException();
		}

		public override void Flush()
		{
		}

		public void Cancel()
		{
			ExTraceGlobals.CommonTracer.TraceDebug((long)this.GetHashCode(), "MsExchangeLogSearch logSearchStream Cancel");
			lock (this.sync)
			{
				if (this.client != null)
				{
					try
					{
						this.client.Cancel(this.sessionId);
					}
					catch (LogSearchException ex)
					{
						ExTraceGlobals.CommonTracer.TraceError<int>((long)this.GetHashCode(), "MsExchangeLogSearch logSearchStream Cancel client cancel failed with LogSearchException. The error code is {0}", ex.ErrorCode);
					}
					catch (RpcException ex2)
					{
						ExTraceGlobals.CommonTracer.TraceError<string>((long)this.GetHashCode(), "MsExchangeLogSearch logSearchStream Cancel client cancel faied with RpcException. The error message is {0}", ex2.Message);
					}
					Monitor.Wait(this.sync);
				}
			}
		}

		public override void Close()
		{
			ExTraceGlobals.CommonTracer.TraceDebug((long)this.GetHashCode(), "MsExchangeLogSearch logSearchStream Close");
			lock (this.sync)
			{
				if (this.client != null)
				{
					if (this.more)
					{
						try
						{
							this.client.Cancel(this.sessionId);
						}
						catch (LogSearchException ex)
						{
							ExTraceGlobals.CommonTracer.TraceError<int>((long)this.GetHashCode(), "MsExchangeLogSearch logSearchStream Close client cancel failed with LogSearchException. The error code is {0}", ex.ErrorCode);
						}
						catch (RpcException ex2)
						{
							ExTraceGlobals.CommonTracer.TraceError<string>((long)this.GetHashCode(), "MsExchangeLogSearch logSearchStream Close client cancel faied with RpcException. The error message is {0}", ex2.Message);
						}
						this.more = false;
					}
					((IDisposable)this.client).Dispose();
					this.client = null;
					Monitor.PulseAll(this.sync);
				}
			}
		}

		private byte[] buffer;

		private int bufferPos;

		private int bufferEnd;

		private LogSearchClient client;

		private Guid sessionId;

		private bool more;

		private int progress;

		private object sync = new object();

		private IProgressReport progressReport;

		private string server;

		private ServerVersion serverVersion;

		private string logName;

		private LogQuery query;
	}
}

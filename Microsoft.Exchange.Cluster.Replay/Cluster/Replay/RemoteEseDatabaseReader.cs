using System;
using System.Globalization;
using System.Security.Permissions;
using System.Threading;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.EseRepl;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	internal class RemoteEseDatabaseReader : IEseDatabaseReader, IDisposeTrackable, IDisposable
	{
		public RemoteEseDatabaseReader(string serverName, Guid databaseGuid, string databaseName, string databasePath)
		{
			this.m_disposeTracker = this.GetDisposeTracker();
			this.m_serverName = serverName;
			this.m_pageSize = 425984807L;
			this.m_databaseGuid = databaseGuid;
			this.m_databaseName = databaseName;
			this.m_databasePath = databasePath;
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RemoteEseDatabaseReader>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.m_disposeTracker != null)
			{
				this.m_disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			if (!this.m_isDisposed)
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		public void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!this.m_isDisposed)
				{
					if (disposing)
					{
						if (this.m_disposeTracker != null)
						{
							this.m_disposeTracker.Dispose();
						}
						this.CloseChannel();
					}
					this.m_isDisposed = true;
				}
			}
		}

		public void ForceNewLog()
		{
			ExTraceGlobals.IncrementalReseederTracer.TraceDebug((long)this.GetHashCode(), "rolling a log file");
			Exception ex = null;
			try
			{
				this.GetChannel();
				SeedPageReaderRollLogFileRequest seedPageReaderRollLogFileRequest = new SeedPageReaderRollLogFileRequest(this.m_channel, this.m_databaseGuid, this.m_databaseName, this.m_databasePath);
				seedPageReaderRollLogFileRequest.Send();
				NetworkChannelMessage message = this.m_channel.GetMessage();
				if (!(message is SeedPageReaderRollLogFileReply))
				{
					this.m_channel.ThrowUnexpectedMessage(message);
				}
			}
			catch (NetworkRemoteException ex2)
			{
				ex = ex2;
			}
			catch (NetworkTransportException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ReplayEventLogConstants.Tuple_ForceNewLogError.LogEvent(string.Empty, new object[]
				{
					this.m_databaseName,
					this.m_serverName,
					ex
				});
				throw ex;
			}
		}

		public long ReadPageSize()
		{
			this.m_pageSize = 3L;
			ExTraceGlobals.IncrementalReseederTracer.TraceDebug((long)this.GetHashCode(), "fetching the page size");
			Exception ex = null;
			try
			{
				this.GetChannel();
				SeedPageReaderPageSizeRequest seedPageReaderPageSizeRequest = new SeedPageReaderPageSizeRequest(this.m_channel, this.m_databaseGuid, this.m_databaseName, this.m_databasePath);
				seedPageReaderPageSizeRequest.Send();
				NetworkChannelMessage message = this.m_channel.GetMessage();
				SeedPageReaderPageSizeReply seedPageReaderPageSizeReply = message as SeedPageReaderPageSizeReply;
				if (seedPageReaderPageSizeReply == null)
				{
					this.m_channel.ThrowUnexpectedMessage(message);
				}
				this.m_pageSize = seedPageReaderPageSizeReply.PageSize;
			}
			catch (NetworkRemoteException ex2)
			{
				ex = ex2;
			}
			catch (NetworkTransportException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ReplayEventLogConstants.Tuple_ReadPageSizeError.LogEvent(string.Empty, new object[]
				{
					this.m_databaseName,
					this.m_serverName,
					ex
				});
				throw ex;
			}
			return this.m_pageSize;
		}

		public byte[] ReadOnePage(long pageNumber, out long lowGen, out long highGen)
		{
			byte[] result = null;
			if (pageNumber < 1L)
			{
				throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, "pageNumber is {0}, must be >= 1 ", new object[]
				{
					pageNumber
				}));
			}
			lowGen = (long)((ulong)-1);
			highGen = 0L;
			try
			{
				this.GetChannel();
				SeedPageReaderSinglePageRequest seedPageReaderSinglePageRequest = new SeedPageReaderSinglePageRequest(this.m_channel, this.m_databaseGuid, this.m_databaseName, this.m_databasePath, (uint)pageNumber);
				seedPageReaderSinglePageRequest.Send();
				NetworkChannelMessage message = this.m_channel.GetMessage();
				SeedPageReaderSinglePageReply seedPageReaderSinglePageReply = message as SeedPageReaderSinglePageReply;
				if (seedPageReaderSinglePageReply == null)
				{
					this.m_channel.ThrowUnexpectedMessage(message);
				}
				this.m_pageSize = (long)seedPageReaderSinglePageReply.PageBytes.Length;
				lowGen = seedPageReaderSinglePageReply.LowGeneration;
				highGen = seedPageReaderSinglePageReply.HighGeneration;
				result = seedPageReaderSinglePageReply.PageBytes;
			}
			catch (NetworkRemoteException ex)
			{
				ReplayEventLogConstants.Tuple_ReadOnePageError.LogEvent(string.Empty, new object[]
				{
					pageNumber,
					this.m_databaseName,
					this.m_serverName,
					ex
				});
				throw;
			}
			catch (NetworkTransportException ex2)
			{
				ReplayEventLogConstants.Tuple_ReadOnePageError.LogEvent(string.Empty, new object[]
				{
					pageNumber,
					this.m_databaseName,
					this.m_serverName,
					ex2
				});
				throw;
			}
			return result;
		}

		public void StartSendingPages(long[] pageList)
		{
			DiagCore.RetailAssert(this.m_pageList == null, "single use only", new object[0]);
			this.m_pageList = pageList;
			this.GetChannel();
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.SendPageNumbersToSource));
		}

		private void SendPageNumbersToSource(object dummy)
		{
			Exception ex = null;
			IOBuffer iobuffer = null;
			int num = this.m_pageList.Length;
			try
			{
				this.GetChannel();
				SeedPageReaderMultiplePageRequest seedPageReaderMultiplePageRequest = new SeedPageReaderMultiplePageRequest(this.m_channel, this.m_databaseGuid, this.m_databaseName, this.m_databasePath, (long)this.m_pageList.Length);
				seedPageReaderMultiplePageRequest.Send();
				iobuffer = IOBufferPool.Allocate();
				iobuffer.AppendOffset = 0;
				foreach (long num2 in this.m_pageList)
				{
					if (iobuffer.RemainingSpace >= 4)
					{
						DiagCore.RetailAssert(num2 <= 2147483647L, "ese uses 31bit page numbers but {0} was passed", new object[]
						{
							num2
						});
						ExBitConverter.Write((uint)num2, iobuffer.Buffer, iobuffer.AppendOffset);
						iobuffer.AppendOffset += 4;
					}
					else
					{
						this.m_channel.Write(iobuffer.Buffer, 0, iobuffer.AppendOffset);
						iobuffer.AppendOffset = 0;
					}
				}
				if (iobuffer.AppendOffset > 0)
				{
					this.m_channel.Write(iobuffer.Buffer, 0, iobuffer.AppendOffset);
				}
			}
			catch (NetworkRemoteException ex2)
			{
				ex = ex2;
			}
			catch (NetworkTransportException ex3)
			{
				ex = ex3;
			}
			finally
			{
				this.m_pageList = null;
				if (iobuffer != null)
				{
					IOBufferPool.Free(iobuffer);
				}
			}
			if (ex != null)
			{
				ReplayCrimsonEvents.SendPageNumbersToSourceFailed.Log<Guid, string, int, Exception>(this.m_databaseGuid, this.DatabaseName, num, ex);
			}
		}

		public byte[] ReadNextPage(long expectedPageNum, out long lowGen, out long highGen)
		{
			if (expectedPageNum < 1L)
			{
				throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, "pageNumber is {0}, must be >= 1 ", new object[]
				{
					expectedPageNum
				}));
			}
			lowGen = (long)((ulong)-1);
			highGen = 0L;
			NetworkChannelMessage message = this.m_channel.GetMessage();
			SeedPageReaderSinglePageReply seedPageReaderSinglePageReply = message as SeedPageReaderSinglePageReply;
			if (seedPageReaderSinglePageReply == null)
			{
				this.m_channel.ThrowUnexpectedMessage(message);
			}
			if (seedPageReaderSinglePageReply.PageNumber != expectedPageNum)
			{
				this.m_channel.ThrowUnexpectedMessage(message);
			}
			lowGen = seedPageReaderSinglePageReply.LowGeneration;
			highGen = seedPageReaderSinglePageReply.HighGeneration;
			return seedPageReaderSinglePageReply.PageBytes;
		}

		private void GetChannel()
		{
			if (this.m_channel == null)
			{
				NetworkPath netPath = NetworkManager.ChooseNetworkPath(this.m_serverName, null, NetworkPath.ConnectionPurpose.Seeding);
				this.m_channel = NetworkChannel.Connect(netPath, TcpChannel.GetDefaultTimeoutInMs(), false);
			}
		}

		private void CloseChannel()
		{
			if (this.m_channel != null)
			{
				this.m_channel.Close();
				this.m_channel = null;
			}
		}

		public long PageSize
		{
			get
			{
				return this.m_pageSize;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.m_databaseName;
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.m_databaseGuid;
			}
		}

		private DisposeTracker m_disposeTracker;

		private bool m_isDisposed;

		private NetworkChannel m_channel;

		private string m_serverName;

		private Guid m_databaseGuid;

		private long m_pageSize;

		private string m_databaseName;

		private string m_databasePath;

		private long[] m_pageList;
	}
}

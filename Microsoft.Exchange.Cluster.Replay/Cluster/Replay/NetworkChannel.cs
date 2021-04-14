using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.EseRepl;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class NetworkChannel
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.NetworkChannelTracer;
			}
		}

		public static NetworkChannel.DataEncodingScheme VerifyDataEncoding(NetworkChannel.DataEncodingScheme requestedEncoding)
		{
			if (requestedEncoding >= NetworkChannel.DataEncodingScheme.LastIndex)
			{
				requestedEncoding = NetworkChannel.DataEncodingScheme.Uncompressed;
			}
			return requestedEncoding;
		}

		public static Exception RunNetworkFunction(Action op)
		{
			Exception result = null;
			try
			{
				op();
			}
			catch (IOException ex)
			{
				result = ex;
			}
			catch (SocketException ex2)
			{
				result = ex2;
			}
			catch (NetworkTransportException ex3)
			{
				result = ex3;
			}
			catch (ObjectDisposedException ex4)
			{
				result = ex4;
			}
			return result;
		}

		private static void ServiceCallback(object context, int bytesAvailable, bool completionIsSynchronous, Exception e)
		{
			NetworkChannel networkChannel = (NetworkChannel)context;
			networkChannel.TcpChannel.ClearIdle();
			if (bytesAvailable > 0 && e == null)
			{
				networkChannel.ServiceRequest();
				return;
			}
			if (e == null)
			{
				NetworkChannel.Tracer.TraceDebug((long)networkChannel.GetHashCode(), "ServiceCallback: Client closed the channel");
			}
			else
			{
				NetworkChannel.Tracer.TraceError<Exception>((long)networkChannel.GetHashCode(), "ServiceCallback: Channel exception: {0}", e);
			}
			networkChannel.Close();
		}

		internal NetworkPackagingLayer PackagingLayer
		{
			get
			{
				return this.m_transport;
			}
		}

		internal TcpChannel TcpChannel
		{
			get
			{
				return this.m_channel;
			}
		}

		internal bool IsClosed
		{
			get
			{
				return this.m_isClosed;
			}
		}

		internal bool IsAborted
		{
			get
			{
				return this.m_isAborted;
			}
		}

		internal bool IsCompressionEnabled
		{
			get
			{
				return this.m_transport.Encoding != NetworkChannel.DataEncodingScheme.Uncompressed;
			}
		}

		internal bool IsEncryptionEnabled
		{
			get
			{
				return this.TcpChannel.IsEncrypted;
			}
		}

		internal string PartnerNodeName
		{
			get
			{
				return this.m_channel.PartnerNodeName;
			}
		}

		public string LocalNodeName { get; set; }

		internal NetworkPath NetworkPath
		{
			get
			{
				return this.m_networkPath;
			}
		}

		internal bool KeepAlive
		{
			get
			{
				return this.m_keepAlive;
			}
			set
			{
				this.m_keepAlive = value;
			}
		}

		internal MonitoredDatabase MonitoredDatabase
		{
			get
			{
				return this.m_sourceDatabase;
			}
			set
			{
				this.m_sourceDatabase = value;
			}
		}

		internal SeederPageReaderServerContext SeederPageReaderServerContext
		{
			get
			{
				return this.m_seederPageReaderServerContext;
			}
			set
			{
				this.m_seederPageReaderServerContext = value;
			}
		}

		public ExchangeNetworkPerfmonCounters PerfCounters
		{
			get
			{
				return this.m_networkPerfCounters;
			}
			protected set
			{
				this.m_networkPerfCounters = value;
			}
		}

		internal string RemoteEndPointString
		{
			get
			{
				return this.m_remoteEndPointString;
			}
		}

		internal string LocalEndPointString
		{
			get
			{
				return this.m_localEndPointString;
			}
		}

		public string NetworkName
		{
			get
			{
				return this.m_networkName;
			}
		}

		public LogChecksummer LogChecksummer { get; private set; }

		public void SetupLogChecksummer(string basename)
		{
			lock (this)
			{
				if (this.LogChecksummer == null)
				{
					this.LogChecksummer = new LogChecksummer(basename);
				}
			}
		}

		internal NetworkChannel(TcpClientChannel ch, NetworkPath path)
		{
			this.Init(ch, path);
		}

		protected NetworkChannel(TcpServerChannel ch)
		{
			this.Init(ch, null);
			this.m_networkPerfCounters = ch.PerfCounters;
			this.m_networkName = ch.NetworkName;
		}

		protected void Init(TcpChannel ch, NetworkPath path)
		{
			this.NetworkChannelManagesAsyncReads = true;
			this.m_channel = ch;
			this.m_networkPath = path;
			this.m_transport = new NetworkPackagingLayer(this, ch);
			this.m_remoteEndPointString = this.m_channel.RemoteEndpoint.ToString();
			this.m_localEndPointString = this.m_channel.LocalEndpoint.ToString();
		}

		internal static NetworkChannel Connect(NetworkPath netPath, int timeoutInMsec, bool ignoreNodeDown)
		{
			TcpClientChannel tcpChannel = NetworkManager.OpenConnection(ref netPath, timeoutInMsec, ignoreNodeDown);
			return NetworkChannel.FinishConnect(tcpChannel, netPath, false);
		}

		private static NetworkChannel FinishConnect(TcpClientChannel tcpChannel, NetworkPath netPath, bool suppressTransparentCompression)
		{
			bool flag = false;
			NetworkChannel networkChannel = null;
			try
			{
				networkChannel = new NetworkChannel(tcpChannel, netPath);
				if (netPath.Purpose != NetworkPath.ConnectionPurpose.TestHealth)
				{
					if (netPath.Purpose == NetworkPath.ConnectionPurpose.Seeding)
					{
						networkChannel.IsSeeding = true;
					}
					networkChannel.m_networkPerfCounters = NetworkManager.GetPerfCounters(netPath.NetworkName);
					if (netPath.Compress && !suppressTransparentCompression)
					{
						networkChannel.NegotiateCompression();
					}
				}
				networkChannel.m_networkName = netPath.NetworkName;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					if (networkChannel != null)
					{
						networkChannel.Close();
						networkChannel = null;
					}
					else if (tcpChannel != null)
					{
						tcpChannel.Close();
					}
				}
			}
			return networkChannel;
		}

		public static NetworkChannel OpenChannel(string targetServerName, ISimpleBufferPool socketStreamBufferPool, IPool<SocketStreamAsyncArgs> socketStreamAsyncArgPool, SocketStream.ISocketStreamPerfCounters perfCtrs, bool suppressTransparentCompression)
		{
			if (socketStreamAsyncArgPool != null ^ socketStreamBufferPool != null)
			{
				string message = "SocketStream use requires both pools or neither";
				throw new ArgumentException(message);
			}
			ITcpConnector tcpConnector = Dependencies.TcpConnector;
			NetworkPath netPath = null;
			TcpClientChannel tcpChannel = tcpConnector.OpenChannel(targetServerName, socketStreamBufferPool, socketStreamAsyncArgPool, perfCtrs, out netPath);
			return NetworkChannel.FinishConnect(tcpChannel, netPath, suppressTransparentCompression);
		}

		internal void NegotiateCompression()
		{
			if (NetworkChannel.disableCompressionDueToFatalError)
			{
				return;
			}
			AmServerName serverName = new AmServerName(this.PartnerNodeName);
			Exception ex;
			IADServer miniServer = AmBestCopySelectionHelper.GetMiniServer(serverName, out ex);
			if (miniServer == null)
			{
				return;
			}
			if (ServerVersion.Compare(NetworkChannel.FirstVersionSupportingCoconet, miniServer.AdminDisplayVersion) > 0)
			{
				return;
			}
			CompressionConfig compressionConfig = ConfigStore.ReadCompressionConfig(out ex);
			if (compressionConfig.Provider == CompressionConfig.CompressionProvider.None)
			{
				return;
			}
			string text = SerializationUtil.ObjectToXml(compressionConfig);
			NetworkChannelCompressionConfigMsg networkChannelCompressionConfigMsg = new NetworkChannelCompressionConfigMsg(this, NetworkChannelCompressionConfigMsg.MessagePurpose.RequestEncoding, text);
			networkChannelCompressionConfigMsg.Send();
			NetworkChannelMessage message = this.GetMessage();
			NetworkChannelCompressionConfigMsg networkChannelCompressionConfigMsg2 = message as NetworkChannelCompressionConfigMsg;
			if (networkChannelCompressionConfigMsg2 == null)
			{
				this.ThrowUnexpectedMessage(message);
			}
			CompressionConfig encoding = CompressionConfig.Deserialize(networkChannelCompressionConfigMsg2.ConfigXml, out ex);
			if (ex != null)
			{
				ReplayCrimsonEvents.NegotiateCompressionFailure.Log<string, string, Exception>(this.PartnerNodeName, text, ex);
				NetworkChannel.disableCompressionDueToFatalError = true;
				return;
			}
			this.SetEncoding(encoding);
		}

		internal void SetEncoding(NetworkChannel.DataEncodingScheme scheme)
		{
			this.m_transport.Encoding = scheme;
			ExTraceGlobals.NetworkChannelTracer.TraceDebug<NetworkChannel.DataEncodingScheme>((long)this.GetHashCode(), "Compression selected: {0}", scheme);
		}

		internal void SetEncoding(CompressionConfig cfg)
		{
			this.m_transport.SetEncoding(cfg);
		}

		public bool ChecksumDataTransfer
		{
			get
			{
				return RegistryParameters.EnableNetworkChecksums != 0;
			}
		}

		internal static void ServiceRequests(TcpServerChannel tcpChannel, TcpListener listener)
		{
			NetworkChannel networkChannel = new NetworkChannel(tcpChannel);
			networkChannel.m_listener = listener;
			networkChannel.LocalNodeName = listener.ListenerConfig.LocalNodeName;
			listener.AddActiveChannel(networkChannel);
			networkChannel.ServiceRequest();
		}

		public bool NetworkChannelManagesAsyncReads { get; set; }

		private void ServiceRequest()
		{
			bool flag = false;
			Exception ex = null;
			try
			{
				if (this.m_listener.ListenerConfig.KeepServerOpenByDefault)
				{
					this.KeepAlive = true;
				}
				NetworkChannelMessage message = this.GetMessage();
				INetworkChannelRequest networkChannelRequest = message as INetworkChannelRequest;
				if (networkChannelRequest == null)
				{
					this.ThrowUnexpectedMessage(message);
				}
				NetworkChannelDatabaseRequest networkChannelDatabaseRequest = message as NetworkChannelDatabaseRequest;
				if (networkChannelDatabaseRequest != null)
				{
					MonitoredDatabase monitoredDatabase = MonitoredDatabase.FindMonitoredDatabase(this.LocalNodeName, networkChannelDatabaseRequest.DatabaseGuid);
					if (monitoredDatabase == null)
					{
						if (this.MonitoredDatabase == null || !(networkChannelDatabaseRequest.DatabaseGuid == this.MonitoredDatabase.DatabaseGuid) || !(networkChannelDatabaseRequest is ProgressCiFileRequest))
						{
							NetworkChannel.Tracer.TraceError<Guid, Type>((long)this.GetHashCode(), "ServiceRequest for db {0} fails for request {1}", networkChannelDatabaseRequest.DatabaseGuid, networkChannelDatabaseRequest.GetType());
							Exception ex2 = new SourceDatabaseNotFoundException(networkChannelDatabaseRequest.DatabaseGuid, this.LocalNodeName);
							this.SendException(ex2);
							throw ex2;
						}
						NetworkChannel.Tracer.TraceError<Guid>((long)this.GetHashCode(), "ServiceRequest allows ProgressCiFileRequest despite missing monDB for {0}", networkChannelDatabaseRequest.DatabaseGuid);
					}
					else
					{
						networkChannelDatabaseRequest.LinkWithMonitoredDatabase(monitoredDatabase);
					}
					this.KeepAlive = true;
				}
				networkChannelRequest.Execute();
				if (this.KeepAlive)
				{
					if (this.NetworkChannelManagesAsyncReads)
					{
						this.TcpChannel.SetIdle();
						this.StartRead(new NetworkChannelCallback(NetworkChannel.ServiceCallback), this);
					}
					flag = true;
				}
			}
			catch (SocketException ex3)
			{
				ex = ex3;
			}
			catch (IOException ex4)
			{
				ex = ex4;
			}
			catch (NetworkTransportException ex5)
			{
				ex = ex5;
			}
			catch (TransientException ex6)
			{
				ex = ex6;
			}
			finally
			{
				if (ex != null)
				{
					ExTraceGlobals.NetworkChannelTracer.TraceError<Exception>(0L, "ServiceRequest caught: {0}", ex);
				}
				if (!flag)
				{
					this.Close();
				}
			}
		}

		internal static void StaticTraceDebug(string format, params object[] args)
		{
			ExTraceGlobals.NetworkChannelTracer.TraceDebug(0L, format, args);
		}

		internal static void StaticTraceError(string format, params object[] args)
		{
			ExTraceGlobals.NetworkChannelTracer.TraceError(0L, format, args);
		}

		internal void TraceDebug(string format, params object[] args)
		{
			ExTraceGlobals.NetworkChannelTracer.TraceDebug((long)this.GetHashCode(), format, args);
		}

		internal void TraceError(string format, params object[] args)
		{
			ExTraceGlobals.NetworkChannelTracer.TraceError((long)this.GetHashCode(), format, args);
		}

		internal void InvokeWithCatch(NetworkChannel.CatchableOperation op)
		{
			Exception ex = null;
			bool flag = true;
			try
			{
				op();
				flag = false;
			}
			catch (FileIOonSourceException ex2)
			{
				flag = false;
				ex = ex2;
			}
			catch (IOException ex3)
			{
				if (ex3.InnerException is ObjectDisposedException)
				{
					ex = new NetworkCancelledException(ex3);
				}
				else
				{
					this.ReportNetworkError(ex3);
					ex = new NetworkCommunicationException(this.PartnerNodeName, ex3.Message, ex3);
				}
			}
			catch (SocketException ex4)
			{
				this.ReportNetworkError(ex4);
				ex = new NetworkCommunicationException(this.PartnerNodeName, ex4.Message, ex4);
			}
			catch (NetworkCommunicationException ex5)
			{
				this.ReportNetworkError(ex5);
				ex = ex5;
			}
			catch (NetworkTimeoutException ex6)
			{
				this.ReportNetworkError(ex6);
				ex = ex6;
			}
			catch (NetworkRemoteException ex7)
			{
				flag = false;
				ex = ex7;
			}
			catch (NetworkEndOfDataException ex8)
			{
				ex = ex8;
			}
			catch (NetworkCorruptDataGenericException)
			{
				ex = new NetworkCorruptDataException(this.PartnerNodeName);
			}
			catch (NetworkTransportException ex9)
			{
				ex = ex9;
			}
			catch (CompressionException innerException)
			{
				ex = new NetworkCorruptDataException(this.PartnerNodeName, innerException);
			}
			catch (DecompressionException innerException2)
			{
				ex = new NetworkCorruptDataException(this.PartnerNodeName, innerException2);
			}
			catch (ObjectDisposedException innerException3)
			{
				ex = new NetworkCancelledException(innerException3);
			}
			catch (SerializationException ex10)
			{
				ex = new NetworkCommunicationException(this.PartnerNodeName, ex10.Message, ex10);
			}
			catch (TargetInvocationException ex11)
			{
				if (ex11.InnerException == null || !(ex11.InnerException is SerializationException))
				{
					throw;
				}
				ex = new NetworkCommunicationException(this.PartnerNodeName, ex11.Message, ex11);
			}
			finally
			{
				if (flag)
				{
					this.Abort();
				}
			}
			if (ex != null)
			{
				this.TraceError("InvokeWithCatch: Forwarding exception: {0}", new object[]
				{
					ex
				});
				throw ex;
			}
		}

		private void HandleFileIOException(string fullSourceFilename, bool throwCorruptLogDetectedException, Action fileAction)
		{
			try
			{
				fileAction();
			}
			catch (IOException ex)
			{
				ExTraceGlobals.NetworkChannelTracer.TraceError<IOException>((long)this.GetHashCode(), "HandleFileIOException(): Received IOException. Will rethrow it. Ex: {0}", ex);
				if (throwCorruptLogDetectedException)
				{
					CorruptLogDetectedException ex2 = new CorruptLogDetectedException(fullSourceFilename, ex.Message, ex);
					throw new FileIOonSourceException(Environment.MachineName, fullSourceFilename, ex2.Message, ex2);
				}
				throw new FileIOonSourceException(Environment.MachineName, fullSourceFilename, ex.Message, ex);
			}
			catch (UnauthorizedAccessException ex3)
			{
				ExTraceGlobals.NetworkChannelTracer.TraceError<UnauthorizedAccessException>((long)this.GetHashCode(), "HandleFileIOException(): Received UnauthorizedAccessException. Will rethrow it. Ex: {0}", ex3);
				throw new FileIOonSourceException(Environment.MachineName, fullSourceFilename, ex3.Message, ex3);
			}
		}

		protected void ReportNetworkError(Exception e)
		{
			if (this.NetworkPath != null && !this.IsClosed && !this.IsAborted)
			{
				NetworkManager.ReportError(this.NetworkPath, e);
			}
		}

		internal NetworkChannelMessage GetMessage()
		{
			this.Read(this.m_tempHeaderBuf, 0, 16);
			return NetworkChannelMessage.ReadMessage(this, this.m_tempHeaderBuf);
		}

		internal NetworkChannelMessage TryGetMessage()
		{
			if (!this.m_transport.HasAsyncDataToRead())
			{
				return null;
			}
			return this.GetMessage();
		}

		internal void Read(byte[] buf, int off, int len)
		{
			this.InvokeWithCatch(delegate
			{
				this.m_transport.Read(buf, off, len);
			});
		}

		internal void StartRead(NetworkChannelCallback callback, object context)
		{
			if (this.m_isAborted || this.m_isClosed)
			{
				throw new NetworkCancelledException();
			}
			this.InvokeWithCatch(delegate
			{
				this.m_transport.StartRead(callback, context);
			});
		}

		internal void Close()
		{
			NetworkChannel.Tracer.TraceFunction((long)this.GetHashCode(), "Closing");
			lock (this)
			{
				if (!this.m_isClosed)
				{
					this.KeepAlive = false;
					if (this.LogChecksummer != null)
					{
						this.LogChecksummer.Dispose();
						this.LogChecksummer = null;
					}
					if (this.m_listener != null)
					{
						this.m_listener.RemoveActiveChannel(this);
					}
					if (this.m_seederPageReaderServerContext != null)
					{
						this.m_seederPageReaderServerContext.Close();
						this.m_seederPageReaderServerContext = null;
					}
					if (this.m_seederServerContext != null)
					{
						this.CloseSeederServerContext();
					}
					if (this.m_transport != null)
					{
						this.m_transport.Close();
					}
					if (this.m_channel != null)
					{
						this.m_channel.Close();
					}
					this.m_isClosed = true;
				}
			}
			NetworkChannel.Tracer.TraceFunction((long)this.GetHashCode(), "Closed");
		}

		internal void Abort()
		{
			this.m_isAborted = true;
			this.KeepAlive = false;
			if (this.m_channel != null)
			{
				this.m_channel.Abort();
			}
		}

		internal void SendException(Exception ex)
		{
			NetworkChannel.Tracer.TraceError<Type>((long)this.GetHashCode(), "SendException: {0}", ex.GetType());
			this.InvokeWithCatch(delegate
			{
				this.m_transport.WriteException(ex);
			});
		}

		internal void SendMessage(byte[] buf, int off, int len)
		{
			this.InvokeWithCatch(delegate
			{
				this.m_transport.WriteMessage(buf, off, len);
			});
		}

		internal void Write(byte[] buf, int off, int len)
		{
			this.InvokeWithCatch(delegate
			{
				this.m_transport.Write(buf, off, len);
			});
		}

		internal void ThrowUnexpectedMessage(NetworkChannelMessage msg)
		{
			NetworkUnexpectedMessageException ex = new NetworkUnexpectedMessageException(this.PartnerNodeName, msg.ToString());
			throw ex;
		}

		internal static void ThrowTimeoutException(string nodeName, string reason)
		{
			throw new NetworkTimeoutException(nodeName, reason);
		}

		internal void ThrowTimeoutException(string reason)
		{
			NetworkChannel.ThrowTimeoutException(this.PartnerNodeName, reason);
		}

		internal SeederPageReaderServerContext GetSeederPageReaderServerContext(string databaseName, string databasePath)
		{
			SeederPageReaderServerContext seederPageReaderServerContext;
			lock (this.m_SeederPageReaderServerContextLocker)
			{
				seederPageReaderServerContext = this.SeederPageReaderServerContext;
				if (seederPageReaderServerContext == null)
				{
					if (string.IsNullOrEmpty(databasePath))
					{
						databasePath = this.m_sourceDatabase.Config.DestinationEdbPath;
					}
					IEseDatabaseReader eseDatabaseReader = EseDatabaseReader.GetEseDatabaseReader(Environment.MachineName, this.m_sourceDatabase.DatabaseGuid, databaseName, databasePath);
					seederPageReaderServerContext = new SeederPageReaderServerContext(eseDatabaseReader, this.PartnerNodeName);
					this.SeederPageReaderServerContext = seederPageReaderServerContext;
				}
			}
			return seederPageReaderServerContext;
		}

		internal SeederServerContext CreateSeederServerContext(Guid dbGuid, Guid? serverGuid, SeedType seedType)
		{
			SeederServerContext seederServerContext = null;
			lock (this.m_SeederServerContextLocker)
			{
				if (this.m_isClosed)
				{
					throw new SeedingChannelIsClosedException(dbGuid);
				}
				if (this.m_seederServerContext != null)
				{
					ReplayCrimsonEvents.SeedingSourceError.Log<Guid, string, string, string>(dbGuid, string.Empty, this.PartnerNodeName, "CreateSeederServerContext:SeedCtx already exists");
					throw new SeedingChannelIsClosedException(dbGuid);
				}
				this.m_seederServerContext = new SeederServerContext(this, this.MonitoredDatabase, serverGuid, seedType);
				seederServerContext = this.m_seederServerContext;
				this.TraceDebug("ServerContext for Database {0} is created", new object[]
				{
					dbGuid
				});
			}
			SourceSeedTable.Instance.RegisterSeed(seederServerContext);
			seederServerContext.StartSeeding();
			return seederServerContext;
		}

		internal SeederServerContext GetSeederServerContext(Guid dbGuid)
		{
			SeederServerContext seederServerContext;
			lock (this.m_SeederServerContextLocker)
			{
				if (this.m_seederServerContext == null)
				{
					ReplayCrimsonEvents.SeedingSourceError.Log<Guid, string, string, string>(dbGuid, string.Empty, this.PartnerNodeName, "GetSeederServerContext:SeedCtx does not exist");
					throw new SeedingChannelIsClosedException(dbGuid);
				}
				DiagCore.RetailAssert(dbGuid.Equals(this.m_seederServerContext.DatabaseGuid), "SeederServer inconsistent. Requested ({0}) Found({1})", new object[]
				{
					dbGuid,
					this.m_seederServerContext.DatabaseGuid
				});
				seederServerContext = this.m_seederServerContext;
			}
			return seederServerContext;
		}

		private void CloseSeederServerContext()
		{
			SeederServerContext seederServerContext = null;
			lock (this.m_SeederServerContextLocker)
			{
				if (this.m_seederServerContext != null)
				{
					seederServerContext = this.m_seederServerContext;
					this.m_seederServerContext = null;
				}
			}
			if (seederServerContext != null)
			{
				SourceSeedTable.Instance.DeregisterSeed(seederServerContext);
				seederServerContext.Close();
				NetworkChannel.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ServerContext for Database {0} is closed and cleared.", seederServerContext.DatabaseGuid);
			}
		}

		internal bool IsSeeding
		{
			get
			{
				return this.m_isSeeding;
			}
			set
			{
				if (value != this.m_isSeeding)
				{
					this.m_isSeeding = value;
					if (this.m_isSeeding)
					{
						this.TcpChannel.ReadTimeoutInMs = RegistryParameters.SeedingNetworkTimeoutInMsec;
						this.TcpChannel.WriteTimeoutInMs = RegistryParameters.SeedingNetworkTimeoutInMsec;
						return;
					}
					this.TcpChannel.ReadTimeoutInMs = RegistryParameters.LogShipTimeoutInMsec;
					this.TcpChannel.WriteTimeoutInMs = RegistryParameters.LogShipTimeoutInMsec;
				}
			}
		}

		internal void ReceiveFile(NetworkChannelFileTransferReply reply, IPerfmonCounters copyPerfCtrs, CheckSummer summer)
		{
			this.ReceiveFileInternal(reply, null, copyPerfCtrs, summer);
		}

		internal void ReceiveSeedingData(NetworkChannelFileTransferReply reply, IReplicaSeederCallback callback)
		{
			CheckSummer summer = null;
			if (this.ChecksumDataTransfer)
			{
				summer = new CheckSummer();
			}
			this.ReceiveFileInternal(reply, callback, null, summer);
		}

		internal void ReceiveFile(NetworkChannelFileTransferReply reply, IReplicaSeederCallback callback, IPerfmonCounters copyPerfCtrs, CheckSummer summer)
		{
			this.ReceiveFileInternal(reply, callback, copyPerfCtrs, summer);
		}

		private void ReceiveFileInternal(NetworkChannelFileTransferReply reply, IReplicaSeederCallback callback, IPerfmonCounters copyPerfCtrs, CheckSummer summer)
		{
			int num = 65536;
			if (this.m_receiveBuf == null)
			{
				this.m_receiveBuf = new byte[num];
			}
			byte[] receiveBuf = this.m_receiveBuf;
			bool flag = false;
			bool flag2 = false;
			StopwatchStamp stamp = StopwatchStamp.GetStamp();
			long num2 = 0L;
			long num3 = 0L;
			long num4 = 0L;
			double num5 = -1.0;
			try
			{
				this.TraceDebug("Receiving '{0}'.", new object[]
				{
					reply.DestinationFileName
				});
				int num6 = 0;
				uint sectorSize = FileOperations.GetSectorSize(reply.DestinationFileName);
				long num7 = Align.RoundUp(reply.FileSize, (long)((ulong)sectorSize));
				using (SafeFileHandle safeFileHandle = LogCopy.OpenFile(reply.DestinationFileName, false, out num6))
				{
					flag2 = true;
					FileStream fileStream = null;
					try
					{
						fileStream = LogCopy.OpenFileStream(safeFileHandle, false);
						fileStream.SetLength(num7);
						long num8 = stamp.Restart();
						NetworkChannel.Tracer.TracePerformance<long>((long)this.GetHashCode(), "ReceiveFileInternal: Ready to write after {0} uSec", StopwatchStamp.TicksToMicroSeconds(num8));
						long num9 = reply.FileSize;
						long num10 = num9;
						while (num9 > 0L)
						{
							int num11 = num;
							if (num9 < (long)((ulong)num))
							{
								num11 = (int)num9;
							}
							num8 += stamp.Restart();
							this.Read(receiveBuf, 0, num11);
							num2 += stamp.Restart();
							num4 += (long)((ulong)num11);
							num9 -= (long)num11;
							if (this.PerfCounters != null)
							{
								if (this.IsSeeding)
								{
									this.PerfCounters.RecordSeederThruputReceived((long)num11);
								}
								else
								{
									this.PerfCounters.RecordLogCopyThruputReceived((long)num11);
								}
							}
							if (summer != null)
							{
								summer.Sum(receiveBuf, 0, num11);
							}
							int num12 = num11;
							int num13 = (int)((long)num12 % (long)((ulong)sectorSize));
							if (num13 != 0)
							{
								num13 = (int)(sectorSize - (uint)num13);
								BufferOperations.Zero(receiveBuf, num12, num13);
								num12 += num13;
							}
							if (callback != null && callback.IsBackupCancelled())
							{
								ExTraceGlobals.SeederServerTracer.TraceDebug<long>((long)this.GetHashCode(), "The seeding was cancelled at the size of {0} kB.\n", num3 / 1024L);
								throw new SeederOperationAbortedException();
							}
							double num14 = (double)num3 * 100.0 / (double)num10;
							if (num11 == 0 || num14 > num5)
							{
								if (callback != null)
								{
									ExTraceGlobals.SeederServerTracer.TraceDebug<string, double, bool>((long)this.GetHashCode(), "Updating progress for edb '{0}'. Percent = {1}. Callback = {2}", reply.DestinationFileName, num14, callback != null);
									callback.ReportProgress(reply.DestinationFileName, num10, num4, num3);
								}
								num5 = num14;
							}
							fileStream.Write(receiveBuf, 0, num12);
							num3 += (long)((ulong)num12);
						}
						if (summer != null)
						{
							this.Read(receiveBuf, 0, 4);
							uint num15 = BitConverter.ToUInt32(receiveBuf, 0);
							if (summer.GetSum() != num15)
							{
								throw new NetworkTransportException("csum err");
							}
						}
						fileStream.Flush();
						fileStream.Dispose();
						fileStream = null;
						if (reply.FileSize != num7)
						{
							NetworkChannel.Tracer.TraceDebug<string, long, long>((long)this.GetHashCode(), "File {0} was written unbuffered, and is now 0x{1:x} bytes. Now shrinking it to 0x{2:x}.", reply.DestinationFileName, num7, reply.FileSize);
							FileOperations.TruncateFile(reply.DestinationFileName, reply.FileSize);
						}
						FileInfo fileInfo = new FileInfo(reply.DestinationFileName);
						fileInfo.LastWriteTimeUtc = reply.LastWriteUtc;
						num8 += stamp.ElapsedTicks;
						if (copyPerfCtrs != null)
						{
							copyPerfCtrs.RecordFileModeWriteLatency(num8);
							copyPerfCtrs.RecordLogCopierNetworkReadLatency(num2);
						}
						long num16 = StopwatchStamp.TicksToMicroSeconds(num8);
						long num17 = StopwatchStamp.TicksToMicroSeconds(num2);
						NetworkChannel.Tracer.TracePerformance<long, long, long>((long)this.GetHashCode(), "ReceiveFileInternal: Finished after {0} ms. Disk={1} uSec Net={2} uSec", (num16 + num17) / 1000L, num16, num17);
						flag = true;
					}
					finally
					{
						if (fileStream != null)
						{
							FileCleanup.DisposeProperly(fileStream);
						}
					}
				}
				this.TraceDebug("Total bytes read: {0}. Total bytes written: {1}. Original source DB file size: {2}", new object[]
				{
					num4,
					num3,
					reply.FileSize
				});
			}
			finally
			{
				if (!flag && flag2)
				{
					FileCleanup.TryDelete(reply.DestinationFileName);
				}
			}
		}

		public void SendFileTransferReply(NetworkChannelFileTransferReply reply, string fullSourceFilename, SafeFileHandle readFileHandle, CheckSummer summer)
		{
			this.InvokeWithCatch(delegate
			{
				this.SendFileInternal(reply, fullSourceFilename, readFileHandle, null, summer, false);
			});
		}

		public void SendLogFileTransferReply(NetworkChannelFileTransferReply reply, string fullSourceFilename, SafeFileHandle readFileHandle, SourceDatabasePerformanceCountersInstance perfCounters, CheckSummer summer)
		{
			this.InvokeWithCatch(delegate
			{
				this.SendFileInternal(reply, fullSourceFilename, readFileHandle, perfCounters, summer, true);
			});
		}

		private void SendFileInternal(NetworkChannelFileTransferReply reply, string fullSourceFilename, SafeFileHandle readFileHandle, SourceDatabasePerformanceCountersInstance perfCounters, CheckSummer summer, bool isLogFile)
		{
			FileStream fs = null;
			FileInfo fileInfo = null;
			StopwatchStamp timer = StopwatchStamp.GetStamp();
			long diskLatency = 0L;
			long num = 0L;
			long num2 = 0L;
			long num3 = 0L;
			long num4 = 0L;
			try
			{
				this.HandleFileIOException(fullSourceFilename, false, delegate
				{
					fs = LogCopy.OpenFileStream(readFileHandle, true);
					fileInfo = new FileInfo(fullSourceFilename);
					reply.FileSize = fileInfo.Length;
					reply.LastWriteUtc = fileInfo.LastWriteTimeUtc;
				});
				diskLatency = timer.Restart();
				NetworkChannel.Tracer.TracePerformance<long>((long)this.GetHashCode(), "SendFileInternal prep in {0} uSec", StopwatchStamp.TicksToMicroSeconds(diskLatency));
				reply.Send();
				long num5 = timer.Restart();
				int bufSize = 65536;
				if (isLogFile)
				{
					bufSize = (int)fileInfo.Length;
				}
				long bytesRemaining = 0L;
				BinaryReader reader = null;
				this.HandleFileIOException(fullSourceFilename, false, delegate
				{
					bytesRemaining = fileInfo.Length;
					reader = new BinaryReader(fs);
				});
				while (bytesRemaining > 0L)
				{
					byte[] buf = null;
					int bytesRead = 0;
					this.HandleFileIOException(fullSourceFilename, true, delegate
					{
						if (isLogFile && 1048576L != fileInfo.Length)
						{
							throw new IOException(ReplayStrings.UnexpectedEOF(fullSourceFilename));
						}
						int num9 = (int)Math.Min(bytesRemaining, (long)bufSize);
						buf = reader.ReadBytes(num9);
						bytesRead = buf.Length;
						if (bytesRead != num9)
						{
							ExTraceGlobals.NetworkChannelTracer.TraceError<int, int, string>((long)this.GetHashCode(), "SendFileInternal. Expected {0} but got {1} bytes from {2}", num9, bytesRead, fullSourceFilename);
							throw new IOException(ReplayStrings.UnexpectedEOF(fullSourceFilename));
						}
						diskLatency += timer.Restart();
						if (summer != null)
						{
							summer.Sum(buf, 0, bytesRead);
						}
						if (isLogFile && this.LogChecksummer != null)
						{
							EsentErrorException ex = this.LogChecksummer.Verify(fullSourceFilename, buf);
							if (ex != null)
							{
								NetworkChannel.Tracer.TraceError<string, EsentErrorException>((long)this.GetHashCode(), "LogChecksum({0}) failed: {1}", fullSourceFilename, ex);
								if (ex is EsentLogFileCorruptException)
								{
									CorruptLogDetectedException ex2 = new CorruptLogDetectedException(fullSourceFilename, ex.Message, ex);
									throw new FileIOonSourceException(Environment.MachineName, fullSourceFilename, ex2.Message, ex2);
								}
								throw new FileIOonSourceException(Environment.MachineName, fullSourceFilename, ex.Message, ex);
							}
						}
					});
					num += timer.Restart();
					this.Write(buf, 0, bytesRead);
					num5 += timer.Restart();
					if (perfCounters != null)
					{
						perfCounters.AverageWriteTime.IncrementBy(num5);
						perfCounters.AverageWriteTimeBase.Increment();
						perfCounters.AverageReadTime.IncrementBy(diskLatency);
						perfCounters.AverageReadTimeBase.Increment();
						perfCounters.TotalBytesSent.IncrementBy((long)bytesRead);
						perfCounters.WriteThruput.IncrementBy((long)bytesRead);
					}
					num3 += diskLatency;
					num2 += num5;
					num4 += num;
					diskLatency = 0L;
					num5 = 0L;
					num = 0L;
					bytesRemaining -= (long)bytesRead;
				}
				if (summer != null)
				{
					byte[] bytes = BitConverter.GetBytes(summer.GetSum());
					this.Write(bytes, 0, 4);
				}
				this.m_transport.Flush();
				num5 += timer.Restart();
				NetworkChannel.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Tcp.SendFile sent {0}", fullSourceFilename);
				long num6 = StopwatchStamp.TicksToMicroSeconds(num3);
				long num7 = StopwatchStamp.TicksToMicroSeconds(num2);
				long num8 = StopwatchStamp.TicksToMicroSeconds(num4);
				NetworkChannel.Tracer.TracePerformance((long)this.GetHashCode(), "SendFile finished in {0} uSec. Read={1} Write={2} Verify={3}", new object[]
				{
					num6 + num7 + num8,
					num6,
					num7,
					num8
				});
			}
			finally
			{
				if (fs != null)
				{
					fs.Dispose();
				}
			}
		}

		public void SendSeedingDataTransferReply(SeedDatabaseFileReply reply, ReadDatabaseCallback readDbCallback)
		{
			this.InvokeWithCatch(delegate
			{
				this.SendSeedingDataInternal(reply, readDbCallback);
			});
		}

		private void SendSeedingDataInternal(SeedDatabaseFileReply reply, ReadDatabaseCallback readDbCallback)
		{
			if (reply == null)
			{
				throw new ArgumentNullException("reply");
			}
			if (readDbCallback == null)
			{
				throw new ArgumentNullException("readDbCallback");
			}
			int num = 65536;
			byte[] buf = new byte[num];
			ulong totalBytesRead = 0UL;
			bool isPassiveCopy = this.MonitoredDatabase.IsPassiveCopy;
			string fullSourceFilename;
			if (isPassiveCopy)
			{
				fullSourceFilename = this.MonitoredDatabase.Config.SourceEdbPath;
			}
			else
			{
				fullSourceFilename = this.MonitoredDatabase.Config.DestinationEdbPath;
			}
			ulong num2 = (ulong)reply.FileSize;
			int bytesRead = 0;
			CheckSummer checkSummer = null;
			if (this.ChecksumDataTransfer)
			{
				checkSummer = new CheckSummer();
			}
			ReplayStopwatch replayStopwatch = new ReplayStopwatch();
			replayStopwatch.Start();
			do
			{
				bool bytesExpectedWasZero = false;
				int bytesExpected = (int)Math.Min(num2, (ulong)((long)num));
				if (bytesExpected == 0)
				{
					bytesExpectedWasZero = true;
					bytesExpected = num;
				}
				this.HandleFileIOException(fullSourceFilename, false, delegate
				{
					bytesRead = readDbCallback(buf, totalBytesRead, bytesExpected);
					if (bytesRead != 0)
					{
						if (bytesRead < bytesExpected)
						{
							ExTraceGlobals.NetworkChannelTracer.TraceError<int, int>((long)this.GetHashCode(), "SendSeedingDataInternal. Expected {0} but got {1} bytes. This is may be a perf problem.", bytesExpected, bytesRead);
						}
						return;
					}
					if (!bytesExpectedWasZero)
					{
						throw new IOException("Read zero bytes while bytes expected to read is " + bytesExpected);
					}
					ExTraceGlobals.NetworkChannelTracer.TraceError<int, int>((long)this.GetHashCode(), "SendSeedingDataInternal. Expected {0} but got {1} bytes", bytesExpected, bytesRead);
				});
				if (bytesRead > 0)
				{
					if (checkSummer != null)
					{
						checkSummer.Sum(buf, 0, bytesRead);
					}
					this.Write(buf, 0, bytesRead);
					num2 -= (ulong)((long)bytesRead);
					totalBytesRead += (ulong)((long)bytesRead);
				}
			}
			while (bytesRead > 0);
			if (checkSummer != null)
			{
				byte[] bytes = BitConverter.GetBytes(checkSummer.GetSum());
				this.Write(bytes, 0, 4);
			}
			this.m_transport.Flush();
			replayStopwatch.Stop();
			string databaseName = this.MonitoredDatabase.DatabaseName;
			string partnerNodeName = this.PartnerNodeName;
			long num3 = replayStopwatch.ElapsedMilliseconds / 1000L;
			if (num3 <= 0L)
			{
				num3 = 1L;
			}
			long num4 = (long)(totalBytesRead / (ulong)num3 / 1024UL);
			string text = string.Format("{0} KB/sec", num4);
			ByteQuantifiedSize byteQuantifiedSize = new ByteQuantifiedSize(totalBytesRead);
			ExTraceGlobals.SeederServerTracer.TraceDebug((long)this.GetHashCode(), "SeedDatabaseFile({0}): Sent {1} bytes in {2} sec = {3}. Target={4}", new object[]
			{
				databaseName,
				totalBytesRead,
				num3,
				text,
				partnerNodeName
			});
			if (isPassiveCopy)
			{
				ReplayCrimsonEvents.PassiveSeedSourceSentEDB.Log<string, string, string, ByteQuantifiedSize, TimeSpan, string>(databaseName, Environment.MachineName, partnerNodeName, byteQuantifiedSize, replayStopwatch.Elapsed, text);
				return;
			}
			ReplayCrimsonEvents.ActiveSeedSourceSentEDB.Log<string, string, string, ByteQuantifiedSize, TimeSpan, string>(databaseName, Environment.MachineName, partnerNodeName, byteQuantifiedSize, replayStopwatch.Elapsed, text);
		}

		private TcpChannel m_channel;

		protected NetworkPackagingLayer m_transport;

		private bool m_isClosed;

		private bool m_isAborted;

		protected NetworkPath m_networkPath;

		private bool m_keepAlive;

		private MonitoredDatabase m_sourceDatabase;

		private SeederPageReaderServerContext m_seederPageReaderServerContext;

		private SeederServerContext m_seederServerContext;

		public object m_SeederPageReaderServerContextLocker = new object();

		public object m_SeederServerContextLocker = new object();

		private TcpListener m_listener;

		private ExchangeNetworkPerfmonCounters m_networkPerfCounters;

		private string m_remoteEndPointString;

		private string m_localEndPointString;

		private string m_networkName;

		private static bool disableCompressionDueToFatalError = false;

		private static readonly ServerVersion FirstVersionSupportingCoconet = new ServerVersion(15, 0, 800, 3);

		private byte[] m_tempHeaderBuf = new byte[16];

		private bool m_isSeeding;

		private byte[] m_receiveBuf;

		public enum DataEncodingScheme
		{
			Uncompressed,
			CompressedXpress,
			Coconet,
			LastIndex
		}

		internal delegate void CatchableOperation();
	}
}

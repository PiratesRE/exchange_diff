using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using Microsoft.Exchange.Cluster.Common.Extensions;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.EseRepl
{
	internal class NetworkPackagingLayer
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.NetworkChannelTracer;
			}
		}

		private static void ReadCallback(IAsyncResult ar)
		{
			NetworkPackagingLayer networkPackagingLayer = (NetworkPackagingLayer)ar.AsyncState;
			NetworkPackagingLayer.Tracer.TraceDebug<bool, int>(0L, "ReadCallback completion was sync: {0} arhash={1}", ar.CompletedSynchronously, ar.GetHashCode());
			Exception ex = null;
			NetworkChannelCallback networkChannelCallback = null;
			object asyncState = null;
			bool flag = false;
			int num = 0;
			try
			{
				num = networkPackagingLayer.m_tcpChannel.Stream.EndRead(ar);
				ExTraceGlobals.NetworkChannelTracer.TraceDebug<int, string, string>((long)networkPackagingLayer.GetHashCode(), "AsyncReadCallback read {0} bytes from {1} on {2}", num, networkPackagingLayer.m_netChannel.RemoteEndPointString, networkPackagingLayer.m_netChannel.LocalEndPointString);
				flag = true;
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (Win32Exception ex3)
			{
				ex = ex3;
			}
			catch (ObjectDisposedException ex4)
			{
				NetworkPackagingLayer.Tracer.TraceError<bool, string>(0L, "ReadCallback has closed={0} ignoring exception {1}", networkPackagingLayer.m_closeCalled, ex4.Message);
			}
			catch (InvalidOperationException ex5)
			{
				NetworkPackagingLayer.Tracer.TraceError<string>(0L, "ReadCallback ignoring exception {0}", ex5.Message);
			}
			finally
			{
				lock (networkPackagingLayer)
				{
					networkChannelCallback = networkPackagingLayer.m_userCallback;
					asyncState = networkPackagingLayer.m_userCallbackCtx;
					networkPackagingLayer.m_asyncException = ex;
					if (flag)
					{
						networkPackagingLayer.m_asyncReadByteCount = num;
						if (num == 0)
						{
							networkPackagingLayer.m_asyncReadGotZeroBytes = true;
						}
					}
					else
					{
						networkPackagingLayer.m_asyncReadByteCount = 0;
					}
					networkPackagingLayer.m_asyncReadActive = false;
					networkPackagingLayer.m_asyncReadCompleteEvent.Set();
				}
			}
			if (networkChannelCallback != null)
			{
				networkChannelCallback(asyncState, num, ar.CompletedSynchronously, ex);
			}
		}

		internal string PartnerNodeName
		{
			get
			{
				return this.m_tcpChannel.PartnerNodeName;
			}
		}

		internal NetworkChannel.DataEncodingScheme Encoding
		{
			get
			{
				return this.m_encoding;
			}
			set
			{
				this.m_encoding = value;
			}
		}

		internal CompressionConfig CompressionConfig { get; private set; }

		private CoconetConfig CoconetConfig
		{
			get
			{
				return this.CompressionConfig.CoconetConfig;
			}
		}

		internal long TotalDecompressedBytesReceived { get; private set; }

		internal long TotalCompressedBytesReceived { get; private set; }

		internal void SetEncoding(CompressionConfig cfg)
		{
			if (cfg.Provider == CompressionConfig.CompressionProvider.Coconet)
			{
				this.Encoding = NetworkChannel.DataEncodingScheme.Coconet;
			}
			else if (cfg.Provider == CompressionConfig.CompressionProvider.Xpress)
			{
				this.Encoding = NetworkChannel.DataEncodingScheme.CompressedXpress;
			}
			else
			{
				this.Encoding = NetworkChannel.DataEncodingScheme.Uncompressed;
			}
			this.CompressionConfig = cfg;
		}

		internal NetworkPackagingLayer(NetworkChannel netChannel, TcpChannel tcpChannel)
		{
			this.m_netChannel = netChannel;
			this.m_tcpChannel = tcpChannel;
			this.m_readPacketHeaderBuf = new byte[Math.Max(Math.Max(5, 5), 9)];
		}

		internal void Close()
		{
			this.m_closeCalled = true;
			this.m_tcpChannel.Close();
			if (this.m_coconetCompressor != null)
			{
				this.m_coconetCompressor.Dispose();
			}
			if (this.m_coconetDecompressor != null)
			{
				this.m_coconetDecompressor.Dispose();
			}
			int num = 0;
			while (!this.WaitForAsyncRead(1000))
			{
				num++;
				ExTraceGlobals.NetworkChannelTracer.TraceError<int>((long)this.GetHashCode(), "Timeout while aborting...still waiting for async read to complete after {0} secs", num);
				if (num > 30)
				{
					Dependencies.AssertRtl(false, "TCP stack is not responding properly", new object[0]);
				}
			}
			this.DisposeCompletionEvent();
		}

		private void DisposeCompletionEvent()
		{
			lock (this)
			{
				if (this.m_asyncReadCompleteEvent != null)
				{
					((IDisposable)this.m_asyncReadCompleteEvent).Dispose();
					this.m_asyncReadCompleteEvent = null;
				}
			}
		}

		private void WriteLeadingBytes(NetworkPackagingLayer.PackageEncoding packetType, int leadingContentLength, byte[] content, int offsetInContent, int totalContentLength, bool flush)
		{
			int num = 5 + leadingContentLength;
			byte[] array = new byte[num];
			array[0] = (byte)packetType;
			int num2 = 1;
			Serialization.SerializeUInt32(array, ref num2, (uint)totalContentLength);
			Array.Copy(content, offsetInContent, array, 5, leadingContentLength);
			if (flush)
			{
				this.m_tcpChannel.WriteAndFlush(array, 0, num);
				return;
			}
			this.m_tcpChannel.Write(array, 0, num);
		}

		private void WriteInternal(byte[] buf, int off, int len, int minLeadingLength, bool flush)
		{
			if (this.Encoding == NetworkChannel.DataEncodingScheme.CompressedXpress && len > 265)
			{
				this.WriteXpress(buf, off, len);
				return;
			}
			if (this.Encoding == NetworkChannel.DataEncodingScheme.Coconet && len > 265)
			{
				this.WriteCoconet(buf, off, len);
				return;
			}
			int num = Math.Min(len, minLeadingLength);
			int num2 = len - num;
			this.WriteLeadingBytes(NetworkPackagingLayer.PackageEncoding.Raw, num, buf, off, len, flush);
			if (num2 > 0)
			{
				this.m_tcpChannel.Write(buf, off + num, num2);
			}
		}

		internal void Write(byte[] buf, int off, int len)
		{
			this.WriteInternal(buf, off, len, 500, false);
		}

		internal void WriteMessage(byte[] buf, int off, int len)
		{
			this.WriteInternal(buf, off, len, len, true);
		}

		internal void WriteMessage(byte[] buf, int off, int len, bool flush)
		{
			this.WriteInternal(buf, off, len, len, flush);
		}

		internal void WriteMessageParts(byte[] buf1, int off1, int len1, byte[] buf2, int off2, int len2, bool flush)
		{
			int num = 5 + len1 + len2;
			byte[] array = new byte[num];
			array[0] = 1;
			int num2 = 1;
			Serialization.SerializeUInt32(array, ref num2, (uint)(len1 + len2));
			Array.Copy(buf1, off1, array, 5, len1);
			Array.Copy(buf2, off2, array, 5 + len1, len2);
			this.m_tcpChannel.Write(array, 0, num, flush);
		}

		private void CheckBlockLen(int len)
		{
			if (len > 1052672 || len < 0)
			{
				this.m_netChannel.TraceError("Invalid block len:{0} max:{1}", new object[]
				{
					len,
					1052672
				});
				throw new NetworkCorruptDataException(this.PartnerNodeName);
			}
		}

		internal void WriteException(Exception ex)
		{
			byte[] array = SerializationUtil.ObjectToBytes(ex);
			if (array.Length > 1052672)
			{
				throw new NetworkDataOverflowGenericException();
			}
			this.WriteLeadingBytes(NetworkPackagingLayer.PackageEncoding.SerializedException, array.Length, array, 0, array.Length, true);
		}

		internal void Flush()
		{
			this.m_tcpChannel.Flush();
		}

		protected void WriteXpress(byte[] buf, int off, int len)
		{
			int i = len;
			while (i > 0)
			{
				int num = Math.Min(i, 65536);
				this.WriteXpressBlock(buf, off, num);
				i -= num;
				off += num;
			}
		}

		protected byte[] CompressBuf
		{
			get
			{
				if (this.m_compressBuf == null)
				{
					if (this.Encoding == NetworkChannel.DataEncodingScheme.CompressedXpress)
					{
						this.m_compressBuf = new byte[65541];
					}
					else if (this.Encoding == NetworkChannel.DataEncodingScheme.Coconet)
					{
						this.m_compressBuf = new byte[1048586];
					}
				}
				return this.m_compressBuf;
			}
		}

		protected void WriteXpressBlock(byte[] buf, int offset, int length)
		{
			int num;
			Xpress.Compress(buf, offset, length, this.CompressBuf, 5, 65536, out num);
			this.CompressBuf[0] = 2;
			int num2 = 1;
			ushort val = (ushort)(num - 1);
			Serialization.SerializeUInt16(this.CompressBuf, ref num2, val);
			val = (ushort)(length - 1);
			Serialization.SerializeUInt16(this.CompressBuf, ref num2, val);
			this.m_tcpChannel.Write(this.CompressBuf, 0, 5 + num);
		}

		private void WriteCoconet(byte[] buf, int off, int len)
		{
			if (this.m_coconetCompressor == null)
			{
				this.m_coconetCompressor = new CoconetCompressor(this.CoconetConfig.DictionarySize, this.CoconetConfig.SampleRate, (CoconetCompressor.LzOption)this.CoconetConfig.LzOption);
			}
			int i = len;
			while (i > 0)
			{
				int num = Math.Min(i, 1048576);
				this.WriteCoconetBlock(buf, off, num);
				i -= num;
				off += num;
			}
		}

		private bool Memcmp(byte[] b1, int off1, byte[] b2, int off2, int len)
		{
			for (int i = 0; i < len; i++)
			{
				if (b1[i + off1] != b2[i + off2])
				{
					return false;
				}
			}
			return true;
		}

		protected void WriteCoconetBlock(byte[] buf, int offset, int length)
		{
			byte[] compressBuf = this.CompressBuf;
			int num;
			this.m_coconetCompressor.Compress(buf, offset, length, compressBuf, 9, 1048577, out num);
			compressBuf[0] = 4;
			ExBitConverter.Write(num, compressBuf, 1);
			ExBitConverter.Write(length, compressBuf, 5);
			this.m_tcpChannel.Write(compressBuf, 0, 9 + num);
		}

		internal void ThrowEndOfData(int bytesExpected, int bytesRead)
		{
			ExTraceGlobals.NetworkChannelTracer.TraceError<int, int>((long)this.GetHashCode(), "End of data: Expected:{0} Read:{1}", bytesExpected, bytesRead);
			throw new NetworkEndOfDataException(this.PartnerNodeName, Strings.NetworkReadEOF);
		}

		private void ReadChunk(byte[] buf, int off, int bytesToRead)
		{
			int num = this.m_tcpChannel.TryReadChunk(buf, off, bytesToRead);
			if (num != bytesToRead)
			{
				this.ThrowEndOfData(bytesToRead, num);
			}
		}

		private bool WaitForAsyncRead(int timeoutInMs)
		{
			bool flag = false;
			lock (this)
			{
				if (this.m_asyncReadCompleteEvent != null && !this.m_asyncReadCompleteEvent.WaitOne(0, false))
				{
					flag = true;
				}
			}
			if (flag)
			{
				ExTraceGlobals.NetworkChannelTracer.TraceDebug<int, int>((long)this.GetHashCode(), "WaitForAsyncRead: waiting a max timeout={0}ms on thread {1}", timeoutInMs, DiagnosticsNativeMethods.GetCurrentThreadId());
				if (!this.m_asyncReadCompleteEvent.WaitOne(timeoutInMs, false))
				{
					ExTraceGlobals.NetworkChannelTracer.TraceError<int>((long)this.GetHashCode(), "WaitForAsyncRead: timed out on thread {0}", DiagnosticsNativeMethods.GetCurrentThreadId());
					return false;
				}
			}
			return true;
		}

		internal void Read(byte[] buf, int off, int len)
		{
			if (!this.WaitForAsyncRead(this.m_tcpChannel.ReadTimeoutInMs))
			{
				this.m_tcpChannel.ThrowTimeoutException(Strings.NetworkReadTimeout(this.m_tcpChannel.ReadTimeoutInMs / 1000));
			}
			if (this.m_asyncReadByteCount <= 0)
			{
				if (this.m_asyncException != null)
				{
					Exception asyncException = this.m_asyncException;
					this.m_asyncException = null;
					throw asyncException;
				}
				if (this.m_asyncReadGotZeroBytes)
				{
					this.m_asyncReadGotZeroBytes = false;
					throw new NetworkEndOfDataException(this.PartnerNodeName, Strings.NetworkReadEOF);
				}
			}
			int num;
			for (int i = len; i > 0; i -= num)
			{
				num = this.ReadData(buf, off, i);
				if (num == 0)
				{
					this.ThrowEndOfData(i, num);
				}
				off += num;
			}
		}

		internal void StartRead(NetworkChannelCallback asyncCallback, object asyncState)
		{
			bool flag = false;
			lock (this)
			{
				if (this.m_asyncReadCompleteEvent == null)
				{
					this.m_asyncReadCompleteEvent = new ManualResetEvent(false);
				}
				else
				{
					this.m_asyncReadCompleteEvent.Reset();
				}
				this.m_asyncReadActive = true;
				this.m_asyncReadGotZeroBytes = false;
				this.m_asyncReadByteCount = 0;
				this.m_asyncException = null;
				this.m_userCallback = asyncCallback;
				this.m_userCallbackCtx = asyncState;
				try
				{
					IAsyncResult asyncResult = this.m_tcpChannel.Stream.BeginRead(this.m_readPacketHeaderBuf, 0, 1, new AsyncCallback(NetworkPackagingLayer.ReadCallback), this);
					NetworkPackagingLayer.Tracer.TraceDebug<bool, int>((long)this.GetHashCode(), "BeginRead completion was sync: {0} arhash={1}", asyncResult.CompletedSynchronously, asyncResult.GetHashCode());
					flag = true;
				}
				finally
				{
					if (!flag)
					{
						this.m_asyncReadActive = false;
						if (this.m_asyncReadCompleteEvent != null)
						{
							this.m_asyncReadCompleteEvent.Set();
						}
					}
				}
			}
		}

		internal bool HasAsyncDataToRead()
		{
			return this.m_asyncReadByteCount > 0;
		}

		private int ReadData(byte[] buf, int off, int len)
		{
			if (this.m_bytesRemainingToReadInCurrentBlock > 0)
			{
				int num = Math.Min(len, this.m_bytesRemainingToReadInCurrentBlock);
				if (this.m_curBlockType == NetworkPackagingLayer.PackageEncoding.Raw)
				{
					int num2 = this.m_tcpChannel.TryReadChunk(buf, off, num);
					this.m_bytesRemainingToReadInCurrentBlock -= num2;
					return num2;
				}
				int sourceIndex = this.m_decompressBufContentLen - this.m_bytesRemainingToReadInCurrentBlock;
				Array.Copy(this.m_decompressBuf, sourceIndex, buf, off, num);
				this.m_bytesRemainingToReadInCurrentBlock -= num;
				return num;
			}
			else
			{
				if (this.m_asyncReadByteCount > 0)
				{
					this.m_asyncReadByteCount = 0;
				}
				else
				{
					this.ReadChunk(this.m_readPacketHeaderBuf, 0, 1);
				}
				this.m_curBlockType = (NetworkPackagingLayer.PackageEncoding)this.m_readPacketHeaderBuf[0];
				if (this.m_curBlockType == NetworkPackagingLayer.PackageEncoding.SerializedException)
				{
					NetworkPackagingLayer.Tracer.TraceDebug((long)this.GetHashCode(), "NetPkg.ReadData found incoming exception");
					this.ReadChunk(this.m_readPacketHeaderBuf, 0, 4);
					int num3 = BitConverter.ToInt32(this.m_readPacketHeaderBuf, 0);
					this.CheckBlockLen(num3);
					byte[] array = new byte[num3];
					this.ReadChunk(array, 0, num3);
					Exception ex;
					try
					{
						ex = (Exception)SerializationUtil.BytesToObject(array);
					}
					catch (SerializationException ex2)
					{
						this.m_netChannel.TraceError("Exception deserialization failed:{0}", new object[]
						{
							ex2
						});
						ex = new NetworkRemoteExceptionUnknown(this.m_netChannel.PartnerNodeName, ex2.Message, ex2);
					}
					catch (TargetInvocationException ex3)
					{
						this.m_netChannel.TraceError("Exception deserialization failed:{0}", new object[]
						{
							ex3
						});
						if (ex3.InnerException == null || !(ex3.InnerException is SerializationException))
						{
							throw;
						}
						ex = new NetworkRemoteExceptionUnknown(this.m_netChannel.PartnerNodeName, ex3.Message, ex3);
					}
					throw new NetworkRemoteException(this.m_netChannel.PartnerNodeName, ex.Message, ex);
				}
				if (this.m_curBlockType == NetworkPackagingLayer.PackageEncoding.Raw)
				{
					this.ReadChunk(this.m_readPacketHeaderBuf, 0, 4);
					this.m_bytesRemainingToReadInCurrentBlock = BitConverter.ToInt32(this.m_readPacketHeaderBuf, 0);
					int num = Math.Min(len, this.m_bytesRemainingToReadInCurrentBlock);
					int num2 = this.m_tcpChannel.TryReadChunk(buf, off, num);
					this.m_bytesRemainingToReadInCurrentBlock -= num2;
					this.TotalDecompressedBytesReceived += (long)num2;
					this.TotalCompressedBytesReceived += (long)num2;
					return num2;
				}
				if (this.m_curBlockType == NetworkPackagingLayer.PackageEncoding.Xpress)
				{
					return this.ReadXpressBlock(buf, off, len);
				}
				if (this.m_curBlockType == NetworkPackagingLayer.PackageEncoding.Coconet)
				{
					return this.ReadCoconetBlock(buf, off, len);
				}
				this.m_netChannel.TraceError("Unknown packet encoding: {0}", new object[]
				{
					this.m_curBlockType
				});
				throw new NetworkCorruptDataException(this.PartnerNodeName);
			}
		}

		private int ReadXpressBlock(byte[] buf, int off, int len)
		{
			if (this.m_xpressReadBuf == null)
			{
				this.m_xpressReadBuf = new byte[65536];
			}
			this.ReadChunk(this.m_readPacketHeaderBuf, 0, 4);
			ushort num = BitConverter.ToUInt16(this.m_readPacketHeaderBuf, 0);
			int num2 = (int)(num + 1);
			num = BitConverter.ToUInt16(this.m_readPacketHeaderBuf, 2);
			int num3 = (int)(num + 1);
			this.ReadChunk(this.m_xpressReadBuf, 0, num2);
			bool flag;
			if (num3 <= len)
			{
				flag = Xpress.Decompress(this.m_xpressReadBuf, 0, num2, buf, off, num3);
			}
			else
			{
				if (this.m_decompressBuf == null)
				{
					this.m_decompressBuf = new byte[65536];
				}
				flag = Xpress.Decompress(this.m_xpressReadBuf, 0, num2, this.m_decompressBuf, 0, num3);
			}
			if (!flag)
			{
				NetworkPackagingLayer.Tracer.TraceError((long)this.GetHashCode(), "Decompression failed");
				throw new NetworkCorruptDataException(this.PartnerNodeName);
			}
			this.TotalDecompressedBytesReceived += (long)num3;
			this.TotalCompressedBytesReceived += (long)num2;
			if (num3 <= len)
			{
				this.m_bytesRemainingToReadInCurrentBlock = 0;
				return num3;
			}
			this.m_decompressBufContentLen = num3;
			this.m_bytesRemainingToReadInCurrentBlock = num3 - len;
			Array.Copy(this.m_decompressBuf, 0, buf, off, len);
			NetworkPackagingLayer.Tracer.TraceError<int, int>((long)this.GetHashCode(), "NetPkg.ReadXpressBlock: Had to buffer 0x{0:X} uncompressed bytes for the next fetch since only 0x{1:X} were requested.", this.m_bytesRemainingToReadInCurrentBlock, len);
			return len;
		}

		private int ReadCoconetBlock(byte[] buf, int off, int len)
		{
			if (this.m_coconetDecompressor == null)
			{
				this.m_coconetDecompressor = new CoconetDecompressor(this.CoconetConfig.DictionarySize, (CoconetCompressor.LzOption)this.CoconetConfig.LzOption);
			}
			if (this.m_xpressReadBuf == null || this.m_xpressReadBuf.Length < 1048577)
			{
				this.m_xpressReadBuf = new byte[1048577];
			}
			this.ReadChunk(this.m_readPacketHeaderBuf, 0, 8);
			int num = BitConverter.ToInt32(this.m_readPacketHeaderBuf, 0);
			int num2 = BitConverter.ToInt32(this.m_readPacketHeaderBuf, 4);
			if (num2 > 1048577 || num2 <= 0)
			{
				throw new NetworkCorruptDataException(this.PartnerNodeName);
			}
			if (num > 1048577 || num <= 0)
			{
				throw new NetworkCorruptDataException(this.PartnerNodeName);
			}
			this.ReadChunk(this.m_xpressReadBuf, 0, num);
			ExTraceGlobals.NetShareTracer.TraceDebug<int, int>((long)this.GetHashCode(), "ReadCoconetBlock l={0} c={1}", num2, num);
			if (num2 <= len)
			{
				this.m_coconetDecompressor.Decompress(this.m_xpressReadBuf, 0, num, buf, off, num2);
			}
			else
			{
				if (this.m_decompressBuf == null || this.m_decompressBuf.Length < 1048577)
				{
					this.m_decompressBuf = new byte[1048577];
				}
				this.m_coconetDecompressor.Decompress(this.m_xpressReadBuf, 0, num, this.m_decompressBuf, 0, num2);
			}
			this.TotalDecompressedBytesReceived += (long)num2;
			this.TotalCompressedBytesReceived += (long)num;
			if (num2 <= len)
			{
				this.m_bytesRemainingToReadInCurrentBlock = 0;
				return num2;
			}
			this.m_decompressBufContentLen = num2;
			this.m_bytesRemainingToReadInCurrentBlock = num2 - len;
			Array.Copy(this.m_decompressBuf, 0, buf, off, len);
			NetworkPackagingLayer.Tracer.TraceError<int, int>((long)this.GetHashCode(), "NetPkg.ReadCoconetBlock: Had to buffer 0x{0:X} uncompressed bytes for the next fetch since only 0x{1:X} were requested.", this.m_bytesRemainingToReadInCurrentBlock, len);
			return len;
		}

		public const int RawHeaderSize = 5;

		private const int XpressHeaderSize = 5;

		private const int CoconetHeaderSize = 9;

		private const int CoconetMaxBlockSize = 1048577;

		private const int MinInitialPackageLength = 500;

		private const int minCocoSize = 265;

		private const int MaxBlockLen = 1052672;

		protected TcpChannel m_tcpChannel;

		protected NetworkChannel m_netChannel;

		protected NetworkChannel.DataEncodingScheme m_encoding;

		private byte[] m_readPacketHeaderBuf;

		private bool m_closeCalled;

		protected byte[] m_compressBuf;

		private CoconetCompressor m_coconetCompressor;

		private CoconetDecompressor m_coconetDecompressor;

		private NetworkPackagingLayer.PackageEncoding m_curBlockType;

		private int m_bytesRemainingToReadInCurrentBlock;

		private ManualResetEvent m_asyncReadCompleteEvent;

		private bool m_asyncReadActive;

		private NetworkChannelCallback m_userCallback;

		private object m_userCallbackCtx;

		private Exception m_asyncException;

		private int m_asyncReadByteCount;

		private bool m_asyncReadGotZeroBytes;

		private byte[] m_decompressBuf;

		private int m_decompressBufContentLen;

		private byte[] m_xpressReadBuf;

		public enum PackageEncoding
		{
			Invalid,
			Raw,
			Xpress,
			SerializedException,
			Coconet
		}
	}
}

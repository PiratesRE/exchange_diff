using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class ExchangeDispatch : IExchangeDispatch
	{
		public ExchangeDispatch(IRpcDispatch rpcDispatch)
		{
			this.rpcDispatch = rpcDispatch;
		}

		public int Connect(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, out IntPtr contextHandle, string userDn, int flags, int connectionModulus, int codePage, int stringLocale, int sortLocale, out TimeSpan pollsMax, out int retryCount, out TimeSpan retryDelay, out string dnPrefix, out string displayName, short[] clientVersion, out short[] serverVersion, ArraySegment<byte> segmentExtendedAuxIn, ArraySegment<byte> segmentExtendedAuxOut, out ArraySegment<byte> auxOutData, IStandardBudget budget)
		{
			ExchangeDispatch.BlockCallTestHook();
			contextHandle = IntPtr.Zero;
			pollsMax = TimeSpan.Zero;
			retryCount = 0;
			retryDelay = TimeSpan.Zero;
			dnPrefix = string.Empty;
			displayName = string.Empty;
			serverVersion = ExchangeDispatch.ExchangeServerVersion;
			auxOutData = Array<byte>.EmptySegment;
			IntPtr localContextHandle = IntPtr.Zero;
			TimeSpan localPollsMax = TimeSpan.Zero;
			int localRetryCount = 0;
			TimeSpan localRetryDelay = TimeSpan.Zero;
			string localDnPrefix = null;
			string localDisplayName = null;
			short[] exchangeServerVersion = ExchangeDispatch.ExchangeServerVersion;
			byte[] array = null;
			byte[] array2 = null;
			int num = 0;
			bool flag = false;
			try
			{
				ArraySegment<byte> segmentAuxIn = Array<byte>.EmptySegment;
				if (segmentExtendedAuxIn.Count > 0)
				{
					ArraySegment<byte>? arraySegment = null;
					segmentAuxIn = ExtendedBufferHelper.Unwrap(CompressAndObfuscate.Instance, segmentExtendedAuxIn, ExchangeDispatch.GetRequestAuxSegment(out array), out arraySegment);
					if (arraySegment != null)
					{
						throw new AbortRpcExecutionException("AuxIn buffer can't be chained");
					}
				}
				ArraySegment<byte> segmentAuxOut = ExchangeDispatch.GetResponseAuxSegment(segmentExtendedAuxOut.Count, out array2);
				int localSizeAuxOut = 0;
				if (segmentAuxIn.Array == null)
				{
					throw new InvalidOperationException("Invalid segmentAuxIn ArraySegment; Array can't be null");
				}
				if (segmentAuxOut.Array == null)
				{
					throw new InvalidOperationException("Invalid segmentAuxOut ArraySegment; Array can't be null");
				}
				num = ExchangeDispatch.ExecuteWrapper(delegate
				{
					if (segmentAuxIn.Array == null)
					{
						throw new InvalidOperationException("Wrapper: Invalid segmentAuxIn ArraySegment; Array can't be null");
					}
					if (segmentAuxOut.Array == null)
					{
						throw new InvalidOperationException("Wrapper: Invalid segmentAuxOut ArraySegment; Array can't be null");
					}
					return this.rpcDispatch.Connect(protocolRequestInfo, clientBinding, out localContextHandle, userDn, flags, connectionModulus, codePage, stringLocale, sortLocale, out localPollsMax, out localRetryCount, out localRetryDelay, out localDnPrefix, out localDisplayName, clientVersion, segmentAuxIn, segmentAuxOut, out localSizeAuxOut, budget);
				});
				if (num == 0)
				{
					contextHandle = localContextHandle;
					pollsMax = localPollsMax;
					retryCount = localRetryCount;
					retryDelay = localRetryDelay;
					dnPrefix = localDnPrefix;
					displayName = localDisplayName;
				}
				else if (localContextHandle != IntPtr.Zero)
				{
					throw new ArgumentException("localContextHandle should be zero");
				}
				serverVersion = exchangeServerVersion;
				if (localSizeAuxOut > 0)
				{
					auxOutData = ExtendedBufferHelper.Wrap(CompressAndObfuscate.Instance, segmentExtendedAuxOut, segmentAuxOut.SubSegment(0, localSizeAuxOut), false, false);
				}
				flag = true;
			}
			finally
			{
				if (!flag && contextHandle != IntPtr.Zero)
				{
					this.Disconnect(null, ref contextHandle, true);
				}
				if (array != null)
				{
					AsyncBufferPools.ReleaseBuffer(array);
				}
				if (array2 != null)
				{
					AsyncBufferPools.ReleaseBuffer(array2);
				}
			}
			return num;
		}

		public int Disconnect(ProtocolRequestInfo protocolRequestInfo, ref IntPtr contextHandle, bool rundown)
		{
			IntPtr localContextHandle = contextHandle;
			int result = ExchangeDispatch.ExecuteWrapper(() => this.rpcDispatch.Disconnect(protocolRequestInfo, ref localContextHandle, rundown));
			contextHandle = IntPtr.Zero;
			return result;
		}

		public int Execute(ProtocolRequestInfo protocolRequestInfo, ref IntPtr contextHandle, int flags, ArraySegment<byte> segmentExtendedRopIn, ArraySegment<byte> segmentExtendedRopOut, out ArraySegment<byte> ropOutData, ArraySegment<byte> segmentExtendedAuxIn, ArraySegment<byte> segmentExtendedAuxOut, out ArraySegment<byte> auxOutData)
		{
			IntPtr localContextHandle = contextHandle;
			ropOutData = Array<byte>.EmptySegment;
			auxOutData = Array<byte>.EmptySegment;
			bool compress = (flags & 1) == 0;
			bool xorMagic = (flags & 2) == 0;
			bool flag = (flags & 4) != 0;
			long num = 0L;
			long num2 = 0L;
			long num3 = 0L;
			long num4 = 0L;
			byte[] array = null;
			byte[] array2 = null;
			byte[] array3 = null;
			byte[][] array4 = null;
			int num5 = 0;
			try
			{
				List<ArraySegment<byte>> segmentRopInArray = ExchangeDispatch.BuildRopInputBufferList(segmentExtendedRopIn, out array4, out num, out num2);
				ArraySegment<byte> segmentAuxIn = Array<byte>.EmptySegment;
				if (segmentExtendedAuxIn.Count > 0)
				{
					ArraySegment<byte>? arraySegment = null;
					long num6;
					long num7;
					segmentAuxIn = ExtendedBufferHelper.Unwrap(CompressAndObfuscate.Instance, segmentExtendedAuxIn, ExchangeDispatch.GetRequestAuxSegment(out array), out arraySegment, out num6, out num7);
					num += num6;
					num2 += num7;
					if (arraySegment != null)
					{
						throw new AbortRpcExecutionException("AuxIn buffer can't be chained");
					}
				}
				this.rpcDispatch.ReportBytesRead(num, num2);
				int num8 = ExchangeDispatch.SizeSubtractAndCap(segmentExtendedRopOut.Count, 0, EmsmdbConstants.MaxChainedExtendedRopBufferSize);
				int num9 = 0;
				int num10 = 0;
				ArraySegment<byte> segmentAuxOut = ExchangeDispatch.GetResponseAuxSegment(segmentExtendedAuxOut.Count, out array2);
				bool fake = false;
				int num11 = 0;
				for (;;)
				{
					if (array3 != null)
					{
						AsyncBufferPools.ReleaseBuffer(array3);
						array3 = null;
					}
					int num12 = num8 - num9;
					ArraySegment<byte> segmentRopOut = ExchangeDispatch.GetResponseRopSegment(num12, out array3);
					num12 = segmentRopOut.Count;
					if (num12 == 0)
					{
						goto IL_3A7;
					}
					int localSizeRopOut = 0;
					int localSizeAuxOut = 0;
					byte[] fakeOut = null;
					if (segmentRopOut.Array == null)
					{
						break;
					}
					if (segmentAuxIn.Array == null)
					{
						goto Block_8;
					}
					if (segmentAuxOut.Array == null)
					{
						goto Block_9;
					}
					num5 = ExchangeDispatch.ExecuteWrapper(delegate
					{
						if (segmentRopOut.Array == null)
						{
							throw new InvalidOperationException("Wrapper: Invalid localSegmentRopOut ArraySegment; Array can't be null");
						}
						if (segmentAuxIn.Array == null)
						{
							throw new InvalidOperationException("Wrapper: Invalid segmentAuxIn ArraySegment; Array can't be null");
						}
						if (segmentAuxOut.Array == null)
						{
							throw new InvalidOperationException("Wrapper: Invalid segmentAuxOut ArraySegment; Array can't be null");
						}
						return this.rpcDispatch.Execute(protocolRequestInfo, ref localContextHandle, segmentRopInArray.ToArray(), segmentRopOut, out localSizeRopOut, segmentAuxIn, segmentAuxOut, out localSizeAuxOut, fake, out fakeOut);
					});
					long num6;
					long num7;
					if (!fake)
					{
						if (localSizeAuxOut > 0)
						{
							auxOutData = ExtendedBufferHelper.Wrap(CompressAndObfuscate.Instance, segmentExtendedAuxOut, segmentAuxOut.SubSegment(0, localSizeAuxOut), false, false, out num6, out num7);
							num3 += num6;
							num4 += num7;
						}
					}
					else if (localSizeRopOut == 0)
					{
						goto IL_3A7;
					}
					if (num5 != 0)
					{
						goto Block_12;
					}
					ArraySegment<byte> arraySegment2 = ExtendedBufferHelper.Wrap(CompressAndObfuscate.Instance, segmentExtendedRopOut.SubSegmentToEnd(num9), segmentRopOut.SubSegment(0, localSizeRopOut), compress, xorMagic, out num6, out num7);
					num3 += num6;
					num4 += num7;
					if (num9 > 0)
					{
						ExtendedBufferHelper.ClearLastFlag(segmentExtendedRopOut.SubSegment(num10, num9 - num10));
					}
					num10 = num9;
					num9 += arraySegment2.Count;
					num12 = num8 - num9;
					num11++;
					if (!flag || (uint)contextHandle.ToInt64() == 0U || fakeOut == null || num11 >= EmsmdbConstants.MaxChainBuffers)
					{
						goto IL_3A7;
					}
					RopId ropId = (RopId)fakeOut[2];
					int num13;
					if (ropId != RopId.QueryRows)
					{
						if (ropId != RopId.FastTransferSourceGetBuffer && ropId != RopId.FastTransferSourceGetBufferExtended)
						{
							num13 = EmsmdbConstants.MinChainSize;
						}
						else
						{
							num13 = EmsmdbConstants.MinFastTransferChainSize;
						}
					}
					else
					{
						num13 = EmsmdbConstants.MinQueryRowsChainSize;
					}
					if (num12 < num13)
					{
						goto IL_3A7;
					}
					segmentAuxIn = Array<byte>.EmptySegment;
					segmentAuxOut = Array<byte>.EmptySegment;
					if (array2 != null)
					{
						AsyncBufferPools.ReleaseBuffer(array2);
						array2 = null;
					}
					segmentRopInArray.Clear();
					segmentRopInArray.Add(new ArraySegment<byte>(fakeOut));
					fakeOut = null;
					fake = true;
				}
				throw new InvalidOperationException("Invalid localSegmentRopOut ArraySegment; Array can't be null");
				Block_8:
				throw new InvalidOperationException("Invalid segmentAuxIn ArraySegment; Array can't be null");
				Block_9:
				throw new InvalidOperationException("Invalid segmentAuxOut ArraySegment; Array can't be null");
				Block_12:
				num9 = 0;
				IL_3A7:
				if (num9 > 0)
				{
					ropOutData = segmentExtendedRopOut.SubSegment(0, num9);
				}
				this.rpcDispatch.ReportBytesWritten(num3, num4);
				contextHandle = localContextHandle;
			}
			finally
			{
				if (array4 != null)
				{
					foreach (byte[] array6 in array4)
					{
						if (array6 != null)
						{
							AsyncBufferPools.ReleaseBuffer(array6);
						}
					}
				}
				if (array3 != null)
				{
					AsyncBufferPools.ReleaseBuffer(array3);
					array3 = null;
				}
				if (array != null)
				{
					AsyncBufferPools.ReleaseBuffer(array);
				}
				if (array2 != null)
				{
					AsyncBufferPools.ReleaseBuffer(array2);
				}
			}
			return num5;
		}

		public int NotificationConnect(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, out IntPtr asynchronousContextHandle)
		{
			IntPtr localAsynchronousContextHandle = IntPtr.Zero;
			int result = ExchangeDispatch.ExecuteWrapper(() => this.rpcDispatch.NotificationConnect(protocolRequestInfo, contextHandle, out localAsynchronousContextHandle));
			asynchronousContextHandle = localAsynchronousContextHandle;
			return result;
		}

		public void NotificationWait(ProtocolRequestInfo protocolRequestInfo, IntPtr asynchronousContextHandle, int flags, Action<bool, int> completion)
		{
			int num = ExchangeDispatch.ExecuteWrapper(delegate
			{
				this.rpcDispatch.NotificationWait(protocolRequestInfo, asynchronousContextHandle, (uint)flags, completion);
				return 0;
			});
			if (num != 0)
			{
				completion(false, num);
			}
		}

		public int RegisterPushNotification(ProtocolRequestInfo protocolRequestInfo, ref IntPtr contextHandle, ArraySegment<byte> segmentContext, int adviseBits, ArraySegment<byte> segmentClientBlob, out int notificationHandle)
		{
			notificationHandle = 0;
			return -2147221246;
		}

		public int UnregisterPushNotification(ProtocolRequestInfo protocolRequestInfo, ref IntPtr contextHandle, int notificationHandle)
		{
			return -2147221246;
		}

		public int Dummy(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding)
		{
			return ExchangeDispatch.ExecuteWrapper(() => this.rpcDispatch.Dummy(protocolRequestInfo, clientBinding));
		}

		public void DroppedConnection(IntPtr asynchronousContextHandle)
		{
			ExchangeDispatch.ExecuteWrapper(delegate
			{
				this.rpcDispatch.DroppedConnection(asynchronousContextHandle);
				return 0;
			});
		}

		internal static ArraySegment<byte> GetRequestAuxSegment(out byte[] buffer)
		{
			buffer = AsyncBufferPools.GetBuffer(EmsmdbConstants.MaxAuxBufferSize);
			return new ArraySegment<byte>(buffer, 0, EmsmdbConstants.MaxAuxBufferSize);
		}

		internal static ArraySegment<byte> GetRequestRopSegment(out byte[] buffer)
		{
			buffer = AsyncBufferPools.GetBuffer(EmsmdbConstants.MaxRopBufferSize);
			return new ArraySegment<byte>(buffer, 0, EmsmdbConstants.MaxRopBufferSize);
		}

		internal static ArraySegment<byte> GetResponseAuxSegment(int size, out byte[] buffer)
		{
			int num = ExchangeDispatch.SizeSubtractAndCap(size, 8, EmsmdbConstants.MaxAuxBufferSize);
			buffer = AsyncBufferPools.GetBuffer(num);
			return new ArraySegment<byte>(buffer, 0, num);
		}

		internal static ArraySegment<byte> GetResponseRopSegment(int size, out byte[] buffer)
		{
			int num = ExchangeDispatch.SizeSubtractAndCap(size, 8, EmsmdbConstants.MaxRopBufferSize);
			buffer = AsyncBufferPools.GetBuffer(num);
			return new ArraySegment<byte>(buffer, 0, num);
		}

		internal static List<ArraySegment<byte>> BuildRopInputBufferList(ArraySegment<byte> segmentExtendedRopIn, out byte[][] ropInArray, out long rawBytesRead, out long uncompressedBytesRead)
		{
			List<ArraySegment<byte>> list = null;
			List<byte[]> list2 = null;
			byte[] array = null;
			bool flag = false;
			List<ArraySegment<byte>> result;
			try
			{
				rawBytesRead = 0L;
				uncompressedBytesRead = 0L;
				ArraySegment<byte>? arraySegment = new ArraySegment<byte>?(segmentExtendedRopIn);
				int num = 0;
				while (num < EmsmdbConstants.MaxChainBuffers && arraySegment != null)
				{
					ArraySegment<byte>? arraySegment2 = null;
					long num2;
					long num3;
					ArraySegment<byte> item = ExtendedBufferHelper.Unwrap(CompressAndObfuscate.Instance, arraySegment.Value, ExchangeDispatch.GetRequestRopSegment(out array), out arraySegment2, out num2, out num3);
					rawBytesRead += num2;
					uncompressedBytesRead += num3;
					num++;
					if (num == EmsmdbConstants.MaxChainBuffers && arraySegment2 != null)
					{
						throw new AbortRpcExecutionException("In buffer exceeded maximum number of chains");
					}
					if (list2 == null)
					{
						if (arraySegment2 != null)
						{
							list2 = new List<byte[]>(EmsmdbConstants.MaxChainBuffers);
							list = new List<ArraySegment<byte>>(EmsmdbConstants.MaxChainBuffers);
						}
						else
						{
							list2 = new List<byte[]>(1);
							list = new List<ArraySegment<byte>>(1);
						}
					}
					list2.Add(array);
					array = null;
					list.Add(item);
					arraySegment = arraySegment2;
				}
				ropInArray = list2.ToArray();
				flag = true;
				result = list;
			}
			finally
			{
				if (!flag)
				{
					if (array != null)
					{
						AsyncBufferPools.ReleaseBuffer(array);
					}
					if (list2 != null)
					{
						foreach (byte[] buffer in list2)
						{
							AsyncBufferPools.ReleaseBuffer(buffer);
						}
					}
				}
			}
			return result;
		}

		private static int ExecuteWrapper(Func<int> executeDelegate)
		{
			int result;
			try
			{
				result = executeDelegate();
			}
			catch (RpcServerException ex)
			{
				result = (int)ex.StoreError;
			}
			return result;
		}

		private static int SizeSubtractAndCap(int minuend, int subtrahend, int maxResult)
		{
			if (minuend <= subtrahend)
			{
				return 0;
			}
			int num = minuend - subtrahend;
			if (num > maxResult)
			{
				return maxResult;
			}
			return num;
		}

		private static void BlockCallTestHook()
		{
			int num = 0;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(3007720765U, ref num);
			if (num == 0)
			{
				return;
			}
			if (ExchangeDispatch.isCallBlocking)
			{
				return;
			}
			ExchangeDispatch.isCallBlocking = true;
			try
			{
				ExDateTime now = ExDateTime.Now;
				while (num != 0 && ExDateTime.Now - now < TimeSpan.FromMinutes(1.0))
				{
					ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(3007720765U, ref num);
					Thread.Sleep(500);
				}
			}
			finally
			{
				ExchangeDispatch.isCallBlocking = false;
			}
		}

		public static readonly short[] ExchangeServerVersion = new short[]
		{
			15,
			0,
			1497,
			12
		};

		private static bool isCallBlocking = false;

		private readonly IRpcDispatch rpcDispatch;
	}
}

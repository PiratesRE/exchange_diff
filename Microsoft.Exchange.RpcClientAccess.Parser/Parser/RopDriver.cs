using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal class RopDriver : BaseObject, IRopDriver, IDisposable
	{
		internal RopDriver(IConnectionHandler handler, IConnectionInformation connection)
		{
			Util.ThrowOnNullArgument(handler, "handler");
			Util.ThrowOnNullArgument(connection, "connection");
			this.handler = handler;
			this.connection = connection;
			this.logonMap = new LogonMap(handler);
			this.ropExecutionHistory = new RopExecutionHistory(200U);
		}

		public IRopHandler RopHandler
		{
			get
			{
				return this.handler.RopHandler;
			}
		}

		public INotificationHandler NotificationHandler
		{
			get
			{
				return this.handler.NotificationHandler;
			}
		}

		internal LogonMap LogonMap
		{
			get
			{
				return this.logonMap;
			}
		}

		internal RopProcessingCallbackDelegate OnBeforeRopExecuted
		{
			get
			{
				return this.onBeforeRopExecuted;
			}
			set
			{
				this.onBeforeRopExecuted = value;
			}
		}

		internal RopProcessingCallbackDelegate OnAfterRopExecuted
		{
			get
			{
				return this.onAfterRopExecuted;
			}
			set
			{
				this.onAfterRopExecuted = value;
			}
		}

		public Func<bool> IsMimumResponseSizeEnforcementEnabled
		{
			set
			{
				this.isMimumResponseSizeEnforcementEnabled = value;
			}
		}

		public static byte[] CreateFakeRopRequest(Rop rop, ServerObjectHandleTable serverObjectHandleTable)
		{
			return BufferWriter.Serialize(delegate(Writer writer)
			{
				Encoding asciiEncoding = CTSGlobals.AsciiEncoding;
				RopDriver.WriteFakeRopRequest(rop, writer, serverObjectHandleTable, asciiEncoding);
			});
		}

		public void Execute(IList<ArraySegment<byte>> inputBufferArray, ArraySegment<byte> outputBuffer, out int outputSize, AuxiliaryData auxiliaryData, bool isFake, out byte[] fakeOut)
		{
			if (inputBufferArray == null)
			{
				throw new ArgumentNullException("inputBufferArray");
			}
			if (inputBufferArray.Count == 0)
			{
				throw new ArgumentException("No input buffers");
			}
			if (outputBuffer.Count > 32767)
			{
				throw new ArgumentException("The output buffer is bigger than the MaxOutputBufferSize.");
			}
			List<ArraySegment<byte>> list = new List<ArraySegment<byte>>(1);
			ServerObjectHandleTable serverObjectHandleTable;
			int num2;
			using (Reader reader = Reader.CreateBufferReader(inputBufferArray[0]))
			{
				ushort num = reader.ReadUInt16();
				if (num < 2 || (int)num > inputBufferArray[0].Count)
				{
					throw new BufferParseException(string.Format("Invalid inputRopsSize found = {0}. Input buffer size = {1}.", num, inputBufferArray[0].Count));
				}
				reader.Position = (long)((ulong)num);
				serverObjectHandleTable = ServerObjectHandleTable.Parse(reader);
				num2 = inputBufferArray[0].Count - (int)num;
			}
			for (int i = 0; i < inputBufferArray.Count; i++)
			{
				using (Reader reader2 = Reader.CreateBufferReader(inputBufferArray[i]))
				{
					ushort num3 = reader2.ReadUInt16();
					if (num3 < 2 || (int)num3 > inputBufferArray[i].Count)
					{
						throw new BufferParseException(string.Format("Invalid inputRopsSize found = {0}. Input buffer size = {1}.", num3, inputBufferArray[i].Count));
					}
					if (i > 0 && (int)num3 != inputBufferArray[i].Count)
					{
						throw new BufferParseException(string.Format("Subsequent buffers cannot have SOHT table. Invalid inputRopsSize found = {0}. Input buffer size = {1}.", num3, inputBufferArray[i].Count));
					}
					list.Add(inputBufferArray[i].SubSegment(2, (int)(num3 - 2)));
				}
			}
			this.handler.BeginRopProcessing(auxiliaryData);
			int num4;
			this.ExecuteRops(list, serverObjectHandleTable, outputBuffer, 2, outputBuffer.Count - 2 - num2, outputBuffer.Count == 32767, out num4, auxiliaryData, isFake, out fakeOut);
			this.handler.EndRopProcessing(auxiliaryData);
			if (num4 >= 0)
			{
				num4 += 2;
				using (Writer writer = new BufferWriter(outputBuffer))
				{
					writer.WriteUInt16((ushort)num4);
					writer.Position = (long)num4;
					serverObjectHandleTable.Serialize(writer);
					outputSize = (int)writer.Position;
					return;
				}
			}
			outputSize = 0;
		}

		public bool TryGetServerObjectMap(byte logonIndex, out ServerObjectMap serverObjectMap, out ErrorCode errorCode)
		{
			return this.LogonMap.TryGetServerObjectMap(logonIndex, out serverObjectMap, out errorCode);
		}

		public bool TryGetServerObject(byte logonIndex, ServerObjectHandle handle, out IServerObject serverObject, out ErrorCode errorCode)
		{
			return this.LogonMap.TryGetServerObject(logonIndex, handle, out serverObject, out errorCode);
		}

		public ServerObjectMap CreateLogon(byte logonIndex, LogonFlags logonFlags)
		{
			return this.LogonMap.CreateLogon(logonIndex, logonFlags);
		}

		public void ReleaseHandle(byte logonIndex, ServerObjectHandle handleToRelease)
		{
			this.LogonMap.ReleaseHandle(logonIndex, handleToRelease);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RopDriver>(this);
		}

		protected override void InternalDispose()
		{
			this.logonMap.Dispose();
			base.InternalDispose();
		}

		private static void WriteFakeRopRequest(Rop rop, Writer writer, ServerObjectHandleTable serverObjectHandleTable, Encoding string8Encoding)
		{
			writer.WriteUInt16(0);
			rop.SerializeInput(writer, string8Encoding);
			long position = writer.Position;
			writer.Position = 0L;
			writer.WriteUInt16((ushort)position);
			writer.Position = position;
			serverObjectHandleTable.Serialize(writer);
		}

		private static void AppendRopBufferTooSmall(IConnectionInformation connection, ArraySegment<byte> inputBuffer, int indexOnInputBufferOfNextRopToExecute, ArraySegment<byte> outputBuffer, int outputIndex, ref int outputSize, int maxOutputSize, ServerObjectHandleTable serverObjectHandleTable)
		{
			ArraySegment<byte> unexecutedBuffer = inputBuffer.SubSegmentToEnd(indexOnInputBufferOfNextRopToExecute);
			BufferTooSmallResult bufferTooSmallResult = new BufferTooSmallResult(32767, unexecutedBuffer, connection.String8Encoding);
			using (Writer writer = new BufferWriter(outputBuffer.SubSegment(outputIndex + outputSize, maxOutputSize - outputSize)))
			{
				bufferTooSmallResult.Serialize(writer);
				outputSize += (int)writer.Position;
			}
			serverObjectHandleTable.MarkLastHandle();
		}

		private static void AppendRopBackoff(IConnectionInformation connection, BackoffInformation backoffInformation, ArraySegment<byte> inputBuffer, int indexOnInputBufferOfNextRopToExecute, ArraySegment<byte> outputBuffer, int outputIndex, ref int outputSize, int maxOutputSize, ServerObjectHandleTable serverObjectHandleTable)
		{
			if (backoffInformation == null)
			{
				throw new ArgumentNullException("backoffInformation");
			}
			BackoffResult backoffResult = new BackoffResult(backoffInformation.LogonId, backoffInformation.Duration, backoffInformation.BackoffRopData, backoffInformation.AdditionalData, connection.String8Encoding);
			int num = 0;
			using (Writer writer = new CountWriter())
			{
				backoffResult.Serialize(writer);
				num = (int)writer.Position;
			}
			ArraySegment<byte> buffer = outputBuffer.SubSegment(outputIndex + outputSize, maxOutputSize - outputSize);
			if (num > buffer.Count)
			{
				RopDriver.AppendRopBufferTooSmall(connection, inputBuffer, indexOnInputBufferOfNextRopToExecute, outputBuffer, outputIndex, ref outputSize, maxOutputSize, serverObjectHandleTable);
				return;
			}
			using (Writer writer2 = new BufferWriter(buffer))
			{
				backoffResult.Serialize(writer2);
				outputSize += (int)writer2.Position;
			}
			serverObjectHandleTable.MarkLastHandle();
		}

		private List<Rop> ParseRops(List<ArraySegment<byte>> inputArraySegmentList, ServerObjectHandleTable serverObjectHandleTable)
		{
			List<Rop> list = null;
			List<Rop> list2 = null;
			this.logonMap.ParseBegin(serverObjectHandleTable);
			try
			{
				foreach (ArraySegment<byte> arraySegment in inputArraySegmentList)
				{
					if (list2 != null)
					{
						list2.Clear();
					}
					RopFactory.CreateRops(arraySegment.Array, arraySegment.Offset, arraySegment.Count, this.logonMap, serverObjectHandleTable, ref list2);
					if (list == null)
					{
						list = list2;
						list2 = null;
					}
					else
					{
						if (list.Count < 1)
						{
							throw new BufferParseException("Can only have chained Rop when previous input Rop buffer is not empty");
						}
						Rop rop = list[list.Count - 1];
						if (list2.Count != 1)
						{
							throw new BufferParseException("Can only have one chained Rop per subsequent input Rop buffer");
						}
						rop.MergeChainedData(list2[0]);
					}
				}
			}
			finally
			{
				this.logonMap.ParseEnd();
			}
			return list;
		}

		private void ExecuteRops(List<ArraySegment<byte>> inputArraySegmentList, ServerObjectHandleTable serverObjectHandleTable, ArraySegment<byte> outputBuffer, int outputIndex, int maxOutputSize, bool isOutputBufferMaxSize, out int outputSize, AuxiliaryData auxiliaryData, bool isFake, out byte[] fakeOut)
		{
			if (inputArraySegmentList == null)
			{
				throw new ArgumentNullException("inputArraySegmentList");
			}
			if (inputArraySegmentList.Count == 0)
			{
				throw new ArgumentException("inputArraySegmentList length is zero");
			}
			fakeOut = null;
			if ((inputArraySegmentList[0].Count != 0 || maxOutputSize < 2) && maxOutputSize < inputArraySegmentList[0].Count + BufferTooSmallResult.HeaderSize)
			{
				throw new BufferTooSmallException();
			}
			List<Rop> list = this.ParseRops(inputArraySegmentList, serverObjectHandleTable);
			int num = 0;
			outputSize = 0;
			int num2 = 0;
			bool flag = list != null && list.Count > 1;
			try
			{
				Rop rop3 = null;
				if (list != null && list.Count > 0)
				{
					this.handler.LogInputRops(from rop in list
					select rop.RopId);
					foreach (Rop rop2 in list)
					{
						rop3 = rop2;
						int num3 = inputArraySegmentList[0].Count - num - rop2.InputBufferSize;
						int num4 = maxOutputSize - outputSize - BufferTooSmallResult.HeaderSize - num3;
						ArraySegment<byte> arraySegment = outputBuffer.SubSegment(outputIndex + outputSize, num4);
						if (rop2.MinimumResponseSizeRequired != null && num4 < rop2.MinimumResponseSizeRequired.Value && this.isMimumResponseSizeEnforcementEnabled != null && this.isMimumResponseSizeEnforcementEnabled())
						{
							throw new BufferTooSmallException();
						}
						try
						{
							this.handler.LogPrepareForRop(rop2.RopId);
							if (this.OnBeforeRopExecuted != null)
							{
								this.OnBeforeRopExecuted(rop2, serverObjectHandleTable);
							}
							this.ropExecutionHistory.OnBeforeRopExecution(rop2, serverObjectHandleTable);
							rop2.Execute(this.connection, this, serverObjectHandleTable, arraySegment);
							this.ropExecutionHistory.OnAfterRopExecution(rop2, this, serverObjectHandleTable);
							using (Writer writer = new BufferWriter(arraySegment))
							{
								rop2.SerializeOutput(writer);
								if (rop2.Result != null)
								{
									long maxSize = (long)arraySegment.Count - writer.Position;
									DiagnosticContextResult diagnosticContextResult = rop2.Result.GetDiagnosticContextResult(maxSize);
									if (diagnosticContextResult != null)
									{
										diagnosticContextResult.String8Encoding = rop2.Result.String8Encoding;
										diagnosticContextResult.Serialize(writer);
									}
								}
								outputSize += (int)writer.Position;
								num += rop2.InputBufferSize;
							}
						}
						finally
						{
							if (this.OnAfterRopExecuted != null)
							{
								this.OnAfterRopExecuted(rop2, serverObjectHandleTable);
							}
							if (rop2.Result != null && rop2.Result.Exception != null && !isFake)
							{
								auxiliaryData.AppendOutput(new ExceptionTraceAuxiliaryBlock((uint)num2, rop2.Result.Exception.ToString()));
							}
							this.handler.LogCompletedRop(rop2.RopId, (rop2.Result != null) ? rop2.Result.ErrorCode : ErrorCode.None);
						}
						num2++;
						if ((rop2.RopId == RopId.SeekRowBookmark || rop2.RopId == RopId.SeekRowApproximate || rop2.RopId == RopId.SeekRow) && rop2.Result != null && rop2.Result.ErrorCode != ErrorCode.None)
						{
							rop3 = null;
							break;
						}
					}
				}
				if (rop3 != null && rop3.Result != null && rop3.Result.ErrorCode == ErrorCode.None)
				{
					fakeOut = rop3.CreateFakeRopRequest(rop3.Result, serverObjectHandleTable);
				}
				if (!isFake && fakeOut == null && this.NotificationHandler.HasPendingNotifications() && maxOutputSize - outputSize >= PendingResult.Size)
				{
					using (Writer writer2 = new BufferWriter(outputBuffer.SubSegment(outputIndex + outputSize, maxOutputSize - outputSize)))
					{
						using (NotificationCollector notificationCollector = new NotificationCollector(maxOutputSize - outputSize - PendingResult.Size))
						{
							this.NotificationHandler.CollectNotifications(notificationCollector);
							foreach (NotifyResult notifyResult in notificationCollector.NotifyResults)
							{
								notifyResult.Serialize(writer2);
							}
						}
						if (this.NotificationHandler.HasPendingNotifications())
						{
							PendingResult pendingResult = new PendingResult(this.connection.SessionId, this.connection.String8Encoding);
							pendingResult.Serialize(writer2);
						}
						outputSize += (int)writer2.Position;
					}
				}
			}
			catch (BufferTooSmallException)
			{
				if (isOutputBufferMaxSize && num2 == 0 && (!this.connection.ClientSupportsBufferTooSmallBreakup || !flag))
				{
					throw;
				}
				RopDriver.AppendRopBufferTooSmall(this.connection, inputArraySegmentList[0], num, outputBuffer, outputIndex, ref outputSize, maxOutputSize, serverObjectHandleTable);
				this.handler.LogCompletedRop(RopId.BufferTooSmall, ErrorCode.BufferTooSmall);
			}
			catch (ClientBackoffException ex)
			{
				if (!isFake)
				{
					if (ex.IsSpecificBackoff && this.connection.ClientSupportsBackoffResult)
					{
						RopDriver.AppendRopBackoff(this.connection, ex.BackoffInformation, inputArraySegmentList[0], num, outputBuffer, outputIndex, ref outputSize, maxOutputSize, serverObjectHandleTable);
						this.handler.LogCompletedRop(RopId.Backoff, ErrorCode.ServerBusy);
					}
					else
					{
						if (num2 == 0)
						{
							throw;
						}
						RopDriver.AppendRopBufferTooSmall(this.connection, inputArraySegmentList[0], num, outputBuffer, outputIndex, ref outputSize, maxOutputSize, serverObjectHandleTable);
						this.handler.LogCompletedRop(RopId.BufferTooSmall, ErrorCode.ServerBusy);
					}
				}
				else
				{
					outputSize = -1;
				}
			}
		}

		internal const ushort MaxOutputBufferSize = 32767;

		private const int RopSizeSize = 2;

		private const uint RopExecutionHistoryLength = 200U;

		private readonly IConnectionHandler handler;

		private readonly IConnectionInformation connection;

		private readonly RopExecutionHistory ropExecutionHistory;

		private LogonMap logonMap;

		private RopProcessingCallbackDelegate onBeforeRopExecuted;

		private RopProcessingCallbackDelegate onAfterRopExecuted;

		private Func<bool> isMimumResponseSizeEnforcementEnabled;
	}
}

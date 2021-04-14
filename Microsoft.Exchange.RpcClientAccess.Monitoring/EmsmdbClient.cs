using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MapiHttp;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ExchangeClient;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EmsmdbClient : BaseObject, IEmsmdbClient, IRpcClient, IDisposable
	{
		public EmsmdbClient(RpcBindingInfo bindingInfo)
		{
			this.protocolClient = new ExchangeAsyncRpcClient(bindingInfo);
			this.connectionUriString = bindingInfo.Uri.ToString();
		}

		public EmsmdbClient(MapiHttpBindingInfo bindingInfo)
		{
			EmsmdbHttpClient emsmdbHttpClient = new EmsmdbHttpClient(bindingInfo);
			this.protocolClient = emsmdbHttpClient;
			string vdirPath = emsmdbHttpClient.VdirPath;
			this.connectionUriString = bindingInfo.BuildRequestPath(ref vdirPath);
		}

		public string BindingString
		{
			get
			{
				RpcClientBase rpcClientBase = this.protocolClient as RpcClientBase;
				if (rpcClientBase != null)
				{
					return rpcClientBase.GetBindingString();
				}
				return "GetBindingString() not implemented";
			}
		}

		public IExchangeAsyncDispatch ProtocolClient
		{
			get
			{
				return this.protocolClient;
			}
		}

		public IAsyncResult BeginConnect(string userDn, TimeSpan timeout, bool useMonitoringContext, AsyncCallback asyncCallback, object asyncState)
		{
			if (this.contextHandle != IntPtr.Zero)
			{
				throw new InvalidOperationException("BeginConnect cannot be issued more than once per client instance");
			}
			return new EmsmdbClient.ConnectCallContext(this.ProtocolClient, userDn, timeout, useMonitoringContext, asyncCallback, asyncState).Begin();
		}

		public ConnectCallResult EndConnect(IAsyncResult asyncResult)
		{
			return ((EmsmdbClient.ConnectCallContext)asyncResult).End(asyncResult, out this.contextHandle);
		}

		public IAsyncResult BeginLogon(string mailboxDn, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState)
		{
			if (this.contextHandle == IntPtr.Zero)
			{
				throw new InvalidOperationException("BeginLogon cannot be issued without a successful connection");
			}
			return new EmsmdbClient.PrivateLogonCallContext(this.ProtocolClient, this.contextHandle, mailboxDn, timeout, asyncCallback, asyncState).Begin();
		}

		public IAsyncResult BeginPublicLogon(string mailboxDn, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState)
		{
			if (this.contextHandle == IntPtr.Zero)
			{
				throw new InvalidOperationException("BeginPublicLogon cannot be issued without a successful connection");
			}
			EmsmdbClient.PublicLogonCallContext publicLogonCallContext = new EmsmdbClient.PublicLogonCallContext(this.ProtocolClient, this.contextHandle, mailboxDn, timeout, asyncCallback, asyncState);
			return publicLogonCallContext.Begin();
		}

		public LogonCallResult EndLogon(IAsyncResult asyncResult)
		{
			return ((EmsmdbClient.PrivateLogonCallContext)asyncResult).End(asyncResult, out this.contextHandle);
		}

		public LogonCallResult EndPublicLogon(IAsyncResult asyncResult)
		{
			return ((EmsmdbClient.PublicLogonCallContext)asyncResult).End(asyncResult, out this.contextHandle);
		}

		public IAsyncResult BeginDummy(TimeSpan timeout, AsyncCallback asyncCallback, object asyncState)
		{
			return new EmsmdbClient.DummyCallContext(this.protocolClient, timeout, asyncCallback, asyncState).Begin();
		}

		public DummyCallResult EndDummy(IAsyncResult asyncResult)
		{
			return ((EmsmdbClient.DummyCallContext)asyncResult).End(asyncResult);
		}

		public string GetConnectionUriString()
		{
			return this.connectionUriString;
		}

		protected sealed override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<EmsmdbClient>(this);
		}

		protected sealed override void InternalDispose()
		{
			if (this.contextHandle != IntPtr.Zero)
			{
				this.ProtocolClient.EndDisconnect(this.ProtocolClient.BeginDisconnect(null, this.contextHandle, null, null), out this.contextHandle);
			}
			IDisposable disposable = this.ProtocolClient as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
			base.InternalDispose();
		}

		private readonly IExchangeAsyncDispatch protocolClient;

		private readonly string connectionUriString;

		private IntPtr contextHandle = IntPtr.Zero;

		private class ConnectCallContext : EmsmdbCallContext<ConnectCallResult>
		{
			public ConnectCallContext(IExchangeAsyncDispatch protocolClient, string userDn, TimeSpan timeout, bool useMonitoringContext, AsyncCallback asyncCallback, object asyncState) : base(protocolClient, timeout, asyncCallback, asyncState)
			{
				Util.ThrowOnNullOrEmptyArgument(userDn, "userDn");
				this.userDn = userDn;
				this.useMonitoringContext = useMonitoringContext;
			}

			public ConnectCallResult End(IAsyncResult asyncResult, out IntPtr contextHandle)
			{
				contextHandle = this.contextHandle;
				return base.GetResult();
			}

			internal static void ParseAuxOut(ArraySegment<byte> segmentExtendedAuxOut, out MonitoringActivityAuxiliaryBlock monitoringActivityAuxiliaryBlock, out ExceptionTraceAuxiliaryBlock exceptionTraceAuxiliaryBlock)
			{
				Util.ThrowOnNullArgument(segmentExtendedAuxOut, "segmentExtendedAuxOut");
				monitoringActivityAuxiliaryBlock = null;
				exceptionTraceAuxiliaryBlock = null;
				if (segmentExtendedAuxOut.Count > 0)
				{
					byte[] buffer = AsyncBufferPools.GetBuffer(EmsmdbConstants.MaxAuxBufferSize);
					ArraySegment<byte> arraySegment = new ArraySegment<byte>(buffer, 0, EmsmdbConstants.MaxAuxBufferSize);
					try
					{
						ArraySegment<byte>? arraySegment2 = null;
						arraySegment = ExtendedBufferHelper.Unwrap(CompressAndObfuscate.Instance, segmentExtendedAuxOut, arraySegment, out arraySegment2);
						if (arraySegment.Count > 0)
						{
							monitoringActivityAuxiliaryBlock = RopClient.ParseAuxiliaryBuffer(arraySegment).OfType<MonitoringActivityAuxiliaryBlock>().FirstOrDefault<MonitoringActivityAuxiliaryBlock>();
							exceptionTraceAuxiliaryBlock = RopClient.ParseAuxiliaryBuffer(arraySegment).OfType<ExceptionTraceAuxiliaryBlock>().FirstOrDefault<ExceptionTraceAuxiliaryBlock>();
						}
					}
					finally
					{
						if (buffer != null)
						{
							AsyncBufferPools.ReleaseBuffer(buffer);
						}
					}
				}
			}

			protected override ICancelableAsyncResult OnBegin(CancelableAsyncCallback asyncCallback, object asyncState)
			{
				bool flag = false;
				ICancelableAsyncResult result;
				try
				{
					MapiVersion outlook = MapiVersion.Outlook15;
					this.auxIn = AsyncBufferPools.GetBuffer(EmsmdbConstants.MaxExtendedAuxBufferSize);
					this.auxOut = AsyncBufferPools.GetBuffer(EmsmdbConstants.MaxExtendedAuxBufferSize);
					ArraySegment<byte> segmentExtendedAuxIn = this.BuildAuxInBuffer(new ArraySegment<byte>(this.auxIn, 0, EmsmdbConstants.MaxExtendedAuxBufferSize));
					ArraySegment<byte> segmentExtendedAuxOut = new ArraySegment<byte>(this.auxOut, 0, EmsmdbConstants.MaxExtendedAuxBufferSize);
					ICancelableAsyncResult cancelableAsyncResult = base.ProtocolClient.BeginConnect(null, null, this.userDn, 0, 0, Encoding.ASCII.CodePage, 0, 0, (from x in outlook.ToQuartet()
					select (short)x).ToArray<short>(), segmentExtendedAuxIn, segmentExtendedAuxOut, asyncCallback, asyncState);
					base.StartRpcTimer();
					flag = true;
					result = cancelableAsyncResult;
				}
				finally
				{
					if (!flag)
					{
						this.Cleanup();
					}
				}
				return result;
			}

			protected override ConnectCallResult OnEnd(ICancelableAsyncResult asyncResult)
			{
				ConnectCallResult result;
				try
				{
					TimeSpan zero = TimeSpan.Zero;
					int num = 0;
					TimeSpan zero2 = TimeSpan.Zero;
					string text;
					string text2;
					short[] array;
					ArraySegment<byte> segmentExtendedAuxOut;
					ErrorCode errorCode = (ErrorCode)base.ProtocolClient.EndConnect(asyncResult, out this.contextHandle, out zero, out num, out zero2, out text, out text2, out array, out segmentExtendedAuxOut);
					MonitoringActivityAuxiliaryBlock activityContext;
					ExceptionTraceAuxiliaryBlock remoteExceptionTrace;
					EmsmdbClient.ConnectCallContext.ParseAuxOut(segmentExtendedAuxOut, out activityContext, out remoteExceptionTrace);
					result = new ConnectCallResult(errorCode, remoteExceptionTrace, activityContext, new MapiVersion?(new MapiVersion((ushort)array[0], (ushort)array[1], (ushort)array[2], (ushort)array[3])), base.GetHttpResponseInformation());
				}
				finally
				{
					base.StopAndCleanupRpcTimer();
					this.Cleanup();
				}
				return result;
			}

			protected override ConnectCallResult OnRpcException(RpcException rpcException)
			{
				return new ConnectCallResult(rpcException);
			}

			protected override ConnectCallResult OnProtocolException(ProtocolException protocolException)
			{
				return new ConnectCallResult(protocolException, base.GetHttpInformationFromProtocolException(protocolException));
			}

			private ArraySegment<byte> BuildAuxInBuffer(ArraySegment<byte> extendedBuffer)
			{
				AuxiliaryBlock[] array = new AuxiliaryBlock[]
				{
					new PerfClientInfoAuxiliaryBlock(0U, 1, ComputerInformation.DnsPhysicalFullyQualifiedDomainName, string.Empty, Array<byte>.EmptySegment, Array<byte>.EmptySegment, string.Empty, Array<byte>.EmptySegment, ClientMode.Cached),
					new PerfProcessInfoAuxiliaryBlock(2, 1, Guid.NewGuid(), Assembly.GetExecutingAssembly().ManifestModule.Name)
				};
				if (this.useMonitoringContext)
				{
					array = array.Concat(new AuxiliaryBlock[]
					{
						new SetMonitoringContextAuxiliaryBlock()
					});
				}
				byte[] array2 = RopClient.CreateAuxiliaryBuffer(array);
				return ExtendedBufferHelper.Wrap(CompressAndObfuscate.Instance, extendedBuffer, new ArraySegment<byte>(array2), false, false);
			}

			private void Cleanup()
			{
				if (this.auxIn != null)
				{
					AsyncBufferPools.ReleaseBuffer(this.auxIn);
					this.auxIn = null;
				}
				if (this.auxOut != null)
				{
					AsyncBufferPools.ReleaseBuffer(this.auxOut);
					this.auxOut = null;
				}
			}

			private readonly string userDn;

			private readonly bool useMonitoringContext;

			private byte[] auxIn;

			private byte[] auxOut;

			private IntPtr contextHandle = IntPtr.Zero;
		}

		private abstract class BaseLogonCallContext : EmsmdbCallContext<LogonCallResult>
		{
			public BaseLogonCallContext(IExchangeAsyncDispatch protocolClient, IntPtr contextHandle, string mailboxDn, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState) : base(protocolClient, timeout, asyncCallback, asyncState)
			{
				Util.ThrowOnIntPtrZero(contextHandle, "contextHandle");
				this.contextHandle = contextHandle;
				this.mailboxDn = mailboxDn;
			}

			protected LogonFlags LogonFlags { get; set; }

			protected OpenFlags OpenFlags { get; set; }

			protected LogonExtendedRequestFlags ExtendedFlags { get; set; }

			public LogonCallResult End(IAsyncResult asyncResult, out IntPtr contextHandle)
			{
				contextHandle = this.contextHandle;
				return base.GetResult();
			}

			protected override ICancelableAsyncResult OnBegin(CancelableAsyncCallback asyncCallback, object asyncState)
			{
				bool flag = false;
				ICancelableAsyncResult result;
				try
				{
					this.ropIn = AsyncBufferPools.GetBuffer(EmsmdbConstants.MaxExtendedRopBufferSize);
					this.ropOut = AsyncBufferPools.GetBuffer(EmsmdbConstants.MaxExtendedRopBufferSize);
					this.auxOut = AsyncBufferPools.GetBuffer(EmsmdbConstants.MaxExtendedAuxBufferSize);
					ArraySegment<byte> segmentExtendedRopIn = this.BuildLogonRequestBuffer(this.mailboxDn, new ArraySegment<byte>(this.ropIn, 0, EmsmdbConstants.MaxExtendedRopBufferSize));
					ArraySegment<byte> segmentExtendedRopOut = new ArraySegment<byte>(this.ropOut, 0, EmsmdbConstants.MaxExtendedRopBufferSize);
					ArraySegment<byte> emptySegment = Array<byte>.EmptySegment;
					ArraySegment<byte> segmentExtendedAuxOut = new ArraySegment<byte>(this.auxOut, 0, EmsmdbConstants.MaxExtendedAuxBufferSize);
					ICancelableAsyncResult cancelableAsyncResult = base.ProtocolClient.BeginExecute(null, this.contextHandle, 0, segmentExtendedRopIn, segmentExtendedRopOut, emptySegment, segmentExtendedAuxOut, asyncCallback, asyncState);
					base.StartRpcTimer();
					flag = true;
					result = cancelableAsyncResult;
				}
				finally
				{
					if (!flag)
					{
						this.Cleanup();
					}
				}
				return result;
			}

			protected override LogonCallResult OnEnd(ICancelableAsyncResult asyncResult)
			{
				LogonCallResult result;
				try
				{
					ArraySegment<byte> segmentExtendedRopOut;
					ArraySegment<byte> segmentExtendedAuxOut;
					ErrorCode errorCode = (ErrorCode)base.ProtocolClient.EndExecute(asyncResult, out this.contextHandle, out segmentExtendedRopOut, out segmentExtendedAuxOut);
					ErrorCode logonErrorCode = (errorCode == ErrorCode.None) ? EmsmdbClient.BaseLogonCallContext.ParseLogonResponseBuffer(segmentExtendedRopOut) : ErrorCode.None;
					MonitoringActivityAuxiliaryBlock activityContext;
					ExceptionTraceAuxiliaryBlock remoteExceptionTrace;
					EmsmdbClient.ConnectCallContext.ParseAuxOut(segmentExtendedAuxOut, out activityContext, out remoteExceptionTrace);
					result = new LogonCallResult(errorCode, remoteExceptionTrace, activityContext, logonErrorCode, base.GetHttpResponseInformation());
				}
				finally
				{
					base.StopAndCleanupRpcTimer();
					this.Cleanup();
				}
				return result;
			}

			protected override LogonCallResult OnRpcException(RpcException rpcException)
			{
				return new LogonCallResult(rpcException);
			}

			protected override LogonCallResult OnProtocolException(ProtocolException protocolException)
			{
				return new LogonCallResult(protocolException, base.GetHttpInformationFromProtocolException(protocolException));
			}

			protected virtual void BuildFlagsForLogonRequestBuffer()
			{
			}

			private static ErrorCode ParseLogonResponseBuffer(ArraySegment<byte> segmentExtendedRopOut)
			{
				Util.ThrowOnNullArgument(segmentExtendedRopOut, "segmentExtendedRopOut");
				byte[] buffer = AsyncBufferPools.GetBuffer(EmsmdbConstants.MaxRopBufferSize);
				ArraySegment<byte> arraySegment = new ArraySegment<byte>(buffer, 0, EmsmdbConstants.MaxRopBufferSize);
				ErrorCode errorCode;
				try
				{
					ArraySegment<byte>? arraySegment2 = null;
					arraySegment = ExtendedBufferHelper.Unwrap(CompressAndObfuscate.Instance, segmentExtendedRopOut, arraySegment, out arraySegment2);
					List<RopResult> list = RopClient.ParseOneRop<RopLogon>(arraySegment);
					errorCode = list[0].ErrorCode;
				}
				finally
				{
					if (buffer != null)
					{
						AsyncBufferPools.ReleaseBuffer(buffer);
					}
				}
				return errorCode;
			}

			private ArraySegment<byte> BuildLogonRequestBuffer(string mailboxDn, ArraySegment<byte> extendedBuffer)
			{
				this.BuildFlagsForLogonRequestBuffer();
				StoreState storeState = StoreState.None;
				RopLogon ropLogon = new RopLogon();
				ropLogon.SetInput(1, 0, this.LogonFlags, this.OpenFlags, storeState, this.ExtendedFlags, new MailboxId?(new MailboxId(mailboxDn)), null, "Client=Microsoft.Exchange.RpcClientAccess.Monitoring", null, null);
				byte[][] array = RopClient.CreateInputBuffer(new Rop[]
				{
					ropLogon
				});
				return ExtendedBufferHelper.Wrap(CompressAndObfuscate.Instance, extendedBuffer, new ArraySegment<byte>(array[0]), false, false);
			}

			private void Cleanup()
			{
				if (this.ropIn != null)
				{
					AsyncBufferPools.ReleaseBuffer(this.ropIn);
					this.ropIn = null;
				}
				if (this.ropOut != null)
				{
					AsyncBufferPools.ReleaseBuffer(this.ropOut);
					this.ropOut = null;
				}
				if (this.auxOut != null)
				{
					AsyncBufferPools.ReleaseBuffer(this.auxOut);
					this.auxOut = null;
				}
			}

			private readonly string mailboxDn;

			private byte[] ropIn;

			private byte[] ropOut;

			private byte[] auxOut;

			private IntPtr contextHandle = IntPtr.Zero;
		}

		private class PrivateLogonCallContext : EmsmdbClient.BaseLogonCallContext
		{
			public PrivateLogonCallContext(IExchangeAsyncDispatch protocolClient, IntPtr contextHandle, string mailboxDn, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState) : base(protocolClient, contextHandle, mailboxDn, timeout, asyncCallback, asyncState)
			{
			}

			protected override void BuildFlagsForLogonRequestBuffer()
			{
				base.LogonFlags = (LogonFlags.Private | LogonFlags.Extended);
				base.OpenFlags = (OpenFlags.HomeLogon | OpenFlags.TakeOwnership | OpenFlags.NoMail | OpenFlags.CliWithPerMdbFix);
				base.ExtendedFlags = LogonExtendedRequestFlags.ApplicationId;
			}
		}

		private class PublicLogonCallContext : EmsmdbClient.BaseLogonCallContext
		{
			public PublicLogonCallContext(IExchangeAsyncDispatch protocolClient, IntPtr contextHandle, string mailboxDn, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState) : base(protocolClient, contextHandle, mailboxDn, timeout, asyncCallback, asyncState)
			{
			}

			protected override void BuildFlagsForLogonRequestBuffer()
			{
				base.LogonFlags = LogonFlags.None;
				base.OpenFlags = (OpenFlags.Public | OpenFlags.HomeLogon | OpenFlags.TakeOwnership | OpenFlags.NoMail | OpenFlags.CliWithPerMdbFix);
				base.ExtendedFlags = LogonExtendedRequestFlags.None;
			}
		}

		private class DummyCallContext : EmsmdbCallContext<DummyCallResult>
		{
			public DummyCallContext(IExchangeAsyncDispatch protocolClient, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState) : base(protocolClient, timeout, asyncCallback, asyncState)
			{
			}

			public DummyCallResult End(IAsyncResult asyncResult)
			{
				return base.GetResult();
			}

			protected override ICancelableAsyncResult OnBegin(CancelableAsyncCallback asyncCallback, object asyncState)
			{
				ICancelableAsyncResult result = base.ProtocolClient.BeginDummy(null, null, asyncCallback, asyncState);
				base.StartRpcTimer();
				return result;
			}

			protected override DummyCallResult OnEnd(ICancelableAsyncResult asyncResult)
			{
				DummyCallResult result;
				try
				{
					ErrorCode errorCode = (ErrorCode)base.ProtocolClient.EndDummy(asyncResult);
					result = new DummyCallResult(errorCode);
				}
				finally
				{
					base.StopAndCleanupRpcTimer();
				}
				return result;
			}

			protected override DummyCallResult OnRpcException(RpcException rpcException)
			{
				return new DummyCallResult(rpcException);
			}

			protected override DummyCallResult OnProtocolException(ProtocolException protocolException)
			{
				return new DummyCallResult(protocolException);
			}
		}
	}
}

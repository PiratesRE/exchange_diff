using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.UM;
using Microsoft.Exchange.UM.Rpc;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal abstract class UMVersionedRpcTargetBase : IVersionedRpcTarget, IRpcTarget
	{
		public UMVersionedRpcTargetBase(Server server)
		{
			this.server = server;
		}

		public string Name
		{
			get
			{
				if (this.server == null)
				{
					return string.Empty;
				}
				return this.server.Name;
			}
		}

		public ADConfigurationObject ConfigObject
		{
			get
			{
				return this.server;
			}
		}

		public Server Server
		{
			get
			{
				return this.server;
			}
		}

		public override bool Equals(object obj)
		{
			UMVersionedRpcTargetBase umversionedRpcTargetBase = obj as UMVersionedRpcTargetBase;
			if (this.Server == null || umversionedRpcTargetBase == null || umversionedRpcTargetBase.Server == null)
			{
				return base.Equals(obj);
			}
			return this.Server.Guid.Equals(umversionedRpcTargetBase.Server.Guid);
		}

		public override int GetHashCode()
		{
			if (this.Server != null)
			{
				return this.Server.Guid.GetHashCode();
			}
			return base.GetHashCode();
		}

		public UMRpcResponse ExecuteRequest(UMVersionedRpcRequest request)
		{
			UMRpcResponse result = null;
			int num = 3;
			while (num-- > 0)
			{
				try
				{
					result = this.InternalExecuteRequest(request);
					break;
				}
				catch (RpcException ex)
				{
					bool flag = UMErrorCode.IsNetworkError(ex.ErrorCode);
					CallIdTracer.TraceError(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMVersionedRpcTargetBase.ExecuteRequest: {0} IsRetriable={1} RetryCount={2} Exception={3}", new object[]
					{
						ex.Message,
						flag,
						num,
						ex
					});
					if (num == 0 || !flag)
					{
						throw;
					}
				}
			}
			return result;
		}

		protected abstract UMVersionedRpcClientBase CreateRpcClient();

		private UMRpcResponse InternalExecuteRequest(UMVersionedRpcRequest request)
		{
			UMRpcResponse result = null;
			using (UMVersionedRpcClientBase umversionedRpcClientBase = this.CreateRpcClient())
			{
				try
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMVersionedRpcTargetBase: Executing {0} ({1}->{2}).", new object[]
					{
						umversionedRpcClientBase.OperationName,
						base.GetType().Name,
						request
					});
					umversionedRpcClientBase.SetTimeOut(30000);
					byte[] array = Serialization.ObjectToBytes(request);
					if (array == null)
					{
						throw new ArgumentException("request");
					}
					byte[] mbinaryData = umversionedRpcClientBase.ExecuteRequest(array);
					result = (UMRpcResponse)Serialization.BytesToObject(mbinaryData);
					CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMVersionedRpcTargetBase: {0} ({1}->{2}) succeeded.", new object[]
					{
						umversionedRpcClientBase.OperationName,
						base.GetType().Name,
						request
					});
				}
				catch (RpcException ex)
				{
					CallIdTracer.TraceWarning(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMVersionedRpcTargetBase: {0} ({1}->{2}) failed. ErrorCode:{3} Exception:{4}", new object[]
					{
						umversionedRpcClientBase.OperationName,
						base.GetType().Name,
						request,
						ex.ErrorCode,
						ex
					});
					throw;
				}
			}
			return result;
		}

		internal const int TimeOutMSEC = 30000;

		private Server server;
	}
}

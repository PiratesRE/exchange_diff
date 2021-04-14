using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Rpc.UM;
using Microsoft.Exchange.UM.Rpc;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class UMVersionedRpcServer : UMVersionedRpcServerBase
	{
		protected abstract UMRPCComponentBase Component { get; }

		protected virtual bool ResponseIsMandatory
		{
			get
			{
				return false;
			}
		}

		public override int ExecuteRequest(byte[] requestBytes, out byte[] responseBytes)
		{
			int num = 0;
			responseBytes = null;
			if (!this.Component.GuardBeforeExecution())
			{
				CallIdTracer.TraceError(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMVersionedRpcServer.ExecuteRequest: UMESERVERUNAVAILABLE", new object[0]);
				return 1722;
			}
			UMVersionedRpcRequest umversionedRpcRequest = null;
			try
			{
				umversionedRpcRequest = (UMVersionedRpcRequest)Serialization.BytesToObject(requestBytes);
				if (umversionedRpcRequest != null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMVersionedRpcServer.ExecuteRequest {0}", new object[]
					{
						umversionedRpcRequest
					});
					this.PrepareRequest(umversionedRpcRequest);
					UMRpcResponse obj = UMVersionedRpcRequest.ExecuteRequest(umversionedRpcRequest);
					responseBytes = Serialization.ObjectToBytes(obj);
					if (this.ResponseIsMandatory && responseBytes == null)
					{
						CallIdTracer.TraceError(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMVersionedRpcServer.ExecuteRequest {0}: Invalid response", new object[]
						{
							umversionedRpcRequest
						});
						num = -2147466752;
					}
				}
				else
				{
					CallIdTracer.TraceError(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMVersionedRpcServer.ExecuteRequest (null): Invalid request", new object[0]);
					num = -2147466750;
				}
			}
			catch (UMRpcException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMVersionedRpcServer.ExecuteRequest {0}: {1}", new object[]
				{
					umversionedRpcRequest,
					ex
				});
				num = ex.ErrorCode;
			}
			finally
			{
				this.Component.GuardAfterExecution();
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMVersionedRpcServer.ExecuteRequest {0}. Returning {1:X}", new object[]
			{
				umversionedRpcRequest,
				num
			});
			return num;
		}

		protected abstract void PrepareRequest(UMVersionedRpcRequest request);
	}
}

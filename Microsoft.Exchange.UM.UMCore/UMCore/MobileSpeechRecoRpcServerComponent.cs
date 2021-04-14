using System;
using System.DirectoryServices;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal sealed class MobileSpeechRecoRpcServerComponent : UMRPCComponentBase
	{
		public static MobileSpeechRecoRpcServerComponent Instance
		{
			get
			{
				if (MobileSpeechRecoRpcServerComponent.instance == null)
				{
					lock (MobileSpeechRecoRpcServerComponent.staticLock)
					{
						if (MobileSpeechRecoRpcServerComponent.instance == null)
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, null, "Creating singleton instance of MobileSpeechRecoRpcServerComponent", new object[0]);
							MobileSpeechRecoRpcServerComponent.instance = new MobileSpeechRecoRpcServerComponent();
						}
					}
				}
				return MobileSpeechRecoRpcServerComponent.instance;
			}
		}

		internal override void RegisterServer()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "Entering MobileSpeechRecoRpcServerComponent.RegisterServer", new object[0]);
			ActiveDirectorySecurity sd = null;
			Util.GetServerSecurityDescriptor(ref sd);
			uint accessMask = 131220U;
			RpcServerBase.RegisterServer(typeof(MobileSpeechRecoRpcServerComponent.MobileSpeechRecoRpcServer), sd, accessMask);
		}

		private static object staticLock = new object();

		private static MobileSpeechRecoRpcServerComponent instance;

		internal sealed class MobileSpeechRecoRpcServer : MobileSpeechRecoRpcServerBase
		{
			public override void ExecuteStepAsync(byte[] inBlob, object token)
			{
				ValidateArgument.NotNull(inBlob, "inBlob");
				ValidateArgument.NotNull(token, "token");
				CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "Entering MobileSpeechRecoRpcServer.ExecuteStepAsync", new object[0]);
				try
				{
					IMobileSpeechRecoRequestStep mobileSpeechRecoRequestStep = MobileSpeechRecoRequestStep.Create(inBlob);
					CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "MobileSpeechRecoRpcServer.ExecuteStepAsync Step Dump={0}", new object[]
					{
						mobileSpeechRecoRequestStep
					});
					mobileSpeechRecoRequestStep.ExecuteAsync(new MobileRecoRequestStepAsyncCompletedDelegate(this.OnRequestStepCompleted), token);
				}
				catch (Exception ex)
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MobileSpeechRecoRPCGeneralUnexpectedFailure, null, new object[]
					{
						CommonUtil.ToEventLogString(ex.ToString())
					});
					UMRPCComponentBase.HandleException(ex);
				}
			}

			public void OnRequestStepCompleted(int returnValue, byte[] outBlob, object token)
			{
				ValidateArgument.NotNull(outBlob, "outBlob");
				ValidateArgument.NotNull(token, "token");
				CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "Entering MobileSpeechRecoRpcServer.OnRequestStepCompleted - Return value='{0}'", new object[]
				{
					returnValue
				});
				base.CompleteAsync(returnValue, outBlob, token);
			}
		}
	}
}

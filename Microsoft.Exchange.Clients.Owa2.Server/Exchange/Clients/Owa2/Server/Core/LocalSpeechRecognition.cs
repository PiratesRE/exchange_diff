using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class LocalSpeechRecognition : SpeechRecognition
	{
		public LocalSpeechRecognition(RequestParameters requestParameters, SpeechRecognitionResultsPriority resultsPriority) : base(requestParameters, resultsPriority)
		{
		}

		public override void AddRecoRequestAsync(SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedDelegate callback)
		{
			ValidateArgument.NotNull(callback, "callback");
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "LocalSpeechRecognition.AddRecoRequestAsync - RequestId='{0}' RequestType='{1}'", base.Parameters.RequestId, base.Parameters.RequestType.ToString());
			base.CollectAndLogStatisticsInformation(MobileSpeechRecoRequestStepLogId.AddRecoRequest, -1);
			try
			{
				ADTopologyLookup adtopologyLookup = ADTopologyLookup.CreateLocalResourceForestLookup();
				Server localServer = adtopologyLookup.GetLocalServer();
				this.rpcServerFqdn = localServer.Fqdn;
				ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "LocalSpeechRecognition.AddRecoRequestAsync - Request Id='{0}', UM server FQDN for RPC='{1}'", base.Parameters.RequestId, this.rpcServerFqdn);
				MobileSpeechRecoRpcClient mobileSpeechRecoRpcClient = new MobileSpeechRecoRpcClient(base.Parameters.RequestId, this.rpcServerFqdn, callback);
				mobileSpeechRecoRpcClient.BeginAddRecoRequest(base.Parameters.RequestType, base.Parameters.UserObjectGuid, base.Parameters.TenantGuid, base.Parameters.Culture, base.Parameters.TimeZone, new AsyncCallback(this.OnAddRecoRequestCompleted), mobileSpeechRecoRpcClient);
				ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<Guid>((long)this.GetHashCode(), "LocalSpeechRecognition.AddRecoRequestAsync - Request Id='{0}', Called BeginAddRecoRequest", base.Parameters.RequestId);
			}
			catch (ArgumentOutOfRangeException e)
			{
				this.HandleException(e, -2147466750, callback);
			}
			catch (Exception e2)
			{
				this.HandleUnexpectedException(e2, callback);
			}
		}

		public override void RecognizeAsync(byte[] audioBytes, SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedDelegate callback)
		{
			ValidateArgument.NotNull(audioBytes, "audioBytes");
			ValidateArgument.NotNull(callback, "callback");
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<Guid, MobileSpeechRecoRequestType, string>((long)this.GetHashCode(), "LocalSpeechRecognition.RecognizeAsync - Request Id='{0}', Request Type='{1}', UM server FQDN for RPC='{2}'", base.Parameters.RequestId, base.Parameters.RequestType, this.rpcServerFqdn);
			base.CollectAndLogStatisticsInformation(MobileSpeechRecoRequestStepLogId.Recognize, audioBytes.Length);
			try
			{
				MobileSpeechRecoRpcClient mobileSpeechRecoRpcClient = new MobileSpeechRecoRpcClient(base.Parameters.RequestId, this.rpcServerFqdn, callback);
				mobileSpeechRecoRpcClient.BeginRecognize(audioBytes, new AsyncCallback(this.OnRecognizeCompleted), mobileSpeechRecoRpcClient);
				ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<Guid>((long)this.GetHashCode(), "LocalSpeechRecognition.RecognizeAsync - Request Id='{0}', Called BeginRecognize", base.Parameters.RequestId);
			}
			catch (Exception e)
			{
				this.HandleUnexpectedException(e, callback);
			}
		}

		private static SpeechRecognitionProcessor.SpeechHttpStatus MapRpcErrorCodeToHttpErrorCode(int errorCode)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int>(0L, "LocalSpeechRecognition.MapRpcErrorCodeToHttpErrorCode - errorCode='{0}'", errorCode);
			if (LocalSpeechRecognition.errorCodeMapping.ContainsKey(errorCode))
			{
				return LocalSpeechRecognition.errorCodeMapping[errorCode];
			}
			return SpeechRecognitionProcessor.SpeechHttpStatus.InternalServerError;
		}

		private void OnAddRecoRequestCompleted(IAsyncResult asyncResult)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "LocalSpeechRecognition.OnAddRecoRequestCompleted - RequestId='{0}' RequestType='{1}'", base.Parameters.RequestId, base.Parameters.RequestType.ToString());
			base.CollectAndLogStatisticsInformation(MobileSpeechRecoRequestStepLogId.AddRecoRequestCompleted, -1);
			MobileSpeechRecoRpcClient mobileSpeechRecoRpcClient = (MobileSpeechRecoRpcClient)asyncResult.AsyncState;
			SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedDelegate speechProcessorAsyncCompletedDelegate = mobileSpeechRecoRpcClient.State as SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedDelegate;
			try
			{
				MobileRecoRPCAsyncCompletedArgs mobileRecoRPCAsyncCompletedArgs = mobileSpeechRecoRpcClient.EndAddRecoRequest(asyncResult);
				SpeechRecognitionProcessor.SpeechHttpStatus httpStatus = LocalSpeechRecognition.MapRpcErrorCodeToHttpErrorCode(mobileRecoRPCAsyncCompletedArgs.ErrorCode);
				SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs args = new SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs(mobileRecoRPCAsyncCompletedArgs.Result, httpStatus);
				speechProcessorAsyncCompletedDelegate(args);
			}
			catch (Exception e)
			{
				this.HandleUnexpectedException(e, speechProcessorAsyncCompletedDelegate);
			}
		}

		private void OnRecognizeCompleted(IAsyncResult asyncResult)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "LocalSpeechRecognition.OnRecognizeCompleted - RequestId='{0}' RequestType='{1}'", base.Parameters.RequestId, base.Parameters.RequestType.ToString());
			base.CollectAndLogStatisticsInformation(MobileSpeechRecoRequestStepLogId.RecognizeCompleted, -1);
			MobileSpeechRecoRpcClient mobileSpeechRecoRpcClient = (MobileSpeechRecoRpcClient)asyncResult.AsyncState;
			SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedDelegate speechProcessorAsyncCompletedDelegate = mobileSpeechRecoRpcClient.State as SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedDelegate;
			try
			{
				MobileRecoRPCAsyncCompletedArgs mobileRecoRPCAsyncCompletedArgs = mobileSpeechRecoRpcClient.EndRecognize(asyncResult);
				SpeechRecognitionProcessor.SpeechHttpStatus httpStatus = LocalSpeechRecognition.MapRpcErrorCodeToHttpErrorCode(mobileRecoRPCAsyncCompletedArgs.ErrorCode);
				SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs args = new SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs(mobileRecoRPCAsyncCompletedArgs.Result, httpStatus);
				speechProcessorAsyncCompletedDelegate(args);
			}
			catch (Exception e)
			{
				this.HandleUnexpectedException(e, speechProcessorAsyncCompletedDelegate);
			}
		}

		private void HandleUnexpectedException(Exception e, SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedDelegate callback)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceError<Exception>((long)this.GetHashCode(), "LocalSpeechRecognition - HandleUnexpectedException='{0}'", e);
			ExWatson.SendReport(e, ReportOptions.None, null);
			this.HandleException(e, -2147466752, callback);
		}

		private void HandleException(Exception e, int errorCode, SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedDelegate callback)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceError<Exception, int>((long)this.GetHashCode(), "LocalSpeechRecognition - Exception='{0}', Error Code='{1}'", e, errorCode);
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_SpeechRecoRequestFailed, null, new object[]
			{
				base.RequestId,
				base.Parameters.UserObjectGuid,
				base.Parameters.TenantGuid,
				CommonUtil.ToEventLogString(e)
			});
			SpeechRecognitionProcessor.SpeechHttpStatus httpStatus = LocalSpeechRecognition.MapRpcErrorCodeToHttpErrorCode(errorCode);
			callback(new SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs(string.Empty, httpStatus));
		}

		private static readonly Dictionary<int, SpeechRecognitionProcessor.SpeechHttpStatus> errorCodeMapping = new Dictionary<int, SpeechRecognitionProcessor.SpeechHttpStatus>
		{
			{
				0,
				SpeechRecognitionProcessor.SpeechHttpStatus.Success
			},
			{
				1722,
				SpeechRecognitionProcessor.SpeechHttpStatus.ServiceUnavailable
			},
			{
				-2147466750,
				SpeechRecognitionProcessor.SpeechHttpStatus.BadRequest
			},
			{
				-2147466747,
				SpeechRecognitionProcessor.SpeechHttpStatus.BadRequest
			},
			{
				-2147466746,
				SpeechRecognitionProcessor.SpeechHttpStatus.BadRequest
			},
			{
				-2147466743,
				SpeechRecognitionProcessor.SpeechHttpStatus.NoSpeechDetected
			}
		};

		private string rpcServerFqdn;
	}
}

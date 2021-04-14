using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class StartFindInGALSpeechRecognitionCommand : SpeechRecognitionCommandBase<StartFindInGALSpeechRecognitionRequest, RecognitionId>
	{
		public StartFindInGALSpeechRecognitionCommand(CallContext callContext, StartFindInGALSpeechRecognitionRequest request) : base(callContext, request)
		{
			this.culture = request.Culture;
			this.timeZone = request.TimeZone;
			this.userObjectGuid = request.UserObjectGuid;
			this.tenantGuid = request.TenantGuid;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new StartFindInGALSpeechRecognitionResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<RecognitionId> Execute()
		{
			ExTraceGlobals.GetEventsCallTracer.TraceDebug((long)this.GetHashCode(), "StartFindInGALSpeechRecognitionCommand.Execute called");
			Guid guid = Guid.NewGuid();
			RecognitionId value = new RecognitionId(guid);
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_StartFindInGALSpeechRecoRequestParams, null, new object[]
			{
				guid,
				this.userObjectGuid,
				this.tenantGuid,
				this.culture,
				this.timeZone
			});
			CultureInfo cultureInfo;
			try
			{
				cultureInfo = new CultureInfo(this.culture);
			}
			catch (ArgumentException arg)
			{
				ExTraceGlobals.GetEventsCallTracer.TraceDebug<string, ArgumentException>((long)this.GetHashCode(), "StartFindInGALSpeechRecognitionCommand.Execute - culture '{0}' is invalid, exception='{1}'", this.culture ?? "<null>", arg);
				throw new InvalidRequestException();
			}
			ExTimeZone exTimeZone;
			if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(this.timeZone, out exTimeZone))
			{
				ExTraceGlobals.GetEventsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "StartFindInGALSpeechRecognitionCommand.Execute - timeZone '{0}' is invalid'", this.timeZone ?? "<null>");
				throw new InvalidRequestException();
			}
			ADTopologyLookup adtopologyLookup = ADTopologyLookup.CreateLocalResourceForestLookup();
			Server localServer = adtopologyLookup.GetLocalServer();
			MobileSpeechRecoRpcClient mobileSpeechRecoRpcClient = new MobileSpeechRecoRpcClient(guid, localServer.Fqdn, null);
			IAsyncResult asyncResult = mobileSpeechRecoRpcClient.BeginAddRecoRequest(MobileSpeechRecoRequestType.FindInGAL, this.userObjectGuid, this.tenantGuid, cultureInfo, exTimeZone, null, null);
			MobileRecoRPCAsyncCompletedArgs mobileRecoRPCAsyncCompletedArgs = mobileSpeechRecoRpcClient.EndAddRecoRequest(asyncResult);
			ServiceResult<RecognitionId> result;
			if (mobileRecoRPCAsyncCompletedArgs.ErrorCode == 0)
			{
				result = new ServiceResult<RecognitionId>(value);
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_StartFindInGALSpeechRecoRequestSuccess, null, new object[]
				{
					guid
				});
			}
			else
			{
				ServiceError serviceError = base.MapRpcErrorToServiceError(mobileRecoRPCAsyncCompletedArgs.ErrorCode, mobileRecoRPCAsyncCompletedArgs.ErrorMessage);
				result = new ServiceResult<RecognitionId>(serviceError);
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_StartFindInGALSpeechRecoRequestFailed, null, new object[]
				{
					guid,
					CommonUtil.ToEventLogString(serviceError.MessageKey),
					CommonUtil.ToEventLogString(serviceError.MessageText)
				});
			}
			return result;
		}

		private readonly string culture;

		private readonly string timeZone;

		private readonly Guid userObjectGuid;

		private readonly Guid tenantGuid;
	}
}

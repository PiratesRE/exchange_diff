using System;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CallRejectedException : LocalizedException
	{
		internal PlatformSignalingHeader DiagnosticHeader { get; private set; }

		internal CallEndingReason Reason { get; private set; }

		private CallRejectedException(LocalizedString exceptionString, Exception innerException) : base(exceptionString, innerException)
		{
		}

		internal static CallRejectedException Create(LocalizedString exceptionString, CallEndingReason endingReason, string extraInfoFormatString, params object[] args)
		{
			return CallRejectedException.Create(exceptionString, null, endingReason, extraInfoFormatString, args);
		}

		internal static CallRejectedException Create(LocalizedString exceptionString, Exception innerException, CallEndingReason endingReason, string extraInfoFormatString, params object[] args)
		{
			return new CallRejectedException(exceptionString, innerException)
			{
				Reason = endingReason,
				DiagnosticHeader = CallRejectedException.RenderDiagnosticHeader(endingReason, extraInfoFormatString, args)
			};
		}

		internal static PlatformSignalingHeader RenderDiagnosticHeader(CallEndingReason endingReason, string extraInfoFormatString, params object[] args)
		{
			int errorCode = endingReason.ErrorCode;
			string reason = endingReason.Reason;
			bool useDataCenterCallRouting = CommonConstants.UseDataCenterCallRouting;
			string text = string.Empty;
			try
			{
				text = Utils.GetOwnerHostFqdn();
			}
			catch (LocalizedException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "Couldnt get correct Source string to use in Diagnostic header. Error : {0}", new object[]
				{
					ex
				});
			}
			if (endingReason.AdminScopeOfAction == null && useDataCenterCallRouting)
			{
				errorCode = CallEndingReason.DatacenterInternalError.ErrorCode;
				reason = CallEndingReason.DatacenterInternalError.Reason;
			}
			string name = "ms-diagnostics-public";
			string text2 = null;
			if (!string.IsNullOrEmpty(extraInfoFormatString))
			{
				text2 = string.Format(CultureInfo.InvariantCulture, extraInfoFormatString, args);
			}
			string text3 = (text2 == null) ? reason : string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				reason,
				text2
			});
			string value;
			if (endingReason.TypeOfCallEnding == null)
			{
				value = string.Format(CultureInfo.InvariantCulture, "{0};reason=\"{1}\"", new object[]
				{
					errorCode,
					text3
				});
			}
			else
			{
				value = string.Format(CultureInfo.InvariantCulture, "{0};source=\"{1}\";reason=\"{2}\"", new object[]
				{
					errorCode,
					text,
					text3
				});
			}
			CallRejectedException.LogCallRejection(endingReason, (text2 != null) ? text2 : string.Empty);
			return Platform.Builder.CreateSignalingHeader(name, value);
		}

		internal static void LogCallRejection(CallEndingReason endingReason, string extraInfo)
		{
			if (CommonConstants.UseDataCenterLogging)
			{
				CallRejectionLogger.CallRejectionLogRow callRejectionLogRow = new CallRejectionLogger.CallRejectionLogRow();
				callRejectionLogRow.UMServerName = Utils.GetLocalHostName();
				callRejectionLogRow.TimeStamp = (DateTime)ExDateTime.UtcNow;
				callRejectionLogRow.ErrorCode = endingReason.ErrorCode;
				callRejectionLogRow.ErrorType = endingReason.TypeOfCallEnding.ToString();
				callRejectionLogRow.ErrorCategory = endingReason.Category.ToString();
				callRejectionLogRow.ErrorDescription = endingReason.Reason.ToString();
				callRejectionLogRow.ExtraInfo = extraInfo;
				CallRejectionLogger.Instance.Append(callRejectionLogRow);
			}
		}
	}
}

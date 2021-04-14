using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Services.Core
{
	internal static class ExceptionHandler<TValue>
	{
		public static ServiceResult<TValue>[] Execute(ExceptionHandler<TValue>.CreateServiceResults createServiceResults)
		{
			return ExceptionHandler<TValue>.Execute(createServiceResults, null);
		}

		public static ServiceResult<TValue>[] Execute(ExceptionHandler<TValue>.CreateServiceResults createServiceResults, ExceptionHandler<TValue>.GenerateMessageXmlForServiceError generateErrorXml)
		{
			ServiceResult<TValue>[] result = null;
			try
			{
				ServiceResult<TValue> serviceResult;
				if (Global.EnableFaultInjection && ExceptionHandler<TValue>.InjectFaultIfNeeded(out serviceResult))
				{
					return new ServiceResult<TValue>[]
					{
						serviceResult
					};
				}
				ExceptionHandler<TValue>.HandleClientDisconnectIfNecessary();
				result = createServiceResults();
			}
			catch (LocalizedException ex)
			{
				ExceptionHandler<TValue>.HandleBudgetExemptException(ex);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(RequestDetailsLogger.Current, ex, "ExceptionHandler_Execute_Multiple");
				result = ExceptionHandler<TValue>.GetServiceResultArray<TValue>(ex, generateErrorXml);
			}
			finally
			{
				if (RequestDetailsLogger.Current != null)
				{
					RequestDetailsLogger.Current.PushDebugInfoToResponseHeaders();
				}
			}
			return result;
		}

		public static ServiceResult<TValue> Execute(ExceptionHandler<TValue>.CreateServiceResult createServiceResult, int index)
		{
			return ExceptionHandler<TValue>.Execute(createServiceResult, index, null);
		}

		public static ServiceResult<TValue> Execute(ExceptionHandler<TValue>.CreateServiceResult createServiceResult, int index, ExceptionHandler<TValue>.GenerateMessageXmlForServiceError generateErrorXml)
		{
			ServiceResult<TValue> result = null;
			try
			{
				ServiceResult<TValue> result2;
				if (Global.EnableFaultInjection && ExceptionHandler<TValue>.InjectFaultIfNeeded(out result2))
				{
					return result2;
				}
				ExceptionHandler<TValue>.HandleClientDisconnectIfNecessary();
				CallContext.Current.Budget.SleepIfNecessary();
				result = createServiceResult(index);
			}
			catch (LocalizedException ex)
			{
				ExceptionHandler<TValue>.HandleBudgetExemptException(ex);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(RequestDetailsLogger.Current, ex, "ExceptionHandler_Execute");
				result = ExceptionHandler<TValue>.GetServiceResult<TValue>(ex, generateErrorXml);
			}
			finally
			{
				if (RequestDetailsLogger.Current != null)
				{
					RequestDetailsLogger.Current.PushDebugInfoToResponseHeaders();
				}
			}
			return result;
		}

		internal static ServiceResult<T>[] GetServiceResultArray<T>(LocalizedException exception, ExceptionHandler<TValue>.GenerateMessageXmlForServiceError generateErrorXml)
		{
			return new ServiceResult<T>[]
			{
				ExceptionHandler<TValue>.GetServiceResult<T>(exception, generateErrorXml)
			};
		}

		internal static ServiceResult<T> GetServiceResult<T>(LocalizedException exception, ExceptionHandler<TValue>.GenerateMessageXmlForServiceError generateErrorXml)
		{
			ExTraceGlobals.ExceptionTracer.TraceError<LocalizedException>(0L, "ExceptionHandler.GetServiceResult called for exception: {0}", exception);
			ServiceError serviceError = ServiceErrors.GetServiceError(exception);
			if (generateErrorXml != null)
			{
				generateErrorXml(serviceError, exception);
			}
			return new ServiceResult<T>(serviceError);
		}

		private static bool IsFilterConditionMatching(out string httpStatus, out string errorCode, out string soapAction, out string userName)
		{
			httpStatus = string.Empty;
			errorCode = string.Empty;
			soapAction = string.Empty;
			userName = string.Empty;
			if (CallContext.Current != null)
			{
				string text = (CallContext.Current.AccessingPrincipal == null) ? string.Empty : CallContext.Current.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString().ToLower();
				string a = CallContext.Current.MethodName ?? string.Empty;
				foreach (Dictionary<string, string> dictionary in Global.FaultsList)
				{
					dictionary.TryGetValue("SoapAction", out soapAction);
					dictionary.TryGetValue("UserName", out userName);
					if ((string.IsNullOrEmpty(userName) || text.Contains(userName)) && (string.IsNullOrEmpty(soapAction) || string.Equals(a, soapAction, StringComparison.OrdinalIgnoreCase)))
					{
						dictionary.TryGetValue("HttpStatus", out httpStatus);
						dictionary.TryGetValue("ErrorCode", out errorCode);
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private static bool InjectFaultIfNeeded(out ServiceResult<TValue> serviceResult)
		{
			serviceResult = null;
			string text;
			string value;
			string text2;
			string text3;
			if (!ExceptionHandler<TValue>.IsFilterConditionMatching(out text, out value, out text2, out text3))
			{
				return false;
			}
			ResponseCodeType responseCodeType;
			if (!Enum.TryParse<ResponseCodeType>(value, true, out responseCodeType))
			{
				responseCodeType = ResponseCodeType.NoError;
			}
			if (text != "200")
			{
				HttpStatusCode outgoingHttpStatusCode;
				if (text != "500" && Enum.TryParse<HttpStatusCode>(text, out outgoingHttpStatusCode))
				{
					EWSSettings.SetOutgoingHttpStatusCode(outgoingHttpStatusCode);
				}
				responseCodeType = (responseCodeType.Equals(ResponseCodeType.NoError) ? ResponseCodeType.ErrorInternalServerError : responseCodeType);
				FaultInjectionPermanentException exception = new FaultInjectionPermanentException(responseCodeType, CallContext.Current.MethodName);
				throw FaultExceptionUtilities.CreateFault(exception, FaultParty.Sender);
			}
			if (!string.IsNullOrEmpty(value))
			{
				string messageText = string.Format("Fault injection in web.config generated an error {0} for {1} (Fault injection)", responseCodeType.ToString(), CallContext.Current.MethodName);
				serviceResult = new ServiceResult<TValue>(new ServiceError(messageText, responseCodeType, 0, ExchangeVersion.Current));
				return true;
			}
			return false;
		}

		internal static void HandleClientDisconnect()
		{
			RequestDetailsLogger.Current.AppendGenericError("Channel Error", "Client disconnected");
			ExTraceGlobals.ServiceCommandBaseCallTracer.TraceDebug(0L, "[ExceptionHandler::HandleClientDisconnect] Client disconnected. Returning without processing request.");
			WsPerformanceCounters.TotalClientDisconnects.Increment();
			BailOut.SetHTTPStatusAndClose(HttpStatusCode.NoContent);
		}

		private static void HandleClientDisconnectIfNecessary()
		{
			if (CallContext.Current == null)
			{
				return;
			}
			if (CallContext.Current.IsWebSocketRequest)
			{
				return;
			}
			if (CallContext.Current.HttpContext.Response.IsClientConnected)
			{
				return;
			}
			ExceptionHandler<TValue>.HandleClientDisconnect();
		}

		private static void HandleBudgetExemptException(LocalizedException exception)
		{
			if (!(exception is StorageTransientException))
			{
				return;
			}
			if (exception.InnerException == null || !(exception.InnerException is MapiExceptionTimeout))
			{
				return;
			}
			EwsBudgetWrapper ewsBudgetWrapper = CallContext.Current.Budget as EwsBudgetWrapper;
			if (ewsBudgetWrapper != null)
			{
				LocalTimeCostHandle localCostHandle = ewsBudgetWrapper.LocalCostHandle;
				if (localCostHandle != null)
				{
					localCostHandle.ReverseBudgetCharge = true;
				}
			}
		}

		internal delegate ServiceResult<TValue>[] CreateServiceResults();

		internal delegate ServiceResult<TValue> CreateServiceResult(int serviceResultIndex);

		internal delegate void GenerateMessageXmlForServiceError(ServiceError error, Exception ex);
	}
}

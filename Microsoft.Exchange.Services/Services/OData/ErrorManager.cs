using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.EventLogs;
using Microsoft.Exchange.Services.ExchangeService;

namespace Microsoft.Exchange.Services.OData
{
	internal static class ErrorManager
	{
		public static ErrorResponse GetErrorResponse(Exception exception)
		{
			ArgumentValidator.ThrowIfNull("exception", exception);
			ErrorResponse errorResponse = null;
			if (exception is ODataResponseException)
			{
				ODataResponseException ex = (ODataResponseException)exception;
				errorResponse = new ErrorResponse
				{
					HttpStatusCode = ex.HttpStatusCode,
					ErrorDetails = new ErrorDetails
					{
						ErrorCode = ex.ErrorCode,
						ErrorMessage = ex.Message,
						Exception = ex
					}
				};
			}
			else if (exception is Microsoft.Exchange.Entities.DataProviders.InvalidRequestException)
			{
				errorResponse = new ErrorResponse
				{
					HttpStatusCode = HttpStatusCode.BadRequest,
					ErrorDetails = new ErrorDetails
					{
						ErrorCode = ResponseCodeType.ErrorInvalidRequest.ToString(),
						ErrorMessage = exception.Message,
						Exception = exception
					}
				};
			}
			else if (exception is ExchangeServiceErrorResponseException)
			{
				ExchangeServiceErrorResponseException ex2 = exception as ExchangeServiceErrorResponseException;
				errorResponse = new ErrorResponse
				{
					HttpStatusCode = ErrorManager.MapHttpStatusCode(ex2.ResponseErrorCode),
					ErrorDetails = new ErrorDetails
					{
						ErrorCode = ex2.ResponseErrorCode.ToString(),
						ErrorMessage = ex2.ResponseErrorText,
						Exception = (ex2.InnerException ?? ex2)
					}
				};
			}
			else if (exception is ServicePermanentException)
			{
				ServicePermanentException ex3 = exception as ServicePermanentException;
				errorResponse = new ErrorResponse
				{
					HttpStatusCode = ErrorManager.MapHttpStatusCode(ex3.ResponseCode),
					ErrorDetails = new ErrorDetails
					{
						ErrorCode = ex3.ResponseCode.ToString(),
						ErrorMessage = ex3.Message,
						Exception = ex3
					}
				};
			}
			else if (exception is LocalizedException)
			{
				LocalizedException exception2 = exception as LocalizedException;
				ServiceError serviceError = ServiceErrors.GetServiceError(exception2);
				if (serviceError != null)
				{
					ResponseCodeType messageKey = serviceError.MessageKey;
					errorResponse = new ErrorResponse
					{
						HttpStatusCode = ErrorManager.MapHttpStatusCode(messageKey),
						ErrorDetails = new ErrorDetails
						{
							ErrorCode = messageKey.ToString(),
							ErrorMessage = serviceError.MessageText,
							Exception = exception
						}
					};
				}
			}
			if (errorResponse == null)
			{
				ServiceDiagnostics.ReportException(exception, ServicesEventLogConstants.Tuple_InternalServerError, null, "Unhandled OData exception: {0}");
				errorResponse = new ErrorResponse
				{
					HttpStatusCode = HttpStatusCode.InternalServerError,
					ErrorDetails = new ErrorDetails
					{
						ErrorCode = ResponseCodeType.ErrorInternalServerError.ToString(),
						ErrorMessage = exception.Message,
						Exception = exception
					}
				};
			}
			return errorResponse;
		}

		public static HttpStatusCode MapHttpStatusCode(ResponseCodeType responseCode)
		{
			HttpStatusCode result = HttpStatusCode.InternalServerError;
			if (ErrorManager.ErrorToHttpStatusCodeMap.Member.TryGetValue(responseCode, out result))
			{
				return result;
			}
			return result;
		}

		private static readonly LazyMember<Dictionary<ResponseCodeType, HttpStatusCode>> ErrorToHttpStatusCodeMap = new LazyMember<Dictionary<ResponseCodeType, HttpStatusCode>>(delegate()
		{
			Dictionary<ResponseCodeType, HttpStatusCode> dictionary = new Dictionary<ResponseCodeType, HttpStatusCode>();
			IEnumerable<MemberInfo> declaredMembers = typeof(ResponseCodeType).GetTypeInfo().DeclaredMembers;
			foreach (MemberInfo memberInfo in declaredMembers)
			{
				object[] customAttributes = memberInfo.GetCustomAttributes(typeof(ODataHttpStatusCodeAttribute), false);
				if (customAttributes != null && customAttributes.Length > 0)
				{
					ODataHttpStatusCodeAttribute odataHttpStatusCodeAttribute = (ODataHttpStatusCodeAttribute)customAttributes.ElementAtOrDefault(0);
					if (odataHttpStatusCodeAttribute != null)
					{
						ResponseCodeType key = EnumUtilities.Parse<ResponseCodeType>(memberInfo.Name);
						dictionary.Add(key, odataHttpStatusCodeAttribute.HttpStatusCode);
					}
				}
			}
			return dictionary;
		});
	}
}

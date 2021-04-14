using System;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class OWAFaultHandler : IErrorHandler
	{
		public bool HandleError(Exception error)
		{
			return true;
		}

		public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
		{
			OWAFaultHandler.LogException(error);
			WebBodyFormatMessageProperty property = new WebBodyFormatMessageProperty(WebContentFormat.Json);
			HttpResponseMessageProperty responseMessageProp = new HttpResponseMessageProperty
			{
				StatusCode = HttpStatusCode.InternalServerError,
				StatusDescription = Strings.GetLocalizedString(-1269063878)
			};
			responseMessageProp.Headers.Set("Content-Type", "application/json; charset=utf-8");
			if (error is OwaExtendedErrorCodeException)
			{
				OwaExtendedErrorCodeException ex = (OwaExtendedErrorCodeException)error;
				OwaExtendedError.SendError(responseMessageProp.Headers, delegate(HttpStatusCode sc)
				{
					responseMessageProp.StatusCode = sc;
				}, ex.ExtendedErrorCode, ex.Message, ex.User, ex.ErrorData);
			}
			else
			{
				MissingIdentityException ex2 = error as MissingIdentityException;
				if (ex2 != null)
				{
					responseMessageProp.StatusCode = ex2.StatusCode;
					responseMessageProp.StatusDescription = ex2.StatusDescription;
					responseMessageProp.Headers.Set(WellKnownHeader.MSDiagnostics, ex2.DiagnosticText ?? string.Empty);
					responseMessageProp.Headers.Set(WellKnownHeader.Authentication, ex2.ChallengeString ?? string.Empty);
				}
			}
			JsonFaultResponse jsonFaultResponse = JsonFaultResponse.CreateFromException(error);
			fault = Message.CreateMessage(version, string.Empty, jsonFaultResponse, new DataContractJsonSerializer(jsonFaultResponse.GetType()));
			fault.Properties.Add("WebBodyFormatMessageProperty", property);
			fault.Properties.Add(HttpResponseMessageProperty.Name, responseMessageProp);
		}

		public void ProvideFault(Exception error, HttpResponse httpResponse)
		{
			OWAFaultHandler.LogException(error);
			if (!httpResponse.IsClientConnected)
			{
				return;
			}
			httpResponse.TrySkipIisCustomErrors = true;
			httpResponse.StatusCode = 500;
			httpResponse.StatusDescription = Strings.GetLocalizedString(-1269063878);
			httpResponse.ContentType = "application/json; charset=utf-8";
			if (error is OwaExtendedErrorCodeException)
			{
				OwaExtendedErrorCodeException ex = (OwaExtendedErrorCodeException)error;
				OwaExtendedError.SendError(httpResponse.Headers, delegate(HttpStatusCode sc)
				{
					httpResponse.StatusCode = (int)sc;
				}, ex.ExtendedErrorCode, ex.Message, ex.User, ex.ErrorData);
			}
			else
			{
				MissingIdentityException ex2 = error as MissingIdentityException;
				if (ex2 != null)
				{
					httpResponse.StatusCode = (int)ex2.StatusCode;
					httpResponse.StatusDescription = ex2.StatusDescription;
					httpResponse.Headers[WellKnownHeader.MSDiagnostics] = (ex2.DiagnosticText ?? string.Empty);
					httpResponse.Headers[WellKnownHeader.Authentication] = (ex2.ChallengeString ?? string.Empty);
				}
			}
			JsonFaultResponse jsonFaultResponse = JsonFaultResponse.CreateFromException(error);
			DataContractJsonSerializer dataContractJsonSerializer = OWAFaultHandler.CreateJsonSerializer(jsonFaultResponse.GetType());
			dataContractJsonSerializer.WriteObject(httpResponse.OutputStream, jsonFaultResponse);
		}

		private static void LogException(Exception error)
		{
			if (!Globals.IsAnonymousCalendarApp)
			{
				RequestDetailsLogger.LogException(error, "OwaFaultHandler", "ProvideFault");
			}
			RequestContext requestContext = RequestContext.Current;
			if (requestContext != null)
			{
				ErrorHandlerUtilities.LogExceptionCodeInIIS(requestContext, error);
			}
		}

		private static DataContractJsonSerializer CreateJsonSerializer(Type objectType)
		{
			DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings
			{
				MaxItemsInObjectGraph = int.MaxValue
			};
			return new DataContractJsonSerializer(objectType, settings);
		}

		public static bool IsTransientError(Exception exception)
		{
			return exception is OwaTransientException || FaultExceptionUtilities.GetIsTransient(exception);
		}

		public static void SetBackOffPeriodInMs(Exception exception, JsonFaultBody response)
		{
			OverBudgetException ex = exception as OverBudgetException;
			if (ex != null)
			{
				response.BackOffPeriodInMs = ex.BackoffTime;
			}
		}

		private const string jsonContentType = "application/json; charset=utf-8";
	}
}

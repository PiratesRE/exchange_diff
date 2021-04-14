using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetLinkPreview : AsyncServiceCommand<GetLinkPreviewResponse>
	{
		public GetLinkPreview(CallContext callContext, GetLinkPreviewRequest request) : base(callContext)
		{
			this.request = request;
			OwsLogRegistry.Register(GetLinkPreview.GetLinkPreviewActionName, GetLinkPreview.GetLinkPreviewMetadataType, new Type[0]);
		}

		private static bool GetActiveViewsConvergenceFlightEnabled(RequestDetailsLogger logger)
		{
			UserContext userContext = UserContextManager.GetUserContext(CallContext.Current.HttpContext, CallContext.Current.EffectiveCaller, true);
			if (userContext == null)
			{
				logger.Set(GetLinkPreviewMetadata.UserContextNull, 1);
				logger.Set(GetLinkPreviewMetadata.ActiveViewConvergenceEnabled, 0);
				return false;
			}
			if (!userContext.FeaturesManager.ClientServerSettings.ActiveViewConvergence.Enabled)
			{
				logger.Set(GetLinkPreviewMetadata.ActiveViewConvergenceEnabled, 0);
				return false;
			}
			logger.Set(GetLinkPreviewMetadata.ActiveViewConvergenceEnabled, 1);
			return true;
		}

		protected override async Task<GetLinkPreviewResponse> InternalExecute()
		{
			GetLinkPreviewResponse getLinkPreviewResponse;
			try
			{
				DataProviderInformation dataProviderInformation = null;
				long elapsedTimeToWebPageStepCompletion = 0L;
				long elapsedTimeToRegExStepCompletion = 0L;
				base.CallContext.ProtocolLog.Set(GetLinkPreviewMetadata.Url, this.request.Url);
				if (GetLinkPreview.GetPreviewsDisabled())
				{
					return this.CreateDisabledResponse();
				}
				if (Interlocked.Increment(ref GetLinkPreview.getPreviewRequestCount) > GetLinkPreview.getPreviewRequestCountMax)
				{
					return this.CreateErrorResponse("MaxConcurrentRequestExceeded", "The maximum number of concurrent requests has been exceeded.");
				}
				bool activeViewConvergenceEnabled = GetLinkPreview.GetActiveViewsConvergenceFlightEnabled(base.CallContext.ProtocolLog);
				Stopwatch stopwatch = Stopwatch.StartNew();
				LinkPreviewDataProvider dataProvider = null;
				dataProvider = LinkPreviewDataProvider.GetDataProvider(this.request, base.CallContext.ProtocolLog, activeViewConvergenceEnabled);
				dataProviderInformation = await dataProvider.GetDataProviderInformation();
				elapsedTimeToWebPageStepCompletion = stopwatch.ElapsedMilliseconds;
				getLinkPreviewResponse = dataProvider.CreatePreview(dataProviderInformation);
				stopwatch.Stop();
				elapsedTimeToRegExStepCompletion = stopwatch.ElapsedMilliseconds;
				getLinkPreviewResponse.ElapsedTimeToWebPageStepCompletion = elapsedTimeToWebPageStepCompletion;
				getLinkPreviewResponse.ElapsedTimeToRegExStepCompletion = elapsedTimeToRegExStepCompletion;
				getLinkPreviewResponse.WebPageContentLength = dataProvider.ContentLength;
				this.LogWebMethodData(getLinkPreviewResponse);
			}
			catch (OwaPermanentException exception)
			{
				getLinkPreviewResponse = this.CreateErrorResponse(exception);
			}
			catch (LocalizedException exception2)
			{
				getLinkPreviewResponse = this.CreateErrorResponse(exception2);
			}
			catch (HttpRequestException requestException)
			{
				getLinkPreviewResponse = this.CreateErrorResponse(requestException);
			}
			catch (TaskCanceledException)
			{
				getLinkPreviewResponse = this.CreateErrorResponse("RequestTimeout", "The web page request timed out.");
			}
			catch (WebException webException)
			{
				getLinkPreviewResponse = this.CreateErrorResponse(webException);
			}
			finally
			{
				Interlocked.Decrement(ref GetLinkPreview.getPreviewRequestCount);
			}
			return getLinkPreviewResponse;
		}

		private void LogWebMethodData(GetLinkPreviewResponse getLinkPreviewResponse)
		{
			base.CallContext.ProtocolLog.Set(GetLinkPreviewMetadata.ElapsedTimeToWebPageStepCompletion, getLinkPreviewResponse.ElapsedTimeToWebPageStepCompletion);
			base.CallContext.ProtocolLog.Set(GetLinkPreviewMetadata.ElapsedTimeToRegExStepCompletion, getLinkPreviewResponse.ElapsedTimeToRegExStepCompletion);
			base.CallContext.ProtocolLog.Set(GetLinkPreviewMetadata.WebPageContentLength, getLinkPreviewResponse.WebPageContentLength);
			base.CallContext.ProtocolLog.Set(GetLinkPreviewMetadata.DescriptionTagCount, getLinkPreviewResponse.DescriptionTagCount);
			base.CallContext.ProtocolLog.Set(GetLinkPreviewMetadata.ImageTagCount, getLinkPreviewResponse.ImageTagCount);
		}

		private static bool GetPreviewsDisabled()
		{
			bool result = true;
			HttpContext httpContext = HttpContext.Current;
			if (httpContext != null && UserContextManager.GetUserContext(httpContext).FeaturesManager != null)
			{
				result = !UserContextManager.GetUserContext(httpContext).FeaturesManager.ClientServerSettings.InlinePreview.Enabled;
			}
			return result;
		}

		private static int CalculateGetPreviewRequestCountMax()
		{
			int num;
			int num2;
			ThreadPool.GetMaxThreads(out num, out num2);
			return (int)Math.Round((double)num2 * 0.1);
		}

		public static void ThrowInvalidRequestException(HttpResponseMessage responseMessage)
		{
			string error = responseMessage.StatusCode.ToString();
			string reasonPhrase = responseMessage.ReasonPhrase;
			OwaInvalidRequestException ex = new OwaInvalidRequestException(reasonPhrase);
			GetLinkPreview.SetErrorData(ex, error);
			throw ex;
		}

		public static void ThrowInvalidRequestException(string error, string errorMessage)
		{
			OwaInvalidRequestException ex = new OwaInvalidRequestException(errorMessage);
			GetLinkPreview.SetErrorData(ex, error);
			throw ex;
		}

		public static void ThrowLocalizedException(string error, LocalizedException localizedException)
		{
			GetLinkPreview.SetErrorData(localizedException, error);
			throw localizedException;
		}

		private static void SetErrorData(Exception exception, string error)
		{
			exception.Data["ErrorKey"] = error;
		}

		private static string GetExceptionMessage(Exception exception)
		{
			if (exception.InnerException != null)
			{
				return exception.InnerException.Message;
			}
			return exception.Message;
		}

		private GetLinkPreviewResponse CreateErrorResponse(HttpRequestException requestException)
		{
			WebException ex = requestException.InnerException as WebException;
			if (ex != null)
			{
				return this.CreateErrorResponse(ex);
			}
			string name = requestException.GetType().Name;
			string exceptionMessage = GetLinkPreview.GetExceptionMessage(requestException);
			return this.CreateErrorResponse(name, exceptionMessage);
		}

		private GetLinkPreviewResponse CreateErrorResponse(WebException webException)
		{
			string error;
			if (webException.Status == WebExceptionStatus.ProtocolError)
			{
				string str = null;
				HttpWebResponse httpWebResponse = webException.Response as HttpWebResponse;
				if (httpWebResponse != null)
				{
					str = ((int)httpWebResponse.StatusCode).ToString();
				}
				error = webException.Status.ToString() + str;
			}
			else
			{
				error = webException.Status.ToString();
			}
			string exceptionMessage = GetLinkPreview.GetExceptionMessage(webException);
			return this.CreateErrorResponse(error, exceptionMessage);
		}

		private GetLinkPreviewResponse CreateErrorResponse(Exception exception)
		{
			string error = (exception.Data["ErrorKey"] != null) ? exception.Data["ErrorKey"].ToString() : exception.GetType().Name;
			string exceptionMessage = GetLinkPreview.GetExceptionMessage(exception);
			return this.CreateErrorResponse(error, exceptionMessage);
		}

		private GetLinkPreviewResponse CreateErrorResponse(string error, string errorMessage)
		{
			base.CallContext.ProtocolLog.Set(GetLinkPreviewMetadata.Error, error);
			base.CallContext.ProtocolLog.Set(GetLinkPreviewMetadata.ErrorMessage, errorMessage);
			return new GetLinkPreviewResponse
			{
				Error = error,
				ErrorMessage = errorMessage
			};
		}

		private GetLinkPreviewResponse CreateDisabledResponse()
		{
			base.CallContext.ProtocolLog.Set(GetLinkPreviewMetadata.DisabledResponse, 1);
			return new GetLinkPreviewResponse
			{
				IsDisabled = true
			};
		}

		private const string ErrorKey = "ErrorKey";

		private const double IoThreadFractionForLinkPreviewRequests = 0.1;

		private readonly GetLinkPreviewRequest request;

		private static int getPreviewRequestCount;

		private static readonly int getPreviewRequestCountMax = GetLinkPreview.CalculateGetPreviewRequestCountMax();

		private static readonly string GetLinkPreviewActionName = typeof(GetLinkPreview).Name;

		private static readonly Type GetLinkPreviewMetadataType = typeof(GetLinkPreviewMetadata);
	}
}

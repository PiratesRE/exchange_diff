using System;
using System.Net;
using System.Web;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;

namespace Microsoft.Exchange.HttpProxy
{
	internal class AuthMetadataHttpHandler : IHttpHandler
	{
		public AuthMetadataHttpHandler(HttpContext context)
		{
			this.httpContext = context;
		}

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		private Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AuthMetadataTracer;
			}
		}

		private RequestDetailsLogger Logger
		{
			get
			{
				return RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(this.httpContext);
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			try
			{
				this.InternalProcessRequest(context);
			}
			catch (Exception ex)
			{
				this.Logger.AppendGenericError("AuthMetadataHttpHandler_UnhandledException", ex.ToString());
				this.Tracer.TraceError<Exception>((long)this.GetHashCode(), "[AuthMetadataHttpHandler.ProcessRequest] Unhandled exception occurred. Error: {0}", ex);
				throw;
			}
		}

		private void InternalProcessRequest(HttpContext context)
		{
			this.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "[AuthMetadataHttpHandler.InternalProcessRequest] Start processing request {0}.", this.Logger.ActivityId);
			HttpRequest request = context.Request;
			HttpResponse response = context.Response;
			this.Logger.ActivityScope.Action = AuthMetadataHttpHandler.ProtocolLogAction;
			try
			{
				string content = null;
				bool isError = true;
				if (request.Url.ToString().TrimEnd(AuthMetadataHttpHandler.TrailingSlash).EndsWith("metadata/json/1", StringComparison.OrdinalIgnoreCase))
				{
					isError = false;
					content = AuthMetadataBuilder.Singleton.Build(request.Url);
				}
				else
				{
					response.StatusCode = 404;
				}
				this.WriteResponse(response, content, isError);
			}
			catch (AuthMetadataBuilderException ex)
			{
				this.HandleBuilderExceptions(response, ex);
			}
			finally
			{
				this.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "[AuthMetadataHttpHandler.InternalProcessRequest] End processing request {0}.", this.Logger.ActivityId);
			}
		}

		private void HandleBuilderExceptions(HttpResponse response, AuthMetadataBuilderException ex)
		{
			if (ex is AuthMetadataInternalException)
			{
				this.ReportBuilderException(response, ex, true, HttpStatusCode.InternalServerError, new LocalizedString?(HttpProxyStrings.ErrorInternalServerError));
			}
		}

		private void ReportBuilderException(HttpResponse response, AuthMetadataBuilderException ex, bool logCallStack, HttpStatusCode httpStatusCode, LocalizedString? overridingError)
		{
			this.Logger.Set(ServiceCommonMetadata.ErrorCode, ex.GetType().Name);
			string value = logCallStack ? ex.ToString() : ex.Message;
			this.Logger.AppendGenericError(ex.GetType().Name, value);
			response.StatusCode = (int)httpStatusCode;
			response.TrySkipIisCustomErrors = true;
			LocalizedString? localizedString = new LocalizedString?(overridingError ?? ex.LocalizedString);
			LocalizedString? localizedString2 = localizedString;
			this.WriteResponse(response, (localizedString2 != null) ? localizedString2.GetValueOrDefault() : null, true);
		}

		private void WriteResponse(HttpResponse response, string content, bool isError)
		{
			response.Clear();
			response.ContentType = (isError ? "text/plain" : "text/json");
			response.Charset = "utf-8";
			response.Output.Write(content);
		}

		private static readonly string ProtocolLogAction = "AuthMetadata";

		private static readonly char[] TrailingSlash = new char[]
		{
			'/'
		};

		private HttpContext httpContext;
	}
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotosDiagnostics
	{
		private PhotosDiagnostics()
		{
		}

		public bool ShouldTraceGetPersonaPhotoRequest(HttpRequest request)
		{
			return request != null && request.QueryString.AllKeys.Contains("trace", StringComparer.OrdinalIgnoreCase);
		}

		public bool ShouldTraceGetUserPhotoRequest(HttpRequest request)
		{
			return request != null && (request.QueryString.AllKeys.Contains("trace", StringComparer.OrdinalIgnoreCase) || request.Headers.AllKeys.Contains("X-Exchange-GetUserPhoto-TraceEnabled", StringComparer.OrdinalIgnoreCase));
		}

		public void StampTracesOnGetPersonaPhotosResponse(ITracer tracer, OutgoingWebResponseContext response)
		{
			if (tracer == null || response == null)
			{
				return;
			}
			this.StampTracesOnPhotosResponse(tracer, response.Headers, "X-Exchange-GetPersonaPhoto-Traces");
		}

		public void StampTracesOnGetUserPhotosResponse(ITracer tracer, OutgoingWebResponseContext response)
		{
			if (tracer == null || response == null)
			{
				return;
			}
			this.StampTracesOnPhotosResponse(tracer, response.Headers, "X-Exchange-GetUserPhoto-Traces");
		}

		public void StampTracesOnGetUserPhotosResponse(ITracer tracer, HttpResponse response)
		{
			if (tracer == null || response == null)
			{
				return;
			}
			this.StampTracesOnPhotosResponse(tracer, response.Headers, "X-Exchange-GetUserPhoto-Traces");
		}

		public void StampGetUserPhotoTraceEnabledHeaders(Dictionary<string, string> headers)
		{
			if (headers == null)
			{
				return;
			}
			headers.Add("X-Exchange-GetUserPhoto-TraceEnabled", "1");
		}

		public void StampGetUserPhotoTraceEnabledHeaders(WebRequest request)
		{
			if (request == null)
			{
				return;
			}
			request.Headers.Set("X-Exchange-GetUserPhoto-TraceEnabled", "1");
		}

		public string ReadGetUserPhotoTracesFromResponseHeaders(Dictionary<string, string> headers)
		{
			if (headers == null)
			{
				return string.Empty;
			}
			string text;
			if (!headers.TryGetValue("X-Exchange-GetUserPhoto-Traces", out text) || string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			return text;
		}

		public string ReadGetUserPhotoTracesFromResponse(WebResponse response)
		{
			if (response == null || response.Headers == null || response.Headers.Count == 0)
			{
				return string.Empty;
			}
			return response.Headers["X-Exchange-GetUserPhoto-Traces"] ?? string.Empty;
		}

		public PhotoHandlers GetHandlersToSkip(HttpRequest request)
		{
			if (request == null)
			{
				return PhotoHandlers.None;
			}
			PhotoHandlers photoHandlers = PhotoHandlers.None;
			photoHandlers |= PhotosDiagnostics.IsHandlerToBeSkipped(request, PhotoHandlers.FileSystem, "skipfs");
			photoHandlers |= PhotosDiagnostics.IsHandlerToBeSkipped(request, PhotoHandlers.Mailbox, "skipmbx");
			photoHandlers |= PhotosDiagnostics.IsHandlerToBeSkipped(request, PhotoHandlers.ActiveDirectory, "skipad");
			photoHandlers |= PhotosDiagnostics.IsHandlerToBeSkipped(request, PhotoHandlers.Caching, "skipcaching");
			photoHandlers |= PhotosDiagnostics.IsHandlerToBeSkipped(request, PhotoHandlers.Http, "skiphttp");
			photoHandlers |= PhotosDiagnostics.IsHandlerToBeSkipped(request, PhotoHandlers.Private, "skipprv");
			return photoHandlers | PhotosDiagnostics.IsHandlerToBeSkipped(request, PhotoHandlers.RemoteForest, "skiprf");
		}

		public string GetHandlersToSkipQueryStringParametersWithLeadingAmpersand(PhotoRequest request)
		{
			if (request == null || request.HandlersToSkip == PhotoHandlers.None)
			{
				return string.Empty;
			}
			PhotoHandlers handlersToSkip = request.HandlersToSkip;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(PhotosDiagnostics.GetSkipHandlerQueryParameterWithLeadingAmpersand(PhotoHandlers.FileSystem, "&skipfs=1", handlersToSkip));
			stringBuilder.Append(PhotosDiagnostics.GetSkipHandlerQueryParameterWithLeadingAmpersand(PhotoHandlers.Mailbox, "&skipmbx=1", handlersToSkip));
			stringBuilder.Append(PhotosDiagnostics.GetSkipHandlerQueryParameterWithLeadingAmpersand(PhotoHandlers.ActiveDirectory, "&skipad=1", handlersToSkip));
			stringBuilder.Append(PhotosDiagnostics.GetSkipHandlerQueryParameterWithLeadingAmpersand(PhotoHandlers.Caching, "&skipcaching=1", handlersToSkip));
			stringBuilder.Append(PhotosDiagnostics.GetSkipHandlerQueryParameterWithLeadingAmpersand(PhotoHandlers.Http, "&skiphttp=1", handlersToSkip));
			stringBuilder.Append(PhotosDiagnostics.GetSkipHandlerQueryParameterWithLeadingAmpersand(PhotoHandlers.Private, "&skipprv=1", handlersToSkip));
			stringBuilder.Append(PhotosDiagnostics.GetSkipHandlerQueryParameterWithLeadingAmpersand(PhotoHandlers.RemoteForest, "&skiprf=1", handlersToSkip));
			return stringBuilder.ToString();
		}

		private static PhotoHandlers IsHandlerToBeSkipped(HttpRequest request, PhotoHandlers handler, string skipHandlerQueryStringParameter)
		{
			if (!request.QueryString.AllKeys.Contains(skipHandlerQueryStringParameter, StringComparer.OrdinalIgnoreCase))
			{
				return PhotoHandlers.None;
			}
			return handler;
		}

		private static string GetSkipHandlerQueryParameterWithLeadingAmpersand(PhotoHandlers handlerToTest, string skipHandlerQueryStringParameterWithLeadingAmpersand, PhotoHandlers handlersToSkip)
		{
			if ((handlersToSkip & handlerToTest) != PhotoHandlers.None)
			{
				return skipHandlerQueryStringParameterWithLeadingAmpersand;
			}
			return string.Empty;
		}

		private void StampTracesOnPhotosResponse(ITracer tracer, NameValueCollection responseHeaders, string header)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				tracer.Dump(stringWriter, true, true);
			}
			responseHeaders[header] = this.SanitizeHttpHeaderValue(stringBuilder.ToString());
		}

		private string SanitizeHttpHeaderValue(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(s.Length);
			foreach (char c in s)
			{
				if (c == '\r')
				{
					stringBuilder.Append("\\r");
				}
				else if (c == '\n')
				{
					stringBuilder.Append("\\n");
				}
				else if (c >= ' ' && c < '\u007f')
				{
					stringBuilder.Append(c);
				}
				else
				{
					stringBuilder.Append('.');
				}
			}
			return stringBuilder.ToString();
		}

		private const char Space = ' ';

		private const char Delete = '\u007f';

		private const string TraceEnabledQueryStringParameter = "trace";

		private const string GetUserPhotoTraceEnabledHeaderName = "X-Exchange-GetUserPhoto-TraceEnabled";

		private const string GetUserPhotoTraceEnabledHeaderValue = "1";

		private const string GetPersonaPhotoTracesHeaderName = "X-Exchange-GetPersonaPhoto-Traces";

		private const string GetUserPhotoTracesHeaderName = "X-Exchange-GetUserPhoto-Traces";

		public static readonly PhotosDiagnostics Instance = new PhotosDiagnostics();
	}
}

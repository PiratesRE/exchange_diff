using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.SoapWebClient;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[XmlInclude(typeof(BaseRequestType))]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[WebServiceBinding(Name = "ServiceSoap", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	internal class Service : CustomSoapHttpClientProtocol, IService
	{
		internal Service(WebServiceUri webServiceUri) : base(Globals.CertificateValidationComponentId, new RemoteCertificateValidationCallback(CertificateErrorHandler.CertValidationCallback), true, false)
		{
			base.Url = webServiceUri.Uri.OriginalString;
			this.serviceVersion = webServiceUri.ServerVersion;
		}

		internal override XmlNamespaceDefinition[] PredefinedNamespaces
		{
			get
			{
				return Constants.EwsNamespaces;
			}
		}

		public RequestTypeHeader requestTypeValue
		{
			get
			{
				return this.requestTypeValueField;
			}
			set
			{
				this.requestTypeValueField = value;
			}
		}

		public RequestServerVersion RequestServerVersionValue
		{
			get
			{
				return this.requestServerVersionValueField;
			}
			set
			{
				this.requestServerVersionValueField = value;
			}
		}

		public TimeZoneContext TimeZoneDefinitionContextValue
		{
			get
			{
				return this.timeZoneDefinitionContextValueField;
			}
			set
			{
				this.timeZoneDefinitionContextValueField = value;
			}
		}

		public int ServiceVersion
		{
			get
			{
				return this.serviceVersion;
			}
		}

		public bool SupportsProxyAuthentication
		{
			get
			{
				return true;
			}
		}

		[SoapHeader("TimeZoneDefinitionContextValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetUserAvailability", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("requestTypeValue")]
		[return: XmlElement("GetUserAvailabilityResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetUserAvailabilityResponse GetUserAvailability([XmlElement(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", ElementName = "GetUserAvailabilityRequest")] GetUserAvailabilityRequest getUserAvailabilityRequest)
		{
			object[] array = this.Invoke("GetUserAvailability", new object[]
			{
				getUserAvailabilityRequest
			});
			return (GetUserAvailabilityResponse)array[0];
		}

		public IAsyncResult BeginGetUserAvailability(GetUserAvailabilityRequest request, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetUserAvailability", new object[]
			{
				request
			}, callback, asyncState);
		}

		public GetUserAvailabilityResponse EndGetUserAvailability(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetUserAvailabilityResponse)array[0];
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetMailTips", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("requestTypeValue")]
		[return: XmlElement("GetMailTipsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetMailTipsResponseMessageType GetMailTips([XmlElement("GetMailTips", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetMailTipsType getMailTips1)
		{
			object[] array = this.Invoke("GetMailTips", new object[]
			{
				getMailTips1
			});
			return (GetMailTipsResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetMailTips(GetMailTipsType getMailTips1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetMailTips", new object[]
			{
				getMailTips1
			}, callback, asyncState);
		}

		public GetMailTipsResponseMessageType EndGetMailTips(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetMailTipsResponseMessageType)array[0];
		}

		internal Service(WebServiceUri webServiceUri, bool traceRequest, ITracer upstreamTracer) : base(Globals.CertificateValidationComponentId, new RemoteCertificateValidationCallback(CertificateErrorHandler.CertValidationCallback), true)
		{
			this.traceRequest = traceRequest;
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
			base.Url = webServiceUri.Uri.OriginalString;
			this.serviceVersion = webServiceUri.ServerVersion;
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetUserPhoto", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("requestTypeValue")]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("GetUserPhotoResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetUserPhotoResponseMessageType GetUserPhoto([XmlElement("GetUserPhoto", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetUserPhotoRequestType getUserPhoto1)
		{
			object[] array = this.Invoke("GetUserPhoto", new object[]
			{
				getUserPhoto1
			});
			return (GetUserPhotoResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetUserPhoto(PhotoRequest request, PhotosConfiguration configuration, AsyncCallback callback, object asyncState)
		{
			base.HttpHeaders.Add("If-None-Match", request.ETag);
			if (this.traceRequest)
			{
				PhotosDiagnostics.Instance.StampGetUserPhotoTraceEnabledHeaders(base.HttpHeaders);
			}
			return base.BeginInvoke("GetUserPhoto", new GetUserPhotoRequestType[]
			{
				Service.ConvertPhotoRequestToGetUserPhotoRequestType(request)
			}, callback, asyncState);
		}

		public GetUserPhotoResponseMessageType EndGetUserPhoto(IAsyncResult asyncResult)
		{
			object[] array;
			try
			{
				array = base.EndInvoke(asyncResult);
			}
			catch (WebException ex)
			{
				this.WriteTracesCollectedByRemoteServerOntoLocalTracer(PhotosDiagnostics.Instance.ReadGetUserPhotoTracesFromResponse(ex.Response));
				throw;
			}
			GetUserPhotoResponseMessageType getUserPhotoResponseMessageType = (GetUserPhotoResponseMessageType)array[0];
			this.WriteTracesCollectedByRemoteServerOntoLocalTracer(PhotosDiagnostics.Instance.ReadGetUserPhotoTracesFromResponseHeaders(base.ResponseHttpHeaders));
			if (getUserPhotoResponseMessageType.ResponseClass != ResponseClassType.Success)
			{
				this.tracer.TraceError((long)this.GetHashCode(), "Request to remote service FAILED.  Returning HTTP 500: Internal Server Error.");
				getUserPhotoResponseMessageType.StatusCode = HttpStatusCode.InternalServerError;
				return getUserPhotoResponseMessageType;
			}
			if (!getUserPhotoResponseMessageType.HasChanged)
			{
				getUserPhotoResponseMessageType.StatusCode = HttpStatusCode.NotModified;
			}
			else
			{
				getUserPhotoResponseMessageType.StatusCode = HttpStatusCode.OK;
			}
			string text;
			if (!base.ResponseHttpHeaders.TryGetValue("ETag", out text) || string.IsNullOrEmpty(text))
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "Remote service returned NO ETag or ETag is blank.");
			}
			getUserPhotoResponseMessageType.CacheId = text;
			string text2;
			if (!base.ResponseHttpHeaders.TryGetValue("Expires", out text2) || string.IsNullOrEmpty(text2))
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "Remote service returned NO Expires header or header is blank.");
			}
			getUserPhotoResponseMessageType.Expires = text2;
			this.tracer.TraceDebug((long)this.GetHashCode(), "Returning message from proxy with cacheId: {0}; HTTP status: {1};  HTTP Expires: {2};  Content-type: '{3}'", new object[]
			{
				getUserPhotoResponseMessageType.CacheId,
				getUserPhotoResponseMessageType.StatusCode,
				getUserPhotoResponseMessageType.Expires,
				getUserPhotoResponseMessageType.ContentType
			});
			return getUserPhotoResponseMessageType;
		}

		private void WriteTracesCollectedByRemoteServerOntoLocalTracer(string traces)
		{
			if (string.IsNullOrEmpty(traces))
			{
				return;
			}
			this.tracer.TraceDebug<string, string>((long)this.GetHashCode(), "Traces collected by {0}: [[ {1} ]]", base.Url, traces);
		}

		private static GetUserPhotoRequestType ConvertPhotoRequestToGetUserPhotoRequestType(PhotoRequest request)
		{
			return new GetUserPhotoRequestType
			{
				Email = request.TargetSmtpAddress,
				SizeRequested = (UserPhotoSize)request.Size
			};
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/FindMessageTrackingReport", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("requestTypeValue")]
		[return: XmlElement("FindMessageTrackingReportResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public FindMessageTrackingReportResponseMessageType FindMessageTrackingReport([XmlElement("FindMessageTrackingReport", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] FindMessageTrackingReportRequestType findMessageTrackingReport1)
		{
			object[] array = this.Invoke("FindMessageTrackingReport", new object[]
			{
				findMessageTrackingReport1
			});
			return (FindMessageTrackingReportResponseMessageType)array[0];
		}

		public IAsyncResult BeginFindMessageTrackingReport(FindMessageTrackingReportRequestType findMessageTrackingReport1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("FindMessageTrackingReport", new object[]
			{
				findMessageTrackingReport1
			}, callback, asyncState);
		}

		public FindMessageTrackingReportResponseMessageType EndFindMessageTrackingReport(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (FindMessageTrackingReportResponseMessageType)array[0];
		}

		[SoapHeader("requestTypeValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetMessageTrackingReport", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("GetMessageTrackingReportResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetMessageTrackingReportResponseMessageType GetMessageTrackingReport([XmlElement("GetMessageTrackingReport", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetMessageTrackingReportRequestType getMessageTrackingReport1)
		{
			object[] array = this.Invoke("GetMessageTrackingReport", new object[]
			{
				getMessageTrackingReport1
			});
			return (GetMessageTrackingReportResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetMessageTrackingReport(GetMessageTrackingReportRequestType getMessageTrackingReport1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetMessageTrackingReport", new object[]
			{
				getMessageTrackingReport1
			}, callback, asyncState);
		}

		public GetMessageTrackingReportResponseMessageType EndGetMessageTrackingReport(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetMessageTrackingReportResponseMessageType)array[0];
		}

		CookieContainer IService.get_CookieContainer()
		{
			return base.CookieContainer;
		}

		void IService.set_CookieContainer(CookieContainer A_1)
		{
			base.CookieContainer = A_1;
		}

		IWebProxy IService.get_Proxy()
		{
			return base.Proxy;
		}

		void IService.set_Proxy(IWebProxy A_1)
		{
			base.Proxy = A_1;
		}

		ICredentials IService.get_Credentials()
		{
			return base.Credentials;
		}

		void IService.set_Credentials(ICredentials A_1)
		{
			base.Credentials = A_1;
		}

		bool IService.get_EnableDecompression()
		{
			return base.EnableDecompression;
		}

		void IService.set_EnableDecompression(bool A_1)
		{
			base.EnableDecompression = A_1;
		}

		string IService.get_UserAgent()
		{
			return base.UserAgent;
		}

		void IService.set_UserAgent(string A_1)
		{
			base.UserAgent = A_1;
		}

		string IService.get_Url()
		{
			return base.Url;
		}

		int IService.get_Timeout()
		{
			return base.Timeout;
		}

		void IService.set_Timeout(int A_1)
		{
			base.Timeout = A_1;
		}

		Dictionary<string, string> IService.get_HttpHeaders()
		{
			return base.HttpHeaders;
		}

		private RequestServerVersion requestServerVersionValueField;

		private RequestTypeHeader requestTypeValueField;

		private TimeZoneContext timeZoneDefinitionContextValueField;

		private int serviceVersion;

		private readonly ITracer tracer;

		private readonly bool traceRequest;
	}
}

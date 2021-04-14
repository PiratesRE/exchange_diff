using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	internal interface IService
	{
		CookieContainer CookieContainer { get; set; }

		IWebProxy Proxy { get; set; }

		ICredentials Credentials { get; set; }

		bool EnableDecompression { get; set; }

		string UserAgent { get; set; }

		string Url { get; }

		int Timeout { get; set; }

		RequestTypeHeader requestTypeValue { get; set; }

		RequestServerVersion RequestServerVersionValue { get; set; }

		int ServiceVersion { get; }

		Dictionary<string, string> HttpHeaders { get; }

		bool SupportsProxyAuthentication { get; }

		void Abort();

		void Dispose();

		IAsyncResult BeginGetUserPhoto(PhotoRequest request, PhotosConfiguration configuration, AsyncCallback callback, object asyncState);

		GetUserPhotoResponseMessageType EndGetUserPhoto(IAsyncResult asyncResult);

		IAsyncResult BeginGetUserAvailability(GetUserAvailabilityRequest request, AsyncCallback callback, object asyncState);

		GetUserAvailabilityResponse EndGetUserAvailability(IAsyncResult asyncResult);

		IAsyncResult BeginGetMailTips(GetMailTipsType getMailTips1, AsyncCallback callback, object asyncState);

		GetMailTipsResponseMessageType EndGetMailTips(IAsyncResult asyncResult);

		IAsyncResult BeginFindMessageTrackingReport(FindMessageTrackingReportRequestType findMessageTrackingReport1, AsyncCallback callback, object asyncState);

		FindMessageTrackingReportResponseMessageType EndFindMessageTrackingReport(IAsyncResult asyncResult);

		IAsyncResult BeginGetMessageTrackingReport(GetMessageTrackingReportRequestType getMessageTrackingReport1, AsyncCallback callback, object asyncState);

		GetMessageTrackingReportResponseMessageType EndGetMessageTrackingReport(IAsyncResult asyncResult);
	}
}

using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal interface IHttpSession
	{
		event EventHandler<HttpWebEventArgs> SendingRequest;

		event EventHandler<HttpWebEventArgs> ResponseReceived;

		event EventHandler<TestEventArgs> TestStarted;

		event EventHandler<TestEventArgs> TestFinished;

		object EventState { get; set; }

		string UserAgent { get; set; }

		IResponseTracker ResponseTracker { get; }

		ExCookieContainer CookieContainer { get; set; }

		Dictionary<string, string> PersistentHeaders { get; }

		AuthenticationData? AuthenticationData { get; set; }

		SslValidationOptions SslValidationOptions { get; set; }

		void NotifyTestStarted(TestId testId);

		void NotifyTestFinished(TestId testId);

		IAsyncResult BeginGet(TestId stepId, string uri, AsyncCallback callback, Dictionary<string, object> asyncState);

		T EndGet<T>(IAsyncResult result, Func<HttpWebResponseWrapper, T> processResponse);

		T EndGet<T>(IAsyncResult result, HttpStatusCode[] expectedStatusCodes, Func<HttpWebResponseWrapper, T> processResponse);

		IAsyncResult BeginPost(TestId stepId, string uri, RequestBody body, string contentType, Dictionary<string, string> headers, AsyncCallback callback, Dictionary<string, object> asyncState);

		IAsyncResult BeginPost(TestId stepId, string uri, RequestBody body, string contentType, AsyncCallback callback, Dictionary<string, object> asyncState);

		IAsyncResult BeginPostFollowingRedirections(TestId stepId, Uri uri, RequestBody body, string contentType, Dictionary<string, string> headers, RedirectionOptions redirectionOptions, AsyncCallback callback, Dictionary<string, object> asyncState);

		IAsyncResult BeginPostFollowingRedirections(TestId stepId, string uri, RequestBody body, string contentType, Dictionary<string, string> headers, RedirectionOptions redirectionOptions, AsyncCallback callback, Dictionary<string, object> asyncState);

		T EndPost<T>(IAsyncResult result, Func<HttpWebResponseWrapper, T> processResponse);

		T EndPost<T>(IAsyncResult result, HttpStatusCode[] expectedStatusCodes, Func<HttpWebResponseWrapper, T> processResponse);

		IAsyncResult BeginGetFollowingRedirections(TestId stepId, string uri, AsyncCallback callback, Dictionary<string, object> asyncState);

		IAsyncResult BeginGetFollowingRedirections(TestId stepId, Uri uri, RedirectionOptions redirectionOptions, AsyncCallback callback, Dictionary<string, object> asyncState);

		IAsyncResult BeginGetFollowingRedirections(TestId stepId, string uri, RedirectionOptions redirectionOptions, AsyncCallback callback, Dictionary<string, object> asyncState);

		T EndGetFollowingRedirections<T>(IAsyncResult result, Func<HttpWebResponseWrapper, T> processResponse);

		T EndPostFollowingRedirections<T>(IAsyncResult result, Func<HttpWebResponseWrapper, T> processResponse);

		void CloseConnections();

		ScenarioException VerifyScenarioExceededRunTime(TimeSpan? maxAllowedTime);

		List<string> GetHostNames(RequestTarget requestTarget);
	}
}

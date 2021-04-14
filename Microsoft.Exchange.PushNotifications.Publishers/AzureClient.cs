using System;
using System.IO;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureClient : HttpClientBase
	{
		public AzureClient(HttpClient httpClient) : base(httpClient)
		{
		}

		public virtual ICancelableAsyncResult BeginSendNotificationRequest(AzureSendRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			return base.HttpClient.BeginDownload(request.Uri, request, null, null);
		}

		public virtual AzureResponse EndSendNotificationRequest(ICancelableAsyncResult asyncResult)
		{
			return this.EndRequest(asyncResult);
		}

		public virtual ICancelableAsyncResult BeginReadRegistrationRequest(AzureReadRegistrationRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			return base.HttpClient.BeginDownload(request.Uri, request, null, null);
		}

		public virtual AzureReadRegistrationResponse EndReadRegistrationRequest(ICancelableAsyncResult asyncResult)
		{
			ArgumentValidator.ThrowIfNull("asyncResult", asyncResult);
			DownloadResult result = base.HttpClient.EndDownload(asyncResult);
			string responseBody = this.GetResponseBody(result);
			if (result.IsSucceeded)
			{
				return new AzureReadRegistrationResponse(responseBody, result.ResponseHeaders);
			}
			WebException ex = result.Exception as WebException;
			if (ex == null)
			{
				return new AzureReadRegistrationResponse(result.Exception, result.LastKnownRequestedUri, responseBody);
			}
			return new AzureReadRegistrationResponse(ex, result.LastKnownRequestedUri, responseBody);
		}

		public virtual ICancelableAsyncResult BeginNewRegistrationRequest(AzureNewRegistrationRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			return base.HttpClient.BeginDownload(request.Uri, request, null, null);
		}

		public virtual AzureResponse EndNewRegistrationRequest(ICancelableAsyncResult asyncResult)
		{
			return this.EndRequest(asyncResult);
		}

		public virtual ICancelableAsyncResult BeginCreateOrUpdateRegistrationRequest(AzureCreateOrUpdateRegistrationRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			return base.HttpClient.BeginDownload(request.Uri, request, null, null);
		}

		public virtual AzureResponse EndCreateOrUpdateRegistrationRequest(ICancelableAsyncResult asyncResult)
		{
			return this.EndRequest(asyncResult);
		}

		public virtual ICancelableAsyncResult BeginNewRegistrationIdRequest(AzureNewRegistrationIdRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			return base.HttpClient.BeginDownload(request.Uri, request, null, null);
		}

		public virtual AzureNewRegistrationIdResponse EndNewRegistrationIdRequest(ICancelableAsyncResult asyncResult)
		{
			DownloadResult result = base.HttpClient.EndDownload(asyncResult);
			string responseBody = this.GetResponseBody(result);
			if (result.IsSucceeded)
			{
				return new AzureNewRegistrationIdResponse(responseBody, result.ResponseHeaders);
			}
			WebException ex = result.Exception as WebException;
			if (ex == null)
			{
				return new AzureNewRegistrationIdResponse(result.Exception, result.LastKnownRequestedUri, responseBody);
			}
			return new AzureNewRegistrationIdResponse(ex, result.LastKnownRequestedUri, responseBody);
		}

		public virtual ICancelableAsyncResult BeginSendAuthRequest(AcsAuthRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			return base.HttpClient.BeginDownload(request.Uri, request, null, null);
		}

		public virtual AzureResponse EndSendAuthRequest(ICancelableAsyncResult asyncResult)
		{
			return this.EndRequest(asyncResult);
		}

		public virtual ICancelableAsyncResult BeginHubCretionRequest(AzureHubCreationRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			return base.HttpClient.BeginDownload(request.Uri, request, null, null);
		}

		public virtual AzureResponse EndHubCretionRequest(ICancelableAsyncResult asyncResult)
		{
			return this.EndRequest(asyncResult);
		}

		public virtual ICancelableAsyncResult BeginRegistrationChallengeRequest(AzureRegistrationChallengeRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			return base.HttpClient.BeginDownload(request.Uri, request, null, null);
		}

		public virtual AzureResponse EndRegistrationChallengeRequest(ICancelableAsyncResult asyncResult)
		{
			return this.EndRequest(asyncResult);
		}

		private AzureResponse EndRequest(ICancelableAsyncResult asyncResult)
		{
			ArgumentValidator.ThrowIfNull("asyncResult", asyncResult);
			DownloadResult result = base.HttpClient.EndDownload(asyncResult);
			string responseBody = this.GetResponseBody(result);
			if (result.IsSucceeded)
			{
				return new AzureResponse(responseBody, result.ResponseHeaders);
			}
			WebException ex = result.Exception as WebException;
			if (ex == null)
			{
				return new AzureResponse(result.Exception, result.LastKnownRequestedUri, responseBody);
			}
			return new AzureResponse(ex, result.LastKnownRequestedUri, responseBody);
		}

		private string GetResponseBody(DownloadResult result)
		{
			string result2 = string.Empty;
			if (result.ResponseStream != null)
			{
				using (StreamReader streamReader = new StreamReader(result.ResponseStream))
				{
					result2 = streamReader.ReadToEnd();
				}
			}
			return result2;
		}
	}
}

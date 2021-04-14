using System;
using System.IO;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class GcmClient : HttpClientBase
	{
		public GcmClient(Uri gcmUri, HttpClient httpClient) : base(httpClient)
		{
			ArgumentValidator.ThrowIfNull("gcmUri", gcmUri);
			this.GcmUri = gcmUri;
		}

		private Uri GcmUri { get; set; }

		public virtual ICancelableAsyncResult BeginSendNotification(GcmRequest notificationRequest)
		{
			return base.HttpClient.BeginDownload(this.GcmUri, notificationRequest, null, null);
		}

		public virtual GcmResponse EndSendNotification(ICancelableAsyncResult asyncResult)
		{
			DownloadResult downloadResult = base.HttpClient.EndDownload(asyncResult);
			GcmResponse result;
			using (downloadResult.ResponseStream)
			{
				Exception ex = null;
				if (downloadResult.IsSucceeded)
				{
					try
					{
						StreamReader streamReader = new StreamReader(downloadResult.ResponseStream);
						string responseBody = streamReader.ReadToEnd();
						return new GcmResponse(responseBody);
					}
					catch (ArgumentNullException ex2)
					{
						ex = ex2;
						goto IL_5C;
					}
					catch (ArgumentException ex3)
					{
						ex = ex3;
						goto IL_5C;
					}
					catch (IOException ex4)
					{
						ex = ex4;
						goto IL_5C;
					}
				}
				ex = downloadResult.Exception;
				IL_5C:
				WebException ex5 = ex as WebException;
				result = ((ex5 != null) ? new GcmResponse(ex5) : new GcmResponse(ex));
			}
			return result;
		}
	}
}

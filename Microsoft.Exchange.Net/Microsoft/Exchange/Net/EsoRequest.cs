using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Net
{
	internal class EsoRequest : HttpSessionConfig, IDisposable
	{
		static EsoRequest()
		{
			CertificateValidationManager.RegisterCallback("ExchangeServiceToOwa", new RemoteCertificateValidationCallback(EsoRequest.ValidateRemoteCertificate));
		}

		public EsoRequest(string action, string userAgentBinder, string payload)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("action", action);
			this.uri = new Uri("https://127.0.0.1:444/owa/service.svc?action=" + action);
			base.Method = "POST";
			base.UserAgent = "EsoRequest/" + userAgentBinder;
			base.ContentType = "application/json; charset=UTF-8";
			base.UnsafeAuthenticatedConnectionSharing = true;
			base.PreAuthenticate = true;
			base.AllowAutoRedirect = false;
			base.Credentials = CredentialCache.DefaultNetworkCredentials;
			base.Headers = new WebHeaderCollection();
			base.Headers.Add("Action", action);
			base.Headers.Add("ActionId", EsoRequest.GetNextID().ToString());
			CertificateValidationManager.SetComponentId(base.Headers, "ExchangeServiceToOwa");
			base.RequestStream = new MemoryStream(string.IsNullOrEmpty(payload) ? new byte[0] : Encoding.UTF8.GetBytes(payload));
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				if (disposing && base.RequestStream != null)
				{
					base.RequestStream.Close();
				}
				base.RequestStream = null;
				this.isDisposed = true;
			}
		}

		public static bool IsEsoRequest(HttpRequest request)
		{
			string value = request.Headers["Action"];
			return !string.IsNullOrWhiteSpace(value) && EsoRequest.WellKnownActions.All.Contains(value);
		}

		public ICancelableAsyncResult BeginSend()
		{
			HttpClient httpClient = new HttpClient();
			return httpClient.BeginDownload(this.uri, this, null, httpClient);
		}

		public virtual DownloadResult EndSend(ICancelableAsyncResult asyncResult)
		{
			DownloadResult result;
			using (HttpClient httpClient = (HttpClient)asyncResult.AsyncState)
			{
				result = httpClient.EndDownload(asyncResult);
			}
			return result;
		}

		private static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
		{
			return true;
		}

		private static long GetNextID()
		{
			return Interlocked.Increment(ref EsoRequest.idCounter);
		}

		private const string ComponentId = "ExchangeServiceToOwa";

		private const string LocalhostBackendUri = "https://127.0.0.1:444/owa/service.svc?action=";

		private const string EsoContentType = "application/json; charset=UTF-8";

		private const string UserAgentPrefix = "EsoRequest/";

		private const string ActionHeader = "Action";

		private const string ActionIdHeader = "ActionId";

		private static long idCounter;

		private Uri uri;

		private bool isDisposed;

		internal class WellKnownActions
		{
			static WellKnownActions()
			{
				string[] list = new string[]
				{
					"PublishO365Notification"
				};
				EsoRequest.WellKnownActions.All = new ReadOnlyCollection<string>(list);
			}

			public const string PublishO365Notification = "PublishO365Notification";

			public static readonly ReadOnlyCollection<string> All;
		}
	}
}

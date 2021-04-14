using System;
using System.IO;
using System.Net;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal sealed class HttpProtocol : IDisposable
	{
		public HttpProtocol()
		{
			ServicePointManager.DefaultConnectionLimit = int.MaxValue;
			this.disposed = false;
		}

		public static void QueryFileNameSize(ref DownloadFileInfo download)
		{
			Logger.LoggerMessage("Attempting to connect to the remote server and getting the filesize...");
			WebResponse webResponse = null;
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(download.UriLink);
				httpWebRequest.KeepAlive = false;
				httpWebRequest.Timeout = 60000;
				webResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				download.FilePath = HttpProtocol.GetFilenameFromUri(webResponse.ResponseUri.AbsoluteUri);
				download.FileSize = webResponse.ContentLength;
			}
			finally
			{
				if (webResponse != null)
				{
					webResponse.Close();
				}
			}
		}

		private static string GetFilenameFromUri(string absoluteUri)
		{
			if (string.IsNullOrEmpty(absoluteUri))
			{
				return string.Empty;
			}
			int num = absoluteUri.LastIndexOf("/");
			if (num == -1)
			{
				return string.Empty;
			}
			int num2 = num + 1;
			if (num2 >= absoluteUri.Length)
			{
				return string.Empty;
			}
			return absoluteUri.Substring(num2);
		}

		public Stream GetStream(int startPosition, int endPosition, Uri downloadUrl, int numberOfThreads)
		{
			this.request = (HttpWebRequest)WebRequest.Create(downloadUrl);
			this.request.Timeout = 60000;
			this.request.KeepAlive = false;
			if (numberOfThreads > 1)
			{
				this.request.AddRange(startPosition, endPosition);
				this.response = (HttpWebResponse)this.request.GetResponse();
				if (this.response.StatusCode == HttpStatusCode.PartialContent)
				{
					return this.response.GetResponseStream();
				}
				this.request.Abort();
				return null;
			}
			else
			{
				this.response = (HttpWebResponse)this.request.GetResponse();
				if (this.response.StatusCode == HttpStatusCode.OK)
				{
					return this.response.GetResponseStream();
				}
				this.request.Abort();
				return null;
			}
		}

		public void CancelDownload()
		{
			this.request.Abort();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing && this.response != null)
				{
					this.response.Close();
				}
				this.response = null;
				this.disposed = true;
			}
		}

		private const int TimeoutValue = 60000;

		private HttpWebResponse response;

		private HttpWebRequest request;

		private bool disposed;
	}
}

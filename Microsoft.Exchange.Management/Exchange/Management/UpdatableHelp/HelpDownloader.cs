using System;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.UpdatableHelp
{
	internal class HelpDownloader
	{
		internal HelpDownloader(HelpUpdater updater)
		{
			this.helpUpdater = updater;
		}

		internal void DownloadManifest()
		{
			string downloadUrl = this.ResolveUri(this.helpUpdater.ManifestUrl);
			if (!this.helpUpdater.Cmdlet.Abort)
			{
				this.AsyncDownloadFile(UpdatableHelpStrings.UpdateComponentManifest, downloadUrl, this.helpUpdater.LocalManifestPath, 30000, new DownloadProgressChangedEventHandler(this.OnManifestProgressChanged), new AsyncCompletedEventHandler(this.OnManifestDownloadCompleted));
			}
		}

		internal void DownloadPackage(string packageUrl)
		{
			this.AsyncDownloadFile(UpdatableHelpStrings.UpdateComponentCabinet, packageUrl, this.helpUpdater.LocalCabinetPath, 120000, new DownloadProgressChangedEventHandler(this.OnCabinetProgressChanged), new AsyncCompletedEventHandler(this.OnCabinetDownloadCompleted));
		}

		internal UpdatableHelpVersionRange SearchManifestForApplicableUpdates(UpdatableHelpVersion currentVersion, int currentRevision)
		{
			StreamReader streamReader = new StreamReader(this.helpUpdater.LocalManifestPath);
			string xml = streamReader.ReadToEnd();
			streamReader.Close();
			HelpSchema helpSchema = new HelpSchema();
			return helpSchema.ParseManifestForApplicableUpdates(xml, currentVersion, currentRevision);
		}

		private void AsyncDownloadFile(string description, string downloadUrl, string localFilePath, int timeoutMilliseconds, DownloadProgressChangedEventHandler progressHandler, AsyncCompletedEventHandler completionHandler)
		{
			LocalizedString value = UpdatableHelpStrings.UpdateDownloadComplete;
			using (WebClient webClient = new WebClient())
			{
				AutoResetEvent autoResetEvent = new AutoResetEvent(false);
				webClient.DownloadProgressChanged += progressHandler;
				webClient.DownloadFileCompleted += completionHandler;
				this.downloadException = null;
				webClient.DownloadFileAsync(new Uri(downloadUrl), localFilePath, autoResetEvent);
				DateTime utcNow = DateTime.UtcNow;
				int num = 0;
				while (!this.helpUpdater.Cmdlet.Abort && num <= timeoutMilliseconds)
				{
					num += 100;
					if (autoResetEvent.WaitOne(100))
					{
						IL_78:
						if (num > timeoutMilliseconds)
						{
							value = UpdatableHelpStrings.UpdateDownloadTimeout;
						}
						TimeSpan timeSpan = DateTime.UtcNow - utcNow;
						string elapsedTime = string.Format("{0}.{1} seconds", timeSpan.TotalSeconds, timeSpan.TotalMilliseconds.ToString().PadLeft(3, '0'));
						this.helpUpdater.Cmdlet.WriteVerbose(UpdatableHelpStrings.UpdateDownloadTimeElapsed(description, value, elapsedTime));
						goto IL_EB;
					}
				}
				value = UpdatableHelpStrings.UpdateDownloadCancelled;
				webClient.CancelAsync();
				goto IL_78;
			}
			IL_EB:
			if (this.downloadException != null)
			{
				throw this.downloadException;
			}
		}

		private void OnManifestProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			this.OnProgressChanged(sender, e, UpdatePhase.Checking, UpdatableHelpStrings.UpdateSubtaskCheckingManifest);
		}

		private void OnCabinetProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			this.OnProgressChanged(sender, e, UpdatePhase.Downloading, LocalizedString.Empty);
		}

		private void OnProgressChanged(object sender, DownloadProgressChangedEventArgs e, UpdatePhase phase, LocalizedString subTask)
		{
		}

		private void OnManifestDownloadCompleted(object sender, AsyncCompletedEventArgs e)
		{
			this.OnDownloadCompleted(sender, e, UpdatePhase.Checking, UpdatableHelpStrings.UpdateDownloadManifestFailureErrorID, UpdatableHelpStrings.UpdateDownloadManifestFailure);
		}

		private void OnCabinetDownloadCompleted(object sender, AsyncCompletedEventArgs e)
		{
			this.OnDownloadCompleted(sender, e, UpdatePhase.Downloading, UpdatableHelpStrings.UpdateDownloadCabinetFailureErrorID, UpdatableHelpStrings.UpdateDownloadCabinetFailure);
		}

		private void OnDownloadCompleted(object sender, AsyncCompletedEventArgs e, UpdatePhase phase, LocalizedString errorId, LocalizedString errorMessage)
		{
			if (e.Cancelled)
			{
				this.downloadException = new UpdatableExchangeHelpSystemException(errorId, errorMessage, ErrorCategory.OperationStopped, this, null);
			}
			else if (e.Error != null)
			{
				this.downloadException = new UpdatableExchangeHelpSystemException(errorId, errorMessage, ErrorCategory.ResourceUnavailable, this, e.Error);
			}
			else
			{
				this.downloadException = null;
			}
			AutoResetEvent autoResetEvent = (AutoResetEvent)e.UserState;
			autoResetEvent.Set();
		}

		private string ResolveUri(string baseUri)
		{
			string text = baseUri;
			try
			{
				for (int i = 0; i < 5; i++)
				{
					if (this.helpUpdater.Cmdlet.Abort)
					{
						return text;
					}
					string text2 = text;
					HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(text2);
					httpWebRequest.AllowAutoRedirect = false;
					httpWebRequest.Timeout = 30000;
					HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
					WebHeaderCollection headers = httpWebResponse.Headers;
					try
					{
						if (httpWebResponse.StatusCode == HttpStatusCode.Found || httpWebResponse.StatusCode == HttpStatusCode.Found || httpWebResponse.StatusCode == HttpStatusCode.MovedPermanently || httpWebResponse.StatusCode == HttpStatusCode.MovedPermanently)
						{
							Uri uri = new Uri(headers["Location"], UriKind.RelativeOrAbsolute);
							if (uri.IsAbsoluteUri)
							{
								text = uri.ToString();
							}
							else
							{
								text = text.Replace(httpWebRequest.Address.AbsolutePath, uri.ToString());
							}
							text = text.Trim();
							this.helpUpdater.Cmdlet.WriteVerbose(UpdatableHelpStrings.UpdateRedirectingToHost(text2, text));
						}
						else if (httpWebResponse.StatusCode == HttpStatusCode.OK)
						{
							return text;
						}
					}
					finally
					{
						httpWebResponse.Close();
					}
				}
			}
			catch (UriFormatException ex)
			{
				throw new UpdatableExchangeHelpSystemException(UpdatableHelpStrings.UpdateInvalidHelpInfoUriErrorID, new LocalizedString(ex.Message), ErrorCategory.InvalidData, null, ex);
			}
			throw new UpdatableExchangeHelpSystemException(UpdatableHelpStrings.UpdateTooManyUriRedirectionsErrorID, UpdatableHelpStrings.UpdateTooManyUriRedirections(5), ErrorCategory.InvalidOperation, null, null);
		}

		private const int ThreadWaitMilliseconds = 100;

		private const int ManifestTimeoutMilliseconds = 30000;

		private const int CabinetTimeoutMilliseconds = 120000;

		private const int MaxUrlRedirections = 5;

		private HelpUpdater helpUpdater;

		private UpdatableExchangeHelpSystemException downloadException;
	}
}

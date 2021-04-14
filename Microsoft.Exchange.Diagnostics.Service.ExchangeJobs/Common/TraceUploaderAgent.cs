using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics.Service.Common;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Common
{
	public class TraceUploaderAgent : RetentionAgent
	{
		public TraceUploaderAgent(string enforcedDirectory, TimeSpan retentionPeriod, int maxDirectorySizeMBytes, TimeSpan checkInterval, bool logDataLossMessage) : this(enforcedDirectory, retentionPeriod, maxDirectorySizeMBytes, checkInterval, logDataLossMessage, new HttpClientHandler())
		{
		}

		public TraceUploaderAgent(string enforcedDirectory, TimeSpan retentionPeriod, int maxDirectorySizeMBytes, TimeSpan checkInterval, bool logDataLossMessage, HttpMessageHandler httpMessageHandler) : base(enforcedDirectory, retentionPeriod, maxDirectorySizeMBytes, checkInterval, logDataLossMessage)
		{
			if (checkInterval < TimeSpan.FromHours(1.0))
			{
				throw new ArgumentOutOfRangeException("Agent supports check-intervals of at least 1 hour.", "checkInterval");
			}
			Dictionary<string, string> configStrings = Configuration.GetConfigStrings(TraceUploaderAgent.PropertyPrefix);
			string text;
			string value;
			string text2;
			if (!configStrings.TryGetValue("AzureBlobAccountName", out text) || string.IsNullOrEmpty(text) || !configStrings.TryGetValue("AzureBlobContainer", out value) || string.IsNullOrEmpty(value) || !configStrings.TryGetValue("AzureBlobSharedAccessSignature", out text2) || string.IsNullOrEmpty(text2) || !configStrings.TryGetValue("FilterPattern", out this.traceFilterPattern) || string.IsNullOrEmpty(this.traceFilterPattern))
			{
				throw new ApplicationException("Missing key configuration values.");
			}
			this.container = new TraceUploaderAgent.AzureContainer(text, value, text2, httpMessageHandler);
			this.completionHandle = new AutoResetEvent(false);
			TimeSpan period = TimeSpan.FromMilliseconds(checkInterval.TotalMilliseconds / 2.0);
			this.timer = new Timer(new TimerCallback(this.ProcessEnforcedDirectory), null, TimeSpan.Zero, period);
		}

		public WaitHandle CompletionHandle
		{
			get
			{
				return this.completionHandle;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
			if (disposing)
			{
				if (this.timer != null)
				{
					this.timer.Dispose();
				}
				if (this.container != null)
				{
					this.container.Dispose();
				}
				if (this.completionHandle != null)
				{
					this.completionHandle.Dispose();
				}
			}
		}

		private async void ProcessEnforcedDirectory(object state)
		{
			string[] fileNames = Directory.GetFiles(base.EnforcedDirectory, this.traceFilterPattern, SearchOption.TopDirectoryOnly);
			foreach (string fileName in fileNames)
			{
				bool processed = false;
				try
				{
					using (FileStream file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						await this.container.UploadFileAsync(file);
						processed = true;
					}
				}
				catch (UnauthorizedAccessException)
				{
				}
				catch (Exception ex)
				{
					Logger.LogErrorMessage("Unable to process and upload trace due to:{0}{1}", new object[]
					{
						Environment.NewLine,
						ex
					});
				}
				finally
				{
					if (processed)
					{
						File.Move(fileName, fileName + ".old");
					}
				}
			}
			this.completionHandle.Set();
		}

		public const string AzureBlobAccountNamePropertyName = "AzureBlobAccountName";

		public const string AzureBlobContainerPropertyName = "AzureBlobContainer";

		public const string AzureBlobSharedAccessSignaturePropertyName = "AzureBlobSharedAccessSignature";

		public const string FilterPatternPropertyName = "FilterPattern";

		public static readonly string PropertyPrefix = typeof(TraceUploaderAgent).Name + ".";

		private readonly Timer timer;

		private readonly TraceUploaderAgent.AzureContainer container;

		private readonly string traceFilterPattern;

		private readonly AutoResetEvent completionHandle;

		public class AzureContainer : IDisposable
		{
			public AzureContainer(string accountName, string container, string sharedAccessSignature, HttpMessageHandler httpMessageHandler)
			{
				if (string.IsNullOrEmpty(accountName))
				{
					throw new ArgumentNullException("accountName");
				}
				if (string.IsNullOrEmpty(container))
				{
					throw new ArgumentNullException("targetContainer");
				}
				if (string.IsNullOrEmpty(sharedAccessSignature))
				{
					throw new ArgumentNullException("sharedAccessSignature");
				}
				this.sharedAccessSignature = sharedAccessSignature;
				Uri uri = new Uri(string.Format("https://{0}.blob.core.windows.net", accountName));
				Uri baseAddress = new Uri(uri.OriginalString + "/" + container + "/");
				ServicePoint servicePoint = ServicePointManager.FindServicePoint(uri);
				servicePoint.UseNagleAlgorithm = false;
				servicePoint.Expect100Continue = false;
				servicePoint.ConnectionLimit = 100;
				this.httpClient = new HttpClient(httpMessageHandler, true)
				{
					BaseAddress = baseAddress
				};
				this.httpClient.DefaultRequestHeaders.ExpectContinue = new bool?(false);
			}

			public async Task UploadFileAsync(FileStream file)
			{
				if (file == null)
				{
					throw new ArgumentNullException("file");
				}
				string blobName = Path.GetFileName(file.Name);
				byte[] buffer = new byte[4194304];
				int chunkId = 0;
				List<string> blockIds = new List<string>();
				for (;;)
				{
					int read = await file.ReadAsync(buffer, 0, buffer.Length);
					if (read > 0)
					{
						string blockId = Convert.ToBase64String(BitConverter.GetBytes(chunkId++));
						string blockUrl = this.GetBlobUrl(blobName, new string[]
						{
							"comp",
							"block",
							"blockid",
							Uri.EscapeDataString(blockId)
						});
						using (MemoryStream stream = new MemoryStream(buffer, 0, read))
						{
							using (StreamContent content = new StreamContent(stream))
							{
								using (HttpResponseMessage response = await this.httpClient.PutAsync(blockUrl, content))
								{
									response.EnsureSuccessStatusCode();
									blockIds.Add(blockId);
								}
							}
							continue;
						}
						break;
					}
					break;
				}
				await this.SetBlockListAsync(blobName, blockIds);
			}

			public void Dispose()
			{
				if (!this.disposed)
				{
					if (this.httpClient != null)
					{
						this.httpClient.Dispose();
					}
					this.disposed = true;
				}
				GC.SuppressFinalize(this);
			}

			private string GetBlobUrl(string blobName, params string[] options)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(blobName);
				if (options != null && options.Any<string>() && options.Length % 2 == 0)
				{
					stringBuilder.Append(this.sharedAccessSignature);
					for (int i = 0; i < options.Length; i += 2)
					{
						stringBuilder.AppendFormat("&{0}={1}", options[i], options[i + 1]);
					}
				}
				return stringBuilder.ToString();
			}

			private async Task SetBlockListAsync(string blobName, List<string> blockIds)
			{
				XElement blockList = new XElement("BlockList");
				XDocument blockListDocument = new XDocument(new object[]
				{
					blockList
				});
				blockList.Add(from id in blockIds
				select new XElement("Latest", id));
				string blockListUrl = this.GetBlobUrl(blobName, new string[]
				{
					"comp",
					"blocklist"
				});
				using (StringContent content = new StringContent(blockListDocument.ToString()))
				{
					using (HttpResponseMessage response = await this.httpClient.PutAsync(blockListUrl, content))
					{
						response.EnsureSuccessStatusCode();
					}
				}
			}

			public const int BytesPerBlock = 4194304;

			private readonly HttpClient httpClient;

			private readonly string sharedAccessSignature;

			private bool disposed;
		}
	}
}

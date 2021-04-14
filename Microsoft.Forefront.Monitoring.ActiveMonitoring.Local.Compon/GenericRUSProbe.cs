using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Forefront.Hygiene.Rus.Client;
using Microsoft.Forefront.Hygiene.Rus.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class GenericRUSProbe : ProbeWorkItem
	{
		private ClientId GenericRUSClientId { get; set; }

		private bool IsUploader { get; set; }

		private int MaxMinorUpdate { get; set; }

		private int ProbabilityPercentCount { get; set; }

		private int MinorUpdateSize { get; set; }

		private int MajorUpdateSize { get; set; }

		private bool IsMinorDownloader { get; set; }

		private int GoBehindCurrentMinor { get; set; }

		private bool VerifyUpdate { get; set; }

		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.ReadInputXML();
			if (this.IsUploader)
			{
				if (this.GenerateUpdate(this.ProbabilityPercentCount))
				{
					this.CreateAndUploadNextUpdate().Wait();
					return;
				}
			}
			else
			{
				this.DownloadUpdates().Wait();
			}
		}

		private async Task DownloadUpdates()
		{
			RusClient client = new RusClient();
			Version latestVersion = await client.GetLatestVersionAsync(this.GenericRUSClientId);
			if (latestVersion == null)
			{
				string text = string.Format("SpamDB has no data for client Id - {0}", this.GenericRUSClientId);
				base.Result.Error = text;
				this.TraceError(text);
				throw new ApplicationException(text);
			}
			if (this.IsMinorDownloader)
			{
				int numberOfUpdates;
				Version currentVersion = this.GetCurrentVersion(latestVersion, out numberOfUpdates);
				await this.GetUpdates(client, currentVersion, latestVersion, numberOfUpdates);
			}
			else
			{
				Version currentVersion = new Version(0, 0, 0, 0);
				await this.GetUpdates(client, currentVersion, latestVersion, latestVersion.Minor + 1);
			}
		}

		private async Task GetUpdates(RusClient client, Version currentVersion, Version latestVersion, int numberOfUpdates)
		{
			IDictionary<Version, byte[]> downloadedData = null;
			string errorMsg = string.Empty;
			if (currentVersion == null || latestVersion == null)
			{
				errorMsg = string.Format("Current version or latest version cannot be null. Client Id - {0}", this.GenericRUSClientId);
				base.Result.Error = errorMsg;
				this.TraceError(errorMsg);
				throw new ApplicationException(errorMsg);
			}
			int i = 0;
			while (i < 3)
			{
				downloadedData = await this.DownloadData(client, this.GenericRUSClientId, currentVersion);
				if (numberOfUpdates != 0 && this.VerifyUpdate)
				{
					if (downloadedData != null && downloadedData.Count >= numberOfUpdates)
					{
						if (!this.IsMinorDownloader)
						{
							currentVersion = new Version(latestVersion.Major, 0, 0, 0);
							if (currentVersion == latestVersion && downloadedData.First<KeyValuePair<Version, byte[]>>().Key.Major < currentVersion.Major)
							{
								Thread.Sleep(60000);
								currentVersion = new Version(0, 0, 0, 0);
								goto IL_28C;
							}
						}
						else if (new Version(this.GetNextVersion(currentVersion)) < downloadedData.First<KeyValuePair<Version, byte[]>>().Key)
						{
							return;
						}
						this.VerifyDownloadedUpdates(downloadedData, currentVersion, latestVersion);
						return;
					}
					if (downloadedData != null && downloadedData.Count > 0)
					{
						Version key = downloadedData.First<KeyValuePair<Version, byte[]>>().Key;
						if (key.Major == currentVersion.Major + 1)
						{
							return;
						}
					}
					Thread.Sleep(60000);
					IL_28C:
					i++;
					continue;
				}
				return;
			}
			errorMsg = string.Format("GenericRusProbe Failed to download all the updates in GetUpdates Method - \r\nCurrentVersion {0} \r\nLatestVersion {1} \r\nClientId: {2}", currentVersion.ToString(), latestVersion.ToString(), this.GenericRUSClientId);
			if (downloadedData != null)
			{
				foreach (KeyValuePair<Version, byte[]> keyValuePair in downloadedData)
				{
					errorMsg = errorMsg + "\r\n" + keyValuePair.Key.ToString();
				}
			}
			base.Result.Error = errorMsg;
			this.TraceError(errorMsg);
			throw new ApplicationException(errorMsg);
		}

		private async Task<IDictionary<Version, byte[]>> DownloadData(RusClient client, ClientId clientId, Version currentVersion)
		{
			IEnumerable<ContentInfo> updateVersionInfoList = await client.GetUpdateVersions(clientId, currentVersion);
			Dictionary<Version, byte[]> updates = new Dictionary<Version, byte[]>();
			Version version;
			if (updateVersionInfoList != null && ((List<ContentInfo>)updateVersionInfoList).Count > 0 && (version = ((List<ContentInfo>)updateVersionInfoList)[0].Version) < currentVersion)
			{
				string text = string.Format("Failed to download correct versions for Client ID {0}, CurrentVersion {1} First available download version {2}", clientId, currentVersion, version);
				base.Result.Error = text;
				this.TraceError(text);
				throw new ApplicationException(text);
			}
			foreach (ContentInfo contentInfo in updateVersionInfoList)
			{
				using (IResponseStreamManager responseStreamManager = ResponseStreamManagerFactory.Create(clientId, contentInfo.BlobId, contentInfo.Version, contentInfo.DataSize))
				{
					Task<Stream> task = client.DownloadBlobAsync(clientId, contentInfo.Version, responseStreamManager, null);
					using (Stream result = task.GetAwaiter().GetResult())
					{
						if (result == null)
						{
							return updates;
						}
						byte[] array = new byte[result.Length];
						result.Read(array, 0, array.Count<byte>());
						updates.Add(contentInfo.Version, array);
					}
				}
			}
			return updates;
		}

		private void VerifyDownloadedUpdates(IDictionary<Version, byte[]> downloadedData, Version currentVersion, Version latestVersion)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = string.Empty;
			if (currentVersion < latestVersion && downloadedData.Count == 0)
			{
				text = string.Format("GenericRusProbe Failed to download updates in VerifyDownloadedUpdates #1- \r\nCurrent Version {0}\r\nLatest Version {1}\r\nDownloaded Version : Null \r\nClientId: {2}", currentVersion.ToString(), latestVersion.ToString(), this.GenericRUSClientId);
				base.Result.Error = text;
				this.TraceError(text);
				throw new ApplicationException(text);
			}
			Version version = currentVersion;
			if (this.IsMinorDownloader)
			{
				version = new Version(this.GetNextVersion(currentVersion));
				if (version.Minor == 0)
				{
					version = new Version(this.GetNextVersion(version));
				}
				if (latestVersion.Minor == 0 && latestVersion.Major != 1)
				{
					latestVersion = new Version(latestVersion.Major - 1, this.MaxMinorUpdate, 0, 0);
				}
			}
			Encoding encoding = new ASCIIEncoding();
			foreach (KeyValuePair<Version, byte[]> keyValuePair in downloadedData)
			{
				if (!(keyValuePair.Key == version))
				{
					stringBuilder.Clear();
					stringBuilder.Append(string.Format("GenericRusProbe Failed to download update VerifyDownloadedUpdates #2 Next version mismatch - \r\nFailure Version {0} \r\nClientId: {1} \r\n Current Version - {2}\r\n Latest Version  - {3}\r\n IsMinorDownloader - {4}", new object[]
					{
						version.ToString(),
						this.GenericRUSClientId,
						currentVersion.ToString(),
						latestVersion.ToString(),
						this.IsMinorDownloader.ToString()
					}));
					foreach (KeyValuePair<Version, byte[]> keyValuePair2 in downloadedData)
					{
						stringBuilder.Append("\r\n" + keyValuePair2.Key.ToString());
					}
					text = stringBuilder.ToString();
					base.Result.Error = text;
					this.TraceError(text);
					throw new ApplicationException(text);
				}
				Stream stream = this.GenerateTestData(version.ToString());
				StreamReader streamReader = new StreamReader(stream);
				string a = streamReader.ReadToEnd();
				string @string = encoding.GetString(keyValuePair.Value);
				if (!string.Equals(a, @string))
				{
					text = string.Format("Incorrect Data downloaded - \r\nCurrent Version {0}\r\nClientId: {1}", version.ToString(), this.GenericRUSClientId);
					base.Result.Error = text;
					this.TraceError(text);
					throw new ApplicationException(text);
				}
				if (keyValuePair.Key.Equals(latestVersion))
				{
					this.TraceDebug(string.Format("GenericRusProbe Successfully downloaded the updates - \r\nCurrent Version {0}\r\n Latest Version {1}\r\n ClientId: {2}", currentVersion.ToString(), latestVersion.ToString(), this.GenericRUSClientId));
					return;
				}
				version = new Version(this.GetNextVersion(keyValuePair.Key));
				if (this.IsMinorDownloader && version.Minor == 0)
				{
					version = new Version(this.GetNextVersion(version));
				}
			}
			stringBuilder.Clear();
			stringBuilder.Append(string.Format("GenericRusProbe Failed to download all the updates VerifyDownloadedUpdates #3 - \r\nCurrent Version {0}\r\nLatest Version {1}\r\nFinal downloaded Version {2} \r\nClientId: {3}", new object[]
			{
				currentVersion.ToString(),
				latestVersion.ToString(),
				version.ToString(),
				this.GenericRUSClientId
			}));
			foreach (KeyValuePair<Version, byte[]> keyValuePair3 in downloadedData)
			{
				stringBuilder.Append("\r\n" + keyValuePair3.Key.ToString());
			}
			text = stringBuilder.ToString();
			base.Result.Error = text;
			this.TraceError(text);
			throw new ApplicationException(text);
		}

		private Version GetCurrentVersion(Version latestVersion, out int numberOfUpdates)
		{
			numberOfUpdates = 0;
			Version result;
			if (latestVersion.Minor - this.GoBehindCurrentMinor >= 0)
			{
				int major = latestVersion.Major;
				int minor = latestVersion.Minor - this.GoBehindCurrentMinor;
				result = new Version(major, minor, 0, 0);
				numberOfUpdates = this.GoBehindCurrentMinor;
			}
			else
			{
				int major;
				int minor;
				if (latestVersion.Major == 1)
				{
					major = 1;
					minor = 0;
					numberOfUpdates = latestVersion.Minor;
				}
				else
				{
					major = latestVersion.Major - 1;
					minor = this.MaxMinorUpdate - (this.GoBehindCurrentMinor - latestVersion.Minor);
					numberOfUpdates = this.GoBehindCurrentMinor;
				}
				result = new Version(major, minor, 0, 0);
			}
			return result;
		}

		private async Task CreateAndUploadNextUpdate()
		{
			RusClient client = new RusClient();
			Version currentVersion = await client.GetLatestVersionAsync(this.GenericRUSClientId);
			string nextVersion;
			if (currentVersion == null)
			{
				nextVersion = "1.0.0.0";
			}
			else
			{
				nextVersion = this.GetNextVersion(currentVersion);
			}
			Stream data = this.GenerateTestData(nextVersion);
			Version dataVersion = new Version(nextVersion);
			bool uploadingResult = await client.PublishBlobAsync(this.GenericRUSClientId, data, dataVersion, null, false, 2);
			if (uploadingResult)
			{
				this.TraceDebug(string.Format("GenericRusProbe Successfully published an update - \r\nVersion {0} \r\nClientId: {1}", nextVersion, this.GenericRUSClientId));
				return;
			}
			string text = string.Format("GenericRusProbe Failed to publish an update - \r\nVersion {0} \r\nClientId: {1}", nextVersion, this.GenericRUSClientId);
			base.Result.Error = text;
			this.TraceError(text);
			throw new ApplicationException(text);
		}

		private void ReadInputXML()
		{
			string extensionAttributes = base.Definition.ExtensionAttributes;
			if (string.IsNullOrWhiteSpace(extensionAttributes))
			{
				throw new ArgumentNullException("WorkContext definition Xml is null or empty");
			}
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(extensionAttributes);
			XmlElement xmlElement = Utils.CheckXmlElement(xmlDocument.SelectSingleNode("//WorkContext"), "//WorkContext");
			string attribute = xmlElement.GetAttribute("IsUploader");
			if (string.IsNullOrWhiteSpace(attribute))
			{
				throw new ArgumentNullException(string.Format("WorkContext definition Xml does not have {0} attribute", "IsUploader"));
			}
			this.IsUploader = (attribute == "1");
			string attribute2 = xmlElement.GetAttribute("ClientId");
			if (string.IsNullOrWhiteSpace(attribute2))
			{
				throw new ArgumentNullException(string.Format("WorkContext definition Xml does not have {0} attribute", "ClientId"));
			}
			ClientId genericRUSClientId;
			if (!Enum.TryParse<ClientId>(attribute2, out genericRUSClientId))
			{
				throw new ArgumentException(string.Format("Incorrect ClientId - {0}", attribute2));
			}
			this.GenericRUSClientId = genericRUSClientId;
			XmlElement xmlElement2 = Utils.CheckXmlElement(xmlDocument.SelectSingleNode("//Uploader"), "//Uploader");
			string attribute3 = xmlElement2.GetAttribute("MinorUpdateSizeInBytes");
			if (string.IsNullOrWhiteSpace(attribute3))
			{
				this.MinorUpdateSize = 20480;
			}
			else
			{
				this.MinorUpdateSize = int.Parse(attribute3);
			}
			string attribute4 = xmlElement2.GetAttribute("MajorUpdateSizeInBytes");
			if (string.IsNullOrWhiteSpace(attribute4))
			{
				this.MajorUpdateSize = 10485760;
			}
			else
			{
				this.MajorUpdateSize = int.Parse(attribute4);
			}
			string attribute5 = xmlElement2.GetAttribute("MaxNumberMinorUpdate");
			if (string.IsNullOrWhiteSpace(attribute5))
			{
				this.MaxMinorUpdate = 60;
			}
			else
			{
				this.MaxMinorUpdate = int.Parse(attribute5);
			}
			string attribute6 = xmlElement2.GetAttribute("GenerateUpdateProbabilityInPercent");
			if (string.IsNullOrWhiteSpace(attribute6))
			{
				this.ProbabilityPercentCount = 100;
			}
			else
			{
				this.ProbabilityPercentCount = int.Parse(attribute6);
			}
			XmlElement xmlElement3 = Utils.CheckXmlElement(xmlDocument.SelectSingleNode("//Downloader"), "//Downloader");
			string attribute7 = xmlElement3.GetAttribute("IsMinorDownloader");
			if (string.IsNullOrWhiteSpace(attribute7))
			{
				this.IsMinorDownloader = true;
			}
			else if (int.Parse(attribute7) == 1)
			{
				this.IsMinorDownloader = true;
			}
			else
			{
				this.IsMinorDownloader = false;
			}
			string attribute8 = xmlElement3.GetAttribute("GoBehindCurrentMinor");
			if (string.IsNullOrWhiteSpace(attribute8))
			{
				this.GoBehindCurrentMinor = 2;
			}
			else
			{
				this.GoBehindCurrentMinor = int.Parse(attribute8);
			}
			string attribute9 = xmlElement3.GetAttribute("VerifyUpdate");
			if (string.IsNullOrWhiteSpace(attribute9))
			{
				this.VerifyUpdate = true;
				return;
			}
			this.VerifyUpdate = (int.Parse(attribute9) == 1);
		}

		private string GetNextVersion(Version currentVersion)
		{
			int major = currentVersion.Major;
			int num = currentVersion.Minor;
			if (num < this.MaxMinorUpdate)
			{
				num++;
				return string.Concat(new object[]
				{
					major.ToString(),
					".",
					num,
					".0.0"
				});
			}
			return (major + 1).ToString() + ".0.0.0";
		}

		private bool GenerateUpdate(int percentageCount)
		{
			Random random = new Random();
			int num = random.Next(1, 101);
			return num <= percentageCount;
		}

		private Stream GenerateTestData(string versionNumber)
		{
			Version version = new Version(versionNumber);
			int num;
			if (version.Minor == 0)
			{
				num = this.MajorUpdateSize;
			}
			else
			{
				num = this.MinorUpdateSize;
			}
			StringBuilder stringBuilder = new StringBuilder(num);
			stringBuilder.Append(versionNumber);
			num -= versionNumber.Length;
			short num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (num2 == 26)
				{
					num2 = 0;
				}
				stringBuilder.Append((char)(num2 + 97));
				num2 += 1;
			}
			byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
			return new MemoryStream(bytes);
		}

		private void TraceDebug(string debugMsg)
		{
			ProbeResult result = base.Result;
			result.ExecutionContext = result.ExecutionContext + debugMsg + " ";
			WTFDiagnostics.TraceDebug(ExTraceGlobals.BackgroundTracer, base.TraceContext, debugMsg, null, "TraceDebug", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\GenericRus\\GenericRUSProbe.cs", 731);
		}

		private void TraceError(string errorMsg)
		{
			ProbeResult result = base.Result;
			result.ExecutionContext = result.ExecutionContext + errorMsg + " ";
			WTFDiagnostics.TraceError(ExTraceGlobals.BackgroundTracer, base.TraceContext, errorMsg, null, "TraceError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\GenericRus\\GenericRUSProbe.cs", 741);
		}

		private const string UploaderXMLNodeString = "//Uploader";

		private const string DownloaderXMLNodeString = "//Downloader";

		private const string IsUploaderAttributeString = "IsUploader";

		private const string ClientIdAttributeString = "ClientId";

		private const string MinorUpdateSizeAttributeString = "MinorUpdateSizeInBytes";

		private const string MajorUpdateSizeAttributeString = "MajorUpdateSizeInBytes";

		private const string HighestMinorUpdateString = "MaxNumberMinorUpdate";

		private const string UpdateProbabilityString = "GenerateUpdateProbabilityInPercent";

		private const string IsMinorDownloaderString = "IsMinorDownloader";

		private const string GoBehindCurrentMinorString = "GoBehindCurrentMinor";

		private const string VerifyUpdateString = "VerifyUpdate";

		private const int DefaultMaxMinorUpdate = 60;

		private const int DefaultProbabilityPercentCount = 100;

		private const int DefaultMinorUpdateSize = 20480;

		private const int DefaultMajorUpdateSize = 10485760;

		private const bool DefaultIsMinorDownloader = true;

		private const int DefaultGoBehindMinor = 2;

		private const bool DefaultVerifyUpdate = true;
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Forefront.Hygiene.Rus.EngineUpdateCommon;
using Microsoft.Win32;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class RusEngine
	{
		public RusEngine(string engineName, string platform)
		{
			this.EngineName = engineName;
			this.Platform = platform;
		}

		public static string RusEngineDownloadUrl
		{
			get
			{
				return RusEngine.GetExchangeLabsRegKeyValue("RusEngineDownloadUrl");
			}
		}

		public string EngineName { get; set; }

		public string Platform { get; set; }

		public string ForefrontdlManifestCabUrl
		{
			get
			{
				return Path.Combine(new string[]
				{
					RusEngine.RusEngineDownloadUrl,
					this.Platform,
					this.EngineName,
					"Package",
					this.GetLatestPackageVersionFromForeFrontdl(),
					"Manifest.cab"
				}).Replace("\\", "/");
			}
		}

		public string ManifestTempCabFolder
		{
			get
			{
				return Path.Combine(Path.GetTempPath(), "EngineManifestFromForefrontdl", this.Platform, this.EngineName);
			}
		}

		public DateTime ForefrontdlManifestCreatedTimeInUtc
		{
			get
			{
				string manifestCabFilePath = RusEngine.DownLoadManifestFile(this.ForefrontdlManifestCabUrl, this.ManifestTempCabFolder, "Manifest.cab");
				string text = RusEngine.ExtractManifestCabFileToXml(manifestCabFilePath, "Manifest.xml");
				ManifestFile manifestFile = ManifestManager.OpenManifest(text);
				DateTime dateTime = DateTime.Parse(manifestFile.created);
				TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
				return TimeZoneInfo.ConvertTimeToUtc(dateTime, sourceTimeZone);
			}
		}

		public static bool DownloadFile(string remoteFileURL, string localFile)
		{
			IFileDownloader fileDownloader = FileDownloaderFactory.CreateFileDownloader(0);
			bool result;
			try
			{
				result = (fileDownloader.DownloadFile(remoteFileURL, localFile) == 1);
			}
			catch (EngineDownloadException arg)
			{
				throw new ApplicationException(string.Format("Failed to download file from remote url. DownloadUrl: [{0}] LocalFilePath: [{1}]. Exception: {2}", remoteFileURL, localFile, arg));
			}
			return result;
		}

		public static string DownLoadManifestFile(string remoteManifestUrl, string localManifestFolder, string manifestFileName)
		{
			string text = Path.Combine(localManifestFolder, manifestFileName);
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			if (!Directory.Exists(localManifestFolder))
			{
				Directory.CreateDirectory(localManifestFolder);
			}
			if (!RusEngine.DownloadFile(remoteManifestUrl, text))
			{
				throw new ApplicationException(string.Format("Manifest file download from remote url failed. DownloadUrl: [{0}] DownloadFolderPath: [{1}] FilePath: [{2}]", remoteManifestUrl, localManifestFolder, text));
			}
			return text;
		}

		public static string ExtractManifestCabFileToXml(string manifestCabFilePath, string xmlFileName)
		{
			string directoryName = Path.GetDirectoryName(manifestCabFilePath);
			string text = Path.Combine(directoryName, xmlFileName);
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			if (new ExtractCab(directoryName).ExtractFiles(manifestCabFilePath) != 0)
			{
				throw new ApplicationException(string.Format("File extraction failed. Path: {0}", manifestCabFilePath));
			}
			return text;
		}

		public static string GetExchangeLabsRegKeyValue(string regStringName)
		{
			string result = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeLabs"))
				{
					if (registryKey == null)
					{
						throw new ApplicationException(string.Format("HKLM ExchangeLabs registry key [{0}] is found to be null", "SOFTWARE\\Microsoft\\ExchangeLabs"));
					}
					if (registryKey.GetValue(regStringName) == null)
					{
						throw new ApplicationException(string.Format("RegistryKey.GetValue() returned null for the reg string [{0}]", regStringName));
					}
					result = (registryKey.GetValue(regStringName) as string);
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException(string.Format("An error occured while loading registry key value [{0}]. Exception: {1}", regStringName, ex.ToString()));
			}
			return result;
		}

		public DateTime GetEngineFilesDownloadedTimeFromQds(string rusPrimaryFileShareRootPath, string rusAlternateFileShareRootPath, bool isLatest = true)
		{
			Dictionary<string, DateTime> dictionary = null;
			Dictionary<string, DateTime> dictionary2 = null;
			string text = string.Empty;
			string text2 = string.Empty;
			DateTime result;
			try
			{
				if (!string.IsNullOrEmpty(rusPrimaryFileShareRootPath))
				{
					text = Path.Combine(new string[]
					{
						rusPrimaryFileShareRootPath,
						"EngineFileShare",
						this.Platform,
						this.EngineName,
						"DownloadedTime.txt"
					});
					dictionary = this.GetDownloadedDates(text);
				}
				if (!string.IsNullOrEmpty(rusAlternateFileShareRootPath))
				{
					text2 = Path.Combine(new string[]
					{
						rusAlternateFileShareRootPath,
						"EngineFileShare",
						this.Platform,
						this.EngineName,
						"DownloadedTime.txt"
					});
					dictionary2 = this.GetDownloadedDates(text2);
				}
				if (dictionary == null && dictionary2 == null)
				{
					throw new ApplicationException(string.Format("Both Primary and Alternate QDS share engine files DownloadedTime files are not reachable. Primary share path: {0}. Alternate share path: {1}.", text, text2));
				}
				string key = isLatest ? "LatestDownloadedTime" : "PreviousDownloadedTime";
				if (dictionary2 == null)
				{
					result = dictionary[key];
				}
				else if (dictionary == null)
				{
					result = dictionary2[key];
				}
				else
				{
					result = ((dictionary[key] > dictionary2[key]) ? dictionary[key] : dictionary2[key]);
				}
			}
			catch (KeyNotFoundException ex)
			{
				string message = string.Format("Expected {0} or {1} strings not found in {2} file. Error: {3}", new object[]
				{
					"LatestDownloadedTime",
					"PreviousDownloadedTime",
					"DownloadedTime.txt",
					ex.ToString()
				});
				throw new ApplicationException(message);
			}
			return result;
		}

		private Dictionary<string, DateTime> GetDownloadedDates(string qdsShareEngineFilesDownloadedTextFilePath)
		{
			if (!File.Exists(qdsShareEngineFilesDownloadedTextFilePath))
			{
				return null;
			}
			Dictionary<string, DateTime> dictionary = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
			string text = IniFileHelper.IniReadValue(qdsShareEngineFilesDownloadedTextFilePath, "SignaturePollingDownloadTimes", "LatestDownloadedTime");
			DateTime value;
			if (!DateTime.TryParse(text, out value))
			{
				throw new ApplicationException(string.Format("Failed to convert [{0}: {1}] string value to dateTime", "LatestDownloadedTime", text));
			}
			dictionary.Add("LatestDownloadedTime", value);
			string text2 = IniFileHelper.IniReadValue(qdsShareEngineFilesDownloadedTextFilePath, "SignaturePollingDownloadTimes", "PreviousDownloadedTime");
			DateTime value2;
			if (DateTime.TryParse(text2, out value2))
			{
				dictionary.Add("PreviousDownloadedTime", value2);
				return dictionary;
			}
			throw new ApplicationException(string.Format("Failed to convert [{0}: {1}] string value to dateTime", "PreviousDownloadedTime", text2));
		}

		private string GetLatestPackageVersionFromForeFrontdl()
		{
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = Path.Combine(Path.GetTempPath(), "UMPackageFromForefrontdl", this.EngineName);
			try
			{
				text2 = RusPublishingPipelineBase.ForeFrontdlUniversalManifestCabUrl;
				string manifestCabFilePath = RusEngine.DownLoadManifestFile(text2, text3, "UniversalManifest.cab");
				string text4 = RusEngine.ExtractManifestCabFileToXml(manifestCabFilePath, "UniversalManifest.xml");
				UniversalManifest universalManifest = UniversalManifestManager.OpenManifest(text4);
				EnginePackagingInfoEngineName engineName;
				if (!Enum.TryParse<EnginePackagingInfoEngineName>(this.EngineName, true, out engineName))
				{
					throw new ArgumentException("Engine Name is invalid.");
				}
				EnginePackagingInfoPlatform platform;
				if (!Enum.TryParse<EnginePackagingInfoPlatform>(this.Platform, true, out platform))
				{
					throw new ArgumentException("Platform is invalid.");
				}
				EnginePackagingInfo enginePackagingInfo = new EnginePackagingInfo
				{
					category = "Antivirus",
					engineName = engineName,
					platform = platform
				};
				text = UniversalManifestManager.GetEngineInfo(universalManifest, enginePackagingInfo).Package[0].version;
			}
			catch (Exception ex)
			{
				throw new ApplicationException(string.Format("An error occured while fetching package version from UM. Url: {0}, LocalFolder: {1}. Error: {2}", text2, text3, ex.ToString()));
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				throw new ApplicationException("Package version cannot be null or empty");
			}
			return text;
		}

		private const string TempEngineManifestFolderName = "EngineManifestFromForefrontdl";

		private const string EngineFilesFolderName = "EngineFileShare";

		private const string PackageFolderName = "Package";

		private const string EngineFilesDownloadedFileName = "DownloadedTime.txt";

		private const string LatestDownloadedTimeKey = "LatestDownloadedTime";

		private const string PreviousDownloadedTimeKey = "PreviousDownloadedTime";

		private const string DownloadedTimeTextFileHeader = "SignaturePollingDownloadTimes";

		private const string UMPackageVersionFolderFromForefrontdl = "UMPackageFromForefrontdl";

		private const string AntivirusNodeInUMXml = "Antivirus";

		private const string ExchangeLabsRegKey = "SOFTWARE\\Microsoft\\ExchangeLabs";

		private const string RusEngineDownloadUrlRegistryKeyName = "RusEngineDownloadUrl";
	}
}

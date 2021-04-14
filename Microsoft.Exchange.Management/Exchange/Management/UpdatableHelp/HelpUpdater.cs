using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.UpdatableHelp
{
	internal class HelpUpdater
	{
		internal HelpUpdater(UpdatableExchangeHelpCommand cmdlet)
		{
			this.localCabinetFilename = null;
			this.CurrentHelpVersion = null;
			this.Cmdlet = cmdlet;
			this.ProgressNumerator = 0.0;
		}

		internal string PowerShellPsmamlSchemaFilePath { get; private set; }

		internal double ProgressNumerator { get; set; }

		internal string ModuleBase { get; private set; }

		internal string LocalTempBase { get; private set; }

		internal string ManifestUrl { get; private set; }

		internal UpdatableHelpVersion CurrentHelpVersion { get; private set; }

		internal int CurrentHelpRevision { get; private set; }

		internal int ThrottlingPeriodHours { get; private set; }

		internal DateTime LastSuccessfulCheckTimestampValue { get; private set; }

		internal UpdatableExchangeHelpCommand Cmdlet { get; private set; }

		internal string ExchangeVersion { get; private set; }

		internal string LocalManifestPath
		{
			get
			{
				return this.LocalTempBase + "UpdateHelp.$$$\\ExchangeHelpInfo.xml";
			}
		}

		internal string LocalCabinetPath
		{
			get
			{
				string str = this.LocalTempBase + "UpdateHelp.$$$\\";
				if (string.IsNullOrEmpty(this.localCabinetFilename))
				{
					string str2 = string.Empty;
					do
					{
						str2 = Path.GetRandomFileName() + ".cab";
					}
					while (File.Exists(str + str2));
					this.localCabinetFilename = str2;
				}
				return str + this.localCabinetFilename;
			}
		}

		internal string LocalCabinetExtractionTargetPath
		{
			get
			{
				return this.LocalTempBase + "UpdateHelp.$$$\\Files\\";
			}
		}

		internal void UpdateProgress(UpdatePhase phase, LocalizedString subTask, int numerator, int denominator)
		{
			UpdatableExchangeHelpProgressEventArgs e = new UpdatableExchangeHelpProgressEventArgs(phase, subTask, numerator, denominator);
			this.Cmdlet.HandleProgressChanged(null, e);
		}

		internal void LoadConfiguration()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup"))
			{
				this.ModuleBase = null;
				if (registryKey != null)
				{
					string text = (string)registryKey.GetValue("MsiInstallpath", string.Empty);
					if (string.IsNullOrEmpty(text))
					{
						throw new UpdatableExchangeHelpSystemException(UpdatableHelpStrings.UpdateInstallationNotFoundErrorID, UpdatableHelpStrings.UpdateInstallationNotFound, ErrorCategory.MetadataError, null, null);
					}
					this.ModuleBase = text;
					if (!this.ModuleBase.EndsWith("\\"))
					{
						this.ModuleBase += "\\";
					}
					this.ModuleBase = text + "bin\\";
					this.Cmdlet.WriteVerbose(UpdatableHelpStrings.UpdateInstallationFound(this.ModuleBase));
					int num = (int)registryKey.GetValue("MsiProductMajor");
					int num2 = (int)registryKey.GetValue("MsiProductMinor");
					int num3 = (int)registryKey.GetValue("MsiBuildMajor");
					int num4 = (int)registryKey.GetValue("MsiBuildMinor");
					this.ExchangeVersion = string.Format("{0}.{1}.{2}.{3}", new object[]
					{
						num,
						num2,
						num3,
						num4
					});
					this.CurrentHelpVersion = new UpdatableHelpVersion(this.ExchangeVersion);
					this.CurrentHelpRevision = 0;
				}
				this.LocalTempBase = this.ModuleBase;
			}
			this.PowerShellPsmamlSchemaFilePath = null;
			RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey(string.Format("SOFTWARE\\Microsoft\\PowerShell\\{0}\\PowerShellEngine", 3));
			if (registryKey2 == null)
			{
				registryKey2 = Registry.LocalMachine.OpenSubKey(string.Format("SOFTWARE\\Microsoft\\PowerShell\\{0}\\PowerShellEngine", 1));
			}
			if (registryKey2 != null)
			{
				try
				{
					string text2 = registryKey2.GetValue("ApplicationBase", null).ToString();
					if (!string.IsNullOrEmpty(text2))
					{
						this.PowerShellPsmamlSchemaFilePath = text2 + "\\Schemas\\PSMaml\\maml.xsd";
					}
				}
				finally
				{
					registryKey2.Dispose();
				}
			}
			this.ManifestUrl = string.Empty;
			this.ThrottlingPeriodHours = 24;
			this.LastSuccessfulCheckTimestampValue = new DateTime(1980, 1, 1).ToUniversalTime();
			RegistryKey registryKey3 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UpdateExchangeHelp");
			if (registryKey3 == null)
			{
				registryKey3 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UpdateExchangeHelp");
			}
			if (registryKey3 != null)
			{
				try
				{
					this.ManifestUrl = registryKey3.GetValue("ManifestUrl", "http://go.microsoft.com/fwlink/p/?LinkId=287244").ToString();
					if (string.IsNullOrEmpty(this.ManifestUrl))
					{
						throw new UpdatableExchangeHelpSystemException(UpdatableHelpStrings.UpdateRegkeyNotFoundErrorID, UpdatableHelpStrings.UpdateRegkeyNotFound("SOFTWARE\\Microsoft\\ExchangeServer\\v15", "\\UpdateExchangeHelp", "ManifestUrl"), ErrorCategory.MetadataError, null, null);
					}
					int num5 = (int)registryKey3.GetValue("CurrentHelpRevision", 0);
					if (num5 > 0)
					{
						this.CurrentHelpVersion = new UpdatableHelpVersion(this.ExchangeVersion);
						this.CurrentHelpRevision = num5;
					}
					int throttlingPeriodHours;
					if (int.TryParse(registryKey3.GetValue("ThrottlingPeriodHours", this.ThrottlingPeriodHours).ToString(), out throttlingPeriodHours))
					{
						this.ThrottlingPeriodHours = throttlingPeriodHours;
					}
					DateTime lastSuccessfulCheckTimestampValue;
					if (DateTime.TryParse(registryKey3.GetValue("LastSuccessfulCheckTimestamp", this.LastSuccessfulCheckTimestampValue.ToString()).ToString(), out lastSuccessfulCheckTimestampValue))
					{
						this.LastSuccessfulCheckTimestampValue = lastSuccessfulCheckTimestampValue;
					}
					this.Cmdlet.WriteVerbose(UpdatableHelpStrings.UpdateManifestUrl(this.ManifestUrl));
					this.Cmdlet.WriteVerbose(UpdatableHelpStrings.UpdateCurrentHelpVersion(this.CurrentHelpVersion.NormalizedVersionNumberWithRevision(this.CurrentHelpRevision)));
					return;
				}
				finally
				{
					registryKey3.Close();
					registryKey3.Dispose();
				}
			}
			throw new UpdatableExchangeHelpSystemException(UpdatableHelpStrings.UpdateConfigRegKeyNotFoundErrorID, UpdatableHelpStrings.UpdateConfigRegKeyNotFound("SOFTWARE\\Microsoft\\ExchangeServer\\v15", "\\UpdateExchangeHelp"), ErrorCategory.MetadataError, null, null);
		}

		internal UpdatableExchangeHelpSystemException UpdateHelp()
		{
			double num = 90.0;
			UpdatableExchangeHelpSystemException result = null;
			this.ProgressNumerator = 0.0;
			if (!this.Cmdlet.Force)
			{
				if (!this.DownloadThrottleExpired())
				{
					this.Cmdlet.WriteVerbose(UpdatableHelpStrings.UpdateUseForceToUpdateHelp(this.ThrottlingPeriodHours));
					return result;
				}
			}
			try
			{
				this.UpdateProgress(UpdatePhase.Checking, LocalizedString.Empty, (int)this.ProgressNumerator, 100);
				string path = this.LocalTempBase + "UpdateHelp.$$$\\";
				this.CleanDirectory(path);
				this.EnsureDirectory(path);
				HelpDownloader helpDownloader = new HelpDownloader(this);
				helpDownloader.DownloadManifest();
				if (!this.Cmdlet.Abort)
				{
					UpdatableHelpVersionRange updatableHelpVersionRange = helpDownloader.SearchManifestForApplicableUpdates(this.CurrentHelpVersion, this.CurrentHelpRevision);
					if (updatableHelpVersionRange != null)
					{
						double num2 = 20.0;
						this.ProgressNumerator = 10.0;
						this.UpdateProgress(UpdatePhase.Downloading, LocalizedString.Empty, (int)this.ProgressNumerator, 100);
						string[] array = this.EnumerateAffectedCultures(updatableHelpVersionRange.CulturesAffected);
						if (array.Length > 0)
						{
							this.Cmdlet.WriteVerbose(UpdatableHelpStrings.UpdateApplyingRevision(updatableHelpVersionRange.HelpRevision, string.Join(", ", array)));
							helpDownloader.DownloadPackage(updatableHelpVersionRange.CabinetUrl);
							if (this.Cmdlet.Abort)
							{
								return result;
							}
							this.ProgressNumerator += num2;
							this.UpdateProgress(UpdatePhase.Extracting, LocalizedString.Empty, (int)this.ProgressNumerator, 100);
							HelpInstaller helpInstaller = new HelpInstaller(this, array, num);
							helpInstaller.ExtractToTemp();
							if (this.Cmdlet.Abort)
							{
								return result;
							}
							this.ProgressNumerator += num2;
							this.UpdateProgress(UpdatePhase.Validating, LocalizedString.Empty, (int)this.ProgressNumerator, 100);
							Dictionary<string, LocalizedString> dictionary = helpInstaller.ValidateFiles();
							if (this.Cmdlet.Abort)
							{
								return result;
							}
							if (dictionary != null && dictionary.Count > 0)
							{
								this.Cmdlet.WriteVerbose(UpdatableHelpStrings.UpdateInvalidHelpFiles);
								foreach (KeyValuePair<string, LocalizedString> keyValuePair in dictionary)
								{
									this.Cmdlet.WriteVerbose(UpdatableHelpStrings.UpdateInvalidFileDescription(keyValuePair.Key, keyValuePair.Value));
								}
								throw new UpdatableExchangeHelpSystemException(UpdatableHelpStrings.UpdateContentXmlValidationFailureErrorID, UpdatableHelpStrings.UpdateContentXmlValidationFailure, ErrorCategory.NotInstalled, null, null);
							}
							this.ProgressNumerator += num2;
							this.UpdateProgress(UpdatePhase.Installing, LocalizedString.Empty, (int)this.ProgressNumerator, 100);
							if (!helpInstaller.AtomicInstallFiles())
							{
								throw new UpdatableExchangeHelpSystemException(UpdatableHelpStrings.UpdateInstallFilesExceptionErrorID, UpdatableHelpStrings.UpdateInstallFilesException, ErrorCategory.NotInstalled, null, null);
							}
						}
						else
						{
							this.Cmdlet.WriteVerbose(UpdatableHelpStrings.UpdateSkipRevision(updatableHelpVersionRange.HelpRevision));
						}
						this.UpdateCurrentVersionInRegistry(updatableHelpVersionRange.HelpRevision);
						this.Cmdlet.WriteVerbose(UpdatableHelpStrings.UpdateRevisionApplied(updatableHelpVersionRange.HelpRevision));
						this.ProgressNumerator += num2;
					}
					else
					{
						this.Cmdlet.WriteVerbose(UpdatableHelpStrings.UpdateNoApplicableUpdates);
					}
					this.ProgressNumerator = num;
					this.UpdateProgress(UpdatePhase.Finalizing, LocalizedString.Empty, (int)this.ProgressNumerator, 100);
					try
					{
						this.CleanDirectory(path);
						if (Directory.Exists(path))
						{
							Directory.Delete(path);
						}
					}
					catch
					{
					}
					this.UpdateLastSuccessfulCheckTimestamp(DateTime.UtcNow);
				}
			}
			catch (Exception ex)
			{
				if (ex.GetType() == typeof(UpdatableExchangeHelpSystemException))
				{
					result = (UpdatableExchangeHelpSystemException)ex;
				}
				else
				{
					result = new UpdatableExchangeHelpSystemException(UpdatableHelpStrings.UpdateInstallFilesExceptionErrorID, UpdatableHelpStrings.UpdateInstallFilesException, ErrorCategory.InvalidOperation, null, ex);
				}
			}
			this.ProgressNumerator = 100.0;
			this.UpdateProgress(UpdatePhase.Finalizing, LocalizedString.Empty, (int)this.ProgressNumerator, 100);
			return result;
		}

		internal void EnsureDirectory(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		internal void CleanDirectory(string path)
		{
			if (Directory.Exists(path))
			{
				this.RecursiveDescent(0, path, string.Empty, null, true, null);
			}
		}

		internal void RecursiveDescent(int recursionLevel, string path, string relativePath, string[] topLevelFilter, bool delete, Dictionary<string, List<string>> files)
		{
			if (recursionLevel >= 10)
			{
				throw new UpdatableExchangeHelpSystemException(UpdatableHelpStrings.UpdateTooManySubdirectoryLevelsErrorID, UpdatableHelpStrings.UpdateTooManySubdirectoryLevels, ErrorCategory.InvalidData, null, null);
			}
			string text = (path + relativePath).ToLower();
			string[] directories = Directory.GetDirectories(text);
			foreach (string path2 in directories)
			{
				string fileName = Path.GetFileName(path2);
				if (recursionLevel != 0 || topLevelFilter == null || topLevelFilter.Length <= 0 || topLevelFilter.Contains(fileName.ToLower()))
				{
					this.RecursiveDescent(recursionLevel + 1, path, relativePath + fileName + "\\", topLevelFilter, delete, files);
				}
			}
			string[] files2 = Directory.GetFiles(text);
			foreach (string path3 in files2)
			{
				string fileName2 = Path.GetFileName(path3);
				if (files != null)
				{
					if (!files.ContainsKey(relativePath))
					{
						files.Add(relativePath, new List<string>());
					}
					files[relativePath].Add(fileName2.ToLower());
				}
				if (delete)
				{
					string path4 = text + fileName2;
					FileAttributes fileAttributes = File.GetAttributes(path4);
					if (fileAttributes.HasFlag(FileAttributes.ReadOnly))
					{
						fileAttributes &= ~FileAttributes.ReadOnly;
						File.SetAttributes(path4, fileAttributes);
					}
					File.Delete(path4);
				}
			}
			if (delete && recursionLevel > 0)
			{
				Directory.Delete(text);
			}
		}

		internal bool DownloadThrottleExpired()
		{
			return DateTime.UtcNow > this.LastSuccessfulCheckTimestampValue.AddHours((double)this.ThrottlingPeriodHours);
		}

		internal string[] EnumerateAffectedCultures(string[] culturesUpdated)
		{
			List<string> list = new List<string>();
			foreach (string text in culturesUpdated)
			{
				if (Directory.Exists(this.ModuleBase + text))
				{
					list.Add(text);
				}
			}
			return list.ToArray();
		}

		internal void UpdateCurrentVersionInRegistry(int newRevision)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UpdateExchangeHelp", true))
			{
				registryKey.SetValue("CurrentHelpRevision", newRevision, RegistryValueKind.DWord);
			}
		}

		internal void UpdateLastSuccessfulCheckTimestamp(DateTime timestampUtc)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UpdateExchangeHelp", true))
			{
				registryKey.SetValue("LastSuccessfulCheckTimestamp", timestampUtc.ToString(), RegistryValueKind.String);
			}
		}

		private const int ExpectedPowerShellMajorVersion = 3;

		private const int MinimumPowerShellMajorVersion = 1;

		private const int MaxRecursionLevel = 10;

		private const string ManifestName = "ExchangeHelpInfo.xml";

		private const string ConfigPowerShellRegistryKey = "SOFTWARE\\Microsoft\\PowerShell\\{0}\\PowerShellEngine";

		private const string ConfigPowerShellApplicationBaseValueName = "ApplicationBase";

		private const string ConfigRegistryKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15";

		private const string ConfigSetupRegistrySubkey = "\\Setup";

		private const string ConfigUpdaterRegistrySubkey = "\\UpdateExchangeHelp";

		private const string ConfigMsiInstallPathValueName = "MsiInstallpath";

		private const string ConfigMsiBuildMajorVersionKey = "MsiBuildMajor";

		private const string ConfigMsiBuildMinorVersionKey = "MsiBuildMinor";

		private const string ConfigMsiProductMajorVersionKey = "MsiProductMajor";

		private const string ConfigMsiProductMinorVersionKey = "MsiProductMinor";

		private const string ConfigManifestUrlValueName = "ManifestUrl";

		private const string DefaultManifestLink = "http://go.microsoft.com/fwlink/p/?LinkId=287244";

		private const string ConfigCurrentHelpRevisionValueName = "CurrentHelpRevision";

		private const string ConfigThrottlingPeriodHoursValueName = "ThrottlingPeriodHours";

		private const string ConfigLastSuccessfulCheckTimestampValueName = "LastSuccessfulCheckTimestamp";

		private const string LocalTempDirectoryName = "UpdateHelp.$$$\\";

		private const string ExtractionDirectoryName = "Files\\";

		private const string CabinetExtension = ".cab";

		private string localCabinetFilename;
	}
}

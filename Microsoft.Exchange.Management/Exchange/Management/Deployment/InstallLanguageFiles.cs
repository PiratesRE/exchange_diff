using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.CabUtility;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Install", "LanguageFiles", SupportsShouldProcess = true)]
	public sealed class InstallLanguageFiles : Task
	{
		public InstallLanguageFiles()
		{
			this.monadConnection = new MonadConnection("pooled=false");
			this.monadConnection.Open();
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath LogFilePath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["LogFilePath"];
			}
			set
			{
				base.Fields["LogFilePath"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public LongPath LangPackPath
		{
			get
			{
				return (LongPath)base.Fields["LangPackPath"];
			}
			set
			{
				base.Fields["LangPackPath"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public NonRootLocalLongFullPath InstallPath
		{
			get
			{
				return (NonRootLocalLongFullPath)base.Fields["InstallPath"];
			}
			set
			{
				base.Fields["InstallPath"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public bool SourceIsBundle
		{
			get
			{
				return (bool)base.Fields["SourceIsBundle"];
			}
			set
			{
				base.Fields["SourceIsBundle"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string[] LanguagePacksToInstall
		{
			get
			{
				return (string[])base.Fields["LanguagePacksToInstall"];
			}
			set
			{
				base.Fields["LanguagePacksToInstall"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string[] LPClientFlags
		{
			get
			{
				return (string[])base.Fields["LPClientFlags"];
			}
			set
			{
				base.Fields["LPClientFlags"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string[] LPServerFlags
		{
			get
			{
				return (string[])base.Fields["LPServerFlags"];
			}
			set
			{
				base.Fields["LPServerFlags"] = value;
			}
		}

		public LocalizedString Description
		{
			get
			{
				return Strings.LanguagePackDescription;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.monadConnection != null)
			{
				this.monadConnection.Close();
			}
			base.Dispose(disposing);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.datetime = ((DateTime)ExDateTime.Now).ToString("yyyyMMdd-HHmmss");
			try
			{
				string value = "LOGVERBOSE=1 TARGETDIR=\"" + this.InstallPath + "\"";
				Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
				Dictionary<string, bool> dictionary2 = new Dictionary<string, bool>();
				int num = 0;
				for (int i = 0; i < this.LanguagePacksToInstall.Length; i++)
				{
					bool flag = Convert.ToBoolean(this.LPServerFlags[i]);
					dictionary.Add(this.LanguagePacksToInstall[i], flag);
					if (flag)
					{
						num++;
					}
					flag = Convert.ToBoolean(this.LPClientFlags[i]);
					dictionary2.Add(this.LanguagePacksToInstall[i], flag);
					if (flag)
					{
						num++;
					}
				}
				int num2 = num * 2;
				if (this.SourceIsBundle)
				{
					num2 += this.LanguagePacksToInstall.Length;
				}
				int num3 = 0;
				foreach (string text in this.LanguagePacksToInstall)
				{
					int lcid = CultureInfo.GetCultureInfo(text).LCID;
					string text2 = "";
					if (this.SourceIsBundle)
					{
						TaskLogger.Log(Strings.ExtractingLang(text));
						base.WriteProgress(this.Description, Strings.ExtractingLang(text), (int)((double)num3 * 100.0 / (double)num2 + 0.5));
						DirectoryInfo directoryInfo = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(this.LangPackPath.PathName)));
						string text3 = text + "\\";
						EmbeddedCabWrapper.ExtractFiles(this.LangPackPath.PathName, directoryInfo.FullName, text3);
						text2 = directoryInfo.FullName + '\\' + text3;
						num3++;
					}
					else
					{
						text2 = Path.Combine(this.LangPackPath.PathName, text);
					}
					string text4 = Path.Combine(text2, "ClientLanguagePack.msi");
					if (dictionary2[text] && File.Exists(text4))
					{
						TaskLogger.Log(Strings.UninstallOldMSIFor(text, "client"));
						base.WriteProgress(this.Description, Strings.UninstallOldMSIFor(text, "client"), (int)((double)num3 * 100.0 / (double)num2 + 0.5));
						using (MonadCommand monadCommand = new MonadCommand("Uninstall-MsiPackage", this.monadConnection))
						{
							string value2 = this.CreateMsilogPathname("Uninstall", "Client", text);
							monadCommand.Parameters.AddWithValue("LogFile", value2);
							Guid productCode = MsiUtility.GetProductCode(text4);
							monadCommand.Parameters.AddWithValue("ProductCode", productCode);
							monadCommand.Execute();
						}
						num3++;
						TaskLogger.Log(Strings.InstallingMSIFor(text, "client"));
						base.WriteProgress(this.Description, Strings.InstallingMSIFor(text, "client"), (int)((double)num3 * 100.0 / (double)num2 + 0.5));
						using (MonadCommand monadCommand2 = new MonadCommand("Install-MsiPackage", this.monadConnection))
						{
							monadCommand2.Parameters.AddWithValue("PackagePath", text4);
							string value3 = this.CreateMsilogPathname("Install", "Client", text);
							monadCommand2.Parameters.AddWithValue("LogFile", value3);
							monadCommand2.Parameters.AddWithValue("Features", new string[]
							{
								"AdminTools",
								"Mailbox",
								"ClientAccess",
								"Gateway",
								"Bridgehead",
								"UnifiedMessaging",
								"ClientLanguagePack"
							});
							monadCommand2.Parameters.AddWithValue("PropertyValues", value);
							monadCommand2.Execute();
						}
						num3++;
					}
					text4 = Path.Combine(text2, "ServerLanguagePack.msi");
					if (dictionary[text] && File.Exists(text4))
					{
						TaskLogger.Log(Strings.UninstallOldMSIFor(text, "server"));
						base.WriteProgress(this.Description, Strings.UninstallOldMSIFor(text, "server"), (int)((double)num3 * 100.0 / (double)num2 + 0.5));
						using (MonadCommand monadCommand3 = new MonadCommand("Uninstall-MsiPackage", this.monadConnection))
						{
							string value4 = this.CreateMsilogPathname("Uninstall", "Server", text);
							monadCommand3.Parameters.AddWithValue("LogFile", value4);
							Guid productCode2 = MsiUtility.GetProductCode(text4);
							monadCommand3.Parameters.AddWithValue("ProductCode", productCode2);
							monadCommand3.Execute();
						}
						num3++;
						TaskLogger.Log(Strings.InstallingMSIFor(text, "server"));
						base.WriteProgress(this.Description, Strings.InstallingMSIFor(text, "server"), (int)((double)num3 * 100.0 / (double)num2 + 0.5));
						using (MonadCommand monadCommand4 = new MonadCommand("Install-MsiPackage", this.monadConnection))
						{
							monadCommand4.Parameters.AddWithValue("PackagePath", text4);
							string value5 = this.CreateMsilogPathname("Install", "Server", text);
							monadCommand4.Parameters.AddWithValue("LogFile", value5);
							monadCommand4.Parameters.AddWithValue("Features", new string[]
							{
								"AdminTools",
								"Mailbox",
								"ClientAccess",
								"Gateway",
								"Bridgehead",
								"UnifiedMessaging",
								"ServerLanguagePack"
							});
							monadCommand4.Parameters.AddWithValue("PropertyValues", value);
							monadCommand4.Execute();
						}
						num3++;
					}
					if (this.SourceIsBundle)
					{
						try
						{
							Directory.Delete(text2, true);
						}
						catch (IOException e)
						{
							TaskLogger.LogError(e);
						}
					}
				}
			}
			finally
			{
				base.WriteProgress(this.Description, Strings.ProgressStatusCompleted, 100);
				TaskLogger.LogExit();
			}
		}

		private string CreateMsilogPathname(string action, string lpType, string language)
		{
			return string.Concat(new object[]
			{
				Path.Combine(this.LogFilePath.PathName, action),
				'.',
				language,
				'.',
				lpType,
				".",
				this.datetime,
				".msilog"
			});
		}

		private const string ClientMSI = "ClientLanguagePack.msi";

		private const string ServerMSI = "ServerLanguagePack.msi";

		private MonadConnection monadConnection;

		private string datetime = "0";
	}
}

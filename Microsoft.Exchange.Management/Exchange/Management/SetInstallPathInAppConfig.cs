using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management
{
	[LocDescription(Strings.IDs.SetInstallPathInAppConfig)]
	[Cmdlet("Set", "InstallPathInAppConfig")]
	public class SetInstallPathInAppConfig : Task
	{
		public SetInstallPathInAppConfig()
		{
			TaskLogger.LogEnter();
			this.ExchangeInstallPath = ConfigurationContext.Setup.InstallPath;
			this.ReplacementString = "%ExchangeInstallDir%";
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			bool flag = base.Fields.Contains("ConfigFileAbsolutePath") && !string.IsNullOrEmpty(this.ConfigFileAbsolutePath);
			bool flag2 = base.Fields.Contains("ConfigFileRelativePath") && !string.IsNullOrEmpty(this.ConfigFileRelativePath);
			if (!flag && !flag2)
			{
				base.WriteError(new ArgumentException(Strings.ErrorMustSpecifyEitherAbsoluteOrRelativePath), ErrorCategory.InvalidArgument, string.Empty);
			}
			if (flag)
			{
				this.ConfigFileFullPath = this.ConfigFileAbsolutePath;
			}
			else
			{
				this.ConfigFileFullPath = Path.Combine(this.ExchangeInstallPath, this.ConfigFileRelativePath);
			}
			string text = Path.Combine(this.ConfigFileFullPath, this.ConfigFileName);
			string text2 = text + ".bak";
			TaskLogger.Trace("ExchangeInstallPath = {0}, Replacement String = {1}, File Path = {2}", new object[]
			{
				this.ExchangeInstallPath,
				this.ReplacementString,
				text
			});
			File.Copy(text, text2, true);
			File.Delete(text);
			using (StreamReader streamReader = new StreamReader(text2, true))
			{
				streamReader.Peek();
				using (StreamWriter streamWriter = new StreamWriter(text, false, streamReader.CurrentEncoding))
				{
					string text3;
					while ((text3 = streamReader.ReadLine()) != null)
					{
						text3 = text3.Replace(this.ReplacementString, this.ExchangeInstallPath);
						streamWriter.WriteLine(text3);
					}
					streamWriter.Flush();
					streamWriter.Close();
				}
				streamReader.Close();
			}
			TaskLogger.LogExit();
		}

		[Parameter(Mandatory = false)]
		public string ExchangeInstallPath
		{
			get
			{
				return (string)base.Fields["ExchangeInstallPath"];
			}
			set
			{
				base.Fields["ExchangeInstallPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ReplacementString
		{
			get
			{
				return (string)base.Fields["ReplacementString"];
			}
			set
			{
				base.Fields["ReplacementString"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ConfigFileAbsolutePath
		{
			get
			{
				return (string)base.Fields["ConfigFileAbsolutePath"];
			}
			set
			{
				base.Fields["ConfigFileAbsolutePath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ConfigFileRelativePath
		{
			get
			{
				return (string)base.Fields["ConfigFileRelativePath"];
			}
			set
			{
				base.Fields["ConfigFileRelativePath"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string ConfigFileName
		{
			get
			{
				return (string)base.Fields["ConfigFileName"];
			}
			set
			{
				base.Fields["ConfigFileName"] = value;
			}
		}

		private AssemblyName FindAssemblyName(string assemblyName)
		{
			string[] array = new string[]
			{
				this.ExchangeInstallPath + "bin",
				this.ExchangeInstallPath + "Common"
			};
			foreach (string path in array)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(path);
				FileInfo[] files = directoryInfo.GetFiles(assemblyName + ".*");
				FileInfo[] array3 = files;
				int j = 0;
				while (j < array3.Length)
				{
					FileInfo fileInfo = array3[j];
					if (fileInfo.Name.EndsWith(".dll"))
					{
						goto IL_95;
					}
					if (fileInfo.Name.EndsWith(".exe"))
					{
						goto Block_2;
					}
					IL_F5:
					j++;
					continue;
					Block_2:
					try
					{
						IL_95:
						TaskLogger.Trace("Found qualify Assembly {0}", new object[]
						{
							fileInfo.FullName
						});
						return AssemblyName.GetAssemblyName(fileInfo.FullName);
					}
					catch (IOException ex)
					{
						TaskLogger.Trace("Fault reflecting assembly {0}, returning null.  Error: ", new object[]
						{
							fileInfo.FullName,
							ex.Message
						});
						return null;
					}
					goto IL_F5;
				}
			}
			TaskLogger.Trace("QualifyAssembly {0} Not Found, returning null.", new object[]
			{
				assemblyName
			});
			return null;
		}

		private static string PublicKeyTokenFromFullName(string fullName)
		{
			string text = "PublicKeyToken=";
			return fullName.Substring(fullName.IndexOf(text) + text.Length);
		}

		private const string ConfigFileAbsolutePathKey = "ConfigFileAbsolutePath";

		private const string ConfigFileRelativePathKey = "ConfigFileRelativePath";

		private string ConfigFileFullPath;
	}
}

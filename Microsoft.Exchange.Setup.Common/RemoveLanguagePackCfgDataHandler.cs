using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RemoveLanguagePackCfgDataHandler : ConfigurationDataHandler
	{
		public RemoveLanguagePackCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, "", RemoveLanguagePackCfgDataHandler.commandText, connection)
		{
			base.InstallableUnitName = "LanguagePacks";
			LocalLongFullPath.TryParse(ConfigurationContext.Setup.SetupLoggingPath, out this.logFilePath);
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			SetupLogger.Log(Strings.RemoveLanguagePacksLogFilePath((this.LogFilePath == null) ? "" : this.LogFilePath.PathName));
			base.Parameters.AddWithValue("LogFilePath", this.LogFilePath);
			base.Parameters.AddWithValue("InstallMode", base.SetupContext.InstallationMode);
			if (base.SetupContext.InstallationMode != InstallationModes.Uninstall)
			{
				base.Parameters.AddWithValue("LanguagePacksToRemove", this.GetPackageGuidsToRemove());
			}
			SetupLogger.TraceExit();
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
		}

		public LocalLongFullPath LogFilePath
		{
			get
			{
				return this.logFilePath;
			}
		}

		private string[] GetPackageGuidsToRemove()
		{
			List<string> list = new List<string>();
			Regex regex = new Regex("{DEDFFB[0-9a-fA-F]{2}-42EC-4E26-[0-9a-fA-F]{4}-430E86DF378C}");
			Regex regex2 = new Regex("{521E60[0-9a-fA-F]{2}-B4B1-4CBC-[0-9a-fA-F]{4}-25AD697801FA}");
			foreach (string text in base.SetupContext.InstalledLanguagePacks)
			{
				bool flag = regex.IsMatch(text);
				bool flag2 = regex2.IsMatch(text);
				if (flag || flag2)
				{
					int culture = int.Parse(text.Substring(20, 4), NumberStyles.HexNumber);
					CultureInfo cultureInfo = CultureInfo.GetCultureInfo(culture);
					while (cultureInfo != CultureInfo.InvariantCulture)
					{
						Array array;
						if (base.SetupContext.LanguagePacksToInstall.TryGetValue(cultureInfo.Name, out array))
						{
							bool[] array2 = (bool[])array;
							if ((flag && array2[0]) || (flag2 && array2[1]))
							{
								list.Add(text);
								break;
							}
							break;
						}
						else
						{
							cultureInfo = cultureInfo.Parent;
						}
					}
				}
			}
			return list.ToArray();
		}

		private static readonly string commandText = "remove-InstalledLanguages";

		private LocalLongFullPath logFilePath;
	}
}

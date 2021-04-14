using System;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Setup.Common;

namespace Microsoft.Exchange.Setup.GUI
{
	internal static class LicenseAgreementFactory
	{
		public static string GetLicenseFileFullPathName(InstallationModes mode)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			string result = null;
			text2 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Eula");
			if (Directory.Exists(text2))
			{
				text = LicenseAgreementFactory.GetLicenseFileName(mode);
				try
				{
					result = LocalizedResources.GetFile(text2, text);
				}
				catch (FileNotFoundException)
				{
					SetupLogger.Log(Strings.SetupNotFoundInSourceDirError(Path.Combine(text2, text)));
				}
			}
			return result;
		}

		private static string GetLicenseFileName(InstallationModes mode)
		{
			string result = string.Empty;
			switch (mode)
			{
			case InstallationModes.Install:
				result = "License.rtf";
				break;
			case InstallationModes.BuildToBuildUpgrade:
				result = "UpgradeLicense.rtf";
				break;
			}
			return result;
		}

		private const string LicenseFileName = "License.rtf";

		private const string UpgradeLicenseFileName = "UpgradeLicense.rtf";
	}
}

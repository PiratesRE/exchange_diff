using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.CabUtility;
using Microsoft.Exchange.Setup.SignatureVerification;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	public static class MspUtility
	{
		public static int UnpackMspCabs(string mspFilePath, string toPath)
		{
			if (!MsiHelper.IsMspFileExtension(mspFilePath))
			{
				throw new MsiException(Strings.WrongFileType("mspFilePath"));
			}
			List<string> list = new List<string>();
			using (MsiDatabase msiDatabase = new MsiDatabase(mspFilePath))
			{
				try
				{
					list = msiDatabase.ExtractCabs(toPath);
				}
				catch (MsiException)
				{
					return 0;
				}
			}
			foreach (string path in list)
			{
				EmbeddedCabWrapper.ExtractCabFiles(Path.Combine(toPath, path), toPath, string.Empty, false);
			}
			return list.Count;
		}

		public static bool IsMspInterimUpdate(string mspFilePath)
		{
			if (!MsiHelper.IsMspFileExtension(mspFilePath))
			{
				throw new MsiException(Strings.WrongFileType("mspFilePath"));
			}
			bool result;
			try
			{
				using (MsiDatabase msiDatabase = new MsiDatabase(mspFilePath))
				{
					string text = msiDatabase.QueryProperty("MsiPatchMetadata", "DisplayName");
					result = text.StartsWith("Interim Update", StringComparison.CurrentCultureIgnoreCase);
				}
			}
			catch (MsiException)
			{
				result = false;
			}
			return result;
		}

		public static List<string> GetApplicableMsps(string msiFilePath, bool sort, params string[] mspFiles)
		{
			List<string> result;
			try
			{
				result = MsiHelper.DetermineApplicableMsps(msiFilePath, sort, mspFiles);
			}
			catch (MsiException)
			{
				result = null;
			}
			return result;
		}

		public static bool VerifyMspSignature(string mspFilePath)
		{
			if (!MsiHelper.IsMspFileExtension(mspFilePath))
			{
				throw new MsiException(Strings.WrongFileType("mspFilePath"));
			}
			try
			{
				string location = Assembly.GetExecutingAssembly().Location;
				SignVerfWrapper signVerfWrapper = new SignVerfWrapper();
				bool flag = signVerfWrapper.VerifyEmbeddedSignature(location, false);
				if (flag)
				{
					return signVerfWrapper.IsFileMicrosoftTrusted(mspFilePath, true);
				}
			}
			catch (SignatureVerificationException)
			{
				return false;
			}
			return true;
		}

		public static bool IsMspCompatibleWithLanguagPack(string mspFilePath, string lpFilePath, string pathToLocalXML, string pathToLangPackBundleXML)
		{
			if (!MsiHelper.IsMspFileExtension(mspFilePath))
			{
				throw new MsiException(Strings.WrongFileType("mspFilePath"));
			}
			ValidationHelper.ThrowIfFileNotExist(lpFilePath, "lpFilePath");
			ValidationHelper.ThrowIfFileNotExist(pathToLocalXML, "pathToLocalXML");
			ValidationHelper.ThrowIfFileNotExist(pathToLangPackBundleXML, "pathToLangPackBundleXML");
			Version mspVersion = MspUtility.GetMspVersion(mspFilePath);
			Version lpVersion = new Version(FileVersionInfo.GetVersionInfo(lpFilePath).FileVersion);
			LanguagePackVersion languagePackVersion = new LanguagePackVersion(pathToLocalXML, pathToLangPackBundleXML);
			return languagePackVersion.IsExchangeInApplicableRange(lpVersion, mspVersion);
		}

		public static Version GetMspVersion(string mspFilePath)
		{
			if (!MsiHelper.IsMspFileExtension(mspFilePath))
			{
				throw new MsiException(Strings.WrongFileType("mspFilePath"));
			}
			Version result;
			using (MsiDatabase msiDatabase = new MsiDatabase(mspFilePath))
			{
				try
				{
					string text = msiDatabase.QueryProperty("MsiPatchMetadata", "DisplayName");
					int num = text.LastIndexOf(" ");
					if (num == -1)
					{
						result = null;
					}
					else
					{
						string version = text.Substring(num).Trim();
						result = new Version(version);
					}
				}
				catch (MsiException)
				{
					result = null;
				}
				catch (ArgumentException)
				{
					result = null;
				}
			}
			return result;
		}

		private const string MsiPatchMetadataTable = "MsiPatchMetadata";

		private const string DisplayNameProperty = "DisplayName";

		private const string InterimUpdate = "Interim Update";

		private const string SpaceSeparator = " ";
	}
}

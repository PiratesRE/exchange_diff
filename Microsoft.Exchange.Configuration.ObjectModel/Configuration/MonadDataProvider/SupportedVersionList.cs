using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	[Serializable]
	public class SupportedVersionList
	{
		static SupportedVersionList()
		{
			string name = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\AdminTools";
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name))
				{
					if (registryKey != null)
					{
						bool.TryParse(registryKey.GetValue("EMC.SkipVersionCheck") as string, out SupportedVersionList.skipVersionCheck);
					}
				}
			}
			catch (SecurityException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
		}

		public SupportedVersionList(string versionList)
		{
			string[] array = versionList.Split(new char[]
			{
				';'
			});
			foreach (string text in array)
			{
				string text2 = text.Trim();
				if (!string.IsNullOrEmpty(text2))
				{
					this.supportedVersionList.Add(ExchangeBuild.Parse(text2));
				}
			}
		}

		public int Count
		{
			get
			{
				return this.supportedVersionList.Count;
			}
		}

		public ExchangeBuild this[int pos]
		{
			get
			{
				return this.supportedVersionList[pos];
			}
		}

		public bool IsSupported(string version)
		{
			if (SupportedVersionList.skipVersionCheck)
			{
				return false;
			}
			ExchangeBuild build = ExchangeBuild.Parse(version);
			return (from c in this.supportedVersionList
			where c.Major == build.Major && c.Minor == build.Minor && c.Build == build.Build
			select c).Count<ExchangeBuild>() > 0;
		}

		public string GetLatestVersion()
		{
			if (this.supportedVersionList.Count <= 0)
			{
				return string.Empty;
			}
			return this.supportedVersionList.Max<ExchangeBuild>().ToString();
		}

		public static SupportedVersionList Parse(string list)
		{
			SupportedVersionList result = null;
			try
			{
				result = new SupportedVersionList((list != null) ? list.ToString() : string.Empty);
			}
			catch (ArgumentException ex)
			{
				throw new SupportedVersionListFormatException(new LocalizedString(ex.Message));
			}
			return result;
		}

		public static string DefaultVersionString = ConfigurationContext.Setup.GetExecutingVersion().ToString();

		private List<ExchangeBuild> supportedVersionList = new List<ExchangeBuild>();

		private static bool skipVersionCheck = false;
	}
}

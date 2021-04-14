using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Microsoft.Exchange.Data.Internal
{
	internal class DefaultApplicationServices : IApplicationServices
	{
		public static Stream CreateTemporaryStorage(Func<int, byte[]> acquireBuffer, Action<byte[]> releaseBuffer)
		{
			TemporaryDataStorage temporaryDataStorage = new TemporaryDataStorage(acquireBuffer, releaseBuffer);
			Stream result = temporaryDataStorage.OpenWriteStream(false);
			temporaryDataStorage.Release();
			return result;
		}

		public Stream CreateTemporaryStorage()
		{
			return DefaultApplicationServices.CreateTemporaryStorage(null, null);
		}

		public IList<CtsConfigurationSetting> GetConfiguration(string subSectionName)
		{
			Dictionary<string, IList<CtsConfigurationSetting>> dictionary = this.GetConfigurationSubSections();
			IList<CtsConfigurationSetting> result;
			if (!dictionary.TryGetValue(subSectionName ?? string.Empty, out result))
			{
				return this.emptySubSection;
			}
			return result;
		}

		public void LogConfigurationErrorEvent()
		{
		}

		public void RefreshConfiguration()
		{
			ConfigurationManager.RefreshSection("appSettings");
			ConfigurationManager.RefreshSection("CTS");
			lock (this.lockObject)
			{
				this.configurationSubSections = null;
			}
		}

		private Dictionary<string, IList<CtsConfigurationSetting>> GetConfigurationSubSections()
		{
			Dictionary<string, IList<CtsConfigurationSetting>> dictionary = this.configurationSubSections;
			if (dictionary == null)
			{
				CtsConfigurationSection ctsConfigurationSection = null;
				try
				{
					ctsConfigurationSection = (ConfigurationManager.GetSection("CTS") as CtsConfigurationSection);
				}
				catch (ConfigurationErrorsException)
				{
					this.LogConfigurationErrorEvent();
				}
				CtsConfigurationSetting ctsConfigurationSetting = null;
				try
				{
					string value = ConfigurationManager.AppSettings["TemporaryStoragePath"];
					if (!string.IsNullOrEmpty(value))
					{
						ctsConfigurationSetting = new CtsConfigurationSetting("TemporaryStorage");
						ctsConfigurationSetting.AddArgument("Path", value);
					}
				}
				catch (ConfigurationErrorsException)
				{
					this.LogConfigurationErrorEvent();
				}
				lock (this.lockObject)
				{
					dictionary = this.configurationSubSections;
					if (dictionary == null)
					{
						if (ctsConfigurationSection != null)
						{
							dictionary = ctsConfigurationSection.SubSectionsDictionary;
						}
						else
						{
							dictionary = new Dictionary<string, IList<CtsConfigurationSetting>>();
							dictionary.Add(string.Empty, new List<CtsConfigurationSetting>());
						}
						if (ctsConfigurationSetting != null)
						{
							IList<CtsConfigurationSetting> list = dictionary[string.Empty];
							bool flag2 = false;
							foreach (CtsConfigurationSetting ctsConfigurationSetting2 in list)
							{
								if (string.Equals(ctsConfigurationSetting2.Name, ctsConfigurationSetting.Name))
								{
									flag2 = true;
									break;
								}
							}
							if (!flag2)
							{
								list.Add(ctsConfigurationSetting);
							}
						}
						this.configurationSubSections = dictionary;
					}
				}
			}
			return dictionary;
		}

		private IList<CtsConfigurationSetting> emptySubSection = new List<CtsConfigurationSetting>();

		private object lockObject = new object();

		private volatile Dictionary<string, IList<CtsConfigurationSetting>> configurationSubSections;
	}
}

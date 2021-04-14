using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Common.Net.Cryptography
{
	public class CryptoAlgorithm
	{
		public CryptoAlgorithm(int id, string name)
		{
			this.Id = id;
			this.Name = name;
			this.settings = new Dictionary<string, string>();
		}

		public static string PreferredKeyedHashAlgorithm { get; set; }

		public static string PreferredHashAlgorithm { get; set; }

		public static string PreferredSymmetricAlgorithm { get; set; }

		public int Id { get; private set; }

		public string Name { get; private set; }

		public void AddOrUpdateAlgorithmSetting(string settingName, string settingValue)
		{
			this.settings[settingName] = settingValue;
		}

		private Dictionary<string, string> settings;
	}
}

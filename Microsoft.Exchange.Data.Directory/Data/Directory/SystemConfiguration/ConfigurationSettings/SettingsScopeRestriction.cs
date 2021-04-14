using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[Serializable]
	public class SettingsScopeRestriction : XMLSerializableBase
	{
		public SettingsScopeRestriction()
		{
		}

		public SettingsScopeRestriction(string nameMatch, string minVersion, string maxVersion) : this(null, nameMatch, minVersion, maxVersion)
		{
		}

		public SettingsScopeRestriction(Guid guidMatch) : this(null, guidMatch.ToString(), null, null)
		{
		}

		public SettingsScopeRestriction(string subType, string nameMatch) : this(subType, nameMatch, null, null)
		{
		}

		private SettingsScopeRestriction(string subType, string nameMatch, string minVersion, string maxVersion)
		{
			this.SubType = subType;
			this.NameMatch = nameMatch;
			this.MinVersion = minVersion;
			this.MaxVersion = maxVersion;
		}

		[XmlAttribute(AttributeName = "SbT")]
		public string SubType { get; set; }

		[XmlAttribute(AttributeName = "NmM")]
		public string NameMatch { get; set; }

		[XmlAttribute(AttributeName = "MinV")]
		public string MinVersion
		{
			get
			{
				return this.minVersion;
			}
			set
			{
				this.minVersion = value;
				this.minServerVersion = null;
				this.minExchangeVersion = null;
			}
		}

		[XmlAttribute(AttributeName = "MaxV")]
		public string MaxVersion
		{
			get
			{
				return this.maxVersion;
			}
			set
			{
				this.maxVersion = value;
				this.maxServerVersion = null;
				this.maxExchangeVersion = null;
			}
		}

		internal Guid? GuidMatch
		{
			get
			{
				Guid guid;
				if (Guid.TryParse(this.NameMatch, out guid) && guid != Guid.Empty)
				{
					return new Guid?(guid);
				}
				return null;
			}
		}

		internal ServerVersion MinServerVersion
		{
			get
			{
				if (this.minServerVersion == null && this.minVersion != null)
				{
					this.minServerVersion = SettingsScopeRestriction.ParseServerVersion(this.minVersion);
				}
				return this.minServerVersion;
			}
		}

		internal ServerVersion MaxServerVersion
		{
			get
			{
				if (this.maxServerVersion == null && this.maxVersion != null)
				{
					this.maxServerVersion = SettingsScopeRestriction.ParseServerVersion(this.maxVersion);
				}
				return this.maxServerVersion;
			}
		}

		internal ExchangeObjectVersion MinExchangeVersion
		{
			get
			{
				if (this.minExchangeVersion == null && this.minVersion != null)
				{
					this.minExchangeVersion = SettingsScopeRestriction.ParseExchangeVersion(this.minVersion);
				}
				return this.minExchangeVersion;
			}
		}

		internal ExchangeObjectVersion MaxExchangeVersion
		{
			get
			{
				if (this.maxExchangeVersion == null && this.maxVersion != null)
				{
					this.maxExchangeVersion = SettingsScopeRestriction.ParseExchangeVersion(this.maxVersion);
				}
				return this.maxExchangeVersion;
			}
		}

		public static void ValidateNameMatch(string expression)
		{
			SettingsScopeRestriction.CreateRegex(expression);
		}

		public static void ValidateAsServerVersion(string version)
		{
			SettingsScopeRestriction.ValidateAsServerVersion(SettingsScopeRestriction.ParseServerVersion(version));
		}

		public static void ValidateAsServerVersion(int version)
		{
			SettingsScopeRestriction.ValidateAsServerVersion(new ServerVersion(version));
		}

		public static void ValidateAsServerVersion(ServerVersion serverVersion)
		{
			if (serverVersion.Major < 15)
			{
				throw new ConfigurationSettingsUnsupportedVersionException(serverVersion.ToString());
			}
		}

		public static void ValidateAsExchangeVersion(string version)
		{
			SettingsScopeRestriction.ValidateAsExchangeVersion(SettingsScopeRestriction.ParseExchangeVersion(version));
		}

		public static void ValidateAsExchangeVersion(ExchangeObjectVersion exchangeVersion)
		{
			if (exchangeVersion.ExchangeBuild < ExchangeObjectVersion.Exchange2012.ExchangeBuild)
			{
				throw new ConfigurationSettingsUnsupportedVersionException(exchangeVersion.ToString());
			}
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.SubType))
			{
				return string.Format("{0}={1}", this.SubType, this.NameMatch);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Match={0}", this.NameMatch);
			if (!string.IsNullOrEmpty(this.MinVersion))
			{
				stringBuilder.AppendFormat(",MinVer={0}", this.MinVersion);
			}
			if (!string.IsNullOrEmpty(this.MaxVersion))
			{
				stringBuilder.AppendFormat(",MaxVer={0}", this.MaxVersion);
			}
			return stringBuilder.ToString();
		}

		private static Regex CreateRegex(string expression)
		{
			Regex result;
			try
			{
				result = new Regex(expression, RegexOptions.IgnoreCase);
			}
			catch (ArgumentException innerException)
			{
				throw new ConfigurationSettingsInvalidMatchException(expression, innerException);
			}
			return result;
		}

		private static ExchangeObjectVersion ParseExchangeVersion(string version)
		{
			ExchangeObjectVersion result;
			if (!SettingsScopeRestriction.TryParseExchangeVersion(version, out result))
			{
				throw new ConfigurationSettingsUnsupportedVersionException(version);
			}
			return result;
		}

		private static bool TryParseExchangeVersion(string version, out ExchangeObjectVersion exchangeVersion)
		{
			exchangeVersion = null;
			bool result;
			try
			{
				exchangeVersion = ExchangeObjectVersion.Parse(version);
				result = true;
			}
			catch (ArgumentException)
			{
				result = false;
			}
			return result;
		}

		private static ServerVersion ParseServerVersion(string version)
		{
			ServerVersion result;
			if (!SettingsScopeRestriction.TryParseServerVersion(version, out result))
			{
				throw new ConfigurationSettingsUnsupportedVersionException(version);
			}
			return result;
		}

		private static bool TryParseServerVersion(string version, out ServerVersion serverVersion)
		{
			serverVersion = null;
			if (string.IsNullOrEmpty(version))
			{
				return false;
			}
			int versionNumber;
			if (int.TryParse(version, out versionNumber))
			{
				serverVersion = new ServerVersion(versionNumber);
				return true;
			}
			return ServerVersion.TryParseFromSerialNumber(version, out serverVersion);
		}

		private string minVersion;

		private string maxVersion;

		private ExchangeObjectVersion minExchangeVersion;

		private ExchangeObjectVersion maxExchangeVersion;

		private ServerVersion minServerVersion;

		private ServerVersion maxServerVersion;
	}
}

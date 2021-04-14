using System;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Xml.Serialization;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[XmlRoot("S")]
	[Serializable]
	public sealed class SettingOverrideXml : XMLSerializableBase
	{
		private static Version GetVersion()
		{
			FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(typeof(VariantConfiguration).Assembly.Location);
			return Version.Parse(versionInfo.ProductVersion);
		}

		public SettingOverrideXml()
		{
			this.Parameters = new MultiValuedProperty<string>();
		}

		[XmlAttribute("CN")]
		public string ComponentName { get; set; }

		[XmlAttribute("SN")]
		public string SectionName { get; set; }

		[XmlAttribute("FN")]
		public string FlightName { get; set; }

		[XmlAttribute("MB")]
		public string ModifiedBy { get; set; }

		[XmlAttribute("R")]
		public string Reason { get; set; }

		[XmlIgnore]
		public Version MinVersion
		{
			get
			{
				return this.minVersion;
			}
			set
			{
				this.minVersion = this.Normalize(value);
			}
		}

		[XmlIgnore]
		public Version MaxVersion
		{
			get
			{
				return this.maxVersion;
			}
			set
			{
				this.maxVersion = this.Normalize(value);
			}
		}

		[XmlIgnore]
		public Version FixVersion
		{
			get
			{
				return this.fixVersion;
			}
			set
			{
				this.fixVersion = this.Normalize(value);
			}
		}

		[XmlArray("Ss")]
		[XmlArrayItem("S", typeof(string))]
		public string[] Server { get; set; }

		[XmlArray("Ps")]
		[XmlArrayItem("P", typeof(string))]
		public MultiValuedProperty<string> Parameters { get; set; }

		[XmlAttribute("LV")]
		public string MinVersionString
		{
			get
			{
				if (!(this.MinVersion != null))
				{
					return null;
				}
				return this.MinVersion.ToString();
			}
			set
			{
				this.MinVersion = (string.IsNullOrEmpty(value) ? null : Version.Parse(value));
			}
		}

		[XmlAttribute("HV")]
		public string MaxVersionString
		{
			get
			{
				if (!(this.MaxVersion != null))
				{
					return null;
				}
				return this.MaxVersion.ToString();
			}
			set
			{
				this.MaxVersion = (string.IsNullOrEmpty(value) ? null : Version.Parse(value));
			}
		}

		[XmlAttribute("FV")]
		public string FixVersionString
		{
			get
			{
				if (!(this.FixVersion != null))
				{
					return null;
				}
				return this.FixVersion.ToString();
			}
			set
			{
				this.FixVersion = (string.IsNullOrEmpty(value) ? null : Version.Parse(value));
			}
		}

		[XmlIgnore]
		internal bool Applies
		{
			get
			{
				if (this.MinVersion != null && SettingOverrideXml.CurrentVersion < this.MinVersion)
				{
					return false;
				}
				if (this.MaxVersion != null && SettingOverrideXml.CurrentVersion > this.MaxVersion)
				{
					return false;
				}
				if (this.FixVersion != null && SettingOverrideXml.CurrentVersion >= this.FixVersion)
				{
					return false;
				}
				if (this.Server != null && this.Server.Length > 0)
				{
					if (!this.Server.Any((string wildcard) => new WildcardPattern(wildcard, WildcardOptions.IgnoreCase).IsMatch(Environment.MachineName)))
					{
						return false;
					}
				}
				return true;
			}
		}

		private Version Normalize(Version version)
		{
			if (version == null)
			{
				return null;
			}
			return new Version(version.Major, version.Minor, (version.Build < 0) ? 0 : version.Build, (version.Revision < 0) ? 0 : version.Revision);
		}

		internal static readonly Version CurrentVersion = SettingOverrideXml.GetVersion();

		private Version minVersion;

		private Version maxVersion;

		private Version fixVersion;
	}
}

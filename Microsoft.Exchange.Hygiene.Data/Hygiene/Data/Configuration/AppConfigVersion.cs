using System;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Exchange.Hygiene.Data.Configuration
{
	internal struct AppConfigVersion : IEquatable<AppConfigVersion>
	{
		public AppConfigVersion(long version)
		{
			if (version < 0L)
			{
				throw new ArgumentOutOfRangeException("version");
			}
			this.version = (ulong)version;
		}

		public int Major
		{
			get
			{
				return (int)((this.version & 18446462598732840960UL) >> 48);
			}
			set
			{
				if (value < 0 || value > 32767)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.version = ((281474976710655UL & this.version) | (ulong)((ulong)((long)value) << 48));
			}
		}

		public int Minor
		{
			get
			{
				return (int)((this.version & 281470681743360UL) >> 32);
			}
			set
			{
				if (value < 0 || value > 65535)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.version = ((18446462603027808255UL & this.version) | (ulong)((ulong)((long)value) << 32));
			}
		}

		public int Build
		{
			get
			{
				return (int)((this.version & (ulong)-65536) >> 16);
			}
			set
			{
				if (value < 0 || value > 65535)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.version = ((18446744069414649855UL & this.version) | (ulong)((ulong)((long)value) << 16));
			}
		}

		public int Revision
		{
			get
			{
				return (int)(this.version & 65535UL);
			}
			set
			{
				if (value < 0 || value > 65535)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.version = ((18446744073709486080UL & this.version) | (ulong)value);
			}
		}

		public static AppConfigVersion GetCallingAssemblyVersion()
		{
			Assembly callingAssembly = Assembly.GetCallingAssembly();
			AssemblyFileVersionAttribute assemblyFileVersionAttribute = (AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(callingAssembly, typeof(AssemblyFileVersionAttribute));
			Version version = (assemblyFileVersionAttribute != null) ? Version.Parse(assemblyFileVersionAttribute.Version) : callingAssembly.GetName().Version;
			return new AppConfigVersion
			{
				Major = version.Major,
				Minor = version.Minor,
				Build = version.Build,
				Revision = version.Revision
			};
		}

		public static bool TryParse(string version, out AppConfigVersion configVersion)
		{
			if (string.IsNullOrWhiteSpace(version))
			{
				configVersion = default(AppConfigVersion);
				return false;
			}
			long num;
			if (long.TryParse(version, out num) && num >= 0L)
			{
				configVersion = new AppConfigVersion(num);
				return true;
			}
			string[] array = version.Split(new char[]
			{
				'.'
			});
			ushort major;
			ushort minor;
			ushort build;
			ushort revision;
			if (array.Length == 4 && ushort.TryParse(array[0], out major) && ushort.TryParse(array[1], out minor) && ushort.TryParse(array[2], out build) && ushort.TryParse(array[3], out revision))
			{
				configVersion = default(AppConfigVersion);
				configVersion.Major = (int)major;
				configVersion.Minor = (int)minor;
				configVersion.Build = (int)build;
				configVersion.Revision = (int)revision;
				return true;
			}
			configVersion = default(AppConfigVersion);
			return false;
		}

		public override string ToString()
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			string text = this.Major.ToString(invariantCulture);
			string text2 = this.Minor.ToString(invariantCulture);
			string text3 = this.Build.ToString(invariantCulture);
			string text4 = this.Revision.ToString(invariantCulture);
			return string.Concat(new string[]
			{
				text,
				".",
				text2,
				".",
				text3,
				".",
				text4
			});
		}

		public string ToString(bool formatAsBuildVersion)
		{
			if (formatAsBuildVersion)
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				string text = this.Major.ToString("D2", invariantCulture);
				string text2 = this.Minor.ToString("D2", invariantCulture);
				string text3 = this.Build.ToString("D4", invariantCulture);
				string text4 = this.Revision.ToString("D3", invariantCulture);
				return string.Concat(new string[]
				{
					text,
					".",
					text2,
					".",
					text3,
					".",
					text4
				});
			}
			return this.ToString();
		}

		public override bool Equals(object obj)
		{
			return obj is AppConfigVersion && this.version == ((AppConfigVersion)obj).version;
		}

		public bool Equals(AppConfigVersion obj)
		{
			return this.version == obj.version;
		}

		public override int GetHashCode()
		{
			return this.version.GetHashCode();
		}

		public long ToInt64()
		{
			return (long)this.version;
		}

		private ulong version;
	}
}

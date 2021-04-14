using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class BuildVersion : ConfigurablePropertyBag
	{
		public ulong DALBuildVersion
		{
			get
			{
				return (ulong)this[BuildVersion.DALBuildVersionProp];
			}
		}

		public ulong DBBuildVersion
		{
			get
			{
				return (ulong)this[BuildVersion.DBBuildVersionProp];
			}
		}

		public override ObjectId Identity
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public static string GetBuildVersion(ulong version)
		{
			return string.Format("{0}.{1}.{2}.{3}", new object[]
			{
				(version & 18446462598732840960UL) >> 48,
				(version & 281470681743360UL) >> 32,
				(version & (ulong)-65536) >> 16,
				version & 65535UL
			});
		}

		public static ulong GetBuildVersion(string version)
		{
			string[] array = version.Split(new char[]
			{
				'.'
			});
			if (array != null && array.Length == 4)
			{
				if (!array.Any(delegate(string item)
				{
					ushort num5;
					return !ushort.TryParse(item, out num5);
				}))
				{
					ulong num = (ulong)ushort.Parse(array[0]);
					ulong num2 = (ulong)ushort.Parse(array[1]);
					ulong num3 = (ulong)ushort.Parse(array[2]);
					ulong num4 = (ulong)ushort.Parse(array[3]);
					return num << 48 | num2 << 32 | num3 << 16 | num4;
				}
			}
			throw new ArgumentException("Invalid build version format");
		}

		public static Version GetBuildVersionObject(ulong version)
		{
			return Version.Parse(BuildVersion.GetBuildVersion(version));
		}

		public static Version GetBuildVersionObject(int major, int minor, int build, int revision)
		{
			Version result = null;
			if (major != -1 && minor != -1)
			{
				if (build != -1 && revision != -1)
				{
					result = new Version(major, minor, build, revision);
				}
				else if (build != -1)
				{
					result = new Version(major, minor, build);
				}
				else
				{
					result = new Version(major, minor);
				}
			}
			return result;
		}

		public static ulong GetBuildVersion(Version version)
		{
			if (version == null)
			{
				throw new ArgumentNullException("version");
			}
			ulong num = (ulong)((ushort)version.Major);
			ulong num2 = (ulong)((ushort)version.Minor);
			ulong num3 = (ulong)((ushort)((version.Build != -1) ? version.Build : 0));
			ulong num4 = (ulong)((ushort)((version.Revision != -1) ? version.Revision : 0));
			return num << 48 | num2 << 32 | num3 << 16 | num4;
		}

		public static readonly HygienePropertyDefinition DALBuildVersionProp = new HygienePropertyDefinition("DALBuildVersion", typeof(ulong), 0UL, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition DBBuildVersionProp = new HygienePropertyDefinition("DBBuildVersion", typeof(ulong), 0UL, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}

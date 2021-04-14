using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public struct ComponentVersion : IComparable<ComponentVersion>
	{
		public static ComponentVersion Zero
		{
			get
			{
				return ComponentVersion.zero;
			}
		}

		public static bool TryParse(string value, out ComponentVersion result)
		{
			string[] array = value.Split(new char[]
			{
				'.'
			});
			short major;
			ushort minor;
			if (array.Length == 1)
			{
				int num;
				if (int.TryParse(array[0], out num))
				{
					result = new ComponentVersion(num);
					return true;
				}
			}
			else if (array.Length == 2 && short.TryParse(array[0], out major) && ushort.TryParse(array[1], out minor))
			{
				result = new ComponentVersion(major, minor);
				return true;
			}
			result = ComponentVersion.Zero;
			return false;
		}

		public int Value
		{
			get
			{
				return this.value;
			}
		}

		public short Major
		{
			get
			{
				return (short)(this.value >> 16);
			}
		}

		public ushort Minor
		{
			get
			{
				return (ushort)(this.value & 65535);
			}
		}

		public ComponentVersion(int value)
		{
			this = new ComponentVersion((short)(value >> 16), (ushort)(value & 65535));
		}

		public ComponentVersion(short major, ushort minor)
		{
			this.value = ((int)major << 16 | (int)minor);
			this.versionCache = string.Format("{0}.{1}", major, minor);
		}

		public bool IsSupported(int minVersion, int maxVersion)
		{
			return minVersion <= this.value && this.value <= maxVersion;
		}

		public bool IsSupported(ComponentVersion minVersion, ComponentVersion maxVersion)
		{
			return this.IsSupported(minVersion.Value, maxVersion.Value);
		}

		public int CompareTo(ComponentVersion other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public override string ToString()
		{
			return this.versionCache;
		}

		private static ComponentVersion zero = new ComponentVersion(0);

		private int value;

		private string versionCache;
	}
}

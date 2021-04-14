using System;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PropTags
{
	public class StorePropName : IComparable<StorePropName>, IEquatable<StorePropName>
	{
		public StorePropName(Guid guid, string name)
		{
			this.guid = guid;
			this.name = name;
			this.dispId = uint.MaxValue;
		}

		public StorePropName(Guid guid, uint dispId)
		{
			this.guid = guid;
			this.name = null;
			this.dispId = dispId;
		}

		public static StorePropName Invalid
		{
			get
			{
				return StorePropName.invalid;
			}
		}

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public uint DispId
		{
			get
			{
				return this.dispId;
			}
		}

		public static bool operator ==(StorePropName name1, StorePropName name2)
		{
			return name1.Equals(name2);
		}

		public static bool operator !=(StorePropName name1, StorePropName name2)
		{
			return !name1.Equals(name2);
		}

		public static bool IsValidName(Guid guid, string name)
		{
			if (guid.Equals(StorePropName.InternetHeadersNamespaceGuid))
			{
				if (name == null)
				{
					return false;
				}
				foreach (char c in name)
				{
					if (c < '!' || c > '~' || c == ':')
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool Equals(StorePropName other)
		{
			return this.guid == other.guid && (object.ReferenceEquals(this.name, other.name) || string.Equals(this.name, other.name)) && this.dispId == other.dispId;
		}

		public override bool Equals(object other)
		{
			return other is StorePropName && this.Equals((StorePropName)other);
		}

		public int CompareTo(StorePropName other)
		{
			int num = this.guid.CompareTo(other.guid);
			if (num != 0)
			{
				return num;
			}
			if (!object.ReferenceEquals(this.name, other.name))
			{
				if (this.name == null)
				{
					return -1;
				}
				return this.name.CompareTo(other.name);
			}
			else
			{
				if (this.dispId > other.dispId)
				{
					return 1;
				}
				if (this.dispId < other.dispId)
				{
					return -1;
				}
				return 0;
			}
		}

		public override int GetHashCode()
		{
			return this.guid.GetHashCode() + (int)this.dispId + ((this.name == null) ? 0 : this.name.GetHashCode());
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			this.AppendToString(stringBuilder);
			return stringBuilder.ToString();
		}

		public void AppendToString(StringBuilder sb)
		{
			sb.Append(this.guid.ToString("D"));
			sb.Append(":");
			if (this.name != null)
			{
				sb.Append(this.name);
				return;
			}
			sb.Append("N:0x");
			sb.Append(this.dispId.ToString("X8"));
		}

		public static readonly Guid UnnamedPropertyNamespaceGuid = new Guid("00020328-0000-0000-C000-000000000046");

		public static readonly Guid InternetHeadersNamespaceGuid = new Guid("00020386-0000-0000-C000-000000000046");

		private static readonly StorePropName invalid = new StorePropName(Guid.Empty, null);

		private readonly Guid guid;

		private readonly string name;

		private readonly uint dispId;
	}
}

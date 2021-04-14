using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class ConfigObjectId : ObjectId
	{
		public ConfigObjectId(string identity)
		{
			this.identity = identity;
		}

		public static bool operator ==(ConfigObjectId left, ConfigObjectId right)
		{
			if (left == null)
			{
				return null == right;
			}
			return left.Equals(right);
		}

		public static bool operator !=(ConfigObjectId left, ConfigObjectId right)
		{
			return !(left == right);
		}

		public static explicit operator string(ConfigObjectId id)
		{
			if (null != id)
			{
				return id.identity;
			}
			return null;
		}

		public override bool Equals(object other)
		{
			return other != null && other is ConfigObjectId && this.identity == ((ConfigObjectId)other).identity;
		}

		public override int GetHashCode()
		{
			if (this.identity != null)
			{
				return this.identity.GetHashCode();
			}
			return 0;
		}

		public override string ToString()
		{
			return this.identity;
		}

		public override byte[] GetBytes()
		{
			if (this.identity != null)
			{
				return Encoding.Unicode.GetBytes(this.identity);
			}
			return null;
		}

		private string identity;
	}
}

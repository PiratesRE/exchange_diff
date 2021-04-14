using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class NetID : IEquatable<NetID>
	{
		public NetID(long netID)
		{
			this.netID = netID;
		}

		public NetID(byte[] netID) : this(NetID.BytesToString(netID))
		{
		}

		public NetID(string netID)
		{
			if (string.IsNullOrEmpty(netID))
			{
				throw new ArgumentNullException("netID");
			}
			if (netID.Length != 16 || netID.Trim().Length != 16)
			{
				throw new FormatException(DataStrings.ErrorIncorrectLiveIdFormat(netID));
			}
			try
			{
				this.netID = long.Parse(netID, NumberStyles.HexNumber);
			}
			catch (FormatException)
			{
				throw new FormatException(DataStrings.ErrorIncorrectLiveIdFormat(netID));
			}
		}

		public static NetID Parse(string netID)
		{
			return new NetID(netID);
		}

		public static bool TryParse(string netID, out NetID outNetID)
		{
			outNetID = null;
			long num;
			if (netID != null && netID.Length == 16 && netID.Trim().Length == 16 && long.TryParse(netID, NumberStyles.HexNumber, null, out num))
			{
				outNetID = new NetID(num);
				return true;
			}
			return false;
		}

		public bool Equals(NetID other)
		{
			return other != null && this.netID == other.netID;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as NetID);
		}

		public static bool operator ==(NetID left, NetID right)
		{
			if (left != null)
			{
				return left.Equals(right);
			}
			return right == null;
		}

		public static bool operator !=(NetID left, NetID right)
		{
			return !(left == right);
		}

		public override int GetHashCode()
		{
			return this.netID.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0:X16}", this.netID);
		}

		internal byte[] ToByteArray()
		{
			byte[] bytes = BitConverter.GetBytes(this.netID);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}

		internal ulong ToUInt64()
		{
			return (ulong)this.netID;
		}

		private static string BytesToString(byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in bytes)
			{
				stringBuilder.Append(string.Format("{0:X2}", b));
			}
			return stringBuilder.ToString();
		}

		internal const string PrefixMSWLID = "MS-WLID:";

		internal const string Prefix = "KERBEROS:";

		internal const string PrefixExWLID = "ExWLID:";

		internal const string ExWLIDFormat = "ExWLID:{0}-{1}";

		internal const string Suffix = "@live.com";

		internal const string PrefixConsumerWLID = "CS-WLID:";

		internal const string PrefixOriginalNetID = "EXORIGNETID:";

		private long netID;
	}
}

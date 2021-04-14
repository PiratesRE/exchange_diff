using System;

namespace Microsoft.Exchange.Connections.Pop
{
	[Serializable]
	public struct SmtpAddress : IEquatable<SmtpAddress>, IComparable<SmtpAddress>
	{
		internal string Local { get; set; }

		internal string Domain { get; set; }

		public static bool operator ==(SmtpAddress value1, SmtpAddress value2)
		{
			throw new NotImplementedException();
		}

		public static bool operator !=(SmtpAddress value1, SmtpAddress value2)
		{
			throw new NotImplementedException();
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public override bool Equals(object address)
		{
			throw new NotImplementedException();
		}

		public bool Equals(SmtpAddress address)
		{
			throw new NotImplementedException();
		}

		public int CompareTo(SmtpAddress address)
		{
			throw new NotImplementedException();
		}

		internal static readonly SmtpAddress Empty = default(SmtpAddress);
	}
}

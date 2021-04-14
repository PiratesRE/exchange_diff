using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Data
{
	[ImmutableObject(true)]
	public abstract class NetworkAddress : ProtocolAddress, IComparable<NetworkAddress>
	{
		protected NetworkAddress(NetworkProtocol protocol, string address) : base(protocol, address)
		{
			if (protocol == null)
			{
				throw new ArgumentNullException("protocol");
			}
			if (!NetworkProtocol.IsSupportedProtocol(protocol))
			{
				throw new ArgumentException(DataStrings.ExceptionUnsupportedNetworkProtocol);
			}
		}

		public static NetworkAddress Parse(string expression)
		{
			NetworkAddress result;
			NetworkAddress.ErrorCode errorCode;
			if (!NetworkAddress.InternalTryParse(expression, out result, out errorCode))
			{
				switch (errorCode)
				{
				case NetworkAddress.ErrorCode.InvalidFormat:
					throw new FormatException(DataStrings.ExceptionInvlidNetworkAddressFormat);
				case NetworkAddress.ErrorCode.UnsupportProtocol:
					throw new ArgumentException(DataStrings.ExceptionUnsupportedNetworkProtocol);
				case NetworkAddress.ErrorCode.InvalidAddress:
					throw new ArgumentOutOfRangeException(DataStrings.ExceptionInvlidProtocolAddressFormat);
				}
			}
			return result;
		}

		public static bool TryParse(string expression, out NetworkAddress address)
		{
			NetworkAddress.ErrorCode errorCode;
			return NetworkAddress.InternalTryParse(expression, out address, out errorCode);
		}

		private static bool InternalTryParse(string expression, out NetworkAddress address, out NetworkAddress.ErrorCode error)
		{
			bool result = false;
			address = null;
			error = (NetworkAddress.ErrorCode)0;
			string[] array = expression.Split(new char[]
			{
				':'
			}, 2, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length != 2)
			{
				error = NetworkAddress.ErrorCode.InvalidFormat;
			}
			else
			{
				NetworkProtocol networkProtocol = null;
				try
				{
					networkProtocol = NetworkProtocol.Parse(array[0].Trim());
				}
				catch (ArgumentException)
				{
					error = NetworkAddress.ErrorCode.UnsupportProtocol;
				}
				if (networkProtocol == null)
				{
					error = NetworkAddress.ErrorCode.UnsupportProtocol;
				}
				else
				{
					try
					{
						address = networkProtocol.GetNetworkAddress(array[1].Trim());
						result = true;
					}
					catch (ArgumentOutOfRangeException)
					{
						error = NetworkAddress.ErrorCode.InvalidAddress;
					}
				}
			}
			return result;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public static bool operator ==(NetworkAddress a, NetworkAddress b)
		{
			return a == b;
		}

		public static bool operator !=(NetworkAddress a, NetworkAddress b)
		{
			return a != b;
		}

		public int CompareTo(NetworkAddress other)
		{
			return base.CompareTo(other);
		}

		private enum ErrorCode
		{
			InvalidFormat = 1,
			UnsupportProtocol,
			InvalidAddress
		}
	}
}

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.Cluster
{
	[Serializable]
	public sealed class ExtendedErrorInfo
	{
		private void BuildToString()
		{
			Exception exception = this.m_Exception;
			if (exception != null)
			{
				this.m_toString = exception.ToString();
			}
		}

		public ExtendedErrorInfo(Exception exception)
		{
			this.m_Exception = exception;
			this.BuildToString();
		}

		public sealed override string ToString()
		{
			if (string.IsNullOrEmpty(this.m_toString))
			{
				return base.ToString();
			}
			return this.m_toString;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public static bool operator ==(ExtendedErrorInfo left, ExtendedErrorInfo right)
		{
			return object.ReferenceEquals(left, right) || (left != null && right != null && left.m_Exception == right.m_Exception);
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public static bool operator !=(ExtendedErrorInfo left, ExtendedErrorInfo right)
		{
			return ((!(left == right)) ? 1 : 0) != 0;
		}

		public static ExtendedErrorInfo Deserialize(string serializedString)
		{
			return SerializationServices.Deserialize<ExtendedErrorInfo>(Convert.FromBase64String(serializedString));
		}

		public static ExtendedErrorInfo Deserialize(byte[] serializedBytes)
		{
			return SerializationServices.Deserialize<ExtendedErrorInfo>(serializedBytes);
		}

		public byte[] SerializeToBytes()
		{
			return SerializationServices.Serialize(this);
		}

		public string SerializeToString()
		{
			return Convert.ToBase64String(SerializationServices.Serialize(this));
		}

		public Exception FailureException
		{
			get
			{
				return this.m_Exception;
			}
			set
			{
				this.m_Exception = value;
			}
		}

		private Exception m_Exception;

		private string m_toString;
	}
}

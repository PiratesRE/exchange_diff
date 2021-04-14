using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.Cluster
{
	[Serializable]
	internal class RpcErrorExceptionInfo
	{
		public RpcErrorExceptionInfo(RpcErrorExceptionInfo other)
		{
			this.m_errorCode = other.m_errorCode;
			this.m_errorMessage = null;
			this.m_reconstitutedException = null;
			if (!string.IsNullOrEmpty(other.m_errorMessage))
			{
				string errorMessage = other.m_errorMessage;
				this.m_errorMessage = string.Copy(errorMessage);
			}
			byte[] serializedException = other.m_serializedException;
			if (serializedException != null && serializedException.Length > 0)
			{
				byte[] array = new byte[serializedException.Length];
				this.m_serializedException = array;
				serializedException.CopyTo(array, 0);
			}
		}

		public RpcErrorExceptionInfo()
		{
			this.m_errorCode = 0;
			base..ctor();
		}

		public override string ToString()
		{
			string arg;
			if (this.m_errorMessage == null)
			{
				arg = "<null>";
			}
			else
			{
				arg = this.m_errorMessage;
			}
			int errorCode = this.m_errorCode;
			return string.Format(RpcErrorExceptionInfo.ToStringFormat, errorCode.ToString(), arg);
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public bool IsFailed()
		{
			byte[] serializedException = this.m_serializedException;
			return (serializedException != null && serializedException.Length > 0) || this.m_reconstitutedException != null || this.m_errorMessage != null || this.m_errorCode != 0;
		}

		public int ErrorCode
		{
			get
			{
				return this.m_errorCode;
			}
			set
			{
				this.m_errorCode = value;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.m_errorMessage;
			}
			set
			{
				this.m_errorMessage = value;
			}
		}

		public byte[] SerializedException
		{
			get
			{
				return this.m_serializedException;
			}
			set
			{
				this.m_serializedException = value;
			}
		}

		public Exception ReconstitutedException
		{
			get
			{
				return this.m_reconstitutedException;
			}
			set
			{
				this.m_reconstitutedException = value;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static RpcErrorExceptionInfo()
		{
			RpcErrorExceptionInfo.ToStringFormat = "RpcErrorExceptionInfo: [ErrorCode='{0}', ErrorMessage='{1}']";
		}

		private int m_errorCode;

		private string m_errorMessage;

		private byte[] m_serializedException;

		private Exception m_reconstitutedException;

		private static string ToStringFormat;

		public static int EcSuccess = 0;
	}
}

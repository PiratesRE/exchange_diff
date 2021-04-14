using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.Cluster
{
	[Serializable]
	internal sealed class RpcSeederStatus
	{
		public RpcSeederStatus(RpcSeederStatus other)
		{
			this.m_bytesRead = other.m_bytesRead;
			this.m_bytesWritten = other.m_bytesWritten;
			this.m_bytesTotal = other.m_bytesTotal;
			this.m_bytesTotalDivisor = other.m_bytesTotalDivisor;
			this.m_state = other.m_state;
			RpcErrorExceptionInfo errorInfo = other.m_errorInfo;
			this.m_errorInfo = new RpcErrorExceptionInfo(errorInfo);
			base..ctor();
			if (other.m_fileFullPath != null)
			{
				string fileFullPath = other.m_fileFullPath;
				string text = fileFullPath;
				string text2 = fileFullPath;
				this.m_fileFullPath = new string(text2.ToCharArray(), 0, text.Length);
			}
			if (other.m_addressForData != null)
			{
				string addressForData = other.m_addressForData;
				string text3 = addressForData;
				string text4 = addressForData;
				this.m_addressForData = new string(text4.ToCharArray(), 0, text3.Length);
			}
		}

		public RpcSeederStatus()
		{
			this.m_bytesRead = 0L;
			this.m_bytesWritten = 0L;
			this.m_bytesTotal = 0L;
			this.m_fileFullPath = string.Empty;
			this.m_addressForData = string.Empty;
			this.m_state = SeederState.Unknown;
			this.m_errorInfo = new RpcErrorExceptionInfo();
		}

		public long BytesRead
		{
			get
			{
				return this.m_bytesRead;
			}
			set
			{
				this.m_bytesRead = value;
			}
		}

		public long BytesWritten
		{
			get
			{
				return this.m_bytesWritten;
			}
			set
			{
				this.m_bytesWritten = value;
			}
		}

		public long BytesRemaining
		{
			get
			{
				return Math.Max(this.m_bytesTotal - this.m_bytesRead, 0L);
			}
		}

		public long BytesTotal
		{
			get
			{
				return this.m_bytesTotal;
			}
			set
			{
				this.m_bytesTotal = value;
			}
		}

		public long BytesTotalDivisor
		{
			get
			{
				return this.m_bytesTotalDivisor;
			}
			set
			{
				this.m_bytesTotalDivisor = value;
			}
		}

		public int PercentComplete
		{
			get
			{
				long bytesTotal = this.m_bytesTotal;
				if (bytesTotal == 0L)
				{
					return 0;
				}
				long bytesTotalDivisor = this.m_bytesTotalDivisor;
				long num = (bytesTotalDivisor == 0L) ? bytesTotal : bytesTotalDivisor;
				return (int)(this.m_bytesWritten * 100L / num);
			}
		}

		public string FileFullPath
		{
			get
			{
				return this.m_fileFullPath;
			}
			set
			{
				this.m_fileFullPath = value;
			}
		}

		public string AddressForData
		{
			get
			{
				return this.m_addressForData;
			}
			set
			{
				this.m_addressForData = value;
			}
		}

		public SeederState State
		{
			get
			{
				return this.m_state;
			}
			set
			{
				this.m_state = value;
			}
		}

		public RpcErrorExceptionInfo ErrorInfo
		{
			get
			{
				return this.m_errorInfo;
			}
			set
			{
				this.m_errorInfo = value;
			}
		}

		public uint FileNumber
		{
			get
			{
				return this.m_fileNumber;
			}
			set
			{
				this.m_fileNumber = value;
			}
		}

		public uint FileCount
		{
			get
			{
				return this.m_fileCount;
			}
			set
			{
				this.m_fileCount = value;
			}
		}

		public long BytesTotalDirectory
		{
			get
			{
				return this.m_bytesTotalDirectory;
			}
			set
			{
				this.m_bytesTotalDirectory = value;
			}
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public bool IsSeederStatusDataAvailable()
		{
			return this.PercentComplete >= 0 && this.m_fileFullPath != null && this.m_state != SeederState.Unknown;
		}

		private long m_bytesRead;

		private long m_bytesWritten;

		private long m_bytesTotal;

		private long m_bytesTotalDivisor;

		private string m_fileFullPath;

		private string m_addressForData;

		private SeederState m_state;

		private RpcErrorExceptionInfo m_errorInfo;

		private uint m_fileNumber;

		private uint m_fileCount;

		private long m_bytesTotalDirectory;
	}
}

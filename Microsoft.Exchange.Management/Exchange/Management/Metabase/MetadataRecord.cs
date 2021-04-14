using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Management.Metabase
{
	[StructLayout(LayoutKind.Sequential)]
	internal class MetadataRecord : IDisposable
	{
		internal MetadataRecord(int bufferSize)
		{
			if (bufferSize > 0)
			{
				this.DataBuf = new SafeHGlobalHandle(Marshal.AllocHGlobal(bufferSize));
			}
			else
			{
				this.DataBuf = SafeHGlobalHandle.InvalidHandle;
			}
			this.DataLen = bufferSize;
		}

		internal MetadataRecord(string value)
		{
			this.DataBuf = new SafeHGlobalHandle(Marshal.StringToHGlobalUni(value));
			this.DataLen = (value.Length + 1) * 2;
			this.DataType = MBDataType.String;
		}

		public void Dispose()
		{
			if (this.DataBuf != null)
			{
				this.DataBuf.Dispose();
			}
		}

		public MBIdentifier Identifier;

		public MBAttributes Attributes;

		public MBUserType UserType;

		public MBDataType DataType;

		public int DataLen;

		public SafeHGlobalHandle DataBuf;

		public int DataTag;
	}
}

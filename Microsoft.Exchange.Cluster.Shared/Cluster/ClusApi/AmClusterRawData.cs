using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal class AmClusterRawData : SafeDisposeTrackerHandleZeroOrMinusOneIsInvalid
	{
		internal AmClusterRawData(IntPtr data, uint dataSize, bool ownsHandle) : base(ownsHandle)
		{
			base.SetHandle(data);
			this.Size = dataSize;
		}

		private AmClusterRawData(IntPtr unused)
		{
			throw new NotImplementedException("Do not call this!");
		}

		internal IntPtr Buffer
		{
			get
			{
				return this.handle;
			}
		}

		internal uint Size { get; private set; }

		internal static AmClusterRawData Allocate(uint dataSize)
		{
			return new AmClusterRawData(Marshal.AllocHGlobal((int)dataSize), dataSize, true);
		}

		internal int ReadInt32()
		{
			return Marshal.ReadInt32(this.handle);
		}

		internal string ReadString()
		{
			return Marshal.PtrToStringUni(this.handle);
		}

		protected override bool ReleaseHandle()
		{
			Marshal.FreeHGlobal(this.handle);
			return true;
		}
	}
}

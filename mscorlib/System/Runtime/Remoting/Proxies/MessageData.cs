using System;

namespace System.Runtime.Remoting.Proxies
{
	internal struct MessageData
	{
		internal IntPtr pFrame;

		internal IntPtr pMethodDesc;

		internal IntPtr pDelegateMD;

		internal IntPtr pSig;

		internal IntPtr thGoverningType;

		internal int iFlags;
	}
}

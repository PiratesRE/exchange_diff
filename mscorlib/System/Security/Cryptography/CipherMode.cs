using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	[Serializable]
	public enum CipherMode
	{
		CBC = 1,
		ECB,
		OFB,
		CFB,
		CTS
	}
}

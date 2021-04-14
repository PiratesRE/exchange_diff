using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	[Serializable]
	public enum PaddingMode
	{
		None = 1,
		PKCS7,
		Zeros,
		ANSIX923,
		ISO10126
	}
}

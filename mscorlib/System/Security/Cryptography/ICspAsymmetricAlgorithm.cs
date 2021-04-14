using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public interface ICspAsymmetricAlgorithm
	{
		CspKeyContainerInfo CspKeyContainerInfo { get; }

		byte[] ExportCspBlob(bool includePrivateParameters);

		void ImportCspBlob(byte[] rawData);
	}
}

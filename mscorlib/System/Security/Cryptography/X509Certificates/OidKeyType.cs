using System;

namespace System.Security.Cryptography.X509Certificates
{
	internal enum OidKeyType
	{
		Oid = 1,
		Name,
		AlgorithmID,
		SignatureID,
		CngAlgorithmID,
		CngSignatureID
	}
}

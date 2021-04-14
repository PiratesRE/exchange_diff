using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IPrivateKeyDecryptor
	{
		byte[] Decrypt(string encryptedData);
	}
}

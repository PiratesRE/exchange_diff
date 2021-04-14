using System;
using System.Security.Cryptography;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	internal class E4eDecryptionHelperV2 : E4eDecryptionHelper
	{
		internal new static E4eDecryptionHelperV2 Instance
		{
			get
			{
				if (E4eDecryptionHelperV2.instance == null)
				{
					E4eDecryptionHelperV2.instance = new E4eDecryptionHelperV2();
				}
				return E4eDecryptionHelperV2.instance;
			}
		}

		protected override bool VerifySignature(byte[] signatureByteArray, RSACryptoServiceProvider csp, byte[] data)
		{
			bool result;
			using (SHA256Cng sha256Cng = new SHA256Cng())
			{
				byte[] rgbHash = sha256Cng.ComputeHash(data);
				result = csp.VerifyHash(rgbHash, CryptoConfig.MapNameToOID("SHA256"), signatureByteArray);
			}
			return result;
		}

		private static E4eDecryptionHelperV2 instance;
	}
}

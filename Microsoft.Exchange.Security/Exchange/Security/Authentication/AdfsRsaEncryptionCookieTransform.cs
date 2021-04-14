using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Web;

namespace Microsoft.Exchange.Security.Authentication
{
	public class AdfsRsaEncryptionCookieTransform : AdfsCookieTransform
	{
		public AdfsRsaEncryptionCookieTransform(X509Certificate2[] certificates) : base(certificates)
		{
			for (int i = 0; i < certificates.Length; i++)
			{
				this.Transforms[i] = new RsaEncryptionCookieTransform(certificates[i]);
			}
		}
	}
}

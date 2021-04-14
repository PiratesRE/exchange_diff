using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Web;

namespace Microsoft.Exchange.Security.Authentication
{
	public class AdfsRsaSignatureCookieTransform : AdfsCookieTransform
	{
		public AdfsRsaSignatureCookieTransform(X509Certificate2[] certificates) : base(certificates)
		{
			for (int i = 0; i < certificates.Length; i++)
			{
				this.Transforms[i] = new RsaSignatureCookieTransform(certificates[i]);
			}
		}
	}
}

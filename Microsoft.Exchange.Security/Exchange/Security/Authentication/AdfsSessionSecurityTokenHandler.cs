using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Web;

namespace Microsoft.Exchange.Security.Authentication
{
	public class AdfsSessionSecurityTokenHandler : SessionSecurityTokenHandler
	{
		public AdfsSessionSecurityTokenHandler() : base(AdfsSessionSecurityTokenHandler.CreateTransforms())
		{
		}

		protected override byte[] ApplyTransforms(byte[] cookie, bool outbound)
		{
			byte[] result = cookie;
			try
			{
				result = base.ApplyTransforms(cookie, outbound);
			}
			catch (AdfsConfigurationException ex)
			{
				ExTraceGlobals.AdfsAuthModuleTracer.TraceError<AdfsConfigurationException>(0L, "[AdfsSessionAuthModule::ApplyTransforms]: Exception occurred: {0}.", ex);
				HttpContext.Current.Response.Redirect(string.Format(CultureInfo.InvariantCulture, "{0}?msg={1}", new object[]
				{
					"/owa/auth/errorfe.aspx",
					ex.Reason
				}), true);
			}
			return result;
		}

		private static ReadOnlyCollection<CookieTransform> CreateTransforms()
		{
			X509Certificate2[] certificates = Utility.GetCertificates();
			return new List<CookieTransform>
			{
				new DeflateCookieTransform(),
				new AdfsRsaSignatureCookieTransform(certificates),
				new AdfsRsaEncryptionCookieTransform(certificates)
			}.AsReadOnly();
		}
	}
}

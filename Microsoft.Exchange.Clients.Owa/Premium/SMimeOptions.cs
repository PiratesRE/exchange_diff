using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class SMimeOptions : OwaPage, IRegistryOnlyForm
	{
		protected override void OnLoad(EventArgs e)
		{
			if (!base.UserContext.IsFeatureEnabled(Feature.SMime))
			{
				throw new OwaSegmentationException("SMime is disabled.");
			}
		}

		protected static string SMimeClientControlPath
		{
			get
			{
				if (SMimeOptions.path == null)
				{
					SMimeOptions.path = "smime/owasmime.msi?v=" + Utilities.ReadSMimeControlVersionOnServer();
				}
				return SMimeOptions.path;
			}
		}

		protected bool ManuallyPickCertificate
		{
			get
			{
				return base.UserContext.UserOptions.ManuallyPickCertificate;
			}
		}

		protected string SigningCertificateSubject
		{
			get
			{
				return base.UserContext.UserOptions.SigningCertificateSubject ?? string.Empty;
			}
		}

		protected string SigningCertificateId
		{
			get
			{
				return base.UserContext.UserOptions.SigningCertificateId ?? string.Empty;
			}
		}

		protected bool HasSelectCertificateSection
		{
			get
			{
				return OwaRegistryKeys.AllowUserChoiceOfSigningCertificate;
			}
		}

		private static string path;
	}
}

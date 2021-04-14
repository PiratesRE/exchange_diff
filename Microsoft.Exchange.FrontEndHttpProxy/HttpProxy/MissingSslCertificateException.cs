using System;

namespace Microsoft.Exchange.HttpProxy
{
	[Serializable]
	internal class MissingSslCertificateException : Exception
	{
		public MissingSslCertificateException() : base(MissingSslCertificateException.ErrorMessage)
		{
		}

		private static readonly string ErrorMessage = "Failed to load SSL certificate.";
	}
}

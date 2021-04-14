using System;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal sealed class ParameterValues
	{
		private ParameterValues()
		{
			throw new NotSupportedException();
		}

		public const string WinMailDotDat = "winmail.dat";

		public const string DeliveryStatus = "delivery-status";

		public const string DispositionNotification = "disposition-notification";

		public const string SignedData = "signed-data";

		public const string EnvelopedData = "enveloped-data";

		public const string CertsOnly = "certs-only";

		public const string ApplicationPgpSignature = "application/pgp-signature";

		public const string ApplicationPgpEncrypted = "application/pgp-encrypted";

		public const string ApplicationXSmimeSignedXml = "application/xsmime-signed+xml";

		public const string ApplicationXSmimeEncryptedXml = "application/xsmime-encrypted+xml";

		public const string MessageRpmsg = "message.rpmsg";
	}
}

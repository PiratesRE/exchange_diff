﻿using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal enum SmtpSessionCertificateUse
	{
		DirectTrust,
		STARTTLS,
		RemoteDirectTrust,
		RemoteSTARTTLS
	}
}

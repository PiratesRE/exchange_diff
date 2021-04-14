using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface ISmtpMessageContextBlob
	{
		bool TryGetOrderedListOfBlobsToReceive(string mailCommandParameter, out MailCommandMessageContextParameters messageContextInfo);

		List<SmtpMessageContextBlob> GetAdvertisedMandatoryBlobs(IEhloOptions ehloOptions);

		AdrcSmtpMessageContextBlob AdrcSmtpMessageContextBlob { get; }

		ExtendedPropertiesSmtpMessageContextBlob ExtendedPropertiesSmtpMessageContextBlob { get; }

		FastIndexSmtpMessageContextBlob FastIndexSmtpMessageContextBlob { get; }
	}
}

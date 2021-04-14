using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpMessageContextBlobWrapper : ISmtpMessageContextBlob
	{
		public bool TryGetOrderedListOfBlobsToReceive(string mailCommandParameter, out MailCommandMessageContextParameters messageContextInfo)
		{
			ArgumentValidator.ThrowIfNull("mailCommandParameter", mailCommandParameter);
			return SmtpMessageContextBlob.TryGetOrderedListOfBlobsToReceive(mailCommandParameter, out messageContextInfo);
		}

		public List<SmtpMessageContextBlob> GetAdvertisedMandatoryBlobs(IEhloOptions ehloOptions)
		{
			ArgumentValidator.ThrowIfNull("ehloOptions", ehloOptions);
			return SmtpMessageContextBlob.GetAdvertisedMandatoryBlobs(ehloOptions);
		}

		public AdrcSmtpMessageContextBlob AdrcSmtpMessageContextBlob
		{
			get
			{
				return SmtpMessageContextBlob.AdrcSmtpMessageContextBlobInstance;
			}
		}

		public ExtendedPropertiesSmtpMessageContextBlob ExtendedPropertiesSmtpMessageContextBlob
		{
			get
			{
				return SmtpMessageContextBlob.ExtendedPropertiesSmtpMessageContextBlobInstance;
			}
		}

		public FastIndexSmtpMessageContextBlob FastIndexSmtpMessageContextBlob
		{
			get
			{
				return SmtpMessageContextBlob.FastIndexSmtpMessageContextBlobInstance;
			}
		}
	}
}

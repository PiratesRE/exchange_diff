using System;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal interface IMailRecipientCollectionFacade
	{
		void Add(string smtpAddress);

		void AddWithoutDsnRequested(string smtpAddress);
	}
}

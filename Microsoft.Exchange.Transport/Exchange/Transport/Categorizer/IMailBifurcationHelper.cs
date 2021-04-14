using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal interface IMailBifurcationHelper<T> where T : IEquatable<T>, IComparable<T>, new()
	{
		bool GetBifurcationInfo(MailRecipient recipient, out T bifurcationInfo);

		TransportMailItem GenerateNewMailItem(IList<MailRecipient> newMailItemRecipients, T bifurcationInfo);

		bool NeedsBifurcation();
	}
}

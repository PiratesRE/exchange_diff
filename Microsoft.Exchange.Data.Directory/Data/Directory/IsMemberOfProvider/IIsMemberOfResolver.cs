using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.IsMemberOfProvider
{
	internal interface IIsMemberOfResolver<in TGroupKeyType> : IDisposable
	{
		void ClearCache();

		bool IsMemberOf(IRecipientSession session, ADObjectId recipientId, TGroupKeyType groupKey);

		bool IsMemberOf(IRecipientSession session, Guid recipientObjectGuid, TGroupKeyType groupKey);
	}
}

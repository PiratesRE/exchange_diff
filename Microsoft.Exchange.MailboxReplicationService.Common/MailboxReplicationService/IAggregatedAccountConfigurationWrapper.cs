using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IAggregatedAccountConfigurationWrapper
	{
		ADUser TargetUser { get; set; }

		Guid TargetExchangeGuid { get; set; }

		IExchangePrincipal GetExchangePrincipal();

		void SetExchangePrincipal();

		void UpdateData(RequestJobBase requestJob);

		void Save(MailboxStoreTypeProvider provider);

		void Delete(MailboxStoreTypeProvider provider);
	}
}

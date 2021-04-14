using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[ServiceContract(SessionMode = SessionMode.Required)]
	internal interface IInjectorService : IVersionedService, IDisposeTrackable, IDisposable
	{
		[OperationContract]
		void InjectMoves(Guid targetDatabase, string batchName, IEnumerable<LoadEntity> mailboxes);

		[OperationContract]
		void InjectSingleMove(Guid targetDatabase, string batchName, LoadEntity mailbox);
	}
}

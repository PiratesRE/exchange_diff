using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[ServiceContract(SessionMode = SessionMode.Required)]
	internal interface IMailboxReplicationServiceSlim
	{
		[OperationContract]
		void SyncNow(List<SyncNowNotification> notifications);
	}
}

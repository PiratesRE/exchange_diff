using System;
using System.ServiceModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[ServiceContract(SessionMode = SessionMode.Required)]
	internal interface IVersionedService : IDisposeTrackable, IDisposable
	{
		[OperationContract]
		void ExchangeVersionInformation(VersionInformation clientVersion, out VersionInformation serverVersion);
	}
}

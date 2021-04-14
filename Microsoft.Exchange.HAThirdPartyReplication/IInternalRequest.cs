using System;
using System.CodeDom.Compiler;
using System.ServiceModel;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0")]
	[ServiceContract(Namespace = "http://Microsoft.Exchange.ThirdPartyReplication.Requests", ConfigurationName = "Microsoft.Exchange.ThirdPartyReplication.IInternalRequest")]
	public interface IInternalRequest
	{
		[OperationContract(Action = "http://Microsoft.Exchange.ThirdPartyReplication.Requests/IInternalRequest/GetPrimaryActiveManager", ReplyAction = "http://Microsoft.Exchange.ThirdPartyReplication.Requests/IInternalRequest/GetPrimaryActiveManagerResponse")]
		string GetPrimaryActiveManager(out byte[] ex);

		[OperationContract(Action = "http://Microsoft.Exchange.ThirdPartyReplication.Requests/IInternalRequest/ChangeActiveServer", ReplyAction = "http://Microsoft.Exchange.ThirdPartyReplication.Requests/IInternalRequest/ChangeActiveServerResponse")]
		byte[] ChangeActiveServer(Guid databaseId, string newActiveServerName);

		[OperationContract(Action = "http://Microsoft.Exchange.ThirdPartyReplication.Requests/IInternalRequest/ImmediateDismountMailboxDatabase", ReplyAction = "http://Microsoft.Exchange.ThirdPartyReplication.Requests/IInternalRequest/ImmediateDismountMailboxDatabaseResponse")]
		byte[] ImmediateDismountMailboxDatabase(Guid databaseId);

		[OperationContract(Action = "http://Microsoft.Exchange.ThirdPartyReplication.Requests/IInternalRequest/AmeIsStarting", ReplyAction = "http://Microsoft.Exchange.ThirdPartyReplication.Requests/IInternalRequest/AmeIsStartingResponse")]
		void AmeIsStarting(TimeSpan retryDelay, TimeSpan openTimeout, TimeSpan sendTimeout, TimeSpan receiveTimeout);

		[OperationContract(Action = "http://Microsoft.Exchange.ThirdPartyReplication.Requests/IInternalRequest/AmeIsStopping", ReplyAction = "http://Microsoft.Exchange.ThirdPartyReplication.Requests/IInternalRequest/AmeIsStoppingResponse")]
		void AmeIsStopping();
	}
}

using System;
using System.CodeDom.Compiler;
using System.ServiceModel;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0")]
	[ServiceContract(Namespace = "http://Microsoft.Exchange.ThirdPartyReplication.Notifications", ConfigurationName = "Microsoft.Exchange.ThirdPartyReplication.IInternalNotify")]
	public interface IInternalNotify
	{
		[OperationContract(IsOneWay = true, Action = "http://Microsoft.Exchange.ThirdPartyReplication.Notifications/IInternalNotify/BecomePame")]
		void BecomePame();

		[OperationContract(IsOneWay = true, Action = "http://Microsoft.Exchange.ThirdPartyReplication.Notifications/IInternalNotify/RevokePame")]
		void RevokePame();

		[OperationContract(Action = "http://Microsoft.Exchange.ThirdPartyReplication.Notifications/IInternalNotify/DatabaseMoveNeeded", ReplyAction = "http://Microsoft.Exchange.ThirdPartyReplication.Notifications/IInternalNotify/DatabaseMoveNeededResponse")]
		NotificationResponse DatabaseMoveNeeded(Guid databaseId, string currentActiveFqdn, bool mountDesired);

		[OperationContract(Action = "http://Microsoft.Exchange.ThirdPartyReplication.Notifications/IInternalNotify/GetTimeouts", ReplyAction = "http://Microsoft.Exchange.ThirdPartyReplication.Notifications/IInternalNotify/GetTimeoutsResponse")]
		int GetTimeouts(out TimeSpan retryDelay, out TimeSpan openTimeout, out TimeSpan sendTimeout, out TimeSpan receiveTimeout);
	}
}

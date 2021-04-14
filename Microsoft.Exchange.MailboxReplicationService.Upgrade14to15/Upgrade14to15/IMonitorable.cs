using System;
using System.CodeDom.Compiler;
using System.ServiceModel;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ServiceContract(ConfigurationName = "Microsoft.Exchange.MailboxReplicationService.Upgrade14to15.WorkloadService.IMonitorable")]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	public interface IMonitorable
	{
		[FaultContract(typeof(AccessDeniedFault), Action = "http://tempuri.org/IMonitorable/PingAccessDeniedFaultFault", Name = "AccessDeniedFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.Common")]
		[OperationContract(Action = "http://tempuri.org/IMonitorable/Ping", ReplyAction = "http://tempuri.org/IMonitorable/PingResponse")]
		string Ping();

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IMonitorable/Ping", ReplyAction = "http://tempuri.org/IMonitorable/PingResponse")]
		IAsyncResult BeginPing(AsyncCallback callback, object asyncState);

		string EndPing(IAsyncResult result);
	}
}

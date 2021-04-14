using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[ServiceContract(Namespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", ConfigurationName = "Microsoft.Exchange.Data.Directory.Sync.IFederatedServiceOnboarding")]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	public interface IFederatedServiceOnboarding
	{
		[ServiceKnownType(typeof(DirectoryReference))]
		[XmlSerializerFormat]
		[FaultContract(typeof(ArgumentException), Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/GetServiceInstanceMapArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[ServiceKnownType(typeof(DirectoryProperty))]
		[ServiceKnownType(typeof(CompanyDomainValue))]
		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/GetServiceInstanceMap", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/GetServiceInstanceMapResponse")]
		[FaultContract(typeof(BindingRedirectionFault), Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/GetServiceInstanceMapBindingRedirectionFaultFault", Name = "BindingRedirectionFault", Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11")]
		GetServiceInstanceMapResponse GetServiceInstanceMap(GetServiceInstanceMapRequest request);

		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/GetServiceInstanceMap", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/GetServiceInstanceMapResponse")]
		Task<GetServiceInstanceMapResponse> GetServiceInstanceMapAsync(GetServiceInstanceMapRequest request);

		[FaultContract(typeof(ArgumentException), Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/SetServiceInstanceMapArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[XmlSerializerFormat]
		[ServiceKnownType(typeof(DirectoryReference))]
		[ServiceKnownType(typeof(DirectoryProperty))]
		[ServiceKnownType(typeof(CompanyDomainValue))]
		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/SetServiceInstanceMap", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/SetServiceInstanceMapResponse")]
		[FaultContract(typeof(BindingRedirectionFault), Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/SetServiceInstanceMapBindingRedirectionFaultFault", Name = "BindingRedirectionFault", Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11")]
		SetServiceInstanceMapResponse SetServiceInstanceMap(SetServiceInstanceMapRequest request);

		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/SetServiceInstanceMap", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/SetServiceInstanceMapResponse")]
		Task<SetServiceInstanceMapResponse> SetServiceInstanceMapAsync(SetServiceInstanceMapRequest request);

		[ServiceKnownType(typeof(DirectoryProperty))]
		[ServiceKnownType(typeof(CompanyDomainValue))]
		[ServiceKnownType(typeof(DirectoryReference))]
		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/GetServiceInstanceInfo", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/GetServiceInstanceInfoResponse")]
		[FaultContract(typeof(ArgumentException), Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/GetServiceInstanceInfoArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[FaultContract(typeof(BindingRedirectionFault), Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/GetServiceInstanceInfoBindingRedirectionFaultFault", Name = "BindingRedirectionFault", Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11")]
		[XmlSerializerFormat]
		GetServiceInstanceInfoResponse GetServiceInstanceInfo(GetServiceInstanceInfoRequest request);

		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/GetServiceInstanceInfo", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/GetServiceInstanceInfoResponse")]
		Task<GetServiceInstanceInfoResponse> GetServiceInstanceInfoAsync(GetServiceInstanceInfoRequest request);

		[FaultContract(typeof(BindingRedirectionFault), Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/SetServiceInstanceInfoBindingRedirectionFaultFault", Name = "BindingRedirectionFault", Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11")]
		[ServiceKnownType(typeof(CompanyDomainValue))]
		[ServiceKnownType(typeof(DirectoryReference))]
		[ServiceKnownType(typeof(DirectoryProperty))]
		[FaultContract(typeof(ArgumentException), Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/SetServiceInstanceInfoArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/SetServiceInstanceInfo", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/SetServiceInstanceInfoResponse")]
		[XmlSerializerFormat]
		SetServiceInstanceInfoResponse SetServiceInstanceInfo(SetServiceInstanceInfoRequest request);

		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/SetServiceInstanceInfo", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/SetServiceInstanceInfoResponse")]
		Task<SetServiceInstanceInfoResponse> SetServiceInstanceInfoAsync(SetServiceInstanceInfoRequest request);

		[XmlSerializerFormat]
		[FaultContract(typeof(BindingRedirectionFault), Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/QueueEduFaultinBindingRedirectionFaultFault", Name = "BindingRedirectionFault", Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11")]
		[ServiceKnownType(typeof(DirectoryReference))]
		[ServiceKnownType(typeof(DirectoryProperty))]
		[ServiceKnownType(typeof(CompanyDomainValue))]
		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/QueueEduFaultin", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/QueueEduFaultinResponse")]
		[FaultContract(typeof(ArgumentException), Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/QueueEduFaultinArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		QueueEduFaultinResponse QueueEduFaultin(QueueEduFaultinRequest request);

		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/QueueEduFaultin", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/QueueEduFaultinResponse")]
		Task<QueueEduFaultinResponse> QueueEduFaultinAsync(QueueEduFaultinRequest request);

		[ServiceKnownType(typeof(DirectoryReference))]
		[ServiceKnownType(typeof(CompanyDomainValue))]
		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/GetEduFaultinStatus", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/GetEduFaultinStatusResponse")]
		[ServiceKnownType(typeof(DirectoryProperty))]
		[FaultContract(typeof(ArgumentException), Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/GetEduFaultinStatusArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[FaultContract(typeof(BindingRedirectionFault), Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/GetEduFaultinStatusBindingRedirectionFaultFault", Name = "BindingRedirectionFault", Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11")]
		[XmlSerializerFormat]
		GetEduFaultinStatusResponse GetEduFaultinStatus(GetEduFaultinStatusRequest request);

		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/GetEduFaultinStatus", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11/IFederatedServiceOnboarding/GetEduFaultinStatusResponse")]
		Task<GetEduFaultinStatusResponse> GetEduFaultinStatusAsync(GetEduFaultinStatusRequest request);
	}
}

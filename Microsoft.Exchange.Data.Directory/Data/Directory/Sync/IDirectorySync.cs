using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[ServiceContract(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", ConfigurationName = "Microsoft.Exchange.Data.Directory.Sync.IDirectorySync")]
	public interface IDirectorySync
	{
		[XmlSerializerFormat]
		[FaultContract(typeof(ArgumentException), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/NewCookieArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[FaultContract(typeof(BindingRedirectionFault), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/NewCookieBindingRedirectionFaultFault", Name = "BindingRedirectionFault")]
		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/NewCookie", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/NewCookieResponse")]
		NewCookieResponse NewCookie(NewCookieRequest request);

		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/NewCookie", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/NewCookieResponse")]
		Task<NewCookieResponse> NewCookieAsync(NewCookieRequest request);

		[FaultContract(typeof(BindingRedirectionFault), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/NewCookie2BindingRedirectionFaultFault", Name = "BindingRedirectionFault")]
		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/NewCookie2", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/NewCookie2Response")]
		[XmlSerializerFormat]
		[FaultContract(typeof(ArgumentException), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/NewCookie2ArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		NewCookie2Response NewCookie2(NewCookie2Request request);

		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/NewCookie2", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/NewCookie2Response")]
		Task<NewCookie2Response> NewCookie2Async(NewCookie2Request request);

		[XmlSerializerFormat]
		[ServiceKnownType(typeof(DirectoryProperty))]
		[FaultContract(typeof(ArgumentException), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetChangesArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[ServiceKnownType(typeof(CompanyDomainValue))]
		[ServiceKnownType(typeof(DirectoryReference))]
		[FaultContract(typeof(SecretEncryptionFailureFault), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetChangesSecretEncryptionFailureFaultFault", Name = "SecretEncryptionFailureFault")]
		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetChanges", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetChangesResponse")]
		[FaultContract(typeof(BindingRedirectionFault), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetChangesBindingRedirectionFaultFault", Name = "BindingRedirectionFault")]
		GetChangesResponse GetChanges(GetChangesRequest request);

		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetChanges", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetChangesResponse")]
		Task<GetChangesResponse> GetChangesAsync(GetChangesRequest request);

		[FaultContract(typeof(BindingRedirectionFault), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/PublishBindingRedirectionFaultFault", Name = "BindingRedirectionFault")]
		[ServiceKnownType(typeof(DirectoryProperty))]
		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/Publish", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/PublishResponse")]
		[FaultContract(typeof(ArgumentException), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/PublishArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[XmlSerializerFormat]
		[ServiceKnownType(typeof(CompanyDomainValue))]
		[ServiceKnownType(typeof(DirectoryReference))]
		PublishResponse Publish(PublishRequest request);

		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/Publish", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/PublishResponse")]
		Task<PublishResponse> PublishAsync(PublishRequest request);

		[XmlSerializerFormat]
		[FaultContract(typeof(BindingRedirectionFault), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetContextBindingRedirectionFaultFault", Name = "BindingRedirectionFault")]
		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetContext", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetContextResponse")]
		[FaultContract(typeof(ArgumentException), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetContextArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[ServiceKnownType(typeof(CompanyDomainValue))]
		[ServiceKnownType(typeof(DirectoryProperty))]
		[FaultContract(typeof(SecretEncryptionFailureFault), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetContextSecretEncryptionFailureFaultFault", Name = "SecretEncryptionFailureFault")]
		[ServiceKnownType(typeof(DirectoryReference))]
		GetContextResponse GetContext(GetContextRequest request);

		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetContext", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetContextResponse")]
		Task<GetContextResponse> GetContextAsync(GetContextRequest request);

		[ServiceKnownType(typeof(CompanyDomainValue))]
		[ServiceKnownType(typeof(DirectoryProperty))]
		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetDirectoryObjects", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetDirectoryObjectsResponse")]
		[FaultContract(typeof(ArgumentException), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetDirectoryObjectsArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[FaultContract(typeof(BindingRedirectionFault), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetDirectoryObjectsBindingRedirectionFaultFault", Name = "BindingRedirectionFault")]
		[FaultContract(typeof(SecretEncryptionFailureFault), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetDirectoryObjectsSecretEncryptionFailureFaultFault", Name = "SecretEncryptionFailureFault")]
		[XmlSerializerFormat]
		[ServiceKnownType(typeof(DirectoryReference))]
		GetDirectoryObjectsResponse GetDirectoryObjects(GetDirectoryObjectsRequest request);

		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetDirectoryObjects", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetDirectoryObjectsResponse")]
		Task<GetDirectoryObjectsResponse> GetDirectoryObjectsAsync(GetDirectoryObjectsRequest request);

		[FaultContract(typeof(SecretEncryptionFailureFault), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/MergeCookiesSecretEncryptionFailureFaultFault", Name = "SecretEncryptionFailureFault")]
		[XmlSerializerFormat]
		[ServiceKnownType(typeof(DirectoryReference))]
		[ServiceKnownType(typeof(DirectoryProperty))]
		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/MergeCookies", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/MergeCookiesResponse")]
		[FaultContract(typeof(ArgumentException), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/MergeCookiesArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[FaultContract(typeof(BindingRedirectionFault), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/MergeCookiesBindingRedirectionFaultFault", Name = "BindingRedirectionFault")]
		[ServiceKnownType(typeof(CompanyDomainValue))]
		MergeCookiesResponse MergeCookies(MergeCookiesRequest request);

		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/MergeCookies", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/MergeCookiesResponse")]
		Task<MergeCookiesResponse> MergeCookiesAsync(MergeCookiesRequest request);

		[FaultContract(typeof(BindingRedirectionFault), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetDirSyncDrainageStatusBindingRedirectionFaultFault", Name = "BindingRedirectionFault")]
		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetDirSyncDrainageStatus", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetDirSyncDrainageStatusResponse")]
		[FaultContract(typeof(ArgumentException), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetDirSyncDrainageStatusArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[ServiceKnownType(typeof(CompanyDomainValue))]
		[ServiceKnownType(typeof(DirectoryProperty))]
		[XmlSerializerFormat]
		[ServiceKnownType(typeof(DirectoryReference))]
		GetDirSyncDrainageStatusResponse GetDirSyncDrainageStatus(GetDirSyncDrainageStatusRequest request);

		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetDirSyncDrainageStatus", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetDirSyncDrainageStatusResponse")]
		Task<GetDirSyncDrainageStatusResponse> GetDirSyncDrainageStatusAsync(GetDirSyncDrainageStatusRequest request);

		[ServiceKnownType(typeof(DirectoryProperty))]
		[ServiceKnownType(typeof(DirectoryReference))]
		[FaultContract(typeof(ArgumentException), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/UpdateCookieArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/UpdateCookie", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/UpdateCookieResponse")]
		[FaultContract(typeof(BindingRedirectionFault), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/UpdateCookieBindingRedirectionFaultFault", Name = "BindingRedirectionFault")]
		[XmlSerializerFormat]
		[ServiceKnownType(typeof(CompanyDomainValue))]
		UpdateCookieResponse UpdateCookie(UpdateCookieRequest request);

		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/UpdateCookie", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/UpdateCookieResponse")]
		Task<UpdateCookieResponse> UpdateCookieAsync(UpdateCookieRequest request);

		[ServiceKnownType(typeof(CompanyDomainValue))]
		[XmlSerializerFormat]
		[ServiceKnownType(typeof(DirectoryReference))]
		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetCookieUpdateStatus", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetCookieUpdateStatusResponse")]
		[FaultContract(typeof(ArgumentException), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetCookieUpdateStatusArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[ServiceKnownType(typeof(DirectoryProperty))]
		[FaultContract(typeof(BindingRedirectionFault), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetCookieUpdateStatusBindingRedirectionFaultFault", Name = "BindingRedirectionFault")]
		GetCookieUpdateStatusResponse GetCookieUpdateStatus(GetCookieUpdateStatusRequest request);

		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetCookieUpdateStatus", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/GetCookieUpdateStatusResponse")]
		Task<GetCookieUpdateStatusResponse> GetCookieUpdateStatusAsync(GetCookieUpdateStatusRequest request);

		[ServiceKnownType(typeof(DirectoryReference))]
		[ServiceKnownType(typeof(CompanyDomainValue))]
		[ServiceKnownType(typeof(DirectoryProperty))]
		[XmlSerializerFormat]
		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/FilterAndGetContextRecoveryInfo", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/FilterAndGetContextRecoveryInfoResponse")]
		[FaultContract(typeof(ArgumentException), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/FilterAndGetContextRecoveryInfoArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[FaultContract(typeof(BindingRedirectionFault), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/FilterAndGetContextRecoveryInfoBindingRedirectionFaultFault", Name = "BindingRedirectionFault")]
		FilterAndGetContextRecoveryInfoResponse FilterAndGetContextRecoveryInfo(FilterAndGetContextRecoveryInfoRequest request);

		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/FilterAndGetContextRecoveryInfo", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/FilterAndGetContextRecoveryInfoResponse")]
		Task<FilterAndGetContextRecoveryInfoResponse> FilterAndGetContextRecoveryInfoAsync(FilterAndGetContextRecoveryInfoRequest request);

		[XmlSerializerFormat]
		[FaultContract(typeof(ArgumentException), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/EstimateBacklogArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[ServiceKnownType(typeof(CompanyDomainValue))]
		[FaultContract(typeof(BindingRedirectionFault), Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/EstimateBacklogBindingRedirectionFaultFault", Name = "BindingRedirectionFault")]
		[ServiceKnownType(typeof(DirectoryReference))]
		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/EstimateBacklog", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/EstimateBacklogResponse")]
		[ServiceKnownType(typeof(DirectoryProperty))]
		EstimateBacklogResponse EstimateBacklog(EstimateBacklogRequest request);

		[OperationContract(Action = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/EstimateBacklog", ReplyAction = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11/IDirectorySync/EstimateBacklogResponse")]
		Task<EstimateBacklogResponse> EstimateBacklogAsync(EstimateBacklogRequest request);
	}
}

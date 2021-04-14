using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync;

[ServiceContract(ConfigurationName = "IAdminSyncService")]
[GeneratedCode("System.ServiceModel", "3.0.0.0")]
internal interface IAdminSyncService
{
	[OperationContract(Action = "http://tempuri.org/IAdminSyncService/Ping", ReplyAction = "http://tempuri.org/IAdminSyncService/PingResponse")]
	void Ping();

	[OperationContract(Action = "http://tempuri.org/IAdminSyncService/SyncAdminAccounts", ReplyAction = "http://tempuri.org/IAdminSyncService/SyncAdminAccountsResponse")]
	[FaultContract(typeof(InvalidContractFault), Action = "http://tempuri.org/IAdminSyncService/SyncAdminAccountsInvalidContractFaultFault", Name = "InvalidContractFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[FaultContract(typeof(AuthorizationFault), Action = "http://tempuri.org/IAdminSyncService/SyncAdminAccountsAuthorizationFaultFault", Name = "AuthorizationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[FaultContract(typeof(InvalidCompanyFault), Action = "http://tempuri.org/IAdminSyncService/SyncAdminAccountsInvalidCompanyFaultFault", Name = "InvalidCompanyFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[FaultContract(typeof(InternalFault), Action = "http://tempuri.org/IAdminSyncService/SyncAdminAccountsInternalFaultFault", Name = "InternalFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	FailedAdminAccounts SyncAdminAccounts(CompanyAdministrators companyAdministrators);

	[FaultContract(typeof(AuthorizationFault), Action = "http://tempuri.org/IAdminSyncService/SyncGroupUsersAuthorizationFaultFault", Name = "AuthorizationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[FaultContract(typeof(InvalidGroupFault), Action = "http://tempuri.org/IAdminSyncService/SyncGroupUsersInvalidGroupFaultFault", Name = "InvalidGroupFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[FaultContract(typeof(InvalidCompanyFault), Action = "http://tempuri.org/IAdminSyncService/SyncGroupUsersInvalidCompanyFaultFault", Name = "InvalidCompanyFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[FaultContract(typeof(InvalidContractFault), Action = "http://tempuri.org/IAdminSyncService/SyncGroupUsersInvalidContractFaultFault", Name = "InvalidContractFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[FaultContract(typeof(InternalFault), Action = "http://tempuri.org/IAdminSyncService/SyncGroupUsersInternalFaultFault", Name = "InternalFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[OperationContract(Action = "http://tempuri.org/IAdminSyncService/SyncGroupUsers", ReplyAction = "http://tempuri.org/IAdminSyncService/SyncGroupUsersResponse")]
	Dictionary<string, ErrorInfo> SyncGroupUsers(int companyId, Guid groupId, string[] users);

	[OperationContract(Action = "http://tempuri.org/IAdminSyncService/RemoveGroups", ReplyAction = "http://tempuri.org/IAdminSyncService/RemoveGroupsResponse")]
	[FaultContract(typeof(InvalidContractFault), Action = "http://tempuri.org/IAdminSyncService/RemoveGroupsInvalidContractFaultFault", Name = "InvalidContractFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[FaultContract(typeof(InternalFault), Action = "http://tempuri.org/IAdminSyncService/RemoveGroupsInternalFaultFault", Name = "InternalFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	Dictionary<Guid, RemoveGroupErrorInfo> RemoveGroups(Guid[] groupIds);

	[FaultContract(typeof(InvalidContractFault), Action = "http://tempuri.org/IAdminSyncService/GetGroupMembersInvalidContractFaultFault", Name = "InvalidContractFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[OperationContract(Action = "http://tempuri.org/IAdminSyncService/GetGroupMembers", ReplyAction = "http://tempuri.org/IAdminSyncService/GetGroupMembersResponse")]
	[FaultContract(typeof(AuthorizationFault), Action = "http://tempuri.org/IAdminSyncService/GetGroupMembersAuthorizationFaultFault", Name = "AuthorizationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[FaultContract(typeof(InternalFault), Action = "http://tempuri.org/IAdminSyncService/GetGroupMembersInternalFaultFault", Name = "InternalFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[FaultContract(typeof(InvalidGroupFault), Action = "http://tempuri.org/IAdminSyncService/GetGroupMembersInvalidGroupFaultFault", Name = "InvalidGroupFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	string[] GetGroupMembers(Guid groupId);

	[OperationContract(Action = "http://tempuri.org/IAdminSyncService/GetAdminAccounts", ReplyAction = "http://tempuri.org/IAdminSyncService/GetAdminAccountsResponse")]
	[FaultContract(typeof(InvalidCompanyFault), Action = "http://tempuri.org/IAdminSyncService/GetAdminAccountsInvalidCompanyFaultFault", Name = "InvalidCompanyFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[FaultContract(typeof(InvalidContractFault), Action = "http://tempuri.org/IAdminSyncService/GetAdminAccountsInvalidContractFaultFault", Name = "InvalidContractFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[FaultContract(typeof(AuthorizationFault), Action = "http://tempuri.org/IAdminSyncService/GetAdminAccountsAuthorizationFaultFault", Name = "AuthorizationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[FaultContract(typeof(InternalFault), Action = "http://tempuri.org/IAdminSyncService/GetAdminAccountsInternalFaultFault", Name = "InternalFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	CompanyAdministrators GetAdminAccounts(int companyId);

	[FaultContract(typeof(AuthorizationFault), Action = "http://tempuri.org/IAdminSyncService/GetUserPermissionsAuthorizationFaultFault", Name = "AuthorizationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[FaultContract(typeof(InvalidUserFault), Action = "http://tempuri.org/IAdminSyncService/GetUserPermissionsInvalidUserFaultFault", Name = "InvalidUserFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[OperationContract(Action = "http://tempuri.org/IAdminSyncService/GetUserPermissions", ReplyAction = "http://tempuri.org/IAdminSyncService/GetUserPermissionsResponse")]
	[FaultContract(typeof(InternalFault), Action = "http://tempuri.org/IAdminSyncService/GetUserPermissionsInternalFaultFault", Name = "InternalFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[FaultContract(typeof(InvalidContractFault), Action = "http://tempuri.org/IAdminSyncService/GetUserPermissionsInvalidContractFaultFault", Name = "InvalidContractFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	Dictionary<int, Role[]> GetUserPermissions(string userEmailAddress);
}

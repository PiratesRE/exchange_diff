using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync;

[GeneratedCode("System.ServiceModel", "3.0.0.0")]
[DebuggerStepThrough]
internal class AdminSyncServiceClient : ClientBase<IAdminSyncService>, IAdminSyncService
{
	public AdminSyncServiceClient()
	{
	}

	public AdminSyncServiceClient(string endpointConfigurationName) : base(endpointConfigurationName)
	{
	}

	public AdminSyncServiceClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public AdminSyncServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public AdminSyncServiceClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
	{
	}

	public void Ping()
	{
		base.Channel.Ping();
	}

	public FailedAdminAccounts SyncAdminAccounts(CompanyAdministrators companyAdministrators)
	{
		return base.Channel.SyncAdminAccounts(companyAdministrators);
	}

	public Dictionary<string, ErrorInfo> SyncGroupUsers(int companyId, Guid groupId, string[] users)
	{
		return base.Channel.SyncGroupUsers(companyId, groupId, users);
	}

	public Dictionary<Guid, RemoveGroupErrorInfo> RemoveGroups(Guid[] groupIds)
	{
		return base.Channel.RemoveGroups(groupIds);
	}

	public string[] GetGroupMembers(Guid groupId)
	{
		return base.Channel.GetGroupMembers(groupId);
	}

	public CompanyAdministrators GetAdminAccounts(int companyId)
	{
		return base.Channel.GetAdminAccounts(companyId);
	}

	public Dictionary<int, Role[]> GetUserPermissions(string userEmailAddress)
	{
		return base.Channel.GetUserPermissions(userEmailAddress);
	}
}

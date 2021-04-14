using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Online.Administration.WebService;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
public class ProvisioningWebServiceClient : ClientBase<IProvisioningWebService>, IProvisioningWebService
{
	public ProvisioningWebServiceClient()
	{
	}

	public ProvisioningWebServiceClient(string endpointConfigurationName) : base(endpointConfigurationName)
	{
	}

	public ProvisioningWebServiceClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public ProvisioningWebServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public ProvisioningWebServiceClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
	{
	}

	public Response RemoveServicePrincipalBySpn(RemoveServicePrincipalBySpnRequest request)
	{
		return base.Channel.RemoveServicePrincipalBySpn(request);
	}

	public GetServicePrincipalResponse GetServicePrincipal(GetServicePrincipalRequest request)
	{
		return base.Channel.GetServicePrincipal(request);
	}

	public GetServicePrincipalByAppPrincipalIdResponse GetServicePrincipalByAppPrincipalId(GetServicePrincipalByAppPrincipalIdRequest request)
	{
		return base.Channel.GetServicePrincipalByAppPrincipalId(request);
	}

	public GetServicePrincipalBySpnResponse GetServicePrincipalBySpn(GetServicePrincipalBySpnRequest request)
	{
		return base.Channel.GetServicePrincipalBySpn(request);
	}

	public Response RemoveServicePrincipalCredentials(RemoveServicePrincipalCredentialsRequest request)
	{
		return base.Channel.RemoveServicePrincipalCredentials(request);
	}

	public Response RemoveServicePrincipalCredentialsBySpn(RemoveServicePrincipalCredentialsBySpnRequest request)
	{
		return base.Channel.RemoveServicePrincipalCredentialsBySpn(request);
	}

	public Response RemoveServicePrincipalCredentialsByAppPrincipalId(RemoveServicePrincipalCredentialsByAppPrincipalIdRequest request)
	{
		return base.Channel.RemoveServicePrincipalCredentialsByAppPrincipalId(request);
	}

	public Response SetServicePrincipal(SetServicePrincipalRequest request)
	{
		return base.Channel.SetServicePrincipal(request);
	}

	public Response SetServicePrincipalByAppPrincipalId(SetServicePrincipalByAppPrincipalIdRequest request)
	{
		return base.Channel.SetServicePrincipalByAppPrincipalId(request);
	}

	public Response SetServicePrincipalBySpn(SetServicePrincipalBySpnRequest request)
	{
		return base.Channel.SetServicePrincipalBySpn(request);
	}

	public Response SetServicePrincipalName(SetServicePrincipalNameRequest request)
	{
		return base.Channel.SetServicePrincipalName(request);
	}

	public ListServicePrincipalsResponse ListServicePrincipals(ListServicePrincipalsRequest request)
	{
		return base.Channel.ListServicePrincipals(request);
	}

	public NavigateListServicePrincipalsResponse NavigateListServicePrincipals(NavigateListServicePrincipalsRequest request)
	{
		return base.Channel.NavigateListServicePrincipals(request);
	}

	public ListServicePrincipalCredentialsResponse ListServicePrincipalCredentials(ListServicePrincipalCredentialsRequest request)
	{
		return base.Channel.ListServicePrincipalCredentials(request);
	}

	public ListServicePrincipalCredentialsByAppPrincipalIdResponse ListServicePrincipalCredentialsByAppPrincipalId(ListServicePrincipalCredentialsByAppPrincipalIdRequest request)
	{
		return base.Channel.ListServicePrincipalCredentialsByAppPrincipalId(request);
	}

	public ListServicePrincipalCredentialsBySpnResponse ListServicePrincipalCredentialsBySpn(ListServicePrincipalCredentialsBySpnRequest request)
	{
		return base.Channel.ListServicePrincipalCredentialsBySpn(request);
	}

	public NavigateGroupMemberResultsResponse NavigateGroupMemberResults(NavigateGroupMemberResultsRequest request)
	{
		return base.Channel.NavigateGroupMemberResults(request);
	}

	public GetRoleResponse GetRole(GetRoleRequest request)
	{
		return base.Channel.GetRole(request);
	}

	public GetRoleByNameResponse GetRoleByName(GetRoleByNameRequest request)
	{
		return base.Channel.GetRoleByName(request);
	}

	public ListRolesResponse ListRoles(Request request)
	{
		return base.Channel.ListRoles(request);
	}

	public ListRolesForUserResponse ListRolesForUser(ListRolesForUserRequest request)
	{
		return base.Channel.ListRolesForUser(request);
	}

	public ListRolesForUserByUpnResponse ListRolesForUserByUpn(ListRolesForUserByUpnRequest request)
	{
		return base.Channel.ListRolesForUserByUpn(request);
	}

	public Response AddRoleMembers(AddRoleMembersRequest request)
	{
		return base.Channel.AddRoleMembers(request);
	}

	public Response AddRoleMembersByRoleName(AddRoleMembersByRoleNameRequest request)
	{
		return base.Channel.AddRoleMembersByRoleName(request);
	}

	public Response RemoveRoleMembers(RemoveRoleMembersRequest request)
	{
		return base.Channel.RemoveRoleMembers(request);
	}

	public Response RemoveRoleMembersByRoleName(RemoveRoleMembersByRoleNameRequest request)
	{
		return base.Channel.RemoveRoleMembersByRoleName(request);
	}

	public ListRoleMembersResponse ListRoleMembers(ListRoleMembersRequest request)
	{
		return base.Channel.ListRoleMembers(request);
	}

	public NavigateRoleMemberResultsResponse NavigateRoleMemberResults(NavigateRoleMemberResultsRequest request)
	{
		return base.Channel.NavigateRoleMemberResults(request);
	}

	public AddUserResponse AddUser(AddUserRequest request)
	{
		return base.Channel.AddUser(request);
	}

	public Response RemoveUser(RemoveUserRequest request)
	{
		return base.Channel.RemoveUser(request);
	}

	public Response RemoveUserByUpn(RemoveUserByUpnRequest request)
	{
		return base.Channel.RemoveUserByUpn(request);
	}

	public Response SetUser(SetUserRequest request)
	{
		return base.Channel.SetUser(request);
	}

	public ChangeUserPrincipalNameResponse ChangeUserPrincipalName(ChangeUserPrincipalNameRequest request)
	{
		return base.Channel.ChangeUserPrincipalName(request);
	}

	public ChangeUserPrincipalNameByUpnResponse ChangeUserPrincipalNameByUpn(ChangeUserPrincipalNameByUpnRequest request)
	{
		return base.Channel.ChangeUserPrincipalNameByUpn(request);
	}

	public ResetUserPasswordResponse ResetUserPassword(ResetUserPasswordRequest request)
	{
		return base.Channel.ResetUserPassword(request);
	}

	public ResetUserPasswordByUpnResponse ResetUserPasswordByUpn(ResetUserPasswordByUpnRequest request)
	{
		return base.Channel.ResetUserPasswordByUpn(request);
	}

	public GetUserResponse GetUser(GetUserRequest request)
	{
		return base.Channel.GetUser(request);
	}

	public GetUserByUpnResponse GetUserByUpn(GetUserByUpnRequest request)
	{
		return base.Channel.GetUserByUpn(request);
	}

	public GetUserByLiveIdResponse GetUserByLiveId(GetUserByLiveIdRequest request)
	{
		return base.Channel.GetUserByLiveId(request);
	}

	public ListUsersResponse ListUsers(ListUsersRequest request)
	{
		return base.Channel.ListUsers(request);
	}

	public NavigateUserResultsResponse NavigateUserResults(NavigateUserResultsRequest request)
	{
		return base.Channel.NavigateUserResults(request);
	}

	public Response SetUserLicenses(SetUserLicensesRequest request)
	{
		return base.Channel.SetUserLicenses(request);
	}

	public Response SetUserLicensesByUpn(SetUserLicensesByUpnRequest request)
	{
		return base.Channel.SetUserLicensesByUpn(request);
	}

	public ConvertFederatedUserToManagedResponse ConvertFederatedUserToManaged(ConvertFederatedUserToManagedRequest request)
	{
		return base.Channel.ConvertFederatedUserToManaged(request);
	}

	public RestoreUserResponse RestoreUser(RestoreUserRequest request)
	{
		return base.Channel.RestoreUser(request);
	}

	public RestoreUserByUpnResponse RestoreUserByUpn(RestoreUserByUpnRequest request)
	{
		return base.Channel.RestoreUserByUpn(request);
	}

	public AddServicePrincipalResponse AddServicePrincipal(AddServicePrincipalRequest request)
	{
		return base.Channel.AddServicePrincipal(request);
	}

	public Response AddServicePrincipalCredentials(AddServicePrincipalCredentialsRequest request)
	{
		return base.Channel.AddServicePrincipalCredentials(request);
	}

	public Response AddServicePrincipalCredentialsBySpn(AddServicePrincipalCredentialsBySpnRequest request)
	{
		return base.Channel.AddServicePrincipalCredentialsBySpn(request);
	}

	public Response AddServicePrincipalCredentialsByAppPrincipalId(AddServicePrincipalCredentialsByAppPrincipalIdRequest request)
	{
		return base.Channel.AddServicePrincipalCredentialsByAppPrincipalId(request);
	}

	public Response RemoveServicePrincipal(RemoveServicePrincipalRequest request)
	{
		return base.Channel.RemoveServicePrincipal(request);
	}

	public Response RemoveServicePrincipalByAppPrincipalId(RemoveServicePrincipalByAppPrincipalIdRequest request)
	{
		return base.Channel.RemoveServicePrincipalByAppPrincipalId(request);
	}

	public GetHeaderInfoResponse GetHeaderInfo(Request request)
	{
		return base.Channel.GetHeaderInfo(request);
	}

	public MsolConnectResponse MsolConnect(Request request)
	{
		return base.Channel.MsolConnect(request);
	}

	public GetPartnerInformationResponse GetPartnerInformation(Request request)
	{
		return base.Channel.GetPartnerInformation(request);
	}

	public GetCompanyInformationResponse GetCompanyInformation(Request request)
	{
		return base.Channel.GetCompanyInformation(request);
	}

	public GetSubscriptionResponse GetSubscription(GetSubscriptionRequest request)
	{
		return base.Channel.GetSubscription(request);
	}

	public ListSubscriptionsResponse ListSubscriptions(Request request)
	{
		return base.Channel.ListSubscriptions(request);
	}

	public ListAccountSkusResponse ListAccountSkus(ListAccountSkusRequest request)
	{
		return base.Channel.ListAccountSkus(request);
	}

	public Response SetPartnerInformation(SetPartnerInformationRequest request)
	{
		return base.Channel.SetPartnerInformation(request);
	}

	public Response SetCompanyContactInformation(SetCompanyContactInformationRequest request)
	{
		return base.Channel.SetCompanyContactInformation(request);
	}

	public Response SetCompanyDirSyncEnabled(SetCompanyDirSyncEnabledRequest request)
	{
		return base.Channel.SetCompanyDirSyncEnabled(request);
	}

	public Response SetCompanySettings(SetCompanySettingsRequest request)
	{
		return base.Channel.SetCompanySettings(request);
	}

	public ListPartnerContractsResponse ListPartnerContracts(ListPartnerContractsRequest request)
	{
		return base.Channel.ListPartnerContracts(request);
	}

	public NavigatePartnerContractsResponse NavigatePartnerContracts(NavigatePartnerContractsRequest request)
	{
		return base.Channel.NavigatePartnerContracts(request);
	}

	public Response RemoveContact(RemoveContactRequest request)
	{
		return base.Channel.RemoveContact(request);
	}

	public GetContactResponse GetContact(GetContactRequest request)
	{
		return base.Channel.GetContact(request);
	}

	public ListContactsResponse ListContacts(ListContactsRequest request)
	{
		return base.Channel.ListContacts(request);
	}

	public NavigateContactResultsResponse NavigateContactResults(NavigateContactResultsRequest request)
	{
		return base.Channel.NavigateContactResults(request);
	}

	public AddDomainResponse AddDomain(AddDomainRequest request)
	{
		return base.Channel.AddDomain(request);
	}

	public Response VerifyDomain(VerifyDomainRequest request)
	{
		return base.Channel.VerifyDomain(request);
	}

	public Response RemoveDomain(RemoveDomainRequest request)
	{
		return base.Channel.RemoveDomain(request);
	}

	public Response SetDomain(SetDomainRequest request)
	{
		return base.Channel.SetDomain(request);
	}

	public Response SetDomainAuthentication(SetDomainAuthenticationRequest request)
	{
		return base.Channel.SetDomainAuthentication(request);
	}

	public GetDomainFederationSettingsResponse GetDomainFederationSettings(GetDomainFederationSettingsRequest request)
	{
		return base.Channel.GetDomainFederationSettings(request);
	}

	public Response SetDomainFederationSettings(SetDomainFederationSettingsRequest request)
	{
		return base.Channel.SetDomainFederationSettings(request);
	}

	public GetDomainResponse GetDomain(GetDomainRequest request)
	{
		return base.Channel.GetDomain(request);
	}

	public ListDomainsResponse ListDomains(ListDomainsRequest request)
	{
		return base.Channel.ListDomains(request);
	}

	public GetDomainVerificationDnsResponse GetDomainVerificationDns(GetDomainVerificationDnsRequest request)
	{
		return base.Channel.GetDomainVerificationDns(request);
	}

	public AddGroupResponse AddGroup(AddGroupRequest request)
	{
		return base.Channel.AddGroup(request);
	}

	public Response RemoveGroup(RemoveGroupRequest request)
	{
		return base.Channel.RemoveGroup(request);
	}

	public Response SetGroup(SetGroupRequest request)
	{
		return base.Channel.SetGroup(request);
	}

	public GetGroupResponse GetGroup(GetGroupRequest request)
	{
		return base.Channel.GetGroup(request);
	}

	public ListGroupsResponse ListGroups(ListGroupsRequest request)
	{
		return base.Channel.ListGroups(request);
	}

	public NavigateGroupResultsResponse NavigateGroupResults(NavigateGroupResultsRequest request)
	{
		return base.Channel.NavigateGroupResults(request);
	}

	public Response AddGroupMembers(AddGroupMembersRequest request)
	{
		return base.Channel.AddGroupMembers(request);
	}

	public Response RemoveGroupMembers(RemoveGroupMembersRequest request)
	{
		return base.Channel.RemoveGroupMembers(request);
	}

	public ListGroupMembersResponse ListGroupMembers(ListGroupMembersRequest request)
	{
		return base.Channel.ListGroupMembers(request);
	}
}

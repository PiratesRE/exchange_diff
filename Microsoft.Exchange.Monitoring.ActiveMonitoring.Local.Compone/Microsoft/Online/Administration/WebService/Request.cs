using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[KnownType(typeof(ListServicePrincipalCredentialsRequest))]
	[KnownType(typeof(SetDomainRequest))]
	[KnownType(typeof(RemoveDomainRequest))]
	[KnownType(typeof(VerifyDomainRequest))]
	[KnownType(typeof(ListContactsRequest))]
	[KnownType(typeof(RemoveContactRequest))]
	[KnownType(typeof(GetContactRequest))]
	[KnownType(typeof(AddServicePrincipalCredentialsByAppPrincipalIdRequest))]
	[KnownType(typeof(RemoveServicePrincipalRequest))]
	[KnownType(typeof(RemoveServicePrincipalBySpnRequest))]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "Request", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[KnownType(typeof(GetServicePrincipalRequest))]
	[KnownType(typeof(GetServicePrincipalByAppPrincipalIdRequest))]
	[KnownType(typeof(GetServicePrincipalBySpnRequest))]
	[KnownType(typeof(RemoveServicePrincipalCredentialsRequest))]
	[KnownType(typeof(RemoveServicePrincipalCredentialsBySpnRequest))]
	[KnownType(typeof(RemoveServicePrincipalCredentialsByAppPrincipalIdRequest))]
	[KnownType(typeof(SetServicePrincipalRequest))]
	[KnownType(typeof(SetServicePrincipalByAppPrincipalIdRequest))]
	[KnownType(typeof(SetServicePrincipalBySpnRequest))]
	[KnownType(typeof(SetServicePrincipalNameRequest))]
	[KnownType(typeof(ListServicePrincipalsRequest))]
	[KnownType(typeof(NavigateListServicePrincipalsRequest))]
	[KnownType(typeof(ListServicePrincipalCredentialsByAppPrincipalIdRequest))]
	[KnownType(typeof(ListServicePrincipalCredentialsBySpnRequest))]
	[KnownType(typeof(NavigateGroupMemberResultsRequest))]
	[KnownType(typeof(GetRoleRequest))]
	[KnownType(typeof(GetRoleByNameRequest))]
	[KnownType(typeof(ListRolesForUserRequest))]
	[KnownType(typeof(ListRolesForUserByUpnRequest))]
	[KnownType(typeof(AddRoleMembersRequest))]
	[KnownType(typeof(AddRoleMembersByRoleNameRequest))]
	[KnownType(typeof(RemoveRoleMembersRequest))]
	[KnownType(typeof(RemoveRoleMembersByRoleNameRequest))]
	[KnownType(typeof(ListRoleMembersRequest))]
	[KnownType(typeof(NavigateRoleMemberResultsRequest))]
	[KnownType(typeof(AddUserRequest))]
	[KnownType(typeof(RemoveUserRequest))]
	[KnownType(typeof(RemoveUserByUpnRequest))]
	[KnownType(typeof(SetUserRequest))]
	[KnownType(typeof(ChangeUserPrincipalNameRequest))]
	[KnownType(typeof(ChangeUserPrincipalNameByUpnRequest))]
	[KnownType(typeof(ResetUserPasswordRequest))]
	[KnownType(typeof(ResetUserPasswordByUpnRequest))]
	[KnownType(typeof(GetUserRequest))]
	[KnownType(typeof(GetUserByUpnRequest))]
	[KnownType(typeof(GetUserByLiveIdRequest))]
	[KnownType(typeof(ListUsersRequest))]
	[KnownType(typeof(NavigateUserResultsRequest))]
	[KnownType(typeof(SetUserLicensesRequest))]
	[KnownType(typeof(SetUserLicensesByUpnRequest))]
	[KnownType(typeof(ConvertFederatedUserToManagedRequest))]
	[KnownType(typeof(RestoreUserRequest))]
	[KnownType(typeof(RestoreUserByUpnRequest))]
	[KnownType(typeof(AddServicePrincipalRequest))]
	[KnownType(typeof(AddServicePrincipalCredentialsRequest))]
	[KnownType(typeof(AddServicePrincipalCredentialsBySpnRequest))]
	[KnownType(typeof(RemoveServicePrincipalByAppPrincipalIdRequest))]
	[KnownType(typeof(GetSubscriptionRequest))]
	[KnownType(typeof(ListAccountSkusRequest))]
	[KnownType(typeof(SetPartnerInformationRequest))]
	[KnownType(typeof(SetCompanyContactInformationRequest))]
	[KnownType(typeof(SetCompanyDirSyncEnabledRequest))]
	[KnownType(typeof(SetCompanySettingsRequest))]
	[KnownType(typeof(ListPartnerContractsRequest))]
	[KnownType(typeof(NavigatePartnerContractsRequest))]
	[KnownType(typeof(NavigateContactResultsRequest))]
	[KnownType(typeof(AddDomainRequest))]
	[KnownType(typeof(SetDomainAuthenticationRequest))]
	[KnownType(typeof(GetDomainFederationSettingsRequest))]
	[KnownType(typeof(SetDomainFederationSettingsRequest))]
	[KnownType(typeof(GetDomainRequest))]
	[KnownType(typeof(ListDomainsRequest))]
	[KnownType(typeof(GetDomainVerificationDnsRequest))]
	[KnownType(typeof(AddGroupRequest))]
	[KnownType(typeof(RemoveGroupRequest))]
	[KnownType(typeof(SetGroupRequest))]
	[KnownType(typeof(GetGroupRequest))]
	[KnownType(typeof(ListGroupsRequest))]
	[KnownType(typeof(NavigateGroupResultsRequest))]
	[KnownType(typeof(AddGroupMembersRequest))]
	[KnownType(typeof(RemoveGroupMembersRequest))]
	[KnownType(typeof(ListGroupMembersRequest))]
	public class Request : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

		[DataMember]
		public Version BecVersion
		{
			get
			{
				return this.BecVersionField;
			}
			set
			{
				this.BecVersionField = value;
			}
		}

		[DataMember]
		public Guid? TenantId
		{
			get
			{
				return this.TenantIdField;
			}
			set
			{
				this.TenantIdField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Version BecVersionField;

		private Guid? TenantIdField;
	}
}

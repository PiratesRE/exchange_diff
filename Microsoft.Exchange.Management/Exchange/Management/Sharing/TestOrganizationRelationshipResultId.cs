using System;

namespace Microsoft.Exchange.Management.Sharing
{
	public enum TestOrganizationRelationshipResultId
	{
		Unknown,
		FailureToGetDelegationToken,
		AutodiscoverUrlsDiffer,
		AutodiscoverServiceCallFailed,
		NoOrganizationRelationshipInstancesWereReturnedByTheRemoteParty,
		MultipleOrganizationRelationshipInstancesReturnedByTheRemoteParty,
		VerificationOfRemoteOrganizationRelationshipFailed,
		ApplicationUrisDiffer,
		AccessMismatchLocalRemote,
		CouldNotParseRemoteValue,
		AccessMismatchRemoteLocal,
		PropertiesDiffer,
		NoDomainInTheRemoteOrganizationRelationshipIsFederatedLocally,
		LocalFederatedDomainsAreMissingFromTheRemoteOrganizationRelationsipDomains,
		MismatchedSTS,
		NoLocalDomainIsFederatedRemotely,
		TargetSharingEprDoesNotMatchAnyExternalURI,
		RemoteOrgRelationshipHasNoDomainsDefined,
		UserFedDomainNotInRemoteOrgRelationship,
		UserFedDomainInMultipleRemoteOrgRelationship,
		NoRemoteFederatedDomainInLocalOrgRelationship,
		FederatedOrganizationIdNotEnabled,
		ApplicationUriMissing,
		MismatchedFederation,
		UnknownFederationDomainAuthenticationType,
		FederatedIdentityTypeMismatch,
		RequiredIdentityInformationNotSet,
		UserFederatedDomainDoesNotMatchAccountNamespace,
		UserFederatedIdentityIsNotFederatedDomain,
		AutoDiscoverNotSetInOrgRelationship
	}
}

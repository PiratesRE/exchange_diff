using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[KnownType(typeof(InvalidContextException))]
	[KnownType(typeof(ObjectNotFoundException))]
	[KnownType(typeof(DomainUnexpectedAuthenticationException))]
	[KnownType(typeof(DomainCapabilitySetException))]
	[KnownType(typeof(DomainLiveNamespaceExistsException))]
	[KnownType(typeof(DomainNotRootException))]
	[KnownType(typeof(DomainLiveNamespaceAuthenticationException))]
	[KnownType(typeof(DomainLiveNamespaceUriConflictException))]
	[KnownType(typeof(DomainNameForbiddenWordException))]
	[KnownType(typeof(DomainPendingDeletionException))]
	[KnownType(typeof(DomainVerificationMissingCnameException))]
	[KnownType(typeof(DomainVerificationWrongCnameTargetException))]
	[KnownType(typeof(DomainVerificationMissingDnsRecordException))]
	[KnownType(typeof(DomainLiveNamespaceUnmanagedException))]
	[KnownType(typeof(DomainCapabilityUnsetException))]
	[KnownType(typeof(DefaultDomainDeletionException))]
	[KnownType(typeof(InitialDomainDeletionException))]
	[KnownType(typeof(DomainHasChildDomainException))]
	[KnownType(typeof(DomainNotEmptyException))]
	[KnownType(typeof(SetUnverifiedDomainAsDefaultException))]
	[KnownType(typeof(DefaultDomainUnsetException))]
	[KnownType(typeof(DefaultDomainInvalidAuthenticationException))]
	[KnownType(typeof(InitialDomainUpdateException))]
	[KnownType(typeof(DomainCapabilityUnavailableException))]
	[KnownType(typeof(DomainOverlappingOperationException))]
	[KnownType(typeof(BindingRedirectionException))]
	[KnownType(typeof(ThrottlingException))]
	[KnownType(typeof(ClientVersionException))]
	[KnownType(typeof(LiveTokenExpiredException))]
	[KnownType(typeof(InvalidHeaderException))]
	[KnownType(typeof(InvalidParameterException))]
	[KnownType(typeof(InvalidLicenseConfigurationException))]
	[KnownType(typeof(InvalidUserLicenseOptionException))]
	[KnownType(typeof(InvalidUserLicenseException))]
	[KnownType(typeof(InvalidSubscriptionStatusException))]
	[KnownType(typeof(LicenseQuotaExceededException))]
	[KnownType(typeof(TenantNotPartnerTypeException))]
	[KnownType(typeof(PropertyValidationException))]
	[KnownType(typeof(StringLengthValidationException))]
	[KnownType(typeof(StringSyntaxValidationException))]
	[KnownType(typeof(InvalidPasswordException))]
	[KnownType(typeof(InvalidPasswordWeakException))]
	[KnownType(typeof(InvalidPasswordContainMemberNameException))]
	[KnownType(typeof(IncorrectPasswordException))]
	[KnownType(typeof(ServicePrincipalCredentialNotSettableException))]
	[KnownType(typeof(ItemCountValidationException))]
	[KnownType(typeof(TooManySearchResultsException))]
	[KnownType(typeof(TooManyUnverifiedDomainException))]
	[KnownType(typeof(PropertyNotSettableException))]
	[KnownType(typeof(PropertyDomainValidationException))]
	[KnownType(typeof(UniquenessValidationException))]
	[KnownType(typeof(ServiceUnavailableException))]
	[KnownType(typeof(InternalServiceException))]
	[KnownType(typeof(DirectoryInternalServiceException))]
	[KnownType(typeof(IdentityInternalServiceException))]
	[KnownType(typeof(UserAccountDisabledException))]
	[KnownType(typeof(RequiredPropertyNotSetException))]
	[KnownType(typeof(QuotaExceededException))]
	[KnownType(typeof(UserAuthenticationUnchangedException))]
	[KnownType(typeof(MailEnabledGroupsNotSupportedException))]
	[KnownType(typeof(GroupDeletionNotAllowedException))]
	[KnownType(typeof(GroupUpdateNotAllowedException))]
	[KnownType(typeof(AccessDeniedException))]
	[KnownType(typeof(DomainDataOperationException))]
	[KnownType(typeof(ServicePrincipalNotFoundException))]
	[KnownType(typeof(UserNotFoundException))]
	[KnownType(typeof(CompanyNotFoundException))]
	[KnownType(typeof(RoleNotFoundException))]
	[KnownType(typeof(RoleMemberNotFoundException))]
	[KnownType(typeof(SubscriptionNotFoundException))]
	[KnownType(typeof(DomainNotFoundException))]
	[KnownType(typeof(ContractNotFoundException))]
	[KnownType(typeof(ContactNotFoundException))]
	[KnownType(typeof(GroupNotFoundException))]
	[KnownType(typeof(GroupMemberNotFoundException))]
	[KnownType(typeof(PageNotAvailableException))]
	[KnownType(typeof(InvalidListContextException))]
	[KnownType(typeof(ObjectAlreadyExistsException))]
	[KnownType(typeof(MemberAlreadyExistsException))]
	[KnownType(typeof(UserAlreadyExistsException))]
	[KnownType(typeof(DomainAlreadyExistsException))]
	[KnownType(typeof(DomainAlreadyExistsInOldSystemException))]
	[KnownType(typeof(DomainNameConflictException))]
	[KnownType(typeof(GroupAlreadyExistsException))]
	[KnownType(typeof(RemoveSelfFromRoleException))]
	[KnownType(typeof(RemoveDirSyncObjectNotAllowedException))]
	[KnownType(typeof(UserRemoveSelfException))]
	[KnownType(typeof(UserConflictAuthenticationException))]
	[KnownType(typeof(RestoreUserLicenseErrorException))]
	[KnownType(typeof(RestoreUserErrorException))]
	[KnownType(typeof(RestoreUserNotAllowedException))]
	[KnownType(typeof(DirSyncStatusChangeNotAllowedException))]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "MsolAdministrationException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[KnownType(typeof(DataOperationException))]
	public class MsolAdministrationException : IExtensibleDataObject
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
		public string HelpLink
		{
			get
			{
				return this.HelpLinkField;
			}
			set
			{
				this.HelpLinkField = value;
			}
		}

		[DataMember]
		public string Message
		{
			get
			{
				return this.MessageField;
			}
			set
			{
				this.MessageField = value;
			}
		}

		[DataMember]
		public Guid? OperationId
		{
			get
			{
				return this.OperationIdField;
			}
			set
			{
				this.OperationIdField = value;
			}
		}

		[DataMember]
		public string Source
		{
			get
			{
				return this.SourceField;
			}
			set
			{
				this.SourceField = value;
			}
		}

		[DataMember]
		public string StackTrace
		{
			get
			{
				return this.StackTraceField;
			}
			set
			{
				this.StackTraceField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string HelpLinkField;

		private string MessageField;

		private Guid? OperationIdField;

		private string SourceField;

		private string StackTraceField;
	}
}

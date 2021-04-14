using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[DataContract(Name = "FaultId", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	internal enum FaultId
	{
		[EnumMember]
		InvalidParameterSpecified = 1,
		[EnumMember]
		NullOrEmptyParameterSpecified,
		[EnumMember]
		CompanyExistsUnderThisReseller = 100,
		[EnumMember]
		CompanyExistsOutsideThisReseller,
		[EnumMember]
		CompanyDoesNotExist,
		[EnumMember]
		ParentCompanySpecifiedDoesNotExist,
		[EnumMember]
		CompanyIsDisabled,
		[EnumMember]
		CannotDisableCompany,
		[EnumMember]
		ParentCompanyIdNotSpecified,
		[EnumMember]
		InvalidCompanyIdSpecified,
		[EnumMember]
		CanNotCreateResellerCompany,
		[EnumMember]
		CanNotDeleteEnabledCompany,
		[EnumMember]
		NullOrEmptyCompanyNameSpecified,
		[EnumMember]
		NoParentCompany,
		[EnumMember]
		CompanyConfigurationNotFound,
		[EnumMember]
		InvalidCompanyGuidSpecified,
		[EnumMember]
		CompanyGuidIsNotUnique,
		[EnumMember]
		CompanyNameShouldOnlyBeAscii,
		[EnumMember]
		DomainExistUnderThisCompany = 200,
		[EnumMember]
		DomainExistOutsideThisCompany,
		[EnumMember]
		DomainDoesNotExist,
		[EnumMember]
		DomainIsDisabled,
		[EnumMember]
		CannotDisabledDomain,
		[EnumMember]
		DomainNameValidationFailed,
		[EnumMember]
		NullOrEmptyDomainNameSpecified,
		[EnumMember]
		ShouldNotEnableNonValidatedDomain,
		[EnumMember]
		CanNotAddDomainToReseller,
		[EnumMember]
		CanNotDeleteEnabledDomain,
		[EnumMember]
		DomainGuidIsNotUnique,
		[EnumMember]
		SmtpProfileAndMailServerCanNotBeSpecifiedTogether,
		[EnumMember]
		SmtpProfileWithTheSameNameExists = 300,
		[EnumMember]
		SpecifiedSmtpProfileDoesNotExist,
		[EnumMember]
		AccessDenied,
		[EnumMember]
		FailedToAddIP,
		[EnumMember]
		FailedToRemoveIP,
		[EnumMember]
		IPValidationFailed,
		[EnumMember]
		SmtpProfileNameTooLong,
		[EnumMember]
		InvalidSmtpProfileName,
		[EnumMember]
		InvalidSmtpProfile,
		[EnumMember]
		InvalidIPList,
		[EnumMember]
		IpAlreadyInOurSystem,
		[EnumMember]
		InboundIPOrSmtpProfileShouldBeSpecified,
		[EnumMember]
		FailedToCreateCompany = 400,
		[EnumMember]
		FailedToCreateDomain,
		[EnumMember]
		UnknownError,
		[EnumMember]
		InvalidOperation,
		[EnumMember]
		InvalidTargetActionSpecified,
		[EnumMember]
		CannotApplySettingAtResellerLevel,
		[EnumMember]
		CannotApplySettingAtCompanyLevel,
		[EnumMember]
		SubscriptionNotSetupAtResellerLevel,
		[EnumMember]
		FailedToSaveSubscription,
		[EnumMember]
		BatchSizeExceededLimit,
		[EnumMember]
		UnableToConnectToDatabase,
		[EnumMember]
		ConnectorDoesNotExists,
		[EnumMember]
		ConnectorNotInResellerScope
	}
}

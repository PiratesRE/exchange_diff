using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal interface IClassificationDefinitionsDiagnosticsReporter
	{
		void WriteCorruptRulePackageDiagnosticsInformation(int traceSourceHashCode, OrganizationId currentOrganizationId, string offendingRulePackageObjectDn, Exception underlyingException);

		void WriteDuplicateRuleIdAcrossRulePacksDiagnosticsInformation(int traceSourceHashCode, OrganizationId currentOrganizationId, string offendingRulePackageObjectDn1, string offendingRulePackageObjectDn2, string duplicateRuleId);

		void WriteClassificationEngineConfigurationErrorInformation(int traceSourceHashCode, Exception underlyingException);

		void WriteClassificationEngineUnexpectedFailureInValidation(int traceSourceHashCode, OrganizationId currentOrganizationId, int engineErrorCode);

		void WriteClassificationEngineTimeoutInValidation(int traceSourceHashCode, OrganizationId currentOrganizationId, int validationTimeout);

		void WriteInvalidObjectInformation(int traceSourceHashCode, OrganizationId currentOrganizationId, string offendingRulePackageObjectDn);
	}
}

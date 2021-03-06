using System;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	public enum ClassificationEngineResults
	{
		E_CE_NOT_INITIALIZED = -2147220992,
		E_CE_INVALID_OOB_RULE_CONFIGURATION,
		E_CE_ENGINE_FAILURE = -2147220989,
		E_CE_UNABLE_TO_CLASSIFY,
		E_CE_RULE_RETRIEVAL_FAILED = -2147220986,
		E_CE_RULE_PACKAGE_NOT_FOUND,
		E_CE_RULES_NOT_FOUND,
		E_CE_UPDATE_RETRIEVAL_FAILED,
		E_CE_INVALID_ENGINE_SETTINGS,
		E_CE_INVALID_CUSTOM_RULE_CONFIGURATION,
		E_CE_RULE_PACKAGE_SET_NOT_FOUND,
		E_CE_RULE_PACKAGE_DECRYPTION_FAILED,
		E_CE_REGEX_COMPILATION_FAILED,
		E_CE_REGEX_MEMORY_EXHAUSTED,
		E_CE_KEYWORD_MAX_LENGTH_EXCEEDED,
		S_CE_SOME_RULES_NOT_FOUND = 262673
	}
}

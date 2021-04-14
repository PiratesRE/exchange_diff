using System;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	internal static class RuleConstants
	{
		internal static bool TryParseEnabled(string enabledString, out RuleState state)
		{
			if (enabledString != null)
			{
				if (enabledString == "true")
				{
					state = RuleState.Enabled;
					return true;
				}
				if (enabledString == "false")
				{
					state = RuleState.Disabled;
					return true;
				}
			}
			state = RuleState.Enabled;
			return false;
		}

		internal static T TryParseEnum<T>(string enumText, T defaultValue) where T : struct
		{
			T result;
			if (string.IsNullOrEmpty(enumText) || !Enum.TryParse<T>(enumText, out result))
			{
				return defaultValue;
			}
			return result;
		}

		internal static string StringFromRuleState(RuleState state)
		{
			switch (state)
			{
			case RuleState.Enabled:
				return "true";
			case RuleState.Disabled:
				return "false";
			default:
				throw new InvalidOperationException("Invalid RuleState Enum Value.");
			}
		}

		internal const string TagRules = "rules";

		internal const string TagRule = "rule";

		internal const string TagVersion = "version";

		internal const string TagCondition = "condition";

		internal const string TagAction = "action";

		internal const string TagTrue = "true";

		internal const string TagFalse = "false";

		internal const string TagNot = "not";

		internal const string TagAnd = "and";

		internal const string TagOr = "or";

		internal const string TagArgument = "argument";

		internal const string TagGreaterThan = "greaterThan";

		internal const string TagLessThan = "lessThan";

		internal const string TagGreaterThanOrEqual = "greaterThanOrEqual";

		internal const string TagLessThanOrEqual = "lessThanOrEqual";

		internal const string TagEqual = "equal";

		internal const string TagNameValuesPairConfiguration = "NameValuesPairConfiguration";

		internal const string TagNotEqual = "notEqual";

		internal const string TagContains = "contains";

		internal const string TagMatches = "matches";

		internal const string TagMatchesRegex = "matchesRegex";

		internal const string TagIs = "is";

		internal const string TagExists = "exists";

		internal const string TagNotExists = "notExists";

		internal const string TagQueryMatch = "queryMatch";

		internal const string TagTextQueryMatch = "textQueryMatch";

		internal const string TagValue = "value";

		internal const string TagKeyValueCollection = "keyValues";

		internal const string TagKeyValue = "keyValue";

		internal const string AttributeKey = "key";

		internal const string TagList = "list";

		internal const string TagPartner = "partner";

		internal const string TagHold = "Hold";

		internal const string TagRetentionExpire = "RetentionExpire";

		internal const string TagRetentionRecycle = "RetentionRecycle";

		internal const string TypeInteger = "integer";

		internal const string TypeString = "string";

		internal const string TagRuleCOllectionName = "rulesVersioned";

		internal const string AttributeName = "name";

		internal const string AttributeExternalName = "externalName";

		internal const string AttributeComments = "comments";

		internal const string AttributeValue = "value";

		internal const string AttributeProperty = "property";

		internal const string AttributeType = "type";

		internal const string AttributeSupplementalInfo = "suppl";

		internal const string AttributeEnabled = "enabled";

		internal const string AttributeImmutableId = "id";

		internal const string AttributeExpiryDate = "expiryDate";

		internal const string AttributeActivationDate = "activationDate";

		internal const string AttributeException = "exception";

		internal const string AttributeMode = "mode";

		internal const string AttributeSubType = "subType";

		internal const string ErrorAction = "errorAction";

		internal const string EnabledTrue = "true";

		internal const string EnabledFalse = "false";

		internal const string RequiredMinimumVersion = "requiredMinVersion";

		internal const string TagAuditOperations = "auditOperations";

		internal const string TagBlockAccess = "BlockAccess";

		internal const string TagGenerateIncidentReportAction = "GenerateIncidentReport";

		internal const string TagNotifyAuthorsAction = "NotifyAuthors";

		internal const string TagContentMetadataContains = "contentMetadataContains";

		internal const string TagContentContainsDataClassification = "containsDataClassification";

		public static class RuleTagNames
		{
			internal const string Tags = "tags";

			internal const string Tag = "tag";

			internal const string NameAttribute = "name";

			internal const string TypeAttribute = "type";
		}
	}
}

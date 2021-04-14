using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal static class RulesStrings
	{
		static RulesStrings()
		{
			RulesStrings.stringIDs.Add(3561617716U, "InvalidPropertyType");
			RulesStrings.stringIDs.Add(2809257232U, "RuleSubtypeNone");
			RulesStrings.stringIDs.Add(800827665U, "ValueTextNotFound");
			RulesStrings.stringIDs.Add(853321755U, "StateDisabled");
			RulesStrings.stringIDs.Add(230388220U, "ModeAuditAndNotify");
			RulesStrings.stringIDs.Add(3847357176U, "ArgumentIncorrect");
			RulesStrings.stringIDs.Add(2209094239U, "StringArrayPropertyRequiredForMultiValue");
			RulesStrings.stringIDs.Add(250418233U, "EmptyPropertyName");
			RulesStrings.stringIDs.Add(3358696673U, "TemplateTypeAll");
			RulesStrings.stringIDs.Add(2539054471U, "MissingValue");
			RulesStrings.stringIDs.Add(4131692042U, "RuleSubtypeDlp");
			RulesStrings.stringIDs.Add(1535753010U, "ConditionTagNotFound");
			RulesStrings.stringIDs.Add(3988942463U, "RuleDescriptionExpiry");
			RulesStrings.stringIDs.Add(1547451386U, "RuleErrorActionIgnore");
			RulesStrings.stringIDs.Add(2828094232U, "RuleDescriptionAndDelimiter");
			RulesStrings.stringIDs.Add(181941971U, "TemplateTypeDistributed");
			RulesStrings.stringIDs.Add(3623941994U, "TooManyRules");
			RulesStrings.stringIDs.Add(2040889586U, "SearchablePropertyRequired");
			RulesStrings.stringIDs.Add(508904078U, "TemplateTypeArchived");
			RulesStrings.stringIDs.Add(2588488610U, "RuleDescriptionActivation");
			RulesStrings.stringIDs.Add(242810468U, "RuleErrorActionDefer");
			RulesStrings.stringIDs.Add(41715449U, "ModeEnforce");
			RulesStrings.stringIDs.Add(3213119304U, "StateEnabled");
			RulesStrings.stringIDs.Add(1688265212U, "RuleDescriptionTakeActions");
			RulesStrings.stringIDs.Add(3869829980U, "ModeAudit");
			RulesStrings.stringIDs.Add(2173155634U, "EndOfStream");
			RulesStrings.stringIDs.Add(2986652960U, "InconsistentValueTypesInConditionProperties");
			RulesStrings.stringIDs.Add(2264090780U, "RuleDescriptionExceptIf");
			RulesStrings.stringIDs.Add(250901884U, "RuleDescriptionOrDelimiter");
			RulesStrings.stringIDs.Add(1859427639U, "RuleDescriptionIf");
			RulesStrings.stringIDs.Add(2445813381U, "NoMultiValueForActionArgument");
		}

		public static LocalizedString InvalidPropertyType
		{
			get
			{
				return new LocalizedString("InvalidPropertyType", "Ex825852", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleSubtypeNone
		{
			get
			{
				return new LocalizedString("RuleSubtypeNone", "", false, false, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValueTextNotFound
		{
			get
			{
				return new LocalizedString("ValueTextNotFound", "ExC7F50D", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StateDisabled
		{
			get
			{
				return new LocalizedString("StateDisabled", "Ex726DDA", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAttribute(string attributeName, string tagName, string tagValue)
		{
			return new LocalizedString("InvalidAttribute", "Ex8A23FC", false, true, RulesStrings.ResourceManager, new object[]
			{
				attributeName,
				tagName,
				tagValue
			});
		}

		public static LocalizedString RuleInvalidOperationDescription(string details)
		{
			return new LocalizedString("RuleInvalidOperationDescription", "ExDDC7D0", false, true, RulesStrings.ResourceManager, new object[]
			{
				details
			});
		}

		public static LocalizedString ModeAuditAndNotify
		{
			get
			{
				return new LocalizedString("ModeAuditAndNotify", "", false, false, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArgumentIncorrect
		{
			get
			{
				return new LocalizedString("ArgumentIncorrect", "Ex7F8483", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidActionName(string name)
		{
			return new LocalizedString("InvalidActionName", "ExB6A978", false, true, RulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString StringArrayPropertyRequiredForMultiValue
		{
			get
			{
				return new LocalizedString("StringArrayPropertyRequiredForMultiValue", "Ex12CF73", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelimeterNotFound(string propertyName)
		{
			return new LocalizedString("DelimeterNotFound", "Ex8C7A04", false, true, RulesStrings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString NameTooLong(string node, int maxLength)
		{
			return new LocalizedString("NameTooLong", "Ex1053AF", false, true, RulesStrings.ResourceManager, new object[]
			{
				node,
				maxLength
			});
		}

		public static LocalizedString InvalidPropertyForRule(string property, string rule)
		{
			return new LocalizedString("InvalidPropertyForRule", "Ex9EAEE4", false, true, RulesStrings.ResourceManager, new object[]
			{
				property,
				rule
			});
		}

		public static LocalizedString InvalidValue(string valueName)
		{
			return new LocalizedString("InvalidValue", "", false, false, RulesStrings.ResourceManager, new object[]
			{
				valueName
			});
		}

		public static LocalizedString EmptyPropertyName
		{
			get
			{
				return new LocalizedString("EmptyPropertyName", "Ex324B29", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TemplateTypeAll
		{
			get
			{
				return new LocalizedString("TemplateTypeAll", "Ex332B3A", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingValue
		{
			get
			{
				return new LocalizedString("MissingValue", "Ex88B534", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValueIsNotAllowed(string name)
		{
			return new LocalizedString("ValueIsNotAllowed", "Ex79F9E4", false, true, RulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString RuleSubtypeDlp
		{
			get
			{
				return new LocalizedString("RuleSubtypeDlp", "", false, false, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidArgumentForType(string argument, string type)
		{
			return new LocalizedString("InvalidArgumentForType", "Ex6597F9", false, true, RulesStrings.ResourceManager, new object[]
			{
				argument,
				type
			});
		}

		public static LocalizedString ConditionTagNotFound
		{
			get
			{
				return new LocalizedString("ConditionTagNotFound", "Ex6DDB27", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionExpiry
		{
			get
			{
				return new LocalizedString("RuleDescriptionExpiry", "", false, false, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PropertyTypeIsFixed(string name)
		{
			return new LocalizedString("PropertyTypeIsFixed", "ExE3AFB8", false, true, RulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ActionNotFound(string name)
		{
			return new LocalizedString("ActionNotFound", "Ex4EB782", false, true, RulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString RuleErrorActionIgnore
		{
			get
			{
				return new LocalizedString("RuleErrorActionIgnore", "", false, false, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleNameExists(string ruleName)
		{
			return new LocalizedString("RuleNameExists", "Ex17E8C7", false, true, RulesStrings.ResourceManager, new object[]
			{
				ruleName
			});
		}

		public static LocalizedString RuleDescriptionAndDelimiter
		{
			get
			{
				return new LocalizedString("RuleDescriptionAndDelimiter", "Ex318FEA", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TemplateTypeDistributed
		{
			get
			{
				return new LocalizedString("TemplateTypeDistributed", "Ex98338D", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TooManyRules
		{
			get
			{
				return new LocalizedString("TooManyRules", "Ex79D216", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchablePropertyRequired
		{
			get
			{
				return new LocalizedString("SearchablePropertyRequired", "Ex9FDCE3", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TemplateTypeArchived
		{
			get
			{
				return new LocalizedString("TemplateTypeArchived", "ExC49CEF", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EndTagNotFound(string elementName)
		{
			return new LocalizedString("EndTagNotFound", "Ex0A7237", false, true, RulesStrings.ResourceManager, new object[]
			{
				elementName
			});
		}

		public static LocalizedString RuleDescriptionActivation
		{
			get
			{
				return new LocalizedString("RuleDescriptionActivation", "", false, false, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorActionDefer
		{
			get
			{
				return new LocalizedString("RuleErrorActionDefer", "", false, false, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPropertyName(string Name)
		{
			return new LocalizedString("InvalidPropertyName", "Ex032889", false, true, RulesStrings.ResourceManager, new object[]
			{
				Name
			});
		}

		public static LocalizedString ModeEnforce
		{
			get
			{
				return new LocalizedString("ModeEnforce", "", false, false, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StateEnabled
		{
			get
			{
				return new LocalizedString("StateEnabled", "Ex1B5E80", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionTakeActions
		{
			get
			{
				return new LocalizedString("RuleDescriptionTakeActions", "Ex5F7B8F", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModeAudit
		{
			get
			{
				return new LocalizedString("ModeAudit", "", false, false, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EndOfStream
		{
			get
			{
				return new LocalizedString("EndOfStream", "ExE9105D", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InconsistentValueTypesInConditionProperties
		{
			get
			{
				return new LocalizedString("InconsistentValueTypesInConditionProperties", "", false, false, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TagNotFound(string elementName)
		{
			return new LocalizedString("TagNotFound", "ExD072AB", false, true, RulesStrings.ResourceManager, new object[]
			{
				elementName
			});
		}

		public static LocalizedString ActionRequiresConstantArguments(string name)
		{
			return new LocalizedString("ActionRequiresConstantArguments", "Ex04D6DE", false, true, RulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InvalidKeyValueParameter(string valueType)
		{
			return new LocalizedString("InvalidKeyValueParameter", "", false, false, RulesStrings.ResourceManager, new object[]
			{
				valueType
			});
		}

		public static LocalizedString RuleParsingError(string diagnostic, int lineNumber, int linePosition)
		{
			return new LocalizedString("RuleParsingError", "Ex119BBB", false, true, RulesStrings.ResourceManager, new object[]
			{
				diagnostic,
				lineNumber,
				linePosition
			});
		}

		public static LocalizedString InvalidArgumentType(string type)
		{
			return new LocalizedString("InvalidArgumentType", "Ex42DFEA", false, true, RulesStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString InvalidCondition(string conditionName)
		{
			return new LocalizedString("InvalidCondition", "Ex3B434A", false, true, RulesStrings.ResourceManager, new object[]
			{
				conditionName
			});
		}

		public static LocalizedString ActionArgumentMismatch(string name)
		{
			return new LocalizedString("ActionArgumentMismatch", "ExE8E9BF", false, true, RulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString RuleDescriptionExceptIf
		{
			get
			{
				return new LocalizedString("RuleDescriptionExceptIf", "Ex00BFD1", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionOrDelimiter
		{
			get
			{
				return new LocalizedString("RuleDescriptionOrDelimiter", "ExAB3F67", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StringPropertyOrValueRequired(string name)
		{
			return new LocalizedString("StringPropertyOrValueRequired", "Ex16C9F7", false, true, RulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString AttributeNotFound(string attributeName, string elementName)
		{
			return new LocalizedString("AttributeNotFound", "Ex0E7C3B", false, true, RulesStrings.ResourceManager, new object[]
			{
				attributeName,
				elementName
			});
		}

		public static LocalizedString PropertyNotFound(string name)
		{
			return new LocalizedString("PropertyNotFound", "Ex768F43", false, true, RulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString NumericalPropertyRequiredForPredicate(string name)
		{
			return new LocalizedString("NumericalPropertyRequiredForPredicate", "Ex6E1D0E", false, true, RulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString EmptyTag(string elementName)
		{
			return new LocalizedString("EmptyTag", "Ex73F5EB", false, true, RulesStrings.ResourceManager, new object[]
			{
				elementName
			});
		}

		public static LocalizedString RuleDescriptionIf
		{
			get
			{
				return new LocalizedString("RuleDescriptionIf", "Ex3C413B", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoMultiValueForActionArgument
		{
			get
			{
				return new LocalizedString("NoMultiValueForActionArgument", "Ex36F9A8", false, true, RulesStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(RulesStrings.IDs key)
		{
			return new LocalizedString(RulesStrings.stringIDs[(uint)key], RulesStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(31);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.MessagingPolicies.Rules.RulesStrings", typeof(RulesStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			InvalidPropertyType = 3561617716U,
			RuleSubtypeNone = 2809257232U,
			ValueTextNotFound = 800827665U,
			StateDisabled = 853321755U,
			ModeAuditAndNotify = 230388220U,
			ArgumentIncorrect = 3847357176U,
			StringArrayPropertyRequiredForMultiValue = 2209094239U,
			EmptyPropertyName = 250418233U,
			TemplateTypeAll = 3358696673U,
			MissingValue = 2539054471U,
			RuleSubtypeDlp = 4131692042U,
			ConditionTagNotFound = 1535753010U,
			RuleDescriptionExpiry = 3988942463U,
			RuleErrorActionIgnore = 1547451386U,
			RuleDescriptionAndDelimiter = 2828094232U,
			TemplateTypeDistributed = 181941971U,
			TooManyRules = 3623941994U,
			SearchablePropertyRequired = 2040889586U,
			TemplateTypeArchived = 508904078U,
			RuleDescriptionActivation = 2588488610U,
			RuleErrorActionDefer = 242810468U,
			ModeEnforce = 41715449U,
			StateEnabled = 3213119304U,
			RuleDescriptionTakeActions = 1688265212U,
			ModeAudit = 3869829980U,
			EndOfStream = 2173155634U,
			InconsistentValueTypesInConditionProperties = 2986652960U,
			RuleDescriptionExceptIf = 2264090780U,
			RuleDescriptionOrDelimiter = 250901884U,
			RuleDescriptionIf = 1859427639U,
			NoMultiValueForActionArgument = 2445813381U
		}

		private enum ParamIDs
		{
			InvalidAttribute,
			RuleInvalidOperationDescription,
			InvalidActionName,
			DelimeterNotFound,
			NameTooLong,
			InvalidPropertyForRule,
			InvalidValue,
			ValueIsNotAllowed,
			InvalidArgumentForType,
			PropertyTypeIsFixed,
			ActionNotFound,
			RuleNameExists,
			EndTagNotFound,
			InvalidPropertyName,
			TagNotFound,
			ActionRequiresConstantArguments,
			InvalidKeyValueParameter,
			RuleParsingError,
			InvalidArgumentType,
			InvalidCondition,
			ActionArgumentMismatch,
			StringPropertyOrValueRequired,
			AttributeNotFound,
			PropertyNotFound,
			NumericalPropertyRequiredForPredicate,
			EmptyTag
		}
	}
}

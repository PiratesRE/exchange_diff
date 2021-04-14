using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMPhoneSession
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UMPhoneSessionSchema : SimpleProviderObjectSchema
	{
		public static SimpleProviderPropertyDefinition CallState
		{
			get
			{
				return UMPhoneSessionSchema.sessionState;
			}
		}

		public static SimpleProviderPropertyDefinition EventCause
		{
			get
			{
				return UMPhoneSessionSchema.sessionEventCause;
			}
		}

		public static SimpleProviderPropertyDefinition OperationResult
		{
			get
			{
				return UMPhoneSessionSchema.sessionResult;
			}
		}

		public static SimpleProviderPropertyDefinition PhoneNumber
		{
			get
			{
				return UMPhoneSessionSchema.phoneNumber;
			}
		}

		private static SimpleProviderPropertyDefinition CreatePropertyDefinition(string propertyName, Type propertyType, object defaultValue)
		{
			return new SimpleProviderPropertyDefinition(propertyName, ExchangeObjectVersion.Exchange2010, propertyType, PropertyDefinitionFlags.None, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}

		public const string UMMailboxParameterName = "UMMailbox";

		public const string PhoneNumberParameterName = "PhoneNumber";

		public const string DefaultVoicemailGreetingParameterName = "DefaultVoicemailGreeting";

		public const string AwayVoicemailGreetingParameterName = "AwayVoicemailGreeting";

		public const string CallAnsweringRuleIdParameterName = "CallAnsweringRuleId";

		private static SimpleProviderPropertyDefinition sessionState = UMPhoneSessionSchema.CreatePropertyDefinition("CallState", typeof(UMCallState), UMCallState.Disconnected);

		private static SimpleProviderPropertyDefinition sessionEventCause = UMPhoneSessionSchema.CreatePropertyDefinition("EventCause", typeof(UMEventCause), UMEventCause.None);

		private static SimpleProviderPropertyDefinition sessionResult = UMPhoneSessionSchema.CreatePropertyDefinition("OperationResult", typeof(UMOperationResult), UMOperationResult.Failure);

		private static SimpleProviderPropertyDefinition phoneNumber = UMPhoneSessionSchema.CreatePropertyDefinition("PhoneNumber", typeof(string), string.Empty);
	}
}

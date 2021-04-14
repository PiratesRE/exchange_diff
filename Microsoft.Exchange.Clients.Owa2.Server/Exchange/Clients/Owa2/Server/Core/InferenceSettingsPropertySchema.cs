using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class InferenceSettingsPropertySchema : UserConfigurationPropertySchemaBase
	{
		private InferenceSettingsPropertySchema()
		{
		}

		internal static InferenceSettingsPropertySchema Instance
		{
			get
			{
				if (InferenceSettingsPropertySchema.instance == null)
				{
					InferenceSettingsPropertySchema.instance = new InferenceSettingsPropertySchema();
				}
				return InferenceSettingsPropertySchema.instance;
			}
		}

		internal override UserConfigurationPropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return InferenceSettingsPropertySchema.propertyDefinitions;
			}
		}

		internal override UserConfigurationPropertyId PropertyDefinitionsBaseId
		{
			get
			{
				return UserConfigurationPropertyId.IsClutterUIEnabled;
			}
		}

		private static readonly UserConfigurationPropertyDefinition[] propertyDefinitions = new UserConfigurationPropertyDefinition[]
		{
			new UserConfigurationPropertyDefinition("IsClutterUIEnabled", typeof(bool?), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateIsClutterUIEnabledCallback))
		};

		private static InferenceSettingsPropertySchema instance;
	}
}

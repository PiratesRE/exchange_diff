using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal class BudgetMetadataSchema : ObjectSchema
	{
		internal static SimpleProviderPropertyDefinition BuildStringPropDef(string name)
		{
			return BudgetMetadataSchema.BuildRefTypePropDef<string>(name);
		}

		internal static SimpleProviderPropertyDefinition BuildRefTypePropDef<T>(string name)
		{
			return new SimpleProviderPropertyDefinition(name, ExchangeObjectVersion.Current, typeof(T), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}

		internal static SimpleProviderPropertyDefinition BuildValueTypePropDef<T>(string name, T defaultValue)
		{
			return new SimpleProviderPropertyDefinition(name, ExchangeObjectVersion.Current, typeof(T), PropertyDefinitionFlags.PersistDefaultValue, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}

		internal static SimpleProviderPropertyDefinition BuildUnlimitedPropDef(string name)
		{
			return new SimpleProviderPropertyDefinition(name, ExchangeObjectVersion.Current, typeof(Unlimited<uint>), PropertyDefinitionFlags.None, Unlimited<uint>.UnlimitedValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}

		internal static SimpleProviderPropertyDefinition BuildValueTypePropDef<T>(string name)
		{
			return BudgetMetadataSchema.BuildValueTypePropDef<T>(name, default(T));
		}

		public static readonly SimpleProviderPropertyDefinition InCutoff = BudgetMetadataSchema.BuildValueTypePropDef<bool>("InCutoff");

		public static readonly SimpleProviderPropertyDefinition InMicroDelay = BudgetMetadataSchema.BuildValueTypePropDef<bool>("InMicroDelay");

		public static readonly SimpleProviderPropertyDefinition NotThrottled = BudgetMetadataSchema.BuildValueTypePropDef<bool>("NotThrottled");

		public static readonly SimpleProviderPropertyDefinition Connections = BudgetMetadataSchema.BuildValueTypePropDef<int>("Connections");

		public static readonly SimpleProviderPropertyDefinition Balance = BudgetMetadataSchema.BuildValueTypePropDef<float>("Balance");

		public static readonly SimpleProviderPropertyDefinition OutstandingActionCount = BudgetMetadataSchema.BuildValueTypePropDef<int>("OutstandingActionCount");

		public static readonly SimpleProviderPropertyDefinition IsServiceAccount = BudgetMetadataSchema.BuildValueTypePropDef<bool>("IsServiceAccount");

		public static readonly SimpleProviderPropertyDefinition ThrottlingPolicy = BudgetMetadataSchema.BuildStringPropDef("ThrottlingPolicy");

		public static readonly SimpleProviderPropertyDefinition LiveTime = BudgetMetadataSchema.BuildValueTypePropDef<TimeSpan>("LiveTime");

		public static readonly SimpleProviderPropertyDefinition Name = BudgetMetadataSchema.BuildStringPropDef("Name");

		public static readonly SimpleProviderPropertyDefinition IsGlobalPolicy = BudgetMetadataSchema.BuildValueTypePropDef<bool>("IsGlobalPolicy");

		public static readonly SimpleProviderPropertyDefinition IsOrgPolicy = BudgetMetadataSchema.BuildValueTypePropDef<bool>("IsOrgPolicy");

		public static readonly SimpleProviderPropertyDefinition IsRegularPolicy = BudgetMetadataSchema.BuildValueTypePropDef<bool>("IsRegularPolicy");
	}
}

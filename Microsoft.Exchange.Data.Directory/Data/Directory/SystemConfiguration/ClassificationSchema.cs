using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ClassificationSchema : ADConfigurationObjectSchema
	{
		private static void PermissionMenuVisibleSetter(object o, IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ClassificationSchema.ClassificationFlags];
			propertyBag[ClassificationSchema.ClassificationFlags] = (((bool)o) ? (num | 1) : (num & -2));
		}

		private static object PermissionMenuVisibleGetter(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ClassificationSchema.ClassificationFlags];
			return (num & 1) == 1;
		}

		private static void RetainClassificationEnabledSetter(object o, IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ClassificationSchema.ClassificationFlags];
			propertyBag[ClassificationSchema.ClassificationFlags] = (((bool)o) ? (num | 2) : (num & -3));
		}

		private static object RetainClassificationEnabledGetter(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ClassificationSchema.ClassificationFlags];
			return (num & 2) == 2;
		}

		private static object LocaleGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			if (adobjectId == null)
			{
				return null;
			}
			string name = adobjectId.Parent.Name;
			if (!(name == "Default"))
			{
				return name;
			}
			return null;
		}

		private const int FlagDisplayable = 1;

		private const int FlagRetainClassification = 2;

		public static readonly ADPropertyDefinition ClassificationID = new ADPropertyDefinition("classificationID", ExchangeObjectVersion.Exchange2007, typeof(Guid), "msExchMessageClassificationID", ADPropertyDefinitionFlags.WriteOnce | ADPropertyDefinitionFlags.Binary, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Version = new ADPropertyDefinition("version", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMessageClassificationVersion", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ClassificationFlags = new ADPropertyDefinition("flags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMessageClassificationFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 3, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Locale = new ADPropertyDefinition("locale", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchMessageClassificationLocale", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, new CustomFilterBuilderDelegate(ADObject.DummyCustomFilterBuilderDelegate), new GetterDelegate(ClassificationSchema.LocaleGetter), null, null, null);

		public static readonly ADPropertyDefinition DisplayName = new ADPropertyDefinition("displayName", ExchangeObjectVersion.Exchange2007, typeof(string), "displayName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SenderDescription = new ADPropertyDefinition("senderDescription", ExchangeObjectVersion.Exchange2007, typeof(string), "adminDescription", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RecipientDescription = new ADPropertyDefinition("recipientDescription", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchMessageClassificationBanner", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DisplayPrecedence = new ADPropertyDefinition("displayPrecedence", ExchangeObjectVersion.Exchange2007, typeof(ClassificationDisplayPrecedenceLevel), "msExchMessageClassificationDisplayPrecedence", ADPropertyDefinitionFlags.None, ClassificationDisplayPrecedenceLevel.Medium, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ClassificationDisplayPrecedenceLevel))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PermissionMenuVisible = new ADPropertyDefinition("permissionMenuVisible", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ClassificationSchema.ClassificationFlags
		}, null, new GetterDelegate(ClassificationSchema.PermissionMenuVisibleGetter), new SetterDelegate(ClassificationSchema.PermissionMenuVisibleSetter), null, null);

		public static readonly ADPropertyDefinition RetainClassificationEnabled = new ADPropertyDefinition("retainClassification", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ClassificationSchema.ClassificationFlags
		}, null, new GetterDelegate(ClassificationSchema.RetainClassificationEnabledGetter), new SetterDelegate(ClassificationSchema.RetainClassificationEnabledSetter), null, null);
	}
}

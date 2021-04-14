using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class GlobalSettingsPropertyDefinition : PropertyDefinition
	{
		public GlobalSettingsPropertyDefinition(string name, Type type, object defaultValue, PropertyDefinitionConstraint readConstraint, bool logMissingEntries, Func<GlobalSettingsPropertyDefinition, object> getter) : base(name, type)
		{
			this.LogMissingEntries = logMissingEntries;
			this.Getter = (getter ?? new Func<GlobalSettingsPropertyDefinition, object>(this.DefaultGetter));
			this.DefaultValue = defaultValue;
			this.ReadConstraint = readConstraint;
		}

		public object DefaultValue { get; private set; }

		public PropertyDefinitionConstraint ReadConstraint { get; private set; }

		public bool LogMissingEntries { get; private set; }

		internal Func<GlobalSettingsPropertyDefinition, object> Getter { get; private set; }

		internal object DefaultGetter(GlobalSettingsPropertyDefinition propDef)
		{
			string appSetting = GlobalSettingsSchema.GetAppSetting(propDef);
			if (appSetting == null)
			{
				if (propDef.LogMissingEntries)
				{
					AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_GlobalValueHasBeenDefaulted, new string[]
					{
						propDef.Name,
						propDef.DefaultValue.ToString()
					});
				}
				return propDef.DefaultValue;
			}
			return GlobalSettingsPropertyDefinition.ConvertValueFromString(propDef, appSetting);
		}

		public static object ConvertValueFromString(GlobalSettingsPropertyDefinition propDef, string valueToConvert)
		{
			return GlobalSettingsPropertyDefinition.ConvertValueFromString(valueToConvert, propDef.Type, propDef.Name, propDef.DefaultValue);
		}

		public static object ConvertValueFromString(string valueToConvert, Type destinationType, string propName, object defaultValue)
		{
			if (string.IsNullOrEmpty(valueToConvert))
			{
				return defaultValue;
			}
			bool flag;
			if (destinationType == typeof(bool) && bool.TryParse(valueToConvert, out flag))
			{
				return flag;
			}
			object result;
			if (destinationType.IsEnum && EnumValidator.TryParse(destinationType, valueToConvert, EnumParseOptions.Default, out result))
			{
				return result;
			}
			if (destinationType.IsGenericType && destinationType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				bool flag2 = valueToConvert == null || "null".Equals(valueToConvert, StringComparison.OrdinalIgnoreCase) || "$null".Equals(valueToConvert, StringComparison.OrdinalIgnoreCase);
				if (flag2)
				{
					return null;
				}
			}
			object result2;
			try
			{
				result2 = LanguagePrimitives.ConvertTo(valueToConvert, destinationType);
			}
			catch (PSInvalidCastException)
			{
				AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_GlobalValueNotParsable, new string[]
				{
					propName,
					destinationType.Name,
					valueToConvert,
					defaultValue.ToString()
				});
				result2 = defaultValue;
			}
			return result2;
		}

		public PropertyConstraintViolationError Validate(object value)
		{
			if (this.ReadConstraint == null)
			{
				return null;
			}
			return this.ReadConstraint.Validate(value, this, null);
		}

		public override int GetHashCode()
		{
			return base.Name.GetHashCode() ^ base.Type.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			GlobalSettingsPropertyDefinition globalSettingsPropertyDefinition = obj as GlobalSettingsPropertyDefinition;
			return globalSettingsPropertyDefinition != null && globalSettingsPropertyDefinition.Name == base.Name && globalSettingsPropertyDefinition.Type == base.Type;
		}
	}
}

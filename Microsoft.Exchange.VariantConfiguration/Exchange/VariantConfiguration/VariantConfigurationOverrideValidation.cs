using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Flighting;
using Microsoft.Exchange.VariantConfiguration.Reflection;

namespace Microsoft.Exchange.VariantConfiguration
{
	public sealed class VariantConfigurationOverrideValidation
	{
		public VariantConfigurationOverrideValidation(VariantConfigurationOverride o) : this(o, false)
		{
		}

		public VariantConfigurationOverrideValidation(VariantConfigurationOverride o, bool criticalOnly)
		{
			this.Override = o;
			this.criticalOnly = criticalOnly;
		}

		public VariantConfigurationOverride Override { get; private set; }

		private bool IncludeInternal
		{
			get
			{
				return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).VariantConfig.InternalAccess.Enabled;
			}
		}

		public void Validate()
		{
			if (this.Override == null)
			{
				throw new NullOverrideException(null);
			}
			if (!this.criticalOnly)
			{
				if (this.Override.IsFlight)
				{
					this.ValidateFlightOverride();
				}
				else
				{
					this.ValidateSettingOverride();
				}
			}
			this.LoadWithOverride();
		}

		private void ValidateFlightOverride()
		{
			if (!this.IncludeInternal)
			{
				throw new FlightNameValidationException(this.Override, new string[0], null);
			}
			if (string.IsNullOrEmpty(this.Override.ComponentName))
			{
				throw new FlightNameValidationException(this.Override, VariantConfiguration.Flights.Flights.OrderBy((string name) => name, StringComparer.InvariantCultureIgnoreCase), null);
			}
			if (!VariantConfiguration.Flights.Contains(this.Override.ComponentName))
			{
				throw new FlightNameValidationException(this.Override, VariantConfiguration.Flights.Flights.OrderBy((string name) => name, StringComparer.InvariantCultureIgnoreCase), null);
			}
			this.ValidateParameters(VariantConfiguration.Flights[this.Override.ComponentName].Type);
		}

		private void ValidateSettingOverride()
		{
			if (string.IsNullOrEmpty(this.Override.ComponentName))
			{
				throw new ComponentNameValidationException(this.Override, VariantConfiguration.Settings.GetComponents(this.IncludeInternal).OrderBy((string name) => name, StringComparer.InvariantCultureIgnoreCase), null);
			}
			if (!VariantConfiguration.Settings.Contains(this.Override.ComponentName, this.IncludeInternal))
			{
				throw new ComponentNameValidationException(this.Override, VariantConfiguration.Settings.GetComponents(this.IncludeInternal).OrderBy((string name) => name, StringComparer.InvariantCultureIgnoreCase), null);
			}
			VariantConfigurationComponent variantConfigurationComponent = VariantConfiguration.Settings[this.Override.ComponentName];
			if (!variantConfigurationComponent.Contains(this.Override.SectionName, this.IncludeInternal))
			{
				throw new SectionNameValidationException(this.Override, variantConfigurationComponent.GetSections(this.IncludeInternal).OrderBy((string name) => name, StringComparer.InvariantCultureIgnoreCase), null);
			}
			this.ValidateParameters(variantConfigurationComponent[this.Override.SectionName].Type);
		}

		private void ValidateParameters(Type type)
		{
			foreach (string line in this.Override.Parameters)
			{
				string text;
				KeyValuePair<string, string>[] array;
				string value;
				this.ParseParameterLine(line, out text, out array, out value);
				try
				{
					if (type.GetProperty(text) == null)
					{
						HashSet<string> hashSet = new HashSet<string>();
						foreach (Type type2 in type.GetInterfaces().Concat(new Type[]
						{
							type
						}))
						{
							PropertyInfo[] properties = type2.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
							foreach (string item in from name in Array.ConvertAll<PropertyInfo, string>(properties, (PropertyInfo prop) => prop.Name)
							where !name.Equals("Name")
							select name)
							{
								hashSet.Add(item);
							}
						}
						throw new ParameterNameValidationException(this.Override, text, hashSet.OrderBy((string name) => name, StringComparer.InvariantCultureIgnoreCase), null);
					}
				}
				catch (AmbiguousMatchException)
				{
				}
				foreach (KeyValuePair<string, string> keyValuePair in array)
				{
					if (!VariantType.Variants.Contains(keyValuePair.Key, this.IncludeInternal))
					{
						throw new VariantNameValidationException(this.Override, keyValuePair.Key, VariantType.Variants.GetNames(this.IncludeInternal).OrderBy((string name) => name, StringComparer.InvariantCultureIgnoreCase), null);
					}
					VariantType variantByName = VariantType.Variants.GetVariantByName(keyValuePair.Key, this.IncludeInternal);
					if (variantByName.Type == typeof(bool))
					{
						if (!keyValuePair.Value.Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase) && !keyValuePair.Value.Equals(bool.FalseString, StringComparison.InvariantCultureIgnoreCase))
						{
							throw new VariantValueValidationException(this.Override, variantByName, keyValuePair.Value, "true|false", null);
						}
					}
					else if (variantByName.Type == typeof(Version))
					{
						if (!VariantType.VersionRegex.IsMatch(keyValuePair.Value))
						{
							throw new VariantValueValidationException(this.Override, variantByName, keyValuePair.Value, "NN.NN.NNNN.NNN", null);
						}
					}
					else if (variantByName.Type == typeof(Guid) && !VariantType.GuidRegex.IsMatch(keyValuePair.Value))
					{
						throw new VariantValueValidationException(this.Override, variantByName, keyValuePair.Value, "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", null);
					}
				}
				if (type is IFlight && (string.Equals(text, "Ramp", StringComparison.OrdinalIgnoreCase) || string.Equals(text, "Rotate", StringComparison.OrdinalIgnoreCase)) && !VariantConfigurationValidator.IsHashBucketValueParsable(value))
				{
					throw new SyntaxValidationException(this.Override, null);
				}
			}
		}

		private void ParseParameterLine(string line, out string parameterName, out KeyValuePair<string, string>[] variants, out string parameterValue)
		{
			Match match = VariantConfigurationOverrideValidation.ParameterRegex.Match(line);
			if (!match.Success)
			{
				throw new ParameterSyntaxValidationException(this.Override, line, null);
			}
			parameterName = match.Groups["PN"].Value;
			Group group = match.Groups["VN"];
			Group group2 = match.Groups["VV"];
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			if (group.Success && group2.Success)
			{
				for (int i = 0; i < group.Captures.Count; i++)
				{
					list.Add(new KeyValuePair<string, string>(group.Captures[i].Value, group2.Captures[i].Value));
				}
			}
			variants = list.ToArray();
			parameterValue = match.Groups["PV"].Value;
		}

		private void LoadWithOverride()
		{
			try
			{
				VariantConfiguration.GetProviderForValidation(this.Override);
			}
			catch (Exception innerException)
			{
				throw new SyntaxValidationException(this.Override, innerException);
			}
		}

		private static readonly string VariantValueUnquoted = "(?<VV>[\\w\\.\\-\\+\\^]+)";

		private static readonly string VariantValueQuoted = "(?:\"(?<VV>(?:\"\"|[^\"])*)\")";

		private static readonly string ParameterName = "[\\w\\.\\-\\+\\\\\\/\\{\\}\\(\\)#\\*\\$\\[\\]:]+";

		private static readonly Regex ParameterRegex = new Regex(string.Format("^\\s*(?<PN>{0})\\s*((&\\s*(?<VN>[\\w\\.\\-\\+\\^]+)\\s*:\\s*(?:{1}|{2})\\s*)+)?=\\s*(?<PV>.*?)\\s*$", VariantConfigurationOverrideValidation.ParameterName, VariantConfigurationOverrideValidation.VariantValueUnquoted, VariantConfigurationOverrideValidation.VariantValueQuoted), RegexOptions.Compiled);

		private readonly bool criticalOnly;
	}
}

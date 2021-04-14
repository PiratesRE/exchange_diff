using System;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[Serializable]
	public class SettingOverride : ADConfigurationObject
	{
		public static ADObjectId GetContainerId(bool isFlight)
		{
			ADObjectId rootOrgContainerIdForLocalForest = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
			return rootOrgContainerIdForLocalForest.GetDescendantId(isFlight ? SettingOverride.FlightsRelativePath : SettingOverride.SettingsRelativePath);
		}

		public static void Validate(VariantConfigurationOverride o)
		{
			SettingOverride.Validate(o, false);
		}

		public static void Validate(VariantConfigurationOverride o, bool criticalOnly)
		{
			try
			{
				new VariantConfigurationOverrideValidation(o, criticalOnly).Validate();
			}
			catch (NullOverrideException innerException)
			{
				throw new SettingOverrideNullException(innerException);
			}
			catch (FlightNameValidationException ex)
			{
				throw new SettingOverrideInvalidFlightNameException(ex.Override.ComponentName, string.Join(", ", ex.AllowedValues), ex);
			}
			catch (ComponentNameValidationException ex2)
			{
				throw new SettingOverrideInvalidComponentNameException(ex2.Override.ComponentName, string.Join(", ", ex2.AllowedValues), ex2);
			}
			catch (SectionNameValidationException ex3)
			{
				throw new SettingOverrideInvalidSectionNameException(ex3.Override.ComponentName, ex3.Override.SectionName, string.Join(", ", ex3.AllowedValues), ex3);
			}
			catch (ParameterNameValidationException ex4)
			{
				throw new SettingOverrideInvalidParameterNameException(ex4.Override.ComponentName, ex4.Override.SectionName, ex4.ParameterName, string.Join(", ", ex4.AllowedValues), ex4);
			}
			catch (ParameterSyntaxValidationException ex5)
			{
				throw new SettingOverrideInvalidParameterSyntaxException(ex5.Override.ComponentName, ex5.Override.SectionName, ex5.ParameterLine, ex5);
			}
			catch (VariantNameValidationException ex6)
			{
				throw new SettingOverrideInvalidVariantNameException(ex6.Override.ComponentName, ex6.Override.SectionName, ex6.VariantName, string.Join(", ", ex6.AllowedValues), ex6);
			}
			catch (VariantValueValidationException ex7)
			{
				throw new SettingOverrideInvalidVariantValueException(ex7.Override.ComponentName, ex7.Override.SectionName, ex7.Value, ex7.Variant.Type.Name, ex7.Format, ex7);
			}
			catch (SyntaxValidationException ex8)
			{
				throw new SettingOverrideSyntaxException(ex8.Override.ComponentName, ex8.Override.SectionName, "@(\"" + string.Join("\", \"", Array.ConvertAll<string, string>(ex8.Override.Parameters, (string parameter) => parameter.Replace("\"", "^\""))) + "\")", (ex8.InnerException != null) ? ex8.InnerException.Message : ex8.Message, ex8);
			}
			catch (OverrideValidationException ex9)
			{
				throw new SettingOverrideGenericException(ex9.GetType().Name, ex9.Override.ComponentName, ex9.Override.SectionName, SettingOverride.FormatParameters(ex9.Override.Parameters), ex9);
			}
			catch (Exception ex10)
			{
				throw new SettingOverrideUnexpectedException(ex10.GetType().Name, ex10);
			}
		}

		public void SetName(string name, bool isFlight)
		{
			base.SetId(SettingOverride.GetContainerId(isFlight).GetChildId(name));
		}

		public VariantConfigurationOverride GetVariantConfigurationOverride()
		{
			if (this.FlightName != null)
			{
				return new VariantConfigurationFlightOverride(this.FlightName, this.Parameters.ToArray());
			}
			return new VariantConfigurationSettingOverride(this.ComponentName, this.SectionName, this.Parameters.ToArray());
		}

		public string ComponentName
		{
			get
			{
				return this.Xml.ComponentName;
			}
		}

		public string SectionName
		{
			get
			{
				return this.Xml.SectionName;
			}
		}

		public string FlightName
		{
			get
			{
				return this.Xml.FlightName;
			}
		}

		public string ModifiedBy
		{
			get
			{
				return this.Xml.ModifiedBy;
			}
		}

		public string Reason
		{
			get
			{
				return this.Xml.Reason;
			}
		}

		public Version MinVersion
		{
			get
			{
				return this.Xml.MinVersion;
			}
		}

		public Version MaxVersion
		{
			get
			{
				return this.Xml.MaxVersion;
			}
		}

		public Version FixVersion
		{
			get
			{
				return this.Xml.FixVersion;
			}
		}

		public string[] Server
		{
			get
			{
				return this.Xml.Server;
			}
		}

		public MultiValuedProperty<string> Parameters
		{
			get
			{
				return this.Xml.Parameters;
			}
		}

		internal bool Applies
		{
			get
			{
				return this.Xml != null && this.Xml.Applies && this.Xml.Parameters.Count > 0;
			}
		}

		public string XmlRaw
		{
			get
			{
				return (string)this[SettingOverrideSchema.ConfigurationXmlRaw];
			}
			set
			{
				this[SettingOverrideSchema.ConfigurationXmlRaw] = value;
			}
		}

		internal SettingOverrideXml Xml
		{
			get
			{
				return (SettingOverrideXml)this[SettingOverrideSchema.ConfigurationXml];
			}
			set
			{
				this[SettingOverrideSchema.ConfigurationXml] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return SettingOverride.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return SettingOverride.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return SettingOverride.SettingsRelativePath;
			}
		}

		private static string FormatParameters(string[] parameters)
		{
			return "{'" + string.Join("', '", parameters) + "'}";
		}

		public static ADObjectId SettingsRelativePath = new ADObjectId("CN=Setting Overrides,CN=Global Settings");

		public static ADObjectId FlightsRelativePath = new ADObjectId("CN=Flight Overrides,CN=Global Settings");

		private static SettingOverrideSchema schema = ObjectSchema.GetInstance<SettingOverrideSchema>();

		private static string mostDerivedClass = "msExchConfigSettings";
	}
}

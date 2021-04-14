using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsRequest
{
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[XmlType(Namespace = "HMSETTINGS:")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class ServiceSettingsType
	{
		public string SafetySchemaVersion
		{
			get
			{
				return this.safetySchemaVersionField;
			}
			set
			{
				this.safetySchemaVersionField = value;
			}
		}

		public ServiceSettingsTypeSafetyLevelRules SafetyLevelRules
		{
			get
			{
				return this.safetyLevelRulesField;
			}
			set
			{
				this.safetyLevelRulesField = value;
			}
		}

		public ServiceSettingsTypeSafetyActions SafetyActions
		{
			get
			{
				return this.safetyActionsField;
			}
			set
			{
				this.safetyActionsField = value;
			}
		}

		public ServiceSettingsTypeProperties Properties
		{
			get
			{
				return this.propertiesField;
			}
			set
			{
				this.propertiesField = value;
			}
		}

		public ServiceSettingsTypeLists Lists
		{
			get
			{
				return this.listsField;
			}
			set
			{
				this.listsField = value;
			}
		}

		private string safetySchemaVersionField;

		private ServiceSettingsTypeSafetyLevelRules safetyLevelRulesField;

		private ServiceSettingsTypeSafetyActions safetyActionsField;

		private ServiceSettingsTypeProperties propertiesField;

		private ServiceSettingsTypeLists listsField;
	}
}

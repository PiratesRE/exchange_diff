using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsResponse
{
	[XmlType(AnonymousType = true, Namespace = "HMSETTINGS:")]
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[DebuggerStepThrough]
	[Serializable]
	public class SettingsServiceSettings
	{
		public int Status
		{
			get
			{
				return this.statusField;
			}
			set
			{
				this.statusField = value;
			}
		}

		public RulesResponseType SafetyLevelRules
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

		public RulesResponseType SafetyActions
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

		public SettingsServiceSettingsProperties Properties
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

		public SettingsServiceSettingsLists Lists
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

		private int statusField;

		private RulesResponseType safetyLevelRulesField;

		private RulesResponseType safetyActionsField;

		private SettingsServiceSettingsProperties propertiesField;

		private SettingsServiceSettingsLists listsField;
	}
}

using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[XmlType(TypeName = "ServiceSettingsType", Namespace = "HMSETTINGS:")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class ServiceSettingsType
	{
		[XmlIgnore]
		public string SafetySchemaVersion
		{
			get
			{
				return this.internalSafetySchemaVersion;
			}
			set
			{
				this.internalSafetySchemaVersion = value;
			}
		}

		[XmlIgnore]
		public SafetyLevelRules SafetyLevelRules
		{
			get
			{
				if (this.internalSafetyLevelRules == null)
				{
					this.internalSafetyLevelRules = new SafetyLevelRules();
				}
				return this.internalSafetyLevelRules;
			}
			set
			{
				this.internalSafetyLevelRules = value;
			}
		}

		[XmlIgnore]
		public SafetyActions SafetyActions
		{
			get
			{
				if (this.internalSafetyActions == null)
				{
					this.internalSafetyActions = new SafetyActions();
				}
				return this.internalSafetyActions;
			}
			set
			{
				this.internalSafetyActions = value;
			}
		}

		[XmlIgnore]
		public Properties Properties
		{
			get
			{
				if (this.internalProperties == null)
				{
					this.internalProperties = new Properties();
				}
				return this.internalProperties;
			}
			set
			{
				this.internalProperties = value;
			}
		}

		[XmlIgnore]
		public Lists Lists
		{
			get
			{
				if (this.internalLists == null)
				{
					this.internalLists = new Lists();
				}
				return this.internalLists;
			}
			set
			{
				this.internalLists = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "SafetySchemaVersion", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMSETTINGS:")]
		public string internalSafetySchemaVersion;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(SafetyLevelRules), ElementName = "SafetyLevelRules", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public SafetyLevelRules internalSafetyLevelRules;

		[XmlElement(Type = typeof(SafetyActions), ElementName = "SafetyActions", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public SafetyActions internalSafetyActions;

		[XmlElement(Type = typeof(Properties), ElementName = "Properties", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Properties internalProperties;

		[XmlElement(Type = typeof(Lists), ElementName = "Lists", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Lists internalLists;
	}
}

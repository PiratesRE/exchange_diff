using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[XmlType(TypeName = "ServiceSettings", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class ServiceSettings
	{
		[XmlIgnore]
		public int Status
		{
			get
			{
				return this.internalStatus;
			}
			set
			{
				this.internalStatus = value;
				this.internalStatusSpecified = true;
			}
		}

		[XmlIgnore]
		public RulesResponseType SafetyLevelRules
		{
			get
			{
				if (this.internalSafetyLevelRules == null)
				{
					this.internalSafetyLevelRules = new RulesResponseType();
				}
				return this.internalSafetyLevelRules;
			}
			set
			{
				this.internalSafetyLevelRules = value;
			}
		}

		[XmlIgnore]
		public RulesResponseType SafetyActions
		{
			get
			{
				if (this.internalSafetyActions == null)
				{
					this.internalSafetyActions = new RulesResponseType();
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

		[XmlElement(ElementName = "Status", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalStatus;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalStatusSpecified;

		[XmlElement(Type = typeof(RulesResponseType), ElementName = "SafetyLevelRules", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public RulesResponseType internalSafetyLevelRules;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(RulesResponseType), ElementName = "SafetyActions", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public RulesResponseType internalSafetyActions;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(Properties), ElementName = "Properties", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public Properties internalProperties;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(Lists), ElementName = "Lists", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public Lists internalLists;
	}
}

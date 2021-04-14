using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[XmlType(TypeName = "SafetyActions", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class SafetyActions
	{
		[XmlIgnore]
		public SafetyActionsGetVersion GetVersion
		{
			get
			{
				if (this.internalGetVersion == null)
				{
					this.internalGetVersion = new SafetyActionsGetVersion();
				}
				return this.internalGetVersion;
			}
			set
			{
				this.internalGetVersion = value;
			}
		}

		[XmlIgnore]
		public SafetyActionsGet Get
		{
			get
			{
				if (this.internalGet == null)
				{
					this.internalGet = new SafetyActionsGet();
				}
				return this.internalGet;
			}
			set
			{
				this.internalGet = value;
			}
		}

		[XmlElement(Type = typeof(SafetyActionsGetVersion), ElementName = "GetVersion", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public SafetyActionsGetVersion internalGetVersion;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(SafetyActionsGet), ElementName = "Get", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public SafetyActionsGet internalGet;
	}
}

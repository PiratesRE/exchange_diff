using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[XmlType(TypeName = "SafetyLevelRules", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class SafetyLevelRules
	{
		[XmlIgnore]
		public GetVersion GetVersion
		{
			get
			{
				if (this.internalGetVersion == null)
				{
					this.internalGetVersion = new GetVersion();
				}
				return this.internalGetVersion;
			}
			set
			{
				this.internalGetVersion = value;
			}
		}

		[XmlIgnore]
		public Get Get
		{
			get
			{
				if (this.internalGet == null)
				{
					this.internalGet = new Get();
				}
				return this.internalGet;
			}
			set
			{
				this.internalGet = value;
			}
		}

		[XmlElement(Type = typeof(GetVersion), ElementName = "GetVersion", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public GetVersion internalGetVersion;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(Get), ElementName = "Get", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public Get internalGet;
	}
}

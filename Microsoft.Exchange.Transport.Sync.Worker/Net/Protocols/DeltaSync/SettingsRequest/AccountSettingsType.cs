using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[XmlType(TypeName = "AccountSettingsType", Namespace = "HMSETTINGS:")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class AccountSettingsType
	{
		[XmlIgnore]
		public AccountSettingsTypeGet Get
		{
			get
			{
				if (this.internalGet == null)
				{
					this.internalGet = new AccountSettingsTypeGet();
				}
				return this.internalGet;
			}
			set
			{
				this.internalGet = value;
			}
		}

		[XmlIgnore]
		public Set Set
		{
			get
			{
				if (this.internalSet == null)
				{
					this.internalSet = new Set();
				}
				return this.internalSet;
			}
			set
			{
				this.internalSet = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(AccountSettingsTypeGet), ElementName = "Get", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public AccountSettingsTypeGet internalGet;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(Set), ElementName = "Set", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public Set internalSet;
	}
}

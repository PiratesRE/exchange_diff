using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[XmlType(TypeName = "AccountSettings", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class AccountSettings
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
		public AccountSettingsGet Get
		{
			get
			{
				if (this.internalGet == null)
				{
					this.internalGet = new AccountSettingsGet();
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

		[XmlElement(ElementName = "Status", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalStatus;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalStatusSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(AccountSettingsGet), ElementName = "Get", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public AccountSettingsGet internalGet;

		[XmlElement(Type = typeof(Set), ElementName = "Set", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Set internalSet;
	}
}

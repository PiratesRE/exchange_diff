using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[XmlType(TypeName = "Lists", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class Lists
	{
		[XmlIgnore]
		public ListsGet Get
		{
			get
			{
				if (this.internalGet == null)
				{
					this.internalGet = new ListsGet();
				}
				return this.internalGet;
			}
			set
			{
				this.internalGet = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(ListsGet), ElementName = "Get", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public ListsGet internalGet;
	}
}

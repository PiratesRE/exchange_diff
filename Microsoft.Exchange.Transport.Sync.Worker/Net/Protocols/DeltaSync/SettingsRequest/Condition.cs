using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[XmlType(TypeName = "Condition", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class Condition
	{
		[XmlIgnore]
		public Clause Clause
		{
			get
			{
				if (this.internalClause == null)
				{
					this.internalClause = new Clause();
				}
				return this.internalClause;
			}
			set
			{
				this.internalClause = value;
			}
		}

		[XmlElement(Type = typeof(Clause), ElementName = "Clause", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Clause internalClause;
	}
}

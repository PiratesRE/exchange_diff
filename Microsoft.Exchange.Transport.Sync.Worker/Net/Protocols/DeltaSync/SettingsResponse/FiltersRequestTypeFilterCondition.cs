using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[XmlType(TypeName = "FiltersRequestTypeFilterCondition", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class FiltersRequestTypeFilterCondition
	{
		[XmlIgnore]
		public FiltersRequestTypeFilterConditionClause Clause
		{
			get
			{
				if (this.internalClause == null)
				{
					this.internalClause = new FiltersRequestTypeFilterConditionClause();
				}
				return this.internalClause;
			}
			set
			{
				this.internalClause = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(FiltersRequestTypeFilterConditionClause), ElementName = "Clause", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public FiltersRequestTypeFilterConditionClause internalClause;
	}
}

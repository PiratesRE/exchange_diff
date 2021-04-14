using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsRequest
{
	[XmlType(TypeName = "Options", Namespace = "ItemOperations:")]
	[Serializable]
	public class Options
	{
		[XmlIgnore]
		public Report Report
		{
			get
			{
				if (this.internalReport == null)
				{
					this.internalReport = new Report();
				}
				return this.internalReport;
			}
			set
			{
				this.internalReport = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(Report), ElementName = "Report", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		public Report internalReport;
	}
}

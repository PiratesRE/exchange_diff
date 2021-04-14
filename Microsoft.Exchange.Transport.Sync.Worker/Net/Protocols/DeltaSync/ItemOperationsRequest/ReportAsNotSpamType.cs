using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsRequest
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[XmlType(TypeName = "ReportAsNotSpamType", Namespace = "ItemOperations:")]
	[Serializable]
	public class ReportAsNotSpamType : ItemOpsBaseType
	{
		[XmlIgnore]
		public Options Options
		{
			get
			{
				if (this.internalOptions == null)
				{
					this.internalOptions = new Options();
				}
				return this.internalOptions;
			}
			set
			{
				this.internalOptions = value;
			}
		}

		[XmlElement(Type = typeof(Options), ElementName = "Options", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Options internalOptions;
	}
}

using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[XmlType(TypeName = "FiltersRequestTypeFilterActionsMoveToFolder", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class FiltersRequestTypeFilterActionsMoveToFolder
	{
		[XmlIgnore]
		public string FolderId
		{
			get
			{
				return this.internalFolderId;
			}
			set
			{
				this.internalFolderId = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "FolderId", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMSETTINGS:")]
		public string internalFolderId;
	}
}

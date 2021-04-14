using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[XmlType(TypeName = "Actions", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class Actions
	{
		[XmlIgnore]
		public MoveToFolder MoveToFolder
		{
			get
			{
				if (this.internalMoveToFolder == null)
				{
					this.internalMoveToFolder = new MoveToFolder();
				}
				return this.internalMoveToFolder;
			}
			set
			{
				this.internalMoveToFolder = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(MoveToFolder), ElementName = "MoveToFolder", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public MoveToFolder internalMoveToFolder;
	}
}

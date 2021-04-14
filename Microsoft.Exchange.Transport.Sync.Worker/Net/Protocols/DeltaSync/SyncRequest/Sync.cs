using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Net.Protocols.DeltaSync.HMSync;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SyncRequest
{
	[XmlRoot(ElementName = "Sync", Namespace = "AirSync:", IsNullable = false)]
	[Serializable]
	public class Sync
	{
		[XmlIgnore]
		public DeletesAsMoves DeletesAsMoves
		{
			get
			{
				if (this.internalDeletesAsMoves == null)
				{
					this.internalDeletesAsMoves = new DeletesAsMoves();
				}
				return this.internalDeletesAsMoves;
			}
			set
			{
				this.internalDeletesAsMoves = value;
			}
		}

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

		[XmlIgnore]
		public Collections Collections
		{
			get
			{
				if (this.internalCollections == null)
				{
					this.internalCollections = new Collections();
				}
				return this.internalCollections;
			}
			set
			{
				this.internalCollections = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(DeletesAsMoves), ElementName = "DeletesAsMoves", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSYNC:")]
		public DeletesAsMoves internalDeletesAsMoves;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(Options), ElementName = "Options", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSYNC:")]
		public Options internalOptions;

		[XmlElement(Type = typeof(Collections), ElementName = "Collections", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "AirSync:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Collections internalCollections;
	}
}

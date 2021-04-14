using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessResponse
{
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "DeltaSyncV2:")]
	[DebuggerStepThrough]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[Serializable]
	public class StatelessCollectionCommands
	{
		[XmlElement(Namespace = "HMSYNC:")]
		public int Status
		{
			get
			{
				return this.statusField;
			}
			set
			{
				this.statusField = value;
			}
		}

		[XmlElement("Delete")]
		public StatelessCollectionCommandsDelete[] Delete
		{
			get
			{
				return this.deleteField;
			}
			set
			{
				this.deleteField = value;
			}
		}

		[XmlElement("Change")]
		public StatelessCollectionCommandsChange[] Change
		{
			get
			{
				return this.changeField;
			}
			set
			{
				this.changeField = value;
			}
		}

		[XmlElement("Add")]
		public StatelessCollectionCommandsAdd[] Add
		{
			get
			{
				return this.addField;
			}
			set
			{
				this.addField = value;
			}
		}

		private int statusField;

		private StatelessCollectionCommandsDelete[] deleteField;

		private StatelessCollectionCommandsChange[] changeField;

		private StatelessCollectionCommandsAdd[] addField;
	}
}

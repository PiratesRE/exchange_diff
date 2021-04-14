using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessRequest
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[XmlType(AnonymousType = true, Namespace = "DeltaSyncV2:")]
	[Serializable]
	public class StatelessCollectionCommands
	{
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

		private StatelessCollectionCommandsChange[] changeField;

		private StatelessCollectionCommandsDelete[] deleteField;

		private StatelessCollectionCommandsAdd[] addField;
	}
}

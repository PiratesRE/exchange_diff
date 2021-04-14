using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessResponse
{
	[XmlType(AnonymousType = true, Namespace = "DeltaSyncV2:")]
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DebuggerStepThrough]
	[Serializable]
	public class StatelessCollectionResponses
	{
		[XmlElement("Change")]
		public StatelessCollectionResponsesChange[] Change
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
		public StatelessCollectionResponsesDelete[] Delete
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
		public StatelessCollectionResponsesAdd[] Add
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

		private StatelessCollectionResponsesChange[] changeField;

		private StatelessCollectionResponsesDelete[] deleteField;

		private StatelessCollectionResponsesAdd[] addField;
	}
}

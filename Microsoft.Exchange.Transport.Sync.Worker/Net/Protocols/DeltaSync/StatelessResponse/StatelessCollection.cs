using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessResponse
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true, Namespace = "DeltaSyncV2:")]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[Serializable]
	public class StatelessCollection
	{
		public string Class
		{
			get
			{
				return this.classField;
			}
			set
			{
				this.classField = value;
			}
		}

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

		public StatelessCollectionCommands Commands
		{
			get
			{
				return this.commandsField;
			}
			set
			{
				this.commandsField = value;
			}
		}

		public StatelessCollectionResponses Responses
		{
			get
			{
				return this.responsesField;
			}
			set
			{
				this.responsesField = value;
			}
		}

		private string classField;

		private int statusField;

		private StatelessCollectionCommands commandsField;

		private StatelessCollectionResponses responsesField;
	}
}

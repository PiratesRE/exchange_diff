using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessRequest
{
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[XmlType(AnonymousType = true, Namespace = "DeltaSyncV2:")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
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

		public StatelessCollectionGet Get
		{
			get
			{
				return this.getField;
			}
			set
			{
				this.getField = value;
			}
		}

		public uint WindowSize
		{
			get
			{
				return this.windowSizeField;
			}
			set
			{
				this.windowSizeField = value;
			}
		}

		[XmlIgnore]
		public bool WindowSizeSpecified
		{
			get
			{
				return this.windowSizeFieldSpecified;
			}
			set
			{
				this.windowSizeFieldSpecified = value;
			}
		}

		public string CollectionId
		{
			get
			{
				return this.collectionIdField;
			}
			set
			{
				this.collectionIdField = value;
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

		private string classField;

		private StatelessCollectionGet getField;

		private uint windowSizeField;

		private bool windowSizeFieldSpecified;

		private string collectionIdField;

		private StatelessCollectionCommands commandsField;
	}
}

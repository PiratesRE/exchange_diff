using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessRequest
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "DeltaSyncV2:")]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[Serializable]
	public class StatelessCollectionCommandsDelete
	{
		public string ServerId
		{
			get
			{
				return this.serverIdField;
			}
			set
			{
				this.serverIdField = value;
			}
		}

		[XmlElement(Namespace = "HMMAIL:")]
		public string SourceFolderId
		{
			get
			{
				return this.sourceFolderIdField;
			}
			set
			{
				this.sourceFolderIdField = value;
			}
		}

		[XmlElement(Namespace = "HMSYNC:")]
		public DeletesAsMoves DeletesAsMoves
		{
			get
			{
				return this.deletesAsMovesField;
			}
			set
			{
				this.deletesAsMovesField = value;
			}
		}

		private string serverIdField;

		private string sourceFolderIdField;

		private DeletesAsMoves deletesAsMovesField;
	}
}

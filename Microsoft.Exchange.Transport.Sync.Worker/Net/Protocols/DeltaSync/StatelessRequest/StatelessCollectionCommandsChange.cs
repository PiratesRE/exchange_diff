using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessRequest
{
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true, Namespace = "DeltaSyncV2:")]
	[Serializable]
	public class StatelessCollectionCommandsChange
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

		public ApplicationDataTypeRequest ApplicationData
		{
			get
			{
				return this.applicationDataField;
			}
			set
			{
				this.applicationDataField = value;
			}
		}

		private string serverIdField;

		private string sourceFolderIdField;

		private ApplicationDataTypeRequest applicationDataField;
	}
}

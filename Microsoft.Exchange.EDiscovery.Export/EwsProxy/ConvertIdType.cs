using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class ConvertIdType : BaseRequestType
	{
		[XmlArrayItem("AlternatePublicFolderId", typeof(AlternatePublicFolderIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("AlternateId", typeof(AlternateIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("AlternatePublicFolderItemId", typeof(AlternatePublicFolderItemIdType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public AlternateIdBaseType[] SourceIds
		{
			get
			{
				return this.sourceIdsField;
			}
			set
			{
				this.sourceIdsField = value;
			}
		}

		[XmlAttribute]
		public IdFormatType DestinationFormat
		{
			get
			{
				return this.destinationFormatField;
			}
			set
			{
				this.destinationFormatField = value;
			}
		}

		private AlternateIdBaseType[] sourceIdsField;

		private IdFormatType destinationFormatField;
	}
}

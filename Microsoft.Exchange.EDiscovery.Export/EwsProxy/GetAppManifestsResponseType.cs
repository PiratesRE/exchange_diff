using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class GetAppManifestsResponseType : ResponseMessageType
	{
		[XmlArrayItem("Manifest", DataType = "base64Binary", IsNullable = false)]
		public byte[][] Manifests
		{
			get
			{
				return this.manifestsField;
			}
			set
			{
				this.manifestsField = value;
			}
		}

		private byte[][] manifestsField;
	}
}

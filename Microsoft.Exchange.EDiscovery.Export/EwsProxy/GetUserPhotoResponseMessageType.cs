using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class GetUserPhotoResponseMessageType : ResponseMessageType
	{
		public bool HasChanged
		{
			get
			{
				return this.hasChangedField;
			}
			set
			{
				this.hasChangedField = value;
			}
		}

		[XmlElement(DataType = "base64Binary")]
		public byte[] PictureData
		{
			get
			{
				return this.pictureDataField;
			}
			set
			{
				this.pictureDataField = value;
			}
		}

		private bool hasChangedField;

		private byte[] pictureDataField;
	}
}

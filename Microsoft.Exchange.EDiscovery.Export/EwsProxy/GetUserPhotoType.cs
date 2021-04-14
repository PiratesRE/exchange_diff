using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class GetUserPhotoType : BaseRequestType
	{
		public string Email
		{
			get
			{
				return this.emailField;
			}
			set
			{
				this.emailField = value;
			}
		}

		public UserPhotoSizeType SizeRequested
		{
			get
			{
				return this.sizeRequestedField;
			}
			set
			{
				this.sizeRequestedField = value;
			}
		}

		private string emailField;

		private UserPhotoSizeType sizeRequestedField;
	}
}

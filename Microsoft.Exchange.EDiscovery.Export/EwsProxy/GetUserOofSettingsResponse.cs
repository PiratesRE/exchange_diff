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
	public class GetUserOofSettingsResponse
	{
		public ResponseMessageType ResponseMessage
		{
			get
			{
				return this.responseMessageField;
			}
			set
			{
				this.responseMessageField = value;
			}
		}

		[XmlElement(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public UserOofSettings OofSettings
		{
			get
			{
				return this.oofSettingsField;
			}
			set
			{
				this.oofSettingsField = value;
			}
		}

		public ExternalAudience AllowExternalOof
		{
			get
			{
				return this.allowExternalOofField;
			}
			set
			{
				this.allowExternalOofField = value;
			}
		}

		[XmlIgnore]
		public bool AllowExternalOofSpecified
		{
			get
			{
				return this.allowExternalOofFieldSpecified;
			}
			set
			{
				this.allowExternalOofFieldSpecified = value;
			}
		}

		private ResponseMessageType responseMessageField;

		private UserOofSettings oofSettingsField;

		private ExternalAudience allowExternalOofField;

		private bool allowExternalOofFieldSpecified;
	}
}

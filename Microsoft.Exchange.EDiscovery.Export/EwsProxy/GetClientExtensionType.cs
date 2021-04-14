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
	public class GetClientExtensionType : BaseRequestType
	{
		[XmlArrayItem("String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] RequestedExtensionIds
		{
			get
			{
				return this.requestedExtensionIdsField;
			}
			set
			{
				this.requestedExtensionIdsField = value;
			}
		}

		public GetClientExtensionUserParametersType UserParameters
		{
			get
			{
				return this.userParametersField;
			}
			set
			{
				this.userParametersField = value;
			}
		}

		public bool IsDebug
		{
			get
			{
				return this.isDebugField;
			}
			set
			{
				this.isDebugField = value;
			}
		}

		[XmlIgnore]
		public bool IsDebugSpecified
		{
			get
			{
				return this.isDebugFieldSpecified;
			}
			set
			{
				this.isDebugFieldSpecified = value;
			}
		}

		private string[] requestedExtensionIdsField;

		private GetClientExtensionUserParametersType userParametersField;

		private bool isDebugField;

		private bool isDebugFieldSpecified;
	}
}

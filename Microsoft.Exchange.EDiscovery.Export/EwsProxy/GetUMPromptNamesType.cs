using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[Serializable]
	public class GetUMPromptNamesType : BaseRequestType
	{
		public string ConfigurationObject
		{
			get
			{
				return this.configurationObjectField;
			}
			set
			{
				this.configurationObjectField = value;
			}
		}

		public int HoursElapsedSinceLastModified
		{
			get
			{
				return this.hoursElapsedSinceLastModifiedField;
			}
			set
			{
				this.hoursElapsedSinceLastModifiedField = value;
			}
		}

		private string configurationObjectField;

		private int hoursElapsedSinceLastModifiedField;
	}
}

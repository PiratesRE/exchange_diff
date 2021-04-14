using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[Serializable]
	public class XmlValueThrottleLimit
	{
		public ThrottleLimitValue ThrottleLimit
		{
			get
			{
				return this.throttleLimitField;
			}
			set
			{
				this.throttleLimitField = value;
			}
		}

		private ThrottleLimitValue throttleLimitField;
	}
}

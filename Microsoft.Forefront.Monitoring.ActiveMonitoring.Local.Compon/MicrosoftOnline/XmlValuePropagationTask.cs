using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class XmlValuePropagationTask
	{
		public PropagationTaskValue PropagationTask
		{
			get
			{
				return this.propagationTaskField;
			}
			set
			{
				this.propagationTaskField = value;
			}
		}

		private PropagationTaskValue propagationTaskField;
	}
}

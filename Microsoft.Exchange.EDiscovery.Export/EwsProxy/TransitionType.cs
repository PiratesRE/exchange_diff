using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(RecurringDateTransitionType))]
	[DebuggerStepThrough]
	[XmlInclude(typeof(RecurringDayTransitionType))]
	[XmlInclude(typeof(RecurringTimeTransitionType))]
	[XmlInclude(typeof(AbsoluteDateTransitionType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class TransitionType
	{
		public TransitionTargetType To
		{
			get
			{
				return this.toField;
			}
			set
			{
				this.toField = value;
			}
		}

		private TransitionTargetType toField;
	}
}

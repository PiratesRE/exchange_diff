using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[Serializable]
	public class StrongAuthenticationRuleValue
	{
		[XmlArrayItem("SelectionCondition", IsNullable = false)]
		public SelectionConditionValue[] SelectionConditions
		{
			get
			{
				return this.selectionConditionsField;
			}
			set
			{
				this.selectionConditionsField = value;
			}
		}

		private SelectionConditionValue[] selectionConditionsField;
	}
}

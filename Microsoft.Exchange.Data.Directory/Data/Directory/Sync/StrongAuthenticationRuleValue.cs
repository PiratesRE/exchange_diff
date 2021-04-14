using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class StrongAuthenticationRuleValue
	{
		[XmlArray(Order = 0)]
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

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class TimeZoneDefinitionType
	{
		[XmlArrayItem("Period", IsNullable = false)]
		public PeriodType[] Periods
		{
			get
			{
				return this.periodsField;
			}
			set
			{
				this.periodsField = value;
			}
		}

		[XmlArrayItem("TransitionsGroup", IsNullable = false)]
		public ArrayOfTransitionsType[] TransitionsGroups
		{
			get
			{
				return this.transitionsGroupsField;
			}
			set
			{
				this.transitionsGroupsField = value;
			}
		}

		public ArrayOfTransitionsType Transitions
		{
			get
			{
				return this.transitionsField;
			}
			set
			{
				this.transitionsField = value;
			}
		}

		[XmlAttribute]
		public string Id
		{
			get
			{
				return this.idField;
			}
			set
			{
				this.idField = value;
			}
		}

		[XmlAttribute]
		public string Name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		private PeriodType[] periodsField;

		private ArrayOfTransitionsType[] transitionsGroupsField;

		private ArrayOfTransitionsType transitionsField;

		private string idField;

		private string nameField;
	}
}

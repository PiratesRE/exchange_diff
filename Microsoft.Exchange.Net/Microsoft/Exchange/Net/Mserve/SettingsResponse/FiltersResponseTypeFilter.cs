using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsResponse
{
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true, Namespace = "HMSETTINGS:")]
	[Serializable]
	public class FiltersResponseTypeFilter
	{
		public int ExecutionOrder
		{
			get
			{
				return this.executionOrderField;
			}
			set
			{
				this.executionOrderField = value;
			}
		}

		public byte Enabled
		{
			get
			{
				return this.enabledField;
			}
			set
			{
				this.enabledField = value;
			}
		}

		public RunWhenType RunWhen
		{
			get
			{
				return this.runWhenField;
			}
			set
			{
				this.runWhenField = value;
			}
		}

		public FiltersResponseTypeFilterCondition Condition
		{
			get
			{
				return this.conditionField;
			}
			set
			{
				this.conditionField = value;
			}
		}

		public FiltersResponseTypeFilterActions Actions
		{
			get
			{
				return this.actionsField;
			}
			set
			{
				this.actionsField = value;
			}
		}

		private int executionOrderField;

		private byte enabledField;

		private RunWhenType runWhenField;

		private FiltersResponseTypeFilterCondition conditionField;

		private FiltersResponseTypeFilterActions actionsField;
	}
}

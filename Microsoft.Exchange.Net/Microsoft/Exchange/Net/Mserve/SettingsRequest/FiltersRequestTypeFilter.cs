using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsRequest
{
	[XmlType(AnonymousType = true, Namespace = "HMSETTINGS:")]
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class FiltersRequestTypeFilter
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

		public FiltersRequestTypeFilterCondition Condition
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

		public FiltersRequestTypeFilterActions Actions
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

		private FiltersRequestTypeFilterCondition conditionField;

		private FiltersRequestTypeFilterActions actionsField;
	}
}

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
	public class RuleType
	{
		public string RuleId
		{
			get
			{
				return this.ruleIdField;
			}
			set
			{
				this.ruleIdField = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayNameField;
			}
			set
			{
				this.displayNameField = value;
			}
		}

		public int Priority
		{
			get
			{
				return this.priorityField;
			}
			set
			{
				this.priorityField = value;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return this.isEnabledField;
			}
			set
			{
				this.isEnabledField = value;
			}
		}

		public bool IsNotSupported
		{
			get
			{
				return this.isNotSupportedField;
			}
			set
			{
				this.isNotSupportedField = value;
			}
		}

		[XmlIgnore]
		public bool IsNotSupportedSpecified
		{
			get
			{
				return this.isNotSupportedFieldSpecified;
			}
			set
			{
				this.isNotSupportedFieldSpecified = value;
			}
		}

		public bool IsInError
		{
			get
			{
				return this.isInErrorField;
			}
			set
			{
				this.isInErrorField = value;
			}
		}

		[XmlIgnore]
		public bool IsInErrorSpecified
		{
			get
			{
				return this.isInErrorFieldSpecified;
			}
			set
			{
				this.isInErrorFieldSpecified = value;
			}
		}

		public RulePredicatesType Conditions
		{
			get
			{
				return this.conditionsField;
			}
			set
			{
				this.conditionsField = value;
			}
		}

		public RulePredicatesType Exceptions
		{
			get
			{
				return this.exceptionsField;
			}
			set
			{
				this.exceptionsField = value;
			}
		}

		public RuleActionsType Actions
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

		private string ruleIdField;

		private string displayNameField;

		private int priorityField;

		private bool isEnabledField;

		private bool isNotSupportedField;

		private bool isNotSupportedFieldSpecified;

		private bool isInErrorField;

		private bool isInErrorFieldSpecified;

		private RulePredicatesType conditionsField;

		private RulePredicatesType exceptionsField;

		private RuleActionsType actionsField;
	}
}

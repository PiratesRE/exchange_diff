using System;
using System.Configuration;
using System.Web;

namespace Microsoft.Exchange.Security
{
	[ConfigurationCollection(typeof(DenyRuleUserPattern), AddItemName = "Value")]
	public class DenyRuleUserPatternCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new DenyRuleUserPattern();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((DenyRuleUserPattern)element).Value;
		}

		[ConfigurationProperty("Operator", IsRequired = true)]
		public DenyRuleOperator OperatorType
		{
			get
			{
				return (DenyRuleOperator)base["Operator"];
			}
			set
			{
				base["Operator"] = value;
			}
		}

		internal bool TryLoad()
		{
			foreach (object obj in this)
			{
				DenyRuleUserPattern denyRuleUserPattern = (DenyRuleUserPattern)obj;
				if (!denyRuleUserPattern.TryLoad())
				{
					return false;
				}
			}
			return true;
		}

		internal bool Evaluate(HttpContext httpContext)
		{
			if (string.IsNullOrEmpty(httpContext.Request.ServerVariables["LOGON_USER"]))
			{
				return this.OperatorType != DenyRuleOperator.In;
			}
			if (this.OperatorType == DenyRuleOperator.In)
			{
				foreach (object obj in this)
				{
					DenyRuleUserPattern denyRuleUserPattern = (DenyRuleUserPattern)obj;
					if (denyRuleUserPattern.Evaluate(httpContext.Request.ServerVariables["LOGON_USER"]))
					{
						return true;
					}
				}
				return false;
			}
			foreach (object obj2 in this)
			{
				DenyRuleUserPattern denyRuleUserPattern2 = (DenyRuleUserPattern)obj2;
				if (denyRuleUserPattern2.Evaluate(httpContext.Request.ServerVariables["LOGON_USER"]))
				{
					return false;
				}
			}
			return true;
		}

		private const string requestHeaderLogonUserFieldName = "LOGON_USER";
	}
}

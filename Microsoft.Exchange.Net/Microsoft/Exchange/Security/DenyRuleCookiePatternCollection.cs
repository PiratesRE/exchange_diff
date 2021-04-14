using System;
using System.Configuration;
using System.Web;

namespace Microsoft.Exchange.Security
{
	[ConfigurationCollection(typeof(DenyRuleCookiePattern), AddItemName = "Value")]
	public class DenyRuleCookiePatternCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new DenyRuleCookiePattern();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((DenyRuleCookiePattern)element).Value;
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
				DenyRuleCookiePattern denyRuleCookiePattern = (DenyRuleCookiePattern)obj;
				if (!denyRuleCookiePattern.TryLoad())
				{
					return false;
				}
			}
			return true;
		}

		internal bool Evaluate(HttpContext httpContext)
		{
			if (httpContext.Request.Headers == null || httpContext.Request.Headers["Cookie"] == null)
			{
				return this.OperatorType != DenyRuleOperator.In;
			}
			if (this.OperatorType == DenyRuleOperator.In)
			{
				foreach (object obj in this)
				{
					DenyRuleCookiePattern denyRuleCookiePattern = (DenyRuleCookiePattern)obj;
					if (denyRuleCookiePattern.Evaluate(httpContext.Request.Headers["Cookie"]))
					{
						return true;
					}
				}
				return false;
			}
			foreach (object obj2 in this)
			{
				DenyRuleCookiePattern denyRuleCookiePattern2 = (DenyRuleCookiePattern)obj2;
				if (denyRuleCookiePattern2.Evaluate(httpContext.Request.Headers["Cookie"]))
				{
					return false;
				}
			}
			return true;
		}

		private const string requestHeaderCookieFieldName = "Cookie";
	}
}

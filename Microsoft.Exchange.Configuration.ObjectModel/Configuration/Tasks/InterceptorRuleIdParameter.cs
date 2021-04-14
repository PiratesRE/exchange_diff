using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class InterceptorRuleIdParameter : ADIdParameter
	{
		public InterceptorRuleIdParameter()
		{
		}

		public InterceptorRuleIdParameter(string identity) : base(identity)
		{
		}

		public InterceptorRuleIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public InterceptorRuleIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static explicit operator string(InterceptorRuleIdParameter interceptorRuleIdParameter)
		{
			return interceptorRuleIdParameter.ToString();
		}

		public static InterceptorRuleIdParameter Parse(string identity)
		{
			return new InterceptorRuleIdParameter(identity);
		}
	}
}

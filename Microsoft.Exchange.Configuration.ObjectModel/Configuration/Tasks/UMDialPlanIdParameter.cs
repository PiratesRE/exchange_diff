using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class UMDialPlanIdParameter : ADIdParameter
	{
		public UMDialPlanIdParameter()
		{
		}

		public UMDialPlanIdParameter(string identity) : base(identity)
		{
		}

		public UMDialPlanIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public UMDialPlanIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static UMDialPlanIdParameter Parse(string identity)
		{
			return new UMDialPlanIdParameter(identity);
		}
	}
}

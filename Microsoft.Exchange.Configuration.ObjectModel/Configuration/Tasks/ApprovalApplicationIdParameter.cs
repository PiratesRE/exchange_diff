using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ApprovalApplicationIdParameter : ADIdParameter
	{
		public ApprovalApplicationIdParameter()
		{
		}

		public ApprovalApplicationIdParameter(string identity) : base(identity)
		{
		}

		public ApprovalApplicationIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ApprovalApplicationIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static ApprovalApplicationIdParameter Parse(string identity)
		{
			return new ApprovalApplicationIdParameter(identity);
		}
	}
}

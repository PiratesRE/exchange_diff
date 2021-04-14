using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ApprovalApplicationContainerIdParameter : ADIdParameter
	{
		public ApprovalApplicationContainerIdParameter()
		{
		}

		public ApprovalApplicationContainerIdParameter(string identity) : base(identity)
		{
		}

		public ApprovalApplicationContainerIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ApprovalApplicationContainerIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static ApprovalApplicationContainerIdParameter Parse(string identity)
		{
			return new ApprovalApplicationContainerIdParameter(identity);
		}
	}
}

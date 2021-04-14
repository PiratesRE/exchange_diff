using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "OptionalRPTsPicker")]
	public interface IOptionalRPTsPicker : IGetListService<AllRPTsFilter, OptionalRetentionPolicyTagRow>
	{
	}
}

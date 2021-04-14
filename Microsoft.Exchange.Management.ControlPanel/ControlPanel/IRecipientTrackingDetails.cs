using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "RecipientTrackingDetails")]
	public interface IRecipientTrackingDetails : IGetObjectService<RecipientTrackingEventRow>
	{
	}
}

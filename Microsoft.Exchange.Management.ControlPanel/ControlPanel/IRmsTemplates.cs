using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "RmsTemplates")]
	public interface IRmsTemplates : IGetListService<RmsTemplateFilter, RmsTemplate>
	{
	}
}

using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "MessageCategories")]
	public interface IMessageCategories : IGetListService<MessageCategoryFilter, MessageCategory>
	{
	}
}

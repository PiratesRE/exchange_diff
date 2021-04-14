using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "MailMessages")]
	public interface IMailMessages : INewObjectService<MailMessageRow, NewMailMessage>
	{
	}
}

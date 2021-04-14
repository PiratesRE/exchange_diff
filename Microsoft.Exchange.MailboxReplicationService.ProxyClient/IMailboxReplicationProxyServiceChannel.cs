using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IMailboxReplicationProxyServiceChannel : IMailboxReplicationProxyService, IClientChannel, IContextChannel, IChannel, ICommunicationObject, IExtensibleObject<IContextChannel>, IDisposable
	{
	}
}

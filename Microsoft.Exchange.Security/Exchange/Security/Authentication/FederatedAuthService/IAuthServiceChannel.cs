using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0")]
	internal interface IAuthServiceChannel : IAuthService, IClientChannel, IContextChannel, IChannel, ICommunicationObject, IExtensibleObject<IContextChannel>, IDisposable
	{
	}
}

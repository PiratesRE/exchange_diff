using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MapiHttp;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class Environment : IEnvironment
	{
		public ISampleClient CreateSampleClient()
		{
			throw new NotImplementedException();
		}

		public IVerifyRpcProxyClient CreateVerifyRpcProxyClient(RpcBindingInfo bindingInfo)
		{
			return new VerifyRpcProxyClient(bindingInfo);
		}

		public IEmsmdbClient CreateEmsmdbClient(RpcBindingInfo bindingInfo)
		{
			bindingInfo.AllowImpersonation = true;
			return new EmsmdbClient(bindingInfo);
		}

		public IRfriClient CreateRfriClient(RpcBindingInfo bindingInfo)
		{
			return new RfriClient(bindingInfo);
		}

		public INspiClient CreateNspiClient(RpcBindingInfo bindingInfo)
		{
			return new NspiClient(bindingInfo);
		}

		public IEmsmdbClient CreateEmsmdbClient(MapiHttpBindingInfo bindingInfo)
		{
			return new EmsmdbClient(bindingInfo);
		}
	}
}

using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MapiHttp;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IEnvironment
	{
		ISampleClient CreateSampleClient();

		IVerifyRpcProxyClient CreateVerifyRpcProxyClient(RpcBindingInfo bindingInfo);

		IEmsmdbClient CreateEmsmdbClient(RpcBindingInfo bindingInfo);

		IRfriClient CreateRfriClient(RpcBindingInfo bindingInfo);

		INspiClient CreateNspiClient(RpcBindingInfo bindingInfo);

		IEmsmdbClient CreateEmsmdbClient(MapiHttpBindingInfo bindingInfo);
	}
}

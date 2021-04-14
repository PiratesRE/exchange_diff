using System;

namespace Microsoft.Exchange.Rpc.UM
{
	internal class UMPartnerMessageRpcClient : UMVersionedRpcClientBase
	{
		public UMPartnerMessageRpcClient(string serverFqdn) : base(serverFqdn)
		{
			try
			{
				this.operationName = string.Format("{0}(IUMPartnerMessage.ProcessPartnerMessage)", serverFqdn);
				this.executeRequestDelegate = <Module>.__unep@?cli_ProcessPartnerMessage@@$$J0YAJPEAXHPEAEPEAHPEAPEAE@Z;
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}
	}
}

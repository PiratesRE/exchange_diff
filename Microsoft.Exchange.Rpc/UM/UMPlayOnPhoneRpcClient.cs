using System;

namespace Microsoft.Exchange.Rpc.UM
{
	internal class UMPlayOnPhoneRpcClient : UMVersionedRpcClientBase
	{
		public UMPlayOnPhoneRpcClient(string serverFqdn) : base(serverFqdn)
		{
			try
			{
				this.operationName = string.Format("{0}(IUMPlayOnPhone.ExecutePoPRequest)", serverFqdn);
				this.executeRequestDelegate = <Module>.__unep@?cli_ExecutePoPRequest@@$$J0YAJPEAXHPEAEPEAHPEAPEAE@Z;
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}
	}
}

using System;

namespace Microsoft.Exchange.Rpc.UM
{
	internal class UMPromptPreviewRpcClient : UMVersionedRpcClientBase
	{
		public UMPromptPreviewRpcClient(string serverFqdn) : base(serverFqdn)
		{
			try
			{
				this.operationName = string.Format("{0}(IUMPromptPreview.ExecutePromptPreviewRequest)", serverFqdn);
				this.executeRequestDelegate = <Module>.__unep@?cli_ExecutePromptPreviewRequest@@$$J0YAJPEAXHPEAEPEAHPEAPEAE@Z;
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}
	}
}

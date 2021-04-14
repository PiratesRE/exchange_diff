using System;

namespace Microsoft.Exchange.DxStore.Common
{
	public class DxStoreAccessExceptionTranslator : WcfExceptionTranslator<IDxStoreAccess>
	{
		public override Exception GenerateTransientException(Exception ex)
		{
			throw new DxStoreAccessClientTransientException(ex.Message, ex);
		}

		public override Exception GeneratePermanentException(Exception ex)
		{
			throw new DxStoreAccessClientException(ex.Message, ex);
		}
	}
}

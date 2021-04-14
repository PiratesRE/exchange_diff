using System;

namespace Microsoft.Exchange.DxStore.Common
{
	public class DxStoreManagerExceptionTranslator : WcfExceptionTranslator<IDxStoreManager>
	{
		public override Exception GenerateTransientException(Exception ex)
		{
			throw new DxStoreManagerClientTransientException(ex.Message, ex);
		}

		public override Exception GeneratePermanentException(Exception ex)
		{
			throw new DxStoreManagerClientException(ex.Message, ex);
		}
	}
}

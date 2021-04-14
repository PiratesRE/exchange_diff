using System;

namespace Microsoft.Exchange.DxStore.Common
{
	public class DxStoreInstanceExceptionTranslator : WcfExceptionTranslator<IDxStoreInstance>
	{
		public override Exception GenerateTransientException(Exception ex)
		{
			throw new DxStoreInstanceClientTransientException(ex.Message, ex);
		}

		public override Exception GeneratePermanentException(Exception ex)
		{
			throw new DxStoreInstanceClientException(ex.Message, ex);
		}
	}
}

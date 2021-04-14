using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ADOperationResultWithData<TResult> : ADOperationResult where TResult : class
	{
		public TResult Data { get; private set; }

		public string DomainController { get; private set; }

		public ADOperationResultWithData(string dcName, TResult data, ADOperationErrorCode errorCode, Exception e) : base(errorCode, e)
		{
			this.DomainController = dcName;
			this.Data = data;
		}
	}
}

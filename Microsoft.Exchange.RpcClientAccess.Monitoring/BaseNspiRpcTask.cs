using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class BaseNspiRpcTask : BaseRpcTask
	{
		public BaseNspiRpcTask(IContext context, LocalizedString title, LocalizedString description, TaskType type, params ContextProperty[] dependentProperties) : base(context, title, description, type, dependentProperties)
		{
		}

		protected TaskResult ApplyCallResult(NspiCallResult callResult)
		{
			if (callResult.NspiException != null)
			{
				base.Set<NspiDataException>(ContextPropertySchema.Exception, callResult.NspiException);
			}
			return base.ApplyCallResult(callResult);
		}
	}
}

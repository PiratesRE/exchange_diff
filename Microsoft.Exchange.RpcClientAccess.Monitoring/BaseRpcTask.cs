using System;
using System.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class BaseRpcTask : BaseTask
	{
		public BaseRpcTask(IContext context, LocalizedString title, LocalizedString description, TaskType type, params ContextProperty[] dependentProperties) : base(context, title, description, type, dependentProperties.Concat(new ContextProperty[]
		{
			ContextPropertySchema.Timeout.GetOnly(),
			ContextPropertySchema.Latency.SetOnly(),
			ContextPropertySchema.ActivityContext.SetOnly()
		}).ToArray<ContextProperty>())
		{
		}

		protected TaskResult ApplyCallResult(RpcCallResult callResult)
		{
			if (callResult.ActivityContext != null)
			{
				base.Set<string>(ContextPropertySchema.ActivityContext, callResult.ActivityContext);
			}
			if (callResult.Exception != null)
			{
				base.Set<Exception>(ContextPropertySchema.Exception, callResult.Exception);
			}
			else if (callResult.ErrorCode != ErrorCode.None)
			{
				base.Set<RopExecutionException>(ContextPropertySchema.Exception, new RopExecutionException(Strings.RpcCallResultErrorCodeDescription(callResult.GetType().Name), callResult.ErrorCode, new Exception(callResult.RemoteExceptionTrace)));
			}
			base.Set<TimeSpan>(ContextPropertySchema.Latency, callResult.Latency);
			if (!callResult.IsSuccessful)
			{
				return TaskResult.Failed;
			}
			return TaskResult.Success;
		}
	}
}

using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EmsmdbMapiHttpTask : BaseTask
	{
		public EmsmdbMapiHttpTask(IContext context) : this(context, true)
		{
		}

		public EmsmdbMapiHttpTask(IContext context, bool shouldCallLogon) : base(context, Strings.EmsmdbTaskTitle, Strings.EmsmdbTaskDescription, TaskType.Knowledge, RpcHelper.DependenciesOfBuildMapiHttpBindingInfo.Concat(new ContextProperty[]
		{
			ContextPropertySchema.ActualBinding.SetOnly()
		}))
		{
			this.shouldCallLogon = shouldCallLogon;
		}

		public override BaseTask Copy()
		{
			return new EmsmdbMapiHttpTask(base.CreateContextCopy(), this.shouldCallLogon);
		}

		protected override IEnumerator<ITask> Process()
		{
			using (IEmsmdbClient emsmdbHttpClient = base.Environment.CreateEmsmdbClient(RpcHelper.BuildCompleteMapiHttpBindingInfo(base.Properties)))
			{
				IContext derivedContext = base.CreateDerivedContext();
				if (!derivedContext.Properties.IsPropertyFound(ContextPropertySchema.MailboxLegacyDN))
				{
					derivedContext.Properties.Set(ContextPropertySchema.MailboxLegacyDN, derivedContext.Properties.Get(ContextPropertySchema.UserLegacyDN));
				}
				IContext context = derivedContext;
				ITask[] array = new ITask[2];
				array[0] = new EmsmdbConnectTask(emsmdbHttpClient, derivedContext);
				ITask[] array2 = array;
				int num = 1;
				ITask task2;
				if (!this.shouldCallLogon)
				{
					ITask task = new NullTask();
					task2 = task;
				}
				else
				{
					task2 = new EmsmdbLogonTask(emsmdbHttpClient, derivedContext);
				}
				array2[num] = task2;
				BaseTask composite = new CompositeTask(context, array);
				yield return composite;
				base.Result = composite.Result;
			}
			yield break;
		}

		private readonly bool shouldCallLogon;
	}
}

using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EmsmdbTask : BaseTask
	{
		public EmsmdbTask(IContext context) : this(context, true)
		{
		}

		public EmsmdbTask(IContext context, bool shouldCallLogon) : base(context, Strings.EmsmdbTaskTitle, Strings.EmsmdbTaskDescription, TaskType.Knowledge, RpcHelper.DependenciesOfBuildCompleteBindingInfo.Concat(new ContextProperty[]
		{
			ContextPropertySchema.ActualBinding.SetOnly()
		}))
		{
			this.shouldCallLogon = shouldCallLogon;
		}

		public override BaseTask Copy()
		{
			return new EmsmdbTask(base.CreateContextCopy(), this.shouldCallLogon);
		}

		protected override IEnumerator<ITask> Process()
		{
			using (IEmsmdbClient emsmdbClient = base.Environment.CreateEmsmdbClient(RpcHelper.BuildCompleteBindingInfo(base.Properties, 6001)))
			{
				base.Set<string>(ContextPropertySchema.ActualBinding, emsmdbClient.BindingString);
				IContext derivedContext = base.CreateDerivedContext();
				if (!derivedContext.Properties.IsPropertyFound(ContextPropertySchema.MailboxLegacyDN))
				{
					derivedContext.Properties.Set(ContextPropertySchema.MailboxLegacyDN, derivedContext.Properties.Get(ContextPropertySchema.UserLegacyDN));
				}
				IContext context = derivedContext;
				ITask[] array = new ITask[2];
				array[0] = new EmsmdbConnectTask(emsmdbClient, derivedContext);
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
					task2 = new EmsmdbLogonTask(emsmdbClient, derivedContext);
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

using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublicFolderEmsMdbTask : BaseTask
	{
		public PublicFolderEmsMdbTask(IContext context) : base(context, Strings.PFEmsmdbTaskTitle, Strings.PFEmsmdbTaskDescription, TaskType.Knowledge, RpcHelper.DependenciesOfBuildCompleteBindingInfo.Concat(new ContextProperty[]
		{
			ContextPropertySchema.ActualBinding.SetOnly()
		}))
		{
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
				BaseTask composite = new CompositeTask(derivedContext, new ITask[]
				{
					new EmsmdbConnectTask(emsmdbClient, derivedContext),
					new PublicFolderMailboxLogonTask(emsmdbClient, derivedContext)
				});
				yield return composite;
				base.Result = composite.Result;
			}
			yield break;
		}
	}
}

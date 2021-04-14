using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublicFolderMailboxLogonTask : BaseRpcTask
	{
		public PublicFolderMailboxLogonTask(IEmsmdbClient emsmdbClient, IContext context) : base(context, Strings.PFEmsmdbTaskTitle, Strings.PFEmsmdbTaskDescription, TaskType.Operation, new ContextProperty[]
		{
			context.Properties.IsPropertyFound(ContextPropertySchema.MailboxLegacyDN) ? ContextPropertySchema.MailboxLegacyDN.GetOnly() : null
		})
		{
			this.emsmdbClient = emsmdbClient;
		}

		protected override IEnumerator<ITask> Process()
		{
			string mbxLegDn = null;
			try
			{
				mbxLegDn = base.Get<string>(ContextPropertySchema.MailboxLegacyDN);
			}
			catch (Exception)
			{
			}
			AsyncTask asyncTask = new AsyncTask(base.CreateDerivedContext(), (AsyncCallback asyncCallback, object asyncState) => this.emsmdbClient.BeginPublicLogon(mbxLegDn, this.Get<TimeSpan>(BaseTask.Timeout), asyncCallback, asyncState), (IAsyncResult asyncResult) => base.ApplyCallResult(this.emsmdbClient.EndPublicLogon(asyncResult)));
			yield return asyncTask;
			base.Result = asyncTask.Result;
			yield break;
		}

		private readonly IEmsmdbClient emsmdbClient;
	}
}

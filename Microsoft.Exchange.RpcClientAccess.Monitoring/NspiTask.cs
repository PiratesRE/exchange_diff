using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiTask : BaseTask
	{
		public NspiTask(IContext context) : base(context, Strings.NspiTaskTitle, Strings.NspiTaskDescription, TaskType.Knowledge, RpcHelper.DependenciesOfBuildCompleteBindingInfo.Concat(new ContextProperty[]
		{
			ContextPropertySchema.ActualBinding.SetOnly(),
			ContextPropertySchema.RpcServerLegacyDN.SetOnly()
		}))
		{
		}

		protected override IEnumerator<ITask> Process()
		{
			using (INspiClient nspiClient = base.Environment.CreateNspiClient(RpcHelper.BuildCompleteBindingInfo(base.Properties, 6004)))
			{
				base.Set<string>(ContextPropertySchema.ActualBinding, nspiClient.BindingString);
				NspiBindTask bindTask = new NspiBindTask(nspiClient, base.CreateDerivedContext());
				yield return bindTask;
				base.Result = bindTask.Result;
				IContext homeMdbContext = base.CreateDerivedContext();
				if (base.Result == TaskResult.Success)
				{
					BaseTask dataTasks = new CompositeTask(base.CreateDerivedContext(), new ITask[]
					{
						new NspiGetHierarchyInfoTask(nspiClient, homeMdbContext),
						new NspiQueryHomeMdbTask(nspiClient, homeMdbContext)
					});
					yield return dataTasks;
					base.Result = dataTasks.Result;
				}
				if (base.Result == TaskResult.Success)
				{
					string homeMDB = homeMdbContext.Properties.Get(ContextPropertySchema.HomeMdbLegacyDN);
					int mdbMarker = homeMDB.IndexOf("/cn=Microsoft Private MDB");
					if (mdbMarker == -1)
					{
						throw new NspiDataException("NspiTask", string.Format("homeMdb is not in the right format.  Found: {0}", homeMDB));
					}
					string serverLegacyDN = homeMDB.Substring(0, mdbMarker);
					base.Set<string>(ContextPropertySchema.RpcServerLegacyDN, serverLegacyDN);
					if (RpcHelper.IsRealServerName(serverLegacyDN))
					{
						NspiGetNetworkAddressesTask getNetworkAddressTask = new NspiGetNetworkAddressesTask(nspiClient, base.CreateDerivedContext());
						yield return getNetworkAddressTask;
						base.Result = getNetworkAddressTask.Result;
					}
				}
				if (bindTask.Result == TaskResult.Success)
				{
					NspiUnbindTask unbindTask = new NspiUnbindTask(nspiClient, base.CreateDerivedContext());
					yield return unbindTask;
					if (base.Result == TaskResult.Success)
					{
						base.Result = unbindTask.Result;
					}
				}
			}
			yield break;
		}

		private const string PrivateMDBSuffix = "Microsoft Private MDB";

		private const string ChildRdnPrefix = "/cn=";
	}
}

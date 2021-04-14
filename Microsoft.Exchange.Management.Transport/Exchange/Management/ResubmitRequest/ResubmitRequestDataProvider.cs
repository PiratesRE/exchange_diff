using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Transport.MessageRepository;

namespace Microsoft.Exchange.Management.ResubmitRequest
{
	public class ResubmitRequestDataProvider : IConfigDataProvider
	{
		public ResubmitRequestDataProvider(ServerIdParameter serverId, ResubmitRequestId identity)
		{
			this.serverIdentity = (serverId ?? new ServerIdParameter());
			this.resubmitRequestIdentity = identity;
		}

		private void ResolveServer()
		{
			if (this.serverObject != null)
			{
				return;
			}
			ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 66, "ResolveServer", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\ResubmitRequest\\ResubmitRequestDataProvider.cs");
			Server server = null;
			IEnumerable<Server> objects = this.serverIdentity.GetObjects<Server>(null, session);
			IEnumerator<Server> enumerator = objects.GetEnumerator();
			try
			{
				if (!enumerator.MoveNext())
				{
					throw new LocalizedException(Strings.ErrorServerNotFound(this.serverIdentity));
				}
				server = enumerator.Current;
				if (enumerator.MoveNext())
				{
					throw new LocalizedException(Strings.ErrorServerNotUnique(this.serverIdentity));
				}
			}
			finally
			{
				enumerator.Dispose();
			}
			this.serverObject = server;
		}

		IConfigurable IConfigDataProvider.Read<T>(ObjectId identity)
		{
			throw new NotImplementedException();
		}

		IConfigurable[] IConfigDataProvider.Find<T>(QueryFilter notUsed1, ObjectId notUsed2, bool notUsed3, SortBy notUsed4)
		{
			this.ResolveServer();
			MessageResubmissionRpcClientImpl messageResubmissionRpcClientImpl = new MessageResubmissionRpcClientImpl(this.serverObject.Name);
			return (from item in messageResubmissionRpcClientImpl.GetResubmitRequest(this.resubmitRequestIdentity)
			select this.GetPresentationObject(item)).ToArray<ResubmitRequest>();
		}

		IEnumerable<T> IConfigDataProvider.FindPaged<T>(QueryFilter notUsed1, ObjectId notUsed2, bool notUsed3, SortBy notUsed4, int notUsed5)
		{
			this.ResolveServer();
			MessageResubmissionRpcClientImpl client = new MessageResubmissionRpcClientImpl(this.serverObject.Name);
			foreach (ResubmitRequest request in client.GetResubmitRequest(this.resubmitRequestIdentity))
			{
				yield return (T)((object)this.GetPresentationObject(request));
			}
			yield break;
		}

		void IConfigDataProvider.Save(IConfigurable instance)
		{
			this.ResolveServer();
			ResubmitRequest resubmitRequest = (ResubmitRequest)instance;
			if (resubmitRequest.State != ResubmitRequestState.Running && resubmitRequest.State != ResubmitRequestState.Paused)
			{
				throw new InvalidOperationException(new LocalizedString(Strings.ResubmitRequestStateInvalid));
			}
			MessageResubmissionRpcClientImpl messageResubmissionRpcClientImpl = new MessageResubmissionRpcClientImpl(this.serverObject.Name);
			messageResubmissionRpcClientImpl.SetResubmitRequest((ResubmitRequestId)resubmitRequest.Identity, resubmitRequest.State == ResubmitRequestState.Running);
		}

		void IConfigDataProvider.Delete(IConfigurable instance)
		{
			this.ResolveServer();
			MessageResubmissionRpcClientImpl messageResubmissionRpcClientImpl = new MessageResubmissionRpcClientImpl(this.serverObject.Name);
			messageResubmissionRpcClientImpl.RemoveResubmitRequest((ResubmitRequestId)instance.Identity);
		}

		string IConfigDataProvider.Source
		{
			get
			{
				return this.serverIdentity.Fqdn;
			}
		}

		private ResubmitRequest GetPresentationObject(ResubmitRequest original)
		{
			return ResubmitRequest.Create(original.ResubmitRequestId.ResubmitRequestRowId, original.Server, original.StartTime.ToLocalTime(), original.Destination, original.DiagnosticInformation, original.EndTime.ToLocalTime(), original.CreationTime.ToLocalTime(), (int)original.State);
		}

		private readonly ServerIdParameter serverIdentity;

		private readonly ResubmitRequestId resubmitRequestIdentity;

		private Server serverObject;
	}
}

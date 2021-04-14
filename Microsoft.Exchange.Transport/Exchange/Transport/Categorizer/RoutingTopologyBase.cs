using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal abstract class RoutingTopologyBase
	{
		protected RoutingTopologyBase()
		{
			this.session = RoutingTopologyBase.CreateADSession();
		}

		public abstract TopologyServer LocalServer { get; }

		public abstract IEnumerable<MiniDatabase> GetDatabases(bool forcedReload);

		public abstract IEnumerable<TopologyServer> Servers { get; }

		public abstract IList<TopologySite> Sites { get; }

		public abstract IList<MailGateway> SendConnectors { get; }

		public abstract IList<PublicFolderTree> PublicFolderTrees { get; }

		public abstract IList<RoutingGroup> RoutingGroups { get; }

		public abstract IList<RoutingGroupConnector> RoutingGroupConnectors { get; }

		public abstract IList<Server> HubServersOnEdge { get; }

		public DateTime WhenCreated
		{
			get
			{
				return this.whenCreated;
			}
		}

		protected ADObjectId RootId
		{
			get
			{
				if (this.rootId == null)
				{
					this.rootId = this.session.GetOrgContainerId().GetChildId("Administrative Groups");
				}
				return this.rootId;
			}
		}

		protected ITopologyConfigurationSession Session
		{
			get
			{
				return this.session;
			}
		}

		public static void UnregisterFromADNotifications(IList<ADNotificationRequestCookie> cookies)
		{
			RoutingUtils.ThrowIfNull(cookies, "cookies");
			RoutingDiag.Tracer.TraceDebug(0L, "Unregistering from AD notifications");
			foreach (ADNotificationRequestCookie requestCookie in cookies)
			{
				ADNotificationAdapter.UnregisterChangeNotification(requestCookie, true);
			}
		}

		public ADOperationResult TryRegisterForADNotifications(ADNotificationCallback callback, out IList<ADNotificationRequestCookie> cookies)
		{
			cookies = null;
			List<ADNotificationRequestCookie> tempCookies = new List<ADNotificationRequestCookie>(8);
			RoutingDiag.Tracer.TraceDebug((long)this.GetHashCode(), "Registering for AD notifications");
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				this.RegisterForADNotifications(callback, tempCookies);
			});
			if (!adoperationResult.Succeeded)
			{
				RoutingDiag.Tracer.TraceError<Exception>((long)this.GetHashCode(), "Failed to register for AD notifications due to {0}", adoperationResult.Exception);
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingAdUnavailable, null, new object[0]);
				RoutingTopologyBase.UnregisterFromADNotifications(tempCookies);
			}
			else
			{
				cookies = tempCookies;
			}
			return adoperationResult;
		}

		public void PreLoad()
		{
			this.whenCreated = DateTime.UtcNow;
			this.PreLoadInternal();
			this.Validate();
		}

		public abstract void LogData(RoutingTableLogger logger);

		protected abstract void PreLoadInternal();

		protected abstract void RegisterForADNotifications(ADNotificationCallback callback, IList<ADNotificationRequestCookie> cookies);

		protected abstract void Validate();

		protected IList<T> LoadAll<T>() where T : ADConfigurationObject, new()
		{
			return this.LoadAll<T>((T configObject) => true);
		}

		protected IList<T> LoadAll<T>(Func<T, bool> filter) where T : ADConfigurationObject, new()
		{
			RoutingDiag.Tracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "[{0}] Loading all objects of class {1} from AD", this.whenCreated, typeof(T).Name);
			List<T> list = this.FindAllPaged<T>().Where(filter).ToList<T>();
			RoutingDiag.Tracer.TraceDebug<DateTime, string, int>((long)this.GetHashCode(), "[{0}] Loaded all objects of class {1} from AD; found {2} object(s)", this.whenCreated, typeof(T).Name, list.Count);
			return list;
		}

		protected ADPagedReader<T> FindAllPaged<T>() where T : ADConfigurationObject, new()
		{
			return this.session.FindPaged<T>(this.RootId, QueryScope.SubTree, null, null, ADGenericPagedReader<T>.DefaultPageSize);
		}

		protected void LogSendConnectors(RoutingTableLogger logger)
		{
			logger.WriteStartElement("SendConnectors");
			foreach (MailGateway connector in this.SendConnectors)
			{
				logger.WriteSendConnector(connector);
			}
			logger.WriteEndElement();
		}

		private static ITopologyConfigurationSession CreateADSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 345, "CreateADSession", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Categorizer\\Routing\\RoutingTopologyBase.cs");
		}

		private ITopologyConfigurationSession session;

		private ADObjectId rootId;

		private DateTime whenCreated;
	}
}

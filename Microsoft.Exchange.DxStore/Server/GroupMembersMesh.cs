using System;
using System.Collections.Concurrent;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using FUSE.Paxos;
using FUSE.Paxos.Network;
using FUSE.Weld.Base;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.DxStore;
using Microsoft.Exchange.DxStore.Common;

namespace Microsoft.Exchange.DxStore.Server
{
	public sealed class GroupMembersMesh : ISubject<Tuple<string, Message>>, ISubject<Tuple<string, Message>, Tuple<string, Message>>, IObserver<Tuple<string, Message>>, IObservable<Tuple<string, Message>>
	{
		public GroupMembersMesh(string identity, INodeEndPoints<ServiceEndpoint> nodeEndPoints, InstanceGroupConfig groupConfig)
		{
			this.groupConfig = groupConfig;
			this.identity = identity;
			this.nodeEndPoints = nodeEndPoints;
			this.incoming = Subject.Synchronize<Tuple<string, Message>, Tuple<string, Message>>(new Subject<Tuple<string, Message>>(), Scheduler.TaskPool);
			this.factoryByEndPoint = new ConcurrentDictionary<ServiceEndpoint, WCF.CachedChannelFactory<IDxStoreInstance>>();
			ObservableExtensions.Subscribe<Tuple<string, Message>>(this.incoming, delegate(Tuple<string, Message> item)
			{
				this.TraceMessage(false, item);
			});
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MeshTracer;
			}
		}

		public ISubject<Tuple<string, Message>, Tuple<string, Message>> Incoming
		{
			get
			{
				return this.incoming;
			}
		}

		public void OnCompleted()
		{
			GroupMembersMesh.Tracer.TraceDebug<string>((long)this.identity.GetHashCode(), "{0}: OnCompleted() called", this.identity);
		}

		public void OnError(Exception exception)
		{
			if (GroupMembersMesh.Tracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				GroupMembersMesh.Tracer.TraceError<string, Exception>((long)this.identity.GetHashCode(), "{0}: OnError() called with {1}", this.identity, exception);
			}
		}

		public ChannelFactory<IDxStoreInstance> GetChannelFactory(string nodeName)
		{
			ServiceEndpoint key = this.nodeEndPoints.Map(nodeName);
			WCF.CachedChannelFactory<IDxStoreInstance> orAdd = this.factoryByEndPoint.GetOrAdd(key, delegate(ServiceEndpoint e)
			{
				WCF.Initialize(e);
				return new WCF.CachedChannelFactory<IDxStoreInstance>(e);
			});
			return orAdd.Factory;
		}

		public void OnNext(Tuple<string, Message> item)
		{
			Concurrency.SwallowExceptions(Task.Run(() => this.OnNextAsync(item)), null, new object[0]);
		}

		public IDisposable Subscribe(IObserver<Tuple<string, Message>> observer)
		{
			GroupMembersMesh.Tracer.TraceDebug<string>((long)this.identity.GetHashCode(), "{0}: Subscription requested for incoming messages", this.identity);
			return this.Incoming.Subscribe(observer);
		}

		private void TraceMessage(bool isSend, Tuple<string, Message> msg)
		{
			if (ExTraceGlobals.PaxosMessageTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				int num = 80;
				string text = msg.Item2.ToString();
				if (text.Length < num)
				{
					num = text.Length;
				}
				ExTraceGlobals.PaxosMessageTracer.TraceDebug((long)this.identity.GetHashCode(), "{0}: {1} {2} : {3}", new object[]
				{
					this.identity,
					isSend ? "Send ->" : "Recv <-",
					msg.Item1,
					text.Substring(0, num)
				});
			}
		}

		private bool IsSelf(string target)
		{
			return Utils.IsEqual(this.groupConfig.Self, target, StringComparison.OrdinalIgnoreCase);
		}

		private async Task OnNextAsync(Tuple<string, Message> item)
		{
			try
			{
				this.TraceMessage(true, item);
				if (this.IsSelf(item.Item1))
				{
					this.Incoming.OnNext(item);
				}
				else if (!this.groupConfig.Settings.IsUseHttpTransportForInstanceCommunication)
				{
					ChannelFactory<IDxStoreInstance> factory = this.GetChannelFactory(item.Item1);
					await Concurrency.DropContext(WCF.WithServiceAsync<IDxStoreInstance>(factory, (IDxStoreInstance instance) => instance.PaxosMessageAsync(this.groupConfig.Self, item.Item2), null, default(CancellationToken)));
				}
				else
				{
					HttpRequest.PaxosMessage msg = new HttpRequest.PaxosMessage(this.nodeEndPoints.Self, item.Item2);
					string targetHost = this.groupConfig.GetMemberNetworkAddress(item.Item1);
					await HttpClient.SendMessageAsync(targetHost, item.Item1, this.groupConfig.Name, msg);
				}
			}
			catch (Exception ex)
			{
				if (GroupMembersMesh.Tracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					GroupMembersMesh.Tracer.TraceError((long)this.identity.GetHashCode(), "{0}: OnNextAsync(Node:{1}, Msg:{2}) failed with {3}", new object[]
					{
						this.identity,
						item.Item1,
						item.Item2,
						ex
					});
				}
			}
		}

		private readonly INodeEndPoints<ServiceEndpoint> nodeEndPoints;

		private readonly ConcurrentDictionary<ServiceEndpoint, WCF.CachedChannelFactory<IDxStoreInstance>> factoryByEndPoint;

		private readonly ISubject<Tuple<string, Message>, Tuple<string, Message>> incoming;

		private readonly string identity;

		private readonly InstanceGroupConfig groupConfig;
	}
}

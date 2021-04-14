using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Microsoft.Exchange.DxStore.Common
{
	public class CachedChannelFactory<T> : IDisposable
	{
		public CachedChannelFactory(ServiceEndpoint endpoint)
		{
			this.endpoint = endpoint;
		}

		public ChannelFactory<T> Factory
		{
			get
			{
				ChannelFactory<T> channelFactory = null;
				ChannelFactory<T> result;
				try
				{
					lock (this.gate)
					{
						if (this.factory != null && this.factory.State == CommunicationState.Faulted)
						{
							channelFactory = this.factory;
							this.factory = null;
						}
						result = (this.factory ?? new ChannelFactory<T>(this.endpoint));
					}
				}
				finally
				{
					Utils.AbortBestEffort<T>(channelFactory);
					Utils.DisposeBestEffort(channelFactory);
				}
				return result;
			}
		}

		public void Dispose()
		{
			ChannelFactory<T> disposable;
			lock (this.gate)
			{
				disposable = this.factory;
				this.factory = null;
			}
			Utils.DisposeBestEffort(disposable);
		}

		private readonly object gate = new object();

		private readonly ServiceEndpoint endpoint;

		private ChannelFactory<T> factory;
	}
}

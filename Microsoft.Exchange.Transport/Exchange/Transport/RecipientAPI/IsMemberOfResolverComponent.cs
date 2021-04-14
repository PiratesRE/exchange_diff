using System;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;

namespace Microsoft.Exchange.Transport.RecipientAPI
{
	internal class IsMemberOfResolverComponent<GroupKeyType> : ITransportComponent
	{
		public IsMemberOfResolverComponent(string componentName, TransportAppConfig.IsMemberOfResolverConfiguration appConfig, IsMemberOfResolverADAdapter<GroupKeyType> adAdapter)
		{
			this.componentName = componentName;
			this.configuration = new TransportIsMemberOfResolverConfig(appConfig);
			this.adAdapter = adAdapter;
		}

		public IsMemberOfResolver<GroupKeyType> IsMemberOfResolver
		{
			get
			{
				return this.memberOfResolver;
			}
		}

		public void Load()
		{
			IsMemberOfResolverPerformanceCounters perfCounters = new IsMemberOfResolverPerformanceCounters(this.componentName);
			this.memberOfResolver = new IsMemberOfResolver<GroupKeyType>(this.configuration, perfCounters, this.adAdapter);
		}

		public void Unload()
		{
			if (this.memberOfResolver == null)
			{
				throw new InvalidOperationException("IsMemberOfResolverComponent is not loaded");
			}
			this.memberOfResolver.Dispose();
			this.memberOfResolver = null;
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public virtual void ClearCache()
		{
			if (this.memberOfResolver != null)
			{
				this.memberOfResolver.ClearCache();
			}
		}

		private string componentName;

		private TransportIsMemberOfResolverConfig configuration;

		private IsMemberOfResolverADAdapter<GroupKeyType> adAdapter;

		private IsMemberOfResolver<GroupKeyType> memberOfResolver;
	}
}

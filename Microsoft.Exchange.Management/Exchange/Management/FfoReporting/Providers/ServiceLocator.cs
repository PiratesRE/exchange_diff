using System;
using System.Collections.Concurrent;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.FfoReporting.Providers
{
	internal class ServiceLocator : IServiceLocator
	{
		internal ServiceLocator()
		{
		}

		public static IServiceLocator Current
		{
			get
			{
				return ServiceLocator.hookableInstance.Value;
			}
		}

		internal static IDisposable SetTestHook(IServiceLocator testHook)
		{
			return ServiceLocator.hookableInstance.SetTestHook(testHook);
		}

		private static ServiceLocator CreateWithDefaults()
		{
			ServiceLocator serviceLocator = new ServiceLocator();
			serviceLocator.AddService<IDalProvider>(() => new DalProviderImpl());
			serviceLocator.AddService<ISmtpCheckerProvider>(() => new SmtpCheckerProviderImpl());
			serviceLocator.AddService<IAuthenticationProvider>(() => new AuthenticationProviderImpl());
			return serviceLocator;
		}

		public TServiceType GetService<TServiceType>()
		{
			Func<object> func;
			if (this.services.TryGetValue(typeof(TServiceType), out func))
			{
				return (TServiceType)((object)func());
			}
			throw new ArgumentException(string.Format("Unknown service: {0}", typeof(TServiceType).Name));
		}

		internal void AddService<TProviderType>(Func<object> activator)
		{
			this.services[typeof(TProviderType)] = activator;
		}

		private static Hookable<IServiceLocator> hookableInstance = Hookable<IServiceLocator>.Create(true, ServiceLocator.CreateWithDefaults());

		private ConcurrentDictionary<Type, Func<object>> services = new ConcurrentDictionary<Type, Func<object>>();
	}
}

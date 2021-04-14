using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class OWAServiceFactory : ServiceHostFactory
	{
		public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
		{
			if (Globals.IsAnonymousCalendarApp)
			{
				return this.CreateOwaAnonymousServiceHost(baseAddresses);
			}
			return this.CreateOwaServiceHost(baseAddresses);
		}

		private ServiceHost CreateOwaServiceHost(Uri[] baseAddresses)
		{
			baseAddresses = Array.FindAll<Uri>(baseAddresses, (Uri uri) => uri.Scheme == Uri.UriSchemeHttps);
			ServiceHost serviceHost = new ServiceHost(typeof(OWAService), baseAddresses);
			serviceHost.Authorization.ServiceAuthorizationManager = new OWAAuthorizationManager();
			foreach (Uri uri2 in baseAddresses)
			{
				ServiceEndpoint serviceEndpoint = serviceHost.AddServiceEndpoint(typeof(IOWAService), this.CreateBinding(uri2), uri2);
				serviceEndpoint.Behaviors.Add(new OWAWebHttpBehavior());
				serviceEndpoint.Behaviors.Add(new DispatcherSynchronizationBehavior
				{
					MaxPendingReceives = OWAServiceFactory.MaxPendingReceives
				});
				UriBuilder uriBuilder = new UriBuilder(uri2);
				UriBuilder uriBuilder2 = uriBuilder;
				uriBuilder2.Path += "/s";
				ServiceEndpoint serviceEndpoint2 = serviceHost.AddServiceEndpoint(typeof(IOWAStreamingService), this.CreateBinding(uriBuilder.Uri), uriBuilder.Uri);
				serviceEndpoint2.Behaviors.Add(new OWAWebHttpBehavior());
			}
			return serviceHost;
		}

		private ServiceHost CreateOwaAnonymousServiceHost(Uri[] baseAddresses)
		{
			ServiceHost serviceHost = new ServiceHost(typeof(OWAAnonymousService), baseAddresses);
			foreach (Uri uri in baseAddresses)
			{
				ServiceEndpoint serviceEndpoint = serviceHost.AddServiceEndpoint(typeof(IOWAAnonymousCalendarService), this.CreateBinding(uri), uri);
				serviceEndpoint.Behaviors.Add(new OWAWebHttpBehavior());
			}
			return serviceHost;
		}

		private WebHttpBinding CreateBinding(Uri supportedUri)
		{
			string configurationName = string.Format("{0}Binding", supportedUri.Scheme);
			return new WebHttpBinding(configurationName)
			{
				Security = 
				{
					Transport = 
					{
						ClientCredentialType = Globals.ServiceAuthenticationType
					}
				}
			};
		}

		private const string MaxPendingReceivesKeyName = "MaxPendingReceives";

		private static readonly int DefaultMaxPendingReceives = 2 * Environment.ProcessorCount;

		private static readonly int MaxPendingReceives = Global.GetAppSettingAsInt("MaxPendingReceives", OWAServiceFactory.DefaultMaxPendingReceives);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.PushNotifications.Publishers;
using Microsoft.Exchange.PushNotifications.Server.Commands;
using Microsoft.Exchange.PushNotifications.Server.Core;
using Microsoft.Exchange.PushNotifications.Server.Wcf;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.PushNotifications.Server.Services
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[PushNotificationServiceBehavior]
	internal class PushNotificationOnPremService : ServiceBase, IAzureChallengeRequestServiceContract, IAzureDeviceRegistrationServiceContract, IPublisherServiceContract
	{
		static PushNotificationOnPremService()
		{
			int maxThreadCount = 10 * Environment.ProcessorCount;
			Microsoft.Exchange.WorkloadManagement.UserWorkloadManager.Initialize(maxThreadCount, 500, 10, TimeSpan.FromMinutes(5.0), null);
			if (ActivityContextLogConfig.IsActivityContextLogEnabled())
			{
				ActivityContextLogger.Initialize();
			}
			PushNotificationOnPremService.ConfigWatcher = new PublisherConfigurationWatcher("MSExchangePushNotificationsAppPool", ServiceConfig.ConfigurationRefreshRateInMinutes);
			PushNotificationOnPremService.ConfigWatcher.OnChangeEvent += PushNotificationOnPremService.RestartPushNotificationAppPool;
			PushNotificationPublisherConfiguration pushNotificationPublisherConfiguration = PushNotificationOnPremService.ConfigWatcher.Start();
			PushNotificationOnPremService.IsRunningLegacyMode = (pushNotificationPublisherConfiguration.AzurePublisherSettings.Count<AzurePublisherSettings>() == 0);
			if (!PushNotificationOnPremService.IsRunningLegacyMode)
			{
				PushNotificationOnPremService.ConfigWatcher.OnReadEvent += PushNotificationOnPremService.ConfigurationRead;
				if (pushNotificationPublisherConfiguration.ProxyPublisherSettings != null && pushNotificationPublisherConfiguration.ProxyPublisherSettings.IsSuitable)
				{
					PushNotificationOnPremService.HubName = pushNotificationPublisherConfiguration.ProxyPublisherSettings.HubName;
				}
			}
			PushNotificationPublisherManagerBuilder pushNotificationPublisherManagerBuilder = new PushNotificationPublisherManagerBuilder(new List<PushNotificationPlatform>
			{
				PushNotificationPlatform.Azure,
				PushNotificationPlatform.AzureChallengeRequest,
				PushNotificationPlatform.AzureDeviceRegistration,
				PushNotificationPlatform.Proxy
			});
			PushNotificationOnPremService.PublisherManager = pushNotificationPublisherManagerBuilder.Build(pushNotificationPublisherConfiguration, null, null);
			if (ServiceConfig.IgnoreCertificateErrors)
			{
				ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true));
			}
			AppDomain.CurrentDomain.DomainUnload += delegate(object sender, EventArgs e)
			{
				PushNotificationOnPremService.DisposeStaticResources();
			};
		}

		public PushNotificationOnPremService() : base(Microsoft.Exchange.WorkloadManagement.UserWorkloadManager.Singleton)
		{
		}

		[PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "LocalAdministrators")]
		[PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "UserService")]
		public IAsyncResult BeginPublishNotifications(MailboxNotificationBatch notifications, AsyncCallback asyncCallback, object asyncState)
		{
			if (PushNotificationOnPremService.IsRunningLegacyMode)
			{
				return base.BeginServiceCommand(new PublishOnPremNotifications(notifications, PushNotificationOnPremService.PublisherManager, asyncCallback, asyncState));
			}
			return base.BeginServiceCommand(new PublishNotifications(notifications, PushNotificationOnPremService.PublisherManager, PushNotificationOnPremService.HubName, asyncCallback, asyncState));
		}

		public void EndPublishNotifications(IAsyncResult result)
		{
			base.EndServiceCommand<ServiceCommandResultNone>(result);
		}

		public IAsyncResult BeginChallengeRequest(AzureChallengeRequestInfo issueSecret, AsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginServiceCommand(new IssueRegistrationChallenge(issueSecret, PushNotificationOnPremService.PublisherManager, PushNotificationOnPremService.HubName, asyncCallback, asyncState));
		}

		public void EndChallengeRequest(IAsyncResult result)
		{
			base.EndServiceCommand<ServiceCommandResultNone>(result);
		}

		public IAsyncResult BeginDeviceRegistration(AzureDeviceRegistrationInfo deviceRegistrationInfo, AsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginServiceCommand(new CreateDeviceRegistration(deviceRegistrationInfo, PushNotificationOnPremService.PublisherManager, PushNotificationOnPremService.HubName, asyncCallback, asyncState));
		}

		public void EndDeviceRegistration(IAsyncResult result)
		{
			base.EndServiceCommand<ServiceCommandResultNone>(result);
		}

		internal override IStandardBudget AcquireBudget(IServiceCommand serviceCommand)
		{
			SecurityIdentifier user = OperationContext.Current.ServiceSecurityContext.WindowsIdentity.User;
			return StandardBudget.Acquire(ServiceBase.LocalSystemBudgetKey);
		}

		private static void RestartPushNotificationAppPool(object sender, ConfigurationChangedEventArgs config)
		{
			using (ServerManager serverManager = new ServerManager())
			{
				ApplicationPool applicationPool = serverManager.ApplicationPools["MSExchangePushNotificationsAppPool"];
				applicationPool.Recycle();
			}
		}

		private static void ConfigurationRead(object sender, ConfigurationReadEventArgs config)
		{
			ProxyNotification notification = new ProxyNotification(PushNotificationCannedApp.OnPremProxy.Name, config.Configuration.AzurePublisherSettings);
			PushNotificationOnPremService.PublisherManager.Publish(notification);
		}

		private static void DisposeStaticResources()
		{
			if (PushNotificationOnPremService.ConfigWatcher != null)
			{
				PushNotificationOnPremService.ConfigWatcher.OnChangeEvent -= PushNotificationOnPremService.RestartPushNotificationAppPool;
				PushNotificationOnPremService.ConfigWatcher.Dispose();
			}
			if (PushNotificationOnPremService.PublisherManager != null)
			{
				PushNotificationOnPremService.PublisherManager.Dispose();
			}
		}

		private static readonly PushNotificationPublisherManager PublisherManager;

		private static readonly PublisherConfigurationWatcher ConfigWatcher;

		private static readonly bool IsRunningLegacyMode;

		private static readonly string HubName;
	}
}

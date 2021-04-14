using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PushNotifications.Extensions;
using Microsoft.Exchange.PushNotifications.Publishers;
using Microsoft.Exchange.PushNotifications.Server.Commands;
using Microsoft.Exchange.PushNotifications.Server.Core;
using Microsoft.Exchange.PushNotifications.Server.Wcf;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.PushNotifications.Server.Services
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[PushNotificationServiceBehavior]
	internal class PushNotificationService : ServiceBase, IAzureAppConfigDataServiceContract, IAzureHubCreationServiceContract, IAzureChallengeRequestServiceContract, IAzureDeviceRegistrationServiceContract, IPublisherServiceContract, IOnPremPublisherServiceContract, IOutlookPublisherServiceContract, ILocalUserNotificationPublisherServiceContract, IRemoteUserNotificationPublisherServiceContract
	{
		static PushNotificationService()
		{
			int maxThreadCount = 10 * Environment.ProcessorCount;
			Microsoft.Exchange.WorkloadManagement.UserWorkloadManager.Initialize(maxThreadCount, 500, 10, TimeSpan.FromMinutes(5.0), null);
			if (ActivityContextLogConfig.IsActivityContextLogEnabled())
			{
				ActivityContextLogger.Initialize();
			}
			ADObjectId adobjectId = PushNotificationServiceBudgetKey.ResolveServiceThrottlingPolicyId();
			if (adobjectId != null)
			{
				PushNotificationService.ServiceBudgetKey = new PushNotificationServiceBudgetKey(adobjectId);
			}
			else
			{
				PushNotificationService.ServiceBudgetKey = ServiceBase.LocalSystemBudgetKey;
			}
			PushNotificationService.ConfigWatcher = new PublisherConfigurationWatcher("MSExchangePushNotificationsAppPool", ServiceConfig.ConfigurationRefreshRateInMinutes);
			PushNotificationService.ConfigWatcher.OnChangeEvent += PushNotificationService.RestartPushNotificationAppPool;
			PushNotificationService.Configuration = PushNotificationService.ConfigWatcher.Start();
			PushNotificationPublisherManagerBuilder pushNotificationPublisherManagerBuilder = new PushNotificationPublisherManagerBuilder(new List<PushNotificationPlatform>
			{
				PushNotificationPlatform.APNS,
				PushNotificationPlatform.PendingGet,
				PushNotificationPlatform.WNS,
				PushNotificationPlatform.GCM,
				PushNotificationPlatform.WebApp,
				PushNotificationPlatform.Azure,
				PushNotificationPlatform.AzureHubCreation,
				PushNotificationPlatform.AzureChallengeRequest,
				PushNotificationPlatform.AzureDeviceRegistration
			});
			PushNotificationService.PublisherManager = pushNotificationPublisherManagerBuilder.Build(PushNotificationService.Configuration, DeviceThrottlingManager.Default, new AzureHubEventHandler());
			AppDomain.CurrentDomain.DomainUnload += delegate(object sender, EventArgs e)
			{
				PushNotificationService.DisposeStaticResources();
			};
		}

		public PushNotificationService() : base(Microsoft.Exchange.WorkloadManagement.UserWorkloadManager.Singleton)
		{
		}

		[PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "LocalAdministrators")]
		[PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "UserService")]
		public IAsyncResult BeginPublishNotifications(MailboxNotificationBatch notifications, AsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginServiceCommand(new PublishNotifications(notifications, PushNotificationService.PublisherManager, asyncCallback, asyncState));
		}

		public void EndPublishNotifications(IAsyncResult result)
		{
			base.EndServiceCommand<ServiceCommandResultNone>(result);
		}

		[PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "LocalAdministrators")]
		[PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "UserService")]
		public IAsyncResult BeginPublishOutlookNotifications(OutlookNotificationBatch notifications, AsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginServiceCommand(new PublishOutlookNotifications(notifications, PushNotificationService.PublisherManager, asyncCallback, asyncState));
		}

		public void EndPublishOutlookNotifications(IAsyncResult result)
		{
			base.EndServiceCommand<ServiceCommandResultNone>(result);
		}

		public IAsyncResult BeginPublishOnPremNotifications(MailboxNotificationBatch notifications, AsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginServiceCommand(new PublishProxyNotifications(notifications, PushNotificationService.PublisherManager, asyncCallback, asyncState));
		}

		public void EndPublishOnPremNotifications(IAsyncResult result)
		{
			base.EndServiceCommand<ServiceCommandResultNone>(result);
		}

		[PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "LocalAdministrators")]
		[PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "UserService")]
		public IAsyncResult BeginPublishUserNotifications(LocalUserNotificationBatch notifications, AsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginServiceCommand(new PublishLocalUserNotifications(notifications, PushNotificationService.PublisherManager, asyncCallback, asyncState));
		}

		public void EndPublishUserNotifications(IAsyncResult result)
		{
			base.EndServiceCommand<ServiceCommandResultNone>(result);
		}

		public IAsyncResult BeginPublishUserNotification(RemoteUserNotification notification, AsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginServiceCommand(new PublishUserNotification(notification, PushNotificationService.PublisherManager, asyncCallback, asyncState));
		}

		public void EndPublishUserNotification(IAsyncResult result)
		{
			base.EndServiceCommand<ServiceCommandResultNone>(result);
		}

		public IAsyncResult BeginCreateHub(AzureHubDefinition hubDefinition, AsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginServiceCommand(new CreateAzureHub(hubDefinition, PushNotificationService.PublisherManager, asyncCallback, asyncState));
		}

		public void EndCreateHub(IAsyncResult result)
		{
			base.EndServiceCommand<ServiceCommandResultNone>(result);
		}

		public IAsyncResult BeginChallengeRequest(AzureChallengeRequestInfo issueSecret, AsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginServiceCommand(new IssueRegistrationChallenge(issueSecret, PushNotificationService.PublisherManager, asyncCallback, asyncState));
		}

		public void EndChallengeRequest(IAsyncResult result)
		{
			base.EndServiceCommand<ServiceCommandResultNone>(result);
		}

		public IAsyncResult BeginDeviceRegistration(AzureDeviceRegistrationInfo deviceRegistrationInfo, AsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginServiceCommand(new CreateDeviceRegistration(deviceRegistrationInfo, PushNotificationService.PublisherManager, asyncCallback, asyncState));
		}

		public void EndDeviceRegistration(IAsyncResult result)
		{
			base.EndServiceCommand<ServiceCommandResultNone>(result);
		}

		public IAsyncResult BeginGetAppConfigData(AzureAppConfigRequestInfo requestConfig, AsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginServiceCommand(new GetAppConfigData(requestConfig, PushNotificationService.PublisherManager, PushNotificationService.Configuration, asyncCallback, asyncState));
		}

		public AzureAppConfigResponseInfo EndGetAppConfigData(IAsyncResult result)
		{
			return base.EndServiceCommand<AzureAppConfigResponseInfo>(result);
		}

		internal override IStandardBudget AcquireBudget(IServiceCommand serviceCommand)
		{
			SecurityIdentifier user = OperationContext.Current.ServiceSecurityContext.WindowsIdentity.User;
			if (user == null)
			{
				OAuthIdentity oauthIdentity = OperationContext.Current.GetOAuthIdentity();
				if (oauthIdentity != null)
				{
					return StandardBudget.Acquire(new TenantBudgetKey(oauthIdentity.OrganizationId, BudgetType.PushNotificationTenant));
				}
				WindowsIdentity windowsIdentity = OperationContext.Current.GetWindowsIdentity();
				if (windowsIdentity == null)
				{
					base.ThrowServiceBusyException(serviceCommand.Description, new FailedToAcquireBudgetException(OperationContext.Current.ServiceSecurityContext.WindowsIdentity.Name, OperationContext.Current.GetPrincipal().ToNullableString(null)));
				}
				user = windowsIdentity.User;
			}
			if (!user.IsWellKnown(WellKnownSidType.LocalSystemSid))
			{
				return StandardBudget.Acquire(user, BudgetType.PushNotificationTenant, ADSessionSettings.FromRootOrgScopeSet());
			}
			return StandardBudget.Acquire(PushNotificationService.ServiceBudgetKey);
		}

		private static void RestartPushNotificationAppPool(object sender, ConfigurationChangedEventArgs config)
		{
			using (ServerManager serverManager = new ServerManager())
			{
				ApplicationPool applicationPool = serverManager.ApplicationPools["MSExchangePushNotificationsAppPool"];
				applicationPool.Recycle();
			}
		}

		private static void DisposeStaticResources()
		{
			if (PushNotificationService.ConfigWatcher != null)
			{
				PushNotificationService.ConfigWatcher.OnChangeEvent -= PushNotificationService.RestartPushNotificationAppPool;
				PushNotificationService.ConfigWatcher.Dispose();
			}
			if (PushNotificationService.PublisherManager != null)
			{
				PushNotificationService.PublisherManager.Dispose();
			}
			PushNotificationService.ServerVersionTimer.Dispose();
		}

		internal static readonly LookupBudgetKey ServiceBudgetKey;

		private static readonly PushNotificationPublisherManager PublisherManager;

		private static readonly PublisherConfigurationWatcher ConfigWatcher;

		private static readonly PushNotificationPublisherConfiguration Configuration;

		private static readonly Timer ServerVersionTimer = new Timer(delegate(object _state)
		{
			PushNotificationsLogHelper.LogServerVersion();
		}, null, TimeSpan.Zero, TimeSpan.FromHours(6.0));
	}
}

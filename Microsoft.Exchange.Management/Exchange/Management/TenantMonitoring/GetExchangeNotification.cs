using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.CommonHelpProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.TenantMonitoring
{
	[Cmdlet("Get", "ExchangeNotification")]
	public sealed class GetExchangeNotification : GetTenantADObjectWithIdentityTaskBase<NotificationIdParameter, Notification>
	{
		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint> ResultSize
		{
			get
			{
				return base.InternalResultSize;
			}
			set
			{
				base.InternalResultSize = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ShowDuplicates
		{
			get
			{
				return this.showDuplicates;
			}
			set
			{
				this.showDuplicates = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADUser adUser = (ADUser)base.GetDataObject<ADUser>(GetExchangeNotification.FederatedMailboxId, base.TenantGlobalCatalogSession, null, null, new LocalizedString?(Strings.ErrorUserNotFound(GetExchangeNotification.FederatedMailboxId.ToString())), new LocalizedString?(Strings.ErrorUserNotUnique(GetExchangeNotification.FederatedMailboxId.ToString())));
			return new NotificationDataProvider(adUser, base.SessionSettings);
		}

		protected override Unlimited<uint> DefaultResultSize
		{
			get
			{
				return new Unlimited<uint>(50U);
			}
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 125, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\TenantMonitoring\\GetExchangeNotification.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = true;
			tenantOrTopologyConfigurationSession.UseGlobalCatalog = false;
			ExchangeConfigurationUnit exchangeConfigurationUnit = (ExchangeConfigurationUnit)base.GetDataObject<ExchangeConfigurationUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
			if (exchangeConfigurationUnit.OrganizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				throw new ArgumentException("OrganizationId.ForestWideOrgId is not supported.", "Organization");
			}
			return exchangeConfigurationUnit.OrganizationId;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(StoragePermanentException).IsInstanceOfType(exception);
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			base.WriteResult(this.ConvertDataObjectToPresentationObject(dataObject));
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			Notification notification = dataObject as Notification;
			if (notification == null)
			{
				return base.ConvertDataObjectToPresentationObject(dataObject);
			}
			string eventCategory;
			string localizedEventMessageAndCategory = this.GetLocalizedEventMessageAndCategory(notification, CultureInfo.CurrentUICulture, out eventCategory);
			notification.EventMessage = localizedEventMessageAndCategory;
			notification.EventCategory = eventCategory;
			notification.EventHelpUrl = this.GetHelpUrlForNotification(notification, CultureInfo.CurrentUICulture);
			return notification;
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, TenantNotificationMessageSchema.MonitoringCreationTimeUtc, ExDateTime.UtcNow.Subtract(TimeSpan.FromDays(1.0)));
			}
		}

		protected override IEnumerable<Notification> GetPagedData()
		{
			return default(GetExchangeNotification.NotificationMerger).RemoveDuplicatesAndSort(base.GetPagedData(), !this.ShowDuplicates);
		}

		private string GetLocalizedEventMessageAndCategory(Notification notification, CultureInfo language, out string category)
		{
			string text = string.Empty;
			category = string.Empty;
			try
			{
				text = Utils.GetResourcesFilePath(notification.EventSource);
				if (string.IsNullOrEmpty(text))
				{
					base.WriteDebug(Strings.TenantNotificationDebugEventResourceFileNotFound(notification.EventSource));
					return string.Empty;
				}
			}
			catch (IOException exception)
			{
				this.WriteError(exception, ErrorCategory.ResourceUnavailable, notification.EventSource, false);
				return string.Empty;
			}
			catch (SecurityException exception2)
			{
				this.WriteError(exception2, ErrorCategory.ResourceUnavailable, notification.EventSource, false);
				return string.Empty;
			}
			catch (UnauthorizedAccessException exception3)
			{
				this.WriteError(exception3, ErrorCategory.ResourceUnavailable, notification.EventSource, false);
				return string.Empty;
			}
			string result;
			using (StringWriter stringWriter = new StringWriter(CultureInfo.CurrentUICulture))
			{
				string localizedEventMessageAndCategory = Utils.GetLocalizedEventMessageAndCategory(text, (uint)notification.EventInstanceId, (uint)notification.EventCategoryId, notification.InsertionStrings, language, stringWriter, out category);
				if (base.IsDebugOn)
				{
					string text2 = stringWriter.ToString();
					if (!string.IsNullOrEmpty(text2))
					{
						base.WriteDebug(text2);
					}
				}
				result = localizedEventMessageAndCategory;
			}
			return result;
		}

		private string GetHelpUrlForNotification(Notification notification, CultureInfo language)
		{
			string text = (language != null) ? language.Name : CultureInfo.InvariantCulture.Name;
			Uri uri = HelpProvider.ConstructTenantEventUrl(notification.EventSource, string.Empty, notification.EventDisplayId.ToString(CultureInfo.InvariantCulture), text);
			if (uri == null)
			{
				base.WriteDebug(Strings.TenantNotificationDebugHelpProviderReturnedEmptyUrl(notification.EventSource, notification.EventDisplayId, text));
				return "http://help.outlook.com/ms.exch.evt.default.aspx";
			}
			return uri.ToString();
		}

		private static readonly MailboxIdParameter FederatedMailboxId = new MailboxIdParameter("FederatedEmail.4c1f4d8b-8179-4148-93bf-00a95fa1e042");

		private SwitchParameter showDuplicates;

		private struct NotificationMerger : IComparer<Notification>
		{
			public int Compare(Notification x, Notification y)
			{
				if (x == null)
				{
					throw new ArgumentNullException("x");
				}
				if (y == null)
				{
					throw new ArgumentNullException("y");
				}
				return y.CreationTimeUtc.CompareTo(x.CreationTimeUtc);
			}

			internal IEnumerable<Notification> RemoveDuplicatesAndSort(IEnumerable<Notification> notifications, bool removeDuplicates)
			{
				if (notifications == null)
				{
					return null;
				}
				if (!removeDuplicates)
				{
					return this.SortByCreationTimeDescending(notifications);
				}
				return this.SortByCreationTimeDescending(this.RemoveDuplicates(notifications));
			}

			private IEnumerable<Notification> RemoveDuplicates(IEnumerable<Notification> notifications)
			{
				IEnumerable<IGrouping<long, Notification>> groupedByEventSourceAndId = from n in notifications
				group n by GetExchangeNotification.NotificationMerger.HashForDuplicateRemoval(n);
				foreach (IGrouping<long, Notification> group in groupedByEventSourceAndId)
				{
					yield return group.Aggregate(delegate(Notification oldest, Notification current)
					{
						if (!(current.CreationTimeUtc < oldest.CreationTimeUtc))
						{
							return oldest;
						}
						return current;
					});
				}
				yield break;
			}

			private IEnumerable<Notification> SortByCreationTimeDescending(IEnumerable<Notification> notifications)
			{
				return notifications.OrderBy((Notification n) => n, this);
			}

			private static long HashForDuplicateRemoval(Notification notification)
			{
				return notification.ComputeHashCodeForDuplicateDetection();
			}
		}
	}
}

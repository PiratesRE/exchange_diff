using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class CalendarVDirSettingsLoader : OwaSettingsLoaderBase
	{
		public override bool IsPushNotificationsEnabled
		{
			get
			{
				return false;
			}
		}

		public override bool IsPullNotificationsEnabled
		{
			get
			{
				return false;
			}
		}

		public override bool IsFolderContentNotificationsEnabled
		{
			get
			{
				return false;
			}
		}

		public override bool IsPreCheckinApp
		{
			get
			{
				return false;
			}
		}

		public override int ConnectionCacheSize
		{
			get
			{
				return 0;
			}
		}

		public override bool ListenAdNotifications
		{
			get
			{
				return false;
			}
		}

		public override bool RenderBreadcrumbsInAboutPage
		{
			get
			{
				return false;
			}
		}

		public override int MaximumTemporaryFilteredViewPerUser
		{
			get
			{
				return 0;
			}
		}

		public override int MaximumFilteredViewInFavoritesPerUser
		{
			get
			{
				return 0;
			}
		}

		public override bool DisableBreadcrumbs
		{
			get
			{
				return true;
			}
		}

		public override int MaxBreadcrumbs
		{
			get
			{
				return 0;
			}
		}

		public override bool StoreTransientExceptionEventLogEnabled
		{
			get
			{
				return false;
			}
		}

		public override int StoreTransientExceptionEventLogThreshold
		{
			get
			{
				return 0;
			}
		}

		public override int StoreTransientExceptionEventLogFrequencyInSeconds
		{
			get
			{
				return 0;
			}
		}

		public override int MaxPendingRequestLifeInSeconds
		{
			get
			{
				return 0;
			}
		}

		public override int MaxItemsInConversationExpansion
		{
			get
			{
				return 0;
			}
		}

		public override int MaxItemsInConversationReadingPane
		{
			get
			{
				return 0;
			}
		}

		public override long MaxBytesInConversationReadingPane
		{
			get
			{
				return 0L;
			}
		}

		public override bool HideDeletedItems
		{
			get
			{
				return true;
			}
		}

		public override string OCSServerName
		{
			get
			{
				return null;
			}
		}

		public override int ActivityBasedPresenceDuration
		{
			get
			{
				return 0;
			}
		}

		public override int MailTipsMaxClientCacheSize
		{
			get
			{
				return 0;
			}
		}

		public override int MailTipsMaxMailboxSourcedRecipientSize
		{
			get
			{
				return 0;
			}
		}

		public override int MailTipsClientCacheEntryExpiryInHours
		{
			get
			{
				return 0;
			}
		}

		internal override PhishingLevel MinimumSuspiciousPhishingLevel
		{
			get
			{
				return PhishingLevel.Suspicious1;
			}
		}

		internal override int UserContextLockTimeout
		{
			get
			{
				return 3000;
			}
		}
	}
}

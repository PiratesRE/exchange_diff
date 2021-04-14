using System;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendarConfigurationDataProvider : XsoDictionaryDataProvider
	{
		public CalendarConfigurationDataProvider(ExchangePrincipal mailboxOwner, string action) : base(mailboxOwner, action, new Func<MailboxSession, string, UserConfigurationTypes, bool, IUserConfiguration>(UserConfigurationHelper.GetCalendarConfiguration), new Func<MailboxSession, string, UserConfigurationTypes, bool, IReadableUserConfiguration>(UserConfigurationHelper.GetReadOnlyCalendarConfiguration))
		{
		}

		public CalendarConfigurationDataProvider(MailboxSession session) : base(session, new Func<MailboxSession, string, UserConfigurationTypes, bool, IUserConfiguration>(UserConfigurationHelper.GetCalendarConfiguration), new Func<MailboxSession, string, UserConfigurationTypes, bool, IReadableUserConfiguration>(UserConfigurationHelper.GetReadOnlyCalendarConfiguration))
		{
		}

		internal CalendarConfigurationDataProvider()
		{
		}

		public static bool IsCalendarConfigurationClass(string objectClass)
		{
			return "IPM.Configuration.Calendar".Equals(objectClass, StringComparison.OrdinalIgnoreCase);
		}

		protected override void InternalDelete(ConfigurableObject instance)
		{
			StoreId defaultFolderId = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar);
			if (defaultFolderId != null)
			{
				for (int i = 2; i > 0; i--)
				{
					OperationResult operationResult = base.MailboxSession.UserConfigurationManager.DeleteFolderConfigurations(defaultFolderId, new string[]
					{
						"Calendar"
					});
					if (operationResult == OperationResult.Succeeded)
					{
						return;
					}
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CalendarConfigurationDataProvider>(this);
		}

		internal const string CalendarConfigurationName = "Calendar";

		private const string CalendarConfigurationClass = "IPM.Configuration.Calendar";
	}
}

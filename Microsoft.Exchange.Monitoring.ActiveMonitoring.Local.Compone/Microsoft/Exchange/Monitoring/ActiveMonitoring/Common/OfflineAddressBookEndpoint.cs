using System;
using System.Linq;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class OfflineAddressBookEndpoint : IEndpoint
	{
		public Guid[] OfflineAddressBooks
		{
			get
			{
				return this.cachedOabs;
			}
		}

		public Guid[] OrganizationMailboxDatabases
		{
			get
			{
				return this.cachedOrgMailboxDatabases;
			}
		}

		public bool RestartOnChange
		{
			get
			{
				return true;
			}
		}

		public Exception Exception { get; set; }

		public void Initialize()
		{
			if (!LocalEndpointManager.IsDataCenter)
			{
				Guid[] allOfflineAddressBookGuids = DirectoryAccessor.Instance.GetAllOfflineAddressBookGuids();
				this.cachedOabs = allOfflineAddressBookGuids;
				Guid[] allDatabaseGuidsForOrganizationMailboxes = DirectoryAccessor.Instance.GetAllDatabaseGuidsForOrganizationMailboxes();
				this.cachedOrgMailboxDatabases = allDatabaseGuidsForOrganizationMailboxes;
			}
		}

		public bool DetectChange()
		{
			bool flag = false;
			if (!LocalEndpointManager.IsDataCenter)
			{
				Guid[] allOfflineAddressBookGuids = DirectoryAccessor.Instance.GetAllOfflineAddressBookGuids();
				flag = this.IsChanged(allOfflineAddressBookGuids, this.cachedOabs);
				if (flag)
				{
					return true;
				}
				Guid[] allDatabaseGuidsForOrganizationMailboxes = DirectoryAccessor.Instance.GetAllDatabaseGuidsForOrganizationMailboxes();
				flag = this.IsChanged(allDatabaseGuidsForOrganizationMailboxes, this.cachedOrgMailboxDatabases);
				if (flag)
				{
					return true;
				}
			}
			return flag;
		}

		private bool IsChanged(Guid[] cachedArray, Guid[] newArray)
		{
			return ((newArray == null || newArray.Length == 0) && cachedArray.Length > 0) || (newArray != null && (newArray.Length != cachedArray.Length || !newArray.SequenceEqual(cachedArray)));
		}

		private Guid[] cachedOabs;

		private Guid[] cachedOrgMailboxDatabases;
	}
}

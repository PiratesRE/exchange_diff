using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ADDatabaseCopyWrapper : ADObjectWrapperBase, IADDatabaseCopy, IADObjectCommon, IComparable<ADDatabaseCopyWrapper>
	{
		private ADDatabaseCopyWrapper(DatabaseCopy copy) : base(copy)
		{
			this.DatabaseName = copy.DatabaseName;
			this.HostServerName = copy.HostServerName;
			this.HostServer = copy.HostServer;
			this.ReplayLagTime = copy.ReplayLagTime;
			this.TruncationLagTime = copy.TruncationLagTime;
			this.ActivationPreferenceInternal = copy.ActivationPreferenceInternal;
			this.IsValidForRead = copy.IsValidForRead;
			this.IsHostServerPresent = copy.IsHostServerPresent;
		}

		public static ADDatabaseCopyWrapper CreateWrapper(DatabaseCopy databaseCopy)
		{
			if (databaseCopy == null)
			{
				return null;
			}
			return new ADDatabaseCopyWrapper(databaseCopy);
		}

		public int ActivationPreferenceInternal { get; private set; }

		public int CompareTo(ADDatabaseCopyWrapper other)
		{
			if (other == null)
			{
				return -1;
			}
			return this.ActivationPreferenceInternal.CompareTo(other.ActivationPreferenceInternal);
		}

		public bool IsValidForRead { get; private set; }

		public bool IsHostServerPresent { get; private set; }

		public string DatabaseName { get; private set; }

		public string HostServerName { get; private set; }

		public EnhancedTimeSpan ReplayLagTime { get; private set; }

		public EnhancedTimeSpan TruncationLagTime { get; private set; }

		public int ActivationPreference { get; set; }

		public ADObjectId HostServer { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	internal class MRSRequestMailboxEntry : MRSRequest, IRequestIndexEntry, IConfigurable
	{
		public MRSRequestMailboxEntry()
		{
		}

		public MRSRequestMailboxEntry(UserConfiguration userConfiguration)
		{
			UserConfigurationDictionaryHelper.Fill(userConfiguration, this, MRSRequestSchema.PersistedProperties);
		}

		public RequestIndexId RequestIndexId
		{
			get
			{
				return this.requestIndexId;
			}
			internal set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.requestIndexId = value;
			}
		}

		public static ICollection<MRSRequestMailboxEntry> Read(MailboxSession mailboxSession, RequestIndexEntryQueryFilter requestIndexEntryQueryFilter = null)
		{
			List<MRSRequestMailboxEntry> list = new List<MRSRequestMailboxEntry>();
			ICollection<UserConfiguration> collection = mailboxSession.UserConfigurationManager.FindMailboxConfigurations("MRSRequest", UserConfigurationSearchFlags.Prefix);
			try
			{
				foreach (UserConfiguration userConfiguration in collection)
				{
					MRSRequestMailboxEntry mrsrequestMailboxEntry = new MRSRequestMailboxEntry(userConfiguration);
					if (mrsrequestMailboxEntry.MatchesFilter(requestIndexEntryQueryFilter))
					{
						list.Add(mrsrequestMailboxEntry);
					}
				}
			}
			finally
			{
				foreach (UserConfiguration userConfiguration2 in collection)
				{
					userConfiguration2.Dispose();
				}
			}
			return list;
		}

		private bool MatchesFilter(RequestIndexEntryQueryFilter requestIndexEntryQueryFilter)
		{
			if (requestIndexEntryQueryFilter == null)
			{
				return true;
			}
			if (requestIndexEntryQueryFilter.RequestType != base.Type)
			{
				return false;
			}
			if (requestIndexEntryQueryFilter.RequestGuid != Guid.Empty && requestIndexEntryQueryFilter.RequestGuid != base.RequestGuid)
			{
				return false;
			}
			if (requestIndexEntryQueryFilter.SourceMailbox != null && requestIndexEntryQueryFilter.SourceMailbox.ObjectGuid != ((base.SourceUserId == null) ? Guid.Empty : base.SourceUserId.ObjectGuid))
			{
				return false;
			}
			if (requestIndexEntryQueryFilter.TargetMailbox != null && requestIndexEntryQueryFilter.TargetMailbox.ObjectGuid != ((base.TargetUserId == null) ? Guid.Empty : base.TargetUserId.ObjectGuid))
			{
				return false;
			}
			if (requestIndexEntryQueryFilter.SourceDatabase != null && requestIndexEntryQueryFilter.SourceDatabase.ObjectGuid != ((base.SourceMDB == null) ? Guid.Empty : base.SourceMDB.ObjectGuid))
			{
				return false;
			}
			if (requestIndexEntryQueryFilter.TargetDatabase != null && requestIndexEntryQueryFilter.TargetDatabase.ObjectGuid != ((base.TargetMDB == null) ? Guid.Empty : base.TargetMDB.ObjectGuid))
			{
				return false;
			}
			if (requestIndexEntryQueryFilter.Status != RequestStatus.None && requestIndexEntryQueryFilter.Status != base.Status)
			{
				return false;
			}
			if (requestIndexEntryQueryFilter.Flags != RequestFlags.None && (requestIndexEntryQueryFilter.Flags & base.Flags) != requestIndexEntryQueryFilter.Flags)
			{
				return false;
			}
			if (requestIndexEntryQueryFilter.NotFlags != RequestFlags.None && (~requestIndexEntryQueryFilter.NotFlags & base.Flags) != base.Flags)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(requestIndexEntryQueryFilter.BatchName) && requestIndexEntryQueryFilter.BatchName.Equals(base.BatchName, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			string text = requestIndexEntryQueryFilter.WildcardedNameSearch ? Wildcard.ConvertToRegexPattern(requestIndexEntryQueryFilter.RequestName) : requestIndexEntryQueryFilter.RequestName;
			return string.IsNullOrEmpty(text) || !(requestIndexEntryQueryFilter.WildcardedNameSearch ? (!Regex.IsMatch(base.Name, text, RegexOptions.IgnoreCase)) : (!base.Name.Equals(text, StringComparison.OrdinalIgnoreCase)));
		}

		public RequestJobObjectId GetRequestJobId()
		{
			return new RequestJobObjectId(base.RequestGuid, (base.StorageMDB == null) ? Guid.Empty : base.StorageMDB.ObjectGuid, this);
		}

		public RequestIndexEntryObjectId GetRequestIndexEntryId(RequestBase owner)
		{
			return new RequestIndexEntryObjectId(base.RequestGuid, base.Type, base.OrganizationId, this.RequestIndexId, owner);
		}

		Guid IRequestIndexEntry.get_RequestGuid()
		{
			return base.RequestGuid;
		}

		void IRequestIndexEntry.set_RequestGuid(Guid A_1)
		{
			base.RequestGuid = A_1;
		}

		string IRequestIndexEntry.get_Name()
		{
			return base.Name;
		}

		void IRequestIndexEntry.set_Name(string A_1)
		{
			base.Name = A_1;
		}

		RequestStatus IRequestIndexEntry.get_Status()
		{
			return base.Status;
		}

		void IRequestIndexEntry.set_Status(RequestStatus A_1)
		{
			base.Status = A_1;
		}

		RequestFlags IRequestIndexEntry.get_Flags()
		{
			return base.Flags;
		}

		void IRequestIndexEntry.set_Flags(RequestFlags A_1)
		{
			base.Flags = A_1;
		}

		string IRequestIndexEntry.get_RemoteHostName()
		{
			return base.RemoteHostName;
		}

		void IRequestIndexEntry.set_RemoteHostName(string A_1)
		{
			base.RemoteHostName = A_1;
		}

		string IRequestIndexEntry.get_BatchName()
		{
			return base.BatchName;
		}

		void IRequestIndexEntry.set_BatchName(string A_1)
		{
			base.BatchName = A_1;
		}

		ADObjectId IRequestIndexEntry.get_SourceMDB()
		{
			return base.SourceMDB;
		}

		void IRequestIndexEntry.set_SourceMDB(ADObjectId A_1)
		{
			base.SourceMDB = A_1;
		}

		ADObjectId IRequestIndexEntry.get_TargetMDB()
		{
			return base.TargetMDB;
		}

		void IRequestIndexEntry.set_TargetMDB(ADObjectId A_1)
		{
			base.TargetMDB = A_1;
		}

		ADObjectId IRequestIndexEntry.get_StorageMDB()
		{
			return base.StorageMDB;
		}

		void IRequestIndexEntry.set_StorageMDB(ADObjectId A_1)
		{
			base.StorageMDB = A_1;
		}

		string IRequestIndexEntry.get_FilePath()
		{
			return base.FilePath;
		}

		void IRequestIndexEntry.set_FilePath(string A_1)
		{
			base.FilePath = A_1;
		}

		MRSRequestType IRequestIndexEntry.get_Type()
		{
			return base.Type;
		}

		void IRequestIndexEntry.set_Type(MRSRequestType A_1)
		{
			base.Type = A_1;
		}

		ADObjectId IRequestIndexEntry.get_TargetUserId()
		{
			return base.TargetUserId;
		}

		void IRequestIndexEntry.set_TargetUserId(ADObjectId A_1)
		{
			base.TargetUserId = A_1;
		}

		ADObjectId IRequestIndexEntry.get_SourceUserId()
		{
			return base.SourceUserId;
		}

		void IRequestIndexEntry.set_SourceUserId(ADObjectId A_1)
		{
			base.SourceUserId = A_1;
		}

		OrganizationId IRequestIndexEntry.get_OrganizationId()
		{
			return base.OrganizationId;
		}

		DateTime? IRequestIndexEntry.get_WhenChanged()
		{
			return base.WhenChanged;
		}

		DateTime? IRequestIndexEntry.get_WhenCreated()
		{
			return base.WhenCreated;
		}

		DateTime? IRequestIndexEntry.get_WhenChangedUTC()
		{
			return base.WhenChangedUTC;
		}

		DateTime? IRequestIndexEntry.get_WhenCreatedUTC()
		{
			return base.WhenCreatedUTC;
		}

		private RequestIndexId requestIndexId;
	}
}

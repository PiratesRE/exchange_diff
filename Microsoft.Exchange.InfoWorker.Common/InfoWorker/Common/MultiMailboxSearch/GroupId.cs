using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class GroupId
	{
		public GroupId(GroupType type, Uri uri, int version, string domain = null)
		{
			this.groupType = type;
			this.uri = uri;
			this.serverVersion = version;
			this.Domain = domain;
		}

		public GroupId(Exception error)
		{
			Util.ThrowOnNull(error, "error");
			this.groupType = GroupType.SkippedError;
			this.error = error;
			this.serverVersion = Server.E15MinVersion;
		}

		public GroupType GroupType
		{
			get
			{
				return this.groupType;
			}
		}

		public Uri Uri
		{
			get
			{
				return this.uri;
			}
		}

		public Exception Error
		{
			get
			{
				return this.error;
			}
		}

		public int ServerVersion
		{
			get
			{
				return this.serverVersion;
			}
		}

		public string Domain { get; set; }

		public override bool Equals(object other)
		{
			GroupId groupId = (GroupId)other;
			return groupId != null && this.GroupType == groupId.GroupType && (!(this.Uri != null) || this.Uri.Equals(groupId.Uri)) && (!(this.Uri == null) || !(groupId.Uri != null)) && this.ServerVersion == groupId.ServerVersion;
		}

		public override int GetHashCode()
		{
			int num = (int)this.GroupType;
			if (this.Uri != null)
			{
				num ^= this.Uri.GetHashCode();
			}
			return num ^ this.ServerVersion.GetHashCode();
		}

		private readonly GroupType groupType;

		private readonly Uri uri;

		private readonly Exception error;

		private readonly int serverVersion;
	}
}

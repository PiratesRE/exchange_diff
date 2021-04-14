using System;
using System.Text;
using Microsoft.Exchange.Data.Directory.DirSync;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[Serializable]
	internal class SyncLink : ADDirSyncLink
	{
		public SyncLink(string targetId, LinkState linkState) : base(null, linkState)
		{
			this.targetId = targetId;
		}

		public SyncLink(string targetId, DirectoryObjectClass targetObjectClass, LinkState linkState) : this(targetId, linkState)
		{
			this.targetObjectClass = targetObjectClass;
		}

		public SyncLink(ADObjectId link, LinkState state) : base(link, state)
		{
		}

		public static SyncLink ParseFromADString(string adString)
		{
			if (string.IsNullOrEmpty(adString))
			{
				throw new FormatException(DirectoryStrings.InvalidSyncLinkFormat(adString));
			}
			string[] array = adString.Split(new char[]
			{
				','
			});
			if (array.Length != 4)
			{
				throw new FormatException(DirectoryStrings.InvalidSyncLinkFormat(adString));
			}
			SyncLink syncLink = null;
			try
			{
				LinkState linkState = (LinkState)Enum.Parse(typeof(LinkState), array[0]);
				if (!string.IsNullOrEmpty(array[0]))
				{
					string @string = Encoding.UTF8.GetString(Convert.FromBase64String(array[1]));
					syncLink = new SyncLink(@string, linkState);
				}
				else
				{
					string string2 = Encoding.UTF8.GetString(Convert.FromBase64String(array[2]));
					ADObjectId link = ADObjectId.ParseDnOrGuid(string2);
					syncLink = new SyncLink(link, linkState);
				}
				syncLink.targetObjectClass = (DirectoryObjectClass)Enum.Parse(typeof(DirectoryObjectClass), array[3]);
			}
			catch (FormatException innerException)
			{
				throw new FormatException(DirectoryStrings.InvalidSyncLinkFormat(adString), innerException);
			}
			return syncLink;
		}

		public void UpdateSyncData(string targetId, DirectoryObjectClass targetObjectClass)
		{
			this.targetId = targetId;
			this.targetObjectClass = targetObjectClass;
		}

		public string TargetId
		{
			get
			{
				return this.targetId;
			}
		}

		public DirectoryObjectClass TargetObjectClass
		{
			get
			{
				return this.targetObjectClass;
			}
		}

		public override string ToString()
		{
			return this.targetId ?? string.Empty;
		}

		public override bool Equals(object obj)
		{
			SyncLink syncLink = obj as SyncLink;
			return syncLink != null && (this.targetId == syncLink.targetId && base.Link == syncLink.Link && base.State == syncLink.State) && this.targetObjectClass == syncLink.targetObjectClass;
		}

		public override int GetHashCode()
		{
			return (int)(((this.targetId == null) ? 0 : this.targetId.GetHashCode()) + ((base.Link == null) ? 0 : base.Link.GetHashCode()) + this.targetObjectClass);
		}

		public string ToADString()
		{
			return string.Format("{0},{1},{2},{3}", new object[]
			{
				base.State.ToString(),
				string.IsNullOrEmpty(this.targetId) ? string.Empty : Convert.ToBase64String(Encoding.UTF8.GetBytes(this.targetId)),
				(base.Link == null) ? string.Empty : Convert.ToBase64String(Encoding.UTF8.GetBytes(base.Link.ToGuidOrDNString())),
				this.targetObjectClass.ToString()
			});
		}

		private string targetId;

		private DirectoryObjectClass targetObjectClass;
	}
}

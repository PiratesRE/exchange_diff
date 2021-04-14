using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Management.Migration;
using Microsoft.Exchange.Management.DDIService;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[KnownType(typeof(MailboxIdentity))]
	[KnownType(typeof(MailboxFolderPermissionIdentity))]
	public class Identity : INamedIdentity
	{
		public Identity(ADObjectId identity, string displayName) : this((identity.ObjectGuid == Guid.Empty) ? identity.DistinguishedName : identity.ObjectGuid.ToString(), displayName)
		{
		}

		public Identity(string rawIdentity, string displayName)
		{
			this.RawIdentity = rawIdentity;
			this.DisplayName = displayName;
			this.OnDeserialized(default(StreamingContext));
		}

		public Identity(string identity) : this(identity, identity)
		{
		}

		public Identity(ADObjectId identity) : this(identity, identity.Name)
		{
		}

		public Identity(DagNetworkObjectId id) : this(id.FullName, id.NetName)
		{
		}

		public Identity(AppId id) : this(id.MailboxOwnerId.ObjectGuid.ToString() + '\\' + id.AppIdValue, id.DisplayName)
		{
		}

		public Identity(MigrationBatchId id) : this(id.Id, id.ToString())
		{
		}

		public Identity(MigrationUserId id) : this((id.JobItemGuid != Guid.Empty) ? id.JobItemGuid.ToString() : id.Id, id.ToString())
		{
		}

		public Identity(MigrationEndpointId id) : this(id.Id, id.ToString())
		{
		}

		public Identity(MigrationReportId id) : this(id.ToString(), id.ToString())
		{
		}

		public Identity(MigrationStatisticsId id) : this(id.ToString(), id.ToString())
		{
		}

		[DataMember]
		public string DisplayName { get; private set; }

		[DataMember(IsRequired = true)]
		public string RawIdentity
		{
			get
			{
				return this.rawIdentity;
			}
			private set
			{
				value.FaultIfNullOrEmpty("RawIdentity cannot be null.");
				this.rawIdentity = value;
			}
		}

		string INamedIdentity.Identity
		{
			get
			{
				return this.RawIdentity;
			}
		}

		private ADObjectId InternalIdentity { get; set; }

		public static Identity FromExecutingUserId()
		{
			Identity identity = (EacRbacPrincipal.Instance.ExecutingUserId == null) ? new Identity(EacRbacPrincipal.Instance.Name, null) : new Identity(EacRbacPrincipal.Instance.ExecutingUserId, null);
			identity.InternalIdentity = EacRbacPrincipal.Instance.ExecutingUserId;
			return identity;
		}

		public static bool operator ==(Identity identity1, Identity identity2)
		{
			return (identity1 == null && identity2 == null) || (identity1 != null && identity2 != null && (string.Compare(identity1.RawIdentity, identity2.RawIdentity, StringComparison.OrdinalIgnoreCase) == 0 || (string.Compare(identity1.DisplayName, identity2.DisplayName, StringComparison.OrdinalIgnoreCase) == 0 && EacHttpContext.Instance.PostHydrationActionPresent)));
		}

		public static bool operator !=(Identity identity1, Identity identity2)
		{
			return !(identity1 == identity2);
		}

		public object[] ToPipelineInput()
		{
			object obj = this.InternalIdentity ?? this;
			return new object[]
			{
				obj
			};
		}

		internal static Identity FromIdParameter(object value)
		{
			if (value is string)
			{
				return new Identity((string)value, (string)value);
			}
			return null;
		}

		public static PeopleIdentity[] ConvertToPeopleIdentity(object value)
		{
			if (value is RecipientIdParameter[])
			{
				return ((RecipientIdParameter[])value).ToPeopleIdentityArray();
			}
			return null;
		}

		internal static Identity[] FromIdParameters(object value)
		{
			if (value is string[])
			{
				return (from v in (string[])value
				select Identity.FromIdParameter(v)).ToArray<Identity>();
			}
			return null;
		}

		internal static Identity ParseIdentity(string idStr)
		{
			if (string.IsNullOrEmpty(idStr))
			{
				return null;
			}
			return new Identity(idStr, idStr);
		}

		public bool Equals(Identity identity)
		{
			return this == identity;
		}

		public override bool Equals(object obj)
		{
			return this.Equals((Identity)obj);
		}

		public override int GetHashCode()
		{
			return this.RawIdentity.GetHashCode();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (string.IsNullOrEmpty(this.DisplayName))
			{
				this.DisplayName = this.RawIdentity;
			}
		}

		private static readonly Regex guidRegex = new Regex("^[0-9a-fA-F]{8}(\\-[0-9a-fA-F]{4}){3}\\-[0-9a-fA-F]{12}$", RegexOptions.CultureInvariant);

		private string rawIdentity;
	}
}

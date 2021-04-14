using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Management.Migration
{
	[Serializable]
	public class MigrationUserIdParameter : IIdentityParameter
	{
		public MigrationUserIdParameter() : this(new MigrationUserId(string.Empty, Guid.Empty))
		{
		}

		public MigrationUserIdParameter(INamedIdentity namedIdentity)
		{
			if (namedIdentity == null)
			{
				throw new ArgumentNullException("namedIdentity");
			}
			this.MigrationUserId = MigrationUserIdParameter.ParseMigrationUserId(namedIdentity.Identity);
			this.RawIdentity = namedIdentity.DisplayName;
		}

		public MigrationUserIdParameter(MigrationUserId identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			this.MigrationUserId = identity;
			this.RawIdentity = this.MigrationUserId.ToString();
		}

		public MigrationUserIdParameter(MigrationUser user) : this(user.Identity)
		{
		}

		public MigrationUserIdParameter(SmtpAddress identity)
		{
			if (identity == SmtpAddress.Empty)
			{
				throw new ArgumentNullException("identity");
			}
			this.MigrationUserId = new MigrationUserId(identity.ToString(), Guid.Empty);
			this.RawIdentity = identity.ToString();
		}

		public MigrationUserIdParameter(string identity)
		{
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentNullException("identity");
			}
			this.MigrationUserId = MigrationUserIdParameter.ParseMigrationUserId(identity);
			this.RawIdentity = identity;
		}

		public MigrationUserIdParameter(Guid identity)
		{
			if (identity == Guid.Empty)
			{
				throw new ArgumentNullException("identity");
			}
			this.MigrationUserId = new MigrationUserId(string.Empty, identity);
			this.RawIdentity = identity.ToString();
		}

		public MigrationUserId MigrationUserId { get; private set; }

		public string RawIdentity { get; private set; }

		public static MigrationUserId ParseMigrationUserId(string identity)
		{
			Guid jobItemGuid;
			if (GuidHelper.TryParseGuid(identity, out jobItemGuid))
			{
				return new MigrationUserId(string.Empty, jobItemGuid);
			}
			return new MigrationUserId(identity, Guid.Empty);
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			IConfigurable[] array = session.Find<T>(null, this.MigrationUserId, false, null);
			for (int i = 0; i < array.Length; i++)
			{
				T instance = (T)((object)array[i]);
				yield return instance;
			}
			yield break;
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			notFoundReason = new LocalizedString?(Strings.MigrationUserNotFound(this.RawIdentity));
			return this.GetObjects<T>(rootId, session);
		}

		public void Initialize(ObjectId objectId)
		{
			MigrationUserId migrationUserId = objectId as MigrationUserId;
			if (migrationUserId == null)
			{
				throw new ArgumentException("objectId");
			}
			this.MigrationUserId = migrationUserId;
		}

		public override string ToString()
		{
			return this.RawIdentity;
		}
	}
}

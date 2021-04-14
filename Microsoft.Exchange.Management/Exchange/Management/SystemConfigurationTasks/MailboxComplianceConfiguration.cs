using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public class MailboxComplianceConfiguration : IConfigurable
	{
		public ObjectId Identity
		{
			get
			{
				return this.identity;
			}
			set
			{
				this.identity = value;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.orgId;
			}
			internal set
			{
				this.orgId = value;
			}
		}

		ValidationError[] IConfigurable.Validate()
		{
			return new ValidationError[0];
		}

		void IConfigurable.CopyChangesFrom(IConfigurable changedObject)
		{
			throw new NotImplementedException();
		}

		void IConfigurable.ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		bool IConfigurable.IsValid
		{
			get
			{
				return true;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return ObjectState.New;
			}
		}

		internal bool NoPolicy
		{
			get
			{
				return this.noPolicy;
			}
			set
			{
				this.noPolicy = value;
			}
		}

		internal MailboxComplianceConfiguration(MailboxSession mailboxSession)
		{
		}

		internal void Save(MailboxSession mailboxSession)
		{
		}

		private ObjectId identity;

		private OrganizationId orgId;

		private bool noPolicy;
	}
}

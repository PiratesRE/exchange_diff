using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Serializable]
	public class DelegateUser : IConfigurable
	{
		public DelegateUser(string id, DelegateRoleType roleType, string scope)
		{
			this.id = id;
			this.scope = scope;
			this.roleType = roleType;
			this.identity = new DelegateUserId(id);
		}

		public string Identity
		{
			get
			{
				return this.id;
			}
		}

		public string Scope
		{
			get
			{
				return this.scope;
			}
		}

		public DelegateRoleType Role
		{
			get
			{
				return this.roleType;
			}
		}

		ObjectId IConfigurable.Identity
		{
			get
			{
				return this.identity;
			}
		}

		ValidationError[] IConfigurable.Validate()
		{
			return ValidationError.None;
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
				return ObjectState.Unchanged;
			}
		}

		void IConfigurable.CopyChangesFrom(IConfigurable source)
		{
			throw new NotSupportedException();
		}

		void IConfigurable.ResetChangeTracking()
		{
			throw new NotSupportedException();
		}

		private readonly string id;

		private DelegateUserId identity;

		private readonly string scope;

		private DelegateRoleType roleType;
	}
}

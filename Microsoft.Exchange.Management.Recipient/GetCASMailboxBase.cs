using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class GetCASMailboxBase<TIdentity> : GetRecipientBase<TIdentity, ADUser> where TIdentity : RecipientIdParameter, new()
	{
		public GetCASMailboxBase()
		{
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<CASMailboxSchema>();
			}
		}

		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetCASMailboxBase<TIdentity>.SortPropertiesArray;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			ADUser aduser = (ADUser)dataObject;
			base.WriteResult(aduser);
			if (!aduser.IsOWAEnabledStatusConsistent())
			{
				this.WriteWarning(Strings.ErrorOWAEnabledStatusNotConsistent(aduser.Identity.ToString()));
			}
		}

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ADObjectSchema.Name,
			CASMailboxSchema.DisplayName,
			CASMailboxSchema.ServerLegacyDN
		};
	}
}

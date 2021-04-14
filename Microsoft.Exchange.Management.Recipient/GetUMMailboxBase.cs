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
	public abstract class GetUMMailboxBase<TIdentity> : GetRecipientBase<TIdentity, ADUser> where TIdentity : RecipientIdParameter, new()
	{
		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter internalFilter = base.InternalFilter;
				QueryFilter queryFilter = new BitMaskAndFilter(ADUserSchema.UMEnabledFlags, 1UL);
				if (internalFilter != null)
				{
					queryFilter = new AndFilter(new QueryFilter[]
					{
						internalFilter,
						queryFilter
					});
				}
				return queryFilter;
			}
		}

		public GetUMMailboxBase()
		{
		}

		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetUMMailboxBase<MailboxIdParameter>.SortPropertiesArray;
			}
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<UMMailboxSchema>();
			}
		}

		protected override bool ShouldSkipObject(IConfigurable dataObject)
		{
			if (this.Identity != null)
			{
				ADUser aduser = (ADUser)dataObject;
				if ((aduser.UMEnabledFlags & UMEnabledFlags.UMEnabled) != UMEnabledFlags.UMEnabled)
				{
					TIdentity identity = this.Identity;
					base.WriteVerbose(Strings.MailboxNotUmEnabled(identity.ToString()));
					return true;
				}
			}
			return base.ShouldSkipObject(dataObject);
		}

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ADObjectSchema.Name,
			UMMailboxSchema.DisplayName,
			UMMailboxSchema.ServerLegacyDN
		};
	}
}

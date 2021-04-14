using System;
using System.Security.Principal;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class MiniRecipientWithTokenGroups : MiniRecipient
	{
		internal MiniRecipientWithTokenGroups(IRecipientSession session, PropertyBag propertyBag)
		{
			this.m_Session = session;
			this.propertyBag = (ADPropertyBag)propertyBag;
		}

		public MiniRecipientWithTokenGroups()
		{
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return MiniRecipientWithTokenGroups.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				throw new InvalidADObjectOperationException(DirectoryStrings.ExceptionMostDerivedOnBase("MiniRecipientWithTokenGroups"));
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return null;
			}
		}

		public MultiValuedProperty<SecurityIdentifier> TokenGroupsGlobalAndUniversal
		{
			get
			{
				return (MultiValuedProperty<SecurityIdentifier>)this[MiniRecipientWithTokenGroupsSchema.TokenGroupsGlobalAndUniversal];
			}
		}

		private static readonly MiniRecipientWithTokenGroupsSchema schema = ObjectSchema.GetInstance<MiniRecipientWithTokenGroupsSchema>();
	}
}

using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public abstract class OrganizationalUnitIdParameterBase : ADIdParameter
	{
		public OrganizationalUnitIdParameterBase()
		{
		}

		public OrganizationalUnitIdParameterBase(string identity) : base(identity)
		{
		}

		public OrganizationalUnitIdParameterBase(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public OrganizationalUnitIdParameterBase(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public override string ToString()
		{
			if (this.canonicalName != null)
			{
				return this.canonicalName;
			}
			return base.ToString();
		}

		internal string GetCanonicalName(IDirectorySession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (this.canonicalName == null)
			{
				if (base.InternalADObjectId == null || string.IsNullOrEmpty(base.InternalADObjectId.DistinguishedName))
				{
					this.GetDistinguishedName(session);
				}
				this.canonicalName = NativeHelpers.CanonicalNameFromDistinguishedName(base.InternalADObjectId.DistinguishedName);
				this.canonicalName = this.canonicalName.TrimEnd(new char[]
				{
					'/'
				});
			}
			return this.canonicalName;
		}

		internal string GetDistinguishedName(IDirectorySession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (base.InternalADObjectId == null && base.RawIdentity == null)
			{
				throw new InvalidOperationException(Strings.ErrorUninitializedParameter);
			}
			if (base.InternalADObjectId == null || string.IsNullOrEmpty(base.InternalADObjectId.DistinguishedName))
			{
				ADObjectId internalADObjectId = base.InternalADObjectId;
				if (internalADObjectId != null || ADIdParameter.TryResolveCanonicalName(base.RawIdentity, out internalADObjectId))
				{
					ADRawEntry adrawEntry = session.ReadADRawEntry(internalADObjectId, new ADPropertyDefinition[]
					{
						ADObjectSchema.Id
					});
					if (adrawEntry != null)
					{
						base.UpdateInternalADObjectId(adrawEntry.Id);
					}
				}
				if (base.InternalADObjectId == null || string.IsNullOrEmpty(base.InternalADObjectId.DistinguishedName))
				{
					throw new NameConversionException(Strings.ErrorConversionFailed(base.RawIdentity));
				}
			}
			return base.InternalADObjectId.DistinguishedName;
		}

		protected string canonicalName;
	}
}

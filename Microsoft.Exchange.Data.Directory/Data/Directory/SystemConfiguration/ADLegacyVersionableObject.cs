using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public abstract class ADLegacyVersionableObject : ADConfigurationObject
	{
		internal int? MinAdminVersion
		{
			get
			{
				return (int?)this[ADLegacyVersionableObjectSchema.MinAdminVersion];
			}
			set
			{
				this[ADLegacyVersionableObjectSchema.MinAdminVersion] = value;
			}
		}

		internal void StampDefaultMinAdminVersion()
		{
			object obj = null;
			if (!this.propertyBag.TryGetField(ADLegacyVersionableObjectSchema.MinAdminVersion, ref obj))
			{
				this.MinAdminVersion = new int?(this.MaximumSupportedExchangeObjectVersion.ExchangeBuild.ToExchange2003FormatInt32());
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (base.IsChanged(ADLegacyVersionableObjectSchema.MinAdminVersion) || base.IsChanged(ADObjectSchema.ExchangeVersion))
			{
				if (this.MinAdminVersion == null)
				{
					if (!base.ExchangeVersion.Equals(ExchangeObjectVersion.Exchange2003))
					{
						errors.Add(new PropertyValidationError(DirectoryStrings.ErrorMinAdminVersionNull(base.ExchangeVersion), ADLegacyVersionableObjectSchema.MinAdminVersion, this.MinAdminVersion));
						return;
					}
				}
				else if (base.ExchangeVersion.ExchangeBuild.ToExchange2003FormatInt32() != this.MinAdminVersion.Value)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.ErrorMinAdminVersionOutOfSync(this.MinAdminVersion.Value, base.ExchangeVersion, base.ExchangeVersion.ExchangeBuild.ToExchange2003FormatInt32()), ADLegacyVersionableObjectSchema.MinAdminVersion, this.MinAdminVersion));
				}
			}
		}
	}
}

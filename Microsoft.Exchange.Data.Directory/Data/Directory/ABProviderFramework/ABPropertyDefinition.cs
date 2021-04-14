using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ABProviderFramework
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ABPropertyDefinition : SimpleProviderPropertyDefinition
	{
		public ABPropertyDefinition(string name, Type type, PropertyDefinitionFlags flags, object defaultValue) : base(name, ExchangeObjectVersion.Exchange2010, type, flags, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None)
		{
			if ((flags & ABPropertyDefinition.UnsupportedFlags) != PropertyDefinitionFlags.None)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "ABPropertyDefinition '{0}' has unsupported flags '{1}'.", new object[]
				{
					name,
					flags
				}));
			}
			if ((flags & ABPropertyDefinition.MustHaveFlags) != ABPropertyDefinition.MustHaveFlags)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "ABPropertyDefinition '{0}' has flags '{1}' - '{2}' flags are required.", new object[]
				{
					name,
					flags,
					ABPropertyDefinition.MustHaveFlags
				}));
			}
		}

		private static readonly PropertyDefinitionFlags UnsupportedFlags = PropertyDefinitionFlags.Calculated | PropertyDefinitionFlags.FilterOnly | PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue | PropertyDefinitionFlags.WriteOnce;

		private static readonly PropertyDefinitionFlags MustHaveFlags = PropertyDefinitionFlags.ReadOnly;
	}
}

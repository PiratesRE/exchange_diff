using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AcrPropertyProfile
	{
		internal AcrPropertyProfile(AcrPropertyResolver resolver, bool requireChangeTracking, PropertyDefinition[] propertiesToResolve)
		{
			this.resolver = resolver;
			this.requireChangeTracking = requireChangeTracking;
			this.propertiesToResolve = (propertiesToResolve ?? Array<PropertyDefinition>.Empty);
			this.allProperties = Util.MergeArrays<PropertyDefinition>(new ICollection<PropertyDefinition>[]
			{
				propertiesToResolve,
				resolver.Dependencies
			});
		}

		internal PropertyDefinition[] AllProperties
		{
			get
			{
				return this.allProperties;
			}
		}

		internal PropertyDefinition[] PropertiesToResolve
		{
			get
			{
				return this.propertiesToResolve;
			}
		}

		internal bool RequireChangeTracking
		{
			get
			{
				return this.requireChangeTracking;
			}
		}

		internal AcrPropertyResolver Resolver
		{
			get
			{
				return this.resolver;
			}
		}

		private readonly AcrPropertyResolver resolver;

		private readonly bool requireChangeTracking;

		private readonly PropertyDefinition[] allProperties;

		private readonly PropertyDefinition[] propertiesToResolve;

		internal class ValuesToResolve
		{
			public ValuesToResolve(object clientValue, object serverValue, object originalValue)
			{
				this.ClientValue = clientValue;
				this.ServerValue = serverValue;
				this.OriginalValue = originalValue;
			}

			public readonly object ClientValue;

			public readonly object ServerValue;

			public readonly object OriginalValue;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class AcrPropertyResolver
	{
		internal abstract PropertyDefinition[] Dependencies { get; }

		protected void CheckDependenciesArePresent(IList<AcrPropertyProfile.ValuesToResolve> dependencies)
		{
			for (int i = 0; i < dependencies.Count; i++)
			{
				if (dependencies[i] == null)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "ACR Resolver \"{0}\" has a mandatory dependency missing: {1}", new object[]
					{
						this,
						this.Dependencies[i]
					}), "dependencies");
				}
			}
		}

		protected void CheckValuesToResolveArePresent(IList<AcrPropertyProfile.ValuesToResolve> valuesToResolve)
		{
			for (int i = 0; i < valuesToResolve.Count; i++)
			{
				if (valuesToResolve[i] == null)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "ACR Resolver \"{0}\" has a valueToResolve #{1} missing", new object[]
					{
						this,
						i
					}), "valuesToResolve");
				}
			}
		}

		internal object[] Resolve(IList<AcrPropertyProfile.ValuesToResolve> valuesToResolve, IList<AcrPropertyProfile.ValuesToResolve> dependencies)
		{
			return this.InternalResolve(valuesToResolve, dependencies);
		}

		protected abstract object[] InternalResolve(IList<AcrPropertyProfile.ValuesToResolve> valuesToResolve, IList<AcrPropertyProfile.ValuesToResolve> dependencies);
	}
}

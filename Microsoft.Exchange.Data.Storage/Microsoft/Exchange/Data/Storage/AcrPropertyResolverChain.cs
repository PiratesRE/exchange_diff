using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AcrPropertyResolverChain : AcrPropertyResolver
	{
		internal AcrPropertyResolverChain(AcrPropertyResolverChain.ResolutionFunction[] resolutionFunctions, PropertyDefinition[] dependencies, bool ignoreMissingDependencies)
		{
			this.resolutionFunctions = Array.FindAll<AcrPropertyResolverChain.ResolutionFunction>(resolutionFunctions, (AcrPropertyResolverChain.ResolutionFunction f) => f != null);
			this.dependencies = (dependencies ?? Array<PropertyDefinition>.Empty);
			this.ignoreMissingDependencies = ignoreMissingDependencies;
		}

		internal override PropertyDefinition[] Dependencies
		{
			get
			{
				return this.dependencies;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("AcrPropertyResolverChain(");
			foreach (AcrPropertyResolverChain.ResolutionFunction del in this.resolutionFunctions)
			{
				stringBuilder.Append("{");
				stringBuilder.Append(del.GetMethodInfo().Name);
				stringBuilder.Append("}");
			}
			stringBuilder.Append(")[");
			foreach (PropertyDefinition propertyDefinition in this.dependencies)
			{
				stringBuilder.Append("{");
				stringBuilder.Append(propertyDefinition.ToString());
				stringBuilder.Append("}");
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		protected override object[] InternalResolve(IList<AcrPropertyProfile.ValuesToResolve> valuesToResolve, IList<AcrPropertyProfile.ValuesToResolve> dependencies)
		{
			base.CheckValuesToResolveArePresent(valuesToResolve);
			if (!this.ignoreMissingDependencies)
			{
				base.CheckDependenciesArePresent(valuesToResolve);
			}
			foreach (AcrPropertyResolverChain.ResolutionFunction resolutionFunction in this.resolutionFunctions)
			{
				object[] array2 = null;
				try
				{
					array2 = resolutionFunction(valuesToResolve, dependencies);
				}
				catch (InvalidCastException)
				{
				}
				catch (NullReferenceException)
				{
				}
				if (array2 != null)
				{
					return array2;
				}
			}
			return null;
		}

		private readonly PropertyDefinition[] dependencies;

		private readonly AcrPropertyResolverChain.ResolutionFunction[] resolutionFunctions;

		private readonly bool ignoreMissingDependencies;

		internal delegate object[] ResolutionFunction(IList<AcrPropertyProfile.ValuesToResolve> valuesToResolve, IList<AcrPropertyProfile.ValuesToResolve> dependencies);
	}
}

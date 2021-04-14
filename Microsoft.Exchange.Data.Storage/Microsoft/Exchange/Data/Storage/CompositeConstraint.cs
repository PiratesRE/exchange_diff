using System;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class CompositeConstraint : StoreObjectConstraint
	{
		internal CompositeConstraint(StoreObjectConstraint[] constraints) : base(CompositeConstraint.GetPropertyDefinitions(constraints))
		{
			this.constraints = constraints;
		}

		private static PropertyDefinition[] GetPropertyDefinitions(StoreObjectConstraint[] constraints)
		{
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>();
			foreach (StoreObjectConstraint storeObjectConstraint in constraints)
			{
				foreach (PropertyDefinition item in storeObjectConstraint.RelevantProperties)
				{
					hashSet.TryAdd(item);
				}
			}
			return hashSet.ToArray();
		}

		protected StoreObjectConstraint[] Constraints
		{
			get
			{
				return this.constraints;
			}
		}

		protected abstract string CompositionTypeDescription { get; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}(", this.CompositionTypeDescription);
			foreach (StoreObjectConstraint storeObjectConstraint in this.constraints)
			{
				stringBuilder.AppendFormat("({0})", storeObjectConstraint.ToString());
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		private readonly StoreObjectConstraint[] constraints;
	}
}

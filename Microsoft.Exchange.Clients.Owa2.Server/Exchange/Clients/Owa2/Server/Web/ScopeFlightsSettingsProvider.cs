using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class ScopeFlightsSettingsProvider
	{
		public ScopeFlightsSettingsProvider()
		{
			this.variantConfigurationSnapshotFactory = delegate(string scope)
			{
				ScopeFlightsSettingsProvider.ScopeConstraintProvider constraintProvider = new ScopeFlightsSettingsProvider.ScopeConstraintProvider(scope);
				return VariantConfiguration.GetSnapshot(constraintProvider, null, null);
			};
		}

		public static IReadOnlyList<string> LogicalScopes
		{
			get
			{
				return ScopeFlightsSettingsProvider.logicalScopes;
			}
		}

		public ScopeFlightsSettingsProvider(Func<string, VariantConfigurationSnapshot> variantConfigurationSnapshotFactory)
		{
			this.variantConfigurationSnapshotFactory = variantConfigurationSnapshotFactory;
		}

		public static bool IsLogicalScope(string scope)
		{
			return ScopeFlightsSettingsProvider.LogicalScopes.Contains(scope) || scope.StartsWith("team.");
		}

		public IList<ScopeFlightsSetting> GetFlightsForScope()
		{
			List<ScopeFlightsSetting> list = new List<ScopeFlightsSetting>();
			for (int i = 0; i < ScopeFlightsSettingsProvider.LogicalScopes.Count; i++)
			{
				string scope = ScopeFlightsSettingsProvider.LogicalScopes[i];
				string[] flightsForScope = this.GetFlightsForScope(scope);
				list.Add(new ScopeFlightsSetting(scope, flightsForScope));
			}
			return list;
		}

		public string[] GetFlightsForScope(string scope)
		{
			VariantConfigurationSnapshot variantConfigurationSnapshot = this.variantConfigurationSnapshotFactory(scope);
			return variantConfigurationSnapshot.Flights;
		}

		private static readonly IReadOnlyList<string> logicalScopes = new string[]
		{
			"WorldWide",
			"Microsoft",
			"Dogfood",
			"team.GuestAccess",
			"team.OWA",
			"team.Compass"
		};

		private readonly Func<string, VariantConfigurationSnapshot> variantConfigurationSnapshotFactory;

		private class ScopeConstraintProvider : IConstraintProvider
		{
			public ScopeConstraintProvider(string scope)
			{
				this.constraints = ConstraintCollection.CreateEmpty();
				if (!scope.Equals("WorldWide", StringComparison.CurrentCultureIgnoreCase))
				{
					this.constraints.Add(VariantType.Organization, "Microsoft");
					if (!scope.Equals("Microsoft", StringComparison.CurrentCultureIgnoreCase))
					{
						this.constraints.Add(scope, true);
					}
				}
			}

			public ConstraintCollection Constraints
			{
				get
				{
					return this.constraints;
				}
			}

			public string RampId
			{
				get
				{
					return "ScopeConstraints";
				}
			}

			public string RotationId
			{
				get
				{
					return "ScopeConstraints";
				}
			}

			private const string ScopeConstraintsId = "ScopeConstraints";

			private readonly ConstraintCollection constraints;
		}
	}
}

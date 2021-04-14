using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Flighting;
using Microsoft.Exchange.VariantConfiguration.DataLoad;
using Microsoft.Search.Platform.Parallax;
using Microsoft.Search.Platform.Parallax.DataLoad;

namespace Microsoft.Exchange.VariantConfiguration
{
	public class VariantConfigurationSnapshotProvider
	{
		internal VariantConfigurationSnapshotProvider(IDataSourceReader dataSourceReader, IDataTransformation dataTransformation, string[] dataSourceNames, VariantConfigurationBehavior behavior, string flightingConfigFileName, string overrideAllowedTeam)
		{
			this.Behavior = behavior;
			this.DataSourceNames = dataSourceNames;
			this.flightingConfigFileName = flightingConfigFileName;
			this.overrideAllowedTeamName = overrideAllowedTeam;
			IEnumerable<string> preloadDataSources;
			if (!this.Behavior.HasFlag(VariantConfigurationBehavior.DelayLoadDataSources))
			{
				preloadDataSources = dataSourceNames;
			}
			else
			{
				List<string> list = new List<string>();
				if (this.Behavior.HasFlag(VariantConfigurationBehavior.EvaluateTeams))
				{
					list.Add(this.flightingConfigFileName);
				}
				preloadDataSources = list;
			}
			this.DataLoader = new VariantConfigurationDataLoader(dataSourceReader, dataTransformation, preloadDataSources);
			this.Container.RegisterDataLoader(this.DataLoader);
		}

		internal string[] DataSourceNames { get; private set; }

		internal VariantConfigurationDataLoader DataLoader { get; private set; }

		internal VariantConfigurationBehavior Behavior { get; private set; }

		internal VariantObjectStoreContainer Container
		{
			get
			{
				if (this.container == null)
				{
					lock (this.containerLock)
					{
						if (this.container == null)
						{
							this.container = VariantObjectStoreContainerFactory.Default.Create();
						}
					}
				}
				return this.container;
			}
		}

		public VariantConfigurationSnapshot GetSnapshot(string rotationId, string rampId, ConstraintCollection constraints, IEnumerable<string> overrideFlights = null)
		{
			if (constraints == null)
			{
				throw new ArgumentNullException("constraints");
			}
			int rotationHash;
			if (string.IsNullOrWhiteSpace(rotationId))
			{
				rotationHash = -1;
			}
			else
			{
				rotationHash = RotationHash.ComputeHash(rotationId);
			}
			VariantObjectStore currentSnapshot = this.Container.GetCurrentSnapshot();
			currentSnapshot.DefaultContext.InitializeFrom(constraints);
			if (this.Behavior.HasFlag(VariantConfigurationBehavior.EvaluateTeams))
			{
				this.AddTeamsToStoreContext(currentSnapshot);
			}
			bool flag = this.Behavior.HasFlag(VariantConfigurationBehavior.EvaluateFlights);
			if (flag)
			{
				if (overrideFlights != null && this.IsUserAllowedOverride(currentSnapshot))
				{
					flag = false;
					using (IEnumerator<string> enumerator = overrideFlights.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string str = enumerator.Current;
							currentSnapshot.DefaultContext.AddVariant("flt." + str, bool.TrueString);
						}
						goto IL_D6;
					}
				}
				this.AddFlightsToStoreContext(currentSnapshot, currentSnapshot.DataSourceNames, rotationHash, rampId);
			}
			IL_D6:
			return new VariantConfigurationSnapshot(currentSnapshot, rotationHash, rampId, flag, this);
		}

		internal void AddFlightsToStoreContext(VariantObjectStore store, IEnumerable<string> dataSourceNames, int rotationHash, string rampId)
		{
			this.AddFlightsToStoreContext(store, dataSourceNames, (IFlight flight) => this.Rotate(rotationHash, flight) && this.Ramp(rampId, flight));
		}

		internal bool Ramp(string rampId, IFlight flight)
		{
			if (string.IsNullOrWhiteSpace(flight.Ramp))
			{
				return true;
			}
			int hash;
			if (string.IsNullOrWhiteSpace(rampId) || string.Equals(rampId, MachineSettingsContext.Local.RampId))
			{
				hash = -1;
			}
			else
			{
				hash = this.ComputeHashWithSeed(rampId, flight.RampSeed);
			}
			return this.IsHashInRange(hash, flight.Ramp, flight.Name);
		}

		internal bool Rotate(int rotationHash, IFlight flight)
		{
			return this.IsHashInRange(rotationHash, flight.Rotate, flight.Name);
		}

		internal int ComputeHashWithSeed(string id, string seed)
		{
			if (string.IsNullOrWhiteSpace(seed))
			{
				seed = string.Empty;
			}
			return RotationHash.ComputeHash(seed + id);
		}

		private bool IsHashInRange(int hash, string range, string flightName)
		{
			if (string.IsNullOrWhiteSpace(range))
			{
				return true;
			}
			if (hash == -1)
			{
				return false;
			}
			range = range.Trim();
			if (range.Equals("norotate", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			string text = null;
			try
			{
				if (range.EndsWith("%"))
				{
					if (range.Equals("100%"))
					{
						return true;
					}
					text = range.Substring(0, range.Length - 1);
					int num = (int)(float.Parse(text) * 10f);
					return hash < num;
				}
				else
				{
					foreach (string text2 in range.Split(new char[]
					{
						':'
					}))
					{
						string[] array2 = text2.Split(new char[]
						{
							','
						});
						text = array2[0];
						int num2 = int.Parse(text);
						text = array2[1];
						int num3 = int.Parse(text);
						if (num2 <= hash && hash <= num3)
						{
							return true;
						}
					}
				}
			}
			catch (FormatException innerException)
			{
				throw new FormatException(string.Format("Failed to parse Rotate or Ramp value '{0}' for flight {1}. The following part of the value is malformed: '{2}'.", range, flightName, text), innerException);
			}
			return false;
		}

		private bool IsUserAllowedOverride(VariantObjectStore store)
		{
			string text = "team." + this.overrideAllowedTeamName;
			string[] variantValues = store.DefaultContext.GetVariantValues(text);
			bool flag;
			return variantValues.Length > 0 && bool.TryParse(variantValues.First<string>(), out flag) && flag;
		}

		private void AddTeamsToStoreContext(VariantObjectStore store)
		{
			VariantObjectProvider resolvedObjectProvider = store.GetResolvedObjectProvider(this.flightingConfigFileName);
			foreach (KeyValuePair<string, ITeam> keyValuePair in resolvedObjectProvider.GetObjectsOfType<ITeam>())
			{
				if (keyValuePair.Value.IsMember)
				{
					store.DefaultContext.AddVariant("team." + keyValuePair.Value.Name, bool.TrueString);
				}
			}
		}

		private void AddFlightsToStoreContext(VariantObjectStore store, IEnumerable<string> dataSourceNames, Func<IFlight, bool> isUserInFlightPercentage)
		{
			foreach (string path in dataSourceNames)
			{
				try
				{
					VariantObjectProvider resolvedObjectProvider = store.GetResolvedObjectProvider(Path.GetFileName(path));
					foreach (KeyValuePair<string, IFlight> keyValuePair in resolvedObjectProvider.GetObjectsOfType<IFlight>())
					{
						if (keyValuePair.Value.Enabled && isUserInFlightPercentage(keyValuePair.Value))
						{
							store.DefaultContext.AddVariant(keyValuePair.Value.Name, bool.TrueString);
						}
					}
				}
				catch (DataSourceNotFoundException)
				{
				}
			}
		}

		private const string FlightPrefix = "flt.";

		private const string FlightFileSuffix = ".flight.ini";

		private const string NoRotationValue = "norotate";

		private const string TeamPrefix = "team.";

		private readonly string overrideAllowedTeamName;

		private readonly string flightingConfigFileName;

		private readonly object containerLock = new object();

		private VariantObjectStoreContainer container;
	}
}

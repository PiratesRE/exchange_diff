using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.VariantConfiguration.DataLoad;
using Microsoft.Exchange.VariantConfiguration.Parser;
using Microsoft.Exchange.VariantConfiguration.Reflection;
using Microsoft.Search.Platform.Parallax.DataLoad;
using Microsoft.Win32;

namespace Microsoft.Exchange.VariantConfiguration
{
	public static class VariantConfiguration
	{
		public static event EventHandler<UpdateCommittedEventArgs> UpdateCommitted;

		internal static event EventHandler<OverridesChangedEventArgs> OverridesChanged;

		public static VariantConfigurationSnapshot InvariantNoFlightingSnapshot
		{
			get
			{
				if (VariantConfiguration.invariantNoFlightingSnapshot == null)
				{
					lock (VariantConfiguration.StaticLock)
					{
						if (VariantConfiguration.invariantNoFlightingSnapshot == null)
						{
							VariantConfiguration.invariantNoFlightingSnapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
						}
					}
				}
				return VariantConfiguration.invariantNoFlightingSnapshot;
			}
		}

		public static VariantConfigurationOverride[] Overrides { get; private set; } = new VariantConfigurationOverride[0];

		public static VariantConfigurationSettings Settings
		{
			get
			{
				return VariantConfiguration.settings;
			}
		}

		public static VariantConfigurationFlights Flights
		{
			get
			{
				return VariantConfiguration.flights;
			}
		}

		internal static Func<string, string, ISettings> TestOverride { get; set; }

		internal static ResourcesDataSourceReader ResourcesDataSourceReader
		{
			get
			{
				if (VariantConfiguration.resourcesDataSourceReader == null)
				{
					lock (VariantConfiguration.StaticLock)
					{
						if (VariantConfiguration.resourcesDataSourceReader == null)
						{
							VariantConfiguration.resourcesDataSourceReader = new ResourcesDataSourceReader(Assembly.GetExecutingAssembly());
						}
					}
				}
				return VariantConfiguration.resourcesDataSourceReader;
			}
		}

		private static VariantConfigurationSnapshotProvider Instance
		{
			get
			{
				if (VariantConfiguration.instance == null)
				{
					lock (VariantConfiguration.StaticLock)
					{
						if (VariantConfiguration.instance == null)
						{
							VariantConfigurationBehavior variantConfigurationBehavior = VariantConfigurationBehavior.DelayLoadDataSources;
							ChainedDataSourceReader chainedDataSourceReader = new ChainedDataSourceReader();
							IEnumerable<string> enumerable = VariantConfiguration.ResourcesDataSourceReader.ResourceNames;
							bool flag2 = false;
							string text = null;
							string[] second;
							if (VariantConfiguration.IsProcessAllowedToReadFiles && VariantConfiguration.TryGetConfigFolder(out text) && !string.IsNullOrEmpty(text) && VariantConfiguration.TryGetConfigFiles(text, out second))
							{
								flag2 = true;
								variantConfigurationBehavior = (variantConfigurationBehavior | VariantConfigurationBehavior.EvaluateFlights | VariantConfigurationBehavior.EvaluateTeams);
								chainedDataSourceReader.AddDataSourceReader(new FilesDataSourceReader(text));
								enumerable = enumerable.Union(second);
							}
							chainedDataSourceReader.AddDataSourceReader(VariantConfiguration.ResourcesDataSourceReader);
							if (flag2)
							{
								VariantConfiguration.changeAccumulator = new FileChangesAccumulator(text, "*.ini", 1000, false);
								VariantConfiguration.changeAccumulator.ChangesAccumulated += delegate(object sender, IEnumerable<string> args)
								{
									VariantConfiguration.ReloadChangedDatasources(args);
								};
								VariantConfiguration.changeAccumulator.ErrorDetected += delegate(object sender, Exception args)
								{
									VariantConfiguration.ReloadDatasources();
								};
							}
							OverrideDataTransformation overrideDataTransformation = OverrideDataTransformation.Get(null);
							FlightReader flightReader = new FlightReader(chainedDataSourceReader, overrideDataTransformation, enumerable);
							VariantConfiguration.OverridesChanged += delegate(object sender, OverridesChangedEventArgs overrides)
							{
								VariantConfiguration.ReloadDatasources();
							};
							VariantConfiguration.UpdateCommitted += delegate(object sender, UpdateCommittedEventArgs args)
							{
								lock (VariantConfiguration.StaticLock)
								{
									VariantConfiguration.localMachineSnapshot = null;
								}
							};
							IDataTransformation dataTransformation = new ChainedTransformation(new IDataTransformation[]
							{
								overrideDataTransformation,
								new FlightDependencyTransformation(flightReader)
							});
							VariantConfigurationSnapshotProvider variantConfigurationSnapshotProvider = new VariantConfigurationSnapshotProvider(chainedDataSourceReader, dataTransformation, (from name in enumerable
							where name.EndsWith(".settings.ini", StringComparison.OrdinalIgnoreCase)
							select name).ToArray<string>(), variantConfigurationBehavior, VariantConfiguration.FlightingConfigFileName, "GuestAccess");
							variantConfigurationSnapshotProvider.Container.DataLoadCommitted += VariantConfiguration.OnDataLoadCommitted;
							VariantConfiguration.instance = variantConfigurationSnapshotProvider;
						}
					}
				}
				return VariantConfiguration.instance;
			}
		}

		private static bool IsProcessAllowedToReadFiles
		{
			get
			{
				if (VariantConfiguration.isProcessAllowedToReadFiles == null)
				{
					using (Process currentProcess = Process.GetCurrentProcess())
					{
						VariantConfiguration.isProcessAllowedToReadFiles = new bool?(!string.Equals(ConstraintCollection.Mode, "enterprise") && !currentProcess.ProcessName.StartsWith("ExSetup", StringComparison.OrdinalIgnoreCase) && !currentProcess.ProcessName.StartsWith("Setup", StringComparison.OrdinalIgnoreCase));
					}
				}
				return VariantConfiguration.isProcessAllowedToReadFiles.Value;
			}
		}

		private static string FlightingConfigFileName
		{
			get
			{
				if (VariantConfiguration.flightingConfigFileName == null)
				{
					VariantConfiguration.flightingConfigFileName = "Flighting.settings.ini";
				}
				return VariantConfiguration.flightingConfigFileName;
			}
		}

		public static void Initialize(string customConfigPath)
		{
			VariantConfiguration.configPath = customConfigPath;
		}

		public static VariantConfigurationSnapshot GetSnapshot(IConstraintProvider constraintProvider, ConstraintCollection additionalConstraints = null, IEnumerable<string> overrideFlights = null)
		{
			if (constraintProvider == null)
			{
				throw new ArgumentNullException("constraintProvider");
			}
			if (constraintProvider == MachineSettingsContext.Local && additionalConstraints == null && overrideFlights == null)
			{
				VariantConfigurationSnapshot variantConfigurationSnapshot = VariantConfiguration.localMachineSnapshot;
				if (variantConfigurationSnapshot == null)
				{
					lock (VariantConfiguration.StaticLock)
					{
						if (VariantConfiguration.localMachineSnapshot == null)
						{
							VariantConfiguration.localMachineSnapshot = VariantConfiguration.Instance.GetSnapshot(constraintProvider.RotationId, constraintProvider.RampId, constraintProvider.Constraints, overrideFlights);
						}
						variantConfigurationSnapshot = VariantConfiguration.localMachineSnapshot;
					}
				}
				return variantConfigurationSnapshot;
			}
			ConstraintCollection constraintCollection = constraintProvider.Constraints;
			if (additionalConstraints != null)
			{
				constraintCollection = new ConstraintCollection(constraintCollection);
				constraintCollection.Add(additionalConstraints);
			}
			return VariantConfiguration.Instance.GetSnapshot(constraintProvider.RotationId, constraintProvider.RampId, constraintCollection, overrideFlights);
		}

		public static VariantConfigurationSnapshotProvider CreateSnapshotProvider(string folder, VariantConfigurationBehavior behavior)
		{
			if (!Directory.Exists(folder))
			{
				throw new ArgumentException(string.Format("Directory '{0}' does not exist.", folder));
			}
			IDataSourceReader dataSourceReader = new FilesDataSourceReader(folder);
			OverrideDataTransformation overrideDataTransformation = OverrideDataTransformation.Get(null);
			string[] configFiles = VariantConfiguration.GetConfigFiles(folder);
			FlightReader flightReader = new FlightReader(dataSourceReader, overrideDataTransformation, configFiles);
			IDataTransformation dataTransformation = new ChainedTransformation(new IDataTransformation[]
			{
				overrideDataTransformation,
				new FlightDependencyTransformation(flightReader)
			});
			string[] dataSourceNames = (from file in configFiles
			where file.EndsWith(".settings.ini", StringComparison.OrdinalIgnoreCase)
			select file).ToArray<string>();
			return new VariantConfigurationSnapshotProvider(dataSourceReader, dataTransformation, dataSourceNames, behavior, VariantConfiguration.FlightingConfigFileName, "GuestAccess");
		}

		public static ConfigurationParser CreateConfigurationParser(string folder)
		{
			if (!Directory.Exists(folder))
			{
				throw new ArgumentException(string.Format("Directory '{0}' does not exist.", folder));
			}
			return ConfigurationParser.Create(from file in VariantConfiguration.GetConfigFiles(folder)
			select Path.Combine(folder, file));
		}

		public static bool SetOverrides(VariantConfigurationOverride[] overrides)
		{
			if (overrides == null)
			{
				overrides = new VariantConfigurationOverride[0];
			}
			if (VariantConfiguration.MatchCurrentOverrides(overrides))
			{
				return false;
			}
			VariantConfiguration.Overrides = overrides;
			EventHandler<OverridesChangedEventArgs> overridesChanged = VariantConfiguration.OverridesChanged;
			if (overridesChanged != null)
			{
				Interlocked.Add(ref VariantConfiguration.updateCommittedAccumulationCounter, VariantConfiguration.OverridesChanged.GetInvocationList().Length);
				overridesChanged(null, new OverridesChangedEventArgs(overrides));
			}
			return true;
		}

		internal static void OnDataLoadCommitted(object sender, TransactionCommittedEventArgs args)
		{
			VariantConfiguration.updateCommittedAccumulationTimer.Change(1000, -1);
			int num = Interlocked.Decrement(ref VariantConfiguration.updateCommittedAccumulationCounter);
			if (num == 0)
			{
				VariantConfiguration.RaiseUpdateCommitted();
				return;
			}
			if (num < 0)
			{
				Interlocked.CompareExchange(ref VariantConfiguration.updateCommittedAccumulationCounter, 0, num);
			}
		}

		internal static void TestInitialize(string folder, string file)
		{
			VariantConfiguration.TestInitialize(folder, new string[]
			{
				file
			}, file);
		}

		internal static void TestInitialize(string folder, string[] files, string flightingConfigFileName)
		{
			if (!files.Contains(flightingConfigFileName))
			{
				throw new ArgumentException("flightingConfigFileName must be in files");
			}
			VariantConfiguration.TestInitialize(folder, flightingConfigFileName, (string configPath) => files);
		}

		internal static void TestInitialize(string folder)
		{
			VariantConfiguration.TestInitialize(folder, "Flighting.settings.ini", null);
		}

		internal static void TestReset()
		{
			lock (VariantConfiguration.StaticLock)
			{
				VariantConfiguration.instance = null;
				if (VariantConfiguration.changeAccumulator != null)
				{
					VariantConfiguration.changeAccumulator.Dispose();
					VariantConfiguration.changeAccumulator = null;
				}
			}
			VariantConfiguration.getConfigFilesImplementationForTesting = null;
			VariantConfiguration.flightingConfigFileName = null;
			VariantConfiguration.isProcessAllowedToReadFiles = null;
			VariantConfiguration.configPath = null;
			VariantConfiguration.localMachineSnapshot = null;
			VariantConfiguration.invariantNoFlightingSnapshot = null;
			VariantConfiguration.resourcesDataSourceReader = null;
			VariantConfiguration.OverridesChanged = null;
			VariantConfiguration.TestOverride = null;
			VariantConfiguration.UpdateCommitted = null;
			VariantConfiguration.SetOverrides(null);
		}

		internal static VariantConfigurationSnapshotProvider GetProviderForValidation(VariantConfigurationOverride validationOverride)
		{
			if (validationOverride == null)
			{
				throw new ArgumentNullException("validationOverride");
			}
			VariantConfigurationBehavior behavior = VariantConfigurationBehavior.Default;
			ChainedDataSourceReader chainedDataSourceReader = new ChainedDataSourceReader();
			string text;
			if (VariantConfiguration.IsProcessAllowedToReadFiles && VariantConfiguration.TryGetConfigFolder(out text) && !string.IsNullOrEmpty(text))
			{
				chainedDataSourceReader.AddDataSourceReader(new FilesDataSourceReader(text));
			}
			chainedDataSourceReader.AddDataSourceReader(VariantConfiguration.ResourcesDataSourceReader);
			return new VariantConfigurationSnapshotProvider(chainedDataSourceReader, OverrideDataTransformation.Get(validationOverride), new string[]
			{
				validationOverride.FileName
			}, behavior, VariantConfiguration.FlightingConfigFileName, "GuestAccess");
		}

		internal static string[] GetConfigFiles(string folder)
		{
			if (VariantConfiguration.getConfigFilesImplementationForTesting != null)
			{
				return VariantConfiguration.getConfigFilesImplementationForTesting(folder);
			}
			return (from path in Directory.EnumerateFiles(folder, "*.ini")
			select Path.GetFileName(path)).ToArray<string>();
		}

		internal static string GetConfigFolder()
		{
			if (VariantConfiguration.configPath == null)
			{
				string str;
				if (!VariantConfiguration.TryGetInstallFolder(out str))
				{
					throw new InvalidOperationException(string.Format("Registry entry {0}\\{1} does not exist.  VariantConfiguration is unable to locate its configuration files.", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", "MsiInstallPath"));
				}
				VariantConfiguration.configPath = str + "Config";
			}
			return VariantConfiguration.configPath;
		}

		internal static bool TryGetConfigFolder(out string configFolder)
		{
			if (VariantConfiguration.configPath == null)
			{
				string str;
				if (!VariantConfiguration.TryGetInstallFolder(out str))
				{
					configFolder = null;
					return false;
				}
				VariantConfiguration.configPath = str + "Config";
			}
			configFolder = VariantConfiguration.configPath;
			return true;
		}

		private static bool TryGetInstallFolder(out string installFolder)
		{
			bool result;
			try
			{
				object value = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", "MsiInstallPath", null);
				if (value == null)
				{
					installFolder = null;
					result = false;
				}
				else
				{
					installFolder = (string)value;
					result = true;
				}
			}
			catch (Exception)
			{
				installFolder = null;
				result = false;
			}
			return result;
		}

		private static bool TryGetConfigFiles(string folder, out string[] configFiles)
		{
			bool result;
			try
			{
				if (Directory.Exists(folder))
				{
					configFiles = VariantConfiguration.GetConfigFiles(folder);
					result = true;
				}
				else
				{
					configFiles = null;
					result = false;
				}
			}
			catch (Exception)
			{
				configFiles = null;
				result = false;
			}
			return result;
		}

		private static bool MatchCurrentOverrides(VariantConfigurationOverride[] newOverrides)
		{
			VariantConfigurationOverride[] overrides = VariantConfiguration.Overrides;
			if (overrides == null && newOverrides == null)
			{
				return true;
			}
			if (overrides == null || newOverrides == null)
			{
				return false;
			}
			if (overrides.Length != newOverrides.Length)
			{
				return false;
			}
			for (int i = 0; i < overrides.Length; i++)
			{
				if (!overrides[i].Equals(newOverrides[i]))
				{
					return false;
				}
			}
			return true;
		}

		private static void RaiseUpdateCommitted()
		{
			VariantConfiguration.updateCommittedAccumulationTimer.Change(-1, -1);
			Interlocked.Exchange(ref VariantConfiguration.updateCommittedAccumulationCounter, 0);
			EventHandler<UpdateCommittedEventArgs> updateCommitted = VariantConfiguration.UpdateCommitted;
			if (updateCommitted != null)
			{
				updateCommitted(null, new UpdateCommittedEventArgs());
			}
		}

		private static void ReloadDatasources()
		{
			VariantConfiguration.Instance.DataLoader.ReloadIfLoaded();
		}

		private static void ReloadChangedDatasources(IEnumerable<string> args)
		{
			IEnumerable<string> enumerable = from path in args
			select Path.GetFileName(path);
			if (enumerable.Any((string file) => file.EndsWith(".flight.ini", StringComparison.OrdinalIgnoreCase)))
			{
				VariantConfiguration.ReloadDatasources();
				return;
			}
			IEnumerable<string> dataSources = enumerable.Intersect(VariantConfiguration.Instance.DataSourceNames, StringComparer.OrdinalIgnoreCase);
			VariantConfiguration.Instance.DataLoader.ReloadIfLoaded(dataSources);
		}

		private static void TestInitialize(string configPath, string flightingConfigFileName, Func<string, string[]> getConfigFilesImplementation)
		{
			lock (VariantConfiguration.StaticLock)
			{
				VariantConfiguration.TestReset();
				VariantConfiguration.isProcessAllowedToReadFiles = new bool?(true);
				VariantConfiguration.configPath = configPath;
				VariantConfiguration.flightingConfigFileName = flightingConfigFileName;
				VariantConfiguration.getConfigFilesImplementationForTesting = getConfigFilesImplementation;
			}
		}

		public const string GlobalSnapshotId = "Global";

		internal const int UpdateCommittedAccumlationMs = 1000;

		internal const string DefaultFlightingConfigFileName = "Flighting.settings.ini";

		internal const string DefaultOverrideAllowedTeamName = "GuestAccess";

		private const string SetupInstallKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup";

		private const string InstallPathName = "MsiInstallPath";

		private const string ConfigRelativePath = "Config";

		private const string ConfigWildcard = "*.ini";

		private const string SettingsFileSuffix = ".settings.ini";

		private const string FlightFileSuffix = ".flight.ini";

		private const int DefaultAccumulationTimeoutInMs = 1000;

		private static readonly object StaticLock = new object();

		private static Func<string, string[]> getConfigFilesImplementationForTesting = null;

		private static string flightingConfigFileName = null;

		private static VariantConfigurationSnapshotProvider instance = null;

		private static FileChangesAccumulator changeAccumulator = null;

		private static string configPath = null;

		private static VariantConfigurationSnapshot invariantNoFlightingSnapshot;

		private static ResourcesDataSourceReader resourcesDataSourceReader;

		private static bool? isProcessAllowedToReadFiles = null;

		private static VariantConfigurationSettings settings = new VariantConfigurationSettings();

		private static VariantConfigurationFlights flights = new VariantConfigurationFlights();

		private static VariantConfigurationSnapshot localMachineSnapshot = null;

		private static Timer updateCommittedAccumulationTimer = new Timer(delegate(object state)
		{
			VariantConfiguration.RaiseUpdateCommitted();
		});

		private static int updateCommittedAccumulationCounter = 0;
	}
}

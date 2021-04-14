using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.EseRepl
{
	public class DagNetEnvironment
	{
		private DagNetEnvironment()
		{
		}

		public static DagNetEnvironment Instance
		{
			get
			{
				return DagNetEnvironment.instance;
			}
		}

		public static DagNetChooser NetChooser
		{
			get
			{
				if (DagNetEnvironment.Instance.currentChooser == null)
				{
					lock (DagNetEnvironment.Instance)
					{
						if (DagNetEnvironment.Instance.currentChooser == null)
						{
							DagNetEnvironment.Instance.currentChooser = new DagNetChooser();
						}
					}
				}
				return DagNetEnvironment.Instance.currentChooser;
			}
		}

		public static int ConnectTimeoutInSec
		{
			get
			{
				int num;
				if (DagNetEnvironment.TryRegistryRead<int>("ConnectTimeoutInSec", 15, out num) == null)
				{
					DagNetEnvironment.Instance.connectTimeoutInSec = num;
				}
				return DagNetEnvironment.Instance.connectTimeoutInSec;
			}
		}

		private static ITracer Tracer
		{
			get
			{
				return Dependencies.DagNetEnvironmentTracer;
			}
		}

		public static void Initialize()
		{
			DagNetEnvironment.instance = new DagNetEnvironment();
		}

		public static Exception PublishDagNetConfig(DagNetConfig cfg)
		{
			Exception result;
			try
			{
				string persistString = cfg.Serialize();
				result = DagNetEnvironment.PublishDagNetConfig(persistString);
			}
			catch (SerializationException ex)
			{
				result = ex;
			}
			return result;
		}

		public static Exception PublishDagNetConfig(string persistString)
		{
			Exception ex = RegistryUtil.RunRegistryFunction(delegate()
			{
				IRegistryWriter registryWriter = Dependencies.RegistryWriter;
				registryWriter.CreateSubKey(Registry.LocalMachine, DagNetEnvironment.RegistryRootKeyName);
				registryWriter.SetValue(Registry.LocalMachine, DagNetEnvironment.RegistryRootKeyName, "DagNetConfig", persistString);
			});
			if (ex != null)
			{
				DagNetEnvironment.Tracer.TraceError(0L, "PublishDagNetConfig fails: {0}", new object[]
				{
					ex
				});
			}
			return ex;
		}

		public static DagNetConfig FetchNetConfig()
		{
			return DagNetEnvironment.Instance.FetchConfigInternal();
		}

		public static DagNetConfig FetchLastKnownNetConfig()
		{
			DagNetConfig dagNetConfig = DagNetEnvironment.Instance.currentNetConfig;
			if (dagNetConfig == null)
			{
				dagNetConfig = DagNetEnvironment.Instance.FetchConfigInternal();
			}
			return dagNetConfig;
		}

		public static int CircularIncrement(int input, int limit)
		{
			if (input < limit - 1)
			{
				return input + 1;
			}
			return 0;
		}

		internal static Exception TryRegistryRead<T>(string valueName, T defaultVal, out T returnedVal)
		{
			returnedVal = defaultVal;
			IRegistryReader reader = Dependencies.RegistryReader;
			T readVal = defaultVal;
			Exception ex = RegistryUtil.RunRegistryFunction(delegate()
			{
				readVal = reader.GetValue<T>(Registry.LocalMachine, DagNetEnvironment.RegistryRootKeyName, valueName, defaultVal);
			});
			if (ex != null)
			{
				DagNetEnvironment.Tracer.TraceError(0L, "TryRegistryRead({0}) fails: {1}", new object[]
				{
					valueName,
					ex
				});
			}
			else
			{
				returnedVal = readVal;
			}
			return ex;
		}

		private static DagNetConfig TryDeserializeConfig(string serializedConfig, out Exception ex)
		{
			DagNetConfig result = null;
			ex = null;
			try
			{
				result = DagNetConfig.Deserialize(serializedConfig);
			}
			catch (SerializationException ex2)
			{
				ex = ex2;
				DagNetEnvironment.Tracer.TraceError(0L, "TryDeserializeConfig failed {0}", new object[]
				{
					ex
				});
			}
			return result;
		}

		private DagNetConfig FetchConfigInternal()
		{
			DagNetConfig result;
			lock (this.dagNetConfigLock)
			{
				string text;
				if (DagNetEnvironment.TryRegistryRead<string>("DagNetConfig", null, out text) == null && !string.IsNullOrEmpty(text))
				{
					int hashCode = text.GetHashCode();
					if (this.serializedNetConfig == null || this.serializedNetConfigHashCode != hashCode)
					{
						DagNetEnvironment.Tracer.TraceDebug((long)this.GetHashCode(), "NetConfig change detected.", new object[0]);
						Exception ex;
						DagNetConfig dagNetConfig = DagNetEnvironment.TryDeserializeConfig(text, out ex);
						if (dagNetConfig != null)
						{
							this.currentNetConfig = dagNetConfig;
						}
						this.serializedNetConfig = text;
						this.serializedNetConfigHashCode = hashCode;
					}
				}
				if (this.currentNetConfig == null)
				{
					this.currentNetConfig = new DagNetConfig();
				}
				result = this.currentNetConfig;
			}
			return result;
		}

		public const string DagNetConfigRegistryValueName = "DagNetConfig";

		public const string DagNetTimeoutRegistryValue = "ConnectTimeoutInSec";

		public const int DefaultConnectTimeoutInSec = 15;

		public static readonly string RegistryRootKeyName = Dependencies.Config.RegistryRootKeyName;

		private static DagNetEnvironment instance = new DagNetEnvironment();

		private DagNetChooser currentChooser;

		private int connectTimeoutInSec = 15;

		private DagNetConfig currentNetConfig;

		private string serializedNetConfig;

		private int serializedNetConfigHashCode;

		private object dagNetConfigLock = new object();
	}
}

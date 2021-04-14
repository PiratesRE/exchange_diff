using System;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.EseRepl
{
	internal class Parameters
	{
		public static Parameters CurrentValues
		{
			get
			{
				if (Parameters.instance == null)
				{
					Parameters parameters = new Parameters();
					parameters.ReadFromRegistry();
					Parameters.instance = parameters;
				}
				return Parameters.instance;
			}
		}

		public Parameters()
		{
			this.RegistryRootKeyName = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters";
			this.DefaultEventSuppressionInterval = TimeSpan.FromSeconds(900.0);
			this.DisableSocketStream = false;
			this.LogShipCompressionDisable = false;
			this.DisableNetworkSigning = false;
			this.LogDiagnosticNetworkEvents = false;
			this.LogCopyNetworkTransferSize = 16777216;
			this.SeedingNetworkTransferSize = 16777216;
		}

		private bool IntToBool(int i)
		{
			return i != 0;
		}

		private void ReadFromRegistry()
		{
			ITracer dagNetEnvironmentTracer = Dependencies.DagNetEnvironmentTracer;
			IRegistryReader reader = Dependencies.RegistryReader;
			Exception ex = RegistryUtil.RunRegistryFunction(delegate()
			{
				int value = reader.GetValue<int>(Registry.LocalMachine, this.RegistryRootKeyName, "DisableSocketStream", 0);
				this.DisableSocketStream = this.IntToBool(value);
				value = reader.GetValue<int>(Registry.LocalMachine, this.RegistryRootKeyName, "LogShipCompressionDisable", 0);
				this.LogShipCompressionDisable = this.IntToBool(value);
				value = reader.GetValue<int>(Registry.LocalMachine, this.RegistryRootKeyName, "DisableNetworkSigning", 0);
				this.DisableNetworkSigning = this.IntToBool(value);
				value = reader.GetValue<int>(Registry.LocalMachine, this.RegistryRootKeyName, "LogDiagnosticNetworkEvents", 0);
				this.LogDiagnosticNetworkEvents = this.IntToBool(value);
				value = reader.GetValue<int>(Registry.LocalMachine, this.RegistryRootKeyName, "LogCopyNetworkTransferSize", this.LogCopyNetworkTransferSize);
				this.LogCopyNetworkTransferSize = value;
				value = reader.GetValue<int>(Registry.LocalMachine, this.RegistryRootKeyName, "SeedingNetworkTransferSize", this.SeedingNetworkTransferSize);
				this.SeedingNetworkTransferSize = value;
			});
			if (ex != null)
			{
				dagNetEnvironmentTracer.TraceError(0L, "ReadFromRegistry({0}) fails: {1}", new object[]
				{
					this.RegistryRootKeyName,
					ex
				});
			}
		}

		public string RegistryRootKeyName { get; set; }

		public TimeSpan DefaultEventSuppressionInterval { get; set; }

		public bool DisableSocketStream { get; set; }

		public bool LogShipCompressionDisable { get; set; }

		public bool DisableNetworkSigning { get; set; }

		public int LogCopyNetworkTransferSize { get; set; }

		public int SeedingNetworkTransferSize { get; set; }

		public bool LogDiagnosticNetworkEvents { get; set; }

		public const int LogFileSize = 1048576;

		internal const string DefaultRegistryRootKeyName = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters";

		private static Parameters instance;
	}
}

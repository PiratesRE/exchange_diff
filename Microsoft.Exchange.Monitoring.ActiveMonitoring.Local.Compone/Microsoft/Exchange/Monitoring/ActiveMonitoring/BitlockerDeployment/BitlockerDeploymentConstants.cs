using System;
using System.IO;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.BitlockerDeployment
{
	internal static class BitlockerDeploymentConstants
	{
		public static bool DisableResponders
		{
			get
			{
				int value = BitlockerDeploymentUtility.RegReader.GetValue<int>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\BitlockerDeployment\\Parameters", "BigRedButton", 0);
				return value != 0;
			}
		}

		public static bool WorkflowFve
		{
			get
			{
				string registryValue = BitlockerDeploymentUtility.GetRegistryValue("SOFTWARE\\Microsoft\\ExchangeServer\\BitlockerDeployment", "WorkflowFve");
				int num;
				int.TryParse(registryValue, out num);
				return 0 != num;
			}
		}

		public const string ParameterRegKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\BitlockerDeployment\\Parameters";

		public const string StateRegKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\BitlockerDeployment\\States";

		public const string BitlockerDeploymentRegKey = "SOFTWARE\\Microsoft\\ExchangeServer\\BitlockerDeployment";

		private const string ResponderDisableMasterSwitch = "BigRedButton";

		private const string WorkflowFveSwitch = "WorkflowFve";

		public const string BitlockerUnlockExecutable = "BitlockerDataVolumeUnlocker.exe";

		public const string ServiceName = "BitlockerDeployment";

		public const string EncryptionStatusProbeName = "EncryptionStatusProbe";

		public const string BootVolumeEncryptionStatusProbeName = "BootVolumeEncryptionStatusProbe";

		public const string EncryptionStatusMonitorName = "BitlockerDeploymentStateMonitor";

		public const string BootVolumeEncryptionStatusMonitorName = "BitlockerDeploymentBootVolumeStateMonitor";

		public const string EncryptionStatusResponderName = "BitlockerDeploymentStateResponder";

		public const string EncryptionSuspendedProbeName = "EncryptionSuspendedProbe";

		public const string EncryptionSuspendedMonitorName = "BitlockerDeploymentSuspendMonitor";

		public const string EncryptionSuspendedResponderName = "BitlockerDeploymentSuspendResponder";

		public const string LockStatusProbeName = "LockStatusProbe";

		public const string LockStatusMonitorName = "BitlockerDeploymentLockMonitor";

		public const string LockStatusResponderName = "BitlockerDeploymentLockResponder";

		public const string DraProtectorProbeName = "DraProtectorProbe";

		public const string DraDecryptorProbeName = "DraDecryptorProbe";

		public const string DraProtectorMonitorName = "DraProtectorMonitor";

		public const string DraDecryptorMonitorName = "DraDecryptorMonitor";

		public const string MasterPassword = "563563-218372-416746-433752-541937-608069-594110-446754";

		public const int LockResponderRecurrenceInterval = 1200;

		public const int LockMonitorEscalateInterval = 1260;

		public const int SuspendResponderRecurrenceInterval = 14400;

		public const int HourlyRunningMonitorEscalateInterval = 18000;

		public const string EscalationTeam = "High Availability";

		public static string LogPath = Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), "BitlockerActiveMonitoringLogs");

		public static string DatacenterFolderPath = Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), "Program Files\\Microsoft\\Exchange Server\\V15\\Datacenter");

		public enum BitlockerEncryptionStates
		{
			FullyDecrypted,
			FullyEncrypted,
			EncryptionInProgress,
			DecryptionInProgress,
			EncryptionPaused,
			DecryptionPaused
		}

		public enum BitlokerEncryptionLockStates
		{
			Unlocked,
			Locked
		}

		public enum BitlokerEncryptionProtectionStates
		{
			Unprotected,
			Protected,
			Unknown
		}

		public enum BitlockerKeyProtectorType
		{
			Unknown,
			Tpm,
			ExternalKey,
			NumericalPassword,
			TpmAndPin,
			TpmAndStartupKey,
			TpmAndPinAndStartupKey,
			PublicKey,
			Passphrase,
			TpmCertificate,
			CryptoApiNextGen
		}

		internal enum BitlockerCertificateType
		{
			Dra = 1,
			NonDra
		}
	}
}

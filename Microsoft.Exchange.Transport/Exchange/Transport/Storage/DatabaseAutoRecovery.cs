using System;
using System.Globalization;
using System.IO;
using System.Security;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Win32;

namespace Microsoft.Exchange.Transport.Storage
{
	internal sealed class DatabaseAutoRecovery
	{
		public DatabaseAutoRecovery(DatabaseRecoveryAction databaseRecoveryAction, string registryKey, string databasePath, string logFilePath, string instanceName, string internalDatabaseName, int permanentErrorCountThreshold, IDatabaseAutoRecoveryEventLogger eventLogger = null)
		{
			if (string.IsNullOrEmpty(registryKey))
			{
				throw new ArgumentNullException("registryKey");
			}
			if (string.IsNullOrEmpty(databasePath))
			{
				throw new ArgumentNullException("databasePath");
			}
			if (string.IsNullOrEmpty(logFilePath))
			{
				throw new ArgumentNullException("logFilePath");
			}
			if (string.IsNullOrEmpty(instanceName))
			{
				throw new ArgumentNullException("instanceName");
			}
			if (string.IsNullOrEmpty(internalDatabaseName))
			{
				throw new ArgumentNullException("internalDatabaseName");
			}
			if (databaseRecoveryAction != DatabaseRecoveryAction.None && databaseRecoveryAction != DatabaseRecoveryAction.Move && databaseRecoveryAction != DatabaseRecoveryAction.Delete)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value for Database Auto Recovery Action: [{0}]", new object[]
				{
					databaseRecoveryAction
				}), "databaseRecoveryAction");
			}
			this.databaseRecoveryAction = databaseRecoveryAction;
			this.registryKey = registryKey;
			this.databasePath = databasePath;
			this.logFilePath = logFilePath;
			this.instanceName = instanceName;
			this.eventLogger = eventLogger;
			this.internalDatabaseName = internalDatabaseName;
			this.permanentErrorCountThreshold = permanentErrorCountThreshold;
			if (this.eventLogger == null)
			{
				this.eventLogger = new DatabaseAutoRecoveryEventLogger();
			}
		}

		public override int GetHashCode()
		{
			return this.databasePath.GetHashCode() ^ this.databaseRecoveryAction.GetHashCode() ^ this.eventLogger.GetHashCode() ^ this.instanceName.GetHashCode() ^ this.internalDatabaseName.GetHashCode() ^ this.logFilePath.GetHashCode() ^ this.registryKey.GetHashCode();
		}

		public void PerformDatabaseAutoRecoveryIfNeccessary()
		{
			if (!this.IsDatabaseCorrupted())
			{
				return;
			}
			switch (this.databaseRecoveryAction)
			{
			case DatabaseRecoveryAction.None:
				this.eventLogger.LogDatabaseRecoveryActionNone(this.instanceName, this.databasePath, this.logFilePath);
				break;
			case DatabaseRecoveryAction.Move:
				this.MoveDatabaseDirectory();
				break;
			case DatabaseRecoveryAction.Delete:
				this.DeleteDatabaseDirectory();
				break;
			default:
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unsupported Data Recovery Action: {0}", new object[]
				{
					this.databaseRecoveryAction
				}));
			}
			this.ResetDatabaseCorruptionFlag();
		}

		public bool SetDatabaseCorruptionFlag()
		{
			return this.SetDatabaseCorruptionFlag(this.permanentErrorCountThreshold);
		}

		public bool ResetDatabaseCorruptionFlag()
		{
			return this.SetDatabaseCorruptionFlag(0);
		}

		public int GetDatabaseCorruptionCount()
		{
			return this.GetRegistyValue();
		}

		public bool IncrementDatabaseCorruptionCount()
		{
			bool result;
			lock (this)
			{
				result = this.SetDatabaseCorruptionFlag(this.GetRegistyValue() + 1);
			}
			return result;
		}

		private static bool IsMovable(string sourceDirectory, string destDirectory)
		{
			if (!Directory.Exists(sourceDirectory))
			{
				return true;
			}
			if (!Directory.Exists(destDirectory))
			{
				return true;
			}
			string fullPath = Path.GetFullPath(destDirectory);
			string[] files = Directory.GetFiles(sourceDirectory, "*", SearchOption.TopDirectoryOnly);
			string[] array = files;
			int i = 0;
			while (i < array.Length)
			{
				string fileName = array[i];
				FileInfo fileInfo = new FileInfo(fileName);
				bool result;
				if (fileInfo.FullName.Equals(fullPath, StringComparison.OrdinalIgnoreCase))
				{
					result = false;
				}
				else
				{
					if (!File.Exists(Path.Combine(destDirectory, fileInfo.Name)))
					{
						i++;
						continue;
					}
					result = false;
				}
				return result;
			}
			string[] directories = Directory.GetDirectories(sourceDirectory, "*", SearchOption.TopDirectoryOnly);
			foreach (string path in directories)
			{
				if (!fullPath.Equals(Path.GetFullPath(path), StringComparison.OrdinalIgnoreCase))
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(path);
					if (Directory.Exists(Path.Combine(destDirectory, directoryInfo.Name)))
					{
						return false;
					}
				}
			}
			return true;
		}

		private static void MoveDirectory(string sourceDirectory, string destDirectory)
		{
			if (!Directory.Exists(sourceDirectory))
			{
				return;
			}
			if (!Directory.Exists(destDirectory))
			{
				Directory.CreateDirectory(destDirectory);
			}
			string[] files = Directory.GetFiles(sourceDirectory, "*", SearchOption.TopDirectoryOnly);
			foreach (string fileName in files)
			{
				FileInfo fileInfo = new FileInfo(fileName);
				fileInfo.MoveTo(Path.Combine(destDirectory, fileInfo.Name));
			}
			string fullPath = Path.GetFullPath(destDirectory);
			string[] directories = Directory.GetDirectories(sourceDirectory, "*", SearchOption.TopDirectoryOnly);
			foreach (string path in directories)
			{
				if (!fullPath.Equals(Path.GetFullPath(path), StringComparison.OrdinalIgnoreCase))
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(path);
					directoryInfo.MoveTo(Path.Combine(destDirectory, directoryInfo.Name));
				}
			}
		}

		private static void DeleteDirectory(string directoryToDelete)
		{
			if (!Directory.Exists(directoryToDelete))
			{
				return;
			}
			string[] files = Directory.GetFiles(directoryToDelete, "*", SearchOption.TopDirectoryOnly);
			foreach (string path in files)
			{
				File.Delete(path);
			}
			string[] directories = Directory.GetDirectories(directoryToDelete, "*", SearchOption.TopDirectoryOnly);
			foreach (string path2 in directories)
			{
				Directory.Delete(path2);
			}
		}

		private bool SetDatabaseCorruptionFlag(int value)
		{
			if (value >= this.permanentErrorCountThreshold)
			{
				this.eventLogger.DataBaseCorruptionDetected(this.instanceName, "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\" + this.registryKey);
			}
			return this.TrySetRegistryValue(value);
		}

		private bool MoveDatabaseDirectory()
		{
			string failureReason = string.Empty;
			try
			{
				string str = string.Format(".old-{0:yyyyMMddHHmmss}", DateTime.UtcNow);
				string text = Path.Combine(this.databasePath, this.internalDatabaseName + str);
				string text2 = Path.Combine(this.logFilePath, this.internalDatabaseName + str);
				if (!DatabaseAutoRecovery.IsMovable(this.databasePath, text))
				{
					this.eventLogger.LogDatabaseRecoveryActionFailed(this.instanceName, DatabaseRecoveryAction.Move, Strings.DatabaseIsNotMovable(this.databasePath, text));
					return false;
				}
				if (!Path.GetFullPath(this.logFilePath).Equals(Path.GetFullPath(this.databasePath), StringComparison.OrdinalIgnoreCase))
				{
					if (!DatabaseAutoRecovery.IsMovable(this.logFilePath, text2))
					{
						this.eventLogger.LogDatabaseRecoveryActionFailed(this.instanceName, DatabaseRecoveryAction.Move, Strings.DatabaseIsNotMovable(this.logFilePath, text2));
						return false;
					}
					DatabaseAutoRecovery.MoveDirectory(this.databasePath, text);
					DatabaseAutoRecovery.MoveDirectory(this.logFilePath, text2);
					this.eventLogger.LogDatabaseRecoveryActionMove(this.instanceName, this.databasePath, text, this.logFilePath, text2);
				}
				else
				{
					DatabaseAutoRecovery.MoveDirectory(this.databasePath, text);
					this.eventLogger.LogDatabaseRecoveryActionMove(this.instanceName, this.databasePath, text);
				}
				return true;
			}
			catch (UnauthorizedAccessException ex)
			{
				failureReason = ex.Message;
			}
			catch (ArgumentException ex2)
			{
				failureReason = ex2.Message;
			}
			catch (PathTooLongException ex3)
			{
				failureReason = ex3.Message;
			}
			catch (DirectoryNotFoundException ex4)
			{
				failureReason = ex4.Message;
			}
			catch (IOException ex5)
			{
				failureReason = ex5.Message;
			}
			this.eventLogger.LogDatabaseRecoveryActionFailed(this.instanceName, DatabaseRecoveryAction.Move, failureReason);
			return false;
		}

		private bool DeleteDatabaseDirectory()
		{
			string failureReason = string.Empty;
			try
			{
				DatabaseAutoRecovery.DeleteDirectory(this.databasePath);
				if (!Path.GetFullPath(this.logFilePath).Equals(Path.GetFullPath(this.databasePath), StringComparison.OrdinalIgnoreCase))
				{
					DatabaseAutoRecovery.DeleteDirectory(this.logFilePath);
					this.eventLogger.LogDatabaseRecoveryActionDelete(this.instanceName, this.databasePath, this.logFilePath);
				}
				else
				{
					this.eventLogger.LogDatabaseRecoveryActionDelete(this.instanceName, this.databasePath);
				}
				return true;
			}
			catch (UnauthorizedAccessException ex)
			{
				failureReason = ex.Message;
			}
			catch (ArgumentException ex2)
			{
				failureReason = ex2.Message;
			}
			catch (PathTooLongException ex3)
			{
				failureReason = ex3.Message;
			}
			catch (DirectoryNotFoundException ex4)
			{
				failureReason = ex4.Message;
			}
			catch (IOException ex5)
			{
				failureReason = ex5.Message;
			}
			this.eventLogger.LogDatabaseRecoveryActionFailed(this.instanceName, DatabaseRecoveryAction.Delete, failureReason);
			return false;
		}

		private bool TrySetRegistryValue(int value)
		{
			try
			{
				Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\", this.registryKey, value, RegistryValueKind.DWord);
				return true;
			}
			catch (UnauthorizedAccessException ex)
			{
				this.eventLogger.DatabaseRecoveryActionFailedRegistryAccessDenied(this.instanceName, "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\" + this.registryKey, ex.Message);
			}
			catch (SecurityException ex2)
			{
				this.eventLogger.DatabaseRecoveryActionFailedRegistryAccessDenied(this.instanceName, "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\" + this.registryKey, ex2.Message);
			}
			return false;
		}

		private int GetRegistyValue()
		{
			object obj = null;
			try
			{
				obj = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\", this.registryKey, 0);
			}
			catch (IOException ex)
			{
				ExTraceGlobals.ExpoTracer.TraceError((long)this.GetHashCode(), ex.Message);
			}
			catch (SecurityException ex2)
			{
				this.eventLogger.DatabaseRecoveryActionFailedRegistryAccessDenied(this.instanceName, "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\" + this.registryKey, ex2.Message);
			}
			if (!(obj is int))
			{
				return 0;
			}
			return (int)obj;
		}

		private bool IsDatabaseCorrupted()
		{
			return this.GetRegistyValue() >= this.permanentErrorCountThreshold;
		}

		private const string TransportRegistryPath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\";

		private readonly DatabaseRecoveryAction databaseRecoveryAction;

		private readonly string registryKey;

		private readonly string databasePath;

		private readonly string logFilePath;

		private readonly string instanceName;

		private readonly string internalDatabaseName;

		private readonly int permanentErrorCountThreshold;

		private IDatabaseAutoRecoveryEventLogger eventLogger;
	}
}

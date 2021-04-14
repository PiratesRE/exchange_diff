using System;
using System.IO;
using System.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class RegistryStateIO : IStateIO
	{
		public RegistryStateIO(string machineName, string identity, bool fLocks)
		{
			this.m_machineName = machineName;
			this.m_identity = identity;
			this.m_fStateLock = fLocks;
			if (fLocks)
			{
				this.m_registryKey = string.Format("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\StateLock\\{0}", identity);
			}
			else
			{
				this.m_registryKey = string.Format("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\State\\{0}", identity);
			}
			this.m_key = RegistryStateIO.OpenRemoteKey(machineName, this.m_registryKey);
		}

		private static RegistryKey OpenRemoteKey(string machineName, string registryKey)
		{
			RegistryKey result = null;
			if (!string.IsNullOrEmpty(machineName))
			{
				using (RemoteRegistryBaseKey remoteRegistryBaseKey = new RemoteRegistryBaseKey())
				{
					remoteRegistryBaseKey.Open(RegistryHive.LocalMachine, machineName);
					try
					{
						result = remoteRegistryBaseKey.Key.CreateSubKey(registryKey);
						RegistryStateIO.trace.TraceDebug<string, string>(0L, "remote RegistryStateIO created, key = {0}, machine = {1}", registryKey, machineName);
					}
					catch (SecurityException)
					{
						result = remoteRegistryBaseKey.Key.OpenSubKey(registryKey, RegistryKeyPermissionCheck.ReadSubTree);
						RegistryStateIO.trace.TraceDebug<string, string>(0L, "remote RegistryStateIO opened read subtree, key = {0}, machine = {1}", registryKey, machineName);
					}
					catch (UnauthorizedAccessException)
					{
						result = remoteRegistryBaseKey.Key.OpenSubKey(registryKey, RegistryKeyPermissionCheck.ReadSubTree);
						RegistryStateIO.trace.TraceDebug<string, string>(0L, "remote RegistryStateIO opened read subtree, key = {0}, machine = {1}", registryKey, machineName);
					}
					return result;
				}
			}
			try
			{
				result = Registry.LocalMachine.CreateSubKey(registryKey);
				RegistryStateIO.trace.TraceDebug<string, string>(0L, "local RegistryStateIO created, key = {0}, machine = {1}", registryKey, machineName);
			}
			catch (SecurityException)
			{
				result = Registry.LocalMachine.OpenSubKey(registryKey, RegistryKeyPermissionCheck.ReadSubTree);
				RegistryStateIO.trace.TraceDebug<string, string>(0L, "local RegistryStateIO opened read subtree, key = {0}, machine = {1}", registryKey, machineName);
			}
			catch (UnauthorizedAccessException)
			{
				result = Registry.LocalMachine.OpenSubKey(registryKey, RegistryKeyPermissionCheck.ReadSubTree);
				RegistryStateIO.trace.TraceDebug<string, string>(0L, "local RegistryStateIO opened read subtree, key = {0}, machine = {1}", registryKey, machineName);
			}
			return result;
		}

		public override void DeleteState()
		{
			RegistryStateIO.trace.TraceDebug<string>((long)this.GetHashCode(), "DeleteState: {0}", this.m_registryKey);
			if (this.m_fStateLock)
			{
				using (RegistryKey registryKey = RegistryStateIO.OpenRemoteKey(this.m_machineName, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\StateLock"))
				{
					registryKey.DeleteSubKeyTree(this.m_identity);
					return;
				}
			}
			using (RegistryKey registryKey2 = RegistryStateIO.OpenRemoteKey(this.m_machineName, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\State"))
			{
				registryKey2.DeleteSubKeyTree(this.m_identity);
			}
		}

		protected override bool TryReadFromStore(string valueName, string defaultValue, out string value)
		{
			if (!base.UseIOFailure)
			{
				this.TryReadInternal(valueName, defaultValue, out value);
				return true;
			}
			value = defaultValue;
			Exception ex = null;
			try
			{
				this.TryReadInternal(valueName, defaultValue, out value);
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (InvalidSavedStateException ex3)
			{
				ex = ex3;
			}
			catch (SecurityException ex4)
			{
				ex = ex4;
			}
			catch (UnauthorizedAccessException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				base.IOFailure(ex);
				value = defaultValue;
				return false;
			}
			return true;
		}

		private void TryReadInternal(string valueName, string defaultValue, out string value)
		{
			object value2 = this.m_key.GetValue(valueName);
			if (value2 == null)
			{
				value = defaultValue;
				return;
			}
			if (value2 is string)
			{
				value = (string)value2;
				return;
			}
			if (value2 is int)
			{
				value = ((int)value2).ToString();
				return;
			}
			throw new InvalidSavedStateException();
		}

		protected override bool TryReadValueNamesInternal(out string[] valueNames)
		{
			if (base.UseIOFailure)
			{
				try
				{
					valueNames = this.m_key.GetValueNames();
					return true;
				}
				catch (IOException ex)
				{
					base.IOFailure(ex);
					valueNames = new string[0];
					return false;
				}
				catch (InvalidSavedStateException ex2)
				{
					base.IOFailure(ex2);
					valueNames = new string[0];
					return false;
				}
			}
			valueNames = this.m_key.GetValueNames();
			return true;
		}

		protected override bool TryWriteToStore(string valueName, string value, bool fCritical)
		{
			if (!base.UseIOFailure)
			{
				this.m_key.SetValue(valueName, value);
				return true;
			}
			Exception ex = null;
			try
			{
				this.m_key.SetValue(valueName, value);
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (SecurityException ex3)
			{
				ex = ex3;
			}
			catch (UnauthorizedAccessException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				if (fCritical)
				{
					base.IOFailure(ex);
				}
				return false;
			}
			return true;
		}

		internal const string RegistryStateRootKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\State";

		internal const string RegistryStateLockRootKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\StateLock";

		internal const string RegistryKeyFormat = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\State\\{0}";

		private const string RegistryKeyFormatLocks = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\StateLock\\{0}";

		private RegistryKey m_key;

		private string m_registryKey;

		private string m_machineName;

		private string m_identity;

		private bool m_fStateLock;

		private static Trace trace = ExTraceGlobals.StateTracer;
	}
}

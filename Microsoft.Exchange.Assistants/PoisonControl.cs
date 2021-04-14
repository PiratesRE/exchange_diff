using System;
using System.IO;
using System.Security;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Win32;

namespace Microsoft.Exchange.Assistants
{
	internal abstract class PoisonControl : Base
	{
		public PoisonControl(PoisonControlMaster master, DatabaseInfo databaseInfo, string subKeyContainerName)
		{
			this.master = master;
			this.databaseInfo = databaseInfo;
			this.subKeyDatabaseName = databaseInfo.Guid.ToString() + "\\" + subKeyContainerName;
			if (this.master.RegistryKey == null)
			{
				return;
			}
			ExTraceGlobals.PoisonControlTracer.TraceDebug<PoisonControl, string, string>((long)this.GetHashCode(), "{0}: Creating registry key '{1}\\{2}'", this, this.master.RegistryKey.Name, this.subKeyDatabaseName);
			this.registryKeyDatabase = this.master.RegistryKey.CreateSubKey(this.subKeyDatabaseName, RegistryKeyPermissionCheck.ReadWriteSubTree);
			DateTime utcNow = DateTime.UtcNow;
			string[] subKeyNames = this.registryKeyDatabase.GetSubKeyNames();
			foreach (string text in subKeyNames)
			{
				ExTraceGlobals.PoisonControlTracer.TraceDebug<PoisonControl, string, string>((long)this.GetHashCode(), "{0}: Opening registry key '{1}\\{2}'", this, this.registryKeyDatabase.Name, text);
				CrashData crashData = CrashData.Read(this.registryKeyDatabase, text);
				if (crashData == null)
				{
					ExTraceGlobals.PoisonControlTracer.TraceDebug<PoisonControl, string, string>((long)this.GetHashCode(), "{0}: Discarding registry key '{1}\\{2}' because it has no useful data", this, this.registryKeyDatabase.Name, text);
					this.RemoveSubKey(text);
				}
				else if (utcNow - crashData.Time > PoisonControl.MaximumKeyAge)
				{
					ExTraceGlobals.PoisonControlTracer.TraceDebug((long)this.GetHashCode(), "{0}: Discarding registry key '{1}\\{2}' because it is old ({3})", new object[]
					{
						this,
						this.registryKeyDatabase.Name,
						text,
						crashData.Time
					});
					this.RemoveSubKey(text);
				}
				else
				{
					ExTraceGlobals.PoisonControlTracer.TraceDebug<PoisonControl, string, string>((long)this.GetHashCode(), "{0}: Loading registry key '{1}\\{2}'", this, this.registryKeyDatabase.Name, text);
					this.LoadCrashData(text, crashData.Count);
				}
			}
		}

		protected DatabaseInfo DatabaseInfo
		{
			get
			{
				return this.databaseInfo;
			}
		}

		protected PoisonControlMaster Master
		{
			get
			{
				return this.master;
			}
		}

		public void PoisonCall(EmergencyKit kit, TryDelegate dangerousCall)
		{
			PoisonControl.<>c__DisplayClass1 CS$<>8__locals1 = new PoisonControl.<>c__DisplayClass1();
			CS$<>8__locals1.kit = kit;
			CS$<>8__locals1.<>4__this = this;
			ILUtil.DoTryFilterCatch(dangerousCall, new FilterDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<PoisonCall>b__0)), null);
		}

		protected void SaveCrashData(string subKeyName, int crashCount)
		{
			CrashData.Write(this.registryKeyDatabase, subKeyName, crashCount);
		}

		protected abstract void HandleUnhandledException(object exception, EmergencyKit kit);

		protected abstract void LoadCrashData(string subKeyName, int crashCount);

		protected void RemoveDatabaseKey()
		{
			if (this.registryKeyDatabase != null)
			{
				ExTraceGlobals.PoisonControlTracer.TraceDebug<PoisonControl, string>((long)this.GetHashCode(), "{0}: Deleting registry key named '{1}'", this, this.registryKeyDatabase.Name);
				this.TryDeleteRegistrySubKey(this.Master.RegistryKey, this.subKeyDatabaseName);
				this.registryKeyDatabase = this.master.RegistryKey.CreateSubKey(this.subKeyDatabaseName, RegistryKeyPermissionCheck.ReadWriteSubTree);
			}
		}

		private void RemoveSubKey(string subKeyName)
		{
			if (this.registryKeyDatabase != null)
			{
				ExTraceGlobals.PoisonControlTracer.TraceDebug<PoisonControl, string, string>((long)this.GetHashCode(), "{0}: Deleting registry key named '{1}\\{2}'", this, this.registryKeyDatabase.Name, subKeyName);
				this.TryDeleteRegistrySubKey(this.registryKeyDatabase, subKeyName);
			}
		}

		private void TryDeleteRegistrySubKey(RegistryKey registryKey, string subKeyName)
		{
			Exception ex = null;
			try
			{
				registryKey.DeleteSubKeyTree(subKeyName);
			}
			catch (ArgumentNullException ex2)
			{
				ex = ex2;
			}
			catch (ArgumentException ex3)
			{
				ex = ex3;
			}
			catch (IOException ex4)
			{
				ex = ex4;
			}
			catch (SecurityException ex5)
			{
				ex = ex5;
			}
			catch (ObjectDisposedException ex6)
			{
				ex = ex6;
			}
			catch (UnauthorizedAccessException ex7)
			{
				ex = ex7;
			}
			if (ex != null)
			{
				ExTraceGlobals.PoisonControlTracer.TraceError((long)this.GetHashCode(), "{0}: Unable to delete registry key named '{1}\\{2}' due to exception: {3}", new object[]
				{
					this,
					registryKey.Name,
					subKeyName,
					ex
				});
			}
		}

		private static readonly TimeSpan MaximumKeyAge = TimeSpan.FromHours(24.0);

		private DatabaseInfo databaseInfo;

		private RegistryKey registryKeyDatabase;

		private string subKeyDatabaseName;

		private PoisonControlMaster master;
	}
}

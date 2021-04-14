using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Win32;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public abstract class SchemaUpgrader
	{
		public static SchemaUpgrader Null(ComponentVersion from, ComponentVersion to)
		{
			return new SchemaUpgrader.NullUpgrader(from, to);
		}

		public static SchemaUpgrader Null(short fromMajor, ushort fromMinor, short toMajor, ushort toMinor)
		{
			return SchemaUpgrader.Null(new ComponentVersion(fromMajor, fromMinor), new ComponentVersion(toMajor, toMinor));
		}

		public bool IsNullUpgrader
		{
			get
			{
				return this is SchemaUpgrader.NullUpgrader;
			}
		}

		public Hookable<ComponentVersion> FromHook
		{
			get
			{
				return this.from;
			}
		}

		public Hookable<ComponentVersion> ToHook
		{
			get
			{
				return this.to;
			}
		}

		public ComponentVersion From
		{
			get
			{
				return this.from.Value;
			}
		}

		public ComponentVersion To
		{
			get
			{
				return this.to.Value;
			}
		}

		internal long GetQuarantinedFileTime()
		{
			return RegistryReader.Instance.GetValue<long>(Registry.LocalMachine, this.quarantineSubKey, "QuarantinedTime", 0L);
		}

		internal void SetQuarantinedFileTime(long value)
		{
			RegistryWriter.Instance.CreateSubKey(Registry.LocalMachine, this.quarantineSubKey);
			RegistryWriter.Instance.SetValue(Registry.LocalMachine, this.quarantineSubKey, "QuarantinedTime", value, RegistryValueKind.QWord);
		}

		internal int GetCrashCount()
		{
			return RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, this.quarantineSubKey, "CrashCount", 0);
		}

		internal void SetCrashCount(int value)
		{
			RegistryWriter.Instance.CreateSubKey(Registry.LocalMachine, this.quarantineSubKey);
			RegistryWriter.Instance.SetValue(Registry.LocalMachine, this.quarantineSubKey, "CrashCount", value, RegistryValueKind.DWord);
		}

		public bool IsQuarantined
		{
			get
			{
				long quarantinedFileTime = this.GetQuarantinedFileTime();
				DateTime d = DateTime.FromFileTimeUtc(quarantinedFileTime);
				if (DateTime.UtcNow - d > TimeSpan.FromDays(1.0))
				{
					try
					{
						if (quarantinedFileTime != 0L)
						{
							RegistryWriter.Instance.DeleteSubKeyTree(Registry.LocalMachine, this.quarantineSubKey);
						}
					}
					catch (ArgumentException exception)
					{
						NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
					}
					return false;
				}
				return true;
			}
		}

		public abstract void InitInMemoryDatabaseSchema(Context context, StoreDatabase database);

		public abstract void PerformUpgrade(Context context, ISchemaVersion container);

		public virtual void TransactionAborted(Context context, ISchemaVersion container)
		{
		}

		public SchemaUpgrader(ComponentVersion from, ComponentVersion to)
		{
			if (from.Major == to.Major)
			{
				if (to.Minor != from.Minor + 1)
				{
					throw new ArgumentException("target Minor version of the upgrade must be one greater than the source of the upgrade");
				}
			}
			else
			{
				if (to.Major != from.Major + 1)
				{
					throw new ArgumentException("Major schema upgrades must incremental.");
				}
				if (to.Minor != 0)
				{
					throw new ArgumentException("Major schema upgrades must reset the Minor version to zero.");
				}
			}
			this.from = Hookable<ComponentVersion>.Create(true, from);
			this.to = Hookable<ComponentVersion>.Create(true, to);
			this.quarantineSubKey = "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem\\SchemaUpgraderQuarantines\\" + base.GetType().ToString();
		}

		protected bool TestVersionIsReady(Context context, ISchemaVersion container)
		{
			StoreDatabase storeDatabase = container as StoreDatabase;
			if (storeDatabase == null)
			{
				storeDatabase = context.Database;
			}
			return storeDatabase.PhysicalDatabase.DatabaseType != DatabaseType.Jet || container.GetCurrentSchemaVersion(context).Value >= this.To.Value;
		}

		private const string QuarantinedTimeKey = "QuarantinedTime";

		private const string CrashCountKey = "CrashCount";

		private readonly string quarantineSubKey;

		private Hookable<ComponentVersion> from;

		private Hookable<ComponentVersion> to;

		private sealed class NullUpgrader : SchemaUpgrader
		{
			public NullUpgrader(ComponentVersion from, ComponentVersion to) : base(from, to)
			{
			}

			public override void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
			{
			}

			public override void PerformUpgrade(Context context, ISchemaVersion container)
			{
			}
		}
	}
}

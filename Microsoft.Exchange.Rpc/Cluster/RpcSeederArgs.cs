using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Exchange.Rpc.Cluster
{
	internal sealed class RpcSeederArgs
	{
		private void BuildDebugString()
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			stringBuilder.AppendFormat("RpcSeederArgs: [ InstanceGuid='{0}', ", this.m_instanceGuid.ToString());
			bool fDeleteExistingFiles = this.m_fDeleteExistingFiles;
			stringBuilder.AppendFormat("DeleteExistingFiles='{0}', ", fDeleteExistingFiles.ToString());
			bool fAutoSuspend = this.m_fAutoSuspend;
			stringBuilder.AppendFormat("AutoSuspend='{0}', ", fAutoSuspend.ToString());
			stringBuilder.AppendFormat("SeedingPath='{0}', ", this.m_seedingPath);
			stringBuilder.AppendFormat("LogFolderPath='{0}', ", this.m_logFolderPath);
			stringBuilder.AppendFormat("NetworkId='{0}', ", this.m_networkId);
			bool fStreamingBackup = this.m_fStreamingBackup;
			stringBuilder.AppendFormat("StreamingBackup='{0}', ", fStreamingBackup.ToString());
			stringBuilder.AppendFormat("SourceMachineName='{0}', ", this.m_sourceMachineName);
			stringBuilder.AppendFormat("DatabaseName='{0}', ", this.m_databaseName);
			stringBuilder.AppendFormat("ManualResume='{0}', ", this.m_fManualResume);
			string arg;
			if (this.m_fSeedDatabase)
			{
				arg = "1";
			}
			else
			{
				arg = "0";
			}
			stringBuilder.AppendFormat("SeedDatabase='{0}', ", arg);
			string arg2;
			if (this.m_fSeedCiFiles)
			{
				arg2 = "1";
			}
			else
			{
				arg2 = "0";
			}
			stringBuilder.AppendFormat("SeedCiFiles='{0}', ", arg2);
			stringBuilder.AppendFormat("MaxSeedsInParallel='{0}', ", this.m_maxSeedsInParallel);
			stringBuilder.AppendFormat("SafeDeleteExistingFiles='{0}', ", this.m_fSafeDeleteExistingFiles);
			stringBuilder.AppendFormat("Flags='{0}', ", this.m_flags);
			stringBuilder.AppendFormat("CompressOverride='{0}'='{1}', ", this.m_compressOverride, <Module>.NullableBoolToString(ref this.m_compressOverride));
			stringBuilder.AppendFormat("EncryptOverride='{0}='{1}' ", this.m_encryptOverride, <Module>.NullableBoolToString(ref this.m_encryptOverride));
			stringBuilder.Append("]");
			this.m_debugString = stringBuilder.ToString();
		}

		public RpcSeederArgs(Guid instanceGuid, [MarshalAs(UnmanagedType.U1)] bool fDeleteExistingFiles, [MarshalAs(UnmanagedType.U1)] bool fAutoSuspend, string seedingPath, string logFolderPath, string networkId, [MarshalAs(UnmanagedType.U1)] bool fStreamingBackup, string sourceMachinename, string databaseName, [MarshalAs(UnmanagedType.U1)] bool fManualResume, [MarshalAs(UnmanagedType.U1)] bool fSeedDatabase, [MarshalAs(UnmanagedType.U1)] bool fSeedCiFiles, bool? compressOverride, bool? encryptOverride, int maxSeedsInParallel, [MarshalAs(UnmanagedType.U1)] bool fsafeDeleteExistingFiles, SeederRpcFlags flags)
		{
			this.BuildDebugString();
		}

		public sealed override string ToString()
		{
			return this.m_debugString;
		}

		public Guid InstanceGuid
		{
			get
			{
				return this.m_instanceGuid;
			}
		}

		public bool DeleteExistingFiles
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_fDeleteExistingFiles;
			}
		}

		public bool AutoSuspend
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_fAutoSuspend;
			}
		}

		public string SeedingPath
		{
			get
			{
				return this.m_seedingPath;
			}
		}

		public string LogFolderPath
		{
			get
			{
				return this.m_logFolderPath;
			}
		}

		public string NetworkId
		{
			get
			{
				return this.m_networkId;
			}
		}

		public bool StreamingBackup
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_fStreamingBackup;
			}
		}

		public string SourceMachineName
		{
			get
			{
				return this.m_sourceMachineName;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.m_databaseName;
			}
		}

		public bool ManualResume
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_fManualResume;
			}
		}

		public bool SeedDatabase
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_fSeedDatabase;
			}
		}

		public bool SeedCiFiles
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_fSeedCiFiles;
			}
		}

		public bool? CompressOverride
		{
			get
			{
				return this.m_compressOverride;
			}
		}

		public bool? EncryptOverride
		{
			get
			{
				return this.m_encryptOverride;
			}
		}

		public int MaxSeedsInParallel
		{
			get
			{
				return this.m_maxSeedsInParallel;
			}
		}

		public bool SafeDeleteExistingFiles
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_fSafeDeleteExistingFiles;
			}
		}

		public SeederRpcFlags Flags
		{
			get
			{
				return this.m_flags;
			}
		}

		private Guid m_instanceGuid = instanceGuid;

		private bool m_fDeleteExistingFiles = fDeleteExistingFiles;

		private bool m_fAutoSuspend = fAutoSuspend;

		private string m_seedingPath = seedingPath;

		private string m_logFolderPath = logFolderPath;

		private string m_networkId = networkId;

		private bool m_fStreamingBackup = fStreamingBackup;

		private string m_sourceMachineName = sourceMachinename;

		private string m_databaseName = databaseName;

		private bool m_fManualResume = fManualResume;

		private bool m_fSeedDatabase = fSeedDatabase;

		private bool m_fSeedCiFiles = fSeedCiFiles;

		private bool? m_compressOverride = compressOverride;

		private bool? m_encryptOverride = encryptOverride;

		private string m_debugString;

		private int m_maxSeedsInParallel = maxSeedsInParallel;

		private bool m_fSafeDeleteExistingFiles = fsafeDeleteExistingFiles;

		private SeederRpcFlags m_flags = flags;
	}
}

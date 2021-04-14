using System;
using System.IO;
using System.Threading;

namespace Microsoft.Exchange.ProcessManager
{
	internal class ComWorkerConfiguration
	{
		public ComWorkerConfiguration(string processPath, string cmdlineParams, Guid workerGuid, ComWorkerConfiguration.RunAsFlag flags, Mutex processLaunchMutex, int memoryLimit, int lifeTimeLimit, int workerAllocationTimeout, int maxTransactions, int transactionTimeout, int idleTimeout)
		{
			if (!File.Exists(processPath))
			{
				throw new ArgumentException("Can not find worker process path", "processPath");
			}
			if (processLaunchMutex != null && processLaunchMutex.SafeWaitHandle.IsInvalid)
			{
				throw new ArgumentException("Invalid Mutex for COM object activation", "processLaunchMutex");
			}
			if (memoryLimit < 0)
			{
				throw new ArgumentException("Invalid max worker process memory limit", "memoryLimit");
			}
			if (lifeTimeLimit < 0)
			{
				throw new ArgumentException("Invalid max worker process memory limit", "memoryLimit");
			}
			if (workerAllocationTimeout <= 0)
			{
				throw new ArgumentException("Invalid request allocation timeout", "workerAllocationTimeout");
			}
			if (maxTransactions <= 0)
			{
				throw new ArgumentException("Invalid max number of transactions per process", "maxTransactions");
			}
			if (transactionTimeout <= 0)
			{
				throw new ArgumentException("Invalid transaction timeout", "transactionTimeout");
			}
			if (idleTimeout < 0)
			{
				throw new ArgumentException("Invalid value for idle timeout", "idleTimeout");
			}
			this.workerMemoryLimit = memoryLimit;
			this.workerAllocationTimeout = workerAllocationTimeout;
			this.workerLifetimeLimit = lifeTimeLimit;
			this.maxTransactionsPerProcess = maxTransactions;
			this.transactionTimeout = transactionTimeout;
			this.workerProcessPath = processPath;
			this.cmdlineParams = cmdlineParams;
			this.flags = flags;
			this.processLaunchMutex = processLaunchMutex;
			this.workerGuid = workerGuid;
			this.workerIdleTimeout = idleTimeout;
		}

		public int WorkerMemoryLimit
		{
			get
			{
				return this.workerMemoryLimit;
			}
		}

		public int WorkerAllocationTimeout
		{
			get
			{
				return this.workerAllocationTimeout;
			}
		}

		public int WorkerLifetimeLimit
		{
			get
			{
				return this.workerLifetimeLimit;
			}
		}

		public int WorkerIdleTimeout
		{
			get
			{
				return this.workerIdleTimeout;
			}
		}

		public int MaxTransactionsPerProcess
		{
			get
			{
				return this.maxTransactionsPerProcess;
			}
		}

		public int TransactionTimeout
		{
			get
			{
				return this.transactionTimeout;
			}
		}

		public string WorkerProcessPath
		{
			get
			{
				return this.workerProcessPath;
			}
		}

		public bool RunAsLocalService
		{
			get
			{
				return (this.flags & ComWorkerConfiguration.RunAsFlag.RunAsLocalService) == ComWorkerConfiguration.RunAsFlag.RunAsLocalService;
			}
		}

		public bool MayRunUnderAnotherJobObject
		{
			get
			{
				return (this.flags & ComWorkerConfiguration.RunAsFlag.MayRunUnderAnotherJobObject) == ComWorkerConfiguration.RunAsFlag.MayRunUnderAnotherJobObject;
			}
		}

		public Mutex ProcessLaunchMutex
		{
			get
			{
				return this.processLaunchMutex;
			}
		}

		public Guid WorkerGuid
		{
			get
			{
				return this.workerGuid;
			}
		}

		public string ExtraCommandLineParameters
		{
			get
			{
				return this.cmdlineParams;
			}
		}

		public Type WorkerType
		{
			get
			{
				if (this.workerType == null)
				{
					this.workerType = Type.GetTypeFromCLSID(this.workerGuid);
				}
				return this.workerType;
			}
		}

		private int workerMemoryLimit;

		private int workerAllocationTimeout;

		private int workerLifetimeLimit;

		private int workerIdleTimeout;

		private int maxTransactionsPerProcess;

		private int transactionTimeout;

		private string workerProcessPath;

		private Mutex processLaunchMutex;

		private Guid workerGuid;

		private Type workerType;

		private ComWorkerConfiguration.RunAsFlag flags;

		private string cmdlineParams;

		[Flags]
		public enum RunAsFlag
		{
			None = 0,
			RunAsLocalService = 1,
			MayRunUnderAnotherJobObject = 2
		}
	}
}

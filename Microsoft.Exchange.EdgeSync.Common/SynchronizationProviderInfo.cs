using System;

namespace Microsoft.Exchange.EdgeSync
{
	internal struct SynchronizationProviderInfo
	{
		public SynchronizationProviderInfo(string name, string assemblyPath, string synchronizationProvider, bool enabled)
		{
			this.name = name;
			this.assemblyPath = assemblyPath;
			this.synchronizationProvider = synchronizationProvider;
			this.enabled = enabled;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string AssemblyPath
		{
			get
			{
				return this.assemblyPath;
			}
		}

		public string SynchronizationProvider
		{
			get
			{
				return this.synchronizationProvider;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
		}

		private string name;

		private string assemblyPath;

		private string synchronizationProvider;

		private bool enabled;
	}
}

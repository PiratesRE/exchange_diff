using System;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement.Implementation
{
	internal class SingleContext : IContextPlugin
	{
		public static IContextPlugin Singleton
		{
			get
			{
				if (SingleContext.singleton == null)
				{
					SingleContext.singleton = new SingleContext();
				}
				return SingleContext.singleton;
			}
		}

		public Guid? LocalId
		{
			get
			{
				foreach (IContextPlugin contextPlugin in SingleContext.pluginChain)
				{
					if (contextPlugin.IsContextPresent)
					{
						return contextPlugin.LocalId;
					}
				}
				return null;
			}
			set
			{
				foreach (IContextPlugin contextPlugin in SingleContext.pluginChain)
				{
					contextPlugin.LocalId = value;
				}
			}
		}

		public bool IsContextPresent
		{
			get
			{
				return true;
			}
		}

		public void SetId()
		{
			foreach (IContextPlugin contextPlugin in SingleContext.pluginChain)
			{
				contextPlugin.SetId();
			}
		}

		public bool CheckId()
		{
			foreach (IContextPlugin contextPlugin in SingleContext.pluginChain)
			{
				if (contextPlugin.IsContextPresent)
				{
					return contextPlugin.CheckId();
				}
			}
			return false;
		}

		public void Clear()
		{
			foreach (IContextPlugin contextPlugin in SingleContext.pluginChain)
			{
				contextPlugin.Clear();
			}
			DebugContext.Clear();
		}

		private static readonly IContextPlugin[] pluginChain = new IContextPlugin[]
		{
			HttpCallContext.Singleton,
			DotNetCallContext.Singleton
		};

		private static IContextPlugin singleton = null;
	}
}

using System;

namespace Microsoft.Exchange.Transport
{
	internal class ComponentsWrapper : IComponentsWrapper
	{
		public bool IsPaused
		{
			get
			{
				return Components.IsPaused;
			}
		}

		public bool IsActive
		{
			get
			{
				return Components.IsActive;
			}
		}

		public bool IsShuttingDown
		{
			get
			{
				return Components.ShuttingDown;
			}
		}

		public bool IsBridgeHead
		{
			get
			{
				return Components.IsBridgehead;
			}
		}

		public object SyncRoot
		{
			get
			{
				return Components.SyncRoot;
			}
		}
	}
}

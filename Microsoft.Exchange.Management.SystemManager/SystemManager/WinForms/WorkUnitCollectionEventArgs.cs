using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class WorkUnitCollectionEventArgs : EventArgs
	{
		internal WorkUnitCollectionEventArgs(WorkUnitCollection workUnits)
		{
			this.workUnits = workUnits;
		}

		internal WorkUnitCollection WorkUnits
		{
			get
			{
				return this.workUnits;
			}
		}

		private WorkUnitCollection workUnits;
	}
}

using System;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class DatabaseAvailabilityGroupNetworkCreator : IDataObjectCreator
	{
		public object Create(DataTable table)
		{
			return new DatabaseAvailabilityGroupNetwork();
		}
	}
}

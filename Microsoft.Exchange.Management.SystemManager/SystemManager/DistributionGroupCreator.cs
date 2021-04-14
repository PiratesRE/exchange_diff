using System;
using System.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class DistributionGroupCreator : IDataObjectCreator
	{
		public object Create(DataTable table)
		{
			return new DistributionGroup(new ADGroup());
		}
	}
}

using System;
using Microsoft.Exchange.EDiscovery.Export;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch
{
	internal class CopyTargetLocation : ITargetLocation
	{
		public CopyTargetLocation(string exportLocation, string workingLocation)
		{
			Util.ThrowIfNullOrEmpty(exportLocation, "exportLocation");
			Util.ThrowIfNullOrEmpty(workingLocation, "workingLocation");
			this.ExportLocation = exportLocation;
			this.WorkingLocation = workingLocation;
			this.UnsearchableExportLocation = exportLocation;
		}

		public string ExportLocation { get; private set; }

		public string WorkingLocation { get; private set; }

		public string UnsearchableExportLocation { get; private set; }
	}
}

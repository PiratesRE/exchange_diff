using System;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class RepairTaskAccessLevelAttribute : Attribute
	{
		public RepairTaskAccessLevelAttribute(RepairTaskAccess access)
		{
			this.Access = access;
		}

		public RepairTaskAccess Access { get; private set; }
	}
}

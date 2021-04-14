using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public struct SpecialColumns
	{
		public SpecialColumns(PhysicalColumn propertyBlob, PhysicalColumn offPagePropertyBlob, PhysicalColumn propBag, int numberOfPartioningColumns)
		{
			this.PropertyBlob = propertyBlob;
			this.OffPagePropertyBlob = offPagePropertyBlob;
			this.PropBag = propBag;
			this.NumberOfPartioningColumns = numberOfPartioningColumns;
		}

		public PhysicalColumn PropertyBlob;

		public PhysicalColumn OffPagePropertyBlob;

		public PhysicalColumn PropBag;

		public int NumberOfPartioningColumns;
	}
}

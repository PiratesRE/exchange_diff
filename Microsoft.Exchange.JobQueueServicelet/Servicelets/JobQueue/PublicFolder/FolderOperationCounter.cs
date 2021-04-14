using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Servicelets.JobQueue.PublicFolder
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FolderOperationCounter
	{
		public int Added { get; set; }

		public int Updated { get; set; }

		public int Deleted { get; set; }

		public int OrphanDetected { get; set; }

		public int OrphanFixed { get; set; }

		public int ParentChainMissing { get; set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Added={0},Updated={1},Deleted={2},OrphanDetected={3},OrphanFixed={4},ParentMissing={5}", new object[]
			{
				this.Added,
				this.Updated,
				this.Deleted,
				this.OrphanDetected,
				this.OrphanFixed,
				this.ParentChainMissing
			});
		}
	}
}

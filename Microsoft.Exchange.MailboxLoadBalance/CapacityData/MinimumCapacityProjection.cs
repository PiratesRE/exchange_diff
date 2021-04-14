using System;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.CapacityData
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MinimumCapacityProjection : ICapacityProjection
	{
		public MinimumCapacityProjection(ILogger logger, params ICapacityProjection[] projections)
		{
			this.logger = logger;
			this.projections = projections;
		}

		public BatchCapacityDatum GetCapacity()
		{
			BatchCapacityDatum batchCapacityDatum = this.projections.Min((ICapacityProjection p) => p.GetCapacity());
			this.logger.LogVerbose("Selected minimum capacity datum: {0}", new object[]
			{
				batchCapacityDatum
			});
			return batchCapacityDatum;
		}

		private readonly ILogger logger;

		private readonly ICapacityProjection[] projections;
	}
}

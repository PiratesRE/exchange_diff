using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class OccurrenceCrossingBoundaryExceptionMapping : StaticExceptionMapping
	{
		public OccurrenceCrossingBoundaryExceptionMapping() : base(typeof(OccurrenceCrossingBoundaryException), ResponseCodeType.ErrorOccurrenceCrossingBoundary, CoreResources.IDs.ErrorOccurrenceCrossingBoundary)
		{
		}

		protected override IDictionary<string, string> GetConstantValues(LocalizedException exception)
		{
			OccurrenceCrossingBoundaryException ex = base.VerifyExceptionType<OccurrenceCrossingBoundaryException>(exception);
			return new Dictionary<string, string>
			{
				{
					"AdjacentOccurrenceOriginalStartTime",
					ex.NeighborInfo.OriginalStartTime.ToString()
				},
				{
					"AdjacentOccurrenceStartTime",
					ex.NeighborInfo.StartTime.ToString()
				},
				{
					"AdjacentOccurrenceEndTime",
					ex.NeighborInfo.EndTime.ToString()
				},
				{
					"ModifiedOccurrenceOriginalStartTime",
					ex.OccurrenceInfo.OriginalStartTime.ToString()
				},
				{
					"ModifiedOccurrenceStartTime",
					ex.OccurrenceInfo.StartTime.ToString()
				},
				{
					"ModifiedOccurrenceEndTime",
					ex.OccurrenceInfo.EndTime.ToString()
				}
			};
		}

		private const string AdjacentOccurrenceOriginalStartTime = "AdjacentOccurrenceOriginalStartTime";

		private const string AdjacentOccurrenceStartTime = "AdjacentOccurrenceStartTime";

		private const string AdjacentOccurrenceEndTime = "AdjacentOccurrenceEndTime";

		private const string ModifiedOccurrenceOriginalStartTime = "ModifiedOccurrenceOriginalStartTime";

		private const string ModifiedOccurrenceStartTime = "ModifiedOccurrenceStartTime";

		private const string ModifiedOccurrenceEndTime = "ModifiedOccurrenceEndTime";
	}
}

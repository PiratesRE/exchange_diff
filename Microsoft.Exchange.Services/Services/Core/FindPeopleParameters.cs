using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FindPeopleParameters
	{
		public string QueryString { get; set; }

		public SortResults[] SortResults { get; set; }

		public BasePagingType Paging { get; set; }

		public RestrictionType Restriction { get; set; }

		public RestrictionType AggregationRestriction { get; set; }

		public PersonaResponseShape PersonaShape { get; set; }

		public CultureInfo CultureInfo { get; set; }

		public RequestDetailsLogger Logger { get; set; }
	}
}

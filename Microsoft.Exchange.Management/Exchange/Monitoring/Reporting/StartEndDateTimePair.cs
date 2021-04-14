using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Monitoring.Reporting
{
	public struct StartEndDateTimePair
	{
		public StartEndDateTimePair(ExDateTime startDate, ExDateTime endDate)
		{
			this.startDate = startDate;
			this.endDate = endDate;
		}

		public ExDateTime StartDate
		{
			get
			{
				return this.startDate;
			}
		}

		public ExDateTime EndDate
		{
			get
			{
				return this.endDate;
			}
		}

		private ExDateTime startDate;

		private ExDateTime endDate;
	}
}

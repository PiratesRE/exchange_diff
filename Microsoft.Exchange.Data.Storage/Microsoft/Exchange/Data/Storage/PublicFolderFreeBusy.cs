using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublicFolderFreeBusy
	{
		public ExDateTime StartDate
		{
			get
			{
				return this.startDate;
			}
			set
			{
				this.startDate = value;
			}
		}

		public int NumberOfMonths
		{
			get
			{
				return this.numberOfMonths;
			}
			set
			{
				this.numberOfMonths = value;
			}
		}

		public List<PublicFolderFreeBusyAppointment> Appointments
		{
			get
			{
				return this.appointments;
			}
			set
			{
				this.appointments = value;
			}
		}

		private int numberOfMonths;

		private ExDateTime startDate;

		private List<PublicFolderFreeBusyAppointment> appointments;
	}
}

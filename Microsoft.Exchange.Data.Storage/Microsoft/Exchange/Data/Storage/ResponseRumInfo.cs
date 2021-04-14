using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ResponseRumInfo : AttendeeRumInfo
	{
		private ResponseRumInfo() : this(null)
		{
		}

		private ResponseRumInfo(ExDateTime? originalStartTime) : base(RumType.Response, originalStartTime)
		{
		}

		public static ResponseRumInfo CreateMasterInstance()
		{
			return new ResponseRumInfo();
		}

		public static ResponseRumInfo CreateOccurrenceInstance(ExDateTime originalStartTime)
		{
			return new ResponseRumInfo(new ExDateTime?(originalStartTime));
		}
	}
}

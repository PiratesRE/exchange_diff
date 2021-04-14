using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class RecurrenceHasNoOccurrenceExceptionMapping : StaticExceptionMapping
	{
		public RecurrenceHasNoOccurrenceExceptionMapping() : base(typeof(RecurrenceHasNoOccurrenceException), ResponseCodeType.ErrorRecurrenceHasNoOccurrence, CoreResources.IDs.ErrorRecurrenceHasNoOccurrence)
		{
		}

		protected override IDictionary<string, string> GetConstantValues(LocalizedException exception)
		{
			RecurrenceHasNoOccurrenceException ex = base.VerifyExceptionType<RecurrenceHasNoOccurrenceException>(exception);
			return new Dictionary<string, string>
			{
				{
					"EffectiveStartDate",
					ex.EffectiveStartDate.ToString()
				},
				{
					"EffectiveEndDate",
					ex.EffectiveEndDate.ToString()
				}
			};
		}

		private const string EffectiveStartDate = "EffectiveStartDate";

		private const string EffectiveEndDate = "EffectiveEndDate";
	}
}

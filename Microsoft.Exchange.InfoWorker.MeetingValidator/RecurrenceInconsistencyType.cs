using System;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	internal enum RecurrenceInconsistencyType
	{
		None,
		MissingRecurrence,
		ExtraRecurrence,
		MissingModification,
		ExtraModification,
		MissingDeletion,
		ExtraDeletion
	}
}

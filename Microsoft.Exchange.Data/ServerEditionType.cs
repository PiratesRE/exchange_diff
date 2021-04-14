using System;

namespace Microsoft.Exchange.Data
{
	public enum ServerEditionType
	{
		[LocDescription(DataStrings.IDs.UnknownEdition)]
		Unknown,
		[LocDescription(DataStrings.IDs.StandardEdition)]
		Standard,
		[LocDescription(DataStrings.IDs.StandardTrialEdition)]
		StandardEvaluation,
		[LocDescription(DataStrings.IDs.EnterpriseEdition)]
		Enterprise,
		[LocDescription(DataStrings.IDs.EnterpriseTrialEdition)]
		EnterpriseEvaluation,
		[LocDescription(DataStrings.IDs.CoexistenceEdition)]
		Coexistence,
		[LocDescription(DataStrings.IDs.CoexistenceTrialEdition)]
		CoexistenceEvaluation
	}
}

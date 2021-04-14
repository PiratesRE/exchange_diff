using System;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal interface IRuleEvaluator
	{
		bool Evaluate(IDataLoader dataLoader);
	}
}

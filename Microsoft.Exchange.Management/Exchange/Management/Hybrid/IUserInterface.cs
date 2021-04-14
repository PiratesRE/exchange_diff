using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface IUserInterface
	{
		void WriteVerbose(LocalizedString text);

		void WriteWarning(LocalizedString text);

		void WriteProgessIndicator(LocalizedString activity, LocalizedString statusDescription, int percentCompleted);

		bool ShouldContinue(LocalizedString message);
	}
}

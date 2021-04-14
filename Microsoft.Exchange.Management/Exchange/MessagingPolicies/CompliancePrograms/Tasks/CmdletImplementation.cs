using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	internal class CmdletImplementation
	{
		public CmdletImplementation.ShouldContinueMethod ShouldContinue { get; set; }

		public IConfigDataProvider DataSession { get; set; }

		public virtual void ProcessRecord()
		{
		}

		public virtual void Validate()
		{
		}

		public delegate bool ShouldContinueMethod(LocalizedString prompt);
	}
}

using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Register", "ComInteropTlb")]
	[LocDescription(Strings.IDs.RegisterComInteropTlbTask)]
	public sealed class RegisterComInteropTlbTask : ComInteropTlbTaskBase
	{
		public RegisterComInteropTlbTask() : base(true)
		{
		}

		[Postcondition(ExpectedResult = true, FailureDescriptionId = Strings.IDs.ComInteropTlbNotFound)]
		internal FileExistsCondition ComInteropTlbExistCheck
		{
			get
			{
				return new FileExistsCondition(Path.Combine(ConfigurationContext.Setup.BinPath, "ComInterop.tlb"));
			}
		}
	}
}

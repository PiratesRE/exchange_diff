using System;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.DDIService
{
	internal class RbacCheckerWrapper
	{
		private RbacCheckerWrapper()
		{
		}

		internal static IIsInRole RbacChecker
		{
			get
			{
				return RbacCheckerWrapper.rbacChecker ?? RbacPrincipal.Current;
			}
			set
			{
				RbacCheckerWrapper.rbacChecker = value;
			}
		}

		private static IIsInRole rbacChecker;
	}
}

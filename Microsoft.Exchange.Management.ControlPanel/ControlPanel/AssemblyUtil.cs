using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class AssemblyUtil
	{
		internal static string OwaAppVersion
		{
			get
			{
				if (AssemblyUtil.owaApplicationVersion == null)
				{
					AssemblyUtil.owaApplicationVersion = Globals.ApplicationVersion;
				}
				return AssemblyUtil.owaApplicationVersion;
			}
		}

		private static string owaApplicationVersion;
	}
}

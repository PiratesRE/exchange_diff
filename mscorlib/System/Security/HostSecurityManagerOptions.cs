using System;
using System.Runtime.InteropServices;

namespace System.Security
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum HostSecurityManagerOptions
	{
		None = 0,
		HostAppDomainEvidence = 1,
		[Obsolete("AppDomain policy levels are obsolete and will be removed in a future release of the .NET Framework. See http://go.microsoft.com/fwlink/?LinkID=155570 for more information.")]
		HostPolicyLevel = 2,
		HostAssemblyEvidence = 4,
		HostDetermineApplicationTrust = 8,
		HostResolvePolicy = 16,
		AllFlags = 31
	}
}

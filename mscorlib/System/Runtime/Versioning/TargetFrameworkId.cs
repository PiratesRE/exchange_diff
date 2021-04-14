using System;
using System.Runtime.CompilerServices;

namespace System.Runtime.Versioning
{
	[FriendAccessAllowed]
	internal enum TargetFrameworkId
	{
		NotYetChecked,
		Unrecognized,
		Unspecified,
		NetFramework,
		Portable,
		NetCore,
		Silverlight,
		Phone
	}
}

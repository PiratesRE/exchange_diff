using System;

namespace Microsoft.Exchange.Management.Deployment
{
	[Flags]
	internal enum InstallUILevel
	{
		NoChange = 0,
		Default = 1,
		None = 2,
		Basic = 3,
		Reduced = 4,
		Full = 5,
		EndDialog = 128,
		ProgressOnly = 64,
		HideCancel = 32,
		SourceResOnly = 256
	}
}

using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	[Flags]
	public enum StageGrbit
	{
		None = 0,
		TestEnvLocalMode = 4,
		TestEnvAlphaMode = 16,
		TestEnvBetaMode = 64,
		SelfhostLocalMode = 1024,
		SelfhostAlphaMode = 4096,
		SelfhostBetaMode = 16384,
		ProdLocalMode = 262144,
		ProdAlphaMode = 1048576,
		ProdBetaMode = 4194304
	}
}

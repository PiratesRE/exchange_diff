using System;

namespace Microsoft.Isam.Esent.Interop
{
	public enum JET_IdxInfo
	{
		Default,
		List,
		[Obsolete("This value is not used, and is provided for completeness to match the published header in the SDK.")]
		SysTabCursor,
		[Obsolete("This value is not used, and is provided for completeness to match the published header in the SDK.")]
		OLC,
		[Obsolete("This value is not used, and is provided for completeness to match the published header in the SDK.")]
		ResetOLC,
		SpaceAlloc,
		LCID,
		[Obsolete("Use JET_IdxInfo.LCID")]
		Langid = 6,
		Count,
		VarSegMac,
		IndexId,
		KeyMost
	}
}

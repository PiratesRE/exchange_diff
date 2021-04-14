using System;

namespace System.Diagnostics.Tracing
{
	internal enum ControllerCommand
	{
		Update,
		SendManifest = -1,
		Enable = -2,
		Disable = -3
	}
}

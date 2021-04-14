using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Guid("9de1c535-6ae1-11e0-84e1-18a905bcc53f")]
	internal delegate void WindowsFoundationEventHandler<T>(object sender, T args);
}

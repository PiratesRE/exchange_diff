using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Guid("60D27C8D-5F61-4CCE-B751-690FAE66AA53")]
	[ComImport]
	internal interface IManagedActivationFactory
	{
		void RunClassConstructor();
	}
}

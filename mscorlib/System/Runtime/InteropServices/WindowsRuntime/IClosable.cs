using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Guid("30d5a829-7fa4-4026-83bb-d75bae4ea99e")]
	[ComImport]
	internal interface IClosable
	{
		void Close();
	}
}

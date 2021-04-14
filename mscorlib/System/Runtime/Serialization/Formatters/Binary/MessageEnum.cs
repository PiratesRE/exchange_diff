using System;

namespace System.Runtime.Serialization.Formatters.Binary
{
	[Flags]
	[Serializable]
	internal enum MessageEnum
	{
		NoArgs = 1,
		ArgsInline = 2,
		ArgsIsArray = 4,
		ArgsInArray = 8,
		NoContext = 16,
		ContextInline = 32,
		ContextInArray = 64,
		MethodSignatureInArray = 128,
		PropertyInArray = 256,
		NoReturnValue = 512,
		ReturnValueVoid = 1024,
		ReturnValueInline = 2048,
		ReturnValueInArray = 4096,
		ExceptionInArray = 8192,
		GenericMethod = 32768
	}
}

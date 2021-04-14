using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Guid("7C925755-3E48-42B4-8677-76372267033F")]
	[ComImport]
	internal interface ICustomPropertyProvider
	{
		ICustomProperty GetCustomProperty(string name);

		ICustomProperty GetIndexedProperty(string name, Type indexParameterType);

		string GetStringRepresentation();

		Type Type { get; }
	}
}

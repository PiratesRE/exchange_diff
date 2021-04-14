using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Guid("30DA92C0-23E8-42A0-AE7C-734A0E5D2782")]
	[ComImport]
	internal interface ICustomProperty
	{
		Type Type { get; }

		string Name { get; }

		object GetValue(object target);

		void SetValue(object target, object value);

		object GetValue(object target, object indexValue);

		void SetValue(object target, object value, object indexValue);

		bool CanWrite { get; }

		bool CanRead { get; }
	}
}

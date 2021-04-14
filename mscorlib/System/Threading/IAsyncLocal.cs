using System;
using System.Security;

namespace System.Threading
{
	internal interface IAsyncLocal
	{
		[SecurityCritical]
		void OnValueChanged(object previousValue, object currentValue, bool contextChanged);
	}
}

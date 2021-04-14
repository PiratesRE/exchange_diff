using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Contexts
{
	[ComVisible(true)]
	public interface IContextProperty
	{
		string Name { [SecurityCritical] get; }

		[SecurityCritical]
		bool IsNewContextOK(Context newCtx);

		[SecurityCritical]
		void Freeze(Context newContext);
	}
}

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting
{
	[ComVisible(true)]
	public interface IRemotingTypeInfo
	{
		string TypeName { [SecurityCritical] get; [SecurityCritical] set; }

		[SecurityCritical]
		bool CanCastTo(Type fromType, object o);
	}
}

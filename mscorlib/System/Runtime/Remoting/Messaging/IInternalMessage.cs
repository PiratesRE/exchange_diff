using System;
using System.Security;

namespace System.Runtime.Remoting.Messaging
{
	internal interface IInternalMessage
	{
		ServerIdentity ServerIdentityObject { [SecurityCritical] get; [SecurityCritical] set; }

		Identity IdentityObject { [SecurityCritical] get; [SecurityCritical] set; }

		[SecurityCritical]
		void SetURI(string uri);

		[SecurityCritical]
		void SetCallContext(LogicalCallContext callContext);

		[SecurityCritical]
		bool HasProperties();
	}
}

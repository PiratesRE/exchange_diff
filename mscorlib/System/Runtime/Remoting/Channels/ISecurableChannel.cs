using System;
using System.Security;

namespace System.Runtime.Remoting.Channels
{
	public interface ISecurableChannel
	{
		bool IsSecured { [SecurityCritical] get; [SecurityCritical] set; }
	}
}

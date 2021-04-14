using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Diagnostics.Audit
{
	public class PrivilegeNotHeldException : Exception
	{
		public PrivilegeNotHeldException(string msg) : base(msg)
		{
		}

		public PrivilegeNotHeldException(SerializationInfo info, StreamingContext context)
		{
		}
	}
}

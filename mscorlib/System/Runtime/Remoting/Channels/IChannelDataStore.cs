using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IChannelDataStore
	{
		string[] ChannelUris { [SecurityCritical] get; }

		object this[object key]
		{
			[SecurityCritical]
			get;
			[SecurityCritical]
			set;
		}
	}
}

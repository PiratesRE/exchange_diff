using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Lifetime
{
	[ComVisible(true)]
	public interface ILease
	{
		[SecurityCritical]
		void Register(ISponsor obj, TimeSpan renewalTime);

		[SecurityCritical]
		void Register(ISponsor obj);

		[SecurityCritical]
		void Unregister(ISponsor obj);

		[SecurityCritical]
		TimeSpan Renew(TimeSpan renewalTime);

		TimeSpan RenewOnCallTime { [SecurityCritical] get; [SecurityCritical] set; }

		TimeSpan SponsorshipTimeout { [SecurityCritical] get; [SecurityCritical] set; }

		TimeSpan InitialLeaseTime { [SecurityCritical] get; [SecurityCritical] set; }

		TimeSpan CurrentLeaseTime { [SecurityCritical] get; }

		LeaseState CurrentState { [SecurityCritical] get; }
	}
}

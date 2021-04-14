using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ExecutingUserBinding : StaticBinding
	{
		public override bool HasValue
		{
			get
			{
				return true;
			}
		}

		public override object Value
		{
			get
			{
				Identity identity = Identity.FromExecutingUserId();
				return identity.RawIdentity;
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}

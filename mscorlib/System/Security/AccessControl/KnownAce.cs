using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public abstract class KnownAce : GenericAce
	{
		internal KnownAce(AceType type, AceFlags flags, int accessMask, SecurityIdentifier securityIdentifier) : base(type, flags)
		{
			if (securityIdentifier == null)
			{
				throw new ArgumentNullException("securityIdentifier");
			}
			this.AccessMask = accessMask;
			this.SecurityIdentifier = securityIdentifier;
		}

		public int AccessMask
		{
			get
			{
				return this._accessMask;
			}
			set
			{
				this._accessMask = value;
			}
		}

		public SecurityIdentifier SecurityIdentifier
		{
			get
			{
				return this._sid;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this._sid = value;
			}
		}

		private int _accessMask;

		private SecurityIdentifier _sid;

		internal const int AccessMaskLength = 4;
	}
}

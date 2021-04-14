using System;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Security
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ClientIdentityInfo
	{
		public ClientIdentityInfo(IntPtr __hAuthZ, SecurityIdentifier __sidUser, SecurityIdentifier __sidGroup)
		{
			this._hAuthZ = __hAuthZ;
			this._sidUser = __sidUser;
			this._sidPrimaryGroup = __sidGroup;
			this._hToken = IntPtr.Zero;
		}

		internal ClientIdentityInfo(IntPtr __hToken)
		{
			this._hAuthZ = IntPtr.Zero;
			this._sidUser = null;
			this._sidPrimaryGroup = null;
			this._hToken = __hToken;
		}

		public IntPtr hAuthZ
		{
			get
			{
				return this._hAuthZ;
			}
		}

		public SecurityIdentifier sidPrimaryGroup
		{
			get
			{
				return this._sidPrimaryGroup;
			}
		}

		public SecurityIdentifier sidUser
		{
			get
			{
				return this._sidUser;
			}
		}

		internal IntPtr hToken
		{
			get
			{
				return this._hToken;
			}
		}

		private IntPtr _hAuthZ;

		private SecurityIdentifier _sidUser;

		private SecurityIdentifier _sidPrimaryGroup;

		private IntPtr _hToken;
	}
}

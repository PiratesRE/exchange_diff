using System;
using System.Security.Util;

namespace System.Security
{
	internal struct PermissionSetEnumeratorInternal
	{
		public object Current
		{
			get
			{
				return this.enm.Current;
			}
		}

		internal PermissionSetEnumeratorInternal(PermissionSet permSet)
		{
			this.m_permSet = permSet;
			this.enm = new TokenBasedSetEnumerator(permSet.m_permSet);
		}

		public int GetCurrentIndex()
		{
			return this.enm.Index;
		}

		public void Reset()
		{
			this.enm.Reset();
		}

		public bool MoveNext()
		{
			while (this.enm.MoveNext())
			{
				object current = this.enm.Current;
				IPermission permission = current as IPermission;
				if (permission != null)
				{
					this.enm.Current = permission;
					return true;
				}
				SecurityElement securityElement = current as SecurityElement;
				if (securityElement != null)
				{
					permission = this.m_permSet.CreatePermission(securityElement, this.enm.Index);
					if (permission != null)
					{
						this.enm.Current = permission;
						return true;
					}
				}
			}
			return false;
		}

		private PermissionSet m_permSet;

		private TokenBasedSetEnumerator enm;
	}
}

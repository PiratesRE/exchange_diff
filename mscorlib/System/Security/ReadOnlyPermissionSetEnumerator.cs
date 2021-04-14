using System;
using System.Collections;

namespace System.Security
{
	internal sealed class ReadOnlyPermissionSetEnumerator : IEnumerator
	{
		internal ReadOnlyPermissionSetEnumerator(IEnumerator permissionSetEnumerator)
		{
			this.m_permissionSetEnumerator = permissionSetEnumerator;
		}

		public object Current
		{
			get
			{
				IPermission permission = this.m_permissionSetEnumerator.Current as IPermission;
				if (permission == null)
				{
					return null;
				}
				return permission.Copy();
			}
		}

		public bool MoveNext()
		{
			return this.m_permissionSetEnumerator.MoveNext();
		}

		public void Reset()
		{
			this.m_permissionSetEnumerator.Reset();
		}

		private IEnumerator m_permissionSetEnumerator;
	}
}

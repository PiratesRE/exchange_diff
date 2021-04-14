using System;
using System.Collections;

namespace System.Security
{
	internal class PermissionSetEnumerator : IEnumerator
	{
		public object Current
		{
			get
			{
				return this.enm.Current;
			}
		}

		public bool MoveNext()
		{
			return this.enm.MoveNext();
		}

		public void Reset()
		{
			this.enm.Reset();
		}

		internal PermissionSetEnumerator(PermissionSet permSet)
		{
			this.enm = new PermissionSetEnumeratorInternal(permSet);
		}

		private PermissionSetEnumeratorInternal enm;
	}
}

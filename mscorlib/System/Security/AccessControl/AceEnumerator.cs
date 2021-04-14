using System;
using System.Collections;

namespace System.Security.AccessControl
{
	public sealed class AceEnumerator : IEnumerator
	{
		internal AceEnumerator(GenericAcl collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			this._acl = collection;
			this.Reset();
		}

		object IEnumerator.Current
		{
			get
			{
				if (this._current == -1 || this._current >= this._acl.Count)
				{
					throw new InvalidOperationException(Environment.GetResourceString("Arg_InvalidOperationException"));
				}
				return this._acl[this._current];
			}
		}

		public GenericAce Current
		{
			get
			{
				return ((IEnumerator)this).Current as GenericAce;
			}
		}

		public bool MoveNext()
		{
			this._current++;
			return this._current < this._acl.Count;
		}

		public void Reset()
		{
			this._current = -1;
		}

		private int _current;

		private readonly GenericAcl _acl;
	}
}

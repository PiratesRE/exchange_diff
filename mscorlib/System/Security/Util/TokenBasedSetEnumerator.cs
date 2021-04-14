using System;

namespace System.Security.Util
{
	internal struct TokenBasedSetEnumerator
	{
		public bool MoveNext()
		{
			return this._tb != null && this._tb.MoveNext(ref this);
		}

		public void Reset()
		{
			this.Index = -1;
			this.Current = null;
		}

		public TokenBasedSetEnumerator(TokenBasedSet tb)
		{
			this.Index = -1;
			this.Current = null;
			this._tb = tb;
		}

		public object Current;

		public int Index;

		private TokenBasedSet _tb;
	}
}

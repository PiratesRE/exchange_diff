using System;

namespace System
{
	[Serializable]
	public sealed class ConsoleCancelEventArgs : EventArgs
	{
		internal ConsoleCancelEventArgs(ConsoleSpecialKey type)
		{
			this._type = type;
			this._cancel = false;
		}

		public bool Cancel
		{
			get
			{
				return this._cancel;
			}
			set
			{
				this._cancel = value;
			}
		}

		public ConsoleSpecialKey SpecialKey
		{
			get
			{
				return this._type;
			}
		}

		private ConsoleSpecialKey _type;

		private bool _cancel;
	}
}

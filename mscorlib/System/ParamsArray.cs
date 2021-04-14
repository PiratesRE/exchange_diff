using System;

namespace System
{
	internal struct ParamsArray
	{
		public ParamsArray(object arg0)
		{
			this.arg0 = arg0;
			this.arg1 = null;
			this.arg2 = null;
			this.args = ParamsArray.oneArgArray;
		}

		public ParamsArray(object arg0, object arg1)
		{
			this.arg0 = arg0;
			this.arg1 = arg1;
			this.arg2 = null;
			this.args = ParamsArray.twoArgArray;
		}

		public ParamsArray(object arg0, object arg1, object arg2)
		{
			this.arg0 = arg0;
			this.arg1 = arg1;
			this.arg2 = arg2;
			this.args = ParamsArray.threeArgArray;
		}

		public ParamsArray(object[] args)
		{
			int num = args.Length;
			this.arg0 = ((num > 0) ? args[0] : null);
			this.arg1 = ((num > 1) ? args[1] : null);
			this.arg2 = ((num > 2) ? args[2] : null);
			this.args = args;
		}

		public int Length
		{
			get
			{
				return this.args.Length;
			}
		}

		public object this[int index]
		{
			get
			{
				if (index != 0)
				{
					return this.GetAtSlow(index);
				}
				return this.arg0;
			}
		}

		private object GetAtSlow(int index)
		{
			if (index == 1)
			{
				return this.arg1;
			}
			if (index == 2)
			{
				return this.arg2;
			}
			return this.args[index];
		}

		private static readonly object[] oneArgArray = new object[1];

		private static readonly object[] twoArgArray = new object[2];

		private static readonly object[] threeArgArray = new object[3];

		private readonly object arg0;

		private readonly object arg1;

		private readonly object arg2;

		private readonly object[] args;
	}
}

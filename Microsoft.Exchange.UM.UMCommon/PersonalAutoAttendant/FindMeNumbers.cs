using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class FindMeNumbers
	{
		public FindMeNumbers(string number, int timeout) : this(number, timeout, string.Empty)
		{
		}

		public FindMeNumbers(string number, int timeout, string label)
		{
			this.numbers = new List<FindMe>();
			this.Add(number, timeout, label);
		}

		public FindMe[] NumberList
		{
			get
			{
				return this.numbers.ToArray();
			}
		}

		public int Count
		{
			get
			{
				return this.numbers.Count;
			}
		}

		public FindMe this[int index]
		{
			get
			{
				if (index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this.numbers[index];
			}
			set
			{
				this.numbers[index] = value;
			}
		}

		public void Add(string number, int timeout)
		{
			this.numbers.Add(new FindMe(number, timeout));
		}

		public void Add(string number, int timeout, string label)
		{
			this.numbers.Add(new FindMe(number, timeout, label));
		}

		private List<FindMe> numbers;
	}
}

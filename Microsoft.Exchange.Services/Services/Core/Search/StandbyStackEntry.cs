using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal class StandbyStackEntry<TInput, TOutput>
	{
		public StandbyStackEntry(TInput inputFilter)
		{
			this.inputFilter = inputFilter;
			this.workingStack = new Stack<TOutput>();
			this.currentChild = 0;
		}

		public TInput Filter
		{
			get
			{
				return this.inputFilter;
			}
			set
			{
				this.inputFilter = value;
			}
		}

		public Stack<TOutput> WorkingStack
		{
			get
			{
				return this.workingStack;
			}
			set
			{
				this.workingStack = value;
			}
		}

		public int CurrentChild
		{
			get
			{
				return this.currentChild;
			}
			set
			{
				this.currentChild = value;
			}
		}

		private TInput inputFilter;

		private Stack<TOutput> workingStack;

		private int currentChild;
	}
}

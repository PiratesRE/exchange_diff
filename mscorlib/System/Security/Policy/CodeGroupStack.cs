using System;
using System.Collections;

namespace System.Security.Policy
{
	internal sealed class CodeGroupStack
	{
		internal CodeGroupStack()
		{
			this.m_array = new ArrayList();
		}

		internal void Push(CodeGroupStackFrame element)
		{
			this.m_array.Add(element);
		}

		internal CodeGroupStackFrame Pop()
		{
			if (this.IsEmpty())
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EmptyStack"));
			}
			int count = this.m_array.Count;
			CodeGroupStackFrame result = (CodeGroupStackFrame)this.m_array[count - 1];
			this.m_array.RemoveAt(count - 1);
			return result;
		}

		internal bool IsEmpty()
		{
			return this.m_array.Count == 0;
		}

		private ArrayList m_array;
	}
}

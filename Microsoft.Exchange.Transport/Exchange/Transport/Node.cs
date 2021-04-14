using System;

namespace Microsoft.Exchange.Transport
{
	internal class Node<T>
	{
		public Node(T item)
		{
			this.data = item;
		}

		public T Value
		{
			get
			{
				return this.data;
			}
		}

		public Node<T> Next
		{
			get
			{
				return this.next;
			}
			set
			{
				this.next = value;
			}
		}

		private T data;

		private Node<T> next;
	}
}

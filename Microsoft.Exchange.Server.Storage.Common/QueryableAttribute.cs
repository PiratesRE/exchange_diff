using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class QueryableAttribute : Attribute
	{
		public int Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
			set
			{
				this.length = value;
			}
		}

		public Visibility Visibility
		{
			get
			{
				return this.visibility;
			}
			set
			{
				this.visibility = value;
			}
		}

		public const int DefaultIndex = -1;

		public const int DefaultLength = 1048576;

		private int index = -1;

		private int length = 1048576;

		private Visibility visibility;
	}
}

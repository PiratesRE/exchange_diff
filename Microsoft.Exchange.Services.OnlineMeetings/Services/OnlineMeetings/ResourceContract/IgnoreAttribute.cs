using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[AttributeUsage(AttributeTargets.Property)]
	internal class IgnoreAttribute : Attribute
	{
		public IgnoreAttribute()
		{
		}

		public IgnoreAttribute(string tokenContext)
		{
			this.tokenContext = tokenContext;
		}

		public string ContextToken
		{
			get
			{
				return this.tokenContext;
			}
		}

		public int MaximumRecursionLevel { get; set; }

		private readonly string tokenContext;
	}
}

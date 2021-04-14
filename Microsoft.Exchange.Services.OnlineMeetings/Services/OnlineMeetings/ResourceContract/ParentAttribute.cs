using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class ParentAttribute : Attribute
	{
		public ParentAttribute(string parentToken)
		{
			this.parentToken = parentToken;
		}

		public string ParentToken
		{
			get
			{
				return this.parentToken;
			}
		}

		private readonly string parentToken;
	}
}

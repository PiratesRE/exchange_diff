using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	internal class GetAttribute : HttpMethodAttribute
	{
		public GetAttribute(Type outputType) : base("Get")
		{
			this.outputType = outputType;
		}

		public Type OutputType
		{
			get
			{
				return this.outputType;
			}
		}

		private readonly Type outputType;
	}
}

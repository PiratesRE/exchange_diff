using System;
using System.Net;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	internal class PutAttribute : HttpMethodAttribute
	{
		public PutAttribute(Type inputType) : base("Put")
		{
			this.inputType = inputType;
			base.StatusCode = HttpStatusCode.OK;
		}

		public PutAttribute(Type inputType, Type outputType) : base("Post")
		{
			this.inputType = inputType;
			this.outputType = outputType;
			base.StatusCode = HttpStatusCode.OK;
		}

		public Type InputType
		{
			get
			{
				return this.inputType;
			}
		}

		public Type OutputType
		{
			get
			{
				return this.outputType;
			}
		}

		private readonly Type inputType;

		private readonly Type outputType;
	}
}

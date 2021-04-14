using System;
using System.Net;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	internal class PostAttribute : HttpMethodAttribute
	{
		public PostAttribute() : base("Post")
		{
			base.StatusCode = HttpStatusCode.NoContent;
		}

		public PostAttribute(Type inputType) : base("Post")
		{
			this.inputType = inputType;
			base.StatusCode = HttpStatusCode.NoContent;
		}

		public PostAttribute(Type inputType, Type outputType) : base("Post")
		{
			this.inputType = inputType;
			this.outputType = outputType;
			base.StatusCode = HttpStatusCode.Created;
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

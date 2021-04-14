using System;
using System.Net;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	internal class DeleteAttribute : HttpMethodAttribute
	{
		public DeleteAttribute() : base("Delete")
		{
			base.StatusCode = HttpStatusCode.NoContent;
		}
	}
}

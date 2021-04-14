using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	internal class ExternalResource : IResource
	{
		public object Value { get; set; }

		public string Href { get; set; }

		public string ContentId { get; set; }

		string IResource.SelfUri
		{
			get
			{
				return this.Href;
			}
		}
	}
}

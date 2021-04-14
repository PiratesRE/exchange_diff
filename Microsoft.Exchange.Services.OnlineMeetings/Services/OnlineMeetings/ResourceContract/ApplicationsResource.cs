using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Post(typeof(ApplicationSettings), typeof(ApplicationResource))]
	[CollectionDataContract(Name = "Applications")]
	[Parent("user")]
	internal class ApplicationsResource : Resource
	{
		public ApplicationsResource(string selfUri) : base(selfUri)
		{
		}

		public const string Token = "applications";
	}
}

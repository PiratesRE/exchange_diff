using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	internal abstract class CollectionContainerResource<T> : Resource where T : Resource
	{
		protected CollectionContainerResource(string collectionMemberName, string selfUri) : base(selfUri)
		{
			this.collectionMemberName = collectionMemberName;
		}

		public ResourceCollection<T> Members
		{
			get
			{
				return base.GetValue<ResourceCollection<T>>(this.collectionMemberName);
			}
			set
			{
				base.SetValue<ResourceCollection<T>>(this.collectionMemberName, value);
			}
		}

		private readonly string collectionMemberName;
	}
}

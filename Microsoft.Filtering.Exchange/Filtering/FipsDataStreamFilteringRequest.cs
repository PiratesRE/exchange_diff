using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.UnifiedContent;

namespace Microsoft.Filtering
{
	internal abstract class FipsDataStreamFilteringRequest
	{
		protected FipsDataStreamFilteringRequest(string id, ContentManager contentManager)
		{
			if (contentManager == null)
			{
				throw new ArgumentNullException("contentManager");
			}
			this.Id = id;
			this.ContentManager = contentManager;
		}

		public string Id { get; private set; }

		public ContentManager ContentManager { get; private set; }

		public abstract RecoveryOptions RecoveryOptions { get; set; }

		public FilteringRequest ToFilteringRequest(bool bypassBodyTextTruncation = false)
		{
			ContentManager contentManager = this.ContentManager;
			List<IExtractedContent> list = contentManager.ContentCollection;
			MemoryStream memoryStream = new MemoryStream();
			UnifiedContentSerializer unifiedContentSerializer = new UnifiedContentSerializer(memoryStream, contentManager.GetSharedStream(), list);
			this.Serialize(unifiedContentSerializer, bypassBodyTextTruncation);
			contentManager.GetSharedStream().Flush();
			unifiedContentSerializer.Commit();
			list = (contentManager.ContentCollection = unifiedContentSerializer.ContentCollection);
			FilteringRequest filteringRequest = new FilteringRequest(memoryStream, this.Id)
			{
				ContentCollection = list
			};
			filteringRequest.AddProperty("EnableUnifiedContent", contentManager.GetSharedStream().SharedName);
			return filteringRequest;
		}

		protected abstract void Serialize(UnifiedContentSerializer unifiedContentSerializer, bool bypassBodyTextTruncation = true);
	}
}

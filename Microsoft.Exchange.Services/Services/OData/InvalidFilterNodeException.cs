using System;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData
{
	internal class InvalidFilterNodeException : InvalidFilterException
	{
		public InvalidFilterNodeException(QueryNode queryNode) : base(CoreResources.ErrorInvalidFilterNode)
		{
			this.QueryNode = queryNode;
		}

		public QueryNode QueryNode { get; private set; }
	}
}

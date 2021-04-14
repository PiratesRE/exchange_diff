using System;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData
{
	internal class InvalidFilterFunctionException : InvalidFilterNodeException
	{
		public InvalidFilterFunctionException(QueryNode queryNode) : base(queryNode)
		{
		}
	}
}

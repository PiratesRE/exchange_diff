using System;

namespace Microsoft.Exchange.Assistants
{
	[Serializable]
	public class QueryResponse
	{
		public string ObjectClass { get; set; }

		public string Result { get; set; }

		public QueryResponse()
		{
		}

		public QueryResponse(string objectClass, string result)
		{
			this.ObjectClass = objectClass;
			this.Result = result;
		}

		public static QueryResponse CreateError(string message)
		{
			return new QueryResponse("Error", message);
		}
	}
}

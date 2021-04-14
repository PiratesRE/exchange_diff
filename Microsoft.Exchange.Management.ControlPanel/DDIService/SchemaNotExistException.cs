using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public class SchemaNotExistException : Exception
	{
		public SchemaNotExistException(string schema)
		{
			this.schema = schema;
		}

		public string Schema
		{
			get
			{
				return this.schema;
			}
		}

		private readonly string schema;
	}
}

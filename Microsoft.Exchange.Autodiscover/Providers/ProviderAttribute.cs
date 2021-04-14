using System;

namespace Microsoft.Exchange.Autodiscover.Providers
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	internal sealed class ProviderAttribute : Attribute
	{
		public ProviderAttribute(string guid, string requestSchema, string requestSchemaFile, string responseSchema, string responseSchemaFile)
		{
			this.guid = new Guid(guid);
			if (string.IsNullOrEmpty(requestSchema))
			{
				throw new ArgumentNullException("requestSchema");
			}
			this.requestSchema = requestSchema;
			if (string.IsNullOrEmpty(requestSchemaFile))
			{
				throw new ArgumentNullException("requestSchemaFile");
			}
			this.requestSchemaFile = requestSchemaFile;
			if (string.IsNullOrEmpty(responseSchema))
			{
				throw new ArgumentNullException("responseSchema");
			}
			this.responseSchema = responseSchema;
			if (string.IsNullOrEmpty(responseSchemaFile))
			{
				throw new ArgumentNullException("responseSchemaFile");
			}
			this.responseSchemaFile = responseSchemaFile;
		}

		public string Guid
		{
			get
			{
				return this.guid.ToString();
			}
		}

		public string RequestSchema
		{
			get
			{
				return this.requestSchema;
			}
		}

		public string ResponseSchema
		{
			get
			{
				return this.responseSchema;
			}
		}

		public string RequestSchemaFile
		{
			get
			{
				return this.requestSchemaFile;
			}
		}

		public string ResponseSchemaFile
		{
			get
			{
				return this.responseSchemaFile;
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		private Guid guid;

		private string requestSchema;

		private string requestSchemaFile;

		private string responseSchema;

		private string responseSchemaFile;

		private string description;
	}
}

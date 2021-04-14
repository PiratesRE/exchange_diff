using System;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.FullTextIndex
{
	public class FqlQuery
	{
		public FqlQuery()
		{
			this.valueBuilder = new StringBuilder();
			this.hashedValueBuilder = new StringBuilder();
		}

		public string TermLength { get; set; }

		public string Value
		{
			get
			{
				return this.valueBuilder.ToString();
			}
		}

		public string ScrubbedValue
		{
			get
			{
				return this.hashedValueBuilder.ToString();
			}
		}

		public override string ToString()
		{
			return this.valueBuilder.ToString();
		}

		public void Append(FqlQuery fqlString)
		{
			this.valueBuilder.Append(fqlString.valueBuilder);
			this.hashedValueBuilder.Append(fqlString.hashedValueBuilder);
		}

		public void Append(string value)
		{
			this.valueBuilder.Append(value);
			this.hashedValueBuilder.Append(value);
		}

		public void Append(char value)
		{
			this.valueBuilder.Append(value);
			this.hashedValueBuilder.Append(value);
		}

		public void AppendValue(string value, string valueReplacement)
		{
			this.valueBuilder.Append(value);
			this.hashedValueBuilder.Append(valueReplacement);
		}

		private StringBuilder valueBuilder;

		private StringBuilder hashedValueBuilder;
	}
}

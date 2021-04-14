using System;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	[Serializable]
	internal class SearchableProperty : SchemaMappingEntry
	{
		public SearchableProperty()
		{
		}

		public SearchableProperty(string sourceClassName, string sourcePropertyName)
		{
			this.sourceClassName = sourceClassName;
			this.sourcePropertyName = sourcePropertyName;
		}

		public string SourceClassName
		{
			get
			{
				return this.sourceClassName;
			}
			set
			{
				this.sourceClassName = value;
			}
		}

		public string SourcePropertyName
		{
			get
			{
				return this.sourcePropertyName;
			}
			set
			{
				this.sourcePropertyName = value;
			}
		}

		private string sourceClassName;

		private string sourcePropertyName;
	}
}

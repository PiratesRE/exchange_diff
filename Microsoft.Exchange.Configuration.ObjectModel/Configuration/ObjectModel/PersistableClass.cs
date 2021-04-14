using System;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	[Serializable]
	internal class PersistableClass : SchemaMappingEntry
	{
		public PersistableClass()
		{
		}

		public PersistableClass(string sourceClassName)
		{
			this.sourceClassName = sourceClassName;
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

		private string sourceClassName;
	}
}

using System;

namespace Microsoft.Exchange.Diagnostics
{
	public class CsvField
	{
		public CsvField(string name, Type type, bool isIndexed, Version buildAdded, NormalizeColumnDataMethod normalizeMethod) : this(name, type, isIndexed, normalizeMethod)
		{
			this.buildAdded = buildAdded;
		}

		public CsvField(string name, Type type, Version buildAdded) : this(name, type)
		{
			this.buildAdded = buildAdded;
		}

		public CsvField(string name, Type type, bool isMandatory, Version buildAdded) : this(name, type)
		{
			this.isMandatory = isMandatory;
			this.buildAdded = buildAdded;
		}

		public CsvField(string name, Type type, bool isIndexed, NormalizeColumnDataMethod normalizeMethod) : this(name, type)
		{
			this.isIndexed = isIndexed;
			this.normalizeMethod = normalizeMethod;
		}

		public CsvField(string name, Type type)
		{
			this.name = name;
			this.type = type;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public Type Type
		{
			get
			{
				return this.type;
			}
		}

		public bool IsIndexed
		{
			get
			{
				return this.isIndexed;
			}
		}

		public bool IsMandatory
		{
			get
			{
				return this.isMandatory;
			}
		}

		public Version BuildAdded
		{
			get
			{
				return this.buildAdded;
			}
		}

		public NormalizeColumnDataMethod NormalizeMethod
		{
			get
			{
				return this.normalizeMethod;
			}
		}

		private readonly string name;

		private readonly Type type;

		private readonly Version buildAdded;

		private readonly bool isIndexed;

		private readonly bool isMandatory;

		private readonly NormalizeColumnDataMethod normalizeMethod;
	}
}

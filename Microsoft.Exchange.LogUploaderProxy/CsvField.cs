using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public class CsvField
	{
		public CsvField(string name, Type type, bool isIndexed, Version buildAdded, NormalizeColumnDataMethod normalizeMethod)
		{
			NormalizeColumnDataMethod normalizeMethod2 = (string c) => normalizeMethod(c);
			this.csvFieldImpl = new CsvField(name, type, isIndexed, buildAdded, normalizeMethod2);
		}

		public CsvField(string name, Type type, Version buildAdded)
		{
			this.csvFieldImpl = new CsvField(name, type, buildAdded);
		}

		public CsvField(string name, Type type, bool isMandatory, Version buildAdded)
		{
			this.csvFieldImpl = new CsvField(name, type, isMandatory, buildAdded);
		}

		public CsvField(string name, Type type, bool isIndexed, NormalizeColumnDataMethod normalizeMethod)
		{
			NormalizeColumnDataMethod normalizeMethod2 = (string c) => normalizeMethod(c);
			this.csvFieldImpl = new CsvField(name, type, isIndexed, normalizeMethod2);
		}

		public CsvField(string name, Type type)
		{
			this.csvFieldImpl = new CsvField(name, type);
		}

		public string Name
		{
			get
			{
				return this.csvFieldImpl.Name;
			}
		}

		public Type Type
		{
			get
			{
				return this.csvFieldImpl.Type;
			}
		}

		public bool IsIndexed
		{
			get
			{
				return this.csvFieldImpl.IsIndexed;
			}
		}

		public bool IsMandatory
		{
			get
			{
				return this.csvFieldImpl.IsMandatory;
			}
		}

		public Version BuildAdded
		{
			get
			{
				return this.csvFieldImpl.BuildAdded;
			}
		}

		public NormalizeColumnDataMethod NormalizeMethod
		{
			get
			{
				return (string c) => this.csvFieldImpl.NormalizeMethod(c);
			}
		}

		internal CsvField CsvFieldImpl
		{
			get
			{
				return this.csvFieldImpl;
			}
		}

		private CsvField csvFieldImpl;
	}
}

using System;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	public class ProcessorData : ICloneable
	{
		public ProcessorData(ProcessorData copy)
		{
			this.CachingEnabled = copy.CachingEnabled;
			this.CaseSensitivityType = copy.CaseSensitivityType;
			this.Coefficient = copy.Coefficient;
			this.ExpectedResult = copy.ExpectedResult;
			this.Keywords = copy.Keywords;
			this.Name = copy.Name;
			this.Precondition = copy.Precondition;
			this.ProcessorID = copy.ProcessorID;
			this.ProcessorType = copy.ProcessorType;
			this.Target = copy.Target;
			this.Value = copy.Value;
			this.WordBoundary = copy.WordBoundary;
		}

		public ProcessorData()
		{
		}

		public long ProcessorID { get; set; }

		public ProcessorType ProcessorType { get; set; }

		public string Name { get; set; }

		public int? ExpectedResult { get; set; }

		public string WordBoundary { get; set; }

		public long? Precondition { get; set; }

		public string Value { get; set; }

		public string[] Keywords { get; set; }

		public string Target { get; set; }

		public byte? CaseSensitivityType { get; set; }

		public bool? CachingEnabled { get; set; }

		public double? Coefficient { get; set; }

		public object Clone()
		{
			return new ProcessorData(this);
		}
	}
}

using System;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps
{
	public class FailureCategory
	{
		public string IDRegex
		{
			get
			{
				return this.idRegex;
			}
			set
			{
				this.idRegex = value;
				if (!string.IsNullOrEmpty(this.idRegex))
				{
					this.fcReg = new Regex(this.idRegex);
				}
			}
		}

		public string Description { get; set; }

		[XmlArray]
		[XmlArrayItem("Instance")]
		public FailureInstance[] Instances { get; set; }

		public bool IsMatch(string errorMessage)
		{
			return this.fcReg != null && this.fcReg.IsMatch(errorMessage);
		}

		private Regex fcReg;

		private string idRegex;
	}
}

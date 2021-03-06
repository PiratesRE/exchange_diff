using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class Path
	{
		[XmlAttribute(AttributeName = "select")]
		public string Select
		{
			get
			{
				return this.selectField;
			}
			set
			{
				this.selectField = value;
			}
		}

		private string selectField;
	}
}

using System;
using System.IO;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps
{
	public class FailureCategoryList
	{
		public FailureCategory[] FailureCategories { get; set; }

		public static FailureCategoryList LoadFrom(string filePath)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(FailureCategoryList));
			TextReader textReader = new StreamReader(filePath);
			return (FailureCategoryList)xmlSerializer.Deserialize(textReader);
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class DiscoveredDataClassification : DataClassification
	{
		private DiscoveredDataClassification()
		{
		}

		public DiscoveredDataClassification(string id, string name, uint recommendedMinimumConfidence, IEnumerable<DataClassificationSourceInfo> sourceInfos = null) : base(id)
		{
			this.ClassificationName = name;
			this.RecommendedMinimumConfidence = recommendedMinimumConfidence;
			if (sourceInfos != null)
			{
				this.MatchingSourceInfos = new List<DataClassificationSourceInfo>(sourceInfos);
				return;
			}
			this.MatchingSourceInfos = new List<DataClassificationSourceInfo>();
		}

		public string ClassificationName { get; set; }

		public int TotalCount
		{
			get
			{
				if (!this.MatchingSourceInfos.Any<DataClassificationSourceInfo>())
				{
					return 0;
				}
				return this.MatchingSourceInfos.Sum((DataClassificationSourceInfo matchingSourceInfo) => matchingSourceInfo.Count);
			}
		}

		public uint MaxConfidenceLevel
		{
			get
			{
				if (!this.MatchingSourceInfos.Any<DataClassificationSourceInfo>())
				{
					return 0U;
				}
				return this.MatchingSourceInfos.Max((DataClassificationSourceInfo matchingSourceInfo) => matchingSourceInfo.ConfidenceLevel);
			}
		}

		public uint RecommendedMinimumConfidence { get; set; }

		public List<DataClassificationSourceInfo> MatchingSourceInfos { get; set; }

		public static string SerializeToXml(IEnumerable<DiscoveredDataClassification> discoveredDataClassifications)
		{
			if (discoveredDataClassifications != null)
			{
				List<DiscoveredDataClassification> list = new List<DiscoveredDataClassification>();
				foreach (DiscoveredDataClassification item in discoveredDataClassifications)
				{
					list.Add(item);
				}
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<DiscoveredDataClassification>));
				using (StringWriter stringWriter = new StringWriter())
				{
					xmlSerializer.Serialize(stringWriter, list);
					return stringWriter.ToString();
				}
			}
			return null;
		}

		public static IEnumerable<DiscoveredDataClassification> DeserializeFromXml(string xml)
		{
			if (!string.IsNullOrEmpty(xml))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<DiscoveredDataClassification>));
				using (StringReader stringReader = new StringReader(xml))
				{
					return (IEnumerable<DiscoveredDataClassification>)xmlSerializer.Deserialize(stringReader);
				}
			}
			return null;
		}

		public static string ToString(IEnumerable<DiscoveredDataClassification> discoveredDataClassifications)
		{
			if (discoveredDataClassifications == null)
			{
				return string.Empty;
			}
			return string.Join(";", from discoveredDataClassification in discoveredDataClassifications
			select discoveredDataClassification.ToString());
		}

		public override string ToString()
		{
			string format = "Name:{0}/Id:{1}/Count:{2}/Confidence:{3}/MatchingSourceInfos:{4}.";
			object[] array = new object[5];
			array[0] = this.ClassificationName;
			array[1] = base.Id;
			array[2] = this.TotalCount;
			array[3] = this.MaxConfidenceLevel;
			array[4] = string.Join(";", from matchingSourceInfo in this.MatchingSourceInfos
			select matchingSourceInfo.ToString());
			return string.Format(format, array);
		}

		public IEnumerable<DataClassificationMatchLocation> Locations
		{
			get
			{
				List<DataClassificationMatchLocation> list = new List<DataClassificationMatchLocation>();
				foreach (DataClassificationSourceInfo dataClassificationSourceInfo in this.MatchingSourceInfos)
				{
					list.AddRange(dataClassificationSourceInfo.Locations);
				}
				return list;
			}
		}

		private const string ToStringFormat = "Name:{0}/Id:{1}/Count:{2}/Confidence:{3}/MatchingSourceInfos:{4}.";

		public static readonly string RecommendedWeightKey = "RecommendedWeight";
	}
}

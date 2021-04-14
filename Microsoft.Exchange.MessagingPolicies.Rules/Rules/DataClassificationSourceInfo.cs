using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class DataClassificationSourceInfo
	{
		public int SourceId { get; set; }

		public string SourceName { get; set; }

		public string TopLevelSourceName { get; set; }

		public int Count { get; set; }

		public uint ConfidenceLevel { get; set; }

		[XmlIgnore]
		public List<DataClassificationMatchLocation> Locations { get; set; }

		private DataClassificationSourceInfo()
		{
			this.Locations = new List<DataClassificationMatchLocation>();
		}

		public DataClassificationSourceInfo(int sourceId, string sourceName, string topLevelSourceName, int count, uint confidence)
		{
			if (string.IsNullOrEmpty(sourceName))
			{
				throw new ArgumentNullException("sourceName");
			}
			if (string.IsNullOrEmpty(topLevelSourceName))
			{
				throw new ArgumentNullException("topLevelSourceName");
			}
			this.SourceId = sourceId;
			this.SourceName = sourceName;
			this.TopLevelSourceName = topLevelSourceName;
			this.Count = count;
			this.ConfidenceLevel = confidence;
			this.Locations = new List<DataClassificationMatchLocation>();
		}

		public static string ToString(IEnumerable<DataClassificationSourceInfo> dataClassificationSourceInfos)
		{
			if (dataClassificationSourceInfos == null)
			{
				return string.Empty;
			}
			return string.Join(";", from dataClassificationSourceInfo in dataClassificationSourceInfos
			select dataClassificationSourceInfo.ToString());
		}

		public override string ToString()
		{
			return string.Format("Name:{0}/Id:{1}/TopLevelName:{2}/Count:{3}/Confidence:{4}.", new object[]
			{
				this.SourceName,
				this.SourceId,
				this.TopLevelSourceName,
				this.Count,
				this.ConfidenceLevel
			});
		}

		private const string ToStringFormat = "Name:{0}/Id:{1}/TopLevelName:{2}/Count:{3}/Confidence:{4}.";
	}
}

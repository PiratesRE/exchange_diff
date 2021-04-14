using System;
using System.Collections.Generic;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ComplianceSearchData
	{
		static ComplianceSearchData()
		{
			ComplianceSearchData.description.ComplianceStructureId = 9;
			ComplianceSearchData.description.RegisterBytePropertyGetterAndSetter(0, (ComplianceSearchData item) => (byte)item.Version, delegate(ComplianceSearchData item, byte value)
			{
				item.Version = (ComplianceSearchData.ComplianceSearchObjectVersion)value;
			});
			ComplianceSearchData.description.RegisterBytePropertyGetterAndSetter(1, (ComplianceSearchData item) => (byte)item.SearchType, delegate(ComplianceSearchData item, byte value)
			{
				item.SearchType = (ComplianceSearch.ComplianceSearchType)value;
			});
			ComplianceSearchData.description.RegisterBytePropertyGetterAndSetter(2, (ComplianceSearchData item) => (byte)item.LogLevel, delegate(ComplianceSearchData item, byte value)
			{
				item.LogLevel = (ComplianceJobLogLevel)value;
			});
			ComplianceSearchData.description.RegisterStringPropertyGetterAndSetter(0, (ComplianceSearchData item) => item.Language, delegate(ComplianceSearchData item, string value)
			{
				item.Language = value;
			});
			ComplianceSearchData.description.RegisterStringPropertyGetterAndSetter(1, (ComplianceSearchData item) => item.KeywordQuery, delegate(ComplianceSearchData item, string value)
			{
				item.KeywordQuery = value;
			});
			ComplianceSearchData.description.RegisterStringPropertyGetterAndSetter(2, (ComplianceSearchData item) => item.SearchOptions, delegate(ComplianceSearchData item, string value)
			{
				item.SearchOptions = value;
			});
			ComplianceSearchData.description.RegisterCollectionPropertyAccessors(0, () => CollectionItemType.String, (ComplianceSearchData item) => item.StatusMailRecipients.Length, (ComplianceSearchData item, int index) => item.StatusMailRecipients[index], delegate(ComplianceSearchData item, object obj, int index)
			{
				item.StatusMailRecipients[index] = (string)obj;
			}, delegate(ComplianceSearchData item, int count)
			{
				item.StatusMailRecipients = new string[count];
			});
			ComplianceSearchData.description.RegisterCollectionPropertyAccessors(1, () => CollectionItemType.Blob, (ComplianceSearchData item) => item.SearchConditions.Count, (ComplianceSearchData item, int index) => item.SearchConditions[index], delegate(ComplianceSearchData item, object obj, int index)
			{
				item.SearchConditions.Add((byte[])obj);
			}, delegate(ComplianceSearchData item, int count)
			{
				item.SearchConditions = new List<byte[]>();
			});
		}

		public ComplianceSearchData()
		{
			this.Version = ComplianceSearchData.ComplianceSearchObjectVersion.Version1;
		}

		public static ComplianceSerializationDescription<ComplianceSearchData> Description
		{
			get
			{
				return ComplianceSearchData.description;
			}
		}

		public ComplianceSearchData.ComplianceSearchObjectVersion Version { get; set; }

		public string KeywordQuery { get; set; }

		public List<byte[]> SearchConditions { get; set; }

		public ComplianceSearch.ComplianceSearchType SearchType { get; set; }

		public string Language { get; set; }

		public string[] StatusMailRecipients { get; set; }

		public ComplianceJobLogLevel LogLevel { get; set; }

		public string SearchOptions { get; set; }

		private static ComplianceSerializationDescription<ComplianceSearchData> description = new ComplianceSerializationDescription<ComplianceSearchData>();

		internal enum ComplianceSearchObjectVersion : byte
		{
			VersionUnknown,
			Version1
		}
	}
}

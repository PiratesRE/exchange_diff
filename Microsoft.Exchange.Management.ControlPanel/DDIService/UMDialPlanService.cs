using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.DDIService
{
	public class UMDialPlanService : DDICodeBehind
	{
		public static void GetListPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			bool flag = !DDIHelper.IsEmptyValue(inputRow["NoSipDialPlans"]) && (bool)inputRow["NoSipDialPlans"];
			Identity identity = inputRow["NoSipDialPlansExcludeId"] as Identity;
			if (flag)
			{
				List<DataRow> list = new List<DataRow>();
				foreach (object obj in dataTable.Rows)
				{
					DataRow dataRow = (DataRow)obj;
					if ((UMUriType)dataRow["URIType"] == UMUriType.SipName && !((ADObjectId)dataRow["Identity"]).ToIdentity().Equals(identity))
					{
						list.Add(dataRow);
					}
				}
				list.ForEach(delegate(DataRow x)
				{
					x.Delete();
				});
			}
		}

		public static List<DropDownItemData> GetAvailableUmLanguages()
		{
			MultiValuedProperty<UMLanguage> multiValuedProperty = Utils.ComputeUnionOfUmServerLanguages();
			List<DropDownItemData> list = new List<DropDownItemData>(multiValuedProperty.Count);
			bool isRtl = RtlUtil.IsRtl;
			list.AddRange(multiValuedProperty.ConvertAll((UMLanguage x) => new DropDownItemData(RtlUtil.ConvertToDecodedBidiString(x.Culture.NativeName, isRtl), x.Culture.LCID.ToString())));
			return list;
		}

		public static object GetInfoAnnouncementEnabled(string infoAnnouncementFilename, bool isInfoAnnouncementInterruptible)
		{
			return string.IsNullOrEmpty(infoAnnouncementFilename) ? InfoAnnouncementEnabledEnum.False : (isInfoAnnouncementInterruptible ? InfoAnnouncementEnabledEnum.True : InfoAnnouncementEnabledEnum.Uninterruptible);
		}

		public static string[] UMDialingRuleEntryToTask(object value)
		{
			if (!DDIHelper.IsEmptyValue(value))
			{
				return UMDialingRuleEntry.GetStringArray(Array.ConvertAll<object, UMDialingRuleEntry>((object[])value, (object x) => (UMDialingRuleEntry)x));
			}
			return null;
		}
	}
}

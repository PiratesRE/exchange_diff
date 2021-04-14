using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class FFOMigrationStatusService
	{
		public static void MigrationReportPostAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			dataRow["ShowError"] = false;
			dataRow["ProblemsMigrating"] = true;
			try
			{
				string text = dataRow["Report"].ToStringWithNull();
				XDocument xdocument = XDocument.Parse(text);
				IEnumerable<XElement> source = from y in xdocument.Descendants("Step")
				select y;
				DateTime dateTime = Convert.ToDateTime(source.First<XElement>().Attribute("StartTime").Value.ToString());
				DateTime dateTime2 = Convert.ToDateTime(source.Last<XElement>().Attribute("EndTime").Value.ToString());
				dataRow["StartedOn"] = dateTime.ToString("g");
				dataRow["CompletedOn"] = dateTime2.ToString("g");
				IEnumerable<XElement> source2 = from x in xdocument.Descendants("Step")
				where Convert.ToInt32(x.Attribute("ToState").Value) >= 100
				select x;
				IEnumerable<FFOMigrationStatusService.MigrationStep> enumerable = from e in source2.Elements("Prop")
				where e.Attribute("PropID").Value == "10003"
				select new FFOMigrationStatusService.MigrationStep(e.Parent.Attribute("StepName").Value, e.Parent.Attribute("ToState").Value, e.Attribute("Value").Value);
				if (enumerable.Count<FFOMigrationStatusService.MigrationStep>() >= 1)
				{
					FFOMigrationStatusService.PopulateStepErrors(enumerable, dataRow, "SpamQuarantineMigrationStep", "SpamQuarantineGroupData", "SpamQuarantineGroupVisible");
					FFOMigrationStatusService.PopulateStepErrors(enumerable, dataRow, "AntispamMigrationStep", "AntispamGroupData", "AntispamGroupVisible");
					FFOMigrationStatusService.PopulateStepErrors(enumerable, dataRow, "AntimalwareMigrationStep", "AntimalwareGroupData", "AntimalwareGroupVisible");
					FFOMigrationStatusService.PopulateStepErrors(enumerable, dataRow, "PolicyMigrationStep", "PolicyRulesGroupData", "PolicyRulesGroupVisible");
					FFOMigrationStatusService.PopulateStepErrors(enumerable, dataRow, "ConnectorMigrationStep", "ConnectorGroupData", "ConnectorGroupVisible");
				}
				else
				{
					dataRow["ProblemsMigrating"] = false;
				}
			}
			catch (Exception exception)
			{
				dataRow["ShowError"] = true;
				DDIHelper.Trace("Error processing Migration Report: {0}", new object[]
				{
					exception.GetTraceFormatter()
				});
			}
		}

		private static void PopulateStepErrors(IEnumerable<FFOMigrationStatusService.MigrationStep> steps, DataRow row, string groupName, string dataField, string visibleField)
		{
			IEnumerable<MigrationReportGroupDetails> source = from s in steps
			where s.StepName == groupName
			select new MigrationReportGroupDetails
			{
				Data = s.LocalizedString
			};
			row[dataField] = source.ToArray<MigrationReportGroupDetails>();
			row[visibleField] = (source.FirstOrDefault<MigrationReportGroupDetails>() != null);
		}

		private class MigrationStep
		{
			public string StepName { get; private set; }

			public string ToState { get; private set; }

			public string LocalizedString { get; private set; }

			public MigrationStep(string stepname, string tostate, string value)
			{
				this.StepName = stepname;
				this.ToState = tostate;
				string serializedString = string.Empty;
				int num;
				if ((num = value.IndexOf("<?xml")) != -1)
				{
					value.IndexOf("</LocalizedString>");
					serializedString = value.Substring(num, value.Length - num - 3);
				}
				LocalizedString localizedString;
				if (LocalizedStringSerializer.TryDeserialize(serializedString, out localizedString))
				{
					this.LocalizedString = localizedString.ToString();
					return;
				}
				this.LocalizedString = string.Empty;
			}
		}
	}
}

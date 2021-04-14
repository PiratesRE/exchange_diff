using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.MessageSecurity.MessageClassifications;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class ComplianceContextMenu : ContextMenu
	{
		public ComplianceContextMenu(UserContext userContext, string id) : base("divCmplM", userContext)
		{
			this.id = id;
			this.shouldScroll = true;
		}

		protected override void RenderExpandoData(TextWriter output)
		{
			output.Write(" sCA=\"");
			output.Write(this.id);
			output.Write("\"");
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			List<object> list = new List<object>();
			bool flag = false;
			bool flag2 = false;
			if (base.UserContext.IsIrmEnabled)
			{
				foreach (RmsTemplate rmsTemplate in this.userContext.ComplianceReader.RmsTemplateReader.GetRmsTemplates())
				{
					flag = true;
					if (rmsTemplate == RmsTemplate.InternetConfidential)
					{
						flag2 = true;
					}
					else if (rmsTemplate != RmsTemplate.DoNotForward)
					{
						list.Add(rmsTemplate);
					}
				}
				if (base.UserContext.ComplianceReader.RmsTemplateReader.TemplateAcquisitionFailed)
				{
					string additionalAttributes = " iCType=\"-1\"";
					base.RenderMenuItem(output, LocalizedStrings.GetNonEncoded(440044585), ThemeFileId.Clear, "divCPLA0", "0", false, additionalAttributes, null, null, null, null, false);
					return;
				}
			}
			foreach (ClassificationSummary item in base.UserContext.ComplianceReader.MessageClassificationReader.GetClassificationsForLocale(base.UserContext.UserCulture))
			{
				list.Add(item);
			}
			IComparer<object> comparer = new ComplianceContextMenu.DisplayNameComparer(base.UserContext.UserCulture);
			list.Sort(comparer);
			base.RenderMenuItem(output, 440044585, ThemeFileId.Clear, "divCPLA0", "0");
			if (flag)
			{
				this.RenderCompliance(output, RmsTemplate.DoNotForward);
				if (flag2)
				{
					this.RenderCompliance(output, RmsTemplate.InternetConfidential);
				}
				ContextMenu.RenderMenuDivider(output, "divCPLA_0");
			}
			for (int i = 0; i < list.Count; i++)
			{
				this.RenderCompliance(output, list[i]);
			}
		}

		private void RenderCompliance(TextWriter output, object compliance)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			ComplianceType complianceType = ComplianceType.Unknown;
			ClassificationSummary classificationSummary = compliance as ClassificationSummary;
			if (classificationSummary != null)
			{
				text = classificationSummary.ClassificationID.ToString();
				text2 = classificationSummary.DisplayName;
				complianceType = ComplianceType.MessageClassification;
			}
			RmsTemplate rmsTemplate = compliance as RmsTemplate;
			if (rmsTemplate != null)
			{
				text = rmsTemplate.Id.ToString();
				text2 = rmsTemplate.GetName(base.UserContext.UserCulture);
				complianceType = ComplianceType.RmsTemplate;
			}
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				string additionalAttributes = " iCType=\"" + (uint)complianceType + "\"";
				base.RenderMenuItem(output, text2, ThemeFileId.Clear, "divCPLA" + text, text, false, additionalAttributes, null, null, null, null, false);
			}
		}

		private const string ComplianceAction = "divCPLA";

		internal const string NoRestrictionComplianceId = "0";

		private string id;

		private class DisplayNameComparer : IComparer<object>
		{
			public DisplayNameComparer(CultureInfo locale)
			{
				this.locale = locale;
			}

			public int Compare(object object1, object object2)
			{
				return string.Compare(this.GetDisplayName(object1), this.GetDisplayName(object2), true, this.locale);
			}

			private string GetDisplayName(object obj)
			{
				if (obj is ClassificationSummary)
				{
					return ((ClassificationSummary)obj).DisplayName;
				}
				if (obj is RmsTemplate)
				{
					return ((RmsTemplate)obj).GetName(this.locale);
				}
				return string.Empty;
			}

			private readonly CultureInfo locale;
		}
	}
}

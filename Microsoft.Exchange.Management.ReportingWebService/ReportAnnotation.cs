using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal class ReportAnnotation : IReportAnnotation
	{
		private ReportAnnotation()
		{
		}

		public string ReportTitle
		{
			get
			{
				return this.reportTitle.GetLocalizedString();
			}
		}

		public IEnumerable<string> Xaxis
		{
			get
			{
				foreach (ReportAnnotation.StringInfo xaixs in this.xaxises)
				{
					yield return xaixs.GetLocalizedString();
				}
				yield break;
			}
		}

		public IEnumerable<string> Yaxis
		{
			get
			{
				foreach (ReportAnnotation.StringInfo yaixs in this.yaxises)
				{
					yield return yaixs.GetLocalizedString();
				}
				yield break;
			}
		}

		public static IReportAnnotation Load(XmlNode annotationNode)
		{
			ReportAnnotation reportAnnotation = new ReportAnnotation();
			reportAnnotation.reportTitle = ReportAnnotation.GetStringInfo(ReportingSchema.SelectSingleNode(annotationNode, "ReportTitle"));
			ReportingSchema.CheckCondition(reportAnnotation.reportTitle != null && !string.IsNullOrEmpty(reportAnnotation.reportTitle.StringId), "Report title isn't present.");
			reportAnnotation.xaxises = ReportAnnotation.LoadSeries(annotationNode, "XAxis");
			reportAnnotation.yaxises = ReportAnnotation.LoadSeries(annotationNode, "YAxis");
			ReportingSchema.CheckCondition(reportAnnotation.yaxises != null && reportAnnotation.yaxises.Count > 0, "Report Y-Axis doesn't present.");
			return reportAnnotation;
		}

		private static List<ReportAnnotation.StringInfo> LoadSeries(XmlNode parentNode, string xpath)
		{
			List<ReportAnnotation.StringInfo> list = new List<ReportAnnotation.StringInfo>();
			using (XmlNodeList xmlNodeList = parentNode.SelectNodes(xpath))
			{
				foreach (object obj in xmlNodeList)
				{
					XmlNode node = (XmlNode)obj;
					list.Add(ReportAnnotation.GetStringInfo(node));
				}
			}
			return list;
		}

		private static ReportAnnotation.StringInfo GetStringInfo(XmlNode node)
		{
			if (node == null)
			{
				return null;
			}
			string value = node.Attributes["Loc"].Value.Trim();
			bool localized = false;
			if (!string.IsNullOrEmpty(value))
			{
				bool.TryParse(value, out localized);
			}
			return new ReportAnnotation.StringInfo(node.InnerText, localized);
		}

		private const string LocAttribute = "Loc";

		private const string ReportTitleNode = "ReportTitle";

		private const string XAxisNode = "XAxis";

		private const string YAxisNode = "YAxis";

		private ReportAnnotation.StringInfo reportTitle;

		private List<ReportAnnotation.StringInfo> xaxises;

		private List<ReportAnnotation.StringInfo> yaxises;

		private class StringInfo
		{
			public StringInfo(string stringId, bool localized)
			{
				this.StringId = stringId;
				this.localized = false;
				AnnotationStrings.IDs annotationStringId;
				if (localized && Enum.TryParse<AnnotationStrings.IDs>(this.StringId, true, out annotationStringId))
				{
					this.AnnotationStringId = annotationStringId;
					this.localized = true;
				}
			}

			public string StringId { get; private set; }

			public AnnotationStrings.IDs AnnotationStringId { get; private set; }

			public string GetLocalizedString()
			{
				if (this.localized)
				{
					return AnnotationStrings.GetLocalizedString(this.AnnotationStringId);
				}
				return this.StringId;
			}

			private readonly bool localized;
		}
	}
}

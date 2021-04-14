using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class PageCookieTvp : DataTable
	{
		public PageCookieTvp()
		{
			this.InitializeSchema();
		}

		internal static PageCookieTvp Deserialize(string pageCookieXml)
		{
			PageCookieTvp pageCookieTvp = new PageCookieTvp();
			if (!string.IsNullOrWhiteSpace(pageCookieXml))
			{
				using (XmlReader xmlReader = XmlReader.Create(new StringReader(pageCookieXml), PageCookieTvp.xrs))
				{
					while (xmlReader.Read())
					{
						if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.Name != "row")
						{
							throw new InvalidOperationException("XML data is invalid. Unable to deserialize page cookie.");
						}
						DataRow dataRow = pageCookieTvp.NewRow();
						string attribute = xmlReader.GetAttribute(PageCookieTvp.DatabaseNameCol);
						if (!string.IsNullOrWhiteSpace(attribute))
						{
							dataRow[PageCookieTvp.DatabaseNameCol] = attribute;
						}
						string attribute2 = xmlReader.GetAttribute(PageCookieTvp.LastChangedDatetimeCol);
						if (!string.IsNullOrWhiteSpace(attribute2))
						{
							dataRow[PageCookieTvp.LastChangedDatetimeCol] = attribute2;
						}
						string attribute3 = xmlReader.GetAttribute(PageCookieTvp.LastEntityIdCol);
						if (!string.IsNullOrWhiteSpace(attribute3))
						{
							dataRow[PageCookieTvp.LastEntityIdCol] = attribute3;
						}
						pageCookieTvp.Rows.Add(dataRow);
					}
				}
			}
			return pageCookieTvp;
		}

		internal static IEnumerable<DateTime> GetDateTimes(string pageCookieXml)
		{
			if (!string.IsNullOrWhiteSpace(pageCookieXml))
			{
				using (XmlReader reader = XmlReader.Create(new StringReader(pageCookieXml), PageCookieTvp.xrs))
				{
					while (reader.Read())
					{
						if (reader.NodeType != XmlNodeType.Element || reader.Name != "row")
						{
							throw new InvalidOperationException("XML data is invalid. Unable to deserialize page cookie.");
						}
						string lastChangedDatetime = reader.GetAttribute(PageCookieTvp.LastChangedDatetimeCol);
						if (!string.IsNullOrWhiteSpace(lastChangedDatetime))
						{
							yield return DateTime.Parse(lastChangedDatetime);
						}
					}
				}
			}
			yield break;
		}

		private void InitializeSchema()
		{
			base.Columns.Add(new DataColumn(PageCookieTvp.DatabaseNameCol, typeof(string)));
			base.Columns.Add(new DataColumn(PageCookieTvp.LastChangedDatetimeCol, typeof(DateTime)));
			base.Columns.Add(new DataColumn(PageCookieTvp.LastEntityIdCol, typeof(Guid)));
		}

		internal static string CreatePageCookie(string[] dbNames, DateTime dateTime)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in dbNames)
			{
				XElement xelement = new XElement("row");
				xelement.SetAttributeValue(PageCookieTvp.LastChangedDatetimeCol, dateTime);
				xelement.SetAttributeValue(PageCookieTvp.DatabaseNameCol, value);
				xelement.SetAttributeValue(PageCookieTvp.LastEntityIdCol, Guid.Empty);
				stringBuilder.Append(xelement);
			}
			return stringBuilder.ToString();
		}

		private static readonly string DatabaseNameCol = "nvc_DatabaseName";

		private static readonly string LastChangedDatetimeCol = "dt_LastChangedDatetime";

		private static readonly string LastEntityIdCol = "id_LastEntityId";

		private static readonly XmlReaderSettings xrs = new XmlReaderSettings
		{
			ConformanceLevel = ConformanceLevel.Fragment
		};
	}
}

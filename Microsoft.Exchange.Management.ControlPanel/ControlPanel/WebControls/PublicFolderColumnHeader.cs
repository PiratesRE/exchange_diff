using System;
using System.Web;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class PublicFolderColumnHeader : ColumnHeader
	{
		public PublicFolderColumnHeader()
		{
			this.AllowHTML = true;
		}

		public override string ToJavaScript()
		{
			return string.Format("new PublicFolderColumnHeader(\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",{8})", new object[]
			{
				base.Name,
				base.SortExpression,
				base.FormatString,
				base.TextAlign.ToJavaScript(),
				HttpUtility.JavaScriptStringEncode(base.EmptyText),
				base.Text,
				this.Width.Value,
				this.Width.Type.ToJavaScript(),
				base.Features
			});
		}
	}
}

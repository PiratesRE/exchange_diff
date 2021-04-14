using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class ColumnHeader
	{
		public ColumnHeader()
		{
			this.IsSortable = true;
			this.EnableColumnResize = true;
			this.EnableColumnSelect = true;
			this.EnableExport = true;
		}

		[DefaultValue(false)]
		public virtual bool AllowHTML { get; set; }

		public string CssClass { get; set; }

		[DefaultValue(true)]
		public bool IsSortable { get; set; }

		[DefaultValue(false)]
		public bool DefaultOff { get; set; }

		[DefaultValue(null)]
		public string EmptyText { get; set; }

		[DefaultValue(null)]
		public string Name { get; set; }

		public string SortExpression
		{
			get
			{
				if (!this.IsSortable)
				{
					return string.Empty;
				}
				if (!string.IsNullOrEmpty(this.sortExpression))
				{
					return this.sortExpression;
				}
				return this.Name;
			}
			set
			{
				this.sortExpression = value;
			}
		}

		[Localizable(true)]
		[DefaultValue(null)]
		public string Text { get; set; }

		[DefaultValue("")]
		public virtual Unit Width { get; set; }

		[DefaultValue(null)]
		public string FormatString { get; set; }

		[DefaultValue(true)]
		public bool EnableExport { get; set; }

		[DefaultValue(true)]
		public bool EnableColumnSelect { get; set; }

		[DefaultValue(true)]
		public bool EnableColumnResize { get; set; }

		public int Features
		{
			get
			{
				ColumnHeaderFlags columnHeaderFlags = (ColumnHeaderFlags)0;
				if (this.EnableExport)
				{
					columnHeaderFlags |= ColumnHeaderFlags.EnableExport;
				}
				if (this.EnableColumnSelect)
				{
					columnHeaderFlags |= ColumnHeaderFlags.EnableColumnSelect;
				}
				if (this.EnableColumnResize)
				{
					columnHeaderFlags |= ColumnHeaderFlags.EnableColumnResize;
				}
				if (this.DefaultOff)
				{
					columnHeaderFlags |= ColumnHeaderFlags.Defaultoff;
				}
				if (this.AllowHTML)
				{
					columnHeaderFlags |= ColumnHeaderFlags.AllowHTML;
				}
				return (int)columnHeaderFlags;
			}
		}

		public HorizontalAlign TextAlign
		{
			get
			{
				return this.textAlign;
			}
			set
			{
				this.textAlign = RtlUtil.GetHorizontalAlign(value);
			}
		}

		public string Description { get; set; }

		public string Role { get; set; }

		[DefaultValue("")]
		protected virtual string SpanCssClass { get; set; }

		public virtual string ToJavaScript()
		{
			return string.Format("new ColumnHeader(\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",{6},\"{7}\",{8})", new object[]
			{
				this.Name,
				this.SortExpression,
				this.FormatString,
				this.TextAlign.ToJavaScript(),
				HttpUtility.JavaScriptStringEncode(this.EmptyText),
				HttpUtility.JavaScriptStringEncode(this.Text),
				(this.Width == Unit.Empty) ? "\"auto\"" : this.Width.Value.ToString(),
				(this.Width == Unit.Empty) ? string.Empty : this.Width.Type.ToJavaScript(),
				this.Features
			});
		}

		private string sortExpression = string.Empty;

		private HorizontalAlign textAlign = RtlUtil.GetHorizontalAlign(HorizontalAlign.Left);
	}
}

using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal abstract class DropDownCombo
	{
		protected DropDownCombo(string id, bool showOnValueMouseDown)
		{
			this.id = id;
			this.showOnValueMouseDown = showOnValueMouseDown;
			this.sessionContext = OwaContext.Current.SessionContext;
		}

		protected DropDownCombo(string id) : this(id, true)
		{
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		public string Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		public string ClassName
		{
			get
			{
				return this.className;
			}
			set
			{
				this.className = value;
			}
		}

		public void Render(TextWriter writer)
		{
			this.Render(writer, !this.enabled);
		}

		public void Render(TextWriter writer, bool disabled)
		{
			this.enabled = !disabled;
			writer.Write("<div id=\"");
			writer.Write(this.id);
			writer.Write("\"");
			writer.Write(" class=\"");
			if (this.ClassName != null)
			{
				writer.Write("cmb ");
				writer.Write(this.className);
			}
			else
			{
				writer.Write("cmb");
			}
			writer.Write("\" ");
			this.RenderExpandoData(writer);
			writer.Write(">");
			this.sessionContext.RenderThemeImage(writer, ThemeFileId.DownButton2, null, new object[]
			{
				"id=\"divCmbDd\"",
				this.enabled ? "tabIndex=\"0\"" : string.Empty
			});
			writer.Write("<div class=\"cmbVis\"><div class=\"cmbCont\">");
			writer.Write("<img class=\"cmbVASp\">");
			writer.Write("<span id=\"spanCmbSel\">");
			this.RenderSelectedValue(writer);
			writer.Write("</span>");
			writer.Write("</div></div>");
			this.RenderDropControl(writer);
			writer.Write("</div>");
		}

		protected virtual void RenderExpandoData(TextWriter writer)
		{
			writer.Write(" fEnbld=");
			writer.Write(this.enabled ? "1" : "0");
			if (this.showOnValueMouseDown)
			{
				writer.Write(" fd=1 style=\"cursor:default;\"");
			}
		}

		protected abstract void RenderDropControl(TextWriter writer);

		protected abstract void RenderSelectedValue(TextWriter writer);

		private string id;

		private bool enabled = true;

		private string className;

		private bool showOnValueMouseDown;

		protected ISessionContext sessionContext;
	}
}

using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class NewMailboxSearch : BaseForm
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
		}

		protected Label tbxKeywords_label;

		protected CheckBox cbxIncludeUnsearchableItems;

		protected Literal lblMessageFromTo;

		protected Label saeSenders_label;

		protected Literal lblMessageFromToAnd;

		protected Label saeRecipients_label;

		protected Label rbDateList_label;

		protected RadioButtonList rbDateList;

		protected HtmlGenericControl divDateList;

		protected Label dcStartDate_label;

		protected Label dcEndDate_label;

		protected Label rbSearchList_label;

		protected RadioButtonList rbSearchList;

		protected Literal lblSearchNameDescription;

		protected Label tbxSearchName_label;

		protected TextBox tbxSearchName;

		protected Label rbSearchTypeList_label;

		protected RadioButtonList rbSearchTypeList;

		protected CheckBox cbxExcludeDuplicateMessages;

		protected CheckBox cbxEnableFullLogging;

		protected HtmlGenericControl divTargetMailbox;

		protected Label pickerTargetMailbox_label;
	}
}

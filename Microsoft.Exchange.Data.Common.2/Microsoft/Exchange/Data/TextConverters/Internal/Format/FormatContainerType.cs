using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal enum FormatContainerType : byte
	{
		Null,
		Root = 129,
		Document,
		Fragment,
		Block,
		BlockQuote,
		HorizontalLine,
		TableContainer = 7,
		TableDefinition,
		TableColumnGroup,
		TableColumn,
		TableCaption = 139,
		TableExtraContent,
		Table,
		TableRow,
		TableCell,
		List,
		ListItem,
		Inline = 18,
		HyperLink,
		Bookmark,
		Image = 85,
		Area = 22,
		Map = 151,
		BaseFont = 24,
		Form,
		FieldSet,
		Label,
		Input,
		Button,
		Legend,
		TextArea,
		Select,
		OptionGroup,
		Option,
		Text = 36,
		PropertyContainer,
		InlineObjectFlag = 64,
		BlockFlag = 128
	}
}

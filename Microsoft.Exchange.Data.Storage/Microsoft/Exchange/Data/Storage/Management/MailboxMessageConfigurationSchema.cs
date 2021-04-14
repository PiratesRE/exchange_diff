using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxMessageConfigurationSchema : XsoMailboxConfigurationObjectSchema
	{
		public static readonly XsoDictionaryPropertyDefinition AfterMoveOrDeleteBehavior = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "nextselection", ExchangeObjectVersion.Exchange2007, typeof(AfterMoveOrDeleteBehavior), PropertyDefinitionFlags.None, Microsoft.Exchange.Data.Storage.Management.AfterMoveOrDeleteBehavior.OpenNextItem, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDictionaryPropertyDefinition NewItemNotification = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "newitemnotify", ExchangeObjectVersion.Exchange2007, typeof(NewItemNotification), PropertyDefinitionFlags.None, Microsoft.Exchange.Data.Storage.Management.NewItemNotification.All, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDictionaryPropertyDefinition EmptyDeletedItemsOnLogoff = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "emptydeleteditemsonlogoff", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDictionaryPropertyDefinition AutoAddSignature = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "autoaddsignature", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDictionaryPropertyDefinition SignatureText = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "signaturetext", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 8000)
		});

		public static readonly XsoDictionaryPropertyDefinition SignatureHtml = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "signaturehtml", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 8000)
		});

		public static readonly XsoDictionaryPropertyDefinition AutoAddSignatureOnMobile = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "autoaddsignatureonmobile", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDictionaryPropertyDefinition SignatureTextOnMobile = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "signaturetextonmobile", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 512)
		});

		public static readonly XsoDictionaryPropertyDefinition UseDesktopSignature = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "usedesktopsignature", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDictionaryPropertyDefinition DefaultFontName = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "composefontname", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, "Calibri", PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 100)
		});

		public static readonly XsoDictionaryPropertyDefinition DefaultFontSize = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "composefontsize", ExchangeObjectVersion.Exchange2007, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 3, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 7)
		});

		public static readonly XsoDictionaryPropertyDefinition DefaultFontColor = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "composefontcolor", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, "#000000", PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RegexConstraint("^\\#[0-9a-fA-F]{6}$", RegexOptions.CultureInvariant, ServerStrings.HexadecimalHtmlColorPatternDescription)
		});

		public static readonly XsoDictionaryPropertyDefinition DefaultFontFlags = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "composefontflags", ExchangeObjectVersion.Exchange2007, typeof(FontFlags), PropertyDefinitionFlags.None, FontFlags.Normal, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDictionaryPropertyDefinition AlwaysShowBcc = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "alwaysshowbcc", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDictionaryPropertyDefinition AlwaysShowFrom = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "alwaysshowfrom", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDictionaryPropertyDefinition DefaultFormat = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "composemarkup", ExchangeObjectVersion.Exchange2007, typeof(MailFormat), PropertyDefinitionFlags.None, MailFormat.Html, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDictionaryPropertyDefinition ReadReceiptResponse = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "readreceipt", ExchangeObjectVersion.Exchange2007, typeof(ReadReceiptResponse), PropertyDefinitionFlags.None, Microsoft.Exchange.Data.Storage.Management.ReadReceiptResponse.DoNotAutomaticallySend, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDictionaryPropertyDefinition PreviewMarkAsReadBehavior = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "previewmarkasread", ExchangeObjectVersion.Exchange2007, typeof(PreviewMarkAsReadBehavior), PropertyDefinitionFlags.None, Microsoft.Exchange.Data.Storage.Management.PreviewMarkAsReadBehavior.OnSelectionChange, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDictionaryPropertyDefinition PreviewMarkAsReadDelaytime = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "markasreaddelaytime", ExchangeObjectVersion.Exchange2007, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 5, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 30)
		});

		public static readonly XsoDictionaryPropertyDefinition ConversationSortOrder = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "ConversationSortOrder", ExchangeObjectVersion.Exchange2007, typeof(ConversationSortOrder), PropertyDefinitionFlags.None, Microsoft.Exchange.Data.Storage.Management.ConversationSortOrder.ChronologicalNewestOnTop, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDictionaryPropertyDefinition ShowConversationAsTree = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "ShowTreeInListView", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDictionaryPropertyDefinition HideDeletedItems = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "HideDeletedItems", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDictionaryPropertyDefinition SendAddressDefault = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "sendaddressdefault", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1123)
		});

		public static readonly XsoDictionaryPropertyDefinition EmailComposeMode = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "EmailComposeMode", ExchangeObjectVersion.Exchange2010, typeof(EmailComposeMode), PropertyDefinitionFlags.None, Microsoft.Exchange.Data.Storage.Management.EmailComposeMode.Inline, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDictionaryPropertyDefinition CheckForForgottenAttachments = new XsoDictionaryPropertyDefinition("OWA.UserOptions", "CheckForForgottenAttachments", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}

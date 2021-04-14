using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public enum ThemeFileId
	{
		[ThemeFileInfo]
		None,
		[ThemeFileInfo("premium.css")]
		PremiumCss,
		[ThemeFileInfo("owafont.css", ThemeFileInfoFlags.Resource)]
		OwaFontCss,
		[ThemeFileInfo("csssprites.css")]
		CssSpritesCss,
		[ThemeFileInfo("csssprites2.css")]
		CssSpritesCss2,
		[ThemeFileInfo("basic.css")]
		BasicCss,
		[ThemeFileInfo("options.css", ThemeFileInfoFlags.Resource)]
		OptionsCss,
		[ThemeFileInfo("logon.css", ThemeFileInfoFlags.Resource)]
		LogonCss,
		[ThemeFileInfo("error2.css", ThemeFileInfoFlags.Resource)]
		Error2Css,
		[ThemeFileInfo("webready.css", ThemeFileInfoFlags.Resource)]
		WebReadyCss,
		[ThemeFileInfo("editorstyles.css", ThemeFileInfoFlags.Resource)]
		EditorCss,
		[ThemeFileInfo("printcalendar.css", ThemeFileInfoFlags.Resource)]
		PrintCalendarCss,
		[ThemeFileInfo("csssprites.png", ThemeFileInfoFlags.LooseImage)]
		CssSpritesPng,
		[ThemeFileInfo("gradientv.png", ThemeFileInfoFlags.LooseImage)]
		GradientVerticalPng,
		[ThemeFileInfo("gradienth.png", ThemeFileInfoFlags.LooseImage)]
		GradientHorizentalPng,
		[ThemeFileInfo("hmask.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		HorizontalGradientMask,
		[ThemeFileInfo("hmaskrtl.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		HorizontalGradientMaskRtl,
		[ThemeFileInfo("vmask.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		VerticalGradientMask,
		[ThemeFileInfo("cobrandgradient.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		CobrandGradientMask,
		[ThemeFileInfo("cobrandgradient-rtl.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		CobrandGradientMaskRtl,
		[ThemeFileInfo("logoowa.png")]
		PremiumLogoOwa,
		[ThemeFileInfo("logob.gif", ThemeFileInfoFlags.LooseImage)]
		BasicLogo,
		[ThemeFileInfo("nothemepreview.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		NoThemePreview,
		[ThemeFileInfo("themepreview.png", ThemeFileInfoFlags.LooseImage)]
		ThemePreview,
		[ThemeFileInfo("paaheader.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		PAAHeader,
		[ThemeFileInfo("about.gif", ThemeFileInfoFlags.PhaseII)]
		AboutOwa,
		[ThemeFileInfo("accsblty.gif", ThemeFileInfoFlags.PhaseII)]
		Accessibility,
		[ThemeFileInfo("addct.gif")]
		AddToContacts,
		[ThemeFileInfo("addrbook.png")]
		AddressBook,
		[ThemeFileInfo("addrbk2.png")]
		AddressBook2,
		[ThemeFileInfo("addricn.gif", ThemeFileInfoFlags.PhaseII)]
		AddressBookIcon,
		[ThemeFileInfo("anropts.gif", ThemeFileInfoFlags.LooseImage)]
		AnrOptions,
		[ThemeFileInfo("appt.gif")]
		Appointment,
		[ThemeFileInfo("attach.png")]
		Attachment1,
		[ThemeFileInfo("attch.png")]
		Attachment2,
		[ThemeFileInfo("bcnclsrch.gif", ThemeFileInfoFlags.LooseImage)]
		BasicCancelSearch,
		[ThemeFileInfo("bsearch.gif", ThemeFileInfoFlags.LooseImage)]
		BasicSearch,
		[ThemeFileInfo("bsa.gif", ThemeFileInfoFlags.LooseImage)]
		BasicSortAscending,
		[ThemeFileInfo("bsd.gif", ThemeFileInfoFlags.LooseImage)]
		BasicSortDescending,
		[ThemeFileInfo("btbattach.png", ThemeFileInfoFlags.LooseImage)]
		BasicToolbarAttach,
		[ThemeFileInfo("btbcheckname.gif", ThemeFileInfoFlags.LooseImage)]
		BasicToolbarCheckNames,
		[ThemeFileInfo("bdi_inf.gif")]
		ButtonDialogInfo,
		[ThemeFileInfo("bdi_qst.gif")]
		ButtonDialogQuestion,
		[ThemeFileInfo("bdi_wrn.gif")]
		Warning,
		[ThemeFileInfo("closedlg.gif")]
		CloseDialog,
		[ThemeFileInfo("cal-attachment.gif")]
		Attachment3,
		[ThemeFileInfo("print-cal-attach.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		PrintAttachment3,
		[ThemeFileInfo("cal-attachment-w.gif")]
		Attachment3White,
		[ThemeFileInfo("print-cal-attach-w.gif", ThemeFileInfoFlags.Resource)]
		PrintAttachment3White,
		[ThemeFileInfo("DayView16.png", ThemeFileInfoFlags.PhaseII)]
		DayView,
		[ThemeFileInfo("cal-down.gif")]
		CalendarDown,
		[ThemeFileInfo("cal-excep.gif")]
		Exception,
		[ThemeFileInfo("print-cal-excep.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		PrintException,
		[ThemeFileInfo("cal-excep-w.gif")]
		ExceptionWhite,
		[ThemeFileInfo("print-cal-excep-w.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		PrintExceptionWhite,
		[ThemeFileInfo("MonthView16.png", ThemeFileInfoFlags.PhaseII)]
		MonthView,
		[ThemeFileInfo("cal-mnupriv.gif", ThemeFileInfoFlags.PhaseII)]
		MenuPrivate,
		[ThemeFileInfo("cal-next.png", ThemeFileInfoFlags.PhaseII)]
		CalendarNext,
		[ThemeFileInfo("cal-prev.png", ThemeFileInfoFlags.PhaseII)]
		CalendarPrevious,
		[ThemeFileInfo("cal-priv.gif")]
		Private,
		[ThemeFileInfo("print-cal-priv.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		PrintPrivate,
		[ThemeFileInfo("cal-priv-w.gif")]
		PrivateWhite,
		[ThemeFileInfo("print-cal-priv-w.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		PrintPrivateWhite,
		[ThemeFileInfo("cal-recur.gif", ThemeFileInfoFlags.PhaseII)]
		RecurringAppointment,
		[ThemeFileInfo("print-cal-recur.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		PrintRecurringAppointment,
		[ThemeFileInfo("cal-recur-w.gif")]
		RecurringAppointmentWhite,
		[ThemeFileInfo("print-cal-recur-w.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		PrintRecurringAppointmentWhite,
		[ThemeFileInfo("cal-sel.gif")]
		SelectedCalendar,
		[ThemeFileInfo("cal-up.gif")]
		CalendarUp,
		[ThemeFileInfo("WeekView16.png", ThemeFileInfoFlags.PhaseII)]
		WeekView,
		[ThemeFileInfo("WorkWeekView16.png", ThemeFileInfoFlags.PhaseII)]
		WorkWeekView,
		[ThemeFileInfo("calendar.gif", ThemeFileInfoFlags.LooseImage)]
		Calendar,
		[ThemeFileInfo("calwrkwk.gif", ThemeFileInfoFlags.PhaseII)]
		CalendarWorkWeek,
		[ThemeFileInfo("canmtg.gif", ThemeFileInfoFlags.PhaseII)]
		CancelInvitation,
		[ThemeFileInfo("changepass.gif", ThemeFileInfoFlags.PhaseII)]
		ChangePassword,
		[ThemeFileInfo("checkname.gif")]
		CheckNames,
		[ThemeFileInfo("clear.gif")]
		Clear,
		[ThemeFileInfo("clear1x1.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		Clear1x1,
		[ThemeFileInfo("cllps.gif")]
		Collapse,
		[ThemeFileInfo("clndr.png")]
		Calendar2,
		[ThemeFileInfo("clndrsmll.gif")]
		Calendar2Small,
		[ThemeFileInfo("close.gif", ThemeFileInfoFlags.LooseImage)]
		Close,
		[ThemeFileInfo("conflict.gif")]
		Conflict,
		[ThemeFileInfo("cntct.png")]
		Contact2,
		[ThemeFileInfo("cntctsmll.gif")]
		Contact2Small,
		[ThemeFileInfo("contact.gif")]
		Contact,
		[ThemeFileInfo("contactdl.gif")]
		ContactDL,
		[ThemeFileInfo("crvbtmlt.gif")]
		CornerBottomLeft,
		[ThemeFileInfo("crvbtmrt.gif")]
		CornerBottomRight,
		[ThemeFileInfo("crvtplt.gif")]
		CornerTopLeft,
		[ThemeFileInfo("crvtprt.gif")]
		CornerTopRight,
		[ThemeFileInfo("datebook.gif", ThemeFileInfoFlags.PhaseII)]
		DateBook,
		[ThemeFileInfo("delayed_dlvr.gif")]
		DelayedDelivery,
		[ThemeFileInfo("delete.png")]
		Delete,
		[ThemeFileInfo("deleted.gif")]
		Deleted,
		[ThemeFileInfo("bdeleted.gif", ThemeFileInfoFlags.LooseImage)]
		BasicDeleted,
		[ThemeFileInfo("bddeleted.gif", ThemeFileInfoFlags.LooseImage)]
		BasicDarkDeleted,
		[ThemeFileInfo("down-b.gif", ThemeFileInfoFlags.PhaseII)]
		DownButton,
		[ThemeFileInfo("drafts.png")]
		Drafts,
		[ThemeFileInfo("drparw.gif")]
		DownButton2,
		[ThemeFileInfo("dwn.png")]
		DownButton3,
		[ThemeFileInfo("email-lg.gif", ThemeFileInfoFlags.PhaseII)]
		EMailLarge,
		[ThemeFileInfo("email-xlg.gif", ThemeFileInfoFlags.PhaseII)]
		EMailExtraLarge,
		[ThemeFileInfo("email.png")]
		EMail,
		[ThemeFileInfo("emailcontact.gif")]
		EMailContact,
		[ThemeFileInfo("eml.png")]
		EMail2,
		[ThemeFileInfo("emlsmll.png")]
		EMail2Small,
		[ThemeFileInfo("emptydel.gif", ThemeFileInfoFlags.PhaseII)]
		EmptyDeletedItems,
		[ThemeFileInfo("warn.png")]
		Error,
		[ThemeFileInfo("errorBG.gif", ThemeFileInfoFlags.LooseImage)]
		ErrorBackground,
		[ThemeFileInfo("evt-break.gif", ThemeFileInfoFlags.PhaseII)]
		EventBreak,
		[ThemeFileInfo("exclaim.gif")]
		Exclaim,
		[ThemeFileInfo("expnd.gif")]
		Expand,
		[ThemeFileInfo("fb-tnt.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		Tentative2,
		[ThemeFileInfo("tntgtr.gif", ThemeFileInfoFlags.LooseImage)]
		TentativeBasicGutter,
		[ThemeFileInfo("tntevnt.gif", ThemeFileInfoFlags.LooseImage)]
		TentativeBasicEvent,
		[ThemeFileInfo("tntnnwrk.gif", ThemeFileInfoFlags.LooseImage)]
		TentativeBasicNonWorkingHours,
		[ThemeFileInfo("tntwrk.gif", ThemeFileInfoFlags.LooseImage)]
		TentativeBasicWorkingHours,
		[ThemeFileInfo("fldr.png")]
		Folder,
		[ThemeFileInfo("fldr-opn.gif")]
		FolderOpen,
		[ThemeFileInfo("forecolor.png", ThemeFileInfoFlags.PhaseII)]
		ForeColor,
		[ThemeFileInfo("forward.gif")]
		Forward,
		[ThemeFileInfo("forwardsms.gif")]
		ForwardSms,
		[ThemeFileInfo("fp.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		FirstPage,
		[ThemeFileInfo("fpg.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		FirstPageGray,
		[ThemeFileInfo("globe.gif", ThemeFileInfoFlags.PhaseII)]
		Globe,
		[ThemeFileInfo("go2.gif", ThemeFileInfoFlags.LooseImage)]
		Go2,
		[ThemeFileInfo("help.png")]
		Help,
		[ThemeFileInfo("home.gif")]
		HomePhone,
		[ThemeFileInfo("icon-doc.gif")]
		Document,
		[ThemeFileInfo("doc.gif")]
		FlatDocument,
		[ThemeFileInfo("icon-flag.gif")]
		Flag,
		[ThemeFileInfo("msg-rd.png")]
		MessageRead,
		[ThemeFileInfo("msg-unrd.png")]
		MessageUnread,
		[ThemeFileInfo("icon-wrn.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		WarningIcon,
		[ThemeFileInfo("ih.gif")]
		ImportanceHigh,
		[ThemeFileInfo("il.gif")]
		ImportanceLow,
		[ThemeFileInfo("imphigh.gif")]
		ImportanceHigh2,
		[ThemeFileInfo("implow.gif")]
		ImportanceLow2,
		[ThemeFileInfo("impnorm.gif")]
		ImportanceNormal,
		[ThemeFileInfo("inbox.png")]
		Inbox,
		[ThemeFileInfo("info.gif")]
		Informational,
		[ThemeFileInfo("ignorecnv.png")]
		IgnoreConversation,
		[ThemeFileInfo("journal.gif")]
		Journal,
		[ThemeFileInfo("junkemail.gif")]
		JunkEMail,
		[ThemeFileInfo("junkemailbig.gif", ThemeFileInfoFlags.PhaseII)]
		JunkEmailBig,
		[ThemeFileInfo("lgntopl.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LogonTopLeft,
		[ThemeFileInfo("lgntopm.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LogonTopMiddle,
		[ThemeFileInfo("lgntopr.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LogonTopRight,
		[ThemeFileInfo("lgnbotl.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LogonBottomLeft,
		[ThemeFileInfo("lgnbotm.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LogonBottomMiddle,
		[ThemeFileInfo("lgnbotr.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LogonBottomRight,
		[ThemeFileInfo("lgnexlogo.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LogonExchangeLogo,
		[ThemeFileInfo("lp.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LastPage,
		[ThemeFileInfo("lpg.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LastPageGray,
		[ThemeFileInfo("lvdivide.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		ListViewDivider,
		[ThemeFileInfo("mrkcmplt.gif")]
		MarkComplete,
		[ThemeFileInfo("minus.png")]
		Minus,
		[ThemeFileInfo("ml.gif")]
		MultiLine,
		[ThemeFileInfo("mobile.gif")]
		MobilePhone,
		[ThemeFileInfo("move.gif")]
		Move,
		[ThemeFileInfo("move-folder.gif")]
		MoveFolder,
		[ThemeFileInfo("msgdtls.gif")]
		MessageDetails,
		[ThemeFileInfo("mtg-accept.png")]
		MeetingAccept,
		[ThemeFileInfo("mtg-decline.png")]
		MeetingDecline,
		[ThemeFileInfo("mtg-tent.png")]
		MeetingTentative,
		[ThemeFileInfo("mtg-accept-big.png")]
		MeetingAcceptBig,
		[ThemeFileInfo("mtg-decline-big.png")]
		MeetingDeclineBig,
		[ThemeFileInfo("mtg-tentative-big.png")]
		MeetingTentativeBig,
		[ThemeFileInfo("mtg-info-big.png")]
		MeetingInfoBig,
		[ThemeFileInfo("mtg-open-big.png")]
		MeetingOpenBig,
		[ThemeFileInfo("mtg-delete-big.png")]
		MeetingDeleteBig,
		[ThemeFileInfo("mtgreq-cancel.gif")]
		MeetingRequestCancel,
		[ThemeFileInfo("mtgreq.gif")]
		MeetingRequest,
		[ThemeFileInfo("mtgrsp-accept.gif")]
		MeetingResponseAccept,
		[ThemeFileInfo("mtgrsp-decline.gif")]
		MeetingResponseDecline,
		[ThemeFileInfo("mtgrsp-tent.gif")]
		MeetingResponseTentative,
		[ThemeFileInfo("noterr.gif")]
		NotificationError,
		[ThemeFileInfo("newemail.png")]
		EMail3,
		[ThemeFileInfo("fax-unrd.gif")]
		FaxMessage,
		[ThemeFileInfo("newfldr.gif")]
		Folder2,
		[ThemeFileInfo("next.gif")]
		Next,
		[ThemeFileInfo("nm.gif")]
		NextArrow,
		[ThemeFileInfo("nodata.gif")]
		NoData,
		[ThemeFileInfo("notes.png")]
		Notes,
		[ThemeFileInfo("np.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		NextPage,
		[ThemeFileInfo("npg.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		NextPageGray,
		[ThemeFileInfo("obdv.gif", ThemeFileInfoFlags.LooseImage)]
		OptionsBarDivider,
		[ThemeFileInfo("oof.gif", ThemeFileInfoFlags.PhaseII)]
		Oof,
		[ThemeFileInfo("opt.gif", ThemeFileInfoFlags.PhaseII)]
		OptionalAttendee,
		[ThemeFileInfo("options.gif")]
		Options,
		[ThemeFileInfo("org-up-arrow.gif")]
		OrganizationUpArrow,
		[ThemeFileInfo("org.gif", ThemeFileInfoFlags.PhaseII)]
		Organizer,
		[ThemeFileInfo("outbox.gif")]
		Outbox,
		[ThemeFileInfo("pda.gif", ThemeFileInfoFlags.PhaseII)]
		Pda,
		[ThemeFileInfo("perfmon.gif", ThemeFileInfoFlags.PhaseII)]
		PerformanceMonitor,
		[ThemeFileInfo("progress.gif", ThemeFileInfoFlags.LooseImage)]
		Progress,
		[ThemeFileInfo("pgrs-sm.gif", ThemeFileInfoFlags.LooseImage)]
		ProgressSmall,
		[ThemeFileInfo("pl_ont.gif")]
		PlayOnTelephone,
		[ThemeFileInfo("plus.png")]
		Plus,
		[ThemeFileInfo("pm.gif")]
		PreviousArrow,
		[ThemeFileInfo("pnrsz.gif", ThemeFileInfoFlags.LooseImage)]
		NavigationResize,
		[ThemeFileInfo("post.gif")]
		Post,
		[ThemeFileInfo("postRpActive.png")]
		PostReplyActive,
		[ThemeFileInfo("postRpGhost.png")]
		PostReplyGhost,
		[ThemeFileInfo("postRpDisabled.png")]
		PostReplyDisabled,
		[ThemeFileInfo("postRpGhostDisabled.png")]
		PostReplyGhostDisabled,
		[ThemeFileInfo("pp.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		PreviousPage,
		[ThemeFileInfo("ppg.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		PreviousPageGray,
		[ThemeFileInfo("prev.gif")]
		Previous,
		[ThemeFileInfo("priph.gif", ThemeFileInfoFlags.PhaseII)]
		PrimaryPhone,
		[ThemeFileInfo("print.png")]
		Print,
		[ThemeFileInfo("red-ul.gif", ThemeFileInfoFlags.Resource)]
		RedUnderline,
		[ThemeFileInfo("rem-sm.png")]
		ReminderSmall,
		[ThemeFileInfo("reply.gif")]
		Reply,
		[ThemeFileInfo("replyActive.png")]
		ReplyActiveIcon,
		[ThemeFileInfo("replyGhost.png", ThemeFileInfoFlags.PhaseII)]
		ReplyGhostIcon,
		[ThemeFileInfo("replyDisabled.png")]
		ReplyDisabledIcon,
		[ThemeFileInfo("replyGhostDisabled.png", ThemeFileInfoFlags.PhaseII)]
		ReplyGhostDisabledIcon,
		[ThemeFileInfo("replyall.gif")]
		ReplyAll,
		[ThemeFileInfo("replyAllActive.png")]
		ReplyAllActiveIcon,
		[ThemeFileInfo("replyAllDisabled.png")]
		ReplyAllDisabledIcon,
		[ThemeFileInfo("replyAllGhost.png", ThemeFileInfoFlags.PhaseII)]
		ReplyAllGhostIcon,
		[ThemeFileInfo("replyAllGhostDisabled.png", ThemeFileInfoFlags.PhaseII)]
		ReplyAllGhostDisabledIcon,
		[ThemeFileInfo("forwardActive.png")]
		ForwardActiveIcon,
		[ThemeFileInfo("forwardGhost.png", ThemeFileInfoFlags.PhaseII)]
		ForwardGhostIcon,
		[ThemeFileInfo("forwardDisabled.png")]
		ForwardDisabledIcon,
		[ThemeFileInfo("forwardGhostDisabled.png", ThemeFileInfoFlags.PhaseII)]
		ForwardGhostDisabledIcon,
		[ThemeFileInfo("replyallsms.gif")]
		ReplyAllSms,
		[ThemeFileInfo("replyphone.gif")]
		ReplyByPhone,
		[ThemeFileInfo("replysms.gif")]
		ReplyBySms,
		[ThemeFileInfo("reqd.gif", ThemeFileInfoFlags.PhaseII)]
		RequiredAttendee,
		[ThemeFileInfo("res.gif", ThemeFileInfoFlags.PhaseII)]
		ResourceAttendee,
		[ThemeFileInfo("root.png")]
		Root,
		[ThemeFileInfo("rpb.gif")]
		ReadingPaneBottom,
		[ThemeFileInfo("rpb_rtl.gif")]
		ReadingPaneBottomRTL,
		[ThemeFileInfo("rpo.gif")]
		ReadingPaneOff,
		[ThemeFileInfo("rpo_rtl.gif")]
		ReadingPaneOffRTL,
		[ThemeFileInfo("rpr.gif")]
		ReadingPaneRight,
		[ThemeFileInfo("rpr_rtl.gif")]
		ReadingPaneRightRTL,
		[ThemeFileInfo("hdiv-l.png")]
		HorizontalDividerImageLeft,
		[ThemeFileInfo("hdiv-r.png")]
		HorizontalDividerImageRight,
		[ThemeFileInfo("hdiv-t.png", ThemeFileInfoFlags.LooseImage)]
		HorizontalDividerImageTile,
		[ThemeFileInfo("sa.gif")]
		SortAscending,
		[ThemeFileInfo("save.png")]
		Save,
		[ThemeFileInfo("buttonspanelsave.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		OptionsSave,
		[ThemeFileInfo("sd.gif")]
		SortDescending,
		[ThemeFileInfo("send.gif")]
		Send,
		[ThemeFileInfo("sentitems.png")]
		SentItems,
		[ThemeFileInfo("showcalendar.gif")]
		ShowCalendar,
		[ThemeFileInfo("addcalendar.png")]
		AddCalendar,
		[ThemeFileInfo("sig-lg.gif", ThemeFileInfoFlags.PhaseII)]
		SignatureLarge,
		[ThemeFileInfo("sig.gif")]
		Signature,
		[ThemeFileInfo("sl.gif", ThemeFileInfoFlags.PhaseII)]
		SingleLine,
		[ThemeFileInfo("spelling.gif")]
		Spelling,
		[ThemeFileInfo("sp-dict.gif", ThemeFileInfoFlags.PhaseII | ThemeFileInfoFlags.Resource)]
		SpellingDictionary,
		[ThemeFileInfo("sr.png")]
		CheckMessages,
		[ThemeFileInfo("tsk.gif")]
		Task,
		[ThemeFileInfo("tbdv.gif")]
		ToolbarDivider,
		[ThemeFileInfo("divider.gif", ThemeFileInfoFlags.LooseImage)]
		FolderStatusBarDividerPremium,
		[ThemeFileInfo("themes.gif", ThemeFileInfoFlags.PhaseII)]
		Themes,
		[ThemeFileInfo("tntv.gif", ThemeFileInfoFlags.PhaseII)]
		Tentative,
		[ThemeFileInfo("free.png", ThemeFileInfoFlags.PhaseII)]
		Available,
		[ThemeFileInfo("outofoffice.png", ThemeFileInfoFlags.PhaseII)]
		OutOfOffice,
		[ThemeFileInfo("cal-busy.png", ThemeFileInfoFlags.PhaseII)]
		Busy,
		[ThemeFileInfo("nodata70.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		Nodata70,
		[ThemeFileInfo("fb-tnt70.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		Tentative70,
		[ThemeFileInfo("work.gif", ThemeFileInfoFlags.PhaseII)]
		WorkPhone,
		[ThemeFileInfo("wunderbar_resize.png")]
		PrimaryNavigationResize,
		[ThemeFileInfo("wunderbar_top.gif")]
		PrimaryNavigationTop,
		[ThemeFileInfo("vm-unrd.gif")]
		VoiceMessage,
		[ThemeFileInfo("play.gif")]
		VoiceMessageAttachmentPlay,
		[ThemeFileInfo("stop.gif")]
		VoiceMessageAttachmentStop,
		[ThemeFileInfo("search.png")]
		Search,
		[ThemeFileInfo("copy.gif")]
		Copy,
		[ThemeFileInfo("copy-folder.gif")]
		CopyFolder,
		[ThemeFileInfo("copy-to-folder.gif")]
		CopyToFolder,
		[ThemeFileInfo("opdlvrp.gif")]
		OpenDeliveryReport,
		[ThemeFileInfo("notify.wav")]
		NotificationSound,
		[ThemeFileInfo("evtfrom.gif")]
		EventFrom,
		[ThemeFileInfo("evtfrom-w.gif")]
		EventFromWhite,
		[ThemeFileInfo("evtto.gif")]
		EventTo,
		[ThemeFileInfo("evtto-w.gif")]
		EventToWhite,
		[ThemeFileInfo("cal-up-hl.gif")]
		CalendarUpHighlighted,
		[ThemeFileInfo("cal-down-hl.gif")]
		CalendarDownHighlighted,
		[ThemeFileInfo("addrbook-disabled.png")]
		AddressBookDisabled,
		[ThemeFileInfo("mtgrcpnt.gif", ThemeFileInfoFlags.LooseImage)]
		MeetingRecipients,
		[ThemeFileInfo("recur.gif")]
		Recurrence,
		[ThemeFileInfo("up.gif")]
		Up,
		[ThemeFileInfo("fldr-web.gif", ThemeFileInfoFlags.PhaseII)]
		WebFolder,
		[ThemeFileInfo("mnu-r.gif")]
		FlyoutMenuRight,
		[ThemeFileInfo("mnu-l.gif")]
		FlyoutMenuLeft,
		[ThemeFileInfo("dl-user.gif")]
		DistributionListUser,
		[ThemeFileInfo("dl-other.gif")]
		DistributionListOther,
		[ThemeFileInfo("pkrarwl.gif")]
		PickerArrowLtr,
		[ThemeFileInfo("pkrarwr.gif")]
		PickerArrowRtl,
		[ThemeFileInfo("dl.gif", ThemeFileInfoFlags.PhaseII)]
		AddressBookDL,
		[ThemeFileInfo("fax.gif", ThemeFileInfoFlags.PhaseII)]
		Fax,
		[ThemeFileInfo("up-arrw.gif")]
		UpArrow,
		[ThemeFileInfo("msgopts.gif")]
		MessageOptions,
		[ThemeFileInfo("rt.gif")]
		RightArrow,
		[ThemeFileInfo("lt.gif")]
		LeftArrow,
		[ThemeFileInfo("dc-html.gif")]
		HtmlDocument,
		[ThemeFileInfo("cal-rsc-perm.gif", ThemeFileInfoFlags.PhaseII)]
		CalendarResourcePermissions,
		[ThemeFileInfo("srchfdr.png")]
		SearchFolderIcon,
		[ThemeFileInfo("cal-proc.gif", ThemeFileInfoFlags.PhaseII)]
		AutoCalProcessing,
		[ThemeFileInfo("flg-empty.gif")]
		FlagEmpty,
		[ThemeFileInfo("flg-compl.gif")]
		FlagComplete,
		[ThemeFileInfo("flg-sender.gif")]
		FlagSender,
		[ThemeFileInfo("flg-dsbl.gif")]
		FlagDisabled,
		[ThemeFileInfo("flg-compl-dsbl.gif")]
		FlagCompleteDisabled,
		[ThemeFileInfo("cut.gif")]
		EditorCut,
		[ThemeFileInfo("paste.gif")]
		EditorPaste,
		[ThemeFileInfo("undo.gif")]
		EditorUndo,
		[ThemeFileInfo("dn_mblgn.gif")]
		ExplicitLogonDownArrow,
		[ThemeFileInfo("yellowshield.gif")]
		YellowShield,
		[ThemeFileInfo("cmpldd.gif")]
		ComplianceDropDown,
		[ThemeFileInfo("cmpl_chk.gif", ThemeFileInfoFlags.PhaseII)]
		ComplianceCheck,
		[ThemeFileInfo("addtsk.gif")]
		AddTask,
		[ThemeFileInfo("dwn-gry.gif", ThemeFileInfoFlags.PhaseII)]
		DownArrowGrey,
		[ThemeFileInfo("chkbxhdr.gif")]
		CheckboxHeader,
		[ThemeFileInfo("elcfldr.gif")]
		ELCFolderIcon,
		[ThemeFileInfo("ctgrs.gif")]
		Categories,
		[ThemeFileInfo("ctgrs-hdr.gif")]
		CategoriesHeader,
		[ThemeFileInfo("chk-unchk.gif")]
		CheckUnchecked,
		[ThemeFileInfo("chk-prtl.gif")]
		CheckPartial,
		[ThemeFileInfo("chk-chkd.gif")]
		CheckChecked,
		[ThemeFileInfo("chkmrk.png")]
		Checkmark,
		[ThemeFileInfo("lv-chk.png")]
		ListViewCheckboxChecked,
		[ThemeFileInfo("lv-unchk.png")]
		ListViewCheckboxUnchecked,
		[ThemeFileInfo("rd-sel.gif")]
		RadioSelected,
		[ThemeFileInfo("rd-unsel.gif")]
		RadioUnselected,
		[ThemeFileInfo("cnclsrch.gif")]
		CancelSearch,
		[ThemeFileInfo("fvdoc.gif")]
		AddToFavorites,
		[ThemeFileInfo("dot.gif")]
		Dot,
		[ThemeFileInfo("squiggly.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		SpellCheckUnderline,
		[ThemeFileInfo("rul-sml.gif")]
		RulesSmall,
		[ThemeFileInfo("msg-encyptd.gif")]
		MessageEncrypted,
		[ThemeFileInfo("msg-sgnd.gif")]
		MessageSigned,
		[ThemeFileInfo("msg-irm.gif")]
		MessageIrm,
		[ThemeFileInfo("vm-irm.gif")]
		VoicemailIrm,
		[ThemeFileInfo("encrypted.gif")]
		Encrypted,
		[ThemeFileInfo("sigvalid.gif")]
		ValidSignature,
		[ThemeFileInfo("sigwarning.gif", ThemeFileInfoFlags.PhaseII)]
		WarningSignature,
		[ThemeFileInfo("siginvalid.gif")]
		InvalidSignature,
		[ThemeFileInfo("ovf-exp.gif")]
		MonthlyViewExpandOverflow,
		[ThemeFileInfo("ovf-col.gif")]
		MonthlyViewCollapseOverflow,
		[ThemeFileInfo("ovf-exp-rtl.gif")]
		MonthlyViewExpandOverflowRtl,
		[ThemeFileInfo("ovf-col-rtl.gif")]
		MonthlyViewCollapseOverflowRtl,
		[ThemeFileInfo("dc-msg.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		EmbeddedMessage,
		[ThemeFileInfo("recover.png", ThemeFileInfoFlags.PhaseII)]
		Recover,
		[ThemeFileInfo("rcvdelitms.gif", ThemeFileInfoFlags.PhaseII)]
		RecoverDeletedItems,
		[ThemeFileInfo("rcvdelitms_small.png")]
		RecoverDeletedItemsSmall,
		[ThemeFileInfo("fwdattach.gif")]
		ForwardAsAttachment,
		[ThemeFileInfo("attachitem.png", ThemeFileInfoFlags.PhaseII)]
		AttachItem,
		[ThemeFileInfo("fldr-shared-to.gif")]
		FolderSharedTo,
		[ThemeFileInfo("cal-shared-to.gif", ThemeFileInfoFlags.PhaseII)]
		CalendarSharedTo,
		[ThemeFileInfo("cnt-shared-to.gif", ThemeFileInfoFlags.PhaseII)]
		ContactSharedTo,
		[ThemeFileInfo("tsk-shared-to.gif", ThemeFileInfoFlags.PhaseII)]
		TaskSharedTo,
		[ThemeFileInfo("fldr-shared-out.gif")]
		FolderSharedOut,
		[ThemeFileInfo("cal-shared-out.gif", ThemeFileInfoFlags.PhaseII)]
		CalendarSharedOut,
		[ThemeFileInfo("cnt-shared-out.gif", ThemeFileInfoFlags.PhaseII)]
		ContactSharedOut,
		[ThemeFileInfo("tsk-shared-out.gif", ThemeFileInfoFlags.PhaseII)]
		TaskSharedOut,
		[ThemeFileInfo("approve.gif")]
		Approve,
		[ThemeFileInfo("reject.gif")]
		Reject,
		[ThemeFileInfo("error2.gif")]
		Error2,
		[ThemeFileInfo("delete_small.gif")]
		DeleteSmall,
		[ThemeFileInfo("phone.gif")]
		Phone,
		[ThemeFileInfo("chat.gif")]
		Chat,
		[ThemeFileInfo("newchat.gif")]
		NewChat,
		[ThemeFileInfo("filter.gif")]
		RecipientFilter,
		[ThemeFileInfo("doughboy.png")]
		DoughboyPerson,
		[ThemeFileInfo("doughboySm.png")]
		DoughboyPersonSmall,
		[ThemeFileInfo("doughboyDL.png")]
		DoughboyDL,
		[ThemeFileInfo("away.png")]
		PresenceAway,
		[ThemeFileInfo("busy.png")]
		PresenceBusy,
		[ThemeFileInfo("dnd.png")]
		PresenceDoNotDisturb,
		[ThemeFileInfo("offline.png")]
		PresenceOffline,
		[ThemeFileInfo("avlbl.png")]
		PresenceAvailable,
		[ThemeFileInfo("blocked.png")]
		PresenceBlocked,
		[ThemeFileInfo("unkwn.png")]
		PresenceUnknown,
		[ThemeFileInfo("awayVbar.png")]
		PresenceAwayVbar,
		[ThemeFileInfo("busyVbar.png")]
		PresenceBusyVbar,
		[ThemeFileInfo("dndVbar.png")]
		PresenceDoNotDisturbVbar,
		[ThemeFileInfo("offlineVbar.png")]
		PresenceOfflineVbar,
		[ThemeFileInfo("avlblVbar.png")]
		PresenceAvailableVbar,
		[ThemeFileInfo("blockedVbar.png")]
		PresenceBlockedVbar,
		[ThemeFileInfo("unkwnVbar.png")]
		PresenceUnknownVbar,
		[ThemeFileInfo("awayVbarSm.png")]
		PresenceAwayVbarSmall,
		[ThemeFileInfo("busyVbarSm.png")]
		PresenceBusyVbarSmall,
		[ThemeFileInfo("dndVbarSm.png")]
		PresenceDoNotDisturbVbarSmall,
		[ThemeFileInfo("offlineVbarSm.png")]
		PresenceOfflineVbarSmall,
		[ThemeFileInfo("avlblVbarSm.png")]
		PresenceAvailableVbarSmall,
		[ThemeFileInfo("blockedVbarSm.png")]
		PresenceBlockedVbarSmall,
		[ThemeFileInfo("unkwnVbarSm.png")]
		PresenceUnknownVbarSmall,
		[ThemeFileInfo("big-blocked.png")]
		BigPresenceBlocked,
		[ThemeFileInfo("adbdy.gif")]
		AddBuddy,
		[ThemeFileInfo("rmbdy.gif")]
		RemoveBuddy,
		[ThemeFileInfo("chat-arw.gif")]
		ChatArrow,
		[ThemeFileInfo("failinv.gif", ThemeFileInfoFlags.PhaseII)]
		FailedToInvite,
		[ThemeFileInfo("typng.gif", ThemeFileInfoFlags.PhaseII)]
		Typing,
		[ThemeFileInfo("joinchat.gif")]
		JoinedChat,
		[ThemeFileInfo("leftchat.gif")]
		LeftChat,
		[ThemeFileInfo("sendsms.gif")]
		SendSms,
		[ThemeFileInfo("sms.gif")]
		Sms,
		[ThemeFileInfo("new.png")]
		New,
		[ThemeFileInfo("lvlogo.png")]
		LiveLogo,
		[ThemeFileInfo("yahoo.png")]
		YahooLogo,
		[ThemeFileInfo("oc.png")]
		OfficeCommunicatorLogo,
		[ThemeFileInfo("rss.gif")]
		RssSubscription,
		[ThemeFileInfo("fvrfltr.gif")]
		FavoritesFilter,
		[ThemeFileInfo("minusrtl.png")]
		MinusRTL,
		[ThemeFileInfo("plusrtl.png")]
		PlusRTL,
		[ThemeFileInfo("chgperm.gif")]
		ChangePermission,
		[ThemeFileInfo("fltr-addfav.png")]
		FilterAddToFav,
		[ThemeFileInfo("fltr-clr.gif")]
		FilterClear,
		[ThemeFileInfo("tsklg.png")]
		Task2,
		[ThemeFileInfo("dcmnt.gif", ThemeFileInfoFlags.PhaseII)]
		Documents,
		[ThemeFileInfo("dcmntsmll.gif", ThemeFileInfoFlags.PhaseII)]
		DocumentsSmall,
		[ThemeFileInfo("pf.gif")]
		PublicFolder,
		[ThemeFileInfo("pfsmll.gif")]
		PublicFolderSmall,
		[ThemeFileInfo("lgnleft.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LogonLeft,
		[ThemeFileInfo("lgnright.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LogonRight,
		[ThemeFileInfo("cnv-his.gif")]
		ConversationHistory,
		[ThemeFileInfo("shw-b.png", ThemeFileInfoFlags.LooseImage)]
		ShadowBottom,
		[ThemeFileInfo("shw-bl.png")]
		ShadowBottomLeft,
		[ThemeFileInfo("shw-br.png")]
		ShadowBottomRight,
		[ThemeFileInfo("shw-r.png", ThemeFileInfoFlags.LooseImage)]
		ShadowRight,
		[ThemeFileInfo("shw-tr.png")]
		ShadowTopRight,
		[ThemeFileInfo("cnv-draft.png", ThemeFileInfoFlags.LooseImage)]
		ConversationsDraftPattern,
		[ThemeFileInfo("headerbgmain.png", ThemeFileInfoFlags.LooseImage)]
		OwaPremiumBackgroundImageMain,
		[ThemeFileInfo("headerbgmainrtl.png", ThemeFileInfoFlags.LooseImage)]
		OwaPremiumBackgroundImageMainRtl,
		[ThemeFileInfo("headerbgright.png", ThemeFileInfoFlags.LooseImage)]
		OwaPremiumBackgroundImageRight,
		[ThemeFileInfo("hdrdiv-l.png")]
		HeaderDividerImageLeft,
		[ThemeFileInfo("hdrdiv-r.png")]
		HeaderDividerImageRight,
		[ThemeFileInfo("hdrdiv-t.png", ThemeFileInfoFlags.LooseImage)]
		HeaderDividerImageTile,
		[ThemeFileInfo("userdropdown.png")]
		UserTileDropDownArrow,
		[ThemeFileInfo("alertdropdown.gif")]
		AlertBarDropDownArrow,
		[ThemeFileInfo("cnv-msg-rd.gif")]
		ConversationIconRead,
		[ThemeFileInfo("cnv-msg-unrd.gif")]
		ConversationIconUnread,
		[ThemeFileInfo("cnv-msg-rpl.gif")]
		ConversationIconReply,
		[ThemeFileInfo("cnv-msg-fwd.gif")]
		ConversationIconForward,
		[ThemeFileInfo("cnv-msg-irm-rd.gif")]
		IrmConversationIconRead,
		[ThemeFileInfo("cnv-msg-irm-unrd.gif")]
		IrmConversationIconUnread,
		[ThemeFileInfo("cnv-msg-irm-rpl.gif")]
		IrmConversationIconReply,
		[ThemeFileInfo("cnv-msg-irm-fwd.gif")]
		IrmConversationIconForward,
		[ThemeFileInfo("sms-conv.gif")]
		ConversationIconSmsReadAndUnread,
		[ThemeFileInfo("sms-conv-reply.gif")]
		ConversationIconSmsReply,
		[ThemeFileInfo("sms-conv-forward.gif")]
		ConversationIconSmsForward,
		[ThemeFileInfo("cnv-mtg.gif")]
		ConversationIconMeeting,
		[ThemeFileInfo("pipe-stop-l.png")]
		PipeStopLarge,
		[ThemeFileInfo("pipe-stop-s.png")]
		PipeStopSmall,
		[ThemeFileInfo("pipe-end.png")]
		PipeEnd,
		[ThemeFileInfo("favicon.ico", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		FavoriteIcon,
		[ThemeFileInfo("excal.png")]
		ExchangeCalendar,
		[ThemeFileInfo("dash.gif")]
		Dash,
		[ThemeFileInfo("warning.png")]
		WarningSmall,
		[ThemeFileInfo("insertimage.png")]
		InsertImage,
		[ThemeFileInfo("cal-web.png", ThemeFileInfoFlags.PhaseII)]
		WebCalendar,
		[ThemeFileInfo("cal-web-big.png", ThemeFileInfoFlags.PhaseII)]
		WebCalendarBig,
		[ThemeFileInfo("print-tentative.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		PrintTentative,
		[ThemeFileInfo("print-tentative-agenda.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		PrintTentativeForAgenda,
		[ThemeFileInfo("dc-accdb.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeAccdb,
		[ThemeFileInfo("dc-accde.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeAccde,
		[ThemeFileInfo("dc-acs.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeAcs,
		[ThemeFileInfo("dc-ascx.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeAscx,
		[ThemeFileInfo("dc-asf.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeAsf,
		[ThemeFileInfo("dc-asp.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeAsp,
		[ThemeFileInfo("dc-aspc.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeAspc,
		[ThemeFileInfo("dc-aspx.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeAspx,
		[ThemeFileInfo("dc-bmp.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeBmp,
		[ThemeFileInfo("dc-cab.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeCab,
		[ThemeFileInfo("dc-chm.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeChm,
		[ThemeFileInfo("dc-cpp.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeCpp,
		[ThemeFileInfo("dc-cs.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeCs,
		[ThemeFileInfo("dc-css.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeCss,
		[ThemeFileInfo("dc-dll.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeDll,
		[ThemeFileInfo("dc-docx.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeDocx,
		[ThemeFileInfo("dc-dot.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeDot,
		[ThemeFileInfo("dc-dotx.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeDotx,
		[ThemeFileInfo("dc-dwt.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeDwt,
		[ThemeFileInfo("dc-exe.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeExe,
		[ThemeFileInfo("icon-doc.gif.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeGif,
		[ThemeFileInfo("dc-htc.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeHtc,
		[ThemeFileInfo("dc-htt.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeHtt,
		[ThemeFileInfo("dc-hxx.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeHxx,
		[ThemeFileInfo("dc-ini.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeIni,
		[ThemeFileInfo("dc-jpg.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeJpg,
		[ThemeFileInfo("dc-js.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeJs,
		[ThemeFileInfo("dc-master.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeMaster,
		[ThemeFileInfo("dc-mht.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeMht,
		[ThemeFileInfo("dc-mpg.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeMpg,
		[ThemeFileInfo("dc-mpnt.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeMpnt,
		[ThemeFileInfo("dc-mpt.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeMpt,
		[ThemeFileInfo("dc-mpw.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeMpw,
		[ThemeFileInfo("dc-mpx.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeMpx,
		[ThemeFileInfo("dc-oicon-doc.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeOdc,
		[ThemeFileInfo("dc-one.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeOne,
		[ThemeFileInfo("dc-onp.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeOnp,
		[ThemeFileInfo("dc-pblsh.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypePblsh,
		[ThemeFileInfo("dc-png.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypePng,
		[ThemeFileInfo("dc-pot.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypePot,
		[ThemeFileInfo("dc-potx.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypePotx,
		[ThemeFileInfo("dc-pps.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypePps,
		[ThemeFileInfo("dc-ppt.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypePpt,
		[ThemeFileInfo("dc-pptx.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypePptx,
		[ThemeFileInfo("dc-prj.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypePrj,
		[ThemeFileInfo("dc-ptm.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypePtm,
		[ThemeFileInfo("dc-rpmsg.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeRpmsg,
		[ThemeFileInfo("dc-rtf.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeRtf,
		[ThemeFileInfo("dc-tif.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeTif,
		[ThemeFileInfo("dc-txt.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeTxt,
		[ThemeFileInfo("dc-url.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeUrl,
		[ThemeFileInfo("dc-vbs.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeVbs,
		[ThemeFileInfo("dc-vdx.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeVdx,
		[ThemeFileInfo("dc-vsd.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeVsd,
		[ThemeFileInfo("dc-vsl.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeVsl,
		[ThemeFileInfo("dc-vss.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeVss,
		[ThemeFileInfo("dc-vst.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeVst,
		[ThemeFileInfo("dc-vsu.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeVsu,
		[ThemeFileInfo("dc-vsw.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeVsw,
		[ThemeFileInfo("dc-vsx.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeVsx,
		[ThemeFileInfo("dc-vtx.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeVtx,
		[ThemeFileInfo("dc-wrd.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeWrd,
		[ThemeFileInfo("dc-wv.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeWv,
		[ThemeFileInfo("dc-xcl.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeXcl,
		[ThemeFileInfo("dc-xlsx.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeXlsx,
		[ThemeFileInfo("dc-xlt.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeXlt,
		[ThemeFileInfo("dc-xltx.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeXltx,
		[ThemeFileInfo("dc-xml.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeXml,
		[ThemeFileInfo("dc-xsd.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeXsd,
		[ThemeFileInfo("dc-xsl.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeXsl,
		[ThemeFileInfo("dc-xslt.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeXslt,
		[ThemeFileInfo("dc-xsn.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeXsn,
		[ThemeFileInfo("dc-zip.gif", ThemeFileInfoFlags.PhaseII, "icon-doc.gif")]
		FileTypeZip,
		[ThemeFileInfo("recall.gif", ThemeFileInfoFlags.PhaseII)]
		MessageRecall,
		[ThemeFileInfo("recallflr.gif", ThemeFileInfoFlags.PhaseII)]
		RecallReportFailure,
		[ThemeFileInfo("recallsuc.gif", ThemeFileInfoFlags.PhaseII)]
		RecallReportSuccess,
		[ThemeFileInfo("tsk-rcvd.gif", ThemeFileInfoFlags.PhaseII)]
		TaskRequestUpdate,
		[ThemeFileInfo("tsk-dlg.gif", ThemeFileInfoFlags.PhaseII)]
		TaskDelecated,
		[ThemeFileInfo("tsk-rcr.gif", ThemeFileInfoFlags.PhaseII)]
		TaskRecur,
		[ThemeFileInfo("tskreq.gif", ThemeFileInfoFlags.PhaseII)]
		TaskRequest,
		[ThemeFileInfo("tskreq-acc.gif", ThemeFileInfoFlags.PhaseII)]
		TaskAcceptance,
		[ThemeFileInfo("tskreq-dec.gif", ThemeFileInfoFlags.PhaseII)]
		TaskDecline,
		[ThemeFileInfo("apv_rsp_cl_app.gif", ThemeFileInfoFlags.PhaseII)]
		UnreadApprovedResponse,
		[ThemeFileInfo("apv_rsp_cl_rej.gif", ThemeFileInfoFlags.PhaseII)]
		UnreadRegectedResponse,
		[ThemeFileInfo("apv_rsp_op_app.gif", ThemeFileInfoFlags.PhaseII)]
		ReadApprovedResponse,
		[ThemeFileInfo("apv_rsp_op_rej.gif", ThemeFileInfoFlags.PhaseII)]
		ReadRejectedResponse,
		[ThemeFileInfo("apv_rsp_rep_app.gif", ThemeFileInfoFlags.PhaseII)]
		RepliedToApprovedResponse,
		[ThemeFileInfo("apv_rsp_rep_rej.gif", ThemeFileInfoFlags.PhaseII)]
		RepliedToRejectedResponse,
		[ThemeFileInfo("apv_rsp_fwd_app.gif", ThemeFileInfoFlags.PhaseII)]
		ForwardedApprovedResponse,
		[ThemeFileInfo("apv_rsp_fwd_rej.gif", ThemeFileInfoFlags.PhaseII)]
		ForwardedRejectedResponse,
		[ThemeFileInfo("backcolor.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarBackColor,
		[ThemeFileInfo("blockdirltr.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarDirLTR,
		[ThemeFileInfo("blockdirrtl.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarDirRTL,
		[ThemeFileInfo("createlink.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarCreateLink,
		[ThemeFileInfo("customize.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarCustomize,
		[ThemeFileInfo("indent.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarIndent,
		[ThemeFileInfo("indentrtl.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarIndentRTL,
		[ThemeFileInfo("inserthorizontalrule.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarInsertHorizontalRule,
		[ThemeFileInfo("insertorderedlist.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarInsertOrderList,
		[ThemeFileInfo("insertorderedlistrtl.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarInsertOrderedListRTL,
		[ThemeFileInfo("insertunorderedlist.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarInsertUnorderedList,
		[ThemeFileInfo("insertunorderedlistrtl.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarInsertUnorderedListRTL,
		[ThemeFileInfo("justifycenter.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarJustifyCenter,
		[ThemeFileInfo("justifyleft.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarJustfyLeft,
		[ThemeFileInfo("justifyright.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarJustifyRight,
		[ThemeFileInfo("outdent.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarOutdent,
		[ThemeFileInfo("outdentrtl.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarOutdentRTL,
		[ThemeFileInfo("redo.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarRedo,
		[ThemeFileInfo("removeformat.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarRemoveFormat,
		[ThemeFileInfo("strikethrough.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarStrikeThrough,
		[ThemeFileInfo("subscript.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarSubscript,
		[ThemeFileInfo("superscript.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarSuperscript,
		[ThemeFileInfo("undo.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarUndo,
		[ThemeFileInfo("unlink.png", ThemeFileInfoFlags.PhaseII)]
		FormatbarUnlink,
		[ThemeFileInfo("placeholderimage.png", ThemeFileInfoFlags.LooseImage)]
		PlaceholderImage,
		[ThemeFileInfo("VLVShadowTop.png")]
		VLVShadowTop,
		[ThemeFileInfo("RTL-VLVShadowTop.png")]
		VLVShadowTopRTL,
		[ThemeFileInfo("VLVShadowTile.png", ThemeFileInfoFlags.LooseImage)]
		VLVShadowTile,
		[ThemeFileInfo("RTL-VLVShadowTile.png", ThemeFileInfoFlags.LooseImage)]
		VLVShadowTileRTL,
		[ThemeFileInfo("NavDivideTop.png")]
		NavDivideTop,
		[ThemeFileInfo("NavDivideTile.png", ThemeFileInfoFlags.LooseImage)]
		NavDivideTile,
		[ThemeFileInfo("NavDivideBottom.png")]
		NavDivideBottom,
		[ThemeFileInfo("RTL-NavDivideTop.png")]
		NavDivideTopRTL,
		[ThemeFileInfo("RTL-NavDivideTile.png", ThemeFileInfoFlags.LooseImage)]
		NavDivideTileRTL,
		[ThemeFileInfo("RTL-NavDivideBottom.png")]
		NavDivideBottomRTL,
		[ThemeFileInfo("brdcrmb.png")]
		BreadcrumbsArrow,
		[ThemeFileInfo("brdcrmbrtl.png")]
		BreadcrumbsArrowRtl,
		[ThemeFileInfo("rpbottom-t.png", ThemeFileInfoFlags.LooseImage)]
		ReadingPaneBottomDividerTile,
		[ThemeFileInfo("dropshadow-bottom-left.png")]
		DropShadowBottomLeft,
		[ThemeFileInfo("dropshadow-bottom-right.png")]
		DropShadowBottomRight,
		[ThemeFileInfo("dropshadow-corner-bottom-left.png")]
		DropShadowCornerBottomLeft,
		[ThemeFileInfo("dropshadow-corner-bottom-right.png")]
		DropShadowCornerBottomRight,
		[ThemeFileInfo("dropshadow-top-left.png")]
		DropShadowTopLeft,
		[ThemeFileInfo("dropshadow-top-right.png")]
		DropShadowTopRight,
		[ThemeFileInfo("shadow-div-left.png")]
		ShadowDivLeft,
		[ThemeFileInfo("shadow-div-right.png")]
		ShadowDivRight,
		[ThemeFileInfo("shadow-div-tile.png", ThemeFileInfoFlags.LooseImage)]
		ShadowDivTile,
		[ThemeFileInfo("Error-Icon.png")]
		ErrorIcon,
		[ThemeFileInfo("msgannotation.png")]
		MessageAnnotation,
		[ThemeFileInfo]
		Count
	}
}

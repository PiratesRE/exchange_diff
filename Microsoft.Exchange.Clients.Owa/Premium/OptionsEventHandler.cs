using System;
using System.Security;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.InfoWorker.Common.OOF;
using Microsoft.Exchange.UM.ClientAccess;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("Options")]
	internal sealed class OptionsEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(OptionsEventHandler));
		}

		[OwaEvent("IsSmsConfigured")]
		public void IsSmsConfigured()
		{
			ExTraceGlobals.UserOptionsCallTracer.TraceDebug((long)this.GetHashCode(), "OptionsEventHandler.IsSmsConfigured");
			this.Writer.Write(UnifiedMessagingOptions.IsSMSConfigured(base.UserContext) ? 1 : 0);
		}

		[OwaEventParameter("vmPvSnd", typeof(bool), false, true)]
		[OwaEventParameter("vmPvRec", typeof(bool), false, true)]
		[OwaEventParameter("sms", typeof(int), false, true)]
		[OwaEventParameter("tpFdr", typeof(string), false, true)]
		[OwaEvent("SaveUmOpts")]
		[OwaEventSegmentation(Feature.UMIntegration)]
		[OwaEventParameter("tpNum", typeof(string), false, true)]
		[OwaEventParameter("vGr", typeof(bool), false, true)]
		[OwaEventParameter("vSo", typeof(bool), false, true)]
		[OwaEventParameter("mc", typeof(bool), false, true)]
		[OwaEventParameter("plvm", typeof(bool), false, true)]
		public void SaveUnifiedMessagingOptions()
		{
			ExTraceGlobals.UserOptionsCallTracer.TraceDebug((long)this.GetHashCode(), "OptionsEventHandler.SaveUnifiedMessagingOptions");
			using (UMClientCommon umclientCommon = new UMClientCommon(base.UserContext.ExchangePrincipal))
			{
				if (!umclientCommon.IsUMEnabled())
				{
					throw new OwaEventHandlerException("Failed to reset pin because user is not enabled for Unified Messaging", LocalizedStrings.GetNonEncoded(1301599396));
				}
				if (base.IsParameterSet("tpNum"))
				{
					umclientCommon.SetPlayOnPhoneDialString((string)base.GetParameter("tpNum"));
				}
				if (base.IsParameterSet("vGr"))
				{
					umclientCommon.SetOofStatus(!(bool)base.GetParameter("vGr"));
				}
				if (base.IsParameterSet("vSo"))
				{
					umclientCommon.SetUnReadVoiceMailReadingOrder((bool)base.GetParameter("vSo"));
				}
				if (base.IsParameterSet("mc"))
				{
					umclientCommon.SetMissedCallNotificationEnabled((bool)base.GetParameter("mc"));
				}
				if (base.IsParameterSet("plvm"))
				{
					umclientCommon.SetPinlessAccessToVoicemail((bool)base.GetParameter("plvm"));
				}
				if (base.IsParameterSet("sms"))
				{
					umclientCommon.SetSMSNotificationOption((UMSMSNotificationOptions)base.GetParameter("sms"));
				}
				if (base.IsParameterSet("vmPvRec"))
				{
					umclientCommon.SetReceivedVoiceMailPreview((bool)base.GetParameter("vmPvRec"));
				}
				if (base.IsParameterSet("vmPvSnd"))
				{
					umclientCommon.SetSentVoiceMailPreview((bool)base.GetParameter("vmPvSnd"));
				}
				if (base.IsParameterSet("tpFdr"))
				{
					string telephoneAccessFolderEmail = Utilities.ProviderSpecificIdFromStoreObjectId((string)base.GetParameter("tpFdr"));
					umclientCommon.SetTelephoneAccessFolderEmail(telephoneAccessFolderEmail);
				}
			}
		}

		[OwaEventParameter("vGr", typeof(bool), false, false)]
		[OwaEventSegmentation(Feature.UMIntegration)]
		[OwaEventParameter("tpNum", typeof(string), false, false)]
		[OwaEvent("RecUmGr")]
		public void RecordUnifiedMessagingGreeting()
		{
			ExTraceGlobals.UserOptionsCallTracer.TraceDebug((long)this.GetHashCode(), "OptionsEventHandler.RecordUnifiedMessagingGreeting");
			bool flag = (bool)base.GetParameter("vGr");
			string dialString = (string)base.GetParameter("tpNum");
			string lastUMCallId = base.UserContext.LastUMCallId;
			using (UMClientCommon umclientCommon = new UMClientCommon(base.UserContext.ExchangePrincipal))
			{
				if (!umclientCommon.IsPlayOnPhoneEnabled())
				{
					throw new OwaEventHandlerException("Failed to record greeting because user doesn't have Play On Phone Enabled", LocalizedStrings.GetNonEncoded(1385527863));
				}
				if (!string.IsNullOrEmpty(lastUMCallId))
				{
					UMCallState umcallState = UMCallState.Disconnected;
					try
					{
						UMCallInfo callInfo = umclientCommon.GetCallInfo(lastUMCallId);
						umcallState = callInfo.CallState;
					}
					catch (InvalidCallIdException)
					{
						ExTraceGlobals.UnifiedMessagingTracer.TraceDebug((long)this.GetHashCode(), "Failed to get call status from Unified Messaging");
					}
					catch (Exception exception)
					{
						if (!OptionsEventHandler.HandleUMException(exception))
						{
							throw;
						}
					}
					if (umcallState != UMCallState.Disconnected)
					{
						throw new OwaEventHandlerException(LocalizedStrings.GetNonEncoded(460647110), LocalizedStrings.GetNonEncoded(460647110));
					}
					base.UserContext.LastUMCallId = null;
				}
				UMGreetingType greetingType = flag ? UMGreetingType.NormalCustom : UMGreetingType.OofCustom;
				try
				{
					base.UserContext.LastUMCallId = umclientCommon.PlayOnPhoneGreeting(greetingType, dialString);
				}
				catch (Exception exception2)
				{
					if (!OptionsEventHandler.HandleUMException(exception2))
					{
						throw;
					}
				}
			}
		}

		[OwaEvent("ResUmPw")]
		[OwaEventSegmentation(Feature.UMIntegration)]
		public void ResetUnifiedMessagingPassword()
		{
			ExTraceGlobals.UserOptionsCallTracer.TraceDebug((long)this.GetHashCode(), "OptionsEventHandler.ResetUnifiedMessagingPassword");
			using (UMClientCommon umclientCommon = new UMClientCommon(base.UserContext.ExchangePrincipal))
			{
				if (!umclientCommon.IsUMEnabled())
				{
					throw new OwaEventHandlerException("Failed to reset pin because user is not enabled for Unified Messaging", LocalizedStrings.GetNonEncoded(1301599396));
				}
				try
				{
					umclientCommon.ResetPIN();
				}
				catch (ResetPINException innerException)
				{
					throw new OwaEventHandlerException("Unable to Reset PIN in Unified Messaging", LocalizedStrings.GetNonEncoded(51129530), innerException);
				}
			}
		}

		[OwaEventSegmentation(Feature.UMIntegration)]
		[OwaEvent("GetMailTree")]
		public void GetMailFolderTree()
		{
			ExTraceGlobals.UserOptionsCallTracer.TraceDebug((long)this.GetHashCode(), "OptionsEventHandler.GetMailFolderTree");
			FolderTree folderTree = MailboxFolderTree.CreateMailboxFolderTree(base.UserContext, base.UserContext.MailboxSession, FolderTreeRenderType.MailFolderWithoutSearchFolders, true);
			string text = "divFPErr";
			this.Writer.Write("<div id=\"divFPTrR\">");
			Infobar infobar = new Infobar(text, "infobar");
			infobar.Render(this.Writer);
			NavigationHost.RenderTreeRegionDivStart(this.Writer, null);
			NavigationHost.RenderTreeDivStart(this.Writer, "fptree");
			folderTree.ErrDiv = text;
			folderTree.Render(this.Writer);
			NavigationHost.RenderTreeDivEnd(this.Writer);
			NavigationHost.RenderTreeRegionDivEnd(this.Writer);
			this.Writer.Write("</div>");
		}

		[OwaEvent("DsblOof")]
		public void DisableOof()
		{
			ExTraceGlobals.UserOptionsCallTracer.TraceDebug((long)this.GetHashCode(), "OptionsEventHandler.DisableOof");
			UserOofSettings userOofSettings = UserOofSettings.GetUserOofSettings(base.UserContext.MailboxSession);
			userOofSettings.OofState = OofState.Disabled;
			userOofSettings.Save(base.UserContext.MailboxSession);
		}

		[OwaEventParameter("pckCrt", typeof(bool), false, true)]
		[OwaEventParameter("snCrtSub", typeof(string), false, true)]
		[OwaEventParameter("snCrtId", typeof(string), false, true)]
		[OwaEvent("SaveSMimeOpts")]
		[OwaEventParameter("addSig", typeof(bool))]
		[OwaEventParameter("enCntnt", typeof(bool))]
		public void SaveSMimeOptions()
		{
			ExTraceGlobals.UserOptionsCallTracer.TraceDebug((long)this.GetHashCode(), "OptionsEventHandler.SaveSMimeOptions");
			base.UserContext.UserOptions.SmimeEncrypt = (bool)base.GetParameter("enCntnt");
			base.UserContext.UserOptions.SmimeSign = (bool)base.GetParameter("addSig");
			if (base.IsParameterSet("pckCrt"))
			{
				base.UserContext.UserOptions.ManuallyPickCertificate = (bool)base.GetParameter("pckCrt");
				if (base.IsParameterSet("snCrtSub") && base.IsParameterSet("snCrtId"))
				{
					base.UserContext.UserOptions.SigningCertificateSubject = (string)base.GetParameter("snCrtSub");
					base.UserContext.UserOptions.SigningCertificateId = (string)base.GetParameter("snCrtId");
				}
			}
			base.UserContext.UserOptions.CommitChanges();
		}

		[OwaEventParameter("anrFst", typeof(bool))]
		[OwaEventParameter("optAcc", typeof(bool))]
		[OwaEvent("SaveGenOpts")]
		public void SaveGeneralSettingsOptions()
		{
			ExTraceGlobals.UserOptionsCallTracer.TraceDebug((long)this.GetHashCode(), "OptionsEventHandler.SaveGeneralSettingsOptions");
			if (base.UserContext.IsFeatureEnabled(Feature.Contacts))
			{
				base.UserContext.UserOptions.CheckNameInContactsFirst = (bool)base.GetParameter("anrFst");
			}
			base.UserContext.UserOptions.IsOptimizedForAccessibility = (bool)base.GetParameter("optAcc");
			base.UserContext.UserOptions.CommitChanges();
		}

		[OwaEventVerb(OwaEventVerb.Get)]
		[OwaEvent("GetOptsMnu")]
		public void GetOptionsMenu()
		{
			OptionsContextMenu optionsContextMenu = new OptionsContextMenu(base.UserContext);
			optionsContextMenu.Render(this.Writer);
		}

		[OwaEventParameter("thmId", typeof(string))]
		[OwaEvent("SaveThmOpts")]
		public void SaveThemesSettingsOptions()
		{
			ExTraceGlobals.UserOptionsCallTracer.TraceDebug((long)this.GetHashCode(), "OptionsEventHandler.SaveThemesSettingsOptions");
			if (base.UserContext.IsFeatureEnabled(Feature.Themes) && base.IsParameterSet("thmId"))
			{
				string text = (string)base.GetParameter("thmId");
				uint idFromStorageId = ThemeManager.GetIdFromStorageId(text);
				if (idFromStorageId == 4294967295U)
				{
					throw new OwaEventHandlerException("The theme doesn't exist any more in the server", LocalizedStrings.GetNonEncoded(-1332063254));
				}
				if (idFromStorageId == base.UserContext.DefaultTheme.Id)
				{
					base.UserContext.UserOptions.ThemeStorageId = string.Empty;
				}
				else
				{
					base.UserContext.UserOptions.ThemeStorageId = text;
				}
				base.UserContext.Theme = ThemeManager.Themes[(int)((UIntPtr)idFromStorageId)];
				base.UserContext.UserOptions.CommitChanges();
			}
		}

		[OwaEvent("ChangePassword")]
		[OwaEventSegmentation(Feature.ChangePassword)]
		[OwaEventParameter("oldPwd", typeof(string))]
		[OwaEventParameter("newPwd", typeof(string))]
		public void ChangePassword()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "OptionsEventHandler.ChangePassword");
			using (SecureString secureString = Utilities.SecureStringFromString((string)base.GetParameter("oldPwd")))
			{
				using (SecureString secureString2 = Utilities.SecureStringFromString((string)base.GetParameter("newPwd")))
				{
					switch (Utilities.ChangePassword(base.OwaContext.LogonIdentity, secureString, secureString2))
					{
					case Utilities.ChangePasswordResult.InvalidCredentials:
						this.RenderErrorInfobar(LocalizedStrings.GetHtmlEncoded(866665304));
						break;
					case Utilities.ChangePasswordResult.LockedOut:
						this.RenderErrorInfobar(LocalizedStrings.GetHtmlEncoded(-1179631159));
						break;
					case Utilities.ChangePasswordResult.BadNewPassword:
						this.RenderErrorInfobar(LocalizedStrings.GetHtmlEncoded(-782268049));
						break;
					case Utilities.ChangePasswordResult.OtherError:
						this.RenderErrorInfobar(LocalizedStrings.GetHtmlEncoded(-1821890470));
						break;
					}
				}
			}
		}

		[OwaEventParameter("btnSt", typeof(int), false, true)]
		[OwaEvent("SaveFmtBrSt")]
		[OwaEventParameter("mruFnts", typeof(string), false, true)]
		public void SaveFormatBarState()
		{
			ExTraceGlobals.UserOptionsCallTracer.TraceDebug((long)this.GetHashCode(), "OptionsEventHandler.SaveFormatBarState");
			if (base.IsParameterSet("btnSt"))
			{
				base.UserContext.UserOptions.FormatBarState = (FormatBarButtonGroups)base.GetParameter("btnSt");
			}
			if (base.IsParameterSet("mruFnts"))
			{
				base.UserContext.UserOptions.MruFonts = (string)base.GetParameter("mruFnts");
			}
			base.UserContext.UserOptions.CommitChanges();
		}

		[OwaEvent("DisablePont")]
		[OwaEventParameter("pt", typeof(int))]
		public void DisablePont()
		{
			ExTraceGlobals.UserOptionsCallTracer.TraceDebug((long)this.GetHashCode(), "OptionsEventHandler.DisablePont");
			base.UserContext.UserOptions.EnabledPonts &= ~(PontType)base.GetParameter("pt");
			base.UserContext.UserOptions.CommitChanges();
		}

		private static bool HandleUMException(Exception exception)
		{
			if (exception is InvalidObjectIdException)
			{
				throw new OwaEventHandlerException("Failed to play greeting", LocalizedStrings.GetNonEncoded(982875461));
			}
			if (exception is DialingRulesException)
			{
				throw new OwaEventHandlerException("Failed to play greeting", LocalizedStrings.GetNonEncoded(852800569));
			}
			if (exception is IPGatewayNotFoundException)
			{
				throw new OwaEventHandlerException("Failed to play greeting", LocalizedStrings.GetNonEncoded(-600244331));
			}
			if (exception is UMServerNotFoundException)
			{
				throw new OwaEventHandlerException("Failed to play greeting", LocalizedStrings.GetNonEncoded(-620095015));
			}
			if (exception is TransportException)
			{
				throw new OwaEventHandlerException("Failed to play greeting", LocalizedStrings.GetNonEncoded(-2078057245));
			}
			if (exception is InvalidPhoneNumberException)
			{
				throw new OwaEventHandlerException("Failed to play greeting", LocalizedStrings.GetNonEncoded(-74757282));
			}
			if (exception is InvalidSipUriException)
			{
				throw new OwaEventHandlerException("Failed to play greeting", LocalizedStrings.GetNonEncoded(-74757282));
			}
			return false;
		}

		private void RenderErrorInfobar(string messageHtml)
		{
			this.Writer.Write("<div id=eib>");
			this.Writer.Write(messageHtml);
			this.Writer.Write("</div>");
		}

		public const string EventNamespace = "Options";

		public const string MethodSaveUnifiedMessagingOptions = "SaveUmOpts";

		public const string MethodGetUnifiedMessagingOptions = "IsSmsConfigured";

		public const string MethodRecordUnifiedMessagingGreeting = "RecUmGr";

		public const string MethodResetUnifiedMessagingPassword = "ResUmPw";

		public const string MethodGetMailFolderTree = "GetMailTree";

		public const string MethodDisableOof = "DsblOof";

		public const string MethodSaveSMimeOptions = "SaveSMimeOpts";

		public const string MethodSaveGeneralSettingsOptions = "SaveGenOpts";

		public const string MethodSaveFormatBarState = "SaveFmtBrSt";

		public const string MethodGetOptionsMenu = "GetOptsMnu";

		public const string MethodSaveThemesSettingsOptions = "SaveThmOpts";

		public const string MethodDisablePont = "DisablePont";

		public const string MethodChangePassword = "ChangePassword";

		public const string MethodSendMeMailboxLog = "SdLg";

		public const string OldPassword = "oldPwd";

		public const string NewPassword = "newPwd";

		public const string DialString = "tpNum";

		public const string UmOofStatus = "vGr";

		public const string UmUnreadVoiceMailSortOrder = "vSo";

		public const string MissedCall = "mc";

		public const string PinlessVoicemail = "plvm";

		public const string VoicemailPreviewReceive = "vmPvRec";

		public const string VoicemailPreviewSend = "vmPvSnd";

		public const string SMSNotificationOption = "sms";

		public const string TelephoneAccessFolder = "tpFdr";

		public const string EncryptContent = "enCntnt";

		public const string AddSignature = "addSig";

		public const string ManuallyPickCertificate = "pckCrt";

		public const string SigningCertificateSubject = "snCrtSub";

		public const string SigningCertificateId = "snCrtId";

		public const string EmptyDeletedItems = "emDel";

		public const string AnrFirst = "anrFst";

		public const string OptimizeForAccessibility = "optAcc";

		public const string ThemeStorageId = "thmId";

		public const string FormatBarState = "btnSt";

		public const string MruFonts = "mruFnts";

		public const string PontType = "pt";
	}
}

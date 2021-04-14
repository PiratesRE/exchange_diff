using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public class MobileOptions : OptionsBase
	{
		public MobileOptions(OwaContext owaContext, TextWriter writer) : base(owaContext, writer)
		{
			if (!Utilities.IsPostRequest(this.request) || string.IsNullOrEmpty(base.Command))
			{
				return;
			}
			string formParameter = Utilities.GetFormParameter(this.request, "hidptl", true);
			string formParameter2 = Utilities.GetFormParameter(this.request, "hiddtp", true);
			string formParameter3 = Utilities.GetFormParameter(this.request, "hiddid", true);
			DeviceIdentity deviceIdentity = new DeviceIdentity(formParameter3, formParameter2, formParameter);
			if (base.Command == "r")
			{
				DeviceInfo.RemoveDevice(this.userContext.MailboxSession, deviceIdentity, true);
				return;
			}
			if (base.Command == "w")
			{
				this.WipeMobileDevice(deviceIdentity);
				return;
			}
			if (base.Command == "c")
			{
				this.CancelWipe(deviceIdentity);
				return;
			}
			throw new OwaInvalidRequestException("Unknown command");
		}

		public override void Render()
		{
			base.RenderHeaderRow(ThemeFileId.Pda, -1231836625);
			this.writer.Write("<tr><td class=\"bd\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1723095566));
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td class=\"bd\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-50929706));
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td class=\"bd\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(1502785969));
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td class=\"bd\">");
			this.writer.Write("<a href=\"#\" onclick=\"return rmvDvc();\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-324768784));
			this.writer.Write("</a> &nbsp; | &nbsp; <a href=\"#\" id=\"wpDv\" onclick=\"return wpDvc();\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(1706553082));
			this.writer.Write("</a>");
			this.writer.Write("<input type=\"hidden\" id=\"cnclWp\" value=\"");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1844257559));
			this.writer.Write("\">");
			this.writer.Write("<input type=\"hidden\" id=\"cnclBlk\" value=\"");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1844257559));
			this.writer.Write("\">");
			this.writer.Write("<input type=\"hidden\" id=\"wp\" value=\"");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(1706553082));
			this.writer.Write("\">");
			this.writer.Write("<input type=\"hidden\" id=\"blkDv\" value=\"");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(252962010));
			this.writer.Write("\">");
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td class=\"bd\">");
			this.writer.Write("<table id=\"mdTbl\" cellpadding=0 cellspacing=0><col width=20><col width=150><col width=150>");
			this.writer.Write("<tr id=\"mdHd\"><td class=\"l\">&nbsp;</td><td>");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1802232508));
			this.writer.Write("</td><td class=\"m\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1961919607));
			this.writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;");
			this.writer.Write("<img src=\"");
			this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.BasicSortDescending);
			this.writer.Write("\"></td><td>");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1233897451));
			this.writer.Write("</td></tr>");
			this.RenderDevices();
			this.writer.Write("<tr id=\"mdFt\"><td class=\"l\">&nbsp;</td><td>&nbsp;</td><td class=\"m\">&nbsp;</td><td class=\"r\">&nbsp;</td></tr>");
			this.writer.Write("</table>");
			this.writer.Write("</td></tr>");
			this.writer.Write("<tr><td class=\"bd\">");
			this.writer.Write("</td></tr>");
			this.RenderHiddenInput("hidptl");
			this.RenderHiddenInput("hiddtp");
			this.RenderHiddenInput("hiddid");
		}

		public override void RenderScript()
		{
			RenderingUtilities.RenderStringVariable(this.writer, "L_CfRmDv", -642460312);
			RenderingUtilities.RenderStringVariable(this.writer, "L_CfBlkDv", 1558810407);
			RenderingUtilities.RenderStringVariable(this.writer, "L_CfWpDt", 2018287373);
			RenderingUtilities.RenderStringVariable(this.writer, "L_RmWp", -1890179315);
		}

		private static void RenderDeviceStatusColon(Strings.IDs name, string value, TextWriter writer)
		{
			if (!string.IsNullOrEmpty(value))
			{
				writer.Write("<br><br>");
				writer.Write(LocalizedStrings.GetHtmlEncoded(name));
				writer.Write(" ");
				Utilities.HtmlEncode(value, writer);
			}
		}

		private void RenderDevices()
		{
			DeviceInfo[] allDeviceInfo = DeviceInfo.GetAllDeviceInfo(this.userContext.MailboxSession, MobileClientType.EAS | MobileClientType.MOWA);
			if (allDeviceInfo != null)
			{
				Array.Sort(allDeviceInfo, new MobileOptions.SortByLastSyncTime());
				string weekdayDateTimeFormat = this.userContext.UserOptions.GetWeekdayDateTimeFormat(false);
				for (int i = allDeviceInfo.Length - 1; i >= 0; i--)
				{
					DeviceInfo deviceInfo = allDeviceInfo[i];
					string value = i.ToString();
					this.writer.Write("<tr class=\"data\"><td class=\"l\">");
					this.writer.Write("<input name=\"device\" type=\"radio\" value=\"");
					this.writer.Write(value);
					this.writer.Write("\" onclick=\"onClkMd(this)\">");
					this.writer.Write("<input type=\"hidden\" id=\"ptl");
					this.writer.Write(value);
					this.writer.Write("\" value=\"");
					Utilities.HtmlEncode(deviceInfo.DeviceIdentity.Protocol, this.writer);
					this.writer.Write("\"><input type=\"hidden\" id=\"dtp");
					this.writer.Write(value);
					this.writer.Write("\" value=\"");
					Utilities.HtmlEncode(deviceInfo.DeviceIdentity.DeviceType, this.writer);
					this.writer.Write("\"><input type=\"hidden\" id=\"did");
					this.writer.Write(value);
					this.writer.Write("\" value=\"");
					Utilities.HtmlEncode(deviceInfo.DeviceIdentity.DeviceId, this.writer);
					this.writer.Write("\"><input type=\"hidden\" id=\"wpSts");
					this.writer.Write(value);
					this.writer.Write("\" value=\"");
					this.writer.Write(this.GetWipeStatus(deviceInfo));
					this.writer.Write("\"><input type=\"hidden\" id=\"wpSpt");
					this.writer.Write(value);
					this.writer.Write("\" value=\"");
					this.writer.Write(deviceInfo.IsRemoteWipeSupported ? "1" : "0");
					this.writer.Write("\"></td><td>");
					Utilities.HtmlEncode(deviceInfo.DeviceIdentity.DeviceType, this.writer);
					this.writer.Write("</td><td class=\"m\">");
					if (deviceInfo.LastSyncAttemptTime != null)
					{
						Utilities.HtmlEncode(this.userContext.TimeZone.ConvertDateTime(deviceInfo.LastSyncAttemptTime.Value).ToString(weekdayDateTimeFormat), this.writer);
					}
					this.writer.Write("</td><td class=\"r\">");
					if (deviceInfo.WipeAckTime != null)
					{
						this.writer.Write("<font color=\"green\">");
						this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1509940370));
						this.writer.Write("</font>");
						this.writer.Write(LocalizedStrings.GetHtmlEncoded(1484375142));
						this.writer.Write("<br> <font color=\"red\"> <b>");
						this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1104574967));
						this.writer.Write(" </b>");
						this.writer.Write(LocalizedStrings.GetHtmlEncoded(-827928519));
						this.writer.Write("</font>");
					}
					else
					{
						if (deviceInfo.WipeRequestTime != null || deviceInfo.WipeSentTime != null)
						{
							this.writer.Write("<font color=\"red\">");
							this.writer.Write(deviceInfo.IsRemoteWipeSupported ? LocalizedStrings.GetHtmlEncoded(-737505483) : LocalizedStrings.GetHtmlEncoded(-306081826));
							this.writer.Write("</font> ");
							this.writer.Write(LocalizedStrings.GetHtmlEncoded((deviceInfo.WipeSentTime != null) ? -1560055325 : 1150686522));
							this.writer.Write(" ");
							ExDateTime exDateTime = (deviceInfo.WipeSentTime != null) ? deviceInfo.WipeSentTime.Value : deviceInfo.WipeRequestTime.Value;
							this.writer.Write(exDateTime.ToString(weekdayDateTimeFormat));
							if (deviceInfo.IsRemoteWipeSupported)
							{
								this.writer.Write("<br><b>");
								this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1104574967));
								this.writer.Write(" </b>");
								this.writer.Write(LocalizedStrings.GetHtmlEncoded(1704998644));
							}
						}
						else
						{
							this.writer.Write(LocalizedStrings.GetHtmlEncoded(2041362128));
						}
						if (deviceInfo.FirstSyncTime != null)
						{
							ExDateTime exDateTime2 = this.userContext.TimeZone.ConvertDateTime(deviceInfo.FirstSyncTime.Value);
							this.writer.Write("<br><br>");
							this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1006820371));
							this.writer.Write(" ");
							this.writer.Write(exDateTime2.ToString(weekdayDateTimeFormat));
						}
						MobileOptions.RenderDeviceStatusColon(-433064091, deviceInfo.DeviceFriendlyName, this.writer);
						MobileOptions.RenderDeviceStatusColon(-1924890268, deviceInfo.DeviceIdentity.DeviceId, this.writer);
						MobileOptions.RenderDeviceStatusColon(-538137132, deviceInfo.DeviceModel, this.writer);
						MobileOptions.RenderDeviceStatusColon(1804230700, deviceInfo.DevicePhoneNumber, this.writer);
						MobileOptions.RenderDeviceStatusColon(-2089297643, deviceInfo.DeviceOS, this.writer);
						MobileOptions.RenderDeviceStatusColon(-1969259845, deviceInfo.DeviceOSLanguage, this.writer);
						MobileOptions.RenderDeviceStatusColon(-390457951, deviceInfo.DeviceImei, this.writer);
						MobileOptions.RenderDeviceStatusColon(1030256077, deviceInfo.UserAgent, this.writer);
					}
					this.writer.Write("</td></tr>");
				}
			}
		}

		private string GetWipeStatus(DeviceInfo device)
		{
			if (device.WipeAckTime != null)
			{
				return "3";
			}
			if (device.WipeSentTime != null)
			{
				return "2";
			}
			if (device.WipeRequestTime != null)
			{
				return "1";
			}
			return "0";
		}

		private void WipeMobileDevice(DeviceIdentity deviceIdentity)
		{
			using (SyncStateStorage syncStateStorage = SyncStateStorage.Bind(this.userContext.MailboxSession, deviceIdentity, null))
			{
				if (syncStateStorage == null)
				{
					throw new OwaInvalidRequestException("Attempting to wipe device that does not exist.");
				}
				DeviceInfo.StartRemoteWipe(syncStateStorage, ExDateTime.UtcNow, this.userContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
			}
		}

		private void CancelWipe(DeviceIdentity deviceIdentity)
		{
			using (SyncStateStorage syncStateStorage = SyncStateStorage.Bind(this.userContext.MailboxSession, deviceIdentity, null))
			{
				if (syncStateStorage == null)
				{
					throw new OwaInvalidRequestException("Attempting to cancel wipe on device that does not exist.");
				}
				DeviceInfo.CancelRemoteWipe(syncStateStorage);
			}
		}

		private void RenderHiddenInput(string name)
		{
			this.writer.Write("<input type=\"hidden\" name=\"");
			this.writer.Write(name);
			this.writer.Write("\" value=\"\">");
		}

		private const string ProtocolParameter = "hidptl";

		private const string DeviceTypeParameter = "hiddtp";

		private const string DeviceIdParameter = "hiddid";

		private const string RemoveCommand = "r";

		private const string WipeCommand = "w";

		private const string CancelWipeCommand = "c";

		private sealed class SortByLastSyncTime : IComparer
		{
			int IComparer.Compare(object x, object y)
			{
				DeviceInfo deviceInfo = x as DeviceInfo;
				DeviceInfo deviceInfo2 = y as DeviceInfo;
				if (deviceInfo == null && deviceInfo2 == null)
				{
					return 0;
				}
				if (deviceInfo.LastSyncAttemptTime == null && deviceInfo2.LastSyncAttemptTime == null)
				{
					return 0;
				}
				if (deviceInfo == null || deviceInfo.LastSyncAttemptTime == null)
				{
					return -1;
				}
				if (deviceInfo2 == null || deviceInfo2.LastSyncAttemptTime == null)
				{
					return 1;
				}
				return ExDateTime.Compare(deviceInfo.LastSyncAttemptTime.Value, deviceInfo2.LastSyncAttemptTime.Value);
			}
		}
	}
}

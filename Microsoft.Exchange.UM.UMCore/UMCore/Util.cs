using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.FaultInjection;
using Microsoft.Exchange.UM.UMCommon.MessageContent;
using Microsoft.Exchange.UM.UMCore.Exceptions;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.UM.UMCore
{
	internal static class Util
	{
		public static bool TryDeleteFile(string filename)
		{
			try
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, 0, "Deleting file {0}", new object[]
				{
					filename
				});
				File.Delete(filename);
				return true;
			}
			catch (IOException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, 0, "Failed to delete the file {0}. Exception : {1}", new object[]
				{
					filename,
					ex
				});
			}
			catch (UnauthorizedAccessException ex2)
			{
				CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, 0, "Failed to delete the file {0}. Exception : {1}", new object[]
				{
					filename,
					ex2
				});
			}
			return false;
		}

		internal static bool VerifyServerIsInDialPlan(UMServer server, Guid dialPlanGuid, bool throwException)
		{
			if (VariantConfiguration.InvariantNoFlightingSnapshot.UM.ServerDialPlanLink.Enabled && dialPlanGuid.Equals(Guid.Empty))
			{
				throw new ArgumentException("Dialplan guid cannot be Empty for non datacenter scenarios");
			}
			if (VariantConfiguration.InvariantNoFlightingSnapshot.UM.ServerDialPlanLink.Enabled)
			{
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromRootOrg(true);
				UMDialPlan dialPlanFromId = iadsystemConfigurationLookup.GetDialPlanFromId(new ADObjectId(dialPlanGuid));
				return Util.VerifyServerLinkedToDialPlan(server, dialPlanFromId, throwException);
			}
			return true;
		}

		internal static bool VerifyServerLinkedToDialPlan(UMServer server, UMDialPlan dialPlan, bool throwException)
		{
			if (Utils.IsUMServerLinkedToDialplan(server, dialPlan))
			{
				return true;
			}
			if (throwException)
			{
				throw CallRejectedException.Create(Strings.ServerNotAssociatedWithDialPlan(dialPlan.Id.ToString()), CallEndingReason.DialPlanNotLinked, "UM dial plan: {0}.", new object[]
				{
					dialPlan.Id.ToString()
				});
			}
			return false;
		}

		internal static bool MaxCallLimitExceeded()
		{
			bool flag;
			return Util.MaxCallLimitExceeded(out flag);
		}

		internal static bool MaxCallLimitExceeded(out bool lowWarning)
		{
			lowWarning = false;
			long rawValue = GeneralCounters.CurrentCalls.RawValue;
			FaultInjectionUtils.FaultInjectChangeValue<long>(3676712253U, ref rawValue);
			if (CommonConstants.MaxCallsAllowed != null)
			{
				float num = (float)rawValue / (float)CommonConstants.MaxCallsAllowed.Value;
				if (num == float.NaN || num > 0.8f)
				{
					lowWarning = true;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.DiagnosticTracer, 0, "Util::AvailableToTakeCalls() ActiveCalls = {0} MaxCalls = {1} LowWarning = {2}", new object[]
				{
					rawValue,
					CommonConstants.MaxCallsAllowed.Value,
					lowWarning
				});
				return CommonConstants.MaxCallsAllowed == 0 || rawValue > (long)CommonConstants.MaxCallsAllowed.Value;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.DiagnosticTracer, 0, "Util::AvailableToTakeCalls() ActiveCalls = {0} MaxCalls = {1} LowWarning = {2}", new object[]
			{
				rawValue,
				"not set",
				lowWarning
			});
			return false;
		}

		internal static void GetServerSecurityDescriptor(ref ActiveDirectorySecurity sd)
		{
			ADTopologyLookup adtopologyLookup = ADTopologyLookup.CreateLocalResourceForestLookup();
			Server serverFromName = adtopologyLookup.GetServerFromName(Utils.GetLocalHostName());
			RawSecurityDescriptor rawSecurityDescriptor = serverFromName.ReadSecurityDescriptor();
			sd = new ActiveDirectorySecurity();
			byte[] array = new byte[rawSecurityDescriptor.BinaryLength];
			rawSecurityDescriptor.GetBinaryForm(array, 0);
			sd.SetSecurityDescriptorBinaryForm(array);
			SecurityIdentifier identity = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
			ActiveDirectoryAccessRule rule = new ActiveDirectoryAccessRule(identity, ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.Self | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow);
			sd.AddAccessRule(rule);
		}

		internal static FileSecurity GetAllowExchangeServerSecurity()
		{
			FileSecurity fileSecurity = new FileSecurity();
			IRootOrganizationRecipientSession rootOrganizationRecipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 275, "GetAllowExchangeServerSecurity", "f:\\15.00.1497\\sources\\dev\\um\\src\\umcore\\utility.cs");
			SecurityIdentifier exchangeServersUsgSid = rootOrganizationRecipientSession.GetExchangeServersUsgSid();
			FileSystemAccessRule fileSystemAccessRule = new FileSystemAccessRule(exchangeServersUsgSid, FileSystemRights.ReadData, AccessControlType.Allow);
			fileSecurity.SetAccessRule(fileSystemAccessRule);
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
			fileSystemAccessRule = new FileSystemAccessRule(securityIdentifier, FileSystemRights.ReadData, AccessControlType.Allow);
			fileSecurity.AddAccessRule(fileSystemAccessRule);
			fileSecurity.SetOwner(securityIdentifier);
			return fileSecurity;
		}

		internal static bool GetOperatorExtension(CallContext ctx, out PhoneNumber operatorExtension)
		{
			bool result = false;
			operatorExtension = null;
			switch (ctx.CallType)
			{
			case 1:
			case 3:
				result = PhoneNumber.TryParse(ctx.DialPlan.OperatorExtension, out operatorExtension);
				break;
			case 2:
				result = CommonUtil.GetOperatorExtensionForAutoAttendant(ctx.AutoAttendantInfo, ctx.CurrentAutoAttendantSettings, ctx.DialPlan, true, out operatorExtension);
				break;
			}
			return result;
		}

		internal static PhoneNumber SA_GetOperatorNumber(UMDialPlan originatingDialPlan, UMSubscriber targetUser)
		{
			PhoneNumber phoneNumber = null;
			if (targetUser != null)
			{
				UMMailbox adummailboxSettings = targetUser.ADUMMailboxSettings;
				if (!Util.TryGetDialableNumber(adummailboxSettings.OperatorNumber, originatingDialPlan, null, out phoneNumber))
				{
					PIIMessage data = PIIMessage.Create(PIIType._EmailAddress, targetUser.MailAddress);
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, null, data, "UMMailbox(_EmailAddress).OperatorNumber({0}) cannot be dialed.", new object[]
					{
						adummailboxSettings.OperatorNumber
					});
				}
			}
			if (phoneNumber == null && !Util.TryGetDialableNumber(originatingDialPlan.OperatorExtension, originatingDialPlan, null, out phoneNumber))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, null, "UMDialPlan({0}).OperatorExtension({1}) cannot be dialed.", new object[]
				{
					originatingDialPlan,
					originatingDialPlan.OperatorExtension
				});
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, null, "UMDialPlan({0}).OperatorExtension({1}) returning dialable operator number {2}", new object[]
			{
				originatingDialPlan,
				originatingDialPlan.OperatorExtension,
				(phoneNumber != null) ? phoneNumber.Number : "<null>"
			});
			return phoneNumber;
		}

		internal static string BuildAttachmentName(string extension, int seconds, AttachmentCollection ac, CultureInfo culture, AudioCodecEnum codec, bool protectedContent)
		{
			if (seconds <= 0)
			{
				return null;
			}
			int num = 100;
			int num2 = 0;
			string duration = Util.BuildDurationString(seconds, culture);
			string str = protectedContent ? AudioCodec.GetFileExtensionForProtectedContent(codec) : AudioCodec.GetFileExtension(codec);
			string text = Strings.AttachmentName(extension, duration).ToString(culture) + str;
			if (ac != null)
			{
				while (Util.MatchesExistingAttachment(text, ac) && --num > 0)
				{
					text = Strings.AttachmentNameWithNumber(extension, duration, ++num2).ToString(culture) + str;
				}
			}
			return Attachment.TrimFilename(text);
		}

		internal static bool MatchesExistingAttachment(string candidate, AttachmentCollection collection)
		{
			foreach (AttachmentHandle handle in collection)
			{
				using (Attachment attachment = collection.Open(handle))
				{
					if (string.Compare(attachment.FileName, candidate, true, CultureInfo.InvariantCulture) == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static string BuildDurationString(int seconds, CultureInfo culture)
		{
			return Util.BuildDurationString(seconds / 60, seconds % 60, culture);
		}

		internal static string SanitizeStringForSayAs(string inputString)
		{
			return inputString.Replace("\r\n", " ");
		}

		internal static string AddProsodyWithVolume(CultureInfo culture, string text)
		{
			return string.Concat(new object[]
			{
				"<prosody volume=\"",
				TTSVolumeMap.GetVolume(culture),
				"\">",
				text,
				"</prosody>"
			});
		}

		internal static void IncrementCounter(ExPerformanceCounter counter)
		{
			Util.IncrementCounter(counter, 1L);
		}

		internal static void IncrementCounter(ExPerformanceCounter counter, long count)
		{
			if (UmServiceGlobals.ArePerfCountersEnabled)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "Updating counter '{0}' from {1} to {2}.", new object[]
				{
					counter.CounterName,
					counter.RawValue,
					counter.RawValue + count
				});
				counter.IncrementBy(count);
			}
		}

		internal static void DecrementCounter(ExPerformanceCounter counter)
		{
			Util.DecrementCounter(counter, 1L);
		}

		internal static void DecrementCounter(ExPerformanceCounter counter, long count)
		{
			Util.IncrementCounter(counter, -count);
		}

		internal static void SetCounter(ExPerformanceCounter counter, long value)
		{
			if (UmServiceGlobals.ArePerfCountersEnabled)
			{
				counter.RawValue = value;
			}
		}

		internal static string WavPathFromCulture(CultureInfo culture)
		{
			return Path.Combine(AppConfig.Instance.WaveDirectory.WaveDir, Utils.GetPromptDirectoryNameForCulture(culture));
		}

		internal static bool IsDiskSpaceAvailable(out long availableFreeDiskSpace, out bool lowWarning)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(Utils.VoiceMailFilePath);
			lowWarning = false;
			availableFreeDiskSpace = 0L;
			bool result;
			try
			{
				DriveInfo driveInfo = new DriveInfo(directoryInfo.Root.Name);
				long num = VariantConfiguration.InvariantNoFlightingSnapshot.UM.VoicemailDiskSpaceDatacenterLimit.Enabled ? 50L : 500L;
				long num2 = num * 1024L * 1024L;
				long num3 = 786432000L;
				availableFreeDiskSpace = driveInfo.AvailableFreeSpace;
				FaultInjectionUtils.FaultInjectChangeValue<long>(2871405885U, ref availableFreeDiskSpace);
				if (availableFreeDiskSpace < num2)
				{
					CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, 0, "Low disk space. Current free disk space is {0} B, at least {1} B are needed.", new object[]
					{
						availableFreeDiskSpace,
						num2
					});
					result = false;
				}
				else if (availableFreeDiskSpace < num3)
				{
					CallIdTracer.TraceWarning(ExTraceGlobals.UtilTracer, 0, "Warning: disk space.getting low. Current free disk space is {0} B, at least {1} B are needed.", new object[]
					{
						availableFreeDiskSpace,
						num2
					});
					result = true;
				}
				else
				{
					result = true;
				}
			}
			catch (Exception ex)
			{
				if (!GrayException.IsGrayException(ex))
				{
					throw;
				}
				CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, 0, "Could not find out the free disk space. Exception {0}", new object[]
				{
					ex.Message
				});
				result = false;
			}
			return result;
		}

		internal static string GetTranscriptionGrammarDir(CultureInfo culture)
		{
			string programFilesx = Util.GetProgramFilesx86();
			string text = string.Format(CultureInfo.InvariantCulture, "Common Files\\microsoft shared\\Speech\\Tokens\\SR_MS_{0}_TRANS_11.0\\Grammars", new object[]
			{
				culture.ToString()
			});
			string text2 = Path.Combine(programFilesx, text);
			CallIdTracer.TraceDebug(ExTraceGlobals.DiagnosticTracer, 0, "GetTranscriptionGrammarDir combined '{0}' and '{1}' into '{2}'", new object[]
			{
				programFilesx,
				text,
				text2
			});
			return text2;
		}

		internal static bool IsSpeechCulture(CultureInfo culture)
		{
			foreach (CultureInfo obj in GlobCfg.VuiCultures)
			{
				if (culture.Equals(obj))
				{
					return true;
				}
			}
			return false;
		}

		internal static string GetProgramFilesx86()
		{
			if (8 == IntPtr.Size || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432")))
			{
				return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
			}
			return Environment.GetEnvironmentVariable("ProgramFiles");
		}

		internal static CultureInfo GetDefaultCulture(UMDialPlan dialPlan)
		{
			if (UmCultures.IsPromptCultureAvailable(dialPlan.DefaultLanguage.Culture))
			{
				return dialPlan.DefaultLanguage.Culture;
			}
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_DialPlanDefaultLanguageNotFound, dialPlan.DefaultLanguage.Name, new object[]
			{
				dialPlan.Name,
				dialPlan.DefaultLanguage.DisplayName
			});
			return CommonConstants.DefaultCulture;
		}

		internal static bool UseAsrAutoAttendant(UMAutoAttendant autoAttendant)
		{
			bool speechEnabled = autoAttendant.SpeechEnabled;
			if (!speechEnabled)
			{
				return false;
			}
			UMLanguage language = autoAttendant.Language;
			CultureInfo culture = language.Culture;
			bool flag = Util.IsSpeechCulture(culture);
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, null, "UseAsrAutoAttendant:: GlobCfg.AsrOverride={0} aa.SpeechEnabled={1} aa.Language={4} aa.Culture=\"{2}\" IsSpeechCulture={3}.", new object[]
			{
				GlobCfg.AsrOverride,
				autoAttendant.SpeechEnabled,
				culture,
				flag,
				language
			});
			return speechEnabled && flag;
		}

		internal static DisambiguationFieldEnum GetDisambiguationField(CallContext callContext)
		{
			UMAutoAttendant autoAttendantInfo = callContext.AutoAttendantInfo;
			UMDialPlan dialPlan = callContext.DialPlan;
			if (callContext.CallType != 2)
			{
				return dialPlan.MatchedNameSelectionMethod;
			}
			DisambiguationFieldEnum result;
			switch (autoAttendantInfo.MatchedNameSelectionMethod)
			{
			case AutoAttendantDisambiguationFieldEnum.Title:
				result = DisambiguationFieldEnum.Title;
				break;
			case AutoAttendantDisambiguationFieldEnum.Department:
				result = DisambiguationFieldEnum.Department;
				break;
			case AutoAttendantDisambiguationFieldEnum.Location:
				result = DisambiguationFieldEnum.Location;
				break;
			case AutoAttendantDisambiguationFieldEnum.None:
				result = DisambiguationFieldEnum.None;
				break;
			case AutoAttendantDisambiguationFieldEnum.PromptForAlias:
				result = DisambiguationFieldEnum.PromptForAlias;
				break;
			case AutoAttendantDisambiguationFieldEnum.InheritFromDialPlan:
				result = dialPlan.MatchedNameSelectionMethod;
				break;
			default:
				CallIdTracer.TraceError(ExTraceGlobals.AutoAttendantTracer, null, "Unexpected MatchedNameSelectionMethod [{0}] on autoattendant.", new object[]
				{
					autoAttendantInfo.MatchedNameSelectionMethod
				});
				throw new UnexpectedSwitchValueException(autoAttendantInfo.MatchedNameSelectionMethod.ToString());
			}
			return result;
		}

		internal static bool UseAsrMenus(UMSubscriber user, UMDialPlan dialplan)
		{
			bool flag = GlobCfg.AsrOverride || user.UseASR;
			return flag && Util.IsSpeechCulture(user.TelephonyCulture);
		}

		internal static string TextNormalize(string text)
		{
			return Util.Normalize(text, Constants.RegularExpressions.TextNormalizer);
		}

		internal static string EmailNormalize(string text)
		{
			return Util.Normalize(text, Constants.RegularExpressions.EmailNormalizer);
		}

		internal static string Normalize(string text, Regex regex)
		{
			MatchCollection matchCollection = regex.Matches(text);
			if (matchCollection.Count == 0)
			{
				return text;
			}
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			int num = 0;
			foreach (object obj in matchCollection)
			{
				Match match = (Match)obj;
				int num2 = match.Index - num;
				if (num2 >= 0)
				{
					text.Substring(num, num2);
					stringBuilder.Append(text.Substring(num, num2));
					if (match.Groups["fromHeader"] != null && match.Groups["fromHeader"].Success)
					{
						stringBuilder.Append(match.Groups["fromHeader"].Value);
						stringBuilder.Append("\r\n");
					}
					else if (match.Groups["basicURL"] != null && match.Groups["basicURL"].Success)
					{
						stringBuilder.Append(match.Groups["basicURL"].Value.TrimEnd(new char[]
						{
							'/'
						}));
					}
					else if ((match.Groups["cidURL"] == null || !match.Groups["cidURL"].Success) && match.Groups["nuanceHack"] != null)
					{
						bool success = match.Groups["nuanceHack"].Success;
					}
					num = match.Index + match.Length;
				}
			}
			stringBuilder.Append(text.Substring(num));
			return stringBuilder.ToString();
		}

		internal static PhoneNumber GetNumberToDial(UMSubscriber caller, ContactInfo contact)
		{
			PhoneNumber result = null;
			if (caller == null || contact == null)
			{
				return null;
			}
			PhoneNumber rawNumber = null;
			if (contact.ADOrgPerson != null)
			{
				PhoneNumber phoneNumber = null;
				if (DialPermissions.GetBestOfficeNumber(contact.ADOrgPerson, caller.DialPlan, out phoneNumber))
				{
					rawNumber = phoneNumber;
				}
			}
			else
			{
				PhoneNumber.TryParse(contact.BusinessPhone, out rawNumber);
			}
			if (Util.TryGetDialableNumber(rawNumber, caller, contact, out result))
			{
				return result;
			}
			if (Util.TryGetDialableNumber(contact.MobilePhone, caller, contact, out result))
			{
				return result;
			}
			if (Util.TryGetDialableNumber(contact.HomePhone, caller, contact, out result))
			{
				return result;
			}
			return null;
		}

		internal static bool TryGetDialableNumber(string phoneNumberString, UMDialPlan originatingDialPlan, UMDialPlan targetDialPlan, out PhoneNumber dialableNumber)
		{
			PhoneNumber rawNumber = null;
			dialableNumber = null;
			return PhoneNumber.TryParse(phoneNumberString, out rawNumber) && Util.TryGetDialableNumber(rawNumber, originatingDialPlan, targetDialPlan, out dialableNumber);
		}

		internal static bool TryGetDialableNumber(PhoneNumber rawNumber, UMDialPlan originatingDialPlan, UMDialPlan targetDialPlan, out PhoneNumber dialableNumber)
		{
			bool result = false;
			dialableNumber = null;
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, null, "TryGetDialableNumber(Number=({0}) OriginatingDP=({1}) targetDP=({2}))", new object[]
			{
				rawNumber,
				(originatingDialPlan != null) ? originatingDialPlan.Name : "<null>",
				(targetDialPlan != null) ? targetDialPlan.Name : "<null>"
			});
			if (rawNumber == null)
			{
				return false;
			}
			PhoneNumber phoneNumber = DialPermissions.Canonicalize(rawNumber, originatingDialPlan, targetDialPlan);
			if (phoneNumber != null)
			{
				result = DialPermissions.Check(phoneNumber, originatingDialPlan, targetDialPlan, out dialableNumber);
			}
			return result;
		}

		internal static bool IsDialableNumber(PhoneNumber phoneNumber, BaseUMCallSession vo, ADRecipient recipient)
		{
			bool result = false;
			PIIMessage[] data = new PIIMessage[]
			{
				PIIMessage.Create(PIIType._PhoneNumber, phoneNumber),
				PIIMessage.Create(PIIType._UserDisplayName, (recipient != null) ? recipient.DisplayName : "<null>")
			};
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, null, data, "IsDialableNumber(Number=(_PhoneNumber) OriginatingDP=({0}) recipient=(_UserDisplayName))", new object[]
			{
				vo.CurrentCallContext.DialPlan
			});
			if (phoneNumber != null)
			{
				DialingPermissionsCheck dialingPermissionsCheck = DialingPermissionsCheckFactory.Create(vo);
				DialingPermissionsCheck.DialingPermissionsCheckResult dialingPermissionsCheckResult;
				if (recipient == null)
				{
					dialingPermissionsCheckResult = dialingPermissionsCheck.CheckPhoneNumber(phoneNumber);
				}
				else
				{
					dialingPermissionsCheckResult = dialingPermissionsCheck.CheckDirectoryUser(recipient, phoneNumber);
				}
				result = dialingPermissionsCheckResult.AllowCall;
			}
			return result;
		}

		internal static bool IsSipUriValid(string uri)
		{
			PlatformSipUri platformSipUri;
			return Platform.Builder.TryCreateSipUri("SIP:" + uri, out platformSipUri);
		}

		internal static bool TryParseDiversionHeader(string headerValue, out string originalCalledParty)
		{
			originalCalledParty = null;
			if (string.IsNullOrEmpty(headerValue))
			{
				return false;
			}
			Match match = Constants.DiversionRegex.Match(headerValue);
			if (match.Success)
			{
				originalCalledParty = match.Groups["number"].Value;
			}
			return match.Success;
		}

		internal static Guid GetTenantGuid(PlatformSipUri requestUri)
		{
			ValidateArgument.NotNull(requestUri, "requestUri");
			Guid empty = Guid.Empty;
			if (CommonConstants.UseDataCenterCallRouting)
			{
				string text = requestUri.FindParameter("ms-organization-guid");
				if (!GuidHelper.TryParseGuid(text, out empty) || empty == Guid.Empty)
				{
					throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.MsOrganizationGuidRequired, "ms-organization-guid = '{0}'", new object[]
					{
						text ?? "<null>"
					});
				}
			}
			return empty;
		}

		internal static string GetTenantName(ADObject adObject)
		{
			if (adObject != null && adObject.OrganizationId.OrganizationalUnit != null)
			{
				return adObject.OrganizationId.OrganizationalUnit.Name;
			}
			return string.Empty;
		}

		internal static string FormatE164Number(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return input;
			}
			if (!input.StartsWith("+", StringComparison.Ordinal))
			{
				input = "+" + input;
			}
			return input;
		}

		internal static int GetLocalServerRedirectPort(bool callIsSecured)
		{
			int result;
			if (!callIsSecured)
			{
				result = UMRecyclerConfig.TcpListeningPort;
			}
			else
			{
				result = UMRecyclerConfig.TlsListeningPort;
			}
			return result;
		}

		internal static string GetProductVersion()
		{
			return GlobCfg.ProductVersion;
		}

		internal static bool PingServerWithRpc(Server server, Guid dialPlanGuid, string fakeServerName)
		{
			bool flag = false;
			if (Utils.RunningInTestMode && server.Name == fakeServerName)
			{
				return true;
			}
			int num = 2;
			while (num-- > 0)
			{
				try
				{
					using (UMServerPingRpcClient umserverPingRpcClient = new UMServerPingRpcClient(server.Fqdn))
					{
						umserverPingRpcClient.SetTimeOut(3000);
						umserverPingRpcClient.Ping(dialPlanGuid, ref flag);
						CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, 0, "Util::PingServerWithRpc() Server {0} DialPlan {1} AvailableToTakeCalls = {2}", new object[]
						{
							server.Fqdn,
							dialPlanGuid,
							flag
						});
						break;
					}
				}
				catch (RpcException ex)
				{
					CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, 0, "Util::PingServerWithRpc() Server {0} DialPlan {1} Error = {2}", new object[]
					{
						server.Fqdn,
						dialPlanGuid,
						ex
					});
					if (num == 0 || (ex.ErrorCode != 1753 && ex.ErrorCode != 1727))
					{
						break;
					}
				}
			}
			return flag;
		}

		internal static void AddTlsErrorEventLogEntry(ExEventLog.EventTuple eventTuple, X509Certificate remoteCertificate, IPEndPoint remoteEndPoint, IPEndPoint localEndPoint, string description)
		{
			string obj = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			if (remoteCertificate != null)
			{
				X509Certificate2 x509Certificate = new X509Certificate2(remoteCertificate);
				obj = x509Certificate.Thumbprint;
				foreach (string value in TlsCertificateInfo.GetFQDNs(x509Certificate))
				{
					stringBuilder.Append(value);
					stringBuilder.Append(" ");
				}
			}
			UmGlobals.ExEvent.LogEvent(eventTuple, null, new object[]
			{
				CommonUtil.ToEventLogString(description),
				CommonUtil.ToEventLogString(obj),
				CommonUtil.ToEventLogString(stringBuilder.ToString().Trim()),
				CommonUtil.ToEventLogString(remoteEndPoint),
				CommonUtil.ToEventLogString(localEndPoint)
			});
		}

		internal static string GenerateMessageIdFromSeed(object seed)
		{
			Random random = new Random(seed.GetHashCode());
			byte[] array = new byte[16];
			random.NextBytes(array);
			Guid guid = new Guid(array);
			return guid.ToString();
		}

		internal static string FormatDiagnosticsInfoRedirect(string uri)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0};source=\"{1}\";reason=\"Redirecting to:{2};time={3}\"", new object[]
			{
				CallEndingReason.RedirectDiagnostics.ErrorCode,
				Utils.GetLocalHostFqdn(),
				uri,
				DateTime.UtcNow.ToString("o")
			});
		}

		private static string GetDiagnosticsServiceVersion()
		{
			return string.Format("{0}/{1}", Assembly.GetEntryAssembly().GetName().Name, FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);
		}

		internal static string FormatDiagnosticsInfoCallReceived()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0};source=\"{1}\";reason=\"{2}\";service=\"{3}\";time=\"{4}\"", new object[]
			{
				CallEndingReason.CallReceivedDiagnostics.ErrorCode,
				Utils.GetLocalHostFqdn(),
				CallEndingReason.CallReceivedDiagnostics.Reason,
				Util.GetDiagnosticsServiceVersion(),
				DateTime.UtcNow.ToString("o")
			});
		}

		internal static string FormatDiagnosticsInfoServerHealth()
		{
			PerformanceCounter performanceCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
			performanceCounter.NextValue();
			Thread.Sleep(TimeSpan.FromSeconds(1.0));
			float num = performanceCounter.NextValue();
			performanceCounter.Dispose();
			string text = string.Format("{0:0.00}%/{1}", num, Environment.ProcessorCount);
			return string.Format(CultureInfo.InvariantCulture, "{0};source=\"{1}\";reason=\"{2}\";service=\"{3}\";health=\"{4}\";time=\"{5}\"", new object[]
			{
				CallEndingReason.ServerHealthDiagnostics.ErrorCode,
				Utils.GetLocalHostFqdn(),
				CallEndingReason.ServerHealthDiagnostics.Reason,
				Util.GetDiagnosticsServiceVersion(),
				text,
				DateTime.UtcNow.ToString("o")
			});
		}

		internal static string FormatDiagnosticsInfoCallTimeout()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0};source=\"{1}\";reason=\"{2}\";service=\"{3}\";time=\"{4}\"", new object[]
			{
				CallEndingReason.CallTimeoutDiagnostics.ErrorCode,
				Utils.GetLocalHostFqdn(),
				CallEndingReason.CallTimeoutDiagnostics.Reason,
				Util.GetDiagnosticsServiceVersion(),
				DateTime.UtcNow.ToString("o")
			});
		}

		internal static LatencyDetectionContext StartCallLatencyDetection(string latencyDetectionLocation, string callId)
		{
			LatencyDetectionContextFactory latencyDetectionContextFactory;
			lock (Util.latencyFactoriesLock)
			{
				if (!Util.latencyFactories.TryGetValue(latencyDetectionLocation, out latencyDetectionContextFactory))
				{
					latencyDetectionContextFactory = LatencyDetectionContextFactory.CreateFactory(latencyDetectionLocation);
					Util.latencyFactories.Add(latencyDetectionLocation, latencyDetectionContextFactory);
				}
			}
			return latencyDetectionContextFactory.CreateContext(ContextOptions.DoNotCreateReport, CommonConstants.ApplicationVersion, callId, new IPerformanceDataProvider[]
			{
				PerformanceContext.Current,
				MServeLatencyContext.Current
			});
		}

		internal static void EndCallLatencyDetection(LatencyDetectionContext latencyContext, string callId, ExDateTime callStartTime, string userAgent, NonBlockingCallAnsweringData userData, bool addToLog)
		{
			TaskPerformanceData[] array = latencyContext.StopAndFinalizeCollection();
			if (!addToLog)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoipPlatformTracer, null, "Latency logging not requested", new object[0]);
				return;
			}
			CallPerformanceLogger.CallPerformanceLogRow callPerformanceLogRow = new CallPerformanceLogger.CallPerformanceLogRow
			{
				CallId = callId,
				UMServerName = Utils.GetLocalHostName(),
				CallStartTime = (DateTime)callStartTime,
				Duration = (int)(ExDateTime.UtcNow - callStartTime).TotalMilliseconds,
				Component = Assembly.GetEntryAssembly().GetName().Name,
				UserAgent = userAgent
			};
			if (userData != null)
			{
				callPerformanceLogRow.UserDataRpcCount = userData.RpcCount;
				callPerformanceLogRow.UserDataRpcLatency = (int)userData.RpcLatency.TotalMilliseconds;
				callPerformanceLogRow.UserDataAdCount = userData.AdCount;
				callPerformanceLogRow.UserDataAdLatency = (int)userData.AdLatency.TotalMilliseconds;
				callPerformanceLogRow.UserDataDuration = (int)userData.ElapsedTime.TotalMilliseconds;
				callPerformanceLogRow.UserDataTimedOut = userData.TimedOut;
			}
			TaskPerformanceData taskPerformanceData = array[0];
			if (taskPerformanceData.End != PerformanceData.Zero)
			{
				callPerformanceLogRow.AdCount = taskPerformanceData.Difference.Count;
				callPerformanceLogRow.AdLatency = taskPerformanceData.Difference.Milliseconds;
			}
			TaskPerformanceData taskPerformanceData2 = array[1];
			if (taskPerformanceData2.End != PerformanceData.Zero)
			{
				callPerformanceLogRow.MServeCount = taskPerformanceData2.Difference.Count;
				callPerformanceLogRow.MServeLatency = taskPerformanceData2.Difference.Milliseconds;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.VoipPlatformTracer, null, "AD reqs={0} latency={1}  MServe reqs={2} latency={3} total latency={4}", new object[]
			{
				callPerformanceLogRow.AdCount,
				callPerformanceLogRow.AdLatency,
				callPerformanceLogRow.MServeCount,
				callPerformanceLogRow.MServeLatency,
				callPerformanceLogRow.Duration
			});
			CallIdTracer.TraceDebug(ExTraceGlobals.VoipPlatformTracer, null, "UserData: RPC reqs={0} latency={1}  AD reqs={2} latency={3} total latency={4} timed out={5}", new object[]
			{
				callPerformanceLogRow.UserDataRpcCount,
				callPerformanceLogRow.UserDataRpcLatency,
				callPerformanceLogRow.UserDataAdCount,
				callPerformanceLogRow.UserDataAdLatency,
				callPerformanceLogRow.UserDataDuration,
				callPerformanceLogRow.UserDataTimedOut
			});
			CallPerformanceLogger.Instance.Append(callPerformanceLogRow);
		}

		private static bool TryGetDialableNumber(string phoneNumberString, UMSubscriber caller, ContactInfo contact, out PhoneNumber dialableNumber)
		{
			PhoneNumber rawNumber = null;
			dialableNumber = null;
			return PhoneNumber.TryParse(phoneNumberString, out rawNumber) && Util.TryGetDialableNumber(rawNumber, caller, contact, out dialableNumber);
		}

		private static bool TryGetDialableNumber(PhoneNumber rawNumber, UMSubscriber caller, ContactInfo contact, out PhoneNumber dialableNumber)
		{
			bool result = false;
			dialableNumber = null;
			PIIMessage[] data = new PIIMessage[]
			{
				PIIMessage.Create(PIIType._PhoneNumber, rawNumber),
				PIIMessage.Create(PIIType._UserDisplayName, (caller != null) ? caller.DisplayName : "<null>"),
				PIIMessage.Create(PIIType._UserDisplayName, (contact != null) ? contact.DisplayName : "<null>")
			};
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, null, data, "TryGetDialableNumber(Number=(_PhoneNumber) Caller=(_UserDisplayName1) Contact=(_UserDisplayName2))", new object[0]);
			if (rawNumber == null)
			{
				return false;
			}
			PhoneNumber phoneNumber = DialPermissions.Canonicalize(rawNumber, caller.DialPlan, contact.ADOrgPerson, contact.DialPlan);
			if (phoneNumber != null)
			{
				result = DialPermissions.Check(phoneNumber, caller.ADRecipient as ADUser, caller.DialPlan, contact.DialPlan, out dialableNumber);
			}
			return result;
		}

		private static string BuildDurationString(int minutes, int seconds, CultureInfo culture)
		{
			string result;
			if (minutes == 0)
			{
				if (1 == seconds)
				{
					result = Strings.OneSecond.ToString(culture);
				}
				else
				{
					result = Strings.Seconds(seconds).ToString(culture);
				}
			}
			else if (1 == minutes)
			{
				if (1 == seconds)
				{
					result = Strings.OneMinuteOneSecond.ToString(culture);
				}
				else if (seconds > 0)
				{
					result = Strings.OneMinuteSeconds(seconds).ToString(culture);
				}
				else
				{
					result = Strings.OneMinute.ToString(culture);
				}
			}
			else if (1 == seconds)
			{
				result = Strings.MinutesOneSecond(minutes).ToString(culture);
			}
			else if (seconds > 0)
			{
				result = Strings.MinutesSeconds(minutes, seconds).ToString(culture);
			}
			else
			{
				result = Strings.Minutes(minutes).ToString(culture);
			}
			return result;
		}

		private static Dictionary<string, LatencyDetectionContextFactory> latencyFactories = new Dictionary<string, LatencyDetectionContextFactory>();

		private static object latencyFactoriesLock = new object();
	}
}

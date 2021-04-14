using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DateTimeAndDurationRecognitionBehavior : MowaStaticGrammarRecognitionBehaviorBase
	{
		public DateTimeAndDurationRecognitionBehavior(Guid id, CultureInfo culture, Guid userObjectGuid, Guid tenantGuid, ExTimeZone timeZone) : base(id, culture, userObjectGuid, tenantGuid, timeZone)
		{
		}

		public override string MowaGrammarRuleName
		{
			get
			{
				return "DateTimeAndDurationRecognition";
			}
		}

		public override List<string> TagsToProcess
		{
			get
			{
				return DateTimeAndDurationRecognitionBehavior.tagsToProcess;
			}
		}

		protected override MobileSpeechRecoResultType[] SupportedResultTypes
		{
			get
			{
				return DateTimeAndDurationRecognitionBehavior.supportedResultTypes;
			}
		}

		protected override void ProcessSemanticTags(Dictionary<string, string> semanticTags)
		{
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "DateTimeAndDurationRecognitionBehavior.ProcessSemanticTags", new object[0]);
			int num = int.Parse(semanticTags["IsStartHourRelative"], CultureInfo.InvariantCulture);
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "IsStartHourRelative: {0}", new object[]
			{
				num
			});
			if (num == 1)
			{
				MobileSpeechRecoTracer.TraceDebug(this, base.Id, "IsStartHourRelative set, trying to resolve time...", new object[0]);
				ExDateTime exDateTime = new ExDateTime(base.TimeZone, DateTime.UtcNow);
				MobileSpeechRecoTracer.TraceDebug(this, base.Id, "currentTimeClientLocal: {0}", new object[]
				{
					exDateTime
				});
				ExDateTime exDateTime2 = this.GetClientLocalStartTime(semanticTags);
				ExDateTime exDateTime3 = exDateTime2.ToUtc();
				MobileSpeechRecoTracer.TraceDebug(this, base.Id, "startTimeClientLocal: {0}. UTC: {1}", new object[]
				{
					exDateTime2,
					exDateTime3
				});
				if (exDateTime2 < exDateTime)
				{
					exDateTime2 = exDateTime2.AddHours(12.0);
					MobileSpeechRecoTracer.TraceDebug(this, base.Id, "startTime < current. Using(+12h): {0}", new object[]
					{
						exDateTime2
					});
				}
				else
				{
					bool flag = false;
					WorkingHours recipientWorkingHours = this.GetRecipientWorkingHours(out flag);
					MobileSpeechRecoTracer.TraceDebug(this, base.Id, "WorkingHours: {0} Supports12HourFormat: {1}", new object[]
					{
						recipientWorkingHours,
						flag
					});
					if (recipientWorkingHours != null && flag)
					{
						bool flag2 = recipientWorkingHours.InWorkingHours(exDateTime3, exDateTime3);
						MobileSpeechRecoTracer.TraceDebug(this, base.Id, "InWorkingHours({0}): {1}", new object[]
						{
							exDateTime2,
							flag2
						});
						if (!flag2)
						{
							ExDateTime exDateTime4 = exDateTime2.AddHours(12.0);
							MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Not in working hours. Testing with(+12h): {0}", new object[]
							{
								exDateTime4
							});
							exDateTime3 = exDateTime4.ToUtc();
							flag2 = recipientWorkingHours.InWorkingHours(exDateTime3, exDateTime3);
							MobileSpeechRecoTracer.TraceDebug(this, base.Id, "InWorkingHours({0}): {1}", new object[]
							{
								exDateTime4,
								flag2
							});
							if (flag2)
							{
								exDateTime2 = exDateTime4;
								MobileSpeechRecoTracer.TraceDebug(this, base.Id, "New startTimeClientLocal: {0}", new object[]
								{
									exDateTime2
								});
							}
						}
					}
				}
				MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Setting StarHour to {0}.", new object[]
				{
					exDateTime2.Hour
				});
				semanticTags["StartHour"] = exDateTime2.Hour.ToString(CultureInfo.InvariantCulture);
			}
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "DateTimeAndDurationRecognitionBehavior.ProcessSemanticTags done.", new object[0]);
		}

		protected virtual WorkingHours GetRecipientWorkingHours(out bool supports12HourFormat)
		{
			supports12HourFormat = false;
			WorkingHours result;
			try
			{
				using (UMMailboxRecipient ummailboxRecipient = UMRecipient.Factory.FromADRecipient<UMMailboxRecipient>(base.GetADRecipient()))
				{
					using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = ummailboxRecipient.CreateSessionLock())
					{
						MailboxSession session = mailboxSessionLock.Session;
						MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Getting working hours for {0}.", new object[]
						{
							session.MailboxOwnerLegacyDN
						});
						WorkingHours workingHours = WorkingHours.LoadFrom(session, session.GetDefaultFolderId(DefaultFolderType.Calendar));
						MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Working hours {0}.", new object[]
						{
							workingHours
						});
						CultureInfo preferredClientCulture = UmCultures.GetPreferredClientCulture(ummailboxRecipient.PreferredCultures);
						MobileSpeechRecoTracer.TraceDebug(this, base.Id, "GetPreferredClientCulture -> {0}.", new object[]
						{
							preferredClientCulture
						});
						if (preferredClientCulture != null)
						{
							string text;
							string text2;
							CommonUtil.GetStandardTimeFormats(preferredClientCulture, out text, out text2);
							MobileSpeechRecoTracer.TraceDebug(this, base.Id, "GetStandardTimeFormats:{0} 12HF:{1} 24HF:{2}", new object[]
							{
								preferredClientCulture,
								text,
								text2
							});
							supports12HourFormat = !string.IsNullOrEmpty(text);
						}
						result = workingHours;
					}
				}
			}
			catch (LocalizedException ex)
			{
				MobileSpeechRecoTracer.TraceError(this, base.Id, "Could not get working hours: {0}.", new object[]
				{
					ex
				});
				throw new MobileRecoRequestCannotBeHandledException(ex.LocalizedString, ex);
			}
			return result;
		}

		protected ExDateTime GetClientLocalStartTime(Dictionary<string, string> semanticTags)
		{
			int day = int.Parse(semanticTags["Day"], CultureInfo.InvariantCulture);
			int year = int.Parse(semanticTags["Year"], CultureInfo.InvariantCulture);
			int month = int.Parse(semanticTags["Month"], CultureInfo.InvariantCulture);
			int hour = int.Parse(semanticTags["StartHour"], CultureInfo.InvariantCulture);
			int minute = int.Parse(semanticTags["StartMinute"], CultureInfo.InvariantCulture);
			return new ExDateTime(base.TimeZone, year, month, day, hour, minute, 0);
		}

		private const int RelativeYes = 1;

		private static readonly List<string> tagsToProcess = new List<string>
		{
			"Day",
			"Month",
			"Year",
			"StartHour",
			"StartMinute",
			"DurationInMinutes",
			"IsStartHourRelative",
			"RecoEvent"
		};

		private static readonly MobileSpeechRecoResultType[] supportedResultTypes = new MobileSpeechRecoResultType[]
		{
			MobileSpeechRecoResultType.AppointmentCreation
		};
	}
}

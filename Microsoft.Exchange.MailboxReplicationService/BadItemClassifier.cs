using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class BadItemClassifier
	{
		public void Classify(BadMessageRec item, TestIntegration testIntegration)
		{
			foreach (BadItemCategory badItemCategory in BadItemClassifier.categories)
			{
				if (badItemCategory.IsMatch(item, testIntegration))
				{
					item.Category = badItemCategory.Name;
					break;
				}
			}
		}

		public int GetLimit(string categoryName)
		{
			if (this.categoryLimits.Count == 0)
			{
				foreach (BadItemCategory badItemCategory in BadItemClassifier.categories)
				{
					this.categoryLimits.Add(badItemCategory.Name, badItemCategory.GetLimit());
				}
			}
			int result;
			if (!this.categoryLimits.TryGetValue(categoryName, out result))
			{
				result = 0;
			}
			return result;
		}

		private static readonly List<BadItemCategory> categories = new List<BadItemCategory>(new BadItemCategory[]
		{
			new BadItemClassifier.FaultInjectionCorruption(),
			new BadItemClassifier.SimpleBadItem("FolderPropertyMismatchCorruption", new WellKnownException?(WellKnownException.MrsPropertyMismatch), new BadItemKind?(BadItemKind.FolderPropertyMismatch), "BadItemLimitFolderPropertyMismatchCorruption"),
			new BadItemClassifier.SimpleBadItem("FolderPropertyCorruption", BadItemKind.CorruptFolderProperty, "BadItemLimiFolderPropertyCorruption"),
			new BadItemClassifier.Contact(),
			new BadItemClassifier.DistributionList(),
			new BadItemClassifier.CalendarRecurrenceCorruption(),
			new BadItemClassifier.StartGreaterThanEndCalendarCorruption(),
			new BadItemClassifier.ConflictEntryIdCorruption(),
			new BadItemClassifier.NonCanonicalAclCorruption(),
			new BadItemClassifier.UnifiedMessagingReportRecipientCorruption(),
			new BadItemClassifier.RecipientCorruption(),
			new BadItemClassifier.StringArrayCorruption(),
			new BadItemClassifier.InvalidMultivalueElementCorruption(),
			new BadItemClassifier.NonUnicodeValueCorruption(),
			new BadItemClassifier.InDumpster(),
			new BadItemClassifier.OldNonContact(),
			new BadItemClassifier.Default()
		});

		private Dictionary<string, int> categoryLimits = new Dictionary<string, int>();

		private class Default : BadItemCategory
		{
			public Default() : base("Default", "BadItemLimitDefault")
			{
			}

			public override bool IsMatch(BadMessageRec message, TestIntegration testIntegration)
			{
				return true;
			}
		}

		private class SimpleBadItem : BadItemCategory
		{
			public SimpleBadItem(string name, WellKnownException wke, string configName) : this(name, new WellKnownException?(wke), null, configName)
			{
			}

			public SimpleBadItem(string name, BadItemKind kind, string configName) : this(name, null, new BadItemKind?(kind), configName)
			{
			}

			public SimpleBadItem(string name, WellKnownException? wke, BadItemKind? badItemKind, string configName) : base(name, configName)
			{
				this.wke = wke;
				this.badItemKind = badItemKind;
			}

			public override bool IsMatch(BadMessageRec message, TestIntegration testIntegration)
			{
				if (message == null)
				{
					return false;
				}
				bool result = true;
				if (this.badItemKind != null && message.Kind != this.badItemKind.Value)
				{
					result = false;
				}
				if (this.wke != null && !CommonUtils.ExceptionIs(message.RawFailure, new WellKnownException[]
				{
					this.wke.Value
				}))
				{
					result = false;
				}
				return result;
			}

			private WellKnownException? wke;

			private BadItemKind? badItemKind;
		}

		private class Contact : BadItemCategory
		{
			public Contact() : base("Contact", "BadItemLimitContact")
			{
			}

			public override bool IsMatch(BadMessageRec message, TestIntegration testIntegration)
			{
				return message != null && message.MessageClass != null && ObjectClass.IsContact(message.MessageClass);
			}
		}

		private class DistributionList : BadItemCategory
		{
			public DistributionList() : base("DistributionList", "BadItemLimitDistributionList")
			{
			}

			public override bool IsMatch(BadMessageRec message, TestIntegration testIntegration)
			{
				return message != null && message.MessageClass != null && ObjectClass.IsDistributionList(message.MessageClass);
			}
		}

		private class OldNonContact : BadItemCategory
		{
			public OldNonContact() : base("OldNonContact", "BadItemLimitOldNonContact")
			{
				this.ageLimit = ConfigBase<MRSConfigSchema>.GetConfig<TimeSpan>("OldItemAge");
			}

			public override bool IsMatch(BadMessageRec message, TestIntegration testIntegration)
			{
				return message != null && (message.MessageClass == null || !ObjectClass.IsContact(message.MessageClass)) && message.DateSent != null && message.DateSent.Value < DateTime.UtcNow - this.ageLimit;
			}

			private readonly TimeSpan ageLimit;
		}

		private class InDumpster : BadItemCategory
		{
			public InDumpster() : base("InDumpster", "BadItemLimitInDumpster")
			{
			}

			public override bool IsMatch(BadMessageRec message, TestIntegration testIntegration)
			{
				return message != null && FolderFilterParser.IsDumpster(message.WellKnownFolderType);
			}
		}

		private class CalendarRecurrenceCorruption : BadItemCategory
		{
			public CalendarRecurrenceCorruption() : base("CalendarRecurrenceCorruption", "BadItemLimitCalendarRecurrenceCorruption")
			{
			}

			public override bool IsMatch(BadMessageRec message, TestIntegration testIntegration)
			{
				return message != null && message.Failure != null && message.Failure.DataContext != null && message.Failure.Message != null && message.Failure.FailureSideInt == 2 && !(message.Failure.FailureType != "ObjectValidationException") && message.Failure.DataContext.Contains("ISourceMailbox.ExportMessages") && message.Failure.Message.Contains("Microsoft.Exchange.Data.Storage.RecurrenceBlobConstraint");
			}
		}

		private class StartGreaterThanEndCalendarCorruption : BadItemCategory
		{
			public StartGreaterThanEndCalendarCorruption() : base("StartGreaterThanEndCalendarCorruption", "BadItemLimitStartGreaterThanEndCalendarCorruption")
			{
			}

			public override bool IsMatch(BadMessageRec message, TestIntegration testIntegration)
			{
				return message.Failure != null && message.Failure.DataContext != null && message.Failure.Message != null && message.Failure.FailureSideInt == 2 && !(message.Failure.FailureType != "ObjectValidationException") && message.Failure.DataContext.Contains("ISourceMailbox.ExportMessages") && message.Failure.Message.Contains(this.failureSignature);
			}

			private readonly string failureSignature = CalendarItemInstanceSchema.StartTimeMustBeLessThanOrEqualToEndTimeConstraint.ToString();
		}

		private class FaultInjectionCorruption : BadItemCategory
		{
			public FaultInjectionCorruption() : base("FaultInjection", "FaultInjection")
			{
			}

			public override bool IsMatch(BadMessageRec message, TestIntegration testIntegration)
			{
				return testIntegration.ClassifyBadItemFaults && (message != null && message.Failure != null && message.Failure.Message != null) && message.Failure.Message.Contains("Lid: 48184   StoreEc: 0x8000400");
			}

			public override int GetLimit()
			{
				return 2;
			}
		}

		private class ConflictEntryIdCorruption : BadItemCategory
		{
			public ConflictEntryIdCorruption() : base("ConflictEntryIdCorruption", "BadItemLimitConflictEntryIdCorruption")
			{
			}

			public override bool IsMatch(BadMessageRec message, TestIntegration testIntegration)
			{
				return message != null && message.RawFailure != null && CommonUtils.ExceptionIs(message.RawFailure, new WellKnownException[]
				{
					WellKnownException.ConflictEntryIdCorruption
				});
			}
		}

		private class RecipientCorruption : BadItemCategory
		{
			public RecipientCorruption() : base("RecipientCorruption", "BadItemLimitRecipientCorruption")
			{
			}

			public override bool IsMatch(BadMessageRec message, TestIntegration testIntegration)
			{
				return message != null && message.RawFailure != null && CommonUtils.ExceptionIs(message.RawFailure, new WellKnownException[]
				{
					WellKnownException.CorruptRecipient
				});
			}
		}

		private class UnifiedMessagingReportRecipientCorruption : BadItemCategory
		{
			public UnifiedMessagingReportRecipientCorruption() : base("UnifiedMessagingReportRecipientCorruption", "BadItemLimitUnifiedMessagingReportRecipientCorruption")
			{
			}

			public override bool IsMatch(BadMessageRec message, TestIntegration testIntegration)
			{
				return message != null && message.RawFailure != null && CommonUtils.ExceptionIs(message.RawFailure, new WellKnownException[]
				{
					WellKnownException.CorruptRecipient
				}) && message.MessageClass == "IPM.Note.Microsoft.CDR.UM";
			}
		}

		private class NonCanonicalAclCorruption : BadItemCategory
		{
			public NonCanonicalAclCorruption() : base("NonCanonicalAcl", "BadItemLimitNonCanonicalAclCorruption")
			{
			}

			public override bool IsMatch(BadMessageRec message, TestIntegration testIntegration)
			{
				return message != null && message.RawFailure != null && CommonUtils.ExceptionIs(message.RawFailure, new WellKnownException[]
				{
					WellKnownException.NonCanonicalACL
				});
			}
		}

		private class StringArrayCorruption : BadItemCategory
		{
			public StringArrayCorruption() : base("StringArrayCorruption", "BadItemLimitStringArrayCorruption")
			{
			}

			public override bool IsMatch(BadMessageRec message, TestIntegration testIntegration)
			{
				return message != null && message.RawFailure != null && message.Failure != null && message.Failure.Message != null && CommonUtils.ExceptionIs(message.RawFailure, new WellKnownException[]
				{
					WellKnownException.CorruptData
				}) && message.Failure.Message.Contains("[String]") && (message.Failure.Message.Contains("[{00062008-0000-0000-c000-000000000046}:0x853a]") || message.Failure.Message.Contains("[{00020329-0000-0000-c000-000000000046}:'Keywords']"));
			}
		}

		private class InvalidMultivalueElementCorruption : BadItemCategory
		{
			public InvalidMultivalueElementCorruption() : base("InvalidMultivalueElement", "BadItemLimitInvalidMultivalueElementCorruption")
			{
			}

			public override bool IsMatch(BadMessageRec message, TestIntegration testIntegration)
			{
				return message != null && message.RawFailure != null && CommonUtils.ExceptionIs(message.RawFailure, new WellKnownException[]
				{
					WellKnownException.InvalidMultivalueElement
				});
			}
		}

		private class NonUnicodeValueCorruption : BadItemCategory
		{
			public NonUnicodeValueCorruption() : base("NonUnicodeValueCorruption", "BadItemLimitNonUnicodeValueCorruption")
			{
			}

			public override bool IsMatch(BadMessageRec message, TestIntegration testIntegration)
			{
				return message != null && message.Failure != null && message.Failure.Message != null && message.Failure.Message.Contains("Lid: 37736   dwParam: 0x") && message.Failure.Message.Contains("Lid: 55288   StoreEc: 0x80040117");
			}
		}
	}
}

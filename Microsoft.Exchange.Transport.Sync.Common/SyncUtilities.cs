using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Security.Dkm;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class SyncUtilities
	{
		internal static string Fqdn
		{
			get
			{
				if (SyncUtilities.fqdn == null)
				{
					string hostName = Dns.GetHostName();
					IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
					SyncUtilities.fqdn = hostEntry.HostName;
				}
				return SyncUtilities.fqdn;
			}
		}

		public static bool IsDatacenterMode()
		{
			if (SyncUtilities.datacenterMode == null)
			{
				lock (SyncUtilities.syncRoot)
				{
					if (SyncUtilities.datacenterMode == null)
					{
						try
						{
							SyncUtilities.datacenterMode = new bool?(VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled || SyncUtilities.IsEnabledInEnterprise());
						}
						catch (CannotDetermineExchangeModeException ex)
						{
							CommonLoggingHelper.SyncLogSession.LogError((TSLID)8UL, "Unable to determine exchange mode. Assuming not datacenter. Error: {0}", new object[]
							{
								ex
							});
							SyncUtilities.datacenterMode = new bool?(false);
						}
					}
				}
			}
			return SyncUtilities.datacenterMode.Value;
		}

		public static string GetNextSessionId()
		{
			return Interlocked.Increment(ref SyncUtilities.nextSessionId).ToString("X16", NumberFormatInfo.InvariantInfo);
		}

		public static string SecureStringToString(SecureString secureString)
		{
			return secureString.AsUnsecureString();
		}

		public static SecureString StringToSecureString(string clearString)
		{
			return clearString.AsSecureString();
		}

		public static void ThrowIfArgumentNull(string name, object arg)
		{
			if (arg == null)
			{
				throw new ArgumentNullException(name);
			}
		}

		public static void ThrowIfArgumentNullOrEmpty(string name, string arg)
		{
			if (string.IsNullOrEmpty(arg))
			{
				throw new ArgumentException("The value is set to null or empty", name);
			}
		}

		public static void ThrowIfArgumentNullOrEmpty(string name, ICollection arg)
		{
			if (arg == null || arg.Count == 0)
			{
				throw new ArgumentException("The collection is set to null or is empty", name);
			}
		}

		public static void ThrowIfArgumentInvalid(string name, bool condition)
		{
			if (!condition)
			{
				throw new ArgumentException("The argument is invalid", name);
			}
		}

		public static void ThrowIfGuidEmpty(string argumentName, Guid arg)
		{
			if (object.Equals(arg, Guid.Empty))
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Guid {0} is Guid.Empty.", new object[]
				{
					argumentName
				}));
			}
		}

		public static void ThrowIfArgumentLessThanZero(string name, long arg)
		{
			if (arg < 0L)
			{
				throw new ArgumentOutOfRangeException(name, arg, "The value is set to less than 0.");
			}
		}

		public static void ThrowIfArgumentLessThanZero(string name, int arg)
		{
			if (arg < 0)
			{
				throw new ArgumentOutOfRangeException(name, arg, "The value is set to less than 0.");
			}
		}

		public static void ThrowIfArgumentLessThanEqualToZero(string name, int arg)
		{
			if (arg <= 0)
			{
				throw new ArgumentOutOfRangeException(name, arg, "The value is set to less than equal to 0.");
			}
		}

		public static void ThrowIfArgumentLessThanZero(string name, TimeSpan arg)
		{
			if (arg.TotalSeconds < 0.0)
			{
				throw new ArgumentOutOfRangeException(name, arg, "The TimeSpan value is negative.");
			}
		}

		public static void ThrowIfArg1LessThenArg2(string name1, long arg1, string name2, long arg2)
		{
			if (arg1 < arg2)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} value({1}) is set to less than {2} value({3}).", new object[]
				{
					name1,
					arg1,
					name2,
					arg2
				}));
			}
		}

		public static void ThrowIfArg1LessThenArg2(string name1, ExDateTime arg1, string name2, ExDateTime arg2)
		{
			if (arg1 < arg2)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} value({1}) is set to less than {2} value({3}).", new object[]
				{
					name1,
					arg1,
					name2,
					arg2
				}));
			}
		}

		public static void ThrowIfArgumentOutOfRange(string argumentName, double arg, double inclusiveMin, double inclusiveMax)
		{
			if (arg < inclusiveMin || arg > inclusiveMax)
			{
				string message = string.Format(CultureInfo.InvariantCulture, "{0}({1}) not in the range [{2}-{3}]", new object[]
				{
					argumentName,
					arg,
					inclusiveMin,
					inclusiveMax
				});
				throw new ArgumentOutOfRangeException(argumentName, arg, message);
			}
		}

		public static TOut SafeGetProperty<TOut>(IStorePropertyBag item, PropertyDefinition propertyDefinition)
		{
			return SyncUtilities.SafeGetProperty<TOut>(item, propertyDefinition, default(TOut));
		}

		public static TOut SafeGetProperty<TOut>(IStorePropertyBag item, PropertyDefinition propertyDefinition, TOut defaultValue)
		{
			object obj = null;
			StorePropertyDefinition storePropertyDefinition = propertyDefinition as StorePropertyDefinition;
			if (storePropertyDefinition != null)
			{
				obj = item.TryGetProperty(storePropertyDefinition);
			}
			if (obj == null || obj is PropertyError)
			{
				return defaultValue;
			}
			return (TOut)((object)obj);
		}

		public static DateTime ToDateTime(string dateTimeString)
		{
			return new DateHeader("<empty>", DateTime.UtcNow)
			{
				Value = dateTimeString
			}.UtcDateTime;
		}

		public static ExDateTime GetReceivedDate(Stream mimeStream, bool useSentTime)
		{
			ExDateTime? exDateTime = null;
			try
			{
				using (MimeReader mimeReader = new MimeReader(Streams.CreateSuppressCloseWrapper(mimeStream)))
				{
					if (mimeReader.ReadNextPart())
					{
						while (mimeReader.HeaderReader.ReadNextHeader())
						{
							if (mimeReader.HeaderReader.HeaderId == HeaderId.Received)
							{
								ReceivedHeader receivedHeader = Header.ReadFrom(mimeReader.HeaderReader) as ReceivedHeader;
								if (receivedHeader != null && receivedHeader.Date != null)
								{
									DateTime dateTime = SyncUtilities.ToDateTime(receivedHeader.Date);
									return new ExDateTime(ExTimeZone.UtcTimeZone, dateTime);
								}
							}
							if (useSentTime && mimeReader.HeaderReader.HeaderId == HeaderId.Date)
							{
								DateHeader dateHeader = Header.ReadFrom(mimeReader.HeaderReader) as DateHeader;
								if (dateHeader != null)
								{
									exDateTime = new ExDateTime?(new ExDateTime(ExTimeZone.UtcTimeZone, dateHeader.DateTime));
								}
							}
						}
					}
				}
			}
			finally
			{
				mimeStream.Seek(0L, SeekOrigin.Begin);
			}
			if (exDateTime != null)
			{
				return exDateTime.Value;
			}
			return ExDateTime.MinValue;
		}

		public static bool ExistsInCollection<T>(T value, IEnumerable<T> collection, IEqualityComparer<T> comparer)
		{
			foreach (T x in collection)
			{
				if (comparer.Equals(x, value))
				{
					return true;
				}
			}
			return false;
		}

		public static bool CompareByteArrays(byte[] first, byte[] second)
		{
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			if (second == null)
			{
				throw new ArgumentNullException("second");
			}
			if (first.Length != second.Length)
			{
				return false;
			}
			for (int i = 0; i < second.Length; i++)
			{
				if (first[i] != second[i])
				{
					return false;
				}
			}
			return true;
		}

		public static bool TryGetConnectedAccountsDetailsUrl(IExchangePrincipal subscriptionExchangePrincipal, AggregationSubscription subscription, SyncLogSession syncLogSession, out string connectedAccountsDetailsUrl)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionExchangePrincipal", subscriptionExchangePrincipal);
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			connectedAccountsDetailsUrl = null;
			AggregationSubscriptionType subscriptionType = subscription.SubscriptionType;
			switch (subscriptionType)
			{
			case AggregationSubscriptionType.Pop:
				return EcpUtilities.TryGetPopSubscriptionDetailsUrl(subscriptionExchangePrincipal, (PopAggregationSubscription)subscription, syncLogSession, out connectedAccountsDetailsUrl);
			case (AggregationSubscriptionType)3:
				break;
			case AggregationSubscriptionType.DeltaSyncMail:
				return EcpUtilities.TryGetHotmailSubscriptionDetailsUrl(subscriptionExchangePrincipal, (DeltaSyncAggregationSubscription)subscription, syncLogSession, out connectedAccountsDetailsUrl);
			default:
				if (subscriptionType == AggregationSubscriptionType.IMAP)
				{
					return EcpUtilities.TryGetImapSubscriptionDetailsUrl(subscriptionExchangePrincipal, (IMAPAggregationSubscription)subscription, syncLogSession, out connectedAccountsDetailsUrl);
				}
				if (subscriptionType == AggregationSubscriptionType.Facebook)
				{
					throw new InvalidOperationException("Facebook subscriptions are not viewable under 'Connected Accounts Details'");
				}
				break;
			}
			throw new InvalidOperationException("Unknown subscription type: " + subscription.SubscriptionType);
		}

		public static bool HasUnicodeCharacters(string toCheck)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("toCheck", toCheck);
			foreach (char c in toCheck)
			{
				if (c > 'ÿ')
				{
					return true;
				}
			}
			return false;
		}

		public static bool HasUnicodeCharacters(SecureString toCheck)
		{
			SyncUtilities.ThrowIfArgumentNull("toCheck", toCheck);
			if (toCheck.Length == 0)
			{
				throw new ArgumentException("The value has no contents", "toCheck");
			}
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				intPtr = Marshal.SecureStringToCoTaskMemUnicode(toCheck);
				for (int i = 0; i < toCheck.Length; i++)
				{
					char c = (char)Marshal.ReadInt16(intPtr, i * Marshal.SizeOf(typeof(short)));
					if (c > 'ÿ')
					{
						return true;
					}
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.ZeroFreeCoTaskMemUnicode(intPtr);
				}
			}
			return false;
		}

		[Conditional("DEBUG")]
		public static void CheckCallStackForTest()
		{
			string stackTrace = Environment.StackTrace;
			if (!stackTrace.Contains("Internal.Exchange.MailboxTransport"))
			{
				string[] separator = new string[]
				{
					Environment.NewLine
				};
				string[] array = stackTrace.Split(separator, StringSplitOptions.None);
				string str = array[2].Replace("at ", string.Empty).Trim();
				throw new InvalidOperationException("Only test code is to call the method: " + str);
			}
		}

		public static void GetHoursAndDaysWithoutSuccessfulSync(AggregationSubscription subscription, bool useLastSyncTimeAsReference, out int days, out int hours)
		{
			TimeSpan timeWithoutSuccessfulSync = SyncUtilities.GetTimeWithoutSuccessfulSync(subscription, useLastSyncTimeAsReference);
			hours = Convert.ToInt32(Math.Floor(timeWithoutSuccessfulSync.TotalHours));
			days = Convert.ToInt32(Math.Floor(timeWithoutSuccessfulSync.TotalDays));
		}

		public static string SelectTimeBasedString(int days, int hours, string daySingularString, string dayPluralString, string hourSingularString, string hourPluralString, string defaultString)
		{
			if (days > 0)
			{
				if (days > 1)
				{
					return dayPluralString;
				}
				return daySingularString;
			}
			else
			{
				if (hours <= 0)
				{
					return defaultString;
				}
				if (hours > 1)
				{
					return hourPluralString;
				}
				return hourSingularString;
			}
		}

		public static void RunUserWorkItemOnNewThreadAndBlockCurrentThread(ThreadStart userWorkItem)
		{
			if (userWorkItem != null)
			{
				Thread thread = new Thread(userWorkItem);
				thread.Start();
				thread.Join();
			}
		}

		public static long ConvertToLong(double doubleValue)
		{
			if (doubleValue > 9.2233720368547758E+18)
			{
				return long.MaxValue;
			}
			if (doubleValue < -9.2233720368547758E+18)
			{
				return long.MinValue;
			}
			return (long)doubleValue;
		}

		public static bool IsDetailedAggregationStatusDueToResourceProtectionThrottling(DetailedAggregationStatus status)
		{
			return status == DetailedAggregationStatus.RemoteServerIsSlow || status == DetailedAggregationStatus.RemoteServerIsBackedOff || status == DetailedAggregationStatus.RemoteServerIsPoisonous || status == DetailedAggregationStatus.SyncStateSizeError;
		}

		public static string ComputeSHA512Hash(string inputString)
		{
			if (string.IsNullOrEmpty(inputString))
			{
				return inputString;
			}
			string result;
			using (SHA512Cng sha512Cng = new SHA512Cng())
			{
				byte[] inArray = sha512Cng.ComputeHash(Encoding.UTF8.GetBytes(inputString));
				string text = Convert.ToBase64String(inArray);
				result = text;
			}
			return result;
		}

		public static string GenerateMessageId(string id)
		{
			return string.Format("<{0}@{1}>", id, SyncUtilities.Fqdn);
		}

		public static bool IsContactSubscriptionType(AggregationSubscriptionType subscriptionType)
		{
			return subscriptionType == AggregationSubscriptionType.Facebook || subscriptionType == AggregationSubscriptionType.LinkedIn;
		}

		public static MailboxSession OpenMailboxSessionAndHaveCompleteExchangePrincipal(Guid mailboxGuid, Guid databaseGuid, SyncUtilities.MailboxSessionOpener mailboxSessionOpenner)
		{
			SyncUtilities.ThrowIfGuidEmpty("mailboxGuid", mailboxGuid);
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			SyncUtilities.ThrowIfArgumentNull("mailboxSessionOpenner", mailboxSessionOpenner);
			ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromMailboxData(mailboxGuid, databaseGuid, Array<CultureInfo>.Empty);
			MailboxSession mailboxSession = mailboxSessionOpenner(exchangePrincipal);
			bool flag = false;
			try
			{
				mailboxSession.ReconstructExchangePrincipal();
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					mailboxSession.Dispose();
					mailboxSession = null;
				}
			}
			return mailboxSession;
		}

		public static IExchangeGroupKey CreateExchangeGroupKey()
		{
			return PeopleConnectExchangeGroupKeyFactory.Create();
		}

		public static bool IsEnabledInEnterprise()
		{
			return PeopleConnectRegistryReader.Read().DogfoodInEnterprise;
		}

		public static bool VerifyNestedInnerExceptionType(Exception exception, Type exceptionType)
		{
			for (Exception innerException = exception.InnerException; innerException != null; innerException = innerException.InnerException)
			{
				if (exceptionType.IsAssignableFrom(innerException.GetType()))
				{
					return true;
				}
			}
			return false;
		}

		private static TimeSpan GetTimeWithoutSuccessfulSync(AggregationSubscription subscription, bool useLastSyncTimeAsReference)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			DateTime d = DateTime.UtcNow;
			if (useLastSyncTimeAsReference)
			{
				d = ((subscription.LastSyncTime != null) ? subscription.LastSyncTime.Value : subscription.CreationTime);
			}
			return d - subscription.AdjustedLastSuccessfulSyncTime;
		}

		public static readonly int MaximumFqdnLength = 126;

		public static readonly DateTime ZeroTime = new DateTime(504911232000000000L, DateTimeKind.Utc);

		public static readonly ExDateTime ExZeroTime = (ExDateTime)SyncUtilities.ZeroTime;

		public static readonly long DataNotAvailable = -1L;

		public static readonly string WorkerClientInfoString = "Client=TransportSync;Action=SyncWorker";

		private static long nextSessionId = DateTime.UtcNow.Ticks;

		private static bool? datacenterMode;

		private static object syncRoot = new object();

		private static string fqdn;

		public delegate MailboxSession MailboxSessionOpener(IExchangePrincipal exchangePrincipal);
	}
}

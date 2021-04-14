using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class UserOofSettings
	{
		public static UserOofSettings CreateDefault()
		{
			return new UserOofSettings
			{
				internalReply = ReplyBody.CreateDefault(),
				externalReply = ReplyBody.CreateDefault(),
				oofState = OofState.Disabled,
				setByLegacyClient = false,
				externalAudience = ExternalAudience.None
			};
		}

		public OofState OofState
		{
			get
			{
				return this.oofState;
			}
			set
			{
				this.oofState = value;
			}
		}

		public ExternalAudience ExternalAudience
		{
			get
			{
				return this.externalAudience;
			}
			set
			{
				this.externalAudience = value;
			}
		}

		public Duration Duration
		{
			get
			{
				return this.duration;
			}
			set
			{
				this.duration = value;
			}
		}

		public ReplyBody InternalReply
		{
			get
			{
				return this.internalReply;
			}
			set
			{
				this.internalReply = value;
			}
		}

		public ReplyBody ExternalReply
		{
			get
			{
				return this.externalReply;
			}
			set
			{
				this.externalReply = value;
			}
		}

		[XmlIgnore]
		public bool SetByLegacyClient
		{
			get
			{
				return this.setByLegacyClient;
			}
			set
			{
				this.setByLegacyClient = value;
				this.ExternalReply.SetByLegacyClient = value;
				this.InternalReply.SetByLegacyClient = value;
			}
		}

		internal static ExternalAudience GetUserPolicy(IExchangePrincipal mailboxOwner)
		{
			if (mailboxOwner == null)
			{
				throw new ArgumentNullException("mailboxOwner");
			}
			ExternalAudience result = ExternalAudience.All;
			try
			{
				result = UserOofSettingsStorage.GetUserPolicy(mailboxOwner);
			}
			catch (ADPossibleOperationException arg)
			{
				UserOofSettings.Tracer.TraceError<string, ADPossibleOperationException>(0L, "Mailbox:{0}: Exception while getting user policy, exception = {1}.", mailboxOwner.MailboxInfo.DisplayName, arg);
			}
			catch (MailStorageNotFoundException arg2)
			{
				UserOofSettings.Tracer.TraceError<string, MailStorageNotFoundException>(0L, "Mailbox:{0}: Exception while getting user policy, exception = {1}.", mailboxOwner.MailboxInfo.DisplayName, arg2);
			}
			return result;
		}

		internal static UserOofSettings Create()
		{
			return new UserOofSettings();
		}

		internal static string OofLog(MailboxSession itemStore)
		{
			string itemContent = UserOofSettings.OofLogStore.GetItemContent(itemStore);
			if (itemContent == null)
			{
				return "empty oof log";
			}
			return itemContent;
		}

		internal static UserOofSettings GetUserOofSettings(MailboxSession itemStore)
		{
			return UserOofSettingsStorage.LoadUserOofSettings(itemStore);
		}

		internal DateTime? UserChangeTime
		{
			get
			{
				return this.userChangeTime;
			}
			set
			{
				this.userChangeTime = value;
			}
		}

		internal void Save(MailboxSession itemStore)
		{
			UserOofSettings.Validate(itemStore, this);
			this.setByLegacyClient = false;
			this.userChangeTime = new DateTime?(DateTime.MaxValue);
			UserOofSettingsStorage.SaveUserOofSettings(itemStore, this);
		}

		internal bool Scheduled
		{
			get
			{
				return this.oofState == OofState.Scheduled;
			}
		}

		internal DateTime EndTime
		{
			get
			{
				return this.duration.EndTime;
			}
		}

		internal bool GlobalOofEnabled(MailboxSession itemStore)
		{
			return OofStateHandler.Get(itemStore);
		}

		internal DateTime StartTime
		{
			get
			{
				return this.duration.StartTime;
			}
		}

		private static void ValidateDuration(Duration duration)
		{
			if (duration == null || duration.EndTime <= duration.StartTime || duration.EndTime <= DateTime.UtcNow || duration.StartTime.Kind != DateTimeKind.Utc || duration.EndTime.Kind != DateTimeKind.Utc)
			{
				throw new InvalidScheduledOofDuration();
			}
		}

		private static void Validate(MailboxSession itemStore, UserOofSettings userOofSettings)
		{
			if (userOofSettings.internalReply == null || userOofSettings.externalReply == null)
			{
				throw new InvalidUserOofSettings();
			}
			if (userOofSettings.OofState == OofState.Scheduled)
			{
				UserOofSettings.ValidateDuration(userOofSettings.duration);
			}
			if ((userOofSettings.ExternalAudience == ExternalAudience.Known || userOofSettings.ExternalAudience == ExternalAudience.All) && UserOofSettings.GetUserPolicy(itemStore.MailboxOwner) == ExternalAudience.None)
			{
				userOofSettings.ExternalAudience = ExternalAudience.None;
			}
		}

		private UserOofSettings()
		{
		}

		private ReplyBody internalReply;

		private ReplyBody externalReply;

		private Duration duration;

		private OofState oofState;

		private bool setByLegacyClient;

		private ExternalAudience externalAudience;

		private DateTime? userChangeTime = null;

		private static readonly SingleInstanceItemHandler OofLogStore = new SingleInstanceItemHandler("IPM.Microsoft.OOF.Log", DefaultFolderType.Configuration);

		private static readonly Trace Tracer = ExTraceGlobals.OOFTracer;
	}
}

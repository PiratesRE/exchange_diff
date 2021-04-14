using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class InstantMessagePayload : NotificationPayloadBase
	{
		public InstantMessagePayload()
		{
		}

		public InstantMessagePayload(InstantMessagePayloadType payloadType)
		{
			this.PayloadType = payloadType;
		}

		[IgnoreDataMember]
		public InstantMessagePayloadType PayloadType { get; set; }

		[DataMember(Name = "PayloadType")]
		public string PayloadTypeString
		{
			get
			{
				return this.PayloadType.ToString();
			}
			set
			{
				this.PayloadType = InstantMessageUtilities.ParseEnumValue<InstantMessagePayloadType>(value, InstantMessagePayloadType.None);
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public int? ChatSessionId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string SipUri { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string DisplayName { get; set; }

		[IgnoreDataMember]
		public InstantMessagePresenceType NewUserPresence { get; set; }

		[DataMember(Name = "NewUserPresence", EmitDefaultValue = false)]
		public string NewUserPresenceString
		{
			get
			{
				if (this.NewUserPresence == InstantMessagePresenceType.None)
				{
					return null;
				}
				return this.NewUserPresence.ToString();
			}
			set
			{
				this.NewUserPresence = InstantMessageUtilities.ParseEnumValue<InstantMessagePresenceType>(value, InstantMessagePresenceType.None);
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PresenceChange[] UserPresenceChanges { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string[] DeletedGroupIds { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string[] DeletedBuddySipUrls { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public InstantMessageContact[] PendingContacts { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public InstantMessageGroup[] AddedGroups { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public InstantMessageBuddy[] AddedContacts { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public InstantMessageGroup[] RenamedGroups { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string ToastMessage { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool IsConference { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string MessageContent { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string MessageFormat { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string MessageSubject { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool IsNewSession { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool SignOnStarted { get; set; }

		[IgnoreDataMember]
		public UserActivityType UserActivity { get; set; }

		[DataMember(Name = "UserActivity", EmitDefaultValue = false)]
		public string UserActivityString
		{
			get
			{
				if (this.UserActivity == UserActivityType.None)
				{
					return null;
				}
				return this.UserActivity.ToString();
			}
			set
			{
				this.UserActivity = InstantMessageUtilities.ParseEnumValue<UserActivityType>(value, UserActivityType.None);
			}
		}

		[IgnoreDataMember]
		public InstantMessageServiceError ServiceError { get; set; }

		[DataMember(Name = "ServiceError", EmitDefaultValue = false)]
		public string ServiceErrorString
		{
			get
			{
				if (this.ServiceError == InstantMessageServiceError.None)
				{
					return null;
				}
				return this.ServiceError.ToString();
			}
			set
			{
				this.ServiceError = InstantMessageUtilities.ParseEnumValue<InstantMessageServiceError>(value, InstantMessageServiceError.None);
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ErrorMessage { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool IsUserInUcsMode { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int? ReconnectInterval { get; set; }

		[IgnoreDataMember]
		public InstantMessageOperationType FailedOperationType { get; set; }

		[DataMember(Name = "FailedOperationType", EmitDefaultValue = false)]
		public string FailedOperationTypeString
		{
			get
			{
				if (this.FailedOperationType == InstantMessageOperationType.Unspecified)
				{
					return null;
				}
				return this.FailedOperationType.ToString();
			}
			set
			{
				this.FailedOperationType = InstantMessageUtilities.ParseEnumValue<InstantMessageOperationType>(value, InstantMessageOperationType.Unspecified);
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public Persona Persona { get; set; }

		public static InstantMessagePayload ReportError(string errorMessage, InstantMessageOperationType failedOperationType)
		{
			return new InstantMessagePayload(InstantMessagePayloadType.ReportError)
			{
				ErrorMessage = errorMessage,
				FailedOperationType = failedOperationType
			};
		}
	}
}

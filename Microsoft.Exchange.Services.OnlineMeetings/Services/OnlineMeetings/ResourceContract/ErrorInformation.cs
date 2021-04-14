using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "reason")]
	internal class ErrorInformation
	{
		static ErrorInformation()
		{
			ErrorInformation.PopulateCommonCodes();
			ErrorInformation.PopulateApplicationCodes();
			ErrorInformation.PopulateMeCodes();
			ErrorInformation.PopulatePeopleCodes();
			ErrorInformation.PopulateOnlineMeetingCodes();
			ErrorInformation.PopulateCommunicationsCodes();
		}

		public ErrorInformation()
		{
		}

		public ErrorInformation(ErrorSubcode errorSubcode)
		{
			this.Subcode = errorSubcode;
		}

		public ErrorInformation(ErrorSubcode errorSubcode, IDictionary<string, string> errorProperties)
		{
			this.Subcode = errorSubcode;
			if (errorProperties != null && errorProperties.Keys.Count > 0)
			{
				this.errorProperties = new Collection<ErrorInformation.Property>();
				foreach (string text in errorProperties.Keys)
				{
					this.errorProperties.Add(new ErrorInformation.Property(text, errorProperties[text]));
				}
			}
		}

		[DataMember(Name = "Links", EmitDefaultValue = false)]
		public Collection<Link> Links { get; set; }

		[IgnoreDataMember]
		public ErrorCode Code { get; set; }

		[IgnoreDataMember]
		public ErrorSubcode Subcode { get; set; }

		[DataMember(Name = "Message", EmitDefaultValue = false)]
		public string Message
		{
			get
			{
				string result = string.Empty;
				if (ErrorInformation.MessageMap.ContainsKey(this.Subcode))
				{
					result = ErrorInformation.MessageMap[this.Subcode];
				}
				return result;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(Name = "Property", EmitDefaultValue = false)]
		public Collection<ErrorInformation.Property> ErrorProperties
		{
			get
			{
				return this.errorProperties;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(Name = "Code", EmitDefaultValue = false)]
		private string ErrorCodeJson
		{
			get
			{
				return this.Code.ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(Name = "Subcode", EmitDefaultValue = false)]
		private string ErrorSubcodeJson
		{
			get
			{
				return this.Subcode.ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		private static void PopulateCommonCodes()
		{
			ErrorInformation.MessageMap.Add(ErrorSubcode.ResourceNotFound, "The requested resource could not be found.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.Forbidden, "The requested operation is not allowed.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.BadRequest, "Bad Request.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.ServiceFailure, "There was a failure in completing the operation.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.MethodNotAllowed, "The method to support the given request does not exist.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.InvalidOperation, "The requested operation is not valid in the current context.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.TooManyRequests, "There are too many outstanding requests. Retry again later.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.Conflict, "There was a conflict that prevents the operation to be completed.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.RequestTooLarge, "The request size was too large to perform the operation.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.InvalidResourceKey, "The resource key supplied is invalid.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.ResourceExists, "The resource already exists.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.ResourceTerminating, "The resources is being terminated.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.InvalidResourceState, "The resource state is invalid.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.InvalidRequestBody, "The request body is invalid.");
		}

		private static void PopulateApplicationCodes()
		{
			ErrorInformation.MessageMap.Add(ErrorSubcode.ApplicationNotFound, "The application was not found.");
		}

		private static void PopulateOnlineMeetingCodes()
		{
			ErrorInformation.MessageMap.Add(ErrorSubcode.OnlineMeetingNotFound, "The online meeting was not found.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.OnlineMeetingExists, "The online meeting already exists.");
		}

		private static void PopulateMeCodes()
		{
		}

		private static void PopulatePeopleCodes()
		{
		}

		private static void PopulateCommunicationsCodes()
		{
			ErrorInformation.MessageMap.Add(ErrorSubcode.ConversationNotFound, "The conversation was not found.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.InvitationNotFound, "The invitation was not found.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.CallNotFound, "The call was not found.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.SessionNotFound, "The session was not found.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.ConversationOperationFailed, "The conversation operation failed.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.InvalidInvitationType, "The invitation type given is invalid.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.SessionContextNotChangable, "The session context cannot be changed once created.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.PendingSessionRenegotiation, "Theere is already a pending negotiation in progress.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.CallNotAnswered, "The call was not answered.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.CallCancelled, "The call was cancelled.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.CallDeclined, "The call was declined.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.CallFailed, "The call failed to connect.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.CallTransfered, "The call was transferred.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.CallReplaced, "The call was replaced.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.InvalidSDP, "The SDP supplied was invalid.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.MediaTypeNotSupported, "The media type is not supported.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.OfferAnswerFailure, "There was a failure in offer/answer negotiation.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.AudioUnavailable, "The audio is not available for this conference.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.UserNotEnabledForOutsideVoice, "The user is not enabled for outside voice.");
			ErrorInformation.MessageMap.Add(ErrorSubcode.CallTransferFailed, "The call transfer failed.");
		}

		public const string Token = "Error";

		private static readonly Dictionary<ErrorSubcode, string> MessageMap = new Dictionary<ErrorSubcode, string>();

		private readonly Collection<ErrorInformation.Property> errorProperties;

		internal class Property
		{
			public Property()
			{
			}

			public Property(string name, string val)
			{
				this.Name = name;
				this.Val = val;
			}

			[DataMember(Name = "Name", EmitDefaultValue = false)]
			public string Name { get; set; }

			[DataMember(Name = "Value", EmitDefaultValue = false)]
			public string Val { get; set; }
		}
	}
}

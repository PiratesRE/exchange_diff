using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.UM.ClientAccess;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMPhoneSession
{
	internal class UMPlayOnPhoneDataProvider : IConfigDataProvider
	{
		public UMPlayOnPhoneDataProvider(ADUser adUser, TypeOfPlayOnPhoneGreetingCall callType)
		{
			if (adUser != null)
			{
				this.principal = ExchangePrincipal.FromADUser(adUser, null);
			}
			this.typeOfCall = callType;
		}

		public UMPlayOnPhoneDataProvider(ADUser adUser, Guid? paaIdentity) : this(adUser, TypeOfPlayOnPhoneGreetingCall.PlayOnPhoneGreetingRecording)
		{
			ValidateArgument.NotNull(adUser, "adUser");
			ValidateArgument.NotNull(paaIdentity, "paaIdentity");
			this.paaIdentity = paaIdentity;
		}

		public string Source
		{
			get
			{
				return string.Empty;
			}
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			UMPhoneSession umphoneSession = new UMPhoneSession();
			umphoneSession[SimpleProviderObjectSchema.Identity] = identity;
			UMClientCommonBase umClientCommon = this.GetUmClientCommon();
			try
			{
				UMCallInfoEx callInfo = umClientCommon.GetCallInfo(identity.ToString());
				umphoneSession.OperationResult = callInfo.EndResult;
				umphoneSession.CallState = callInfo.CallState;
				umphoneSession.EventCause = callInfo.EventCause;
			}
			catch (LocalizedException ex)
			{
				throw new DataSourceOperationException(ex.LocalizedString);
			}
			finally
			{
				umClientCommon.Dispose();
			}
			return umphoneSession;
		}

		public IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			throw new InvalidOperationException();
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			throw new InvalidOperationException();
		}

		public void Save(IConfigurable instance)
		{
			UMPhoneSession umphoneSession = instance as UMPhoneSession;
			UMClientCommonBase umClientCommon = this.GetUmClientCommon();
			try
			{
				string sessionId = this.PlayOnPhoneGreeting(umClientCommon, umphoneSession.PhoneNumber);
				umphoneSession[SimpleProviderObjectSchema.Identity] = new UMPhoneSessionId(sessionId);
				umphoneSession.OperationResult = UMOperationResult.InProgress;
				umphoneSession.CallState = UMCallState.Connecting;
				umphoneSession.EventCause = UMEventCause.None;
			}
			catch (LocalizedException ex)
			{
				throw new DataSourceOperationException(ex.LocalizedString);
			}
			finally
			{
				umClientCommon.Dispose();
			}
		}

		public void Delete(IConfigurable instance)
		{
			UMPhoneSession umphoneSession = instance as UMPhoneSession;
			UMClientCommonBase umClientCommon = this.GetUmClientCommon();
			try
			{
				umClientCommon.Disconnect(umphoneSession.Identity.ToString());
			}
			catch (LocalizedException ex)
			{
				throw new DataSourceOperationException(ex.LocalizedString);
			}
			finally
			{
				umClientCommon.Dispose();
			}
		}

		private UMClientCommonBase GetUmClientCommon()
		{
			TypeOfPlayOnPhoneGreetingCall typeOfPlayOnPhoneGreetingCall = this.typeOfCall;
			if (typeOfPlayOnPhoneGreetingCall == TypeOfPlayOnPhoneGreetingCall.Unknown)
			{
				return new UMClientCommon();
			}
			return new UMClientCommon(this.principal);
		}

		private string PlayOnPhoneGreeting(UMClientCommonBase client, string dialString)
		{
			switch (this.typeOfCall)
			{
			case TypeOfPlayOnPhoneGreetingCall.VoicemailGreetingRecording:
				return ((UMClientCommon)client).PlayOnPhoneGreeting(UMGreetingType.NormalCustom, dialString);
			case TypeOfPlayOnPhoneGreetingCall.AwayGreetingRecording:
				return ((UMClientCommon)client).PlayOnPhoneGreeting(UMGreetingType.OofCustom, dialString);
			case TypeOfPlayOnPhoneGreetingCall.PlayOnPhoneGreetingRecording:
				return ((UMClientCommon)client).PlayOnPhonePAAGreeting(this.paaIdentity.Value, dialString);
			default:
				throw new InvalidOperationException();
			}
		}

		private Guid? paaIdentity;

		private ExchangePrincipal principal;

		private TypeOfPlayOnPhoneGreetingCall typeOfCall;
	}
}

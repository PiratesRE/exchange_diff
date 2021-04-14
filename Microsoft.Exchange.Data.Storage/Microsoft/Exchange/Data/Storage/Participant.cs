using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class Participant : IParticipant, IEquatable<IParticipant>, IReadOnlyPropertyBag, IEquatable<Participant>
	{
		public Participant(string displayName, string emailAddress, string routingType) : this(displayName, emailAddress, routingType, null, Array<PropValue>.Empty)
		{
		}

		public Participant(string displayName, string emailAddress, string routingType, string originalDisplayName) : this(displayName, emailAddress, routingType, originalDisplayName, null, Array<PropValue>.Empty)
		{
		}

		public Participant(string displayName, string emailAddress, string routingType, ParticipantOrigin origin, params KeyValuePair<PropertyDefinition, object>[] additionalProperties) : this(displayName, emailAddress, routingType, origin, PropValue.ConvertEnumerator<PropertyDefinition>(additionalProperties))
		{
		}

		public Participant(string displayName, string emailAddress, string routingType, string originalDisplayName, ParticipantOrigin origin, params KeyValuePair<PropertyDefinition, object>[] additionalProperties) : this(displayName, emailAddress, routingType, originalDisplayName, origin, PropValue.ConvertEnumerator<PropertyDefinition>(additionalProperties))
		{
		}

		public Participant(ADRawEntry adEntry) : this((string)adEntry[ADRecipientSchema.DisplayName], (string)adEntry[ADRecipientSchema.LegacyExchangeDN], "EX", new DirectoryParticipantOrigin(adEntry), new KeyValuePair<PropertyDefinition, object>[0])
		{
		}

		public Participant(IExchangePrincipal principal) : this(Participant.GetDisplayName(principal), principal.LegacyDn, "EX", new DirectoryParticipantOrigin(principal), new KeyValuePair<PropertyDefinition, object>[0])
		{
		}

		internal Participant(string displayName, string emailAddress, string routingType, string originalDisplayName, ParticipantOrigin origin, IEnumerable<PropValue> additionalProperties) : this(origin, Util.CompositeEnumerator<PropValue>(new IEnumerable<PropValue>[]
		{
			Participant.ListCoreProperties(displayName, emailAddress, routingType, originalDisplayName),
			additionalProperties
		}))
		{
			foreach (PropValue propValue in additionalProperties)
			{
				if (propValue.Property.Equals(ParticipantSchema.DisplayName) || propValue.Property.Equals(ParticipantSchema.EmailAddress) || propValue.Property.Equals(ParticipantSchema.RoutingType) || propValue.Property.Equals(ParticipantSchema.OriginalDisplayName))
				{
					throw new ArgumentException();
				}
			}
		}

		internal Participant(string displayName, string emailAddress, string routingType, ParticipantOrigin origin, IEnumerable<PropValue> additionalProperties) : this(origin, Util.CompositeEnumerator<PropValue>(new IEnumerable<PropValue>[]
		{
			Participant.ListCoreProperties(displayName, emailAddress, routingType),
			additionalProperties
		}))
		{
			foreach (PropValue propValue in additionalProperties)
			{
				if (propValue.Property.Equals(ParticipantSchema.DisplayName) || propValue.Property.Equals(ParticipantSchema.EmailAddress) || propValue.Property.Equals(ParticipantSchema.RoutingType))
				{
					throw new ArgumentException();
				}
			}
		}

		private Participant(ParticipantOrigin origin, Participant.ParticipantPropertyBag propertyBag)
		{
			this.hashCode = new LazilyInitialized<int>(new Func<int>(this.ComputeHashCode));
			this.validationStatus = new LazilyInitialized<ParticipantValidationStatus>(new Func<ParticipantValidationStatus>(this.InternalValidate));
			this.propertyBag = propertyBag;
			this.NormalizeCoreProperties();
			this.origin = (origin ?? Participant.InferOrigin(this.propertyBag));
			this.propertyBag.SetDefaultProperties(this.origin.GetProperties() ?? ((IEnumerable<PropValue>)Array<PropValue>.Empty));
			this.routingTypeDriver = this.SelectRoutingTypeDriver();
			this.routingTypeDriver.Normalize(this.propertyBag);
			this.propertyBag.Freeze();
		}

		private Participant(ParticipantOrigin origin, IEnumerable<PropValue> allProperties) : this(origin, new Participant.ParticipantPropertyBag(allProperties))
		{
		}

		private static string GetDisplayName(IExchangePrincipal principal)
		{
			RemoteUserMailboxPrincipal remoteUserMailboxPrincipal = principal as RemoteUserMailboxPrincipal;
			if (remoteUserMailboxPrincipal != null)
			{
				return remoteUserMailboxPrincipal.DisplayName;
			}
			return principal.MailboxInfo.DisplayName;
		}

		public static IEqualityComparer<Participant> AddressEqualityComparer
		{
			get
			{
				return ParticipantComparer.EmailAddress;
			}
		}

		public IEqualityComparer<IParticipant> EmailAddressEqualityComparer
		{
			get
			{
				return this.routingTypeDriver.AddressEqualityComparer;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<string>(ParticipantSchema.DisplayName);
			}
			private set
			{
				this.propertyBag.SetOrDeleteProperty(ParticipantSchema.DisplayName, value);
			}
		}

		public string OriginalDisplayName
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<string>(ParticipantSchema.OriginalDisplayName);
			}
			private set
			{
				this.propertyBag.SetOrDeleteProperty(ParticipantSchema.OriginalDisplayName, value);
			}
		}

		public string EmailAddress
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<string>(ParticipantSchema.EmailAddress);
			}
			private set
			{
				this.propertyBag.SetOrDeleteProperty(ParticipantSchema.EmailAddress, value);
			}
		}

		public string SmtpEmailAddress
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<string>(ParticipantSchema.SmtpAddress);
			}
			private set
			{
				this.propertyBag.SetOrDeleteProperty(ParticipantSchema.SmtpAddress, value);
			}
		}

		public string SipUri
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<string>(ParticipantSchema.SipUri);
			}
			private set
			{
				this.propertyBag.SetOrDeleteProperty(ParticipantSchema.SipUri, value);
			}
		}

		public ParticipantOrigin Origin
		{
			get
			{
				return this.origin;
			}
		}

		public string RoutingType
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<string>(ParticipantSchema.RoutingType);
			}
			private set
			{
				this.propertyBag.SetOrDeleteProperty(ParticipantSchema.RoutingType, value);
			}
		}

		public ParticipantValidationStatus ValidationStatus
		{
			get
			{
				return this.validationStatus;
			}
		}

		public ICollection<PropertyDefinition> LoadedProperties
		{
			get
			{
				return this.propertyBag.Keys;
			}
		}

		public object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				return this.propertyBag[propertyDefinition];
			}
		}

		public bool Submitted { get; set; }

		public static Participant Parse(string inputString)
		{
			Participant result;
			bool flag = Participant.TryParse(inputString, out result);
			if (flag)
			{
				return result;
			}
			throw new InvalidParticipantException(ServerStrings.CantParseParticipant(inputString), ParticipantValidationStatus.InvalidAddressFormat);
		}

		public static bool TryParse(string inputString, out Participant participant)
		{
			if (inputString == null)
			{
				throw new ArgumentNullException("inputString");
			}
			foreach (Participant.TryParseHandler tryParseHandler in Participant.parsingChain)
			{
				IEnumerable<PropValue> enumerable = tryParseHandler(inputString);
				if (enumerable != null)
				{
					participant = new Participant(null, enumerable);
					return true;
				}
			}
			participant = null;
			return false;
		}

		public static bool operator ==(Participant v1, Participant v2)
		{
			return object.Equals(v1, v2);
		}

		public static bool operator !=(Participant v1, Participant v2)
		{
			return !object.Equals(v1, v2);
		}

		public static bool? IsRoutable(string routingType, StoreSession session)
		{
			routingType = Participant.NormalizeRoutingType(routingType);
			return RoutingTypeDriver.PickRoutingTypeDriver(routingType).IsRoutable(routingType, session);
		}

		public static bool RoutingTypeEquals(string routingType1, string routingType2)
		{
			return (string.IsNullOrEmpty(routingType1) && string.IsNullOrEmpty(routingType2)) || string.Equals(routingType1, routingType2, StringComparison.OrdinalIgnoreCase);
		}

		public static Participant[] TryConvertTo(Participant[] sources, string destinationRoutingType, IRecipientSession adRecipientSession)
		{
			if (adRecipientSession == null)
			{
				throw new ArgumentNullException("adRecipientSession");
			}
			return Participant.InternalTryConvertTo(sources, adRecipientSession.SearchRoot, adRecipientSession.SessionSettings, destinationRoutingType, adRecipientSession.GetHashCode());
		}

		public static Participant[] TryConvertTo(Participant[] sources, string destinationRoutingType, MailboxSession session)
		{
			return Participant.InternalTryConvertTo(sources, null, Participant.BatchBuilder.GetADSessionSettings(session), destinationRoutingType, (session != null) ? session.GetHashCode() : 0);
		}

		public static bool HasSameEmail(Participant participant1, Participant participant2)
		{
			return Participant.HasSameEmail(participant1, participant2, null, true);
		}

		public static bool HasSameEmail(Participant participant1, Participant participant2, bool canLookup)
		{
			return Participant.HasSameEmail(participant1, participant2, null, canLookup);
		}

		public static bool HasSameEmail(Participant participant1, Participant participant2, MailboxSession session, bool canLookup)
		{
			return Participant.InternalHasSameEmail(participant1, participant2, null, canLookup ? Participant.BatchBuilder.GetADSessionSettings(session) : null, canLookup);
		}

		public static bool HasSameEmail(Participant participant1, Participant participant2, IRecipientSession adRecipientSession)
		{
			if (adRecipientSession == null)
			{
				throw new ArgumentNullException("adRecipientSession");
			}
			return Participant.InternalHasSameEmail(participant1, participant2, adRecipientSession.SearchRoot, adRecipientSession.SessionSettings, true);
		}

		public static bool HasSameEmail(Participant participant1, Participant participant2, IExchangePrincipal scopingPrincipal)
		{
			if (scopingPrincipal == null)
			{
				throw new ArgumentNullException("scopingPrincipal");
			}
			return Participant.InternalHasSameEmail(participant1, participant2, null, scopingPrincipal.MailboxInfo.OrganizationId.ToADSessionSettings(), true);
		}

		public static Participant TryConvertTo(Participant source, string destinationRoutingType)
		{
			return Participant.TryConvertTo(source, destinationRoutingType, true);
		}

		public static Participant TryConvertTo(Participant source, string destinationRoutingType, bool canLookup)
		{
			return Participant.TryConvertTo(new Participant[]
			{
				source
			}, destinationRoutingType, null)[0];
		}

		public static bool TryGetParticipantsFromDisplayNameProperty(IStorePropertyBag propertyBag, StorePropertyDefinition displayNamePropertyDefinitions, out IList<string> displayNames)
		{
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(displayNamePropertyDefinitions, null);
			return Participant.TryGetParticipantsFromDisplayNameProperty(valueOrDefault, out displayNames);
		}

		public static bool TryGetParticipantsFromDisplayNameProperty(PropertyBag.BasicPropertyStore propertyBag, AtomicStorePropertyDefinition displayNamePropertyDefinitions, out IList<string> displayNames)
		{
			string displayString = propertyBag.GetValue(displayNamePropertyDefinitions) as string;
			return Participant.TryGetParticipantsFromDisplayNameProperty(displayString, out displayNames);
		}

		public static Participant TryConvertTo(Participant source, string destinationRoutingType, MailboxSession session)
		{
			return Participant.TryConvertTo(new Participant[]
			{
				source
			}, destinationRoutingType, session)[0];
		}

		private static bool TryGetParticipantsFromDisplayNameProperty(string displayString, out IList<string> displayNames)
		{
			displayNames = new List<string>();
			if (displayString == null)
			{
				return true;
			}
			if (displayString.Length >= 255)
			{
				return false;
			}
			foreach (string text in displayString.Split(Participant.DisplayNamesSeparator, StringSplitOptions.RemoveEmptyEntries))
			{
				displayNames.Add(text.Trim());
			}
			return true;
		}

		public bool TryGetADRecipient(IRecipientSession session, out ADRecipient adRecipient)
		{
			Util.ThrowOnNullArgument(session, "session");
			if (string.IsNullOrEmpty(this.RoutingType) || string.IsNullOrEmpty(this.EmailAddress))
			{
				adRecipient = null;
				return false;
			}
			ProxyAddress proxyAddress = ProxyAddress.Parse(this.RoutingType, this.EmailAddress);
			if (proxyAddress is InvalidProxyAddress)
			{
				adRecipient = null;
				return false;
			}
			return ADRecipient.TryGetFromProxyAddress(proxyAddress, session, out adRecipient);
		}

		public void Validate()
		{
			if (this.ValidationStatus != ParticipantValidationStatus.NoError)
			{
				throw new InvalidParticipantException(this.GetValidationMessage(), this.ValidationStatus);
			}
		}

		public bool AreAddressesEqual(Participant v)
		{
			return ParticipantComparer.EmailAddress.Equals(this, v);
		}

		public bool Equals(Participant other)
		{
			return this.Equals(other);
		}

		public bool Equals(IParticipant other)
		{
			if (object.ReferenceEquals(other, null) || this.hashCode != other.GetHashCode())
			{
				return false;
			}
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			foreach (PropertyDefinition propertyDefinition in this.LoadedProperties)
			{
				object valueOrDefault = other.GetValueOrDefault<object>(propertyDefinition);
				if (!Util.ValueEquals(this.GetValueOrDefault<object>(propertyDefinition), valueOrDefault))
				{
					return false;
				}
			}
			foreach (PropertyDefinition propertyDefinition2 in other.LoadedProperties)
			{
				object valueOrDefault2 = other.GetValueOrDefault<object>(propertyDefinition2);
				if (!PropertyError.IsPropertyNotFound(valueOrDefault2) && !Util.ValueEquals(this.GetValueOrDefault<object>(propertyDefinition2), valueOrDefault2))
				{
					return false;
				}
			}
			return this.Submitted == other.Submitted;
		}

		public override bool Equals(object o)
		{
			return this.Equals(o as Participant);
		}

		public override int GetHashCode()
		{
			return this.hashCode;
		}

		public bool? IsRoutable(StoreSession session)
		{
			if (this.ValidationStatus == ParticipantValidationStatus.NoError)
			{
				return this.routingTypeDriver.IsRoutable(this.RoutingType, session);
			}
			return new bool?(false);
		}

		public override string ToString()
		{
			return this.ToString(AddressFormat.Verbose);
		}

		public object TryGetProperty(PropertyDefinition propertyDefinition)
		{
			StorePropertyDefinition propertyDefinition2 = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			return this.TryGetProperty(propertyDefinition2);
		}

		public string ToString(AddressFormat addressFormat)
		{
			if (addressFormat == AddressFormat.Verbose)
			{
				return string.Format("[{0} \"{1}\", {3}:{2}, {4}:{5}]", new object[]
				{
					this.origin,
					this.DisplayName ?? "(none)",
					this.EmailAddress ?? "(none)",
					this.RoutingType ?? "(none)",
					"SMTP",
					this.SmtpEmailAddress ?? "(none)"
				});
			}
			string text = this.routingTypeDriver.FormatAddress(this, addressFormat);
			if (text != null)
			{
				return text;
			}
			throw new NotSupportedException(ServerStrings.EmailFormatNotSupported(addressFormat, this.RoutingType));
		}

		object[] IReadOnlyPropertyBag.GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
		{
			return this.propertyBag.GetProperties<PropertyDefinition>(propertyDefinitionArray);
		}

		public Participant ChangeOrigin(ParticipantOrigin newOrigin)
		{
			Util.ThrowOnNullArgument(newOrigin, "newOrigin");
			return new Participant.Builder(this)
			{
				Origin = newOrigin
			}.ToParticipant();
		}

		public T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition)
		{
			return this.GetValueOrDefault<T>(propertyDefinition, default(T));
		}

		public T GetValueOrDefault<T>(PropertyDefinition propertyDefinition)
		{
			StorePropertyDefinition propertyDefinition2 = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			return this.GetValueOrDefault<T>(propertyDefinition2, default(T));
		}

		public T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition, T defaultValue)
		{
			return this.propertyBag.GetValueOrDefault<T>(propertyDefinition, defaultValue);
		}

		public T? GetValueAsNullable<T>(StorePropertyDefinition propertyDefinition) where T : struct
		{
			return this.propertyBag.GetValueAsNullable<T>(propertyDefinition);
		}

		public bool ExistIn(IEnumerable<Participant> participants)
		{
			foreach (Participant participant in participants)
			{
				if (Participant.HasSameEmail(this, participant, true))
				{
					return true;
				}
			}
			return false;
		}

		internal static List<PropValue> ListCoreProperties(string displayName, string emailAddress, string routingType)
		{
			return Participant.ListCoreProperties(displayName, emailAddress, routingType, null);
		}

		internal static List<PropValue> ListCoreProperties(string displayName, string emailAddress, string routingType, string originalDisplayName)
		{
			List<PropValue> list = new List<PropValue>(4);
			if (displayName != null)
			{
				list.Add(new PropValue(ParticipantSchema.DisplayName, displayName));
			}
			if (emailAddress != null)
			{
				list.Add(new PropValue(ParticipantSchema.EmailAddress, emailAddress));
			}
			if (routingType != null)
			{
				list.Add(new PropValue(ParticipantSchema.RoutingType, routingType));
			}
			if (originalDisplayName != null)
			{
				list.Add(new PropValue(ParticipantSchema.OriginalDisplayName, originalDisplayName));
			}
			return list;
		}

		internal static string NormalizeRoutingType(string routingType)
		{
			if (string.IsNullOrEmpty(routingType))
			{
				return null;
			}
			return routingType.ToUpperInvariant();
		}

		internal static bool HasProxyAddress(ADRecipient user, Participant proxy)
		{
			if (!string.IsNullOrEmpty(proxy.EmailAddress))
			{
				IEqualityComparer<string> equalityComparer = proxy.routingTypeDriver.AddressEqualityComparer as IEqualityComparer<string>;
				if (equalityComparer != null)
				{
					if (proxy.RoutingType == "EX" && !string.IsNullOrEmpty(user.LegacyExchangeDN) && equalityComparer.Equals(user.LegacyExchangeDN, proxy.EmailAddress))
					{
						return true;
					}
					foreach (ProxyAddress proxyAddress in user.EmailAddresses)
					{
						ProxyAddressPrefix prefix = ProxyAddressPrefix.GetPrefix(proxy.RoutingType);
						if ((prefix.Equals(proxyAddress.Prefix) || (proxy.RoutingType == "EX" && proxyAddress.Prefix == ProxyAddressPrefix.X500)) && equalityComparer.Equals(proxyAddress.AddressString, proxy.EmailAddress))
						{
							return true;
						}
					}
					return false;
				}
			}
			return false;
		}

		public object TryGetProperty(StorePropertyDefinition propertyDefinition)
		{
			return this.propertyBag.TryGetProperty(propertyDefinition);
		}

		private static bool InternalHasSameEmail(Participant participant1, Participant participant2, ADObjectId searchRoot, ADSessionSettings sessionSettings, bool canLookup)
		{
			if (object.ReferenceEquals(participant1, participant2))
			{
				return true;
			}
			if (participant1 == null || participant2 == null)
			{
				return false;
			}
			if (participant1.AreAddressesEqual(participant2))
			{
				return true;
			}
			bool result;
			if (Participant.TryMatchParticipantWithADUser(participant1, participant2, out result))
			{
				return result;
			}
			if (Participant.TryMatchParticipantWithADUser(participant2, participant1, out result))
			{
				return result;
			}
			if (participant1.ValidationStatus == ParticipantValidationStatus.NoError && participant2.ValidationStatus == ParticipantValidationStatus.NoError && !StandaloneFuzzing.IsEnabled)
			{
				if (canLookup && participant1.RoutingType == "SMTP")
				{
					participant1 = (Participant.InternalTryConvertTo(new Participant[]
					{
						participant1
					}, searchRoot, sessionSettings, "EX", participant1.GetHashCode())[0] ?? participant1);
				}
				if (canLookup && participant2.RoutingType == "SMTP")
				{
					participant2 = (Participant.InternalTryConvertTo(new Participant[]
					{
						participant2
					}, searchRoot, sessionSettings, "EX", participant2.GetHashCode())[0] ?? participant2);
				}
				return participant1.AreAddressesEqual(participant2);
			}
			return false;
		}

		private static Participant[] InternalTryConvertTo(Participant[] sources, ADObjectId searchRoot, ADSessionSettings sessionSettings, string destinationRoutingType, int scopingObjectHashCode)
		{
			Participant.Job job = new Participant.Job(sources);
			PropertyDefinition propertyDefinition;
			Participant.BatchBuilder.Execute(job, new Participant.BatchBuilder[]
			{
				Participant.BatchBuilder.ConvertRoutingType(destinationRoutingType, out propertyDefinition),
				Participant.BatchBuilder.RequestAllProperties(),
				Participant.BatchBuilder.CopyPropertiesFromInput(),
				Participant.BatchBuilder.RequestAllProperties(),
				Participant.BatchBuilder.GetPropertiesFromAD(searchRoot, sessionSettings, new PropertyDefinition[]
				{
					propertyDefinition
				})
			});
			Participant[] array = new Participant[sources.Length];
			for (int i = 0; i < sources.Length; i++)
			{
				if (job[i].Result != null)
				{
					array[i] = job[i].Result.ToParticipant();
					array[i].Submitted = job[i].Input.Submitted;
				}
				else
				{
					Participant.BatchBuilderError batchBuilderError = job[i].Error as Participant.BatchBuilderError;
					ExTraceGlobals.StorageTracer.TraceDebug((long)scopingObjectHashCode, "Failed to convert a participant: source=\"{0}\", destination=\"{1}\", error={2}, innerError={3}", new object[]
					{
						sources[i],
						destinationRoutingType,
						job[i].Error,
						(batchBuilderError != null) ? batchBuilderError.InnerError : null
					});
				}
			}
			return array;
		}

		private static ParticipantOrigin InferOrigin(PropertyBag propertyBag)
		{
			return new OneOffParticipantOrigin();
		}

		private static ParticipantValidationStatus ValidateStringLength(string value, int maxLength, ParticipantValidationStatus errorStatus)
		{
			if (value == null || value.Length <= maxLength)
			{
				return ParticipantValidationStatus.NoError;
			}
			return errorStatus;
		}

		private static ParticipantValidationStatus ValidateAll(params ParticipantValidationStatus[] validationResults)
		{
			ParticipantValidationStatus participantValidationStatus = ParticipantValidationStatus.NoError;
			foreach (ParticipantValidationStatus participantValidationStatus2 in validationResults)
			{
				if (participantValidationStatus2 > participantValidationStatus)
				{
					participantValidationStatus = participantValidationStatus2;
				}
			}
			return participantValidationStatus;
		}

		private static bool TryMatchParticipantWithADUser(Participant participantWithADUser, Participant normalParticipant, out bool hasSameEmail)
		{
			hasSameEmail = false;
			DirectoryParticipantOrigin directoryParticipantOrigin = participantWithADUser.Origin as DirectoryParticipantOrigin;
			if (directoryParticipantOrigin != null)
			{
				ADRecipient adrecipient = directoryParticipantOrigin.ADEntry as ADRecipient;
				if (adrecipient != null)
				{
					hasSameEmail = Participant.HasProxyAddress(adrecipient, normalParticipant);
					return true;
				}
			}
			return false;
		}

		private int ComputeHashCode()
		{
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in this.LoadedProperties)
			{
				object obj = this.TryGetProperty(propertyDefinition);
				if (!PropertyError.IsPropertyError(obj) && !(obj is Array))
				{
					num ^= obj.GetHashCode();
				}
			}
			num ^= this.Submitted.GetHashCode();
			return num;
		}

		private string DetectRoutingType(out RoutingTypeDriver detectedRtDriver)
		{
			string result;
			if (RoutingTypeDriver.TryDetectRoutingType(this.propertyBag, out detectedRtDriver, out result))
			{
				return result;
			}
			this.validationStatus.Set(ParticipantValidationStatus.RoutingTypeRequired);
			this.Validate();
			throw new Exception("Participant.Validate() didn't throw on error");
		}

		private LocalizedString GetValidationMessage()
		{
			ParticipantValidationStatus participantValidationStatus = this.ValidationStatus;
			if (participantValidationStatus <= ParticipantValidationStatus.RoutingTypeRequired)
			{
				if (participantValidationStatus <= ParticipantValidationStatus.AddressAndRoutingTypeMismatch)
				{
					if (participantValidationStatus == ParticipantValidationStatus.NoError)
					{
						return LocalizedString.Empty;
					}
					if (participantValidationStatus == ParticipantValidationStatus.AddressAndRoutingTypeMismatch)
					{
						return ServerStrings.AddressAndRoutingTypeMismatch(this.RoutingType);
					}
				}
				else
				{
					if (participantValidationStatus == ParticipantValidationStatus.AddressRequiredForRoutingType)
					{
						return ServerStrings.AddressRequiredForRoutingType(this.RoutingType);
					}
					if (participantValidationStatus == ParticipantValidationStatus.DisplayNameRequiredForRoutingType)
					{
						return ServerStrings.DisplayNameRequiredForRoutingType(this.RoutingType ?? "(null)");
					}
					if (participantValidationStatus == ParticipantValidationStatus.RoutingTypeRequired)
					{
						return ServerStrings.RoutingTypeRequired;
					}
				}
			}
			else if (participantValidationStatus <= ParticipantValidationStatus.OperationNotSupportedForRoutingType)
			{
				if (participantValidationStatus == ParticipantValidationStatus.InvalidAddressFormat)
				{
					return ServerStrings.InvalidAddressFormat(this.RoutingType, this.EmailAddress);
				}
				if (participantValidationStatus == ParticipantValidationStatus.AddressAndOriginMismatch)
				{
					return ServerStrings.AddressAndOriginMismatch(this.origin);
				}
				if (participantValidationStatus == ParticipantValidationStatus.OperationNotSupportedForRoutingType)
				{
					return LocalizedString.Empty;
				}
			}
			else
			{
				if (participantValidationStatus == ParticipantValidationStatus.DisplayNameTooBig)
				{
					return ServerStrings.ParticipantPropertyTooBig("DisplayName");
				}
				if (participantValidationStatus == ParticipantValidationStatus.EmailAddressTooBig)
				{
					return ServerStrings.ParticipantPropertyTooBig("EmailAddress");
				}
				if (participantValidationStatus == ParticipantValidationStatus.RoutingTypeTooBig)
				{
					return ServerStrings.ParticipantPropertyTooBig("RoutingType");
				}
			}
			return LocalizedString.Empty;
		}

		private ParticipantValidationStatus InternalValidate()
		{
			return Participant.ValidateAll(new ParticipantValidationStatus[]
			{
				this.InternalValidateCoreProperties(),
				this.routingTypeDriver.Validate(this),
				this.origin.Validate(this)
			});
		}

		private ParticipantValidationStatus InternalValidateCoreProperties()
		{
			return Participant.ValidateAll(new ParticipantValidationStatus[]
			{
				Participant.ValidateStringLength(this.DisplayName, 8000, ParticipantValidationStatus.DisplayNameTooBig),
				Participant.ValidateStringLength(this.EmailAddress, 1860, ParticipantValidationStatus.EmailAddressTooBig),
				Participant.ValidateStringLength(this.RoutingType, 9, ParticipantValidationStatus.RoutingTypeTooBig),
				(this.RoutingType == null || ProxyAddressPrefix.IsPrefixStringValid(this.RoutingType)) ? ParticipantValidationStatus.NoError : ParticipantValidationStatus.InvalidRoutingTypeFormat,
				(this.EmailAddress == null || ProxyAddressBase.IsAddressStringValid(this.EmailAddress)) ? ParticipantValidationStatus.NoError : ParticipantValidationStatus.InvalidAddressFormat
			});
		}

		private void NormalizeCoreProperties()
		{
			string displayName = this.DisplayName;
			if (this.ShouldClearDisplayName(displayName))
			{
				this.propertyBag.Delete(ParticipantSchema.DisplayName);
			}
			foreach (StorePropertyDefinition propertyDefinition in Participant.coreProperties)
			{
				string valueOrDefault = this.GetValueOrDefault<string>(propertyDefinition);
				if (valueOrDefault != null && valueOrDefault.Length == 0)
				{
					this.propertyBag.Delete(propertyDefinition);
				}
			}
		}

		private bool ShouldClearDisplayName(string displayName)
		{
			if (displayName == null)
			{
				return false;
			}
			foreach (char c in displayName)
			{
				char c2 = c;
				switch (c2)
				{
				case 'ᅟ':
				case 'ᅠ':
					break;
				default:
					if (c2 != 'ㅤ' && c2 != 'ﾠ' && char.IsLetterOrDigit(c))
					{
						return false;
					}
					break;
				}
			}
			return true;
		}

		private RoutingTypeDriver SelectRoutingTypeDriver()
		{
			string text = this.RoutingType;
			if (text == null)
			{
				RoutingTypeDriver routingTypeDriver = null;
				text = (this.RoutingType = this.DetectRoutingType(out routingTypeDriver));
				if (routingTypeDriver.IsRoutingTypeSupported(text))
				{
					return routingTypeDriver;
				}
			}
			for (int i = 0; i < text.Length; i++)
			{
				if (char.IsLower(text[i]))
				{
					text = (this.RoutingType = Participant.NormalizeRoutingType(text));
					break;
				}
			}
			return RoutingTypeDriver.PickRoutingTypeDriver(text);
		}

		public static Participant[] TryConvertTo(Participant[] sources, string destinationRoutingType, IExchangePrincipal scopingPrincipal, ADObjectId searchRoot)
		{
			if (scopingPrincipal == null)
			{
				throw new ArgumentNullException("scopingPrincipal");
			}
			ADSessionSettings sessionSettings = scopingPrincipal.MailboxInfo.OrganizationId.ToADSessionSettings();
			return Participant.InternalTryConvertTo(sources, searchRoot, sessionSettings, destinationRoutingType, scopingPrincipal.GetHashCode());
		}

		public const string EX = "EX";

		public const string MapiPDL = "MAPIPDL";

		public const string SMTP = "SMTP";

		public const string FAX = "FAX";

		public const string MOBILE = "MOBILE";

		public const string Unspecified = null;

		internal const int MaxDisplayNameLength = 8000;

		internal const int MaxDisplayNamePropertyLenght = 255;

		internal const int MaxEmailAddressLength = 1860;

		internal const int MaxRoutingTypeLength = 9;

		private const string VerboseFormatString = "[{0} \"{1}\", {3}:{2}, {4}:{5}]";

		private static readonly char[] DisplayNamesSeparator = new char[]
		{
			';'
		};

		private static readonly Participant.TryParseHandler[] parsingChain = new Participant.TryParseHandler[]
		{
			new Participant.TryParseHandler(ExRoutingTypeDriver.TryParseExchangeLegacyDN),
			new Participant.TryParseHandler(GenericCustomRoutingTypeDriver.TryParseOutlookFormat),
			new Participant.TryParseHandler(MobileRoutingTypeDriver.TryParseMobilePhoneNumber),
			new Participant.TryParseHandler(UnspecifiedRoutingTypeDriver.TryParse)
		};

		private static readonly StorePropertyDefinition[] coreProperties = new StorePropertyDefinition[]
		{
			ParticipantSchema.DisplayName,
			ParticipantSchema.EmailAddress,
			ParticipantSchema.RoutingType,
			ParticipantSchema.OriginalDisplayName
		};

		private readonly LazilyInitialized<int> hashCode;

		private readonly ParticipantOrigin origin;

		private readonly Participant.ParticipantPropertyBag propertyBag;

		private readonly RoutingTypeDriver routingTypeDriver;

		private readonly LazilyInitialized<ParticipantValidationStatus> validationStatus;

		private delegate IEnumerable<PropValue> TryParseHandler(string inputString);

		private sealed class ParticipantPropertyBag : MemoryPropertyBag
		{
			public ParticipantPropertyBag(Participant.ParticipantPropertyBag copyFrom) : base(copyFrom)
			{
			}

			public ParticipantPropertyBag(IEnumerable<PropValue> propValues)
			{
				foreach (PropValue propValue in propValues)
				{
					if (base.ContainsKey(propValue.Property))
					{
						throw new ArgumentException();
					}
					base[propValue.Property] = propValue.Value;
				}
				base.SetAllPropertiesLoaded();
			}

			public void Freeze()
			{
				if (!this.isFrozen)
				{
					this.isFrozen = true;
					base.ClearChangeInfo();
				}
			}

			public void SetProperties(IEnumerable<PropValue> properties)
			{
				foreach (PropValue propValue in properties)
				{
					base[propValue.Property] = propValue.Value;
				}
			}

			public void SetDefaultProperties(IEnumerable<PropValue> properties)
			{
				foreach (PropValue propValue in properties)
				{
					if (PropertyError.IsPropertyNotFound(base.TryGetProperty(propValue.Property)))
					{
						base[propValue.Property] = propValue.Value;
					}
				}
			}

			protected override void SetValidatedStoreProperty(StorePropertyDefinition propertyDefinition, object propertyValue)
			{
				this.CheckCanModify(propertyDefinition);
				base.SetValidatedStoreProperty(propertyDefinition, propertyValue);
			}

			protected override void DeleteStoreProperty(StorePropertyDefinition propertyDefinition)
			{
				this.CheckCanModify(propertyDefinition);
				base.DeleteStoreProperty(propertyDefinition);
			}

			private void CheckCanModify(PropertyDefinition propertyDefinition)
			{
				if (this.isFrozen)
				{
					throw PropertyError.ToException(new PropertyError[]
					{
						new PropertyError(propertyDefinition, PropertyErrorCode.NotSupported)
					});
				}
			}

			private bool isFrozen;
		}

		public sealed class Builder
		{
			public Builder()
			{
				this.propertyBag = new Participant.ParticipantPropertyBag(Array<PropValue>.Empty);
			}

			public Builder(Participant copyFrom)
			{
				this.propertyBag = new Participant.ParticipantPropertyBag(copyFrom.propertyBag);
				this.origin = copyFrom.origin;
			}

			public Builder(string displayName, string emailAddress, string routingType) : this()
			{
				this.DisplayName = displayName;
				this.EmailAddress = emailAddress;
				this.RoutingType = routingType;
			}

			public object this[StorePropertyDefinition propDef]
			{
				get
				{
					this.CheckState();
					return this.propertyBag[propDef];
				}
				set
				{
					this.CheckState();
					this.propertyBag[propDef] = value;
				}
			}

			public string DisplayName
			{
				get
				{
					this.CheckState();
					return this.propertyBag.GetValueOrDefault<string>(ParticipantSchema.DisplayName);
				}
				set
				{
					this.CheckState();
					this.propertyBag.SetOrDeleteProperty(ParticipantSchema.DisplayName, value);
				}
			}

			public string EmailAddress
			{
				get
				{
					this.CheckState();
					return this.propertyBag.GetValueOrDefault<string>(ParticipantSchema.EmailAddress);
				}
				set
				{
					this.CheckState();
					this.propertyBag.SetOrDeleteProperty(ParticipantSchema.EmailAddress, value);
				}
			}

			public ParticipantOrigin Origin
			{
				get
				{
					this.CheckState();
					return this.origin;
				}
				set
				{
					this.CheckState();
					this.origin = value;
				}
			}

			public string RoutingType
			{
				get
				{
					this.CheckState();
					return this.propertyBag.GetValueOrDefault<string>(ParticipantSchema.RoutingType);
				}
				set
				{
					this.CheckState();
					this.propertyBag.SetOrDeleteProperty(ParticipantSchema.RoutingType, value);
				}
			}

			public void SetPropertiesFrom(ParticipantEntryId entryId)
			{
				if (entryId == null)
				{
					throw new ArgumentNullException("entryId");
				}
				this.CheckState();
				this.Add(entryId.GetParticipantProperties());
				this.origin = (entryId.GetParticipantOrigin() ?? this.origin);
			}

			public Participant ToParticipant()
			{
				this.CheckState();
				Participant result = new Participant(this.origin, this.propertyBag);
				this.Invalidate();
				return result;
			}

			public object TryGetProperty(PropertyDefinition propDef)
			{
				return this.propertyBag.TryGetProperty(propDef);
			}

			internal void Add(IEnumerable<PropValue> propValues)
			{
				this.CheckState();
				this.propertyBag.SetProperties(propValues);
			}

			internal T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition)
			{
				return this.GetValueOrDefault<T>(propertyDefinition, default(T));
			}

			internal T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition, T defaultValue)
			{
				return PropertyBag.CheckPropertyValue<T>(propertyDefinition, this.TryGetProperty(propertyDefinition), defaultValue);
			}

			internal void SetOrDeleteProperty(StorePropertyDefinition propertyDefinition, object propertyValue)
			{
				this.propertyBag.SetOrDeleteProperty(propertyDefinition, propertyValue);
			}

			private void CheckState()
			{
				if (this.propertyBag == null)
				{
					throw new InvalidOperationException("For performance reasons, Participant.Builder instance cannot be reused once ToParticipant() has been called on it");
				}
			}

			private void Invalidate()
			{
				this.propertyBag = null;
				this.origin = null;
			}

			private ParticipantOrigin origin;

			private Participant.ParticipantPropertyBag propertyBag;
		}

		internal abstract class BatchBuilder
		{
			private event Action<Participant.JobItem> ErrorSet;

			public static Participant.BatchBuilder ImceaEncode(string incapsulationDomain)
			{
				return new Participant.ImceaEncoderBatchBuilder(incapsulationDomain, null);
			}

			public static Participant.BatchBuilder ConvertRoutingType(string destinationRoutingType, out PropertyDefinition keyProperty)
			{
				Participant.RoutingTypeConverterBatchBuider routingTypeConverterBatchBuider = new Participant.RoutingTypeConverterBatchBuider(destinationRoutingType, null);
				keyProperty = routingTypeConverterBatchBuider.SpecializedAddressPropertyDefinition;
				return new Participant.RoutingTypeConverterBatchBuider(destinationRoutingType, null);
			}

			public static Participant.BatchBuilder CopyPropertiesFromInput()
			{
				return new Participant.CopyFromInputBatchBuilder();
			}

			public static Participant.BatchBuilder ReplaceProperty(StorePropertyDefinition propertyDefinitionToReplace, StorePropertyDefinition propertyDefinitionToReplaceWith)
			{
				return new Participant.ReplacePropertyBatchBuilder(propertyDefinitionToReplace, propertyDefinitionToReplaceWith);
			}

			public static Participant.BatchBuilder RequestProperties(ICollection<PropertyDefinition> propDefs)
			{
				return new Participant.RequestPropertiesBatchBuilder(propDefs);
			}

			public static Participant.BatchBuilder RequestAllProperties()
			{
				return new Participant.RequestPropertiesBatchBuilder(ParticipantSchema.Instance.InternalAllProperties);
			}

			public static Participant.BatchBuilder RequestProperties(params PropertyDefinition[] propDefs)
			{
				return new Participant.RequestPropertiesBatchBuilder((ICollection<PropertyDefinition>)propDefs);
			}

			public static void Execute(Participant.Job job, params Participant.BatchBuilder[] batchBuilders)
			{
				LinkedListNode<Participant.BatchBuilder> linkedListNode = Participant.BatchBuilder.CreateChain(batchBuilders);
				linkedListNode.Value.Execute(job, linkedListNode.Next);
			}

			public Participant.BatchBuilder SuppressErrors()
			{
				this.ErrorSet += delegate(Participant.JobItem jobItem)
				{
					jobItem.IgnoreError();
				};
				return this;
			}

			public Participant.BatchBuilder HandleErrors(Predicate<Participant.JobItem> errorHandler)
			{
				this.ErrorSet += delegate(Participant.JobItem jobItem)
				{
					if (errorHandler(jobItem))
					{
						jobItem.IgnoreError();
					}
				};
				return this;
			}

			internal static ADSessionSettings GetADSessionSettings(ICoreObject storeObject)
			{
				if (storeObject != null)
				{
					return Participant.BatchBuilder.GetADSessionSettings(storeObject.Session);
				}
				return Participant.BatchBuilder.GetADSessionSettings(null);
			}

			internal static ADSessionSettings GetADSessionSettings(StoreSession session)
			{
				if (session != null)
				{
					return session.GetADSessionSettings();
				}
				return OrganizationId.ForestWideOrgId.ToADSessionSettings();
			}

			protected internal virtual void Execute(Participant.Job job, LinkedListNode<Participant.BatchBuilder> nextBatchBuilder)
			{
				if (nextBatchBuilder != null)
				{
					nextBatchBuilder.Value.Execute(job, nextBatchBuilder.Next);
				}
			}

			protected virtual void AddToChain(LinkedList<Participant.BatchBuilder> chain)
			{
				chain.AddLast(this);
			}

			protected void SetError(Participant.JobItem jobItem, string descritpion, ProviderError innerError)
			{
				jobItem.InternalSetError(new Participant.BatchBuilderError(this, descritpion, innerError));
				if (this.ErrorSet != null)
				{
					this.ErrorSet(jobItem);
				}
			}

			private static LinkedListNode<Participant.BatchBuilder> CreateChain(Participant.BatchBuilder[] batchBuilders)
			{
				LinkedList<Participant.BatchBuilder> linkedList = new LinkedList<Participant.BatchBuilder>();
				foreach (Participant.BatchBuilder batchBuilder in batchBuilders)
				{
					if (batchBuilder != null)
					{
						batchBuilder.AddToChain(linkedList);
					}
				}
				if (linkedList.First == null)
				{
					throw new ArgumentException();
				}
				return linkedList.First;
			}

			public static Participant.BatchBuilder GetPropertiesFromAD(IADRecipientCache recipientCache, params PropertyDefinition[] keyProperties)
			{
				return new Participant.ADLookupBatchBuilder(new Participant.ADLookupBatchBuilder.LookupADRecipientDelegate(recipientCache.FindAndCacheRecipients), keyProperties);
			}

			public static Participant.BatchBuilder GetPropertiesFromAD(IRecipientSession recipientSession, params PropertyDefinition[] keyProperties)
			{
				return new Participant.ADLookupBatchBuilder((ProxyAddress[] proxyAddresses) => recipientSession.FindByProxyAddresses(proxyAddresses, Util.CollectionToArray<ADPropertyDefinition>(ParticipantSchema.SupportedADProperties)), keyProperties);
			}

			public static Participant.BatchBuilder GetPropertiesFromAD(ADObjectId searchRoot, ADSessionSettings adSettings, params PropertyDefinition[] keyProperties)
			{
				Util.ThrowOnNullArgument(adSettings, "adSettings");
				if (adSettings.ConfigScopes == ConfigScopes.TenantLocal && OrganizationId.ForestWideOrgId.Equals(adSettings.CurrentOrganizationId))
				{
					adSettings = ADSessionSettings.RescopeToSubtree(adSettings);
				}
				return Participant.BatchBuilder.GetPropertiesFromAD(DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, searchRoot, CultureInfo.CurrentCulture.LCID, true, ConsistencyMode.PartiallyConsistent, null, adSettings, 817, "GetPropertiesFromAD", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Items\\Participants\\ParticipantBatchBuilder.cs"), keyProperties);
			}
		}

		private class ImceaEncoderBatchBuilder : Participant.BatchBuilder
		{
			internal ImceaEncoderBatchBuilder(string incapsulationDomain, Predicate<Participant.JobItem> isEncodingNeeded)
			{
				this.incapsulationDomain = incapsulationDomain;
				this.isEncodingNeeded = isEncodingNeeded;
			}

			protected internal override void Execute(Participant.Job job, LinkedListNode<Participant.BatchBuilder> nextBatchBuilder)
			{
				base.Execute(job, nextBatchBuilder);
				foreach (Participant.JobItem jobItem in job.GetActiveJobItems())
				{
					if (this.IsEncodingNeeded(jobItem))
					{
						string valueOrDefault = jobItem.GetValueOrDefault<string>(ParticipantSchema.EmailAddress);
						string valueOrDefault2 = jobItem.GetValueOrDefault<string>(ParticipantSchema.RoutingType);
						if (!string.IsNullOrEmpty(valueOrDefault) && !string.IsNullOrEmpty(valueOrDefault2))
						{
							jobItem.SetOrDeleteProperty(ParticipantSchema.EmailAddress, ImceaAddress.Encode(valueOrDefault2, valueOrDefault, this.incapsulationDomain));
							jobItem.SetOrDeleteProperty(ParticipantSchema.RoutingType, "SMTP");
						}
						else
						{
							base.SetError(jobItem, ServerStrings.ExBatchBuilderNeedsPropertyToConvertRT(ParticipantSchema.EmailAddress, valueOrDefault2, "SMTP", jobItem.Input.ToString()), null);
						}
					}
				}
			}

			protected virtual bool IsEncodingNeeded(Participant.JobItem jobItem)
			{
				return !Participant.RoutingTypeEquals(jobItem.GetValueOrDefault<string>(ParticipantSchema.RoutingType), "SMTP") && (this.isEncodingNeeded == null || this.isEncodingNeeded(jobItem));
			}

			private readonly string incapsulationDomain;

			private readonly Predicate<Participant.JobItem> isEncodingNeeded;
		}

		private class RoutingTypeConverterBatchBuider : Participant.BatchBuilder
		{
			internal RoutingTypeConverterBatchBuider(string destinationRoutingType, Predicate<Participant.JobItem> isConversionNeeded)
			{
				this.destinationRoutingType = Participant.NormalizeRoutingType(destinationRoutingType);
				this.isConversionNeeded = isConversionNeeded;
				this.specializedAddressPropDef = Participant.RoutingTypeConverterBatchBuider.GetSpecializedAddressPropertyForRoutingType(destinationRoutingType);
			}

			internal StorePropertyDefinition SpecializedAddressPropertyDefinition
			{
				get
				{
					return this.specializedAddressPropDef;
				}
			}

			protected internal override void Execute(Participant.Job job, LinkedListNode<Participant.BatchBuilder> nextBatchBuilder)
			{
				foreach (Participant.JobItem jobItem in job.GetActiveJobItems())
				{
					if (this.IsConversionNeeded(jobItem))
					{
						jobItem.RequestProperty(this.specializedAddressPropDef);
					}
				}
				base.Execute(job, nextBatchBuilder);
				foreach (Participant.JobItem jobItem2 in job.GetActiveJobItems())
				{
					if (this.IsConversionNeeded(jobItem2))
					{
						string valueOrDefault = jobItem2.Result.GetValueOrDefault<string>(this.specializedAddressPropDef);
						if (valueOrDefault != null)
						{
							jobItem2.SetOrDeleteProperty(ParticipantSchema.EmailAddress, valueOrDefault);
							jobItem2.SetOrDeleteProperty(ParticipantSchema.RoutingType, this.destinationRoutingType);
						}
						else
						{
							base.SetError(jobItem2, ServerStrings.ExBatchBuilderNeedsPropertyToConvertRT(this.specializedAddressPropDef, jobItem2.Input.RoutingType, this.destinationRoutingType, jobItem2.Input.ToString()), null);
						}
					}
				}
			}

			protected virtual bool IsConversionNeeded(Participant.JobItem jobItem)
			{
				string valueOrDefault = jobItem.GetValueOrDefault<string>(ParticipantSchema.RoutingType);
				return !Participant.RoutingTypeEquals(valueOrDefault, this.destinationRoutingType) && (this.isConversionNeeded == null || this.isConversionNeeded(jobItem));
			}

			private static StorePropertyDefinition GetSpecializedAddressPropertyForRoutingType(string routingType)
			{
				if (routingType != null)
				{
					if (routingType == "SMTP")
					{
						return ParticipantSchema.SmtpAddress;
					}
					if (routingType == "EX")
					{
						return ParticipantSchema.LegacyExchangeDN;
					}
				}
				throw new NotSupportedException(string.Format("Batch conversion to {0} routing type is not supported", routingType));
			}

			private readonly string destinationRoutingType;

			private readonly Predicate<Participant.JobItem> isConversionNeeded;

			private readonly StorePropertyDefinition specializedAddressPropDef;
		}

		private sealed class RequestPropertiesBatchBuilder : Participant.BatchBuilder
		{
			internal RequestPropertiesBatchBuilder(ICollection<PropertyDefinition> propDefs)
			{
				this.propDefs = InternalSchema.ToStorePropertyDefinitions(propDefs);
			}

			protected internal override void Execute(Participant.Job job, LinkedListNode<Participant.BatchBuilder> nextBatchBuilder)
			{
				foreach (Participant.JobItem jobItem in job.GetActiveJobItems())
				{
					jobItem.RequestProperties(this.propDefs);
				}
				base.Execute(job, nextBatchBuilder);
			}

			private readonly StorePropertyDefinition[] propDefs;
		}

		private sealed class CopyFromInputBatchBuilder : Participant.BatchBuilder
		{
			internal CopyFromInputBatchBuilder()
			{
			}

			protected internal override void Execute(Participant.Job job, LinkedListNode<Participant.BatchBuilder> nextBatchBuilder)
			{
				foreach (Participant.JobItem jobItem in job.GetActiveJobItems())
				{
					if (jobItem.Result.Origin == null)
					{
						jobItem.Result.Origin = jobItem.Input.Origin;
					}
					foreach (StorePropertyDefinition storePropertyDefinition in new List<StorePropertyDefinition>(jobItem.RequestedProperties))
					{
						if ((storePropertyDefinition.PropertyFlags & PropertyFlags.ReadOnly) == PropertyFlags.None)
						{
							jobItem.SetOrDeleteProperty(storePropertyDefinition, jobItem.Input.TryGetProperty(storePropertyDefinition));
						}
					}
				}
				base.Execute(job, nextBatchBuilder);
			}
		}

		private sealed class ReplacePropertyBatchBuilder : Participant.BatchBuilder
		{
			internal ReplacePropertyBatchBuilder(StorePropertyDefinition propertyDefinitionToReplace, StorePropertyDefinition propertyDefinitionToReplaceWith)
			{
				this.propertyDefinitionToReplace = propertyDefinitionToReplace;
				this.propertyDefinitionToReplaceWith = propertyDefinitionToReplaceWith;
			}

			protected internal override void Execute(Participant.Job job, LinkedListNode<Participant.BatchBuilder> nextBatchBuilder)
			{
				base.Execute(job, nextBatchBuilder);
				foreach (Participant.JobItem jobItem in job.GetActiveJobItems())
				{
					jobItem.SetOrDeleteProperty(this.propertyDefinitionToReplace, jobItem.Result.TryGetProperty(this.propertyDefinitionToReplaceWith));
				}
			}

			private readonly StorePropertyDefinition propertyDefinitionToReplace;

			private readonly StorePropertyDefinition propertyDefinitionToReplaceWith;
		}

		private class ADLookupBatchBuilder : Participant.BatchBuilder
		{
			internal ADLookupBatchBuilder(Participant.ADLookupBatchBuilder.LookupADRecipientDelegate lookupADRecipientDelegate, IList<PropertyDefinition> keyProperties)
			{
				if (lookupADRecipientDelegate == null)
				{
					throw new ArgumentNullException("lookupADRecipientDelegate");
				}
				if (keyProperties != null && keyProperties.Count < 1)
				{
					throw new ArgumentOutOfRangeException("keyProperties.Length");
				}
				this.lookupADRecipientDelegate = (StandaloneFuzzing.IsEnabled ? new Participant.ADLookupBatchBuilder.LookupADRecipientDelegate(Participant.ADLookupBatchBuilder.NoADLookup) : lookupADRecipientDelegate);
				if (keyProperties != null)
				{
					this.keyProperties = new HashSet<StorePropertyDefinition>(InternalSchema.ToStorePropertyDefinitions(keyProperties));
				}
			}

			protected internal override void Execute(Participant.Job job, LinkedListNode<Participant.BatchBuilder> nextBatchBuilder)
			{
				List<Participant.JobItem> list;
				List<ProxyAddress> list2;
				this.SelectJobItemsToLookup(job, out list, out list2);
				IList<Result<ADRawEntry>> list3;
				try
				{
					list3 = this.lookupADRecipientDelegate(list2.ToArray());
				}
				catch (DataSourceOperationException ex)
				{
					throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex, null, "Participant.ADRecipientCacheBatchBuilder.Execute. Failed due to directory exception {0}.", new object[]
					{
						ex
					});
				}
				catch (DataSourceTransientException ex2)
				{
					throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex2, null, "Participant.ADRecipientCacheBatchBuilder.Execute. Failed due to directory exception {0}.", new object[]
					{
						ex2
					});
				}
				for (int i = 0; i < list3.Count; i++)
				{
					if (list3[i].Data != null)
					{
						Participant.ADLookupBatchBuilder.CopyPropertiesFromAD(list[i], list3[i].Data, new List<StorePropertyDefinition>(list[i].RequestedProperties));
					}
					if (list3[i].Error != null)
					{
						base.SetError(list[i], ServerStrings.ExBatchBuilderADLookupFailed(list2[i], list3[i].Error), list3[i].Error);
					}
				}
				base.Execute(job, nextBatchBuilder);
			}

			private static IList<Result<ADRawEntry>> NoADLookup(IList<ProxyAddress> proxyAddresses)
			{
				return Array<Result<ADRawEntry>>.Empty;
			}

			private static void CopyPropertiesFromAD(Participant.JobItem jobItem, ADRawEntry source, IEnumerable<StorePropertyDefinition> propertyDefinitions)
			{
				ConversionPropertyBag conversionPropertyBag = new ConversionPropertyBag(source, StoreToDirectorySchemaConverter.Instance);
				foreach (StorePropertyDefinition storePropertyDefinition in propertyDefinitions)
				{
					if ((storePropertyDefinition.PropertyFlags & PropertyFlags.ReadOnly) == PropertyFlags.None)
					{
						object propertyValue = conversionPropertyBag.TryGetProperty(storePropertyDefinition);
						if (!PropertyError.IsPropertyError(propertyValue))
						{
							jobItem.SetOrDeleteProperty(storePropertyDefinition, propertyValue);
						}
					}
				}
				if (jobItem.Result.Origin == null || jobItem.Result.Origin is OneOffParticipantOrigin || jobItem.Result.Origin is DirectoryParticipantOrigin)
				{
					jobItem.Result.Origin = new DirectoryParticipantOrigin(source);
				}
			}

			private void SelectJobItemsToLookup(Participant.Job job, out List<Participant.JobItem> jobItemsToLookup, out List<ProxyAddress> proxyAddresses)
			{
				jobItemsToLookup = new List<Participant.JobItem>(job.Count);
				proxyAddresses = new List<ProxyAddress>(jobItemsToLookup.Capacity);
				foreach (Participant.JobItem jobItem in job.GetActiveJobItems())
				{
					Participant participant = new Participant(null, jobItem.GetValueOrDefault<string>(ParticipantSchema.EmailAddress), jobItem.GetValueOrDefault<string>(ParticipantSchema.RoutingType));
					if (participant.EmailAddress != null)
					{
						if (participant.ValidationStatus == ParticipantValidationStatus.NoError)
						{
							if ((this.keyProperties == null && jobItem.AnyPropertiesRequested) || jobItem.AreSomePropertiesRequested(this.keyProperties))
							{
								jobItemsToLookup.Add(jobItem);
								proxyAddresses.Add(new CustomProxyAddress(new CustomProxyAddressPrefix(participant.RoutingType), participant.EmailAddress, false));
							}
						}
						else
						{
							base.SetError(jobItem, participant.GetValidationMessage(), null);
						}
					}
				}
			}

			private readonly HashSet<StorePropertyDefinition> keyProperties;

			private readonly Participant.ADLookupBatchBuilder.LookupADRecipientDelegate lookupADRecipientDelegate;

			internal delegate IList<Result<ADRawEntry>> LookupADRecipientDelegate(ProxyAddress[] proxyAddresses);
		}

		internal class JobItem
		{
			public JobItem(Participant input)
			{
				this.input = input;
			}

			public JobItem(Participant input, Action<Result<Participant>> resultSetter) : this(input)
			{
				this.resultSetter = resultSetter;
			}

			public ProviderError Error
			{
				get
				{
					return this.error;
				}
			}

			public Participant Input
			{
				get
				{
					return this.input;
				}
			}

			public Participant.Builder Result
			{
				get
				{
					if (!this.IsActive)
					{
						return null;
					}
					return this.result;
				}
			}

			internal bool AnyPropertiesRequested
			{
				get
				{
					bool flag;
					using (IEnumerator<StorePropertyDefinition> enumerator = this.RequestedProperties.GetEnumerator())
					{
						flag = enumerator.MoveNext();
					}
					return flag;
				}
			}

			internal bool IsActive
			{
				get
				{
					return this.input != null && !this.inactiveBecauseOfError;
				}
			}

			internal IEnumerable<StorePropertyDefinition> RequestedProperties
			{
				get
				{
					foreach (StorePropertyDefinition propDef in this.requestedProperties)
					{
						if (this.IsPropertyRequestVisible(propDef))
						{
							yield return propDef;
						}
					}
					yield break;
				}
			}

			public void ApplyResult()
			{
				if (this.resultSetter != null)
				{
					this.resultSetter(new Result<Participant>((this.Result != null) ? this.Result.ToParticipant() : null, this.error));
				}
			}

			internal bool AreSomePropertiesRequested(params StorePropertyDefinition[] propDefs)
			{
				return this.AreSomePropertiesRequested((IEnumerable<StorePropertyDefinition>)propDefs);
			}

			internal bool AreSomePropertiesRequested(IEnumerable<StorePropertyDefinition> propDefs)
			{
				foreach (StorePropertyDefinition propDef in propDefs)
				{
					if (this.IsPropertyRequested(propDef))
					{
						return true;
					}
				}
				return false;
			}

			internal void IgnoreError()
			{
				this.inactiveBecauseOfError = false;
			}

			internal bool IsPropertyRequested(StorePropertyDefinition propDef)
			{
				return this.requestedProperties.Contains(propDef) && this.IsPropertyRequestVisible(propDef);
			}

			internal void RequestProperties(IEnumerable<StorePropertyDefinition> propDefs)
			{
				foreach (StorePropertyDefinition propDef in propDefs)
				{
					this.RequestProperty(propDef);
				}
			}

			internal void RequestProperty(StorePropertyDefinition propDef)
			{
				this.requestedProperties.TryAdd(propDef);
			}

			internal T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition)
			{
				return this.GetValueOrDefault<T>(propertyDefinition, default(T));
			}

			internal T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition, T defaultPropertyValue)
			{
				object propertyValue = this.result.TryGetProperty(propertyDefinition);
				if (PropertyError.IsPropertyNotFound(propertyValue))
				{
					propertyValue = this.input.TryGetProperty(propertyDefinition);
				}
				return PropertyBag.CheckPropertyValue<T>(propertyDefinition, propertyValue, defaultPropertyValue);
			}

			internal void InternalSetError(ProviderError error)
			{
				if (error == null)
				{
					throw new ArgumentNullException("error");
				}
				this.error = error;
				this.inactiveBecauseOfError = true;
			}

			internal void SetOrDeleteProperty(StorePropertyDefinition propertyDefinition, object propertyValue)
			{
				this.result.SetOrDeleteProperty(propertyDefinition, propertyValue);
				if (!PropertyError.IsPropertyNotFound(propertyValue))
				{
					this.requestedProperties.Remove(propertyDefinition);
				}
			}

			internal void SetOrDeleteRequestedProperty(StorePropertyDefinition propertyDefinition, object propertyValue)
			{
				if (this.requestedProperties.Contains(propertyDefinition))
				{
					this.SetOrDeleteProperty(propertyDefinition, propertyValue);
				}
			}

			private bool IsPropertyRequestVisible(StorePropertyDefinition propDef)
			{
				return (propDef.PropertyFlags & PropertyFlags.ReadOnly) == PropertyFlags.None || PropertyError.IsPropertyNotFound(this.result.TryGetProperty(propDef));
			}

			private readonly Participant input;

			private readonly HashSet<StorePropertyDefinition> requestedProperties = new HashSet<StorePropertyDefinition>();

			private readonly Participant.Builder result = new Participant.Builder();

			private readonly Action<Result<Participant>> resultSetter;

			private bool inactiveBecauseOfError;

			private ProviderError error;
		}

		internal sealed class BatchBuilderError : ProviderError
		{
			internal BatchBuilderError(Participant.BatchBuilder batchBuilder, string description, ProviderError innerError)
			{
				if (batchBuilder == null)
				{
					throw new ArgumentNullException("batchBuilder");
				}
				this.batchBuilder = batchBuilder;
				this.description = description;
				this.innerError = innerError;
			}

			public Participant.BatchBuilder BatchBuilder
			{
				get
				{
					return this.batchBuilder;
				}
			}

			public string Description
			{
				get
				{
					return this.description;
				}
			}

			public ProviderError InnerError
			{
				get
				{
					return this.innerError;
				}
			}

			public override string ToString()
			{
				return ServerStrings.ExBatchBuilderError(this.batchBuilder, this.description);
			}

			private readonly Participant.BatchBuilder batchBuilder;

			private readonly string description;

			private readonly ProviderError innerError;
		}

		internal class Job : Collection<Participant.JobItem>
		{
			public Job()
			{
			}

			public Job(IList<Participant.JobItem> jobItems) : base(jobItems)
			{
			}

			public Job(int capacity)
			{
				((List<Participant.JobItem>)base.Items).Capacity = capacity;
			}

			public Job(ICollection<Participant> jobItemInputs) : this(jobItemInputs.Count)
			{
				foreach (Participant input in jobItemInputs)
				{
					base.Add(new Participant.JobItem(input));
				}
			}

			public void ApplyResults()
			{
				foreach (Participant.JobItem jobItem in this)
				{
					jobItem.ApplyResult();
				}
			}

			internal IEnumerable<Participant.JobItem> GetActiveJobItems()
			{
				foreach (Participant.JobItem jobItem in this)
				{
					if (jobItem.IsActive)
					{
						yield return jobItem;
					}
				}
				yield break;
			}

			protected override void InsertItem(int index, Participant.JobItem item)
			{
				if (item == null)
				{
					throw new ArgumentNullException("item");
				}
				base.InsertItem(index, item);
			}

			protected override void SetItem(int index, Participant.JobItem item)
			{
				if (item == null)
				{
					throw new ArgumentNullException("item");
				}
				base.SetItem(index, item);
			}
		}
	}
}

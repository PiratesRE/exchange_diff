using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal abstract class SmtpMessageContextBlob : IInboundMessageContextBlob
	{
		protected SmtpMessageContextBlob(bool acceptBlobFromSmptIn, bool sendToSmtpOut, ProcessTransportRole role)
		{
			this.acceptBlobFromSmptIn = acceptBlobFromSmptIn;
			this.sendToSmtpOut = sendToSmtpOut;
			this.processTransportRole = role;
		}

		public static AdrcSmtpMessageContextBlob AdrcSmtpMessageContextBlobInstance
		{
			get
			{
				if (SmtpMessageContextBlob.adrcSmtpMessageContextBlobInstance == null)
				{
					SmtpMessageContextBlob.adrcSmtpMessageContextBlobInstance = new AdrcSmtpMessageContextBlob(Components.TransportAppConfig.MessageContextBlobConfiguration.AdvertiseADRecipientCache, Components.TransportAppConfig.MessageContextBlobConfiguration.TransferADRecipientCache, Components.Configuration.ProcessTransportRole);
				}
				return SmtpMessageContextBlob.adrcSmtpMessageContextBlobInstance;
			}
		}

		public static ExtendedPropertiesSmtpMessageContextBlob ExtendedPropertiesSmtpMessageContextBlobInstance
		{
			get
			{
				if (SmtpMessageContextBlob.extendedPropertiesSmtpMessageContextBlobInstance == null)
				{
					SmtpMessageContextBlob.extendedPropertiesSmtpMessageContextBlobInstance = new ExtendedPropertiesSmtpMessageContextBlob(Components.TransportAppConfig.MessageContextBlobConfiguration.AdvertiseExtendedProperties, Components.TransportAppConfig.MessageContextBlobConfiguration.TransferExtendedProperties, Components.Configuration.ProcessTransportRole);
				}
				return SmtpMessageContextBlob.extendedPropertiesSmtpMessageContextBlobInstance;
			}
		}

		public static FastIndexSmtpMessageContextBlob FastIndexSmtpMessageContextBlobInstance
		{
			get
			{
				if (SmtpMessageContextBlob.fastIndexSmtpMessageContextBlobInstance == null)
				{
					SmtpMessageContextBlob.fastIndexSmtpMessageContextBlobInstance = new FastIndexSmtpMessageContextBlob(Components.TransportAppConfig.MessageContextBlobConfiguration.AdvertiseFastIndex, Components.TransportAppConfig.MessageContextBlobConfiguration.TransferFastIndex, Components.Configuration.ProcessTransportRole);
				}
				return SmtpMessageContextBlob.fastIndexSmtpMessageContextBlobInstance;
			}
		}

		public virtual bool IsMandatory
		{
			get
			{
				return false;
			}
		}

		public abstract string Name { get; }

		public abstract ByteQuantifiedSize MaxBlobSize { get; }

		public bool AcceptBlobFromSmptIn
		{
			get
			{
				return this.acceptBlobFromSmptIn;
			}
		}

		public static List<SmtpMessageContextBlob> GetBlobsToSend(IEhloOptions ehloOptions, SmtpOutSession smtpOutSession)
		{
			List<SmtpMessageContextBlob> list = null;
			foreach (SmtpMessageContextBlob smtpMessageContextBlob in SmtpMessageContextBlob.GetSupportedBlobs())
			{
				if (smtpMessageContextBlob.sendToSmtpOut && smtpMessageContextBlob.IsAdvertised(ehloOptions) && smtpMessageContextBlob.HasDataToSend(smtpOutSession) && smtpMessageContextBlob.AllowNextHopType(smtpOutSession))
				{
					if (list == null)
					{
						list = new List<SmtpMessageContextBlob>();
					}
					list.Add(smtpMessageContextBlob);
				}
			}
			return list;
		}

		protected virtual bool AllowNextHopType(SmtpOutSession smtpOutSession)
		{
			return true;
		}

		public static bool TryGetOrderedListOfBlobsToReceive(string mailCommandParameter, out MailCommandMessageContextParameters messageContextInfo)
		{
			messageContextInfo = null;
			Match match = SmtpMessageContextBlob.MessageContextMailFromRegex.Match(mailCommandParameter);
			if (!match.Success)
			{
				return false;
			}
			List<IInboundMessageContextBlob> list = new List<IInboundMessageContextBlob>(3);
			Version adrc = null;
			Version eprop = null;
			Version fastIndex = null;
			foreach (object obj in match.Groups["Blobs"].Captures)
			{
				Capture capture = (Capture)obj;
				if (capture.Value.StartsWith(AdrcSmtpMessageContextBlob.ADRCBlobName, true, CultureInfo.InvariantCulture))
				{
					if (list.Contains(SmtpMessageContextBlob.AdrcSmtpMessageContextBlobInstance))
					{
						return false;
					}
					if (!AdrcSmtpMessageContextBlob.IsSupportedVersion(capture.Value, true, out adrc))
					{
						return false;
					}
					list.Add(SmtpMessageContextBlob.AdrcSmtpMessageContextBlobInstance);
				}
				else if (capture.Value.StartsWith(ExtendedPropertiesSmtpMessageContextBlob.ExtendedPropertiesBlobName, true, CultureInfo.InvariantCulture))
				{
					if (list.Contains(SmtpMessageContextBlob.ExtendedPropertiesSmtpMessageContextBlobInstance))
					{
						return false;
					}
					if (!ExtendedPropertiesSmtpMessageContextBlob.IsSupportedVersion(capture.Value, out eprop))
					{
						return false;
					}
					list.Add(SmtpMessageContextBlob.ExtendedPropertiesSmtpMessageContextBlobInstance);
				}
				else
				{
					if (!capture.Value.StartsWith(FastIndexSmtpMessageContextBlob.FastIndexBlobName, true, CultureInfo.InvariantCulture))
					{
						throw new InvalidOperationException("Unexpected " + capture.Value);
					}
					if (list.Contains(SmtpMessageContextBlob.FastIndexSmtpMessageContextBlobInstance))
					{
						return false;
					}
					if (!FastIndexSmtpMessageContextBlob.IsSupportedVersion(capture.Value, out fastIndex))
					{
						return false;
					}
					list.Add(SmtpMessageContextBlob.FastIndexSmtpMessageContextBlobInstance);
				}
			}
			messageContextInfo = new MailCommandMessageContextParameters(mailCommandParameter, adrc, eprop, fastIndex, list);
			return true;
		}

		public static List<SmtpMessageContextBlob> GetAdvertisedMandatoryBlobs(IEhloOptions ehloOptions)
		{
			List<SmtpMessageContextBlob> list = new List<SmtpMessageContextBlob>(1);
			foreach (SmtpMessageContextBlob smtpMessageContextBlob in SmtpMessageContextBlob.GetSupportedBlobs())
			{
				if (smtpMessageContextBlob.IsMandatory && smtpMessageContextBlob.IsAdvertised(ehloOptions))
				{
					list.Add(smtpMessageContextBlob);
				}
			}
			return list;
		}

		public abstract bool IsAdvertised(IEhloOptions ehloOptions);

		public abstract void DeserializeBlob(Stream stream, ISmtpInSession smtpInSession, long blobSize);

		public abstract void DeserializeBlob(Stream stream, SmtpInSessionState sessionState, long blobSize);

		public abstract Stream SerializeBlob(SmtpOutSession smtpOutSession);

		public abstract bool VerifyPermission(Permission permission);

		public virtual bool HasDataToSend(SmtpOutSession smtpOutSession)
		{
			return true;
		}

		public virtual string GetVersionToSend(IEhloOptions ehloOptions)
		{
			return this.Name;
		}

		protected static int ReadInt(Stream stream, byte[] intValueReadBuffer)
		{
			stream.Read(intValueReadBuffer, 0, 4);
			return BitConverter.ToInt32(intValueReadBuffer, 0);
		}

		protected T ReadTypeAndValidate<T>(TransportPropertyStreamReader reader)
		{
			object obj = reader.ReadValue();
			if (obj == null || obj.GetType() != typeof(T))
			{
				string text = (obj == null) ? "<null>" : obj.GetType().ToString();
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Encountered unexpected value {0} while deserializing. Expected type was {1}", new object[]
				{
					text,
					typeof(T)
				}));
			}
			return (T)((object)obj);
		}

		protected void SerializeValue(Stream ms, ref byte[] buffer, object value)
		{
			StreamPropertyType propetyTypeForValue = this.GetPropetyTypeForValue(value);
			int size = TransportPropertyStreamWriter.SizeOf(propetyTypeForValue, value);
			buffer = this.IncreaseBufferIfSmall(size, buffer);
			int count = 0;
			TransportPropertyStreamWriter.Serialize(propetyTypeForValue, value, buffer, ref count);
			ms.Write(buffer, 0, count);
		}

		protected bool IsNativelySupported(ADPropertyDefinition propertyDefinition)
		{
			Type type = propertyDefinition.Type;
			if (!propertyDefinition.IsMultivalued)
			{
				return SmtpMessageContextBlob.supportedTypes.ContainsKey(type);
			}
			return SmtpMessageContextBlob.multiValuedSupportedTypes.Contains(type);
		}

		protected StreamPropertyType GetPropetyTypeForValue(object value)
		{
			if (value == null)
			{
				return StreamPropertyType.Null;
			}
			StreamPropertyType result;
			if (SmtpMessageContextBlob.supportedTypes.TryGetValue(value.GetType(), out result))
			{
				return result;
			}
			if (typeof(ProxyAddress).IsAssignableFrom(value.GetType()))
			{
				return StreamPropertyType.ProxyAddress;
			}
			throw new InvalidOperationException(string.Format("Do not know how to serialize type {0}. If this is a new type added to ADRecipientCache, make sure it is convertable by ADValueConvertor or add support for the type in PropertWriter/PropertyReader. Also remember to increment the version if needed.", value.GetType()));
		}

		protected byte[] IncreaseBufferIfSmall(int size, byte[] buffer)
		{
			if (size > buffer.Length)
			{
				buffer = new byte[size * 2];
			}
			return buffer;
		}

		private static List<SmtpMessageContextBlob> GetSupportedBlobs()
		{
			if (SmtpMessageContextBlob.supportedBlobs == null)
			{
				SmtpMessageContextBlob.supportedBlobs = new List<SmtpMessageContextBlob>
				{
					SmtpMessageContextBlob.AdrcSmtpMessageContextBlobInstance,
					SmtpMessageContextBlob.ExtendedPropertiesSmtpMessageContextBlobInstance,
					SmtpMessageContextBlob.FastIndexSmtpMessageContextBlobInstance
				};
			}
			return SmtpMessageContextBlob.supportedBlobs;
		}

		private static HashSet<Type> GetMultiValuedSupportedTypes()
		{
			return new HashSet<Type>
			{
				typeof(bool),
				typeof(byte),
				typeof(sbyte),
				typeof(short),
				typeof(ushort),
				typeof(int),
				typeof(uint),
				typeof(long),
				typeof(ulong),
				typeof(float),
				typeof(double),
				typeof(decimal),
				typeof(char),
				typeof(string),
				typeof(DateTime),
				typeof(Guid),
				typeof(IPAddress),
				typeof(IPEndPoint),
				typeof(RoutingAddress),
				typeof(ADObjectId),
				typeof(ADObjectIdWithString),
				typeof(byte[]),
				typeof(ProxyAddress)
			};
		}

		private static Dictionary<Type, StreamPropertyType> GetSupportedTypes()
		{
			TypeEntry[] array = new TypeEntry[]
			{
				new TypeEntry(typeof(DBNull), StreamPropertyType.Null),
				new TypeEntry(typeof(bool), StreamPropertyType.Bool),
				new TypeEntry(typeof(byte), StreamPropertyType.Byte),
				new TypeEntry(typeof(sbyte), StreamPropertyType.SByte),
				new TypeEntry(typeof(short), StreamPropertyType.Int16),
				new TypeEntry(typeof(ushort), StreamPropertyType.UInt16),
				new TypeEntry(typeof(int), StreamPropertyType.Int32),
				new TypeEntry(typeof(uint), StreamPropertyType.UInt32),
				new TypeEntry(typeof(long), StreamPropertyType.Int64),
				new TypeEntry(typeof(ulong), StreamPropertyType.UInt64),
				new TypeEntry(typeof(float), StreamPropertyType.Single),
				new TypeEntry(typeof(double), StreamPropertyType.Double),
				new TypeEntry(typeof(decimal), StreamPropertyType.Decimal),
				new TypeEntry(typeof(char), StreamPropertyType.Char),
				new TypeEntry(typeof(string), StreamPropertyType.String),
				new TypeEntry(typeof(DateTime), StreamPropertyType.DateTime),
				new TypeEntry(typeof(Guid), StreamPropertyType.Guid),
				new TypeEntry(typeof(IPAddress), StreamPropertyType.IPAddress),
				new TypeEntry(typeof(IPEndPoint), StreamPropertyType.IPEndPoint),
				new TypeEntry(typeof(RoutingAddress), StreamPropertyType.RoutingAddress),
				new TypeEntry(typeof(ADObjectId), StreamPropertyType.ADObjectIdUTF8),
				new TypeEntry(typeof(Microsoft.Exchange.Data.Directory.Recipient.RecipientType), StreamPropertyType.RecipientType),
				new TypeEntry(typeof(ADObjectIdWithString), StreamPropertyType.ADObjectIdWithString),
				new TypeEntry(typeof(bool[]), StreamPropertyType.Bool | StreamPropertyType.Array),
				new TypeEntry(typeof(byte[]), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.Array),
				new TypeEntry(typeof(sbyte[]), StreamPropertyType.SByte | StreamPropertyType.Array),
				new TypeEntry(typeof(short[]), StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.Array),
				new TypeEntry(typeof(ushort[]), StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.Array),
				new TypeEntry(typeof(int[]), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.Array),
				new TypeEntry(typeof(uint[]), StreamPropertyType.UInt32 | StreamPropertyType.Array),
				new TypeEntry(typeof(long[]), StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.Array),
				new TypeEntry(typeof(ulong[]), StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.Array),
				new TypeEntry(typeof(float[]), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.Array),
				new TypeEntry(typeof(double[]), StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array),
				new TypeEntry(typeof(decimal[]), StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array),
				new TypeEntry(typeof(char[]), StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array),
				new TypeEntry(typeof(string[]), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array),
				new TypeEntry(typeof(DateTime[]), StreamPropertyType.DateTime | StreamPropertyType.Array),
				new TypeEntry(typeof(Guid[]), StreamPropertyType.Null | StreamPropertyType.DateTime | StreamPropertyType.Array),
				new TypeEntry(typeof(IPAddress[]), StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.Array),
				new TypeEntry(typeof(IPEndPoint[]), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.Array),
				new TypeEntry(typeof(RoutingAddress[]), StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array),
				new TypeEntry(typeof(ADObjectId[]), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array),
				new TypeEntry(typeof(ADObjectIdWithString[]), StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.Array),
				new TypeEntry(typeof(List<bool>), StreamPropertyType.Bool | StreamPropertyType.List),
				new TypeEntry(typeof(List<byte>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.List),
				new TypeEntry(typeof(List<sbyte>), StreamPropertyType.SByte | StreamPropertyType.List),
				new TypeEntry(typeof(List<short>), StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.List),
				new TypeEntry(typeof(List<ushort>), StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.List),
				new TypeEntry(typeof(List<int>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.List),
				new TypeEntry(typeof(List<uint>), StreamPropertyType.UInt32 | StreamPropertyType.List),
				new TypeEntry(typeof(List<long>), StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.List),
				new TypeEntry(typeof(List<ulong>), StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.List),
				new TypeEntry(typeof(List<float>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.List),
				new TypeEntry(typeof(List<double>), StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List),
				new TypeEntry(typeof(List<decimal>), StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List),
				new TypeEntry(typeof(List<char>), StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List),
				new TypeEntry(typeof(List<string>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List),
				new TypeEntry(typeof(List<DateTime>), StreamPropertyType.DateTime | StreamPropertyType.List),
				new TypeEntry(typeof(List<Guid>), StreamPropertyType.Null | StreamPropertyType.DateTime | StreamPropertyType.List),
				new TypeEntry(typeof(List<IPAddress>), StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.List),
				new TypeEntry(typeof(List<IPEndPoint>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.List),
				new TypeEntry(typeof(List<RoutingAddress>), StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List),
				new TypeEntry(typeof(List<ADObjectId>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List),
				new TypeEntry(typeof(List<ADObjectIdWithString>), StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.List),
				new TypeEntry(typeof(List<byte[]>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.Array | StreamPropertyType.List),
				new TypeEntry(typeof(MultiValuedProperty<bool>), StreamPropertyType.Bool | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<byte>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<sbyte>), StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<short>), StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<ushort>), StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<int>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<uint>), StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<long>), StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<ulong>), StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<float>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<double>), StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<decimal>), StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<char>), StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<string>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<DateTime>), StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<Guid>), StreamPropertyType.Null | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<IPAddress>), StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<IPEndPoint>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<RoutingAddress>), StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<ADObjectId>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<ADObjectIdWithString>), StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(MultiValuedProperty<byte[]>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.Array | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ProxyAddressCollection), StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<bool>), StreamPropertyType.Bool | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<byte>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<sbyte>), StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<short>), StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<ushort>), StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<int>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<uint>), StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<long>), StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<ulong>), StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<float>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<double>), StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<decimal>), StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<char>), StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<string>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<DateTime>), StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<Guid>), StreamPropertyType.Null | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<IPAddress>), StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<IPEndPoint>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<RoutingAddress>), StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<ADObjectId>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<ADObjectIdWithString>), StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty),
				new TypeEntry(typeof(ADMultiValuedProperty<byte[]>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.Array | StreamPropertyType.MultiValuedProperty)
			};
			Dictionary<Type, StreamPropertyType> dictionary = new Dictionary<Type, StreamPropertyType>(array.Length);
			foreach (TypeEntry typeEntry in array)
			{
				dictionary.Add(typeEntry.Type, typeEntry.Identifier);
			}
			return dictionary;
		}

		public static readonly string ProcessTransportRoleKey = "Microsoft.Exchange.Protocols.Smtp.SmtpMessageContextBlob.ProcessTransportRole";

		private static readonly Regex MessageContextMailFromRegex = new Regex("^((?<Blobs>(ADRC|EPROP|FSTINDX)-\\d{1,5}\\.\\d{1,7}\\.\\d{1,7}\\.\\d{1,7})(,)?){1,20}$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

		private static AdrcSmtpMessageContextBlob adrcSmtpMessageContextBlobInstance;

		private static ExtendedPropertiesSmtpMessageContextBlob extendedPropertiesSmtpMessageContextBlobInstance;

		private static FastIndexSmtpMessageContextBlob fastIndexSmtpMessageContextBlobInstance;

		private static List<SmtpMessageContextBlob> supportedBlobs;

		private static Dictionary<Type, StreamPropertyType> supportedTypes = SmtpMessageContextBlob.GetSupportedTypes();

		private static HashSet<Type> multiValuedSupportedTypes = SmtpMessageContextBlob.GetMultiValuedSupportedTypes();

		private readonly bool acceptBlobFromSmptIn;

		private readonly bool sendToSmtpOut;

		protected readonly ProcessTransportRole processTransportRole;
	}
}

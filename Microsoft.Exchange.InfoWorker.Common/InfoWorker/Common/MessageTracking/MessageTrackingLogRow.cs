using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.LogSearch;
using Microsoft.Exchange.Transport.Logging.Search;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal sealed class MessageTrackingLogRow
	{
		private static MessageTrackingLogRow.ValidateMethod[] CreateValidateMethods()
		{
			MessageTrackingLogRow.ValidateMethod[] array = new MessageTrackingLogRow.ValidateMethod[MessageTrackingLogRow.FieldCount];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = null;
			}
			array[8] = new MessageTrackingLogRow.ValidateMethod(MessageTrackingLogRow.EnumerationValidator<MessageTrackingEvent>);
			array[7] = new MessageTrackingLogRow.ValidateMethod(MessageTrackingLogRow.EnumerationValidator<MessageTrackingSource>);
			array[9] = new MessageTrackingLogRow.ValidateMethod(MessageTrackingLogRow.InternalMessageIdValidator);
			array[12] = new MessageTrackingLogRow.ValidateMethod(MessageTrackingLogRow.SmtpAddressArrayValidator);
			array[19] = new MessageTrackingLogRow.ValidateMethod(MessageTrackingLogRow.SmtpAddressValidator);
			array[20] = null;
			return array;
		}

		private static bool DateTimeValidator(object value)
		{
			DateTime dateTime;
			return DateTime.TryParse((string)value, out dateTime);
		}

		private static bool SmtpAddressValidator(object value)
		{
			return SmtpAddress.IsValidSmtpAddress((string)value);
		}

		private static bool SmtpAddressValidatorWithNull(object value)
		{
			return string.IsNullOrEmpty((string)value) || MessageTrackingLogRow.SmtpAddressValidator(value);
		}

		private static bool EnumerationValidator<T>(object value) where T : struct
		{
			T t;
			return value != null && EnumValidator<T>.TryParse((string)value, EnumParseOptions.Default, out t);
		}

		private static bool InternalMessageIdValidator(object value)
		{
			long num;
			return string.IsNullOrEmpty((string)value) || long.TryParse((string)value, out num);
		}

		private static bool SmtpAddressArrayValidator(object value)
		{
			if (value == null)
			{
				return true;
			}
			string[] array = (string[])value;
			if (array.Length == 0 || (array.Length == 1 && string.IsNullOrEmpty(array[0])))
			{
				return true;
			}
			foreach (string value2 in array)
			{
				if (!MessageTrackingLogRow.SmtpAddressValidator(value2))
				{
					return false;
				}
			}
			return true;
		}

		private static bool ProxyAddressArrayValidator(object value)
		{
			string[] array = (string[])value;
			foreach (string value2 in array)
			{
				if (!MessageTrackingLogRow.ProxyAddressValidator(value2))
				{
					return false;
				}
			}
			return true;
		}

		private static bool ProxyAddressValidator(object value)
		{
			ProxyAddress proxyAddress = null;
			return ProxyAddress.TryParse(value as string, out proxyAddress);
		}

		private static bool ValidateField(MessageTrackingLogRow logEntry, int fieldIdx, object value, TrackingErrorCollection errors)
		{
			MessageTrackingLogRow.ValidateMethod validator = MessageTrackingLogRow.GetValidator(logEntry, fieldIdx);
			if (validator != null && !validator(value))
			{
				string text = string.Format("The message-tracking data on server {0} had invalid data in column {1} for this message. Event \"{2}\" will be ignored", logEntry.serverFqdn, fieldIdx, logEntry.EventId.ToString());
				errors.Add(ErrorCode.UnexpectedErrorPermanent, logEntry.ServerHostName, text, string.Empty);
				TraceWrapper.SearchLibraryTracer.TraceError(logEntry.GetHashCode(), text, new object[0]);
				return false;
			}
			return true;
		}

		private static MessageTrackingLogRow.ValidateMethod GetValidator(MessageTrackingLogRow logEntry, int fieldIdx)
		{
			if (!logEntry.IsValidColumn(MessageTrackingField.EventId))
			{
				return null;
			}
			if (logEntry.EventId == MessageTrackingEvent.THROTTLE && fieldIdx == 12)
			{
				return new MessageTrackingLogRow.ValidateMethod(MessageTrackingLogRow.ProxyAddressArrayValidator);
			}
			if (logEntry.EventId == MessageTrackingEvent.DEFER && logEntry.Source == MessageTrackingSource.SMTP && fieldIdx == 19)
			{
				return null;
			}
			return MessageTrackingLogRow.validateMethods[fieldIdx];
		}

		private static string[] CanonicalizeStringArray(string[] value)
		{
			if (value == null || value.Length == 0 || (value.Length == 1 && string.IsNullOrEmpty(value[0])))
			{
				return null;
			}
			return value;
		}

		private MessageTrackingLogRow()
		{
		}

		private T GetColumn<T>(MessageTrackingField idx)
		{
			if (!this.validColumns[(int)idx])
			{
				throw new InvalidOperationException();
			}
			return (T)((object)this.columns[(int)idx]);
		}

		public static bool TryRead(string server, LogSearchCursor cursor, BitArray fieldsToGet, TrackingErrorCollection errors, out MessageTrackingLogRow logEntry)
		{
			logEntry = null;
			MessageTrackingLogRow messageTrackingLogRow = new MessageTrackingLogRow();
			messageTrackingLogRow.serverFqdn = server;
			for (int i = 0; i < fieldsToGet.Length; i++)
			{
				if (fieldsToGet[i])
				{
					object obj = null;
					Exception ex = null;
					try
					{
						obj = cursor.GetField(i);
					}
					catch (LogSearchException ex2)
					{
						ex = ex2;
						int errorCode = ex2.ErrorCode;
					}
					catch (RpcException ex3)
					{
						ex = ex3;
						int errorCode2 = ex3.ErrorCode;
					}
					if (ex != null)
					{
						TrackingTransientException.AddAndRaiseETX(errors, ErrorCode.LogSearchConnection, server, ex.ToString());
					}
					if (obj != null && obj.GetType() != MessageTrackingSchema.MessageTrackingEvent.Fields[i].Type)
					{
						throw new InvalidOperationException(string.Format("Schema for column {0} in message-tracking is of sync with tasks", i));
					}
					if (!MessageTrackingLogRow.ValidateField(messageTrackingLogRow, i, obj, errors))
					{
						return false;
					}
					messageTrackingLogRow.columns[i] = cursor.GetField(i);
					messageTrackingLogRow.validColumns[i] = true;
				}
			}
			if (fieldsToGet[8] && (messageTrackingLogRow.EventId == MessageTrackingEvent.EXPAND || messageTrackingLogRow.EventId == MessageTrackingEvent.RESOLVE))
			{
				MessageTrackingLogRow.SmtpAddressValidatorWithNull(messageTrackingLogRow.RelatedRecipientAddress);
			}
			logEntry = messageTrackingLogRow;
			return true;
		}

		public static BitArray GetColumnFilter(params MessageTrackingField[] columns)
		{
			BitArray bitArray = new BitArray(MessageTrackingLogRow.FieldCount);
			foreach (MessageTrackingField index in columns)
			{
				bitArray[(int)index] = true;
			}
			return bitArray;
		}

		public bool IsValidColumn(MessageTrackingField column)
		{
			return this.validColumns[(int)column];
		}

		public DateTime DateTime
		{
			get
			{
				return this.GetColumn<DateTime>(MessageTrackingField.Timestamp);
			}
		}

		public string MessageId
		{
			get
			{
				return this.GetColumn<string>(MessageTrackingField.MessageId);
			}
		}

		public string ClientHostName
		{
			get
			{
				return this.GetColumn<string>(MessageTrackingField.ClientHostname);
			}
		}

		public string ClientIP
		{
			get
			{
				return this.GetColumn<string>(MessageTrackingField.ClientIp);
			}
		}

		public long InternalMessageId
		{
			get
			{
				string column = this.GetColumn<string>(MessageTrackingField.InternalMessageId);
				if (string.IsNullOrEmpty(column))
				{
					return 0L;
				}
				return long.Parse(column);
			}
		}

		public string SenderAddress
		{
			get
			{
				return this.GetColumn<string>(MessageTrackingField.SenderAddress);
			}
		}

		public string[] RecipientAddresses
		{
			get
			{
				return MessageTrackingLogRow.CanonicalizeStringArray(this.GetColumn<string[]>(MessageTrackingField.RecipientAddress));
			}
		}

		public string[] RecipientStatuses
		{
			get
			{
				return MessageTrackingLogRow.CanonicalizeStringArray(this.GetColumn<string[]>(MessageTrackingField.RecipientStatus));
			}
		}

		public string MessageSubject
		{
			get
			{
				return this.GetColumn<string>(MessageTrackingField.MessageSubject);
			}
		}

		public MessageTrackingEvent EventId
		{
			get
			{
				string column = this.GetColumn<string>(MessageTrackingField.EventId);
				return EnumValidator<MessageTrackingEvent>.Parse(column, EnumParseOptions.Default);
			}
		}

		public MessageTrackingSource Source
		{
			get
			{
				string column = this.GetColumn<string>(MessageTrackingField.Source);
				return EnumValidator<MessageTrackingSource>.Parse(column, EnumParseOptions.Default);
			}
		}

		public string ServerHostName
		{
			get
			{
				return this.GetColumn<string>(MessageTrackingField.ServerHostname);
			}
		}

		public string ServerIP
		{
			get
			{
				return this.GetColumn<string>(MessageTrackingField.ServerIp);
			}
		}

		public string[] References
		{
			get
			{
				return this.GetColumn<string[]>(MessageTrackingField.Reference);
			}
		}

		public string RelatedRecipientAddress
		{
			get
			{
				return this.GetColumn<string>(MessageTrackingField.RelatedRecipientAddress);
			}
		}

		public string Context
		{
			get
			{
				return this.GetColumn<string>(MessageTrackingField.SourceContext);
			}
		}

		public string ServerFqdn
		{
			get
			{
				return this.serverFqdn;
			}
		}

		public KeyValuePair<string, object>[] CustomData
		{
			get
			{
				return this.GetColumn<KeyValuePair<string, object>[]>(MessageTrackingField.CustomData);
			}
		}

		public string TenantId
		{
			get
			{
				return this.GetColumn<string>(MessageTrackingField.TenantId);
			}
		}

		public bool IsLogCompatible
		{
			get
			{
				if (this.isLogCompatible == null)
				{
					string customData = this.GetCustomData<string>("Version", string.Empty);
					this.isLogCompatible = new bool?(string.IsNullOrEmpty(customData));
				}
				return this.isLogCompatible.Value;
			}
		}

		public T GetCustomData<T>(string key, T defaultValue)
		{
			KeyValuePair<string, object>[] customData = this.CustomData;
			if (customData == null || customData.Length == 0)
			{
				return defaultValue;
			}
			foreach (KeyValuePair<string, object> keyValuePair in customData)
			{
				if (string.Equals(keyValuePair.Key, key, StringComparison.OrdinalIgnoreCase))
				{
					object value = keyValuePair.Value;
					if (!(value is T))
					{
						TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "The message-tracking data on server {0} had invalid data in the CustomData column for this message", new object[]
						{
							this.ServerFqdn
						});
					}
					return (T)((object)value);
				}
			}
			return defaultValue;
		}

		public const string VersionPropertyName = "Version";

		public static readonly int FieldCount = MessageTrackingSchema.MessageTrackingEvent.Fields.Length;

		private static MessageTrackingLogRow.ValidateMethod[] validateMethods = MessageTrackingLogRow.CreateValidateMethods();

		private object[] columns = new object[MessageTrackingLogRow.FieldCount];

		private BitArray validColumns = new BitArray(MessageTrackingLogRow.FieldCount);

		private string serverFqdn;

		private bool? isLogCompatible;

		private delegate bool ValidateMethod(object value);
	}
}

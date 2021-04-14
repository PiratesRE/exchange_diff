using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.HttpProxy.Routing.RoutingDestinations;
using Microsoft.Exchange.HttpProxy.Routing.RoutingEntries;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy.Routing.Serialization
{
	public static class RoutingEntryHeaderSerializer
	{
		public static IRoutingEntry Deserialize(string headerValue)
		{
			if (headerValue == null)
			{
				throw new ArgumentNullException("headerValue");
			}
			RoutingEntryHeaderSerializer.RoutingEntryParts routingEntryParts;
			if (!RoutingEntryHeaderSerializer.TrySplitEntry(headerValue, out routingEntryParts))
			{
				throw new ArgumentException("Value is incorrectly formatted", "headerValue");
			}
			IRoutingKey key = RoutingEntryHeaderSerializer.DeserializeRoutingKey(routingEntryParts.KeyType, routingEntryParts.KeyValue);
			IRoutingDestination destination = RoutingEntryHeaderSerializer.DeserializeRoutingDestination(routingEntryParts.DestinationType, routingEntryParts.DestinationValue);
			return RoutingEntryHeaderSerializer.AssembleRoutingEntry(key, destination, routingEntryParts.Timestamp);
		}

		public static bool IsValidHeaderString(string headerValue)
		{
			RoutingEntryHeaderSerializer.RoutingEntryParts routingEntryParts;
			return headerValue != null && RoutingEntryHeaderSerializer.TrySplitEntry(headerValue, out routingEntryParts);
		}

		public static string Serialize(IRoutingEntry routingEntry)
		{
			if (routingEntry == null)
			{
				throw new ArgumentNullException("routingEntry");
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(RoutingEntryHeaderSerializer.SerializableRoutingTypeToString(routingEntry.Key.RoutingItemType));
			stringBuilder.Append(':');
			stringBuilder.Append(Uri.EscapeDataString(routingEntry.Key.Value));
			stringBuilder.Append('=');
			stringBuilder.Append(RoutingEntryHeaderSerializer.SerializableRoutingTypeToString(routingEntry.Destination.RoutingItemType));
			stringBuilder.Append(':');
			stringBuilder.Append(Uri.EscapeDataString(routingEntry.Destination.Value));
			foreach (string value in routingEntry.Destination.Properties)
			{
				stringBuilder.Append('+');
				stringBuilder.Append(value);
			}
			stringBuilder.Append('@');
			stringBuilder.Append(routingEntry.Timestamp);
			return stringBuilder.ToString();
		}

		public static string RoutingTypeToString(RoutingItemType routingType)
		{
			string result;
			if (!RoutingEntryHeaderSerializer.TryGetDefinedRoutingTypeToString(routingType, out result))
			{
				result = routingType.ToString();
			}
			return result;
		}

		internal static string SerializableRoutingTypeToString(RoutingItemType routingType)
		{
			if (routingType == RoutingItemType.Unknown)
			{
				throw new NotSupportedException("Cannot serialize routing entries of type Unknown");
			}
			string result;
			if (!RoutingEntryHeaderSerializer.TryGetDefinedRoutingTypeToString(routingType, out result))
			{
				throw new ArgumentException(string.Format("Unrecognized routing type: {0}", routingType));
			}
			return result;
		}

		internal static bool TryGetDefinedRoutingTypeToString(RoutingItemType routingType, out string routingTypeString)
		{
			routingTypeString = null;
			switch (routingType)
			{
			case RoutingItemType.ArchiveSmtp:
				routingTypeString = "ArchiveSmtp";
				return true;
			case RoutingItemType.DatabaseGuid:
				routingTypeString = "DatabaseGuid";
				return true;
			case RoutingItemType.Error:
				routingTypeString = "Error";
				return true;
			case RoutingItemType.MailboxGuid:
				routingTypeString = "MailboxGuid";
				return true;
			case RoutingItemType.Server:
				routingTypeString = "Server";
				return true;
			case RoutingItemType.Smtp:
				routingTypeString = "Smtp";
				return true;
			case RoutingItemType.ExternalDirectoryObjectId:
				routingTypeString = "Oid";
				return true;
			case RoutingItemType.LiveIdMemberName:
				routingTypeString = "LiveIdMemberName";
				return true;
			case RoutingItemType.Unknown:
				routingTypeString = "Unknown";
				return true;
			default:
				return false;
			}
		}

		private static IRoutingEntry AssembleRoutingEntry(IRoutingKey key, IRoutingDestination destination, long timestamp)
		{
			if (key is ServerRoutingKey)
			{
				if (destination is ServerRoutingDestination)
				{
					return new SuccessfulServerRoutingEntry((ServerRoutingKey)key, (ServerRoutingDestination)destination, timestamp);
				}
				if (destination is ErrorRoutingDestination)
				{
					return new FailedServerRoutingEntry((ServerRoutingKey)key, (ErrorRoutingDestination)destination, timestamp);
				}
			}
			if (key is DatabaseGuidRoutingKey)
			{
				if (destination is ServerRoutingDestination)
				{
					return new SuccessfulDatabaseGuidRoutingEntry((DatabaseGuidRoutingKey)key, (ServerRoutingDestination)destination, timestamp);
				}
				if (destination is ErrorRoutingDestination)
				{
					return new FailedDatabaseGuidRoutingEntry((DatabaseGuidRoutingKey)key, (ErrorRoutingDestination)destination, timestamp);
				}
			}
			else if (key is ArchiveSmtpRoutingKey || key is MailboxGuidRoutingKey || key is SmtpRoutingKey || key is ExternalDirectoryObjectIdRoutingKey || key is LiveIdMemberNameRoutingKey)
			{
				if (destination is DatabaseGuidRoutingDestination)
				{
					return new SuccessfulMailboxRoutingEntry(key, (DatabaseGuidRoutingDestination)destination, timestamp);
				}
				if (destination is ErrorRoutingDestination)
				{
					return new FailedMailboxRoutingEntry(key, (ErrorRoutingDestination)destination, timestamp);
				}
			}
			return new GenericRoutingEntry(key, destination, timestamp);
		}

		private static IRoutingDestination DeserializeRoutingDestination(string type, string value)
		{
			int num = value.IndexOf('+');
			string[] properties = Array<string>.Empty;
			if (num != -1)
			{
				properties = value.Substring(num + 1).Split(new char[]
				{
					'+'
				});
				value = value.Substring(0, num);
			}
			if (type != null)
			{
				DatabaseGuidRoutingDestination result3;
				if (!(type == "DatabaseGuid"))
				{
					ErrorRoutingDestination result2;
					if (!(type == "Error"))
					{
						if (type == "Server")
						{
							ServerRoutingDestination result;
							if (ServerRoutingDestination.TryParse(value, properties, out result))
							{
								return result;
							}
						}
					}
					else if (ErrorRoutingDestination.TryParse(value, properties, out result2))
					{
						return result2;
					}
				}
				else if (DatabaseGuidRoutingDestination.TryParse(value, properties, out result3))
				{
					return result3;
				}
			}
			return new UnknownRoutingDestination(type, value, properties);
		}

		private static IRoutingKey DeserializeRoutingKey(string type, string value)
		{
			switch (type)
			{
			case "ArchiveSmtp":
			{
				ArchiveSmtpRoutingKey result;
				if (ArchiveSmtpRoutingKey.TryParse(value, out result))
				{
					return result;
				}
				break;
			}
			case "DatabaseGuid":
			{
				DatabaseGuidRoutingKey result2;
				if (DatabaseGuidRoutingKey.TryParse(value, out result2))
				{
					return result2;
				}
				break;
			}
			case "MailboxGuid":
			{
				MailboxGuidRoutingKey result3;
				if (MailboxGuidRoutingKey.TryParse(value, out result3))
				{
					return result3;
				}
				break;
			}
			case "Smtp":
			{
				SmtpRoutingKey result4;
				if (SmtpRoutingKey.TryParse(value, out result4))
				{
					return result4;
				}
				break;
			}
			case "Server":
			{
				ServerRoutingKey result5;
				if (ServerRoutingKey.TryParse(value, out result5))
				{
					return result5;
				}
				break;
			}
			case "Oid":
			{
				ExternalDirectoryObjectIdRoutingKey result6;
				if (ExternalDirectoryObjectIdRoutingKey.TryParse(value, out result6))
				{
					return result6;
				}
				break;
			}
			case "LiveIdMemberName":
			{
				LiveIdMemberNameRoutingKey result7;
				if (LiveIdMemberNameRoutingKey.TryParse(value, out result7))
				{
					return result7;
				}
				break;
			}
			}
			return new UnknownRoutingKey(type, value);
		}

		private static bool TrySplitEntry(string entryString, out RoutingEntryHeaderSerializer.RoutingEntryParts parts)
		{
			parts = default(RoutingEntryHeaderSerializer.RoutingEntryParts);
			int num = entryString.IndexOf('=');
			if (num == -1)
			{
				return false;
			}
			int num2 = entryString.IndexOf('@', num);
			if (num2 == -1)
			{
				return false;
			}
			string text = entryString.Substring(0, num);
			string text2 = entryString.Substring(num + 1, num2 - num - 1);
			int num3 = text.IndexOf(':');
			if (num3 == -1)
			{
				return false;
			}
			parts.KeyType = text.Substring(0, num3);
			parts.KeyValue = Uri.UnescapeDataString(text.Substring(num3 + 1));
			num3 = text2.IndexOf(':');
			if (num3 == -1)
			{
				return false;
			}
			parts.DestinationType = text2.Substring(0, num3);
			parts.DestinationValue = Uri.UnescapeDataString(text2.Substring(num3 + 1));
			long timestamp;
			if (!long.TryParse(entryString.Substring(num2 + 1), out timestamp))
			{
				return false;
			}
			parts.Timestamp = timestamp;
			return true;
		}

		private const char DestinationSeparator = '=';

		private const char TimestampSeparator = '@';

		private const char TypeSeparator = ':';

		private const char PropertiesSeparator = '+';

		private const string ArchiveSmtpTypeString = "ArchiveSmtp";

		private const string DatabaseGuidTypeString = "DatabaseGuid";

		private const string ErrorTypeString = "Error";

		private const string MailboxGuidTypeString = "MailboxGuid";

		private const string ServerTypeString = "Server";

		private const string SmtpTypeString = "Smtp";

		private const string ExternalDirectoryObjectIdTypeString = "Oid";

		private const string LiveIdMemberNameTypeString = "LiveIdMemberName";

		private const string UnknownTypeString = "Unknown";

		private struct RoutingEntryParts
		{
			public string KeyType { get; set; }

			public string KeyValue { get; set; }

			public string DestinationType { get; set; }

			public string DestinationValue { get; set; }

			public long Timestamp { get; set; }
		}
	}
}

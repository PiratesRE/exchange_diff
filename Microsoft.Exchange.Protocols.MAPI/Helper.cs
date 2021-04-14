using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class Helper
	{
		public static ObjectType GetPropTagObjectType(MapiObjectType mapiType)
		{
			ObjectType result = ObjectType.Invalid;
			switch (mapiType)
			{
			case MapiObjectType.Attachment:
				result = ObjectType.Attachment;
				break;
			case MapiObjectType.Event:
				result = ObjectType.Event;
				break;
			case MapiObjectType.Folder:
				result = ObjectType.Folder;
				break;
			case MapiObjectType.Logon:
				result = ObjectType.Mailbox;
				break;
			case MapiObjectType.Message:
			case MapiObjectType.MessageView:
				result = ObjectType.Message;
				break;
			case MapiObjectType.EmbeddedMessage:
				result = ObjectType.EmbeddedMessage;
				break;
			case MapiObjectType.Person:
				result = ObjectType.Recipient;
				break;
			case MapiObjectType.FolderView:
				result = ObjectType.FolderView;
				break;
			case MapiObjectType.AttachmentView:
				result = ObjectType.AttachmentView;
				break;
			case MapiObjectType.FastTransferStream:
				result = ObjectType.FastTransferStream;
				break;
			}
			return result;
		}

		public static object GetValue(MapiLogon logon, byte[] buff, ref int pos, int posMax, PropertyType propType)
		{
			if (propType <= PropertyType.Binary)
			{
				if (propType <= PropertyType.SysTime)
				{
					switch (propType)
					{
					case PropertyType.Null:
						return null;
					case PropertyType.Int16:
						return (short)ParseSerialize.GetWord(buff, ref pos, posMax);
					case PropertyType.Int32:
						return (int)ParseSerialize.GetDword(buff, ref pos, posMax);
					case PropertyType.Real32:
						return ParseSerialize.GetFloat(buff, ref pos, posMax);
					case PropertyType.Real64:
					case PropertyType.AppTime:
						return ParseSerialize.GetDouble(buff, ref pos, posMax);
					case PropertyType.Currency:
					case PropertyType.Int64:
						return (long)ParseSerialize.GetQword(buff, ref pos, posMax);
					case (PropertyType)8:
					case (PropertyType)9:
					case (PropertyType)12:
					case (PropertyType)14:
					case (PropertyType)15:
					case (PropertyType)16:
					case (PropertyType)17:
					case (PropertyType)18:
					case (PropertyType)19:
						goto IL_22F;
					case PropertyType.Error:
						return (ErrorCodeValue)ParseSerialize.GetDword(buff, ref pos, posMax);
					case PropertyType.Boolean:
						return ParseSerialize.GetByte(buff, ref pos, posMax) != 0;
					case PropertyType.Object:
						break;
					default:
						switch (propType)
						{
						case PropertyType.String8:
							return ParseSerialize.GetStringFromASCII(buff, ref pos, posMax);
						case PropertyType.Unicode:
							return ParseSerialize.GetStringFromUnicode(buff, ref pos, posMax);
						default:
							if (propType != PropertyType.SysTime)
							{
								goto IL_22F;
							}
							return ParseSerialize.GetSysTime(buff, ref pos, posMax);
						}
						break;
					}
				}
				else
				{
					if (propType == PropertyType.Guid)
					{
						return ParseSerialize.GetGuid(buff, ref pos, posMax);
					}
					switch (propType)
					{
					case PropertyType.SvrEid:
					case PropertyType.SRestriction:
					case PropertyType.Actions:
						break;
					case (PropertyType)252:
						goto IL_22F;
					default:
						if (propType != PropertyType.Binary)
						{
							goto IL_22F;
						}
						break;
					}
				}
				return ParseSerialize.GetByteArray(buff, ref pos, posMax);
			}
			if (propType <= PropertyType.MVUnicode)
			{
				switch (propType)
				{
				case PropertyType.MVInt16:
					return ParseSerialize.GetMVInt16(buff, ref pos, posMax);
				case PropertyType.MVInt32:
					return ParseSerialize.GetMVInt32(buff, ref pos, posMax);
				case PropertyType.MVReal32:
					return ParseSerialize.GetMVReal32(buff, ref pos, posMax);
				case PropertyType.MVReal64:
				case PropertyType.MVAppTime:
					return ParseSerialize.GetMVR8(buff, ref pos, posMax);
				case PropertyType.MVCurrency:
					break;
				default:
					if (propType != PropertyType.MVInt64)
					{
						switch (propType)
						{
						case PropertyType.MVString8:
							return ParseSerialize.GetMVString8(buff, ref pos, posMax);
						case PropertyType.MVUnicode:
							return ParseSerialize.GetMVUnicode(buff, ref pos, posMax);
						default:
							goto IL_22F;
						}
					}
					break;
				}
				return ParseSerialize.GetMVInt64(buff, ref pos, posMax);
			}
			if (propType == PropertyType.MVSysTime)
			{
				return ParseSerialize.GetMVSysTime(buff, ref pos, posMax);
			}
			if (propType == PropertyType.MVGuid)
			{
				return ParseSerialize.GetMVGuid(buff, ref pos, posMax);
			}
			if (propType == PropertyType.MVBinary)
			{
				return ParseSerialize.GetMVBinary(buff, ref pos, posMax);
			}
			IL_22F:
			throw new ExExceptionInvalidParameter((LID)61240U, "Unsupported propType : " + PropertyTypeHelper.PropertyTypeToString(propType));
		}

		public static byte[] FormatSvrEid(bool export, ExchangeId fid, ExchangeId mid)
		{
			return EntryIdHelpers.FormatServerEntryId(export, fid, mid, 0);
		}

		internal static bool ParseSvrEid(MapiLogon logon, byte[] svrEidValue, bool export, out ExchangeId fid, out ExchangeId mid)
		{
			int num;
			return EntryIdHelpers.ParseServerEntryId(logon.StoreMailbox.CurrentOperationContext, logon.StoreMailbox.ReplidGuidMap, svrEidValue, export, out fid, out mid, out num);
		}

		internal static bool ParseMessageEntryId(MapiContext context, MapiLogon logon, byte[] messageEntryId, out ExchangeId fid, out ExchangeId mid)
		{
			fid = ExchangeId.Null;
			mid = ExchangeId.Null;
			int num = 0;
			if (messageEntryId == null || messageEntryId.Length < 22)
			{
				if (ExTraceGlobals.GeneralTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.GeneralTracer.TraceError(45240L, "Mesage entryid is null or invalid length");
				}
				return false;
			}
			num += 4;
			Guid guid = ParseSerialize.ParseGuid(messageEntryId, num);
			if (guid.CompareTo(logon.MailboxInstanceGuid) != 0)
			{
				if (ExTraceGlobals.GeneralTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					StringBuilder stringBuilder = new StringBuilder(200);
					stringBuilder.Append("MapiEntryGuid \"");
					stringBuilder.Append(guid.ToString());
					stringBuilder.Append("\" is not for this mailbox which has mapiEntryId \"");
					stringBuilder.Append(logon.MailboxInstanceGuid.ToString());
					ExTraceGlobals.GeneralTracer.TraceError(61624L, stringBuilder.ToString());
				}
				return false;
			}
			num += 16;
			short num2 = ParseSerialize.ParseInt16(messageEntryId, num);
			if (num2 < 6)
			{
				if (ExTraceGlobals.GeneralTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.GeneralTracer.TraceError(61624L, "This is not a message entryid");
				}
				return false;
			}
			num += 2;
			bool flag = (num2 & 1) != 0;
			if (flag)
			{
				if (messageEntryId.Length != 70)
				{
					if (ExTraceGlobals.GeneralTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.GeneralTracer.TraceError(37048L, "Invalid length for long term message entryid, length=" + messageEntryId.Length);
					}
					return false;
				}
				fid = ExchangeId.CreateFrom24ByteArray(context, logon.StoreMailbox.ReplidGuidMap, messageEntryId, num);
				num += 24;
				mid = ExchangeId.CreateFrom24ByteArray(context, logon.StoreMailbox.ReplidGuidMap, messageEntryId, num);
			}
			else
			{
				if (messageEntryId.Length != 38)
				{
					if (ExTraceGlobals.GeneralTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.GeneralTracer.TraceError(53432L, "Invalid length for short term message entryid, length=" + messageEntryId.Length);
					}
					return false;
				}
				fid = ExchangeId.CreateFrom8ByteArray(context, logon.StoreMailbox.ReplidGuidMap, messageEntryId, num);
				num += 8;
				mid = ExchangeId.CreateFrom8ByteArray(context, logon.StoreMailbox.ReplidGuidMap, messageEntryId, num);
			}
			return true;
		}

		internal static byte[] ConvertServerEIdFromExportToOursFormat(MapiLogon logon, byte[] serverIdExportFormat)
		{
			ExchangeId fid;
			ExchangeId mid;
			if (!Helper.ParseSvrEid(logon, serverIdExportFormat, true, out fid, out mid))
			{
				return serverIdExportFormat;
			}
			return Helper.FormatSvrEid(false, fid, mid);
		}

		internal static byte[] ConvertServerEIdFromOursToExportFormat(MapiLogon logon, byte[] serverIdOursFormat)
		{
			ExchangeId fid;
			ExchangeId mid;
			if (Helper.ParseSvrEid(logon, serverIdOursFormat, false, out fid, out mid))
			{
				return Helper.FormatSvrEid(true, fid, mid);
			}
			if (serverIdOursFormat != null && serverIdOursFormat.Length >= 1 && (serverIdOursFormat[0] >= 4 || (serverIdOursFormat[0] == 1 && serverIdOursFormat.Length != 21)))
			{
				throw new StoreException((LID)57928U, ErrorCodeValue.InvalidEntryId, "InvalidEntryId");
			}
			return serverIdOursFormat;
		}
	}
}

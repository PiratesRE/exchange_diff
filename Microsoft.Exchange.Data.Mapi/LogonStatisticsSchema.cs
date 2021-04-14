using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	internal sealed class LogonStatisticsSchema : MapiObjectSchema
	{
		public static readonly MapiPropertyDefinition AdapterSpeed = new MapiPropertyDefinition("AdapterSpeed", typeof(uint?), PropTag.ClientAdapterSpeed, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition ClientIPAddress = new MapiPropertyDefinition("ClientIPAddress", typeof(string), PropTag.ClientIP, MapiPropertyDefinitionFlags.ReadOnly, null, new MapiPropValueExtractorDelegate(CustomizedMapiPropValueConvertor.ExtractIpV4StringFromIpV6Bytes), null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition ClientMode = new MapiPropertyDefinition("ClientMode", typeof(ClientMode), PropTag.QuotaReceiveThreshold, MapiPropertyDefinitionFlags.ReadOnly, Microsoft.Exchange.Data.Mapi.ClientMode.Unknown, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition ClientName = new MapiPropertyDefinition("ClientName", typeof(string), PropTag.ClientMachineName, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition ClientVersion = new MapiPropertyDefinition("ClientVersion", typeof(string), PropTag.ClientVersion, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition CodePage = new MapiPropertyDefinition("CodePage", typeof(uint?), PropTag.CodePageId, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition CurrentOpenAttachments = new MapiPropertyDefinition("CurrentOpenAttachments", typeof(uint?), PropTag.OpenAttachCount, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition CurrentOpenFolders = new MapiPropertyDefinition("CurrentOpenFolders", typeof(uint?), PropTag.OpenFolderCount, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition CurrentOpenMessages = new MapiPropertyDefinition("CurrentOpenMessages", typeof(uint?), PropTag.OpenMessageCount, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition FolderOperationCount = new MapiPropertyDefinition("FolderOperationCount", typeof(uint?), PropTag.FolderOpRate, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition FullMailboxDirectoryName = new MapiPropertyDefinition("FullMailboxDirectoryName", typeof(string), PropTag.MailboxDN, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition FullUserDirectoryName = new MapiPropertyDefinition("FullUserDirectoryName", typeof(string), PropTag.UserDN, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition HostAddress = new MapiPropertyDefinition("HostAddress", typeof(string), PropTag.HostAddress, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition LastAccessTime = new MapiPropertyDefinition("LastAccessTime", typeof(DateTime?), PropTag.LastOpTime, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition Latency = new MapiPropertyDefinition("Latency", typeof(uint?), PropTag.ClientLatency, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition LocaleID = new MapiPropertyDefinition("LocaleID", typeof(uint?), PropTag.LocaleId, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition LogonTime = new MapiPropertyDefinition("LogonTime", typeof(DateTime?), PropTag.LogonTime, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition MACAddress = new MapiPropertyDefinition("MACAddress", typeof(string), PropTag.ClientMacAddress, MapiPropertyDefinitionFlags.ReadOnly, null, new MapiPropValueExtractorDelegate(CustomizedMapiPropValueConvertor.ExtractMacAddressStringFromBytes), null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition MessagingOperationCount = new MapiPropertyDefinition("MessagingOperationCount", typeof(uint?), PropTag.MessagingOpRate, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition OtherOperationCount = new MapiPropertyDefinition("OtherOperationCount", typeof(uint?), PropTag.OtherOpRate, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition ProgressOperationCount = new MapiPropertyDefinition("ProgressOperationCount", typeof(uint?), PropTag.ProgressOpRate, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition RPCCallsSucceeded = new MapiPropertyDefinition("RPCCallsSucceeded", typeof(uint?), PropTag.ClientRpcsSucceeded, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition StreamOperationCount = new MapiPropertyDefinition("StreamOperationCount", typeof(uint?), PropTag.StreamOpRate, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition TableOperationCount = new MapiPropertyDefinition("TableOperationCount", typeof(uint?), PropTag.TableOpRate, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition TotalOperationCount = new MapiPropertyDefinition("TotalOperationCount", typeof(uint?), PropTag.TotalOpRate, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition TransferOperationCount = new MapiPropertyDefinition("TransferOperationCount", typeof(uint?), PropTag.TransferOpRate, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition UserName = new MapiPropertyDefinition("UserName", typeof(string), PropTag.UserDisplayName, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition Windows2000Account = new MapiPropertyDefinition("Windows2000Account", typeof(string), PropTag.NTUserName, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition ApplicationId = new MapiPropertyDefinition("ApplicationId", typeof(string), PropTag.ApplicationId, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition SessionId = new MapiPropertyDefinition("SessionId", typeof(long?), PropTag.SessionId, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}

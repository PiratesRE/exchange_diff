using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	internal sealed class MailboxResourceMonitorSchema : MapiObjectSchema
	{
		public static readonly MapiPropertyDefinition DigestCategory = new MapiPropertyDefinition("DigestCategory", typeof(string), PropTag.DigestCategory, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 256)
		});

		public static readonly MapiPropertyDefinition SampleId = new MapiPropertyDefinition("SampleId", typeof(uint?), PropTag.SampleId, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition SampleTime = new MapiPropertyDefinition("SampleTime", typeof(DateTime?), PropTag.SampleTime, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition DisplayName = MapiPropertyDefinitions.DisplayName;

		public static readonly MapiPropertyDefinition MailboxGuid = MapiPropertyDefinitions.MailboxGuid;

		public static readonly MapiPropertyDefinition TimeInServer = new MapiPropertyDefinition("TimeInServer", typeof(uint?), PropTag.TimeInServer, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition TimeInCPU = new MapiPropertyDefinition("TimeInCPU", typeof(uint?), PropTag.TimeInCPU, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition ROPCount = new MapiPropertyDefinition("ROPCount", typeof(uint?), PropTag.ROPCount, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition PageRead = new MapiPropertyDefinition("PageRead", typeof(uint?), PropTag.PageRead, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition PagePreread = new MapiPropertyDefinition("PagePreread", typeof(uint?), PropTag.PagePreread, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition LogRecordCount = new MapiPropertyDefinition("LogRecordCount", typeof(uint?), PropTag.LogRecordCount, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition LogRecordBytes = new MapiPropertyDefinition("LogRecordBytes", typeof(uint?), PropTag.LogRecordBytes, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition LdapReads = new MapiPropertyDefinition("LdapReads", typeof(uint?), PropTag.LdapReads, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition LdapSearches = new MapiPropertyDefinition("LdapSearches", typeof(uint?), PropTag.LdapSearches, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition IsQuarantined = new MapiPropertyDefinition("IsQuarantined", typeof(bool?), PropTag.MailboxQuarantined, MapiPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}

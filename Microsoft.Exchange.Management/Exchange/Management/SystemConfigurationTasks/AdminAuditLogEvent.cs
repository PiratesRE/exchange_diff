using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class AdminAuditLogEvent : ConfigurableObject
	{
		public AdminAuditLogEvent() : base(new SimpleProviderPropertyBag())
		{
		}

		public AdminAuditLogEvent(AdminAuditLogEventId identity, string eventLog)
		{
			AdminAuditLogEvent.<>c__DisplayClass4 CS$<>8__locals1 = new AdminAuditLogEvent.<>c__DisplayClass4();
			CS$<>8__locals1.eventLog = eventLog;
			this..ctor();
			CS$<>8__locals1.<>4__this = this;
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (CS$<>8__locals1.eventLog == null)
			{
				throw new ArgumentNullException("eventLog");
			}
			this.propertyBag[SimpleProviderObjectSchema.Identity] = identity;
			Dictionary<string, AdminAuditLogModifiedProperty> modifiedProperties = new Dictionary<string, AdminAuditLogModifiedProperty>(StringComparer.OrdinalIgnoreCase);
			List<PropertyParseSchema> list = new List<PropertyParseSchema>(AdminAuditLogEvent.AdminAuditLogRecordParseSchema);
			list.Add(new PropertyParseSchema("Parameter", AdminAuditLogSchema.CmdletParameters, (string line) => CS$<>8__locals1.<>4__this.ParseCmdletParameter(line, CS$<>8__locals1.eventLog)));
			list.Add(new PropertyParseSchema("Property Modified", AdminAuditLogSchema.ModifiedProperties, (string line) => CS$<>8__locals1.<>4__this.ParseModifiedProperty(line, modifiedProperties, true, CS$<>8__locals1.eventLog)));
			list.Add(new PropertyParseSchema("Property Original", AdminAuditLogSchema.ModifiedProperties, (string line) => CS$<>8__locals1.<>4__this.ParseModifiedProperty(line, modifiedProperties, false, CS$<>8__locals1.eventLog)));
			AuditLogParseSerialize.ParseAdminAuditLogRecord(this, list, CS$<>8__locals1.eventLog);
			if (!string.IsNullOrEmpty(this.ObjectModified) && string.IsNullOrEmpty(this.ModifiedObjectResolvedName))
			{
				this.ModifiedObjectResolvedName = this.ObjectModified;
			}
		}

		public string ObjectModified
		{
			get
			{
				return this.propertyBag[AdminAuditLogSchema.ObjectModified] as string;
			}
			internal set
			{
				this.propertyBag[AdminAuditLogSchema.ObjectModified] = value;
			}
		}

		internal string SearchObject
		{
			get
			{
				string text = this.propertyBag[AdminAuditLogSchema.CmdletName] as string;
				if (AdminAuditLogHelper.IsDiscoverySearchModifierCmdlet(text))
				{
					return this.ObjectModified;
				}
				if (string.Compare(text, "new-mailboxsearch", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					MultiValuedProperty<AdminAuditLogCmdletParameter> multiValuedProperty = this.propertyBag[AdminAuditLogSchema.CmdletParameters] as MultiValuedProperty<AdminAuditLogCmdletParameter>;
					if (multiValuedProperty != null)
					{
						foreach (AdminAuditLogCmdletParameter adminAuditLogCmdletParameter in multiValuedProperty)
						{
							if (string.Compare(adminAuditLogCmdletParameter.Name, "name", StringComparison.InvariantCultureIgnoreCase) == 0)
							{
								return adminAuditLogCmdletParameter.Value;
							}
						}
					}
				}
				return string.Empty;
			}
			private set
			{
				this.propertyBag[SimpleProviderObjectSchema.Identity] = value;
			}
		}

		internal string ModifiedObjectResolvedName
		{
			get
			{
				return this.propertyBag[AdminAuditLogSchema.ModifiedObjectResolvedName] as string;
			}
			set
			{
				this.propertyBag[AdminAuditLogSchema.ModifiedObjectResolvedName] = value;
			}
		}

		public string CmdletName
		{
			get
			{
				return this.propertyBag[AdminAuditLogSchema.CmdletName] as string;
			}
			internal set
			{
				this.propertyBag[AdminAuditLogSchema.CmdletName] = value;
			}
		}

		public MultiValuedProperty<AdminAuditLogCmdletParameter> CmdletParameters
		{
			get
			{
				return this.propertyBag[AdminAuditLogSchema.CmdletParameters] as MultiValuedProperty<AdminAuditLogCmdletParameter>;
			}
			internal set
			{
				this.propertyBag[AdminAuditLogSchema.CmdletParameters] = value;
			}
		}

		public MultiValuedProperty<AdminAuditLogModifiedProperty> ModifiedProperties
		{
			get
			{
				return this.propertyBag[AdminAuditLogSchema.ModifiedProperties] as MultiValuedProperty<AdminAuditLogModifiedProperty>;
			}
			internal set
			{
				this.propertyBag[AdminAuditLogSchema.ModifiedProperties] = value;
			}
		}

		public string Caller
		{
			get
			{
				return this.propertyBag[AdminAuditLogSchema.Caller] as string;
			}
			internal set
			{
				this.propertyBag[AdminAuditLogSchema.Caller] = value;
			}
		}

		public bool? ExternalAccess
		{
			get
			{
				return this.propertyBag[AdminAuditLogSchema.ExternalAccess] as bool?;
			}
			internal set
			{
				this.propertyBag[AdminAuditLogSchema.ExternalAccess] = value;
			}
		}

		public bool? Succeeded
		{
			get
			{
				return this.propertyBag[AdminAuditLogSchema.Succeeded] as bool?;
			}
			internal set
			{
				this.propertyBag[AdminAuditLogSchema.Succeeded] = value;
			}
		}

		public string Error
		{
			get
			{
				return this.propertyBag[AdminAuditLogSchema.Error] as string;
			}
			internal set
			{
				this.propertyBag[AdminAuditLogSchema.Error] = value;
			}
		}

		public DateTime? RunDate
		{
			get
			{
				return this.propertyBag[AdminAuditLogSchema.RunDate] as DateTime?;
			}
			internal set
			{
				this.propertyBag[AdminAuditLogSchema.RunDate] = value;
			}
		}

		public string OriginatingServer
		{
			get
			{
				return this.propertyBag[AdminAuditLogSchema.OriginatingServer] as string;
			}
			internal set
			{
				this.propertyBag[AdminAuditLogSchema.OriginatingServer] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return AdminAuditLogEvent.schema;
			}
		}

		private static PropertyParseSchema[] AdminAuditLogRecordParseSchema
		{
			get
			{
				if (AdminAuditLogEvent.adminAuditLogRecordParseSchema == null)
				{
					PropertyParseSchema[] array = new PropertyParseSchema[9];
					array[0] = new PropertyParseSchema("Cmdlet Name", AdminAuditLogSchema.CmdletName, null);
					array[1] = new PropertyParseSchema("Object Modified", AdminAuditLogSchema.ObjectModified, null);
					array[2] = new PropertyParseSchema("Modified Object Resolved Name", AdminAuditLogSchema.ModifiedObjectResolvedName, null);
					array[3] = new PropertyParseSchema("Caller", AdminAuditLogSchema.Caller, null);
					array[4] = new PropertyParseSchema("ExternalAccess", AdminAuditLogSchema.ExternalAccess, (string line) => AuditLogParseSerialize.ParseBoolean(line));
					array[5] = new PropertyParseSchema("Succeeded", AdminAuditLogSchema.Succeeded, (string line) => AuditLogParseSerialize.ParseBoolean(line));
					array[6] = new PropertyParseSchema("Run Date", AdminAuditLogSchema.RunDate, (string line) => AdminAuditLogEvent.FixAndParseRunDate(line));
					array[7] = new PropertyParseSchema("Error", AdminAuditLogSchema.Error, null);
					array[8] = new PropertyParseSchema("OriginatingServer", AdminAuditLogSchema.OriginatingServer, null);
					AdminAuditLogEvent.adminAuditLogRecordParseSchema = array;
				}
				return AdminAuditLogEvent.adminAuditLogRecordParseSchema;
			}
		}

		private object ParseCmdletParameter(string line, string eventLog)
		{
			try
			{
				AdminAuditLogCmdletParameter item = AdminAuditLogCmdletParameter.Parse(line);
				this.CmdletParameters.Add(item);
			}
			catch (ArgumentException)
			{
				TaskLogger.LogWarning(Strings.WarningInvalidParameterOrModifiedPropertyInAdminAuditLog(eventLog));
			}
			return null;
		}

		private object ParseModifiedProperty(string line, Dictionary<string, AdminAuditLogModifiedProperty> modifiedProperties, bool newValue, string eventLog)
		{
			try
			{
				AdminAuditLogModifiedProperty adminAuditLogModifiedProperty = AdminAuditLogModifiedProperty.Parse(line, newValue);
				if (modifiedProperties.ContainsKey(adminAuditLogModifiedProperty.Name))
				{
					if (newValue)
					{
						if (modifiedProperties[adminAuditLogModifiedProperty.Name].NewValue != null)
						{
							TaskLogger.LogWarning(Strings.WarningDuplicatedPropertyModifiedEntry(adminAuditLogModifiedProperty.Name));
						}
						modifiedProperties[adminAuditLogModifiedProperty.Name].NewValue = adminAuditLogModifiedProperty.NewValue;
					}
					else
					{
						if (modifiedProperties[adminAuditLogModifiedProperty.Name].OldValue != null)
						{
							TaskLogger.LogWarning(Strings.WarningDuplicatedPropertyOriginalEntry(adminAuditLogModifiedProperty.Name));
						}
						modifiedProperties[adminAuditLogModifiedProperty.Name].OldValue = adminAuditLogModifiedProperty.OldValue;
					}
				}
				else
				{
					this.ModifiedProperties.Add(adminAuditLogModifiedProperty);
					modifiedProperties[adminAuditLogModifiedProperty.Name] = adminAuditLogModifiedProperty;
				}
			}
			catch (ArgumentException)
			{
				TaskLogger.LogWarning(Strings.WarningInvalidParameterOrModifiedPropertyInAdminAuditLog(eventLog));
			}
			return null;
		}

		private static object FixAndParseRunDate(string line)
		{
			if (line.EndsWith("UTC", StringComparison.OrdinalIgnoreCase))
			{
				line = line.Substring(0, line.Length - 3);
			}
			ExDateTime exDateTime;
			if (ExDateTime.TryParse(line, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out exDateTime))
			{
				return exDateTime.UniversalTime.ToLocalTime();
			}
			return null;
		}

		private static readonly ObjectSchema schema = ObjectSchema.GetInstance<AdminAuditLogSchema>();

		private static PropertyParseSchema[] adminAuditLogRecordParseSchema = null;
	}
}

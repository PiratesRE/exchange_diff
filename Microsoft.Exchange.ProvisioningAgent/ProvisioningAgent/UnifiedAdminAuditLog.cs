using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Compliance.Audit.Schema;
using Microsoft.Office.Compliance.Audit.Schema.Admin;

namespace Microsoft.Exchange.ProvisioningAgent
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UnifiedAdminAuditLog : IAuditLog, IDisposable
	{
		public void Dispose()
		{
			this.logger.Dispose();
		}

		public UnifiedAdminAuditLog(string organizationId, string organizationName)
		{
			this.organizationId = organizationId;
			this.organizationName = organizationName;
			this.logger = new UnifiedAuditLogger();
		}

		public UnifiedAuditLogger LoggerForTest
		{
			get
			{
				return this.logger;
			}
		}

		public DateTime EstimatedLogStartTime
		{
			get
			{
				return DateTime.MinValue;
			}
		}

		public DateTime EstimatedLogEndTime
		{
			get
			{
				return DateTime.MaxValue;
			}
		}

		public bool IsAsynchronous
		{
			get
			{
				return true;
			}
		}

		public IAuditQueryContext<TFilter> CreateAuditQueryContext<TFilter>()
		{
			throw new InvalidOperationException();
		}

		public int WriteAuditRecord(IAuditLogRecord auditRecord)
		{
			AuditRecord record = this.CreateAuditRecordObject(auditRecord);
			return this.logger.WriteAuditRecord(record);
		}

		public AuditRecord CreateAuditRecordObject(IAuditLogRecord auditRecord)
		{
			AuditRecord auditRecord2;
			Action<string, string> action = this.CreateAuditRecordObject(auditRecord, out auditRecord2);
			auditRecord2.Initialize();
			auditRecord2.OrganizationId = this.organizationId;
			auditRecord2.OrganizationName = (string.IsNullOrEmpty(this.organizationName) ? "First Org" : this.organizationName);
			auditRecord2.ObjectId = auditRecord.ObjectId;
			auditRecord2.Operation = auditRecord.Operation;
			auditRecord2.UserId = auditRecord.UserId;
			auditRecord2.CreationTime = auditRecord.CreationTime;
			foreach (KeyValuePair<string, string> keyValuePair in auditRecord.GetDetails())
			{
				string key = keyValuePair.Key;
				string value = keyValuePair.Value;
				action(key, value);
			}
			return auditRecord2;
		}

		private Action<string, string> CreateAuditRecordObject(IAuditLogRecord auditRecord, out AuditRecord record)
		{
			if (auditRecord.RecordType != AuditLogRecordType.AdminAudit)
			{
				throw new ArgumentException(string.Format("Invalid audit record type {0}.", auditRecord.RecordType), "auditRecord");
			}
			return this.CreateAdminAuditRecordObject(out record);
		}

		private Action<string, string> CreateAdminAuditRecordObject(out AuditRecord record)
		{
			ExchangeAdminAuditRecord adminAuditRecord = new ExchangeAdminAuditRecord();
			record = adminAuditRecord;
			Dictionary<string, ModifiedProperty> modifiedProperties = new Dictionary<string, ModifiedProperty>();
			return delegate(string field, string val)
			{
				Action<ExchangeAdminAuditRecord, string, Dictionary<string, ModifiedProperty>> action;
				if (UnifiedAdminAuditLog.AdminRecordSetters.TryGetValue(field, out action))
				{
					action(adminAuditRecord, val, modifiedProperties);
				}
			};
		}

		private static NameValuePair ParseValuePair(string multiValue)
		{
			int num = multiValue.IndexOf('=');
			if (num <= 0)
			{
				return new NameValuePair
				{
					Name = string.Empty,
					Value = multiValue
				};
			}
			string name = multiValue.Substring(0, num).TrimEnd(new char[0]);
			string value = multiValue.Substring(num + 1).TrimStart(new char[0]);
			return new NameValuePair
			{
				Name = name,
				Value = value
			};
		}

		private static List<NameValuePair> AddParameter(List<NameValuePair> values, string paramValue)
		{
			if (values == null)
			{
				values = new List<NameValuePair>();
			}
			NameValuePair item = UnifiedAdminAuditLog.ParseValuePair(paramValue);
			values.Add(item);
			return values;
		}

		private static List<ModifiedProperty> AddModifiedProperty(List<ModifiedProperty> values, string propertyValuePair, bool isNewValue, Dictionary<string, ModifiedProperty> context)
		{
			if (values == null)
			{
				values = new List<ModifiedProperty>();
			}
			NameValuePair nameValuePair = UnifiedAdminAuditLog.ParseValuePair(propertyValuePair);
			string name = nameValuePair.Name;
			ModifiedProperty modifiedProperty;
			if (!context.TryGetValue(name, out modifiedProperty))
			{
				modifiedProperty = new ModifiedProperty
				{
					Name = name
				};
				context[name] = modifiedProperty;
				values.Add(modifiedProperty);
			}
			if (isNewValue)
			{
				modifiedProperty.NewValue = nameValuePair.Value;
			}
			else
			{
				modifiedProperty.OldValue = nameValuePair.Value;
			}
			return values;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static UnifiedAdminAuditLog()
		{
			Dictionary<string, Action<ExchangeAdminAuditRecord, string, Dictionary<string, ModifiedProperty>>> dictionary = new Dictionary<string, Action<ExchangeAdminAuditRecord, string, Dictionary<string, ModifiedProperty>>>();
			dictionary.Add("Error", delegate(ExchangeAdminAuditRecord record, string val, Dictionary<string, ModifiedProperty> context)
			{
				record.Error = val;
			});
			dictionary.Add("ExternalAccess", delegate(ExchangeAdminAuditRecord record, string val, Dictionary<string, ModifiedProperty> context)
			{
				record.ExternalAccess = bool.Parse(val);
			});
			dictionary.Add("Modified Object Resolved Name", delegate(ExchangeAdminAuditRecord record, string val, Dictionary<string, ModifiedProperty> context)
			{
				record.ModifiedObjectResolvedName = val;
			});
			dictionary.Add("OriginatingServer", delegate(ExchangeAdminAuditRecord record, string val, Dictionary<string, ModifiedProperty> context)
			{
				record.OriginatingServer = val;
			});
			dictionary.Add("Succeeded", delegate(ExchangeAdminAuditRecord record, string val, Dictionary<string, ModifiedProperty> context)
			{
				record.Succeeded = new bool?(bool.Parse(val));
			});
			dictionary.Add("Parameter", delegate(ExchangeAdminAuditRecord record, string val, Dictionary<string, ModifiedProperty> context)
			{
				record.Parameters = UnifiedAdminAuditLog.AddParameter(record.Parameters, val);
			});
			dictionary.Add("Property Modified", delegate(ExchangeAdminAuditRecord record, string val, Dictionary<string, ModifiedProperty> context)
			{
				record.ModifiedProperties = UnifiedAdminAuditLog.AddModifiedProperty(record.ModifiedProperties, val, true, context);
			});
			dictionary.Add("Property Original", delegate(ExchangeAdminAuditRecord record, string val, Dictionary<string, ModifiedProperty> context)
			{
				record.ModifiedProperties = UnifiedAdminAuditLog.AddModifiedProperty(record.ModifiedProperties, val, false, context);
			});
			UnifiedAdminAuditLog.AdminRecordSetters = dictionary;
		}

		private readonly string organizationId;

		private readonly string organizationName;

		private readonly UnifiedAuditLogger logger;

		private static readonly Dictionary<string, Action<ExchangeAdminAuditRecord, string, Dictionary<string, ModifiedProperty>>> AdminRecordSetters;
	}
}

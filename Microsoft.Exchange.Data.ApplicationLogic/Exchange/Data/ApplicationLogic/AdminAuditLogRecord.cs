using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Text;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal class AdminAuditLogRecord : IAuditLogRecord
	{
		public AdminAuditLogRecord(Trace trace)
		{
			if (trace == null)
			{
				throw new ArgumentNullException("trace");
			}
			this.Tracer = trace;
		}

		public string Cmdlet { get; set; }

		public IDictionary Parameters { get; set; }

		public IDictionary ModifiedPropertyValues { get; set; }

		public IDictionary OriginalPropertyValues { get; set; }

		public string ObjectModified { get; set; }

		public string ModifiedObjectResolvedName { get; set; }

		public bool Succeeded { get; set; }

		public bool ExternalAccess { get; set; }

		public string Error { get; set; }

		public string UserId { get; set; }

		public DateTime RunDate { get; set; }

		public bool Verbose { get; set; }

		public AuditLogRecordType RecordType
		{
			get
			{
				return AuditLogRecordType.AdminAudit;
			}
		}

		public DateTime CreationTime
		{
			get
			{
				return this.RunDate;
			}
		}

		public string Operation
		{
			get
			{
				return this.Cmdlet;
			}
		}

		public string ObjectId
		{
			get
			{
				return this.ObjectModified;
			}
		}

		public IEnumerable<KeyValuePair<string, string>> GetDetails()
		{
			yield return new KeyValuePair<string, string>("Cmdlet Name", this.Cmdlet);
			yield return new KeyValuePair<string, string>("Object Modified", this.ObjectModified);
			if (this.Verbose && this.ModifiedObjectResolvedName != null && !this.ModifiedObjectResolvedName.Equals(this.ObjectModified))
			{
				yield return new KeyValuePair<string, string>("Modified Object Resolved Name", this.ModifiedObjectResolvedName);
			}
			foreach (KeyValuePair<string, string> parameter in this.GetPropertyBagDetails("Parameter", this.Parameters, true))
			{
				yield return parameter;
			}
			if (this.Verbose)
			{
				foreach (KeyValuePair<string, string> parameter2 in this.GetPropertyBagDetails("Property Modified", this.ModifiedPropertyValues, true))
				{
					yield return parameter2;
				}
				foreach (KeyValuePair<string, string> parameter3 in this.GetPropertyBagDetails("Property Original", this.OriginalPropertyValues, true))
				{
					yield return parameter3;
				}
			}
			yield return new KeyValuePair<string, string>("Caller", this.UserId);
			yield return new KeyValuePair<string, string>("ExternalAccess", this.ExternalAccess.ToString());
			yield return new KeyValuePair<string, string>("Succeeded", this.Succeeded.ToString());
			if (this.Verbose)
			{
				yield return new KeyValuePair<string, string>("Error", this.Error);
			}
			yield return new KeyValuePair<string, string>("Run Date", string.Format("{0:s}", this.RunDate));
			yield return new KeyValuePair<string, string>("OriginatingServer", string.Format("{0} ({1})", AdminAuditLogRecord.MachineName, "15.00.1497.015"));
			yield break;
		}

		private static StringBuilder GetValueString(object value)
		{
			return AdminAuditLogRecord.GetValueString(value, ';');
		}

		public static StringBuilder GetValueString(object value, char delimiter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (value is byte[])
			{
				int length = Math.Min(((byte[])value).Length, AdminAuditLogRecord.MaximumByteArrayPropertyValueSize);
				stringBuilder.Append(Convert.ToBase64String((byte[])value, 0, length));
				stringBuilder.Append("...");
			}
			else if (value is ICollection)
			{
				foreach (object value2 in ((ICollection)value))
				{
					stringBuilder.Append(AdminAuditLogRecord.GetLoggableString(value2) + delimiter);
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
				}
			}
			else
			{
				stringBuilder.Append(AdminAuditLogRecord.GetLoggableString(value));
			}
			return stringBuilder;
		}

		private static string GetLoggableString(object value)
		{
			if (value is SecureString)
			{
				return Strings.SecureStringParameter;
			}
			if (value != null)
			{
				return value.ToString();
			}
			return string.Empty;
		}

		private IEnumerable<KeyValuePair<string, string>> GetPropertyBagDetails(string label, IDictionary propertyBag, bool truncate)
		{
			if (propertyBag != null)
			{
				foreach (object obj in propertyBag.Keys)
				{
					string propName = (string)obj;
					StringBuilder value = AdminAuditLogRecord.GetValueString(propertyBag[propName]);
					if (truncate && value.Length > AdminAuditLogRecord.MaximumPropertyValueSize)
					{
						int num = value.Length - AdminAuditLogRecord.MaximumPropertyValueSize;
						value.Remove(AdminAuditLogRecord.MaximumPropertyValueSize, num);
						value.Append("...");
						if (this.Tracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							this.Tracer.TraceDebug<StringBuilder, int>((long)this.GetHashCode(), "{0} Successfully truncated property bag value by {1} characters", value, num);
						}
					}
					yield return new KeyValuePair<string, string>(label, string.Format("{0} = {1}", propName, value.ToString()));
				}
			}
			yield break;
		}

		private static readonly string MachineName = Environment.MachineName;

		private static int MaximumPropertyValueSize = 1024;

		private static int MaximumByteArrayPropertyValueSize = 64;

		private readonly Trace Tracer;
	}
}

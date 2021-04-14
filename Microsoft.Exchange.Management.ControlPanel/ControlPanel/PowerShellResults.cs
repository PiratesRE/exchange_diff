using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class PowerShellResults
	{
		public PowerShellResults()
		{
			this.ErrorRecords = Array<ErrorRecord>.Empty;
			this.Warnings = Array<string>.Empty;
			this.Informations = Array<string>.Empty;
			this.Cmdlets = Array<string>.Empty;
		}

		public PowerShellResults(PowerShellResults results)
		{
			this.ErrorRecords = results.ErrorRecords;
			this.Warnings = results.Warnings;
			this.Informations = results.Informations;
			this.Cmdlets = results.Cmdlets;
			this.TranslationIdentity = results.TranslationIdentity;
		}

		public bool Succeeded
		{
			get
			{
				return this.ErrorRecords.IsNullOrEmpty();
			}
		}

		public bool HasWarnings
		{
			get
			{
				return !this.Warnings.IsNullOrEmpty();
			}
		}

		public bool SucceededWithoutWarnings
		{
			get
			{
				return this.Succeeded && !this.HasWarnings;
			}
		}

		public bool Failed
		{
			get
			{
				return !this.Succeeded;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public JsonDictionary<object> OutputOnError { get; set; }

		[DataMember]
		public ErrorRecord[] ErrorRecords { get; set; }

		[DataMember]
		public string[] Warnings { get; set; }

		[DataMember]
		public string[] Informations { get; set; }

		[DataMember]
		public string[] Cmdlets { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ProgressRecord ProgressRecord { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string ProgressId { get; set; }

		[DataMember]
		public bool IsDDIEnabled { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool ShouldRetry
		{
			get
			{
				if (this.Cmdlets.All((string cmdlet) => cmdlet.StartsWith("Get-", StringComparison.OrdinalIgnoreCase)))
				{
					return this.ErrorRecords.Any((ErrorRecord error) => typeof(TransientException).IsInstanceOfType(error.Exception));
				}
				return false;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public CmdExecuteInfo[] CmdletLogInfo { get; set; }

		public PowerShellResults MergeErrors(PowerShellResults results)
		{
			this.ErrorRecords = ((this.ErrorRecords == null) ? results.ErrorRecords : this.ErrorRecords.Concat(results.ErrorRecords ?? new ErrorRecord[0]).ToArray<ErrorRecord>());
			if (this.Warnings == null)
			{
				this.Warnings = results.Warnings;
			}
			else if (results.Warnings != null)
			{
				List<string> list = this.Warnings.ToList<string>();
				foreach (string item in results.Warnings)
				{
					if (!list.Contains(item))
					{
						list.Add(item);
					}
				}
				this.Warnings = list.ToArray();
			}
			this.Informations = ((this.Informations == null) ? results.Informations : this.Informations.Concat(results.Informations ?? new string[0]).ToArray<string>());
			this.Cmdlets = ((this.Cmdlets == null) ? results.Cmdlets : this.Cmdlets.Concat(results.Cmdlets ?? new string[0]).ToArray<string>());
			this.CmdletLogInfo = ((this.CmdletLogInfo == null) ? results.CmdletLogInfo : this.CmdletLogInfo.Concat(results.CmdletLogInfo ?? new CmdExecuteInfo[0]).ToArray<CmdExecuteInfo>());
			if (!string.IsNullOrEmpty(results.ProgressId))
			{
				this.ProgressId = results.ProgressId;
			}
			this.OutputOnError = ((this.OutputOnError == null) ? results.OutputOnError : this.OutputOnError.Merge(results.OutputOnError));
			if (results.ProgressRecord != null)
			{
				this.ProgressRecord = results.ProgressRecord;
			}
			this.TranslationIdentity = (this.TranslationIdentity ?? results.TranslationIdentity);
			return results;
		}

		public PowerShellResults<O> MergeErrors<O>(PowerShellResults<O> results)
		{
			return (PowerShellResults<O>)this.MergeErrors(results);
		}

		public Identity TranslationIdentity { get; set; }

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			if (this.Failed && this.ShouldTranslate)
			{
				foreach (ErrorRecord errorRecord in this.ErrorRecords)
				{
					errorRecord.Translate(this.TranslationIdentity);
				}
			}
		}

		protected virtual bool ShouldTranslate
		{
			get
			{
				return PowerShellMessageTranslator.ShouldTranslate;
			}
		}

		public virtual void UseAsRbacScopeInCurrentHttpContext()
		{
		}
	}
}

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MigrationProcessorValidationException : MigrationPermanentException
	{
		public MigrationProcessorValidationException(string processor, string jobname) : base(Strings.MigrationProcessorInvalidation(processor, jobname))
		{
			this.processor = processor;
			this.jobname = jobname;
		}

		public MigrationProcessorValidationException(string processor, string jobname, Exception innerException) : base(Strings.MigrationProcessorInvalidation(processor, jobname), innerException)
		{
			this.processor = processor;
			this.jobname = jobname;
		}

		protected MigrationProcessorValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.processor = (string)info.GetValue("processor", typeof(string));
			this.jobname = (string)info.GetValue("jobname", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("processor", this.processor);
			info.AddValue("jobname", this.jobname);
		}

		public string Processor
		{
			get
			{
				return this.processor;
			}
		}

		public string Jobname
		{
			get
			{
				return this.jobname;
			}
		}

		private readonly string processor;

		private readonly string jobname;
	}
}

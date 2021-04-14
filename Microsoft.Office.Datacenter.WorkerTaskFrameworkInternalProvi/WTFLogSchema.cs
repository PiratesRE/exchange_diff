using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public class WTFLogSchema : LogSchema
	{
		public WTFLogSchema(WTFLogConfiguration logConfiguration) : base("Microsoft Exchange Server Active Monitoring", logConfiguration.SoftwareVersion, logConfiguration.LogType, WTFLogSchema.Headers)
		{
		}

		private static readonly string[] Headers = new string[]
		{
			"Timestamp",
			"Instance",
			"Type",
			"Definition",
			"CreatedBy",
			"Result",
			"Component",
			"Process:Thread",
			"LogLevel",
			"Method",
			"Source",
			"Message"
		};

		public enum Head
		{
			Timestamp,
			Instance,
			Type,
			Definition,
			CreatedBy,
			Result,
			Component,
			ProcessAndThreadId,
			LogLevel,
			Method,
			Source,
			Message
		}
	}
}

using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.OAB;

namespace Microsoft.Exchange.OAB
{
	internal sealed class OABGeneratorAssistantRunNowParameters
	{
		public static bool TryParse(string input, out OABGeneratorAssistantRunNowParameters output)
		{
			string[] array = input.Split(new char[]
			{
				','
			});
			if (array.Length != 3)
			{
				OABGeneratorAssistantRunNowParameters.Tracer.TraceError<string>(0L, "OABGeneratorAssistantRunNowParameters.FromParametersString: ignoring on-demand request due malformed string parameter: {0}.", input);
				output = null;
				return false;
			}
			PartitionId partitionId;
			try
			{
				partitionId = new PartitionId(array[0]);
			}
			catch (ArgumentException arg)
			{
				OABGeneratorAssistantRunNowParameters.Tracer.TraceError<string, ArgumentException>(0L, "OABGeneratorAssistantRunNowParameters.FromParametersString: ignoring on-demand request due malformed PartitionId in string parameter: {0}. Exception: {1}", input, arg);
				output = null;
				return false;
			}
			Exception ex = null;
			Guid objectGuid = Guid.Empty;
			try
			{
				objectGuid = new Guid(array[1]);
			}
			catch (FormatException ex2)
			{
				ex = ex2;
			}
			catch (OverflowException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				OABGeneratorAssistantRunNowParameters.Tracer.TraceError<string, Exception>(0L, "OABGeneratorAssistantRunNowParameters.FromParametersString: ignoring on-demand request due malformed GUID in string parameter: {0}. Exception: {1}", input, ex);
				output = null;
				return false;
			}
			output = new OABGeneratorAssistantRunNowParameters
			{
				PartitionId = partitionId,
				ObjectGuid = objectGuid,
				Description = array[2]
			};
			return true;
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.PartitionId.ForestFQDN,
				",",
				this.ObjectGuid.ToString(),
				",",
				this.Description
			});
		}

		public PartitionId PartitionId { get; set; }

		public Guid ObjectGuid { get; set; }

		public string Description { get; set; }

		private static readonly Trace Tracer = ExTraceGlobals.RunNowTracer;
	}
}

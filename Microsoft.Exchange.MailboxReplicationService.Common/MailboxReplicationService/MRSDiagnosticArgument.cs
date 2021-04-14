using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MRSDiagnosticArgument : DiagnosableArgument
	{
		public MRSDiagnosticArgument(string argument)
		{
			base.Initialize(argument);
		}

		protected override void InitializeSchema(Dictionary<string, Type> schema)
		{
			schema["job"] = typeof(string);
			schema["reservations"] = typeof(Guid);
			schema["resources"] = typeof(string);
			schema["healthstats"] = typeof(string);
			schema["unhealthy"] = typeof(string);
			schema["workloads"] = typeof(bool);
			schema["queues"] = typeof(string);
			schema["pickupresults"] = typeof(string);
			schema["maxsize"] = typeof(int);
			schema["requesttype"] = typeof(MRSRequestType);
			schema["binaryversions"] = typeof(string);
			schema["showtimeslots"] = typeof(bool);
		}

		public const string JobArgument = "job";

		public const string ReservationsArgument = "reservations";

		public const string ResourcesArgument = "resources";

		public const string HealthStatsArgument = "healthstats";

		public const string UnhealthyArgument = "unhealthy";

		public const string WorkloadsArgument = "workloads";

		public const string QueuesArgument = "queues";

		public const string PickupResultsArgument = "pickupresults";

		public const string MaxSizeArgument = "maxsize";

		public const string RequestTypeArgument = "requesttype";

		public const string BinaryVersionsArgument = "binaryversions";

		public const string ShowTimeSlots = "showtimeslots";
	}
}

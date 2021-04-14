using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationDiagnosticArgument : DiagnosableArgument
	{
		public MigrationDiagnosticArgument(string argument)
		{
			base.Initialize(argument);
		}

		protected override void InitializeSchema(Dictionary<string, Type> schema)
		{
			schema["user"] = typeof(string);
			schema["organization"] = typeof(string);
			schema["partition"] = typeof(string);
			schema["storage"] = typeof(bool);
			schema["status"] = typeof(string);
			schema["maxsize"] = typeof(int);
			schema["type"] = typeof(string);
			schema["batch"] = typeof(string);
			schema["reports"] = typeof(bool);
			schema["endpoints"] = typeof(bool);
			schema["reportid"] = typeof(string);
			schema["attachmentid"] = typeof(string);
			schema["verbose"] = typeof(bool);
			schema["slotid"] = typeof(Guid);
		}

		protected override bool FailOnMissingArgument
		{
			get
			{
				return true;
			}
		}

		public override XElement RunDiagnosticOperation(Func<XElement> operation)
		{
			XElement result;
			try
			{
				result = operation();
			}
			catch (MigrationMailboxNotFoundOnServerException ex)
			{
				result = new XElement("error", "mailbox moved to " + ex.HostServer);
			}
			catch (TransientException ex2)
			{
				result = new XElement("error", "Encountered exception: " + ex2.Message);
			}
			catch (MigrationDataCorruptionException ex3)
			{
				result = new XElement("error", "Encountered data corruption exception: " + ex3.Message);
			}
			catch (InvalidDataException ex4)
			{
				result = new XElement("error", "Encountered exception: " + ex4.Message);
			}
			catch (MigrationPermanentException ex5)
			{
				result = new XElement("error", "Encountered exception: " + ex5.Message);
			}
			catch (StoragePermanentException ex6)
			{
				result = new XElement("error", "Encountered exception: " + ex6.Message);
			}
			catch (DiagnosticArgumentException ex7)
			{
				result = new XElement("error", "Encountered exception: " + ex7.Message);
			}
			return result;
		}

		public const string VerboseArgument = "verbose";

		public const string UserArgument = "user";

		public const string OrganizationArgument = "organization";

		public const string PartitionArgument = "partition";

		public const string StorageArgument = "storage";

		public const string StatusArgument = "status";

		public const string MaxSizeArgument = "maxsize";

		public const string TypeArgument = "type";

		public const string BatchNameArgument = "batch";

		public const string ReportsArgument = "reports";

		public const string EndpointsArgument = "endpoints";

		public const string ReportIdArgument = "reportid";

		public const string AttachmentIdArgument = "attachmentid";

		public const string SlotIdArgument = "slotid";
	}
}

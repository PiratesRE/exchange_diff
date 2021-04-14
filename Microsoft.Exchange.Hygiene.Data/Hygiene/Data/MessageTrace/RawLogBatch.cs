using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class RawLogBatch : ConfigurablePropertyBag
	{
		public RawLogBatch()
		{
			this.RawLogLines = new List<byte[][]>();
			this.identity = Guid.NewGuid().ToString();
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.identity);
			}
		}

		public string MachineName
		{
			get
			{
				return (string)this[RawLogBatch.MachineNameProperty];
			}
			set
			{
				this[RawLogBatch.MachineNameProperty] = value;
			}
		}

		public string FileName
		{
			get
			{
				return (string)this[RawLogBatch.FileNameProperty];
			}
			set
			{
				this[RawLogBatch.FileNameProperty] = value;
			}
		}

		public string LogPrefix
		{
			get
			{
				return (string)this[RawLogBatch.LogPrefixProperty];
			}
			set
			{
				this[RawLogBatch.LogPrefixProperty] = value;
			}
		}

		public string LogVersion
		{
			get
			{
				return (string)this[RawLogBatch.LogVersionProperty];
			}
			set
			{
				this[RawLogBatch.LogVersionProperty] = value;
			}
		}

		public List<byte[][]> RawLogLines
		{
			get
			{
				return (List<byte[][]>)this[RawLogBatch.RawLogLinesProperty];
			}
			set
			{
				this[RawLogBatch.RawLogLinesProperty] = value;
			}
		}

		internal static readonly HygienePropertyDefinition MachineNameProperty = new HygienePropertyDefinition("MachineName", typeof(string));

		internal static readonly HygienePropertyDefinition FileNameProperty = new HygienePropertyDefinition("FileName", typeof(string));

		internal static readonly HygienePropertyDefinition LogPrefixProperty = new HygienePropertyDefinition("LogPrefix", typeof(string));

		internal static readonly HygienePropertyDefinition LogVersionProperty = new HygienePropertyDefinition("LogVersion", typeof(string));

		internal static readonly HygienePropertyDefinition RawLogLinesProperty = new HygienePropertyDefinition("RawLogLines", typeof(IList<byte[][]>));

		private readonly string identity;
	}
}

using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Optics
{
	internal class OpticsLogBatch : ConfigurablePropertyBag
	{
		public OpticsLogBatch()
		{
			this.identity = new ConfigObjectId(Guid.NewGuid().ToString());
		}

		public override ObjectId Identity
		{
			get
			{
				return this.Identity;
			}
		}

		public string LogType
		{
			get
			{
				return (string)this[OpticsLogBatch.LogTypeProperty];
			}
			set
			{
				this[OpticsLogBatch.LogTypeProperty] = value;
			}
		}

		public byte[] VersionMask
		{
			get
			{
				return (byte[])this[OpticsLogBatch.VersionMaskProperty];
			}
			set
			{
				this[OpticsLogBatch.VersionMaskProperty] = value;
			}
		}

		public byte[] Data
		{
			get
			{
				return (byte[])this[OpticsLogBatch.DataProperty];
			}
			set
			{
				this[OpticsLogBatch.DataProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition LogTypeProperty = new HygienePropertyDefinition("log-type", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition VersionMaskProperty = new HygienePropertyDefinition("version-mask", typeof(byte[]));

		public static readonly HygienePropertyDefinition DataProperty = new HygienePropertyDefinition("data", typeof(byte[]));

		private ObjectId identity;
	}
}

using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class AdObjectResolverRow : BaseRow
	{
		public AdObjectResolverRow(ADRawEntry aDRawEntry) : base(new Identity(aDRawEntry[ADObjectSchema.Guid].ToString(), aDRawEntry[ADObjectSchema.Name].ToString()), aDRawEntry)
		{
			this.ADRawEntry = aDRawEntry;
		}

		[DataMember]
		public virtual string DisplayName
		{
			get
			{
				return (string)this.ADRawEntry[ADObjectSchema.Name];
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		protected internal ADRawEntry ADRawEntry { get; internal set; }

		public static PropertyDefinition[] Properties = new ADPropertyDefinition[]
		{
			ADObjectSchema.Guid,
			ADObjectSchema.Name
		};
	}
}

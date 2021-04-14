using System;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Conversion;

namespace Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol
{
	[KnownType(typeof(DarTaskAggregateParams))]
	[DataContract]
	[KnownType(typeof(DarTaskParams))]
	public class DarTaskParamsBase
	{
		[DataMember]
		public string TaskId { get; set; }

		[DataMember]
		public byte[] TenantId { get; set; }

		[DataMember]
		public string TaskType { get; set; }

		public static T FromBytes<T>(byte[] data) where T : DarTaskParamsBase
		{
			string @string = Encoding.UTF8.GetString(data);
			return JsonConverter.Deserialize<T>(@string, null);
		}

		public byte[] ToBytes()
		{
			return Encoding.UTF8.GetBytes(JsonConverter.Serialize<DarTaskParamsBase>(this, null));
		}

		public override string ToString()
		{
			return JsonConverter.Serialize<DarTaskParamsBase>(this, null);
		}
	}
}

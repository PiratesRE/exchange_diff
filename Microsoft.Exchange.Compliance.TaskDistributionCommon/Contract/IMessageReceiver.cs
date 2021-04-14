using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Contract
{
	[ServiceContract(Namespace = "http://schemas.microsoft.com/informationprotection/computefabric")]
	public interface IMessageReceiver
	{
		[OperationContract]
		Task<byte[][]> ReceiveMessagesAsync(byte[][] messageBlobs);
	}
}

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Security
{
	public static class RPSErrorHandler
	{
		public static RPSErrorCategory CategorizeRPSException(COMException e)
		{
			return RPSErrorHandler.CategorizeRPSError(e.ErrorCode);
		}

		public static RPSErrorCategory CategorizeRPSError(int error)
		{
			if (error <= -2147183611)
			{
				if (error != -2147205020)
				{
					switch (error)
					{
					case -2147184128:
					case -2147184124:
					case -2147184123:
					case -2147184121:
					case -2147184112:
					case -2147184109:
					case -2147184104:
					case -2147184103:
					case -2147184102:
					case -2147184100:
					case -2147184095:
					case -2147184091:
					case -2147184081:
					case -2147184080:
					case -2147184075:
					case -2147184074:
					case -2147184073:
					case -2147184072:
					case -2147184071:
					case -2147184070:
					case -2147184069:
					case -2147184068:
					case -2147184067:
					case -2147184066:
					case -2147184065:
					case -2147184064:
					case -2147184063:
					case -2147184062:
					case -2147184061:
					case -2147184060:
					case -2147184059:
					case -2147184058:
					case -2147184057:
					case -2147184056:
					case -2147184055:
					case -2147184054:
					case -2147184053:
					case -2147184052:
					case -2147184051:
					case -2147184050:
					case -2147184049:
					case -2147184048:
					case -2147184047:
					case -2147184046:
					case -2147184045:
					case -2147184044:
					case -2147184043:
					case -2147184042:
					case -2147184041:
					case -2147184040:
					case -2147184039:
						return RPSErrorCategory.OperationError;
					case -2147184127:
					case -2147184122:
					case -2147184119:
					case -2147184118:
					case -2147184116:
					case -2147184113:
					case -2147184111:
					case -2147184110:
					case -2147184108:
					case -2147184106:
					case -2147184101:
					case -2147184098:
					case -2147184097:
					case -2147184096:
					case -2147184094:
					case -2147184093:
					case -2147184090:
					case -2147184089:
					case -2147184086:
					case -2147184083:
					case -2147184082:
					case -2147184079:
					case -2147184078:
					case -2147184077:
						return RPSErrorCategory.ConfigurationError;
					case -2147184126:
					case -2147184125:
					case -2147184120:
					case -2147184117:
					case -2147184107:
					case -2147184085:
					case -2147184084:
						break;
					case -2147184115:
					case -2147184114:
					case -2147184105:
					case -2147184099:
						return RPSErrorCategory.ClientError;
					case -2147184092:
					case -2147184088:
					case -2147184087:
					case -2147184076:
						return RPSErrorCategory.TransientError;
					default:
						switch (error)
						{
						case -2147183616:
						case -2147183614:
						case -2147183612:
						case -2147183611:
							return RPSErrorCategory.OperationError;
						case -2147183615:
							break;
						case -2147183613:
							return RPSErrorCategory.ClientError;
						default:
							return RPSErrorCategory.OperationError;
						}
						break;
					}
					return RPSErrorCategory.ExternalError;
				}
			}
			else if (error <= -2146893802)
			{
				if (error == -2147023901)
				{
					return RPSErrorCategory.TransientError;
				}
				if (error != -2146893802)
				{
					return RPSErrorCategory.OperationError;
				}
			}
			else if (error != -2146885628)
			{
				switch (error)
				{
				case -2146885621:
				case -2146885620:
					break;
				default:
					return RPSErrorCategory.OperationError;
				}
			}
			return RPSErrorCategory.ConfigurationError;
		}
	}
}

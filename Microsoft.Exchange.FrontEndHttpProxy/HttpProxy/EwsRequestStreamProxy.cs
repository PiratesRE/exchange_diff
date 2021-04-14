using System;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.HttpProxy
{
	internal class EwsRequestStreamProxy : StreamProxy
	{
		public EwsRequestStreamProxy(StreamProxy.StreamProxyType streamProxyType, Stream source, Stream target, byte[] buffer, IRequestContext requestContext, bool shouldInsertSecurityContext, bool shouldInsertFreeBusyDefaultSecurityContext, string requestVersionToAdd) : base(streamProxyType, source, target, buffer, requestContext)
		{
			this.shouldInsertSecurityContext = shouldInsertSecurityContext;
			this.shouldInsertFreeBusyDefaultSecurityContext = shouldInsertFreeBusyDefaultSecurityContext;
			this.requestVersionToAdd = requestVersionToAdd;
		}

		public static byte[] FreeBusyPermissionDefaultSecurityAccessToken
		{
			get
			{
				if (EwsRequestStreamProxy.freeBusyPermissionDefaultSecurityContextBytes == null)
				{
					SerializedSecurityAccessToken serializedSecurityAccessToken = new SerializedSecurityAccessToken();
					try
					{
						using (ClientSecurityContext clientSecurityContext = ClientSecurityContext.FreeBusyPermissionDefaultClientSecurityContext.Clone())
						{
							clientSecurityContext.SetSecurityAccessToken(serializedSecurityAccessToken);
						}
					}
					catch (AuthzException ex)
					{
						throw new HttpException(401, ex.Message);
					}
					EwsRequestStreamProxy.freeBusyPermissionDefaultSecurityContextBytes = serializedSecurityAccessToken.GetSecurityContextBytes();
				}
				return EwsRequestStreamProxy.freeBusyPermissionDefaultSecurityContextBytes;
			}
		}

		protected override byte[] GetUpdatedBufferToSend(ArraySegment<byte> buffer)
		{
			if (this.haveAddedEwsProxyHeader)
			{
				return null;
			}
			if ((long)buffer.Count + base.TotalBytesProxied < (long)"<Body".Length)
			{
				return null;
			}
			string @string = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
			int num = @string.IndexOf("<Header", StringComparison.OrdinalIgnoreCase);
			if (num == -1)
			{
				num = @string.IndexOf(":Header", StringComparison.OrdinalIgnoreCase);
				if (num != -1)
				{
					num = @string.LastIndexOf('<', num - 1);
				}
			}
			int num2 = @string.IndexOf("<Body", StringComparison.OrdinalIgnoreCase);
			if (num2 == -1)
			{
				num2 = @string.IndexOf(":Body", StringComparison.OrdinalIgnoreCase);
				if (num2 != -1)
				{
					num2 = @string.LastIndexOf('<', num2 - 1);
				}
			}
			byte[] array = new byte[0];
			if (this.shouldInsertSecurityContext)
			{
				byte[] inArray = this.shouldInsertFreeBusyDefaultSecurityContext ? EwsRequestStreamProxy.FreeBusyPermissionDefaultSecurityAccessToken : base.RequestContext.HttpContext.CreateSerializedSecurityAccessToken();
				string s = "<ProxySecurityContext xmlns='http://schemas.microsoft.com/exchange/services/2006/types'>" + Convert.ToBase64String(inArray) + "</ProxySecurityContext>";
				array = Encoding.UTF8.GetBytes(s);
			}
			byte[] array2;
			if (num != -1 && this.shouldInsertSecurityContext)
			{
				int num3 = @string.IndexOf('>', num) + 1;
				if (num3 == 0)
				{
					throw new HttpException(400, "Bad open element of SOAP header.");
				}
				array2 = new byte[buffer.Count + array.Length];
				Array.Copy(buffer.Array, buffer.Offset, array2, 0, num3);
				Array.Copy(array, 0, array2, num3, array.Length);
				Array.Copy(buffer.Array, buffer.Offset + num3, array2, num3 + array.Length, buffer.Count - num3);
			}
			else
			{
				if (num2 == -1)
				{
					throw new HttpException(400, "Cannot find the appropriate SOAP header or body.");
				}
				string text = "<Header xmlns='http://schemas.xmlsoap.org/soap/envelope/'>";
				if (this.requestVersionToAdd != null)
				{
					text += "<RequestServerVersion xmlns='http://schemas.microsoft.com/exchange/services/2006/types' Version='";
					text += this.requestVersionToAdd;
					text += "' />";
				}
				byte[] bytes = Encoding.UTF8.GetBytes(text);
				byte[] bytes2 = Encoding.UTF8.GetBytes("</Header>");
				array2 = new byte[buffer.Count + bytes.Length + array.Length + bytes2.Length];
				Array.Copy(buffer.Array, buffer.Offset, array2, 0, num2);
				Array.Copy(bytes, 0, array2, num2, bytes.Length);
				if (this.shouldInsertSecurityContext)
				{
					Array.Copy(array, 0, array2, num2 + bytes.Length, array.Length);
				}
				Array.Copy(bytes2, 0, array2, num2 + bytes.Length + array.Length, bytes2.Length);
				Array.Copy(buffer.Array, buffer.Offset + num2, array2, num2 + bytes.Length + array.Length + bytes2.Length, buffer.Count - num2);
			}
			this.haveAddedEwsProxyHeader = true;
			return array2;
		}

		protected override void OnTargetStreamUpdate()
		{
			base.OnTargetStreamUpdate();
			this.haveAddedEwsProxyHeader = false;
		}

		private const string BeginOfSoapHeaderTagNoNamespace = "<Header";

		private const string BeginOfSoapHeaderTagWithNamespace = ":Header";

		private const string BeginOfSoapBodyTagNoNamespace = "<Body";

		private const string BeginOfSoapBodyTagWithNamespace = ":Body";

		private const string SoapHeaderBegin = "<Header xmlns='http://schemas.xmlsoap.org/soap/envelope/'>";

		private const string SoapHeaderEnd = "</Header>";

		private const string RequestServerVersionHeaderStart = "<RequestServerVersion xmlns='http://schemas.microsoft.com/exchange/services/2006/types' Version='";

		private const string RequestServerVersionHeaderEnd = "' />";

		private const string ProxySecurityContextHeaderStart = "<ProxySecurityContext xmlns='http://schemas.microsoft.com/exchange/services/2006/types'>";

		private const string ProxySecurityContextHeaderEnd = "</ProxySecurityContext>";

		private static byte[] freeBusyPermissionDefaultSecurityContextBytes;

		private readonly bool shouldInsertSecurityContext;

		private readonly bool shouldInsertFreeBusyDefaultSecurityContext;

		private readonly string requestVersionToAdd;

		private bool haveAddedEwsProxyHeader;
	}
}

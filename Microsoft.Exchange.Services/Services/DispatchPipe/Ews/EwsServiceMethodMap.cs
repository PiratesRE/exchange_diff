using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Services.DispatchPipe.Base;

namespace Microsoft.Exchange.Services.DispatchPipe.Ews
{
	internal sealed class EwsServiceMethodMap : ServiceMethodMapBase
	{
		internal EwsServiceMethodMap(Type contractType) : base(contractType)
		{
		}

		internal bool Contains(string methodName)
		{
			ServiceMethodInfo serviceMethodInfo = null;
			return this.TryGetMethodInfo(methodName, out serviceMethodInfo);
		}

		internal override bool TryGetMethodInfo(string methodName, out ServiceMethodInfo methodInfo)
		{
			if (Global.HttpHandleDisabledMethods.Contains(methodName))
			{
				methodInfo = null;
				return false;
			}
			return base.TryGetMethodInfo(methodName, out methodInfo);
		}

		protected override ServiceMethodInfo PostProcessMethod(ServiceMethodInfo methodInfo)
		{
			if (methodInfo == null || methodInfo.RequestType == null || methodInfo.ResponseType == null)
			{
				return null;
			}
			if (EwsServiceMethodMap.UnsupportedMethods.Contains(methodInfo.Name))
			{
				return null;
			}
			FieldInfo field = methodInfo.RequestType.GetField("Body");
			FieldInfo field2 = methodInfo.ResponseType.GetField("Body");
			if (field != null)
			{
				methodInfo.RequestBodyType = field.FieldType;
			}
			if (field2 != null)
			{
				methodInfo.WrappedResponseBodyField = field2;
				methodInfo.ResponseBodyType = field2.FieldType;
			}
			if (methodInfo.RequestBodyType != null && methodInfo.ResponseBodyType != null)
			{
				methodInfo.BeginMethod = methodInfo.RequestBodyType.GetMethod("Submit", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(new Type[]
				{
					methodInfo.ResponseBodyType
				});
				return methodInfo;
			}
			throw new NullReferenceException(string.Format("RequestBodyType and/or ResponseBodyType is null for method: {0}", methodInfo.Name));
		}

		public override Type GetWrappedRequestType(string methodName)
		{
			throw new NotImplementedException();
		}

		private static HashSet<string> UnsupportedMethods = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"GetUserPhoto"
		};
	}
}

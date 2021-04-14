using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Management.DDIService;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[TypeConverter(typeof(WebServiceReference.WebServiceReferenceTypeConverter))]
	public sealed class WebServiceReference
	{
		public WebServiceReference(string serviceUrl)
		{
			this.ServiceUrl = serviceUrl;
			int num = serviceUrl.IndexOf('?');
			if (num >= 0)
			{
				this.serviceAbsolutePath = serviceUrl.Substring(0, num);
				return;
			}
			this.serviceAbsolutePath = serviceUrl;
		}

		public string ServiceUrl { get; private set; }

		public Type ServiceType
		{
			get
			{
				if (this.serviceType == null && !WebServiceReference.knownServiceTypes.TryGetValue(this.serviceAbsolutePath, out this.serviceType))
				{
					this.serviceType = WebServiceReference.WebServiceReferenceTypeConverter.GetServiceClass(this.serviceAbsolutePath);
					WebServiceReference.knownServiceTypes[this.serviceAbsolutePath] = this.serviceType;
				}
				return this.serviceType;
			}
		}

		public object ServiceInstance
		{
			get
			{
				if (this.serviceInstance == null)
				{
					this.serviceInstance = Activator.CreateInstance(this.ServiceType);
					MethodInfo method = this.ServiceType.GetMethod("InitializeOperationContext", new Type[]
					{
						typeof(string)
					});
					if (method != null)
					{
						method.Invoke(this.serviceInstance, new object[]
						{
							this.ServiceUrl
						});
					}
				}
				return this.serviceInstance;
			}
		}

		public PowerShellResults GetObject(Identity identity)
		{
			MethodInfo method = this.ServiceType.GetMethod("GetObject", new Type[]
			{
				typeof(Identity)
			});
			return (PowerShellResults)method.Invoke(this.ServiceInstance, new object[]
			{
				identity
			});
		}

		public PowerShellResults GetObjectForNew(Identity identity)
		{
			MethodInfo method = this.ServiceType.GetMethod("GetObjectForNew", new Type[]
			{
				typeof(Identity)
			});
			return (PowerShellResults)method.Invoke(this.ServiceInstance, new object[]
			{
				identity
			});
		}

		public PowerShellResults GetObjectOnDemand(Identity identity, string workflowName)
		{
			MethodInfo method = this.ServiceType.GetMethod("GetObjectOnDemand", new Type[]
			{
				typeof(Identity),
				typeof(string)
			});
			return (PowerShellResults)method.Invoke(this.ServiceInstance, new object[]
			{
				identity,
				workflowName
			});
		}

		public PowerShellResults<JsonDictionary<object>> GetList(DDIParameters filter, SortOptions sort)
		{
			MethodInfo method = this.ServiceType.GetMethod("GetList", new Type[]
			{
				typeof(DDIParameters),
				typeof(SortOptions)
			});
			return (PowerShellResults<JsonDictionary<object>>)method.Invoke(this.ServiceInstance, new object[]
			{
				filter,
				sort
			});
		}

		private static readonly SynchronizedDictionary<string, Type> knownServiceTypes = new SynchronizedDictionary<string, Type>();

		private string serviceAbsolutePath;

		private Type serviceType;

		private object serviceInstance;

		private class WebServiceReferenceTypeConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				if (value is string)
				{
					string serviceUrl = (value as string).Replace("~", HttpRuntime.AppDomainAppVirtualPath);
					return new WebServiceReference(serviceUrl);
				}
				return base.ConvertFrom(context, culture, value);
			}

			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				WebServiceReference webServiceReference = value as WebServiceReference;
				if (destinationType == typeof(InstanceDescriptor) && webServiceReference != null)
				{
					ConstructorInfo constructor = typeof(WebServiceReference).GetConstructor(new Type[]
					{
						typeof(string)
					});
					return new InstanceDescriptor(constructor, new object[]
					{
						webServiceReference.ServiceUrl
					}, true);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}

			internal static Type GetServiceClass(string serviceUrl)
			{
				string[] array = BuildManager.GetCompiledCustomString(serviceUrl).Split(WebServiceReference.WebServiceReferenceTypeConverter.compiledCustomStringSeparator, 4);
				string text = array[2];
				Type type = Type.GetType(text);
				if (type == null)
				{
					foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
					{
						type = assembly.GetType(text, false);
						if (type != null)
						{
							break;
						}
					}
					if (type == null)
					{
						throw new TypeLoadException(string.Format("Can't load class type '{0}' for service URL '{1}'.", text, serviceUrl));
					}
				}
				return type;
			}

			private static char[] compiledCustomStringSeparator = new char[]
			{
				'|'
			};
		}
	}
}

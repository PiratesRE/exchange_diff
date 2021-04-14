using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Context : IContext
	{
		private Context(IEnvironment environment, ILogger logger, PropertyBag properties, Context rootContext)
		{
			Util.ThrowOnNullArgument(environment, "environment");
			Util.ThrowOnNullArgument(logger, "logger");
			this.environment = environment;
			this.logger = logger;
			this.properties = properties;
			this.rootContext = rootContext;
		}

		public ILogger Logger
		{
			get
			{
				return this.logger;
			}
		}

		public IEnvironment Environment
		{
			get
			{
				return this.environment;
			}
		}

		public IPropertyBag Properties
		{
			get
			{
				return this.properties;
			}
		}

		public static IContext CreateRoot(IEnvironment environment, ILogger logger)
		{
			return new Context(environment, logger, new PropertyBag(), null);
		}

		public IContext CreateDerived()
		{
			return new Context(this.environment, this.logger, this.CreateDerivedPropertyBag(), this.rootContext ?? this);
		}

		private PropertyBag CreateDerivedPropertyBag()
		{
			PropertyBag propertyBag = new PropertyBag();
			foreach (ContextProperty property in this.properties.AllProperties)
			{
				object value;
				ExAssert.RetailAssert(this.properties.TryGet(property, out value), "Property that was returned from AllProperites must yield true from TryGet");
				propertyBag.Set(property, value);
			}
			return propertyBag;
		}

		private readonly IEnvironment environment;

		private readonly ILogger logger;

		private readonly PropertyBag properties;

		private readonly Context rootContext;
	}
}

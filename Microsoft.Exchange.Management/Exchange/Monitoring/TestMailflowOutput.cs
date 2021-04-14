using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class TestMailflowOutput : ConfigurableObject
	{
		internal TestMailflowOutput(string testMailflowResult, EnhancedTimeSpan messageLatencyTime, bool isRemoteTest) : base(new SimpleProviderPropertyBag())
		{
			this.TestMailflowResult = testMailflowResult;
			this.MessageLatencyTime = messageLatencyTime;
			this.IsRemoteTest = isRemoteTest;
		}

		public string TestMailflowResult
		{
			get
			{
				return (string)this.propertyBag[TestMailflowOutputSchema.TestMailflowResult];
			}
			private set
			{
				this.propertyBag[TestMailflowOutputSchema.TestMailflowResult] = value;
			}
		}

		public EnhancedTimeSpan MessageLatencyTime
		{
			get
			{
				return (EnhancedTimeSpan)this.propertyBag[TestMailflowOutputSchema.MessageLatencyTime];
			}
			private set
			{
				this.propertyBag[TestMailflowOutputSchema.MessageLatencyTime] = value;
			}
		}

		public bool IsRemoteTest
		{
			get
			{
				return (bool)this.propertyBag[TestMailflowOutputSchema.IsRemoteTest];
			}
			private set
			{
				this.propertyBag[TestMailflowOutputSchema.IsRemoteTest] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return TestMailflowOutput.schema;
			}
		}

		private static ObjectSchema schema = ObjectSchema.GetInstance<TestMailflowOutputSchema>();
	}
}

using System;

namespace System.Runtime.CompilerServices
{
	// Token: 0x02000002 RID: 2
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	internal sealed class IgnoresAccessChecksToAttribute : Attribute
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public IgnoresAccessChecksToAttribute(string assemblyName)
		{
		}
	}
}

﻿using System;

namespace System.Runtime.CompilerServices
{
	// Token: 0x02000002 RID: 2
	[CompilerGenerated]
	[AttributeUsage(AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
	internal sealed class RefSafetyRulesAttribute : Attribute
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public RefSafetyRulesAttribute(int A_1)
		{
			this.Version = A_1;
		}

		// Token: 0x04000001 RID: 1
		public readonly int Version;
	}
}

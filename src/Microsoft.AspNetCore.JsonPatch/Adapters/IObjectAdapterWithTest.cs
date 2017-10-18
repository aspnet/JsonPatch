// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.JsonPatch.Operations;

namespace Microsoft.AspNetCore.JsonPatch.Adapters
{
    /// <summary>
    /// Defines the operations that can be performed on a JSON patch document, including "test".
    /// </summary>
    public interface IObjectAdapterWithTest : IObjectAdapter
    {
        void Test(Operation operation, object objectToApplyTo);
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.JsonPatch.Helpers
{
    internal interface IDictionaryWrapper
    {
        object GetValueForCaseInsensitiveKey(object key);
        void SetValueForCaseInsensitiveKey(object key, object value);
        void RemoveValueForCaseInsensitiveKey(object key);
        bool ContainsCaseInsensitiveKey(object key);
    }
}

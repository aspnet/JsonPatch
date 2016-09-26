// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.JsonPatch.Helpers
{
    internal interface ICaseInsensitiveDictionary
    {
        object Get(object key);
        void Add(object key, object value);
        void Set(object key, object value);
        void Remove(object key);
        bool ContainsKey(object key);
    }
}

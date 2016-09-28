// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.JsonPatch.Helpers
{
    internal static class ConversionResultProvider
    {
        public static ConversionResult ConvertTo(Type propertyType, object value)
        {
            try
            {
                var deserialized = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value), propertyType);

                return new ConversionResult(canBeConverted: true, convertedInstance: deserialized);
            }
            catch
            {
                return new ConversionResult(canBeConverted: false, convertedInstance: null);
            }
        }
    }
}

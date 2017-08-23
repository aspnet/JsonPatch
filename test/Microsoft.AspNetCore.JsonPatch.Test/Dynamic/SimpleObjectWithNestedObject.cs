// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.AspNetCore.JsonPatch.Internal
{
    public class SimpleObjectWithNestedObject
    {
        public int IntegerValue { get; set; }
        public NestedObject NestedObject { get; set; }
        public SimpleObject SimpleObject { get; set; }
        public List<SimpleObject> ListOfSimpleDTO { get; set; }

        public SimpleObjectWithNestedObject()
        {
            NestedObject = new NestedObject();
            SimpleObject = new SimpleObject();
            ListOfSimpleDTO = new List<SimpleObject>();
        }
    }
}

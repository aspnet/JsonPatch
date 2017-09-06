// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Xunit;

namespace Microsoft.AspNetCore.JsonPatch
{
    public class GetPathTest
    {
        [Fact]
        public void ExpressionType_MemberAccess()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();

            // Act
            var path = patchDocument.GetPath(p => p.SimpleObject.IntegerList);

            // Assert
            Assert.Equal("/simpleobject/integerlist", path);
        }

        [Fact]
        public void ExpressionType_ArrayIndex()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<int[]>();

            // Act
            var path = patchDocument.GetPath(p => p[3]);

            // Assert
            Assert.Equal("/3", path);
        }

        [Fact]
        public void ExpressionType_Call()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<List<int>>();

            // Act
            var path = patchDocument.GetPath(p => p[3]);

            // Assert
            Assert.Equal("/3", path);
        }

        [Fact]
        public void ExpressionType_Parameter()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<SimpleObject>();

            // Act
            var path = patchDocument.GetPath(p => p);

            // Assert
            Assert.Empty(path);
        }

        [Fact]
        public void ExpressionType_Convert()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<NestedObjectWithDerivedClass>();

            // Act
            var path = patchDocument.GetPath(p => (BaseClass)p.DerivedObject);

            // Assert
            Assert.Equal("/derivedobject", path);
        }
    }

    internal class DerivedClass : BaseClass
    {
        public DerivedClass()
        {
        }
    }

    internal class NestedObjectWithDerivedClass
    {
        public DerivedClass DerivedObject { get; set; }
    }

    internal class BaseClass
    {
    }
}

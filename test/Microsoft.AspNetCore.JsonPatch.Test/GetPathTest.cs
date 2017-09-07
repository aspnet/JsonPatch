﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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
            var path = patchDocument.GetPath(p => p.SimpleObject.IntegerList, null);

            // Assert
            Assert.Equal("/simpleobject/integerlist", path);
        }

        [Fact]
        public void ExpressionType_ArrayIndex()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<int[]>();

            // Act
            var path = patchDocument.GetPath(p => p[3], null);

            // Assert
            Assert.Equal("/3", path);
        }

        [Fact]
        public void ExpressionType_Call()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<List<int>>();

            // Act
            var path = patchDocument.GetPath(p => p[3], null);

            // Assert
            Assert.Equal("/3", path);
        }

        [Fact]
        public void ExpressionType_Parameter()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<SimpleObject>();

            // Act
            var path = patchDocument.GetPath(p => p, null);

            // Assert
            Assert.Equal("/", path);
        }

        [Fact]
        public void ExpressionType_Convert()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<NestedObjectWithDerivedClass>();

            // Act
            var path = patchDocument.GetPath(p => (BaseClass)p.DerivedObject, null);

            // Assert
            Assert.Equal("/derivedobject", path);
        }

        [Fact]
        public void ExpressionType_NotSupported()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<SimpleObject>();

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                patchDocument.GetPath(p => p.IntegerValue >= 4, null);
            });

            // Assert
            Assert.Equal(
                string.Format("The expression '(p.IntegerValue >= 4)' is not supported."),
                exception.Message);
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

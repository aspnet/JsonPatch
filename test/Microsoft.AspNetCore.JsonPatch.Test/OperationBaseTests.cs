// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;

namespace Microsoft.AspNetCore.JsonPatch.Operations
{
    public class OperationBaseTests
    {
        [Theory]
        [InlineData("add", OperationType.Add)]
        [InlineData("copy", OperationType.Copy)]
        [InlineData("move", OperationType.Move)]
        [InlineData("remove", OperationType.Remove)]
        [InlineData("replace", OperationType.Replace)]
        [InlineData("test", OperationType.Test)]
        public void SetValidOperationType(string op, OperationType operationType)
        {
            // Arrange
            var operationBase = new OperationBase();
            operationBase.op = op;

            // Act & Assert
            Assert.Equal(operationType, operationBase.OperationType);
        }

        [Theory]
        [InlineData("invalid", OperationType.Invalid)]
        [InlineData("coppy", OperationType.Invalid)]
        [InlineData("notvalid", OperationType.Invalid)]
        public void InvalidOperationType_SetsOperationTypeInvalid(string op, OperationType operationType)
        {
            // Arrange
            var operationBase = new OperationBase();
            operationBase.op = op;

            // Act & Assert
            Assert.Equal(operationType, operationBase.OperationType);
        }
    }
}

using Microsoft.AspNetCore.JsonPatch.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace Microsoft.AspNetCore.JsonPatch.Test
{
    public class SimpleObjectGeneratePatchTests
    {
        [Fact]
        public void GeneratePatchInteger()
        {
            var original = new SimpleDTO()
            {
                IntegerValue = 1
            };

            var updated = new SimpleDTO()
            {
                IntegerValue = 2
            };

            // create patch
            var patchDoc = updated.GeneratePatch(original);

            // Act
            patchDoc.ApplyTo(original);

            // Assert
            Assert.Equal(2, original.IntegerValue);
        }

        [Fact]
        public void GeneratePatchString()
        {
            var original = new SimpleDTO()
            {
                StringProperty = "A"
            };

            var updated = new SimpleDTO()
            {
                StringProperty = "B"
            };

            // create patch
            var patchDoc = updated.GeneratePatch(original);

            // Act
            patchDoc.ApplyTo(original);

            // Assert
            Assert.Equal("B", original.StringProperty);
        }

        [Fact]
        public void GeneratePatchDouble()
        {
            var original = new SimpleDTO()
            {
                DoubleValue = 1.2
            };

            var updated = new SimpleDTO()
            {
                DoubleValue = 1.5
            };

            // create patch
            var patchDoc = updated.GeneratePatch(original);

            // Act
            patchDoc.ApplyTo(original);

            // Assert
            Assert.Equal(1.5, original.DoubleValue);
        }

        [Fact]
        public void GeneratePatchDecimal()
        {
            var original = new SimpleDTO()
            {
                DecimalValue = 1.2M
            };

            var updated = new SimpleDTO()
            {
                DecimalValue = 1.5M
            };

            // create patch
            var patchDoc = updated.GeneratePatch(original);

            // Act
            patchDoc.ApplyTo(original);

            // Assert
            Assert.Equal(1.5M, original.DecimalValue);
        }

        [Fact]
        public void AddToBeginnerIntegerIList()
        {
            var original = new SimpleDTO()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var updated = new SimpleDTO()
            {
                IntegerList = new List<int>() { 4, 1, 2, 3 }
            };

            // create patch
            var patchDoc = updated.GeneratePatch(original);

            // Act
            patchDoc.ApplyTo(original);

            // Assert
            Assert.Equal(new List<int>() { 4, 1, 2, 3 }, original.IntegerList);
        }

        [Fact]
        public void AddToEndIntegerIList()
        {
            var original = new SimpleDTO()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var updated = new SimpleDTO()
            {
                IntegerList = new List<int>() { 1, 2, 3, 4 }
            };

            // create patch
            var patchDoc = updated.GeneratePatch(original);

            // Act
            patchDoc.ApplyTo(original);

            // Assert
            Assert.Equal(new List<int>() { 1, 2, 3, 4 }, original.IntegerList);
        }

        [Fact]
        public void RemoverFromBeginnerIntegerIList()
        {
            var original = new SimpleDTO()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var updated = new SimpleDTO()
            {
                IntegerList = new List<int>() { 2, 3 }
            };

            // create patch
            var patchDoc = updated.GeneratePatch(original);

            // Act
            patchDoc.ApplyTo(original);

            // Assert
            Assert.Equal(new List<int>() { 2, 3 }, original.IntegerList);
        }

        [Fact]
        public void RemoverFromEndIntegerIList()
        {
            var original = new SimpleDTO()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var updated = new SimpleDTO()
            {
                IntegerList = new List<int>() { 1, 2 }
            };

            // create patch
            var patchDoc = updated.GeneratePatch(original);

            // Act
            patchDoc.ApplyTo(original);

            // Assert
            Assert.Equal(new List<int>() { 1, 2 }, original.IntegerList);
        }

        [Fact]
        public void MultiFieldsPath()
        {
            var original = new SimpleDTO()
            {
                IntegerValue = 12,
                StringProperty = "A",
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var updated = new SimpleDTO()
            {
                IntegerValue = 14,
                StringProperty = "B",
                IntegerList = new List<int>() { 1, 2 }
            };

            // create patch
            var patchDoc = updated.GeneratePatch(original);

            // Act
            patchDoc.ApplyTo(original);

            // Assert
            Assert.Equal(new List<int>() { 1, 2 }, original.IntegerList);
            Assert.Equal("B", original.StringProperty);
            Assert.Equal(14, original.IntegerValue);
        }
    }
}

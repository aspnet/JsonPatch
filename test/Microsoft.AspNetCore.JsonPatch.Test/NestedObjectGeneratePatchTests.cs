using Microsoft.AspNetCore.JsonPatch.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace Microsoft.AspNetCore.JsonPatch.Test
{
    public class NestedObjectGeneratePatchTests
    {
        [Fact]
        public void NestedAndInteger()
        {
            var original = new SimpleDTOWithNestedDTO()
            {
                IntegerValue = 1,
                NestedDTO = new NestedDTO() { StringProperty = "B" }
            };

            var updated = new SimpleDTOWithNestedDTO()
            {
                IntegerValue = 2,
                NestedDTO = new NestedDTO() { StringProperty = "C" }
            };

            // create patch
            var patchDoc = updated.GeneratePatch(original);

            // Act
            patchDoc.ApplyTo(original);

            // Assert
            Assert.Equal(2, original.IntegerValue);
            Assert.Equal("C", original.NestedDTO.StringProperty);
        }


        [Fact]
        public void NestedAndListIntegerInteger()
        {
            var original = new SimpleDTOWithNestedDTO()
            {
                SimpleDTO = new SimpleDTO()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            var updated = new SimpleDTOWithNestedDTO()
            {
                SimpleDTO = new SimpleDTO()
                {
                    IntegerList = new List<int>() { 4, 1, 2, 3 }
                },
                NestedDTO = new NestedDTO() { StringProperty = "C" }
            };

            // create patch
            var patchDoc = updated.GeneratePatch(original);

            // Act
            patchDoc.ApplyTo(original);

            // Assert
            Assert.Equal("C", original.NestedDTO.StringProperty);
            Assert.Equal(new List<int>() { 4, 1, 2, 3 }, original.SimpleDTO.IntegerList);
        }

        [Fact]
        public void ComplextTypeListSpecifyIndex()
        {
            // Arrange
            var original = new SimpleDTOWithNestedDTO()
            {
                SimpleDTOList = new List<SimpleDTO>()
                {
                    new SimpleDTO
                    {
                        StringProperty = "String1"
                    },
                    new SimpleDTO
                    {
                        StringProperty = "String2"
                    }
                }
            };

            var updated = new SimpleDTOWithNestedDTO()
            {
                SimpleDTOList = new List<SimpleDTO>()
                {
                    new SimpleDTO
                    {
                        StringProperty = "ChangedString1"
                    },
                    new SimpleDTO
                    {
                        StringProperty = "String2"
                    }
                }
            };

            // create patch
            var patchDoc = updated.GeneratePatch(original);

            // Act
            patchDoc.ApplyTo(original);

            // Assert
            Assert.Equal("ChangedString1", original.SimpleDTOList[0].StringProperty);
        }
    }
}

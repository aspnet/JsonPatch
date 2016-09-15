// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Newtonsoft.Json;
using System.Linq;
using Xunit;

namespace Microsoft.AspNetCore.JsonPatch.Test
{
    public class JsonPropertyTests
    {
        [Fact]
        public void HonourJsonPropertyOnSerialization()
        {
            JsonPatchDocument<JsonPropertyDTO> patchDoc = new JsonPatchDocument<JsonPropertyDTO>();
            patchDoc.Add(p => p.Name, "John");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            // serialized value should have "AnotherName" as path
            // deserialize to a JsonPatchDocument<JsonPropertyWithAnotherNameDTO> to check
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyWithAnotherNameDTO>>(serialized);

            Assert.Equal(deserialized.Operations.First().path, "/anothername");
        }

        [Fact]
        public void CanApplyToDifferentlyTypedClassWithPropertyMatchingJsonPropertyName()
        {
            JsonPatchDocument<JsonPropertyDTO> patchDocToSerialize =
                new JsonPatchDocument<JsonPropertyDTO>();
            patchDocToSerialize.Add(p => p.Name, "John");

            // the patchdoc will deserialize to "anothername".  We should thus be able to apply 
            // it to a class that HAS that other property name.
            var doc = new JsonPropertyWithAnotherNameDTO()
            {
                AnotherName = "InitialValue"
            };

            var serialized = JsonConvert.SerializeObject(patchDocToSerialize);
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyWithAnotherNameDTO>>
                (serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(doc.AnotherName, "John");
        }

        [Fact]
        public void CanApplyToSameTypedClassWithMatchingJsonPropertyName()
        {
            JsonPatchDocument<JsonPropertyDTO> patchDocToSerialize =
                new JsonPatchDocument<JsonPropertyDTO>();
            patchDocToSerialize.Add(p => p.Name, "John");

            // the patchdoc will deserialize to "anothername".  As JsonPropertyDTO has
            // a JsonProperty signifying that "Name" should be deseriallized from "AnotherName",
            // we should be able to apply the patchDoc.

            var doc = new JsonPropertyDTO()
            {
                Name = "InitialValue"
            };

            var serialized = JsonConvert.SerializeObject(patchDocToSerialize);
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyDTO>>
                (serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(doc.Name, "John");
        }

        [Fact]
        public void HonourJsonPropertyOnApplyForAdd()
        {
            var doc = new JsonPropertyDTO()
            {
                Name = "InitialValue"
            };
            
            // serialization should serialize to "AnotherName"
            var serialized = "[{\"value\":\"Kevin\",\"path\":\"/AnotherName\",\"op\":\"add\"}]";
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyDTO>>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal("Kevin", doc.Name);
        }

        [Fact]
        public void HonourJsonPropertyOnApplyForRemove()
        {
            var doc = new JsonPropertyDTO()
            {
                Name = "InitialValue"
            };

            // serialization should serialize to "AnotherName"
            var serialized = "[{\"path\":\"/AnotherName\",\"op\":\"remove\"}]";
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyDTO>>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(null, doc.Name);
        }
    }
}

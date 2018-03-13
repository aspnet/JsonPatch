using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.JsonPatch.Converters;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.JsonPatch.Test
{
    public class TestAdapterFactory : IAdapterFactory
    {
        public IAdapter Create(object target, IContractResolver contractResolver)
        {
            return new TestAdapter(target, contractResolver);
        }
    }

    public class TestAdapter : IAdapter
    {
        public Object Target { get; }
        public IContractResolver ContractResolver { get; }
        public TestAdapter(object target, IContractResolver contractResolver)
        {
            Target = target;
            ContractResolver = contractResolver;
        }
        public bool TryAdd(object target, string segment, IContractResolver contractResolver, object value, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        public bool TryGet(object target, string segment, IContractResolver contractResolver, out object value, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        public bool TryRemove(object target, string segment, IContractResolver contractResolver, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        public bool TryReplace(object target, string segment, IContractResolver contractResolver, object value, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        public bool TryTest(object target, string segment, IContractResolver contractResolver, object value, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        public bool TryTraverse(object target, string segment, IContractResolver contractResolver, out object nextTarget, out string errorMessage)
        {
            throw new NotImplementedException();
        }
    }

    public class JsonPatchDocumentResolverForCustomFactory:DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            return new JsonPatchDocumentConverterWithCustomFactory();
        }
    }

    public class JsonPatchDocumentConverterWithCustomFactory: TypedJsonPatchDocumentConverter
    {
        public JsonPatchDocumentConverterWithCustomFactory()
        {
        }

        protected override object CreateTypedContainer(Type objectType, object operations)
        {
            return Activator.CreateInstance(objectType, operations, new DefaultContractResolver(), new TestAdapterFactory());
        }
    }
}

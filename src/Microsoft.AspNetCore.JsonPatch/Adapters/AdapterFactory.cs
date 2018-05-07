﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Newtonsoft.Json.Serialization;

namespace Microsoft.AspNetCore.JsonPatch.Adapters
{
    /// <summary>
    /// The default AdapterFactory to be used for resolving <see cref="IAdapter"/>.
    /// </summary>
    public class AdapterFactory : IAdapterFactory
    {
        /// <inheritdoc />
        public virtual IAdapter Create(object target, IContractResolver contractResolver)
        {
            var jsonContract = contractResolver.ResolveContract(target.GetType());

            if (target is IList)
            {
                return new ListAdapter();
            }
            else if (jsonContract is JsonDictionaryContract jsonDictionaryContract)
            {
                var type = typeof(DictionaryAdapter<,>).MakeGenericType(jsonDictionaryContract.DictionaryKeyType, jsonDictionaryContract.DictionaryValueType);
                return (IAdapter)Activator.CreateInstance(type);
            }
            else if (jsonContract is JsonDynamicContract)
            {
                return new DynamicObjectAdapter();
            }
            else
            {
                return new PocoAdapter();
            }
        }
    }
}

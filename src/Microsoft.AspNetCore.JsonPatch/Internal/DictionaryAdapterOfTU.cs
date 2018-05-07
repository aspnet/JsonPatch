﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Microsoft.AspNetCore.JsonPatch.Internal
{
    public class DictionaryAdapter<TKey, TValue> : IAdapter
    {
        public virtual bool TryAdd(
            object target,
            string segment,
            IContractResolver contractResolver,
            object value,
            out string errorMessage)
        {
            var contract = (JsonDictionaryContract)contractResolver.ResolveContract(target.GetType());
            var key = contract.DictionaryKeyResolver(segment);
            var dictionary = (IDictionary<TKey, TValue>)target;

            // As per JsonPatch spec, if a key already exists, adding should replace the existing value
            if (!TryConvertKey(key, out var convertedKey, out errorMessage))
            {
                return false;
            }

            if (!TryConvertValue(value, out var convertedValue, out errorMessage))
            {
                return false;
            }

            dictionary[convertedKey] = convertedValue;
            errorMessage = null;
            return true;
        }

        public virtual bool TryGet(
            object target,
            string segment,
            IContractResolver contractResolver,
            out object value,
            out string errorMessage)
        {
            var contract = (JsonDictionaryContract)contractResolver.ResolveContract(target.GetType());
            var key = contract.DictionaryKeyResolver(segment);
            var dictionary = (IDictionary<TKey, TValue>)target;

            if (!TryConvertKey(key, out var convertedKey, out errorMessage))
            {
                value = null;
                return false;
            }

            if (!dictionary.ContainsKey(convertedKey))
            {
                value = null;
                errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
                return false;
            }

            value = dictionary[convertedKey];
            errorMessage = null;
            return true;
        }

        public virtual bool TryRemove(
            object target,
            string segment,
            IContractResolver contractResolver,
            out string errorMessage)
        {
            var contract = (JsonDictionaryContract)contractResolver.ResolveContract(target.GetType());
            var key = contract.DictionaryKeyResolver(segment);
            var dictionary = (IDictionary<TKey, TValue>)target;

            if (!TryConvertKey(key, out var convertedKey, out errorMessage))
            {
                return false;
            }

            // As per JsonPatch spec, the target location must exist for remove to be successful
            if (!dictionary.ContainsKey(convertedKey))
            {
                errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
                return false;
            }

            dictionary.Remove(convertedKey);

            errorMessage = null;
            return true;
        }

        public virtual bool TryReplace(
            object target,
            string segment,
            IContractResolver contractResolver,
            object value,
            out string errorMessage)
        {
            var contract = (JsonDictionaryContract)contractResolver.ResolveContract(target.GetType());
            var key = contract.DictionaryKeyResolver(segment);
            var dictionary = (IDictionary<TKey, TValue>)target;

            if (!TryConvertKey(key, out var convertedKey, out errorMessage))
            {
                return false;
            }

            // As per JsonPatch spec, the target location must exist for remove to be successful
            if (!dictionary.ContainsKey(convertedKey))
            {
                errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
                return false;
            }

            if (!TryConvertValue(value, out var convertedValue, out errorMessage))
            {
                return false;
            }

            dictionary[convertedKey] = convertedValue;

            errorMessage = null;
            return true;
        }

        public virtual bool TryTest(
            object target,
            string segment,
            IContractResolver contractResolver,
            object value,
            out string errorMessage)
        {
            var contract = (JsonDictionaryContract)contractResolver.ResolveContract(target.GetType());
            var key = contract.DictionaryKeyResolver(segment);
            var dictionary = (IDictionary<TKey, TValue>)target;

            if (!TryConvertKey(key, out var convertedKey, out errorMessage))
            {
                return false;
            }

            // As per JsonPatch spec, the target location must exist for test to be successful
            if (!dictionary.ContainsKey(convertedKey))
            {
                errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
                return false;
            }

            if (!TryConvertValue(value, out var convertedValue, out errorMessage))
            {
                return false;
            }

            var currentValue = dictionary[convertedKey];

            // The target segment does not have an assigned value to compare the test value with
            if (currentValue == null || string.IsNullOrEmpty(currentValue.ToString()))
            {
                errorMessage = Resources.FormatValueForTargetSegmentCannotBeNullOrEmpty(segment);
                return false;
            }

            if (!JToken.DeepEquals(JsonConvert.SerializeObject(currentValue), JsonConvert.SerializeObject(convertedValue)))
            {
                errorMessage = Resources.FormatValueNotEqualToTestValue(currentValue, value, segment);
                return false;
            }
            else
            {
                errorMessage = null;
                return true;
            }
        }

        public virtual bool TryTraverse(
            object target,
            string segment,
            IContractResolver contractResolver,
            out object nextTarget,
            out string errorMessage)
        {
            var contract = (JsonDictionaryContract)contractResolver.ResolveContract(target.GetType());
            var key = contract.DictionaryKeyResolver(segment);
            var dictionary = (IDictionary<TKey, TValue>)target;

            if (!TryConvertKey(key, out var convertedKey, out errorMessage))
            {
                nextTarget = null;
                return false;
            }

            if (dictionary.ContainsKey(convertedKey))
            {
                nextTarget = dictionary[convertedKey];
                errorMessage = null;
                return true;
            }
            else
            {
                nextTarget = null;
                errorMessage = null;
                return false;
            }
        }

        protected virtual bool TryConvertKey(string key, out TKey convertedKey, out string errorMessage)
        {
            var conversionResult = ConversionResultProvider.ConvertTo(key, typeof(TKey));
            if (conversionResult.CanBeConverted)
            {
                errorMessage = null;
                convertedKey = (TKey)conversionResult.ConvertedInstance;
                return true;
            }
            else
            {
                errorMessage = Resources.FormatInvalidPathSegment(key);
                convertedKey = default(TKey);
                return false;
            }
        }

        protected virtual bool TryConvertValue(object value, out TValue convertedValue, out string errorMessage)
        {
            var conversionResult = ConversionResultProvider.ConvertTo(value, typeof(TValue));
            if (conversionResult.CanBeConverted)
            {
                errorMessage = null;
                convertedValue = (TValue)conversionResult.ConvertedInstance;
                return true;
            }
            else
            {
                errorMessage = Resources.FormatInvalidValueForProperty(value);
                convertedValue = default(TValue);
                return false;
            }
        }
    }
}

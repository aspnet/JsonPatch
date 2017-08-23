﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Dynamic;

namespace Microsoft.AspNetCore.JsonPatch.Test.Dynamic
{
    public class DynamicTestObject : DynamicObject
    {
        private Dictionary<string, object> _dictionary = new Dictionary<string, object>();

        public object this[string key] { get => ((IDictionary<string, object>)_dictionary)[key]; set => ((IDictionary<string, object>)_dictionary)[key] = value; }

        public ICollection<string> Keys => ((IDictionary<string, object>)_dictionary).Keys;

        public ICollection<object> Values => ((IDictionary<string, object>)_dictionary).Values;

        public int Count => ((IDictionary<string, object>)_dictionary).Count;

        public bool IsReadOnly => ((IDictionary<string, object>)_dictionary).IsReadOnly;

        public void Add(string key, object value)
        {
            ((IDictionary<string, object>)_dictionary).Add(key, value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            ((IDictionary<string, object>)_dictionary).Add(item);
        }

        public void Clear()
        {
            ((IDictionary<string, object>)_dictionary).Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return ((IDictionary<string, object>)_dictionary).Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, object>)_dictionary).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((IDictionary<string, object>)_dictionary).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return ((IDictionary<string, object>)_dictionary).GetEnumerator();
        }

        public bool Remove(string key)
        {
            return ((IDictionary<string, object>)_dictionary).Remove(key);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return ((IDictionary<string, object>)_dictionary).Remove(item);
        }

        public bool TryGetValue(string key, out object value)
        {
            return ((IDictionary<string, object>)_dictionary).TryGetValue(key, out value);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name;

            return TryGetValue(name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _dictionary[binder.Name] = value;

            return true;
        }
    }
}

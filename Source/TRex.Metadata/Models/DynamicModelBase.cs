/* 
 * DynamicModelBase.cs
 * 
 *   Created: Sometime in 2016
 *   Modified: 2023-04-03-09:12:54
 * 
 *   Author: Nick Hauensteiin <Nicholas.Hauenstein@microsoft.com>
 *   Contributors: David G. Moore, Jr. david@dgmjr.io
 *                 CodeGPT (no really, it wrote a lot of this)
 *   
 *   Copyright © 2016 - 2023 Nick Hauenstein & David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */ 

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace TRex.Metadata
{
    /// <summary>
    /// This class can be used as a base class for any operations that require
    /// a completely dynamic schema as either an input or return type. It can be
    /// used as a dynamic or implicitly converted to JToken at any time.
    /// </summary>
    public class DynamicModelBase : DynamicObject
    {
        private JToken data;

        /// <summary>
        /// Returns an enumerable collection of all dynamic member names.
        /// </summary>
        /// <returns>An enumerable containing the dynamic member names.</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var dObj = (data as JObject);
            if (null != dObj)
            {
                if (dObj.Properties() != null && dObj.Properties().Any())
                {
                    return dObj.Properties().Select(p => p.Name).Union(getPublicMemberNames());
                }
            }

            return base.GetDynamicMemberNames();
        }

        /// <summary>
        /// Provides the implementation of the retrieval of a dynamic member.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="result">The dynamic member result.</param>\n    /// <returns>Returns whether the bind operation is successful.</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            if (binder == null) return false;

            try
            {
                if (getPublicMemberNames().Contains(binder.Name))
                {
                    PropertyInfo propInfo = null;
                    FieldInfo fieldInfo = null;

                    if (null != (propInfo = this.GetType().GetProperty(binder.Name)))
                    {
                        data[binder.Name] = JToken.FromObject(propInfo.GetValue(this));
                    }
                    else if (null != (fieldInfo = this.GetType().GetField(binder.Name)))
                    {
                        data[binder.Name] = JToken.FromObject(fieldInfo.GetValue(this));
                    }
                    else { }
                }

                result = data[binder.Name];
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Provides the implementation of the setting of a dynamic member.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="value">The dynamic member value.</param>
        /// <returns>Returns whether the setting operation is successful.</returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (binder == null) return false;

            try
            {
                if (getPublicMemberNames().Contains(binder.Name))
                {
                    PropertyInfo propInfo = null;
                    FieldInfo fieldInfo = null;

                    if (null != (propInfo = this.GetType().GetProperty(binder.Name)))
                    {
                        propInfo.SetValue(this, value);
                    }
                    else if (null != (fieldInfo = this.GetType().GetField(binder.Name)))
                    {
                        fieldInfo.SetValue(this, value);
                    }
                    else { }
                }

                data[binder.Name] = value == null ? null : JToken.FromObject(value);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns an enumerable collection of all public member names.
        /// </summary>
        /// <returns>An enumerable containing the public member names.</returns>
        private IEnumerable<string> getPublicMemberNames()
        {
            return this.GetType()
                                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                .Select(p => p.Name).Union(
                            this.GetType()
                                .GetFields(BindingFlags.Instance | BindingFlags.Public)
                                .Select(p => p.Name)).OrderBy(p => p);
        }

        /// <summary>
        /// Implicitly casts a DynamicModelBase instance to a JToken instance.
        /// </summary>
        /// <param name="source">The source instance.</param>
        /// <returns>A JToken instance.</returns>
        public static implicit operator JToken(DynamicModelBase source)
        {
            if (source == null) return null;
            return source.data;
        }

        /// <summary>
        /// Implicitly casts a JToken instance to a DynamicModelBase instance.
        /// </summary>
        /// <param name="source">The source instance.</param>
        /// <returns>A DynamicModelBase instance.</returns>
        public static implicit operator DynamicModelBase(JToken source)
        {
            if (source == null) return null;
            return new DynamicModelBase() { data = source };
        }

        /// <summary>
        /// Explicitly casts a DynamicModelBase instance to a JToken instance.
        /// </summary>
        /// <param name="source">The source instance.</param>
        /// <returns>A JToken instance.</returns>
        public static JToken ToJToken(DynamicModelBase source)
        {
            return source;
        }

        /// <summary>
        /// Explicitly casts a JToken instance to a DynamicModelBase instance.
        /// </summary>
        /// <param name="source">The source instance.</param>
        /// <returns>A DynamicModelBase instance.</returns>
        public static DynamicModelBase FromJToken(JToken source)
        {
            return source;
        }
    }
}

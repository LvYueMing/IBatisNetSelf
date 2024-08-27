using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Utilities.Objects.Members;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects
{
    /// <summary>
    /// Description ObjectProbe.
    /// </summary>
    public sealed class ObjectProbe
    {
        private static ArrayList simpleTypeMap = new ArrayList();

        static ObjectProbe()
        {
            simpleTypeMap.Add(typeof(string));
            simpleTypeMap.Add(typeof(Byte));
            simpleTypeMap.Add(typeof(Int16));
            simpleTypeMap.Add(typeof(char));
            simpleTypeMap.Add(typeof(Int32));
            simpleTypeMap.Add(typeof(Int64));
            simpleTypeMap.Add(typeof(Single));
            simpleTypeMap.Add(typeof(Double));
            simpleTypeMap.Add(typeof(Boolean));
            simpleTypeMap.Add(typeof(DateTime));
            simpleTypeMap.Add(typeof(Decimal));
            simpleTypeMap.Add(typeof(SByte));
            simpleTypeMap.Add(typeof(UInt16));
            simpleTypeMap.Add(typeof(UInt32));
            simpleTypeMap.Add(typeof(UInt64));
            simpleTypeMap.Add(typeof(IEnumerator));

            //_simpleTypeMap.Add(typeof(Hashtable));
            //_simpleTypeMap.Add(typeof(SortedList));
            //_simpleTypeMap.Add(typeof(ArrayList));
            //_simpleTypeMap.Add(typeof(Array));

            //simpleTypeMap.Add(LinkedList.class);
            //simpleTypeMap.Add(HashSet.class);
            //simpleTypeMap.Add(TreeSet.class);
            //simpleTypeMap.Add(Vector.class);
            //simpleTypeMap.Add(Hashtable.class);
        }


        /// <summary>
        /// Returns an array of the readable properties names exposed by an object
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>The properties name</returns>
        public static string[] GetReadablePropertyNames(object obj)
        {
            return ReflectionInfo.GetInstance(obj.GetType()).GetReadableMemberNames();
        }


        /// <summary>
        /// Returns an array of the writeable members name exposed by a object
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>The members name</returns>
        public static string[] GetWriteableMemberNames(object obj)
        {
            return ReflectionInfo.GetInstance(obj.GetType()).GetWriteableMemberNames();
        }


        /// <summary>
        ///  Returns the type that the set expects to receive as a parameter when
        ///  setting a member value.
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <param name="memberName">The name of the member</param>
        /// <returns>The type of the member</returns>
        public static Type GetMemberTypeForSetter(object obj, string memberName)
        {
            Type _type = obj.GetType();

            if (obj is IDictionary)
            {
                IDictionary _map = (IDictionary)obj;
                object value = _map[memberName];
                if (value == null)
                {
                    _type = typeof(object);
                }
                else
                {
                    _type = value.GetType();
                }
            }
            else
            {
                if (memberName.IndexOf('.') > -1)
                {
                    StringTokenizer _parser = new StringTokenizer(memberName, ".");
                    IEnumerator _enumerator = _parser.GetEnumerator();

                    while (_enumerator.MoveNext())
                    {
                        memberName = (string)_enumerator.Current;
                        _type = ReflectionInfo.GetInstance(_type).GetSetterType(memberName);
                    }
                }
                else
                {
                    _type = ReflectionInfo.GetInstance(_type).GetSetterType(memberName);
                }
            }

            return _type;
        }


        /// <summary>
        ///  Returns the type that the set expects to receive as a parameter when
        ///  setting a member value.
        /// </summary>
        /// <param name="type">The class type to check</param>
        /// <param name="memberName">The name of the member</param>
        /// <returns>The type of the member</returns>
        public static Type GetMemberTypeForSetter(Type type, string memberName)
        {
            Type memberType = type;
            if (memberName.IndexOf('.') > -1)
            {
                StringTokenizer parser = new StringTokenizer(memberName, ".");
                IEnumerator enumerator = parser.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    memberName = (string)enumerator.Current;
                    memberType = ReflectionInfo.GetInstance(memberType).GetSetterType(memberName);
                }
            }
            else
            {
                memberType = ReflectionInfo.GetInstance(type).GetSetterType(memberName);
            }

            return memberType;
        }

        /// <summary>
        ///  Returns the type that the get expects to receive as a parameter when
        ///  setting a member value.
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <param name="memberName">The name of the member</param>
        /// <returns>The type of the member</returns>
        public static Type GetMemberTypeForGetter(object obj, string memberName)
        {
            Type type = obj.GetType();

            if (obj is IDictionary)
            {
                IDictionary map = (IDictionary)obj;
                object value = map[memberName];
                if (value == null)
                {
                    type = typeof(object);
                }
                else
                {
                    type = value.GetType();
                }
            }
            else
            {
                if (memberName.IndexOf('.') > -1)
                {
                    StringTokenizer parser = new StringTokenizer(memberName, ".");
                    IEnumerator enumerator = parser.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        memberName = (string)enumerator.Current;
                        type = ReflectionInfo.GetInstance(type).GetGetterType(memberName);
                    }
                }
                else
                {
                    type = ReflectionInfo.GetInstance(type).GetGetterType(memberName);
                }
            }

            return type;
        }


        /// <summary>
        ///  Returns the type that the get expects to receive as a parameter when
        ///  setting a member value.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="memberName">The name of the member</param>
        /// <returns>The type of the member</returns>
        public static Type GetMemberTypeForGetter(Type type, string memberName)
        {
            if (memberName.IndexOf('.') > -1)
            {
                StringTokenizer parser = new StringTokenizer(memberName, ".");
                IEnumerator enumerator = parser.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    memberName = (string)enumerator.Current;
                    type = ReflectionInfo.GetInstance(type).GetGetterType(memberName);
                }
            }
            else
            {
                type = ReflectionInfo.GetInstance(type).GetGetterType(memberName);
            }

            return type;
        }


        /// <summary>
        ///  Returns the MemberInfo of the set member on the specified type.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="memberName">The name of the member</param>
        /// <returns>The type of the member</returns>
        public static MemberInfo GetMemberInfoForSetter(Type type, string memberName)
        {
            MemberInfo memberInfo = null;
            if (memberName.IndexOf('.') > -1)
            {
                StringTokenizer parser = new StringTokenizer(memberName, ".");
                IEnumerator enumerator = parser.GetEnumerator();
                Type parentType = null;

                while (enumerator.MoveNext())
                {
                    memberName = (string)enumerator.Current;
                    parentType = type;
                    type = ReflectionInfo.GetInstance(type).GetSetterType(memberName);
                }
                memberInfo = ReflectionInfo.GetInstance(parentType).GetSetter(memberName);
            }
            else
            {
                memberInfo = ReflectionInfo.GetInstance(type).GetSetter(memberName);
            }

            return memberInfo;
        }


        /// <summary>
        /// Gets the value of an array member on the specified object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="indexedName">The array index.</param>
        /// <param name="accessorFactory">The accessor factory.</param>
        /// <returns>The member value.</returns>
        private static object GetArrayMember(object obj, string indexedName, AccessorFactory accessorFactory)
        {
            object _value = null;

            try
            {
                int _startIndex = indexedName.IndexOf("[");
                int _length = indexedName.IndexOf("]");
                string _name = indexedName.Substring(0, _startIndex);
                string _index = indexedName.Substring(_startIndex + 1, _length - (_startIndex + 1));
                int i = System.Convert.ToInt32(_index);

                if (_name.Length > 0)
                {
                    _value = GetMember(obj, _name, accessorFactory);
                }
                else
                {
                    _value = obj;
                }

                if (_value is IList)
                {
                    _value = ((IList)_value)[i];
                }
                else
                {
                    throw new ProbeException("The '" + _name + "' member of the " + obj.GetType().Name + " class is not a List or Array.");
                }
            }
            catch (ProbeException pe)
            {
                throw pe;
            }
            catch (Exception e)
            {
                throw new ProbeException("Error getting ordinal value from .net object. Cause" + e.Message, e);
            }

            return _value;
        }

        /// <summary>
        /// Sets the array member.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="indexedName">Name of the indexed.</param>
        /// <param name="value">The value.</param>
        /// <param name="accessorFactory">The accessor factory.</param>
        private static void SetArrayMember(object obj, string indexedName, object value,
            AccessorFactory accessorFactory)
        {
            try
            {
                int startIndex = indexedName.IndexOf("[");
                int length = indexedName.IndexOf("]");
                string name = indexedName.Substring(0, startIndex);
                string index = indexedName.Substring(startIndex + 1, length - (startIndex + 1));
                int i = System.Convert.ToInt32(index);

                object list = null;
                if (name.Length > 0)
                {
                    list = GetMember(obj, name, accessorFactory);
                }
                else
                {
                    list = obj;
                }

                if (list is IList)
                {
                    ((IList)list)[i] = value;
                }
                else
                {
                    throw new ProbeException("The '" + name + "' member of the " + obj.GetType().Name + " class is not a List or Array.");
                }
            }
            catch (ProbeException pe)
            {
                throw pe;
            }
            catch (Exception e)
            {
                throw new ProbeException("Error getting ordinal value from .net object. Cause" + e.Message, e);
            }
        }


        /// <summary>
        /// Return the specified member on an object. 
        /// </summary>
        /// <param name="obj">The Object on which to invoke the specified property.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="accessorFactory">The accessor factory.</param>
        /// <returns>An Object representing the return value of the invoked property.</returns>
        public static object GetMemberValue(object obj, string memberName, AccessorFactory accessorFactory)
        {
            if (memberName.IndexOf('.') > -1)
            {
                StringTokenizer _parser = new StringTokenizer(memberName, ".");
                IEnumerator _enumerator = _parser.GetEnumerator();
                object _value = obj;
                string _token = null;

                while (_enumerator.MoveNext())
                {
                    _token = (string)_enumerator.Current;
                    _value = GetMember(_value, _token, accessorFactory);

                    if (_value == null)
                    {
                        break;
                    }
                }
                return _value;
            }
            else
            {
                return GetMember(obj, memberName, accessorFactory);
            }
        }


        /// <summary>
        /// Gets the member's value on the specified object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="accessorFactory">The accessor factory.</param>
        /// <returns>The member's value</returns>
        public static object GetMember(object obj, string memberName, AccessorFactory accessorFactory)
        {
            try
            {
                object _value = null;

                // Is this an array member?
                if (memberName.IndexOf("[") > -1)
                {
                    _value = GetArrayMember(obj, memberName, accessorFactory);
                }
                else
                {
                    if (obj is IDictionary)
                    {
                        _value = ((IDictionary)obj)[memberName];
                    }
                    else
                    {
                        Type _targetType = obj.GetType();
                        IGetAccessorFactory _getAccessorFactory = accessorFactory.GetAccessorFactory;
                        IGetAccessor _getAccessor = _getAccessorFactory.CreateGetAccessor(_targetType, memberName);

                        if (_getAccessor == null)
                        {
                            throw new ProbeException("No Get method for member " + memberName + " on instance of " + obj.GetType().Name);
                        }
                        try
                        {
                            _value = _getAccessor.Get(obj);
                        }
                        catch (Exception ae)
                        {
                            throw new ProbeException(ae);
                        }
                    }
                }
                return _value;
            }
            catch (ProbeException pe)
            {
                throw pe;
            }
            catch (Exception e)
            {
                throw new ProbeException("Could not Set property '" + memberName + "' for " + obj.GetType().Name + ".  Cause: " + e.Message, e);
            }
        }

        /// <summary>
        /// Sets the member value.
        /// </summary>
        /// <param name="obj">he Object on which to invoke the specified member.</param>
        /// <param name="aMemberName">Name of the member.</param>
        /// <param name="aMemberValue">The member value.</param>
        /// <param name="objectFactory">The object factory.</param>
        /// <param name="accessorFactory">The accessor factory.</param>
        public static void SetMemberValue(object obj, string aMemberName, object aMemberValue,
            IObjectFactory objectFactory,
            AccessorFactory accessorFactory)
        {
            if (aMemberName.IndexOf('.') > -1)
            {
                StringTokenizer _parser = new StringTokenizer(aMemberName, ".");
                IEnumerator _enumerator = _parser.GetEnumerator();
                _enumerator.MoveNext();

                string _currentPropertyName = (string)_enumerator.Current;
                object _child = obj;

                while (_enumerator.MoveNext())
                {
                    Type _type = GetMemberTypeForSetter(_child, _currentPropertyName);
                    object _parent = _child;
                    _child = GetMember(_parent, _currentPropertyName, accessorFactory);
                    if (_child == null)
                    {
                        try
                        {
                            IFactory _factory = objectFactory.CreateFactory(_type, Type.EmptyTypes);
                            _child = _factory.CreateInstance(Type.EmptyTypes);

                            SetMemberValue(_parent, _currentPropertyName, _child, objectFactory, accessorFactory);
                        }
                        catch (Exception e)
                        {
                            throw new ProbeException("Cannot set value of property '" + aMemberName + "' because '" + _currentPropertyName + "' is null and cannot be instantiated on instance of " + _type.Name + ". Cause:" + e.Message, e);
                        }
                    }
                    _currentPropertyName = (string)_enumerator.Current;
                }
                SetMember(_child, _currentPropertyName, aMemberValue, accessorFactory);
            }
            else
            {
                SetMember(obj, aMemberName, aMemberValue, accessorFactory);
            }
        }


        /// <summary>
        /// Sets the member.
        /// </summary>
        /// <param name="aObj">The obj.</param>
        /// <param name="aMemberName">Name of the member.</param>
        /// <param name="aMemberValue">The member value.</param>
        /// <param name="aAccessorFactory">The accessor factory.</param>
        public static void SetMember(object aObj, string aMemberName, object aMemberValue,
            AccessorFactory aAccessorFactory)
        {
            try
            {
                if (aMemberName.IndexOf("[") > -1)
                {
                    SetArrayMember(aObj, aMemberName, aMemberValue, aAccessorFactory);
                }
                else
                {
                    if (aObj is IDictionary)
                    {
                        ((IDictionary)aObj)[aMemberName] = aMemberValue;
                    }
                    else
                    {
                        Type _targetType = aObj.GetType();
                        ISetAccessorFactory _setAccessorFactory = aAccessorFactory.SetAccessorFactory;
                        ISetAccessor _setAccessor = _setAccessorFactory.CreateSetAccessor(_targetType, aMemberName);

                        if (_setAccessor == null)
                        {
                            throw new ProbeException("No Set method for member " + aMemberName + " on instance of " + aObj.GetType().Name);
                        }
                        try
                        {
                            _setAccessor.Set(aObj, aMemberValue);
                        }
                        catch (Exception ex)
                        {
                            throw new ProbeException(ex);
                        }
                    }
                }
            }
            catch (ProbeException pe)
            {
                throw pe;
            }
            catch (Exception e)
            {
                throw new ProbeException("Could not Get property '" + aMemberName + "' for " + aObj.GetType().Name + ".  Cause: " + e.Message, e);
            }
        }


        /// <summary>
        /// Checks to see if a Object has a writable property/field be a given name
        /// </summary>
        /// <param name="obj"> The object to check</param>
        /// <param name="propertyName">The property to check for</param>
        /// <returns>True if the property exists and is writable</returns>
        public static bool HasWritableProperty(object obj, string propertyName)
        {
            bool hasProperty = false;
            if (obj is IDictionary)
            {
                hasProperty = ((IDictionary)obj).Contains(propertyName);
            }
            else
            {
                if (propertyName.IndexOf('.') > -1)
                {
                    StringTokenizer parser = new StringTokenizer(propertyName, ".");
                    IEnumerator enumerator = parser.GetEnumerator();
                    Type type = obj.GetType();

                    while (enumerator.MoveNext())
                    {
                        propertyName = (string)enumerator.Current;
                        type = ReflectionInfo.GetInstance(type).GetGetterType(propertyName);
                        hasProperty = ReflectionInfo.GetInstance(type).HasWritableMember(propertyName);
                    }
                }
                else
                {
                    hasProperty = ReflectionInfo.GetInstance(obj.GetType()).HasWritableMember(propertyName);
                }
            }
            return hasProperty;
        }


        /// <summary>
        /// Checks to see if the Object have a property/field be a given name.
        /// </summary>
        /// <param name="obj">The Object on which to invoke the specified property.</param>
        /// <param name="propertyName">The name of the property to check for.</param>
        /// <returns>
        /// True or false if the property exists and is readable.
        /// </returns>
        public static bool HasReadableProperty(object obj, string propertyName)
        {
            bool hasProperty = false;

            if (obj is IDictionary)
            {
                hasProperty = ((IDictionary)obj).Contains(propertyName);
            }
            else
            {
                if (propertyName.IndexOf('.') > -1)
                {
                    StringTokenizer parser = new StringTokenizer(propertyName, ".");
                    IEnumerator enumerator = parser.GetEnumerator();
                    Type type = obj.GetType();

                    while (enumerator.MoveNext())
                    {
                        propertyName = (string)enumerator.Current;
                        type = ReflectionInfo.GetInstance(type).GetGetterType(propertyName);
                        hasProperty = ReflectionInfo.GetInstance(type).HasReadableMember(propertyName);
                    }
                }
                else
                {
                    hasProperty = ReflectionInfo.GetInstance(obj.GetType()).HasReadableMember(propertyName);
                }
            }

            return hasProperty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSimpleType(Type type)
        {
            if (simpleTypeMap.Contains(type))
            {
                return true;
            }
            else if (typeof(ICollection).IsAssignableFrom(type))
            {
                return true;
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return true;
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                return true;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}

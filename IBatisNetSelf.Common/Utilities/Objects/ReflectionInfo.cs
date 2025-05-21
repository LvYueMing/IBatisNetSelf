using IBatisNetSelf.Common.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects
{
    /// <summary>
    /// This class represents a cached set of class definition information that
    /// allows for easy mapping between property names and get/set methods.
    /// </summary>
    public sealed class ReflectionInfo
    {
        /// <summary>
        /// BindingFlags for property
        /// </summary>
        public static BindingFlags BINDING_FLAGS_PROPERTY = BindingFlags.Public
                                                            | BindingFlags.NonPublic
                                                            | BindingFlags.Instance;

        /// <summary>
        /// BindingFlags for field
        /// </summary>
        public static BindingFlags BINDING_FLAGS_FIELD = BindingFlags.Public
                                                         | BindingFlags.NonPublic
                                                         | BindingFlags.Instance;

        private static readonly string[] emptyStringArray = new string[0];
        private static ArrayList simleTypeMap = new ArrayList();
        private static Hashtable reflectionInfoMap = Hashtable.Synchronized(new Hashtable());

        private string className = string.Empty;
        private string[] readableMemberNames = emptyStringArray;
        private string[] writeableMemberNames = emptyStringArray;


        private List<string> readablePropertyList = new List<string>();
        private List<string> writeablePropertyList = new List<string>();
        private string[] readablePropertyNames = emptyStringArray;
        private string[] writeablePropertyNames = emptyStringArray;

        // (memberName, MemberInfo)
        private Hashtable setMembers = new Hashtable();
        // (memberName, MemberInfo)
        private Hashtable getMembers = new Hashtable();
        // (memberName, member type)
        private Hashtable setTypes = new Hashtable();
        // (memberName, member type)
        private Hashtable getTypes = new Hashtable();

        /// <summary>
        /// 
        /// </summary>
        public string ClassName
        {
            get => this.className;
        }

        /// <summary>
        /// 
        /// </summary>
        static ReflectionInfo()
        {
            simleTypeMap.Add(typeof(string));
            simleTypeMap.Add(typeof(byte));
            simleTypeMap.Add(typeof(char));
            simleTypeMap.Add(typeof(bool));
            simleTypeMap.Add(typeof(Guid));
            simleTypeMap.Add(typeof(Int16));
            simleTypeMap.Add(typeof(Int32));
            simleTypeMap.Add(typeof(Int64));
            simleTypeMap.Add(typeof(Single));
            simleTypeMap.Add(typeof(Double));
            simleTypeMap.Add(typeof(Decimal));
            simleTypeMap.Add(typeof(DateTime));
            simleTypeMap.Add(typeof(TimeSpan));
            simleTypeMap.Add(typeof(Hashtable));
            simleTypeMap.Add(typeof(SortedList));
            simleTypeMap.Add(typeof(ListDictionary));
            simleTypeMap.Add(typeof(HybridDictionary));
            simleTypeMap.Add(typeof(ArrayList));
            simleTypeMap.Add(typeof(IEnumerator));

            //			simleTypeMap.Add(Class.class);
            //			simleTypeMap.Add(Collection.class);
            //			simleTypeMap.Add(HashMap.class);
            //			simleTypeMap.Add(TreeMap.class);
            //			simleTypeMap.Add(HashSet.class);
            //			simleTypeMap.Add(TreeSet.class);
            //			simleTypeMap.Add(Vector.class);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aType"></param>
        private ReflectionInfo(Type aType)
        {
            this.className = aType.Name;
            this.AddMembers(aType);

            string[] _getArray = new string[this.getMembers.Count];
            this.getMembers.Keys.CopyTo(_getArray, 0);
            this.readableMemberNames = _getArray;

            string[] _getPropertyArray = new string[this.readablePropertyList.Count];
            this.readablePropertyList.CopyTo(_getPropertyArray,0);
            this.readablePropertyNames = _getPropertyArray;

            string[] _setArray = new string[this.setMembers.Count];
            this.setMembers.Keys.CopyTo(_setArray, 0);
            this.writeableMemberNames = _setArray;

            string[] _setPropertyArray = new string[this.writeablePropertyList.Count];
            this.writeablePropertyList.CopyTo(_setPropertyArray, 0);
            this.writeablePropertyNames = _setPropertyArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aType"></param>
        private void AddMembers(Type aType)
        {
            #region Properties

            PropertyInfo[] _properties = aType.GetProperties(BINDING_FLAGS_PROPERTY);
            for (int i = 0; i < _properties.Length; i++)
            {
                if (_properties[i].CanWrite)
                {
                    string _name = _properties[i].Name;
                    this.setMembers[_name] = _properties[i];
                    this.setTypes[_name] = _properties[i].PropertyType;
                    this.writeablePropertyList.Add(_name);
                }
                if (_properties[i].CanRead)
                {
                    string _name = _properties[i].Name;
                    this.getMembers[_name] = _properties[i];
                    this.getTypes[_name] = _properties[i].PropertyType;
                    this.readablePropertyList.Add(_name);
                }
            }

            #endregion

            #region Fields

            FieldInfo[] _fields = aType.GetFields(BINDING_FLAGS_FIELD);
            for (int i = 0; i < _fields.Length; i++)
            {
                string _name = _fields[i].Name;
                this.setMembers[_name] = _fields[i];
                this.setTypes[_name] = _fields[i].FieldType;
                this.getMembers[_name] = _fields[i];
                this.getTypes[_name] = _fields[i].FieldType;
            }

            #endregion

            // Fix for problem with interfaces inheriting other interfaces
            if (aType.IsInterface)
            {
                // Loop through interfaces for the type and add members from
                // these types too
                foreach (Type interf in aType.GetInterfaces())
                {
                    AddMembers(interf);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aMemberName"></param>
        /// <returns></returns>
        public MemberInfo GetSetter(string aMemberName)
        {
            MemberInfo _memberInfo = this.setMembers[aMemberName] as MemberInfo;

            if (_memberInfo == null)
            {
                throw new ProbeException($"There is no Set member named '{aMemberName}' in class '{this.className}'");
            }

            return _memberInfo;
        }


        /// <summary>
        /// Gets the <see cref="MemberInfo"/>.
        /// </summary>
        /// <param name="aMemberName">Member's name.</param>
        /// <returns>The <see cref="MemberInfo"/></returns>
        public MemberInfo GetGetter(string aMemberName)
        {
            MemberInfo _memberInfo = this.getMembers[aMemberName] as MemberInfo;
            if (_memberInfo == null)
            {
                throw new ProbeException($"There is no Get member named '{aMemberName}' in class '{this.className}'");
            }
            return _memberInfo;
        }


        /// <summary>
        /// Gets the type of the member.
        /// </summary>
        /// <param name="aMemberName">Member's name.</param>
        /// <returns></returns>
        public Type GetSetterType(string aMemberName)
        {
            Type _type = this.setTypes[aMemberName] as Type;
            if (_type == null)
            {
                throw new ProbeException($"There is no Set member named '{aMemberName}' in class '{this.className}'");
            }
            return _type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aMemberName"></param>
        /// <returns></returns>
        public Type GetGetterType(string aMemberName)
        {
            Type _type = this.getTypes[aMemberName] as Type;
            if (_type == null)
            {
                throw new ProbeException($"There is no Get member named '{aMemberName}' in class '{this.className}'");
            }
            return _type;
        }


        public string[] GetReadableMemberNames()
        {
            return this.readableMemberNames;
        }

        public string[] GetReadablePropertyNames()
        {
            return this.readablePropertyNames;
        }


        public string[] GetWriteableMemberNames()
        {
            return this.writeableMemberNames;
        }

        public string[] GetWriteablePropertyNames()
        {
            return this.writeablePropertyNames;
        }

        public bool HasWritableMember(string memberName)
        {
            return this.setMembers.ContainsKey(memberName);
        }


        public bool HasReadableMember(string memberName)
        {
            return this.getMembers.ContainsKey(memberName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsKnownType(Type type)
        {
            if (simleTypeMap.Contains(type))
            {
                return true;
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                return true;
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return true;
            }
            else if (typeof(IEnumerator).IsAssignableFrom(type))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets an instance of ReflectionInfo for the specified type.
        /// </summary>summary>
        /// <param name="aType">The type for which to lookup the method cache.</param>
        /// <returns>The properties cache for the type</returns>
        public static ReflectionInfo GetInstance(Type aType)
        {
            lock (aType)
            {
                ReflectionInfo _cache = reflectionInfoMap[aType] as ReflectionInfo;
                if (_cache == null)
                {
                    _cache = new ReflectionInfo(aType);
                    reflectionInfoMap.Add(aType, _cache);
                }
                return _cache;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helper
{
    public static class DBAttributeExtend
    {
        public static string GetMappingName<T>(this T t) where T : MemberInfo
        {
            if (t.IsDefined(typeof(BaseMappingAttribute), true))
            {
                BaseMappingAttribute attribute = (BaseMappingAttribute)t.GetCustomAttribute(typeof(BaseMappingAttribute), true);
                return attribute.GetName();
            }
            else
            {
                return t.Name;
            }
        }

        public static IEnumerable<PropertyInfo> GetPropertiesWithoutKey(this Type type)
        {
            return type.GetProperties().Where(p => !p.IsDefined(typeof(KeyAttribute), true));
        }

        /// <summary>
        /// 获取主键
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MemberInfo GetPropertiesKey(this Type type)
        {
            return type.GetProperties().Where(c => c.IsDefined(typeof(KeyAttribute), true)).FirstOrDefault();
        }
    }
}

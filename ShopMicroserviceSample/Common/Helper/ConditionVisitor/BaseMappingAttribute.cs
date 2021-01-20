using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helper
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class BaseMappingAttribute : Attribute
    {
        private string _Name = null;
        public BaseMappingAttribute(string name)
        {
            this._Name = name;
        }

        public virtual string GetName()
        {
            return this._Name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
        public KeyAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : BaseMappingAttribute
    {
        public TableAttribute(string name) : base(name)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : BaseMappingAttribute
    {
        public ColumnAttribute(string name) : base(name)
        {
        }
    }
}

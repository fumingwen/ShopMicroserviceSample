using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    /// <summary>
    /// 系统通用Name Value字典
    /// </summary>
    /// <typeparam name="Tn"></typeparam>
    /// <typeparam name="Tv"></typeparam>
    public class ItemValue<Tn, Tv>
    {
        public Tn Name { get; set; }

        public Tv Value { get; set; }
    }
}

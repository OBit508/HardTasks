using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardTasks
{
    internal class ChangeableValue<ValueT>
    {
        public ChangeableValue(ValueT value)
        {
            Value = value;
        }
        public ValueT Value;
    }
}

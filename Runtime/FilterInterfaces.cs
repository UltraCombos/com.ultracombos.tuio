using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUIO;

namespace UnityUtils.TUIO
{
    public interface ITuioHandler
    {
    }

    public interface ITuioFilterHandler
    {
        void Filter(TuioContainer tcur);
    }
}

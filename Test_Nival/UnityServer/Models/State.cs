using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnityServer.Models
{
    [Serializable]
    public class State
    {

        public int FieldSize;
        public int NumberOfPlayer;

        public List<Unit> Units = new List<Unit>();

    }
}
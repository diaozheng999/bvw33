using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PGT.Core
{
    public class Base : IPrivateEventDispatchable
    {
        Guid guid = Guid.Empty;

        public string instanceId()
        {
            if (guid == Guid.Empty) guid = Guid.NewGuid();
            return guid.ToString();
        }
    }
}
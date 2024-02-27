﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace apptab
{
    public class TxLISTETRAIT
    {
        public Guid? No { get; set; }
        public string REF { get; set; }
        public string BENEF { get; set; }
        public string MONTENGAGEMENT { get; set; }
        public DateTime? DATENGAGEMENT { get; set; }
        public string MONTPAIE { get; set; }
        public DateTime? DATEPAIE { get; set; }
        public string SOA { get; set; }
        public string PROJET { get; set; }
        public bool? isLATE { get; set; }
    }
}

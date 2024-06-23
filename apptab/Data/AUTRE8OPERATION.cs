// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apptab.Data
{
    public class AUTRE8OPERATION
    {
        public string NUMEROOPERATION {  get; set; }
        public string LIBELLE { get; set; }
        public DateTime? DATEOPERATION {  get; set; }
        public decimal? MONTANTLOCAL {  get; set; }
        public decimal? MONTANTRAPPORT {  get; set; }
        public decimal? MONTANTDEVISE {  get; set; }
        public string JOURNAL {  get; set; }
        public string NORDPAIEMENT {  get; set; }
        public string FINANCEMENT {  get; set; }
        public string MARCHER {  get; set; }
        public string ACTIVITER {  get; set; }
        public string CATEGORIE {  get; set; }
        public string TYPE {  get; set; }
        public string NUMEROLIQUIDATION {  get; set; }
        public string NUMEROREG{  get; set; }
        public bool AUTREOP{  get; set; }
        public string SITE {  get; set; }
        public DateTime? DATEMAJ {  get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Jarvis_Phase3
{
    using System;
    using System.Collections.Generic;
    
    public partial class Detail
    {
        public int accountID { get; set; }
        public Nullable<int> familysize { get; set; }
        public Nullable<int> children { get; set; }
        public Nullable<int> adults { get; set; }
        public Nullable<int> rooms { get; set; }
        public string address { get; set; }
    
        public virtual Account Account { get; set; }
    }
}

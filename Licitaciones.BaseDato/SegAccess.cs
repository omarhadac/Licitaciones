//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Licitaciones.BaseDato
{
    using System;
    using System.Collections.Generic;
    
    public partial class SegAccess
    {
        public int Id { get; set; }
        public short Type_Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Data { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public short Posicion { get; set; }
        public Nullable<int> Parent_Access_Id { get; set; }
        public int IsDelete { get; set; }
    }
}

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
    
    public partial class SegUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public Nullable<System.DateTime> LastLogin { get; set; }
        public string Email { get; set; }
        public short SessionOpen { get; set; }
        public Nullable<long> State_Type_Id { get; set; }
        public short FailLoginCount { get; set; }
        public byte[] Photo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<int> SegOffice_Id { get; set; }
        public Nullable<int> SegVisualiza_Id { get; set; }
        public int InfoVersion { get; set; }
        public string Company { get; set; }
        public string numeroContacto { get; set; }
        public string motivoRegistro { get; set; }
        public string tokenSecurity { get; set; }
    }
}
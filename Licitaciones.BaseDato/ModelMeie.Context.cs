﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class db_meieEntities : DbContext
    {
        public db_meieEntities()
            : base("name=db_meieEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<GrlType> GrlTypes { get; set; }
        public virtual DbSet<GrlTypeConfig> GrlTypeConfigs { get; set; }
        public virtual DbSet<PryLicitacion> PryLicitacions { get; set; }
        public virtual DbSet<PryProyecto> PryProyectoes { get; set; }
        public virtual DbSet<SegAccess> SegAccesses { get; set; }
        public virtual DbSet<SegOffice> SegOffices { get; set; }
        public virtual DbSet<SegPassword> SegPasswords { get; set; }
        public virtual DbSet<SegProfile> SegProfiles { get; set; }
        public virtual DbSet<SegProfileAccess> SegProfileAccesses { get; set; }
        public virtual DbSet<SegTitulo> SegTituloes { get; set; }
        public virtual DbSet<SegUser> SegUsers { get; set; }
        public virtual DbSet<SegUserProfile> SegUserProfiles { get; set; }
        public virtual DbSet<SegUserTitulo> SegUserTituloes { get; set; }
        public virtual DbSet<GrlDepartament> GrlDepartaments { get; set; }
        public virtual DbSet<PryEmpresa> PryEmpresas { get; set; }
        public virtual DbSet<PryEspecialidad> PryEspecialidads { get; set; }
        public virtual DbSet<PryEstadoEtapa> PryEstadoEtapas { get; set; }
        public virtual DbSet<PryEtapa> PryEtapas { get; set; }
        public virtual DbSet<PryFuentesFinanciamiento> PryFuentesFinanciamientoes { get; set; }
        public virtual DbSet<PryOrganismosEjecutore> PryOrganismosEjecutores { get; set; }
        public virtual DbSet<PryProyectoCategoria> PryProyectoCategorias { get; set; }
        public virtual DbSet<PryProyectoDimension> PryProyectoDimensions { get; set; }
        public virtual DbSet<PryProyectoEmpresa> PryProyectoEmpresas { get; set; }
        public virtual DbSet<PryProyectoEspecialidad> PryProyectoEspecialidads { get; set; }
        public virtual DbSet<PryProyectoEstado> PryProyectoEstadoes { get; set; }
        public virtual DbSet<PryProyectoEtapa> PryProyectoEtapas { get; set; }
        public virtual DbSet<PryProyectoExpediente> PryProyectoExpedientes { get; set; }
        public virtual DbSet<PryProyectoFuente> PryProyectoFuentes { get; set; }
        public virtual DbSet<PryProyectoMunicipio> PryProyectoMunicipios { get; set; }
        public virtual DbSet<PryProyectoTipoCertificacion> PryProyectoTipoCertificacions { get; set; }
        public virtual DbSet<PryTipoNroExpediente> PryTipoNroExpedientes { get; set; }
        public virtual DbSet<PryProyectoPlanificacion> PryProyectoPlanificacion { get; set; }
        public virtual DbSet<LicEmpresaObra> LicEmpresaObra { get; set; }
        public virtual DbSet<LicDocGeneral> LicDocGeneral { get; set; }
        public virtual DbSet<LicDocObra> LicDocObra { get; set; }
        public virtual DbSet<LicProyecto> LicProyecto { get; set; }
        public virtual DbSet<LicArchivoEmpresa> LicArchivoEmpresa { get; set; }
        public virtual DbSet<LicArchivoEstado> LicArchivoEstado { get; set; }
        public virtual DbSet<LicOferta> LicOferta { get; set; }
        public virtual DbSet<LicOfertaEstado> LicOfertaEstado { get; set; }
        public virtual DbSet<LicProyectoFecha> LicProyectoFecha { get; set; }
        public virtual DbSet<UfiOrgano> UfiOrgano { get; set; }
        public virtual DbSet<LicActaEstado> LicActaEstado { get; set; }
        public virtual DbSet<LicActaGenerica> LicActaGenerica { get; set; }
        public virtual DbSet<LicActaObra> LicActaObra { get; set; }
        public virtual DbSet<PryProyectoSubEspecialidad> PryProyectoSubEspecialidad { get; set; }
        public virtual DbSet<PrySubEspecialidad> PrySubEspecialidad { get; set; }
        public virtual DbSet<LicArchivoCategoria> LicArchivoCategoria { get; set; }
        public virtual DbSet<LicArchivoObra> LicArchivoObra { get; set; }
        public virtual DbSet<LicOfertaEmpresa> LicOfertaEmpresa { get; set; }
        public virtual DbSet<LicEmail> LicEmail { get; set; }
    }
}
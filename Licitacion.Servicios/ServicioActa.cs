using Licitaciones.BaseDato;
using Licitaciones.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licitacion.Servicios
{
    public class ServicioActa
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string ruta = System.Web.HttpRuntime.AppDomainAppPath + "\\bin\\log4net.config";

        public ActaViewModels buscarActaGenerica(int? idActa, int? idObra)
        {
            ActaViewModels unActa = new ActaViewModels();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Buscar Acta Generica");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicActaGenerica
                        .Where(x => x.idLicActaGenerica == idActa)
                        .FirstOrDefault();
                    if (existe != null)
                    {
                        unActa.textoEncabezado = existe.actaEncabezado;
                    }
                    var unObra = (from lic in db.PryProyectoes
                                  join edo in db.PryEstadoEtapas
                                  on lic.GrlTypeState_Id equals edo.Id
                                  join pyl in db.PryLicitacions
                                  on lic.Id equals pyl.IdPryProyecto
                                      into pylo
                                  from pyj in pylo.DefaultIfEmpty()
                                  where lic.Id == idObra
                                  select new licitacionGrillaViewModels
                                  {
                                      nombreObra = lic.Nombre,
                                      idObra = lic.Id,
                                      montoObra = pyj.MontoOficial,
                                      nroExpediente = lic.Expediente,
                                      nroLicitacion = lic.numeroLicitacion,
                                      plazo = lic.Plazo
                                  }).FirstOrDefault();

                    if (unObra != null)
                    {
                        unActa.nombreObra = string.IsNullOrEmpty(unObra.nombreObra) ? "" : unObra.nombreObra;
                        unActa.nroExpediente = string.IsNullOrEmpty(unObra.nroExpediente) ? "" : unObra.nroExpediente;
                        unActa.nroLicitacion = unObra.nroLicitacion;
                        unActa.plazoEjecucion = unObra.plazo + " Mes/es ";
                        unActa.montoObra = unObra.montoObra;

                        unActa.textoEncabezado = unActa.textoEncabezado.Replace("#plazo", unActa.plazoEjecucion);
                        unActa.textoEncabezado = unActa.textoEncabezado.Replace("#nombreObra", unActa.nombreObra);
                        unActa.textoEncabezado = unActa.textoEncabezado.Replace("#nroLicitacion", unActa.nroLicitacion);
                        unActa.textoEncabezado = unActa.textoEncabezado.Replace("#nroExpediente", unActa.nroExpediente);
                        unActa.textoEncabezado = unActa.textoEncabezado.Replace("#montoPresupuesto ", unActa.montoObraString);
                    }

                }
            }
            catch (Exception ex)
            {
                log.Error("Error buscar Acta Generica " + ex.Message);
            }
            return unActa;
        }
        public ActaViewModels buscarActaFormal(int? idActa, int? idObra)
        {
            ActaViewModels unActa = new ActaViewModels();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Buscar Acta Generica");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    string fecha = DateTime.Now.ToLongDateString();
                    var existe = db.LicActaGenerica
                        .Where(x => x.idLicActaGenerica == idActa)
                        .FirstOrDefault();
                    if (existe != null)
                    {
                        unActa.textoFormal = existe.actaFormal;
                        unActa.textoOferta = existe.actaOferta;
                        unActa.textoCierre = existe.actaCierre;
                    }
                    var unObra = (from lic in db.LicProyecto
                                  where lic.idProyecto == idObra
                                  select new ActaFormalViewModels
                                  {
                                      domicilioApertura = lic.domicilioApertura
                                  }).FirstOrDefault();

                    if (unObra != null)
                    {
                        unActa.textoFormal = unActa.textoFormal.Replace("#fechaActa", fecha);
                        unActa.textoFormal = unActa.textoFormal.Replace("#domicilioApertura", unObra.domicilioApertura);
                    }
                    unActa.textoCierre = unActa.textoCierre + " " + DateTime.Now.Hour + " Horas " + DateTime.Now.Minute + " Minutos";
                    var textoOferta = buscarOfertas(idObra);
                    unActa.textoOferta = unActa.textoOferta + textoOferta;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error buscar Acta Generica " + ex.Message);
            }
            return unActa;
        }
        private string buscarOfertas(int? idObra)
        {
            string value = "";
            ActaViewModels unActa = new ActaViewModels();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var listaOferta = db.LicOfertaEmpresa.Where(x => x.idObra == idObra).ToList();
                    int nroOferta = 1;
                    foreach (var item in listaOferta)
                    {
                        using (DB_RACOPEntities db1 = new DB_RACOPEntities())
                        {
                            var unEmpresa = db1.rc_Empresa.Where(x => x.idEmpresa == item.idEmpresa).FirstOrDefault();
                            if (unEmpresa != null)
                            {
                                value = value + "<b>Oferta Nro " + nroOferta + ": " + unEmpresa.razonSocial + "</b></br>";
                                var listaRequisitos1 = db.LicDocObra.Where(x => x.idObra == idObra && x.nroSobre == 1).ToList();
                                value = value + "Sobre N° 1: ";
                                value = value + "</br>";
                                foreach (var unSobre1 in listaRequisitos1)
                                {
                                    var aprobado = false;
                                    var archivoObservado = "";
                                    var unArchivo = db.LicArchivoEmpresa
                                        .Where(x => x.idRequisito == unSobre1.idLicDocObra && x.idObra == idObra && x.idEmpresa == unEmpresa.idEmpresa)
                                        .FirstOrDefault();
                                    if (unArchivo != null)
                                    {
                                        var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre1.idDocumentacion).FirstOrDefault();
                                        if (unReq != null)
                                        {
                                            if (unArchivo.idEstadoArchivo == 1)
                                            {
                                                archivoObservado = archivoObservado + unReq.nombre + " <b>Presentado </b>" + "</br>";
                                            }
                                            else
                                            {
                                                if (unArchivo.idEstadoArchivo == 5)
                                                {
                                                    archivoObservado = archivoObservado + unReq.nombre + " <b>Rechazado:</b> " + unArchivo.observaciones + "</br>";
                                                }
                                                else
                                                {
                                                    archivoObservado = archivoObservado + unReq.nombre + " <b>Observación: </b>" + unArchivo.observaciones + "</br>";
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre1.idDocumentacion).FirstOrDefault();
                                        if (unReq != null)
                                        {
                                            archivoObservado = archivoObservado + unReq.nombre + " <b>No ha sido presentado</b>" + ". ";
                                        }
                                    }
                                    value = value + archivoObservado;
                                }

                                var listaRequisitos2 = db.LicDocObra.Where(x => x.idObra == idObra && x.nroSobre == 2).ToList();
                                value = value + "</br>Sobre N° 2: ";
                                value = value + "</br>";
                                foreach (var unSobre2 in listaRequisitos2)
                                {
                                    var archivoObservado = "";
                                    var unArchivo = db.LicArchivoEmpresa
                                        .Where(x => x.idRequisito == unSobre2.idLicDocObra && x.idObra == idObra && x.idEmpresa == unEmpresa.idEmpresa)
                                        .FirstOrDefault();
                                    if (unArchivo != null)
                                    {
                                        var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre2.idDocumentacion).FirstOrDefault();
                                        if (unReq != null)
                                        {
                                            if (unArchivo.idEstadoArchivo == 1)
                                            {
                                                archivoObservado = archivoObservado + unReq.nombre + " <b>Presentado </b>" + "</br>";
                                            }
                                            else
                                            {
                                                if (unArchivo.idEstadoArchivo == 5)
                                                {
                                                    archivoObservado = archivoObservado + unReq.nombre + " <b>Rechazado:</b> " + unArchivo.observaciones + "</br>";
                                                }
                                                else
                                                {
                                                    archivoObservado = archivoObservado + unReq.nombre + " <b>Observación: </b>" + unArchivo.observaciones + "</br>";
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre2.idDocumentacion).FirstOrDefault();
                                        if (unReq != null)
                                        {
                                            archivoObservado = archivoObservado + unReq.nombre + " <b>No ha sido presentado</b>" + ". ";
                                        }
                                    }
                                    value = value + archivoObservado;
                                }

                                var listaRequisitos3 = db.LicDocObra.Where(x => x.idObra == idObra && x.nroSobre == 3).ToList();
                                value = value + "</br>Sobre N° 3: ";
                                value = value + "</br>";
                                foreach (var unSobre3 in listaRequisitos3)
                                {
                                    var aprobado = false;
                                    var archivoObservado = "";
                                    var unArchivo = db.LicArchivoEmpresa
                                        .Where(x => x.idRequisito == unSobre3.idLicDocObra && x.idObra == idObra && x.idEmpresa == unEmpresa.idEmpresa)
                                        .FirstOrDefault();
                                    if (unArchivo != null)
                                    {
                                        var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre3.idDocumentacion).FirstOrDefault();
                                        if (unReq != null)
                                        {
                                            if (unArchivo.idEstadoArchivo == 1)
                                            {
                                                archivoObservado = archivoObservado + unReq.nombre + " <b>Presentado </b>" + "</br>";
                                            }
                                            else
                                            {
                                                if (unArchivo.idEstadoArchivo == 5)
                                                {
                                                    archivoObservado = archivoObservado + unReq.nombre + " <b>Rechazado:</b> " + unArchivo.observaciones + "</br>";
                                                }
                                                else
                                                {
                                                    archivoObservado = archivoObservado + unReq.nombre + " <b>Observación: </b>" + unArchivo.observaciones + "</br>";
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre3.idDocumentacion).FirstOrDefault();
                                        if (unReq != null)
                                        {
                                            archivoObservado = archivoObservado + unReq.nombre + " <b>No ha sido presentado</b>" + ". ";
                                        }
                                    }

                                    value = value + archivoObservado + "</br>";
                                }
                            }
                            nroOferta = nroOferta + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return value;
        }
        public int grabarActaObra(ActaViewModels unActa)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Grabar Acta Obra");
            int value = 0;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existeActa = db.LicActaObra.Where(x => x.idObra == unActa.idObra).FirstOrDefault();
                    if (existeActa != null)
                    {
                        existeActa.fechaModif = DateTime.Now;
                        existeActa.actaCierre = unActa.textoCierre;
                        existeActa.actaEncabezado = unActa.textoEncabezado;
                        existeActa.actaFormal = unActa.textoFormal;
                        existeActa.actaOferta = unActa.textoOferta;
                        db.SaveChanges();
                        value = existeActa.idLicActaObra;
                    }
                    else
                    {
                        LicActaObra actaObra = new LicActaObra();
                        actaObra.actaCierre = unActa.textoCierre;
                        actaObra.actaEncabezado = unActa.textoEncabezado;
                        actaObra.actaFormal = unActa.textoFormal;
                        actaObra.actaOferta = unActa.textoOferta;
                        actaObra.fechaAlta = DateTime.Now;
                        actaObra.idObra = unActa.idObra;
                        db.LicActaObra.Add(actaObra);
                        db.SaveChanges();
                        value = actaObra.idLicActaObra;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error grabar Acta Generica " + ex.Message);
            }
            return value;
        }
        public ActaViewModels buscarActaObra(int? idObra)
        {
            ActaViewModels unActa = new ActaViewModels();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Buscar Acta Obra");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicActaObra
                        .Where(x => x.idObra == idObra)
                        .FirstOrDefault();
                    if (existe != null)
                    {
                        unActa.textoEncabezado = existe.actaEncabezado;
                        unActa.idObra = existe.idObra;
                        unActa.textoCierre = existe.actaCierre;
                        unActa.textoEncabezado = existe.actaEncabezado;
                        unActa.textoFormal = existe.actaFormal;
                        unActa.textoOferta = existe.actaOferta;
                    }

                }
            }
            catch (Exception ex)
            {
                log.Error("Error buscar Acta Obra " + ex.Message);
            }
            return unActa;
        }
        public void grabarArchivo(int? idObra, int? idCategoria, string nombreArchivo, string ruta)
        {
            ActaViewModels unActa = new ActaViewModels();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Grabar Archivo Acta Obra");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicArchivoObra.Where(x => x.idCategoria == idCategoria && x.idObra == idObra).FirstOrDefault();
                    if (existe != null)
                    {
                        existe.nombreArchivo = nombreArchivo;
                        existe.rutaArchivo = ruta;
                        db.SaveChanges();
                    }
                    else
                    {
                        LicArchivoObra archivoObra = new LicArchivoObra();
                        archivoObra.idObra = idObra;
                        archivoObra.idCategoria = idCategoria;
                        archivoObra.nombreArchivo = nombreArchivo;
                        archivoObra.rutaArchivo = ruta;
                        db.LicArchivoObra.Add(archivoObra);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error grabar Archivo Acta " + ex.Message);
            }
        }
        public ArchivoViewModels buscarArchivo(int? idObra, int? idCategoria)
        {
            ArchivoViewModels value = new ArchivoViewModels();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var unArchivo = db.LicArchivoObra.Where(x => x.idCategoria == idCategoria && x.idObra == idObra).FirstOrDefault();
                    if (unArchivo != null)
                    {
                        value.ruta = unArchivo.rutaArchivo;
                        value.nombre = unArchivo.nombreArchivo;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return value; ;
        }
        public int buscarTieneActa(int? idObra)
        {
            var value = 0;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicActaObra.Where(x => x.idObra == idObra).FirstOrDefault();
                    if (existe != null)
                    {
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return value;
        }
    }
}

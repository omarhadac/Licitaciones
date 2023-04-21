
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
    public class ServicioEmpresa
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string ruta = System.Web.HttpRuntime.AppDomainAppPath + "\\bin\\log4net.config";
        public List<licitacionGrillaViewModels> listaLicitaciones
        (ref int recordsTotal, string sortColumn, string sortColumnDir, string searchValue,
            string nroExpediente, string nombreObra, string qObra, int? idEmpresa, int? idFavorito, DateTime? fechaPub, int? idOrganismo,
            int pageSize = 0, int skip = 0)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Grilla Licitaciones Empresa");
            List<licitacionGrillaViewModels> lista = new List<licitacionGrillaViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = from lic in db.PryProyectoes
                                join edo in db.PryEstadoEtapas
                                on lic.GrlTypeState_Id equals edo.Id
                                join pyl in db.PryLicitacions
                                on lic.Id equals pyl.IdPryProyecto
                                    into pylo
                                from pyj in pylo.DefaultIfEmpty()
                                join tcont in db.GrlTypes 
                                 on pyj.TipoDeContratacion_Id equals tcont.Id 
                                   into tconto 
                                   from tcontj in tconto.DefaultIfEmpty()
                                join org in db.PryOrganismosEjecutores
                                on lic.PryOrganismoEjecutor_Id equals org.Id
                                    into orgo
                                from orgj in orgo.DefaultIfEmpty()
                                join empO in db.LicEmpresaObra.Where(x=>x.idEmpresa == idEmpresa)
                                on lic.Id equals empO.idObra
                                    into empOo
                                from empOj in empOo.DefaultIfEmpty()
                                join ofe in db.LicOfertaEmpresa.Where(x => x.idEmpresa == idEmpresa)
                                on lic.Id equals ofe.idObra
                                    into ofeo
                                from ofej in ofeo.DefaultIfEmpty()
                                where edo.PryStage_Id == 49 && lic.Eliminado == false && edo.Id < 9 && edo.Id > 5
                                select new licitacionGrillaViewModels
                                {
                                    nombreEtapa = edo.Name,
                                    idEtapa = edo.Id,
                                    nombreObra = lic.Nombre,
                                    idObra = lic.Id,
                                    fechaApertura = pyj.FechaApertura,
                                    fechaPublicacion = pyj.FechaPublicacionDesde,
                                    montoObra = pyj.MontoOficial,
                                    nroExpediente = lic.Expediente,
                                    nombreOrganismo = orgj.NombreOrganismo,
                                    idOrganismo = lic.PryOrganismoEjecutor_Id,
                                    idContratacion = pyj.TipoDeContratacion_Id,
                                    nombreContratacion = tcontj.Name,
                                    idFavorito = empOj.esFavorito,
                                    fechaOferta=ofej.fecha,
                                };

                    if (!string.IsNullOrEmpty(qObra))
                    {
                        if (qObra != "*")
                        {
                            var array = qObra.Split(',');
                            List<int?> listaObra = new List<int?>();
                            foreach(var item in array)
                            {
                                listaObra.Add(Convert.ToInt32(item));
                            }
                            query = query.Where(x => listaObra.Contains(x.idObra));
                        }
                    }
                    if (idOrganismo != 0)
                    {
                        query = query.Where(x => x.idOrganismo == idOrganismo);
                    }
                    if (idFavorito != 0)
                    {
                        query = query.Where(x => x.idFavorito == idFavorito);
                    }
                    if (!string.IsNullOrEmpty(nombreObra))
                    {
                        query = query.Where(x => x.nombreObra.Contains(nombreObra));
                    }
                    if (!string.IsNullOrEmpty(nroExpediente))
                    {
                        query = query.Where(x => x.nroExpediente.Contains(nroExpediente));
                    }
                    //if ((monto != null)&&(monto != 0))
                    //{
                    //    query = query.Where(x => x.montoObra == monto);
                    //}
                    if (fechaPub.HasValue)
                    {
                        query = query.Where(x => x.fechaPublicacion == fechaPub);
                    }
                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                    {
                        if ((sortColumn == "nombreObra") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nombreObra);
                        }
                        if ((sortColumn == "nombreObra") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nombreObra);
                        }
                        if ((sortColumn == "nroExpedienteString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nroExpediente);
                        }
                        if ((sortColumn == "nroExpedienteString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nroExpediente);
                        }
                        if ((sortColumn == "fechaPublicacionString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.fechaPublicacion);
                        }
                        if ((sortColumn == "fechaPublicacionString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.fechaPublicacion);
                        }
                        if ((sortColumn == "fechaAperturaString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.fechaApertura);
                        }
                        if ((sortColumn == "fechaAperturaString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.fechaApertura);
                        }
                        if ((sortColumn == "nombreOrganismoString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nombreOrganismo);
                        }
                        if ((sortColumn == "nombreOrganismoString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nombreOrganismo);
                        }
                        if ((sortColumn == "montoObraString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.montoObra);
                        }
                        if ((sortColumn == "montoObraString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.montoObra);
                        }
                        if ((sortColumn == "nombreEtapa") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nombreEtapa);
                        }
                        if ((sortColumn == "nombreEtapa") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nombreEtapa);
                        }
                        if ((sortColumn == "accion") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nombreObra);
                        }
                        if ((sortColumn == "accion") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nombreObra);
                        }
                        if ((sortColumn == "estadoOferta") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.fechaOferta);
                        }
                        if ((sortColumn == "estadoOferta") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.fechaOferta);
                        }
                    }
                    else
                    {
                        query = query = query.OrderBy(x => x.nombreObra);
                    }

                    query = query.Take(500);
                    recordsTotal = query.Count();

                    var lst = query.Skip(skip).Take(pageSize);
                    lista = lst.ToList();
                    return lista;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en Grilla Licitaciones Empresa", ex);
            }
            return lista;
        }
        public unLicitacionViewModels buscarUna(int? idObra)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Una Licitacion");
            unLicitacionViewModels unLicitacion = new unLicitacionViewModels();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = from lic in db.PryProyectoes
                                join edo in db.PryEstadoEtapas
                                on lic.GrlTypeState_Id equals edo.Id
                                join pyl in db.PryLicitacions
                                on lic.Id equals pyl.IdPryProyecto
                                    into pylo
                                from pyj in pylo.DefaultIfEmpty()
                                join org in db.PryOrganismosEjecutores
                                on lic.PryOrganismoEjecutor_Id equals org.Id
                                    into orgo
                                from orgj in orgo.DefaultIfEmpty()
                                join licpry in db.LicProyecto
                                on lic.Id equals licpry.idProyecto
                                    into licpryo
                                from licpryj in licpryo.DefaultIfEmpty()
                                where lic.Id == idObra && lic.Eliminado == false
                                select new unLicitacionViewModels
                                {
                                    nombreEtapa = edo.Name,
                                    idEtapa = edo.Id,
                                    nombreObra = lic.Nombre,
                                    idObra = lic.Id,
                                    fechaApertura = pyj.FechaApertura,
                                    fechaPublicacion = pyj.FechaPublicacionDesde,
                                    montoObra = pyj.MontoOficial,
                                    nroExpediente = lic.Expediente,
                                    nombreOrganismo = orgj.NombreOrganismo,
                                    idOrganismo = lic.PryOrganismoEjecutor_Id,
                                    caratula = lic.CaratulaExpediente,
                                    descripcion = lic.descripcion,
                                    plazo = pyj.Plazo,
                                    latitud = lic.Latitud,
                                    longitud = lic.Longitud,
                                    domicilio = lic.Dirección,
                                    valorPliego = pyj.ValorPliego,
                                    horaApertura = pyj.Hora,
                                    idTipoContratacion = pyj.TipoDeContratacion_Id,
                                    idMoneda = licpryj.moneda,
                                    domicilioApertura = licpryj.domicilioApertura,
                                    domicilioPresentacion = licpryj.domicilioPresentacion,
                                    urlPliego = licpryj.urlPliego,
                                    mailConsulta=licpryj.mailConsulta,
                                    fechaCierre = licpryj.fechaCierre,
                                    lugarVisita = licpryj.lugarVisita,
                                    fechaVisita = licpryj.fechaVisita,
                                    fechaConsulta = licpryj.limiteConsulta
                                };
                    unLicitacion = query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                log.Error("Error grabar Buscar Una Licitacion Empresa", ex);
            }
            return unLicitacion;
        }    
        public async Task<int> grabarDetalleLicitacion(unLicitacionViewModels unLicitacion)
        {
            int value = 0;
            unLicitacion = validarDetalleLicitacion(unLicitacion);
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicProyecto.Where(x => x.idProyecto == unLicitacion.idObra).FirstOrDefault();
                    
                    if(existe != null)
                    {
                        existe.domicilioApertura = unLicitacion.domicilioApertura;
                        existe.domicilioPresentacion = unLicitacion.domicilioPresentacion;
                        existe.limiteConsulta = unLicitacion.fechaConsulta;
                        existe.idProyecto = unLicitacion.idObra.Value;
                        existe.mailConsulta = unLicitacion.mailConsulta;
                        existe.urlPliego = unLicitacion.urlPliego;
                        existe.moneda = unLicitacion.idMoneda;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        LicProyecto licProyecto = new LicProyecto();
                        licProyecto.domicilioApertura = unLicitacion.domicilioApertura;
                        licProyecto.domicilioPresentacion = unLicitacion.domicilioPresentacion;
                        licProyecto.limiteConsulta = unLicitacion.fechaConsulta;
                        licProyecto.idProyecto = unLicitacion.idObra.Value;
                        licProyecto.mailConsulta = unLicitacion.mailConsulta;
                        licProyecto.urlPliego = unLicitacion.urlPliego;
                        licProyecto.moneda = unLicitacion.idMoneda;
                        db.LicProyecto.Add(licProyecto);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch(Exception ex)
            {
                log.Error("Error grabar Detalle Licitacion", ex);
            }
            return value;
        }    
        private unLicitacionViewModels validarDetalleLicitacion(unLicitacionViewModels unLicitacion)
        {
            try
            {
                if (string.IsNullOrEmpty(unLicitacion.domicilioApertura))
                {
                    unLicitacion.domicilioApertura = string.Empty;
                }
                if (string.IsNullOrEmpty(unLicitacion.domicilioPresentacion))
                {
                    unLicitacion.domicilioPresentacion = string.Empty;
                }
                if (string.IsNullOrEmpty(unLicitacion.mailConsulta))
                {
                    unLicitacion.mailConsulta = string.Empty;
                }
            }
            catch(Exception ex)
            {
                log.Error("Error validar Detalle Licitacion", ex);
            }
            return unLicitacion;
        }
        public async Task<int> grabarDetalleProyecto(unLicitacionViewModels unLicitacion)
        {
            int value = 0;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.PryProyectoes.Where(x => x.Id == unLicitacion.idObra).FirstOrDefault();

                    if (existe != null)
                    {
                        existe.GrlTypeState_Id = unLicitacion.idEtapa;
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error grabar Detalle Proyecto", ex);
            }
            return value;
        }
        public async Task<int> grabarLicitacion(unLicitacionViewModels unLicitacion)
        {
            int value = 0;
            unLicitacion = validarDetalleLicitacion(unLicitacion);
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.PryLicitacions.Where(x => x.IdPryProyecto == unLicitacion.idObra).FirstOrDefault();

                    if (existe != null)
                    {
                        existe.MontoOficial = unLicitacion.montoObra;
                        existe.FechaApertura = unLicitacion.fechaApertura;
                        existe.Hora = unLicitacion.horaApertura;
                        existe.Plazo = unLicitacion.plazo;
                        existe.ValorPliego = unLicitacion.valorPliego;
                        existe.FechaPublicacionDesde = unLicitacion.fechaPublicacion;
                        existe.TipoDeContratacion_Id = unLicitacion.idTipoContratacion;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        PryLicitacion pry = new PryLicitacion();
                        pry.MontoOficial = unLicitacion.montoObra;
                        pry.FechaApertura = unLicitacion.fechaApertura;
                        pry.Hora = unLicitacion.horaApertura;
                        pry.FechaPublicacionDesde = unLicitacion.fechaPublicacion;
                        pry.TipoDeContratacion_Id = unLicitacion.idTipoContratacion;
                        pry.Plazo = unLicitacion.plazo;
                        pry.ValorPliego = unLicitacion.valorPliego;
                        db.PryLicitacions.Add(pry);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error grabar Licitacion", ex);
            }
            return value;
        }
        public async Task<int> marcarFavoritas(obraFavoritaViewModels favorito)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Marcar Favorita");
            int value = 1;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var unFavorito = db.LicEmpresaObra
                        .Where(x => x.idEmpresa == favorito.idEmpresa && x.idObra == favorito.idObra)
                        .FirstOrDefault();
                    if(unFavorito != null)
                    {
                        if(unFavorito.esFavorito == 0)
                        {
                            unFavorito.esFavorito = 1;
                        }
                        else
                        {
                            unFavorito.esFavorito = 0;
                        }
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        LicEmpresaObra licEmpresa = new LicEmpresaObra();
                        licEmpresa.esFavorito = 1;
                        licEmpresa.idObra = favorito.idObra.Value;
                        licEmpresa.idEmpresa = favorito.idEmpresa;
                        db.LicEmpresaObra.Add(licEmpresa);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch(Exception ex)
            {
                value = 0;
                log.Error("Error Marcar Favorita Empresa", ex);
            }
            return value;
        }
        public async Task<int> enviarOferta(empresaOfertaViewModels oferta)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Enviar Oferta");
            int value = 1;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var unOferta = db.LicOfertaEmpresa
                        .Where(x => x.idEmpresa == oferta.idEmpresa && x.idObra == oferta.idObra)
                        .FirstOrDefault();
                    if (unOferta == null)
                    {
                        LicOfertaEmpresa ofertaEmpresa = new LicOfertaEmpresa();
                        ofertaEmpresa.idEmpresa = oferta.idEmpresa;
                        ofertaEmpresa.idObra = oferta.idObra;
                        ofertaEmpresa.fecha = DateTime.Now;
                        db.LicOfertaEmpresa.Add(ofertaEmpresa);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                value = 0;
                log.Error("Error enviarOferta Empresa", ex);
            }
            return value;
        }
        public string buscarNombreEmpresa(int? idEmpresa)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Una buscarNombreEmpresa");
            string nombreEmpresa = string.Empty;
            try
            {
                using (DB_RACOPEntities db = new DB_RACOPEntities())
                {
                    var query = db.rc_Empresa.Where(x => x.idEmpresa == idEmpresa).FirstOrDefault();
                    if(query != null)
                    {
                        nombreEmpresa = query.razonSocial;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error buscarNombreEmpresa", ex);
            }
            return nombreEmpresa;
        }
        public List<licitacionGrillaViewModels> listarObraEmpresaExpotar
        (ref int recordsTotal, string sortColumn, string sortColumnDir, string searchValue,
            string nroExpediente, string nombreObra, int? idEmpresa, int? idFavorito, DateTime? fechaPub, int? idOrganismo,
            int pageSize = 0, int skip = 0)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Grilla Licitaciones Empresa");
            var listaFiltrada = new List<licitacionGrillaViewModels>();
            List<licitacionGrillaViewModels> lista = new List<licitacionGrillaViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = from lic in db.PryProyectoes
                                join edo in db.PryEstadoEtapas
                                on lic.GrlTypeState_Id equals edo.Id
                                join pyl in db.PryLicitacions
                                on lic.Id equals pyl.IdPryProyecto
                                    into pylo
                                from pyj in pylo.DefaultIfEmpty()
                                join tcont in db.GrlTypes
                                 on pyj.TipoDeContratacion_Id equals tcont.Id
                                   into tconto
                                from tcontj in tconto.DefaultIfEmpty()
                                join org in db.PryOrganismosEjecutores
                                on lic.PryOrganismoEjecutor_Id equals org.Id
                                    into orgo
                                from orgj in orgo.DefaultIfEmpty()
                                join empO in db.LicEmpresaObra.Where(x => x.idEmpresa == idEmpresa)
                                on lic.Id equals empO.idObra
                                    into empOo
                                from empOj in empOo.DefaultIfEmpty()
                                join ofe in db.LicOfertaEmpresa.Where(x => x.idEmpresa == idEmpresa)
                                on lic.Id equals ofe.idObra
                                    into ofeo
                                from ofej in ofeo.DefaultIfEmpty()
                                where edo.PryStage_Id == 49 && lic.Eliminado == false && edo.Id < 9 && edo.Id > 5
                                select new licitacionGrillaViewModels
                                {
                                    nombreEtapa = edo.Name,
                                    idEtapa = edo.Id,
                                    nombreObra = lic.Nombre,
                                    idObra = lic.Id,
                                    fechaApertura = pyj.FechaApertura,
                                    fechaPublicacion = pyj.FechaPublicacionDesde,
                                    montoObra = pyj.MontoOficial,
                                    nroExpediente = lic.Expediente,
                                    nombreOrganismo = orgj.NombreOrganismo,
                                    idOrganismo = lic.PryOrganismoEjecutor_Id,
                                    idContratacion = pyj.TipoDeContratacion_Id,
                                    nombreContratacion = tcontj.Name,
                                    idFavorito = empOj.esFavorito,
                                    fechaOferta = ofej.fecha,
                                };


                    if (idOrganismo != 0)
                    {
                        query = query.Where(x => x.idOrganismo == idOrganismo);
                    }
                    if (idFavorito != 0)
                    {
                        query = query.Where(x => x.idFavorito == idFavorito);
                    }
                    if (!string.IsNullOrEmpty(nombreObra))
                    {
                        query = query.Where(x => x.nombreObra.Contains(nombreObra));
                    }
                    if (!string.IsNullOrEmpty(nroExpediente))
                    {
                        query = query.Where(x => x.nroExpediente.Contains(nroExpediente));
                    }
                    //if ((monto != null)&&(monto != 0))
                    //{
                    //    query = query.Where(x => x.montoObra == monto);
                    //}
                    if (fechaPub.HasValue)
                    {
                        query = query.Where(x => x.fechaPublicacion == fechaPub);
                    }
                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                    {
                        if ((sortColumn == "nombreObra") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nombreObra);
                        }
                        if ((sortColumn == "nombreObra") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nombreObra);
                        }
                        if ((sortColumn == "nroExpedienteString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nroExpediente);
                        }
                        if ((sortColumn == "nroExpedienteString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nroExpediente);
                        }
                        if ((sortColumn == "fechaPublicacionString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.fechaPublicacion);
                        }
                        if ((sortColumn == "fechaPublicacionString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.fechaPublicacion);
                        }
                        if ((sortColumn == "fechaAperturaString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.fechaApertura);
                        }
                        if ((sortColumn == "fechaAperturaString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.fechaApertura);
                        }
                        if ((sortColumn == "nombreOrganismoString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nombreOrganismo);
                        }
                        if ((sortColumn == "nombreOrganismoString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nombreOrganismo);
                        }
                        if ((sortColumn == "montoObraString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.montoObra);
                        }
                        if ((sortColumn == "montoObraString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.montoObra);
                        }
                        if ((sortColumn == "nombreEtapa") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nombreEtapa);
                        }
                        if ((sortColumn == "nombreEtapa") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nombreEtapa);
                        }
                        if ((sortColumn == "accion") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nombreObra);
                        }
                        if ((sortColumn == "accion") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nombreObra);
                        }
                        if ((sortColumn == "estadoOferta") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.fechaOferta);
                        }
                        if ((sortColumn == "estadoOferta") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.fechaOferta);
                        }
                    }
                    else
                    {
                        query = query = query.OrderBy(x => x.nombreObra);
                    }

                    query = query.Take(500);
                    recordsTotal = query.Count();

                    var lst = query.Skip(skip).Take(pageSize);
                    lista = lst.ToList();
                    listaFiltrada = query.ToList();

                    return listaFiltrada;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en Grilla Licitaciones Empresa", ex);
            }
            return listaFiltrada;
        }
        public ReporteExcelVM generarExcelGrilla(List<licitacionGrillaViewModels> obras, bool? nombre, bool? expediente, bool? publicacion, bool? organismo, bool? monto, bool? etapa, bool? apertura, bool? oferta)
        {
            ReporteExcelVM excel = new ReporteExcelVM();
            excel.filas = new List<DetalleExcelVM>();

            using (db_meieEntities db = new db_meieEntities())
            {
                foreach (var item in obras)
                {
                    excel.encabezado = new List<string>();
                    if (obras != null)
                    {
                        DetalleExcelVM detalle = new DetalleExcelVM();
                        excel.encabezado.Add("Grilla Obras");
                        detalle.nombre = item.nombreObra;
                        detalle.expediente = item.nroExpedienteString;
                        detalle.publicacion = item.fechaPublicacionString;
                        detalle.organismo = item.nombreOrganismo;
                        detalle.monto = item.montoObraString;
                        detalle.etapa = item.nombreEtapa;
                        detalle.fechaApertura = item.fechaAperturaString;
                        detalle.estadoOferta = item.estadoOferta;
                        excel.filas.Add(detalle);
                    }
                }
            }
            ExcelUtility excelUtility = new ExcelUtility();
            excel = excelUtility.GenerarExcelGrillaEmpresa(excel, nombre, expediente, publicacion, organismo, monto, etapa, apertura, oferta);
            return excel;
        }
        public List<EmpresaOfertaViewModels> listarEmpresaOferta(int? idObra)
        {
            List<EmpresaOfertaViewModels> lista = new List<EmpresaOfertaViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = from of in db.LicOfertaEmpresa
                                where of.idObra == idObra
                                select new EmpresaOfertaViewModels
                                {
                                    idEmpresa = of.idEmpresa,
                                    idObra = of.idObra,
                                    esGanadora = of.esGanadora
                                };
                    lista = query.OrderByDescending(x => x.esGanadora).ToList();
                }
                using(DB_RACOPEntities db1 = new DB_RACOPEntities())
                {
                    foreach (var item in lista)
                    {
                        var unEmpresa = db1.rc_Empresa.Where(x => x.idEmpresa == item.idEmpresa).FirstOrDefault();
                        if(unEmpresa != null)
                        {
                            item.nombreEmpresa = unEmpresa.razonSocial;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return lista;
        }
        public int marcarGanadora(int? idObra, int? idEmpresa)
        {
            var value = 0;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicOfertaEmpresa.Where(x => x.idEmpresa == idEmpresa && x.idObra == idObra).FirstOrDefault();
                    if (existe != null)
                    {
                        ServicioLicitacion servicioLicitacion = new ServicioLicitacion();
                        value = servicioLicitacion.cambiarEtapa(idObra, 14); //PreAdjudicada;
                        if (value == 1)
                        {
                            existe.esGanadora = 1;
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return value;
        }
        public DatosEmpresaViewModels buscarDatosEmpresa(int? idEmpresa)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Una buscarDatosEmpresa");
            DatosEmpresaViewModels unEmpresa = new DatosEmpresaViewModels();
            try
            {
                using (DB_RACOPEntities db = new DB_RACOPEntities())
                {
                    var query = db.rc_Empresa.Where(x => x.idEmpresa == idEmpresa).FirstOrDefault();
                    if (query != null)
                    {
                        unEmpresa.nombreEmpresa = query.razonSocial;
                        unEmpresa.idEmpresa = query.idEmpresa;
                        unEmpresa.emailEmpresa = query.mailContacto;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error buscarDatosEmpresa", ex);
            }
            return unEmpresa;
        }
        public async Task<int> grabarEmailEmpresa(DatosEmpresaViewModels unEmpresa)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Una grabarEmailEmpresa");
            try
            {
                using (DB_RACOPEntities db = new DB_RACOPEntities())
                {
                    var query = db.rc_Empresa.Where(x => x.idEmpresa == unEmpresa.idEmpresa).FirstOrDefault();
                    if (query != null)
                    {
                        query.mailContacto = unEmpresa.emailEmpresa;
                        await db.SaveChangesAsync();
                        unEmpresa.nombreEmpresa = query.razonSocial;
                        await enviarMailOferta(unEmpresa);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error grabarEmailEmpresa", ex);
                return 0;
            }
            return 1;
        }
        public async Task enviarMailOferta(DatosEmpresaViewModels unEmpresa)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("enviarMailOferta");
            try
            {
                using (DB_RACOPEntities db = new DB_RACOPEntities())
                {
                    // Buscar Oferta 
                    ServicioDocumentacion servicioDocumentacion = new ServicioDocumentacion();
                    List<RequisitoViewModels> listaArchivo = new List<RequisitoViewModels>();
                    listaArchivo = servicioDocumentacion.listarRequisitoPorObraEmpresa(unEmpresa.idEmpresa, unEmpresa.idObra);
                    // Buscar Tipo Mail
                    ServicioEmail servicioEmail = new ServicioEmail();
                    EmailViewModels emailViewModels = new EmailViewModels();
                    emailViewModels = servicioEmail.buscarMail(1);
                    emailViewModels.destino = unEmpresa.emailEmpresa;
                    var lista = string.Empty;
                    lista = "<ul>";
                    foreach(var item in listaArchivo)
                    {
                        lista = lista + "<li>";
                        lista = lista + "Requisito " + item.nombre + " Archivo: " + item.nombreArchivo;
                        lista = lista + "</li>";
                    }
                    lista = lista + "</ul>";
                    emailViewModels.cuerpo = emailViewModels.cuerpo.Replace("#ListaArchivos", lista);
                    emailViewModels.cuerpo = emailViewModels.cuerpo.Replace("#Empresa", unEmpresa.nombreEmpresa);
                    emailViewModels.cuerpo = emailViewModels.cuerpo.Replace("#NombreObra", unEmpresa.nombreObra);
                    // Enviar Mail
                    await servicioEmail.enviarEmail(emailViewModels);
                }
            }
            catch (Exception ex)
            {
                log.Error("enviarMailOferta", ex);
                return ;
            }
            return;
        }
    }

}


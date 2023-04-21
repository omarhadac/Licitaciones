using Licitacion.Servicios;
using Licitaciones.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Licitaciones.Areas.Licitacion.Controllers.Api
{
    public class ArchivoController : ApiController
    {

        /// <summary>
        /// Upload archivo de requisitos por parte de la empresa
        /// </summary>
        /// <param name="Upload">Archivo</param>
        /// <param name="idObra">Identificacion Obra</param>
        /// <param name="idEmpresa">Tipo de Archivo</param>
        /// <param name="idRequisito">Tipo de Archivo</param>
        /// <param name="nroSobre">Nro de Sobre</param>
        /// <returns>string con mensaje de exito o error</returns>
        /// <response code="200">Ok devuelve lista</response>
        [Route("api/Archivo/UploadFile")]
        [System.Web.Http.ActionName("UploadFile")]
        [System.Web.Http.HttpPost]
        public string UploadFile()
        {
            try
            {
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    var httpPostedFile = System.Web.HttpContext.Current.Request.Files["Upload"];
                    int idObra = Convert.ToInt32(System.Web.HttpContext.Current.Request["idObra"].ToString());
                    int idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                    int idRequisito = Convert.ToInt32(System.Web.HttpContext.Current.Request["idRequisito"].ToString());
                    int nroSobre = Convert.ToInt32(System.Web.HttpContext.Current.Request["nroSobre"].ToString());

                    if (httpPostedFile != null)
                    {
                        string ruta = ConfigurationSettings.AppSettings["repositorioFiles"].ToString();
                        string nombreArchivo = DateTime.Now.TimeOfDay.Minutes + DateTime.Now.TimeOfDay.Milliseconds + "_" + httpPostedFile.FileName;
                        string folderName = idObra.ToString();
                        string pathString = System.IO.Path.Combine(ruta, folderName);
                        if (!System.IO.Directory.Exists(pathString))
                        {
                            System.IO.Directory.CreateDirectory(pathString);
                        }
                        ruta = pathString;
                        var filePath = ruta + "\\" + nombreArchivo;
                        ServicioArchivo _servicio = new ServicioArchivo();

                        httpPostedFile.SaveAs(filePath);
                        _servicio.nuevo
                            (ruta, httpPostedFile.FileName, nombreArchivo, httpPostedFile.ContentType, idObra, idEmpresa, idRequisito, nroSobre);
                        return "Ok";
                    }
                }
                return "";
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        /// <summary>
        /// Elimina archivo de requisitos por parte de la empresa
        /// </summary>
        /// <param name="idArchivo">Identificacion Obra</param>
        /// <returns>string con mensaje de exito o error</returns>
        /// <response code="200">Ok devuelve lista</response>
        [Route("api/Archivo/EliminarArchivo")]
        [System.Web.Http.ActionName("EliminarArchivo")]
        [System.Web.Http.HttpGet]
        public string EliminarArchivo(int? id)
        {
            ServicioArchivo servicio = new ServicioArchivo();
            servicio.eliminarArchivo(id);
            return "Ok";
        }

        [Route("api/Archivo/DescargarArchivo")]
        [System.Web.Http.ActionName("DescargarArchivo")]
        [HttpGet]
        public IHttpActionResult DescargarArchivo(int id)
        {
            HttpResponseMessage result = null;
            ServicioArchivo servicio = new ServicioArchivo();
            var unArchivo = servicio.buscarArchivo(id);
            if (string.IsNullOrEmpty(unArchivo.ruta))
            {               
                return BadRequest();
            }
            else{
                string filePath = unArchivo.ruta;
                IHttpActionResult response;
                HttpResponseMessage responseMsg = new HttpResponseMessage(HttpStatusCode.OK);
                var fileStream = new FileStream(filePath, FileMode.Open);
                responseMsg.Content = new StreamContent(fileStream);
                responseMsg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                responseMsg.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                responseMsg.Content.Headers.ContentDisposition.FileName = unArchivo.nombre;
                response = ResponseMessage(responseMsg);
                return response;
            }
        }

        /// <summary>
        /// Upload archivo de requisitos por parte de la empresa
        /// </summary>
        /// <param name="Upload">Archivo</param>
        /// <param name="idObra">Identificacion Obra</param>
        /// <param name="idCategoria">Tipo de Archivo</param>
        /// <returns>string con mensaje de exito o error</returns>
        /// <response code="200">Ok devuelve lista</response>
        [Route("api/Archivo/UploadFileObra")]
        [System.Web.Http.ActionName("UploadFileObra")]
        [System.Web.Http.HttpPost]
        public string UploadFileObra()
        {
            try
            {
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    var httpPostedFile = System.Web.HttpContext.Current.Request.Files["Upload"];
                    int idObra = Convert.ToInt32(System.Web.HttpContext.Current.Request["idObra"].ToString());
                    int idCategoria = Convert.ToInt32(System.Web.HttpContext.Current.Request["idCategoria"].ToString());

                    if (httpPostedFile != null)
                    {
                        string ruta = ConfigurationSettings.AppSettings["repositorioFiles"].ToString();
                        string nombreArchivo = DateTime.Now.TimeOfDay.Minutes + DateTime.Now.TimeOfDay.Milliseconds + "_" + httpPostedFile.FileName;
                        string folderName = idObra.ToString();
                        string pathString = System.IO.Path.Combine(ruta, folderName);
                        if (!System.IO.Directory.Exists(pathString))
                        {
                            System.IO.Directory.CreateDirectory(pathString);
                        }
                        ruta = pathString;
                        var filePath = ruta + "\\" + nombreArchivo;
                        ServicioArchivo _servicio = new ServicioArchivo();

                        httpPostedFile.SaveAs(filePath);
                        _servicio.nuevoObra(filePath, nombreArchivo, idObra, idCategoria);
                        return "Ok";
                    }
                }
                return "";
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        [Route("api/Archivo/listarArchivoObra")]
        [System.Web.Http.ActionName("listarArchivoObra")]
        [HttpGet]
        public List<ArchivoObraViewModels> listarArchivoObra(int idObra)
        {
            ServicioArchivo servicio = new ServicioArchivo();
            var lista = servicio.listarArchivoObra(idObra);
            return lista;
        }
    }
}

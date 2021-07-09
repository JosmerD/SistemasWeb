using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using SistemasWeb.Areas.Cursos.Models;
using SistemasWeb.Areas.Inscripciones.Models;
using SistemasWeb.Data;
using SistemasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemasWeb.Library
{
    public class LCursos
    {
        private ApplicationDbContext _context;
        private IWebHostEnvironment environment;
        private Uploadimage _image;

        public LCursos(ApplicationDbContext context)
        {
            _context = context;
        }

        public LCursos(ApplicationDbContext context, IWebHostEnvironment environment) : this(context)
        {
            this.environment = environment;
            _image = new Uploadimage();
        }

        public async Task<IdentityError> RegistrarCursoAsync(DataPaginador<TCursos> model)
        {
            IdentityError identityError;
            try
            {
           
              
                if (model.Input.CursoID.Equals(0))
                {
                    var imageByte = await _image.ByteAvatarImageAsync(model.AvatarImage, environment);

                    var curso = new TCursos
                    {
                        Curso = model.Input.Curso,
                        Informacion = model.Input.Informacion,
                        Horas = model.Input.Horas,
                        Costo = model.Input.Costo,
                        Estado = model.Input.Estado,
                        Image = imageByte,
                        CategoriaID = model.Input.CategoriaID
                    };

                    _context.Add(curso);
                }
                else
                {
                    byte[] imageByte;
                    if (model.AvatarImage!=null)
                    {
                        imageByte= await _image.ByteAvatarImageAsync(model.AvatarImage, environment);
                    }
                    else
                    {
                        imageByte = model.Input.Image;
                    }
                    var curso = new TCursos
                    {
                        CursoID=model.Input.CursoID,
                        Curso = model.Input.Curso,
                        Informacion = model.Input.Informacion,
                        Horas = model.Input.Horas,
                        Costo = model.Input.Costo,
                        Estado = model.Input.Estado,
                        Image = imageByte,
                        CategoriaID = model.Input.CategoriaID
                    };
                    _context.Update(curso);
                }
                
             
                _context.SaveChanges();

                identityError = new IdentityError
                {
                    Code = "Done"
                };
            }
            catch (Exception e)
            {

                identityError = new IdentityError
                {
                    Code = "Error",
                    Description = e.Message
                };
            }
            return identityError;
        }
        internal List<TCursos> getTCursos(string search)
        {
            List<TCursos> listCursos;
            if (search==null)
            {
                listCursos = _context._TCursos.ToList();
            }
            else
            {
                listCursos = _context._TCursos.Where(c => c.Curso.StartsWith(search)).ToList();
            }
            return listCursos;
        }
        internal IdentityError UpdateEstado(int id)
        {
            IdentityError identityError;
            try
            {              
                var curso = _context._TCursos.Where(c => c.CursoID.Equals(id)).ToList().ElementAt(0);
                curso.Estado = curso.Estado ? false : true;
                _context.Update(curso);
                _context.SaveChanges();

                identityError = new IdentityError
                {
                    Code = "Done"
                };
            }
            catch (Exception e)
            {

                identityError = new IdentityError
                {
                    Code = "Error",
                    Description = e.Message
                };
            }
            return identityError;
        }
        internal IdentityError DeleteCurso(int CursoID)
        {
            IdentityError identityError;
        
            try
            {
                var curso = new TCursos {
                    CursoID = CursoID
                };                
                _context.Remove(curso);
                _context.SaveChanges();

                identityError = new IdentityError
                {
                    Code = "Done"
                };
            }
            catch (Exception e)
            {

                identityError = new IdentityError
                {
                    Code = "Error",
                    Description = e.Message
                };
            }
            return identityError;
        }
        public DataCurso getTCurso(int id)
        {
            DataCurso dataCurso = null;
            var query = _context._TCategoria.Join(_context._TCursos,
                cat=>cat.CategoriaID,
                cur=>cur.CategoriaID,
                (cat,cur)=> new { 
                
                    cat.CategoriaID,
                    cat.Categoria,
                    cur.CursoID,
                    cur.Curso,
                    cur.Informacion,
                    cur.Horas,
                    cur.Costo,
                    cur.Estado,
                    cur.Image
                }).Where(a=>a.CursoID.Equals(id)).ToList();
           
            if (!query.Count.Equals(0))
            {
                var data = query.Last();
                dataCurso = new DataCurso
                { 
                    CursoID=data.CursoID,
                    Curso=data.Curso,
                    Informacion=data.Informacion,
                    Horas=data.Horas,
                    Costo=data.Costo,
                    Estado=data.Estado,
                    Image=data.Image,
                    Categoria=data.Categoria
                };
            }
            return dataCurso;
        }
        public IdentityError Inscripcion(string idUser, int cursoID)
        {
            IdentityError identityError;
            try
            {
                var cursoInscripcion = _context._TInscripcion.Where(c => c.CursoID.Equals(cursoID) && c.EstudianteID.Equals(idUser)).ToList();
                if (cursoInscripcion.Count.Equals(0))
                {
                    var curso = getTCurso(cursoID);
                    var inscripcion = new Inscripcion
                    {
                        EstudianteID = idUser,
                        Fecha = DateTime.Now,
                        Pago = curso.Costo,
                        CursoID = curso.CursoID

                    };
                    _context.Add(inscripcion);
                    _context.SaveChanges();
                    identityError = new IdentityError { Description = "Done" };
                }
                else
                {
                    identityError = new IdentityError { Description = "Ya esta sucrito en el curso" };
                }
            }
            catch (Exception e)
            {

                identityError = new IdentityError
                {
                    Code = "Error",
                    Description = e.Message
                };
            }
            return identityError;
        }
        public List<DataCurso> Inscripciones(string idUser, string search)
        {
            List<DataCurso> cursos = new List<DataCurso>();

            var inscripciones = _context._TInscripcion.Where(c => c.EstudianteID.Equals(idUser)).ToList();
            if (!inscripciones.Count.Equals(0))
            {
                inscripciones.ForEach(c =>
                {
                    if (search==null || search.Equals(""))
                    {
                        var query = _context._TCategoria.Join(_context._TCursos,
                            c=>c.CategoriaID,
                            t=>t.CategoriaID,
                            (c,t)=> new {

                                c.CategoriaID,
                                c.Categoria,
                                t.CursoID,
                                t.Curso,
                                t.Informacion,
                                t.Horas,
                                t.Costo,
                                t.Estado,
                                t.Image
                            }).Where(a => a.CursoID.Equals(c.CursoID)).ToList();
                       
                        if (!query.Count.Equals(0))
                        {
                            var data = query.Last();
                            cursos.Add ( new DataCurso
                            {
                                CursoID = data.CursoID,
                                Curso = data.Curso,
                                Informacion = data.Informacion,
                                Horas = data.Horas,
                                Costo = data.Costo,
                                Estado = data.Estado,
                                Image = data.Image,
                                Categoria = data.Categoria
                            });
                        }
                        else
                        {
                           query = _context._TCategoria.Join(_context._TCursos,
                           c => c.CategoriaID,
                           t => t.CategoriaID,
                           (c, t) => new {

                               c.CategoriaID,
                               c.Categoria,
                               t.CursoID,
                               t.Curso,
                               t.Informacion,
                               t.Horas,
                               t.Costo,
                               t.Estado,
                               t.Image
                           }).Where(a => a.CursoID.Equals(c.CursoID) && a.Curso.StartsWith(search)).ToList();

                            if (!query.Count.Equals(0))
                            {
                                var data = query.Last();
                                cursos.Add(new DataCurso
                                {
                                    CursoID = data.CursoID,
                                    Curso = data.Curso,
                                    Informacion = data.Informacion,
                                    Horas = data.Horas,
                                    Costo = data.Costo,
                                    Estado = data.Estado,
                                    Image = data.Image,
                                    Categoria = data.Categoria
                                });
                            }
                        }
                    }
                });
            }
            return cursos;
        }
    }
}

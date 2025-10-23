using AssesmentUC.Service.Service.Interface;
using AssesmentUC.Infrastructure.Repository.Interface;
using AssesmentUC.Model.Entity;
using AssesmentUC.Service.DTO.Encuesta;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using ClosedXML.Excel;
using Microsoft.Extensions.Configuration;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Text;


namespace AssesmentUC.Service.Service.Impl
{
    public class EncuestaService : IEncuestaService
    {
        private readonly IEncuestaRepository _encuestaRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        public EncuestaService(IEncuestaRepository encuestaRepository, IEmailService emailService, IConfiguration configuration)
        {
            _encuestaRepository = encuestaRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<List<EncuestaListAllDTO>> ListarPlantillasEncuestasAsync(int pageNumber, int pageSize)
        {
            var encuestas = await _encuestaRepository.ListarPlantillaEncuestasRepository(pageNumber, pageSize);

            var dtoList = encuestas.Select(e => new EncuestaListAllDTO
            {
                EncuestaId = e.EncuestaId,
                NombreEncuesta = e.NombreEncuesta,
                DescripcionEncuesta = e.DescripcionEncuesta,
                NombreTipoEncuesta = e.NombreTipoEncuesta,
                FechaCreacion = e.FechaCreacion
            }).ToList();

            return dtoList;
        }

        public async Task<List<EncuestaListAllDTO>> ListarAsignaturaEncuestasAsync(EncuestaAsignaturaFiltroDTO dto)
        {
            var filtroEncuestaAsignatura = new Encuesta
            {
                Seccion = dto.Seccion,
                Modulo = dto.Modulo,
                Docente = dto.Docente,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin
            };

            var encuestas = await _encuestaRepository.ListarAsignaturaEncuestasRepository(filtroEncuestaAsignatura, dto.pageNumber, dto.pageSize);

            var dtoList = encuestas.Select(e => new EncuestaListAllDTO
            {
                EncuestaId = e.EncuestaId,
                NombreEncuesta = e.NombreEncuesta,
                DescripcionEncuesta = e.DescripcionEncuesta,
                NombreTipoEncuesta = e.NombreTipoEncuesta,
                Sede = e.Sede,
                Periodo = e.Periodo,
                Programa = e.TipoPrograma,
                Seccion = e.Seccion,
                Modulo = e.Modulo,
                TipoEncuestadoId = e.TipoEncuestadoId,
                FechaCreacion = e.FechaCreacion
            }).ToList();

            return dtoList;
        }

        public async Task<EncuestaPlantillaDetailDTO> ListarPlantillaEncuestaIdAsync(int id)
        {
            var encuesta = await _encuestaRepository.ListarPlantillaEncuestaIdRepository(id);

            if (encuesta == null)
                throw new InvalidOperationException($"No se encontró la encuesta con ID {id}");

            var dtoEncuesta = new EncuestaPlantillaDetailDTO
            {
                EncuestaId = encuesta.EncuestaId,
                NombreEncuesta = encuesta.NombreEncuesta,
                DescripcionEncuesta = encuesta.DescripcionEncuesta,
                TipoEncuestaId = encuesta.TipoEncuestaId,
                TipoEncuesta = encuesta.NombreTipoEncuesta,
                FechaCreacion = encuesta.FechaCreacion,
                Bloques = encuesta.Bloques.Select(b => new BloqueDetailDTO
                {
                    BloqueId = b.BloqueId,
                    TituloBloque = b.TituloBloque,
                    Orden = b.OrdenBloque,
                    Preguntas = b.Preguntas.Select(p => new PreguntaDetailDTO
                    {
                        PreguntaId = p.EncuestaDetalleId,
                        TextoPregunta = p.TextoPregunta,
                        TipoPregunta = p.TipoPregunta,
                        Orden = p.OrdenPregunta,
                        OpcionesJson = p.OpcionesJson
                    }).ToList()
                }).ToList()
            };

            return dtoEncuesta;
        }

        public async Task<List<ListaTiposDTO>> ListarTipoEncuestaAsync()
        {
            var tiposEncuesta = await _encuestaRepository.ListarTipoEncuestaRepository();

            var dtoList = tiposEncuesta
                .Select(e => new ListaTiposDTO
                {
                    ListaTipoId = e.TipoEncuestaId,
                    NombreTipo = e.NombreTipoEncuesta
                })
                .ToList();

            return dtoList;
        }

        public async Task<List<ListaTiposDTO>> ListarSedesAsync()
        {
            var sedes = await _encuestaRepository.ListarSedesRepository();

            var dtoList = sedes
                .Select(e => new ListaTiposDTO
                {
                    SedeId = e.SedeId,
                    NombreTipo = e.Sede
                })
                .ToList();

            return dtoList;
        }
        public async Task<List<ListaTiposDTO>> ListarPeriodosAsync()
        {
            var periodos = await _encuestaRepository.ListarPeriodosRepository();

            var dtoList = periodos
                .Select(e => new ListaTiposDTO
                {
                    PeriodoId = e.PeriodoId,
                    NombreTipo = e.Periodo
                })
                .ToList();

            return dtoList;
        }
        public async Task<List<ListaTiposDTO>> ListarSeccionesAsync()
        {
            var secciones = await _encuestaRepository.ListarSeccionesRepository();

            var dtoList = secciones
                .Select(e => new ListaTiposDTO
                {
                    SeccionId = e.SeccionId,
                    NombreTipo = e.Seccion
                })
                .ToList();

            return dtoList;
        }

        public async Task<List<ListaTiposDTO>> ListarAsignaturasAsync(string seccion, string? programa)
        {
            var asignaturas = await _encuestaRepository.ListarAsignaturasRepository(seccion, programa);

            var dtoList = asignaturas
                .Select(e => new ListaTiposDTO
                {
                    AsignaturaId = e.NRC,
                    NombreTipo = e.Modulo
                })
                .ToList();

            return dtoList;
        }

        public async Task<List<ListaTiposDTO>> ListarDocentesAsync(string seccion, string asignatura)
        {
            var docentes = await _encuestaRepository.ListarDocentesRepository(seccion, asignatura);

            var dtoList = docentes
                .Select(e => new ListaTiposDTO
                {
                    DocenteId = e.DocenteId,
                    NombreTipo = e.Docente
                })
                .ToList();

            return dtoList;
        }

        public async Task<List<ListaTiposDTO>> ListarTipoProgramaAsync(string seccion)
        {
            var tiposPrograma = await _encuestaRepository.ListarTipoProgramaRepository(seccion);

            var dtoList = tiposPrograma
                .Select(e => new ListaTiposDTO
                {
                    ProgramaId = e.TipoProgramaId,
                    NombreTipo = e.TipoPrograma
                })
                .ToList();

            return dtoList;
        }

        public async Task<List<ListaTiposDTO>> ListarAsesoresAsync(string seccion)
        {
            var listAsesores = await _encuestaRepository.ListarAsesoresRepository(seccion);

            var dtoList = listAsesores
                .Select(e => new ListaTiposDTO
                {
                    AsesorId = e.AsesorId,
                    NombreTipo = e.Asesor
                })
                .ToList();

            return dtoList;
        }

        public async Task<List<ListaTiposDTO>> ListarTipoEncuestadoAsync()
        {
            var tiposEncuestado = await _encuestaRepository.ListarTipoEncuestadoRepository();

            var dtoList = tiposEncuestado
                .Select(e => new ListaTiposDTO
                {
                    ListaTipoId = e.TipoEncuestadoId,
                    NombreTipo = e.NombreTipoEncuestado
                })
                .ToList();

            return dtoList;
        }

        public async Task CrearAsignaturaEncuestaAsync(CrearEncuestaAsignaturaRequestDTO dto)
        {
            int encuestaPlantillaId = dto.Encuesta.EncuestaId;

            var encuestaPlantilla = await _encuestaRepository.ListarPlantillaEncuestaIdRepository(encuestaPlantillaId);

            var encuesta = new Encuesta
            {
                NombreEncuesta = encuestaPlantilla.NombreEncuesta,
                DescripcionEncuesta = encuestaPlantilla.DescripcionEncuesta,
                Sede = dto.Encuesta.Sede,
                TipoEncuestaId = encuestaPlantilla.TipoEncuestaId,
                TipoEncuestadoId = dto.Encuesta.TipoEncuestadoId,
                TipoPrograma = dto.Encuesta.TipoPrograma,
                PeriodoId = dto.Encuesta.PeriodoId,
                SeccionId = dto.Encuesta.SeccionId,
                Modulo = dto.Encuesta.AsignaturaNombre,
                Docente = dto.Encuesta.Docente,
                FechaInicio = dto.Encuesta.FechaInicio,
                FechaFin = dto.Encuesta.FechaFin,
                FechaCreacion = DateTime.Now,
                Activo = true,
                UsuarioCreacion = dto.Usuario,
                Bloques = encuestaPlantilla.Bloques?.Select(b => new EncuestaBloque
                {
                    TituloBloque = b.TituloBloque,
                    OrdenBloque = b.OrdenBloque,
                    UsuarioCreacion = dto.Usuario,
                    FechaCreacion = DateTime.Now,
                    Preguntas = b.Preguntas?.Select(p => new EncuestaPregunta
                    {
                        TextoPregunta = p.TextoPregunta,
                        TipoPregunta = p.TipoPregunta,
                        OrdenPregunta = p.OrdenPregunta,
                        OpcionesJson = p.OpcionesJson,
                        FechaCreacion = DateTime.Now,
                        UsuarioCreacion = dto.Usuario,
                    }).ToList() ?? new List<EncuestaPregunta>()
                }).ToList() ?? new List<EncuestaBloque>()
            };

            int encuestaId = await _encuestaRepository.CrearAsignaturaEncuestaRepository(encuesta);

            List<EncuestadoDNIDTO> listaEncuestados = await EnviarCorreoEncuestaAsync(dto.DatosCorreo, dto.Encuesta, encuestaId);

            List<string> dniEncuestados = listaEncuestados
                                            .Select(e => e.EncuestadoId)                            
                                            .ToList();

            await _encuestaRepository.InsertarEncuestasPorAsignaturaBulkAsync(encuestaId, dto.Usuario, dniEncuestados, dto.Encuesta.TipoEncuestadoId);

        }

        public async Task<int> CrearPlantillaEncuestaAsync(EncuestaPlantillaCreateDTO dto, string usuario)
        {
            var encuesta = new Encuesta
            {
                NombreEncuesta = dto.NombreEncuesta,
                DescripcionEncuesta = dto.DescripcionEncuesta,
                TipoEncuestaId = dto.TipoEncuestaId,
                FechaCreacion = DateTime.Now,
                Estado = true,
                UsuarioCreacion = usuario,
                Bloques = dto.Bloques?.Select(b => new EncuestaBloque
                {
                    TituloBloque = b.TituloBloque,
                    OrdenBloque = b.OrdenBloque,
                    Estado = true,
                    UsuarioCreacion = usuario,
                    FechaCreacion = DateTime.Now,
                    Preguntas = b.Preguntas?.Select(p => new EncuestaPregunta
                    {
                        TextoPregunta = p.TextoPregunta,
                        TipoPregunta = p.TipoPregunta,
                        OrdenPregunta = p.OrdenPregunta,
                        OpcionesJson = p.OpcionesJson,
                        Estado = true,
                        FechaCreacion = DateTime.Now,
                        UsuarioCreacion = usuario,
                    }).ToList() ?? new List<EncuestaPregunta>()
                }).ToList() ?? new List<EncuestaBloque>()
            };

            return await _encuestaRepository.CrearPlantillaEncuestaRepository(encuesta);
        }



        public async Task EditarEncuestaPlantillaAsync(EncuestaPlantillaUpdateDTO dto, string usuario)
        {
            var encuesta = new Encuesta
            {
                EncuestaId = dto.EncuestaId,
                NombreEncuesta = dto.NombreEncuesta ?? string.Empty,
                DescripcionEncuesta = dto.DescripcionEncuesta ?? string.Empty,
                TipoEncuestaId = dto.TipoEncuestaId ?? 0,
                UsuarioModificacion = usuario,
                FechaModificacion = DateTime.Now,
                Bloques = dto.Bloques?.Select(b => new EncuestaBloque
                {
                    BloqueId = b.BloqueId ?? 0,
                    TituloBloque = b.TituloBloque ?? string.Empty,
                    OrdenBloque = b.Orden ?? 0,
                    Estado = true,
                    UsuarioModificacion = usuario,
                    FechaModificacion = DateTime.Now,
                    Preguntas = b.Preguntas?.Select(p => new EncuestaPregunta
                    {
                        EncuestaDetalleId = p.PreguntaId ?? 0,
                        TextoPregunta = p.TextoPregunta ?? string.Empty,
                        TipoPregunta = p.TipoPregunta ?? string.Empty,
                        OrdenPregunta = p.Orden ?? 0,
                        OpcionesJson = p.OpcionesJson,
                        Estado = true
                    }).ToList() ?? new List<EncuestaPregunta>()
                }).ToList() ?? new List<EncuestaBloque>()
            };

            await _encuestaRepository.EditarEncuestaPlantillaRepository(encuesta);
        }

        public async Task EliminarEncuestaAsync(int id, string usuario)
        {
            await _encuestaRepository.EliminarEncuestaRepository(id, usuario);
        }

        public async Task EliminarBloqueAsync(int id, string usuario)
        {
            await _encuestaRepository.EliminarBloqueRepository(id, usuario);
        }

        public async Task EliminarPreguntaAsync(int id, string usuario)
        {
            await _encuestaRepository.EliminarPreguntaRepository(id, usuario);
        }

        public async Task<List<EncuestadoDNIDTO>> EnviarCorreoEncuestaAsync(EncuestaDatosCorreoDTO dtoCorreo, EncuestaAsignaturaCreateDTO dtoEncuesta, int encuestaId)
        {
            List<EncuestadoDNIDTO> listaEncuestados = new List<EncuestadoDNIDTO>();

            switch (dtoEncuesta.TipoEncuestadoId)
            {
                case 1: //alumnos
                    var alumnos = await _encuestaRepository.ListarAlumnosRepository(dtoEncuesta.SeccionId, dtoEncuesta.NRC);
                    listaEncuestados = alumnos.Select(a => new EncuestadoDNIDTO
                    {
                        EncuestadoId = a.AlumnoId,
                        EncuestadoNombre = a.Alumno
                    }).ToList();
                    break;

                case 2: //docentes
                    var docentes = await _encuestaRepository.ListarDocentesRepository(dtoEncuesta.SeccionId, dtoEncuesta.NRC);
                    listaEncuestados = docentes.Select(d => new EncuestadoDNIDTO
                    {
                        EncuestadoId = d.DocenteId,
                        EncuestadoNombre = d.Docente
                    }).ToList();
                    break;

                case 3: //administrativos
                    listaEncuestados = new List<EncuestadoDNIDTO>
                    {
                        new EncuestadoDNIDTO { EncuestadoId = dtoEncuesta.AsesorId, EncuestadoNombre = dtoEncuesta.Asesor }
                    };
                    break;

                default:
                    listaEncuestados = new List<EncuestadoDNIDTO>
                    {
                        new EncuestadoDNIDTO { EncuestadoId = "qqcorreoError", EncuestadoNombre = "Error" }
                    };
                    break;
            }

            var correosDestino = listaEncuestados.Select(e => $"{e.EncuestadoId}@continental.edu.pe").ToList();

            if (correosDestino == null || !correosDestino.Any())
            {
                throw new InvalidOperationException("Encuesta creada pero no se envió a ningún correo (no había destinatarios).");
            }


            //BORRAR -- SOLO PARA PRUEBAS
            var datosHtml = new StringBuilder();
            datosHtml.Append("<ul>");
            foreach (var item in listaEncuestados)
            {
                datosHtml.Append($"<li><strong>{item.EncuestadoId}: </strong> {item.EncuestadoNombre}</li>");
            }
            datosHtml.Append("</ul>");
            //BORRAR----------------------

            string baseUrl = _configuration["Paths:URLEncuestaDev"]!;
            string linkEncuesta = $"{baseUrl}{encuestaId}";
            string cuerpoCorreoHtml = dtoCorreo.CuerpoCorreo.Replace(Environment.NewLine, "<br>");
            string cuerpo = $"{cuerpoCorreoHtml}<br><br>{linkEncuesta}<br><br>{datosHtml}"; //BORRAR PARA PROD
            //string cuerpo = $"{cuerpoCorreoHtml}<br>{linkEncuesta}<br><br>{datosHtml}"; DESCOMENTAR PARA PROD

            const int maxDestinatariosPorCorreo = 100;

            var bloques = correosDestino
                .Select((correo, index) => new { correo, index })
                .GroupBy(x => x.index / maxDestinatariosPorCorreo)
                .Select(g => g.Select(x => x.correo).ToList());

            foreach (var bloque in bloques)
            {
                try
                {
                    await _emailService.SendEmailAsync(
                        dtoCorreo.MotivoCorreo ?? "Encuesta disponible",
                        cuerpo,
                        bloque
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error enviando bloque: {ex.Message}");
                }
            }

            return listaEncuestados;
        }


    }
}

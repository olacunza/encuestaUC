using AssesmentUC.Service.Service.Interface;
using AssesmentUC.Infrastructure.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssesmentUC.Model.Entity;
using AssesmentUC.Service.DTO.Encuesta;

namespace AssesmentUC.Service.Service.Impl
{
    public class EncuestaService : IEncuestaService
    {
        private readonly IEncuestaRepository _encuestaRepository;
        public EncuestaService(IEncuestaRepository encuestaRepository)
        {
            _encuestaRepository = encuestaRepository;
        }

        public async Task<List<EncuestaListAllDTO>> ListarEncuestas()
        {
            var encuestas = await _encuestaRepository.ListarEncuestasRepository();

            var dtoList = encuestas.Select(e => new EncuestaListAllDTO
            {
                EncuestaId = e.EncuestaId,
                NombreEncuesta = e.NombreEncuesta,
                DescripcionEncuesta = e.DescripcionEncuesta,
                TipoPrograma = e.TipoPrograma,
                FechaCreacion = e.FechaCreacion
            }).ToList();

            return dtoList;
        }

        public async Task<EncuestaDetailDTO> ListarEncuestaId(int id)
        {
            var encuesta = await _encuestaRepository.ListarEncuestaIdRepository(id);

            var dtoEncuesta = new EncuestaDetailDTO
            {
                EncuestaId = encuesta.EncuestaId,
                NombreEncuesta = encuesta.NombreEncuesta,
                TipoPrograma = encuesta.TipoPrograma,
                Sede = encuesta.Sede,
                Periodo = encuesta.Periodo,
                Seccion = encuesta.Seccion,
                FechaInicio = encuesta.FechaInicio,
                FechaFin = encuesta.FechaFin,
                Completado = encuesta.Completado,
                Activo = encuesta.Activo,
                FechaCreacion = encuesta.FechaCreacion,
                Bloques = encuesta.Bloques.Select( b => new BloqueDetailDTO
                {
                    BloqueId = b.BloqueId,
                    TituloBloque = b.TituloBloque,
                    Orden = b.OrdenBloque,
                    Preguntas = b.Preguntas.Select( p => new PreguntaDetailDTO
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

        public async Task<int> CrearEncuestaAsync(EncuestaCreateDTO dto, string usuario)
        {
            var encuesta = new Encuesta
            {
                NombreEncuesta = dto.NombreEncuesta,
                DescripcionEncuesta = dto.DescripcionEncuesta,
                Sede = dto.Sede,
                TipoPrograma = dto.TipoPrograma,
                Periodo = dto.Periodo,
                Seccion = dto.Seccion,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                FechaCreacion = DateTime.Now,
                Activo = false,
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

            return await _encuestaRepository.CrearEncuestaRepository(encuesta);
        }

        public async Task EditarEncuestaAsync(EncuestaUpdateDTO dto, string usuario)
        {
            var encuesta = new Encuesta
            {
                EncuestaId = dto.EncuestaId,
                NombreEncuesta = dto.NombreEncuesta ?? string.Empty,
                DescripcionEncuesta = dto.DescripcionEncuesta ?? string.Empty,
                TipoPrograma = dto.TipoPrograma ?? 0,
                Sede = dto.Sede ?? 0,
                Periodo = dto.Preiodo ?? String.Empty,
                Seccion = dto.Seccion ?? String.Empty,
                FechaInicio = dto.FechaInicio ?? DateTime.MinValue,
                FechaFin = dto.FechaFin ?? DateTime.MinValue,
                Activo = dto.Activo,
                Estado = true,
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

            await _encuestaRepository.EditarEncuestaRepository(encuesta);
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

    }
}

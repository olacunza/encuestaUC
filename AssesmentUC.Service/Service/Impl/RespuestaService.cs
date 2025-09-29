using AssesmentUC.Infrastructure.Repository.Impl;
using AssesmentUC.Infrastructure.Repository.Interface;
using AssesmentUC.Model.Entity;
using AssesmentUC.Service.DTO.Encuesta;
using AssesmentUC.Service.DTO.Respuesta;
using AssesmentUC.Service.Service.Interface;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service.Service.Impl
{
    public class RespuestaService : IRespuestaService
    {
        public readonly IRespuestaRepository _respuestaRepository;

        public RespuestaService(IRespuestaRepository respuestaRepository)
        {
            _respuestaRepository = respuestaRepository;
        }

        public async Task<List<RespuestaListAllDTO>> ListarEncuestasRespondidasAsync(string alumnoId)
        {
            var rpta = await _respuestaRepository.ListarEncuestasRespondidasRepository(alumnoId);

            return rpta.Select(r => new RespuestaListAllDTO
            {
                EncuestaId = r.EncuestaId,
                NombreEncuesta = r.Encuesta.NombreEncuesta,
                FechaRespuesta = r.FechaRespuesta,
                Completado = r.Completado
            }).ToList();

        }

        public async Task RegistrarRespuestaAsync(RespuestaEncuestaCreateDTO dto)
        {
            var respuestaModel = new RespuestaEncuesta
            {
                EncuestaId = dto.EncuestaId,
                AlumnoId = dto.AlumnoId,
                FechaRespuesta = DateTime.Now,
                Completado = dto.Completado,
                Respuestas = dto.Respuestas.Select( r => new RespuestaPregunta
                {
                    EncuestaPreguntaId = r.EncuestaPreguntaId,
                    ValorRespuesta = r.ValorRespuesta
                }).ToList()
            };

            await _respuestaRepository.RegistrarRespuestaRepository(respuestaModel);

            await _respuestaRepository.ActualizarEncuestaCompletadaRepository(dto.EncuestaId, dto.AlumnoId);
        }

        public async Task<DTO.Respuesta.EncuestaAsignaturaDetailDTO> ListarPreguntasEncuestaAsync(int encuestaId)
        {
            var encuesta = await _respuestaRepository.ListaPreguntasEncuestaRepository(encuestaId);

            string nombreDocente = await _respuestaRepository.BuscarNombreDocente(encuesta.DocenteId);

            if (encuesta == null)
                //throw new InvalidOperationException($"No se encontró la encuesta con ID {encuestaId}");
                return null;

            var dtoEncuesta = new DTO.Respuesta.EncuestaAsignaturaDetailDTO
            {
                EncuestaId = encuesta.EncuestaId,
                NombreEncuesta = encuesta.NombreEncuesta,
                DescripcionEncuesta = encuesta.DescripcionEncuesta,
                TipoEncuesta = encuesta.NombreTipoEncuesta,
                TipoPrograma = encuesta.TipoPrograma,
                Sede = encuesta.Sede,
                Seccion = encuesta.Seccion,
                Modulo = encuesta.Modulo,
                Docente = nombreDocente,
                FechaInicio = encuesta.FechaInicio,
                FechaFin = encuesta.FechaFin,
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
        public async Task<List<EncuestasPendientesDTO>> ListaEncuestaAsignaturaAsync(string alumnoId)
        {
            var encuestas = await _respuestaRepository.ListaEncuestaAsignaturaPendienteRepository(alumnoId);

            var dtoList = encuestas.Select(e => new EncuestasPendientesDTO
            {
                EncuestaId = e.EncuestaId,
                NombreEncuesta = e.NombreEncuesta,
                DescripcionEncuesta = e.DescripcionEncuesta,
                NombreTipoEncuesta = e.NombreTipoEncuesta,
                FechaInicio = e.FechaInicio,
                FechaFin = e.FechaFin
            }).ToList();

            return dtoList;
        }

        public async Task<bool> VerificarSiRespondioAsync(int encuestaId, string alumnoId)
        {
            return await _respuestaRepository.VerificarSiRespondioRepository(encuestaId, alumnoId);
        }

        public async Task<bool> VerificarEncuestaActivaAsync(int encuestaId)
        {
            return await _respuestaRepository.VerificarEncuestaActivaRepository(encuestaId);
        }

    }
}

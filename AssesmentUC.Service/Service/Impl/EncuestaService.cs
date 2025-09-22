using AssesmentUC.Service.Service.Interface;
using AssesmentUC.Infrastructure.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssesmentUC.Model.Entity;
using AssesmentUC.Service.DTO.Encuesta;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using ClosedXML.Excel;
using Azure.Core;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Wordprocessing;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using MimeKit;
using Google.Apis.Gmail.v1.Data;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace AssesmentUC.Service.Service.Impl
{
    public class EncuestaService : IEncuestaService
    {
        private readonly IEncuestaRepository _encuestaRepository;
        public EncuestaService(IEncuestaRepository encuestaRepository)
        {
            _encuestaRepository = encuestaRepository;
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

        public async Task<List<ListaTiposDTO>> ListarAsignaturasAsync(string seccion)
        {
            var asignaturas = await _encuestaRepository.ListarAsignaturasRepository(seccion);

            var dtoList = asignaturas
                .Select(e => new ListaTiposDTO
                {
                    AsignaturaId = e.NRC,
                    NombreTipo = e.NombreAsignatura
                })
                .ToList();

            return dtoList;
        }

        public async Task<List<ListaTiposDTO>> ListarTipoProgramaAsync()
        {
            var tiposPrograma = await _encuestaRepository.ListarTipoProgramaRepository();

            var dtoList = tiposPrograma
                .Select(e => new ListaTiposDTO
                {
                    ProgramaId = e.TipoProgramaId,
                    NombreTipo = e.TipoPrograma
                })
                .ToList();

            return dtoList;
        }

        public async Task<int> CrearAsignaturaEncuestaAsync(EncuestaAsignaturaCreateDTO dto, string usuario)
        {
            var encuesta = new Encuesta
            {
                NombreEncuesta = dto.NombreEncuesta,
                DescripcionEncuesta = dto.DescripcionEncuesta,
                SedeId = dto.SedeId,
                TipoEncuestaId = dto.TipoEncuestaId,
                TipoProgramaId = dto.TipoProgramaId,
                PeriodoId = dto.PeriodoId,
                SeccionId = dto.SeccionId,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                FechaCreacion = DateTime.Now,
                Activo = true,
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

            return await _encuestaRepository.CrearAsignaturaEncuestaRepository(encuesta);
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

        public async Task<EncuestaExportarPdfDTO> ObtenerEncuestaParaExportar(int encuestaId)
        {
            var encuesta = await _encuestaRepository.ListarPlantillaEncuestaIdRepository(encuestaId);

            if (encuesta == null)
                throw new Exception("Encuesta no encontrada");

            var dto = new EncuestaExportarPdfDTO
            {
                NombreEncuesta = encuesta.NombreEncuesta,
                DescripcionEncuesta = encuesta.DescripcionEncuesta,
                Periodo = encuesta.Periodo,
                Seccion = encuesta.Seccion,
                FechaInicio = encuesta.FechaInicio,
                FechaFin = encuesta.FechaFin,
                Bloques = encuesta.Bloques.Select(b => new BloqueExportPdfDTO
                {
                    TituloBloque = b.TituloBloque,
                    Orden = b.OrdenBloque,
                    Preguntas = b.Preguntas.Select(p => new PreguntaExportPdfDTO
                    {
                        TextoPregunta = p.TextoPregunta,
                        TipoPregunta = p.TipoPregunta,
                        Orden = p.OrdenPregunta
                    }).ToList()
                }).ToList()
            };

            return dto;
        }

        public async Task<byte[]> GenerarPdfEncuesta(int encuestaId)
        {
            var dto = await ObtenerEncuestaParaExportar(encuestaId);

            using var stream = new MemoryStream();
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Arial", 12, XFontStyle.Regular);

            int y = 40;
            gfx.DrawString($"Encuesta: {dto.NombreEncuesta}", font, XBrushes.Black, new XPoint(40, y));
            y += 25;
            gfx.DrawString($"Periodo: {dto.Periodo}  Sección: {dto.Seccion}", font, XBrushes.Black, new XPoint(40, y));
            y += 25;

            foreach (var bloque in dto.Bloques.OrderBy(b => b.Orden))
            {
                gfx.DrawString($"Bloque: {bloque.TituloBloque}", font, XBrushes.DarkBlue, new XPoint(40, y));
                y += 20;

                foreach (var pregunta in bloque.Preguntas.OrderBy(p => p.Orden))
                {
                    gfx.DrawString($"• {pregunta.TextoPregunta} ({pregunta.TipoPregunta})", font, XBrushes.Black, new XPoint(60, y));
                    y += 18;
                }

                y += 10;
            }

            document.Save(stream, false);
            return stream.ToArray();
        }

        public async Task<byte[]> GenerarExcelEncuesta(int encuestaId)
        {
            var dto = await ObtenerEncuestaParaExportar(encuestaId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Encuesta");

            int row = 1;

            worksheet.Cell(row++, 1).Value = $"Nombre de Encuesta: {dto.NombreEncuesta}";
            worksheet.Cell(row++, 1).Value = $"Periodo: {dto.Periodo ?? "-"}";
            worksheet.Cell(row++, 1).Value = $"Sección: {dto.Seccion ?? "-"}";
            row++;

            foreach (var bloque in dto.Bloques.OrderBy(b => b.Orden))
            {
                worksheet.Cell(row++, 1).Value = $"Bloque: {bloque.TituloBloque}";

                worksheet.Cell(row, 1).Value = "N°";
                worksheet.Cell(row, 2).Value = "Texto de la Pregunta";
                worksheet.Cell(row, 3).Value = "Tipo";
                row++;

                foreach (var pregunta in bloque.Preguntas.OrderBy(p => p.Orden))
                {
                    worksheet.Cell(row, 1).Value = pregunta.Orden;
                    worksheet.Cell(row, 2).Value = pregunta.TextoPregunta;
                    worksheet.Cell(row, 3).Value = pregunta.TipoPregunta;
                    row++;
                }

                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task EnviarCorreoEncuestaAsync(string accessToken, string userEmail, int encuestaId)
        {
            EnviarCorreoEncuesta dataCorreo = await _encuestaRepository.ValoresCorreoEncuestaRepository();

            var credential = GoogleCredential.FromAccessToken(accessToken);

            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = dataCorreo.NombreEncuesta,
            });

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userEmail, userEmail));
            message.To.Add(new MailboxAddress("", dataCorreo.ToEmail));
            message.Subject = dataCorreo.Subject;
            message.Body = new TextPart("plain") { Text = dataCorreo.Body };

            using var ms = new MemoryStream();
            message.WriteTo(ms);
            var raw = Convert.ToBase64String(ms.ToArray())
                .Replace("+", "-").Replace("/", "_").Replace("=", "");

            var gmailMessage = new Message { Raw = raw };

            // "me" = el usuario autenticado con el access_token
            var result = await service.Users.Messages.Send(gmailMessage, "me").ExecuteAsync();

            //if (result != null && !string.IsNullOrEmpty(result.Id))
            //{
            //    await _encuestaRepository.ActualizarEncuestaCompletadaRepository(encuestaId, userEmail);
            //}

        }

    }
}
